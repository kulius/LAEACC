using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Windows.Forms;
using FastReport;
using FastReport.Utils;
using FastReport.Table;
using FastReport.Forms;

namespace FastReport.Export.OoXML
{
    /// <summary>
    ///  Picture container
    /// </summary>
    internal class XPS_Picture : OoXMLBase
    {
        #region "Class overrides"
        public override string RelationType { get { return "http://schemas.microsoft.com/xps/2005/06/required-resource"; } }
        public override string ContentType { get { return null; } } //"application/vnd.openxmlformats-officedocument.presentationml.slide+xml"; } }
        public override string FileName { get { return ImageFileName; } }
        #endregion

        #region "Private fields"
        private ReportComponentBase picture;
        private string ImageFileName;
//        private XPSExport FExport;
        #endregion

        internal string SaveImage(XPSExport export, ReportComponentBase obj, bool ClearBackground)
        {
            export.PicturesCount++;
            ImageFileName = "/Resources/" + obj.Name + "." + export.PicturesCount + ".png";

            using (System.Drawing.Image image = new System.Drawing.Bitmap((int)Math.Round(obj.Width), (int)Math.Round(obj.Height)))
            using (Graphics g = Graphics.FromImage(image))
            using (GraphicCache cache = new GraphicCache())
            {
                g.TranslateTransform(-obj.AbsLeft, -obj.AbsTop);
                if (ClearBackground)
                {
                    g.Clear(Color.White);
                }
                obj.Draw(new FRPaintEventArgs(g, 1, 1, cache));
                image.Save(export.TempFolder + ImageFileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            return ImageFileName;
        }

        public XPS_Picture(ReportComponentBase obj)
        {
            picture = obj;
        }
    }

    /// <summary>
    ///  List of all document fonts
    /// </summary>
    internal class XPS_FontList : IDisposable
    {
        private Hashtable fonthash;

        internal XPS_Font AddFont(XPSExport export, Font f, out bool FirstOccurrence)
        {
            int HashCode;
            XPS_Font font;

            FontStyle style = f.Style & (FontStyle.Bold | FontStyle.Italic);
            HashCode = f.Name.GetHashCode() + style.GetHashCode();

            if (fonthash.ContainsKey(HashCode) == false)
            {
                font = new XPS_Font(f);
                fonthash.Add(HashCode, font);
                FirstOccurrence = true;
            }
            else
            {
                font = fonthash[HashCode] as XPS_Font;
                font.SourceFont = f;
                FirstOccurrence = false;
            }

            return font;
        }

        internal XPS_FontList()
        {
            fonthash = new Hashtable();
        }

        internal void Export(XPSExport export)
        {
            foreach (XPS_Font font in fonthash.Values)
            {
                font.ExportFont(export);
            }
        }

        public void Dispose()
        {
            foreach (XPS_Font font in fonthash)
                font.Dispose();
        }

    }

    /// <summary>
    ///  Single page export
    /// </summary>
    internal class XPS_PageContent : OoXMLBase
    {
        const float FontMultiplier = 1.32805F;
        const float MetrixMultiplier = 3.776F;
        
        #region "Class overrides"
        public override string RelationType { get { return null;}} // "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"; } }
        public override string ContentType { get { return null;}} //"application/vnd.openxmlformats-officedocument.presentationml.slide+xml"; } }
        public override string FileName { get { return "/Documents/1/Pages/" + PageNumber + ".fpage"; } }
        #endregion

        NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
        
        #region Private fields
        int PageNumber;
        int RelationCount;
        XPSExport FExport;
        private float FMarginLeft;
        private float FMarginTop;
        #endregion

        #region Helpers
        private string GetAlphaColor(Color color)
        {
            return Quoted("#" + color.A.ToString("x2") + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2"));
        }

        private string GetColor(Color color)
        {
            return Quoted("#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2"));
        }

        private string TranslateText(string text)
        {
            StringBuilder TextStrings = new StringBuilder();
            int start_idx = 0;

            while (true)
            {
                int idx = text.IndexOfAny("&<>\"".ToCharArray(), start_idx);
                if (idx != -1)
                {
                    TextStrings.Append(text.Substring(start_idx, idx - start_idx));
                    switch (text[idx])
                    {
                        case '&': TextStrings.Append("&amp;"); break;
                        case '<': TextStrings.Append("&lt;"); break;
                        case '>': TextStrings.Append("&gt;"); break;
                        case '"': TextStrings.Append("&quot;"); break;
                    }
                    start_idx = ++idx;
                    continue;
                }
                TextStrings.Append(text.Substring(start_idx));
                break;
            }

            return TextStrings.ToString();
        }

        private bool CheckGlyph(string text, XPS_Font font)
        {
            bool empty = true;
            foreach (char ch in text)
            {
                if ( empty && !Char.IsWhiteSpace(ch)) empty = false;
            }
            return empty;
        }

        private void Add_Glyphs(
            StreamWriter Out, 
            TextObject obj, 
            XPS_Font fnt, 
            Color color, 
            float left, float top, float width, string text)
        {
            string render_transform = "";
            string font_indexes = "";
            string unicode_string = "";
            string bidi_level = "";


            if ( ! CheckGlyph( text,  fnt ) )
            {
                if (obj.RightToLeft)
                {
                    ;
                }
                string remaped_text = fnt.AddString(text, obj.RightToLeft);

                if (obj.Angle != 0 || obj.FontWidthRatio != 1)
                {
                    float fix_angle = (float)(obj.Angle * Math.PI / 180);

                    float c = (float)Math.Round(Math.Cos(fix_angle), 5);
                    float s = (float)Math.Round(Math.Sin(fix_angle), 5);

                    float x = 0;
                    float y = 0;

                    if (obj.FontWidthRatio == 1)
                    {
                        // Fix padding
                        x = obj.AbsLeft + FMarginLeft + obj.Width / 2;
                        y = obj.AbsTop + FMarginTop + obj.Height / 2;
                    }

                    render_transform = "RenderTransform=" + Quoted(
                            (c * obj.FontWidthRatio).ToString(nfi) + "," + s.ToString(nfi) + "," +
                            (-s).ToString(nfi) + "," + c.ToString(nfi) + "," +
                            x.ToString(nfi) + "," + y.ToString(nfi));
                }

                font_indexes = fnt.GetXpsIndexes(remaped_text);

                //if (obj.RightToLeft == true)
                //{
                //    bidi_level = "BidiLevel=" + Quoted(1) + " ";
                //}

                if (this.FExport.HumanReadable)
                {
                    unicode_string = " UnicodeString=" + Quoted(TranslateText(text));
                }

                if (obj.TextFill is SolidFill)
                {
                    Out.WriteLine("<Glyphs Fill=" + GetAlphaColor(color) +
                            bidi_level +
                            " FontUri=" + Quoted(fnt.FileName) +
                            " FontRenderingEmSize=" + Quoted(fnt.Size * FontMultiplier) +
                            " StyleSimulations=" + Quoted("None") +
                            " OriginX=" + Quoted(left) +
                            " OriginY=" + Quoted(top + fnt.Size * FontMultiplier) +
                            font_indexes +
                            unicode_string +
                            render_transform +
                        " />");
                }
                else
                {
                    LinearGradientFill fill = obj.TextFill as LinearGradientFill;

                    float right = FMarginLeft + obj.AbsRight;
                    float bottom = top + fnt.Size * FontMultiplier;
                    float x = 0;

                    Out.WriteLine("<Glyphs" +
                            bidi_level +
                            " FontUri=" + Quoted(fnt.FileName) +
                            " FontRenderingEmSize=" + Quoted(fnt.Size * FontMultiplier) +
                            " StyleSimulations=" + Quoted("None") +
                            " OriginX=" + Quoted(left) +
                            " OriginY=" + Quoted(top + fnt.Size * FontMultiplier) +
                            font_indexes +
                            unicode_string +
                            render_transform + " >");

                    Out.Write("<Glyphs.Fill>" +
                        "<LinearGradientBrush MappingMode=\"Absolute\" StartPoint=" +
                        Quoted(x.ToString(nfi) + "," + top.ToString(nfi)) +
                        " EndPoint=" +
                        Quoted(x.ToString(nfi) + "," + bottom.ToString(nfi)) + ">" +
                        "<LinearGradientBrush.GradientStops>" +
                        "<GradientStop Color=" + GetColor(fill.EndColor) + " Offset=\"0\" />" +
                        "<GradientStop Color=" + GetColor(fill.StartColor) + " Offset=\"1\" />" +
                        "</LinearGradientBrush.GradientStops></LinearGradientBrush></Glyphs.Fill>");

                    Out.WriteLine("</Glyphs>");
                }

                if ( (fnt.Style & FontStyle.Underline) == FontStyle.Underline)
                {
                    float y = top + (fnt.Size + 2) * FontMultiplier;
                    Out.Write("<Path Data=" + Quoted( 
                            "M " + left.ToString(nfi) + "," + y.ToString(nfi) + 
                            " L " + (left + width).ToString(nfi) + "," + y.ToString(nfi) ) + 
                        " Stroke=" + GetAlphaColor(color)+
                        " StrokeThickness=" + Quoted( (fnt.Size / 16).ToString(nfi) ) + 
                        render_transform + " />");
                }

                if ( (fnt.Style & FontStyle.Strikeout) == FontStyle.Strikeout)
                {
                    float y = top + (fnt.Height / 2) * FontMultiplier;
                    Out.Write("<Path Data=" + Quoted(
                            "M " + left.ToString(nfi) + "," + y.ToString(nfi) +
                            " L " + (left + width).ToString(nfi) + "," + y.ToString(nfi)) +
                        " Stroke=" + GetAlphaColor(color) +
                        " StrokeThickness=" + Quoted( (fnt.Size / 32).ToString(nfi)) + 
                        render_transform + " />");
                }
            }
        }

        private void FillRectangle(StreamWriter Out, ReportComponentBase obj)
        {
            float left = FMarginLeft + obj.AbsLeft;
            float right = FMarginLeft + obj.AbsRight;
            float top = FMarginTop + obj.AbsTop;
            float bottom = FMarginTop + obj.AbsBottom;

            if (obj.Fill is SolidFill)
            {
                if (obj.FillColor.A == 0) return;

                Out.Write("<Path Data=\"F1 M " + left.ToString(nfi) + "," + top.ToString(nfi) +
                    " H " + right.ToString(nfi) +
                    " V " + bottom.ToString(nfi) +
                    " H " + left.ToString(nfi) +
                    " z\" Fill=" + GetAlphaColor(obj.FillColor) + " />");
            }
            else if (obj.Fill is GlassFill)
            {
                GlassFill fill = obj.Fill as GlassFill;
                Out.Write("<Path Data=\"F1 M " + left.ToString(nfi) + "," + top.ToString(nfi) +
                    " H " + right.ToString(nfi) +
                    " V " + (bottom - obj.Height / 2).ToString(nfi) +
                    " H " + left.ToString(nfi) +
                    " z\" Opacity="+Quoted((1 - fill.Blend).ToString(nfi)) +" Fill=" + GetAlphaColor(fill.Color) + " />");

                Out.Write("<Path Data=\"F1 M " + left.ToString(nfi) + "," + (top + obj.Height / 2).ToString(nfi) +
                    " H " + right.ToString(nfi) +
                    " V " + bottom.ToString(nfi) +
                    " H " + left.ToString(nfi) +
                    " z\" Fill=" + GetAlphaColor(fill.Color) + " />");
            }
            else if (obj.Fill is LinearGradientFill)
            {
                LinearGradientFill fill = obj.Fill as LinearGradientFill;

                float x = 0;
                // to do: support focus, contrast

                Out.Write("<Path>");
		        Out.Write("<Path.Fill>"+
                    "<LinearGradientBrush MappingMode=\"Absolute\" StartPoint=" + 
                    Quoted( x.ToString(nfi) + "," + top.ToString(nfi) ) + 
                    " EndPoint=" +
                    Quoted( x.ToString(nfi) + "," + bottom.ToString(nfi) ) + ">" +
                    "<LinearGradientBrush.GradientStops>" +
                    "<GradientStop Color=" + GetColor(fill.EndColor) + " Offset=\"0\" />" +
                    "<GradientStop Color="+ GetColor(fill.StartColor) + " Offset=\"1\" />" + 
				    "</LinearGradientBrush.GradientStops></LinearGradientBrush></Path.Fill>");

                Out.Write("<Path.Data><PathGeometry><PathFigure StartPoint=" + 
                    Quoted( left.ToString(nfi) +","+ top.ToString(nfi) )+">" +
                    "<PolyLineSegment Points=" + 
                    Quoted(right.ToString(nfi)+ "," + top.ToString(nfi) + " " + 
                    right.ToString(nfi) +"," + bottom.ToString(nfi) + " " + 
                    left.ToString(nfi)+ "," + bottom.ToString(nfi)) + " />" +
                    "</PathFigure></PathGeometry></Path.Data>");

                Out.Write("</Path>");
            }
        }

        private string GetLineStyle(LineStyle style)
        {
            string result_style = "StrokeDashArray=";
            switch(style)
            {
                case LineStyle.Solid: return "";
                case LineStyle.Dot:
                    result_style += Quoted("1.0 1.0");
                    break;
                case LineStyle.Dash:
                    result_style += Quoted("2.75 1.0");
                    break;
                case LineStyle.DashDot:
                    result_style += Quoted("2.75 1.0 1.0 1.0");
                    break;
                case LineStyle.DashDotDot:
                    result_style += Quoted("2.75 1.0 1.0 1.0 1.0 1.0");
                    break;
            }
            return result_style;
        }

        private void DrawBorder(StreamWriter Out, ReportComponentBase obj)
        {
            Border b = obj.Border;

            if (b.Lines == BorderLines.None) return;

            float left = FMarginLeft + obj.AbsLeft;
            float right = FMarginLeft + obj.AbsRight;
            float top = FMarginTop + obj.AbsTop;
            float bottom = FMarginTop + obj.AbsBottom;

            bool same_border =
                b.Lines == BorderLines.All &&
                (b.BottomLine.Color == b.LeftLine.Color) &&
                (b.BottomLine.Color == b.TopLine.Color) &&
                (b.BottomLine.Color == b.RightLine.Color) &&

                (b.BottomLine.DashStyle == b.LeftLine.DashStyle) &&
                (b.BottomLine.DashStyle == b.TopLine.DashStyle) &&
                (b.BottomLine.DashStyle == b.RightLine.DashStyle) &&

                (b.BottomLine.Width == b.LeftLine.Width) &&
                (b.BottomLine.Width == b.TopLine.Width) &&
                (b.BottomLine.Width == b.RightLine.Width);

            if (same_border)
            {
                Out.Write("<Path Data=\"M " + left.ToString(nfi) + "," + top.ToString(nfi) +
                    " H " + right.ToString(nfi) +
                    " V " + bottom.ToString(nfi) +
                    " H " + left.ToString(nfi) +
                    " z\" Stroke=" + GetAlphaColor(obj.Border.Color) + 
                    GetLineStyle(obj.Border.Style) + 
                    " StrokeThickness=" + Quoted(obj.Border.Width) + " />");
            }
            else
            {
                if ((b.Lines & BorderLines.Left) == BorderLines.Left)
                {
                    Out.Write("<Path Data=\"M " + left.ToString(nfi) + "," + top.ToString(nfi) + " V " + bottom.ToString(nfi) +
                        "\" Stroke=" + GetAlphaColor(b.LeftLine.Color) +
                        GetLineStyle(b.LeftLine.Style) +
                        " StrokeThickness=" + Quoted(b.LeftLine.Width) + " />");
                }
                if ((b.Lines & BorderLines.Bottom) == BorderLines.Bottom)
                {
                    Out.Write("<Path Data=\"M " + left.ToString(nfi) + "," + bottom.ToString(nfi) + " H " + right.ToString(nfi) +
                        "\" Stroke=" + GetAlphaColor(b.BottomLine.Color) +
                        GetLineStyle(b.BottomLine.Style) +
                        " StrokeThickness=" + Quoted(b.BottomLine.Width) + " />");
                }
                if ((b.Lines & BorderLines.Right) == BorderLines.Right)
                {
                    Out.Write("<Path Data=\"M " + right.ToString(nfi) + "," + top.ToString(nfi) + " V " + bottom.ToString(nfi) +
                        "\" Stroke=" + GetAlphaColor(b.RightLine.Color) +
                        GetLineStyle(b.RightLine.Style) +
                        " StrokeThickness=" + Quoted(b.RightLine.Width) + " />");
                }
                if ((b.Lines & BorderLines.Top) == BorderLines.Top)
                {
                    Out.Write("<Path Data=\"M " + left.ToString(nfi) + "," + top.ToString(nfi) + " H " + right.ToString(nfi) +
                        "\" Stroke=" + GetAlphaColor(b.TopLine.Color) +
                        GetLineStyle(b.TopLine.Style) +
                        " StrokeThickness=" + Quoted(b.TopLine.Width) + " />");
                }
            }
        }

        private void DrawShadow(StreamWriter Out, ReportComponentBase obj)
        {
            if (obj.Border.Shadow)
            {
                float x, y, sz;

                sz = obj.Border.ShadowWidth;
                x = FMarginLeft + obj.AbsRight;
                y = FMarginTop + obj.AbsTop;

                Out.Write("<Path Data=\"F1 M " + x.ToString(nfi) + "," + (y + sz).ToString(nfi) +
                    " H " + (x + sz).ToString(nfi) +
                    " V " + (y + sz + obj.Height).ToString(nfi) +
                    " H " + (x + sz - obj.Width).ToString(nfi) +
                    " V " + (y + obj.Height).ToString(nfi) +
                    " H " + x.ToString(nfi) +
                    " z\" Fill=" + GetAlphaColor(obj.Border.ShadowColor) + " />");
            }
        }

        #endregion

        internal void Export(XPSExport export, ReportPage page)
        {
            FExport = export;
            FMarginLeft = page.LeftMargin * MetrixMultiplier;
            FMarginTop = page.TopMargin * MetrixMultiplier;

            using (FileStream file = new FileStream(export.TempFolder + FileName, FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                Out.WriteLine("<FixedPage xmlns=\"http://schemas.microsoft.com/xps/2005/06\"" +
                    " Width=" + Quoted(page.PaperWidth * MetrixMultiplier) +
                    " Height=" + Quoted(page.PaperHeight * MetrixMultiplier) +
                    " xml:lang=\"und\">");

                // bitmap watermark on bottom
                if (page.Watermark.Enabled && !page.Watermark.ShowImageOnTop)
                    AddBitmapWatermark(Out, page);

                // text watermark on bottom
                if (page.Watermark.Enabled && !page.Watermark.ShowTextOnTop)
                    AddTextWatermark(Out, page);

                using (page) foreach (Base c in page.AllObjects)
                    {
                        ReportComponentBase obj = c as ReportComponentBase;
                        if (obj is CellularTextObject)
                            obj = (obj as CellularTextObject).GetTable();
                        if (obj is TableCell)
                            continue;
                        else if (obj is TableBase)
                            AddTable(Out, obj as TableBase);
                        else if (obj is TextObject)
                            AddTextObject(Out, obj as TextObject);
                        else if (obj is BandBase)
                            AddBandObject(Out, obj as BandBase);
                        else if (obj is LineObject)
                            AddLine(Out, obj as LineObject);
                        else if (obj is ShapeObject)
                            AddShape(Out, obj as ShapeObject);
                        else if (obj is PictureObject)
                            AddPictureObject(Out, rId, obj as PictureObject, "ppt/media/image");
                        else if (obj is Barcode.BarcodeObject)
                            AddPictureObject(Out, rId, obj as ReportComponentBase, "ppt/media/BarcodeImage");
                        else if (obj is ZipCodeObject)
                            AddPictureObject(Out, rId, obj as ReportComponentBase, "ppt/media/ZipCodeImage");
                        else if (obj is MSChart.MSChartObject)
                            AddPictureObject(Out, rId, obj as ReportComponentBase, "ppt/media/MSChartImage");
                        else if (obj is RichObject)
                            AddPictureObject(Out, rId, obj as ReportComponentBase, "ppt/media/RichTextImage");
                        else if (obj is CheckBoxObject)
                            AddCheckboxObject(Out, obj as CheckBoxObject );
                        else if (obj == null)
                        {
                            ;
                        }
                        else
                        {
                            AddPictureObject(Out, rId, obj as ReportComponentBase, "ppt/media/FixMeImage");
                        }
                    }

                // bitmap watermark on top
                if (page.Watermark.Enabled && page.Watermark.ShowImageOnTop)
                    AddBitmapWatermark(Out, page);

                // text watermark on top
                if (page.Watermark.Enabled && page.Watermark.ShowTextOnTop)
                    AddTextWatermark(Out, page);

                Out.WriteLine("</FixedPage>");
            }

            this.ExportRelations(FExport);
        }

        #region "Export report object primitives"
        private void AddCheckboxObject(StreamWriter Out, CheckBoxObject checkBox)
        {
            if (checkBox.HideIfUnchecked && !checkBox.Checked) return;

            RectangleF drawRect = new RectangleF(
                FMarginLeft + checkBox.AbsLeft, FMarginTop + checkBox.AbsTop, checkBox.Width, checkBox.Height);

            FillRectangle(Out, checkBox);
            DrawBorder(Out, checkBox);

            if (!checkBox.Checked && checkBox.UncheckedSymbol == UncheckedSymbol.None) return;

            Out.Write("<Path Data=\"");

            float ratio = checkBox.Width / (Units.Millimeters * 5);
            drawRect.Inflate(-4 * ratio, -4 * ratio);

            if (checkBox.Checked)
            {
                switch (checkBox.CheckedSymbol)
                {
                    case CheckedSymbol.Check:
                        Out.Write("M " + drawRect.Left.ToString(nfi) + "," +
                            (drawRect.Top + drawRect.Height / 10 * 5).ToString(nfi) + " L " +
                            (drawRect.Left + drawRect.Width / 10 * 4).ToString(nfi) + "," +
                            (drawRect.Bottom - drawRect.Height / 10).ToString(nfi) + " " +
                            drawRect.Right.ToString(nfi) + "," +
                            (drawRect.Top + drawRect.Height / 10).ToString(nfi)
                        );
                        break;

                    case CheckedSymbol.Cross:
                        Out.Write("M " + drawRect.Left.ToString(nfi) + "," + drawRect.Top.ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) +
                            " M " + drawRect.Left.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + drawRect.Top.ToString(nfi)
                        );
                        break;

                    case CheckedSymbol.Plus:
                        Out.Write("M " + drawRect.Left.ToString(nfi) + "," + (drawRect.Top + drawRect.Height / 2).ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + (drawRect.Top + drawRect.Height / 2).ToString(nfi) +
                            " M " + (drawRect.Left + drawRect.Width / 2).ToString(nfi) + "," + drawRect.Top.ToString(nfi) +
                            " L " + (drawRect.Left + drawRect.Width / 2).ToString(nfi) + "," + drawRect.Bottom.ToString(nfi)
                        );
                        break;

                    case CheckedSymbol.Fill:
                        Out.Write("M " + drawRect.Left.ToString(nfi) + "," + drawRect.Top.ToString(nfi) + " L " +
                            drawRect.Right.ToString(nfi) + "," + drawRect.Top.ToString(nfi) + " " +
                            drawRect.Right.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) + " " +
                            drawRect.Left.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) + " z "
                        );
                        break;
                }
            }
            else
            {
                switch (checkBox.UncheckedSymbol)
                {
                    case UncheckedSymbol.Cross:
                        Out.Write("M " + drawRect.Left.ToString(nfi) + "," + drawRect.Top.ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) +
                            " M " + drawRect.Left.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + drawRect.Top.ToString(nfi)
                        );
                        break;

                    case UncheckedSymbol.Minus:
                        Out.Write("M " + drawRect.Left.ToString(nfi) + "," + (drawRect.Top + drawRect.Height / 2).ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + (drawRect.Top + drawRect.Height / 2).ToString(nfi)
                        );
                        break;
                }
            }
            Out.WriteLine("\" Stroke=" + GetAlphaColor(checkBox.CheckColor) + " StrokeThickness=" + Quoted(checkBox.Border.Width * 1.5F) + ">");
            if (checkBox.Checked && checkBox.CheckedSymbol == CheckedSymbol.Fill)
            {
                Out.WriteLine("<Path.Fill><SolidColorBrush Color=" + GetColor(checkBox.CheckColor) + "/></Path.Fill>");
            }
            Out.WriteLine("</Path>");
        }

