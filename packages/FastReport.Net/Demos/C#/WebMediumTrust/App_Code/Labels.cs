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
  public class Labels : Report
  {
    public FastReport.Report Report;
    public FastReport.Engine.ReportEngine Engine;
    public FastReport.Table.TableCell Cell1;
    public FastReport.Table.TableCell Cell11;
    public FastReport.Table.TableCell Cell12;
    public FastReport.Table.TableCell Cell16;
    public FastReport.Table.TableCell Cell17;
    public FastReport.Table.TableCell Cell2;
    public FastReport.Table.TableCell Cell21;
    public FastReport.Table.TableCell Cell22;
    public FastReport.Table.TableCell Cell6;
    public FastReport.Table.TableCell Cell7;
    public FastReport.Table.TableColumn Column1;
    public FastReport.Table.TableColumn Column2;
    public FastReport.DataBand Data1;
    public FastReport.ReportPage Page1;
    public FastReport.PageFooterBand PageFooter1;
    public FastReport.ReportTitleBand ReportTitle1;
    public FastReport.Table.TableRow Row1;
    public FastReport.Table.TableRow Row2;
    public FastReport.Table.TableRow Row3;
    public FastReport.Table.TableRow Row4;
    public FastReport.Table.TableRow Row5;
    public FastReport.Table.TableObject Table1;
    public FastReport.TextObject Text1;
    public FastReport.TextObject Text12;
    private object Sum(TableCell cell)
    {
      return cell.Table.Sum(cell);
    }

    private object Min(TableCell cell)
    {
      return cell.Table.Min(cell);
    }

    private object Max(TableCell cell)
    {
      return cell.Table.Max(cell);
    }

    private object Avg(TableCell cell)
    {
      return cell.Table.Avg(cell);
    }

    private object Count(TableCell cell)
    {
      return cell.Table.Count(cell);
    }

    protected override object CalcExpression(string expression, Variant Value)
    {
      return null;
    }

    private void InitializeComponent()
    {
      string reportString = 
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Report TextQuality=\"Regular\" ReportInf" +
        "o.Description=\"Demonstrates how to use the Table object. Notes:&#13;&#10;&#13;&#" +
        "10;- to set the number of columns and rows, use &quot;ColumnCount&quot; and &quo" +
        "t;RowCount&quot; properties. Also you may add new columns/rows from a column/row" +
        " context menu;&#13;&#10;&#13;&#10;- to join/split cells, use context menu of the" +
        " cell;&#13;&#10;&#13;&#10;- to set the column/row autosize, use context menu of " +
        "the column/row. \" ReportInfo.Created=\"01/17/2008 17:08:04\" ReportInfo.Modified=\"" +
        "11/11/2008 18:11:28\" ReportInfo.CreatorVersion=\"1.0.0.0\">\r\n  <Styles Name=\"Stand" +
        "ard\">\r\n    <Style Name=\"Title\" Font=\"Arial, 12pt, style=Bold\"/>\r\n    <Style Name" +
        "=\"Header\" Font=\"Arial, 10pt, style=Bold\"/>\r\n    <Style Name=\"Group\" Font=\"Arial," +
        " 10pt, style=Bold\"/>\r\n    <Style Name=\"Data\"/>\r\n    <Style Name=\"Footer\"/>\r\n    " +
        "<Style Name=\"EvenRows\" Fill.Color=\"WhiteSmoke\"/>\r\n  </Styles>\r\n  <Dictionary>\r\n " +
        "   <TableDataSource Name=\"Customers\" ReferenceName=\"NorthWind.Customers\" Enabled" +
        "=\"true\">\r\n      <Column Name=\"CustomerID\" DataType=\"System.String\"/>\r\n      <Col";
      reportString += "umn Name=\"CompanyName\" DataType=\"System.String\"/>\r\n      <Column Name=\"ContactNa" +
        "me\" DataType=\"System.String\"/>\r\n      <Column Name=\"ContactTitle\" DataType=\"Syst" +
        "em.String\"/>\r\n      <Column Name=\"Address\" DataType=\"System.String\"/>\r\n      <Co" +
        "lumn Name=\"City\" DataType=\"System.String\"/>\r\n      <Column Name=\"Region\" DataTyp" +
        "e=\"System.String\"/>\r\n      <Column Name=\"PostalCode\" DataType=\"System.String\"/>\r" +
        "\n      <Column Name=\"Country\" DataType=\"System.String\"/>\r\n      <Column Name=\"Ph" +
        "one\" DataType=\"System.String\"/>\r\n      <Column Name=\"Fax\" DataType=\"System.Strin" +
        "g\"/>\r\n    </TableDataSource>\r\n  </Dictionary>\r\n  <ReportPage Name=\"Page1\" Column" +
        "s.Count=\"2\" Columns.Width=\"95\" Columns.Positions=\"0,95\">\r\n    <ReportTitleBand N" +
        "ame=\"ReportTitle1\" Width=\"718.2\" Height=\"37.8\">\r\n      <TextObject Name=\"Text1\" " +
        "Width=\"718.2\" Height=\"37.8\" Dock=\"Fill\" Text=\"Customers\" HorzAlign=\"Center\" Vert" +
        "Align=\"Center\" Font=\"Tahoma, 14pt, style=Bold\"/>\r\n    </ReportTitleBand>\r\n    <D";
      reportString += "ataBand Name=\"Data1\" Top=\"41.8\" Width=\"359.1\" Height=\"113.4\" CanGrow=\"true\" Data" +
        "Source=\"Customers\">\r\n      <TableObject Name=\"Table1\" Left=\"9.45\" Top=\"9.45\" Wid" +
        "th=\"340.2\" Height=\"103.95\">\r\n        <TableColumn Name=\"Column1\" Width=\"151.2\"/>" +
        "\r\n        <TableColumn Name=\"Column2\" Width=\"189\"/>\r\n        <TableRow Name=\"Row" +
        "1\" Height=\"28.35\">\r\n          <TableCell Name=\"Cell1\" Border.Lines=\"Bottom\" Bord" +
        "er.Color=\"White\" Fill=\"Glass\" Fill.Color=\"Chocolate\" Fill.Blend=\"0.18\" Fill.Hatc" +
        "h=\"false\" Text=\"[Customers.CompanyName]\" Padding=\"5, 0, 0, 0\" VertAlign=\"Center\"" +
        " Font=\"Tahoma, 12pt, style=Bold\" TextFill.Color=\"White\" ColSpan=\"2\"/>\r\n         " +
        " <TableCell Name=\"Cell2\" Fill.Color=\"Chocolate\" Font=\"Tahoma, 8pt\" TextFill.Colo" +
        "r=\"White\"/>\r\n        </TableRow>\r\n        <TableRow Name=\"Row2\" AutoSize=\"true\">" +
        "\r\n          <TableCell Name=\"Cell6\" Border.Lines=\"Right\" Border.Color=\"White\" Fi" +
        "ll.Color=\"Chocolate\" Text=\"Country\" Padding=\"5, 2, 0, 2\" Font=\"Tahoma, 8pt, styl";
      reportString += "e=Bold\" TextFill.Color=\"White\"/>\r\n          <TableCell Name=\"Cell7\" Fill.Color=\"" +
        "Chocolate\" Text=\"[Customers.Country]\" Padding=\"2, 2, 2, 2\" Font=\"Tahoma, 8pt\" Te" +
        "xtFill.Color=\"White\"/>\r\n        </TableRow>\r\n        <TableRow Name=\"Row3\" AutoS" +
        "ize=\"true\">\r\n          <TableCell Name=\"Cell11\" Border.Lines=\"Right\" Border.Colo" +
        "r=\"White\" Fill.Color=\"Chocolate\" Text=\"Address\" Padding=\"5, 2, 0, 2\" Font=\"Tahom" +
        "a, 8pt, style=Bold\" TextFill.Color=\"White\"/>\r\n          <TableCell Name=\"Cell12\"" +
        " Fill.Color=\"Chocolate\" Text=\"[Customers.Address]\" Padding=\"2, 2, 2, 2\" Font=\"Ta" +
        "homa, 8pt\" TextFill.Color=\"White\"/>\r\n        </TableRow>\r\n        <TableRow Name" +
        "=\"Row4\" AutoSize=\"true\">\r\n          <TableCell Name=\"Cell16\" Border.Lines=\"Right" +
        "\" Border.Color=\"White\" Fill.Color=\"Chocolate\" Text=\"City\" Padding=\"5, 2, 0, 2\" F" +
        "ont=\"Tahoma, 8pt, style=Bold\" TextFill.Color=\"White\"/>\r\n          <TableCell Nam" +
        "e=\"Cell17\" Fill.Color=\"Chocolate\" Text=\"[Customers.City]\" Padding=\"2, 2, 2, 2\" F";
      reportString += "ont=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n        </TableRow>\r\n        <TableR" +
        "ow Name=\"Row5\" AutoSize=\"true\">\r\n          <TableCell Name=\"Cell21\" Border.Lines" +
        "=\"Right\" Border.Color=\"White\" Fill.Color=\"Chocolate\" Text=\"Postal code\" Padding=" +
        "\"5, 2, 0, 2\" Font=\"Tahoma, 8pt, style=Bold\" TextFill.Color=\"White\"/>\r\n          " +
        "<TableCell Name=\"Cell22\" Fill.Color=\"Chocolate\" Text=\"[Customers.PostalCode]\" Pa" +
        "dding=\"2, 2, 2, 2\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n        </TableR" +
        "ow>\r\n      </TableObject>\r\n    </DataBand>\r\n    <PageFooterBand Name=\"PageFooter" +
        "1\" Top=\"159.2\" Width=\"718.2\" Height=\"18.9\">\r\n      <TextObject Name=\"Text12\" Wid" +
        "th=\"718.2\" Height=\"18.9\" Dock=\"Fill\" Text=\"[PageN]\" HorzAlign=\"Right\" Font=\"Taho" +
        "ma, 8pt\"/>\r\n    </PageFooterBand>\r\n  </ReportPage>\r\n</Report>\r\n";
      LoadFromString(reportString);

      InternalInit();
    }

    public Labels()
    {
      InitializeComponent();
    }
  }
}
