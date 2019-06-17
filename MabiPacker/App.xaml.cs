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

            // for i18n
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString());

            //new PackBrowser("C:\\Nexon\\Mabinogi\\package\\438_to_439.pack").Show();
            //return;

            if (File.Exists(Query) && Path.GetExtension(@Query) == ".pack")
            {
                // Pack Browser Mode (unmounted)
                new PackBrowser(Query).Show();
            }
            else if (Win32.AttachConsole(uint.MaxValue))
            {
                // Console Mode
                new Cui(e.Args);
                Win32.FreeConsole();
                Shutdown();
            }
            else
            {
                // Default GUI Mode
                new MainWindow().Show();
            }
        }
    }
}