        private void AddPictureObject(StreamWriter Out, object p, ReportComponentBase pictureObject, string ImageNameMask)
        {
            XPS_Picture xps_picture = new XPS_Picture(pictureObject);

            float left = FMarginLeft + pictureObject.AbsLeft;
            float right = FMarginLeft + pictureObject.AbsRight;
            float top = FMarginTop + pictureObject.AbsTop;
            float bottom = FMarginTop + pictureObject.AbsBottom;
            
            string picturePath = xps_picture.SaveImage(FExport, pictureObject, true);

            Out.Write("<Path Data=\"");
            Out.Write("M " + left.ToString(nfi) + "," + top.ToString(nfi) + 
                " L " + right.ToString(nfi) + "," + top.ToString(nfi) + 
                " " + right.ToString(nfi) + "," + bottom.ToString(nfi) +
                " " + left.ToString(nfi) + "," + bottom.ToString(nfi) + " z\">");
            Out.WriteLine("<Path.Fill>");

            Out.Write("<ImageBrush TileMode=\"None\" ViewboxUnits=\"Absolute\" ViewportUnits=\"Absolute\"");
            Out.Write(" ImageSource=" + Quoted(picturePath));
            Out.Write(" Viewbox=\"0,0," +
                pictureObject.Width.ToString(nfi) + "," +
                pictureObject.Height.ToString(nfi) + "\"");
            Out.Write(" Viewport=\"" +
                left.ToString(nfi) + "," +
                top.ToString(nfi)  + "," +
                pictureObject.Width.ToString(nfi)   + "," +
                pictureObject.Height.ToString(nfi)  + "\" />");

            Out.WriteLine("</Path.Fill></Path>");

            DrawBorder(Out, pictureObject);

            if (this.AddRelation(RelationCount, xps_picture) == true) RelationCount++;
        }

