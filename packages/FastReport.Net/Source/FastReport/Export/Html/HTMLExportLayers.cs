// to-do: page breaks
// to-do: improve draw the borders 

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FastReport.Utils;
using FastReport.Table;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections;

namespace FastReport.Export.Html
{
    /// <summary>
    /// Represents the HTML export filter.
    /// </summary>
    public partial class HTMLExport : ExportBase
    {
        private string pageAncor;
        private bool doPageBreak;

        private string GetStyle(Font Font, Color TextColor, Color FillColor, bool RTL, HorzAlign HAlign, Border Border)
        {
            StringBuilder style = new StringBuilder(512);

            if (Font != null)
            {
                if (Zoom != 1)
                {
                    Font newFont = new Font(Font.FontFamily, Font.Size * Zoom, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
                    HTMLFontStyle(style, newFont);
                }
                else
                    HTMLFontStyle(style, Font);
            }

            style.Append("text-align:");
            if (HAlign == HorzAlign.Left)
                style.Append(RTL ? "right" : "left");
            else if (HAlign == HorzAlign.Right)
                style.Append(RTL ? "left" : "right");
            else if (HAlign == HorzAlign.Center)
                style.Append("center");
            else
                style.Append("justify");
            style.Append(";");            

            style.Append(
                String.Join(String.Empty, new String[] {
                "position:absolute;overflow:hidden;color:", 
                ExportUtils.HTMLColor(TextColor),
                ";background-color:",
                FillColor == Color.Transparent ? "transparent" : ExportUtils.HTMLColor(FillColor), ";", 
                RTL ? "direction:rtl;" : String.Empty }));

            Border newBorder = Border; //.Clone();

            newBorder.LeftLine.Width *= Zoom;
            if (newBorder.LeftLine.Width > 0 && newBorder.LeftLine.Width < 1)
                newBorder.LeftLine.Width = 1;

            newBorder.RightLine.Width *= Zoom;
            if (newBorder.RightLine.Width > 0 && newBorder.RightLine.Width < 1)
                newBorder.RightLine.Width = 1;

            newBorder.TopLine.Width *= Zoom;
            if (newBorder.TopLine.Width > 0 && newBorder.TopLine.Width < 1)
                newBorder.TopLine.Width = 1;

            newBorder.BottomLine.Width *= Zoom;
            if (newBorder.BottomLine.Width > 0 && newBorder.BottomLine.Width < 1)
                newBorder.BottomLine.Width = 1;

            HTMLBorder(style, newBorder);

            return style.ToString();
        }

        private int UpdateCSSTable(ReportComponentBase obj)
        {
            string style;
            if (obj is TextObject)
            {
                TextObject textObj = obj as TextObject;
                style = GetStyle(textObj.Font, textObj.TextColor, textObj.FillColor, textObj.RightToLeft, textObj.HorzAlign, textObj.Border);
            }
            else
                style = GetStyle(null, Color.White, obj.FillColor, false, HorzAlign.Center, obj.Border);
            return UpdateCSSTable(style);
        }

        private int UpdateCSSTable(string style)
        {
            int i = FCSSStyles.IndexOf(style);
            if (i == -1)
            {
                i = FCSSStyles.Count;
                FCSSStyles.Add(style);
            }
            return i;
        }

        private void ExportPageStylesLayers(StringBuilder styles, int PageNumber)
        {
            if (FPrevStyleListIndex < FCSSStyles.Count)
            {
                styles.Append(HTMLGetStylesHeader(PageNumber));
                for (int i = FPrevStyleListIndex; i < FCSSStyles.Count; i++)
                    styles.Append(HTMLGetStyleHeader(i)).Append(FCSSStyles[i]).AppendLine("}");
                styles.AppendLine(HTMLGetStylesFooter());
            }
        }

        private string GetStyleTag(int index)
        {           
            return String.Join(String.Empty, new String[] { "class=\"", FStylePrefix, "s", index.ToString(), "\"" });
        }

        private void Layer(StringBuilder Page, float Left, float Top, float Width, float Height, string Text, string style, string addstyletag)
        {
            Page.Append(String.Join(String.Empty, new String[] { 
                "<div ",
                style,
                " style=\"",                        
                "left:", 
                Px(Left * Zoom),
                "top:",
                Px(Top * Zoom) } ));

            if (Width != 0)
                Page.Append("width:").Append(Px(Width * Zoom));
            if (Height != 0)
                Page.Append("height:").Append(Px(Height * Zoom));
            
            Page.Append(addstyletag);
            Page.Append("\">");

            if (!String.IsNullOrEmpty(pageAncor))
            {
                Page.Append(pageAncor);
                pageAncor = String.Empty;
            }

            Page.Append(String.IsNullOrEmpty(Text) ? NBSP : Text);

            Page.AppendLine("</div>");
        }

        private string GetSpanText(string text, float top, float width, Padding padding)
        {
            StringBuilder sb = new StringBuilder(48);

            // not work correct in Opera, good for IE and FF
            //if (top > 0)
            //    sb.Append("<div ").
            //        Append(GetStyleTag(UpdateCSSTable("border:0;font-size:1px;height:" + Px((top - 1) * Zoom) ))).
            //        Append(">&nbsp;</div>");

            StringBuilder style = new StringBuilder(32);
            style.Append("display:block;border:0;width:").Append(Px(width * Zoom));
            if (padding.Left != 0)
                style.Append("padding-left:").Append(Px(padding.Left));
            if (padding.Right != 0)
                style.Append("padding-right:").Append(Px(padding.Right));
            if (top != 0)
                style.Append("margin-top:").Append(Px(top * Zoom));

            sb.Append("<div ").
                Append(GetStyleTag(UpdateCSSTable(style.ToString()))).
                Append(">").
                Append(text).
                Append("</div>");

            return sb.ToString();
        }

        private void LayerText(StringBuilder Page, TextObject obj)
        {
            float top = 0;
            if (obj.VertAlign != VertAlign.Top)
            {
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                using (Font f = new Font(obj.Font.Name, obj.Font.Size, obj.Font.Style))
                {
                    RectangleF textRect = new RectangleF(obj.AbsLeft, obj.AbsTop, obj.Width, obj.Height);
                    StringFormat format = obj.GetStringFormat(Report.GraphicCache, 0);
                    Brush textBrush = Report.GraphicCache.GetBrush(obj.TextColor);
                    AdvancedTextRenderer renderer = new AdvancedTextRenderer(obj.Text, g, f, textBrush,
                        textRect, format, obj.HorzAlign, obj.VertAlign, obj.LineHeight, obj.Angle, obj.FontWidthRatio,
                        obj.ForceJustify, obj.Wysiwyg, obj.HtmlTags, false);
                    if (renderer.Paragraphs.Count > 0)
                        if (renderer.Paragraphs[0].Lines.Count > 0)
                            top = renderer.Paragraphs[0].Lines[0].Top - obj.AbsTop;
                }
            }

            LayerBack(Page, obj,
                GetSpanText(ExportUtils.HtmlString(obj.Text, obj.HtmlTags),
                top + obj.Padding.Top,
                obj.Width - obj.Padding.Horizontal,
                obj.Padding));
        }

        private string GetLayerPicture(ReportComponentBase obj, out float Width, out float Height)
        {                        
            string result = String.Empty;
            Width = 0;
            Height = 0;

            if (obj != null)
            {
                if (FPictures)
                {
                    MemoryStream PictureStream = new MemoryStream();
                    System.Drawing.Imaging.ImageFormat FPictureFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                    if (FImageFormat == ImageFormat.Png)
                        FPictureFormat = System.Drawing.Imaging.ImageFormat.Png;
                    else if (FImageFormat == ImageFormat.Jpeg)
                        FPictureFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    else if (FImageFormat == ImageFormat.Gif)
                        FPictureFormat = System.Drawing.Imaging.ImageFormat.Gif;

                    Width = obj.Width == 0 ? obj.Border.LeftLine.Width : obj.Width;
                    Height = obj.Height == 0 ? obj.Border.TopLine.Width : obj.Height;

                    if (Math.Abs(Width) * Zoom < 1 && Zoom > 0)
                        Width = 1 / Zoom;

                    if (Math.Abs(Height) * Zoom < 1 && Zoom > 0)
                        Height = 1 / Zoom;

                    using (System.Drawing.Image image = new Bitmap((int)(Math.Abs(Width * Zoom)), (int)(Math.Abs(Height * Zoom))))
                    {
                        using (Graphics g = Graphics.FromImage(image))
                        {
                            if (obj is TextObjectBase)
                                g.Clear(Color.White);

                            float Left = Width > 0 ? obj.AbsLeft : obj.AbsLeft + Width;
                            float Top = Height > 0 ? obj.AbsTop : obj.AbsTop + Height;

                            float dx = 0;// (obj.Border.Lines & BorderLines.Left) != 0 ? obj.Border.LeftLine.Width : 0; !!! fix for borders
                            float dy = 0;// (obj.Border.Lines & BorderLines.Top) != 0 ? obj.Border.TopLine.Width : 0;
                            g.TranslateTransform((-Left - dx) * Zoom, (-Top - dy) * Zoom);

                            BorderLines oldLines = obj.Border.Lines;
                            obj.Border.Lines = BorderLines.None;
                            obj.Draw(new FRPaintEventArgs(g, Zoom, Zoom, Report.GraphicCache));
                            obj.Border.Lines = oldLines;
                        }

                        if (FPictureFormat == System.Drawing.Imaging.ImageFormat.Jpeg)
                        {
                            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                            ImageCodecInfo ici = null;
                            foreach (ImageCodecInfo codec in codecs)
                            {
                                if (codec.MimeType == "image/jpeg")
                                {
                                    ici = codec;
                                    break;
                                }
                            }
                            EncoderParameters ep = new EncoderParameters();
                            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 95);
                            image.Save(PictureStream, ici, ep);
                        }
                        else
                            image.Save(PictureStream, FPictureFormat);
                    }
                    PictureStream.Position = 0;

                    result = HTMLGetImage(0, 0, 0, ExportUtils.Crc32(PictureStream, PictureStream.Length - 128, 128).ToString(), true, null, PictureStream);
                }
            }
            return result;
        }

