using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MabinogiResource;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using Tao.DevIl;

namespace MabiPacker
{
    public partial class PackBrowser : Form
    {
		private TreeNode m_Root;
		private PackResourceSet m_Pack;
		private string PackFile;

		public PackBrowser(string filename)
		{
			InitializeComponent();
			this.PackFile = filename;
            this.Text = "Package File Browser - MabiPacker";
		}
		private void PackBrowser_Shown(object sender, EventArgs e)
		{
			// Insert File tree
			m_Pack = PackResourceSet.CreateFromFile(PackFile);
			if (m_Pack != null)
			{
				Status.Text = Properties.Resources.Str_Initialize;
				uint files = m_Pack.GetFileCount();
				Progress.Maximum = (int)files;
				this.Update();
				m_Tree.BeginUpdate();
				m_Tree.Nodes.Clear();
				m_Root = m_Tree.Nodes.Add("data","data",0,0);
				for (uint i = 0; i < files; ++i)
				{
					InsertFileNode(i);
					Progress.Value = (int)i;
					Status.Text = String.Format("Loading Package... ({0} of {1})", i, files);
					this.Update();
				}
				m_Root.Expand();
				m_Tree.EndUpdate();
				Progress.Visible = false;
				Status.Text = "Ready";
                this.Text = this.PackFile + " - MabiPacker";
				this.Update();
			}
		}
		private void PackBrowser_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_Pack.Dispose();
		}

		private void tbExport_Click(object sender, EventArgs e)
		{
			TreeNode node = m_Tree.SelectedNode;	// get selected node tag
			if (node != null){
				UnpackById((uint)node.Tag);
			}
		}

		private void m_Tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Tag != null) PreviewById((uint)e.Node.Tag);
		}
		/***********************************************************************************************/
		private void InsertFileNode(uint id)
		{
			PackResource pr = m_Pack.GetFileByIndex(id);

			if (pr != null)
			{
				string filePath = pr.GetName();
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
									node = node.Nodes.Add(paths[i], paths[i], 1, 1);
									break;
								case ".txt" :
									node = node.Nodes.Add(paths[i], paths[i], 2, 2);
									break;
								case ".xml" :
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
			PackResource Res = m_Pack.GetFileByIndex(id);
			if (Res != null)
			{
				String InternalName = Res.GetName();
				SaveFileDialog dSaveAs = new SaveFileDialog();
				dSaveAs.FileName = System.IO.Path.GetFileName(InternalName);

				if (dSaveAs.ShowDialog() == DialogResult.OK)
				{
					// loading file content.
					byte[] buffer = new byte[Res.GetSize()];
					Res.GetData(buffer);
					Res.Close();

					// Delete old
					if (File.Exists(dSaveAs.FileName))
					{
						System.IO.File.Delete(dSaveAs.FileName);
					}
					// Write to file.
					System.IO.FileStream fs = new System.IO.FileStream(dSaveAs.FileName, System.IO.FileMode.Create);
					fs.Write(buffer, 0, buffer.Length);
					fs.Close();

					// Modify File time
					System.IO.File.SetCreationTime(dSaveAs.FileName, Res.GetCreated());
					System.IO.File.SetLastAccessTime(dSaveAs.FileName, Res.GetAccessed());
					System.IO.File.SetLastWriteTime(dSaveAs.FileName, Res.GetModified());
				}
			}
		}
		private void PreviewById(uint id){
			PackResource Res = m_Pack.GetFileByIndex(id);
			Status.Text = "Now loading data for preview...";
			this.Update();
			if (Res != null)
			{
				PicturePanel.Hide();
				TextView.Clear();
				TextView.Hide();

				String InternalName = Res.GetName();
				string Ext = System.IO.Path.GetExtension(@InternalName);

				// loading file content.
				byte[] buffer = new byte[Res.GetSize()];
				Res.GetData(buffer);
				Res.Close();

				switch (Ext)
				{
					case ".dds":
					case ".psd":
					case ".raw":
						Bitmap bmp = DDSDataToBMP(buffer);
						PictureView.Image = bmp;
						PictureView.Update();
						Status.Text = String.Format("Image file. {0} x {1}", PictureView.Width, PictureView.Height);
						PicturePanel.Show();
					break;
					
					case ".jpg":
					case ".gif":
					case ".bmp":
						// http://stackoverflow.com/questions/2868739/listof-byte-to-picturebox
						var ms = new MemoryStream(buffer);
						PictureView.Image = Image.FromStream(ms);
						ms.Dispose();
						PictureView.Update();
						Status.Text = String.Format("Image file. {0} x {1}", PictureView.Width, PictureView.Height);
						PicturePanel.Show();
						break;
					case ".xml":
					case ".html":
					case ".txt":
						string text = Encoding.Unicode.GetString(buffer);
						
						TextView.Text = text;
						TextView.Update();
						TextView.Show();
						Status.Text = String.Format("ASCII file. {0} chars", text.Length);
						break;
					case ".wav":
					case ".mp3":
						// http://msdn.microsoft.com/en-us/library/ms143770%28v=VS.100%29.aspx 
						WavePlayer wave = new WavePlayer(buffer);
						Status.Text = "Playing sound file.";
						wave.Play();
					break;
					default:
					Status.Text = "Not supported format.";
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
	}
}
