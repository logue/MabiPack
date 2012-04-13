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
			String internal_filename = "";

			// Get Filelist
			string[] filelist = Directory.GetFiles(InputDir, "*", SearchOption.AllDirectories);
			Progress.Maximum = filelist.Length;

			// Instance
			m_Pack = new PackResourceSetCreater(OutputVer, Level);
			// store file list for pack
			foreach (string path in filelist)
			{
				Progress.Value++;
				internal_filename = path.Replace(InputDir, "");
				m_Pack.AddFile(internal_filename, path);
				Progress.Value++;
				Status.Text = internal_filename;
			}
			// Start packing
			Status.Text = Properties.Resources.Str_Packing;
			m_Pack.CreatePack(OutputFile);
			m_Pack.Dispose();

			Status.Text = Properties.Resources.Str_Finish;
		}
		public void Unpack(string InputFile, string OutputDir, bool isCUI)
		{
			this.Name = Properties.Resources.Str_Unpack;
			Status.Text = Properties.Resources.Str_Initialize;
			m_Unpack = PackResourceSet.CreateFromFile(InputFile);

			uint packed_files = m_Unpack.GetFileCount();
			Progress.Maximum = (int)packed_files;
	
			for (uint i = 0; i < packed_files; ++i)
			{
				PackResource Res = m_Unpack.GetFileByIndex(i);
				String InternalName = Res.GetName();
				Status.Text = InternalName;

				String outputPath = @OutputDir + "\\data\\" + InternalName;
				// Get Directory Name
				String DirPath = System.Text.RegularExpressions.Regex.Replace(outputPath, @"\\[\w|\.]+$", "");
				if (!Directory.Exists(Path.GetDirectoryName(DirPath)))
				{
					Directory.CreateDirectory(DirPath);
				}

				// loading file content.
				System.IO.FileStream fs = new System.IO.FileStream( outputPath ,System.IO.FileMode.Create);
				byte[] buffer = new byte[Res.GetSize()];
				Res.GetData(buffer);
				Res.Close();	

				// Write to file.
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();
				Progress.Value++;
			}
			m_Unpack.Dispose();
			Status.Text = Properties.Resources.Str_Finish;
		}
	}
}
