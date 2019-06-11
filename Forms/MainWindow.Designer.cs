namespace MabiPacker {
      partial class MainWindow {
            /// <summary>
            /// 必要なデザイナー変数です。
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            /// <summary>
            /// 使用中のリソースをすべてクリーンアップします。
            /// </summary>
            /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
            protected override void Dispose (bool disposing) {
                  if (disposing && (components != null)) {
                        components.Dispose ();
                  }
                  base.Dispose (disposing);
            }

            #region Windows フォーム デザイナーで生成されたコード

            /// <summary>
            /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
            /// コード エディターで変更しないでください。
            /// </summary>
            private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.bInputDirSelector = new System.Windows.Forms.Button();
            this.InputDir = new System.Windows.Forms.TextBox();
            this.lInputDir = new System.Windows.Forms.Label();
            this.lSaveAs = new System.Windows.Forms.Label();
            this.SaveAs = new System.Windows.Forms.TextBox();
            this.bSaveAs = new System.Windows.Forms.Button();
            this.lVersion = new System.Windows.Forms.Label();
            this.PackageVersion = new System.Windows.Forms.NumericUpDown();
            this.bPack = new System.Windows.Forms.Button();
            this.lcopyright = new System.Windows.Forms.LinkLabel();
            this.layoutPack2 = new System.Windows.Forms.TableLayoutPanel();
            this.Tab = new System.Windows.Forms.TabControl();
            this.tPack = new System.Windows.Forms.TabPage();
            this.tlPack = new System.Windows.Forms.TableLayoutPanel();
            this.Level = new System.Windows.Forms.ComboBox();
            this.lLevel = new System.Windows.Forms.Label();
            this.tUnpack = new System.Windows.Forms.TabPage();
            this.tlUnpack = new System.Windows.Forms.TableLayoutPanel();
            this.bContent = new System.Windows.Forms.Button();
            this.bUnpack = new System.Windows.Forms.Button();
            this.bExtractTo = new System.Windows.Forms.Button();
            this.ExtractTo = new System.Windows.Forms.TextBox();
            this.lOpenPack = new System.Windows.Forms.Label();
            this.lExtractTo = new System.Windows.Forms.Label();
            this.OpenPack = new System.Windows.Forms.TextBox();
            this.bOpenPack = new System.Windows.Forms.Button();
            this.tAbout = new System.Windows.Forms.TabPage();
            this.tlAbout = new System.Windows.Forms.TableLayoutPanel();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.LinkLabel();
            this.Logo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PackageVersion)).BeginInit();
            this.Tab.SuspendLayout();
            this.tPack.SuspendLayout();
            this.tlPack.SuspendLayout();
            this.tUnpack.SuspendLayout();
            this.tlUnpack.SuspendLayout();
            this.tAbout.SuspendLayout();
            this.tlAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.SuspendLayout();
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
            this.InputDir.TextChanged += new System.EventHandler(this.InputDir_TextChanged);
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
            1000,
            0,
            0,
            0});
            this.PackageVersion.ValueChanged += new System.EventHandler(this.PackageVersion_ValueChanged);
            // 
            // bPack
            // 
            resources.ApplyResources(this.bPack, "bPack");
            this.bPack.Name = "bPack";
            this.bPack.UseVisualStyleBackColor = true;
            this.bPack.Click += new System.EventHandler(this.bPack_Click);
            // 
            // lcopyright
            // 
            resources.ApplyResources(this.lcopyright, "lcopyright");
            this.lcopyright.Name = "lcopyright";
            this.lcopyright.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lCopyright_LinkClicked);
            // 
            // layoutPack2
            // 
            resources.ApplyResources(this.layoutPack2, "layoutPack2");
            this.layoutPack2.Name = "layoutPack2";
            // 
            // Tab
            // 
            this.Tab.Controls.Add(this.tPack);
            this.Tab.Controls.Add(this.tUnpack);
            this.Tab.Controls.Add(this.tAbout);
            resources.ApplyResources(this.Tab, "Tab");
            this.Tab.Name = "Tab";
            this.Tab.SelectedIndex = 0;
            // 
            // tPack
            // 
            this.tPack.Controls.Add(this.lSaveAs);
            this.tPack.Controls.Add(this.bSaveAs);
            this.tPack.Controls.Add(this.bInputDirSelector);
            this.tPack.Controls.Add(this.tlPack);
            this.tPack.Controls.Add(this.lInputDir);
            this.tPack.Controls.Add(this.InputDir);
            this.tPack.Controls.Add(this.SaveAs);
            resources.ApplyResources(this.tPack, "tPack");
            this.tPack.Name = "tPack";
            this.tPack.UseVisualStyleBackColor = true;
            // 
            // tlPack
            // 
            resources.ApplyResources(this.tlPack, "tlPack");
            this.tlPack.Controls.Add(this.PackageVersion, 2, 0);
            this.tlPack.Controls.Add(this.Level, 2, 1);
            this.tlPack.Controls.Add(this.lLevel, 1, 1);
            this.tlPack.Controls.Add(this.lVersion, 1, 0);
            this.tlPack.Controls.Add(this.bPack, 3, 1);
            this.tlPack.Name = "tlPack";
            // 
            // Level
            // 
            resources.ApplyResources(this.Level, "Level");
            this.Level.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Level.Items.AddRange(new object[] {
            resources.GetString("Level.Items"),
            resources.GetString("Level.Items1"),
            resources.GetString("Level.Items2"),
            resources.GetString("Level.Items3"),
            resources.GetString("Level.Items4"),
            resources.GetString("Level.Items5"),
            resources.GetString("Level.Items6"),
            resources.GetString("Level.Items7"),
            resources.GetString("Level.Items8"),
            resources.GetString("Level.Items9"),
            resources.GetString("Level.Items10")});
            this.Level.Name = "Level";
            // 
            // lLevel
            // 
            resources.ApplyResources(this.lLevel, "lLevel");
            this.lLevel.Name = "lLevel";
            // 
            // tUnpack
            // 
            this.tUnpack.Controls.Add(this.tlUnpack);
            this.tUnpack.Controls.Add(this.bExtractTo);
            this.tUnpack.Controls.Add(this.ExtractTo);
            this.tUnpack.Controls.Add(this.lOpenPack);
            this.tUnpack.Controls.Add(this.lExtractTo);
            this.tUnpack.Controls.Add(this.OpenPack);
            this.tUnpack.Controls.Add(this.bOpenPack);
            resources.ApplyResources(this.tUnpack, "tUnpack");
            this.tUnpack.Name = "tUnpack";
            this.tUnpack.UseVisualStyleBackColor = true;
            // 
            // tlUnpack
            // 
            resources.ApplyResources(this.tlUnpack, "tlUnpack");
            this.tlUnpack.Controls.Add(this.bContent, 1, 1);
            this.tlUnpack.Controls.Add(this.bUnpack, 4, 1);
            this.tlUnpack.Name = "tlUnpack";
            // 
            // bContent
            // 
            resources.ApplyResources(this.bContent, "bContent");
            this.bContent.Name = "bContent";
            this.bContent.UseVisualStyleBackColor = true;
            this.bContent.Click += new System.EventHandler(this.bContent_Click);
            // 
            // bUnpack
            // 
            resources.ApplyResources(this.bUnpack, "bUnpack");
            this.bUnpack.Name = "bUnpack";
            this.bUnpack.UseVisualStyleBackColor = true;
            this.bUnpack.Click += new System.EventHandler(this.bUnpack_Click);
            // 
            // bExtractTo
            // 
            resources.ApplyResources(this.bExtractTo, "bExtractTo");
            this.bExtractTo.Name = "bExtractTo";
            this.bExtractTo.UseVisualStyleBackColor = true;
            this.bExtractTo.Click += new System.EventHandler(this.bExtractTo_Click);
            // 
            // ExtractTo
            // 
            resources.ApplyResources(this.ExtractTo, "ExtractTo");
            this.ExtractTo.Name = "ExtractTo";
            // 
            // lOpenPack
            // 
            resources.ApplyResources(this.lOpenPack, "lOpenPack");
            this.lOpenPack.Name = "lOpenPack";
            // 
            // lExtractTo
            // 
            resources.ApplyResources(this.lExtractTo, "lExtractTo");
            this.lExtractTo.Name = "lExtractTo";
            // 
            // OpenPack
            // 
            resources.ApplyResources(this.OpenPack, "OpenPack");
            this.OpenPack.Name = "OpenPack";
            this.OpenPack.TextChanged += new System.EventHandler(this.OpenPack_TextChanged);
            // 
            // bOpenPack
            // 
            resources.ApplyResources(this.bOpenPack, "bOpenPack");
            this.bOpenPack.Name = "bOpenPack";
            this.bOpenPack.UseVisualStyleBackColor = true;
            this.bOpenPack.Click += new System.EventHandler(this.bOpenPack_Click);
            // 
            // tAbout
            // 
            this.tAbout.Controls.Add(this.tlAbout);
            this.tAbout.Controls.Add(this.Logo);
            resources.ApplyResources(this.tAbout, "tAbout");
            this.tAbout.Name = "tAbout";
            this.tAbout.UseVisualStyleBackColor = true;
            // 
            // tlAbout
            // 
            resources.ApplyResources(this.tlAbout, "tlAbout");
            this.tlAbout.Controls.Add(this.labelProductName, 0, 0);
            this.tlAbout.Controls.Add(this.labelVersion, 0, 3);
            this.tlAbout.Controls.Add(this.labelDescription, 0, 1);
            this.tlAbout.Controls.Add(this.labelCopyright, 0, 4);
            this.tlAbout.Name = "tlAbout";
            // 
            // labelProductName
            // 
            resources.ApplyResources(this.labelProductName, "labelProductName");
            this.labelProductName.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelProductName.Name = "labelProductName";
            // 
            // labelVersion
            // 
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.Name = "labelVersion";
            // 
            // labelDescription
            // 
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.labelDescription.Name = "labelDescription";
            // 
            // labelCopyright
            // 
            resources.ApplyResources(this.labelCopyright, "labelCopyright");
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.TabStop = true;
            // 
            // Logo
            // 
            this.Logo.Image = global::MabiPacker.Properties.Resources.logo;
            resources.ApplyResources(this.Logo, "Logo");
            this.Logo.Name = "Logo";
            this.Logo.TabStop = false;
            this.Logo.Click += new System.EventHandler(this.Logo_Click);
            // 
            // MainWindow
            // 
            this.AcceptButton = this.bPack;
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.Tab);
            this.Controls.Add(this.lcopyright);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            ((System.ComponentModel.ISupportInitialize)(this.PackageVersion)).EndInit();
            this.Tab.ResumeLayout(false);
            this.tPack.ResumeLayout(false);
            this.tPack.PerformLayout();
            this.tlPack.ResumeLayout(false);
            this.tlPack.PerformLayout();
            this.tUnpack.ResumeLayout(false);
            this.tUnpack.PerformLayout();
            this.tlUnpack.ResumeLayout(false);
            this.tlUnpack.PerformLayout();
            this.tAbout.ResumeLayout(false);
            this.tlAbout.ResumeLayout(false);
            this.tlAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.ResumeLayout(false);

            }

            #endregion

            private System.Windows.Forms.Button bInputDirSelector;
            private System.Windows.Forms.TextBox InputDir;
            private System.Windows.Forms.Label lInputDir;
            private System.Windows.Forms.Label lSaveAs;
            private System.Windows.Forms.TextBox SaveAs;
            private System.Windows.Forms.Button bSaveAs;
            private System.Windows.Forms.Label lVersion;
            private System.Windows.Forms.NumericUpDown PackageVersion;
            private System.Windows.Forms.Button bPack;
            private System.Windows.Forms.LinkLabel lcopyright;
            private System.Windows.Forms.TableLayoutPanel layoutPack2;
            private System.Windows.Forms.TabControl Tab;
            private System.Windows.Forms.TabPage tPack;
            private System.Windows.Forms.TabPage tUnpack;
            private System.Windows.Forms.Button bExtractTo;
            private System.Windows.Forms.TextBox ExtractTo;
            private System.Windows.Forms.Label lExtractTo;
            private System.Windows.Forms.Button bOpenPack;
            private System.Windows.Forms.TextBox OpenPack;
            private System.Windows.Forms.Label lOpenPack;
            private System.Windows.Forms.TableLayoutPanel tlPack;
            private System.Windows.Forms.Label lLevel;
            private System.Windows.Forms.ComboBox Level;
            private System.Windows.Forms.TableLayoutPanel tlUnpack;
            private System.Windows.Forms.Button bUnpack;
            private System.Windows.Forms.Button bContent;
            private System.Windows.Forms.TabPage tAbout;
            private System.Windows.Forms.PictureBox Logo;
            private System.Windows.Forms.TableLayoutPanel tlAbout;
            private System.Windows.Forms.Label labelProductName;
            private System.Windows.Forms.Label labelVersion;
            private System.Windows.Forms.Label labelDescription;
            private System.Windows.Forms.LinkLabel labelCopyright;

      }
}
