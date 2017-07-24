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
  public class SimpleList : Report
  {
    public FastReport.Report Report;
    public FastReport.Engine.ReportEngine Engine;
    public FastReport.ChildBand Child1;
    public FastReport.DataBand Data1;
    public FastReport.LineObject Line1;
    public FastReport.ReportPage Page1;
    public FastReport.PageFooterBand PageFooter1;
    public FastReport.PictureObject Picture1;
    public FastReport.ReportTitleBand ReportTitle1;
    public FastReport.TextObject Text1;
    public FastReport.TextObject Text10;
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
        "o.Description=\"Demonstrates simple list report. To create it:&#13;&#10;&#13;&#10" +
        ";- go &quot;Data&quot; menu and select &quot;Choose Report Data...&quot; item to" +
        " select datasource;&#13;&#10;&#13;&#10;- go &quot;Report|Configure Bands...&quot" +
        "; menu to create the band structure;&#13;&#10;&#13;&#10;- return to the report p" +
        "age, doubleclick the data band to show its editor;&#13;&#10;&#13;&#10;- choose t" +
        "he datasource;&#13;&#10;&#13;&#10;- drag data from the Data Dictionary window to" +
        " the band.\" ReportInfo.Created=\"01/17/2008 03:05:57\" ReportInfo.Modified=\"11/11/" +
        "2008 18:10:37\" ReportInfo.CreatorVersion=\"1.0.0.0\">\r\n  <Dictionary>\r\n    <TableD" +
        "ataSource Name=\"Employees\" ReferenceName=\"NorthWind.Employees\" Enabled=\"true\">\r\n" +
        "      <Column Name=\"EmployeeID\" DataType=\"System.Int32\"/>\r\n      <Column Name=\"L" +
        "astName\" DataType=\"System.String\"/>\r\n      <Column Name=\"FirstName\" DataType=\"Sy" +
        "stem.String\"/>\r\n      <Column Name=\"Title\" DataType=\"System.String\"/>\r\n      <Co" +
        "lumn Name=\"TitleOfCourtesy\" DataType=\"System.String\"/>\r\n      <Column Name=\"Birt";
      reportString += "hDate\" DataType=\"System.DateTime\"/>\r\n      <Column Name=\"HireDate\" DataType=\"Sys" +
        "tem.DateTime\"/>\r\n      <Column Name=\"Address\" DataType=\"System.String\"/>\r\n      " +
        "<Column Name=\"City\" DataType=\"System.String\"/>\r\n      <Column Name=\"Region\" Data" +
        "Type=\"System.String\"/>\r\n      <Column Name=\"PostalCode\" DataType=\"System.String\"" +
        "/>\r\n      <Column Name=\"Country\" DataType=\"System.String\"/>\r\n      <Column Name=" +
        "\"HomePhone\" DataType=\"System.String\"/>\r\n      <Column Name=\"Extension\" DataType=" +
        "\"System.String\"/>\r\n      <Column Name=\"Photo\" DataType=\"System.Byte[]\" BindableC" +
        "ontrol=\"Picture\"/>\r\n      <Column Name=\"Notes\" DataType=\"System.String\"/>\r\n     " +
        " <Column Name=\"ReportsTo\" DataType=\"System.Int32\"/>\r\n    </TableDataSource>\r\n  <" +
        "/Dictionary>\r\n  <ReportPage Name=\"Page1\">\r\n    <ReportTitleBand Name=\"ReportTitl" +
        "e1\" Width=\"718.2\" Height=\"56.7\">\r\n      <TextObject Name=\"Text1\" Width=\"718.2\" H" +
        "eight=\"37.8\" Text=\"EMPLOYEES\" HorzAlign=\"Center\" VertAlign=\"Center\" Font=\"Tahoma";
      reportString += ", 14pt, style=Bold\"/>\r\n    </ReportTitleBand>\r\n    <DataBand Name=\"Data1\" Top=\"6" +
        "0.7\" Width=\"718.2\" Height=\"151.2\" CanGrow=\"true\" CanShrink=\"true\" DataSource=\"Em" +
        "ployees\">\r\n      <TextObject Name=\"Text2\" Left=\"189\" Width=\"321.3\" Height=\"28.35" +
        "\" Border.Lines=\"All\" Border.Color=\"Gainsboro\" Fill=\"Glass\" Fill.Color=\"239, 239," +
        " 239\" Fill.Blend=\"0.73\" Fill.Hatch=\"false\" Text=\"[Employees.FirstName] [Employee" +
        "s.LastName]\" VertAlign=\"Center\" Font=\"Tahoma, 12pt, style=Bold\"/>\r\n      <Pictur" +
        "eObject Name=\"Picture1\" Width=\"160\" Height=\"170\" Border.Lines=\"All\" Border.Color" +
        "=\"Gainsboro\" Border.Width=\"2\" CanGrow=\"true\" CanShrink=\"true\" SizeMode=\"AutoSize" +
        "\" DataColumn=\"Employees.Photo\"/>\r\n      <TextObject Name=\"Text3\" Left=\"189\" Top=" +
        "\"37.8\" Width=\"85.05\" Height=\"18.9\" Text=\"Birth date:\" Font=\"Tahoma, 8pt, style=B" +
        "old\"/>\r\n      <TextObject Name=\"Text4\" Left=\"283.5\" Top=\"37.8\" Width=\"226.8\" Hei" +
        "ght=\"18.9\" Text=\"[Employees.BirthDate]\" Format=\"Date\" Format.Format=\"D\" Font=\"Ta";
      reportString += "homa, 8pt\"/>\r\n      <TextObject Name=\"Text5\" Left=\"189\" Top=\"56.7\" Width=\"85.05\"" +
        " Height=\"18.9\" Text=\"Address:\" Font=\"Tahoma, 8pt, style=Bold\"/>\r\n      <TextObje" +
        "ct Name=\"Text6\" Left=\"283.5\" Top=\"56.7\" Width=\"226.8\" Height=\"18.9\" CanGrow=\"tru" +
        "e\" Text=\"[Employees.Address]\" Font=\"Tahoma, 8pt\"/>\r\n      <TextObject Name=\"Text" +
        "7\" Left=\"189\" Top=\"75.6\" Width=\"85.05\" Height=\"18.9\" Text=\"Phone:\" Font=\"Tahoma," +
        " 8pt, style=Bold\"/>\r\n      <TextObject Name=\"Text8\" Left=\"283.5\" Top=\"75.6\" Widt" +
        "h=\"226.8\" Height=\"18.9\" Text=\"[Employees.HomePhone]\" Font=\"Tahoma, 8pt\"/>\r\n     " +
        " <TextObject Name=\"Text9\" Left=\"189\" Top=\"103.95\" Width=\"321.3\" Height=\"18.9\" Ca" +
        "nGrow=\"true\" CanShrink=\"true\" Text=\"[Employees.Notes]\" HorzAlign=\"Justify\" Font=" +
        "\"Tahoma, 8pt\"/>\r\n      <ChildBand Name=\"Child1\" Top=\"215.9\" Width=\"718.2\" Height" +
        "=\"37.8\">\r\n        <LineObject Name=\"Line1\" Top=\"18.9\" Width=\"510.3\" Border.Color" +
        "=\"Gainsboro\"/>\r\n      </ChildBand>\r\n    </DataBand>\r\n    <PageFooterBand Name=\"P";
      reportString += "ageFooter1\" Top=\"257.7\" Width=\"718.2\" Height=\"18.9\">\r\n      <TextObject Name=\"Te" +
        "xt10\" Left=\"623.7\" Width=\"94.5\" Height=\"18.9\" Text=\"[PageN]\" HorzAlign=\"Right\" F" +
        "ont=\"Tahoma, 8pt\"/>\r\n    </PageFooterBand>\r\n  </ReportPage>\r\n</Report>\r\n";
      LoadFromString(reportString);

      InternalInit();
    }

    public SimpleList()
    {
      InitializeComponent();
    }
  }
}
