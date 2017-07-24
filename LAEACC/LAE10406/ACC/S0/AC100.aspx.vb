Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class AC100
    Inherits System.Web.UI.Page

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString

#Region "@資料庫變數@"
    Dim strCSQL As String '查詢數量
    Dim strSSQL As String '查詢資料

    '程序專用*****
    Dim objCon99 As SqlConnection
    Dim objCmd99 As SqlCommand
    Dim objDR99 As SqlDataReader
    Dim strSQL99, retstr As String
#End Region
#Region "@固定變數@"
    Dim strPage As String = ""    '表單編號
    Dim I As Integer              '累進變數
    Dim strMessage As String = "" '訊息字串
    Dim strIRow, strIValue, strUValue, strWValue As String '資料處理參數(新增欄位；新增資料；異動資料；條件)

#End Region

#Region "@程式變數@"

    Dim sqlstr As String
    Dim TempDataSet As DataSet
    Dim mydataset, mydataset2, myDatasetS, myDatasetT As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱

        nudYear.Text = Session("sYear")
        sqlstr = "select * from acnop"
        Dim myds As DataSet = Master.ADO.openmember(DNS_ACC, "acnop", sqlstr)
        With myds.Tables("acnop")
            For intI As Integer = 0 To .Rows.Count - 1
                If .Rows(intI).Item("kind") = "1" Then txtSno1.Text = Master.ADO.nz(.Rows(intI).Item("endno"), 0) + 1
                If .Rows(intI).Item("kind") = "2" Then txtSno2.Text = Master.ADO.nz(.Rows(intI).Item("endno"), 0) + 1
                If .Rows(intI).Item("kind") = "3" Then txtSno3.Text = Master.ADO.nz(.Rows(intI).Item("endno"), 0) + 1
            Next
        End With

        sqlstr = "select * from acfno where accyear=" & nudYear.Text & " and kind<='3'"
        myds = Master.ADO.openmember(DNS_ACC, "acnop", sqlstr)
        With myds.Tables("acnop")
            For intI As Integer = 0 To .Rows.Count - 1
                If .Rows(intI).Item("kind") = "1" Then txtEno1.Text = Master.ADO.nz(.Rows(intI).Item("cont_no"), 0)
                If .Rows(intI).Item("kind") = "2" Then txtEno2.Text = Master.ADO.nz(.Rows(intI).Item("cont_no"), 0)
                If .Rows(intI).Item("kind") = "3" Then txtEno3.Text = Master.ADO.nz(.Rows(intI).Item("cont_no"), 0)
            Next
        End With

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then

        End If
    End Sub

    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
#End Region

    Protected Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號

        Param.Add("nudYear", nudYear.Text)    '使用者代號
        Param.Add("txtSno1", txtSno1.Text)    '使用者代號
        Param.Add("txtEno1", txtEno1.Text)    '使用者代號
        Param.Add("txtSno2", txtSno2.Text)    '使用者代號
        Param.Add("txtEno2", txtEno2.Text)    '使用者代號
        Param.Add("txtSno3", txtSno3.Text)    '使用者代號
        Param.Add("txtEno3", txtEno3.Text)    '使用者代號




        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期


        Master.PrintFR("AC100傳票交付簿", Session("ORG"), DNS_ACC, Param)
    End Sub

    Protected Sub nudYear_TextChanged(sender As Object, e As EventArgs) Handles nudYear.TextChanged
        sqlstr = "select * from acfno where accyear=" & nudYear.Text & " and kind<='3'"
        Dim myds As DataSet = Master.ADO.openmember(DNS_ACC, "acnop", sqlstr)
        With myds.Tables("acnop")
            For intI As Integer = 0 To .Rows.Count - 1
                If .Rows(intI).Item("kind") = "1" Then txtEno1.Text = Master.ADO.nz(.Rows(intI).Item("cont_no"), 0)
                If .Rows(intI).Item("kind") = "2" Then txtEno2.Text = Master.ADO.nz(.Rows(intI).Item("cont_no"), 0)
                If .Rows(intI).Item("kind") = "3" Then txtEno3.Text = Master.ADO.nz(.Rows(intI).Item("cont_no"), 0)
            Next
        End With
    End Sub
End Class