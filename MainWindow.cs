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
using System.Reflection;

namespace MabiPacker
{
	public partial class MainWindow : Form
	{
		private MabinogiResource.PackResourceSetCreater Pack;
		public MainWindow()
		{
			InitializeComponent();
			this.Text = VersionInfo;
			SaveAs.Text = Properties.Settings.Default.LastPackFile;
			InputDir.Text = Properties.Settings.Default.LastDataDir;
			PackageVersion.Value = Properties.Settings.Default.LastPackVer;
		}

		private void uCurrentVer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://mabiplus.no-ip.org/global.html");
			return;
		}

		private void bInputDirSelector_Click(object sender, EventArgs e)
		{
			dInputDirSelector.SelectedPath = InputDir.Text;
			if (dInputDirSelector.ShowDialog(this) == DialogResult.OK){
				InputDir.Text = dInputDirSelector.SelectedPath;
				Properties.Settings.Default.LastDataDir = dInputDirSelector.SelectedPath;
			}
		}

		private void bSaveAs_Click(object sender, EventArgs e)
		{
			dSaveAs.InitialDirectory = SaveAs.Text;
			if (dSaveAs.ShowDialog(this) == DialogResult.OK)
			{
				SaveAs.Text = dSaveAs.FileName;
				Properties.Settings.Default.LastPackFile = dSaveAs.FileName;
			}
		}
		private void bExecute_Click(object sender, EventArgs e)
		{
			// Check directory exsists
			if (!Directory.Exists(InputDir.Text))
			{
				MessageBox.Show(Properties.Resources.Str_DataDirNotExists, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return;
			}
			// Check output file exsists.
			if (File.Exists(SaveAs.Text))
			{
				DialogResult overwrite = MessageBox.Show(Properties.Resources.Str_Overwrite, Properties.Resources.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				if (overwrite == DialogResult.No) {
					return;
				}				
			}
			
			DialogResult result = MessageBox.Show(
				Properties.Resources.Str_Confirm,
				Properties.Resources.Confirm,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1);

			if (result == DialogResult.OK){
				var version = (uint)PackageVersion.Value;
				var default_status_txt = Status.Text;
				var internal_filename = "";

				// Get Filelist
				string[] filelist = Directory.GetFiles(InputDir.Text, "*", SearchOption.AllDirectories);
				//Progress.Value = 0;
				//Progress.Visible = true;
				//Progress.Maximum = filelist.Length;

				// Instance
				Pack = new PackResourceSetCreater(version);
				Pack.CreatePack(version, SaveAs.Text);
				foreach (string path in filelist)
				{
				//	Progress.Value++;
					internal_filename = path.Replace(InputDir.Text, "data");
					Status.Text = internal_filename;
					Pack.AddFile(internal_filename, path);
					Console.WriteLine(internal_filename);
				}
				//Progress.Visible = false;
				Status.Text = Properties.Resources.Str_Packing;
				try{
					Pack.CreatePack(version, SaveAs.Text);
				}catch(Exception err){
					Console.WriteLine(err);
				}
				Pack.Dispose();
				MessageBox.Show(Properties.Resources.Str_Done, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
				Status.Text = default_status_txt;
			}
			return;
		}

		public string VersionInfo
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product + " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}
		private void lCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://logue.be/");
			return;
		}
	}
}
