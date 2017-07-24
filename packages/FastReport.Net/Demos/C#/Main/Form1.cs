using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Collections;
using FastReport;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Design;
using FastReport.Design.StandardDesigner;
using DevComponents.DotNetBar;

namespace Demo
{
  public partial class Form1 : Office2007Form
  {
    private DataSet FDataSet;
    private List<Category> FBusinessObject;
    private Report FReport;
    private string FReportsFolder;

    public static void TestMessage()
    {
      MessageBox.Show("Hello from the main application!");
    }

    private void FindReportsFolder()
    {
      FReportsFolder = "";
      string thisFolder = Config.ApplicationFolder;

      for (int i = 0; i < 6; i++)
      {
        if (Directory.Exists(thisFolder + @"Demos\Reports"))
        {
          FReportsFolder = Path.GetFullPath(thisFolder + @"Demos\Reports\");
          break;
        }
        thisFolder += @"..\";
      }

      if (FReportsFolder == "")
        throw new Exception("Could not locate the Reports folder.");
    }

    private void CreateDataSources()
    {
      FDataSet = new DataSet();
      FDataSet.ReadXml(FReportsFolder + "nwind.xml");

      FBusinessObject = new List<Category>();

      Category category = new Category("Beverages", "Soft drinks, coffees, teas, beers");
      category.Products.Add(new Product("Chai", 18m));
      category.Products.Add(new Product("Chang", 19m));
      category.Products.Add(new Product("Ipoh coffee", 46m));
      FBusinessObject.Add(category);

      category = new Category("Confections", "Desserts, candies, and sweet breads");
      category.Products.Add(new Product("Chocolade", 12.75m));
      category.Products.Add(new Product("Scottish Longbreads", 12.5m));
      category.Products.Add(new Product("Tarte au sucre", 49.3m));
      FBusinessObject.Add(category);

      category = new Category("Seafood", "Seaweed and fish");
      category.Products.Add(new Product("Boston Crab Meat", 18.4m));
      category.Products.Add(new Product("Red caviar", 15m));
      FBusinessObject.Add(category);
    }

    private void RegisterData()
    {
      FReport.RegisterData(FDataSet, "NorthWind");
      FReport.RegisterData(FBusinessObject, "Categories BusinessObject");
    }
    
    private void DesignReport()
    {
      if (FReport.IsRunning)
        return;

      FReport.Load((string)tvReports.SelectedNode.Tag);
      RegisterData();
      FReport.Design();
      PreviewReport();
    }


    private void PreviewReport()
    {
      if (FReport.IsRunning)
        return;

      RegisterData();
      FReport.Show();
    }

    private void tvReports_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (FReport.IsRunning)
        return;

      TreeNode selectedNode = tvReports.SelectedNode;
      if (selectedNode.Tag == null)
      {
        tvReports.CollapseAll();
        tvReports.SelectedNode = e.Node.Nodes[0];
      }
      else
      {
        FReport.Load((string)selectedNode.Tag);
        tbDescription.Text = FReport.ReportInfo.Description;
        PreviewReport();
      }
    }