        private void AddShape(StreamWriter Out, ShapeObject shapeObject)
        {
            float x = FMarginLeft + (shapeObject.AbsLeft + shapeObject.Border.Width / 2);
            float y = FMarginTop + (shapeObject.AbsTop + shapeObject.Border.Width / 2);
            float dx = (shapeObject.Width - shapeObject.Border.Width) - 1;
            float dy = (shapeObject.Height - shapeObject.Border.Width) - 1;
            float x1 = x + dx;
            float y1 = y + dy;

            Out.Write("<Path Data=\"");

            switch (shapeObject.Shape)
            {
                case ShapeKind.Diamond:
                    Out.Write("M " + (x+dx/2).ToString(nfi) + "," + y.ToString(nfi) + " L " +
                        x1.ToString(nfi) + "," + (y+dy/2).ToString(nfi) + " " +
                        (x+dx/2).ToString(nfi) + "," + y1.ToString(nfi) + " " +
                        x.ToString(nfi) + "," + (y+dy/2).ToString(nfi) + " ");
                    break;
                case ShapeKind.Ellipse:
                    x1 = x + dx / 2;
                    dx /= 2;
                    dy /= 2;
                    Out.Write("M " + x1.ToString(nfi) + "," + y.ToString(nfi) +
                        " A " + dx.ToString(nfi) + "," + dy.ToString(nfi) +
                        " 0 1 0 " + (x1 + 0.1).ToString(nfi) + "," + y.ToString(nfi) + " ");
                    break;
                case ShapeKind.Rectangle:
                    Out.Write("M " + x.ToString(nfi) + "," + y.ToString(nfi) + " L " + 
                        x1.ToString(nfi) + "," + y.ToString(nfi) + " " +
                        x1.ToString(nfi) + "," + y1.ToString(nfi) + " " +
                        x.ToString(nfi) + "," + y1.ToString(nfi) + " ");
                    break;
                case ShapeKind.RoundRectangle:
                    float min = Math.Min(dx, dy);
                    if (shapeObject.Curve == 0) min = min / 4; else min = Math.Min(min, shapeObject.Curve * 10);
                    Out.Write("M " + (x + min).ToString(nfi) + "," + y.ToString(nfi) + " L " +
                        (x1 - min).ToString(nfi) + "," + y.ToString(nfi) +
                        " A " + min.ToString(nfi) + "," + min.ToString(nfi) + 
                            " 0 0 1 " + x1.ToString(nfi) + "," + (y + min).ToString(nfi) +
                        " L " + x1.ToString(nfi) + "," + (y1 - min).ToString(nfi) +
                        " A " + min.ToString(nfi) + "," + min.ToString(nfi) + 
                            " 0 0 1 " + (x1 - min).ToString(nfi) + "," + y1.ToString(nfi) +
                        " L " + (x + min).ToString(nfi) + "," + y1.ToString(nfi) +
                        " A " + min.ToString(nfi) + "," + min.ToString(nfi) + 
                            " 0 0 1 " + x.ToString(nfi) + "," + (y1 - min).ToString(nfi) +
                        " L " + x.ToString(nfi) + "," + (y + min).ToString(nfi) +
                        " A " + min.ToString(nfi) + "," + min.ToString(nfi) + 
                            " 0 0 1 " + (x + min).ToString(nfi) + "," + y.ToString(nfi) + " ");

                    break;
                case ShapeKind.Triangle:
                    Out.Write("M " + x1.ToString(nfi) + "," + y1.ToString(nfi) + " L " +
                        x.ToString(nfi) + "," + y1.ToString(nfi) + " " +
                        (x + dx /2).ToString(nfi) + "," + y.ToString(nfi) + " ");
                    break;
                default: throw new Exception("Unsupported shape kind");
            }
            Out.WriteLine("z\" Stroke="+GetAlphaColor(shapeObject.Border.Color)+" StrokeThickness="+Quoted(shapeObject.Border.Width)+" >");
            Out.WriteLine("</Path>");
        }

