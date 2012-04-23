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
			this.m_Tree = new System.Windows.Forms.TreeView();
			this.IconList = new System.Windows.Forms.ImageList(this.components);
			this.SplitContainer = new System.Windows.Forms.SplitContainer();
			this.PicturePanel = new System.Windows.Forms.Panel();
			this.PictureView = new System.Windows.Forms.PictureBox();
			this.TextView = new System.Windows.Forms.RichTextBox();
			this.Layout = new System.Windows.Forms.TableLayoutPanel();
			this.StatusBar = new System.Windows.Forms.StatusStrip();
			this.Progress = new System.Windows.Forms.ToolStripProgressBar();
			this.Status = new System.Windows.Forms.ToolStripStatusLabel();
			this.Toolbar = new System.Windows.Forms.ToolStrip();
			this.tbExtract = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
			this.SplitContainer.Panel1.SuspendLayout();
			this.SplitContainer.Panel2.SuspendLayout();
			this.SplitContainer.SuspendLayout();
			this.PicturePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PictureView)).BeginInit();
			this.Layout.SuspendLayout();
			this.StatusBar.SuspendLayout();
			this.Toolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_Tree
			// 
			this.m_Tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_Tree.ImageIndex = 0;
			this.m_Tree.ImageList = this.IconList;
			this.m_Tree.Location = new System.Drawing.Point(0, 0);
			this.m_Tree.Name = "m_Tree";
			this.m_Tree.SelectedImageIndex = 0;
			this.m_Tree.Size = new System.Drawing.Size(295, 508);
			this.m_Tree.TabIndex = 0;
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
			// SplitContainer
			// 
			this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SplitContainer.Location = new System.Drawing.Point(3, 28);
			this.SplitContainer.Name = "SplitContainer";
			// 
			// SplitContainer.Panel1
			// 
			this.SplitContainer.Panel1.Controls.Add(this.m_Tree);
			// 
			// SplitContainer.Panel2
			// 
			this.SplitContainer.Panel2.Controls.Add(this.PicturePanel);
			this.SplitContainer.Panel2.Controls.Add(this.TextView);
			this.SplitContainer.Size = new System.Drawing.Size(778, 508);
			this.SplitContainer.SplitterDistance = 295;
			this.SplitContainer.TabIndex = 1;
			// 
			// PicturePanel
			// 
			this.PicturePanel.AutoScroll = true;
			this.PicturePanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.PicturePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PicturePanel.Controls.Add(this.PictureView);
			this.PicturePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PicturePanel.Location = new System.Drawing.Point(0, 0);
			this.PicturePanel.Name = "PicturePanel";
			this.PicturePanel.Size = new System.Drawing.Size(479, 508);
			this.PicturePanel.TabIndex = 4;
			this.PicturePanel.Visible = false;
			// 
			// PictureView
			// 
			this.PictureView.BackColor = System.Drawing.SystemColors.Control;
			this.PictureView.BackgroundImage = global::MabiPacker.Properties.Resources.bg;
			this.PictureView.Location = new System.Drawing.Point(0, 0);
			this.PictureView.Margin = new System.Windows.Forms.Padding(0);
			this.PictureView.Name = "PictureView";
			this.PictureView.Size = new System.Drawing.Size(100, 100);
			this.PictureView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.PictureView.TabIndex = 1;
			this.PictureView.TabStop = false;
			// 
			// TextView
			// 
			this.TextView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TextView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TextView.Font = new System.Drawing.Font("Courier New", 9F);
			this.TextView.Location = new System.Drawing.Point(0, 0);
			this.TextView.Name = "TextView";
			this.TextView.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.TextView.Size = new System.Drawing.Size(479, 508);
			this.TextView.TabIndex = 5;
			this.TextView.Text = "Sample";
			// 
			// Layout
			// 
			this.Layout.AutoSize = true;
			this.Layout.ColumnCount = 1;
			this.Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.Layout.Controls.Add(this.SplitContainer, 0, 1);
			this.Layout.Controls.Add(this.StatusBar, 0, 2);
			this.Layout.Controls.Add(this.Toolbar, 0, 0);
			this.Layout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Layout.Location = new System.Drawing.Point(0, 0);
			this.Layout.Name = "Layout";
			this.Layout.RowCount = 3;
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
			this.Layout.Size = new System.Drawing.Size(784, 562);
			this.Layout.TabIndex = 2;
			// 
			// StatusBar
			// 
			this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Progress,
            this.Status});
			this.StatusBar.Location = new System.Drawing.Point(0, 539);
			this.StatusBar.Name = "StatusBar";
			this.StatusBar.Size = new System.Drawing.Size(784, 23);
			this.StatusBar.TabIndex = 2;
			this.StatusBar.Text = "statusStrip1";
			// 
			// Progress
			// 
			this.Progress.Name = "Progress";
			this.Progress.Size = new System.Drawing.Size(100, 17);
			// 
			// Status
			// 
			this.Status.Name = "Status";
			this.Status.Size = new System.Drawing.Size(46, 18);
			this.Status.Text = "Status";
			// 
			// Toolbar
			// 
			this.Toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbExtract});
			this.Toolbar.Location = new System.Drawing.Point(0, 0);
			this.Toolbar.Name = "Toolbar";
			this.Toolbar.Size = new System.Drawing.Size(784, 25);
			this.Toolbar.TabIndex = 3;
			this.Toolbar.Text = "toolStrip1";
			// 
			// tbExtract
			// 
			this.tbExtract.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tbExtract.Image = ((System.Drawing.Image)(resources.GetObject("tbExtract.Image")));
			this.tbExtract.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tbExtract.Name = "tbExtract";
			this.tbExtract.Size = new System.Drawing.Size(54, 22);
			this.tbExtract.Text = "Extract";
			this.tbExtract.Click += new System.EventHandler(this.tbExtract_Click);
			// 
			// PackBrowser
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.Layout);
			this.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PackBrowser";
			this.Text = "Browser";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PackBrowser_FormClosing);
			this.Shown += new System.EventHandler(this.PackBrowser_Shown);
			this.SplitContainer.Panel1.ResumeLayout(false);
			this.SplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
			this.SplitContainer.ResumeLayout(false);
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
		private System.Windows.Forms.ToolStrip Toolbar;
		private System.Windows.Forms.ToolStripButton tbExtract;
		private System.Windows.Forms.StatusStrip StatusBar;
		private System.Windows.Forms.ToolStripProgressBar Progress;
		private System.Windows.Forms.ToolStripStatusLabel Status;
		private System.Windows.Forms.ImageList IconList;
		private System.Windows.Forms.RichTextBox TextView;
		private System.Windows.Forms.Panel PicturePanel;
		private System.Windows.Forms.PictureBox PictureView;
		
    }
}

