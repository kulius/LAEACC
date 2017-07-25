Imports System.Data
Imports System.Data.SqlClient

Public Class PAY021
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

        ViewState("arrayNo") = ""
        ViewState("intCnt") = 0
        ViewState("intChkForm") = 1

        '電子轉帳
        ViewState("blnUseTR") = False  '無使用電子轉帳
        Dim myds As DataSet = Master.ADO.openmember(DNS_ACC, "user", "select * from accname where accno='5'")
        If myds.Tables("user").Rows.Count > 0 Then
            If Master.ADO.nz(myds.Tables("user").Rows(0).Item("bank"), "") = "TR" Then
                ViewState("blnUseTR") = True '有使用電子轉帳
            End If
        End If

        txtNo1.Focus()

        sqlstr = "SELECT *,bank+' '+bankname as bname FROM CHF020 " & _
        " order by bank"

        Master.Controller.objDropDownListOptionEX(cbobank, DNS_ACC, sqlstr, "bank", "bname", 0)

        testChkname.Text = "測試抬頭"
        testAmt.Text = "123456789"
        testRemark.Text = "測試摘要"

        'cbobank.Items.Add(New ListItem("全部", "全部"))
        'cbobank.SelectedIndex = -1
        'cbobank.Items.FindByValue("全部").Selected = True

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


        If Trim(txtNo1.Text) = "" Then
            MessageBx("請輸入傳票")
            txtNo1.Focus()
            Exit Sub
        End If
        If ViewState("arrayNo").IndexOf(Format(Val(txtNo1.Text), "00000")) >= 0 Then
            MessageBx("傳票重複")
            txtNo1.Text = ""
            txtNo1.Focus()
            Exit Sub
        End If
        sqlstr = "select date_2, no_1_no, no_2_no, remark, act_amt, bank, chkno from acf010 where kind='2' and item='9' and accyear=" & _
                  Session("sYear") & " and no_1_no=" & Master.Models.ValComa(Trim(txtNo1.Text))
        mydsS = Master.ADO.openmember(DNS_ACC, "acf010t", sqlstr)
        If mydsS.Tables("acf010t").Rows.Count = 0 Then
            MessageBx("無此傳票")
            txtNo1.Text = ""
            txtNo1.Focus()
            Exit Sub
        End If
        If Not IsDBNull(mydsS.Tables("acf010t").Rows(0).Item("date_2")) Then
            MessageBx("此傳票已入帳," & mydsS.Tables("acf010t").Rows(0).Item("no_2_no") & "號")
            txtNo1.Text = ""
            txtNo1.Focus()
            Exit Sub
        End If
        If Not IsDBNull(mydsS.Tables("acf010t").Rows(0).Item("chkno")) Then
            If Trim(mydsS.Tables("acf010t").Rows(0).Item("chkno")) <> "" Then
                MessageBx("此傳票已開支票" & mydsS.Tables("acf010t").Rows(0).Item("chkno"))
                txtNo1.Text = ""
                txtNo1.Focus()
                Exit Sub
            End If
        End If

        If ViewState("intCnt") > 0 Then   '第二筆以後要檢察銀行要相同
            If mydsS.Tables("acf010t").Rows(0).Item("bank") <> lblBank.Text Then
                MessageBx("銀行不相同,請重輸入傳票" & mydsS.Tables("acf010t").Rows(0).Item("bank"))
                txtNo1.Text = ""
                txtNo1.Focus()
                Exit Sub
            End If
        End If
        lblNo1.Text = mydsS.Tables("acf010t").Rows(0).Item("no_1_no")
        lblBank.Text = mydsS.Tables("acf010t").Rows(0).Item("bank")
        lblAct_Amt.Text = FormatNumber(mydsS.Tables("acf010t").Rows(0).Item("act_amt"), 0)
        lblRemark.Text = Trim(mydsS.Tables("acf010t").Rows(0).Item("remark"))
        Dim intTemp As Integer
        If ViewState("intCnt") = 0 Then    '第一筆
            ViewState("sBank") = lblBank.Text '記錄第一筆銀行以便控制銀行要相同
            intTemp = lblRemark.Text.IndexOf("  ")   '由摘要空二格取受款人
            'If intTemp <= 0 Then intTemp = Len(lblRemark.Text) - 7 '找不到則取最後五個字
            txtRemark.Text = lblRemark.Text
            If intTemp <= 0 Then
                intTemp = Len(lblRemark.Text)
                txtChkname.Text = Microsoft.VisualBasic.Right(lblRemark.Text, IIf(intTemp >= 5, 5, intTemp))
            Else
                txtChkname.Text = Trim(Mid(lblRemark.Text, intTemp + 2))      '由摘要取受款人
            End If
            lblMsg.Text = ""
            sqlstr = "SELECT * FROM chf020 WHERE bank = '" & lblBank.Text & "'"
            mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
            If mydataset.Tables("chf020").Rows.Count > 0 Then
                With mydataset.Tables("chf020").Rows(0)
                    lblBankName.Text = .Item("bank") & .Item("bankname")
                    '帳戶餘額
                    lblBalance.Text = FormatNumber(.Item("balance") + .Item("day_income") - .Item("day_pay") - .Item("unpay") + .Item("credit"), 2)
                    '支票號+1
                    txtChkNo.Text = Master.ADO.nz(.Item("chkno"), " ")
                    txtChkNo.Text = Master.Controller.AddCheckNo(txtChkNo.Text)
                    ViewState("intChkForm") = Master.ADO.nz(.Item("chkform"), 1)  '未定支票格式者則給土銀格式
                End With
            End If
            btnSure.Visible = True
            btnFinish.Visible = True
            mydataset = Nothing

            '電子 add
            If ViewState("blnUseTR") = True Then
                btnSureTR.Visible = True   '顯示"開立電子支票"button 
            End If

        End If

        If Master.Models.ValComa(lblBalance.Text) < Master.Models.ValComa(lblTotAmt.Text) + Master.Models.ValComa(lblAct_Amt.Text) Then
            MessageBx("存款不足,存款餘額:" & lblBalance.Text & ",不足支付" & (Master.Models.ValComa(lblTotAmt.Text) + Master.Models.ValComa(lblAct_Amt.Text)))
            txtNo1.Text = ""
            txtNo1.Focus()
            Exit Sub
        End If
        Call AddDataGrid()
        txtNo1.Text = ""
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

    Protected Sub btnSureTR_Click(sender As Object, e As EventArgs) Handles btnSureTR.Click
        lblTotamt2.Text = FormatNumber(Master.Models.ValComa(lblTotAmt.Text), 0)
        TabContainer1.ActiveTabIndex = 1

        ViewState("strCheckForm") = "PAPER"  '電子轉帳 add
        lblChkTR.Text = ""      '電子轉帳 add

        ViewState("strCheckForm") = "TRANS"  '再設定為TRANS
        lblChkTR.Text = "本支票採電子轉帳付款,支票號由系統自動編號"
        txtChkNo.Text = "TR" & Format(CInt(Session("sYear")), "000") & Format(Master.Controller.QueryNO(CInt(Session("sYear")), "T") + 1, "00000") '取ACFNO KIND='T' 號數 TRyyyXXXXX
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        If lblTotAmt.Text = "" Or ViewState("intCnt") = 0 Then   '金額=0 or 沒有傳票
            TabContainer1.ActiveTabIndex = 0
            Exit Sub
        End If
        Dim intI, intNo As Integer
        Dim Bankbalance As Decimal
        Dim tempdataset As DataSet

        '電子轉帳 add 
        If ViewState("strCheckForm") = "TRANS" Then
            '真正取轉帳支票號
            txtChkNo.Text = "TR" & Format(CInt(Session("sYear")), "000") & Format(Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "T"), "00000")  '取ACFNO KIND='T' 號數 TRyyyXXXXX
        End If

        '檢查支票碼重複
        sqlstr = "SELECT chkno FROM chf010 where kind='2' and chkno='" & txtChkNo.Text & "' and accyear=" & Session("sYear")
        tempdataset = Master.ADO.openmember(DNS_ACC, "chf010", sqlstr)
        If tempdataset.Tables("chf010").Rows.Count > 0 Then
            MessageBx("支票碼重複")
            Exit Sub
        End If

        ' 新增一筆資料至chf010
        Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
        Master.ADO.GenInsSql("kind", "2", "T")
        Master.ADO.GenInsSql("bank", ViewState("sBank"), "T")
        Master.ADO.GenInsSql("chkno", txtChkNo.Text, "T")
        Master.ADO.GenInsSql("date_1", Session("UserDate"), "D")
        Master.ADO.GenInsSql("chkname", txtChkname.Text, "U")
        Master.ADO.GenInsSql("remark", txtRemark.Text, "U")
        Master.ADO.GenInsSql("amt", lblTotAmt.Text, "N")
        Master.ADO.GenInsSql("no_1_no", ViewState("arrayNo"), "T")
        sqlstr = "insert into chf010 " & Master.ADO.GenInsFunc
        Dim retstr As String
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("新增支票寫入chf010失敗,請檢查" & sqlstr)
            Exit Sub
        End If

        tempdataset = Nothing

        '將支票號碼寫入acf010->chkno  item='9'  
        For intI = 1 To ViewState("intCnt")    '傳票張數
            intNo = Val(Mid(ViewState("arrayNo"), (intI - 1) * 6 + 1, 5))
            If intNo = 0 Then Exit For
            sqlstr = "update acf010 set chkno='" & txtChkNo.Text & "', chkseq=" & intI & " where accyear=" & Session("sYear") & _
                     " and no_1_no=" & intNo & " and kind='2' and item='9'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("支票號碼寫入acf010->chkno失敗,請檢查" & sqlstr)
            End If
        Next

        '將支票金額加入已開未領欄(chf020->unpay) & update chkno

        '電子轉帳 add & mod
        If ViewState("strCheckForm") = "TRANS" Then
            sqlstr = "update chf020 set unpay = unpay + '" & Master.Models.ValComa(lblTotAmt.Text) & "' where bank='" & ViewState("sBank") & "'"
        Else
            sqlstr = "update chf020 set unpay = unpay + '" & Master.Models.ValComa(lblTotAmt.Text) & "', chkno = '" & _
                      txtChkNo.Text & "' where bank='" & ViewState("sBank") & "'"
        End If
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MsgBox("將支票金額加入已開未領欄(chf020->unpay)失敗,請檢查" & sqlstr)
        End If
        Bankbalance = Master.Models.ValComa(lblBalance.Text) - Master.Models.ValComa(lblTotAmt.Text)

        '電子轉帳 add & mod
        If ViewState("strCheckForm") = "TRANS" Then
            ' 新增一筆資料至chf050
            Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
            Master.ADO.GenInsSql("vchkno", txtChkNo.Text, "T")
            Master.ADO.GenInsSql("date_1", Session("UserDate"), "D")
            Master.ADO.GenInsSql("bank", ViewState("sBank"), "T")
            sqlstr = "insert into chf050 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("新增支票寫入chf050失敗,請檢查" & sqlstr)
                Exit Sub
            End If
        Else   '原程式
            '列印支票
            ' PrintCheck(sBank, lblBankName.Text, sDate, txtChkname.Text, ValComa(lblTotAmt.Text), txtChkNo.Text & txtRemark.Text, Bankbalance) '列印支票 at vbdataio.vb
            Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
            Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
            Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
            Param.Add("UserId", Session("UserId"))    '使用者代號

            sqlstr = "SELECT bank FROM CHFPRINT where bank='" & ViewState("sBank") & "'"
            Dim tempdataset1 As DataSet
            tempdataset1 = Master.ADO.openmember(DNS_ACC, "CHFPRINT", sqlstr)
            If tempdataset1.Tables("CHFPRINT").Rows.Count > 0 Then
                Param.Add("Bank", ViewState("sBank"))
            Else
                Param.Add("Bank", "00")
            End If

            'Param.Add("Bank", ViewState("sBank"))
            Param.Add("Bankname", lblBankName.Text)

            Param.Add("Chkname", txtChkname.Text)
            Param.Add("Amt", lblTotAmt.Text)
            Param.Add("Remark", txtChkNo.Text & txtRemark.Text)
            Param.Add("BankBalance", Bankbalance)

            If ckprint3.Checked Then
                Param.Add("sDate", txtDate.Text)
            Else
                Param.Add("sDate", Session("UserDate"))
            End If

            Param.Add("ckprint1", IIf(ckprint1.Checked = True, "1", "0"))
            Param.Add("ckprint2", IIf(ckprint2.Checked = True, "1", "0"))
            Param.Add("ckprint3", IIf(ckprint3.Checked = True, "1", "0"))

            Param.Add("sSeason", Session("sSeason"))    '第幾季
            Param.Add("UserDate", Session("UserDate"))    '登入日期

            Master.PrintFR("PAY021支票套印", Session("ORG"), DNS_ACC, Param)
        End If


        lblMsg.Text = " 上張支票" & txtChkNo.Text & " 處理完畢"
        ViewState("arrayNo") = ""                          '清除該張支票所含之傳票號

        Dim dtCurrentTable As DataTable
        Dim drCurrentRow As DataRow = Nothing
        If ViewState("dtgTarget") IsNot Nothing Then
            dtCurrentTable = DirectCast(ViewState("dtgTarget"), DataTable)
            dtCurrentTable.Clear()
        Else
            MessageBx("ViewState無內容")
            Exit Sub
        End If

        ViewState("dtgTarget") = dtCurrentTable
        DataGridView.DataSource = dtCurrentTable
        DataGridView.DataBind()

        lblTotAmt.Text = ""
        txtChkNo.Text = ""
        btnSure.Visible = False
        btnSureTR.Visible = False '電子轉帳  add 
        btnFinish.Visible = False
        ViewState("intCnt") = 0       '傳票筆數=0
        lblTotNo.Text = ""
        TabContainer1.ActiveTabIndex = 0         '回datagrid PAGE 1

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

    Protected Sub btnPrintSave_Click(sender As Object, e As EventArgs) Handles btnPrintSave.Click
        Master.ADO.GenUpdsql("公司全銜", 公司全銜.Text, "T")
        Master.ADO.GenUpdsql("中文字型名稱", 中文字型名稱.Text, "T")
        Master.ADO.GenUpdsql("左側日期X", 左側日期X.Text, "T")
        Master.ADO.GenUpdsql("左側日期Y", 左側日期Y.Text, "T")
        Master.ADO.GenUpdsql("左側日期W", 左側日期W.Text, "T")
        Master.ADO.GenUpdsql("左側日期H", 左側日期H.Text, "T")
        Master.ADO.GenUpdsql("左側日期字體大小", 左側日期字體大小.Text, "T")
        Master.ADO.GenUpdsql("左側日期要列印", 左側日期要列印.Text, "T")
        Master.ADO.GenUpdsql("左側受款人X", 左側受款人X.Text, "T")
        Master.ADO.GenUpdsql("左側受款人Y", 左側受款人Y.Text, "T")
        Master.ADO.GenUpdsql("左側受款人W", 左側受款人W.Text, "T")
        Master.ADO.GenUpdsql("左側受款人H", 左側受款人H.Text, "T")
        Master.ADO.GenUpdsql("左側受款人字體大小", 左側受款人字體大小.Text, "T")
        Master.ADO.GenUpdsql("左側受款人要列印", 左側受款人要列印.Text, "T")
        Master.ADO.GenUpdsql("左側金額X", 左側金額X.Text, "T")
        Master.ADO.GenUpdsql("左側金額Y", 左側金額Y.Text, "T")
        Master.ADO.GenUpdsql("左側金額W", 左側金額W.Text, "T")
        Master.ADO.GenUpdsql("左側金額H", 左側金額H.Text, "T")
        Master.ADO.GenUpdsql("左側金額字體大小", 左側金額字體大小.Text, "T")
        Master.ADO.GenUpdsql("左側金額要列印", 左側金額要列印.Text, "T")
        Master.ADO.GenUpdsql("左側用途X", 左側用途X.Text, "T")
        Master.ADO.GenUpdsql("左側用途Y", 左側用途Y.Text, "T")
        Master.ADO.GenUpdsql("左側用途W", 左側用途W.Text, "T")
        Master.ADO.GenUpdsql("左側用途H", 左側用途H.Text, "T")
        Master.ADO.GenUpdsql("左側用途字體大小", 左側用途字體大小.Text, "T")
        Master.ADO.GenUpdsql("左側用途要列印", 左側用途要列印.Text, "T")
        Master.ADO.GenUpdsql("左側餘額X", 左側餘額X.Text, "T")
        Master.ADO.GenUpdsql("左側餘額Y", 左側餘額Y.Text, "T")
        Master.ADO.GenUpdsql("左側餘額W", 左側餘額W.Text, "T")
        Master.ADO.GenUpdsql("左側餘額H", 左側餘額H.Text, "T")
        Master.ADO.GenUpdsql("左側餘額字體大小", 左側餘額字體大小.Text, "T")
        Master.ADO.GenUpdsql("左側餘額要列印", 左側餘額要列印.Text, "T")
        Master.ADO.GenUpdsql("右側受款人X", 右側受款人X.Text, "T")
        Master.ADO.GenUpdsql("右側受款人Y", 右側受款人Y.Text, "T")
        Master.ADO.GenUpdsql("右側受款人W", 右側受款人W.Text, "T")
        Master.ADO.GenUpdsql("右側受款人H", 右側受款人H.Text, "T")
        Master.ADO.GenUpdsql("右側受款人字體大小", 右側受款人字體大小.Text, "T")
        Master.ADO.GenUpdsql("右側受款人要列印", 右側受款人要列印.Text, "T")
        Master.ADO.GenUpdsql("右側受款人字數臨界點", 右側受款人字數臨界點.Text, "T")
        Master.ADO.GenUpdsql("右側受款人字體大小臨界點", 右側受款人字體大小臨界點.Text, "T")
        Master.ADO.GenUpdsql("右側金額X", 右側金額X.Text, "T")
        Master.ADO.GenUpdsql("右側金額Y", 右側金額Y.Text, "T")
        Master.ADO.GenUpdsql("右側金額W", 右側金額W.Text, "T")
        Master.ADO.GenUpdsql("右側金額H", 右側金額H.Text, "T")
        Master.ADO.GenUpdsql("右側金額字體大小", 右側金額字體大小.Text, "T")
        Master.ADO.GenUpdsql("右側金額要列印", 右側金額要列印.Text, "T")
        Master.ADO.GenUpdsql("右側金額字數臨界點", 右側金額字數臨界點.Text, "T")
        Master.ADO.GenUpdsql("右側金額字體大小臨界點", 右側金額字體大小臨界點.Text, "T")
        Master.ADO.GenUpdsql("右側金額格子精確度", 右側金額格子精確度.Text, "T")
        Master.ADO.GenUpdsql("右側中文金額X", 右側中文金額X.Text, "T")
        Master.ADO.GenUpdsql("右側中文金額Y", 右側中文金額Y.Text, "T")
        Master.ADO.GenUpdsql("右側中文金額W", 右側中文金額W.Text, "T")
        Master.ADO.GenUpdsql("右側中文金額H", 右側中文金額H.Text, "T")
        Master.ADO.GenUpdsql("右側中文金額字體大小", 右側中文金額字體大小.Text, "T")
        Master.ADO.GenUpdsql("右側中文金額要列印", 右側中文金額要列印.Text, "T")
        Master.ADO.GenUpdsql("右側中文金額字數臨界點", 右側中文金額字數臨界點.Text, "T")
        Master.ADO.GenUpdsql("右側中文金額字體大小臨界點", 右側中文金額字體大小臨界點.Text, "T")
        Master.ADO.GenUpdsql("右側日期X", 右側日期X.Text, "T")
        Master.ADO.GenUpdsql("右側日期Y", 右側日期Y.Text, "T")
        Master.ADO.GenUpdsql("右側日期W", 右側日期W.Text, "T")
        Master.ADO.GenUpdsql("右側日期H", 右側日期H.Text, "T")
        Master.ADO.GenUpdsql("右側日期字體大小", 右側日期字體大小.Text, "T")
        Master.ADO.GenUpdsql("右側日期要列印", 右側日期要列印.Text, "T")
        Master.ADO.GenUpdsql("禁止背書轉讓X", 禁止背書轉讓X.Text, "T")
        Master.ADO.GenUpdsql("禁止背書轉讓Y", 禁止背書轉讓Y.Text, "T")
        Master.ADO.GenUpdsql("禁止背書轉讓W", 禁止背書轉讓W.Text, "T")
        Master.ADO.GenUpdsql("禁止背書轉讓H", 禁止背書轉讓H.Text, "T")
        Master.ADO.GenUpdsql("禁止背書轉讓字體大小", 禁止背書轉讓字體大小.Text, "T")
        Master.ADO.GenUpdsql("禁止背書轉讓要列印", 禁止背書轉讓要列印.Text, "T")
        Master.ADO.GenUpdsql("禁止背書轉讓樣式", 禁止背書轉讓樣式.Text, "T")
        Master.ADO.GenUpdsql("雙斜線X", 雙斜線X.Text, "T")
        Master.ADO.GenUpdsql("雙斜線Y", 雙斜線Y.Text, "T")
        Master.ADO.GenUpdsql("雙斜線W", 雙斜線W.Text, "T")
        Master.ADO.GenUpdsql("雙斜線H", 雙斜線H.Text, "T")
        Master.ADO.GenUpdsql("雙斜線字體大小", 雙斜線字體大小.Text, "T")
        Master.ADO.GenUpdsql("雙斜線要列印", 雙斜線要列印.Text, "T")
        Master.ADO.GenUpdsql("公司全銜X", 公司全銜X.Text, "T")
        Master.ADO.GenUpdsql("公司全銜Y", 公司全銜Y.Text, "T")
        Master.ADO.GenUpdsql("公司全銜W", 公司全銜W.Text, "T")
        Master.ADO.GenUpdsql("公司全銜H", 公司全銜H.Text, "T")
        Master.ADO.GenUpdsql("公司全銜字體大小", 公司全銜字體大小.Text, "T")
        Master.ADO.GenUpdsql("公司全銜要列印", 公司全銜要列印.Text, "T")

        sqlstr = "update CHFPRINT set " & Master.ADO.genupdfunc & " where bank='" & cbobank.SelectedValue & "'"

        Master.ADO.runsql(DNS_ACC, sqlstr)
    End Sub

    Protected Sub btnPrintTes_Click(sender As Object, e As EventArgs) Handles btnPrintTes.Click
        

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號

        Param.Add("Bank", cbobank.SelectedValue)
        Param.Add("Bankname", cbobank.Text)
        Param.Add("sDate", Session("UserDate"))
        Param.Add("Chkname", testChkname.Text)
        Param.Add("Amt", testAmt.Text)
        Param.Add("Remark", testRemark.Text)
        Param.Add("BankBalance", testAmt.Text)

        Param.Add("ckprint1", IIf(ckprint1.Checked, 1, 0))
        Param.Add("ckprint2", IIf(ckprint2.Checked, 1, 0))
        Param.Add("ckprint3", IIf(ckprint3.Checked, 1, 0))

        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        Master.PrintFR("PAY021支票套印", Session("ORG"), DNS_ACC, Param)
    End Sub

    Protected Sub cbobank_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbobank.SelectedIndexChanged
        sqlstr = "select * from CHFPRINT where bank='" & cbobank.SelectedValue & "'"
        mydataset = Master.ADO.openmember(DNS_ACC, "CHFPRINT", sqlstr)
        If mydataset.Tables("CHFPRINT").Rows.Count = 0 Then
            btnPrintSerach_Click(sender, e)
        End If


    End Sub

    Protected Sub btnPrintSerach_Click(sender As Object, e As EventArgs) Handles btnPrintSerach.Click
        Dim msave As Boolean = False
        sqlstr = "select * from CHFPRINT where bank='" & cbobank.SelectedValue & "'"
        mydataset = Master.ADO.openmember(DNS_ACC, "CHFPRINT", sqlstr)
        If mydataset.Tables("CHFPRINT").Rows.Count = 0 Then
            Master.ADO.GenInsSql("bank", cbobank.SelectedValue, "T")
            sqlstr = "insert into CHFPRINT " & Master.ADO.GenInsFunc
            Master.ADO.runsql(DNS_ACC, sqlstr)

            sqlstr = "select * from CHFPRINT where bank='00'"
            mydataset = Master.ADO.openmember(DNS_ACC, "CHFPRINT", sqlstr)

            msave = True
        End If

        公司全銜.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("公司全銜")
        中文字型名稱.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("中文字型名稱")

        左側日期X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側日期X")
        左側日期Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側日期Y")
        左側日期W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側日期W")
        左側日期H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側日期H")
        左側日期字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側日期字體大小")
        左側日期要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側日期要列印")
        左側受款人X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側受款人X")
        左側受款人Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側受款人Y")
        左側受款人W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側受款人W")
        左側受款人H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側受款人H")
        左側受款人字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側受款人字體大小")
        左側受款人要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側受款人要列印")
        左側金額X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側金額X")
        左側金額Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側金額Y")
        左側金額W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側金額W")
        左側金額H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側金額H")
        左側金額字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側金額字體大小")
        左側金額要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側金額要列印")
        左側用途X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側用途X")
        左側用途Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側用途Y")
        左側用途W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側用途W")
        左側用途H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側用途H")
        左側用途字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側用途字體大小")
        左側用途要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側用途要列印")
        左側餘額X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側餘額X")
        左側餘額Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側餘額Y")
        左側餘額W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側餘額W")
        左側餘額H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側餘額H")
        左側餘額字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側餘額字體大小")
        左側餘額要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("左側餘額要列印")
        右側受款人X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側受款人X")
        右側受款人Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側受款人Y")
        右側受款人W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側受款人W")
        右側受款人H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側受款人H")
        右側受款人字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側受款人字體大小")
        右側受款人要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側受款人要列印")
        右側受款人字數臨界點.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側受款人字數臨界點")
        右側受款人字體大小臨界點.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側受款人字體大小臨界點")
        右側金額X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額X")
        右側金額Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額Y")
        右側金額W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額W")
        右側金額H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額H")
        右側金額字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額字體大小")
        右側金額要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額要列印")
        右側金額字數臨界點.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額字數臨界點")
        右側金額字體大小臨界點.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額字體大小臨界點")
        右側金額格子精確度.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側金額格子精確度")
        右側中文金額X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側中文金額X")
        右側中文金額Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側中文金額Y")
        右側中文金額W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側中文金額W")
        右側中文金額H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側中文金額H")
        右側中文金額字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側中文金額字體大小")
        右側中文金額要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側中文金額要列印")
        右側中文金額字數臨界點.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側中文金額字數臨界點")
        右側中文金額字體大小臨界點.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側中文金額字體大小臨界點")
        右側日期X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側日期X")
        右側日期Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側日期Y")
        右側日期W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側日期W")
        右側日期H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側日期H")
        右側日期字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側日期字體大小")
        右側日期要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("右側日期要列印")

        禁止背書轉讓X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("禁止背書轉讓X")
        禁止背書轉讓Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("禁止背書轉讓Y")
        禁止背書轉讓W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("禁止背書轉讓W")
        禁止背書轉讓H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("禁止背書轉讓H")
        禁止背書轉讓字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("禁止背書轉讓字體大小")
        禁止背書轉讓要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("禁止背書轉讓要列印")
        禁止背書轉讓樣式.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("禁止背書轉讓樣式")
        雙斜線X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("雙斜線X")
        雙斜線Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("雙斜線Y")
        雙斜線W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("雙斜線W")
        雙斜線H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("雙斜線H")
        雙斜線字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("雙斜線字體大小")
        雙斜線要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("雙斜線要列印")
        公司全銜X.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("公司全銜X")
        公司全銜Y.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("公司全銜Y")
        公司全銜W.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("公司全銜W")
        公司全銜H.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("公司全銜H")
        公司全銜字體大小.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("公司全銜字體大小")
        公司全銜要列印.Text = mydataset.Tables("CHFPRINT").Rows(0).Item("公司全銜要列印")



        If msave Then
            Master.ADO.GenUpdsql("公司全銜", 公司全銜.Text, "T")
            Master.ADO.GenUpdsql("中文字型名稱", 中文字型名稱.Text, "T")
            Master.ADO.GenUpdsql("左側日期X", 左側日期X.Text, "T")
            Master.ADO.GenUpdsql("左側日期Y", 左側日期Y.Text, "T")
            Master.ADO.GenUpdsql("左側日期W", 左側日期W.Text, "T")
            Master.ADO.GenUpdsql("左側日期H", 左側日期H.Text, "T")
            Master.ADO.GenUpdsql("左側日期字體大小", 左側日期字體大小.Text, "T")
            Master.ADO.GenUpdsql("左側日期要列印", 左側日期要列印.Text, "T")
            Master.ADO.GenUpdsql("左側受款人X", 左側受款人X.Text, "T")
            Master.ADO.GenUpdsql("左側受款人Y", 左側受款人Y.Text, "T")
            Master.ADO.GenUpdsql("左側受款人W", 左側受款人W.Text, "T")
            Master.ADO.GenUpdsql("左側受款人H", 左側受款人H.Text, "T")
            Master.ADO.GenUpdsql("左側受款人字體大小", 左側受款人字體大小.Text, "T")
            Master.ADO.GenUpdsql("左側受款人要列印", 左側受款人要列印.Text, "T")
            Master.ADO.GenUpdsql("左側金額X", 左側金額X.Text, "T")
            Master.ADO.GenUpdsql("左側金額Y", 左側金額Y.Text, "T")
            Master.ADO.GenUpdsql("左側金額W", 左側金額W.Text, "T")
            Master.ADO.GenUpdsql("左側金額H", 左側金額H.Text, "T")
            Master.ADO.GenUpdsql("左側金額字體大小", 左側金額字體大小.Text, "T")
            Master.ADO.GenUpdsql("左側金額要列印", 左側金額要列印.Text, "T")
            Master.ADO.GenUpdsql("左側用途X", 左側用途X.Text, "T")
            Master.ADO.GenUpdsql("左側用途Y", 左側用途Y.Text, "T")
            Master.ADO.GenUpdsql("左側用途W", 左側用途W.Text, "T")
            Master.ADO.GenUpdsql("左側用途H", 左側用途H.Text, "T")
            Master.ADO.GenUpdsql("左側用途字體大小", 左側用途字體大小.Text, "T")
            Master.ADO.GenUpdsql("左側用途要列印", 左側用途要列印.Text, "T")
            Master.ADO.GenUpdsql("左側餘額X", 左側餘額X.Text, "T")
            Master.ADO.GenUpdsql("左側餘額Y", 左側餘額Y.Text, "T")
            Master.ADO.GenUpdsql("左側餘額W", 左側餘額W.Text, "T")
            Master.ADO.GenUpdsql("左側餘額H", 左側餘額H.Text, "T")
            Master.ADO.GenUpdsql("左側餘額字體大小", 左側餘額字體大小.Text, "T")
            Master.ADO.GenUpdsql("左側餘額要列印", 左側餘額要列印.Text, "T")
            Master.ADO.GenUpdsql("右側受款人X", 右側受款人X.Text, "T")
            Master.ADO.GenUpdsql("右側受款人Y", 右側受款人Y.Text, "T")
            Master.ADO.GenUpdsql("右側受款人W", 右側受款人W.Text, "T")
            Master.ADO.GenUpdsql("右側受款人H", 右側受款人H.Text, "T")
            Master.ADO.GenUpdsql("右側受款人字體大小", 右側受款人字體大小.Text, "T")
            Master.ADO.GenUpdsql("右側受款人要列印", 右側受款人要列印.Text, "T")
            Master.ADO.GenUpdsql("右側受款人字數臨界點", 右側受款人字數臨界點.Text, "T")
            Master.ADO.GenUpdsql("右側受款人字體大小臨界點", 右側受款人字體大小臨界點.Text, "T")
            Master.ADO.GenUpdsql("右側金額X", 右側金額X.Text, "T")
            Master.ADO.GenUpdsql("右側金額Y", 右側金額Y.Text, "T")
            Master.ADO.GenUpdsql("右側金額W", 右側金額W.Text, "T")
            Master.ADO.GenUpdsql("右側金額H", 右側金額H.Text, "T")
            Master.ADO.GenUpdsql("右側金額字體大小", 右側金額字體大小.Text, "T")
            Master.ADO.GenUpdsql("右側金額要列印", 右側金額要列印.Text, "T")
            Master.ADO.GenUpdsql("右側金額字數臨界點", 右側金額字數臨界點.Text, "T")
            Master.ADO.GenUpdsql("右側金額字體大小臨界點", 右側金額字體大小臨界點.Text, "T")
            Master.ADO.GenUpdsql("右側金額格子精確度", 右側金額格子精確度.Text, "T")
            Master.ADO.GenUpdsql("右側中文金額X", 右側中文金額X.Text, "T")
            Master.ADO.GenUpdsql("右側中文金額Y", 右側中文金額Y.Text, "T")
            Master.ADO.GenUpdsql("右側中文金額W", 右側中文金額W.Text, "T")
            Master.ADO.GenUpdsql("右側中文金額H", 右側中文金額H.Text, "T")
            Master.ADO.GenUpdsql("右側中文金額字體大小", 右側中文金額字體大小.Text, "T")
            Master.ADO.GenUpdsql("右側中文金額要列印", 右側中文金額要列印.Text, "T")
            Master.ADO.GenUpdsql("右側中文金額字數臨界點", 右側中文金額字數臨界點.Text, "T")
            Master.ADO.GenUpdsql("右側中文金額字體大小臨界點", 右側中文金額字體大小臨界點.Text, "T")
            Master.ADO.GenUpdsql("右側日期X", 右側日期X.Text, "T")
            Master.ADO.GenUpdsql("右側日期Y", 右側日期Y.Text, "T")
            Master.ADO.GenUpdsql("右側日期W", 右側日期W.Text, "T")
            Master.ADO.GenUpdsql("右側日期H", 右側日期H.Text, "T")
            Master.ADO.GenUpdsql("右側日期字體大小", 右側日期字體大小.Text, "T")
            Master.ADO.GenUpdsql("右側日期要列印", 右側日期要列印.Text, "T")
            Master.ADO.GenUpdsql("禁止背書轉讓X", 禁止背書轉讓X.Text, "T")
            Master.ADO.GenUpdsql("禁止背書轉讓Y", 禁止背書轉讓Y.Text, "T")
            Master.ADO.GenUpdsql("禁止背書轉讓W", 禁止背書轉讓W.Text, "T")
            Master.ADO.GenUpdsql("禁止背書轉讓H", 禁止背書轉讓H.Text, "T")
            Master.ADO.GenUpdsql("禁止背書轉讓字體大小", 禁止背書轉讓字體大小.Text, "T")
            Master.ADO.GenUpdsql("禁止背書轉讓要列印", 禁止背書轉讓要列印.Text, "T")
            Master.ADO.GenUpdsql("禁止背書轉讓樣式", 禁止背書轉讓樣式.Text, "T")
            Master.ADO.GenUpdsql("雙斜線X", 雙斜線X.Text, "T")
            Master.ADO.GenUpdsql("雙斜線Y", 雙斜線Y.Text, "T")
            Master.ADO.GenUpdsql("雙斜線W", 雙斜線W.Text, "T")
            Master.ADO.GenUpdsql("雙斜線H", 雙斜線H.Text, "T")
            Master.ADO.GenUpdsql("雙斜線字體大小", 雙斜線字體大小.Text, "T")
            Master.ADO.GenUpdsql("雙斜線要列印", 雙斜線要列印.Text, "T")
            Master.ADO.GenUpdsql("公司全銜X", 公司全銜X.Text, "T")
            Master.ADO.GenUpdsql("公司全銜Y", 公司全銜Y.Text, "T")
            Master.ADO.GenUpdsql("公司全銜W", 公司全銜W.Text, "T")
            Master.ADO.GenUpdsql("公司全銜H", 公司全銜H.Text, "T")
            Master.ADO.GenUpdsql("公司全銜字體大小", 公司全銜字體大小.Text, "T")
            Master.ADO.GenUpdsql("公司全銜要列印", 公司全銜要列印.Text, "T")

            sqlstr = "update CHFPRINT set " & Master.ADO.genupdfunc & " where bank='" & cbobank.SelectedValue & "'"

            Master.ADO.runsql(DNS_ACC, sqlstr)
        End If
    End Sub
End Class