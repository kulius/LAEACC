Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class AC220
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


        Dim myds As DataSet
        '先由chf020找目前收付款日   
        sqlstr = "SELECT date_2 FROM chf020 where date_2 is not null"
        myds = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        If myds.Tables("chf020").Rows.Count > 0 Then
            ViewState("sDate") = myds.Tables("chf020").Rows(0).Item("date_2")  '目前收付款日
        Else
            MessageBx("目前無收付款日,本功能只提供當日之帳務修改")
        End If
        ViewState("sYear") = Year(ViewState("sDate"))   '年度

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
        If Trim(txtChkNo.Text) = "" Then
            MessageBx("請輸入支票號")
            txtChkNo.Focus()
            Exit Sub
        End If
        Dim sqlstr As String
        ViewState("skind") = IIf(rdbKind1.Checked, "1", "2")
        sqlstr = "select * from chf010 where kind='" & ViewState("skind") & _
                 "' and bank='" & txtBank.Text & "' and chkno='" & txtChkNo.Text & "' and accyear=" & ViewState("sYear")
        myds = Master.ADO.openmember(DNS_ACC, "acf010t", sqlstr)
        If myds.Tables("acf010t").Rows.Count = 0 Then
            MessageBx("無此支票")
            txtChkNo.Focus()
            Exit Sub
        End If
        If IsDBNull(myds.Tables("acf010t").Rows(0).Item("date_2")) Then
            MessageBx("此支票尚未領取,請檢查")
            txtChkNo.Focus()
            Exit Sub
        End If
        If Master.Models.FullDate(myds.Tables("acf010t").Rows(0).Item("date_2")) <> Master.Models.FullDate(ViewState("sDate")) Then
            MessageBx("此功能只提供當日之帳務修改,目前收付款日=" & myds.Tables("acf010t").Rows(0).Item("date_2") & "<>" & Master.Models.FullDate(ViewState("sDate")))
            txtChkNo.Focus()
            Exit Sub
        End If
        With myds.Tables("acf010t")
            txtNewBank.Text = Master.ADO.nz(.Rows(0).Item("bank"), "")
            txtNewChkNo.Text = Master.ADO.nz(.Rows(0).Item("chkno"), "")
            lblDate_2.Text = Master.Models.ShortDate(.Rows(0).Item("date_2"))
            lblAmt.Text = FormatNumber(Master.ADO.nz(.Rows(0).Item("amt"), 0), 2)
            lblNo2.Text = FormatNumber(Master.ADO.nz(.Rows(0).Item("start_no"), 0), 0) & "至" & FormatNumber(Master.ADO.nz(.Rows(0).Item("end_no"), 0), 0)
            lblRemark.Text = Master.ADO.nz(.Rows(0).Item("remark"), "")
            lblChkname.Text = Master.ADO.nz(.Rows(0).Item("chkname"), "")
        End With
        btnFinish.Enabled = True
        btnSureNo.Enabled = False
        'gbxNew.Enabled = True
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        Dim myds As DataSet
        If txtBank.Text = txtNewBank.Text And txtChkNo.Text = txtNewChkNo.Text Then
            MessageBx("未異動,本作業不作任何資料修正")
            Exit Sub
        End If
        Dim sqlstr, retstr As String
        sqlstr = "select * from chf010 where kind='" & ViewState("skind") & _
                "' and bank='" & txtNewBank.Text & "' and chkno='" & txtNewChkNo.Text & "' and accyear=" & ViewState("sYear")
        myds = Master.ADO.openmember("", "acf010t", sqlstr)
        If myds.Tables("acf010t").Rows.Count > 0 Then
            MessageBx("支票號重複!")
            txtNewChkNo.Focus()
            Exit Sub
        End If

        '更正acf010傳票
        sqlstr = "update acf010 set bank='" & txtNewBank.Text & "', chkno='" & txtNewChkNo.Text & "'" & _
                 " where kind='" & ViewState("skind") & "' and accyear=" & ViewState("sYear") & " and date_2='" & Master.Models.FullDate(ViewState("sDate")) & _
                 "' and bank='" & txtBank.Text & "' and chkno='" & txtChkNo.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("更正acf010傳票失敗,請檢查" & sqlstr)
            Exit Sub
        End If
        If txtBank.Text <> txtNewBank.Text Then   '銀行異動要改傳票科目
            sqlstr = "select accno from chf020 where bank='" & txtNewBank.Text & "'"
            myds = Master.ADO.openmember("", "acf010t", sqlstr)
            If myds.Tables("acf010t").Rows.Count > 0 Then
                Dim straccno As String = myds.Tables("acf010t").Rows(0).Item(0)
                sqlstr = "update acf010 set accno='" & straccno & "'" & _
                         " where item='9' and kind='" & ViewState("skind") & "' and accyear=" & ViewState("sYear") & " and date_2='" & Master.Models.FullDate(ViewState("sDate")) & _
                         "' and bank='" & txtNewBank.Text & "' and chkno='" & txtNewChkNo.Text & "'"
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                If retstr <> "sqlok" Then
                    MessageBx("更正acf010傳票失敗,請檢查" & sqlstr)
                    Exit Sub
                End If
            End If
        End If

        '更正chf020本日共收 or 本日共付
        If txtBank.Text <> txtNewBank.Text Then
            '扣除原銀行收付
            If ViewState("skind") = "1" Then
                sqlstr = "update chf020 set day_income = day_income - " & Master.Models.ValComa(lblAmt.Text) & " where bank='" & txtBank.Text & "'"
            Else
                sqlstr = "update chf020 set day_pay = day_pay - " & Master.Models.ValComa(lblAmt.Text) & " where bank='" & txtBank.Text & "'"
            End If
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("更正chf020失敗,請檢查" & sqlstr)
            End If
            '加至新銀行收付
            If ViewState("skind") = "1" Then
                sqlstr = "update chf020 set day_income = day_income + " & Master.Models.ValComa(lblAmt.Text)
            Else
                sqlstr = "update chf020 set day_pay = day_pay + " & Master.Models.ValComa(lblAmt.Text)
            End If
            sqlstr &= ", date_2='" & Master.Models.FullDate(ViewState("sDate")) & "', prt_code=' '  where bank='" & txtNewBank.Text & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("更正chf020失敗,請檢查" & sqlstr)
            End If
        End If

        '更正支票chf010 
        sqlstr = "update chf010 set bank ='" & txtNewBank.Text & "', chkno='" & txtNewChkNo.Text & _
                 "' where bank='" & txtBank.Text & "' and chkno='" & txtChkNo.Text & "' and accyear=" & ViewState("sYear")
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("update chf010失敗,請檢查" & sqlstr)
            Exit Sub
        End If
        MessageBx("修改完成")
        btnSureNo.Enabled = True
        btnFinish.Enabled = True
        'gbxNew.Enabled = False
    End Sub

    Protected Sub btnGiveUp_Click(sender As Object, e As EventArgs) Handles btnGiveUp.Click
        btnSureNo.Enabled = True
    End Sub
End Class