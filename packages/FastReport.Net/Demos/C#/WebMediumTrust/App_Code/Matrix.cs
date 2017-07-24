using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using FastReport;
using FastReport.Data;
using FastReport.Dialog;
using FastReport.Barcode;
using FastReport.Table;
using FastReport.Utils;

namespace FastReport
{
  public class MatrixReport : Report
  {
    public FastReport.Report Report;
    public FastReport.Engine.ReportEngine Engine;
    public FastReport.Table.TableCell Cell1;
    public FastReport.Table.TableCell Cell10;
    public FastReport.Table.TableCell Cell11;
    public FastReport.Table.TableCell Cell12;
    public FastReport.Table.TableCell Cell13;
    public FastReport.Table.TableCell Cell14;
    public FastReport.Table.TableCell Cell15;
    public FastReport.Table.TableCell Cell16;
    public FastReport.Table.TableCell Cell2;
    public FastReport.Table.TableCell Cell3;
    public FastReport.Table.TableCell Cell4;
    public FastReport.Table.TableCell Cell5;
    public FastReport.Table.TableCell Cell6;
    public FastReport.Table.TableCell Cell7;
    public FastReport.Table.TableCell Cell8;
    public FastReport.Table.TableCell Cell9;
    public FastReport.Table.TableColumn Column1;
    public FastReport.Table.TableColumn Column2;
    public FastReport.Table.TableColumn Column3;
    public FastReport.Table.TableColumn Column4;
    public FastReport.Matrix.MatrixObject Matrix1;
    public FastReport.ReportPage Page1;
    public FastReport.ReportTitleBand ReportTitle1;
    public FastReport.Table.TableRow Row1;
    public FastReport.Table.TableRow Row2;
    public FastReport.Table.TableRow Row5;
    public FastReport.Table.TableRow Row7;
    public FastReport.TextObject Text1;
    protected override object CalcExpression(string expression, Variant Value)
    {
      return null;
    }

