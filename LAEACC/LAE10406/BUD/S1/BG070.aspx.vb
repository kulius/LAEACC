Imports System.Data
Imports System.Data.SqlClient

Public Class BG070
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

        Dim myds As DataSet
        '在會計科目='5'的主計審核者欄標示  Y OR N
        sqlstr = "SELECT * FROM ACCNAME WHERE ACCNO='5'"
        mydataset = Master.ADO.openmember(DNS_ACC, "ACCNAME", sqlstr)
        If mydataset.Tables(0).Rows.Count <= 0 Then
            MessageBx("貴會會計科目檔少了科目=5的科目,請主計人員先行補上 ")

        End If
        If Mid(Master.ADO.nz(mydataset.Tables(0).Rows(0).Item("account_no"), ""), 1, 1) <> "Y" Then
            rdoYes.Checked = False
            rdoNo.Checked = True
            lblmsg.Text = "目前為不可異動"
        Else
            rdoYes.Checked = True
            rdoNo.Checked = False
            lblmsg.Text = "目前為可異動(充許業務單位修改)"
        End If


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



    Protected Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        Dim sqlstr, retstr As String
        Dim strYes As String

        '在會計科目='5'的主計審核者欄標示  Y OR N
        If rdoYes.Checked Then
            strYes = "Y"
        Else
            strYes = "N"
        End If
        sqlstr = "UPDATE ACCNAME SET ACCOUNT_NO='" & strYes & "' WHERE ACCNO='5'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            If strYes = "Y" Then
                MessageBx("已控制可異動")
                lblmsg.Text = "目前為可異動(充許業務單位修改)"
            Else
                MessageBx("已控制不可異動")
                lblmsg.Text = "目前為不可異動"
            End If
        End If
    End Sub
End Class