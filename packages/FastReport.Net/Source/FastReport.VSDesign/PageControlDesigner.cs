using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;
using FastReport.Controls;

namespace FastReport.VSDesign
{
  internal class PageControlDesigner : ParentControlDesigner
  {
    private PageControl FPageControl;

    public override void Initialize(IComponent component)
    {
      base.Initialize(component);
      FPageControl = base.Control as PageControl;
    }

    private void AddPage(object sender, EventArgs e)
    {
      IDesignerHost host = ((IDesignerHost)base.GetService(typeof(IDesignerHost)));
      DesignerTransaction transaction = host.CreateTransaction("Add Page");
      PageControlPage page = host.CreateComponent(typeof(PageControlPage)) as PageControlPage;
      FPageControl.Controls.Add(page);
      page.Dock = DockStyle.Fill;
      page.BackColor = SystemColors.Window;
      page.Text = "Page" + FPageControl.Controls.Count.ToString(); 
      FPageControl.ActivePage = page;
      transaction.Commit();
    }

    private void SelectNextPage(object sender, EventArgs e)
    {
      IDesignerHost host = ((IDesignerHost)base.GetService(typeof(IDesignerHost)));
      DesignerTransaction transaction = host.CreateTransaction("Select Next Page");
      FPageControl.SelectNextPage();
      transaction.Commit();
    }

    private void SelectPrevPage(object sender, EventArgs e)
    {
      IDesignerHost host = ((IDesignerHost)base.GetService(typeof(IDesignerHost)));
      DesignerTransaction transaction = host.CreateTransaction("Select Prev Page");
      FPageControl.SelectPrevPage();
      transaction.Commit();
    }

    public override DesignerVerbCollection Verbs
    {
      get 
      { 
        return new DesignerVerbCollection(new DesignerVerb[] { 
          new DesignerVerb("Add Page", new EventHandler(AddPage)),
          new DesignerVerb("Select Next Page", new EventHandler(SelectNextPage)),
          new DesignerVerb("Select Prev Page", new EventHandler(SelectPrevPage)) }); 
      }
    }

    protected override bool GetHitTest(Point point)
    {
      if (FPageControl.SelectorWidth > 0)
      {
        point = FPageControl.PointToClient(point);
        if (FPageControl.GetTabAt(point) != -1)
          return true;
      }
      
      FPageControl.HighlightPageIndex = -1;
      return false;
    }

    protected override bool DrawGrid
    {
      get { return false; }
    }
  }
}