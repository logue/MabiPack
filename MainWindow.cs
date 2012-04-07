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
	public partial class MainWindow : Form
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void uCurrentVer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://mabiplus.no-ip.org/global.html");
			return;
		}

		private void bInputDirSelector_Click(object sender, EventArgs e)
		{
			if (dInputDirSelector.ShowDialog(this) == DialogResult.OK){
				InputDir.Text = dInputDirSelector.SelectedPath;
			}
		}

		private void bSaveAs_Click(object sender, EventArgs e)
		{
			if (dSaveAs.ShowDialog(this) == DialogResult.OK)
			{
				SaveAs.Text = dSaveAs.FileName;
			}
		}
		private MabinogiResource.PackResourceSetCreater Pack;

		private void bExecute_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists(InputDir.Text))
			{
				MessageBox.Show("Directory Not found. Please check data directory value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return;
			}

			
			DialogResult result = MessageBox.Show(
				"Are you sure?",
				"Continue",
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1);

			if (result == DialogResult.OK){
				Pack = new PackResourceSetCreater((uint)PackageVersion.Value);
				var default_status_txt = Status.Text;
				// Get Filelist
				
				string[] files = Directory.GetFiles(InputDir.Text, "*", SearchOption.AllDirectories);
				Status.Text = "Adding Files...";
				Progress.Value = 0;
				Progress.Visible = true;
				Progress.Maximum = files.Length;
				

				var file = "";
				var path = "";
				foreach (string s in files)
				{
					Progress.Value++;
					file = Path.GetFileName(s);
					path = Path.GetDirectoryName(s).Replace(InputDir.Text,"data");
					Status.Text = file;
					Console.WriteLine(path + file);
					Pack.AddFile(path, file);
				}
				Progress.Visible = false;
				Status.Text = "Packing...";
				Pack.CreatePack((uint)PackageVersion.Value, SaveAs.Text);
				Status.Text = default_status_txt;
				Pack.Dispose();
			}
		}

		private void Status_Click(object sender, EventArgs e)
		{

		}
	}
}
