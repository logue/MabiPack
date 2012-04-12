namespace MabiPacker
{
	partial class WorkerWindow
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
			this.Progress = new System.Windows.Forms.ProgressBar();
			this.Status = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// Progress
			// 
			this.Progress.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.Progress.Location = new System.Drawing.Point(9, 3);
			this.Progress.Name = "Progress";
			this.Progress.Size = new System.Drawing.Size(283, 23);
			this.Progress.TabIndex = 0;
			this.Progress.UseWaitCursor = true;
			// 
			// Status
			// 
			this.Status.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.Status.AutoSize = true;
			this.Status.Location = new System.Drawing.Point(118, 29);
			this.Status.Name = "Status";
			this.Status.Size = new System.Drawing.Size(66, 15);
			this.Status.TabIndex = 1;
			this.Status.Text = "Infomation";
			this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.Status, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.Progress, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(302, 40);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// WorkerWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(302, 40);
			this.ControlBox = false;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WorkerWindow";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Progress";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar Progress;
		private System.Windows.Forms.Label Status;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}