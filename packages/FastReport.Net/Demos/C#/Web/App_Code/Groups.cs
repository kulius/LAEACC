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
  public class Groups : Report
  {
    public FastReport.Report Report;
    public FastReport.Engine.ReportEngine Engine;
    public FastReport.DataBand Data1;
    public FastReport.GroupFooterBand GroupFooter1;
    public FastReport.GroupHeaderBand GroupHeader1;
    public FastReport.ReportPage Page1;
    public FastReport.PageFooterBand PageFooter1;
    public FastReport.ReportTitleBand ReportTitle1;
    public FastReport.TextObject Text1;
    public FastReport.TextObject Text2;
    public FastReport.TextObject Text4;
    public FastReport.TextObject Text7;
    public FastReport.TextObject Text8;
    public FastReport.TextObject Text9;
    protected override object CalcExpression(string expression, Variant Value)
    {
      if (expression == "[Products.ProductName].Substring(0,1)")
        return ((String)Report.GetColumnValue("Products.ProductName")).Substring(0,1);
      if (expression == "[Row#] % 2 == 0")
        return ((Int32)Report.GetParameterValue("Row#")) % 2 == 0;
      return null;
    }

    private void InitializeComponent()
    {
      string reportString = 
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Report TextQuality=\"Regular\" ReportInf" +
        "o.Description=\"Demonstrates group report. To create it:&#13;&#10;&#13;&#10;- go " +
        "&quot;Report|Configure Bands...&quot; menu;&#13;&#10;&#13;&#10;- press &quot;Add" +
        "&quot; button and add a group header (this will add a data band and group footer" +
        " as well);&#13;&#10;&#13;&#10;- return to the report page, doubleclick the group" +
        " header to show its editor.\" ReportInfo.Created=\"01/17/2008 04:31:41\" ReportInfo" +
        ".Modified=\"11/11/2008 18:10:48\" ReportInfo.CreatorVersion=\"1.0.0.0\">\r\n  <Styles>" +
        "\r\n    <Style Name=\"EvenRows\" Fill.Color=\"OldLace\"/>\r\n  </Styles>\r\n  <Dictionary>" +
        "\r\n    <TableDataSource Name=\"Products\" ReferenceName=\"NorthWind.Products\" Enable" +
        "d=\"true\">\r\n      <Column Name=\"ProductID\" DataType=\"System.Int32\"/>\r\n      <Colu" +
        "mn Name=\"ProductName\" DataType=\"System.String\"/>\r\n      <Column Name=\"SupplierID" +
        "\" DataType=\"System.Int32\"/>\r\n      <Column Name=\"CategoryID\" DataType=\"System.In" +
        "t32\"/>\r\n      <Column Name=\"QuantityPerUnit\" DataType=\"System.String\"/>\r\n      <" +
        "Column Name=\"UnitPrice\" DataType=\"System.Decimal\"/>\r\n      <Column Name=\"UnitsIn";
      reportString += "Stock\" DataType=\"System.Int16\"/>\r\n      <Column Name=\"UnitsOnOrder\" DataType=\"Sy" +
        "stem.Int16\"/>\r\n      <Column Name=\"ReorderLevel\" DataType=\"System.Int16\"/>\r\n    " +
        "  <Column Name=\"Discontinued\" DataType=\"System.Boolean\" BindableControl=\"CheckBo" +
        "x\"/>\r\n      <Column Name=\"EAN13\" DataType=\"System.String\"/>\r\n    </TableDataSour" +
        "ce>\r\n    <Total Name=\"TotalProducts\" TotalType=\"Count\" Evaluator=\"Data1\" Resette" +
        "r=\"GroupFooter1\"/>\r\n  </Dictionary>\r\n  <ReportPage Name=\"Page1\">\r\n    <ReportTit" +
        "leBand Name=\"ReportTitle1\" Width=\"718.2\" Height=\"37.8\">\r\n      <TextObject Name=" +
        "\"Text1\" Width=\"718.2\" Height=\"28.35\" Text=\"ALPHABETICAL PRODUCT LIST\" HorzAlign=" +
        "\"Center\" VertAlign=\"Center\" Font=\"Tahoma, 14pt, style=Bold\"/>\r\n    </ReportTitle" +
        "Band>\r\n    <GroupHeaderBand Name=\"GroupHeader1\" Top=\"41.8\" Width=\"718.2\" Height=" +
        "\"28.35\" KeepWithData=\"true\" Condition=\"[Products.ProductName].Substring(0,1)\" So" +
        "rtOrder=\"None\">\r\n      <TextObject Name=\"Text7\" Left=\"9.45\" Width=\"359.1\" Height";
      reportString += "=\"28.35\" Border.Lines=\"All\" Border.Color=\"LightSkyBlue\" Fill=\"Glass\" Fill.Color=" +
        "\"LightSkyBlue\" Fill.Blend=\"0.2\" Fill.Hatch=\"false\" Text=\"[[Products.ProductName]" +
        ".Substring(0,1)]\" Padding=\"5, 0, 0, 0\" VertAlign=\"Center\" Font=\"Tahoma, 10pt, st" +
        "yle=Bold\"/>\r\n      <DataBand Name=\"Data1\" Top=\"74.15\" Width=\"718.2\" Height=\"18.9" +
        "\" DataSource=\"Products\">\r\n        <TextObject Name=\"Text2\" Left=\"9.45\" Width=\"26" +
        "4.6\" Height=\"18.9\" Border.Lines=\"Left\" Border.Color=\"LightSkyBlue\" Text=\"[Produc" +
        "ts.ProductName]\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\">\r\n          <Highlight>\r\n" +
        "            <Condition Expression=\"[Row#] % 2 == 0\" Fill.Color=\"AliceBlue\" TextF" +
        "ill.Color=\"Black\" ApplyFill=\"true\" ApplyTextFill=\"false\"/>\r\n          </Highligh" +
        "t>\r\n        </TextObject>\r\n        <TextObject Name=\"Text4\" Left=\"274.05\" Width=" +
        "\"94.5\" Height=\"18.9\" Border.Lines=\"Right\" Border.Color=\"LightSkyBlue\" Text=\"[Pro" +
        "ducts.UnitPrice]\" Format=\"Currency\" Format.UseLocale=\"true\" HorzAlign=\"Right\" Ve";
      reportString += "rtAlign=\"Center\" Font=\"Tahoma, 8pt\">\r\n          <Highlight>\r\n            <Condit" +
        "ion Expression=\"[Row#] % 2 == 0\" Fill.Color=\"AliceBlue\" TextFill.Color=\"Black\" A" +
        "pplyFill=\"true\" ApplyTextFill=\"false\"/>\r\n          </Highlight>\r\n        </TextO" +
        "bject>\r\n        <Sort>\r\n          <Sort Expression=\"[Products.ProductName]\"/>\r\n " +
        "       </Sort>\r\n      </DataBand>\r\n      <GroupFooterBand Name=\"GroupFooter1\" To" +
        "p=\"97.05\" Width=\"718.2\" Height=\"47.25\" KeepWithData=\"true\">\r\n        <TextObject" +
        " Name=\"Text8\" Left=\"9.45\" Width=\"359.1\" Height=\"18.9\" Border.Lines=\"Left, Right," +
        " Bottom\" Border.Color=\"LightSkyBlue\" Text=\"Total products: [TotalProducts]\" Horz" +
        "Align=\"Right\" VertAlign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n      </Group" +
        "FooterBand>\r\n    </GroupHeaderBand>\r\n    <PageFooterBand Name=\"PageFooter1\" Top=" +
        "\"148.3\" Width=\"718.2\" Height=\"18.9\">\r\n      <TextObject Name=\"Text9\" Left=\"623.7" +
        "\" Width=\"94.5\" Height=\"18.9\" Text=\"[PageN]\" HorzAlign=\"Right\" VertAlign=\"Center\"";
      reportString += " Font=\"Tahoma, 8pt\"/>\r\n    </PageFooterBand>\r\n  </ReportPage>\r\n</Report>\r\n";
      LoadFromString(reportString);

      InternalInit();
    }

    public Groups()
    {
      InitializeComponent();
    }
  }
}
