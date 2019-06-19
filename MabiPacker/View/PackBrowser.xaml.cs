using MabiPacker.Library;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Pfim;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MabiPacker.View
{

    /// <summary>
    /// Interaction logic for PackBrowser.xaml
    /// </summary>
    public partial class PackBrowser : MetroWindow
    {
        public TreeViewItem root;
        private readonly Unpacker _unpacker;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file"></param>
        public PackBrowser(string file)
        {
            _unpacker = new Unpacker(file);
            InitializeComponent();
            Loaded += PackBrowser_Loaded;
        }

        public async void PackBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("PackBrowser_Loaded");
            ProgressDialogController controller = await this.ShowProgressAsync(
                LocalizationProvider.GetLocalizedValue<string>("Button_Unpack"),
                LocalizationProvider.GetLocalizedValue<string>("String_Initializing")
            );
            controller.SetIndeterminate();
            controller.Maximum = _unpacker.Count();
            controller.Minimum = 0;

            root = new TreeViewItem
            {
                Header = "data"
            };
            TreeView_FileList.Items.Add(root);
            int i = 0;
            foreach (Entry entry in _unpacker.Entries())
            {
                TreeViewItem n = root;
                controller.SetProgress(i);

                foreach (string val in entry.File.Split('\\'))
                {
                    bool isNew = true;

                    foreach (TreeViewItem existingNode in n.Items)
                    {
                        object tag = existingNode.Tag;
                        // Check Directory Hierarchy
                        if (tag != null)
                        {
                            foreach (string filePath in ((Entry)tag).File.Split('\\'))
                            {
                                if (filePath == val)
                                {
                                    n = existingNode;
                                    isNew = false;
                                }
                            }
                        }
                    }

                    if (isNew)
                    {
                        TreeViewItem newNode = null;
                        if (Path.GetExtension(val) == "")
                        {
                            // is Directory
                            newNode = new TreeViewItem
                            {
                                Header = val,
                                Tag = entry
                            };
                        }
                        else
                        {
                            // is File
                            newNode = new TreeViewItem
                            {
                                Header = CreateHeader(entry),
                                Tag = entry
                            };
                            // Attach Event
                            newNode.MouseDoubleClick += ItemDoubleClickedHandler;
                        }

                        n.Items.Add(newNode);

                        n = newNode;
                    }
                }
            }

            await controller.CloseAsync();
            ProgressRing.Visibility = Visibility.Collapsed;
            return;
        }
        /// <summary>
        /// Destractor
        /// </summary>
        ~PackBrowser()
        {
            _unpacker.Dispose();
        }

        private enum Parser
        {
            DdsImage,
            Image,
            Text,
            Binary,
            Audio,
            Xml
        }

        private void ItemDoubleClickedHandler(object sender, RoutedEventArgs e)
        {
            Entry entry = (Entry)((TreeViewItem)e.Source).Tag;
            byte[] buffer = _unpacker.GetContent(entry.Index);

            TextViewer.Visibility = Visibility.Collapsed;
            PicturePanel.Visibility = Visibility.Collapsed;
            DocViewer.Visibility = Visibility.Collapsed;
            TextViewer.Visibility = Visibility.Collapsed;
            MediaViewer.Visibility = Visibility.Collapsed;
            HexViewer.Visibility = Visibility.Collapsed;

            ProgressRing.Visibility = Visibility.Visible;
            string Info;
            Enum parser;
            switch (entry.Extension)
            {
                case ".dds":
                    Info = "DDS (Direct Draw Surfice) image";
                    parser = Parser.DdsImage;
                    break;
                case ".tga":
                    Info = "DDS (Direct Draw Surfice) image";
                    parser = Parser.DdsImage;
                    break;
                case ".jpg":
                    Info = "JPEG image";
                    parser = Parser.Image;
                    break;
                case ".gif":
                    Info = "GIF image";
                    parser = Parser.Image;
                    break;
                case ".bmp":
                    Info = "Bitmap image";
                    parser = Parser.Image;
                    break;
                case ".png":
                    Info = "PNG (Portable Network Graphic) image";
                    parser = Parser.Image;
                    break;
                case ".xml":
                    Info = "XML (eXtensible Markup Language) image";
                    parser = Parser.Xml;
                    break;
                case ".html":
                    Info = "HTML (HyperText Markup Language) file";
                    parser = Parser.Xml;
                    break;
                case ".txt":
                    Info = "Plain text file";
                    parser = Parser.Text;
                    break;
                case ".trn":
                    Info = "Text file.";
                    parser = Parser.Text;
                    break;
                case ".wav":
                    Info = "Wave audio file.";
                    parser = Parser.Audio;
                    break;
                case ".mp3":
                    Info = "MP3 audio file.";
                    parser = Parser.Audio;
                    break;
                default:
                    if (entry.Name == "vf.dat")
                    {
                        Info = "Version infomation file.";
                        parser = Parser.Text;
                    }
                    else
                    {
                        Info = "Unknown file.";
                        parser = Parser.Binary;
                    }
                    break;
            }

            switch (parser)
            {
                case Parser.DdsImage:
                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        ImageViewer.Source = DdsStream2BitmapSource(ms);
                        StatusBarItem_Status.Content = string.Format("{0} ({1} x {2})", Info, ImageViewer.Width, ImageViewer.Height);
                        ProgressRing.Visibility = Visibility.Collapsed;
                        PicturePanel.Visibility = Visibility.Visible;
                    }
                    break;
                case Parser.Image:
                    using (MemoryStream ms2 = new MemoryStream(buffer))
                    {
                        BitmapImage imageSource = new BitmapImage();
                        imageSource.BeginInit();
                        imageSource.StreamSource = ms2;
                        imageSource.EndInit();

                        // Assign the Source property of your image
                        ImageViewer.Source = imageSource;
                    }

                    ProgressRing.Visibility = Visibility.Collapsed;
                    PicturePanel.Visibility = Visibility.Visible;
                    break;
                case Parser.Text:
                    TextViewer.Text = Encoding.Unicode.GetString(buffer);
                    ProgressRing.Visibility = Visibility.Collapsed;
                    TextViewer.Visibility = Visibility.Visible;
                    StatusBarItem_Status.Content = Info;
                    break;
                default:
                    StatusBarItem_Status.Content = Info;
                    ProgressRing.Visibility = Visibility.Collapsed;
                    HexViewer.Stream = new MemoryStream(buffer);
                    HexViewer.Visibility = Visibility.Visible;
                    break;

            };

            if (ImageViewer.IsVisible)
            {
                StatusBarItem_Status.Content = string.Format("{0} ({1} x {2})", Info, ImageViewer.Width, ImageViewer.Height);
            }
            else
            {
                StatusBarItem_Status.Content = Info;
            }

            //ms.Close();
            //ms.Dispose();
        }

        /// <summary>
        /// Convert DDS data to Bitmap data.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
		private BitmapSource DdsStream2BitmapSource(MemoryStream ms)
        {
            IImage image = Pfim.Pfim.FromStream(ms);
            PixelFormat format;
            switch (image.Format)
            {
                case ImageFormat.Rgb24:
                    format = PixelFormats.Bgr24;
                    break;

                case ImageFormat.Rgba32:
                    format = PixelFormats.Bgr32;
                    break;

                case ImageFormat.Rgb8:
                    format = PixelFormats.Gray8;
                    break;

                case ImageFormat.R5g5b5a1:
                case ImageFormat.R5g5b5:
                    format = PixelFormats.Bgr555;
                    break;

                case ImageFormat.R5g6b5:
                    format = PixelFormats.Bgr565;
                    break;

                default:
                    throw new Exception($"Unable to convert {image.Format} to WPF PixelFormat");
            }
            // Create a WPF ImageSource and then set an Image to our variable.
            // Make sure you notify property changes as appropriate ;)
            return BitmapSource.Create(image.Width, image.Height,
                96.0, 96.0, format, null, image.Data, image.Stride);
        }

        /// <summary>
        /// Folder icon handler (unused)
        /// </summary>
        private void IconChange()
        {
            StackPanel sp = (StackPanel)root.Header;
            PackIconFontAwesome Icon = (PackIconFontAwesome)sp.Children[0];
            if (root.IsExpanded)
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.FolderOpenRegular };
            }
            else
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.FolderRegular };
            }
            // TODO
        }
        /// <summary>
        /// Add item icon
        /// </summary>
        /// <returns></returns>
        private StackPanel CreateHeader(Entry entry)
        {
            StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
            PackIconFontAwesomeKind Icon;
            switch (entry.Extension)
            {
                case "":
                    Icon = PackIconFontAwesomeKind.FolderRegular;
                    break;
                case ".txt":
                    Icon = PackIconFontAwesomeKind.FileAltRegular;
                    break;
                case ".xml":
                case ".trn":
                    Icon = PackIconFontAwesomeKind.FileCodeRegular;
                    break;
                case ".jpg":
                case ".psd":
                case ".bmp":
                case ".dds":
                case ".gif":
                case ".png":
                    Icon = PackIconFontAwesomeKind.FileImageRegular;
                    break;
                case ".ttf":
                case ".ttc":
                    Icon = PackIconFontAwesomeKind.FontSolid;
                    break;
                case ".wav":
                case ".mp3":
                    Icon = PackIconFontAwesomeKind.FileAudioRegular;
                    break;
                default:
                    Icon = PackIconFontAwesomeKind.FileRegular;
                    break;
            }
            sp.Children.Add(new PackIconFontAwesome()
            {
                Kind = Icon,
            });
            sp.Children.Add(new TextBlock() { Text = entry.Name });
            return sp;
        }
    }
}