        private void LayerPicture(StringBuilder Page, ReportComponentBase obj, string text)
        {
            if (FPictures)
            {
                int styleindex = UpdateCSSTable(obj);
                string style = GetStyleTag(styleindex);
                string old_text = String.Empty;

                if (IsMemo(obj))
                {
                    old_text = (obj as TextObject).Text;
                    (obj as TextObject).Text = String.Empty;
                }
                
                float Width, Height;
                string pic = GetLayerPicture(obj, out Width, out Height);

                if (IsMemo(obj))
                    (obj as TextObject).Text = old_text;

                string addstyle = String.Join(String.Empty, new String[] { " background: url('", pic, "') no-repeat;" });

                if (String.IsNullOrEmpty(text))
                    text = NBSP;

                float x = Width > 0 ? obj.AbsLeft : (obj.AbsLeft + (float)Math.Truncate(Width));
                float y = Height > 0 ? FHPos + obj.AbsTop : (FHPos + obj.AbsTop + (float)Math.Truncate(Height));

                Layer(Page, x, y, (float)Math.Abs(Math.Truncate(Width)), (float)Math.Abs(Math.Truncate(Height)), text, style, addstyle);
            }
        }

        private void LayerBack(StringBuilder Page, ReportComponentBase obj, string text)
        {
            if (obj.Border.Shadow)
            {
                using (TextObject shadow = new TextObject())
                {
                    shadow.Left = obj.AbsLeft + obj.Border.ShadowWidth;
                    shadow.Top = obj.AbsTop + obj.Height;
                    shadow.Width = obj.Width;
                    shadow.Height = obj.Border.ShadowWidth;
                    shadow.FillColor = obj.Border.ShadowColor;
                    shadow.Border.Lines = BorderLines.None;
                    LayerBack(Page, shadow, String.Empty);

                    shadow.Left = obj.AbsLeft + obj.Width;
                    shadow.Top = obj.AbsTop + obj.Border.ShadowWidth;
                    shadow.Width = obj.Border.ShadowWidth;
                    shadow.Height = obj.Height;
                    LayerBack(Page, shadow, String.Empty);
                }
            }

            if (obj.Fill is SolidFill)
                Layer(Page, obj.AbsLeft, FHPos + obj.AbsTop, obj.Width, obj.Height, text, GetStyleTag(UpdateCSSTable(obj)), String.Empty);
            else
                LayerPicture(Page, obj, text);
        }

