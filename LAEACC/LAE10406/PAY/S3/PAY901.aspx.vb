Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing


Public Class PAY901
    Inherits System.Web.UI.Page

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString

#Region "@資料庫變數@"
    Dim strCSQL As String '查詢數量
    Dim strSSQL As String '查詢資料
#End Region
#Region "@固定變數@"
    Dim strPage As String = ""    '表單編號
    Dim I As Integer              '累進變數
    Dim strMessage As String = "" '訊息字串
    Dim strIRow, strIValue, strUValue, strWValue As String '資料處理參數(新增欄位；新增資料；異動資料；條件)
#End Region

#Region "@Page及功能操作@"
    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

        End If
    End Sub

    '查詢
    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click        
        '防呆
        If txtDate2.Text = "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('『支票日期』，未選擇!!');", True) : Exit Sub

        Dim strQuery As String = " AND a.DATE_1 = '" & Master.Models.strDateChinessToAD(txtDate2.Text) & " 00:00:00'"

        FillData(strQuery) '資料查詢
    End Sub

    '匯出
    Protected Sub btnExcel_Click(sender As Object, e As EventArgs) Handles btnExcel.Click
        '防呆
        If txtDate2.Text = "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('『支票日期』，未選擇!!');", True) : Exit Sub

        Dim strQuery As String = " AND a.DATE_1 = '" & Master.Models.strDateChinessToAD(txtDate2.Text) & " 00:00:00'"

        ExportData(strQuery) '資料查詢
    End Sub
#End Region

#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(Optional ByVal strQuery As String = "")
        Dim txtCount As Label = New Label

        strSSQL = "SELECT  '' AS 領款日期, b.REMARK AS 領款名稱, b.ACT_AMT AS 金額, a.CHKNO AS 支票號碼,"
        strSSQL &= " (SELECT SUM(ACT_AMT) FROM ACF010 t1 WHERE b.CHKNO = t1.CHKNO)AS 支票金額, '' AS 領款人"
        strSSQL &= " FROM CHF010 a"
        strSSQL &= " LEFT JOIN ACF010 b ON a.CHKNO = b.CHKNO"
        strSSQL &= " WHERE 1=1 AND LEN(a.CHKNO) > 5"
        strSSQL &= strQuery
        strSSQL &= " ORDER BY a.END_NO ASC"

        Master.Controller.objDataGrid(DataGridView, txtCount, DNS_ACC, strSSQL, "查詢檔")
    End Sub

    '匯出資料
    Public Sub ExportData(Optional ByVal strQuery As String = "")
        Dim MyDataSet As DataSet


        strSSQL = "SELECT  '' AS 領款日期, b.REMARK AS 領款名稱, b.ACT_AMT AS 金額, a.CHKNO AS 支票號碼,"
        strSSQL &= " (SELECT SUM(ACT_AMT) FROM ACF010 t1 WHERE b.CHKNO = t1.CHKNO)AS 支票金額, '' AS 領款人"
        strSSQL &= " FROM CHF010 a"
        strSSQL &= " LEFT JOIN ACF010 b ON a.CHKNO = b.CHKNO"
        strSSQL &= " WHERE 1=1 AND LEN(a.CHKNO) > 5"
        strSSQL &= strQuery
        strSSQL &= " ORDER BY a.END_NO ASC"

        MyDataSet = Master.ADO.openmember(DNS_ACC, "查詢檔", strSSQL)

        Master.ExportDataTableToExcel(MyDataSet.Tables("查詢檔"))
    End Sub
#End Region
End Class