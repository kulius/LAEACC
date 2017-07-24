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
    internal partial class XpsExportForm : BaseExportForm
    {
        public override void Init(ExportBase export)
        {
            base.Init(export);
        }
        
        protected override void Done()
        {
            base.Done();
        }
        
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,Xps");
            Text = res.Get("");
        }        
        
        public XpsExportForm()
        {
            InitializeComponent();
        }

    }
}

