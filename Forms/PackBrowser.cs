using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using MabinogiResource;
using Tao.DevIl;

namespace MabiPacker
{
    public partial class PackBrowser : Form
    {
		private TreeNode m_Root;
		private PackResourceSet m_Pack;
		private PackResource Res;
		private Worker w;
		private Dialogs d;
		private ProgressDialog pd;
		private WavePlayer wave;
		private string PackFile;
		private bool isVista;
		bool bDrag = false;
		Point posStart;

		public PackBrowser(string filename)
		{
			// Set PrgressDialog
			this.pd = new ProgressDialog(this.Handle);
			this.pd.Title = this.Name;
			this.pd.Caption = Properties.Resources.Str_Initialize;
			this.pd.Animation = 151;
			this.pd.ShowDialog();
			
			InitializeComponent();
			this.PackFile = filename;
			this.w = new Worker();
			this.d = new Dialogs();
			this.isVista = (Environment.OSVersion.Version.Major >= 6) ? true : false;

		}
		private void PackBrowser_Shown(object sender, EventArgs e)
		{
			
			m_Tree.Enabled = false;
			// Insert File tree
			try{
				m_Pack = PackResourceSet.CreateFromFile(PackFile);
				if (m_Pack != null)
				{
					Status.Text = Properties.Resources.Str_Initialize;
					this.Text = this.PackFile + " - MabiPacker";
					uint files = m_Pack.GetFileCount();
					this.pd.Maximum = files;
					this.pd.Caption = Properties.Resources.Str_Loading;
					this.pd.Animation = 151;
					this.Update();
					m_Tree.BeginUpdate();
					m_Tree.Nodes.Clear();
					m_Root = m_Tree.Nodes.Add("data","data",0,0);
					for (uint i = 0; i < files; ++i)
					{
						InsertFileNode(i);
						string info = String.Format(Properties.Resources.Str_LoadingMsg,  i, files);
						this.pd.Value = i;

						this.pd.Message = info;
						Status.Text = info;
						if (this.pd.HasUserCancelled)
						{
							m_Pack.Dispose();
							this.Close();
							this.pd.CloseDialog();
							return;
						}
						this.Update();
					}
					m_Root.Expand();
					m_Tree.EndUpdate();
					this.pd.CloseDialog();
					this.Update();
					this.pd.Animation = 150;
					this.pd.ShowDialog(ProgressDialog.PROGDLG.MarqueeProgress, ProgressDialog.PROGDLG.NoCancel);
					this.pd.Caption = Properties.Resources.Str_Sorting;
					this.pd.Message = Properties.Resources.Str_SortingMsg;
					m_Tree.Sort();
					m_Tree.Refresh();

					Status.Text = Properties.Resources.Str_Ready;
					this.Update();
				}
				m_Tree.Enabled = true;
			}catch(Exception ex){
				d.Error(ex,this.Name);
			}finally{
				this.pd.CloseDialog();
			}
		}
		private void PackBrowser_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_Pack.Dispose();
		}

		private void tbExport_Click(object sender, EventArgs e)
		{
			TreeNode node = m_Tree.SelectedNode;	// get selected node tag
			if (node != null && node.Tag != null){
				UnpackById((uint)node.Tag);
			}
		}