    private void InitializeComponent()
    {
      string reportString = 
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Report TextQuality=\"Regular\" ReportInf" +
        "o.Description=\"Demonstrates the matrix with two row dimensions. To create such m" +
        "atrix:&#13;&#10;&#13;&#10;- drag a data from the Data Dictionary window to creat" +
        "e a row;&#13;&#10;&#13;&#10;- drag the next data to the right of just created ro" +
        "w. Watch the drag indicator to select the position where to insert a new item;&#" +
        "13;&#10;&#13;&#10;- you may drag the existing rows/columns/cells to the new posi" +
        "tion. To do this, select the appropriate cell, grab its border and drag it to th" +
        "e new position.\" ReportInfo.Created=\"05/16/2008 01:44:40\" ReportInfo.Modified=\"1" +
        "1/11/2008 18:12:56\" ReportInfo.CreatorVersion=\"1.0.0.0\">\r\n  <Dictionary>\r\n    <T" +
        "ableDataSource Name=\"MatrixDemo\" ReferenceName=\"NorthWind.MatrixDemo\" Enabled=\"t" +
        "rue\">\r\n      <Column Name=\"Name\" DataType=\"System.String\"/>\r\n      <Column Name=" +
        "\"Year\" DataType=\"System.Int32\"/>\r\n      <Column Name=\"Month\" DataType=\"System.In" +
        "t32\"/>\r\n      <Column Name=\"ItemsSold\" DataType=\"System.Int32\"/>\r\n      <Column " +
        "Name=\"Revenue\" DataType=\"System.Decimal\"/>\r\n    </TableDataSource>\r\n  </Dictiona";
      reportString += "ry>\r\n  <ReportPage Name=\"Page1\">\r\n    <ReportTitleBand Name=\"ReportTitle1\" Width" +
        "=\"718.2\" Height=\"152.56\">\r\n      <MatrixObject Name=\"Matrix1\" Left=\"9.45\" Top=\"4" +
        "7.25\" Width=\"285.48\" Height=\"78.29\" FixedRows=\"1\" FixedColumns=\"2\" AutoSize=\"fal" +
        "se\" DataSource=\"MatrixDemo\" Style=\"Orange\">\r\n        <MatrixColumns>\r\n          " +
        "<Header Expression=\"[MatrixDemo.Name]\"/>\r\n        </MatrixColumns>\r\n        <Mat" +
        "rixRows>\r\n          <Header Expression=\"[MatrixDemo.Year]\"/>\r\n          <Header " +
        "Expression=\"[MatrixDemo.Month]\"/>\r\n        </MatrixRows>\r\n        <MatrixCells>\r" +
        "\n          <Cell Expression=\"[MatrixDemo.Revenue]\"/>\r\n        </MatrixCells>\r\n  " +
        "      <TableColumn Name=\"Column1\" Width=\"48.26\"/>\r\n        <TableColumn Name=\"Co" +
        "lumn2\" Width=\"47.33\"/>\r\n        <TableColumn Name=\"Column3\" Width=\"106.11\"/>\r\n  " +
        "      <TableColumn Name=\"Column4\" Width=\"83.78\"/>\r\n        <TableRow Name=\"Row1\"" +
        " Height=\"26.66\">\r\n          <TableCell Name=\"Cell1\" Border.Lines=\"All\" Border.Co";
      reportString += "lor=\"DimGray\" Fill=\"Glass\" Fill.Color=\"64, 64, 64\" Fill.Blend=\"0.13\" Fill.Hatch=" +
        "\"false\" Text=\"Year\" HorzAlign=\"Center\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\" Tex" +
        "tFill.Color=\"White\"/>\r\n          <TableCell Name=\"Cell2\" Border.Lines=\"All\" Bord" +
        "er.Color=\"DimGray\" Fill=\"Glass\" Fill.Color=\"64, 64, 64\" Fill.Blend=\"0.13\" Fill.H" +
        "atch=\"false\" Text=\"Month\" HorzAlign=\"Center\" VertAlign=\"Center\" Font=\"Tahoma, 8p" +
        "t\" TextFill.Color=\"White\"/>\r\n          <TableCell Name=\"Cell7\" Border.Lines=\"All" +
        "\" Border.Color=\"DimGray\" Fill=\"Glass\" Fill.Color=\"64, 64, 64\" Fill.Blend=\"0.13\" " +
        "Fill.Hatch=\"false\" Text=\"Steven Buchanan\" AllowExpressions=\"false\" HorzAlign=\"Ce" +
        "nter\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n          " +
        "<TableCell Name=\"Cell10\" Border.Lines=\"All\" Border.Color=\"DimGray\" Fill=\"Glass\" " +
        "Fill.Color=\"64, 64, 64\" Fill.Blend=\"0.13\" Fill.Hatch=\"false\" Text=\"Total\" HorzAl" +
        "ign=\"Center\" VertAlign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\" TextFill.Color=\"W";
      reportString += "hite\"/>\r\n        </TableRow>\r\n        <TableRow Name=\"Row2\" Height=\"17.21\">\r\n   " +
        "       <TableCell Name=\"Cell3\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill.C" +
        "olor=\"64, 64, 64\" Text=\"2002\" AllowExpressions=\"false\" HorzAlign=\"Center\" VertAl" +
        "ign=\"Center\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\" RowSpan=\"2\"/>\r\n          " +
        "<TableCell Name=\"Cell4\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill.Color=\"6" +
        "4, 64, 64\" Text=\"1\" AllowExpressions=\"false\" HorzAlign=\"Center\" VertAlign=\"Cente" +
        "r\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n          <TableCell Name=\"Cell8" +
        "\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill.Color=\"Gray\" AllowExpressions=" +
        "\"false\" Format=\"Currency\" Format.UseLocale=\"true\" HorzAlign=\"Right\" VertAlign=\"C" +
        "enter\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n          <TableCell Name=\"C" +
        "ell11\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill.Color=\"Gray\" Text=\"3 500," +
        "00р.\" Format=\"Currency\" Format.UseLocale=\"true\" HorzAlign=\"Right\" VertAlign=\"Cen";
      reportString += "ter\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n        </TableRow>\r\n        <" +
        "TableRow Name=\"Row5\" Height=\"17.21\">\r\n          <TableCell Name=\"Cell5\" Fill.Col" +
        "or=\"64, 64, 64\" TextFill.Color=\"White\"/>\r\n          <TableCell Name=\"Cell6\" Bord" +
        "er.Lines=\"All\" Border.Color=\"DarkGray\" Fill.Color=\"64, 64, 64\" Text=\"Total\" Horz" +
        "Align=\"Center\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n " +
        "         <TableCell Name=\"Cell9\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill" +
        ".Color=\"64, 64, 64\" Text=\"0,00р.\" Format=\"Currency\" Format.UseLocale=\"true\" Horz" +
        "Align=\"Right\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n  " +
        "        <TableCell Name=\"Cell12\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill" +
        ".Color=\"64, 64, 64\" Text=\"3 500,00р.\" Format=\"Currency\" Format.UseLocale=\"true\" " +
        "HorzAlign=\"Right\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>" +
        "\r\n        </TableRow>\r\n        <TableRow Name=\"Row7\" Height=\"17.21\">\r\n          ";
      reportString += "<TableCell Name=\"Cell13\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill.Color=\"" +
        "64, 64, 64\" Text=\"Total\" HorzAlign=\"Center\" VertAlign=\"Center\" Font=\"Tahoma, 8pt" +
        ", style=Bold\" TextFill.Color=\"White\" ColSpan=\"2\"/>\r\n          <TableCell Name=\"C" +
        "ell14\" Fill.Color=\"64, 64, 64\" TextFill.Color=\"White\"/>\r\n          <TableCell Na" +
        "me=\"Cell15\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill.Color=\"64, 64, 64\" T" +
        "ext=\"12 099,00р.\" Format=\"Currency\" Format.UseLocale=\"true\" HorzAlign=\"Right\" Ve" +
        "rtAlign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\" TextFill.Color=\"White\"/>\r\n      " +
        "    <TableCell Name=\"Cell16\" Border.Lines=\"All\" Border.Color=\"DarkGray\" Fill.Col" +
        "or=\"64, 64, 64\" Text=\"39 999,00р.\" Format=\"Currency\" Format.UseLocale=\"true\" Hor" +
        "zAlign=\"Right\" VertAlign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\" TextFill.Color=" +
        "\"White\"/>\r\n        </TableRow>\r\n      </MatrixObject>\r\n      <TextObject Name=\"T" +
        "ext1\" Left=\"9.45\" Top=\"9.45\" Width=\"406.35\" Height=\"18.9\" Text=\"Revenue by Emplo";
      reportString += "yee\" Font=\"Tahoma, 10pt, style=Bold\"/>\r\n    </ReportTitleBand>\r\n  </ReportPage>\r" +
        "\n</Report>\r\n";
      LoadFromString(reportString);

      InternalInit();
    }

    public MatrixReport()
    {
      InitializeComponent();
    }
  }
}
