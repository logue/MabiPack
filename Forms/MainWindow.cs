using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MabiPacker
{
    public partial class MainWindow : GlassForm
    {
        private String PackageDir;
        private int MabiVer;
        private String filter;
        private Worker w;
        private Dialogs d;
        private MabiEnvironment env;
        private bool isVista;
        public MainWindow()
        {
            InitializeComponent();
            OperatingSystem osInfo = Environment.OSVersion;
            this.isVista = (osInfo.Version.Major >= 6) ? true : false;
            //this.env = new MabiEnvironment(Properties.Resources.Uri_PatchTxt);
            this.env = new MabiEnvironment();
            this.d = new Dialogs();
            this.w = new Worker();
            this.PackageDir = this.env.MabinogiDir + "\\Package";
            this.MabiVer = (int)this.env.LocalVersion;
            this.Text = AssemblyProduct + String.Format(" v.{0}", AssemblyVersion);
            this.filter = Properties.Resources.PackFileDesc + "(*.pack)|";
            String PackageDir = (Properties.Settings.Default.LastPackFile != "") ?
                Properties.Settings.Default.LastPackFile :
                this.PackageDir;
            if (isVista)
            {
                GlassExtensions.HookGlassRender(InputDir);
                GlassExtensions.HookGlassRender(SaveAs);
                GlassExtensions.HookGlassRender(Level);
                //new GlassRenderer(Level, 0, 0);
                GlassExtensions.HookGlassRender(PackageVersion);
                GlassExtensions.HookGlassRender(OpenPack);
                GlassExtensions.HookGlassRender(ExtractTo);
            }
            #region Init Pack Tab
            PackageVersion.Minimum = this.MabiVer;
            PackageVersion.Value = Int32.Parse(DateTime.Today.ToString("yyMMdd"));
            SaveAs.Text = env.MabinogiDir + "\\Package\\custom-" + PackageVersion.Value.ToString() + ".pack";
            Level.SelectedIndex = 0;
            #endregion
            #region Init Unpack Tab

            string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            ExtractTo.Text = path;
            #endregion
            #region Init About Tab
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = String.Format("v.{0}", AssemblyVersion);
            labelCopyright.Text = AssemblyCopyright;
            labelDescription.Text = AssemblyDescription;
            #endregion
        }
        #region Pack Tab Event Handler
        private void bInputDirSelector_Click(object sender, EventArgs e)
        {
            InputDir.Text = d.InputDir(InputDir.Text);
        }
        private void bSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs.Text = d.OutputFile(SaveAs.Text);
        }
        private void InputDir_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(InputDir.Text))
            {
                bPack.Enabled = true;
            }
            else
            {
                bPack.Enabled = false;
            }
        }
        private void bPack_Click(object sender, EventArgs e)
        {
            Console.WriteLine(InputDir.Text);
            Console.WriteLine(SaveAs.Text);
            Console.WriteLine(PackageVersion.Value);
            Console.WriteLine(Level.SelectedIndex - 1);
            d.Pack(InputDir.Text, SaveAs.Text, (uint)PackageVersion.Value, (Level.SelectedIndex - 1));
        }
        private void PackageVersion_ValueChanged(object sender, EventArgs e)
        {
            SaveAs.Text = env.MabinogiDir + "\\Package\\custom-" + PackageVersion.Value.ToString() + ".pack";
        }
        #endregion
        #region Unpack Tab Event Handler
        private void bOpenPack_Click(object sender, EventArgs e)
        {
            OpenPack.Text = d.InputFile(OpenPack.Text);
        }
        private void bExtractTo_Click(object sender, EventArgs e)
        {
            ExtractTo.Text = d.OutputDir(ExtractTo.Text);
        }
        private void OpenPack_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(OpenPack.Text))
            {
                bContent.Enabled = true;
                bUnpack.Enabled = true;
            }
            else
            {
                bContent.Enabled = false;
                bUnpack.Enabled = false;
            }
        }
        private void bContent_Click(object sender, EventArgs e)
        {
            PackBrowser b = new PackBrowser(OpenPack.Text);
            b.Show();
        }
        private void bUnpack_Click(object sender, EventArgs e)
        {
            d.Unpack(OpenPack.Text, ExtractTo.Text);
        }
        #endregion
        #region About Tab Event Handler
        private void lCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://logue.be/");
            return;
        }
        private void Logo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://mabiassist.logue.be/MabiPacker");
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
        private void bRepack_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(this.env.MabinogiDir + "\\Package\\");
            foreach (string file in files)
            {

            }
        }
    }
}
