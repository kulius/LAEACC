Imports FastReport.Export.Pdf
Imports System
Imports System.IO
Imports System.IO.File
Public Class FRPrint1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Protected Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
        ' No temp files
        FastReport.Utils.Config.WebMode = True

        ' Set PDF export props
        Dim pdfExport As New FastReport.Export.Pdf.PDFExport()
        pdfExport.ShowProgress = False
        pdfExport.Subject = Session("ReportTitle")
        pdfExport.Title = Session("ReportTitle")
        pdfExport.Compressed = True
        pdfExport.AllowPrint = True
        pdfExport.EmbeddingFonts = True

        ' Load our report
        Dim report As New FastReport.Report

        Dim evn As New FastReport.EnvironmentSettings()

        report.Load(Session("ReportPath"))
        'Dim ConnStr As String = "Data Source=KULIUSNB\SQL2012;AttachDbFilename=;Initial Catalog=accdb_db;Integrated Security=False;Persist Security Info=True;User ID=acc;Password=acc"
        report.SetParameterValue("ConnStr", Session("ReportConnStr"))

        Dim ReportParam As Dictionary(Of String, String) = DirectCast(Session("ReportParam1"), Dictionary(Of String, String))

        For Each Param In ReportParam
            report.SetParameterValue(Param.Key, Param.Value)
        Next

        report.Prepare()

        Dim strm As New MemoryStream()

        report.Export(pdfExport, strm)
        report.Dispose()
        pdfExport.Dispose()

        ' Stream the PDF back to the client as an attachment
        Response.ClearContent()
        Response.ClearHeaders()
        Response.Buffer = True
        'Response.ContentType = "Application/PDF"
        '下載的寫法
        ' Response.AddHeader("Content-Disposition", "attachment;filename=some_filename.pdf")
        '直接開啓的寫法
        Response.AddHeader("Content-Disposition", "inline; filename=some_filename.pdf")
        Response.ContentType = "Application/PDF"

        strm.Position = 0
        strm.WriteTo(Response.OutputStream)
        strm.Dispose()

        Response.[End]()

    End Sub
End Class