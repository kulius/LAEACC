Imports System.Data
Imports System.Data.SqlClient

Public Class PAY024
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

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤
        txtBank.Focus()

        Call GiveUp()

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtBank.Attributes.Add("maxlength", "2")
        txtBank.Attributes.Add("onkeyup", "return ismaxlength(this)")
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            btnFinish.Attributes.Add("onclick", "this.disabled = true;this.value = '請稍候..';" + Page.ClientScript.GetPostBackEventReference(btnFinish, ""))
        End If
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****

    End Sub
    Sub LoadGridFunc()

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

    Protected Sub btnChkno_Click(sender As Object, e As EventArgs) Handles btnChkno.Click
        Dim sYear As Integer = Session("sYear")
        lblMsg.Text = ""
        sqlstr = "select * from chf010 where bank='" & txtBank.Text & "' and chkno='" & txtChkNo.Text & _
                 "' and accyear=" & sYear
        mydataset = Master.ADO.openmember(DNS_ACC, "chf010", sqlstr)
        If mydataset.Tables("chf010").Rows.Count = 0 Then
            MessageBx("無此支票")
            Exit Sub
        End If
        If mydataset.Tables("chf010").Rows(0).Item("start_no") <> 0 Then
            MessageBx("此支票已入帳不可作廢")
            Exit Sub
        End If
        lblMsg.Text = ""
        txtChkNo.Enabled = False
        txtBank.Enabled = False
        btnChkno.Enabled = False
        lblRemark.Text = mydataset.Tables("chf010").Rows(0).Item("remark")
        lblamt.Text = FormatNumber(mydataset.Tables("chf010").Rows(0).Item("amt"), 0)
        lblChkname.Text = mydataset.Tables("chf010").Rows(0).Item("chkname")
        lblNo1.Text = mydataset.Tables("chf010").Rows(0).Item("no_1_no")

        btnFinish.Visible = True
        btnGiveUp.Visible = True
        btnFinish.Focus()
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        Dim sYear As Integer = Session("sYear")
        Dim mastconn As String = DNS_ACC
        Dim retstr As String
        txtChkNo.Text = txtChkNo.Text.ToUpper

        '清除acf010支票號
        sqlstr = "update acf010 set chkno='' where accyear=" & sYear & _
                 " and chkno='" & txtChkNo.Text & "' and kind='2' and item='9'"
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("acf010->chkno支票號清除失敗,請檢查" & sqlstr)
            Exit Sub
        End If

        '刪除支票chf010 
        sqlstr = "delete chf010  where bank='" & txtBank.Text & _
                 "' and chkno='" & txtChkNo.Text & "' and accyear=" & sYear
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("刪除支票 chf010失敗,請檢查" & sqlstr)
            Exit Sub
        End If

        '將支票金額由已開未領欄扣除(chf020->unpay) 
        sqlstr = "update chf020 set unpay = unpay - " & Master.Models.ValComa(lblamt.Text) & " where bank='" & txtBank.Text & "'"
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("將支票金額由已開未領欄扣除(chf020->unpay)失敗,請檢查" & sqlstr)
            Exit Sub
        End If

        '電子轉帳 ADD 
        If Mid(txtChkNo.Text, 1, 2) = "TR" Then
            sqlstr = "delete chf050  where bank='" & txtBank.Text & _
                 "' and vchkno='" & txtChkNo.Text & "' and accyear=" & sYear
            retstr = Master.ADO.runsql(mastconn, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("刪除電子轉帳檔 chf050失敗,請檢查" & sqlstr)
                Exit Sub
            End If
        End If

        lblMsg.Text = "作廢完成"
        txtChkNo.Enabled = True
        txtBank.Enabled = True
        btnChkno.Enabled = True
        btnFinish.Visible = False
        btnGiveUp.Visible = False
        txtBank.Focus()
    End Sub


    Protected Sub btnGiveUp_Click(sender As Object, e As EventArgs) Handles btnGiveUp.Click
        txtChkNo.Text = ""
        txtBank.Text = ""
        Call GiveUp()
    End Sub
    Sub GiveUp()
        txtChkNo.Enabled = True
        txtBank.Enabled = True
        btnChkno.Enabled = True
        btnFinish.Visible = False
        btnGiveUp.Visible = False
    End Sub
End Class