        private void AddLine(StreamWriter Out, LineObject lineObject)
        {
            float left = FMarginLeft + lineObject.AbsLeft;
            float right = FMarginLeft + lineObject.AbsRight;
            float top = FMarginTop + lineObject.AbsTop;
            float bottom = FMarginTop + lineObject.AbsBottom;

            Out.Write("<Path Data=\"");
            Out.Write("M " + left.ToString(nfi) + "," + top.ToString(nfi) + " L " + right.ToString(nfi) + "," + bottom.ToString(nfi));
            Out.WriteLine("\" Stroke="+GetAlphaColor(lineObject.Border.Color)+" StrokeThickness="+Quoted(lineObject.Border.Width)+" >");
            Out.WriteLine("</Path>");
        }

        private void AddBandObject(StreamWriter Out, BandBase bandBase)
        {
            if (bandBase.HasBorder)
            {
                DrawBorder(Out, bandBase);
            }
            if (bandBase.HasFill) 
            {
                FillRectangle(Out, bandBase);
            }
        }

        private void AddTextObject(StreamWriter Out, TextObject obj)
        {
            float FDpiFX = 96f / DrawUtils.ScreenDpi;
            bool FirstOccurrence;

            FillRectangle(Out, obj);
            DrawShadow(Out, obj);
            DrawBorder(Out, obj);

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            using (Font f = new Font(obj.Font.Name, obj.Font.Size * FDpiFX, obj.Font.Style))
            {
                XPS_Font font = FExport.FontList.AddFont(FExport, f, out FirstOccurrence);
                if (this.AddRelation(RelationCount, font) == true) RelationCount++;
                using (GraphicCache cache = new GraphicCache())
                {
                    RectangleF textRect = new RectangleF(
                      FMarginLeft + obj.AbsLeft + obj.Padding.Left,
                      FMarginTop + obj.AbsTop + obj.Padding.Top,
                      obj.Width - obj.Padding.Horizontal,
                      obj.Height - obj.Padding.Vertical);

                    StringFormat format = obj.GetStringFormat(cache, 0);
                    Brush textBrush = cache.GetBrush(obj.TextColor);
                    AdvancedTextRenderer renderer = new AdvancedTextRenderer(obj.Text, g, f, textBrush,
                        textRect, format, obj.HorzAlign, obj.VertAlign, obj.LineHeight, obj.Angle, obj.FontWidthRatio,
                        obj.ForceJustify, obj.Wysiwyg, obj.HtmlTags, true);

                    foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                    {
                        foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                        {
                            foreach (AdvancedTextRenderer.Word word in line.Words)
                            {
                              float fix_hpos = f.Height * 0.1f; // to match .net char X offset
                              // invert offset in case of rtl
                              if (obj.RightToLeft)
                                fix_hpos = -fix_hpos;
                              // we don't need this offset if text is centered
                              if (obj.HorzAlign == HorzAlign.Center)
                                fix_hpos = 0;  

                              
                                if (renderer.HtmlTags)
                                    foreach (AdvancedTextRenderer.Run run in word.Runs)
                                    {
                                        XPS_Font mod_font = FExport.FontList.AddFont(FExport, run.GetFont(), out FirstOccurrence);
                                        if (this.AddRelation(RelationCount, mod_font) == true) RelationCount++;
                                        Add_Glyphs(Out, obj, mod_font, run.Style.Color, fix_hpos + run.Left, run.Top, run.Width, run.Text);
                                    }
                                else
                                    Add_Glyphs(Out, obj, font, obj.TextColor, fix_hpos + word.Left, word.Top, word.Width, word.Text);
                            }
                        }
                    }
                }
            }
        }

