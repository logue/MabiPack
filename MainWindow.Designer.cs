namespace MabiPacker
{
	partial class MainWindow
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.dInputDirSelector = new System.Windows.Forms.FolderBrowserDialog();
			this.bInputDirSelector = new System.Windows.Forms.Button();
			this.InputDir = new System.Windows.Forms.TextBox();
			this.lInputDir = new System.Windows.Forms.Label();
			this.lSaveAs = new System.Windows.Forms.Label();
			this.SaveAs = new System.Windows.Forms.TextBox();
			this.dSaveAs = new System.Windows.Forms.SaveFileDialog();
			this.bSaveAs = new System.Windows.Forms.Button();
			this.lVersion = new System.Windows.Forms.Label();
			this.PackageVersion = new System.Windows.Forms.NumericUpDown();
			this.uCurrentVer = new System.Windows.Forms.LinkLabel();
			this.bExecute = new System.Windows.Forms.Button();
			this.Status = new System.Windows.Forms.ToolStripStatusLabel();
			this.lcopyright = new System.Windows.Forms.LinkLabel();
			this.StatusBar = new System.Windows.Forms.StatusStrip();
			this.bottomLayout = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.PackageVersion)).BeginInit();
			this.StatusBar.SuspendLayout();
			this.bottomLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// dInputDirSelector
			// 
			resources.ApplyResources(this.dInputDirSelector, "dInputDirSelector");
			this.dInputDirSelector.RootFolder = System.Environment.SpecialFolder.MyComputer;
			this.dInputDirSelector.ShowNewFolderButton = false;
			// 
			// bInputDirSelector
			// 
			resources.ApplyResources(this.bInputDirSelector, "bInputDirSelector");
			this.bInputDirSelector.Name = "bInputDirSelector";
			this.bInputDirSelector.UseVisualStyleBackColor = true;
			this.bInputDirSelector.Click += new System.EventHandler(this.bInputDirSelector_Click);
			// 
			// InputDir
			// 
			this.InputDir.AllowDrop = true;
			resources.ApplyResources(this.InputDir, "InputDir");
			this.InputDir.Name = "InputDir";
			// 
			// lInputDir
			// 
			resources.ApplyResources(this.lInputDir, "lInputDir");
			this.lInputDir.Name = "lInputDir";
			// 
			// lSaveAs
			// 
			resources.ApplyResources(this.lSaveAs, "lSaveAs");
			this.lSaveAs.Name = "lSaveAs";
			// 
			// SaveAs
			// 
			resources.ApplyResources(this.SaveAs, "SaveAs");
			this.SaveAs.Name = "SaveAs";
			// 
			// dSaveAs
			// 
			this.dSaveAs.FileName = "custom.pack";
			this.dSaveAs.InitialDirectory = "C:\\Nexon\\Mabinogi\\Package";
			this.dSaveAs.RestoreDirectory = true;
			// 
			// bSaveAs
			// 
			resources.ApplyResources(this.bSaveAs, "bSaveAs");
			this.bSaveAs.Name = "bSaveAs";
			this.bSaveAs.UseVisualStyleBackColor = true;
			this.bSaveAs.Click += new System.EventHandler(this.bSaveAs_Click);
			// 
			// lVersion
			// 
			resources.ApplyResources(this.lVersion, "lVersion");
			this.lVersion.Name = "lVersion";
			// 
			// PackageVersion
			// 
			resources.ApplyResources(this.PackageVersion, "PackageVersion");
			this.PackageVersion.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
			this.PackageVersion.Name = "PackageVersion";
			this.PackageVersion.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// uCurrentVer
			// 
			resources.ApplyResources(this.uCurrentVer, "uCurrentVer");
			this.uCurrentVer.Name = "uCurrentVer";
			this.uCurrentVer.TabStop = true;
			this.uCurrentVer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uCurrentVer_LinkClicked);
			// 
			// bExecute
			// 
			resources.ApplyResources(this.bExecute, "bExecute");
			this.bExecute.Name = "bExecute";
			this.bExecute.UseVisualStyleBackColor = true;
			this.bExecute.Click += new System.EventHandler(this.bExecute_Click);
			// 
			// Status
			// 
			this.Status.Name = "Status";
			resources.ApplyResources(this.Status, "Status");
			// 
			// lcopyright
			// 
			resources.ApplyResources(this.lcopyright, "lcopyright");
			this.lcopyright.Name = "lcopyright";
			this.lcopyright.TabStop = true;
			this.lcopyright.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lCopyright_LinkClicked);
			// 
			// StatusBar
			// 
			this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status});
			resources.ApplyResources(this.StatusBar, "StatusBar");
			this.StatusBar.Name = "StatusBar";
			// 
			// bottomLayout
			// 
			resources.ApplyResources(this.bottomLayout, "bottomLayout");
			this.bottomLayout.Controls.Add(this.PackageVersion, 1, 0);
			this.bottomLayout.Controls.Add(this.lVersion, 0, 0);
			this.bottomLayout.Controls.Add(this.uCurrentVer, 2, 0);
			this.bottomLayout.Controls.Add(this.bExecute, 2, 1);
			this.bottomLayout.Controls.Add(this.lcopyright, 0, 1);
			this.bottomLayout.Name = "bottomLayout";
			// 
			// MainWindow
			// 
			this.AcceptButton = this.bExecute;
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.bInputDirSelector);
			this.Controls.Add(this.InputDir);
			this.Controls.Add(this.bottomLayout);
			this.Controls.Add(this.StatusBar);
			this.Controls.Add(this.lSaveAs);
			this.Controls.Add(this.bSaveAs);
			this.Controls.Add(this.SaveAs);
			this.Controls.Add(this.lInputDir);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Name = "MainWindow";
			((System.ComponentModel.ISupportInitialize)(this.PackageVersion)).EndInit();
			this.StatusBar.ResumeLayout(false);
			this.StatusBar.PerformLayout();
			this.bottomLayout.ResumeLayout(false);
			this.bottomLayout.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FolderBrowserDialog dInputDirSelector;
		private System.Windows.Forms.Button bInputDirSelector;
		private System.Windows.Forms.TextBox InputDir;
		private System.Windows.Forms.Label lInputDir;
		private System.Windows.Forms.Label lSaveAs;
		private System.Windows.Forms.TextBox SaveAs;
		private System.Windows.Forms.SaveFileDialog dSaveAs;
		private System.Windows.Forms.Button bSaveAs;
		private System.Windows.Forms.Label lVersion;
		private System.Windows.Forms.NumericUpDown PackageVersion;
		private System.Windows.Forms.LinkLabel uCurrentVer;
		private System.Windows.Forms.Button bExecute;
		private System.Windows.Forms.LinkLabel lcopyright;
		private System.Windows.Forms.StatusStrip StatusBar;
		private System.Windows.Forms.ToolStripStatusLabel Status;
		private System.Windows.Forms.TableLayoutPanel bottomLayout;
	}
}