        private void LayerTable(StringBuilder Page, TableBase table)
        {
            float y = 0;
            for (int i = 0; i < table.RowCount; i++)
            {
                float x = 0;
                for (int j = 0; j < table.ColumnCount; j++)
                {
                    if (!table.IsInsideSpan(table[j, i]))
                    {
                        TableCell textcell = table[j, i];
                        textcell.Left = x;
                        textcell.Top = y;
                        if (textcell is TextObject)
                            LayerText(Page, textcell as TextObject);
                        else
                        {
                            LayerBack(Page, textcell as ReportComponentBase, String.Empty);
                            LayerPicture(Page, textcell as ReportComponentBase, String.Empty);
                        }
                    }
                    x += (table.Columns[j]).Width;
                }
                y += (table.Rows[i]).Height;
            }
        }

        private bool IsMemo(ReportComponentBase Obj)
        {
            return (Obj is TextObject && ((Obj as TextObject).Angle == 0) && (Obj as TextObject).FontWidthRatio == 1);
        }

        private void Watermark(StringBuilder Page, ReportPage page, bool drawText)
        {
            using (PictureObject pictureWatermark = new PictureObject())
            {
                pictureWatermark.Left = 0;
                pictureWatermark.Top = 0;

                pictureWatermark.Width = (page.PaperWidth - page.LeftMargin - page.RightMargin) * Units.Millimeters;
                pictureWatermark.Height = (page.PaperHeight - page.TopMargin - page.BottomMargin) * Units.Millimeters;

                pictureWatermark.SizeMode = PictureBoxSizeMode.Normal;
                pictureWatermark.Image = new Bitmap((int)pictureWatermark.Width, (int)pictureWatermark.Height);

                using (Graphics g = Graphics.FromImage(pictureWatermark.Image))
                {
                    g.Clear(Color.Transparent);
                    if (drawText)
                        page.Watermark.DrawText(new FRPaintEventArgs(g, Zoom, Zoom, Report.GraphicCache),
                            new RectangleF(0, 0, pictureWatermark.Width, pictureWatermark.Height), Report, true);
                    else
                        page.Watermark.DrawImage(new FRPaintEventArgs(g, Zoom, Zoom, Report.GraphicCache),
                            new RectangleF(0, 0, pictureWatermark.Width, pictureWatermark.Height), Report, true);
                    pictureWatermark.Transparency = page.Watermark.ImageTransparency;
                    LayerBack(Page, pictureWatermark, String.Empty);
                    LayerPicture(Page, pictureWatermark, String.Empty);
                }
            }
        }

