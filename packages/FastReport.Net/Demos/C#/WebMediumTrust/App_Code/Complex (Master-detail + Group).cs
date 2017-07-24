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
  public class ComplexMasterdetailGroup : Report
  {
    public FastReport.Report Report;
    public FastReport.Engine.ReportEngine Engine;
    public FastReport.DataBand Data1;
    public FastReport.DataBand Data2;
    public FastReport.DataFooterBand DataFooter1;
    public FastReport.GroupHeaderBand GroupHeader1;
    public FastReport.ReportPage Page1;
    public FastReport.PageFooterBand PageFooter1;
    public FastReport.ReportTitleBand ReportTitle1;
    public FastReport.TextObject Text1;
    public FastReport.TextObject Text10;
    public FastReport.TextObject Text11;
    public FastReport.TextObject Text12;
    public FastReport.TextObject Text13;
    public FastReport.TextObject Text14;
    public FastReport.TextObject Text15;
    public FastReport.TextObject Text16;
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
      if (expression == "Orders.Customers.CompanyName")
        return ((String)Report.GetColumnValue("Orders.Customers.CompanyName"));
      if (expression == "Order Details.Products.ProductName")
        return ((String)Report.GetColumnValue("Order Details.Products.ProductName"));
      if (expression == "[Order Details.UnitPrice] * [Order Details.Quantity]")
        return ((Decimal)Report.GetColumnValue("Order Details.UnitPrice")) * ((Int16)Report.GetColumnValue("Order Details.Quantity"));
      return null;
    }

    private void InitializeComponent()
    {
      string reportString = 
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Report TextQuality=\"Regular\" ReportInf" +
        "o.Description=\"Demonstrates the master-detail report with groups. To create this" +
        " report:&#13;&#10;&#13;&#10;- go &quot;Report|Configure Bands...&quot; menu and " +
        "create the report structure.&#13;&#10;&#13;&#10;The page can contain service ban" +
        "ds such as page header, report title, and one or several data bands or groups.&#" +
        "13;&#10;&#13;&#10;Each data band can contain one or several data bands or groups" +
        ".&#13;&#10;&#13;&#10;Each group can contain either nested group or data band. \" " +
        "ReportInfo.Created=\"01/18/2008 00:04:46\" ReportInfo.Modified=\"11/11/2008 18:11:5" +
        "8\" ReportInfo.CreatorVersion=\"1.0.0.0\">\r\n  <Styles>\r\n    <Style Name=\"EvenRows\" " +
        "Fill.Color=\"Linen\"/>\r\n  </Styles>\r\n  <Dictionary>\r\n    <TableDataSource Name=\"Cu" +
        "stomers\" ReferenceName=\"NorthWind.Customers\" Enabled=\"true\">\r\n      <Column Name" +
        "=\"CustomerID\" DataType=\"System.String\"/>\r\n      <Column Name=\"CompanyName\" DataT" +
        "ype=\"System.String\"/>\r\n      <Column Name=\"ContactName\" DataType=\"System.String\"" +
        "/>\r\n      <Column Name=\"ContactTitle\" DataType=\"System.String\"/>\r\n      <Column ";
      reportString += "Name=\"Address\" DataType=\"System.String\"/>\r\n      <Column Name=\"City\" DataType=\"S" +
        "ystem.String\"/>\r\n      <Column Name=\"Region\" DataType=\"System.String\"/>\r\n      <" +
        "Column Name=\"PostalCode\" DataType=\"System.String\"/>\r\n      <Column Name=\"Country" +
        "\" DataType=\"System.String\"/>\r\n      <Column Name=\"Phone\" DataType=\"System.String" +
        "\"/>\r\n      <Column Name=\"Fax\" DataType=\"System.String\"/>\r\n    </TableDataSource>" +
        "\r\n    <TableDataSource Name=\"Orders\" ReferenceName=\"NorthWind.Orders\" Enabled=\"t" +
        "rue\">\r\n      <Column Name=\"OrderID\" DataType=\"System.Int32\"/>\r\n      <Column Nam" +
        "e=\"CustomerID\" DataType=\"System.String\"/>\r\n      <Column Name=\"EmployeeID\" DataT" +
        "ype=\"System.Int32\"/>\r\n      <Column Name=\"OrderDate\" DataType=\"System.DateTime\"/" +
        ">\r\n      <Column Name=\"RequiredDate\" DataType=\"System.DateTime\"/>\r\n      <Column" +
        " Name=\"ShippedDate\" DataType=\"System.DateTime\"/>\r\n      <Column Name=\"ShipVia\" D" +
        "ataType=\"System.Int32\"/>\r\n      <Column Name=\"Freight\" DataType=\"System.Decimal\"";
      reportString += "/>\r\n      <Column Name=\"ShipName\" DataType=\"System.String\"/>\r\n      <Column Name" +
        "=\"ShipAddress\" DataType=\"System.String\"/>\r\n      <Column Name=\"ShipCity\" DataTyp" +
        "e=\"System.String\"/>\r\n      <Column Name=\"ShipRegion\" DataType=\"System.String\"/>\r" +
        "\n      <Column Name=\"ShipPostalCode\" DataType=\"System.String\"/>\r\n      <Column N" +
        "ame=\"ShipCountry\" DataType=\"System.String\"/>\r\n    </TableDataSource>\r\n    <Table" +
        "DataSource Name=\"Order Details\" ReferenceName=\"NorthWind.Order Details\" Enabled=" +
        "\"true\">\r\n      <Column Name=\"OrderID\" DataType=\"System.Int32\"/>\r\n      <Column N" +
        "ame=\"ProductID\" DataType=\"System.Int32\"/>\r\n      <Column Name=\"UnitPrice\" DataTy" +
        "pe=\"System.Decimal\"/>\r\n      <Column Name=\"Quantity\" DataType=\"System.Int16\"/>\r\n" +
        "      <Column Name=\"Discount\" DataType=\"System.Single\"/>\r\n    </TableDataSource>" +
        "\r\n    <TableDataSource Name=\"Products\" ReferenceName=\"NorthWind.Products\" Enable" +
        "d=\"true\">\r\n      <Column Name=\"ProductID\" DataType=\"System.Int32\"/>\r\n      <Colu";
      reportString += "mn Name=\"ProductName\" DataType=\"System.String\"/>\r\n      <Column Name=\"SupplierID" +
        "\" DataType=\"System.Int32\"/>\r\n      <Column Name=\"CategoryID\" DataType=\"System.In" +
        "t32\"/>\r\n      <Column Name=\"QuantityPerUnit\" DataType=\"System.String\"/>\r\n      <" +
        "Column Name=\"UnitPrice\" DataType=\"System.Decimal\"/>\r\n      <Column Name=\"UnitsIn" +
        "Stock\" DataType=\"System.Int16\"/>\r\n      <Column Name=\"UnitsOnOrder\" DataType=\"Sy" +
        "stem.Int16\"/>\r\n      <Column Name=\"ReorderLevel\" DataType=\"System.Int16\"/>\r\n    " +
        "  <Column Name=\"Discontinued\" DataType=\"System.Boolean\" BindableControl=\"CheckBo" +
        "x\"/>\r\n      <Column Name=\"EAN13\" DataType=\"System.String\"/>\r\n    </TableDataSour" +
        "ce>\r\n    <Relation Name=\"CustomersOrders\" ReferenceName=\"NorthWind.CustomersOrde" +
        "rs\" ParentDataSource=\"Customers\" ChildDataSource=\"Orders\" ParentColumns=\"Custome" +
        "rID\" ChildColumns=\"CustomerID\"/>\r\n    <Relation Name=\"OrdersOrderDetails\" Refere" +
        "nceName=\"NorthWind.OrdersOrderDetails\" ParentDataSource=\"Orders\" ChildDataSource";
      reportString += "=\"Order Details\" ParentColumns=\"OrderID\" ChildColumns=\"OrderID\"/>\r\n    <Relation" +
        " Name=\"ProductsOrderDetails\" ReferenceName=\"NorthWind.ProductsOrderDetails\" Pare" +
        "ntDataSource=\"Products\" ChildDataSource=\"Order Details\" ParentColumns=\"ProductID" +
        "\" ChildColumns=\"ProductID\"/>\r\n    <Total Name=\"SumOfOrder\" Expression=\"[Order De" +
        "tails.UnitPrice] * [Order Details.Quantity]\" Evaluator=\"Data1\" Resetter=\"DataFoo" +
        "ter1\"/>\r\n  </Dictionary>\r\n  <ReportPage Name=\"Page1\">\r\n    <ReportTitleBand Name" +
        "=\"ReportTitle1\" Width=\"718.2\" Height=\"37.8\">\r\n      <TextObject Name=\"Text6\" Wid" +
        "th=\"718.2\" Height=\"28.35\" Text=\"CUSTOMERS ORDERS\" HorzAlign=\"Center\" VertAlign=\"" +
        "Center\" Font=\"Tahoma, 14pt, style=Bold\"/>\r\n    </ReportTitleBand>\r\n    <GroupHea" +
        "derBand Name=\"GroupHeader1\" Top=\"41.8\" Width=\"718.2\" Height=\"28.35\" Fill=\"Glass\"" +
        " Fill.Color=\"YellowGreen\" Fill.Blend=\"0.2\" Fill.Hatch=\"true\" Condition=\"[Orders." +
        "CustomerID]\">\r\n      <TextObject Name=\"Text1\" Width=\"349.65\" Height=\"28.35\" Text";
      reportString += "=\"[Orders.Customers.CompanyName]\" VertAlign=\"Center\" Font=\"Tahoma, 12pt, style=B" +
        "old\" TextFill.Color=\"White\"/>\r\n      <DataBand Name=\"Data2\" Top=\"74.15\" Width=\"7" +
        "18.2\" Height=\"47.25\" Border.Lines=\"Top\" Border.Color=\"White\" Border.Width=\"2\" Fi" +
        "ll.Color=\"230, 255, 204\" DataSource=\"Orders\" PrintIfDetailEmpty=\"true\">\r\n       " +
        " <TextObject Name=\"Text4\" Left=\"66.15\" Width=\"94.5\" Height=\"18.9\" Text=\"[Orders." +
        "OrderID]\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\"/>\r\n        <TextObject Name=\"Tex" +
        "t2\" Left=\"283.5\" Width=\"94.5\" Height=\"18.9\" Text=\"[Orders.OrderDate]\" Format=\"Da" +
        "te\" Format.Format=\"d\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\"/>\r\n        <TextObje" +
        "ct Name=\"Text7\" Left=\"472.5\" Width=\"122.85\" Height=\"18.9\" Text=\"[Orders.ShippedD" +
        "ate]\" Format=\"Date\" Format.Format=\"d\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\"/>\r\n " +
        "       <TextObject Name=\"Text5\" Width=\"66.15\" Height=\"18.9\" Text=\"OrderID\" VertA" +
        "lign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n        <TextObject Name=\"Text3\"";
      reportString += " Left=\"207.9\" Width=\"75.6\" Height=\"18.9\" Text=\"OrderDate\" HorzAlign=\"Right\" Vert" +
        "Align=\"Center\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n        <TextObject Name=\"Text8" +
        "\" Left=\"387.45\" Width=\"85.05\" Height=\"18.9\" Text=\"ShippedDate\" HorzAlign=\"Right\"" +
        " VertAlign=\"Center\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n        <TextObject Name=\"" +
        "Text14\" Left=\"18.9\" Top=\"28.35\" Width=\"189\" Height=\"18.9\" Border.Lines=\"All\" Bor" +
        "der.Color=\"White\" Fill.Color=\"YellowGreen\" Text=\"Product name\" VertAlign=\"Center" +
        "\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n        <TextObject Name=\"Text15\" Left=\"207." +
        "9\" Top=\"28.35\" Width=\"94.5\" Height=\"18.9\" Border.Lines=\"All\" Border.Color=\"White" +
        "\" Fill.Color=\"YellowGreen\" Text=\"Unit Price\" HorzAlign=\"Right\" VertAlign=\"Center" +
        "\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n        <TextObject Name=\"Text16\" Left=\"302." +
        "4\" Top=\"28.35\" Width=\"94.5\" Height=\"18.9\" Border.Lines=\"All\" Border.Color=\"White" +
        "\" Fill.Color=\"YellowGreen\" Text=\"Quantity\" HorzAlign=\"Center\" VertAlign=\"Center\"";
      reportString += " Font=\"Tahoma, 8pt, style=Bold\"/>\r\n        <DataBand Name=\"Data1\" Top=\"125.4\" Wi" +
        "dth=\"718.2\" Height=\"18.9\" Fill.Color=\"230, 255, 204\" DataSource=\"Order Details\">" +
        "\r\n          <TextObject Name=\"Text11\" Left=\"18.9\" Width=\"189\" Height=\"18.9\" Bord" +
        "er.Lines=\"All\" Border.Color=\"White\" Fill.Color=\"YellowGreen\" Text=\"[Order Detail" +
        "s.Products.ProductName]\" VertAlign=\"Center\" Font=\"Tahoma, 8pt\"/>\r\n          <Tex" +
        "tObject Name=\"Text12\" Left=\"207.9\" Width=\"94.5\" Height=\"18.9\" Border.Lines=\"All\"" +
        " Border.Color=\"White\" Fill.Color=\"YellowGreen\" Text=\"[Order Details.UnitPrice]\" " +
        "Format=\"Currency\" Format.UseLocale=\"true\" HorzAlign=\"Right\" VertAlign=\"Center\" F" +
        "ont=\"Tahoma, 8pt\"/>\r\n          <TextObject Name=\"Text13\" Left=\"302.4\" Width=\"94." +
        "5\" Height=\"18.9\" Border.Lines=\"All\" Border.Color=\"White\" Fill.Color=\"YellowGreen" +
        "\" Text=\"[Order Details.Quantity]\" HorzAlign=\"Center\" VertAlign=\"Center\" Font=\"Ta" +
        "homa, 8pt\"/>\r\n          <DataFooterBand Name=\"DataFooter1\" Top=\"148.3\" Width=\"71";
      reportString += "8.2\" Height=\"37.8\" Fill.Color=\"230, 255, 204\">\r\n            <TextObject Name=\"Te" +
        "xt9\" Left=\"18.9\" Width=\"378\" Height=\"18.9\" Border.Lines=\"All\" Border.Color=\"Whit" +
        "e\" Fill.Color=\"YellowGreen\" Text=\"Total this order: [SumOfOrder]\" Format=\"Curren" +
        "cy\" Format.UseLocale=\"true\" HorzAlign=\"Right\" VertAlign=\"Center\" Font=\"Tahoma, 8" +
        "pt, style=Bold\"/>\r\n          </DataFooterBand>\r\n        </DataBand>\r\n        <So" +
        "rt>\r\n          <Sort Expression=\"[Orders.OrderID]\"/>\r\n        </Sort>\r\n      </D" +
        "ataBand>\r\n    </GroupHeaderBand>\r\n    <PageFooterBand Name=\"PageFooter1\" Top=\"19" +
        "0.1\" Width=\"718.2\" Height=\"18.9\">\r\n      <TextObject Name=\"Text10\" Left=\"623.7\" " +
        "Width=\"94.5\" Height=\"18.9\" Text=\"[PageN]\" HorzAlign=\"Right\" Font=\"Tahoma, 8pt\"/>" +
        "\r\n    </PageFooterBand>\r\n  </ReportPage>\r\n</Report>\r\n";
      LoadFromString(reportString);

      InternalInit();
    }

    public ComplexMasterdetailGroup()
    {
      InitializeComponent();
    }
  }
}
