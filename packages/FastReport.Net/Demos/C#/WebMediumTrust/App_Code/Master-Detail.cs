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
  public class MasterDetail : Report
  {
    public FastReport.Report Report;
    public FastReport.Engine.ReportEngine Engine;
    public FastReport.CheckBoxObject CheckBox1;
    public FastReport.DataBand Data1;
    public FastReport.DataBand Data2;
    public FastReport.DataFooterBand DataFooter1;
    public FastReport.DataHeaderBand DataHeader1;
    public FastReport.ReportPage Page1;
    public FastReport.PageFooterBand PageFooter1;
    public FastReport.PictureObject Picture1;
    public FastReport.ReportTitleBand ReportTitle1;
    public FastReport.TextObject Text1;
    public FastReport.TextObject Text10;
    public FastReport.TextObject Text12;
    public FastReport.TextObject Text2;
    public FastReport.TextObject Text3;
    public FastReport.TextObject Text4;
    public FastReport.TextObject Text5;
    public FastReport.TextObject Text6;
    public FastReport.TextObject Text7;
    public FastReport.TextObject Text8;
    public FastReport.TextObject Text9;
    protected override object CalcExpression(string expression, Variant Value)
    {
      return null;
    }

    private void InitializeComponent()
    {
      string reportString = 
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Report TextQuality=\"Regular\" ReportInf" +
        "o.Description=\"Demonstrates master-detail report. To create it:&#13;&#10;&#13;&#" +
        "10;- go &quot;Report|Configure Bands...&quot; menu;&#13;&#10;&#13;&#10;- select " +
        "existing data band;&#13;&#10;&#13;&#10;- press &quot;Add&quot; button and select" +
        " the &quot;Data&quot; band - this will add a data band to existing one;&#13;&#10" +
        ";&#13;&#10;- return to the report page, doubleclick each data band and set its d" +
        "atasource.&#13;&#10;&#13;&#10;Note: if you have defined the relation between mas" +
        "ter and detail tables, FastReport will use it automatically.\" ReportInfo.Created" +
        "=\"01/17/2008 03:55:42\" ReportInfo.Modified=\"11/11/2008 18:11:19\" ReportInfo.Crea" +
        "torVersion=\"1.0.0.0\">\r\n  <Styles>\r\n    <Style Name=\"EvenRows\"/>\r\n  </Styles>\r\n  " +
        "<Dictionary>\r\n    <TableDataSource Name=\"Categories\" ReferenceName=\"NorthWind.Ca" +
        "tegories\" Enabled=\"true\">\r\n      <Column Name=\"CategoryID\" DataType=\"System.Int3" +
        "2\"/>\r\n      <Column Name=\"CategoryName\" DataType=\"System.String\"/>\r\n      <Colum" +
        "n Name=\"Description\" DataType=\"System.String\"/>\r\n      <Column Name=\"Picture\" Da";
      reportString += "taType=\"System.Byte[]\" BindableControl=\"Picture\"/>\r\n    </TableDataSource>\r\n    " +
        "<TableDataSource Name=\"Products\" ReferenceName=\"NorthWind.Products\" Enabled=\"tru" +
        "e\">\r\n      <Column Name=\"ProductID\" DataType=\"System.Int32\"/>\r\n      <Column Nam" +
        "e=\"ProductName\" DataType=\"System.String\"/>\r\n      <Column Name=\"SupplierID\" Data" +
        "Type=\"System.Int32\"/>\r\n      <Column Name=\"CategoryID\" DataType=\"System.Int32\"/>" +
        "\r\n      <Column Name=\"QuantityPerUnit\" DataType=\"System.String\"/>\r\n      <Column" +
        " Name=\"UnitPrice\" DataType=\"System.Decimal\"/>\r\n      <Column Name=\"UnitsInStock\"" +
        " DataType=\"System.Int16\"/>\r\n      <Column Name=\"UnitsOnOrder\" DataType=\"System.I" +
        "nt16\"/>\r\n      <Column Name=\"ReorderLevel\" DataType=\"System.Int16\"/>\r\n      <Col" +
        "umn Name=\"Discontinued\" DataType=\"System.Boolean\"/>\r\n      <Column Name=\"EAN13\" " +
        "DataType=\"System.String\"/>\r\n    </TableDataSource>\r\n    <Relation Name=\"Categori" +
        "esProducts\" ReferenceName=\"NorthWind.CategoriesProducts\" ParentDataSource=\"Categ";
      reportString += "ories\" ChildDataSource=\"Products\" ParentColumns=\"CategoryID\" ChildColumns=\"Categ" +
        "oryID\"/>\r\n  </Dictionary>\r\n  <ReportPage Name=\"Page1\">\r\n    <ReportTitleBand Nam" +
        "e=\"ReportTitle1\" Width=\"718.2\" Height=\"47.25\">\r\n      <TextObject Name=\"Text3\" W" +
        "idth=\"718.2\" Height=\"37.8\" Text=\"PRODUCT CATALOG\" HorzAlign=\"Center\" VertAlign=\"" +
        "Center\" Font=\"Tahoma, 14pt, style=Bold\"/>\r\n    </ReportTitleBand>\r\n    <DataBand" +
        " Name=\"Data1\" Top=\"51.25\" Width=\"718.2\" Height=\"103.95\" Fill=\"Glass\" Fill.Color=" +
        "\"64, 64, 64\" Fill.Blend=\"0.08\" Fill.Hatch=\"true\" DataSource=\"Categories\" KeepDet" +
        "ail=\"true\">\r\n      <TextObject Name=\"Text1\" Left=\"160.65\" Top=\"9.45\" Width=\"302." +
        "4\" Height=\"37.8\" Text=\"[Categories.CategoryName]\" Font=\"Tahoma, 18pt\" TextFill.C" +
        "olor=\"White\"/>\r\n      <PictureObject Name=\"Picture1\" Left=\"9.45\" Top=\"9.45\" Widt" +
        "h=\"141.75\" Height=\"85.05\" Border.Lines=\"All\" Border.Color=\"Gray\" Border.Width=\"2" +
        "\" SizeMode=\"StretchImage\" DataColumn=\"Categories.Picture\"/>\r\n      <TextObject N";
      reportString += "ame=\"Text2\" Left=\"160.65\" Top=\"56.7\" Width=\"302.4\" Height=\"37.8\" Text=\"[Categori" +
        "es.Description]\" Font=\"Tahoma, 8pt\" TextFill.Color=\"White\"/>\r\n      <DataBand Na" +
        "me=\"Data2\" Top=\"191.55\" Width=\"718.2\" Height=\"18.9\" Fill.Color=\"WhiteSmoke\" Even" +
        "Style=\"EvenRows\" DataSource=\"Products\">\r\n        <TextObject Name=\"Text5\" Width=" +
        "\"292.95\" Height=\"18.9\" Text=\"[Products.ProductName]\" VertAlign=\"Center\" Font=\"Ta" +
        "homa, 8pt\"/>\r\n        <TextObject Name=\"Text7\" Left=\"321.3\" Width=\"122.85\" Heigh" +
        "t=\"18.9\" Text=\"[Products.UnitPrice]\" Format=\"Currency\" Format.UseLocale=\"true\" H" +
        "orzAlign=\"Right\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\"/>\r\n        <TextObject Na" +
        "me=\"Text9\" Left=\"472.5\" Width=\"122.85\" Height=\"18.9\" Text=\"[Products.UnitsInStoc" +
        "k]\" HorzAlign=\"Center\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\"/>\r\n        <CheckBo" +
        "xObject Name=\"CheckBox1\" Left=\"661.5\" Width=\"18.9\" Height=\"18.9\" Checked=\"false\"" +
        " DataColumn=\"Products.Discontinued\"/>\r\n        <DataHeaderBand Name=\"DataHeader1";
      reportString += "\" Top=\"159.2\" Width=\"718.2\" Height=\"28.35\">\r\n          <TextObject Name=\"Text6\" " +
        "Width=\"292.95\" Height=\"28.35\" Text=\"Product name\" VertAlign=\"Center\" Font=\"Tahom" +
        "a, 8pt, style=Bold\"/>\r\n          <TextObject Name=\"Text8\" Left=\"321.3\" Width=\"12" +
        "2.85\" Height=\"28.35\" Text=\"Unit price\" HorzAlign=\"Right\" VertAlign=\"Center\" Font" +
        "=\"Tahoma, 8pt, style=Bold\"/>\r\n          <TextObject Name=\"Text10\" Left=\"472.5\" W" +
        "idth=\"122.85\" Height=\"28.35\" Text=\"Units in stock\" HorzAlign=\"Center\" VertAlign=" +
        "\"Center\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n          <TextObject Name=\"Text12\" L" +
        "eft=\"623.7\" Width=\"94.5\" Height=\"28.35\" Text=\"Discontinued\" HorzAlign=\"Center\" V" +
        "ertAlign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n        </DataHeaderBand>\r\n " +
        "       <DataFooterBand Name=\"DataFooter1\" Top=\"214.45\" Width=\"718.2\" Height=\"37." +
        "8\"/>\r\n        <Sort>\r\n          <Sort Expression=\"[Products.ProductName]\"/>\r\n   " +
        "     </Sort>\r\n      </DataBand>\r\n    </DataBand>\r\n    <PageFooterBand Name=\"Page";
      reportString += "Footer1\" Top=\"256.25\" Width=\"718.2\" Height=\"18.9\">\r\n      <TextObject Name=\"Text" +
        "4\" Left=\"623.7\" Width=\"94.5\" Height=\"18.9\" Text=\"[PageN]\" HorzAlign=\"Right\" Font" +
        "=\"Tahoma, 8pt\"/>\r\n    </PageFooterBand>\r\n  </ReportPage>\r\n</Report>\r\n";
      LoadFromString(reportString);

      InternalInit();
    }

    public MasterDetail()
    {
      InitializeComponent();
    }
  }
}
