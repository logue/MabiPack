using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;

namespace MabiPacker
{
	public partial class MainWindow : GlassForm
	{
		private String PackageDir;
		private int MabiVer;
        private String filter;
		private Utility.Process w;
		private MabiEnvironment ue;
		private bool isVista;
		
		public MainWindow()
		{
			InitializeComponent();

			OperatingSystem osInfo = Environment.OSVersion;
			this.isVista = (osInfo.Version.Major >= 6) ? true : false;
			this.ue = new MabiEnvironment();

			this.PackageDir = this.ue.MabinogiDir + "\\Package";
			this.MabiVer = (int)this.ue.LocalVersion;

			this.Text = AssemblyProduct + String.Format(" v.{0}", AssemblyVersion);
            this.filter = Properties.Resources.PackFileDesc + "(*.pack)|";

			String PackageDir = (Properties.Settings.Default.LastPackFile != "") ? 
				Properties.Settings.Default.LastPackFile : 
				this.PackageDir;
			if (isVista){
				GlassExtensions.HookGlassRender(InputDir);
				GlassExtensions.HookGlassRender(SaveAs);
				GlassExtensions.HookGlassRender(Level);
				//new GlassRenderer(Level, 0, 0);
				GlassExtensions.HookGlassRender(PackageVersion);
				GlassExtensions.HookGlassRender(OpenPack);
				GlassExtensions.HookGlassRender(ExtractTo);
			}
#region Init Pack Tab
			SaveAs.Text = PackageDir;
			dSaveAs.Filter = this.filter;
			dSaveAs.InitialDirectory = this.PackageDir;
			InputDir.Text = Properties.Settings.Default.LastDataDir;

			PackageVersion.Minimum = this.MabiVer;
			PackageVersion.Value = this.MabiVer;
			Level.SelectedIndex = Properties.Settings.Default.CompressLevel;
#endregion
#region Init Unpack Tab
			OpenPack.Text = this.PackageDir;
			dOpenPack.InitialDirectory = this.PackageDir;
            dOpenPack.Filter = this.filter;

			string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
			ExtractTo.Text = path;
			dExtractTo.SelectedPath = path;

            
#endregion
#region Init About Tab
			labelProductName.Text = AssemblyProduct;
			labelVersion.Text = String.Format("v.{0}", AssemblyVersion);
			labelCopyright.Text = AssemblyCopyright;
			labelDescription.Text = AssemblyDescription;
#endregion
		}
		private void MainWindow_Shown(object sender, EventArgs e)
		{
			w = new Utility.Process(this.Handle);
		}

		private void FinishProcess(){
			Properties.Settings.Default.LastDataDir = InputDir.Text;
			Properties.Settings.Default.LastPackFile = SaveAs.Text;
			Properties.Settings.Default.LastPackVer = (int)PackageVersion.Value;
			Properties.Settings.Default.CompressLevel = Level.SelectedIndex;	
		}
#region Pack Tab Event Handler
		private void uCurrentVer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://mabiplus.no-ip.org/global.html");
			return;
		}

		private void bInputDirSelector_Click(object sender, EventArgs e)
		{
			if (this.isVista){
				CommonOpenFileDialog cfdInputDir = new CommonOpenFileDialog();
				cfdInputDir.EnsureReadOnly = true;
				cfdInputDir.IsFolderPicker = true;
				cfdInputDir.AllowNonFileSystemItems = true;
				cfdInputDir.InitialDirectory = InputDir.Text;
				cfdInputDir.Title = dInputDirSelector.Description;

				if (cfdInputDir.ShowDialog() == CommonFileDialogResult.Ok)
				{
					ShellContainer selectedSO = null;
					selectedSO = cfdInputDir.FileAsShellObject as ShellContainer;
					InputDir.Text = selectedSO.ParsingName;
				}
			}else{
				dInputDirSelector.SelectedPath = InputDir.Text;
				if (dInputDirSelector.ShowDialog(this) == DialogResult.OK){
					InputDir.Text = dInputDirSelector.SelectedPath;
					Properties.Settings.Default.LastDataDir = dInputDirSelector.SelectedPath;
				}
			}
		}

