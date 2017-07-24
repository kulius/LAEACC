Imports System.Data
Imports System.Data.SqlClient

Public Class BGQ020
    Inherits System.Web.UI.Page

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString
    Dim DNS_AUTH As String = ConfigurationManager.ConnectionStrings("DNS_AUTH").ConnectionString

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
    Dim mydatasetT As DataSet

#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGrid1, "M")


        '丟當年度所有預算科目to Grid1 
        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT a.user_name, a.unit_id,a.sal, b.unit_name from " & _
         "(select user_name, unit_id,basic_sal+year_sal as sal from auth_user where quit_kind=0 and basic_sal+year_sal between 110 and 0 ) a " & _
         "left outer join auth_unit b on a.unit_id = b.unit_id " & _
         " order by a.user_name"
        mydataset = Master.ADO.openmember(DNS_AUTH, "user", sqlstr)
        DataGrid1.DataSource = mydataset.Tables("user")
        DataGrid1.DataBind()


        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤



    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGrid1, "M")
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


    
End Class