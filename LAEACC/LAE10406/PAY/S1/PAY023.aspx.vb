Imports System.Data
Imports System.Data.SqlClient

Public Class PAY023
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

        ViewState("intI") = 0   '傳票件數
        ViewState("sDate") = Session("userdate") 'put the login date to 開票日
        ViewState("sYear") = Session("sYear")   '年度

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
        lblMsg.Text = ""
        sqlstr = "select * from chf010 where bank='" & txtBank.Text & "' and chkno='" & txtChkNo.Text & _
                 "' and accyear=" & Session("sYear")
        mydataset = Master.ADO.openmember(DNS_ACC, "chf010", sqlstr)
        If mydataset.Tables("chf010").Rows.Count = 0 Then
            MessageBx("無此支票")
            txtChkNo.Focus()
            Exit Sub
        End If
        If mydataset.Tables("chf010").Rows(0).Item("start_no") <> 0 Then
            MessageBx("此支票已入帳不可更動")
            txtChkNo.Focus()
            Exit Sub
        End If
        txtChkNo.Enabled = False
        txtBank.Enabled = False
        btnChkno.Enabled = False
        txtRemark.Text = mydataset.Tables("chf010").Rows(0).Item("remark")
        lblamt.Text = FormatNumber(mydataset.Tables("chf010").Rows(0).Item("amt"), 0)
        txtChkname.Text = mydataset.Tables("chf010").Rows(0).Item("chkname")
        lblNo1.Text = mydataset.Tables("chf010").Rows(0).Item("no_1_no")
        lblChkname.Text = txtChkname.Text  '記錄原先資料
        lblRemark.Text = txtRemark.Text     '記錄原先資料

        '電子轉帳 ADD & mod 
        txtNewChkno.Enabled = True
        If Mid(txtChkNo.Text, 1, 2).ToUpper = "TR" Then
            '支票號不可變動
            txtNewChkno.Enabled = False
            txtNewChkno.Text = txtChkNo.Text.ToUpper
        Else
            '取新支票號
            sqlstr = "SELECT * FROM chf020 WHERE bank = '" & txtBank.Text & "'"
            mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
            If mydataset.Tables("chf020").Rows.Count > 0 Then
                With mydataset.Tables("chf020").Rows(0)
                    lblBankname.Text = .Item("bank") & .Item("bankname")
                    ViewState("BankBalance") = .Item("balance") + .Item("day_income") - .Item("day_pay") - .Item("unpay") + Master.ADO.nz(.Item("credit"), 0)
                    '支票號+1
                    txtNewChkno.Text = Master.ADO.nz(.Item("chkno"), "")
                    txtNewChkno.Text = Master.Controller.AddCheckNo(txtNewChkno.Text)
                    ViewState("intChkForm") = Master.ADO.nz(.Item("chkform"), 1)
                End With
            End If
        End If

        mydataset = Nothing

        txtRemark.Visible = True
        txtChkname.Visible = True
        btnFinish.Visible = True
        btnGiveUp.Visible = True
        txtNewChkno.Visible = True
        txtNewChkno.Focus()
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        Dim retstr As String
        txtChkNo.Text = txtChkNo.Text.ToUpper
        txtNewChkno.Text = txtNewChkno.Text.ToUpper

        If txtChkNo.Text = txtNewChkno.Text And txtChkname.Text = lblChkname.Text And txtRemark.Text = lblRemark.Text Then
            If Mid(txtChkNo.Text, 1, 2) <> "TR" Then
                Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
                Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
                Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
                Param.Add("UserId", Session("UserId"))    '使用者代號

                Param.Add("Bank", txtBank.Text)
                Param.Add("Bankname", lblBankname.Text)
                Param.Add("sDate", ViewState("sDate"))
                Param.Add("Chkname", txtChkname.Text)
                Param.Add("Amt", lblamt.Text)
                Param.Add("Remark", txtNewChkno.Text & txtRemark.Text)
                Param.Add("BankBalance", ViewState("BankBalance"))

                Param.Add("sSeason", Session("sSeason"))    '第幾季
                Param.Add("UserDate", Session("UserDate"))    '登入日期

                Master.PrintFR("PAY021支票套印", Session("ORG"), DNS_ACC, Param)
            End If
            Call GiveUp()
            Exit Sub
        End If

        '檢查支票碼重複
        If txtChkNo.Text <> txtNewChkno.Text Then
            '由一般支票轉為電子支票
            If Mid(txtChkNo.Text, 1, 2) <> "TR" And Mid(txtNewChkno.Text, 1, 2) = "TR" Then
                If Len(RTrim(txtNewChkno.Text)) < 10 Or Not IsNumeric(Mid(txtNewChkno.Text, 6, 5)) Then
                    MessageBx("請輸入TR" & Format(Session("sYear"), "000") & "00000" & "的支票格式")
                    Exit Sub
                End If
                sqlstr = "SELECT chkno FROM chf010 where chkno='" & txtNewChkno.Text & "'"
                mydataset = Master.ADO.openmember("", "chf010", sqlstr)
                If mydataset.Tables("chf010").Rows.Count > 0 Or Val(Mid(txtNewChkno.Text, 6, 5)) = 0 Then '有重複
                    '取新轉帳支票號
                    txtNewChkno.Text = "TR" & Format(Session("sYear"), "000") & Format(Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "T"), "00000")
                Else
                    '沒重複也要判斷號數不可大於控制檔
                    sqlstr = "SELECT * FROM acfno where kind='T' and accyear=" & Session("sYear")
                    mydataset = Master.ADO.openmember("", "acfno", sqlstr)
                    If mydataset.Tables("acfno").Rows.Count > 0 Then
                        If Val(Mid(txtNewChkno.Text, 6, 5)) - 1 > Master.ADO.nz(mydataset.Tables("acfno").Rows(0).Item("cont_no"), 0) Then
                            txtNewChkno.Text = "TR" & Format(Session("sYear"), "000") & Format(Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "T"), "00000")
                        End If
                    End If
                End If
            End If
            sqlstr = "SELECT chkno FROM chf010 where chkno='" & txtNewChkno.Text & "'"
            mydataset = Master.ADO.openmember(DNS_ACC, "chf010", sqlstr)
            If mydataset.Tables("chf010").Rows.Count > 0 Then
                MessageBx("支票碼重複")
                txtNewChkno.Focus()
                Exit Sub
            End If
            mydataset = Nothing

            '資料處理
            sqlstr = "update acf010 set chkno='" & txtNewChkno.Text.ToUpper & "' where accyear=" & Session("sYear") & _
                     " and chkno='" & txtChkNo.Text & "' and kind='2' and item='9'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("支票號碼寫入acf010->chkno失敗,請檢查" & sqlstr)
            End If

            '記錄最後一張支票號
            If Mid(txtNewChkno.Text, 1, 2) <> "TR" Then
                sqlstr = "update chf020 set chkno = '" & txtNewChkno.Text & "' where bank='" & txtBank.Text & "'"
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            Else
                Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
                Master.ADO.GenInsSql("vchkno", txtNewChkno.Text, "T")
                Master.ADO.GenInsSql("date_1", Session("UserDate"), "D")
                Master.ADO.GenInsSql("bank", txtBank.Text, "T")
                sqlstr = "insert into chf050 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                If retstr <> "sqlok" Then
                    MessageBx("新增支票寫入chf050失敗,請檢查" & sqlstr)
                Else
                    MessageBx("新電子支票=" & txtNewChkno.Text)
                End If
            End If
        End If

        '更正chf010 
        sqlstr = "update chf010 set chkno = '" & txtNewChkno.Text.ToUpper & "', chkname = N'" & txtChkname.Text & _
                 "', remark = N'" & txtRemark.Text & "' where bank='" & txtBank.Text & _
                 "' and chkno='" & txtChkNo.Text & "' and accyear=" & Session("sYear")
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("update chf010失敗,請檢查" & sqlstr)
        End If

        lblMsg.Text = "修改完成"

        '電子轉帳 MOD
        If Mid(txtNewChkno.Text, 1, 2) <> "TR" Then
            Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
            Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
            Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
            Param.Add("UserId", Session("UserId"))    '使用者代號

            Param.Add("Bank", txtBank.Text)
            Param.Add("Bankname", lblBankname.Text)
            Param.Add("sDate", ViewState("sDate"))
            Param.Add("Chkname", txtChkname.Text)
            Param.Add("Amt", lblamt.Text)
            Param.Add("Remark", txtNewChkno.Text & txtRemark.Text)
            Param.Add("BankBalance", ViewState("BankBalance"))

            Param.Add("ckprint1", IIf(ckprint1.Checked, 1, 0))
            Param.Add("ckprint2", IIf(ckprint2.Checked, 1, 0))
            Param.Add("ckprint3", IIf(ckprint3.Checked, 1, 0))

            Param.Add("sSeason", Session("sSeason"))    '第幾季
            Param.Add("UserDate", Session("UserDate"))    '登入日期

            Master.PrintFR("PAY021支票套印", Session("ORG"), DNS_ACC, Param)
        End If

        Call GiveUp()
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
        txtBank.Focus()
        txtRemark.Visible = False
        txtChkname.Visible = False
        btnFinish.Visible = False
        btnGiveUp.Visible = False
        txtNewChkno.Visible = False
    End Sub
End Class