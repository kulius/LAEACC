Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class ACF010D
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
    Dim strSQL99, retstr As String
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
    Dim mydataset, mydataset2, myDatasetS, myDatasetT As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱

        txtYear.Text = Session("sYear")

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





    Protected Sub btnSureNo_Click(sender As Object, e As EventArgs) Handles btnSureNo.Click
        Dim myds As DataSet
        If Trim(txtNo1.Text) = "" Or Val(txtYear.Text) = 0 Then
            MessageBx("請輸入正確傳票")
            txtNo1.Focus()
            Exit Sub
        End If
        ViewState("sYear") = Val(txtYear.Text)
        Dim sqlstr As String
        If rdbKind1.Checked Then viewstate("skind") = "1"
        If rdbKind2.Checked Then viewstate("skind") = "2"
        If rdbKind3.Checked Then viewstate("skind") = "3"
        If viewstate("skind") <= "2" Then
            sqlstr = "select * from acf010 where kind='" & viewstate("skind") & "' and item='9'"
        Else
            sqlstr = "select * from acf010 where kind='" & ViewState("skind") & "' and item='1' "
        End If
        sqlstr &= " and accyear=" & viewstate("sYear") & " and no_1_no=" & Trim(txtNo1.Text)
        myds = Master.ADO.openmember(DNS_ACC, "acf010t", sqlstr)
        If myds.Tables("acf010t").Rows.Count = 0 Then
            MessageBx("無此傳票")
            txtNo1.Text = ""
            txtNo1.Focus()
            Exit Sub
        End If
        If Not IsDBNull(myds.Tables(0).Rows(0).Item("date_2")) Then
            MessageBx("此傳票已作帳" & myds.Tables(0).Rows(0).Item("date_2").toshortdatestring & "  第" & Master.ADO.nz(myds.Tables(0).Rows(0).Item("no_2_no"), 0) & "號")
            txtNo1.Text = ""
            txtNo1.Focus()
            Exit Sub
        End If
        If viewstate("skind") <= "2" Then
            If Not IsDBNull(myds.Tables(0).Rows(0).Item("chkno")) Then
                If Master.ADO.nz(myds.Tables(0).Rows(0).Item("chkno"), "") <> "" Then
                    MessageBx("此傳票已開支票,號碼=" & Master.ADO.nz(myds.Tables(0).Rows(0).Item("chkno"), ""))
                    txtNo1.Text = ""
                    txtNo1.Focus()
                    Exit Sub
                End If
            End If
        End If
        lblAccno.Text = Master.ADO.nz(myds.Tables(0).Rows(0).Item("accno"), "")
        lblAmt.Text = FormatNumber(Master.ADO.nz(myds.Tables(0).Rows(0).Item("amt"), 0), 2)
        lblRemark.Text = Master.ADO.nz(myds.Tables(0).Rows(0).Item("remark"), "")

        btnFinish.Visible = True
        btnGiveUp.Visible = True
        GroupBox1.Enabled = False
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        Dim myds As DataSet
        Dim sqlstr, retstr As String
        '更正acf010傳票
        If ViewState("skind") <= "2" Then
            sqlstr = "delete acf020 where kind='" & ViewState("skind") & "' and accyear=" & ViewState("sYear") & " and no_1_no=" & Trim(txtNo1.Text)
        Else
            sqlstr = "delete acf020 where kind>='3' and accyear=" & ViewState("sYear") & " and no_1_no=" & Trim(txtNo1.Text)
        End If
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MsgBox("刪除acf020傳票失敗,請檢查" & sqlstr)
            Exit Sub
        End If
        If ViewState("skind") <= "2" Then
            sqlstr = "delete acf010 where kind='" & ViewState("skind") & "' and accyear=" & ViewState("sYear") & " and no_1_no=" & Trim(txtNo1.Text)
        Else
            sqlstr = "delete acf010 where kind>='3' and accyear=" & ViewState("sYear") & " and no_1_no=" & Trim(txtNo1.Text)
        End If
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("更正acf010傳票失敗,請檢查" & sqlstr)
            Exit Sub
        End If
        '刪除transpay 匯款檔
        If ViewState("skind") = "2" Then
            'insert into TransPay (accyear, date_1, payseq, no_1_NO, item, area, bank, remark, act_amt
            sqlstr = "delete transpay where accyear=" & ViewState("sYear") & " and no_1_no=" & Trim(txtNo1.Text)
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        End If

        lblAccno.Text = ""
        lblAmt.Text = ""
        lblRemark.Text = ""

        btnFinish.Visible = False
        btnGiveUp.Visible = False
        GroupBox1.Enabled = True

        MessageBx("刪除製票編號【" & txtNo1.Text & "】，作業已完成!!")
    End Sub

    Protected Sub btnGiveUp_Click(sender As Object, e As EventArgs) Handles btnGiveUp.Click
        GroupBox1.Enabled = True
        btnFinish.Visible = False
        btnGiveUp.Visible = False
    End Sub
End Class