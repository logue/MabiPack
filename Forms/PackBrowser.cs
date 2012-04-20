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
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

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
				this.Update();
			}
		}
		private void PackBrowser_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_Pack.Dispose();
		}

		private void tbExtract_Click(object sender, EventArgs e)
		{
			TreeNode node = m_Tree.SelectedNode;	// get selected node tag
			if (node != null){
				UnpackById((uint)node.Tag);
			}
		}

		private void m_Tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			PreviewById((uint)e.Node.Tag);
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
									node = node.Nodes.Add(paths[i], paths[i], 4, 4);
									break;
								case ".ttf" :
								case ".ttc":
									node = node.Nodes.Add(paths[i], paths[i], 5, 5);
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
			if (Res != null)
			{
				PicturePanel.Visible = false;
				TextView.Visible = false;
				TextView.Clear();

				String InternalName = Res.GetName();
				string Ext = System.IO.Path.GetExtension(@InternalName);

				// loading file content.
				byte[] buffer = new byte[Res.GetSize()];
				Res.GetData(buffer);
				Res.Close();

				var ms = new MemoryStream(buffer);

				switch (Ext)
				{
					/*
					case ".dds":
					case ".raw":
						PictureView.Image = DDStoBMP(ms);
						PicturePanel.Visible = true;
					break;
					*/
					case ".jpg":
					case ".gif":
					case ".bmp":
						// http://stackoverflow.com/questions/2868739/listof-byte-to-picturebox
						
						PictureView.Image = Image.FromStream(ms);
						PicturePanel.Visible = true;
						break;
					
					case ".xml":
					case ".html":
					case ".txt":
						TextView.Text = Encoding.Unicode.GetString(buffer);
						TextView.Visible = true;
						syntaxHighlight();
						
						break;
					case ".wav":
					case ".mp3":
						// http://msdn.microsoft.com/en-us/library/ms143770%28v=VS.100%29.aspx 
						System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer(ms);
						myPlayer.Play();
					break;
				}
			}
		}
		public Bitmap DDStoBMP(MemoryStream ms)
		{
			// http://pr0jectze10.blogspot.jp/2011/06/xna40.html
			int stride = 4;	// iピクセル辺りの色情報を表すデータの数（RGBAなので4つ）
			Bitmap image = (Bitmap)Image.FromStream(ms);
			// 画像をロックする領域  
			Rectangle lock_rect = new Rectangle { X = 0, Y = 0, Width = image.Width, Height = image.Height };

			// ロックする  
			// Rectangle rect ロック領域  
			// ImageLockMode flags ロック方法 今回は読み取るだけなのでReadOnlyを指定する  
			// PixelFormat format 画像のデータ形式 RGBAデータがほしいのでPixelFormat.Format32bppPArgbを指定する  
			BitmapData bitmap_data = image.LockBits(
				lock_rect,
				ImageLockMode.ReadOnly,
				PixelFormat.Format32bppPArgb
			);

			byte[] buf = new byte[stride];

			// 色情報取得  
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					int pixel_target = x * stride + bitmap_data.Stride * y;
					int array_index = (y * image.Width + x) * stride;

					// ロックしたポインタから色情報を取得（BGRAの順番で格納されてる）  
					for (int i = 0; i < stride; i++)
						buf[array_index + i] =
							System.Runtime.InteropServices.Marshal.ReadByte(
							bitmap_data.Scan0,
							pixel_target + stride - ((1 + i) % stride) - 1);
				}
			}

			// ロックしたらアンロックを忘れずに！
			image.UnlockBits(bitmap_data);

			return image;
		}
		private void syntaxHighlight()
		{
			Font f = new Font("Courier new", 10, FontStyle.Regular);
			Regex r = new Regex("</*(.+?)[ >]");
			regexHighLight(r, f, Color.Purple, 1);
		}

		private void regexHighLight(Regex r, Font font, Color color, int target)
		{
			Regex s = new Regex("\n");
			string[] lines = s.Split(TextView.Text);
			int i = 0;
			foreach (string line in lines)
			{
				MatchCollection col = r.Matches(line);
				foreach (Match m in col)
				{
					TextView.Select(TextView.GetFirstCharIndexFromLine(i) + m.Groups[target].Index, m.Groups[target].Length);
					TextView.SelectionColor = color;
					TextView.SelectionFont = font;
				}
				i++;
			}
		}
	}
}
