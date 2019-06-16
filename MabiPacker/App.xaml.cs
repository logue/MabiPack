using MabiPacker.View;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using WPFLocalizeExtension.Engine;

namespace MabiPacker
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private class Win32
        {
            [DllImport("kernel32.dll")]
            public static extern bool AttachConsole(uint dwProcessId);
            [DllImport("kernel32.dll")]
            public static extern bool FreeConsole();
        }
        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string Query = string.Join(" ", e.Args);
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString());

            if (e.Args.Length == 0)
            {
                // Launch GUI and pass arguments in case you want to use them.
                new MainWindow().Show();
            }
            else if (Win32.AttachConsole(uint.MaxValue))
            {
                new Cui(e.Args);
                Win32.FreeConsole();
                Shutdown();
            }
            else if (File.Exists(Query) && Path.GetExtension(@Query) == ".pack")
            {
                //new PackBrowser(Query);
            }
            else
            {
                Shutdown();
            }
        }
    }
}
