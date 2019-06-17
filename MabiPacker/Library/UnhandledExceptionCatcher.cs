using MabiPacker.View;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Threading;
//#define USE_TASK_DIALOG

//using Microsoft.WindowsAPICodePack.Dialogs;
//public MainWindow()
//{
//	new MyLibrary.UnhandledExceptionCatcher(Properties.Resources.ResourceManager, true, true);
//	InitializeComponent();
//}

// リソースに設定するローカライズメッセージ
//	  ApplicationName アプリケーション名
//	  TarminateMessageMain 表示メッセージ
//		  問題が発生したため、このプログラムは停止しました。
//	  TarminateMessageContent 表示メッセージ
//		  プログラムを終了します。
//	  TarminateMessageContentOutput 表示メッセージ
//		  プログラムを終了します。{0}この問題の内部情報を表示しますか？

namespace MabiPacker.Library
{
    public class UnhandledExceptionCatcher
    {
        private const string applicationName = "MabiPacker";
        private readonly string tarminateMessageMain = LocalizationProvider.GetLocalizedValue<string>("String_TarminateMessageMain");
        private readonly string tarminateMessageContent = LocalizationProvider.GetLocalizedValue<string>("String_TarminateMessageContent");
        private readonly string tarminateMessageContentOutput = LocalizationProvider.GetLocalizedValue<string>("String_TarminateMessageContentOutput");
        private const string ApplicationNameString = "ApplicationName";
        private const string TarminateMessageMainString = "TarminateMessageMain";
        private const string TarminateMessageContentString = "TarminateMessageContent";
        private const string TarminateMessageContentOutputString = "TarminateMessageContentOutput";
        private readonly object lockobj = new object();
        private bool alreadyOccurred = false;
        private readonly List<Exception> exceptions = new List<Exception>();
        private int maxStockExceptions = 100;
        private readonly Assembly Assembly;
        private readonly AppDomain AppDomain;
        private readonly Application Application;
        private readonly ResourceManager ResourceManager;
        private readonly bool OutputException = false;
        private readonly bool OutputExceptionStackTrace = false;

        public UnhandledExceptionCatcher()
            : this(
                Assembly.GetExecutingAssembly(),
                AppDomain.CurrentDomain,
                Application.Current,
                null,
                false,
                false
            )
        { }

        public UnhandledExceptionCatcher(ResourceManager resourceManager, bool outputExeption, bool outputStackTrace)
            : this(
                Assembly.GetExecutingAssembly(),
                AppDomain.CurrentDomain,
                Application.Current,
                resourceManager,
                outputExeption,
                outputStackTrace
            )
        { }

        public UnhandledExceptionCatcher(
            Assembly assembly,
            AppDomain appDomain,
            Application application,
            ResourceManager resourceManager,
            bool outputExeption,
            bool outputStackTrace
            )
        {
            Assembly = assembly;
            AppDomain = appDomain;
            Application = application;
            ResourceManager = resourceManager;
            OutputException = outputExeption;
            OutputExceptionStackTrace = outputStackTrace;

            application.DispatcherUnhandledException += (s, ev) =>
            {
                ev.Handled = true;
                AddToExceptionList(ev.Exception); // and sleep if not first

                ShowReportAndShutdown();
            };

            appDomain.UnhandledException += (s, ev) =>
            {
                HaltUiThread();
                AddToExceptionList(ev.ExceptionObject as Exception); // and sleep if not first

                ShowReportAndShutdown();
            };
        }