		private void m_Tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			try{
				m_Tree.Enabled = false;
				PreviewById((uint)e.Node.Tag);
				
			}catch(Exception){

			}finally{
				m_Tree.Enabled = true;
			}
		}
		private void tbUnpack_Click(object sender, EventArgs e)
		{
			
		}
		private void PictureView_MouseDown(object sender, MouseEventArgs e)
		{
			PictureView.DoDragDrop(PictureView.Image, DragDropEffects.All);
			// ドラッグ開始
			bDrag = true;
			posStart = e.Location;
		}
		private void PictureView_MouseUp(object sender, MouseEventArgs e)
		{
			// ドラッグ終了
			bDrag = false;
		}

		private void PictureView_MouseMove(object sender, MouseEventArgs e)
		{
			// ドラッグ中ならスクロール
			if (bDrag)
			{
				Point pos = new Point(
					e.Location.X - posStart.X,
					e.Location.Y - posStart.Y);

				PicturePanel.AutoScrollPosition = new Point(
					-PicturePanel.AutoScrollPosition.X - pos.X,
					-PicturePanel.AutoScrollPosition.Y - pos.Y);
			}
		}
		private void pPlay_Click(object sender, EventArgs e)
		{
			this.wave.Play();
		}
		/***********************************************************************************************/
		private void InsertFileNode(uint id)
		{
			PackResource pr = m_Pack.GetFileByIndex(id);

			if (pr != null)
			{
				string filePath = pr.GetName();
				this.pd.Detail = filePath;
				string[] paths = filePath.Split('\\');
				TreeNode node = m_Root;
				for (uint i = 0; i < paths.Length; ++i)
				{
					int index = node.Nodes.IndexOfKey(paths[i]);
					if (index < 0)
					{
						// Add( name , text)
						string Ext = System.IO.Path.GetExtension(@paths[i]);
						if (Ext != ""){
							switch (Ext) {
								default:
								if (@paths[i] == "vf.dat")
									{
										node = node.Nodes.Add(paths[i], paths[i], 3, 3);
										PreviewById(id);
									}else{
										node = node.Nodes.Add(paths[i], paths[i], 1, 1);
									}
									break;
								case ".txt" :
									node = node.Nodes.Add(paths[i], paths[i], 2, 2);
									if (@paths[i] == "!Readme.txt"){
										PreviewById(id);
									}
									break;
								case ".xml" :
								case ".trn" :
									node = node.Nodes.Add(paths[i], paths[i], 3, 3);
									break;
								case ".jpg" :
								case ".psd" :
								case ".bmp" :
								case ".dds" :
								case ".gif":
								case ".png":
									node = node.Nodes.Add(paths[i], paths[i], 4, 4);
									break;
								case ".ttf" :
								case ".ttc":
									node = node.Nodes.Add(paths[i], paths[i], 5, 5);
									break;
								case ".wav":
								case ".mp3":
									node = node.Nodes.Add(paths[i], paths[i], 6, 6);
									break;
								
							}
						}else{
							node = node.Nodes.Add(paths[i], paths[i]);
						}
						node.Tag = id;
					}
					else
					{
						node = node.Nodes[index];
					}
				}
			}
		}
		private void UnpackById(uint id){
			Res = m_Pack.GetFileByIndex(id);
			if (Res != null)
			{
				w.UnpackFile(Res);
			}
		}
		private void UnpackByName(string name){
			Res = m_Pack.GetFileByName(name);
			if (Res != null)
			{
				w.UnpackFile(Res);
			}
		}
		private void PreviewById(uint id){			
			PackResource Res = m_Pack.GetFileByIndex(id);
			Status.Text = Properties.Resources.Str_LoadingPreview;
			this.Update();
			if (Res != null)
			{
				PicturePanel.Hide();
				
				hexBox.ResetText();
				TextView.Hide();
				pPlay.Hide();

				String InternalName = Res.GetName();
				string Ext = System.IO.Path.GetExtension(@InternalName);

				// loading file content.
				byte[] buffer = new byte[Res.GetSize()];
				Res.GetData(buffer);
				Res.Close();

				switch (Ext)
				{
					case ".dds":
					case ".jpg":
					case ".gif":
					case ".png":
					case ".bmp":
						string Info = "";
						if (Ext == ".dds"){
							Bitmap bmp = DDSDataToBMP(buffer);
							Info = "DDS (Direct Draw Surfice)";
							PictureView.Image = bmp;
						}else{
							switch (Ext){
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
							var ms = new MemoryStream(buffer);
							PictureView.Image = Image.FromStream(ms);
							ms.Dispose();
						}
						
						PictureView.Update();
						Status.Text = String.Format("{0} Image file. ({1} x {2})", Info, PictureView.Width, PictureView.Height);
						PictureView.SizeMode = PictureBoxSizeMode.AutoSize;
						PicturePanel.AutoScroll = true;
						PicturePanel.Update();
						PicturePanel.Show();
					break;
					case ".xml":
					case ".html":
					case ".txt":
					case ".trn":
						string text = Encoding.Unicode.GetString(buffer);
						TextView.Clear();
						TextView.Text = text;
						TextView.Update();
						TextView.Show();
						Status.Text = String.Format("Ascii file.");
						break;
					case ".wav":
					case ".mp3":
						pPlay.Show();
						// http://msdn.microsoft.com/en-us/library/ms143770%28v=VS.100%29.aspx 
						this.wave = new WavePlayer(buffer);
						this.wave.Play();
						Status.Text = "Sound file.";
					break;
					default:
						if (InternalName == "vf.dat"){
							TextView.Clear();
							TextView.Text = Encoding.ASCII.GetString(buffer);
							TextView.Update();
							TextView.Show();
							Status.Text = "Version infomation.";
						}else{
							DynamicByteProvider d = new DynamicByteProvider(buffer);
							hexBox.ByteProvider = d;
							Status.Text = "Unknown file.";
						}
					break;
				}
				
			}
			this.Update();
		}
		/// <summary>
		/// Converts an in-memory image in DDS format to a System.Drawing.Bitmap
		/// object for easy display in Windows forms.
		/// </summary>
		/// <param name="DDSData">Byte array containing DDS image data</param>
		/// <returns>A Bitmap object that can be displayed</returns>
		public static Bitmap DDSDataToBMP(byte[] DDSData)
		{
			// Create a DevIL image "name" (which is actually a number)
			int img_name;
			Il.ilGenImages(1, out img_name);
			Il.ilBindImage(img_name);

			// Load the DDS file into the bound DevIL image
			Il.ilLoadL(Il.IL_DDS, DDSData, DDSData.Length);

			// Set a few size variables that will simplify later code

			int ImgWidth = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
			int ImgHeight = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
			Rectangle rect = new Rectangle(0, 0, ImgWidth, ImgHeight);

			// Convert the DevIL image to a pixel byte array to copy into Bitmap
			Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE);

			// Create a Bitmap to copy the image into, and prepare it to get data
			Bitmap bmp = new Bitmap(ImgWidth, ImgHeight);
			BitmapData bmd =
			  bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

			// Copy the pixel byte array from the DevIL image to the Bitmap
			Il.ilCopyPixels(0, 0, 0,
			  Il.ilGetInteger(Il.IL_IMAGE_WIDTH),
			  Il.ilGetInteger(Il.IL_IMAGE_HEIGHT),
			  1, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE,
			  bmd.Scan0);

			// Clean up and return Bitmap
			Il.ilDeleteImages(1, ref img_name);
			bmp.UnlockBits(bmd);
			return bmp;
		}
		/// <summary>
		/// Playing sound via winmm.dll
		/// </summary>
		/// <param name="buffer">Byte array containing wave data</param>
		// http://dobon.net/vb/dotnet/programing/playembeddedwave.html
		public class WavePlayer
		{
			public enum PlaySoundFlags : int
			{
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
			[System.Runtime.InteropServices.DllImport("winmm.dll")]
			private static extern bool PlaySound(
				IntPtr pszSound, IntPtr hmod, PlaySoundFlags fdwSound);

			private System.Runtime.InteropServices.GCHandle gcHandle;
			private byte[] waveBuffer = null;

			public WavePlayer(byte[] buffer)
			{
				this.waveBuffer = buffer;
				this.gcHandle = System.Runtime.InteropServices.GCHandle.Alloc(
					buffer, System.Runtime.InteropServices.GCHandleType.Pinned);
			}
			public void Play(){
				// Play wave asyncrous
				PlaySound(this.gcHandle.AddrOfPinnedObject(), IntPtr.Zero,
					PlaySoundFlags.SND_MEMORY | PlaySoundFlags.SND_ASYNC);
			}

			public void Stop(){
				// Stop Wave
				PlaySound(IntPtr.Zero, IntPtr.Zero, PlaySoundFlags.SND_PURGE);

				// free
				this.gcHandle.Free();
				this.waveBuffer = null;
			}
		}
		/// <summary>
		/// Textbox Shortcut key Enabler
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			ContainerControl containerControl;
			// Ctrl + A
			if (keyData == (Keys.A | Keys.Control))
			{
				if (this.ActiveControl is TextBox)
				{
					((TextBox)this.ActiveControl).SelectAll();
					return true;
				}
				else if (this.ActiveControl is ContainerControl)
				{
					containerControl = (ContainerControl)this.ActiveControl;
					while (containerControl.ActiveControl is ContainerControl)
					{
						containerControl = containerControl.ActiveControl as ContainerControl;
					}
					if (containerControl.ActiveControl is TextBox)
					{
						((TextBox)containerControl.ActiveControl).SelectAll();
						return true;
					}
				}
			}
			// Ctrl + C
			else if (keyData == (Keys.C | Keys.Control))
			{
				if (this.ActiveControl is TextBox)
				{
					((TextBox)this.ActiveControl).Copy();
					return true;
				}
				else if (this.ActiveControl is ContainerControl)
				{
					containerControl = (ContainerControl)this.ActiveControl;
					while (containerControl.ActiveControl is ContainerControl)
					{
						containerControl = containerControl.ActiveControl as ContainerControl;
					}
					if (containerControl.ActiveControl is TextBox)
					{
						((TextBox)containerControl.ActiveControl).Copy();
						return true;
					}
				}
			}
			// Ctrl + X
			else if (keyData == (Keys.X | Keys.Control))
			{
				if (this.ActiveControl is TextBox)
				{
					((TextBox)this.ActiveControl).Cut();
					return true;
				}
				else if (this.ActiveControl is ContainerControl)
				{
					containerControl = (ContainerControl)this.ActiveControl;
					while (containerControl.ActiveControl is ContainerControl)
					{
						containerControl = containerControl.ActiveControl as ContainerControl;
					}
					if (containerControl.ActiveControl is TextBox)
					{
						((TextBox)containerControl.ActiveControl).Cut();
						return true;
					}
				}
			}
			// Ctrl + V
			else if (keyData == (Keys.V | Keys.Control))
			{
				if (this.ActiveControl is TextBox)
				{
					((TextBox)this.ActiveControl).Paste();
					return true;
				}
				else if (this.ActiveControl is ContainerControl)
				{
					containerControl = (ContainerControl)this.ActiveControl;
					while (containerControl.ActiveControl is ContainerControl)
					{
						containerControl = containerControl.ActiveControl as ContainerControl;
					}
					if (containerControl.ActiveControl is TextBox)
					{
						((TextBox)containerControl.ActiveControl).Paste();
						return true;
					}
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
