Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing


Public Class PAY080
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
    Dim strSQL99 As String
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
    Dim mydataset, mydataset2 As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        Dim myds As DataSet
        sqlstr = "SELECT date_2 from chf020 where date_2 is not null"
        myds = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        If myds.Tables("chf020").Rows.Count > 0 Then
            dtpDateS.Text = Master.Models.strDateADToChiness(Trim(myds.Tables("chf020").Rows(0).Item("date_2").ToShortDateString.ToString))
            dtpDateE.Text = Master.Models.strDateADToChiness(Trim(myds.Tables("chf020").Rows(0).Item("date_2").ToShortDateString.ToString))
        Else
            dtpDateS.Text = Session("userdate")
            dtpDateE.Text = Session("userdate")
        End If
        txtNo.Text = "1"
        txtSbank.Text = "01"
        txtEbank.Text = "99"

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
        Param.Add("UserId", ViewState("UserId"))    '使用者代號

        Param.Add("dtpDateS", Master.Models.FullDate(dtpDateS.Text))    '起始會計科目
        Param.Add("dtpDateE", Master.Models.FullDate(dtpDateE.Text))    '起始會計科目
        Param.Add("txtNo", txtNo.Text)    '結束會計科目
        Param.Add("txtSbank", txtSbank.Text)    '結束會計科目
        Param.Add("txtEbank", txtEbank.Text)    '結束會計科目
        Param.Add("dtpDate", Mid(Master.Models.FullDate(dtpDateS.Text), 1, 4) + "-01-01")
        Param.Add("begMonth", Mid(Master.Models.FullDate(dtpDateS.Text), 1, 7) + "-01")



        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期


        Master.PrintFR("PAY080存款明細分戶帳", Session("ORG"), DNS_ACC, Param)

    End Sub
End Class