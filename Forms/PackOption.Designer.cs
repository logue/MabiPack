namespace MabiPacker.Forms
{
	partial class PackOption
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
			this.label1 = new System.Windows.Forms.Label();
			this.lVersion = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.Level = new System.Windows.Forms.ComboBox();
			this.PackageVersion = new System.Windows.Forms.NumericUpDown();
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PackageVersion)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(3, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113, 15);
			this.label1.TabIndex = 19;
			this.label1.Text = "Compression &Level :";
			// 
			// lVersion
			// 
			this.lVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lVersion.AutoSize = true;
			this.lVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lVersion.Location = new System.Drawing.Point(3, 8);
			this.lVersion.Name = "lVersion";
			this.lVersion.Size = new System.Drawing.Size(52, 15);
			this.lVersion.TabIndex = 16;
			this.lVersion.Text = "&Version :";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.Level, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.PackageVersion, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.lVersion, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(292, 65);
			this.tableLayoutPanel1.TabIndex = 20;
			// 
			// Level
			// 
			this.Level.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Level.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Level.Items.AddRange(new object[] {
            "Auto",
            "0 - No Compression",
            "1 - Fast",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9 - Best Compression"});
			this.Level.Location = new System.Drawing.Point(122, 35);
			this.Level.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Level.Name = "Level";
			this.Level.Size = new System.Drawing.Size(167, 23);
			this.Level.TabIndex = 2;
			// 
			// PackageVersion
			// 
			this.PackageVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PackageVersion.Location = new System.Drawing.Point(122, 4);
			this.PackageVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.PackageVersion.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
			this.PackageVersion.Name = "PackageVersion";
			this.PackageVersion.Size = new System.Drawing.Size(167, 23);
			this.PackageVersion.TabIndex = 1;
			this.PackageVersion.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(100, 73);
			this.bOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(87, 29);
			this.bOK.TabIndex = 3;
			this.bOK.Text = "OK";
			this.bOK.UseVisualStyleBackColor = true;
			// 
			// bCancel
			// 
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(193, 73);
			this.bCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(87, 29);
			this.bCancel.TabIndex = 4;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			// 
			// PackOption
			// 
			this.AcceptButton = this.bOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(292, 115);
			this.ControlBox = false;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PackOption";
			this.Text = "PackOption";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.PackOption_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.PackageVersion)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lVersion;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ComboBox Level;
		private System.Windows.Forms.NumericUpDown PackageVersion;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
	}
}