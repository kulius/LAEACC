namespace Demo
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.btnDesign = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.lblVersion = new System.Windows.Forms.Label();
      this.linkLabel1 = new System.Windows.Forms.LinkLabel();
      this.tvReports = new System.Windows.Forms.TreeView();
      this.tbDescription = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panel2 = new System.Windows.Forms.Panel();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.previewControl1 = new FastReport.Preview.PreviewControl();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnDesign
      // 
      this.btnDesign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDesign.FlatAppearance.BorderColor = System.Drawing.Color.White;
      this.btnDesign.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.btnDesign.Location = new System.Drawing.Point(136, 384);
      this.btnDesign.Name = "btnDesign";
      this.btnDesign.Size = new System.Drawing.Size(104, 23);
      this.btnDesign.TabIndex = 0;
      this.btnDesign.Text = "Run the Designer";
      this.btnDesign.UseVisualStyleBackColor = true;
      this.btnDesign.Click += new System.EventHandler(this.btnDesign_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.Color.Transparent;
      this.label1.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.label1.Location = new System.Drawing.Point(76, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(191, 25);
      this.label1.TabIndex = 3;
      this.label1.Text = "FastReport.Net";
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(16, 16);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(48, 48);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 4;
      this.pictureBox1.TabStop = false;
      // 
      // lblVersion
      // 
      this.lblVersion.AutoSize = true;
      this.lblVersion.BackColor = System.Drawing.Color.Transparent;
      this.lblVersion.Location = new System.Drawing.Point(80, 48);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(61, 13);
      this.lblVersion.TabIndex = 5;
      this.lblVersion.Text = "Version 1.0";
      // 
      // linkLabel1
      // 
      this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.linkLabel1.AutoSize = true;
      this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
      this.linkLabel1.Location = new System.Drawing.Point(788, 16);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new System.Drawing.Size(143, 13);
      this.linkLabel1.TabIndex = 7;
      this.linkLabel1.TabStop = true;
      this.linkLabel1.Text = "http://www.fast-report.com";
      this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
      // 
      // tvReports
      // 
      this.tvReports.BackColor = System.Drawing.Color.White;
      this.tvReports.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.tvReports.HideSelection = false;
      this.tvReports.Location = new System.Drawing.Point(8, 8);
      this.tvReports.Name = "tvReports";
      this.tvReports.ShowPlusMinus = false;
      this.tvReports.ShowRootLines = false;
      this.tvReports.Size = new System.Drawing.Size(232, 400);
      this.tvReports.TabIndex = 0;
      this.tvReports.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvReports_AfterSelect);
      // 
      // tbDescription
      // 
      this.tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tbDescription.BackColor = System.Drawing.Color.White;
      this.tbDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.tbDescription.Location = new System.Drawing.Point(8, 28);
      this.tbDescription.Multiline = true;
      this.tbDescription.Name = "tbDescription";
      this.tbDescription.ReadOnly = true;
      this.tbDescription.Size = new System.Drawing.Size(232, 220);
      this.tbDescription.TabIndex = 3;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.label4.Location = new System.Drawing.Point(8, 8);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(74, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Description:";
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.Color.White;
      this.panel1.Controls.Add(this.btnDesign);
      this.panel1.Controls.Add(this.tvReports);
      this.panel1.Location = new System.Drawing.Point(16, 80);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(248, 416);
      this.panel1.TabIndex = 10;
      // 
      // panel2
      // 
      this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.panel2.BackColor = System.Drawing.Color.White;
      this.panel2.Controls.Add(this.tbDescription);
      this.panel2.Controls.Add(this.label4);
      this.panel2.Location = new System.Drawing.Point(16, 500);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(248, 256);
      this.panel2.TabIndex = 11;
      // 
      // comboBox1
      // 
      this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[] {
            "Visual Studio",
            "Office 2003",
            "Office 2007 (blue)",
            "Office 2007 (silver)",
            "Office 2007 (black)",
            "Vista Glass"});
      this.comboBox1.Location = new System.Drawing.Point(812, 44);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(117, 21);
      this.comboBox1.TabIndex = 12;
      this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.BackColor = System.Drawing.Color.Transparent;
      this.label2.Location = new System.Drawing.Point(736, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(69, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Appearance:";
      // 
      // previewControl1
      // 
      this.previewControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.previewControl1.BackColor = System.Drawing.SystemColors.AppWorkspace;
      this.previewControl1.Buttons = ((FastReport.PreviewButtons)(((((((((((FastReport.PreviewButtons.Print | FastReport.PreviewButtons.Open)
                  | FastReport.PreviewButtons.Save)
                  | FastReport.PreviewButtons.Email)
                  | FastReport.PreviewButtons.Find)
                  | FastReport.PreviewButtons.Zoom)
                  | FastReport.PreviewButtons.Outline)
                  | FastReport.PreviewButtons.PageSetup)
                  | FastReport.PreviewButtons.Edit)
                  | FastReport.PreviewButtons.Watermark)
                  | FastReport.PreviewButtons.Navigator)));
      this.previewControl1.Font = new System.Drawing.Font("Tahoma", 8F);
      this.previewControl1.Location = new System.Drawing.Point(268, 80);
      this.previewControl1.Name = "previewControl1";
      this.previewControl1.Size = new System.Drawing.Size(660, 676);
      this.previewControl1.TabIndex = 8;
      this.previewControl1.UIStyle = FastReport.Utils.UIStyle.Office2007Black;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.ClientSize = new System.Drawing.Size(944, 773);
      this.Controls.Add(this.comboBox1);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.previewControl1);
      this.Controls.Add(this.linkLabel1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.lblVersion);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.label1);
      this.EnableGlass = false;
      this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Name = "Form1";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "FastReport.Net Demo";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.Form1_Load);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnDesign;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.LinkLabel linkLabel1;
    private System.Windows.Forms.TreeView tvReports;
    private System.Windows.Forms.TextBox tbDescription;
    private FastReport.Preview.PreviewControl previewControl1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Label label2;
  }
}