        private void AddTable(StreamWriter Out, TableBase table)
        {
            TableBase frame = new TableBase();

            frame.Assign(table);
            frame.Left = table.AbsLeft;
            frame.Top = table.AbsTop;

            float x = 0;
            float y = 0;
            for (int j = 0; j < table.Columns.Count; j++)
            {
                y = 0;
                for (int i = 0; i < table.RowCount; i++)
                {
                    if (!table.IsInsideSpan(table[j, i]))
                    {
                        TableCell textcell = table[j, i];

                        textcell.Left = x;
                        textcell.Top = y;

                        AddTextObject(Out, textcell);
                    }
                    y += (table.Rows[i]).Height;
                }
                x += (table.Columns[j]).Width;
            }

            frame.Width = x;
            frame.Height = y;
            DrawBorder(Out, frame);
        }

        private void AddTextWatermark(StreamWriter Out, ReportPage page)
        {
            TextObject obj = page.Watermark.TextObject;
            if (obj == null) return;

            RectangleF displayRect = new RectangleF(
                -FMarginLeft,
                -FMarginTop,
                page.PaperWidth * MetrixMultiplier,
                page.PaperHeight * MetrixMultiplier);

            obj.Bounds = displayRect;
            int angle = 0;
            switch (page.Watermark.TextRotation)
            {
                case WatermarkTextRotation.Horizontal:
                    angle = 0;
                    break;
                case WatermarkTextRotation.Vertical:
                    angle = 270;
                    break;
                case WatermarkTextRotation.ForwardDiagonal:
                    angle = 360 - (int)(Math.Atan(displayRect.Height / displayRect.Width) * (180 / Math.PI));
                    break;
                case WatermarkTextRotation.BackwardDiagonal:
                    angle = (int)(Math.Atan(displayRect.Height / displayRect.Width) * (180 / Math.PI));
                    break;
            }
            obj.Angle = angle;

            AddTextObject(Out, obj);
        }

