using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.Dbf;
using FastReport.Utils;
using System.Globalization;

namespace FastReport.Forms
{
    internal partial class DbfExportForm : BaseExportForm
    {
        #region Methods
        public override void Init(ExportBase export)
        {
            base.Init(export);
            DBFExport dbfExport = Export as DBFExport;
            if (dbfExport.Encoding == Encoding.Default)
                cbbCodepage.SelectedIndex = 0;
            else if (dbfExport.Encoding == Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage))
                cbbCodepage.SelectedIndex = 1;
            tbFieldNames.Text = dbfExport.FieldNames;
            cbDataOnly.Checked = dbfExport.DataOnly;
        }
        
        protected override void Done()
        {
            base.Done();
            DBFExport dbfExport = Export as DBFExport;
            if (cbbCodepage.SelectedIndex == 0)
                dbfExport.Encoding = Encoding.Default;
            else if (cbbCodepage.SelectedIndex == 1)
                dbfExport.Encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            dbfExport.FieldNames = tbFieldNames.Text;
            dbfExport.DataOnly = cbDataOnly.Checked;
        }
        
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,Dbf");
            Text = res.Get("");
            lblCodepage.Text = res.Get("Codepage");                        
            cbbCodepage.Items[0] = res.Get("Default");
            cbbCodepage.Items[1] = res.Get("OEM");
            lblFieldNames.Text = res.Get("FieldNames");
            cbDataOnly.Text = res.Get("DataOnly");
            res = new MyRes("Export,Misc");            
            gbOptions.Text = res.Get("Options");
        }        
        
        public DbfExportForm()
        {
            InitializeComponent();
        }
        #endregion
    }
}