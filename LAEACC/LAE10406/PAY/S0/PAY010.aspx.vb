Imports System.Data
Imports System.Data.SqlClient

Public Class PAY010
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


    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            Call LoadGridFunc()
        End If
    End Sub
    Sub LoadGridFunc()
        Dim sqlstr, qstr, strD, strC As String
        Dim intI As Integer
        sqlstr = "SELECT * FROM  CHF020 where day_income<>0 or day_pay<>0 or date_2 is not null order by bank"
        mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        If myDataSet.Tables("chf020").Rows.Count = 0 Then
            lblMsg.Text = "已開帳完成"
            MessageBx("已開帳完成")
            btnSure.Visible = False
            Exit Sub
        Else
            lblDate2.Text = mydataset.Tables("chf020").Rows(0).Item("date_2").toshortdatestring
            DataGridView.DataSource = mydataset
            DataGridView.DataBind()
        End If
        For intI = 0 To myDataSet.Tables("chf020").Rows.Count - 1
            If myDataSet.Tables("chf020").Rows(intI).Item("prt_code") <> "Y" Then
                btnSure.Visible = False
                lblMsg.Text = "尚未列印結存日計表"
                MessageBx("尚未列印結存日計表")
                Exit Sub
            End If
        Next
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



    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        Dim retstr As String
        Dim intI As Integer
        Dim SBank, SDate As String
        Dim IncomeAmt, PayAmt, DayBalance As Double
        Dim myDataSet, myDataSet2 As DataSet

        sqlstr = "SELECT * FROM  CHF020 where day_income<>0 or day_pay<>0 or date_2 is not null order by bank"
        myDataSet = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)

        For intI = 0 To mydataset.Tables("chf020").Rows.Count - 1
            With mydataset.Tables("chf020").Rows(intI)
                Sbank = .Item("bank")
                IncomeAmt = .Item("day_income")
                PayAmt = .Item("day_pay")
                SDate = .Item("date_2").toshortdatestring
                DayBalance = .Item("balance") + IncomeAmt - PayAmt
            End With


            sqlstr = "SELECT * from chf030 where bank='" & SBank & "' and date_2='" & Master.Models.FullDate(SDate) & "'"
            myDataSet2 = Master.ADO.openmember(DNS_ACC, "chf030", sqlstr)
            If myDataSet2.Tables("chf030").Rows.Count = 0 Then   '原無該日資料則新增一筆
                Master.ADO.GenInsSql("bank", SBank, "T")
                Master.ADO.GenInsSql("date_2", SDate, "H")
                Master.ADO.GenInsSql("day_income", IncomeAmt, "N")
                Master.ADO.GenInsSql("day_PAY", PayAmt, "N")
                Master.ADO.GenInsSql("balance", DayBalance, "N")
                sqlstr = "insert into chf030 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                If retstr <> "sqlok" Then MessageBx("寫入歷史檔 error " & sqlstr)
            Else

                Master.ADO.GenUpdsql("day_income", myDataSet2.Tables("chf030").Rows(0).Item("day_income") + IncomeAmt, "N")
                Master.ADO.GenUpdsql("day_pay", myDataSet2.Tables("chf030").Rows(0).Item("day_pay") + PayAmt, "N")
                Master.ADO.GenUpdsql("balance", myDataSet2.Tables("chf030").Rows(0).Item("balance") + IncomeAmt - PayAmt, "N")
                sqlstr = "update chf030 set " & Master.ADO.genupdfunc & " where bank='" & SBank & "' and date_2='" & Master.Models.FullDate(SDate) & "'"
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                If retstr <> "sqlok" Then MessageBx("update chf030 error " & sqlstr)
            End If
        Next
        mydataset = Nothing
        myDataSet2 = Nothing
        'update chf020 
        sqlstr = "update chf020 set balance=balance+day_income-day_pay, day_income=0, day_pay=0 ,date_2 = null where day_pay<>0 or day_income<>0 or date_2 is not null"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("update chf020 error " & sqlstr)
        lblMsg.Text = "作業完成"
        btnSure.Visible = False
    End Sub

    Sub upd_chf030()
        
    End Sub
End Class