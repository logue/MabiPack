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
		private String MabiDir;
		private int MabiVer;
		
		public MainWindow()
		{
			DetectMabinogi();
			InitializeComponent();

			this.Text = AssemblyProduct + String.Format(" v.{0}", AssemblyVersion);

			String PackageDir = (Properties.Settings.Default.LastPackFile != "") ? 
				Properties.Settings.Default.LastPackFile : 
				this.MabiDir + "\\Package";

			SaveAs.Text = PackageDir;
			dSaveAs.InitialDirectory = this.MabiDir + "\\Package";
			InputDir.Text = Properties.Settings.Default.LastDataDir;
			PackageVersion.Minimum = MabiVer;
			PackageVersion.Value = (PackageVersion.Value > MabiVer) ? Properties.Settings.Default.LastPackVer : MabiVer;
			Level.SelectedIndex = 0;

			OpenPack.Text = this.MabiDir + "\\Package";
			dOpenPack.InitialDirectory = this.MabiDir + "\\Package";
			string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			ExtractTo.Text = path;
			dExtractTo.SelectedPath = path;

			labelProductName.Text = AssemblyProduct;
			labelDescription.Text = String.Format("v.{0}", AssemblyVersion);
			labelCopyright.Text = AssemblyCopyright;
			labelDescription.Text = AssemblyDescription;
		}
#region functions
		private void DetectMabinogi() {
			// Get Mabinogi Directory from Registory
			Microsoft.Win32.RegistryKey regkey =
				Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi", false);
			if (regkey == null){
				regkey =
					Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi_test", false);
				if (regkey == null){
					Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Nexon\Mabinogi_hangame", false);
					if (regkey == null) this.MabiDir = "";
				} 
			}
			this.MabiDir = (string) regkey.GetValue("");	// Returns Mabinogi Directory
			regkey.Close();

			// Get Client Version from version.dat
			string version_dat = this.MabiDir + "\\version.dat";
			if (File.Exists(version_dat))
			{
				byte[] data = File.ReadAllBytes(version_dat);
				this.MabiVer = BitConverter.ToInt32(data,0);
			}else{
				this.MabiVer = 0;
			}
		}
#endregion
#region Pack Tab
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

		private void bPack_Click(object sender, EventArgs e)
		{
#region Pack precheck
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
				if (overwrite == DialogResult.No)
				{
					return;
				}
			}
#endregion
			DialogResult result = MessageBox.Show(
				Properties.Resources.Str_Confirm,
				Properties.Resources.Confirm,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1);

			if (result == DialogResult.OK){
				WorkerWindow w = new WorkerWindow();
				w.Show();
				w.Pack(InputDir.Text, SaveAs.Text, (uint)PackageVersion.Value, Level.SelectedIndex-1);
				w.Dispose();
				MessageBox.Show(Properties.Resources.Str_Done, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
			}
		}
#endregion
#region Unpack Tab
		private void bOpenPack_Click(object sender, EventArgs e)
		{
			dOpenPack.InitialDirectory = OpenPack.Text;
			if (dOpenPack.ShowDialog(this) == DialogResult.OK)
			{
				OpenPack.Text = dOpenPack.FileName;
			}
		}

		private void bExtractTo_Click(object sender, EventArgs e)
		{
			dExtractTo.SelectedPath = ExtractTo.Text;
			if (dInputDirSelector.ShowDialog(this) == DialogResult.OK)
			{
				ExtractTo.Text = dInputDirSelector.SelectedPath;
			}
		}

		private void OpenPack_TextChanged(object sender, EventArgs e)
		{
			if (File.Exists(OpenPack.Text))
			{
			}
		}

		private void bUnpack_Click(object sender, EventArgs e)
		{
			// Check input file exsists
			if (!File.Exists(OpenPack.Text))
			{
				MessageBox.Show("File does not exsists.", Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return;
			}
			// Check output directory exsists.
			if (Directory.Exists(ExtractTo.Text))
			{
				DialogResult overwrite = MessageBox.Show("Directory is exisits, overwrite?", Properties.Resources.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				if (overwrite == DialogResult.No)
				{
					return;
				}
			}

			DialogResult result = MessageBox.Show(
				Properties.Resources.Str_Confirm,
				Properties.Resources.Confirm,
				MessageBoxButtons.OKCancel,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button1);

			if (result == DialogResult.OK)
			{
				WorkerWindow w = new WorkerWindow();
				w.Show();
				w.Unpack(OpenPack.Text, ExtractTo.Text, false);
				w.Dispose();
				MessageBox.Show(Properties.Resources.Str_Done, Properties.Resources.Info, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
			}
		}
#endregion
#region About Tab
		#region Misc
		private void lCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://logue.be/");
			return;
		}
		#endregion
		
		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
#endregion
	}
}
