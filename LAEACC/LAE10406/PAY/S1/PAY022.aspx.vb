Imports System.Data
Imports System.Data.SqlClient

Public Class PAY022
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

        '電子轉帳
        ViewState("blnUseTR") = False  '無使用電子轉帳
        Dim myds As DataSet = Master.ADO.openmember(DNS_ACC, "user", "select * from accname where accno='5'")
        If myds.Tables("user").Rows.Count > 0 Then
            If Master.ADO.nz(myds.Tables("user").Rows(0).Item("bank"), "") = "TR" Then
                ViewState("blnUseTR") = True '有使用電子轉帳
            End If
        End If

        Dim sqlstr As String
        ViewState("sDate") = Session("userdate")    'put the login date to 收付款日

        '先由chf020找目前收付款日   
        sqlstr = "SELECT date_2 FROM chf020 where date_2 is not null"
        mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        If mydataset.Tables("chf020").Rows.Count > 0 Then
            ViewState("oldDate_2") = Master.Models.strDateADToChiness(mydataset.Tables("chf020").Rows(0).Item("date_2").toshortdatestring)  '目前收付款日
            dtpDate_2.Text = ViewState("oldDate_2")
        Else
            ViewState("oldDate_2") = ""  '表示上日已作結存並已新開帳
            dtpDate_2.Text = ViewState("sDate")
            lblMsgDate.Text = "新結存日"
        End If

        If ViewState("blnUseTR") Then   '有使用電子轉帳
            ViewState("blnUseTR") = True
        Else
            ViewState("blnUseTR") = False
        End If

        TabPanel1.Visible = False
        TabPanel2.Visible = True



    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        txtBank.Attributes.Add("maxlength", "2")
        txtBank.Attributes.Add("onkeyup", "return ismaxlength(this)")
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            'AddDefaultFirstRecord()
            btnFinish.Attributes.Add("onclick", "this.disabled = true;this.value = '請稍候..';" + Page.ClientScript.GetPostBackEventReference(btnFinish, ""))
        End If
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

    End Sub
    Sub LoadGridFunc()

    End Sub
    Sub AddDefaultFirstRecord()
        'creating dataTable   
        Dim dt As New DataTable()
        dt.TableName = "dtgTarget"
        dt.Columns.Add(New DataColumn("no_1_no", GetType(String)))
        dt.Columns.Add(New DataColumn("REMARK", GetType(String)))
        dt.Columns.Add(New DataColumn("act_amt", GetType(String)))
        dt.Columns.Add(New DataColumn("bank", GetType(String)))
        dt.Columns.Add(New DataColumn("chkno", GetType(String)))
        dt.Columns.Add(New DataColumn("no_2_no", GetType(String)))
        ViewState("dtgTarget") = dt
        DataGridView.DataSource = dt
        DataGridView.DataBind()
    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
#End Region
#Region "@DataGridView@"
    Sub DataGridView_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGridView.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號


        Select Case e.CommandName
            Case "btnDelete"
                'If ViewState("dtgTarget") IsNot Nothing Then
                '    'get datatable from view state   
                '    Dim dtCurrentTable As DataTable = DirectCast(ViewState("dtgTarget"), DataTable)
                '    Dim drCurrentRow As DataRow = Nothing

                '    For RowI = 0 To dtCurrentTable.Rows.Count - 1
                '        If dtCurrentTable.Rows(RowI)(0).ToString() = txtID.Text Then
                '            'lblMsg.Text = LastPos
                '            lblTotAmt.Text = FormatNumber(Master.Models.ValComa(lblTotAmt.Text) - dtCurrentTable.Rows(RowI)("Act_Amt").ToString(), 0)
                '            ViewState("arrayNo") = ViewState("arrayNo").Replace(Format(CInt(dtCurrentTable.Rows(RowI)("no_1_no")), "00000") & ",", "") '將傳票置array

                '            ViewState("intCnt") -= 1   '筆數減1
                '            lblTotNo.Text = FormatNumber(ViewState("intCnt"), 0)
                '            dtCurrentTable.Rows(RowI).Delete()
                '            Exit For
                '        End If
                '    Next
                '    dtCurrentTable.AcceptChanges()

                '    ViewState("dtgTarget") = dtCurrentTable
                '    DataGridView.DataSource = dtCurrentTable
                '    DataGridView.DataBind()
                'End If

        End Select
    End Sub

#End Region
#Region "按鍵選項"

#End Region



#Region "@共用底層副程式@"

#End Region

#Region "@資料查詢@"


#End Region

#Region "物件選擇異動值"

