Imports System.Data
Imports System.Data.SqlClient

Public Class AC040
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
        Master.Controller.objDataGridStyle(dtgSource, "M")

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤


        lblYear.Text = Session("sYear")
        dtpDate.Text = Session("UserDate")
        lblUseNO.Text = Master.Controller.QueryNO(CInt(Session("sYear")), "6")
        Call LoadGridFunc()
    End Sub

    Sub LoadGridFunc()
        '將acf010->no_2_no=0置入source datagrid 
        Dim sqlstr, qstr, sortstr As String
        sqlstr = "SELECT * from acf010 where kind='3' and no_2_no=0 and seq='1' and item='1' and accyear= " & Session("sYear")
        Dim myDatasetS As DataSet = Master.ADO.openmember(DNS_ACC, "ac010s", sqlstr)
        dtgSource.DataSource = myDatasetS.Tables("ac010s")
        dtgSource.DataBind()
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(dtgSource, "M")
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
    '點選資訊
    Sub dtgSource_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles dtgSource.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號


        Select Case e.CommandName
            Case "btnShow"
                '查詢資料及控制*****
                lblNo_1_no.Text = ""
                FindData(txtID.Text)               '查詢主檔
                'FlagMoveSeat(0, e.Item.ItemIndex)
                TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤



        End Select
    End Sub
#End Region

#Region "@資料查詢@"
    '查詢資料
    Sub FindData(ByVal strKey1 As String)
        '防呆*****
        If strKey1 = "" Then Exit Sub

        '設定關鍵值*****        
        txtKey1.Text = strKey1 : ViewState("FileKey") = strKey1

        '資料查詢*****
        Data_Load(strKey1) '載入資料
    End Sub
    '請購推算檔
    Sub Data_Load(ByVal strKey1 As String)

        Dim SumAmt As Decimal = 0
        Dim intI As Integer
        Dim strI, sqlstr As String
        Dim tempdataset As DataSet
        Call clsScreen()
        sqlstr = "SELECT a.accyear, a.kind, a.no_1_no, a.seq, a.item, a.accno, a.amt, a.remark, a.date_1 " & _
                 "FROM ACF010 a WHERE a.kind = '3' AND a.seq='1' and a.dc='1' and a.accyear=" & Session("sYear") & " and a.no_1_no = " & strKey1 & _
                 " UNION " & _
                 "(SELECT b.accyear, b.kind, b.no_1_no, b.seq, b.item, b.accno, b.amt, b.remark, c.date_1 " & _
                 " FROM  acf020 b left outer join acf010 c on c.accyear=b.accyear and c.kind=b.kind and c.no_1_no=b.no_1_no " & _
                 "WHERE  b.kind='3' AND  b.seq='1' and b.dc='1' and b.accyear=" & Session("sYear") & " and b.NO_1_NO = " & strKey1 & _
                 ") ORDER BY kind, seq, item"
        tempdataset = Master.ADO.openmember(DNS_ACC, "acf010", sqlstr)   '只將借方傳票第一張置入螢幕
        For intI = 0 To tempdataset.Tables("acf010").Rows.Count - 1
            If intI = 0 Then lblNo_1_no.Text = tempdataset.Tables("acf010").Rows(intI).Item("no_1_no")
            If intI = 0 Then lbldate_1.Text = tempdataset.Tables("acf010").Rows(intI).Item("date_1")
            strI = CType(intI + 1, String)
            DirectCast(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = tempdataset.Tables("acf010").Rows(intI).Item("remark")
            DirectCast(TabPanel2.FindControl("txtAmt" & strI), TextBox).Text = FormatNumber(tempdataset.Tables("acf010").Rows(intI).Item("amt"), 2)
            DirectCast(TabPanel2.FindControl("vxtaccno" & strI), TextBox).Text = tempdataset.Tables("acf010").Rows(intI).Item("accno")

        Next
        tempdataset = Nothing
        lblNo_2_no.Text = Master.Controller.QueryNO(CInt(Session("sYear")), "6") + 1

    End Sub

    Sub clsScreen()    '清傳票螢幕
        Dim intI As Integer
        Dim strI As String
        For intI = 1 To 6
            strI = CType(intI, String)
            DirectCast(TabPanel2.FindControl("vxtaccno" & strI), TextBox).Text = ""
            If intI > 1 Then DirectCast(TabPanel2.FindControl("txtcode" & strI), TextBox).Text = ""
            DirectCast(TabPanel2.FindControl("txtremark" & strI), TextBox).Text = ""
            DirectCast(TabPanel2.FindControl("txtAmt" & strI), TextBox).Text = ""

        Next
    End Sub

#End Region


    Protected Sub btnback_Click(sender As Object, e As EventArgs) Handles btnback.Click
        TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        If lblNo_1_no.Text = "" Then Exit Sub
        If Year(dtpDate.Text) <> Session("sYear") Then
            MessageBx("決裁日期與傳票年度不同")
            Exit Sub
        End If
        Dim sqlstr, retstr, updsqlvalue As String
        Dim intNo As Integer
        intNo = Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "6")     '轉帳決裁編號控制kind="6"
        sqlstr = "UPDATE acf010 SET date_2='" & Master.Models.FullDate(dtpDate.Text) & "', no_2_no = " & intNo & " WHERE no_1_no = " & lblNo_1_no.Text & " And accyear = " & Session("sYear") & " And kind >= '3'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlerror" Then
            MessageBx("轉帳決裁有誤acf010 " & lblNo_1_no.Text)
            Exit Sub
        End If

        sqlstr = "UPDATE acf020 SET no_2_no = " & intNo & " WHERE no_1_no = " & lblNo_1_no.Text & " And accyear = " & Session("sYear") & " And kind >= '3'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlerror" Then
            MessageBx("轉帳決裁有誤acf020 " & lblNo_1_no.Text)

        End If


        '並清空傳票來源

        Call LoadGridFunc()

        Call clsScreen()
        lblUseNO.Text = Str(intNo)        '顯示實際使用編號
        TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤     '回datagrid PAGE 1 
    End Sub
End Class