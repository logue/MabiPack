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
        private readonly string MabinogiDir;
        public MainWindow()
        {
            InitializeComponent();
            // Error Handler
            //new UnhandledExceptionCatcher(Properties.Resources.ResourceManager, true, true);

            AppAssembly asm = new AppAssembly();
            TextBlock_ProductName.Text = asm.Title;
            TextBlock_ProductDescription.Text = asm.Description;
            TextBlock_ProductVersion.Text = string.Format(" v.{0}", asm.Version);
            TextBlock_ProductCopyright.Text = asm.Copyright;
            MabiEnvironment env = new MabiEnvironment();
            MabinogiDir = env.MabinogiDir;
            TextBox_PackFileName.Text = MabinogiDir + "\\Package\\custom-" + int.Parse(DateTime.Today.ToString("yyMMdd")) + ".pack";
            TextBox_Version.Value = env.LocalVersion;
            TextBox_UnpackDistination.Text = Path.GetDirectoryName(Path.GetFullPath(Environment.GetCommandLineArgs()[0]));
            TextBox_UnpackFileName.Text = MabinogiDir + "\\Package\\";
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
                InitialDirectory = MabinogiDir + "\\package",
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
            Packer packer = new Packer(TextBox_PackFileName.Text, TextBox_PackDistination.Text, (uint)TextBox_Version.Value, ComboBox_Level.SelectedIndex - 1);
            await PackProcess(this, packer);
        }

        private async Task<object> PackProcess(MetroWindow window, Packer packer)
        {
            ProgressDialogController controller = await window.ShowProgressAsync("Progress", "Pack");
            controller.Maximum = 100;
            controller.Minimum = 0;
            Progress<uint> p = new Progress<uint>((uint percentage) =>
            {
                controller.SetProgress(percentage);
            });
            await Task.Run(() => packer.Pack(p, _cancelToken.Token));
            await controller.CloseAsync();

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
            Unpacker unpacker = new Unpacker(TextBox_UnpackFileName.Text, TextBox_UnpackDistination.Text);
            await UnpackProcess(this, unpacker);
        }

        private async Task<object> UnpackProcess(MetroWindow window, Unpacker unpacker)
        {
            ProgressDialogController controller = await window.ShowProgressAsync("Progress", "Pack");
            controller.Maximum = 100;
            controller.Minimum = 0;
            Progress<uint> p = new Progress<uint>((uint percentage) =>
            {
                controller.SetProgress(percentage);
            });
            await Task.Run(() => unpacker.Unpack(p, _cancelToken.Token));
            await controller.CloseAsync();

            return null;
        }
    }
}
