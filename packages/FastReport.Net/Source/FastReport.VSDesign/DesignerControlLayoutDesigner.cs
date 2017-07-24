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
using FastReport.Design;

namespace FastReport.VSDesign
{
  internal class DesignerControlLayoutDesigner : ParentControlDesigner
  {
    private Designer FDesigner;
    private string FOldLayoutState;
    
    public override void Initialize(IComponent component)
    {
      base.Initialize(component);
      FDesigner = component as Designer;
      FOldLayoutState = FDesigner.LayoutState;
      FDesigner.LayoutChangedEvent += new EventHandler(FDesigner_LayoutChangedEvent);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      FDesigner.LayoutChangedEvent -= new EventHandler(FDesigner_LayoutChangedEvent);
    }

    private void FDesigner_LayoutChangedEvent(object sender, EventArgs e)
    {
      string layout = FDesigner.LayoutState;
      if (layout != FOldLayoutState)
      {
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(FDesigner);
        PropertyDescriptor prop = props.Find("LayoutState", false);
        RaiseComponentChanging(prop);
        RaiseComponentChanged(prop, FOldLayoutState, layout);
        FOldLayoutState = layout;
      }
    }

    protected override bool GetHitTest(Point point)
    {
      return true;
    }

    protected override bool DrawGrid
    {
      get { return false; }
    }
  }
}