        private void AddBitmapWatermark(StreamWriter Out, ReportPage page)
        {
            PictureObject pic = page.Watermark.PictureObject;
            if (pic.Image == null) return;

            RectangleF displayRect = new RectangleF( 
                -FMarginLeft, 
                -FMarginTop, 
                page.PaperWidth * MetrixMultiplier, 
                page.PaperHeight * MetrixMultiplier);

            pic.Bounds = displayRect;

            PictureBoxSizeMode sizeMode = PictureBoxSizeMode.Normal;
            if (page.Watermark.ImageSize == WatermarkImageSize.Stretch)
                sizeMode = PictureBoxSizeMode.StretchImage;
            else if (page.Watermark.ImageSize == WatermarkImageSize.Zoom)
                sizeMode = PictureBoxSizeMode.Zoom;
            else if (page.Watermark.ImageSize == WatermarkImageSize.Center)
                sizeMode = PictureBoxSizeMode.CenterImage;
            pic.SizeMode = sizeMode;
            pic.Tile = page.Watermark.ImageSize == WatermarkImageSize.Tile;

            AddPictureObject(Out, rId, pic, "BackgroundPic");
        }

        #endregion

        internal XPS_PageContent(int pageNo)
        {
            PageNumber = pageNo;
        }
    }

    /// <summary>
    ///  Document structure descriptor
    /// </summary>
    internal class XPS_FixedDocument : OoXMLBase
    {
        #region "Class overrides"
        public override string RelationType { get { return null; } } // "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"; } }
        public override string ContentType { get { return null; } } //"application/vnd.openxmlformats-officedocument.presentationml.slide+xml"; } }
        public override string FileName { get { return "/Documents/1/FixedDocument.fdoc"; } }
        #endregion

        #region Private fields
        private ArrayList FPageList;
        #endregion

        internal void AddPage(XPS_PageContent page)
        {
            FPageList.Add(page);
        }

        internal void Export(XPSExport export)
        {
            using (FileStream file = new FileStream(export.TempFolder + FileName, FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                Out.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                Out.WriteLine("<FixedDocument xmlns=\"http://schemas.microsoft.com/xps/2005/06\">");
                foreach(XPS_PageContent page in FPageList)
                {
                    Out.WriteLine("<PageContent Source=" + Quoted(page.FileName) + "/>");
                }
                Out.WriteLine("</FixedDocument>");
            }
        }

        internal XPS_FixedDocument()
        {
            FPageList = new ArrayList();
        }
    }

    /// <summary>
    ///  Main class of XML export
    /// </summary>
    class XPSExport : OOExportBase, IDisposable
    {
        #region Private fields
        private OoXMLCoreDocumentProperties FCoreDocProp;
        private XPS_FixedDocument           FFixedDocument;
        private XPS_FontList                FFontList;
        #endregion

        #region Internal properties
        internal XPS_FontList FontList { get { return FFontList; } }
        #endregion

        #region Public fields
        public int PicturesCount;
        public bool HumanReadable; 
        #endregion

        #region Private methods
        private void CreateContentTypes()
        {
            using (FileStream file = new FileStream(FTempFolder + "\\[Content_Types].xml", FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                Out.WriteLine(xml_header);
                Out.WriteLine("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
                Out.WriteLine("<Default Extension=\"png\" ContentType=\"image/png\" /> ");
                Out.WriteLine("<Default Extension=\"jpeg\" ContentType=\"image/jpg\" /> ");
                Out.WriteLine("<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\" /> ");
                Out.WriteLine("<Default Extension=\"xml\" ContentType=\"application/xml\" /> ");
                Out.WriteLine("<Default Extension=\"fdseq\" ContentType=\"application/vnd.ms-package.xps-fixeddocumentsequence+xml\" /> ");
                Out.WriteLine("<Default Extension=\"fpage\" ContentType=\"application/vnd.ms-package.xps-fixedpage+xml\" /> ");
                Out.WriteLine("<Default Extension=\"struct\" ContentType=\"application/vnd.ms-package.xps-documentstructure+xml\" /> ");
                Out.WriteLine("<Default Extension=\"jpg\" ContentType=\"image/jpeg\" /> ");
                Out.WriteLine("<Default Extension=\"odttf\" ContentType=\"application/vnd.ms-package.obfuscated-opentype\" /> ");
                Out.WriteLine("<Default Extension=\"fdoc\" ContentType=\"application/vnd.ms-package.xps-fixeddocument+xml\" /> ");
                Out.WriteLine("<Override PartName=\"/docProps/core.xml\" ContentType=\"application/vnd.openxmlformats-package.core-properties+xml\" /> ");
                Out.WriteLine("</Types>");
            }
        }
        private void CreateRelations()
        {
            using (FileStream file = new FileStream(FTempFolder + "\\_rels\\.rels", FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                Out.WriteLine(xml_header);
                Out.WriteLine("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
                Out.WriteLine("<Relationship Id=\"rId3\" Type=" + Quoted(FCoreDocProp.RelationType) + " Target=" + Quoted(FCoreDocProp.FileName) + " /> ");
                //                Out.WriteLine("<Relationship Id="rId2" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail" Target="docProps/thumbnail.jpeg" /> ");
                Out.WriteLine("<Relationship Id=\"rId1\" Type=\"http://schemas.microsoft.com/xps/2005/06/fixedrepresentation\" Target=\"FixedDocSeq.fdseq\" /> ");
                Out.WriteLine("</Relationships>");
            }
        }
        private void CreateFixedDocumentSequence()
        {
            using (FileStream file = new FileStream(TempFolder + "/" + "FixedDocSeq.fdseq", FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                Out.WriteLine(xml_header);
                Out.WriteLine("<FixedDocumentSequence xmlns=\"http://schemas.microsoft.com/xps/2005/06\">");
                Out.WriteLine("<DocumentReference Source=\"/Documents/1/FixedDocument.fdoc\"/>");
                Out.WriteLine("</FixedDocumentSequence>");
            }

        }
        
        private void ExportXPS(Stream Stream)
        {
            CreateContentTypes();
            CreateRelations();
            CreateFixedDocumentSequence();

            FCoreDocProp.Export(this);
            FFixedDocument.Export(this);
            FFontList.Export(this);

            ZipArchive zip = new ZipArchive();
            zip.AddDir(FTempFolder);
            zip.SaveToStream(Stream);
            Directory.Delete(FTempFolder, true);

        }


        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (XpsExportForm form = new XpsExportForm())
            {
                form.Init(this);
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            Directory.CreateDirectory(FTempFolder);
            Directory.CreateDirectory(FTempFolder + "\\_rels");
            Directory.CreateDirectory(FTempFolder + "\\docProps");
            Directory.CreateDirectory(FTempFolder + "\\Documents");
            Directory.CreateDirectory(FTempFolder + "\\Documents\\1");
            Directory.CreateDirectory(FTempFolder + "\\Documents\\1\\Pages");
            Directory.CreateDirectory(FTempFolder + "\\Resources");
        }

        /// <inheritdoc/>
        protected override void ExportPage(int pageNo)
        {
            //const long EMUpmm = 36000;

            //this.FPaperWidth = (long)(FirstPage.PaperWidth * EMUpmm);
            //this.FPaperHeight = (long)(FirstPage.PaperHeight * EMUpmm);
            //this.FLeftMargin = (long)(FirstPage.LeftMargin * EMUpmm);
            //this.FTopMargin = (long)(FirstPage.TopMargin * EMUpmm);

            XPS_PageContent page = new XPS_PageContent(pageNo+1);

            //if (pageNo == 0) page.Reset();
            FFixedDocument.AddPage(page);
            //int relatives_count = FPresentation.RelationList.Count;
            //FPresentation.AddRelation(relatives_count + 1, page);

            page.Export(this, GetPage(pageNo));
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            ExportXPS(Stream);
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("XpsFile");
        }
        #endregion

        public XPSExport()
        {
            FTempFolder = Config.GetTempFolder() + Path.GetRandomFileName();

            PicturesCount = 0;
            HumanReadable = false;

            FCoreDocProp = new OoXMLCoreDocumentProperties();
            FFixedDocument = new XPS_FixedDocument();
            FFontList = new XPS_FontList();
        }

        public new void Dispose()
        {
            base.Dispose();
            FFontList.Dispose();
        }

    }
}