		private void bSaveAs_Click(object sender, EventArgs e)
		{
			if (this.isVista){
				CommonSaveFileDialog cfdSaveAs = new CommonSaveFileDialog();
				cfdSaveAs.Title = dSaveAs.Title;
				cfdSaveAs.AlwaysAppendDefaultExtension = true;
				cfdSaveAs.DefaultExtension = ".pack";
				cfdSaveAs.Filters.Add(new CommonFileDialogFilter(Properties.Resources.PackFileDesc, "*.pack"));
				cfdSaveAs.DefaultDirectory = SaveAs.Text;
				if (cfdSaveAs.ShowDialog() == CommonFileDialogResult.Ok)
				{
					SaveAs.Text = cfdSaveAs.FileName;
				}
			}else{
				dSaveAs.InitialDirectory = SaveAs.Text;
				if (dSaveAs.ShowDialog(this) == DialogResult.OK)
				{
					SaveAs.Text = dSaveAs.FileName;
					Properties.Settings.Default.LastPackFile = dSaveAs.FileName;
				}
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
				this.w.Pack(InputDir.Text, SaveAs.Text, (uint)PackageVersion.Value, Level.SelectedIndex-1);
				FinishProcess();
			}
		}
#endregion
#region Unpack Tab Event Handler
		private void bOpenPack_Click(object sender, EventArgs e)
		{
			if (this.isVista){
				CommonOpenFileDialog cfdOpenPack = new CommonOpenFileDialog();
				cfdOpenPack.EnsureReadOnly = true;
				cfdOpenPack.IsFolderPicker = false;
				cfdOpenPack.AllowNonFileSystemItems = false;
				cfdOpenPack.InitialDirectory = OpenPack.Text;
				cfdOpenPack.Title = dOpenPack.Title;
				cfdOpenPack.Filters.Add(new CommonFileDialogFilter(Properties.Resources.PackFileDesc, "*.pack"));

				if (cfdOpenPack.ShowDialog() == CommonFileDialogResult.Ok)
				{
					OpenPack.Text = cfdOpenPack.FileName;
				}
			}else{
				dOpenPack.InitialDirectory = OpenPack.Text;
				if (dOpenPack.ShowDialog(this) == DialogResult.OK)
				{
					OpenPack.Text = dOpenPack.FileName;
				}
			}
		}

		private void bExtractTo_Click(object sender, EventArgs e)
		{
			if (this.isVista){
				CommonOpenFileDialog cfdExtractTo = new CommonOpenFileDialog();
				cfdExtractTo.EnsureReadOnly = true;
				cfdExtractTo.IsFolderPicker = true;
				cfdExtractTo.AllowNonFileSystemItems = true;
				cfdExtractTo.InitialDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
				cfdExtractTo.Title = dExtractTo.Description;
				if (cfdExtractTo.ShowDialog() == CommonFileDialogResult.Ok)
				{
					ExtractTo.Text = cfdExtractTo.FileName;
				}
			}else{
				dExtractTo.SelectedPath = ExtractTo.Text;
				if (dExtractTo.ShowDialog(this) == DialogResult.OK)
				{
					ExtractTo.Text = dExtractTo.SelectedPath;
				}
			}
		}

		private void OpenPack_TextChanged(object sender, EventArgs e)
		{
			if (File.Exists(OpenPack.Text))
			{
				bContent.Enabled = true;
			}else{
				bContent.Enabled = false;
			}
		}

		private void bContent_Click(object sender, EventArgs e)
		{
			PackBrowser b = new PackBrowser(OpenPack.Text);
			b.Show();
		}

		private void bUnpack_Click(object sender, EventArgs e)
		{
			// Check input file exsists
			if (!File.Exists(OpenPack.Text))
			{
				MessageBox.Show(Properties.Resources.Str_NotFound, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
				return;
			}
			// Check output directory exsists.
			if (Directory.Exists(ExtractTo.Text+"\\data"))
			{
				DialogResult overwrite = MessageBox.Show(Properties.Resources.Str_Overwrite, Properties.Resources.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
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
				w.Unpack(OpenPack.Text, ExtractTo.Text);
				FinishProcess();
			}
		}
#endregion
#region About Tab Event Handler
		private void lCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://logue.be/");
			return;
		}
		private void Logo_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://mabiassist.logue.be/MabiPacker");
			return;
		}
		
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
