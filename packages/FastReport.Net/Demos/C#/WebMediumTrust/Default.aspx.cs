using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using FastReport;
using System.Collections.Generic;
using FastReport.Data;
using FastReport.Utils;

public class Category
{
    public string Name;
    public string Description;
    public List<Product> Products;

    public Category(string name, string description)
    {
        Name = name;
        Description = description;
        Products = new List<Product>();
    }
}

public class Product
{
    public string Name;
    public decimal UnitPrice;

    public Product(string name, decimal unitPrice)
    {
        Name = name;
        UnitPrice = unitPrice;
    }
}

public partial class _Default : System.Web.UI.Page 
{
    public void RegisterData(Report FReport)
    {
        DataSet FDataSet = new DataSet();
        FDataSet.ReadXml(Request.PhysicalApplicationPath + "App_Data\\nwind.xml");

        FReport.RegisterData(FDataSet, "NorthWind");

        List<Category> list = new List<Category>();
        Category category = new Category("Beverages", "Soft drinks, coffees, teas, beers");
        category.Products.Add(new Product("Chai", 18m));
        category.Products.Add(new Product("Chang", 19m));
        category.Products.Add(new Product("Ipoh coffee", 46m));
        list.Add(category);

        category = new Category("Confections", "Desserts, candies, and sweet breads");
        category.Products.Add(new Product("Chocolade", 12.75m));
        category.Products.Add(new Product("Scottish Longbreads", 12.5m));
        category.Products.Add(new Product("Tarte au sucre", 49.3m));
        list.Add(category);

        category = new Category("Seafood", "Seaweed and fish");
        category.Products.Add(new Product("Boston Crab Meat", 18.4m));
        category.Products.Add(new Product("Red caviar", 15m));
        list.Add(category);

        FReport.RegisterData(list, "Categories BusinessObject", BOConverterFlags.AllowFields, 3);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Version.Text = "ver." + Config.Version;
        if (LeftMenu.SelectedItem == null)
        {
            if (LeftMenu.Items.Count > 0)
            {
                MenuItem item = LeftMenu.Items[0].ChildItems[0];
                item.Selected = true;
            }
        }

    }

    protected void LeftMenu_MenuItemClick(object sender, MenuEventArgs e)
    {
        WebReport1.Refresh();
    }

    protected void WebReport1_StartReport(object sender, EventArgs e)
    {
        switch (LeftMenu.SelectedValue)
        {
            case "1":
                WebReport1.Report = new FastReport.SimpleList();
                break;
            case "2":
                WebReport1.Report = new FastReport.Groups();
                break;
            case "3":
                WebReport1.Report = new FastReport.MasterDetail();
                break;
            case "4":
                WebReport1.Report = new FastReport.Labels();
                break;
            case "5":
                WebReport1.Report = new FastReport.Subreport();
                break;
            case "6":
                WebReport1.Report = new FastReport.ComplexMasterdetailGroup();
                break;
            case "7":
                WebReport1.Report = new FastReport.HierarchicList();
                break;
            case "8":
                WebReport1.Report = new FastReport.Barcodes();
                break;
            case "9":
                WebReport1.Report = new FastReport.MatrixReport();
                break;
            default:
                WebReport1.Report = new FastReport.SimpleList();
                break;
        }
        Label2.Style.Add("padding", "5px");
        Label2.Text = WebReport1.Report.ReportInfo.Description.Replace("\r\n", "<br/>");
        RegisterData(WebReport1.Report);
    }

    protected void LeftMenu_Init(object sender, EventArgs e)
    {
        LeftMenu.Items.Clear();
        MenuItem FolderNode = new MenuItem("Reports");
        LeftMenu.Items.Add(FolderNode);
        FolderNode.ChildItems.Add(new MenuItem("Simple list", "1"));
        FolderNode.ChildItems.Add(new MenuItem("Groups", "2"));
        FolderNode.ChildItems.Add(new MenuItem("Master-Detail", "3"));
        FolderNode.ChildItems.Add(new MenuItem("Labels", "4"));
        FolderNode.ChildItems.Add(new MenuItem("Subreport", "5"));
        FolderNode.ChildItems.Add(new MenuItem("Complex (Master-detail + Group)", "6"));
        FolderNode.ChildItems.Add(new MenuItem("Hierarchic List", "7"));
        FolderNode.ChildItems.Add(new MenuItem("Barcodes", "8"));
        FolderNode.ChildItems.Add(new MenuItem("Matrix", "9"));
    }
}

