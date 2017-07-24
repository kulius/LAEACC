Imports System.Data
Imports System.Data.SqlClient

Public Class BGP050
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

        ViewState("UserId") = Session("UserId")
        'DataGrid*****

        'Focus*****

        TabContainer1.Visible = False
        TabContainer1.Enabled = False
        nudYear.Text = Session("sYear")
        vxtStartNo.Text = "1"    '起值
        vxtEndNo.Text = "59"    '迄值
        cboUser.Visible = False
        If Mid(Session("UserUnit"), 1, 2) = "05" Or Mid(Session("UserUnit"), 1, 2) = "08" Then
            cboUser.Visible = True
            sqlstr = "SELECT b.staff_no as userid, b.staff_no+USERTABLE.USERNAME as username FROM USERTABLE right outer JOIN" & _
                    " (SELECT STAFF_NO FROM ACCNAME WHERE STAFF_NO IS NOT NULL AND STAFF_NO <> '    ' GROUP BY STAFF_NO) b " & _
                    " ON USERTABLE.USERID = b.STAFF_NO" & _
                    " WHERE USERTABLE.USERNAME IS NOT NULL" & _
                    " order by usertable.userid"

            Master.Controller.objDropDownListOptionEX(cboUser, DNS_ACC, sqlstr, "userid", "username", 0)

        End If

        Dim trigger As New PostBackTrigger()
        trigger.ControlID = BtnPrint.ID.ToString()
        UpdatePanel1.Triggers.Add(trigger)

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
#Region "@DataGridView@"


#End Region
#Region "按鍵選項"

#End Region



#Region "@共用底層副程式@"

#End Region

#Region "@資料查詢@"


#End Region

#Region "物件選擇異動值"

#End Region






    Protected Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click
        If Mid(Session("UserUnit"), 1, 2) = "05" Or Mid(Session("UserUnit"), 1, 2) = "08" Then
            ViewState("UserId") = cboUser.SelectedValue
        End If

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", ViewState("UserId"))    '使用者代號
        Param.Add("nudYear", nudYear.Text)  '報表年度
        Param.Add("vxtStartNo", vxtStartNo.Text)    '起始會計科目
        Param.Add("vxtEndNo", vxtEndNo.Text)    '結束會計科目

        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        Master.PrintFR("BGP050推算簿列印(分發包及變更)", Session("ORG"), DNS_ACC, Param)
    End Sub
End Class