        private void ExportHTMLPageLayered(HTMLThreadData d)
        {
            float MaxWidth = 0, MaxHeight = 0;

            if (!FSinglePage)
                FCSSStyles.Clear();

            doPageBreak = (FSinglePage && d.PageNumber > 1 && FPageBreaks);

            using (ReportPage reportPage = GetPage(d.ReportPage))
            {
                StringBuilder CSS = new StringBuilder(256);
                StringBuilder Page = new StringBuilder(2048);

                Page = ExportHTMLPageStart(Page, d.PageNumber, d.CurrentPage);

                pageAncor = HTMLGetAncor(d.PageNumber.ToString());

                if (reportPage.Watermark.Enabled && !reportPage.Watermark.ShowImageOnTop)
                    Watermark(Page, reportPage, false);

                if (reportPage.Watermark.Enabled && !reportPage.Watermark.ShowTextOnTop)
                    Watermark(Page, reportPage, true);

                foreach(Base c in reportPage.AllObjects)
                {
                    if (c is ReportComponentBase)
                    {
                        ReportComponentBase obj = c as ReportComponentBase;
                        float val = obj.AbsTop + obj.Height;
                        if (val > MaxHeight)
                            MaxHeight = val;
                        val = obj.AbsLeft + obj.Width;
                        if (val > MaxWidth)
                            MaxWidth = val;
                        if (obj is CellularTextObject)
                            obj = (obj as CellularTextObject).GetTable();
                        if (obj is TableCell)
                            continue;
                        else if (obj is TableBase)
                        {
                            TableBase table = obj as TableBase;
                            if (table.ColumnCount > 0 && table.RowCount > 0)
                            {
                                using (TextObject tableback = new TextObject())
                                {
                                    tableback.Border = table.Border;
                                    tableback.Fill = table.Fill;
                                    tableback.FillColor = table.FillColor;
                                    tableback.Left = table.AbsLeft;
                                    tableback.Top = table.AbsTop;
                                    float tableWidth = 0;
                                    float tableHeight = 0;
                                    for (int i = 0; i < table.ColumnCount; i++)
                                        tableWidth += table[i, 0].Width;
                                    for (int i = 0; i < table.RowCount; i++)
                                        tableHeight += table.Rows[i].Height;
                                    tableback.Width = (tableWidth < table.Width) ? tableWidth : table.Width;
                                    tableback.Height = tableHeight;
                                    LayerText(Page, tableback);
                                }
                                LayerTable(Page, table);
                            }
                        }
                        else if (IsMemo(obj))
                        {
                            LayerText(Page, obj as TextObject);
                        }
                        else if (obj is BandBase)
                        {
                            LayerBack(Page, obj, String.Empty);
                        }
                        else if (obj is LineObject)
                        {
                            LayerPicture(Page, obj, String.Empty);
                        }
                        else
                        {
                            LayerBack(Page, obj, String.Empty);
                            LayerPicture(Page, obj, String.Empty);
                        }                        
                    }
                }

                if (reportPage.Watermark.Enabled && reportPage.Watermark.ShowImageOnTop)
                    Watermark(Page, reportPage, false);

                if (reportPage.Watermark.Enabled && reportPage.Watermark.ShowTextOnTop)
                    Watermark(Page, reportPage, true);

                ExportPageStylesLayers(CSS, d.PageNumber);

                if (FSinglePage)
                {
                    FHPos += MaxHeight;
                    FPrevStyleListIndex = FCSSStyles.Count;
                }
                
                ExportHTMLPageFinal(CSS, Page, d, MaxWidth, MaxHeight);
            }

            if (!FSinglePage && FThreaded)
                if (d.PageNumber == 1)
                    FFirstPageEvent.Set();
        }
    }
}
