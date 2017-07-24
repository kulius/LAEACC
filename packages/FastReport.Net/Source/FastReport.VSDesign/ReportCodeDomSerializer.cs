using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Forms;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using FastReport.Data;
using System.Data;
using FastReport.Utils;
using FastReport.Design.StandardDesigner;

namespace FastReport.VSDesign
{
  public class ReportCodeDomSerializer : CodeDomSerializer
  {
    public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
    {
      CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.
        GetSerializer(typeof(Report).BaseType, typeof(CodeDomSerializer));

      return baseClassSerializer.Deserialize(manager, codeObject);
    }

    public override object Serialize(IDesignerSerializationManager manager, object value)
    {
      CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.
        GetSerializer(typeof(Report).BaseType, typeof(CodeDomSerializer));

      object codeObject = baseClassSerializer.Serialize(manager, value);
      if (codeObject is CodeStatementCollection)
      {
        CodeStatementCollection statements = (CodeStatementCollection)codeObject;
        Report report = value as Report;
        List<string> addedItems = new List<string>();

        foreach (Dictionary.RegDataItem item in report.Dictionary.RegisteredItems)
        {
          string dsName = "";
          if (item.Data is DataTable)
            dsName = (item.Data as DataTable).DataSet.Site.Name;
          else if (item.Data is BindingSource)
            dsName = (item.Data as BindingSource).Site.Name;

          if (dsName != "" && !addedItems.Contains(dsName))
          {
            CodeExpression[] args = new CodeExpression[] {
              new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), dsName),
              new CodePrimitiveExpression(dsName) };
            
            CodeExpression invokeExpression = new CodeMethodInvokeExpression(
              new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), report.Site.Name),
              "RegisterData",
              args);
            statements.Add(new CodeExpressionStatement(invokeExpression));
            addedItems.Add(dsName);
          }
        }
      }

      return codeObject;
    }
  }
  
}
