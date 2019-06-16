using MabiPacker.Library;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MabiPacker.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MabiEnvironment env;
        public MainWindow()
        {
            InitializeComponent();
            // Error Handler
            new UnhandledExceptionCatcher(Properties.Resources.ResourceManager, true, true);

            env = new MabiEnvironment();
            string appPath = Path.GetDirectoryName(Path.GetFullPath(Environment.GetCommandLineArgs()[0]));

            // Display Version infomation from assambly
            AppAssembly asm = new AppAssembly();
            TextBlock_ProductName.Text = asm.Title;
            TextBlock_ProductDescription.Text = asm.Description;
            TextBlock_ProductVersion.Text = string.Format("v.{0}", asm.Version);
            TextBlock_ProductCopyright.Text = asm.Copyright;

            // Insert default value
            TextBox_PackDistination.Text = appPath;
            TextBox_PackFileName.Text = env.MabinogiDir + "\\package\\custom-" + int.Parse(DateTime.Today.ToString("yyMMdd")) + ".pack";
            TextBox_Version.Value = env.LocalVersion;
            TextBox_UnpackDistination.Text = appPath;
            TextBox_UnpackFileName.Text = env.MabinogiDir + "\\package\\";
            TextBox_Version.Minimum = env.LocalVersion + 1;
            _cancelToken = new CancellationTokenSource();
        }

        private readonly CancellationTokenSource _cancelToken;

        private void Button_SetPackDistination_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog
            {
                Description = LocalizationProvider.GetLocalizedValue<string>("String_SetPackDistination")
            };
            ;
            folderBrowser.UseDescriptionForTitle = true;
            folderBrowser.SelectedPath = TextBox_PackDistination.Text;
            Nullable<bool> result = folderBrowser.ShowDialog();
            if (result == true)
            {
                TextBox_PackDistination.Text = folderBrowser.SelectedPath;
            }
        }

        private void Button_SetPackFileName_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = LocalizationProvider.GetLocalizedValue<string>("String_SetPackFileName"),
                Filter = LocalizationProvider.GetLocalizedValue<string>("String_PackFileDesciption") + "(*.pack)|*.pack",
                InitialDirectory = env.MabinogiDir + "\\package",
                FileName = TextBox_PackFileName.Text,
                FilterIndex = 1,
                DefaultExt = ".pack", // Default file extension
                RestoreDirectory = true
            };

            Nullable<bool> result = saveFileDialog.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                TextBox_PackFileName.Text = saveFileDialog.FileName;
            }
        }

        private async void Button_Pack_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(TextBox_PackFileName.Text))
            {
                File.Delete(TextBox_PackFileName.Text);
            }

            using (Packer packer = new Packer(TextBox_PackFileName.Text, TextBox_PackDistination.Text, (uint)TextBox_Version.Value, ComboBox_Level.SelectedIndex - 1))
            {
                await PackProcess(this, packer);
            }
        }

        private async Task<object> PackProcess(MetroWindow window, Packer packer)
        {
            ProgressDialogController controller = await window.ShowProgressAsync(
                LocalizationProvider.GetLocalizedValue<string>("Button_Pack"),
                LocalizationProvider.GetLocalizedValue<string>("String_Initializing")
            );
            controller.SetIndeterminate();
            controller.Maximum = packer.Count();
            controller.Minimum = 0;
            controller.SetCancelable(true);

            Progress<Entry> p = new Progress<Entry>((Entry entry) =>
            {
                controller.SetProgress(entry.Index);
                controller.SetTitle(LocalizationProvider.GetLocalizedValue<string>("String_Unpacking"));
                controller.SetMessage(
                    string.Format("{0} ({1}/{2})", entry.Name, entry.Index, packer.Count())
                );
                if (controller.IsCanceled)
                {
                    _cancelToken.Cancel();
                    return;
                }
            });
            await Task.Run(() => packer.Pack(p, _cancelToken.Token));
            await controller.CloseAsync();
            await window.ShowMessageAsync(LocalizationProvider.GetLocalizedValue<string>("Button_Pack"), LocalizationProvider.GetLocalizedValue<string>("String_Finish"));

            return null;
        }


        private void Button_SetUnpackFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = TextBox_UnpackFileName.Text,
                DefaultExt = ".pack", // Default file extension
                Filter = LocalizationProvider.GetLocalizedValue<string>("String_PackFileDesciption") + "(*.pack)|*.pack"
            };

            // Show open file dialog box
            Nullable<bool> result = openFileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                TextBox_UnpackFileName.Text = openFileDialog.FileName;
            }
        }

        private void Button_SetUnpackDistination_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog
            {
                Description = LocalizationProvider.GetLocalizedValue<string>("String_SetUnackDistination")
            };
            ;
            folderBrowser.UseDescriptionForTitle = true;
            folderBrowser.SelectedPath = TextBox_UnpackDistination.Text;
            Nullable<bool> result = folderBrowser.ShowDialog();
            if (result == true)
            {
                TextBox_UnpackDistination.Text = folderBrowser.SelectedPath;
            }
        }

        private async void Unpack_Click(object sender, RoutedEventArgs e)
        {
            using (Unpacker unpacker = new Unpacker(TextBox_UnpackFileName.Text, TextBox_UnpackDistination.Text))
            {
                await UnpackProcess(this, unpacker);
            }
        }

        private async Task<object> UnpackProcess(MetroWindow window, Unpacker unpacker)
        {
            ProgressDialogController controller = await window.ShowProgressAsync(
                LocalizationProvider.GetLocalizedValue<string>("Button_Unpack"),
                LocalizationProvider.GetLocalizedValue<string>("String_Initializing")
            );
            controller.SetIndeterminate();
            controller.Maximum = unpacker.Count();
            controller.Minimum = 0;
            controller.SetCancelable(true);

            Progress<Entry> p = new Progress<Entry>((Entry entry) =>
            {
                controller.SetProgress(entry.Index);
                controller.SetTitle(LocalizationProvider.GetLocalizedValue<string>("String_Unpacking"));
                controller.SetMessage(
                    string.Format("{0} ({1}/{2})", entry.Name, entry.Index, unpacker.Count())
                );
                if (controller.IsCanceled)
                {
                    _cancelToken.Cancel();
                    return;
                }
            });
            await Task.Run(() => unpacker.Unpack(p, _cancelToken.Token));
            await controller.CloseAsync();
            await window.ShowMessageAsync(
                LocalizationProvider.GetLocalizedValue<string>("Button_Unpack"),
                LocalizationProvider.GetLocalizedValue<string>("String_Finish")
            );

            return null;
        }

        private void TextBlock_ProductCopyright_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://logue.be/");
        }

        private void Button_Visit_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/logue/MabiPack");
        }
    }
}
