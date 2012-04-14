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
using System.Text.RegularExpressions;

namespace MabiPacker
{
	public partial class WorkerWindow : Form
	{
		private PackResourceSetCreater m_Pack;
		private PackResourceSet m_Unpack;
		public WorkerWindow()
		{
			InitializeComponent();
		}
		public void Pack(string InputDir, string OutputFile, uint OutputVer, int Level)
		{
			this.Name = Properties.Resources.Str_Pack;
			Status.Text = Properties.Resources.Str_Initialize;
			this.Update();

			String internal_filename = "";
			if (File.Exists(OutputFile))
			{
				System.IO.File.Delete(@OutputFile);
			}

			// Get Filelist
			string[] filelist = Directory.GetFiles(InputDir, "*", SearchOption.AllDirectories);
			Array.Sort(filelist);
			Progress.Maximum = filelist.Length;

			// Instance
			m_Pack = new PackResourceSetCreater(OutputVer, Level);
			// store file list for pack
			foreach (string path in filelist)
			{
				Progress.Value++;
				internal_filename = path.Replace(InputDir+"\\", "");
				m_Pack.AddFile(internal_filename, path);
				Status.Text = internal_filename;
				this.Update();
			}

			// Start packing
			Status.Text = Properties.Resources.Str_Packing;
			this.Update();
			m_Pack.CreatePack(OutputFile);
			m_Pack.Dispose();

			Status.Text = Properties.Resources.Str_Finish;
		}
		public void Unpack(string InputFile, string OutputDir, bool isCUI)
		{
			this.Name = Properties.Resources.Str_Unpack;
			Status.Text = Properties.Resources.Str_Initialize;
			m_Unpack = PackResourceSet.CreateFromFile(InputFile);
			this.Update();

			uint packed_files = m_Unpack.GetFileCount();
			Progress.Maximum = (int)packed_files;
	
			for (uint i = 0; i < packed_files; ++i)
			{
				PackResource Res = m_Unpack.GetFileByIndex(i);
				String InternalName = Res.GetName();
				Status.Text = InternalName;
				try
				{
					// loading file content.
					byte[] buffer = new byte[Res.GetSize()];
					Res.GetData(buffer);
					Res.Close();

					// Get output Directory Name
					String outputPath = @OutputDir + "\\data\\" + InternalName;

					// Create directory
					String DirPath = System.Text.RegularExpressions.Regex.Replace(outputPath, @"([^\\]*?)$", "");
					if (!Directory.Exists(DirPath))
					{
						Directory.CreateDirectory(DirPath);
					}
					
					// Delete old
					if (File.Exists(outputPath))
					{
						System.IO.File.Delete(@outputPath);
					}
					if (Directory.Exists(outputPath))
					{
						System.IO.Directory.Delete(@outputPath);
					}
					// Write to file.
					System.IO.FileStream fs = new System.IO.FileStream(outputPath, System.IO.FileMode.Create);
					fs.Write(buffer, 0, buffer.Length);
					fs.Close();

					// Modify File time
					System.IO.File.SetCreationTime(outputPath, Res.GetCreated());
					System.IO.File.SetLastAccessTime(outputPath, Res.GetAccessed());
					System.IO.File.SetLastWriteTime(outputPath, Res.GetModified());
				}catch(Exception e){
					Console.WriteLine(e);
					/*
					MessageBox.Show(Properties.Resources.Str_Error + "\r\n" + InternalName,
						Properties.Resources.Error,
						MessageBoxButtons.OK,
						MessageBoxIcon.Error,
						MessageBoxDefaultButton.Button1);
					 */
					continue;
				}
				Progress.Value = (int)i;
				this.Update();
			}
			m_Unpack.Dispose();
			Status.Text = Properties.Resources.Str_Finish;
		}
	}
}
