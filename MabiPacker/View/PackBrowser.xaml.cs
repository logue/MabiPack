using MabiPacker.Library;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.IO;
using System.Windows.Controls;

namespace MabiPacker.View
{

    /// <summary>
    /// Interaction logic for PackBrowser.xaml
    /// </summary>
    public partial class PackBrowser : MetroWindow
    {
        //private WavePlayer wave;
        //private IceResourceSet m_Pack;
        //private IceResource Res;
        public TreeViewItem root;
        private readonly Unpacker _unpacker;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file"></param>
        public PackBrowser(string file)
        {
            InitializeComponent();
            _unpacker = new Unpacker(file);
            root = new TreeViewItem
            {
                Header = "data"
            };

            TreeView_FileList.Items.Add(root);
            foreach (Entry entry in _unpacker.Entries())
            {
                addNode(entry);
            }
        }
        /// <summary>
        /// Destractor
        /// </summary>
        ~PackBrowser()
        {
            _unpacker.Dispose();
        }
        /// <summary>
        /// Create Treeview Node
        /// </summary>
        /// <param name="entry"></param>
        private void addNode(Entry entry)
        {
            TreeViewItem n = root;

            foreach (string val in entry.Name.Split('\\'))
            {
                bool isNew = true;

                foreach (TreeViewItem existingNode in n.Items)
                {
                    // Check Directory Hierarchy
                    foreach (string filePath in ((Entry)existingNode.Tag).Name.Split('\\'))
                    {
                        if (filePath == val)
                        {
                            n = existingNode;
                            isNew = false;
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
                    }

                    n.Items.Add(newNode);

                    n = newNode;
                }
            }
        }

        private void DisplayContentHandler(Entry entry)
        {
            Console.WriteLine(entry.Name);
            /*
            PackResource Res = m_Pack.GetFileByIndex (id);
            Status.Content = "Now Loading...";
            if (Res != null) {
                PicturePanel.Visibility = Visibility.Hidden;
                TextView.Visibility = Visibility.Hidden;
                SoundView.Visibility = Visibility.Hidden;
                String InternalName = Res.GetName ();
                string Ext = System.IO.Path.GetExtension (@InternalName);
                // loading file content.
                byte[] buffer = new byte[Res.GetSize ()];
                Res.GetData (buffer);
                Res.Close ();
                switch (Ext) {
                    case ".dds":
                    case ".jpg":
                    case ".gif":
                    case ".png":
                    case ".bmp":
                        string Info = "";
                        if (Ext == ".dds") {
                            DDSImage dds = new DDSImage (buffer);
                            Info = "DDS (Direct Draw Surfice)";
                            PictureView.Source = Utility.ToImageSource (dds.BitmapImage);
                            dds.Dispose ();
                        } else {
                            switch (Ext) {
                                case ".jpg":
                                    Info = "JPEG";
                                    break;
                                case ".gif":
                                    Info = "GIF";
                                    break;
                                case ".bmp":
                                    Info = "Bitmap";
                                    break;
                                case ".png":
                                    Info = "PNG (Portable Network Graphic)";
                                    break;
                            }
                            BitmapImage bitmapImage = new BitmapImage ();
                            MemoryStream ms = new MemoryStream (buffer);
                            bitmapImage.StreamSource = ms;
                            PictureView.Source = bitmapImage;
                            ms.Dispose ();
                        }
                        Status.Content = String.Format ("{0} Image file. ({1} x {2})", Info, PictureView.Width, PictureView.Height);
                        break;
                    case ".xml":
                    case ".html":
                    case ".txt":
                    case ".trn":
                        string text = Encoding.Unicode.GetString (buffer);
                        TextView.Text = text;
                        Status.Content = String.Format ("Ascii file.");
                        break;
                    case ".wav":
                    case ".mp3":
                        SoundView.Visibility = Visibility.Visible;
                        // http://msdn.microsoft.com/en-us/library/ms143770%28v=VS.100%29.aspx
                        this.wave = new Utility.WavePlayer (buffer);
                        this.wave.Play ();
                        Status.Content = "Sound file.";
                        break;
                    default:
                        Status.Content = "Unknown file.";
                        break;
                }
                */
        }

        /// <summary>
        /// Folder icon handler
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
            switch (Path.GetExtension(entry.Name))
            {
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
            sp.Children.Add(new TextBlock() { Text = Path.GetFileName(entry.Name) });
            return sp;
        }
    }
}
