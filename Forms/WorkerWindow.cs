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

namespace MabiPacker
{
	public partial class WorkerWindow : Form
	{
		private PackResourceSetCreater m_Pack;
		private PackResourceSet m_Unpack;
		public WorkerWindow()
		{
			InitializeComponent();
			Console.WriteLine("Launch Worker");
		}
		public void Pack(string InputDir, string OutputFile, uint OutputVer, int Level, bool isCUI)
		{
#region Pack Prosess
			var internal_filename = "";

			// Get Filelist
			string[] filelist = Directory.GetFiles(InputDir, "*", SearchOption.AllDirectories);
			Progress.Value = 0;
			Progress.Visible = true;
			Progress.Maximum = filelist.Length;

			// Instance
			m_Pack = new PackResourceSetCreater(OutputVer, Level);
			foreach (string path in filelist)
			{
				//	Progress.Value++;
				internal_filename = path.Replace(InputDir, "");
				m_Pack.AddFile(internal_filename, path);
				Progress.Value++;
				Status.Text = internal_filename;
			}
			//Progress.Visible = false;
			Status.Text = Properties.Resources.Str_Packing;
			try
			{
				m_Pack.CreatePack(OutputFile);
			}
			catch (Exception err)
			{
				Console.Write(err);
			}
			m_Pack.Dispose();
#endregion
			MessageBox.Show(Properties.Resources.Str_Done, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
		}
		public void Unpack(string InputFile, string OutputDir, bool isCUI)
		{
			m_Unpack = PackResourceSet.CreateFromFile(InputFile);
			for (uint i = 0; i < m_Unpack.GetFileCount(); ++i){
				PackResource pr = m_Unpack.GetFileByIndex(i);
				Console.WriteLine(pr.GetName());
			}
		}
	}
}
