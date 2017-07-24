using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.OoXML;
using FastReport.Utils;


namespace FastReport.Forms
{
    internal partial class Word2007ExportForm : BaseExportForm
    {
        public override void Init(ExportBase export)
        {
            base.Init(export);
            MyRes res = new MyRes("Export,Docx");
            Text = res.Get("");
            Word2007Export ooxmlExport = Export as Word2007Export;
            this.radioButtonTable.Checked = (ooxmlExport.MatrixBased == true);
        }
        
        protected override void Done()
        {
            base.Done();
            Word2007Export ooxmlExport = Export as Word2007Export;
            ooxmlExport.MatrixBased = (this.radioButtonTable.Checked == true);
        }
        
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,Misc");
            gbOptions.Text = res.Get("Options");
            radioButtonTable.Text = res.Get("TableBased");
            radioButtonLayers.Text = res.Get("LayerBased");
        }        
        
        public Word2007ExportForm()
        {
            InitializeComponent();
        }

        private void gbPageRange_Enter(object sender, EventArgs e)
        {

        }

    }
}

