using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Caching;
using System.IO;
using FastReport;
using FastReport.Export;
using FastReport.Export.Html;
using FastReport.Export.RichText;
using FastReport.Export.Xml;
using FastReport.Export.Odf;
using FastReport.Export.Pdf;
using FastReport.Export.Csv;
using FastReport.Export.Text;
using FastReport.Export.OoXML;
using FastReport.Export.Mht;
using FastReport.Utils;
using System.Threading;
using System.Drawing;
using System.Web.Configuration;
using System.Drawing.Design;
using System.Collections;
using System.Data;
using FastReport.Data;

namespace FastReport.Web
{
    /// <summary>
    /// Represents the Web Report.
    /// </summary>        
    [Designer("FastReport.VSDesign.WebReportComponentDesigner, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL")]
    [ToolboxBitmap(typeof(WebReport), "Resources.Report.bmp")]
    public
    partial class WebReport : WebControl, INamingContainer
    {
        #region Private fields
      
        private ImageButton btnFirst;
        private ImageButton btnPrev;
        private ImageButton btnNext;
        private ImageButton btnLast;
        private HtmlTable tblNavigator;
        private HtmlGenericControl div;
        private ImageButton btnExport;
        private ImageButton btnPrint;
        private DropDownList cbbExportList;
        private DropDownList cbbZoom;
        private ImageButton btnRefresh;
        private Label lblPages;
        private TextBox tbPage;

        //!!!
//        private Button btnPrintBrowser;

        private const string FPicsPrefix = "frximg";
        private const string FBtnPrefix = "frxbtn";
        private const string FExportPrefix = "frxexp";               

        #endregion

        #region Public properties

