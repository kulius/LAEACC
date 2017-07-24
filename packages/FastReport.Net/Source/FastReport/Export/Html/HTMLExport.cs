using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Export;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Globalization;

namespace FastReport.Export.Html
{
    /// <summary>
    /// Specifies the image format in HTML export.
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// Specifies the .bmp format.
        /// </summary>
        Bmp,

        /// <summary>
        /// Specifies the .png format.
        /// </summary>
        Png,

        /// <summary>
        /// Specifies the .jpg format.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Specifies the .gif format.
        /// </summary>
        Gif
    }

    /// <summary>
    /// Specifies the units of HTML sizes.
    /// </summary>
    public enum HtmlSizeUnits
    {
        /// <summary>
        /// Specifies the pixel units.
        /// </summary>
        Pixel,
        /// <summary>
        /// Specifies the percent units.
        /// </summary>
        Percent
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    public class HTMLPageData
    {
        private string FCSSText;
        private string FPageText;
        private List<Stream> FPictures;
        private List<string> FGuids;
        private ManualResetEvent FPageEvent;
        private int FPageNumber;
        private float FWidth;
        private float FHeight;

        /// <summary>
        /// For internal use only.
        /// </summary>
        public float Width
        {
            get { return FWidth; }
            set { FWidth = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public float Height
        {
            get { return FHeight; }
            set { FHeight = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string CSSText
        {
            get { return FCSSText; }
            set { FCSSText = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string PageText
        {
            get { return FPageText; }
            set { FPageText = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public List<Stream> Pictures
        {
            get { return FPictures; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public List<string> Guids
        {
            get { return FGuids; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public ManualResetEvent PageEvent
        {
            get { return FPageEvent; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public int PageNumber
        {
            get { return FPageNumber; }
            set { FPageNumber = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public HTMLPageData()
        {
            FPictures = new List<Stream>();
            FGuids = new List<string>();
            FPageEvent = new ManualResetEvent(false);
        }
    }

    /// <summary>
    /// Represents the HTML export format enum
    /// </summary>
    public enum HTMLExportFormat
    {
        /// <summary>
        /// Represents the message-HTML type
        /// </summary>
        MessageHTML,
        /// <summary>
        /// Represents the HTML type
        /// </summary>
        HTML
    }

    /// <summary>
    /// Represents the HTML export filter.
    /// </summary>
    public partial class HTMLExport : ExportBase
    {

        #region Private fields

        private struct HTMLThreadData
        {
            public int ReportPage;
            public int PageNumber;
            public int CurrentPage;
            public Stream PagesStream; 
        }

        private struct PicsArchiveItem
        {
            public string FileName;
            public MemoryStream Stream;
        }

        private bool FLayers;
        private bool FWysiwyg;
        private MyRes Res;
        private HtmlTemplates FTemplates;
        private string FTargetPath;
        private string FTargetIndexPath;
        private string FTargetFileName;
        private string FFileName;
        private string FNavFileName;
        private string FOutlineFileName;
        private int FPagesCount;
        private string FDocumentTitle;
        private ImageFormat FImageFormat;
        private ManualResetEvent FFirstPageEvent;
        private bool FSubFolder;
        private bool FNavigator;
        private bool FSinglePage;
        private bool FPictures;
        private bool FWebMode;
        private List<HTMLPageData> FPages;
        private int FCount;
        private string FWebImagePrefix;
        private string FStylePrefix;
        private bool FThreaded;
        private string FPrevWatermarkName;
        private long FPrevWatermarkSize;
        private HtmlSizeUnits FWidthUnits;
        private HtmlSizeUnits FHeightUnits;
        private string FSinglePageFileName;
        private string FSubFolderPath;
        private HTMLExportFormat FFormat;
        private MemoryStream FMimeStream;
        private String FBoundary;
        private List<PicsArchiveItem> FPicsArchive;
        private List<ExportIEMStyle> FPrevStyleList;
        private int FPrevStyleListIndex;
        private bool FPageBreaks;
        private bool FPrint;
        private List<string> FCSSStyles;
        private float FHPos;
        private NumberFormatInfo FNumberFormat;

        private const string BODY_BEGIN = "</head>\r\n<body bgcolor=\"#FFFFFF\" text=\"#000000\">";
        private const string BODY_END = "</body>";
        private const string PRINT_JS = "<script language=\"javascript\" type=\"text/javascript\"> parent.focus(); parent.print();</script>";

        private const string NBSP = "&nbsp;";
        #endregion

        #region Public properties

        /// <summary>
        /// Enable or disable layers export mode
        /// </summary>
        public bool Layers
        {
            get { return FLayers; }
            set { FLayers = value; }
        }        

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string StylePrefix
        {
            get { return FStylePrefix; }
            set { FStylePrefix = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string WebImagePrefix
        {
            get { return FWebImagePrefix; }
            set { FWebImagePrefix = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public int Count
        {
            get { return FCount; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public List<HTMLPageData> PreparedPages
        {
            get { return FPages; }            
        }

        /// <summary>
        /// Enable or disable showing of print dialog in browser when html document is opened
        /// </summary>
        public bool Print
        {
            get { return FPrint; }
            set { FPrint = value; }
        }

        /// <summary>
        /// Enable or disable the breaks between pages in print preview when single page mode is enabled
        /// </summary>
        public bool PageBreaks
        {
            get { return FPageBreaks; }
            set { FPageBreaks = value; }
        }

        /// <summary>
        /// Specifies the output format
        /// </summary>
        public HTMLExportFormat Format
        {
            get { return FFormat; }
            set { FFormat = value; }                
        }

        /// <summary>
        /// Specifies the width units in HTML export
        /// </summary>
        public HtmlSizeUnits WidthUnits
        {
            get { return FWidthUnits; }
            set { FWidthUnits = value; }
        }

        /// <summary>
        /// Specifies the height units in HTML export
        /// </summary>
        public HtmlSizeUnits HeightUnits
        {
            get { return FHeightUnits; }
            set { FHeightUnits = value; }
        }

        /// <summary>
        /// Enable or disable the pictures in HTML export
        /// </summary>
        public bool Pictures
        {
            get { return FPictures; }
            set { FPictures = value; }
        }

        /// <summary>
        /// Enable or disable the WEB mode in HTML export
        /// </summary>
        internal bool WebMode
        {
            get { return FWebMode; }
            set { FWebMode = value; }
        }

        /// <summary>
        /// Enable or disable the single HTML page creation 
        /// </summary>
        public bool SinglePage
        {
            get { return FSinglePage; }
            set { FSinglePage = value; }
        }

        /// <summary>
        /// Enable or disable the page navigator in html export
        /// </summary>
        public bool Navigator
        {
            get { return FNavigator; }
            set { FNavigator = value; }
        }

        /// <summary>
        /// Enable or disable the sub-folder for files of export
        /// </summary>
        public bool SubFolder
        {
            get { return FSubFolder;  }
            set { FSubFolder = value; }
        }

        /// <summary>
        ///  Gets or sets the Wysiwyg quality of export
        /// </summary>
        public bool Wysiwyg
        {
            get { return FWysiwyg; }
            set { FWysiwyg = value; }
        }

        /// <summary>
        /// Gets or sets the image format.
        /// </summary>
        public ImageFormat ImageFormat
        {
            get { return FImageFormat; }
            set { FImageFormat = value; }
        }

        #endregion


        #region Private methods

        private string Px(double pixel)
        {
            return String.Join(String.Empty, new String[] { Convert.ToString(Math.Round(pixel, 2), FNumberFormat), "px;" });
            //return String.Join(String.Empty, new String[] { Convert.ToString(Math.Round(pixel, 2), FNumberFormat), ";" });
        }

        private string SizeValue(double value, double maxvalue, HtmlSizeUnits units)
        {
            StringBuilder sb = new StringBuilder(6);
            if (units == HtmlSizeUnits.Pixel)
                sb.Append(Px(value));
            else if (units == HtmlSizeUnits.Percent)
                sb.Append(((int)Math.Round((value * 100 / maxvalue))).ToString()).Append("%");
            else
                sb.Append(value.ToString());
            return sb.ToString();
        }

        private void HTMLFontStyle(StringBuilder FFontDesc, Font font)
        {
            FFontDesc.Append((((font.Style & FontStyle.Bold) > 0) ? "font-weight:bold;" : String.Empty) +
                (((font.Style & FontStyle.Italic) > 0) ? "font-style:italic;" : "font-style:normal;"));
            if ((font.Style & FontStyle.Underline) > 0 && (font.Style & FontStyle.Strikeout) > 0)
                FFontDesc.Append("text-decoration:underline|line-through;");
            else if ((font.Style & FontStyle.Underline) > 0)
                FFontDesc.Append("text-decoration:underline;");
            else if ((font.Style & FontStyle.Strikeout) > 0)
                FFontDesc.Append("text-decoration:line-through;");
            FFontDesc.Append("font-family:").Append(font.Name).Append(";");
            FFontDesc.Append("font-size:").Append(Px(Math.Round(font.Size * 96 / 72)));          
        }

        private void HTMLPadding(StringBuilder PaddingDesc, Padding padding)
        {
            if (padding.Left != 0)
                PaddingDesc.Append("padding-left:").Append(Px(padding.Left));
            if (padding.Right != 0)
                PaddingDesc.Append("padding-right:").Append(Px(padding.Right));
            if (padding.Top != 0)
                PaddingDesc.Append("padding-top:").Append(Px(padding.Top));
            if (padding.Bottom != 0)
                PaddingDesc.Append("padding-bottom:").Append(Px(padding.Bottom));
        }

        private string HTMLBorderStyle(BorderLine line)
        {            
            switch (line.Style)
            {
                case LineStyle.Dash:
                case LineStyle.DashDot:
                case LineStyle.DashDotDot:
                    return "dashed";
                case LineStyle.Dot:
                    return "dotted";
                case LineStyle.Double:
                    return "double";
                default:
                    return "solid";
            }
        }

        private float HTMLBorderWidth(BorderLine line)
        {
            if (line.Style == LineStyle.Double)
                return (line.Width * 3 * Zoom);
            else
                return line.Width * Zoom;
        }

        private void HTMLBorder(StringBuilder BorderDesc, Border border)
        {
            if (border.Lines > 0)
            {
                // bottom
                if ((border.Lines & BorderLines.Bottom) > 0)
                    BorderDesc.Append("border-bottom-width:").
                        Append(Px(HTMLBorderWidth(border.BottomLine))).
                        Append("border-bottom-color:").
                        Append(ExportUtils.HTMLColor(border.BottomLine.Color)).Append(";").
                        Append("border-bottom-style:").Append(HTMLBorderStyle(border.BottomLine)).Append(";");
                else
                    BorderDesc.Append("border-bottom-width:0;");
                // top
                if ((border.Lines & BorderLines.Top) > 0)
                    BorderDesc.Append("border-top-width:").
                        Append(Px(HTMLBorderWidth(border.TopLine))).
                        Append("border-top-color:").
                        Append(ExportUtils.HTMLColor(border.TopLine.Color)).Append(";").
                        Append("border-top-style:").Append(HTMLBorderStyle(border.TopLine)).Append(";");
                else
                    BorderDesc.Append("border-top-width:0;");
                // left
                if ((border.Lines & BorderLines.Left) > 0)
                    BorderDesc.Append("border-left-width:").
                        Append(Px(HTMLBorderWidth(border.LeftLine))).
                        Append("border-left-color:").
                        Append(ExportUtils.HTMLColor(border.LeftLine.Color)).Append(";").
                        Append("border-left-style:").Append(HTMLBorderStyle(border.LeftLine)).Append(";");
                else
                    BorderDesc.Append("border-left-width:0;");
                // right
                if ((border.Lines & BorderLines.Right) > 0)
                    BorderDesc.Append("border-right-width:").
                        Append(Px(HTMLBorderWidth(border.RightLine))).
                        Append("border-right-color:").
                        Append(ExportUtils.HTMLColor(border.RightLine.Color)).Append(";").
                        Append(" border-right-style:").Append(HTMLBorderStyle(border.RightLine)).Append(";");
                else
                    BorderDesc.Append("border-right-width:0;");
            }
        }

        private void HTMLAlign(StringBuilder sb, HorzAlign horzAlign, VertAlign vertAlign)
        {
            sb.Append("text-align:");
            if (horzAlign == HorzAlign.Left)
                sb.Append("Left");
            else if (horzAlign == HorzAlign.Right)
                sb.Append("Right");
            else if (horzAlign == HorzAlign.Center)
                sb.Append("Center");
            else if (horzAlign == HorzAlign.Justify)
                sb.Append("Justify");
            sb.Append(";vertical-align:");
            if (vertAlign == VertAlign.Top)
                sb.Append("Top");
            else if (vertAlign == VertAlign.Bottom)
                sb.Append("Bottom");
            else if (vertAlign == VertAlign.Center)
                sb.Append("Middle");
            sb.Append(";word-wrap:break-word");
            sb.Append(";");
        }

        private void HTMLRtl(StringBuilder sb, bool rtl)
        {
            if (rtl)
                sb.Append("direction:rtl;");
        }

        private string HTMLGetStylesHeader(int PageNumber)
        {
            StringBuilder header = new StringBuilder();
            header.AppendLine("<style type=\"text/css\"><!-- ");
            if (FSinglePage && PageNumber == 1 && FPageBreaks)
                header.AppendLine(".page_break { page-break-before:always; }");
            return header.ToString();
        }

        private string HTMLGetStyleHeader(long index)
        {
            StringBuilder header = new StringBuilder();
            return header.Append(".").Append(FStylePrefix).Append("s").Append(index.ToString()).Append(" { ").ToString();
        }

        private void HTMLGetStyle(StringBuilder style, Font Font, Color TextColor, Color FillColor, HorzAlign HAlign, VertAlign VAlign,
            Border Border, System.Windows.Forms.Padding Padding, bool RTL)
        {            
            HTMLFontStyle(style, Font);
            style.Append("color:").Append(ExportUtils.HTMLColor(TextColor)).Append(";");
            style.Append("background-color:");
            style.Append(FillColor == Color.Transparent ? "transparent" : ExportUtils.HTMLColor(FillColor)).Append(";");
            HTMLAlign(style, HAlign, VAlign);
            HTMLBorder(style, Border);
            HTMLPadding(style, Padding);
            HTMLRtl(style, RTL);
            style.AppendLine("}");
        }

        private string HTMLGetStylesFooter()
        {
            return "--></style>";
        }

        private string HTMLGetAncor(string ancorName)
        {
            StringBuilder ancor = new StringBuilder();
            return ancor.Append("<a name=\"PageN").Append(ancorName).Append("\"></a>").ToString();
        }

        private string HTMLGetImageTag(string file)
        {
            return "<img src=\"" + file + "\" alt=\"\"/>";
        }

        private string HTMLGetImage(int PageNumber, int CurrentPage, int ImageNumber, string hash, bool Base,
            System.Drawing.Image Metafile, MemoryStream PictureStream)
        {
            if (FPictures)
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Bmp;
                if (FImageFormat == ImageFormat.Png)
                    format = System.Drawing.Imaging.ImageFormat.Png;
                else if (FImageFormat == ImageFormat.Jpeg)
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (FImageFormat == ImageFormat.Gif)
                    format = System.Drawing.Imaging.ImageFormat.Gif;
                StringBuilder ImageFileNameBuilder = new StringBuilder(48);
                string ImageFileName = ImageFileNameBuilder.Append(Path.GetFileName(FTargetFileName)).
                    Append(".").Append(hash).
                    Append(".").Append(format.ToString().ToLower()).ToString();
                if (!FWebMode)
                {
                    if (Base)
                    {
                        if (Metafile != null)
                            using (FileStream ImageFileStream =
                                new FileStream(FTargetPath + ImageFileName, FileMode.Create))
                                Metafile.Save(ImageFileStream, format);
                        else if (PictureStream != null)
                        {
                            if (FFormat == HTMLExportFormat.HTML)
                            {
                                string fileName = FTargetPath + ImageFileName;
                                FileInfo info = new FileInfo(fileName);
                                if (!(info.Exists && info.Length == PictureStream.Length))
                                {
                                    using (FileStream ImageFileStream =
                                    new FileStream(fileName, FileMode.Create))
                                        PictureStream.WriteTo(ImageFileStream);                                    
                                }
                            }
                            else
                            {
                                PicsArchiveItem item;
                                item.FileName = ImageFileName;
                                item.Stream = PictureStream;
                                bool founded = false;
                                for (int i = 0; i < FPicsArchive.Count; i++)
                                    if (item.FileName == FPicsArchive[i].FileName)
                                    {
                                        founded = true;
                                        break;
                                    }
                                if (!founded)
                                    FPicsArchive.Add(item);
                            }
                        }
                        GeneratedFiles.Add(FTargetPath + ImageFileName);
                    }
                    if (FSubFolder && FSinglePage && !FNavigator)
                        return ExportUtils.HtmlURL(FSubFolderPath + ImageFileName);
                    else
                        return ExportUtils.HtmlURL(ImageFileName);
                }
                else
                {
                    if (Base)
                    {
                        FPages[CurrentPage].Pictures.Add(PictureStream);
                        FPages[CurrentPage].Guids.Add(hash);
                    }
                    return FWebImagePrefix + "=" + hash;
                }
            }
            else
                return String.Empty;
        }

        private StringBuilder ExportHTMLPageStart(StringBuilder Page, int PageNumber, int CurrentPage)
        {
            if (FWebMode)
            {
                FPages[CurrentPage].CSSText = Page.ToString();
                FPages[CurrentPage].PageNumber = PageNumber;
                Page = new StringBuilder(4096);
            }

            if (!FWebMode && !FSinglePage)
            {
                Page.AppendLine(BODY_BEGIN);
            }

            return Page;
        }

        private void ExportHTMLPageFinal(StringBuilder CSS, StringBuilder Page, HTMLThreadData d, float MaxWidth, float MaxHeight)
        {
            if (!FWebMode)
            {
                if (!FSinglePage)
                    Page.AppendLine(BODY_END);
                if (d.PagesStream == null)
                {
                    string FPageFileName = FTargetIndexPath + FTargetFileName + d.PageNumber.ToString() + ".html";
                    GeneratedFiles.Add(FPageFileName);
                    using (FileStream OutStream = new FileStream(FPageFileName, FileMode.Create))
                    using (StreamWriter Out = new StreamWriter(OutStream))
                    {
                        if (!FSinglePage)
                            Out.Write(String.Format(FTemplates.PageTemplateTitle, FDocumentTitle));
                        if (CSS != null)
                            Out.Write(CSS.ToString());
                        if (Page != null)
                            Out.Write(Page.ToString());
                        if (!FSinglePage)
                            Out.Write(FTemplates.PageTemplateFooter);
                    }
                }
                else
                {
                    if (!FSinglePage)
                        ExportUtils.Write(d.PagesStream, String.Format(FTemplates.PageTemplateTitle, FDocumentTitle));
                    if (CSS != null)
                        ExportUtils.Write(d.PagesStream, CSS.ToString());
                    if (Page != null)
                        ExportUtils.Write(d.PagesStream, Page.ToString());
                    if (!FSinglePage)
                        ExportUtils.Write(d.PagesStream, FTemplates.PageTemplateFooter);
                }
            }
            else
            {
                FPages[d.CurrentPage].Width = MaxWidth / Zoom;
                FPages[d.CurrentPage].Height = MaxHeight / Zoom;
                FPages[d.CurrentPage].PageText = Page.ToString();
                FPages[d.CurrentPage].PageEvent.Set();
            }

            if (!FSinglePage && FThreaded)
                if (d.PageNumber == 1)
                    FFirstPageEvent.Set();
        }

        private void ExportHTMLPage(object data)
        {
            HTMLThreadData d = (HTMLThreadData)data;
            if (FLayers)
                ExportHTMLPageLayered(d);
            else
                ExportHTMLPageTabled(d);

        }

        private void ExportHTMLOutline(Stream OutStream)
        {
            if (!FWebMode)
            {
                // under construction            
            }
            else
            {
                // under construction            
            }
        }

        private void ExportHTMLIndex(Stream Stream)
        {
            using (StreamWriter Out = new StreamWriter(Stream))
                Out.Write(String.Format(FTemplates.IndexTemplate,
                    new object[] { FDocumentTitle, ExportUtils.HtmlURL(FNavFileName), 
                        ExportUtils.HtmlURL(FTargetFileName + 
                        (FSinglePage ? ".main" : "1") + ".html") }));
        }

        private void ExportHTMLNavigator(FileStream OutStream)
        {
            using (StreamWriter Out = new StreamWriter(OutStream))
            {
                //  {0} - pages count {1} - name of report {2} multipage document {3} prefix of pages
                //  {4} first caption {5} previous caption {6} next caption {7} last caption
                //  {8} total caption
                Out.Write(String.Format(FTemplates.NavigatorTemplate, 
                    new object[] { FPagesCount.ToString(), 
                        FDocumentTitle, (FSinglePage ? "0" : "1"), 
                        ExportUtils.HtmlURL(FFileName), Res.Get("First"), Res.Get("Prev"), 
                        Res.Get("Next"), Res.Get("Last"), Res.Get("Total") }));
            }
        }

        #endregion


        #region Protected methods


        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            if (!FWebMode)
                using (HTMLExportForm form = new HTMLExportForm())
                {
                    form.Init(this);
                    return form.ShowDialog() == DialogResult.OK;
                }
            else
                return true;
        }        

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            if (Format == HTMLExportFormat.HTML)
                return new MyRes("FileFilters").Get("HtmlFile");
            else
                return new MyRes("FileFilters").Get("MhtFile");
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            FCSSStyles = new List<string>();
            FHPos = 0;

            FCount = Report.PreparedPages.Count;
            FPagesCount = 0;
            FPrevWatermarkName = String.Empty;
            FPrevWatermarkSize = 0;
            FPrevStyleList = null;
            FPrevStyleListIndex = 0;
            
            if (!FWebMode)
            {
                if (FFormat == HTMLExportFormat.MessageHTML)
                {
                    FSubFolder = false;
                    FSinglePage = true;
                    FNavigator = false;
                    FMimeStream = new MemoryStream();
                    FBoundary = ExportUtils.GetID();
                }

                if (!FNavigator)
                    FSinglePage = true;
                if (FileName == "" && Stream != null)
                {
                    FTargetFileName = "html";
                    FSinglePage = true;  
                    FNavigator = false;
                    if (FFormat == HTMLExportFormat.HTML)
                        FPictures = false;                    
                }
                else
                {
                    FTargetFileName = Path.GetFileNameWithoutExtension(FileName);
                    FFileName = FTargetFileName;
                    FTargetIndexPath = Path.GetDirectoryName(FileName);
                }
                if (!String.IsNullOrEmpty(FTargetIndexPath))
                    FTargetIndexPath += "\\";
                if (FSubFolder)
                {
                    FSubFolderPath = FTargetFileName + ".files\\";
                    FTargetPath = FTargetIndexPath + FSubFolderPath;
                    FTargetFileName = FSubFolderPath + FTargetFileName;
                    if (!Directory.Exists(FTargetPath))
                        Directory.CreateDirectory(FTargetPath);
                }
                else
                    FTargetPath = FTargetIndexPath;
                FNavFileName = FTargetFileName + ".nav.html";
                FOutlineFileName = FTargetFileName + ".outline.html";                
                FDocumentTitle = Report.ReportInfo.Name.Length != 0 ?
                    Report.ReportInfo.Name : Path.GetFileNameWithoutExtension(FileName);
                if (FSinglePage)
                {
                    if (FNavigator)
                    {
                        FSinglePageFileName = FTargetIndexPath + FTargetFileName + ".main.html";
                        using (Stream PageStream = new FileStream(FSinglePageFileName,
                            FileMode.Create))
                        using (StreamWriter Out = new StreamWriter(PageStream))
                        {
                            Out.Write(String.Format(FTemplates.PageTemplateTitle, FDocumentTitle));
                            if (FPrint)
                                Out.WriteLine(PRINT_JS);
                            Out.WriteLine(BODY_BEGIN);
                        }
                    }
                    else
                    {
                        FSinglePageFileName = FileName;
                        Stream PagesStream;
                        if (FFormat == HTMLExportFormat.HTML)
                            PagesStream = Stream;
                        else
                            PagesStream = FMimeStream;
                        ExportUtils.Write(PagesStream, String.Format(FTemplates.PageTemplateTitle, FDocumentTitle));
                        if (FPrint)
                            ExportUtils.WriteLn(PagesStream, PRINT_JS);
                        ExportUtils.WriteLn(PagesStream, BODY_BEGIN);
                    }
                }
            }
            else
            {
                FPages.Clear();
                for (int i = 0; i < FCount; i++)
                    FPages.Add(new HTMLPageData());                    
            }
            
            if (!FSinglePage && FThreaded)
                FFirstPageEvent = new ManualResetEvent(false);
        }

        /// <inheritdoc/>
        protected override void ExportPage(int pageNo)
        {
            FPagesCount++;
            if (FSinglePage)
            {
                HTMLThreadData d = new HTMLThreadData();
                d.ReportPage = pageNo;
                d.PageNumber = FPagesCount;
                if (FNavigator)
                {
                    GeneratedFiles.Add(FSinglePageFileName);
                    using (d.PagesStream = new FileStream(FSinglePageFileName,
                        FileMode.Append))
                    {
                        ExportHTMLPage(d);
                    }
                }
                else
                {
                    if (FFormat == HTMLExportFormat.HTML)
                        d.PagesStream = Stream;
                    else
                        d.PagesStream = FMimeStream;
                    ExportHTMLPage(d);
                }
            }
            else if (!FWebMode)
                ProcessPage(FPagesCount - 1, pageNo);
        }

        /// <summary>
        /// Process Page with number p and real page ReportPage
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ReportPage"></param>
        public void ProcessPage(int p, int ReportPage)
        {
            HTMLThreadData d = new HTMLThreadData();
            d.ReportPage = ReportPage;
            d.PageNumber = FPagesCount;
            d.PagesStream = null;
            d.CurrentPage = p;
            if (!FWebMode && FThreaded)
                ThreadPool.QueueUserWorkItem(ExportHTMLPage, d);
            else
                ExportHTMLPage(d);
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            if (!FSinglePage && !FWebMode && FThreaded)
                while (!FFirstPageEvent.WaitOne(10, true))
                    Application.DoEvents();
                
            if (!FWebMode)
            {
                if (FNavigator)
                {
                    if (FSinglePage)
                    {
                        using (Stream PageStream = new FileStream(FSinglePageFileName,
                            FileMode.Append))
                        using (StreamWriter Out = new StreamWriter(PageStream))
                        {
                            Out.WriteLine(BODY_END);
                            Out.Write(FTemplates.PageTemplateFooter);
                        }
                    }
                    ExportHTMLIndex(Stream);
                    GeneratedFiles.Add(FTargetIndexPath + FNavFileName);
                    using (FileStream OutStream = new FileStream(FTargetIndexPath + FNavFileName, FileMode.Create))
                        ExportHTMLNavigator(OutStream);
                    GeneratedFiles.Add(FTargetIndexPath + FOutlineFileName);
                    using (FileStream OutStream = new FileStream(FTargetIndexPath + FOutlineFileName, FileMode.Create))
                        ExportHTMLOutline(OutStream);
                }
                else if (FFormat == HTMLExportFormat.MessageHTML)
                {

                    ExportUtils.WriteLn(FMimeStream, BODY_END);
                    ExportUtils.Write(FMimeStream, FTemplates.PageTemplateFooter);

                    WriteMHTHeader();
                    WriteMimePart(FMimeStream, "text/html", "utf-8", "index.html");

                    for (int i = 0; i < FPicsArchive.Count; i++)
                    {
                        string imagename = FPicsArchive[i].FileName;                        
                        WriteMimePart(FPicsArchive[i].Stream, "image/" + imagename.Substring(imagename.LastIndexOf('.') + 1), "utf-8", imagename);
                    }

                    string last = "--" + FBoundary + "--";
                    Stream.Write(Encoding.ASCII.GetBytes(last), 0, last.Length);
                }
                else
                {
                    ExportUtils.WriteLn(Stream, BODY_END);
                    ExportUtils.Write(Stream, FTemplates.PageTemplateFooter);
                }
            }
        }

        private void WriteMimePart(Stream stream, string mimetype, string charset, string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("--").AppendLine(FBoundary);
            sb.Append("Content-Type: ").Append(mimetype).Append(";");
            if (charset != String.Empty)
                sb.Append(" charset=\"").Append(charset).AppendLine("\"");
            else
                sb.AppendLine();
            string body;
            byte[] buff = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buff, 0, buff.Length);
            if (mimetype == "text/html")
            {
                sb.AppendLine("Content-Transfer-Encoding: quoted-printable");
                body = ExportUtils.QuotedPrintable(buff);
            }
            else
            {
                sb.AppendLine("Content-Transfer-Encoding: base64");
                body = System.Convert.ToBase64String(buff, Base64FormattingOptions.InsertLineBreaks);
            }
            sb.Append("Content-Location: ").AppendLine(ExportUtils.HtmlURL(filename));
            sb.AppendLine();
            sb.AppendLine(body);            
            sb.AppendLine();
            Stream.Write(Encoding.ASCII.GetBytes(sb.ToString()), 0, sb.Length);
        }

        private void WriteMHTHeader()
        {
            StringBuilder sb = new StringBuilder(256);
            string s = "=?utf-8?B?" + System.Convert.ToBase64String(Encoding.UTF8.GetBytes(FileName)) + "?=";
            sb.Append("From: ").AppendLine(s);
            sb.Append("Subject: ").AppendLine(s);
            sb.Append("Date: ").AppendLine(ExportUtils.GetRFCDate(DateTime.Now));
            sb.AppendLine("MIME-Version: 1.0");
            sb.Append("Content-Type: multipart/related; type=\"text/html\"; boundary=\"").Append(FBoundary).AppendLine("\"");
            sb.AppendLine();
            sb.AppendLine("This is a multi-part message in MIME format.");
            sb.AppendLine();
            ExportUtils.Write(Stream, sb.ToString());
        }

        #endregion


        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteBool("Layers", Layers);
            writer.WriteBool("Wysiwyg", Wysiwyg);
            writer.WriteBool("Pictures", Pictures);
            writer.WriteBool("SubFolder", SubFolder);
            writer.WriteBool("Navigator", Navigator);
            writer.WriteBool("SinglePage", SinglePage);
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public void Init_WebMode()       
        {
            FPages = new List<HTMLPageData>();
            FWebMode = true;
            OpenAfterExport = false;            
        }

        internal void Finish_WebMode()
        {
            FPages.Clear();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HTMLExport"/> class.
        /// </summary>
        public HTMLExport()
        {            
            Zoom = 1.0f;
            FLayers = false;
            FWysiwyg = true;
            FPictures = true;
            FWebMode = false;
            FSubFolder = true;
            FNavigator = true;
            FSinglePage = false;
            FThreaded = false;
            FWidthUnits = HtmlSizeUnits.Pixel;
            FHeightUnits = HtmlSizeUnits.Pixel;
            FImageFormat = ImageFormat.Png;
            FTemplates = new HtmlTemplates();
            FFormat = HTMLExportFormat.HTML;
            FPicsArchive = new List<PicsArchiveItem>();
            FPrevStyleList = null;
            FPrevStyleListIndex = 0;
            FPageBreaks = true;
            FPrint = false;
            FNumberFormat = new NumberFormatInfo();
            FNumberFormat.NumberGroupSeparator = String.Empty;
            FNumberFormat.NumberDecimalSeparator = ".";
            Res = new MyRes("Export,Html");
        }
    }

    class HtmlTemplates
    {
        #region private fields
        private string FPageTemplateTitle;
        private string FPageTemplateFooter;
        private string FNavigatorTemplate;
        private string FOutlineTemplate;
        private string FIndexTemplate;        
        private StringBuilder FCapacitor;
        #endregion

        #region private methods
        private void NewCapacitor()
        {
            FCapacitor = new StringBuilder(512);
        }
        private void Part(string str)
        {
            FCapacitor.AppendLine(str);
        }
        private string Capacitor()
        {
            return FCapacitor.ToString();
        }
        #endregion

        #region public properties
        public string PageTemplateTitle
        {
            get { return FPageTemplateTitle; }
            set { FPageTemplateTitle = value; }
        }
        public string PageTemplateFooter
        {
            get { return FPageTemplateFooter; }
            set { FPageTemplateFooter = value; }
        }
        public string NavigatorTemplate
        {
            get { return FNavigatorTemplate; }
            set { FNavigatorTemplate = value; }
        }
        public string OutlineTemplate
        {
            get { return FOutlineTemplate; }
            set { FOutlineTemplate = value; }
        }
        public string IndexTemplate
        {
            get { return FIndexTemplate; }
            set { FOutlineTemplate = value; }
        }
        #endregion

        public HtmlTemplates()
        {
            #region fill page template
            // {0} - title
            NewCapacitor();
            Part("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
            Part("<html><head>");
            Part("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            Part("<meta name=Generator content=\"FastReport http://www.fast-report.com\">");
            Part("<title>{0}</title>");
            FPageTemplateTitle = Capacitor();

            NewCapacitor();
            Part("</html>");            
            FPageTemplateFooter = Capacitor();
            #endregion

            #region fill navigator template
            //  {0} - pages count {1} - name of report {2} multipage document {3} prefix of pages
            //  {4} first caption {5} previous caption {6} next caption {7} last caption
            //  {8} total caption
            NewCapacitor();
            Part("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
            Part("<html><head>");
            Part("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            Part("<meta name=Generator content=\"FastReport http://www.fast-report.com\">");
            Part("<title></title><style type=\"text/css\"><!--");
            Part("body,input,select {{ font-family:\"Lucida Grande\",Calibri,Arial,sans-serif; font-size: 8px; font-weight: bold; font-style: normal; text-align: center; vertical-align: middle; }}");
            Part("input {{text-align: center}}");
            Part(".nav {{ font-size : 9pt; color : #283e66; font-weight : bold; text-decoration : none;}}");
            Part("--></style><script language=\"javascript\" type=\"text/javascript\"><!--");
            Part("  var frPgCnt = {0}; var frRepName = \"{1}\"; var frMultipage = {2}; var frPrefix=\"{3}\";");
            Part("  function DoPage(PgN) {{");
            Part("    if ((PgN > 0) && (PgN <= frPgCnt) && (PgN != parent.frCurPage)) {{");
            Part("      if (frMultipage > 0)  parent.mainFrame.location = frPrefix + PgN + \".html\";");
            Part("      else parent.mainFrame.location = frPrefix + \".main.html#PageN\" + PgN;");
            Part("      UpdateNav(PgN); }} else document.PgForm.PgEdit.value = parent.frCurPage; }}");
            Part("  function UpdateNav(PgN) {{");
            Part("    parent.frCurPage = PgN; document.PgForm.PgEdit.value = PgN;");
            Part("    if (PgN == 1) {{ document.PgForm.bFirst.disabled = 1; document.PgForm.bPrev.disabled = 1; }}");
            Part("    else {{ document.PgForm.bFirst.disabled = 0; document.PgForm.bPrev.disabled = 0; }}");
            Part("    if (PgN == frPgCnt) {{ document.PgForm.bNext.disabled = 1; document.PgForm.bLast.disabled = 1; }}");
            Part("    else {{ document.PgForm.bNext.disabled = 0; document.PgForm.bLast.disabled = 0; }} }}");
            Part("--></script></head>");
            Part("<body bgcolor=\"#DDDDDD\" text=\"#000000\" leftmargin=\"0\" topmargin=\"4\" onload=\"UpdateNav(parent.frCurPage)\">");
            Part("<form name=\"PgForm\" onsubmit=\"DoPage(document.forms[0].PgEdit.value); return false;\" action=\"\">");
            Part("<table cellspacing=\"0\" align=\"left\" cellpadding=\"0\" border=\"0\" width=\"100%\">");
            Part("<tr valign=\"middle\">");
            Part("<td width=\"60\" align=\"center\"><button name=\"bFirst\" class=\"nav\" type=\"button\" onclick=\"DoPage(1); return false;\"><b>{4}</b></button></td>");
            Part("<td width=\"60\" align=\"center\"><button name=\"bPrev\" class=\"nav\" type=\"button\" onclick=\"DoPage(Math.max(parent.frCurPage - 1, 1)); return false;\"><b>{5}</b></button></td>");
            Part("<td width=\"100\" align=\"center\"><input type=\"text\" class=\"nav\" name=\"PgEdit\" value=\"parent.frCurPage\" size=\"4\"></td>");
            Part("<td width=\"60\" align=\"center\"><button name=\"bNext\" class=\"nav\" type=\"button\" onclick=\"DoPage(parent.frCurPage + 1); return false;\"><b>{6}</b></button></td>");
            Part("<td width=\"60\" align=\"center\"><button name=\"bLast\" class=\"nav\" type=\"button\" onclick=\"DoPage(frPgCnt); return false;\"><b>{7}</b></button></td>");
            Part("<td width=\"20\">&nbsp;</td>\r\n");
            Part("<td align=\"right\">{8}: <script language=\"javascript\" type=\"text/javascript\"> document.write(frPgCnt);</script></td>");
            Part("<td width=\"10\">&nbsp;</td>");
            Part("</tr></table></form></body></html>");
            FNavigatorTemplate = Capacitor();
            #endregion

            #region fill outline template
            // under construction
            FOutlineTemplate = String.Empty;
            #endregion

            #region fill index template
            // {0} - title, {1} - navigator frame, {2} - main frame
            NewCapacitor();
            Part("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\"");
            Part("<html><head>");
            Part("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            Part("<meta name=Generator content=\"FastReport http://www.fast-report.com\">");
            Part("<title>{0}</title>");
            Part("<script language=\"javascript\" type=\"text/javascript\"> var frCurPage = 1;</script></head>");
            Part("<frameset rows=\"36,*\" cols=\"*\">");
            Part("<frame name=\"topFrame\" src=\"{1}\" noresize frameborder=\"0\" scrolling=\"no\">");
            Part("<frame name=\"mainFrame\" src=\"{2}\" frameborder=\"0\">");
            Part("</frameset>");
            Part("</html>");
            FIndexTemplate = Capacitor();
            #endregion
        }
    }

}