#End Region


    Protected Sub btnDate_2_Click(sender As Object, e As EventArgs) Handles btnDate_2.Click
        If ViewState("oldDate_2") <> "" Then
            If CDate(dtpDate_2.Text) <> CDate(ViewState("oldDate_2")) Then
                lblMsgDate.Text = "本日未開帳"
                Exit Sub
            End If
        End If

        '新結存日  找最近收付款日(控制目前收付款日不可小於最近收付款日)
        If ViewState("oldDate_2") = "" Then
            sqlstr = "SELECT MAX(date_2) AS DATE_2 FROM chf030 "
            mydataset = Master.ADO.openmember(DNS_ACC, "chf030", sqlstr)
            If mydataset.Tables("chf030").Rows.Count > 0 Then
                If dtpDate_2.Text < Master.Models.strDateADToChiness(mydataset.Tables("chf030").Rows(0).Item("date_2").toshortdatestring) Then
                    lblMsgDate.Text = "目前收付款日不可小於最近收付款日" & Master.Models.strDateADToChiness(mydataset.Tables("chf030").Rows(0).Item("date_2").toshortdatestring)
                    Exit Sub
                End If
            End If
        End If
        ViewState("sDate") = dtpDate_2.Text
        lblDate_2.Text = ViewState("sDate")
        Session("sdate2") = ViewState("sDate")  'put the date2 to memory for pay025 use 
        ViewState("sYear") = Year(ViewState("sDate"))   '年度
        txtBank.Enabled = True
        txtChkNo.Enabled = True
        btnChkno.Visible = True
        If ViewState("blnUseTR") Then btnChkNo2.Visible = True
        btnPay025.Enabled = True
        txtBank.Focus()

        TabPanel1.Visible = True
        TabPanel2.Visible = False
    End Sub

    Protected Sub btnChkno_Click(sender As Object, e As EventArgs) Handles btnChkno.Click
        lblMsg.Text = "" : txtChkNo.Text = txtChkNo.Text.ToUpper
        sqlstr = "select * from chf010 where bank='" & txtBank.Text & "' and chkno='" & txtChkNo.Text & _
         "' and kind='2' and accyear=" & ViewState("sYear")
        mydataset = Master.ADO.openmember(DNS_ACC, "chf010", sqlstr)
        If mydataset.Tables("chf010").Rows.Count = 0 Then
            MessageBx("無此支票")
            txtChkNo.Focus()
            Exit Sub
        End If
        If mydataset.Tables("chf010").Rows(0).Item("start_no") <> 0 Then
            MessageBx("此支票已入帳")
            txtChkNo.Focus()
            Exit Sub
        End If

        lblamt.Text = FormatNumber(mydataset.Tables("chf010").Rows(0).Item("amt"), 0)
        lblChkname.Text = mydataset.Tables("chf010").Rows(0).Item("chkname")

        '取支款編號
        Dim intNo As Integer
        ViewState("intNo2") = Master.Controller.QueryNO(Val(ViewState("sYear")), "5")    '\accservice\service1.asmx
        ViewState("StartNo") = ViewState("intNo2") + 1
        sqlstr = "select no_1_no, remark, act_amt, bank, 0 as no_2_no from acf010 where accyear=" & ViewState("sYear") & _
                 " and chkno='" & txtChkNo.Text & "' and kind='2' and item='9' order by chkseq"
        Dim myDatasetS As DataSet = Master.ADO.openmember(DNS_ACC, "acf010s", sqlstr)
        For intI = 0 To myDatasetS.Tables("acf010s").Rows.Count - 1
            myDatasetS.Tables("acf010s").Rows(intI).Item("no_2_no") = ViewState("intNo2") + intI + 1
        Next
        ViewState("dtgTarget") = myDatasetS
        DataGridView.DataSource = myDatasetS
        DataGridView.DataBind()

        btnFinish.Visible = True
        btnGiveUp.Visible = True
        btnChkno.Visible = False
        btnChkNo2.Visible = False  '103/2/27  add
        txtBank.Enabled = False
        txtChkNo.Enabled = False
        btnPay025.Enabled = False
        btnFinish.Focus()
    End Sub

    Protected Sub btnChkNo2_Click(sender As Object, e As EventArgs) Handles btnChkNo2.Click
        If Not IsNumeric(txtChkNo.Text) Then
            MessageBx("請輸入數字即可,請重輸入")
            Exit Sub
        End If
        txtChkNo.Text = "TR" & Format(CInt(ViewState("sYear")), "000") & Format(Val(txtChkNo.Text), "00000") '轉成電子支票格式  TRyyyXXXXX
        Call btnChkno_Click(New Object, New System.EventArgs)
    End Sub

    Protected Sub btnPay025_Click(sender As Object, e As EventArgs) Handles btnPay025.Click
        Server.Transfer("Pay025.aspx")
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        Dim intNo1 As Integer
        Dim retstr As String
        txtChkNo.Text = txtChkNo.Text.ToUpper

        Dim myDatasetS As DataSet
        If ViewState("dtgTarget") IsNot Nothing Then
            'get datatable from view state   
            myDatasetS = DirectCast(ViewState("dtgTarget"), DataSet)
        Else
            MessageBx("ViewState中無資料，請重新退出再進入")
            Exit Sub
        End If

        For intI = 0 To myDatasetS.Tables("acf010s").Rows.Count - 1
            intNo1 = myDatasetS.Tables("acf010s").Rows(intI).Item("no_1_no")
            ViewState("intNo2") += 1
            sqlstr = "update acf010 set no_2_no=" & ViewState("intNo2") & ", date_2 = '" & Master.Models.strDateChinessToAD(ViewState("sDate")) & _
                     "'  where accyear=" & ViewState("sYear") & " and kind='2' and no_1_no=" & intNo1
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("填入acf010付款日及支款號錯誤" & sqlstr)
                Exit Sub
            End If

            sqlstr = "update acf020 set no_2_no=" & ViewState("intNo2") & " where accyear=" & ViewState("sYear") & " and kind='2' and no_1_no=" & intNo1
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("填入acf020支款號錯誤" & sqlstr)
                Exit Sub
            End If

            '檢查是否為銀行轉帳,要刪除第一項及第二項
            sqlstr = "select accno from acf010 where kind='2' and item='1' and accyear=" & ViewState("sYear") & " and no_1_no=" & intNo1
            mydataset = Master.ADO.openmember(DNS_ACC, "acf010t", sqlstr)
            If mydataset.Tables("acf010t").Rows.Count > 0 Then
                sqlstr = Master.ADO.nz(RTrim(mydataset.Tables("acf010t").Rows(0).Item("accno")), "")
                If sqlstr = "11102" Or sqlstr = "11103" Then
                    sqlstr = "delete from acf010 where kind='2' and item='1' and accyear=" & ViewState("sYear") & " and no_1_no=" & intNo1
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    sqlstr = "delete from acf020 where kind='2' and item='2' and accyear=" & ViewState("sYear") & " and no_1_no=" & intNo1
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                End If
            End If
        Next

        '更正chf020 支票金額由unpay扣除,並加入本日共支
        sqlstr = "update chf020 set unpay = unpay - " & Master.Models.ValComa(lblamt.Text) & ", day_pay = day_pay + " & _
                 Master.Models.ValComa(lblamt.Text) & ", prt_code = ' ', date_2 = '" & Master.Models.strDateChinessToAD(ViewState("sDate")) & "' where bank='" & txtBank.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("支票金額由unpay扣除,並加入本日共支錯誤" & sqlstr)
            Exit Sub
        End If


        '更正chf010 
        sqlstr = "update chf010 set date_2 = '" & Master.Models.strDateChinessToAD(ViewState("sDate")) & "', start_no= " & ViewState("StartNo") & _
                  ", end_no = " & ViewState("intNo2") & " where bank='" & txtBank.Text & "' and chkno='" & txtChkNo.Text & _
                  "' and kind='2' and accyear=" & ViewState("sYear")
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("填入acf010付款日及支款號錯誤" & sqlstr)
            Exit Sub
        End If


        '更正支款編號acfno 
        sqlstr = "update acfno set cont_no=" & ViewState("intNo2") & " where accyear=" & ViewState("sYear") & " and kind='5'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("更正支款編號acfno錯誤" & sqlstr)
            Exit Sub
        End If


        '填上該電子支票付款日期 chf050.date_2  103/2/27 add
        If Mid(txtChkNo.Text, 1, 2) = "TR" Then
            sqlstr = "update chf050 set date_2='" & Master.Models.strDateChinessToAD(ViewState("sDate")) & "' where bank='" & txtBank.Text & _
                             "' and vchkno='" & txtChkNo.Text & "' and accyear=" & ViewState("sYear")
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("chf050填上付款日期錯誤" & sqlstr)
                Exit Sub
            End If
        End If

        lblMsg.Text = "記帳完成"
        myDatasetS.Tables("acf010s").Clear()  '清除datagrid1資料
        ViewState("dtgTarget") = myDatasetS
        DataGridView.DataSource = myDatasetS
        DataGridView.DataBind()

        txtBank.Text = ""
        txtChkNo.Text = ""
        txtBank.Enabled = True
        txtChkNo.Enabled = True
        btnChkno.Visible = True
        If ViewState("blnUseTR") Then btnChkNo2.Visible = True '103/2/27 add
        btnPay025.Enabled = True
        btnFinish.Visible = False
        btnGiveUp.Visible = False
        txtBank.Focus()
    End Sub

    Protected Sub btnGiveUp_Click(sender As Object, e As EventArgs) Handles btnGiveUp.Click
        btnFinish.Visible = False
        btnGiveUp.Visible = False
        btnChkno.Visible = True
        If ViewState("blnUseTR") Then btnChkNo2.Visible = True '103/2/27 add
        txtBank.Enabled = True
        txtChkNo.Enabled = True
        btnPay025.Enabled = True
        txtBank.Text = ""
        txtChkNo.Text = ""
        txtBank.Focus()
    End Sub


End Class