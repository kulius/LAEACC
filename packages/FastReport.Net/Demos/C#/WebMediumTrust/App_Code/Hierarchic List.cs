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
  public class HierarchicList : Report
  {
    public FastReport.Report Report;
    public FastReport.Engine.ReportEngine Engine;
    public FastReport.DataBand Data1;
    public FastReport.ReportPage Page1;
    public FastReport.PictureObject Picture1;
    public FastReport.ReportTitleBand ReportTitle1;
    public FastReport.TextObject Text1;
    public FastReport.TextObject Text2;
    public FastReport.TextObject Text3;
    protected override object CalcExpression(string expression, Variant Value)
    {
      return null;
    }

    private void InitializeComponent()
    {
      string reportString = 
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Report TextQuality=\"Regular\" ReportInf" +
        "o.Description=\"Demonstrates how to print hierarchic list. &#13;&#10;&#13;&#10;To" +
        " create hierarchy, the FastReport engine needs two values: the row ID and its pa" +
        "rent row ID. In this report, the row ID is represented by the Employee.EmployeeI" +
        "D column; its parent row is Employees.ReportsTo column. To create the report:&#1" +
        "3;&#10;&#13;&#10;- select the databand and go Properties window;&#13;&#10;&#13;&" +
        "#10;- set the &quot;IdColumn&quot; property to &quot;Employee.EmployeeID&quot;;&" +
        "#13;&#10;&#13;&#10;- set the &quot;ParentIdColumn&quot; property to &quot;Employ" +
        "ee.ReportsTo&quot;;&#13;&#10;&#13;&#10;- the &quot;Indent&quot; property will be" +
        " used to shift the databand rows according to hierarchy level. Set it to desired" +
        " value.\" ReportInfo.Created=\"01/17/2008 03:05:57\" ReportInfo.Modified=\"11/11/200" +
        "8 18:12:18\" ReportInfo.CreatorVersion=\"1.0.0.0\">\r\n  <Dictionary>\r\n    <TableData" +
        "Source Name=\"Employees\" ReferenceName=\"NorthWind.Employees\" Enabled=\"true\">\r\n   " +
        "   <Column Name=\"EmployeeID\" DataType=\"System.Int32\"/>\r\n      <Column Name=\"Last";
      reportString += "Name\" DataType=\"System.String\"/>\r\n      <Column Name=\"FirstName\" DataType=\"Syste" +
        "m.String\"/>\r\n      <Column Name=\"Title\" DataType=\"System.String\"/>\r\n      <Colum" +
        "n Name=\"TitleOfCourtesy\" DataType=\"System.String\"/>\r\n      <Column Name=\"BirthDa" +
        "te\" DataType=\"System.DateTime\"/>\r\n      <Column Name=\"HireDate\" DataType=\"System" +
        ".DateTime\"/>\r\n      <Column Name=\"Address\" DataType=\"System.String\"/>\r\n      <Co" +
        "lumn Name=\"City\" DataType=\"System.String\"/>\r\n      <Column Name=\"Region\" DataTyp" +
        "e=\"System.String\"/>\r\n      <Column Name=\"PostalCode\" DataType=\"System.String\"/>\r" +
        "\n      <Column Name=\"Country\" DataType=\"System.String\"/>\r\n      <Column Name=\"Ho" +
        "mePhone\" DataType=\"System.String\"/>\r\n      <Column Name=\"Extension\" DataType=\"Sy" +
        "stem.String\"/>\r\n      <Column Name=\"Photo\" DataType=\"System.Byte[]\" BindableCont" +
        "rol=\"Picture\"/>\r\n      <Column Name=\"Notes\" DataType=\"System.String\"/>\r\n      <C" +
        "olumn Name=\"ReportsTo\" DataType=\"System.Int32\"/>\r\n    </TableDataSource>\r\n  </Di";
      reportString += "ctionary>\r\n  <ReportPage Name=\"Page1\">\r\n    <ReportTitleBand Name=\"ReportTitle1\"" +
        " Width=\"718.2\" Height=\"56.7\">\r\n      <TextObject Name=\"Text1\" Width=\"718.2\" Heig" +
        "ht=\"37.8\" Text=\"EMPLOYEE HIERARCHY\" HorzAlign=\"Center\" VertAlign=\"Center\" Font=\"" +
        "Tahoma, 14pt, style=Bold\"/>\r\n    </ReportTitleBand>\r\n    <DataBand Name=\"Data1\" " +
        "Top=\"60.7\" Width=\"718.2\" Height=\"103.95\" DataSource=\"Employees\" IdColumn=\"Employ" +
        "ees.EmployeeID\" ParentIdColumn=\"Employees.ReportsTo\" Indent=\"85.05\">\r\n      <Tex" +
        "tObject Name=\"Text2\" Left=\"94.5\" Top=\"18.9\" Width=\"245.7\" Height=\"18.9\" Text=\"[E" +
        "mployees.FirstName] [Employees.LastName]\" VertAlign=\"Center\" Font=\"Tahoma, 10pt," +
        " style=Bold\"/>\r\n      <PictureObject Name=\"Picture1\" Width=\"85.05\" Height=\"85.05" +
        "\" CanGrow=\"true\" CanShrink=\"true\" DataColumn=\"Employees.Photo\"/>\r\n      <TextObj" +
        "ect Name=\"Text3\" Left=\"94.5\" Top=\"37.8\" Width=\"245.7\" Height=\"18.9\" Text=\"[Emplo" +
        "yees.Title]\" Font=\"Tahoma, 8pt\"/>\r\n    </DataBand>\r\n  </ReportPage>\r\n</Report>\r\n";
      LoadFromString(reportString);

      InternalInit();
    }

    public HierarchicList()
    {
      InitializeComponent();
    }
  }
}
