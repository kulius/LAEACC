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
  public class Subreport : Report
  {
    public FastReport.Report Report;
    public FastReport.Engine.ReportEngine Engine;
    public FastReport.DataBand Data1;
    public FastReport.DataBand Data2;
    public FastReport.DataFooterBand DataFooter1;
    public FastReport.DataHeaderBand DataHeader1;
    public FastReport.LineObject Line1;
    public FastReport.LineObject Line2;
    public FastReport.ReportPage Page1;
    public FastReport.ReportPage Page2;
    public FastReport.PageFooterBand PageFooter1;
    public FastReport.ReportTitleBand ReportTitle1;
    public FastReport.SubreportObject Subreport1;
    public FastReport.TextObject Text1;
    public FastReport.TextObject Text10;
    public FastReport.TextObject Text11;
    public FastReport.TextObject Text12;
    public FastReport.TextObject Text13;
    public FastReport.TextObject Text14;
    public FastReport.TextObject Text15;
    public FastReport.TextObject Text16;
    public FastReport.TextObject Text17;
    public FastReport.TextObject Text18;
    public FastReport.TextObject Text19;
    public FastReport.TextObject Text2;
    public FastReport.TextObject Text20;
    public FastReport.TextObject Text21;
    public FastReport.TextObject Text22;
    public FastReport.TextObject Text23;
    public FastReport.TextObject Text24;
    public FastReport.TextObject Text3;
    public FastReport.TextObject Text4;
    public FastReport.TextObject Text5;
    public FastReport.TextObject Text6;
    public FastReport.TextObject Text7;
    public FastReport.TextObject Text8;
    public FastReport.TextObject Text9;
    protected override object CalcExpression(string expression, Variant Value)
    {
      if (expression == "[Row#] % 2 == 0")
        return ((Int32)Report.GetParameterValue("Row#")) % 2 == 0;
      return null;
    }

    private void InitializeComponent()
    {
      string reportString = 
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Report TextQuality=\"Regular\" ReportInf" +
        "o.Description=\"Demonstrates how to use the subreport object. To create subreport" +
        ":&#13;&#10;&#13;&#10;- add &quot;Subreport&quot; object to the desired location;" +
        "&#13;&#10;&#13;&#10;- this will create a separate page where you can configure b" +
        "ands and put necessary objects on it;&#13;&#10;&#13;&#10;- if you need to print " +
        "all subreport's output on its parent band, select &quot;Subreport&quot; object, " +
        "righ-click it and select &quot;Print on Parent&quot; option in the context menu." +
        "\" ReportInfo.Created=\"01/17/2008 05:23:50\" ReportInfo.Modified=\"11/11/2008 18:11" +
        ":35\" ReportInfo.CreatorVersion=\"1.0.0.0\">\r\n  <Dictionary>\r\n    <TableDataSource " +
        "Name=\"Products\" ReferenceName=\"NorthWind.Products\" Enabled=\"true\">\r\n      <Colum" +
        "n Name=\"ProductID\" DataType=\"System.Int32\"/>\r\n      <Column Name=\"ProductName\" D" +
        "ataType=\"System.String\"/>\r\n      <Column Name=\"SupplierID\" DataType=\"System.Int3" +
        "2\"/>\r\n      <Column Name=\"CategoryID\" DataType=\"System.Int32\"/>\r\n      <Column N" +
        "ame=\"QuantityPerUnit\" DataType=\"System.String\"/>\r\n      <Column Name=\"UnitPrice\"";
      reportString += " DataType=\"System.Decimal\"/>\r\n      <Column Name=\"UnitsInStock\" DataType=\"System" +
        ".Int16\"/>\r\n      <Column Name=\"UnitsOnOrder\" DataType=\"System.Int16\"/>\r\n      <C" +
        "olumn Name=\"ReorderLevel\" DataType=\"System.Int16\"/>\r\n      <Column Name=\"Discont" +
        "inued\" DataType=\"System.Boolean\" BindableControl=\"CheckBox\"/>\r\n      <Column Nam" +
        "e=\"EAN13\" DataType=\"System.String\"/>\r\n    </TableDataSource>\r\n    <TableDataSour" +
        "ce Name=\"Suppliers\" ReferenceName=\"NorthWind.Suppliers\" Enabled=\"true\">\r\n      <" +
        "Column Name=\"SupplierID\" DataType=\"System.Int32\"/>\r\n      <Column Name=\"CompanyN" +
        "ame\" DataType=\"System.String\"/>\r\n      <Column Name=\"ContactName\" DataType=\"Syst" +
        "em.String\"/>\r\n      <Column Name=\"ContactTitle\" DataType=\"System.String\"/>\r\n    " +
        "  <Column Name=\"Address\" DataType=\"System.String\"/>\r\n      <Column Name=\"City\" D" +
        "ataType=\"System.String\"/>\r\n      <Column Name=\"Region\" DataType=\"System.String\"/" +
        ">\r\n      <Column Name=\"PostalCode\" DataType=\"System.String\"/>\r\n      <Column Nam";
      reportString += "e=\"Country\" DataType=\"System.String\"/>\r\n      <Column Name=\"Phone\" DataType=\"Sys" +
        "tem.String\"/>\r\n      <Column Name=\"Fax\" DataType=\"System.String\"/>\r\n      <Colum" +
        "n Name=\"HomePage\" DataType=\"System.String\"/>\r\n    </TableDataSource>\r\n    <Relat" +
        "ion Name=\"SuppliersProducts\" ReferenceName=\"NorthWind.SuppliersProducts\" ParentD" +
        "ataSource=\"Suppliers\" ChildDataSource=\"Products\" ParentColumns=\"SupplierID\" Chil" +
        "dColumns=\"SupplierID\"/>\r\n    <Total Name=\"TotalProducts\" TotalType=\"Count\" Evalu" +
        "ator=\"Data2\" Resetter=\"DataFooter1\"/>\r\n  </Dictionary>\r\n  <ReportPage Name=\"Page" +
        "1\">\r\n    <ReportTitleBand Name=\"ReportTitle1\" Width=\"718.2\" Height=\"37.8\">\r\n    " +
        "  <TextObject Name=\"Text6\" Width=\"718.2\" Height=\"28.35\" Text=\"PRODUCTS BY SUPPLI" +
        "ERS\" HorzAlign=\"Center\" VertAlign=\"Center\" Font=\"Tahoma, 14pt, style=Bold\"/>\r\n  " +
        "  </ReportTitleBand>\r\n    <DataBand Name=\"Data1\" Top=\"41.8\" Width=\"718.2\" Height" +
        "=\"207.9\" CanGrow=\"true\" DataSource=\"Suppliers\">\r\n      <SubreportObject Name=\"Su";
      reportString += "breport1\" Left=\"264.6\" Top=\"47.25\" Width=\"349.65\" Height=\"103.95\" ReportPage=\"Pa" +
        "ge2\" PrintOnParent=\"true\"/>\r\n      <TextObject Name=\"Text8\" Left=\"9.45\" Top=\"9.4" +
        "5\" Width=\"614.25\" Height=\"28.35\" Border.Lines=\"Bottom\" Border.Color=\"White\" Fill" +
        "=\"Glass\" Fill.Color=\"YellowGreen\" Fill.Blend=\"0.2\" Fill.Hatch=\"false\" Text=\"[Sup" +
        "pliers.CompanyName]\" Padding=\"5, 0, 0, 0\" VertAlign=\"Center\" Font=\"Tahoma, 14pt," +
        " style=Bold\" TextFill.Color=\"White\"/>\r\n      <TextObject Name=\"Text13\" Left=\"9.4" +
        "5\" Top=\"37.8\" Width=\"94.5\" Height=\"18.9\" Border.Lines=\"Top\" Border.Color=\"White\"" +
        " Fill.Color=\"YellowGreen\" Text=\"Country\" Padding=\"5, 2, 5, 0\" Font=\"Tahoma, 8pt," +
        " style=Bold\"/>\r\n      <TextObject Name=\"Text14\" Left=\"9.45\" Top=\"122.85\" Width=\"" +
        "94.5\" Height=\"18.9\" Fill.Color=\"YellowGreen\" Text=\"Phone\" Padding=\"5, 2, 5, 0\" F" +
        "ont=\"Tahoma, 8pt, style=Bold\"/>\r\n      <TextObject Name=\"Text15\" Left=\"9.45\" Top" +
        "=\"141.75\" Width=\"94.5\" Height=\"18.9\" Fill.Color=\"YellowGreen\" Text=\"Fax\" Padding";
      reportString += "=\"5, 2, 5, 0\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n      <TextObject Name=\"Text12\" " +
        "Left=\"9.45\" Top=\"56.7\" Width=\"94.5\" Height=\"28.35\" Fill.Color=\"YellowGreen\" Text" +
        "=\"Address\" Padding=\"5, 2, 5, 0\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n      <TextObj" +
        "ect Name=\"Text16\" Left=\"9.45\" Top=\"85.05\" Width=\"94.5\" Height=\"18.9\" Fill.Color=" +
        "\"YellowGreen\" Text=\"City\" Padding=\"5, 2, 5, 0\" Font=\"Tahoma, 8pt, style=Bold\"/>\r" +
        "\n      <TextObject Name=\"Text17\" Left=\"9.45\" Top=\"103.95\" Width=\"94.5\" Height=\"1" +
        "8.9\" Fill.Color=\"YellowGreen\" Text=\"Region\" Padding=\"5, 2, 5, 0\" Font=\"Tahoma, 8" +
        "pt, style=Bold\"/>\r\n      <TextObject Name=\"Text18\" Left=\"9.45\" Top=\"160.65\" Widt" +
        "h=\"94.5\" Height=\"18.9\" Fill.Color=\"YellowGreen\" Text=\"Contact name\" Padding=\"5, " +
        "2, 5, 0\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n      <TextObject Name=\"Text19\" Left=" +
        "\"9.45\" Top=\"179.55\" Width=\"94.5\" Height=\"18.9\" Fill.Color=\"YellowGreen\" Text=\"Co" +
        "ntact title\" Padding=\"5, 2, 5, 0\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n      <TextO";
      reportString += "bject Name=\"Text9\" Left=\"103.95\" Top=\"37.8\" Width=\"151.2\" Height=\"18.9\" Border.L" +
        "ines=\"Left, Top\" Border.Color=\"White\" Fill.Color=\"YellowGreen\" Text=\"[Suppliers." +
        "Country]\" Padding=\"5, 2, 0, 0\" Font=\"Tahoma, 8pt\"/>\r\n      <TextObject Name=\"Tex" +
        "t10\" Left=\"103.95\" Top=\"122.85\" Width=\"151.2\" Height=\"18.9\" Border.Lines=\"Left\" " +
        "Border.Color=\"White\" Fill.Color=\"YellowGreen\" Text=\"[Suppliers.Phone]\" Padding=\"" +
        "5, 2, 0, 0\" Font=\"Tahoma, 8pt\"/>\r\n      <TextObject Name=\"Text11\" Left=\"103.95\" " +
        "Top=\"141.75\" Width=\"151.2\" Height=\"18.9\" Border.Lines=\"Left\" Border.Color=\"White" +
        "\" Fill.Color=\"YellowGreen\" Text=\"[Suppliers.Fax]\" Padding=\"5, 2, 0, 0\" Font=\"Tah" +
        "oma, 8pt\"/>\r\n      <TextObject Name=\"Text20\" Left=\"103.95\" Top=\"56.7\" Width=\"151" +
        ".2\" Height=\"28.35\" Border.Lines=\"Left\" Border.Color=\"White\" Fill.Color=\"YellowGr" +
        "een\" Text=\"[Suppliers.Address]\" Padding=\"5, 2, 0, 0\" Font=\"Tahoma, 8pt\"/>\r\n     " +
        " <TextObject Name=\"Text21\" Left=\"103.95\" Top=\"85.05\" Width=\"151.2\" Height=\"18.9\"";
      reportString += " Border.Lines=\"Left\" Border.Color=\"White\" Fill.Color=\"YellowGreen\" Text=\"[Suppli" +
        "ers.City]\" Padding=\"5, 2, 0, 0\" Font=\"Tahoma, 8pt\"/>\r\n      <TextObject Name=\"Te" +
        "xt22\" Left=\"103.95\" Top=\"103.95\" Width=\"151.2\" Height=\"18.9\" Border.Lines=\"Left\"" +
        " Border.Color=\"White\" Fill.Color=\"YellowGreen\" Text=\"[Suppliers.Region]\" Padding" +
        "=\"5, 2, 0, 0\" Font=\"Tahoma, 8pt\"/>\r\n      <TextObject Name=\"Text23\" Left=\"103.95" +
        "\" Top=\"160.65\" Width=\"151.2\" Height=\"18.9\" Border.Lines=\"Left\" Border.Color=\"Whi" +
        "te\" Fill.Color=\"YellowGreen\" Text=\"[Suppliers.ContactName]\" Padding=\"5, 2, 0, 0\"" +
        " Font=\"Tahoma, 8pt\"/>\r\n      <TextObject Name=\"Text24\" Left=\"103.95\" Top=\"179.55" +
        "\" Width=\"151.2\" Height=\"18.9\" Border.Lines=\"Left\" Border.Color=\"White\" Fill.Colo" +
        "r=\"YellowGreen\" Text=\"[Suppliers.ContactTitle]\" Padding=\"5, 2, 0, 0\" Font=\"Tahom" +
        "a, 8pt\"/>\r\n      <LineObject Name=\"Line1\" Left=\"623.7\" Top=\"37.8\" Height=\"160.65" +
        "\" Border.Color=\"YellowGreen\"/>\r\n      <LineObject Name=\"Line2\" Left=\"255.15\" Top";
      reportString += "=\"198.45\" Width=\"368.55\" Border.Color=\"YellowGreen\"/>\r\n    </DataBand>\r\n    <Pag" +
        "eFooterBand Name=\"PageFooter1\" Top=\"253.7\" Width=\"718.2\" Height=\"18.9\">\r\n      <" +
        "TextObject Name=\"Text7\" Left=\"623.7\" Width=\"94.5\" Height=\"18.9\" Text=\"[PageN]\" H" +
        "orzAlign=\"Right\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\"/>\r\n    </PageFooterBand>\r" +
        "\n  </ReportPage>\r\n  <ReportPage Name=\"Page2\">\r\n    <DataBand Name=\"Data2\" Top=\"2" +
        "2.9\" Width=\"718.2\" Height=\"18.9\" DataSource=\"Products\">\r\n      <TextObject Name=" +
        "\"Text1\" Left=\"9.45\" Width=\"236.25\" Height=\"18.9\" Text=\"[Products.ProductName]\" V" +
        "ertAlign=\"Center\" Font=\"Tahoma, 8pt\">\r\n        <Highlight>\r\n          <Condition" +
        " Expression=\"[Row#] % 2 == 0\" Fill.Color=\"230, 255, 204\" TextFill.Color=\"Black\" " +
        "ApplyFill=\"true\" ApplyTextFill=\"false\"/>\r\n        </Highlight>\r\n      </TextObje" +
        "ct>\r\n      <TextObject Name=\"Text2\" Left=\"245.7\" Width=\"94.5\" Height=\"18.9\" Text" +
        "=\"[Products.UnitPrice]\" Format=\"Currency\" Format.UseLocale=\"true\" HorzAlign=\"Rig";
      reportString += "ht\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\">\r\n        <Highlight>\r\n          <Cond" +
        "ition Expression=\"[Row#] % 2 == 0\" Fill.Color=\"230, 255, 204\" TextFill.Color=\"Bl" +
        "ack\" ApplyFill=\"true\" ApplyTextFill=\"false\"/>\r\n        </Highlight>\r\n      </Tex" +
        "tObject>\r\n      <DataHeaderBand Name=\"DataHeader1\" Width=\"718.2\" Height=\"18.9\">\r" +
        "\n        <TextObject Name=\"Text3\" Left=\"9.45\" Width=\"236.25\" Height=\"18.9\" Text=" +
        "\"Product name\" VertAlign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n        <Tex" +
        "tObject Name=\"Text4\" Left=\"245.7\" Width=\"94.5\" Height=\"18.9\" Text=\"Unit price\" H" +
        "orzAlign=\"Right\" VertAlign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n      </Da" +
        "taHeaderBand>\r\n      <DataFooterBand Name=\"DataFooter1\" Top=\"45.8\" Width=\"718.2\"" +
        " Height=\"18.9\">\r\n        <TextObject Name=\"Text5\" Left=\"9.45\" Width=\"330.75\" Hei" +
        "ght=\"18.9\" Text=\"Total products: [TotalProducts]\" HorzAlign=\"Right\" VertAlign=\"C" +
        "enter\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n      </DataFooterBand>\r\n      <Sort>\r\n";
      reportString += "        <Sort Expression=\"[Products.ProductName]\"/>\r\n      </Sort>\r\n    </DataBa" +
        "nd>\r\n  </ReportPage>\r\n</Report>\r\n";
      LoadFromString(reportString);

      InternalInit();
    }

    public Subreport()
    {
      InitializeComponent();
    }
  }
}
