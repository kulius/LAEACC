namespace FastReport.Forms
{
    partial class Word2007ExportForm
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
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.radioButtonTable = new System.Windows.Forms.RadioButton();
            this.radioButtonLayers = new System.Windows.Forms.RadioButton();
            this.gbPageRange.SuspendLayout();
            this.pcPages.SuspendLayout();
            this.panPages.SuspendLayout();
            this.gbOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPageRange
            // 
            this.gbPageRange.Location = new System.Drawing.Point(8, 4);
            this.gbPageRange.Enter += new System.EventHandler(this.gbPageRange_Enter);
            // 
            // pcPages
            // 
            this.pcPages.Location = new System.Drawing.Point(0, 0);
            this.pcPages.Size = new System.Drawing.Size(276, 216);
            // 
            // panPages
            // 
            this.panPages.Controls.Add(this.gbOptions);
            this.panPages.Size = new System.Drawing.Size(276, 216);
            this.panPages.Controls.SetChildIndex(this.gbPageRange, 0);
            this.panPages.Controls.SetChildIndex(this.gbOptions, 0);
            // 
            // cbOpenAfter
            // 
            this.cbOpenAfter.Location = new System.Drawing.Point(8, 220);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(112, 244);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(192, 244);
            this.btnCancel.TabIndex = 1;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.radioButtonLayers);
            this.gbOptions.Controls.Add(this.radioButtonTable);
            this.gbOptions.Location = new System.Drawing.Point(8, 136);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(260, 72);
            this.gbOptions.TabIndex = 5;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // radioButtonTable
            // 
            this.radioButtonTable.AutoSize = true;
            this.radioButtonTable.Checked = true;
            this.radioButtonTable.Location = new System.Drawing.Point(12, 19);
            this.radioButtonTable.Name = "radioButtonTable";
            this.radioButtonTable.Size = new System.Drawing.Size(118, 17);
            this.radioButtonTable.TabIndex = 0;
            this.radioButtonTable.TabStop = true;
            this.radioButtonTable.Text = "Table based export";
            this.radioButtonTable.UseVisualStyleBackColor = true;
            // 
            // radioButtonLayers
            // 
            this.radioButtonLayers.AutoSize = true;
            this.radioButtonLayers.Location = new System.Drawing.Point(12, 42);
            this.radioButtonLayers.Name = "radioButtonLayers";
            this.radioButtonLayers.Size = new System.Drawing.Size(119, 17);
            this.radioButtonLayers.TabIndex = 1;
            this.radioButtonLayers.Text = "Layer based export";
            this.radioButtonLayers.UseVisualStyleBackColor = true;
            // 
            // Word2007ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(276, 277);
            this.Name = "Word2007ExportForm";
            this.Text = "Export to MS Word 2007";
            this.gbPageRange.ResumeLayout(false);
            this.gbPageRange.PerformLayout();
            this.pcPages.ResumeLayout(false);
            this.panPages.ResumeLayout(false);
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.RadioButton radioButtonLayers;
        private System.Windows.Forms.RadioButton radioButtonTable;

    }
}
