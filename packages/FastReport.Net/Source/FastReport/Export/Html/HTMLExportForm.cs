using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.Html;
using FastReport.Utils;


namespace FastReport.Forms
{
    internal partial class HTMLExportForm : BaseExportForm
    {

        public override void Init(ExportBase export)
        {
            base.Init(export);
            HTMLExport htmlExport = Export as HTMLExport;
            cbWysiwyg.Checked = htmlExport.Wysiwyg;
            cbPictures.Checked = htmlExport.Pictures;
            cbSinglePage.Checked = htmlExport.SinglePage;
            cbSubFolder.Checked = htmlExport.SubFolder;
            cbNavigator.Checked = htmlExport.Navigator;
            cbLayers.Checked = htmlExport.Layers;
        }
        
        protected override void Done()
        {
            base.Done();
            HTMLExport htmlExport = Export as HTMLExport;
            htmlExport.Layers = cbLayers.Checked;
            htmlExport.Wysiwyg = cbWysiwyg.Checked;
            htmlExport.Pictures = cbPictures.Checked;
            htmlExport.SinglePage = cbSinglePage.Checked;
            htmlExport.SubFolder = cbSubFolder.Checked;
            htmlExport.Navigator = cbNavigator.Checked;
        }
        
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,Html");
            Text = res.Get("");
            cbLayers.Text = res.Get("Layers");
            cbSinglePage.Text = res.Get("SinglePage");
            cbSubFolder.Text = res.Get("SubFolder");
            cbNavigator.Text = res.Get("Navigator");
            res = new MyRes("Export,Misc");
            gbOptions.Text = res.Get("Options");
            cbWysiwyg.Text = res.Get("Wysiwyg");
            cbPictures.Text = res.Get("Pictures");
        }        
        
        public HTMLExportForm()
        {
            InitializeComponent();
        }

        private void gbPageRange_Enter(object sender, EventArgs e)
        {

        }

    }
}