    private void btnDesign_Click(object sender, EventArgs e)
    {
      DesignReport();
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start("http://www.fast-report.com");
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      UIStyle style = (UIStyle)comboBox1.SelectedIndex;
      Office2007ColorTable = UIStyleUtils.GetOffice2007ColorScheme(style);
      Config.UIStyle = style;
      previewControl1.UIStyle = style;
      Refresh();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      lblVersion.Text = "Version " + Config.Version;
      comboBox1.SelectedIndex = (int)Config.UIStyle;
      
      FindReportsFolder();
      CreateDataSources();

      FReport = new Report();
      FReport.Preview = previewControl1;
      Config.ReportSettings.ShowPerformance = true;
      Config.ReportSettings.StartProgress += new EventHandler(ReportSettings_StartProgress);
      Config.ReportSettings.Progress += new ProgressEventHandler(ReportSettings_Progress);
      Config.ReportSettings.FinishProgress += new EventHandler(ReportSettings_FinishProgress);

      tvReports.ImageList = Res.GetImages();

      XmlDocument reports = new XmlDocument();
      reports.Load(FReportsFolder + "reports.xml");

      for (int i = 0; i < reports.Root.Count; i++)
      {
        XmlItem folderItem = reports.Root[i];
        if (folderItem.GetProp("WinDemo") == "false")
          continue;

        string culture = System.Globalization.CultureInfo.CurrentCulture.Name;
        string text = folderItem.GetProp("Name-" + culture);
        if (String.IsNullOrEmpty(text))
          text = folderItem.GetProp("Name");

        TreeNode folderNode = tvReports.Nodes.Add(text + "     ");
        folderNode.ImageIndex = 66;
        folderNode.SelectedImageIndex = folderNode.ImageIndex;
        folderNode.NodeFont = new Font(Font, FontStyle.Bold);

        for (int j = 0; j < folderItem.Count; j++)
        {
          XmlItem reportItem = folderItem[j];
          if (reportItem.GetProp("WinDemo") == "false")
            continue;

          string file = reportItem.GetProp("File");
          string fileName = reportItem.GetProp("Name-" + culture);
          if (String.IsNullOrEmpty(fileName))
            fileName = Path.GetFileNameWithoutExtension(file);

          TreeNode fileNode = folderNode.Nodes.Add(fileName);
          fileNode.ImageIndex = 134;
          fileNode.SelectedImageIndex = fileNode.ImageIndex;
          fileNode.Tag = FReportsFolder + file;
        }
      }

      if (tvReports.Nodes.Count > 0 && tvReports.Nodes[0].Nodes.Count > 0)
        tvReports.SelectedNode = tvReports.Nodes[0].Nodes[0];
      tvReports.Focus();
    }

    private void ReportSettings_StartProgress(object sender, EventArgs e)
    {
    }

    private void ReportSettings_Progress(object sender, ProgressEventArgs e)
    {
      previewControl1.ShowStatus(e.Message);
    }

    private void ReportSettings_FinishProgress(object sender, EventArgs e)
    {
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
      FReport.Dispose();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      Color color = SystemColors.ControlDark;
      
      switch (Config.UIStyle)
      {
        case UIStyle.Office2003:
        case UIStyle.Office2007Blue:
          color = Color.FromArgb(150, 180, 224);
          break;
        
        case UIStyle.Office2007Silver:
          color = Color.FromArgb(156, 160, 167);
          break;
          
        case UIStyle.Office2007Black:
        case UIStyle.VistaGlass:
          color = Color.FromArgb(84, 84, 84);
          break;
      }
      
      using (Brush b = new SolidBrush(color))
      {
        g.FillRectangle(b, ClientRectangle);
      }
      using (Brush b = new SolidBrush(Color.FromArgb(128, Color.White)))
      {
        g.FillEllipse(b, new Rectangle(-300, -50, ClientRectangle.Width + 600, 220));
      }
    }


    public Form1()
    {
      SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
      InitializeComponent();
    }


    public class Category
    {
      private string FName;
      private string FDescription;
      private List<Product> FProducts;

      public string Name
      {
        get { return FName; }
      }

      public string Description
      {
        get { return FDescription; }
      }

      public List<Product> Products
      {
        get { return FProducts; }
      }

      public Category(string name, string description)
      {
        FName = name;
        FDescription = description;
        FProducts = new List<Product>();
      }
    }

    public class Product
    {
      private string FName;
      private decimal FUnitPrice;

      public string Name
      {
        get { return FName; }
      }

      public decimal UnitPrice
      {
        get { return FUnitPrice; }
      }

      public Product(string name, decimal unitPrice)
      {
        FName = name;
        FUnitPrice = unitPrice;
      }
    }
  }
}