Imports System.Data
Imports System.Data.SqlClient

Public Class PAY091
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
    Dim mydataset As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤




        lblDate2.Text = Session("UserDate")




    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            '其他預設值*****

        End If
    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub

#End Region



    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        'sqlstr = "SELECT * FROM  CHF040 where rdate >='" & Master.Models.FullDate(dtpDateS.Text) & "' order by rdate desc"


        lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
        Master.Controller.objDataGrid(DataGridView, lbl_GrdCount, DNS_ACC, sqlstr, "查詢資料檔")


    End Sub

    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        Dim sqlstr, retstr, qstr, strD, strC, skind As String
        Dim intI, intJ, SumAmt, err As Integer

        sqlstr = "SELECT b.*, c.BANKNAME AS bankname, c.ACCNO AS accno FROM " & _
                 "(SELECT MAX(date_2) AS date_2, bank FROM CHF030 WHERE DATE_2<='" & Master.Models.FullDate(lblDate2.Text) & "' GROUP BY BANK) a " & _
                 "INNER JOIN CHF030 b ON a.bank = b.BANK AND a.date_2 = b.DATE_2 " & _
                 "left outer join CHF020 c ON a.bank = c.BANK" & _
                 " WHERE ISNULL(c.accno, '') <> ''" & _
                 " ORDER BY b.bank, c.ACCNO"
        mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        If mydataset.Tables("chf020").Rows.Count = 0 Then
            MessageBx("沒有結存資料")
            btnSure.Visible = False
            Exit Sub
        Else
            DataGridView.DataSource = mydataset
            DataGridView.DataBind()
        End If

        txtPageNo.Text = Val(txtPageNo.Text) - 1  '頁次在列印前先減1, because each page will add 1 

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號
        Param.Add("sdate", Master.Models.FullDate(lblDate2.Text))
        Param.Add("txtPageNo", txtPageNo.Text)
        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期
        Master.PrintFR("PAY091重印日計表", Session("ORG"), DNS_ACC, Param)
    End Sub
End Class