        private void HaltUiThread()
        {
            Application.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    while (true)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }),
                DispatcherPriority.Send
            );
        }

        private void AddToExceptionList(Exception exception)
        {
            bool isNotFirst = false;
            lock (lockobj)
            {
                isNotFirst = alreadyOccurred;
                alreadyOccurred = true;
                if (maxStockExceptions-- > 0)
                {
                    exceptions.Add(exception);
                }
            }

            if (isNotFirst)
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private static string HeaderLine(string x)
        {
            return x + new string('-', 80 - x.Length);
        }

        private static string HeaderNumAndLine(string x, int i)
        {
            string h = string.Format("#{0}:{1}", i, x);
            return h + new string('-', 80 - h.Length);
        }

        private void ShowReportAndShutdown()
        {
            try
            {
                string appName = GetResourceString(ApplicationNameString);
                if (string.IsNullOrWhiteSpace(appName))
                {
                    AssemblyProductAttribute productAttribute = GetAssemblyAttribute<AssemblyProductAttribute>();
                    appName = (productAttribute != null) ? productAttribute.Product : string.Empty;
                }

                string messageMain = GetResourceString(TarminateMessageMainString) ?? tarminateMessageMain;
                messageMain = string.Format(messageMain, Environment.NewLine);
                string messageContent = OutputException ?
                    GetResourceString(TarminateMessageContentOutputString) ?? tarminateMessageContentOutput :
                    GetResourceString(TarminateMessageContentString) ?? tarminateMessageContent;
                messageContent = string.Format(messageContent, Environment.NewLine);

                bool result = ShowDialog(appName, messageMain, messageContent);

                if (result)
                {
                    StringBuilder builder = new StringBuilder();
                    try
                    {
                        builder.AppendLine(HeaderLine("Date"));
                        builder.AppendLine(DateTime.UtcNow.ToString("u"));

                        builder.AppendLine(HeaderLine("CommandLine"));
                        builder.AppendLine(Environment.CommandLine);
                        builder.AppendLine(HeaderLine("Version"));
                        builder.AppendLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                        builder.AppendLine(HeaderLine("CurrentUICulture"));
                        builder.AppendLine(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString());
                        builder.AppendLine(HeaderLine("OS"));
                        builder.AppendLine(
                            string.Format("{0} / Is64BitOperatingSystem:{1} / Is64BitProcess:{2}",
                            Environment.OSVersion.VersionString,
                            Environment.Is64BitOperatingSystem,
                            Environment.Is64BitProcess));

                        lock (lockobj)
                        {
                            int count = 0;
                            foreach (Exception ex in exceptions)
                            {
                                count++;
                                WriteExceptionDetail(builder, count, 0, ex);
                            }
                        }
                        builder.AppendLine();
                        builder.AppendLine();
                        builder.AppendLine();
                    }
                    catch { }
                    finally
                    {
                        System.Diagnostics.Debug.WriteLine(builder.ToString());
#if !DEBUG
                        try
                        {
                            string path = System.IO.Path.Combine(
                                System.IO.Path.GetTempPath(),
                                System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])
                                + ".errorlog.txt");
                            System.IO.File.AppendAllText(path, builder.ToString());
                        }
                        catch { }
#endif
                    }
                }
            }
            catch { }
            finally
            {
#if USE_TASK_DIALOG
			Environment.Exit(1);
#else

#endif
            }
        }

        private void WriteExceptionDetail(StringBuilder builder, int num, int innerCount, Exception exception)
        {
            string inner = (innerCount > 0) ? string.Format(":Inner{0}", innerCount) : string.Empty;
#if USE_TASK_DIALOG
			TaskDialog td = new TaskDialog();
			td.Icon = TaskDialogStandardIcon.Error;
			td.Caption = exception.Source ;
			td.InstructionText =exception.GetType().ToString();
			td.Text = exception.Message;
			td.StandardButtons = TaskDialogStandardButtons.Close;
#endif
            builder.AppendLine(HeaderNumAndLine("Type of exception" + inner, num));
            builder.AppendLine(exception.GetType().ToString());
            builder.AppendLine(HeaderNumAndLine("Exception.Message" + inner, num));
            builder.AppendLine(exception.Message);
            builder.AppendLine(HeaderNumAndLine("Exception.Source" + inner, num));
            builder.AppendLine(exception.Source);
            builder.AppendLine(HeaderNumAndLine("Exception.TargetSite" + inner, num));
            builder.AppendLine((exception.TargetSite != null) ? exception.TargetSite.ToString() : string.Empty);
            if (OutputExceptionStackTrace)
            {
                builder.AppendLine(HeaderNumAndLine("Exception.StackTrace" + inner, num));
                builder.AppendLine(exception.StackTrace);
            }
            builder.AppendLine(HeaderNumAndLine("Exception.Data" + inner, num));
            foreach (object key in exception.Data.Keys)
            {
                if (exception.Data[key] != null)
                {
                    builder.AppendLine(string.Format("{0} = {1}", key.ToString(), exception.Data[key].ToString()));
                }
            }
            if (exception.InnerException != null)
            {
                WriteExceptionDetail(builder, num, innerCount + 1, exception.InnerException);
            }
#if USE_TASK_DIALOG
			td.DetailsExpandedText = exception.StackTrace;
			td.Show();
#else
            ErrorReporter ew = new ErrorReporter(exception.Message, exception.StackTrace);
            ew.Show();
#endif
        }

        private T GetAssemblyAttribute<T>()
        {
            object[] attrs = Assembly.GetCustomAttributes(false);
            foreach (object attr in attrs)
            {
                if (attr.GetType().Equals(typeof(T)))
                {
                    return (T)attr;
                }
            }
            return default(T);
        }

        private string GetResourceString(string name)
        {
            try
            {
                if (ResourceManager != null)
                {
                    return ResourceManager.GetString(name);
                }
            }
            catch { }
            return null;
        }

        private bool ShowDialog(string title, string main, string content)
        {
            try
            {
#if USE_TASK_DIALOG
				TaskDialog td = new TaskDialog();
				td.Icon = TaskDialogStandardIcon.Error;
				td.Caption = title;
				td.InstructionText = main;
				td.Text = content;
				td.StandardButtons =
					TaskDialogStandardButtons.Yes |
					TaskDialogStandardButtons.Close;

				TaskDialogResult result = td.Show();

				return (result.ToString() == "Yes") ? true : false;
#else
                MessageBoxResult result = MessageBox.Show(string.Format("{1}{0}{0}{0}{2}{0}", Environment.NewLine, main, content), title,
                        OutputException ?
                            MessageBoxButton.YesNo :
                            MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return (result == MessageBoxResult.Yes);

#endif
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }
    }
}