        /// <summary>
        /// Get or sets auto width of report
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Browsable(true)]
        public bool AutoWidth
        {
            get
            {
                return (this.ViewState["frxAutoWidth"] != null) ?
                    (bool)this.ViewState["frxAutoWidth"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxAutoWidth"] = value;
                else
                    this.ViewState.Remove("frxAutoWidth");
            }
        }

        /// <summary>
        /// Get or sets auto height of report
        /// </summary>
        [DefaultValue(false)]
        [Category("Layout")]
        [Browsable(true)]
        public bool AutoHeight
        {
            get
            {
                return (this.ViewState["frxAutoHeight"] != null) ?
                    (bool)this.ViewState["frxAutoHeight"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxAutoHeight"] = value;
                else
                    this.ViewState.Remove("frxAutoHeight");
            }
        }

        /// <summary>
        /// Gets or sets Padding of Report section
        /// </summary>
        [Category("Report")]
        [Browsable(true)]
        public System.Windows.Forms.Padding Padding
        {
            get 
            {       
                if (this.ViewState["frxPadding"] == null)
                    return new System.Windows.Forms.Padding(0, 0, 0, 0);
                else
                    return (System.Windows.Forms.Padding)this.ViewState["frxPadding"];
            }
            set 
            {
                this.ViewState["frxPadding"] = value; 
            }
        }

        /// <summary>
        /// Delay in cache in minutes
        /// </summary>
        [Category("Network")]
        [DefaultValue(15)]
        [Browsable(true)]
        public int CacheDelay
        {
            get
            {
                return (this.ViewState["frxCacheDelay"] != null) ?
                    (int)this.ViewState["frxCacheDelay"] : 15;
            }
            set 
            {
                if (value != 15)
                    this.ViewState["frxCacheDelay"] = value;
                else
                    this.ViewState.Remove("frxCacheDelay");
            }
        }

        /// <summary>
        /// Report Resource String
        /// </summary>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        public string ReportResourceString
        {
            get
            {
                return (this.ViewState["frxRRS"] != null) ?
                    (string)this.ViewState["frxRRS"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxRRS"] = value;
                else
                    this.ViewState.Remove("frxRRS");
            }
        }

        /// <summary>
        /// Gets or sets report data source(s).
        /// </summary>
        /// <remarks>
        /// To pass several datasources, use ';' delimiter, for example: 
        /// "sqlDataSource1;sqlDataSource2"
        /// </remarks>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        public string ReportDataSources
        {
            get
            {
                return (this.ViewState["frxReportDataSources"] != null) ?
                    (string)this.ViewState["frxReportDataSources"] : String.Empty;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxReportDataSources"] = value;
                else
                    this.ViewState.Remove("frxReportDataSources");
            }
        }

        /// <summary>
        ///  Switch toolbar visibility
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowToolbar
        {
            get
            {
                return (this.ViewState["frxToolbar"] != null) ?
                    (bool)this.ViewState["frxToolbar"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxToolbar"] = value;
                else
                    this.ViewState.Remove("frxToolbar");
            }
        }

        /// <summary>
        /// Sets the path to custom buttons on site. 
        /// </summary>
        /// <remarks>
        /// Pictures should be named:
        /// Export.gif, First.gif, First_disabled.gif, Last.gif, Last_disabled.gif, Next.gif, 
        /// Next_disabled.gif, Prev.gif, Prev_disabled.gif, Print.gif, Refresh.gif, Zoom.gif
        /// </remarks>
        [DefaultValue("")]
        [Category("Toolbar")]
        [Browsable(true)]
        public string ButtonsPath
        {
            get
            {
                return (this.ViewState["frxButtonsPath"] != null) ?
                    (string)this.ViewState["frxButtonsPath"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxButtonsPath"] = value;
                else
                    this.ViewState.Remove("frxButtonsPath");
            }
        }

        /// <summary>
        ///  Switch visibility of Exports in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowExports
        {
            get
            {
                return (this.ViewState["frxExports"] != null) ?
                    (bool)this.ViewState["frxExports"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxExports"] = value;
                else
                    this.ViewState.Remove("frxExports");
            }
        }

        /// <summary>
        ///  Switch visibility of Print button in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPrint
        {
            get
            {
                return (this.ViewState["frxPrint"] != null) ?
                    (bool)this.ViewState["frxPrint"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPrint"] = value;
                else
                    this.ViewState.Remove("frxPrint");
            }
        }

        /// <summary>
        ///  Switch visibility of First Button in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowFirstButton
        {
            get
            {
                return (this.ViewState["frxFirstButton"] != null) ?
                    (bool)this.ViewState["frxFirstButton"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxFirstButton"] = value;
                else
                    this.ViewState.Remove("frxFirstButton");
            }
        }

        /// <summary>
        ///  Switch visibility of Previous Button in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPrevButton
        {
            get
            {
                return (this.ViewState["frxPrevButton"] != null) ?
                    (bool)this.ViewState["frxPrevButton"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPrevButton"] = value;
                else
                    this.ViewState.Remove("frxPrevButton");
            }
        }

        /// <summary>
        ///  Switch visibility of Next Button in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowNextButton
        {
            get
            {
                return (this.ViewState["frxNextButton"] != null) ?
                    (bool)this.ViewState["frxNextButton"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxNextButton"] = value;
                else
                    this.ViewState.Remove("frxNextButton");
            }
        }

        /// <summary>
        ///  Switch visibility of Last Button in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowLastButton
        {
            get
            {
                return (this.ViewState["frxLastButton"] != null) ?
                    (bool)this.ViewState["frxLastButton"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxLastButton"] = value;
                else
                    this.ViewState.Remove("frxLastButton");
            }
        }

        /// <summary>
        /// Switch visibility of Zoom in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowZoomButton
        {
            get
            {
                return (this.ViewState["frxZoomButton"] != null) ?
                    (bool)this.ViewState["frxZoomButton"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxZoomButton"] = value;
                else
                    this.ViewState.Remove("frxZoomButton");
            }
        }

        /// <summary>
        /// Switch visibility of Refresh in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowRefreshButton
        {
            get
            {
                return (this.ViewState["frxRefreshButton"] != null) ?
                    (bool)this.ViewState["frxRefreshButton"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxRefreshButton"] = value;
                else
                    this.ViewState.Remove("frxRefreshButton");
            }
        }

        /// <summary>
        /// Switch visibility of Page Number in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPageNumber
        {
            get
            {
                return (this.ViewState["frxShowPageNumber"] != null) ?
                    (bool)this.ViewState["frxShowPageNumber"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowPageNumber"] = value;
                else
                    this.ViewState.Remove("frxShowPageNumber");
            }
        }

        #region RTF format

        /// <summary>
        /// Switch visibility of RTF export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowRtfExport
        {
            get
            {
                return (this.ViewState["frxShowRtfExport"] != null) ?
                    (bool)this.ViewState["frxShowRtfExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowRtfExport"] = value;
                else
                    this.ViewState.Remove("frxShowRtfExport");
            }
        }

        /// <summary>
        /// Gets or sets the quality of Jpeg images in RTF file.
        /// </summary>
        /// <remarks>
        /// Default value is 90. This property will be used if you select Jpeg 
        /// in the <see cref="RtfImageFormat"/> property.
        /// </remarks>
        [DefaultValue(90)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public int RtfJpegQuality
        {
            get
            {
                return (this.ViewState["frxRtfJpegQuality"] != null) ?
                    (int)this.ViewState["frxRtfJpegQuality"] : 90;
            }
            set
            {
                if (value != 90)
                    this.ViewState["frxRtfJpegQuality"] = value;
                else
                    this.ViewState.Remove("frxRtfJpegQuality");
            }
        }

        /// <summary>
        /// Gets or sets the image format that will be used to save pictures in RTF file.
        /// </summary>
        /// <remarks>
        /// Default value is <b>Metafile</b>. This format is better for exporting such objects as
        /// <b>MSChartObject</b> and <b>ShapeObject</b>.
        /// </remarks>
        [DefaultValue(RTFImageFormat.Metafile)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public RTFImageFormat RtfImageFormat
        {
            get
            {
                return (this.ViewState["frxRtfImageFormat"] != null) ?
                    (RTFImageFormat)this.ViewState["frxRtfImageFormat"] : RTFImageFormat.Metafile;
            }
            set
            {
                if (value != RTFImageFormat.Metafile)
                    this.ViewState["frxRtfImageFormat"] = value;
                else
                    this.ViewState.Remove("frxRtfImageFormat");
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating that pictures are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public bool RtfPictures
        {
            get
            {
                return (this.ViewState["frxRtfPictures"] != null) ?
                    (bool)this.ViewState["frxRtfPictures"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxRtfPictures"] = value;
                else
                    this.ViewState.Remove("frxRtfPictures");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public bool RtfPageBreaks
        {
            get
            {
                return (this.ViewState["frxRtfPageBreaks"] != null) ?
                    (bool)this.ViewState["frxRtfPageBreaks"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxRtfPageBreaks"] = value;
                else
                    this.ViewState.Remove("frxRtfPageBreaks");
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used 
        /// for better results.
        /// </summary>
        /// <remarks>
        /// Default value is <b>true</b>. In wysiwyg mode, the resulting rtf file will look
        /// as close as possible to the prepared report. On the other side, it may have a lot 
        /// of small rows/columns, which will make it less editable. If you set this property
        /// to <b>false</b>, the number of rows/columns in the resulting file will be decreased.
        /// You will get less wysiwyg, but more editable file.
        /// </remarks>
        [DefaultValue(true)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public bool RtfWysiwyg
        {
            get
            {
                return (this.ViewState["frxRtfWysiwyg"] != null) ?
                    (bool)this.ViewState["frxRtfWysiwyg"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxRtfWysiwyg"] = value;
                else
                    this.ViewState.Remove("frxRtfWysiwyg");
            }
        }

        /// <summary>
        /// Gets or sets the creator of the document.
        /// </summary>
        [DefaultValue("FastReport")]
        [Category("Rtf Format")]
        [Browsable(true)]
        public string RtfCreator
        {
            get
            {
                return (this.ViewState["frxRtfCreator"] != null) ?
                    (string)this.ViewState["frxRtfCreator"] : "FastReport";
            }
            set
            {
                if (value != "FastReport")
                    this.ViewState["frxRtfCreator"] = value;
                else
                    this.ViewState.Remove("frxRtfCreator");
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the rows in the resulting table 
        /// should calculate its height automatically.
        /// </summary>
        /// <remarks>
        /// Default value for this property is <b>false</b>. In this mode, each row in the
        /// resulting table has fixed height to get maximum wysiwyg. If you set it to <b>true</b>,
        /// the height of resulting table will be calculated automatically by the Word processor.
        /// The document will be more editable, but less wysiwyg.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Rtf Format")]
        [Browsable(true)]
        public bool RtfAutoSize
        {
            get
            {
                return (this.ViewState["frxRtfAutoSize"] != null) ?
                    (bool)this.ViewState["frxRtfAutoSize"] : false;
            }
            set
            {
                if (value)
                    this.ViewState["frxRtfAutoSize"] = value;
                else
                    this.ViewState.Remove("frxRtfAutoSize");
            }
        }
        
        #endregion RTF format

        #region MHT format
        /// <summary>
        /// Switch visibility of MHT (web-archive) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowMhtExport
        {
            get
            {
                return (this.ViewState["frxShowMhtExport"] != null) ?
                    (bool)this.ViewState["frxShowMhtExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowMhtExport"] = value;
                else
                    this.ViewState.Remove("frxShowMhtExport");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that pictures are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Mht Format")]
        [Browsable(true)]
        public bool MhtPictures
        {
            get
            {
                return (this.ViewState["frxMhtPictures"] != null) ?
                    (bool)this.ViewState["frxMhtPictures"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxMhtPictures"] = value;
                else
                    this.ViewState.Remove("frxMhtPictures");
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used 
        /// for better results.
        /// </summary>
        /// <remarks>
        /// Default value is <b>true</b>. In wysiwyg mode, the resulting rtf file will look
        /// as close as possible to the prepared report. On the other side, it may have a lot 
        /// of small rows/columns, which will make it less editable. If you set this property
        /// to <b>false</b>, the number of rows/columns in the resulting file will be decreased.
        /// You will get less wysiwyg, but more editable file.
        /// </remarks>
        [DefaultValue(true)]
        [Category("Mht Format")]
        [Browsable(true)]
        public bool MhtWysiwyg
        {
            get
            {
                return (this.ViewState["frxMhtWysiwyg"] != null) ?
                    (bool)this.ViewState["frxMhtWysiwyg"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxMhtWysiwyg"] = value;
                else
                    this.ViewState.Remove("frxMhtWysiwyg");
            }
        }
        #endregion MHT format

        #region ODS format

        /// <summary>
        /// Switch visibility of ODS export in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowOdsExport
        {
            get
            {
                return (this.ViewState["frxShowOdsExport"] != null) ?
                    (bool)this.ViewState["frxShowOdsExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowOdsExport"] = value;
                else
                    this.ViewState.Remove("frxShowOdsExport");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Ods Format")]
        [Browsable(true)]
        public bool OdsPageBreaks
        {
            get
            {
                return (this.ViewState["frxOdsPageBreaks"] != null) ?
                    (bool)this.ViewState["frxOdsPageBreaks"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxOdsPageBreaks"] = value;
                else
                    this.ViewState.Remove("frxOdsPageBreaks");
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used 
        /// for better results.
        /// </summary>
        /// <remarks>
        /// Default value is <b>true</b>. In wysiwyg mode, the resulting rtf file will look
        /// as close as possible to the prepared report. On the other side, it may have a lot 
        /// of small rows/columns, which will make it less editable. If you set this property
        /// to <b>false</b>, the number of rows/columns in the resulting file will be decreased.
        /// You will get less wysiwyg, but more editable file.
        /// </remarks>
        [DefaultValue(true)]
        [Category("Ods Format")]
        [Browsable(true)]
        public bool OdsWysiwyg
        {
            get
            {
                return (this.ViewState["frxOdsWysiwyg"] != null) ?
                    (bool)this.ViewState["frxOdsWysiwyg"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxOdsWysiwyg"] = value;
                else
                    this.ViewState.Remove("frxOdsWysiwyg");
            }
        }

        /// <summary>
        /// Gets or sets the creator of the document.
        /// </summary>
        [DefaultValue("FastReport")]
        [Category("Ods Format")]
        [Browsable(true)]
        public string OdsCreator
        {
            get
            {
                return (this.ViewState["frxOdsCreator"] != null) ?
                    (string)this.ViewState["frxOdsCreator"] : "FastReport";
            }
            set
            {
                if (value != "FastReport")
                    this.ViewState["frxOdsCreator"] = value;
                else
                    this.ViewState.Remove("frxOdsCreator");
            }
        }
        #endregion ODS format

        #region Excel2007 format

        /// <summary>
        /// Switch visibility of Excel 2007 export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowExcel2007Export
        {
            get
            {
                return (this.ViewState["frxShowXlsxExport"] != null) ?
                    (bool)this.ViewState["frxShowXlsxExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowXlsxExport"] = value;
                else
                    this.ViewState.Remove("frxShowXlsxExport");
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Excel 2007 Format")]
        [Browsable(true)]
        public bool XlsxPageBreaks
        {
            get
            {
                return (this.ViewState["frxXlsxPageBreaks"] != null) ?
                    (bool)this.ViewState["frxXlsxPageBreaks"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxXlsxPageBreaks"] = value;
                else
                    this.ViewState.Remove("frxXlsxPageBreaks");
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used 
        /// for better results.
        /// </summary>
        /// <remarks>
        /// Default value is <b>true</b>. In wysiwyg mode, the resulting rtf file will look
        /// as close as possible to the prepared report. On the other side, it may have a lot 
        /// of small rows/columns, which will make it less editable. If you set this property
        /// to <b>false</b>, the number of rows/columns in the resulting file will be decreased.
        /// You will get less wysiwyg, but more editable file.
        /// </remarks>
        [DefaultValue(true)]
        [Category("Excel 2007 Format")]
        [Browsable(true)]
        public bool XlsxWysiwyg
        {
            get
            {
                return (this.ViewState["frxXlsxWysiwyg"] != null) ?
                    (bool)this.ViewState["frxXlsxWysiwyg"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxXlsxWysiwyg"] = value;
                else
                    this.ViewState.Remove("frxXlsxWysiwyg");
            }
        }
        #endregion Excel2007 format 

        #region PowerPoint2007 format
        /// <summary>
        /// Switch visibility of PowerPoint 2007 export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPowerPoint2007Export
        {
            get
            {
                return (this.ViewState["frxShowPptxExport"] != null) ?
                    (bool)this.ViewState["frxShowPptxExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowPptxExport"] = value;
                else
                    this.ViewState.Remove("frxShowPptxExport");
            }
        }

        /// <summary>
        /// Gets or sets the image format that will be used to save pictures in PowerPoint file.
        /// </summary>
        [DefaultValue(PptImageFormat.Png)]
        [Category("PowerPoint 2007 Format")]
        [Browsable(true)]
        public PptImageFormat PptxImageFormat
        {
            get
            {
                return (this.ViewState["frxPptxImageFormat"] != null) ?
                    (PptImageFormat)this.ViewState["frxPptxImageFormat"] : PptImageFormat.Png;
            }
            set
            {
                if (value != PptImageFormat.Png)
                    this.ViewState["frxPptxImageFormat"] = value;
                else
                    this.ViewState.Remove("frxPptxImageFormat");
            }
        }
        #endregion PowerPoint2007 format

        #region XML format
        /// <summary>
        ///  Switch visibility of XML (Excel) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowXmlExcelExport
        {
            get
            {
                return (this.ViewState["frxShowXmlExcelExport"] != null) ?
                    (bool)this.ViewState["frxShowXmlExcelExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowXmlExcelExport"] = value;
                else
                    this.ViewState.Remove("frxShowXmlExcelExport");
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Xml Excel Format")]
        [Browsable(true)]
        public bool XmlExcelPageBreaks
        {
            get
            {
                return (this.ViewState["frxXmlPageBreaks"] != null) ?
                    (bool)this.ViewState["frxXmlPageBreaks"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxXmlPageBreaks"] = value;
                else
                    this.ViewState.Remove("frxXmlPageBreaks");
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used 
        /// for better results.
        /// </summary>
        /// <remarks>
        /// Default value is <b>true</b>. In wysiwyg mode, the resulting rtf file will look
        /// as close as possible to the prepared report. On the other side, it may have a lot 
        /// of small rows/columns, which will make it less editable. If you set this property
        /// to <b>false</b>, the number of rows/columns in the resulting file will be decreased.
        /// You will get less wysiwyg, but more editable file.
        /// </remarks>
        [DefaultValue(true)]
        [Category("Xml Excel Format")]
        [Browsable(true)]
        public bool XmlExcelWysiwyg
        {
            get
            {
                return (this.ViewState["frxXmlWysiwyg"] != null) ?
                    (bool)this.ViewState["frxXmlWysiwyg"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxXmlWysiwyg"] = value;
                else
                    this.ViewState.Remove("frxXmlWysiwyg");
            }
        }

        #endregion XML format

        #region PDF format
        /// <summary>
        ///  Switch visibility of PDF (Adobe Acrobat) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowPdfExport
        {
            get
            {
                return (this.ViewState["frxShowPdfExport"] != null) ?
                    (bool)this.ViewState["frxShowPdfExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowPdfExport"] = value;
                else
                    this.ViewState.Remove("frxShowPdfExport");
            }
        }


        /// <summary>
        /// Enable or disable of embedding the TrueType fonts.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfEmbeddingFonts
        {
            get
            {
                return (this.ViewState["frxPdfEmbeddingFonts"] != null) ?
                    (bool)this.ViewState["frxPdfEmbeddingFonts"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxPdfEmbeddingFonts"] = value;
                else
                    this.ViewState.Remove("frxPdfEmbeddingFonts");
            }
        }

        /// <summary>
        /// Enable or disable of exporting the background.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfBackground
        {
            get
            {
                return (this.ViewState["frxPdfBackground"] != null) ?
                    (bool)this.ViewState["frxPdfBackground"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxPdfBackground"] = value;
                else
                    this.ViewState.Remove("frxPdfBackground");
            }
        }

        /// <summary>
        /// Enable or disable of optimization the images for printing. 
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfPrintOptimized
        {
            get
            {
                return (this.ViewState["frxPdfPrintOptimized"] != null) ?
                    (bool)this.ViewState["frxPdfPrintOptimized"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPdfPrintOptimized"] = value;
                else
                    this.ViewState.Remove("frxPdfPrintOptimized");
            }
        }

        /// <summary>
        /// Enable or disable of document's Outline.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfOutline
        {
            get
            {
                return (this.ViewState["frxPdfOutline"] != null) ?
                    (bool)this.ViewState["frxPdfOutline"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPdfOutline"] = value;
                else
                    this.ViewState.Remove("frxPdfOutline");
            }
        }

        /// <summary>
        /// Enable or disable of displaying document's title.
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfDisplayDocTitle
        {
            get
            {
                return (this.ViewState["frxPdfDisplayDocTitle"] != null) ?
                    (bool)this.ViewState["frxPdfDisplayDocTitle"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPdfDisplayDocTitle"] = value;
                else
                    this.ViewState.Remove("frxPdfDisplayDocTitle");
            }
        }

        /// <summary>
        /// Enable or disable hide the toolbar. 
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfHideToolbar
        {
            get
            {
                return (this.ViewState["frxPdfHideToolbar"] != null) ?
                    (bool)this.ViewState["frxPdfHideToolbar"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxPdfHideToolbar"] = value;
                else
                    this.ViewState.Remove("frxPdfHideToolbar");
            }
        }

        /// <summary>
        /// Enable or disable hide the menu's bar.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfHideMenubar
        {
            get
            {
                return (this.ViewState["frxPdfHideMenubar"] != null) ?
                    (bool)this.ViewState["frxPdfHideMenubar"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxPdfHideMenubar"] = value;
                else
                    this.ViewState.Remove("frxPdfHideMenubar");
            }
        }

        /// <summary>
        /// Enable or disable hide the Windows UI. 
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfHideWindowUI
        {
            get
            {
                return (this.ViewState["frxPdfHideWindowUI"] != null) ?
                    (bool)this.ViewState["frxPdfHideWindowUI"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxPdfHideWindowUI"] = value;
                else
                    this.ViewState.Remove("frxPdfHideWindowUI");
            }
        }

        /// <summary>
        /// Enable or disable of fitting the window. 
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfFitWindow
        {
            get
            {
                return (this.ViewState["frxPdfFitWindow"] != null) ?
                    (bool)this.ViewState["frxPdfFitWindow"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxPdfFitWindow"] = value;
                else
                    this.ViewState.Remove("frxPdfFitWindow");
            }
        }

        /// <summary>
        ///  Enable or disable centering the window.
        /// </summary>
        [DefaultValue(false)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfCenterWindow
        {
            get
            {
                return (this.ViewState["frxPdfCenterWindow"] != null) ?
                    (bool)this.ViewState["frxPdfCenterWindow"] : false;
            }
            set 
            {
                if (value)
                    this.ViewState["frxPdfCenterWindow"] = value;
                else
                    this.ViewState.Remove("frxPdfCenterWindow");
            }
        }

        /// <summary>
        /// Enable or disable of scaling the page for shrink to printable area. 
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfPrintScaling
        {
            get
            {
                return (this.ViewState["frxPdfPrintScaling"] != null) ?
                    (bool)this.ViewState["frxPdfPrintScaling"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPdfPrintScaling"] = value;
                else
                    this.ViewState.Remove("frxPdfPrintScaling");
            }
        }

        /// <summary>
        /// Title of the document. 
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfTitle
        {
            get
            {
                return (this.ViewState["frxPdfTitle"] != null) ?
                    (string)this.ViewState["frxPdfTitle"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxPdfTitle"] = value;
                else
                    this.ViewState.Remove("frxPdfTitle");
            }
        }

        /// <summary>
        /// Author of the document. 
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfAuthor
        {
            get
            {
                return (this.ViewState["frxPdfAuthor"] != null) ?
                    (string)this.ViewState["frxPdfAuthor"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxPdfAuthor"] = value;
                else
                    this.ViewState.Remove("frxPdfAuthor");
            }
        }

        /// <summary>
        ///  Subject of the document.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfSubject
        {
            get
            {
                return (this.ViewState["frxPdfSubject"] != null) ?
                    (string)this.ViewState["frxPdfSubject"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxPdfSubject"] = value;
                else
                    this.ViewState.Remove("frxPdfSubject");
            }
        }

        /// <summary>
        ///  Keywords of the document.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfKeywords
        {
            get
            {
                return (this.ViewState["frxPdfKeywords"] != null) ?
                    (string)this.ViewState["frxPdfKeywords"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxPdfKeywords"] = value;
                else
                    this.ViewState.Remove("frxPdfKeywords");
            }
        }

        /// <summary>
        /// Creator of the document.
        /// </summary>
        [DefaultValue("FastReport")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfCreator
        {
            get
            {
                return (this.ViewState["frxPdfCreator"] != null) ?
                    (string)this.ViewState["frxPdfCreator"] : "FastReport";
            }
            set 
            {
                if (value != "FastReport")
                    this.ViewState["frxPdfCreator"] = value;
                else
                    this.ViewState.Remove("frxPdfCreator");
            }
        }

        /// <summary>
        /// Producer of the document.
        /// </summary>
        [DefaultValue("FastReport.NET")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfProducer
        {
            get
            {
                return (this.ViewState["frxPdfProducer"] != null) ?
                    (string)this.ViewState["frxPdfProducer"] : "FastReport.NET";
            }
            set 
            {
                if (value != "FastReport.NET")
                    this.ViewState["frxPdfProducer"] = value;
                else
                    this.ViewState.Remove("frxPdfProducer");
            }
        }

        /// <summary>
        /// Sets the users password.
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfUserPassword
        {
            get
            {
                return (this.ViewState["frxPdfUserPassword"] != null) ?
                    (string)this.ViewState["frxPdfUserPassword"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxPdfUserPassword"] = value;
                else
                    this.ViewState.Remove("frxPdfUserPassword");
            }
        }

        /// <summary>
        /// Sets the owner password. 
        /// </summary>
        [DefaultValue("")]
        [Category("Pdf Format")]
        [Browsable(true)]
        public string PdfOwnerPassword
        {
            get
            {
                return (this.ViewState["frxPdfOwnerPassword"] != null) ?
                    (string)this.ViewState["frxPdfOwnerPassword"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxPdfOwnerPassword"] = value;
                else
                    this.ViewState.Remove("frxPdfOwnerPassword");
            }
        }

        /// <summary>
        /// Enable or disable printing in protected document. 
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfAllowPrint
        {
            get
            {
                return (this.ViewState["frxPdfAllowPrint"] != null) ?
                    (bool)this.ViewState["frxPdfAllowPrint"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPdfAllowPrint"] = value;
                else
                    this.ViewState.Remove("frxPdfAllowPrint");
            }
        }

        /// <summary>
        /// Enable or disable modifying in protected document. 
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfAllowModify
        {
            get
            {
                return (this.ViewState["frxPdfAllowModify"] != null) ?
                    (bool)this.ViewState["frxPdfAllowModify"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPdfAllowModify"] = value;
                else
                    this.ViewState.Remove("frxPdfAllowModify");
            }
        }

        /// <summary>
        /// Enable or disable copying in protected document. 
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfAllowCopy
        {
            get
            {
                return (this.ViewState["frxPdfAllowCopy"] != null) ?
                    (bool)this.ViewState["frxPdfAllowCopy"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPdfAllowCopy"] = value;
                else
                    this.ViewState.Remove("frxPdfAllowCopy");
            }
        }

        /// <summary>
        /// Enable or disable annotating in protected document. 
        /// </summary>
        [DefaultValue(true)]
        [Category("Pdf Format")]
        [Browsable(true)]
        public bool PdfAllowAnnotate
        {
            get
            {
                return (this.ViewState["frxPdfAllowAnnotate"] != null) ?
                    (bool)this.ViewState["frxPdfAllowAnnotate"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPdfAllowAnnotate"] = value;
                else
                    this.ViewState.Remove("frxPdfAllowAnnotate");
            }
        }
        #endregion PDF format

        #region CSV format
        /// <summary>
        /// Switch visibility of CSV (comma separated values) export in toolbar.
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowCsvExport
        {
            get
            {
                return (this.ViewState["frxShowCsvExport"] != null) ?
                    (bool)this.ViewState["frxShowCsvExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowCsvExport"] = value;
                else
                    this.ViewState.Remove("frxShowCsvExport");
            }
        }

        /// <summary>
        /// Gets or sets of cells separator. 
        /// </summary>
        [DefaultValue(";")]
        [Category("Csv Format")]
        [Browsable(true)]
        public string CsvSeparator
        {
            get
            {
                return (this.ViewState["frxShowCsvSeparator"] != null) ?
                    (string)this.ViewState["frxShowCsvSeparator"] : ";";
            }
            set
            {
                if (value != ";")
                    this.ViewState["frxShowCsvSeparator"] = value;
                else
                    this.ViewState.Remove("frxShowCsvSeparator");
            }
        }

        /// <summary>
        /// Enable or disable of exporting data without any header/group bands.
        /// </summary>
        [DefaultValue(false)]
        [Category("Csv Format")]
        [Browsable(true)]
        public bool CsvDataOnly
        {
            get
            {
                return (this.ViewState["frxCsvDataOnly"] != null) ?
                    (bool)this.ViewState["frxCsvDataOnly"] : false;
            }
            set
            {
                if (value)
                    this.ViewState["frxCsvDataOnly"] = value;
                else
                    this.ViewState.Remove("frxCsvDataOnly");
            }
        }
        #endregion CSV format

        #region Text format
        /// <summary>
        ///  Switch visibility of text (plain text) export in toolbar
        /// </summary>
        [DefaultValue(true)]
        [Category("Toolbar")]
        [Browsable(true)]
        public bool ShowTextExport
        {
            get
            {
                return (this.ViewState["frxShowTextExport"] != null) ?
                    (bool)this.ViewState["frxShowTextExport"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxShowTextExport"] = value;
                else
                    this.ViewState.Remove("frxShowTextExport");
            }
        }

        /// <summary>
        /// Enable or disable of exporting data without any header/group bands.
        /// </summary>
        [DefaultValue(false)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextDataOnly
        {
            get
            {
                return (this.ViewState["frxTextDataOnly"] != null) ?
                    (bool)this.ViewState["frxTextDataOnly"] : false;
            }
            set
            {
                if (value)
                    this.ViewState["frxTextDataOnly"] = value;
                else
                    this.ViewState.Remove("frxTextDataOnly");
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating that page breaks are enabled.
        /// </summary>
        [DefaultValue(true)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextPageBreaks
        {
            get
            {
                return (this.ViewState["frxTextPageBreaks"] != null) ?
                    (bool)this.ViewState["frxTextPageBreaks"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxTextPageBreaks"] = value;
                else
                    this.ViewState.Remove("frxTextPageBreaks");
            }
        }

        /// <summary>
        /// Enable or disable frames in text file.
        /// </summary>
        [DefaultValue(true)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextAllowFrames
        {
            get
            {
                return (this.ViewState["frxTextAllowFrames"] != null) ?
                    (bool)this.ViewState["frxTextAllowFrames"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxTextAllowFrames"] = value;
                else
                    this.ViewState.Remove("frxTextAllowFrames");
            }
        }

        /// <summary>
        /// Enable or disable simple (non graphic) frames in text file.
        /// </summary>
        [DefaultValue(true)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextSimpleFrames
        {
            get
            {
                return (this.ViewState["frxTextSimpleFrames"] != null) ?
                    (bool)this.ViewState["frxTextSimpleFrames"] : true;
            }
            set
            {
                if (!value)
                    this.ViewState["frxTextSimpleFrames"] = value;
                else
                    this.ViewState.Remove("frxTextSimpleFrames");
            }
        }

        /// <summary>
        /// Enable or disable empty lines in text file.
        /// </summary>
        [DefaultValue(false)]
        [Category("Text Format")]
        [Browsable(true)]
        public bool TextEmptyLines
        {
            get
            {
                return (this.ViewState["frxTextEmptyLines"] != null) ?
                    (bool)this.ViewState["frxTextEmptyLines"] : false;
            }
            set
            {
                if (value)
                    this.ViewState["frxTextEmptyLines"] = value;
                else
                    this.ViewState.Remove("frxTextEmptyLines");
            }
        }
        #endregion Text export

        /// <summary>
        /// Set the Toolbar color.
        /// </summary>
        [DefaultValue(0xECE9D8)]
        [Category("Toolbar")]
        [Browsable(true)]
        public System.Drawing.Color ToolbarColor
        {
            get
            {
                return (this.ViewState["frxToolbarColor"] != null) ?
              (System.Drawing.Color)this.ViewState["frxToolbarColor"] : Color.FromArgb(0xECE9D8);
            }
            set 
            { 
                this.ViewState["frxToolbarColor"] = value; 
            }
        }

        /// <summary>
        /// Switch the pictures visibility in report
        /// </summary>
        [DefaultValue(true)]
        [Category("Report")]
        [Browsable(true)]
        public bool Pictures
        {
            get
            {
                return (this.ViewState["frxPictures"] != null) ?
              (bool)this.ViewState["frxPictures"] : true;
            }
            set 
            {
                if (!value)
                    this.ViewState["frxPictures"] = value;
                else
                    this.ViewState.Remove("frxPictures");
            }
        }

        /// <summary>
        /// Gets or sets the name of report file.
        /// </summary>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        [Editor("FastReport.VSDesign.ReportFileEditor, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL", typeof(UITypeEditor))]
        public string ReportFile
        {
            get
            {
                return (this.ViewState["frxReportFile"] != null) ?
                    (string)this.ViewState["frxReportFile"] : String.Empty;
            }
            set 
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxReportFile"] = value;
                else
                    this.ViewState.Remove("frxReportFile");
            }
        }

        /// <summary>
        /// Gets or sets the name of localization file.
        /// </summary>
        [DefaultValue("")]
        [Category("Report")]
        [Browsable(true)]
        [Editor("FastReport.VSDesign.LocalizationFileEditor, FastReport.VSDesign, Version=1.0.0.0, Culture=neutral, PublicKeyToken=db7e5ce63278458c, processorArchitecture=MSIL", typeof(UITypeEditor))]
        public string LocalizationFile
        {
            get
            {
                return (this.ViewState["frxLocalizationFile"] != null) ?
                  (string)this.ViewState["frxLocalizationFile"] : String.Empty;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    this.ViewState["frxLocalizationFile"] = value;
                else
                    this.ViewState.Remove("frxLocalizationFile");
            }
        }

        /// <summary>
        /// Set the zoom factor of previewed page between 0..1
        /// </summary>
        [DefaultValue(1f)]
        [Category("Report")]
        [Browsable(true)]
        public float Zoom
        {
            get
            {
                return (this.ViewState["frxZoom"] != null) ?
              (float)this.ViewState["frxZoom"] : 1f;
            }
            set 
            { 
                this.ViewState["frxZoom"] = value; 
            }
        }

        /// <summary>
        /// Direct access to Report object
        /// </summary>
        [Browsable(false)]
        public Report Report
        {
            get 
            { 
                return (Report)CacheGet("frx" + ReportGuid); 
            }
            set 
            {
                CacheRemove("frx" + ReportGuid);
                CacheAdd("frx" + ReportGuid, value, //null,
                new CacheItemRemovedCallback(this.RemovedCallback), 
                CacheDelay); 
            } 
        }

        /// <summary>
        /// Direct access to HTML export engine
        /// </summary>
        [Browsable(false)]
        public HTMLExport HTMLExport
        {
            get 
            { 
                return (HTMLExport)CacheGet("frxHTML" + ReportGuid); 
            }
            set 
            { 
                CacheAdd("frxHTML" + ReportGuid, value, null, CacheDelay); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public int TotalPages
        {
            get 
            { 
                return (HTMLExport != null) ? HTMLExport.Count : 0; 
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public bool ReportDone
        {
            get
            {
                return (this.ViewState["frxReportDone"] != null) ?
              (bool)this.ViewState["frxReportDone"] : false;
            }
            set 
            { 
                this.ViewState["frxReportDone"] = value; 
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when report execution is started.
        /// </summary>
        [Browsable(true)]
        public event EventHandler StartReport;
        #endregion

        #region Internal properties

        internal bool HTMLDone
        {
            get
            {
                return (this.ViewState["frxHTMLDone"] != null) ?
              (bool)this.ViewState["frxHTMLDone"] : false;
            }
            set 
            { 
                this.ViewState["frxHTMLDone"] = value; 
            }
        }

        internal int CurrentPage
        {
            get 
            { 
                return (this.ViewState["frxCurrentPage"] != null) ? (int)this.ViewState["frxCurrentPage"] : 0; 
            }
            set 
            { 
                this.ViewState["frxCurrentPage"] = value; 
            }
        }

        internal string ReportGuid
        {
            get
            {
                string s = (string)this.ViewState["frxReportGuid"];
                if (s == null)
                {
                    s = Guid.NewGuid().ToString();
                    this.ViewState["frxReportGuid"] = s;
                }
                return s;
            }
        }
        #endregion


        private void ClearExportCache(string guid)
        {
            if (CacheGet("frxExport" + guid) != null)
                CacheRemove("frxExport" + guid);
            if (!ReportDone)
                PrepareReport();
        }

        private string GetExportFileName(string format)
        {
            return Path.GetFileNameWithoutExtension(Report.FileName.Length == 0 ? "report" : Report.FileName) + "." + format;
        }

        private string GetReponseRedirect(string guid)
        {
            return this.Page.Request.CurrentExecutionFilePath + "?" + FExportPrefix + "=" + guid;
        }

        private void ResponseExport(string guid, WebExportItem ExportItem)
        {
            ExportItem.FileName = GetExportFileName(ExportItem.Format);
            CacheAdd("frxExport" + guid, ExportItem, null, 5);
            this.Page.Response.Redirect(GetReponseRedirect(guid));
            this.Page.Response.End();
        }

        /// <summary>
        /// Export in CSV format
        /// </summary>
        public void ExportCsv()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                CSVExport csvExport = new CSVExport();
                csvExport.OpenAfterExport = false;
                // set csv export properties
                csvExport.Separator = CsvSeparator;
                csvExport.DataOnly = CsvDataOnly;
                csvExport.Export(Report, ExportItem.File);
                ExportItem.Format = "csv";
                ResponseExport(guid, ExportItem);
            }
        }

        /// <summary>
        /// Export in Text format
        /// </summary>
        public void ExportText()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                TextExport textExport = new TextExport();
                textExport.OpenAfterExport = false;
                // set text export properties
                textExport.AvoidDataLoss = true;
                textExport.DataOnly = TextDataOnly;
                textExport.PageBreaks = TextPageBreaks;
                textExport.Frames = TextAllowFrames;
                textExport.TextFrames = TextSimpleFrames;
                textExport.EmptyLines = TextEmptyLines;
                textExport.Export(Report, ExportItem.File);
                ExportItem.Format = "txt";
                ResponseExport(guid, ExportItem);
            }
        }

        /// <summary>
        /// Export in PDF format
        /// </summary>
        public void ExportPdf()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                PDFExport pdfExport = new PDFExport();
                pdfExport.OpenAfterExport = false;
                // set pdf export properties
                pdfExport.EmbeddingFonts = PdfEmbeddingFonts;
                pdfExport.Background = PdfBackground;
                pdfExport.PrintOptimized = PdfPrintOptimized;
                pdfExport.Title = PdfTitle;
                pdfExport.Author = PdfAuthor;
                pdfExport.Subject = PdfSubject;
                pdfExport.Keywords = PdfKeywords;
                pdfExport.Creator = PdfCreator;
                pdfExport.Producer = PdfProducer;
                pdfExport.Outline = PdfOutline;
                pdfExport.DisplayDocTitle = PdfDisplayDocTitle;
                pdfExport.HideToolbar = PdfHideToolbar;
                pdfExport.HideMenubar = PdfHideMenubar;
                pdfExport.HideWindowUI = PdfHideWindowUI;
                pdfExport.FitWindow = PdfFitWindow;
                pdfExport.CenterWindow = PdfCenterWindow;
                pdfExport.PrintScaling = PdfPrintScaling;
                pdfExport.UserPassword = PdfUserPassword;
                pdfExport.OwnerPassword = PdfOwnerPassword;
                pdfExport.AllowPrint = PdfAllowPrint;
                pdfExport.AllowCopy = PdfAllowCopy;
                pdfExport.AllowModify = PdfAllowModify;
                pdfExport.AllowAnnotate = PdfAllowAnnotate;
                pdfExport.Export(Report, ExportItem.File);
                ExportItem.Format = "pdf";
                ResponseExport(guid, ExportItem);
            }
        }

        /// <summary>
        /// Export in RTF format
        /// </summary>
        public void ExportRtf()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                RTFExport rtfExport = new RTFExport();
                rtfExport.OpenAfterExport = false;
                // set Rtf export properties
                rtfExport.JpegQuality = RtfJpegQuality;
                rtfExport.ImageFormat = RtfImageFormat;
                rtfExport.Pictures = RtfPictures;
                rtfExport.PageBreaks = RtfPageBreaks;
                rtfExport.Wysiwyg = RtfWysiwyg;
                rtfExport.Creator = RtfCreator;
                rtfExport.AutoSize = RtfAutoSize;
                rtfExport.Export(Report, ExportItem.File);
                ExportItem.Format = "rtf";
                ResponseExport(guid, ExportItem);
            }
        }

        /// <summary>
        /// Export in MHT format
        /// </summary>
        public void ExportMht()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                MHTExport mhtExport = new MHTExport();
                mhtExport.OpenAfterExport = false;
                // set MHT export properties
                mhtExport.Pictures = MhtPictures;
                mhtExport.Wysiwyg = MhtWysiwyg;
                mhtExport.Export(Report, ExportItem.File);
                ExportItem.Format = "mht";
                ResponseExport(guid, ExportItem);
            }
        }

        /// <summary>
        /// Export in XML (Excel 2003) format
        /// </summary>
        public void ExportXmlExcel()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                XMLExport xmlExport = new XMLExport();
                xmlExport.OpenAfterExport = false;
                // set xml export properties
                xmlExport.PageBreaks = XmlExcelPageBreaks;
                xmlExport.Wysiwyg = XmlExcelWysiwyg;
                xmlExport.Export(Report, ExportItem.File);
                ExportItem.Format = "xls";
                ResponseExport(guid, ExportItem);
            }
        }

        /// <summary>
        /// Export in Open Office Spreadsheet format
        /// </summary>
        public void ExportOds()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                ODSExport odsExport = new ODSExport();
                odsExport.OpenAfterExport = false;
                // set ODS export properties
                odsExport.Creator = OdsCreator;
                odsExport.Wysiwyg = OdsWysiwyg;
                odsExport.PageBreaks = OdsPageBreaks;
                odsExport.Export(Report, ExportItem.File);
                ExportItem.Format = "ods";
                ResponseExport(guid, ExportItem);
            }
        }

        /// <summary>
        /// Export in Excel 2007 format
        /// </summary>
        public void ExportExcel2007()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                Excel2007Export xlsxExport = new Excel2007Export();
                xlsxExport.OpenAfterExport = false;
                // set Excel 2007 export properties
                xlsxExport.PageBreaks = XlsxPageBreaks;
                XlsxWysiwyg = XlsxWysiwyg;
                xlsxExport.Export(Report, ExportItem.File);
                ExportItem.Format = "xlsx";
                ResponseExport(guid, ExportItem);
            }
        }

        /// <summary>
        /// Export in PowerPoint 2007 format 
        /// </summary>
        public void ExportPowerPoint2007()
        {
            string guid = ReportGuid;
            ClearExportCache(guid);
            if (ReportDone && TotalPages > 0)
            {
                WebExportItem ExportItem = new WebExportItem();
                PowerPoint2007Export pptxExport = new PowerPoint2007Export();
                pptxExport.OpenAfterExport = false;
                // set Power Point 2007 properties
                pptxExport.ImageFormat = PptxImageFormat;
                pptxExport.Export(Report, ExportItem.File);
                ExportItem.Format = "pptx";
                ResponseExport(guid, ExportItem);
            }
        }

        //private void Redirect(string url, string target, string windowFeatures)
        //{
        //    HttpContext context = HttpContext.Current;
        //    if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) &&
        //        String.IsNullOrEmpty(windowFeatures))
        //    {
        //        context.Response.Redirect(url);
        //    }
        //    else
        //    {
        //        Page page = (Page)HttpContext.Current.Handler;
        //        if (page != null)
        //        {
        //            url = page.ResolveClientUrl(url);
        //            string script;
        //            if (!String.IsNullOrEmpty(windowFeatures))
        //                script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
        //            else
        //                script = @"window.open(""{0}"", ""{1}"");";
        //            script = String.Format(script, url, target, windowFeatures);
        //            page.ClientScript.RegisterStartupScript(this.GetType(), "Startup", script, true);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Print in browser
        ///// </summary>
        //public void PrintBrowser()
        //{
        //    string guid = ReportGuid;
        //    ClearExportCache(guid);
        //    if (ReportDone && TotalPages > 0)
        //    {
        //        WebExportItem ExportItem = new WebExportItem();
        //        HTMLExport html = new HTMLExport();
        //        html.SinglePage = true;
        //        // !!!! fix!!!
        //        html.Pictures = false;
        //        html.Print = true;
        //        html.Export(Report, ExportItem.File);
        //        ExportItem.Format = "html";

        //        ExportItem.FileName = GetExportFileName(ExportItem.Format);
        //        CacheAdd("frxExport" + guid, ExportItem, null, 5);
                
        //        Redirect(GetReponseRedirect(guid), "mywindow", "location=1,status=1,scrollbars=1,width=385,height=200");
                
        //        //this.Page.Response.Redirect(GetReponseRedirect(guid));
        //        //this.Page.Response.End();
        //    }
        //}

        /// <summary>
        /// Print in Adobe Acrobat
        /// </summary>
        public void PrintPdf()
        {
            ExportPdf();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void OnStartReport(EventArgs e)
        {
            if (StartReport != null)
                StartReport(this, e);
        }

        /// <summary>
        /// Force go to next report page
        /// </summary>
        public void NextPage()
        {
            if (CurrentPage < TotalPages - 1)
                CurrentPage++;
        }

        /// <summary>
        /// Force go to previous report page
        /// </summary>
        public void PrevPage()
        {
            if (CurrentPage > 0)
                CurrentPage--;
        }

        /// <summary>
        /// Force go to first report page
        /// </summary>
        public void FirstPage()
        {
            CurrentPage = 0;
        }

        /// <summary>
        /// Force go to last report page
        /// </summary>
        public void LastPage()
        {
            CurrentPage = TotalPages - 1;
        }

        /// <summary>
        /// Force go to "value" report page
        /// </summary>
        public void SetPage(int value)
        {
            if (value >= 0 && value < TotalPages)
                CurrentPage = value;
        }

        /// <summary>
        /// Prepare the report
        /// </summary>
        public void Prepare()
        {
            Refresh();
        }

        /// <summary>
        /// Force refresh of report
        /// </summary>
        public void Refresh()
        {
            CurrentPage = 0;
            ReportDone = false;
            PrepareReport();
        }        

        #region Protected methods

        private void RemovedCallback(String k, Object v, CacheItemRemovedReason r)
        {
            if (v is Report)
            {
                if (((Report)v).PreparedPages != null) 
                    ((Report)v).PreparedPages.Clear();
                ((Report)v).Dispose();
            }
        }

        private Control FindControlRecursive(Control root, string id)
        {
          if (root.ID == id)
            return root;

          foreach (Control ctl in root.Controls)
          {
            Control foundCtl = FindControlRecursive(ctl, id);
            if (foundCtl != null)
              return foundCtl;
          }

          return null;
        }
        
        internal void RegisterData(Report report)
        {
          string[] dataSources = ReportDataSources.Split(new char[] { ';' });
          foreach (string dataSource in dataSources)
          {
            IDataSource ds = FindControlRecursive(Page, dataSource) as IDataSource;
            if (ds == null)
              continue;
            string dataName = (ds as Control).ID;
            RegisterDataAsp(report, ds, dataName);
          }
        }


        /// <summary>
        /// Registers the ASP.NET application data to use it in the report.
        /// </summary>
        /// <param name="report">The <b>Report</b> object.</param>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        public void RegisterDataAsp(Report report, IDataSource data, string name)
        {
          FAspDataName = name;
          FReport = report;
          DataSourceView view = data.GetView("");
          if (view != null)
            view.Select(DataSourceSelectArguments.Empty, new DataSourceViewSelectCallback(RegisterDataAsp));
        }

        private string FAspDataName;
        private Report FReport;
        private void RegisterDataAsp(IEnumerable data)
        {
          if (data != null)
            RegisterDataAsp(FReport, data, FAspDataName);
        }

        /// <summary>
        /// Registers the ASP.NET application data to use it in the report.
        /// </summary>
        /// <param name="report">The <b>Report</b> object.</param>
        /// <param name="data">The application data.</param>
        /// <param name="name">The name of the data.</param>
        public void RegisterDataAsp(Report report, IEnumerable data, string name)
        {
          DataComponentBase existingDs = report.Dictionary.FindDataComponent(name);
          if (existingDs is ViewDataSource && data is DataView)
          {
            // compatibility with old FR versions (earlier than 1.1.45)
            report.Dictionary.RegisterDataView(data as DataView, name, true);
          }
          else
          {
            // in a new versions, always register the business object
            report.Dictionary.RegisterBusinessObject(data, name, 1, true);
          }
        }

        private void PrepareReport()
        {
            if (!ReportDone)
            {
                Config.WebMode = true;
                if (Report == null)
                    Report = new Report();
                else
                    Report.Clear();

                if (!String.IsNullOrEmpty(ReportFile))
                {
                    string fileName = ReportFile;
                    if (!WebUtils.IsAbsolutePhysicalPath(fileName))
                        fileName = this.Context.Request.MapPath(fileName, base.AppRelativeTemplateSourceDirectory, true);
                    Report.Load(fileName);
                }
                else if (!String.IsNullOrEmpty(ReportResourceString))
                    Report.ReportResourceString = ReportResourceString;

                RegisterData(Report);
                OnStartReport(EventArgs.Empty);
                Config.ReportSettings.ShowProgress = false;
                //// usefilecache 
                if (!ReportDone)
                  ReportDone = Report.Prepare(false);
                HTMLDone = false;
            }
            if (!HTMLDone && ReportDone)
            {
                HTMLExport = new HTMLExport();
                HTMLExport.StylePrefix = this.ID;
                HTMLExport.Init_WebMode();
                HTMLExport.Pictures = Pictures;
                HTMLExport.Zoom = Zoom;               
                
                if (AutoWidth)
                    HTMLExport.WidthUnits = HtmlSizeUnits.Percent;
                if (AutoHeight)
                    HTMLExport.HeightUnits = HtmlSizeUnits.Percent;

                HTMLExport.WebImagePrefix = this.Page.Request.CurrentExecutionFilePath + "?" + FPicsPrefix;
                HTMLExport.Export(Report, (Stream)null);                
                HTMLDone = true;
            }
        }

        private bool SendPicture()
        {
            string prefix = "";
            if (this.Page.Request.Params[FPicsPrefix] != null)
            {
                prefix = Convert.ToString(this.Page.Request.Params[FPicsPrefix]);
                this.Page.Response.ContentType = "image/png";
            }
            else if (this.Page.Request.Params[FBtnPrefix] != null)
            {
                prefix = Convert.ToString(this.Page.Request.Params[FBtnPrefix]);
                this.Page.Response.ContentType = "image/gif";
            }

            if (!String.IsNullOrEmpty(prefix))
            {
                byte[] image= (byte[])CacheGet(prefix);
                if (image != null)    
                    this.Page.Response.BinaryWrite(image);
                return true;
            }
            else
                return false;
        }

        private bool SendExportFile()
        {            
            if (this.Page.Request.Params[FExportPrefix] != null)
            {
                string guid = this.Page.Request.Params[FExportPrefix];
                WebExportItem ExportItem = (WebExportItem)CacheGet("frxExport" + guid);
                if (ExportItem != null)
                {
                    if (ExportItem.Format == "html")
                    {
                        this.Page.Response.ContentType = "text/html";
                        this.Page.Response.AppendHeader("Content-Disposition",
                            "inline; filename=\"" + ExportItem.FileName + "\"");
                    }
                    else
                    {
                        this.Page.Response.ContentType = "application/" + ExportItem.Format;
                        this.Page.Response.AppendHeader("Content-Disposition",
                            "attachment; filename=\"" + ExportItem.FileName + "\"");
                    }
                    byte[] buff = ExportItem.File.ToArray();
                    if (buff.Length > 0)
                        this.Page.Response.BinaryWrite(buff);
                    return true;
                }
            }            
            return false;
        }

        private void SendReportPage()
        {
            if (ShowToolbar)
            {
                string zoomvalue = Math.Round(Zoom * 100).ToString();
                for (int i = 0; i < cbbZoom.Items.Count; i++)
                    if (cbbZoom.Items[i].Value == zoomvalue)
                    {
                        cbbZoom.SelectedIndex = i;
                        break;
                    }
                tbPage.Text = (CurrentPage + 1).ToString();
                lblPages.Text = String.Format(Res.Get("Misc,ofM"), TotalPages.ToString());
            }
            if ((HTMLExport != null) && (HTMLExport.Count > 0))
            {
                if (HTMLExport.PreparedPages[CurrentPage].PageText == null)
                    HTMLExport.ProcessPage(CurrentPage, CurrentPage);

                if (HTMLExport.PreparedPages[CurrentPage].CSSText != null
                    && HTMLExport.PreparedPages[CurrentPage].PageText != null)
                {
                    div.InnerHtml = HTMLExport.PreparedPages[CurrentPage].CSSText + HTMLExport.PreparedPages[CurrentPage].PageText;
                    for (int i = 0; i < HTMLExport.PreparedPages[CurrentPage].Pictures.Count; i++)
                    {
                        Stream stream = HTMLExport.PreparedPages[CurrentPage].Pictures[i];
                        byte[] image = new byte[stream.Length];
                        stream.Position = 0;
                        int n = stream.Read(image, 0, (int)stream.Length);
                        CacheRemove(HTMLExport.PreparedPages[CurrentPage].Guids[i]);
                        CacheAdd(HTMLExport.PreparedPages[CurrentPage].Guids[i], image, null, 3);
                    }
                }
            }                        
        }

        /// <inheritdoc/>
        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool result = true;            
            if (args is CommandEventArgs)
            {
                CommandEventArgs c = (CommandEventArgs)args;
                if (c.CommandName == btnNext.CommandName)
                    NextPage();
                else if (c.CommandName == btnFirst.CommandName)
                    FirstPage();
                else if (c.CommandName == btnPrev.CommandName)
                    PrevPage();
                else if (c.CommandName == btnLast.CommandName)
                    LastPage();
                else
                    result = false;
            }
            else
                result = false;
            return result;
        }

        /// <inheritdoc/>
        protected override void OnLoad(EventArgs e)        
        {
            this.EnsureChildControls();
            base.OnLoad(e);
            if (HttpContext.Current != null)
            {
                if (!this.Page.IsPostBack)
                    PrepareReport();
            }
        }

        /// <inheritdoc/>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Config.WebMode = true;

            if (HttpContext.Current != null)
            {
                if (SendPicture() || SendExportFile())
                    this.Page.Response.End();
            }

            if (!String.IsNullOrEmpty(LocalizationFile))
            {
              string fileName = LocalizationFile;
              if (!WebUtils.IsAbsolutePhysicalPath(fileName))
                fileName = this.Context.Request.MapPath(fileName, base.AppRelativeTemplateSourceDirectory, true);
              Res.LoadLocale(fileName);
            }
        }

        /// <inheritdoc/>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            CreateNavigatorControls();
            base.CreateChildControls();
        }

        /// <inheritdoc/>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (HttpContext.Current == null)            
                RenderDesignModeNavigatorControls(writer);
            else
            {
                SetEnableButtons(); 
                SendReportPage();
                base.RenderContents(writer);
            }            
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public WebReport()
        {
            this.Width = Unit.Pixel(550);
            this.Height = Unit.Pixel(500);
            this.ForeColor = Color.Black;
            this.BackColor = Color.White;
        }
    }
}
