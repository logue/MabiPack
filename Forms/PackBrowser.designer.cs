namespace MabiPacker
{
	partial class PackBrowser
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackBrowser));
			this.SplitContainer = new System.Windows.Forms.SplitContainer();
			this.m_Tree = new System.Windows.Forms.TreeView();
			this.IconList = new System.Windows.Forms.ImageList(this.components);
			this.pPlay = new System.Windows.Forms.PictureBox();
			this.PicturePanel = new System.Windows.Forms.Panel();
			this.PictureView = new System.Windows.Forms.PictureBox();
			this.TextView = new System.Windows.Forms.RichTextBox();
			this.Layout = new System.Windows.Forms.TableLayoutPanel();
			this.StatusBar = new System.Windows.Forms.StatusStrip();
			this.Status = new System.Windows.Forms.ToolStripStatusLabel();
			this.Toolbar = new System.Windows.Forms.ToolStrip();
			this.tbExtract = new System.Windows.Forms.ToolStripButton();
			this.tbUnpack = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
			this.SplitContainer.Panel1.SuspendLayout();
			this.SplitContainer.Panel2.SuspendLayout();
			this.SplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pPlay)).BeginInit();
			this.PicturePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PictureView)).BeginInit();
			this.Layout.SuspendLayout();
			this.StatusBar.SuspendLayout();
			this.Toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// SplitContainer
			// 
			resources.ApplyResources(this.SplitContainer, "SplitContainer");
			this.SplitContainer.Name = "SplitContainer";
			// 
			// SplitContainer.Panel1
			// 
			this.SplitContainer.Panel1.Controls.Add(this.m_Tree);
			// 
			// SplitContainer.Panel2
			// 
			this.SplitContainer.Panel2.Controls.Add(this.pPlay);
			this.SplitContainer.Panel2.Controls.Add(this.PicturePanel);
			this.SplitContainer.Panel2.Controls.Add(this.TextView);
			// 
			// m_Tree
			// 
			resources.ApplyResources(this.m_Tree, "m_Tree");
			this.m_Tree.ImageList = this.IconList;
			this.m_Tree.Name = "m_Tree";
			this.m_Tree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.m_Tree_NodeMouseDoubleClick);
			// 
			// IconList
			// 
			this.IconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IconList.ImageStream")));
			this.IconList.TransparentColor = System.Drawing.Color.Transparent;
			this.IconList.Images.SetKeyName(0, "folder.png");
			this.IconList.Images.SetKeyName(1, "binary.png");
			this.IconList.Images.SetKeyName(2, "txt.png");
			this.IconList.Images.SetKeyName(3, "xml.png");
			this.IconList.Images.SetKeyName(4, "image.png");
			this.IconList.Images.SetKeyName(5, "ttf.png");
			this.IconList.Images.SetKeyName(6, "sound.png");
			// 
			// pPlay
			// 
			this.pPlay.BackgroundImage = global::MabiPacker.Properties.Resources.play;
			resources.ApplyResources(this.pPlay, "pPlay");
			this.pPlay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pPlay.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pPlay.Name = "pPlay";
			this.pPlay.TabStop = false;
			this.pPlay.Click += new System.EventHandler(this.pPlay_Click);
			// 
			// PicturePanel
			// 
			resources.ApplyResources(this.PicturePanel, "PicturePanel");
			this.PicturePanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.PicturePanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PicturePanel.Controls.Add(this.PictureView);
			this.PicturePanel.Name = "PicturePanel";
			// 
			// PictureView
			// 
			this.PictureView.BackColor = System.Drawing.SystemColors.Control;
			this.PictureView.BackgroundImage = global::MabiPacker.Properties.Resources.bg;
			this.PictureView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.PictureView, "PictureView");
			this.PictureView.Name = "PictureView";
			this.PictureView.TabStop = false;
			this.PictureView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureView_MouseDown);
			// 
			// TextView
			// 
			this.TextView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.TextView, "TextView");
			this.TextView.Name = "TextView";
			this.TextView.ReadOnly = true;
			// 
			// Layout
			// 
			resources.ApplyResources(this.Layout, "Layout");
			this.Layout.Controls.Add(this.SplitContainer, 0, 1);
			this.Layout.Controls.Add(this.StatusBar, 0, 2);
			this.Layout.Controls.Add(this.Toolbar, 0, 0);
			this.Layout.Name = "Layout";
			// 
			// StatusBar
			// 
			this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status});
			resources.ApplyResources(this.StatusBar, "StatusBar");
			this.StatusBar.Name = "StatusBar";
			// 
			// Status
			// 
			this.Status.Name = "Status";
			resources.ApplyResources(this.Status, "Status");
			// 
			// Toolbar
			// 
			this.Toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbExtract,
            this.tbUnpack});
			resources.ApplyResources(this.Toolbar, "Toolbar");
			this.Toolbar.Name = "Toolbar";
			// 
			// tbExtract
			// 
			resources.ApplyResources(this.tbExtract, "tbExtract");
			this.tbExtract.Name = "tbExtract";
			this.tbExtract.Click += new System.EventHandler(this.tbExport_Click);
			// 
			// tbUnpack
			// 
			resources.ApplyResources(this.tbUnpack, "tbUnpack");
			this.tbUnpack.Name = "tbUnpack";
			this.tbUnpack.Click += new System.EventHandler(this.tbUnpack_Click);
			// 
			// PackBrowser
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.Layout);
			this.Name = "PackBrowser";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PackBrowser_FormClosing);
			this.Shown += new System.EventHandler(this.PackBrowser_Shown);
			this.SplitContainer.Panel1.ResumeLayout(false);
			this.SplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
			this.SplitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pPlay)).EndInit();
			this.PicturePanel.ResumeLayout(false);
			this.PicturePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.PictureView)).EndInit();
			this.Layout.ResumeLayout(false);
			this.Layout.PerformLayout();
			this.StatusBar.ResumeLayout(false);
			this.StatusBar.PerformLayout();
			this.Toolbar.ResumeLayout(false);
			this.Toolbar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

		private System.Windows.Forms.TreeView m_Tree;
		private System.Windows.Forms.TableLayoutPanel Layout;
        private System.Windows.Forms.SplitContainer SplitContainer;
		private System.Windows.Forms.StatusStrip StatusBar;
		private System.Windows.Forms.ToolStripStatusLabel Status;
		private System.Windows.Forms.ImageList IconList;
		private System.Windows.Forms.RichTextBox TextView;
		private System.Windows.Forms.Panel PicturePanel;
		private System.Windows.Forms.PictureBox PictureView;
        private System.Windows.Forms.ToolStrip Toolbar;
        private System.Windows.Forms.ToolStripButton tbExtract;
		private System.Windows.Forms.ToolStripButton tbUnpack;
		private System.Windows.Forms.PictureBox pPlay;
		
    }
}

