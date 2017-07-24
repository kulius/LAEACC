    using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

using FastReport.Utils;
using FastReport.Forms;
using FastReport.Export;
using FastReport.Format;

using System.Globalization;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using FastReport.Table;

namespace FastReport.Export.OoXML
{
    abstract class OoXMLGenerator : OoXMLBase
    {
        protected void Export(OOExportBase OoXML, string resource_file)
        {
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();

            Stream o = a.GetManifestResourceStream(resource_file);

            int length = 4096;
            int bytesRead = 0;
            Byte[] buffer = new Byte[length];

            using (FileStream fs = new FileStream(OoXML.TempFolder + "/" +  FileName, FileMode.Create))
            {
                do
                {
                    bytesRead = o.Read(buffer, 0, length);
                    fs.Write(buffer, 0, bytesRead);
                } while (bytesRead == length);
            }

            o.Dispose();
        }
    }

    class OoXMLWordStyles : OoXMLGenerator
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"; } }
        public override string FileName { get { return "word/styles.xml"; } }
        #endregion

        public void Export(OOExportBase OoXML) { Export( OoXML, "FastReport.Export.OoXML.styles.xml" ); }
    }

    class OoXMLWordSettings : OoXMLGenerator
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml"; } }
        public override string FileName { get { return "word/settings.xml"; } }
        #endregion

        public void Export(OOExportBase OoXML) { Export(OoXML, "FastReport.Export.OoXML.settings.xml"); }
    }

    class OoXMLFontTable : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/fontTable"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.fontTable+xml"; } }
        public override string FileName { get { return "word/fontTable.xml"; } }
        #endregion

        internal void Export(Word2007Export OoXML)
        {
            using (FileStream file = new FileStream(OoXML.TempFolder + "/" + FileName, FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                Out.WriteLine(xml_header);
                Out.WriteLine("<w:fonts xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">");
                Out.WriteLine("<w:font w:name=\"Tahoma\">");
                Out.WriteLine("<w:panose1 w:val=\"020B0604030504040204\" />");
                Out.WriteLine("<w:charset w:val=\"CC\" />");
                Out.WriteLine("<w:family w:val=\"swiss\" />");
                Out.WriteLine("<w:pitch w:val=\"variable\" />");
                Out.WriteLine("<w:sig w:usb0=\"E1002AFF\" w:usb1=\"C000605B\" w:usb2=\"00000029\" w:usb3=\"00000000\" w:csb0=\"000101FF\" w:csb1=\"00000000\" />");
                Out.WriteLine("</w:font>");
                Out.WriteLine("</w:fonts>");
            }
        }
    }

    class OoPictureObject : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image"; } }
        public override string ContentType { get { throw new Exception("Content type not defined"); } }
        public override string FileName { get { return ImageFileName; } }
        #endregion

        string ImageFileName;

        internal void Export(ReportComponentBase pictureObject, string Directory, bool Background)
        {
            float printZoom = 1;
            bool ClearBackground = true;
            System.Drawing.Imaging.ImageFormat image_format = System.Drawing.Imaging.ImageFormat.Png;

            using (System.Drawing.Image image = new System.Drawing.Bitmap((int)Math.Round(pictureObject.Width * printZoom), (int)Math.Round(pictureObject.Height * printZoom)))
            using (Graphics g = Graphics.FromImage(image))
            using (GraphicCache cache = new GraphicCache())
            {
                g.TranslateTransform(-pictureObject.AbsLeft * printZoom, -pictureObject.AbsTop * printZoom);
                if (ClearBackground)
                {
                    g.Clear(Color.White);
                }
                if( Background )
                    pictureObject.DrawBackground(new FRPaintEventArgs(g, printZoom, printZoom, cache));
                else
                    pictureObject.Draw(new FRPaintEventArgs(g, printZoom, printZoom, cache));
                image.Save(Directory + "/" +ImageFileName, image_format);
            }
        }

        internal OoPictureObject(string FileName)
        {
            ImageFileName = FileName;
        }
    }

    class OoXMLDocument : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"; } }
        public override string FileName { get { return "word/document.xml"; } }
        #endregion

        #region "Private properties"
        OOExportBase                            FWordExport;
        StringBuilder                           FTextStrings;
        Dictionary<string, OoPictureObject>     CheckboxList;
        int                                     PictureCount;
        #endregion

        #region "Private methods"
        private string GetRGBString(Color c)
        {
            return "\"#" + /*ExportUtils.ByteToHex(c.A) +*/ ExportUtils.ByteToHex(c.R) + ExportUtils.ByteToHex(c.G) + ExportUtils.ByteToHex(c.B) + "\"";
        }

        private string TranslateText(string text)
        {
            StringBuilder TextStrings = new StringBuilder();
            int start_idx = 0;

            while (true)
            {
                int idx = text.IndexOfAny("&<>".ToCharArray(), start_idx);
                if (idx != -1)
                {
                    TextStrings.Append(text.Substring(start_idx, idx - start_idx));
                    switch (text[idx])
                    {
                        case '&': TextStrings.Append("&amp;"); break;
                        case '<': TextStrings.Append("&lt;"); break;
                        case '>': TextStrings.Append("&gt;"); break;
                    }
                    start_idx = ++idx;
                    continue;
                }
                TextStrings.Append(text.Substring(start_idx));
                break;
            }

            return TextStrings.ToString();
        }
        #endregion

        #region "Tables postponed"
        private void Export_TableProperties(StreamWriter Out)
        { 
            Out.WriteLine("<w:tblPr>");
#if false
            Out.WriteLine("<w:tblW w:w=\"0\" w:type=\"auto\" />");
            Out.WriteLine("<w:tblInd w:w=\"15\" w:type=\"dxa\" />");
            Out.WriteLine("<w:tblLayout w:type=\"fixed\" />");
            Out.WriteLine("<w:tblCellMar>");
            Out.WriteLine("<w:left w:w=\"15\" w:type=\"dxa\" />");
            Out.WriteLine("<w:right w:w=\"15\" w:type=\"dxa\" />");
            Out.WriteLine("</w:tblCellMar>");
            Out.WriteLine("<w:tblLook w:val=\"0000\" />");
#else
Out.WriteLine("<w:tblW w:w=\"0\" w:type=\"auto\" />");
Out.WriteLine("<w:tblBorders>");
Out.WriteLine("<w:top w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
Out.WriteLine("<w:left w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
Out.WriteLine("<w:bottom w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
Out.WriteLine("<w:right w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
Out.WriteLine("<w:insideH w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
Out.WriteLine("<w:insideV w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
Out.WriteLine("</w:tblBorders>");
#endif
            Out.WriteLine("</w:tblPr>");
        }

        private void Export_Picture(ExportIEMObject Obj, StreamWriter Out)
        {

            PictureCount++; // Increase picture counter

            OoPictureObject picture = new OoPictureObject("word/media/image" + PictureCount.ToString() + ".png");
            this.AddRelation(PictureCount + 10, picture);

            using (FileStream pngFile = new FileStream(FWordExport.TempFolder + "\\word\\media\\image" + PictureCount.ToString() + ".png", FileMode.Create))
            {
                pngFile.Write(Obj.PictureStream.ToArray(), 0, (int)Obj.PictureStream.Length);
            }

            long cx = (long) (Obj.Width * 360000 / 37.8f);
            long cy = (long) (Obj.Height * 360000 / 37.8f); 

            Out.WriteLine("<w:p><w:r>");
            Out.WriteLine("<w:rPr><w:noProof /></w:rPr>");
            Out.WriteLine("<w:drawing>");
              Out.WriteLine("<wp:inline distT=\"0\" distB=\"0\" distL=\"0\" distR=\"0\">");
              Out.WriteLine("<wp:extent cx="+Quoted(cx)+" cy="+Quoted(cy)+" />");
//              Out.WriteLine("<wp:effectExtent l="19050" t="0" r="0" b="0" />");
              Out.WriteLine("<wp:docPr id="+Quoted(PictureCount)+" name="+Quoted(PictureCount)+" descr=\"Autogenerated\" />");
              Out.WriteLine("<wp:cNvGraphicFramePr>");
                Out.WriteLine("<a:graphicFrameLocks xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" noChangeAspect=\"1\" />");
              Out.WriteLine("</wp:cNvGraphicFramePr>");
              Out.WriteLine("<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">");
                Out.WriteLine("<a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">");
                  Out.WriteLine("<pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">");
                    
                    Out.WriteLine("<pic:nvPicPr>");
                    Out.WriteLine("<pic:cNvPr id="+Quoted(PictureCount)+" name="+Quoted(PictureCount)+" />");
                    Out.WriteLine("<pic:cNvPicPr />");
                    Out.WriteLine("</pic:nvPicPr>");

                    Out.WriteLine("<pic:blipFill><a:blip r:embed="+Quoted(picture.rId)+" /><a:stretch><a:fillRect /></a:stretch></pic:blipFill>");
                    
                    Out.WriteLine("<pic:spPr>");
                    Out.WriteLine("<a:xfrm><a:off x=\"0\" y=\"0\" /><a:ext cx="+Quoted(cx)+" cy="+Quoted(cy)+" /></a:xfrm>");
                    Out.WriteLine("<a:prstGeom prst=\"rect\"><a:avLst /></a:prstGeom>");
                    Out.WriteLine("</pic:spPr>");

                  Out.WriteLine("</pic:pic>");
                Out.WriteLine("</a:graphicData>");
              Out.WriteLine("</a:graphic>");
              Out.WriteLine("</wp:inline>");
            Out.WriteLine("</w:drawing>");
            Out.WriteLine("</w:r></w:p>");
        }

        private void Export_Paragraph(ExportIEMObject Obj, StreamWriter Out)
        {
            float FDpiFX = 96f / DrawUtils.ScreenDpi;

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            using (Font f = new Font(Obj.Style.Font.Name, Obj.Style.Font.Size * FDpiFX, Obj.Style.Font.Style))
            using (GraphicCache cache = new GraphicCache())
            {
                RectangleF textRect = new RectangleF( Obj.Left, Obj.Top, Obj.Width, Obj.Height);

                StringAlignment align = StringAlignment.Near;
                if (Obj.Style.HAlign == HorzAlign.Center)
                    align = StringAlignment.Center;
                else if (Obj.Style.HAlign == HorzAlign.Right)
                    align = StringAlignment.Far;

                StringAlignment lineAlign = StringAlignment.Near;
                if (Obj.Style.VAlign == VertAlign.Center)
                    lineAlign = StringAlignment.Center;
                else if (Obj.Style.VAlign == VertAlign.Bottom)
                    lineAlign = StringAlignment.Far;

                StringFormatFlags flags = 0;
                if (Obj.Style.RTL)
                    flags |= StringFormatFlags.DirectionRightToLeft;
                //if (! Obj.Style)
                //    flags |= StringFormatFlags.NoWrap;
                //if (! Obj.Style)
                //    flags |= StringFormatFlags.NoClip;
                StringFormat format = cache.GetStringFormat(align, lineAlign, StringTrimming.Word, flags, Obj.Style.FirstTabOffset, Obj.Style.FirstTabOffset/*FTabWidth*/);

                Brush textBrush = cache.GetBrush(Obj.Style.TextColor);
                AdvancedTextRenderer renderer = new AdvancedTextRenderer(Obj.Text, g, f, textBrush,
                    textRect, format, Obj.Style.HAlign, Obj.Style.VAlign,
                    Obj.Style.Font.Height, Obj.Style.Angle, /*textObject.FontWidthRatio*/1,
                    Obj.Style.HAlign == HorzAlign.Justify, true, Obj.HtmlTags, true);

                float w = f.Height * 0.1f; // to match .net char X offset

                foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                {
                    string halign="";
                    switch (Obj.Style.HAlign)
                    {
                        case HorzAlign.Left: halign = "left"; break;
                        case HorzAlign.Right: halign = "right"; break;
                        case HorzAlign.Center: halign = "center"; break;
                        case HorzAlign.Justify: halign = "both"; break;
                    }
                    FTextStrings.AppendLine("<w:p><w:pPr><w:jc w:val=" + Quoted(halign) + " /></w:pPr>");

                    if (renderer.HtmlTags)
                        foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                        {
                            foreach (AdvancedTextRenderer.Word word in line.Words)
                                foreach (AdvancedTextRenderer.Run run in word.Runs)
                                    using (Font fnt = run.GetFont())
                                        Add_Run(fnt, run.Style.Color, run.Text, true);
                        }
                    else
                    {
                        string text = "";
                        foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                            text += " " + line.Text;
                        Add_Run(f, Obj.Style.TextColor, text, false);

                    }
                    Out.WriteLine(FTextStrings.ToString() + "</w:p>");
                    FTextStrings = null;
                    FTextStrings = new StringBuilder();
                }
                if (renderer.Paragraphs.Count == 0) Out.WriteLine("<w:p />");
            }
        }

        private string Get_BorderLineStyle( BorderLine Obj )
        {
            switch (Obj.Style)
            {
                case LineStyle.Solid: return Quoted("single");
                case LineStyle.Double: return Quoted("double");
                case LineStyle.Dot: return Quoted("dotted");
                case LineStyle.Dash: return Quoted("dash");
                case LineStyle.DashDot: return Quoted("dotDash");
                case LineStyle.DashDotDot: return Quoted("dashDotDot");
            }
            return "";
        }

        private string Get_VerticalAlign(VertAlign align)
        {
            switch (align)
            {
                case VertAlign.Top: return Quoted("top");
                case VertAlign.Center: return Quoted("center");
                case VertAlign.Bottom: return Quoted("bottom");
            }
            return "";
        }

        private string Get_TabledRotation(ExportIEMObject obj)
        {
            switch (obj.Style.Angle)
            {
                case 90: return Quoted("tbRlV");
                case 270: return Quoted("tbLrV");
            }
            return Quoted("lrTb");
        }

        private void Export_TableCell(ExportIEMObject Obj, int CellWidth, int dx, int dy, StreamWriter Out)
        {
            Out.WriteLine("<w:tc>");

            Out.WriteLine("<w:tcPr>");
            if(Obj != null)
                Out.WriteLine("<w:tcW w:w=" + Quoted(Obj.Width*15) + " w:type=\"dxa\" />");
            else
                Out.WriteLine("<w:tcW w:w=" + Quoted(CellWidth) + " w:type=\"dxa\" />");

            if (dx > 1)
            {
                Out.WriteLine("<w:gridSpan w:val="+Quoted(dx)+" />");
            }

            if (Obj != null)
            {
                Border border = Obj.Style.Border;

                Out.WriteLine("<w:tcBorders>");
                if ((border.Lines & BorderLines.Top) != 0)
                    Out.WriteLine("<w:top w:val=" +
                        Get_BorderLineStyle(border.TopLine) + "w:sz=" + Quoted(8 * border.TopLine.Width) +
                        " w:space=\"0\" w:color=" + GetRGBString(border.TopLine.Color) + " />");

                if ((border.Lines & BorderLines.Left) != 0)
                    Out.WriteLine("<w:left w:val=" +
                        Get_BorderLineStyle(border.LeftLine) + "w:sz=" + Quoted(8 * border.LeftLine.Width) +
                        " w:space=\"0\" w:color=" + GetRGBString(border.LeftLine.Color) + " />");

                if ((border.Lines & BorderLines.Bottom) != 0)
                    Out.WriteLine("<w:bottom w:val=" +
                        Get_BorderLineStyle(border.BottomLine) + "w:sz=" + Quoted(8 * border.BottomLine.Width) +
                        " w:space=\"0\" w:color=" + GetRGBString(border.BottomLine.Color) + " />");

                if ((border.Lines & BorderLines.Right) != 0)
                    Out.WriteLine("<w:right w:val=" +
                        Get_BorderLineStyle(border.RightLine) + "w:sz=" + Quoted(8 * border.RightLine.Width) +
                        " w:space=\"0\" w:color=" + GetRGBString(border.RightLine.Color) + " />");

                Out.WriteLine("</w:tcBorders>");

                if (dy > 1)
                {
                    if (Obj.Counter == 0)
                        Out.WriteLine("<w:vMerge w:val=\"restart\" />");
                    else
                        Out.WriteLine("<w:vMerge />");
                    Obj.Counter++;
                }

                string text_color = GetRGBString(Obj.Style.TextColor);
                if (Obj.Style.Fill is SolidFill)
                {
                    SolidFill fill = Obj.Style.Fill as SolidFill;
                    Out.WriteLine("<w:shd w:val=\"clear\" w:color=" + text_color + " w:fill=" + GetRGBString(fill.Color) + " />");
                }
                else if (Obj.Style.Fill is GlassFill)
                {
                    GlassFill fill = Obj.Style.Fill as GlassFill;
                    Out.WriteLine("<w:shd w:val=\"clear\" w:color=" + text_color + " w:fill=" + GetRGBString(fill.Color) + " />");
                }
                else if (Obj.Style.Fill is LinearGradientFill)
                {
                    LinearGradientFill fill = Obj.Style.Fill as LinearGradientFill;
                    Color col = fill.StartColor;
                    Out.WriteLine("<w:shd w:val=\"clear\" w:color=" + text_color + " w:fill=" + GetRGBString(col) + " />");
                }
                else
                {
                    //     throw new Exception("Fill not implemented");
                }

                Out.WriteLine("<w:vAlign w:val=" + Get_VerticalAlign(Obj.Style.VAlign) + " />");
                
                // Rotation
                if (Obj.Style.Angle != 0)
                {
                    Out.WriteLine("<w:textDirection w:val=" + Get_TabledRotation(Obj) + " />");
                }
            }

            Out.WriteLine("</w:tcPr>");

            if (Obj != null && Obj.Counter < 2)
            {
                if (Obj.IsText) Export_Paragraph(Obj, Out); else Export_Picture(Obj, Out);
            }
            else
                Out.WriteLine("<w:p />");

            Out.WriteLine("</w:tc>");
        }

        private void Export_TableRow(ExportMatrix Matrix, int y, StreamWriter Out)
        {
            int fx, fy, dx, dy;

            Out.WriteLine("<w:tr>");
            //Out.WriteLine("<w:tblPrEx>");
            //Out.WriteLine("<w:tblCellMar>");
            //Out.WriteLine("<w:top w:w=\"0\" w:type=\"dxa\" />");
            //Out.WriteLine("<w:bottom w:w=\"0\" w:type=\"dxa\" />");
            //Out.WriteLine("</w:tblCellMar>");
            //Out.WriteLine("</w:tblPrEx>");

            Out.WriteLine("<w:trPr>");

            float ht = (Matrix.YPosById(y + 1) - Matrix.YPosById(y)) * 15; // / 1.376f;
            Out.WriteLine("<w:trHeight w:hRule=\"exact\" w:val="+Quoted(ht)+" />");
            Out.WriteLine("</w:trPr>");

            for (int x = 0; x < Matrix.Width - 1; )
            {
                ExportIEMObject Obj = null;
                int w = (int)Math.Round((Matrix.XPosById(x + 1) - Matrix.XPosById(x)) * 15f, 0);
                int i = Matrix.Cell(x, y);
                if (i != -1)
                {
                    Matrix.ObjectPos(i, out fx, out fy, out dx, out dy);
                    Obj = Matrix.ObjectById(i);
                    Export_TableCell(Obj, w, dx, dy, Out);
                    x += dx;
                }
                else
                {
                    Export_TableCell(null, w, 0, 0, Out);
                    x++;
                }

            }
            Out.WriteLine("</w:tr>");
        }

        private void Export_TableGrid(ExportMatrix Matrix, StreamWriter Out)
        { 
            Out.WriteLine("<w:tblGrid>");
            for (int x = 1; x < Matrix.Width; x++)
            {
                float w = ( Matrix.XPosById(x) - Matrix.XPosById(x - 1) ) * 15f;// / 6.3f;
                w = (float) Math.Round(w, 0);
                string s = ExportUtils.FloatToString(w);
                Out.WriteLine("<w:gridCol w:w=" + Quoted(s) + " />");
            }
            Out.WriteLine("</w:tblGrid>");
        }
        #endregion

        internal void AppendLine(string str) { FTextStrings.AppendLine(str); }

        internal void Export(Word2007Export OoXML, ExportMatrix FMatrix)
        {
            using (FileStream file = new FileStream(OoXML.TempFolder + "/" + FileName, FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                if (FMatrix != null) // Table based export
                {
                    Out.WriteLine(xml_header);
                    Out.WriteLine("<w:document xmlns:ve=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\">");
                    Out.WriteLine("<w:body>");

                    Out.WriteLine("<w:tbl>");
                    Export_TableProperties(Out);
                    Export_TableGrid(FMatrix, Out);
                    for (int y = 0; y < FMatrix.Height - 1; y++)
                    {
                        Export_TableRow(FMatrix, y, Out);
                    }
                    Out.WriteLine("</w:tbl>");

                    Out.WriteLine("<w:p />");
                }
                else // Layered export
                {
                    Out.WriteLine(xml_header);
                    Out.WriteLine("<w:document xmlns:ve=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"" +
                                " xmlns:o=\"urn:schemas-microsoft-com:office:office\"" +
                                " xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"" +
                                " xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\"" +
                                " xmlns:v=\"urn:schemas-microsoft-com:vml\"" +
                                " xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\"" +
                                " xmlns:w10=\"urn:schemas-microsoft-com:office:word\"" +
                                " xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"" +
                                " xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\">");

                    Out.WriteLine("<w:body>");

                    // Line shape
                    Out.WriteLine("<v:shapetype id=\"_x0000_t32\" coordsize=\"21600,21600\" o:spt=\"32\" o:oned=\"t\" path=\"m,l21600,21600e\" filled=\"f\">");
                    Out.WriteLine("<v:path arrowok=\"t\" fillok=\"f\" o:connecttype=\"none\"/>");
                    Out.WriteLine("</v:shapetype>");

                    // Shape diamond
                    Out.WriteLine("<v:shapetype id=\"_x0000_t4\" coordsize=\"21600,21600\" o:spt=\"4\" path=\"m10800,l,10800,10800,21600,21600,10800xe\">");
                    Out.WriteLine("<v:stroke joinstyle=\"miter\" />");
                    Out.WriteLine("<v:path gradientshapeok=\"t\" o:connecttype=\"rect\" textboxrect=\"5400,5400,16200,16200\" />");
                    Out.WriteLine("</v:shapetype>");

                    // Shape triangle
                    Out.WriteLine("<v:shapetype id=\"_x0000_t5\" coordsize=\"21600,21600\" o:spt=\"5\" adj=\"10800\" path=\"m@0,l,21600r21600,xe\">");
                    Out.WriteLine("<v:stroke joinstyle=\"miter\" /> ");
                    Out.WriteLine("<v:formulas>");
                    Out.WriteLine("<v:f eqn=\"val #0\" />");
                    Out.WriteLine("<v:f eqn=\"prod #0 1 2\" />");
                    Out.WriteLine("<v:f eqn=\"sum @1 10800 0\" />");
                    Out.WriteLine("</v:formulas>");
                    Out.WriteLine("<v:path gradientshapeok=\"t\" o:connecttype=\"custom\" o:connectlocs=\"@0,0;@1,10800;0,21600;10800,21600;21600,21600;@2,10800\" textboxrect=\"0,10800,10800,18000;5400,10800,16200,18000;10800,10800,21600,18000;0,7200,7200,21600;7200,7200,14400,21600;14400,7200,21600,21600\" />");
                    Out.WriteLine("<v:handles>");
                    Out.WriteLine("<v:h position=\"#0,topLeft\" xrange=\"0,21600\" />");
                    Out.WriteLine("</v:handles>");
                    Out.WriteLine("</v:shapetype>");

                    Out.WriteLine("<w:p>");

                    Out.WriteLine(FTextStrings.ToString());

                    Out.WriteLine("</w:p>");
                }

                Out.WriteLine("<w:sectPr>");
                long PageWidth = (long)(OoXML.PageWidth * 567 + 4) / 10;
                long PageHeight = (long)(OoXML.PageHeight * 567 + 4) / 10;
                long Top = (long)(OoXML.TopMargin * 567 + 4) / 10;
                long Bottom = (long)(OoXML.BottomMargin * 567 + 4) / 10;
                long Left = (long)(OoXML.LeftMargin * 567 + 4) / 10;
                long Right = (long)(OoXML.RightMargin * 567 + 4) / 10;

                Out.WriteLine("<w:pgSz w:w=" + Quoted(PageWidth) + " w:h=" + Quoted(PageHeight) + "/>");
                Out.WriteLine("<w:pgMar w:top=" + Quoted(Top) +
                    " w:right=" + Quoted(Right) +
                    " w:bottom=" + Quoted(Bottom) +
                    " w:left=" + Quoted(Left) +
                    " w:header=" + Quoted(708) +
                    " w:footer=" + Quoted(708) +
                    " w:gutter=" + Quoted(0) + "/>");
                Out.WriteLine("<w:cols w:space=" + Quoted(708) + "/>");
                Out.WriteLine("<w:docGrid w:linePitch=" + Quoted(360) + "/>");
                Out.WriteLine("</w:sectPr>");
                Out.WriteLine("</w:body>");
                Out.WriteLine("</w:document>");
            }

            ExportRelations(OoXML);
        }

        internal void ExportPage(ReportPage page)
        {
            using (page) foreach (Base c in page.AllObjects)
            {
                ReportComponentBase obj = c as ReportComponentBase;
                if (obj is CellularTextObject)
                    obj = (obj as CellularTextObject).GetTable();
                if (obj is TableCell)
                    continue;

                else if (obj is TextObject)
                    AddTextObject( obj as TextObject, null );
                else if (obj is PictureObject)
                    AddPictureObject( obj, "word/media/image");
                else if (obj is ZipCodeObject)
                    AddPictureObject( obj, "word/media/ZipCodeImage");
                else if (obj is Barcode.BarcodeObject)
                    AddPictureObject( obj, "word/media/BarcodeImage");
                else if (obj is MSChart.MSChartObject)
                    AddPictureObject( obj, "word/media/MSChartImage");
                else if (obj is RichObject)
                    AddPictureObject(obj, "word/media/RichTextImage");
                else if (obj is TableBase)
                    AddTable(obj as TableBase);
                else if (obj is BandBase)
                    AddBandObject(obj as BandBase);
                else if (obj is LineObject)
                    AddLine(obj as LineObject);
                else if (obj is ShapeObject)
                    AddShape(obj as ShapeObject);
                else if (obj is CheckBoxObject)
                    AddCheckboxObject( obj as CheckBoxObject );
                else if (obj == null)
                {
                    ;
                }
                else
                {
                    AddPictureObject(obj as ReportComponentBase, "ppt/media/FixMeImage");
                }
             }
        }

        internal void Open_Paragraph(ReportComponentBase Obj)
        {
            string align = "ctr";
            TextObject text_obj = (Obj is TextObject) ? Obj as TextObject : null;

            if (Obj is TextObject) switch (text_obj.HorzAlign)
            {
                case HorzAlign.Left: align = "left"; break;
                case HorzAlign.Right: align = "right"; break;
                case HorzAlign.Center: align = "center"; break;
                case HorzAlign.Justify: align = "both"; break;
            }

        FTextStrings.AppendLine("<w:p><w:pPr><w:jc w:val=" + Quoted(align) + " /></w:pPr>");

    }

        private void Add_Run(
            Font Font,
            Color TextColor,
            string Text,
            bool AddSpace
            )
        {
            long Size = (long)(Font.Size * 2);
            bool Italic = Font.Italic;
            bool Underline = Font.Underline;

            if (Text != null)
            {
                FTextStrings.AppendLine("<w:r>");
                FTextStrings.AppendLine("<w:rPr>");
//                FTextStrings.AppendLine("<w:solidFill><w:srgbClr val=" + GetRGBString(TextColor) + " /></w:solidFill>");
                FTextStrings.AppendLine("<w:rFonts w:ascii=" + Quoted(Font.Name) + " w:hAnsi=" + Quoted(Font.Name) + " w:cs=" + Quoted(Font.Name) + " /> ");
                if (Font.Bold)   FTextStrings.AppendLine("<w:b />");
                if (Font.Italic) FTextStrings.AppendLine("<w:i />");
                if (Font.Underline) FTextStrings.AppendLine("<w:u />");
                FTextStrings.AppendLine("<w:color w:val=" + GetRGBString(TextColor) + " />");
                FTextStrings.AppendLine("<w:sz w:val=" + Quoted(Size) + " />");
                FTextStrings.AppendLine("<w:szCs w:val=" + Quoted(Size) + " />");
                FTextStrings.AppendLine("</w:rPr>");
                FTextStrings.AppendLine("<w:t>" + this.TranslateText(Text) + "</w:t>");
                FTextStrings.AppendLine("</w:r>");
                if (AddSpace)
                { 
                    FTextStrings.AppendLine("<w:r><w:rPr /><w:t xml:space=\"preserve\"> </w:t></w:r>");
                }
            }
        }

        internal void Close_Paragraph()
        {
            FTextStrings.AppendLine("</w:p>");
        }

        private void AddTextObject(ReportComponentBase reportObject, string Fill_String )
        {
            string vertical_anchor = "";

            if (reportObject is TextObject) switch ((reportObject as TextObject).VertAlign)
            {
                case VertAlign.Top: vertical_anchor = "top"; break;
                case VertAlign.Center: vertical_anchor = "middle"; break;
                case VertAlign.Bottom: vertical_anchor = "bottom"; break;
            }

            string fill_color = "";
            string stroke = "";
            string filled = "";

            if (reportObject.Fill is SolidFill)
            {
                SolidFill fill = reportObject.Fill as SolidFill;
                if (fill.Color.A != 0)
                {
                    fill_color = "fillcolor=" + GetRGBString(fill.Color) + " "; // #dbe5f1";
                }
                else
                {
                    if (Fill_String == null) filled = "filled=\"f\" ";
                }
            }
            else
            {
                if (Fill_String != null) throw new Exception("Fix me: fill conflict.");

                /* $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ */
                {
                    int pictureID = 10 + (++PictureCount);

                    string file_extension = "png";
                    string ImageFileName = "word/media/FillMap" + PictureCount.ToString() + "." + file_extension;

                    OoPictureObject pic = new OoPictureObject(ImageFileName);

                    pic.Export(reportObject, this.FWordExport.TempFolder, true);

                    AddRelation(pictureID, pic);

                    Fill_String = "<v:fill r:id=" + Quoted("rId" + pictureID.ToString()) +
                        " o:title=" + Quoted(PictureCount) +
                        " recolor=\"f\" type=\"frame\" />";

                }
            }

            if (reportObject.Border.Lines == BorderLines.All)
            {
                stroke = 
                    " strokecolor=" + GetRGBString(reportObject.Border.Color) + 
                    " strokeweight=" + Quoted(reportObject.Border.Width.ToString());
            }

            FTextStrings.AppendLine("<w:r>");
            FTextStrings.AppendLine("<w:rPr><w:noProof /></w:rPr>");
            FTextStrings.AppendLine("<w:pict>");
            FTextStrings.AppendLine("<v:rect id=" + Quoted(reportObject.Name) + 
                " style=\"" + 
                "position:absolute;" +
                "margin-left:" + reportObject.AbsLeft + ";" +
                "margin-top:" + reportObject.AbsTop + ";" +
                "width:" + reportObject.Width + ";" +
                "height:" + reportObject.Height + ";" +
                "v-text-anchor:" + vertical_anchor + "\" " +
                filled +
                "stroked=" + Quoted(reportObject.Border.Lines == BorderLines.All ? "t" : "f") +
                fill_color +
                stroke +
                ">");

            if (Fill_String != null)
            {
                FTextStrings.AppendLine(Fill_String);
            }

            if (reportObject.Border.Shadow)
            { 
                FTextStrings.AppendLine("<v:shadow on=\"t\" opacity=\".5\" offset=\"6pt,6pt\" />");
            }

            if (reportObject is TextObject)
            {
                FTextStrings.AppendLine("<v:textbox inset=\"0,0,0,0\">");
                FTextStrings.AppendLine("<w:txbxContent>");

                float FDpiFX = 96f / DrawUtils.ScreenDpi;

                TextObject textObject = reportObject as TextObject;

                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                using (Font f = new Font(textObject.Font.Name, textObject.Font.Size * FDpiFX, textObject.Font.Style))
                using (GraphicCache cache = new GraphicCache())
                {
                    RectangleF textRect = new RectangleF(
                      textObject.AbsLeft + textObject.Padding.Left,
                      textObject.AbsTop + textObject.Padding.Top,
                      textObject.Width - textObject.Padding.Horizontal,
                      textObject.Height - textObject.Padding.Vertical);

                    StringFormat format = textObject.GetStringFormat(cache, 0);
                    Brush textBrush = cache.GetBrush(textObject.TextColor);
                    AdvancedTextRenderer renderer = new AdvancedTextRenderer(textObject.Text, g, f, textBrush,
                        textRect, format, textObject.HorzAlign, textObject.VertAlign,
                        textObject.LineHeight, textObject.Angle, textObject.FontWidthRatio,
                        textObject.ForceJustify, textObject.Wysiwyg, textObject.HtmlTags, true);

                    float w = f.Height * 0.1f; // to match .net char X offset

                    foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                    {
                        Open_Paragraph(textObject);
                        if (renderer.HtmlTags)
                        foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                        {
                            foreach (AdvancedTextRenderer.Word word in line.Words)
                                    foreach (AdvancedTextRenderer.Run run in word.Runs)
                                        using (Font fnt = run.GetFont())
                                            Add_Run(fnt, run.Style.Color, run.Text, true);
                        }
                        else
                        {
                            string text = "";
                            foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                                text += " " + line.Text;
                            Add_Run(f, textObject.TextColor, text, false);
                        }
                        Close_Paragraph();
                    }
                }

                FTextStrings.AppendLine("</w:txbxContent>");
                FTextStrings.AppendLine("</v:textbox>");
            }

            FTextStrings.AppendLine("</v:rect>");
            FTextStrings.AppendLine("</w:pict>");
            FTextStrings.AppendLine("</w:r>");
        }

        private OoPictureObject AddPictureObject(ReportComponentBase pictureObject, string imageName)
        {
            int pictureID = 10 + (++PictureCount);

            string file_extension = "png";
            string ImageFileName = imageName + PictureCount.ToString() + "." + file_extension;

            OoPictureObject pic = new OoPictureObject( ImageFileName );

            pic.Export(pictureObject, this.FWordExport.TempFolder, false);

            AddRelation( pictureID, pic );

            string fill = "<v:fill r:id="+Quoted("rId" + pictureID.ToString()) +
                " o:title=" + Quoted(PictureCount) +
                " recolor=\"f\" type=\"frame\" />";

            AddTextObject(pictureObject, fill);

            return pic;
        }

        private void AddTable(TableBase table)
        {
            using (TextObject tableBack = new TextObject())
            {
                tableBack.Left = table.AbsLeft;
                tableBack.Top = table.AbsTop;
                float tableWidth = 0;
                for (int i = 0; i < table.ColumnCount; i++)
                    tableWidth += table[i, 0].Width;
                tableBack.Width = (tableWidth < table.Width) ? tableWidth : table.Width;
                tableBack.Height = table.Height;
                tableBack.Fill = table.Fill;
                tableBack.Text = "";

                // exporting the table fill
                AddTextObject( tableBack, null );

                // exporting the table cells
                float x = 0;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    float y = 0;
                    for (int i = 0; i < table.RowCount; i++)
                    {
                        if (!table.IsInsideSpan(table[j, i]))
                        {
                            TableCell textcell = table[j, i];

                            textcell.Left = x;
                            textcell.Top = y;

                            AddTextObject( textcell, null );
                        }
                        y += (table.Rows[i]).Height;
                    }
                    x += (table.Columns[j]).Width;
                }

                // exporting the table border
                tableBack.Fill = new SolidFill();
                tableBack.Border = table.Border;
                AddTextObject( tableBack, null );
            }
        }

        private void AddBandObject(BandBase band)
        {
            if (band.HasBorder || band.HasFill) using (TextObject newObj = new TextObject())
                {
                    newObj.Left = band.AbsLeft;
                    newObj.Top = band.AbsTop;
                    newObj.Width = band.Width;
                    newObj.Height = band.Height;
                    newObj.Fill = band.Fill;
                    newObj.Border = band.Border;
                    newObj.Text = "";
                    AddTextObject(newObj, null);
                }
        }

        private void AddLine(LineObject line)
        {
            FTextStrings.AppendLine("<w:r>");
            FTextStrings.AppendLine("<w:pict>");

            FTextStrings.AppendLine("<v:shape type=\"#_x0000_t32\""+
                " style=\"position:absolute;" +
                "margin-left:" + line.AbsLeft + "pt;" +
                "margin-top:" + line.AbsTop + "pt;" +
                "width:" + line.Width + "pt;" +
                "height:" + line.Height + "pt\"" +
                " o:connectortype=\"straight\"" +
                " strokecolor=" + GetRGBString(line.Border.Color) +
                " strokeweight=\"" + line.Width + "pt\"" +
                ">");

            string StartCap = null;
            string EndCap = null;

            switch (line.StartCap.Style)
            {
                case CapStyle.Arrow: StartCap = "arrow"; break;
                case CapStyle.Circle: StartCap = "oval"; break;
                case CapStyle.Diamond: StartCap = "diamond"; break;
                case CapStyle.Square: StartCap = "diamond"; break;
            }

            switch (line.EndCap.Style)
            {
                case CapStyle.Arrow: EndCap = "arrow"; break;
                case CapStyle.Circle: EndCap = "oval"; break;
                case CapStyle.Diamond: EndCap = "diamond"; break;
                case CapStyle.Square: EndCap = "diamond"; break;
            }
            if (EndCap != null) FTextStrings.AppendLine("<v:stroke endarrow=" + Quoted(EndCap) + "/>");
            if (StartCap != null) FTextStrings.AppendLine("<v:stroke startarrow=" + Quoted(StartCap) + "/>");


            FTextStrings.AppendLine("</v:shape>");

            FTextStrings.AppendLine("</w:pict>");
            FTextStrings.AppendLine("</w:r>");
        }

        private void AddShape(ShapeObject shape)
        {
            FTextStrings.AppendLine("<w:r>");
            FTextStrings.AppendLine("<w:pict>");

            switch (shape.Shape)
            {
                case ShapeKind.Rectangle:
                    FTextStrings.AppendLine("<v:rect style=\"position:absolute;" +
                        "margin-left:" + shape.AbsLeft + "pt;" +
                        "margin-top:" + shape.AbsTop + "pt;" +
                        "width:" + shape.Width + "pt;" +
                        "height:" + shape.Height + "pt\"");
                    break;
                case ShapeKind.Diamond:
                    FTextStrings.AppendLine("<v:shape type=\"#_x0000_t4\" style=\"position:absolute;" +
                        "margin-left:" + shape.AbsLeft + "pt;" +
                        "margin-top:" + shape.AbsTop + "pt;" +
                        "width:" + shape.Width + "pt;" +
                        "height:" + shape.Height + "pt\"");
                    break;
                case ShapeKind.Ellipse:
                    FTextStrings.AppendLine("<v:oval style=\"position:absolute;" +
                        "margin-left:" + shape.AbsLeft + "pt;" +
                        "margin-top:" + shape.AbsTop + "pt;" +
                        "width:" + shape.Width + "pt;" +
                        "height:" + shape.Height + "pt\"");
                    break;
                case ShapeKind.RoundRectangle: 
                    FTextStrings.AppendLine("<v:roundrect style=\"position:absolute;" +
                        "margin-left:" + shape.AbsLeft + "pt;" +
                        "margin-top:" + shape.AbsTop + "pt;" +
                        "width:" + shape.Width + "pt;" +
                        "height:" + shape.Height + "pt\"" +
                        " arcsize=\"10923f\"");
                    break;
                case ShapeKind.Triangle:
                    FTextStrings.AppendLine("<v:shape type=\"#_x0000_t5\" style=\"position:absolute;" +
                        "margin-left:" + shape.AbsLeft + "pt;" +
                        "margin-top:" + shape.AbsTop + "pt;" +
                        "width:" + shape.Width + "pt;" + 
                        "height:" + shape.Height + "pt\"");
                    break;
                default: throw new Exception("Unsupported shape kind");
            }

            FTextStrings.AppendLine(" strokeweight=\"" + shape.Border.Width + "pt\" />");
            FTextStrings.AppendLine("</w:pict>");
            FTextStrings.AppendLine("</w:r>");
        }

        public OoXMLDocument(OOExportBase OoXML)
        {
            FWordExport = OoXML;
            FTextStrings = new StringBuilder();
            CheckboxList = new Dictionary<string, OoPictureObject>();
            PictureCount = 0;
        }

        private void AddCheckboxObject(CheckBoxObject checkbox)
        {
            OoPictureObject pic;
            string KEY = checkbox.Name + checkbox.Checked.ToString();
            if (!CheckboxList.ContainsKey(KEY))
            {
                pic = AddPictureObject(checkbox, "word/media/RichTextImage");
                CheckboxList.Add(KEY, pic);
            }
            else 
            {
                pic = CheckboxList[KEY];
//                pic.MoveObject(checkbox);
            }

        }
    }

    /// <summary>
    /// MS Word 2007 export class
    /// </summary>
    public class Word2007Export : OOExportBase
    {
        #region Private fields
        private OoXMLCoreDocumentProperties FCoreDocProp;
        private OoXMLApplicationProperties  FApplicationProp;
        private OoXMLDocument               FDocument;
        private OoXMLFontTable              FFontTable;
        private OoXMLWordStyles             FWordStyles;
        private OoXMLWordSettings           FWordSettings;
        internal ExportMatrix               FMatrix;
        internal bool                       MatrixBased = true;
        #endregion

        #region Properties
        internal OoXMLFontTable     FontTable    { get { return FFontTable; } }
        internal OoXMLWordStyles    WordStyles   { get { return FWordStyles; } }
        internal OoXMLWordSettings  WordSettings { get { return FWordSettings; } }
        internal float              PageWidth    { get { return this.GetPage(0).PaperWidth; } }
        internal float              PageHeight   { get { return this.GetPage(0).PaperHeight; } }
        internal float              TopMargin    { get { return this.GetPage(0).TopMargin; } }
        internal float              LeftMargin   { get { return this.GetPage(0).LeftMargin; } }
        internal float              RightMargin  { get { return this.GetPage(0).RightMargin; } }
        internal float              BottomMargin { get { return this.GetPage(0).BottomMargin; } }
        #endregion

        #region Protected Methods

        private void CreateContentTypes()
        { 
            using (FileStream file = new FileStream(FTempFolder + "\\[Content_Types].xml", FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                Out.WriteLine(xml_header);
                Out.Write("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
                Out.Write("<Default Extension=\"png\" ContentType=\"image/png\" />");
                Out.Write("<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\" />");
                Out.Write("<Default Extension=\"xml\" ContentType=\"application/xml\" />");

                Out.Write("<Override PartName=" + QuotedRoot(FDocument.FileName) + " ContentType=" + Quoted(FDocument.ContentType) + "/>");
                Out.Write("<Override PartName=" + QuotedRoot(FWordStyles.FileName) + " ContentType=" + Quoted(FWordStyles.ContentType) + "/>");
                Out.Write("<Override PartName=" + QuotedRoot(FApplicationProp.FileName) + " ContentType=" + Quoted(FApplicationProp.ContentType) + "/>");
                Out.Write("<Override PartName=" + QuotedRoot(FWordSettings.FileName) + " ContentType=" + Quoted(FWordSettings.ContentType) + "/>");
                Out.Write("<Override PartName=" + QuotedRoot(FFontTable.FileName) + " ContentType=" + Quoted(FFontTable.ContentType) + "/>");
                Out.Write("<Override PartName=" + QuotedRoot(FCoreDocProp.FileName) + " ContentType=" + Quoted(FCoreDocProp.ContentType) + "/>");
                Out.Write("</Types>");
            }
        }

        private void CreateRelations()
        {
            using (FileStream file = new FileStream(FTempFolder + "\\_rels\\.rels", FileMode.Create))
            using (StreamWriter Out = new StreamWriter(file))
            {
                Out.WriteLine(xml_header);
                Out.WriteLine("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");

                Out.WriteLine("<Relationship Id=\"rId3\" Type=" + Quoted(FApplicationProp.RelationType) + " Target=" + Quoted(FApplicationProp.FileName) + " />");
                Out.WriteLine("<Relationship Id=\"rId2\" Type=" + Quoted(FCoreDocProp.RelationType) + " Target=" + Quoted(FCoreDocProp.FileName) + " />");
                Out.WriteLine("<Relationship Id=\"rId1\" Type=" + Quoted(FDocument.RelationType) + " Target=" + Quoted(FDocument.FileName) + " />");
                Out.WriteLine("</Relationships>");
            }
        }

        private void ExportOOXML(Stream Stream)
        {
            CreateRelations();
            CreateContentTypes();

            FApplicationProp.Export(this);
            FCoreDocProp.Export(this);
            FFontTable.Export(this);
            FWordStyles.Export(this);
            FWordSettings.Export(this);
            FDocument.Export(this, MatrixBased ? FMatrix : null);

            ZipArchive zip = new ZipArchive();
            zip.AddDir(FTempFolder);
            zip.SaveToStream(Stream);
//            Directory.Delete(FTempFolder, true);
        }
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (Word2007ExportForm form = new Word2007ExportForm())
            {
                form.Init(this);
                return form.ShowDialog() == DialogResult.OK;
            }
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            if (MatrixBased == true)
            {
                FMatrix = new ExportMatrix();
                FMatrix.Inaccuracy = 0.5f;
                FMatrix.PlainRich = true;
                FMatrix.AreaFill = false; // true; // 
                FMatrix.CropAreaFill = true;
                FMatrix.Report = Report;
                FMatrix.Images = true;
                FMatrix.WrapText = false;
                FMatrix.FullTrust = false;
                //            FMatrix.MaxCellHeight = 409 * oxmlYDivider;
            }

            Directory.CreateDirectory(FTempFolder);
            Directory.CreateDirectory(FTempFolder + "\\_rels");
            Directory.CreateDirectory(FTempFolder + "\\docProps");
            Directory.CreateDirectory(FTempFolder + "\\word");
            Directory.CreateDirectory(FTempFolder + "\\word\\_rels");
            Directory.CreateDirectory(FTempFolder + "\\word\\theme");
            Directory.CreateDirectory(FTempFolder + "\\word\\media");
        }

        /// <inheritdoc/>
        protected override void ExportPage(int pageNo)
        {
            using (ReportPage page = GetPage(pageNo))
            {
                if (MatrixBased == true)
                {
                    FMatrix.AddPage(page);
                }
                else
                {
                    FDocument.ExportPage(page);
                    if ((1 + pageNo) < Pages.Length) FDocument.AppendLine("<w:br w:type=" + Quoted("page") + " />");
                }
            }
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            if (MatrixBased == true)
            {
                FMatrix.Prepare();
            }

            ExportOOXML(Stream);
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("DocxFile");
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the Word2007Export class.
        /// </summary>
        public Word2007Export()
        {
            // FTempFolder = Path.GetTempPath() + Path.GetRandomFileName();
            FTempFolder = "C:\\PRES\\test";

            FCoreDocProp = new OoXMLCoreDocumentProperties();
            FApplicationProp = new OoXMLApplicationProperties();
            FDocument = new OoXMLDocument(this);
            FFontTable = new OoXMLFontTable();
            FWordStyles = new OoXMLWordStyles();
            FWordSettings = new OoXMLWordSettings();

            // Set relations to presentation.xml.rels
            FDocument.AddRelation(1, FWordSettings);
            FDocument.AddRelation(2, FWordStyles);
            FDocument.AddRelation(3, FFontTable);
        }
    }
}