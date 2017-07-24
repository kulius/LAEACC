Imports System.Data
Imports System.Data.SqlClient

Public Class PAY025
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

        ViewState("arrayNo") = "" '將傳票置array
        ViewState("intCnt") = 0  '傳票件數
        ViewState("intChkForm") = 1
        ViewState("intI") = 0  '傳票件數

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
        TabPanel2.Visible = False
        TabPanel3.Visible = True




        txtNo1.Focus()



    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            AddDefaultFirstRecord()
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
                If ViewState("dtgTarget") IsNot Nothing Then
                    'get datatable from view state   
                    Dim dtCurrentTable As DataTable = DirectCast(ViewState("dtgTarget"), DataTable)
                    Dim drCurrentRow As DataRow = Nothing

                    For RowI = 0 To dtCurrentTable.Rows.Count - 1
                        If dtCurrentTable.Rows(RowI)(0).ToString() = txtID.Text Then
                            'lblMsg.Text = LastPos
                            lblTotAmt.Text = FormatNumber(Master.Models.ValComa(lblTotAmt.Text) - dtCurrentTable.Rows(RowI)("Act_Amt").ToString(), 0)
                            ViewState("arrayNo") = ViewState("arrayNo").Replace(Format(CInt(dtCurrentTable.Rows(RowI)("no_1_no")), "00000") & ",", "") '將傳票置array

                            ViewState("intCnt") -= 1   '筆數減1
                            lblTotNo.Text = FormatNumber(ViewState("intCnt"), 0)
                            dtCurrentTable.Rows(RowI).Delete()
                            Exit For
                        End If
                    Next
                    dtCurrentTable.AcceptChanges()

                    ViewState("dtgTarget") = dtCurrentTable
                    DataGridView.DataSource = dtCurrentTable
                    DataGridView.DataBind()
                End If

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



    Protected Sub btnSureNo_Click(sender As Object, e As EventArgs) Handles btnSureNo.Click
        Dim mydsS As DataSet
        lblMsg.Text = ""
        If Trim(txtNo1.Text) = "" Then
            MessageBx("請輸入傳票")
            txtNo1.Focus()
            Exit Sub
        End If
        If ViewState("arrayNo").IndexOf(Format(Val(txtNo1.Text), "00000")) >= 0 Then
            MessageBx("傳票重複")
            txtNo1.Focus()
            Exit Sub
        End If
        sqlstr = "select no_1_no, remark, act_amt, bank, chkno, no_2_no from acf010 where kind='2' and item='9' and accyear=" & _
                  ViewState("sYear") & " and no_1_no=" & Trim(txtNo1.Text)
        mydsS = Master.ADO.openmember(DNS_ACC, "acf010t", sqlstr)
        If mydsS.Tables("acf010t").Rows.Count = 0 Then
            MessageBx("無此傳票")
            txtNo1.Focus()
            Exit Sub
        End If
        If Master.ADO.nz(mydsS.Tables("acf010t").Rows(0).Item("chkno"), "") = "" Then
        Else
            If mydsS.Tables("acf010t").Rows(0).Item("no_2_no") <> 0 Then
                MessageBx("此傳票已作帳" & mydsS.Tables("acf010t").Rows(0).Item("no_2_no"))
                txtNo1.Focus()
                Exit Sub
            Else
                MessageBx("此傳票已開支票" & mydsS.Tables("acf010t").Rows(0).Item("chkno"))
                txtNo1.Focus()
                Exit Sub
            End If
        End If
        If ViewState("intCnt") > 0 Then   '第二筆以後要檢察銀行要相同
            If mydsS.Tables("acf010t").Rows(0).Item("bank") <> lblBank.Text Then
                MessageBx("銀行不相同,請重輸入傳票" & mydsS.Tables("acf010t").Rows(0).Item("bank"))
                txtNo1.Focus()
                Exit Sub
            End If
        End If
        lblNo1.Text = mydsS.Tables("acf010t").Rows(0).Item("no_1_no")
        lblBank.Text = mydsS.Tables("acf010t").Rows(0).Item("bank")
        lblAct_Amt.Text = FormatNumber(mydsS.Tables("acf010t").Rows(0).Item("act_amt"), 2)
        lblRemark.Text = Trim(mydsS.Tables("acf010t").Rows(0).Item("remark"))
        Dim intTemp As Integer
        If ViewState("intCnt") = 0 Then    '第一筆
            ViewState("sBank") = lblBank.Text '記錄第一筆銀行以便控制銀行要相同
            intTemp = lblRemark.Text.IndexOf("  ")
            If intTemp < 0 Then intTemp = Len(lblRemark.Text) - 7 '找不到則取最後五個字
            txtRemark.Text = lblRemark.Text
            If intTemp <= 0 Then
                intTemp = Len(lblRemark.Text)
                txtChkname.Text = Microsoft.VisualBasic.Right(lblRemark.Text, IIf(intTemp >= 5, 5, intTemp))
            Else
                txtChkname.Text = Trim(Mid(lblRemark.Text, intTemp + 2))      '由摘要取受款人
            End If
            lblMsg.Text = ""
            '取收款編號
            ViewState("intNo2") = Master.Controller.QueryNO(Val(ViewState("sYear")), "5")     '\accservice\service1.asmx
            ViewState("StartNo") = ViewState("intNo2") + 1
            '支票號
            txtChkNo.Text = Format(ViewState("intNo2") + 1, "00000")
            '帳戶餘額
            sqlstr = "SELECT * FROM chf020 WHERE bank = '" & lblBank.Text & "'"
            mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
            If mydataset.Tables("chf020").Rows.Count > 0 Then
                With mydataset.Tables("chf020").Rows(0)
                    lblBankName.Text = .Item("bank") & .Item("bankname")
                    lblBalance.Text = FormatNumber(.Item("balance") + .Item("day_income") - .Item("day_pay") - .Item("unpay") + .Item("credit"), 2)
                End With
            End If
            mydataset = Nothing
        End If

        If Master.Models.ValComa(lblBalance.Text) < Master.Models.ValComa(lblTotAmt.Text) + Master.Models.ValComa(lblAct_Amt.Text) Then
            MessageBx("存款不足")
            txtNo1.Text = ""
            txtNo1.Focus()
            Exit Sub
        End If

        Call AddDataGrid()
        txtNo1.Text = ""
        btnSure.Enabled = True
        txtNo1.Focus()
    End Sub

    Sub AddDataGrid()
        Dim dtCurrentTable As DataTable
        Dim drCurrentRow As DataRow = Nothing
        If ViewState("dtgTarget") IsNot Nothing Then
            dtCurrentTable = DirectCast(ViewState("dtgTarget"), DataTable)
        Else
            MessageBx("ViewState無內容")
            Exit Sub
        End If
        ViewState("intCnt") += 1
        Dim nr As DataRow
        ViewState("arrayNo") = ViewState("arrayNo") & Format(Val(lblNo1.Text), "00000") & ","   '將傳票置入字串(最後放入資料庫時,會將逗號取消掉)

        drCurrentRow = dtCurrentTable.NewRow()
        drCurrentRow("no_1_no") = lblNo1.Text
        drCurrentRow("remark") = lblRemark.Text
        drCurrentRow("act_amt") = Master.Models.ValComa(lblAct_Amt.Text)
        drCurrentRow("bank") = lblBank.Text
        drCurrentRow("no_2_no") = ViewState("intNo2") + dtCurrentTable.Rows.Count + 1
        dtCurrentTable.Rows.Add(drCurrentRow)

        ViewState("dtgTarget") = dtCurrentTable
        DataGridView.DataSource = dtCurrentTable
        DataGridView.DataBind()

        lblTotAmt.Text = FormatNumber(Master.Models.ValComa(lblTotAmt.Text) + Master.Models.ValComa(lblAct_Amt.Text), 0)
        lblTotNo.Text = FormatNumber(ViewState("intCnt"), 0)
    End Sub

    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        lblTotamt2.Text = FormatNumber(Master.Models.ValComa(lblTotAmt.Text), 0)
        TabContainer1.ActiveTabIndex = 1

        ViewState("strCheckForm") = "PAPER"  '電子轉帳 add
        lblChkTR.Text = ""      '電子轉帳 add
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        Dim intI, intNo As Integer
        Dim tempdataset, myDatasetS, mydsS As DataSet
        Dim retstr As String

        Dim bm As DataTable
        If ViewState("dtgTarget") IsNot Nothing Then
            'get datatable from view state   
            bm = DirectCast(ViewState("dtgTarget"), DataTable)
        Else
            MessageBx("ViewState中無資料，請重新退出再進入")
            Exit Sub
        End If


        If bm.Rows.Count = 0 Then
            MessageBx("無傳票資料")
            TabContainer1.ActiveTabIndex = 0
            txtNo1.Focus()
            Exit Sub
        End If

        If txtChkNo.Text = "" Then
            MessageBx("支票碼錯誤")
            txtChkNo.Focus()
            Exit Sub
        End If

        '檢查支票碼重複
        sqlstr = "SELECT chkno FROM chf010 where kind='2' and chkno='" & txtChkNo.Text & "' and accyear=" & ViewState("sYear")
        tempdataset = Master.ADO.openmember(DNS_ACC, "chf010", sqlstr)
        If tempdataset.Tables("chf010").Rows.Count > 0 Then
            MessageBx("支票碼重複")
            txtChkNo.Focus()
            Exit Sub
        End If
        tempdataset = Nothing

        '將支票號碼寫入acf010->chkno & 新增一筆資料至chf010 
        For intI = 0 To bm.Rows.Count - 1
            'bm.Position = intI
            intNo = bm.Rows(intI).Item("no_1_no")
            ViewState("intNo2") += 1
            sqlstr = "update acf010 set no_2_no=" & ViewState("intNo2") & ", date_2 = '" & Master.Models.strDateChinessToAD(ViewState("sDate")) & _
                     "', chkno='" & txtChkNo.Text & "', chkseq=" & intI + 1 & " where accyear=" & ViewState("sYear") & " and kind='2' and no_1_no=" & intNo
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("支票號碼填入acf010->chkno失敗,請檢查" & sqlstr)
            End If
            Master.ADO.GenUpdsql("no_2_no", ViewState("intNo2"), "N")
            sqlstr = "update acf020 set no_2_no=" & ViewState("intNo2") & " where accyear=" & ViewState("sYear") & " and kind='2' and no_1_no=" & intNo
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then MessageBx("填入acf020支款號錯誤" & sqlstr)
            '檢查是否為銀行轉帳,要刪除第一項及第二項
            sqlstr = "select accno from acf010 where kind='2' and item='1' and accyear=" & ViewState("sYear") & " and no_1_no=" & intNo
            mydsS = Master.ADO.openmember(DNS_ACC, "acf010t", sqlstr)
            If mydsS.Tables("acf010t").Rows.Count > 0 Then
                sqlstr = Master.ADO.nz(RTrim(mydsS.Tables("acf010t").Rows(0).Item("accno")), "")
                If sqlstr = "11102" Or sqlstr = "11103" Then
                    sqlstr = "delete from acf010 where kind='2' and item='1' and accyear=" & ViewState("sYear") & " and no_1_no=" & intNo
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    sqlstr = "delete from acf020 where kind='2' and item='2' and accyear=" & ViewState("sYear") & " and no_1_no=" & intNo
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                End If
            End If
        Next
        Master.ADO.GenInsSql("accyear", ViewState("sYear"), "N")
        Master.ADO.GenInsSql("kind", "2", "T")
        Master.ADO.GenInsSql("bank", ViewState("sBank"), "T")
        Master.ADO.GenInsSql("chkno", txtChkNo.Text, "T")
        Master.ADO.GenInsSql("date_1", ViewState("sDate"), "D")
        Master.ADO.GenInsSql("date_2", ViewState("sDate"), "D")
        Master.ADO.GenInsSql("chkname", txtChkname.Text, "U")
        Master.ADO.GenInsSql("remark", txtRemark.Text, "U")
        Master.ADO.GenInsSql("amt", lblTotAmt.Text, "N")
        Master.ADO.GenInsSql("start_no", ViewState("StartNo"), "N")
        Master.ADO.GenInsSql("End_no", ViewState("intNo2"), "N")
        Master.ADO.GenInsSql("no_1_no", ViewState("arrayNo"), "T")
        sqlstr = "insert into chf010 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("新增支票寫入chf010失敗,請人工檢查" & sqlstr)
        End If

        '將支票金額加入本日共支欄(chf020->day_pay) & update chkno
        '列印碼清空
        sqlstr = "update chf020 set day_pay = day_pay + " & Master.Models.ValComa(lblTotAmt.Text) & ", prt_code = ' ', date_2 = '" & _
                 Master.Models.strDateChinessToAD(ViewState("sDate")) & "' where bank='" & ViewState("sBank") & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("將支票金額加入本日共支欄(chf020->day_pay)失敗,請檢查" & sqlstr)
        End If

        ViewState("arrayNo") = ""                          '清除該張支票所含之傳票號
        ViewState("intCnt") = 0
        'myDatasetS.Tables("acf010s").Clear()  '清除datagrid1資料


        bm.Clear()
        ViewState("dtgTarget") = bm
        DataGridView.DataSource = bm
        DataGridView.DataBind()

        '更正支款編號acfno 
        sqlstr = "update acfno set cont_no=" & ViewState("intNo2") & " where accyear=" & ViewState("sYear") & " and kind='5'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("更正支款編號acfno錯誤" & sqlstr)

        lblMsg.Text = "記帳完成"
        lblTotAmt.Text = ""
        txtNo1.Text = ""
        txtChkNo.Text = ""
        txtChkname.Text = ""
        txtNo1.Focus()
        btnSure.Enabled = False
        TabContainer1.ActiveTabIndex = 0       '回datagrid PAGE 1 

    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnAddPsname_Click(sender As Object, e As EventArgs) Handles btnAddPsname.Click
        If txtChkname.Text <> "" Then
            txtChkname.Text = Trim(txtChkname.Text)

            sqlstr = "insert into psname (unit, seq, psstr) values ('0403', 0, N'" & txtChkname.Text & "')"
            Master.ADO.dbExecute(DNS_ACC, sqlstr)

        End If
    End Sub

    Protected Sub TabContainer1_ActiveTabChanged(sender As Object, e As EventArgs) Handles TabContainer1.ActiveTabChanged
        If TabContainer1.ActiveTabIndex = 1 Then lblTotamt2.Text = FormatNumber(Master.Models.ValComa(lblTotAmt.Text), 0)
    End Sub

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
        ViewState("sYear") = Mid(ViewState("sDate"), 1, 3)   '年度


        TabPanel1.Visible = True
        TabPanel2.Visible = True
        TabPanel3.Visible = False
        TabContainer1.ActiveTabIndex = 0       '回datagrid PAGE 1 
    End Sub
End Class