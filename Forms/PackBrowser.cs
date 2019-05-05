using Be.Windows.Forms;
using MabinogiResource;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MabiPacker
{
    public partial class PackBrowser : Form {
        // MabinogiResource.net.dll
		private PackResourceSet m_Pack;
		private PackResource Res;

        private TreeNode m_Root;
        private WavePlayer wave;

        private Worker w;
		private Dialogs d;
		private ProgressDialog pd;
		
		private string PackFile;
		private static GCHandle handle;

		public PackBrowser (string filename) {
			// Set PrgressDialog
			this.pd = new ProgressDialog (this.Handle);
			this.pd.Title = this.Name;
			this.pd.Caption = Properties.Resources.Str_Initialize;
			this.pd.Animation = 151;
			this.pd.ShowDialog ();
			InitializeComponent ();
			this.PackFile = filename;
			this.w = new Worker (false);
			this.d = new Dialogs ();
		}
		private void PackBrowser_Shown (object sender, EventArgs e) {
			m_Tree.Enabled = false;
			// Insert File tree
			try {
				m_Pack = PackResourceSet.CreateFromFile (PackFile);
				if (m_Pack != null) {
					Status.Text = Properties.Resources.Str_Initialize;
					this.Text = this.PackFile + " - MabiPacker";
					uint files = m_Pack.GetFileCount ();
					this.pd.Maximum = files;
					this.pd.Caption = Properties.Resources.Str_Loading;
					this.pd.Animation = 151;
					this.Update ();
					m_Tree.BeginUpdate ();
					m_Tree.Nodes.Clear ();
					m_Root = m_Tree.Nodes.Add ("data", "data", 0, 0);
					for (uint i = 0; i < files; ++i) {
						InsertFileNode (i);
						string info = String.Format (Properties.Resources.Str_LoadingMsg, i, files);
						this.pd.Value = i;
						this.pd.Message = info;
						Status.Text = info;
						if (this.pd.HasUserCancelled) {
							m_Pack.Dispose ();
							this.Close ();
							this.pd.CloseDialog ();
							return;
						}
						this.Update ();
					}
					m_Root.Expand ();
					m_Tree.EndUpdate ();
					this.pd.CloseDialog ();
					this.Update ();
					this.pd.Animation = 150;
					this.pd.ShowDialog (ProgressDialog.PROGDLG.MarqueeProgress, ProgressDialog.PROGDLG.NoCancel);
					this.pd.Caption = Properties.Resources.Str_Sorting;
					this.pd.Message = Properties.Resources.Str_SortingMsg;
					m_Tree.Sort ();
					m_Tree.Refresh ();
					Status.Text = Properties.Resources.Str_Ready;
					this.Update ();
				}
				m_Tree.Enabled = true;
			} catch (Exception ex) {
				d.Error (ex, this.Name);
			} finally {
				this.pd.CloseDialog ();
			}
		}
		private void PackBrowser_FormClosing (object sender, FormClosingEventArgs e) {
			m_Pack.Dispose ();
		}
		private void tbExport_Click (object sender, EventArgs e) {
			TreeNode node = m_Tree.SelectedNode; // get selected node tag
			if (node != null && node.Tag != null) {
				UnpackById ((uint) node.Tag);
			}
		}
		private void m_Tree_NodeMouseDoubleClick (object sender, TreeNodeMouseClickEventArgs e) {
			try {
				m_Tree.Enabled = false;
				PreviewById ((uint) e.Node.Tag);
			} catch (Exception ex) {
				Console.WriteLine (ex);
			} finally {
				m_Tree.Enabled = true;
			}
		}
		private void tbUnpack_Click (object sender, EventArgs e) {
			string outputDir = d.OutputDir ();
			if (outputDir != "") {
				d.Unpack (this.PackFile, outputDir);
			}
			return;
		}
		private void pPlay_Click (object sender, EventArgs e) {
			this.wave.Play ();
		}
        /***********************************************************************************************/
        /// <summary>
        /// Draw file tree (Left panel)
        /// </summary>
        /// <param name="id"></param>
        private void InsertFileNode (uint id) {
			PackResource pr = m_Pack.GetFileByIndex (id);
			if (pr != null) {
				string filePath = pr.GetName ();
				this.pd.Detail = filePath;
				string[] paths = filePath.Split ('\\');
				TreeNode node = m_Root;
				for (uint i = 0; i < paths.Length; ++i) {
					int index = node.Nodes.IndexOfKey (paths[i]);
					if (index < 0) {
						// Add( name , text)
						string Ext = Path.GetExtension (@paths[i]);
						if (Ext != "") {
							switch (Ext) {
								case ".txt":
									node = node.Nodes.Add (paths[i], paths[i], 2, 2);
									if (@paths[i] == "!Readme.txt") {
										PreviewById (id);
									}
									break;
								case ".xml":
								case ".trn":
									node = node.Nodes.Add (paths[i], paths[i], 3, 3);
									break;
								case ".jpg":
								case ".psd":
								case ".bmp":
								case ".dds":
								case ".tga":
								case ".gif":
								case ".png":
									node = node.Nodes.Add (paths[i], paths[i], 4, 4);
									break;
								case ".ttf":
								case ".ttc":
									node = node.Nodes.Add (paths[i], paths[i], 5, 5);
									break;
								case ".wav":
								case ".mp3":
									node = node.Nodes.Add (paths[i], paths[i], 6, 6);
									break;
								default:
									if (@paths[i] == "vf.dat") {
										node = node.Nodes.Add (paths[i], paths[i], 3, 3);
										PreviewById (id);
									} else {
										node = node.Nodes.Add (paths[i], paths[i], 1, 1);
									}
									break;
							}
						} else {
							node = node.Nodes.Add (paths[i], paths[i]);
						}
						node.Tag = id;
					} else {
						node = node.Nodes[index];
					}
				}
			}
		}
        /// <summary>
        /// Unpack selected file by Id
        /// </summary>
        /// <param name="id"></param>
		private void UnpackById (uint id) {
			Res = m_Pack.GetFileByIndex (id);
			if (Res != null) {
				w.UnpackFile (Res);
			}
		}
        /// <summary>
        /// Unpack selected file by file name (unused)
        /// </summary>
        /// <param name="name"></param>
		private void UnpackByName (string name) {
			Res = m_Pack.GetFileByName (name);
			if (Res != null) {
				w.UnpackFile (Res);
			}
		}
        /// <summary>
        /// Open selected file to display right panel.
        /// </summary>
        /// <param name="id"></param>
		private void PreviewById (uint id) {
			PackResource Res = m_Pack.GetFileByIndex (id);
			Status.Text = Properties.Resources.Str_LoadingPreview;
			this.Update ();
			if (Res != null) {
				PicturePanel.Hide ();
				hexBox.ResetText ();
				TextView.Hide ();
				pPlay.Hide ();
				String InternalName = Res.GetName ();
				string Ext = Path.GetExtension (@InternalName);
				// loading file content.
				byte[] buffer = new byte[Res.GetSize ()];
				Res.GetData (buffer);
				Res.Close ();

                MemoryStream ms = new MemoryStream(buffer);
                switch (Ext) {
					case ".dds":
					case ".tga":
					case ".jpg":
					case ".gif":
					case ".png":
					case ".bmp":
						if (handle.IsAllocated) {
							handle.Free ();
						}

						string Info = "";

						switch (Ext) {
							case ".dds":
							case ".tga":
								Info = "DDS (Direct Draw Surfice)";
								break;
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

						if (Ext == ".tga" || Ext == ".dds") {
							PictureView.Image = DDS2Bitmap (ms);
						} else {
							PictureView.Image = Image.FromStream (ms);
						}
						

						PictureView.Update ();
						Status.Text = String.Format ("{0} Image file. ({1} x {2})", Info, PictureView.Width, PictureView.Height);
						PictureView.SizeMode = PictureBoxSizeMode.AutoSize;
						PicturePanel.AutoScroll = true;
						PicturePanel.Update ();
						PicturePanel.Show ();
						break;
					case ".xml":
					case ".html":
					case ".txt":
					case ".trn":
						string text = Encoding.Unicode.GetString (buffer);
						TextView.Clear ();
						TextView.Text = text;
						TextView.Update ();
						TextView.Show ();
						Status.Text = String.Format ("Ascii file.");
						break;
					case ".wav":
					case ".mp3":
						pPlay.Show ();
						// http://msdn.microsoft.com/en-us/library/ms143770%28v=VS.100%29.aspx
						this.wave = new WavePlayer (buffer);
						this.wave.Play ();
						Status.Text = "Sound file.";
						break;
					default:
						if (InternalName == "vf.dat") {
							TextView.Clear ();
							TextView.Text = Encoding.ASCII.GetString (buffer);
							TextView.Update ();
							TextView.Show ();
							Status.Text = "Version infomation.";
						} else {
							DynamicByteProvider d = new DynamicByteProvider (buffer);
							hexBox.ByteProvider = d;
							Status.Text = "Unknown file.";
						}
						break;
				}
                ms.Dispose();
            }
			this.Update ();
		}
        /// <summary>
        /// Convert DDS data to Bitmap data.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
		private Bitmap DDS2Bitmap (MemoryStream ms) {
			var image = Pfim.Pfim.FromStream (ms);
			PixelFormat format;
			switch (image.Format) {
				case Pfim.ImageFormat.Rgb24:
					format = PixelFormat.Format24bppRgb;
					break;

				case Pfim.ImageFormat.Rgba32:
					format = PixelFormat.Format32bppArgb;
					break;

				case Pfim.ImageFormat.R5g5b5:
					format = PixelFormat.Format16bppRgb555;
					break;

				case Pfim.ImageFormat.R5g6b5:
					format = PixelFormat.Format16bppRgb565;
					break;

				case Pfim.ImageFormat.R5g5b5a1:
					format = PixelFormat.Format16bppArgb1555;
					break;

				case Pfim.ImageFormat.Rgb8:
					format = PixelFormat.Format8bppIndexed;
					break;

				default:
					return null;
			}
			handle = GCHandle.Alloc (image.Data, GCHandleType.Pinned);
			var ptr = Marshal.UnsafeAddrOfPinnedArrayElement (image.Data, 0);
			var bitmap = new Bitmap (image.Width, image.Height, image.Stride, format, ptr);
			// While frameworks like WPF and ImageSharp natively understand 8bit gray values.
			// WinForms can only work with an 8bit palette that we construct of gray values.
			if (format == PixelFormat.Format8bppIndexed) {
				var palette = bitmap.Palette;
				for (int i = 0; i < 256; i++) {
					palette.Entries[i] = Color.FromArgb ((byte) i, (byte) i, (byte) i);
				}
				bitmap.Palette = palette;
			}
			return bitmap;
		}
		/// <summary>
		/// Playing sound via winmm.dll
		/// </summary>
		/// <param name="buffer">Byte array containing wave data</param>
		// http://dobon.net/vb/dotnet/programing/playembeddedwave.html
		public class WavePlayer {
			public enum PlaySoundFlags : int {
				SND_SYNC = 0x0000,
				SND_ASYNC = 0x0001,
				SND_NODEFAULT = 0x0002,
				SND_MEMORY = 0x0004,
				SND_LOOP = 0x0008,
				SND_NOSTOP = 0x0010,
				SND_NOWAIT = 0x00002000,
				SND_ALIAS = 0x00010000,
				SND_ALIAS_ID = 0x00110000,
				SND_FILENAME = 0x00020000,
				SND_RESOURCE = 0x00040004,
				SND_PURGE = 0x0040,
				SND_APPLICATION = 0x0080
			}

			[DllImport ("winmm.dll")]
			private static extern bool PlaySound (
				IntPtr pszSound, IntPtr hmod, PlaySoundFlags fdwSound);
			private GCHandle gcHandle;
			private byte[] waveBuffer = null;
			public WavePlayer (byte[] buffer) {
				this.waveBuffer = buffer;
				this.gcHandle = GCHandle.Alloc (buffer, GCHandleType.Pinned);
			}
			public void Play () {
				// Play wave asyncrous
				PlaySound (this.gcHandle.AddrOfPinnedObject (), IntPtr.Zero,
					PlaySoundFlags.SND_MEMORY | PlaySoundFlags.SND_ASYNC);
			}
			public void Stop () {
				// Stop Wave
				PlaySound (IntPtr.Zero, IntPtr.Zero, PlaySoundFlags.SND_PURGE);
				// free
				this.gcHandle.Free ();
				this.waveBuffer = null;
			}
		}
		/// <summary>
		/// Textbox Shortcut key Enabler
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey (ref Message msg, Keys keyData) {
			ContainerControl containerControl;
			// Ctrl + A
			if (keyData == (Keys.A | Keys.Control)) {
				if (this.ActiveControl is TextBox) {
					((TextBox) this.ActiveControl).SelectAll ();
					return true;
				} else if (this.ActiveControl is ContainerControl) {
					containerControl = (ContainerControl) this.ActiveControl;
					while (containerControl.ActiveControl is ContainerControl) {
						containerControl = containerControl.ActiveControl as ContainerControl;
					}
					if (containerControl.ActiveControl is TextBox) {
						((TextBox) containerControl.ActiveControl).SelectAll ();
						return true;
					}
				}
			}
			// Ctrl + C
			else if (keyData == (Keys.C | Keys.Control)) {
				if (this.ActiveControl is TextBox) {
					((TextBox) this.ActiveControl).Copy ();
					return true;
				} else if (this.ActiveControl is ContainerControl) {
					containerControl = (ContainerControl) this.ActiveControl;
					while (containerControl.ActiveControl is ContainerControl) {
						containerControl = containerControl.ActiveControl as ContainerControl;
					}
					if (containerControl.ActiveControl is TextBox) {
						((TextBox) containerControl.ActiveControl).Copy ();
						return true;
					}
				}
			}
			// Ctrl + X
			else if (keyData == (Keys.X | Keys.Control)) {
				if (this.ActiveControl is TextBox) {
					((TextBox) this.ActiveControl).Cut ();
					return true;
				} else if (this.ActiveControl is ContainerControl) {
					containerControl = (ContainerControl) this.ActiveControl;
					while (containerControl.ActiveControl is ContainerControl) {
						containerControl = containerControl.ActiveControl as ContainerControl;
					}
					if (containerControl.ActiveControl is TextBox) {
						((TextBox) containerControl.ActiveControl).Cut ();
						return true;
					}
				}
			}
			// Ctrl + V
			else if (keyData == (Keys.V | Keys.Control)) {
				if (this.ActiveControl is TextBox) {
					((TextBox) this.ActiveControl).Paste ();
					return true;
				} else if (this.ActiveControl is ContainerControl) {
					containerControl = (ContainerControl) this.ActiveControl;
					while (containerControl.ActiveControl is ContainerControl) {
						containerControl = containerControl.ActiveControl as ContainerControl;
					}
					if (containerControl.ActiveControl is TextBox) {
						((TextBox) containerControl.ActiveControl).Paste ();
						return true;
					}
				}
			}
			return base.ProcessCmdKey (ref msg, keyData);
		}
	}
}