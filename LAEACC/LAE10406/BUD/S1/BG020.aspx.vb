Imports System.Data
Imports System.Data.SqlClient

Public Class BG020
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

    Dim strBGNo As String    '請購編號
    Dim sYear, lastpos, sSeason As Integer  '請購年度
    Dim UserId, UserName, UserUnit, UserDate As String
    Dim direty As Boolean = False '定義直接輸入請購編號
    Dim TempDataSet As DataSet
    Dim sqlstr As String

#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
        Master.Controller.objDataGridStyle(DataGrid0, "S")



        UCBase1.Visible = False

        'Focus*****
        TabContainer1.ActiveTabIndex = 1 '指定Tab頁籤
        txtNO.Focus()

        UserId = Session("USERID")
        UserDate = Session("DATE")
        sYear = Session("sYear")
        sSeason = Session("sSeason")
        lblNoYear.Text = Format(sYear, "000")
        If Session("UnitTitle").indexof("中") >= 0 Then
            direty = True  '台中要直接輸入請購編號
        End If
        If Session("UnitTitle").indexof("嘉南") < 0 Then
            lblMark.Visible = False    '嘉南才要顯示憑證補辦攔位
            txtMark.Visible = False
            cboMark.Visible = False
        Else
            '將憑證補辦片語置combobox   
            sqlstr = "SELECT psstr  FROM psname where unit='mark' order by seq"
            Master.Controller.objDropDownListOptionEX(cboMark, DNS_ACC, sqlstr, "psstr", "psstr", 0)
        End If
        If direty = False Then
            LoadGrid0Func()
        End If
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
        Master.Controller.objDataGridStyle(DataGrid0, "S")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            '其他預設值*****
            SetControls(0) '設定所有輸入控制項的唯讀狀態

            '資料查詢*****
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        End If
    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub

    'UserControl控制項
    Protected Sub UCBase1_Click(ByVal sender As Object, ByVal e As UCBase.ClickEventArgs) Handles UCBase1.Click
        'ViewState("MyStatus")：目前按鍵狀態
        Select Case e.CommandName
            Case "First"      '第一筆
                FlagMoveSeat(1, 0)
            Case "Prior"      '上一筆
                FlagMoveSeat(2, ViewState("ItemIndex"))
            Case "Next"       '下一筆
                FlagMoveSeat(3, ViewState("ItemIndex"))
            Case "Last"       '最末筆
                FlagMoveSeat(4, DataGridView.Items.Count - 1)

            Case "Save"       '存檔
                SaveData(ViewState("MyStatus"))
            Case "CancelEdit" '還原
                ResetData()
            Case "Copy"       '複製
                ViewState("MyStatus") = 1
                SetControls(3)
            Case "AddNew"     '新增       
                ViewState("MyStatus") = 1
                SetControls(1)
            Case "Edit"       '修改
                ViewState("MyStatus") = 2
                SetControls(2)
            Case "Delete"     '刪除
                DeleteData()
        End Select
    End Sub
#End Region
#Region "@DataGridView@"
    '點選資訊
    Sub DataGridView_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGridView.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號


        Select Case e.CommandName
            Case "btnShow"
                '查詢資料及控制*****
                FindData(txtID.Text)               '查詢主檔
                FlagMoveSeat(0, e.Item.ItemIndex)
                TabContainer1.ActiveTabIndex = 2   '指定Tab頁籤
        End Select
    End Sub
    '資料分頁
    Sub DataGridView_PageIndexChanged(ByVal sender As Object, ByVal e As DataGridPageChangedEventArgs) Handles DataGridView.PageIndexChanged
        DataGridView.CurrentPageIndex = e.NewPageIndex

        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '資料查詢
    End Sub
    '自選排序
    Sub DataGridView_SortCommand(ByVal sender As Object, ByVal e As DataGridSortCommandEventArgs) Handles DataGridView.SortCommand
        ViewState("MyOrder") = e.SortExpression
        ViewState("MySort") = IIf(ViewState("MySort") = "" Or ViewState("MySort") = "ASC", "DESC", "ASC")

        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '資料查詢
    End Sub
#End Region
#Region "@DataGrid0@"
    Sub LoadGrid0Func()
        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT a.*, " & _
                 "CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')' " & _
                     " WHEN len(a.accno)<>17 THEN b.accname END AS accname " & _
                 "FROM (select accyear, accno FROM BGF020 WHERE closemark <> 'Y' AND date2 IS NULL " & _
                 "GROUP BY accyear, accno) a " & _
                 "LEFT OUTER JOIN ACCNAME b ON a.ACCNO = b.ACCNO " & _
                 " LEFT OUTER JOIN accname c ON left(a.accno,16)=c.accno and len(a.accno)=17 " & _
                 "WHERE (b.ACCOUNT_NO = '" & Trim(UserId) & "' OR b.ACCOUNT_NO = '')" & _
                 "ORDER BY  a.ACCYEAR, a.ACCNO"
        Master.Controller.objDataGrid(DataGrid0, lbl_DataGrid0Count, DNS_ACC, sqlstr, "查詢資料檔")

        TabContainer1.ActiveTabIndex = 0

        txtNO.Text = ""
        txtNO.Focus()
    End Sub
    '點選資訊
    Sub DataGrid0_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid0.ItemCommand
        '關鍵值*****
        bm0accno = e.Item.FindControl("id") '記錄編號
        bm0accyear = e.Item.FindControl("年度") '記錄編號

        ViewState("bm0accno") = bm0accno.Text
        ViewState("bm0accyear") = bm0accyear.Text

        Select Case e.CommandName
            Case "btnShow1"

                FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '資料查詢
                TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
        End Select
    End Sub
#End Region

#Region "按鍵選項"

#End Region

#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, Optional ByVal strSortType As String = "ASC", Optional ByVal strSearch As String = "")
        '開啟查詢
        If direty = True Then   '直接輸入請購編號 要全部置datagrid 
            sqlstr = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.amt1, a.useableamt, a.remark, a.kind,a.subject," & _
                     "CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')' " & _
                         " WHEN len(a.accno)<>17 THEN b.accname END AS accname " & _
                     "FROM BGF020 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO" & _
                     " LEFT OUTER JOIN accname c ON left(a.accno,16)=c.accno and len(a.accno)=17" & _
                     " WHERE (b.account_NO = '" & Trim(UserId) & "' OR b.account_NO = '')" & _
                     "AND a.CLOSEMARK <> 'Y' and a.date2 is null" & _
                     strSearch & _
                     " ORDER BY a.accno, a.bgno"
        Else   '要分科目置datagrid 
            sqlstr = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.amt1, a.useableamt, a.remark, a.kind,a.subject,"
            sqlstr &= "CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')'"
            sqlstr &= " WHEN len(a.accno)<>17 THEN b.accname END AS accname"
            sqlstr &= " FROM BGF020 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO"
            sqlstr &= " LEFT OUTER JOIN accname c ON left(a.accno,16)=c.accno and len(a.accno)=17"
            sqlstr &= " WHERE 1=1 AND a.CLOSEMARK <> 'Y' and a.date2 is null"
            If strSearch = "" Then
                sqlstr &= " AND a.accyear='" & ViewState("bm0accyear") & "' and a.accno='" & ViewState("bm0accno") & "'"
            Else
                sqlstr &= strSearch
            End If
            sqlstr &= " ORDER BY a.accno, a.bgno"
        End If

        lbl_sort.Text = Master.Controller.objSort(strSortType)
        Master.Controller.objDataGrid(DataGridView, lbl_GrdCount, DNS_ACC, sqlstr, "查詢資料檔")


        '判斷是否有值可供選擇*****
        If DataGridView.Items.Count > 0 Then
            Dim txtID As Label = DataGridView.Items(0).FindControl("id")
            txtKey1.Text = txtID.Text
            FindData(txtID.Text)
        End If
    End Sub
    '移動DataGridView指標
    Public Sub FlagMoveSeat(ByVal Status As Integer, Optional ByVal ItemIndex As Integer = 0)
        Dim myItemIndex As Integer = 0

        Try
            'Status: 0:隨機點選 1:第一筆 2:上一筆 3:下一筆 4:最未筆
            Select Case Status
                Case 1
                    myItemIndex = 0
                Case 2
                    myItemIndex = ItemIndex - 1
                Case 3
                    myItemIndex = ItemIndex + 1
                Case 4
                    myItemIndex = DataGridView.Items.Count - 1
                Case 0
                    myItemIndex = ItemIndex
            End Select

            myItemIndex = DataGridView.Items.Item(myItemIndex).ItemIndex


            '關鍵值*****
            Dim id As Label = DataGridView.Items.Item(myItemIndex).FindControl("id") '記錄編號


            '查詢資料及控制*****
            FindData(id.Text)
            ViewState("MyStatus") = 0
            ViewState("ItemIndex") = myItemIndex
        Catch ex As Exception

        End Try
    End Sub

    '存檔
    Public Sub SaveData(ByVal PrevTableStatus As Integer)
        Dim SaveStatus As Boolean = False
        Dim blnCheck As Boolean = False
        '-- 不可重複(只有新增才需判斷) --
        Dim strRow As String = ""
        If PrevTableStatus = "1" Then
            txtKey1.Text = lblBgno.Text
        End If
        Dim loadkey As String = lblBgno.Text

        '判斷程序為新增或修改*****
        If PrevTableStatus = "1" Then SaveStatus = InsertData() : ViewState("FileKey") = txtKey1.Text '新增
        If PrevTableStatus = "2" Then SaveStatus = UpdateData() : ViewState("FileKey") = txtKey1.Text '修改

        If SaveStatus = True Then
            ViewState("MyStatus") = 0
            SetControls(0)
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            UCBase1.SetButtons()
            FindData(loadkey) '直接查詢值
        End If


        '異動後初始化*****
        'ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作" & IIf(PrevTableStatus = 1, "新增", "修改") & "資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
        Master.ACC.SystemOperate(Session("USERID"), strPage, "BGF020", ViewState("FileKey"), IIf(PrevTableStatus = 1, "I", "U"), IIf(SaveStatus = True, "S", "F")) '系統操作歷程記錄
    End Sub
    '還原
    Public Sub ResetData()
        ViewState("MyStatus") = 0
        SetControls(0)
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        FlagMoveSeat(0, ViewState("ItemIndex"))
    End Sub
    '新增
    Public Function InsertData() As Boolean
        Dim blnCheck As Boolean = False

        '    Dim keyvalue, sqlstr, retstr, updstr, skind, TDC, sDC, J As String  'skind 1:收入傳票 2:支出傳票 sDC 1:借方金額 2:貸方金額
        '    Dim BYear, I As Integer '請購的預算年度
        '    Dim Tamt, TBGAmt As Decimal
        '    Dim BAccno, strMsg As String '請購科目

        '    '檢查
        '    Try
        '        If Master.Models.ValComa(txtAmta.Text) = 0 Then
        '            MsgBox("金額不可為0")
        '            Exit Function
        '        End If

        '        BYear = Mid(cboAccno.SelectedValue, 1, 3)
        '        BAccno = Trim(Mid(cboAccno.SelectedValue, 5, 17))

        '        strAccno = Format(Val(BYear), "000") & " " & Mid(BAccno + Space(17), 1, 17)

        '        TBGAmt = Master.Controller.QueryBGAmt(BYear, BAccno)
        '        If Mid(BAccno, 1, 1) = "4" Then   '99/12/6 update 
        '            If Master.Models.ValComa(txtAmta.Text) > 0 Then   '收入科目  正數表示收入傳票, 貸方金額
        '                skind = "1"
        '                sDC = "2"
        '            Else
        '                skind = "2"
        '                sDC = "1"
        '            End If
        '        Else
        '            If Master.Models.ValComa(txtAmta.Text) > 0 Then  '支出科目  正數表示支出傳票, 借方金額
        '                skind = "2"
        '                sDC = "1"
        '            Else
        '                skind = "1"
        '                sDC = "2"
        '            End If
        '        End If

        '        If TBGAmt - Master.Models.ValComa(txtAmta.Text) < 0 And Session("flow") <> "Y" And skind <> "1" Then
        '            MsgBox("預算餘額不足")
        '            Exit Function
        '        End If

        '        '資料處理(新增一筆至BGF020 & UPDATE BGF010->TOTPER)
        '        intNo = Master.Controller.RequireNO(DNS_ACC, Session("sYear"), "B")    ' 取用得請購編號
        '        Master.ADO.GenInsSql("BGNO", Format(Session("sYear"), "000") + Format(intNo, "00000"), "T")
        '        Master.ADO.GenInsSql("accyear", BYear, "N")
        '        Master.ADO.GenInsSql("accno", BAccno, "T")
        '        Master.ADO.GenInsSql("KIND", skind, "T")
        '        Master.ADO.GenInsSql("DC", sDC, "T")
        '        Master.ADO.GenInsSql("DATE1", dtpDate1a.Text, "D")
        '        Master.ADO.GenInsSql("REMARK", txtRemarka.Text, "U")
        '        Master.ADO.GenInsSql("AMT1", txtAmta.Text, "N")
        '        Master.ADO.GenInsSql("useableAMT", txtAmta.Text, "N")
        '        Master.ADO.GenInsSql("subject", txtSubjecta.Text, "U")
        '        Master.ADO.GenInsSql("CLOSEMARK", " ", "T")
        '        If isArea Then
        '            Master.ADO.GenInsSql("area", txtAreaA.Text, "T")
        '        End If
        '        sqlstr = "insert into BGF020 " & Master.ADO.GenInsFunc
        '        retstr = Master.ADO.dbExecute(DNS_ACC, sqlstr)
        '        If retstr = "True" Then
        '            'Call LoadGridFunc()
        '            'MsgBox("新增成功")
        '        Else
        '            MsgBox("新增失敗" + sqlstr)
        '            Exit Function
        '        End If
        '    Catch ex As Exception
        '        MsgBox("資料錯誤,無法新增")
        '        Exit Function
        '    End Try

        '    'UPDATE BGF010->TOTPER+AMT 
        '    sqlstr = "update BGF010 set TOTPER = TOTPER + " & Master.Models.ValComa(txtAmta.Text) & " where ACCYEAR=" & BYear & " AND ACCNO='" & BAccno & "'"
        '    retstr = Master.ADO.dbExecute(DNS_ACC, sqlstr)
        '    If retstr <> "True" Then
        '        MsgBox("更新BGF010失敗")
        '    End If
        '    If Session("ORG") <> "ptia" Then '彰化
        '        MsgBox("作業完成,請購編號=" + Format(sYear, "000") + Format(intNo, "00000") & vbCrLf & "可繼續新增請購")
        '    Else
        '        sqlstr = "SELECT * FROM BGF010 WHERE accyear=" & sYear & " and accno='" & BAccno & "'"
        '        TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
        '        strMsg = ""
        '        If TempDataSet.Tables(0).Rows.Count > 0 Then
        '            Dim intUseableBg As Integer = 0
        '            With TempDataSet.Tables(0).Rows(0)
        '                Select Case sSeason
        '                    Case Is = 1
        '                        intUseableBg = .Item("bg1") + .Item("up1")
        '                    Case Is = 2
        '                        intUseableBg = .Item("bg1") + .Item("bg2") + .Item("up1") + .Item("up2")
        '                    Case Is = 3
        '                        intUseableBg = .Item("bg1") + .Item("bg2") + .Item("bg3") + .Item("up1") + .Item("up2") + .Item("up3")
        '                    Case Is = 4
        '                        intUseableBg = .Item("bg1") + .Item("bg2") + .Item("bg3") + .Item("bg4") + .Item("up1") + .Item("up2") + .Item("up3") + .Item("up4")
        '                End Select
        '                strMsg = "預算分配累計數=" + FormatNumber(intUseableBg, 0) + vbCrLf + "    本件請購數=" + txtAmta.Text + vbCrLf + _
        '                         "    累計執行數=" + FormatNumber(.Item("totper") + .Item("totuse"), 0) + vbCrLf + "        剩餘數=" + FormatNumber(intUseableBg - (.Item("totper") + .Item("totuse")), 0)
        '            End With
        '        End If
        '        MsgBox("作業完成,請購編號=" + Format(sYear, "000") + Format(intNo, "00000") + vbCrLf + strMsg & vbCrLf & "可繼續新增請購")
        '    End If
        '    If txtBatNo.Text <> "" Then   'And txtPayno.Text <> ""
        '        TabContainer1.ActiveTabIndex = 2   '指定Tab頁籤
        '        'pay000填入bgno
        '        sqlstr = "update pay000 set bgno = '" & Format(sYear, "000") + Format(intNo, "00000") & _
        '                 "' where batno='" & txtBatNo.Text & "' and accyear=" & txtByear.Text
        '        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '    End If
        '    'If ComeFromCBGf020 = True And txtCbgno.Text <> "" Then
        '    '    TabControl1.SelectedIndex = 4
        '    '    'cbgf020填入usebgno
        '    '    sqlstr = "update cbgf020 set usebgno = '" & Format(sYear, "000") + Format(intNo, "00000") & "' where bgno='" & txtCbgno.Text & "'"
        '    '    retstr = ws1.runsql(mastconn, sqlstr)
        '    '    Call Cbgf020REmove()   '次預算remove datagrid record  & 清空單據編號
        '    'End If

        '    'txtUserid.Text = ""
        '    'Call Button1_Click(New System.Object, New System.EventArgs)   'query 新請購號
        '    'Call PutAccnoToCbo()
        '    'cboAccno.SelectedValue = strAccno
        '    'lblUpRecord.Text = "上筆" & Format(sYear, "000") + Format(intNo, "00000") & "已完成新增 " & _
        '    '                   vbCrLf & strAccno & " " & txtRemarka.Text & "  $" & FormatNumber(Val(txtAmta.Text), 0)


        blnCheck = True
        Return blnCheck
    End Function
    '修改
    Public Function UpdateData() As Boolean
        Dim blnCheck As Boolean = False

        'Dim keyvalue, sqlstr, retstr, updstr, TDC, sKind, sDc As String
        'Dim tBgamt As Decimal

        'keyvalue = Trim(lblNo.Text)
        'If Master.Models.ValComa(txtAmta.Text) > Master.Models.ValComa(lblAMT.Text) And Mid(lblAccno.Text, 1, 1) <> "4" Then
        '    tBgamt = Master.Controller.QueryBGAmt(Val(lblYear.Text), lblAccno.Text)
        '    If tBgamt + Master.Models.ValComa(lblAMT.Text) - Master.Models.ValComa(txtAmta.Text) < 0 And Session("flow") <> "Y" Then
        '        MsgBox("預算餘額不足")
        '        Exit Function
        '    End If
        'End If

        'If Master.Models.ValComa(txtAmta.Text) <> Master.Models.ValComa(lblAMT.Text) Then
        '    sqlstr = "update BGF010 set TOTPER = TOTPER - " & Master.Models.ValComa(lblAMT.Text) & " + " & Master.Models.ValComa(txtAmta.Text) & _
        '             " where ACCYEAR=" & Trim(lblYear.Text) & " AND ACCNO='" & Trim(lblAccno.Text) & "'"
        '    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        '    If retstr <> "sqlok" Then
        '        MsgBox("更新BGF010失敗" & sqlstr)
        '    End If
        'End If

        'If Mid(lblAccno.Text, 1, 1) = "4" Then   '99/12/6 update 
        '    If Master.Models.ValComa(txtAmta.Text) > 0 Then   '收入科目  正數表示收入傳票, 貸方金額
        '        sKind = "1"
        '        sDc = "2"
        '    Else
        '        sKind = "2"
        '        sDc = "1"
        '    End If
        'Else
        '    If Master.Models.ValComa(txtAmta.Text) > 0 Then  '支出科目  正數表示支出傳票, 借方金額
        '        sKind = "2"
        '        sDc = "1"
        '    Else
        '        sKind = "1"
        '        sDc = "2"
        '    End If
        'End If
        'Master.ADO.GenUpdsql("kind", sKind, "T")
        'Master.ADO.GenUpdsql("DC", sDc, "T")
        'Master.ADO.GenUpdsql("date1", dtpDate1a.Text, "D")
        'Master.ADO.GenUpdsql("remark", txtRemarka.Text, "U")
        'Master.ADO.GenUpdsql("subject", txtSubjecta.Text, "U")
        'Master.ADO.GenUpdsql("amt1", txtAmta.Text, "N")
        'Master.ADO.GenUpdsql("useableamt", txtAmta.Text, "N")
        'If isArea Then
        '    Master.ADO.GenUpdsql("area", txtAreaA.Text, "T")
        'End If
        'sqlstr = "update BGF020 set " & Master.ADO.genupdfunc & " where bgno='" & keyvalue & "'"
        'retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        'If retstr = "sqlok" Then
        '    blnCheck = True
        'Else
        '    MsgBox("更新失敗" & sqlstr)
        'End If

        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False

        'Dim keyvalue, sqlstr, retstr As String

        'keyvalue = Trim(lblNo.Text)


        'sqlstr = "select * from bgf030 where bgno='" & keyvalue & "'"
        'TempDataSet = Master.ADO.openmember(DNS_ACC, "temp", sqlstr)
        'If TempDataSet.Tables("temp").Rows.Count > 0 Then
        '    MsgBox("此筆已有分次開支資料:" & TempDataSet.Tables("temp").Rows(0).Item("remark") & _
        '    FormatNumber(TempDataSet.Tables("temp").Rows(0).Item("useamt"), 0))
        '    Exit Sub
        'End If
        'sqlstr = "update BGF010 set TOTPER = TOTPER - " & Master.Models.ValComa(txtAmta.Text) & _
        '         " where ACCYEAR=" & Trim(lblYear.Text) & " AND ACCNO='" & Trim(cboAccno.Text) & "'"
        'retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        'If retstr <> "sqlok" Then
        '    MsgBox("更新BGF010失敗" & sqlstr)
        'End If

        ''清PAY000.BGNO   bgno已有年度
        'sqlstr = "update PAY000 set BGNO=NULL where BGNO='" & keyvalue & "'"
        'retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        'sqlstr = "delete from BGF020 where bgno='" & keyvalue & "'"
        'retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        'If retstr = "sqlok" Then
        '    SaveStatus = True
        'End If

        'If SaveStatus = True Then
        '    ViewState("MyStatus") = 0
        '    SetControls(0)
        '    FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        '    FlagMoveSeat(1)
        '    TabContainer1.ActiveTabIndex = 0
        'End If

        '異動後初始化*****
        'ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作刪除資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
        'Master.ACC.SystemOperate(Session("USERID"), strPage, "BGF020", keyvalue, "D", IIf(SaveStatus = True, "S", "F")) '系統操作歷程記錄
    End Sub

    '輸入控制項的 ReadOnly 屬性及 Enabled 屬性
    Public Sub SetControls(ByVal Status As Integer)
        '頁面控制項ID陣列***** 
        'Status: 0:一般模式 1:新增模式 2:修改模式 3:複製模式
        Dim objMainTextBox() As TextBox = {}
        Dim objMainDropDownList() As DropDownList = {}
        Dim objMainRadioButtonList() As RadioButtonList = {}
        Dim objTextBox() As TextBox = {}
        Dim objDropDownList() As DropDownList = {}
        Dim objRadioButtonList() As RadioButtonList = {}

        Master.Controller.Main_Control(objMainTextBox, objMainDropDownList, objMainRadioButtonList, Status)
        Master.Controller.TextBox_Control(objTextBox, Status) : Master.Controller.TextBox_Clear(objTextBox, Status)
        Master.Controller.DropDownList_Control(objDropDownList, Status) : Master.Controller.DropDownList_Clear(objDropDownList, Status)
        Master.Controller.RadioButtonList_Control(objRadioButtonList, Status) : Master.Controller.RadioButtonList_Clear(objRadioButtonList, Status)


        ''自訂項目*****        
        If Status = "1" Then
            'PutAccnoToCbo()
        End If


        '其他控制項
        Select Case Status
            Case 0 '一般模式


            Case 1 '新增模式
                TabContainer1.ActiveTabIndex = 1

            Case 2 '修改模式


            Case 3 '複製模式
                TabContainer1.ActiveTabIndex = 1
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
        BGF020_Load(strKey1) '載入資料
    End Sub
    '請購推算檔
    Sub BGF020_Load(ByVal strKey1 As String)
        '開啟查詢
        objCon99 = New SqlConnection(DNS_ACC)
        objCon99.Open()

        strSQL99 = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.amt1, a.useableamt, a.remark, a.kind,a.subject," & _
         "CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')' " & _
             " WHEN len(a.accno)<>17 THEN b.accname END AS accname " & _
         "FROM BGF020 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO" & _
         " LEFT OUTER JOIN accname c ON left(a.accno,16)=c.accno and len(a.accno)=17" & _
         " WHERE a.BGNO = '" & strKey1 & "'" & _
         " AND a.CLOSEMARK <> 'Y' and a.date2 is null"

        objCmd99 = New SqlCommand(strSQL99, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            lblBgno.Text = Trim(objDR99("BGNO").ToString)  '不允許修改bgno,accyear,accno
            strBGNo = Trim(objDR99("BGNO").ToString)
            lblYear.Text = Trim(objDR99("ACCYEAR").ToString)
            lblAccno.Text = Trim(objDR99("ACCNO").ToString)
            lblAccname.Text = Trim(objDR99("ACCNAME").ToString)
            lblDate1.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE1").ToShortDateString.ToString))

            '傳票
            Dim strAccType As String = Trim(Mid(Trim(objDR99("ACCNO").ToString), 1, 1)) '科目種類
            Dim intAccMoney As Double = Trim(objDR99("AMT1").ToString)

            rdbKind1.Checked = False
            rdbKind2.Checked = False
            Select Case strAccType
                Case "4"
                    If intAccMoney >= 0 Then rdbKind1.Checked = True
                    If intAccMoney < 0 Then rdbKind2.Checked = True
                Case Else
                    If intAccMoney >= 0 Then rdbKind2.Checked = True
                    If intAccMoney < 0 Then rdbKind1.Checked = True
            End Select

            'If Trim(objDR99("KIND").ToString) = "1" Then
            '    rdbKind1.Checked = True
            'Else
            '    rdbKind2.Checked = True
            'End If
            txtRemark.Text = Trim(objDR99("REMARK").ToString)
            lblRemark.Text = Trim(objDR99("REMARK").ToString)
            txtAmt1.Text = FormatNumber(objDR99("useableamt").ToString, 0)
            lblUseableAmt.Text = FormatNumber(Trim(objDR99("useableamt").ToString), 0)
            lblAmt1.Text = FormatNumber(Trim(objDR99("AMT1").ToString), 0)
            txtSubject.Text = Trim(objDR99("SUBJECT").ToString)
            lblSubject.Text = Trim(objDR99("SUBJECT").ToString)
            txtMark.Text = ""  '憑證應補事項
            lblkey.Text = Trim(objDR99("BGNo").ToString)
            If Trim(lblkey.Text) <> "" Then
                lblBgamt.Text = FormatNumber(Master.Controller.QueryBGAmt(CInt(Trim(objDR99("ACCYEAR").ToString)), Trim(objDR99("ACCNO").ToString)), 0)
                lblunUseamt.Text = FormatNumber(Master.Controller.QueryUnUseAmt(CInt(Trim(objDR99("ACCYEAR").ToString)), Trim(objDR99("ACCNO").ToString), Session("sSeason")), 0)  '當季開支餘額
            End If
            lblGrade6.Text = ""

            Dim strRemark As String
            strRemark = Trim(txtRemark.Text)
            If strRemark.IndexOf("小組辦公") > -1 Or strRemark.IndexOf("旅費") > -1 Or _
               strRemark.IndexOf("影印") > -1 Or strRemark.IndexOf("薪") > -1 Or _
               strRemark.IndexOf("獎金") > -1 Or strRemark.IndexOf("退休金") > -1 Or _
               strRemark.IndexOf("辦公費") > -1 Or strRemark.IndexOf("值日") > -1 Or _
               strRemark.IndexOf("生育補助") > -1 Or strRemark.IndexOf("結婚補助") > -1 Or _
               strRemark.IndexOf("教育補助") > -1 Or strRemark.IndexOf("殮葬補助") > -1 Or _
               strRemark.IndexOf("休假") > -1 Or strRemark.IndexOf("喪葬補助") > -1 Or _
               strRemark.IndexOf("勞保") > -1 Or strRemark.IndexOf("電費") > -1 Then

                lblMsg.Text = "此筆應為直接開支"
            Else
                lblMsg.Text = ""
            End If

            'KEY、異動人員及日期*****
            txtKey1.Text = Trim(objDR99("BGNO").ToString)
        Else
            TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
            MessageBx("查無此推算號碼!!!")
        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標    
    End Sub


#End Region

#Region "物件選擇異動值"
    '依請購編號直接查詢
    Protected Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        'Dim strSearch As String = ""

        'strSearch &= " AND a.accyear='" & lblNoYear.Text & "' AND a.bgno LIKE '%" & txtNO.Text & "'"

        'FillData("", "ASC", strSearch)


        TabContainer1.ActiveTabIndex = 2   '指定Tab頁籤
        FindData(lblNoYear.Text & Master.Models.strZero(5, txtNO.Text))               '查詢主檔        
        txtNO.Text = "" : txtNO.Focus()
    End Sub

    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        If Master.Models.ValComa(txtAmt1.Text) = 0 Then Exit Sub
        Dim strRemark As String
        strRemark = Trim(txtRemark.Text)
        'If strRemark.IndexOf("小組辦公") > -1 Or strRemark.IndexOf("旅費") > -1 Or _
        '   strRemark.IndexOf("影印") > -1 Or strRemark.IndexOf("薪") > -1 Or _
        '   strRemark.IndexOf("獎金") > -1 Or strRemark.IndexOf("退休金") > -1 Or _
        '   strRemark.IndexOf("辦公費") > -1 Or strRemark.IndexOf("值日") > -1 Or _
        '   strRemark.IndexOf("生育補助") > -1 Or strRemark.IndexOf("結婚補助") > -1 Or _
        '   strRemark.IndexOf("教育補助") > -1 Or strRemark.IndexOf("殮葬補助") > -1 Or _
        '   strRemark.IndexOf("休假") > -1 Or strRemark.IndexOf("喪葬補助") > -1 Or _
        '   strRemark.IndexOf("勞保") > -1 Or strRemark.IndexOf("電費") > -1 Then
        '    If MsgBox("此筆應為直接開支 確定嗎? (不直接開支請選NO)", MsgBoxStyle.DefaultButton1 + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
        '        Call DirectUse()    '主計直接開支
        '    Else
        '        Call Account_Check1() '主計請購登計
        '    End If
        'Else
        '    Call Account_Check1()  '主計請購登計
        'End If

        Call Account_Check1()  '主計請購登計

        txtNO.Text = ""
        txtNO.Focus()
    End Sub

    Protected Sub btnDirect_Click(sender As Object, e As EventArgs) Handles btnDirect.Click
        'If MsgBox("確定要直接開支嗎?)", MsgBoxStyle.DefaultButton1 + MsgBoxStyle.YesNo) <> MsgBoxResult.Yes Then
        '    Exit Sub
        'End If

        If Val(txtAmt1.Text) = 0 Then Exit Sub
        Call DirectUse()    '主計直接開支
        TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
        txtNO.Text = ""
        txtNO.Focus()
    End Sub

    Protected Sub btnBack_tab1_Click(sender As Object, e As EventArgs) Handles btnBack_tab1.Click
        TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤
    End Sub
    Protected Sub btnBack_tab2_Click(sender As Object, e As EventArgs) Handles btnBack_tab2.Click
        TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
    End Sub

    Protected Sub btnGrade6_Click(sender As Object, e As EventArgs) Handles btnGrade6.Click
        '六級餘額查詢   this function=bg010.vb 
        Dim BYear, I As Integer '請購的預算年度
        Dim BAccno As String '請購科目
        BYear = lblYear.Text
        BAccno = lblAccno.Text
        If Len(BAccno) >= 7 Then
            Dim tempdataset As DataSet
            Dim sqlstr As String
            sqlstr = "SELECT sum(bg1+bg2+bg3+bg4+up1+up2+up3+up4-totper-totuse) as balance FROM bgf010 " & _
                     " WHERE accyear=" & BYear & " and left(accno,9)='" & Mid(BAccno, 1, 9) & "'"
            tempdataset = Master.ADO.openmember(DNS_ACC, "temp", sqlstr)
            If tempdataset.Tables("temp").Rows.Count = 0 Then
                lblGrade6.Text = "0"
            Else
                lblGrade6.Text = FormatNumber(tempdataset.Tables("temp").Rows(0).Item(0), 0)
            End If
        Else
            lblGrade6.Text = "請選擇大於六級科目"
        End If
    End Sub

    Sub Account_Check1()   '主計請購登記
        strBGNo = lblBgno.Text
        Dim sqlstr, retstr As String

        '更新請購檔BGF020
        If Trim(lblRemark.Text) <> Trim(txtRemark.Text) Then Master.ADO.GenUpdsql("remark", txtRemark.Text, "U")
        If Master.Models.ValComa(lblUseableAmt.Text) <> Master.Models.ValComa(txtAmt1.Text) Then
            Master.ADO.GenUpdsql("amt1", txtAmt1.Text, "N")      '以主計審核金額當請購金額
            Master.ADO.GenUpdsql("useableamt", txtAmt1.Text, "N") '以主計審核金額當可支用金額
        End If
        Master.ADO.GenUpdsql("date2", UserDate, "D")  '以LOGIN日期當審核日
        If Trim(lblSubject.Text) <> Trim(txtSubject.Text) Then Master.ADO.GenUpdsql("SUBJECT", txtSubject.Text, "U")
        sqlstr = "update BGF020 set " & Master.ADO.genupdfunc & " where bgno='" & strBGNo & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
        Else
            MessageBx("更新失敗" & sqlstr)
            Exit Sub
        End If

        'check 請購金額不相同 update bgf010 
        If Master.Models.ValComa(lblUseableAmt.Text) <> Master.Models.ValComa(txtAmt1.Text) Then
            sqlstr = "update bgf010 set totper = totper - " & Master.Models.ValComa(lblUseableAmt.Text) & " + " & Master.Models.ValComa(txtAmt1.Text) & _
                     " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("預算檔bgf010更新錯誤" & lblYear.Text & "  " & lblAccno.Text)
                Exit Sub
            End If
        End If

        'MessageBx("請購登記完成")
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        TabContainer1.ActiveTabIndex = 1  '指定Tab頁籤
    End Sub

    Sub DirectUse()  '主計直接開支
        strBGNo = lblBgno.Text

        Dim keyvalue, sqlstr, retstr, updstr, skind, TDC, sDC, J As String
        Dim BYear, SumBg, I, Tamt, intRel As Integer '請購的預算年度
        Dim BAccno, strClose As String '請購科目
        Dim sumRtot As Decimal
        If Mid(lblAccno.Text, 1, 1) <> "4" Then
            If Master.Models.ValComa(txtAmt1.Text) > 0 And Master.Models.ValComa(lblunUseamt.Text) - Master.Models.ValComa(txtAmt1.Text) < 0 And Session("flow") <> "Y" Then  '不可溢支
                MessageBx("當季開支餘額不足,請退回")
                Exit Sub
            End If
            If Master.Models.ValComa(txtAmt1.Text) > 0 And Master.Models.ValComa(lblAmt1.Text) < Master.Models.ValComa(txtAmt1.Text) Then
                MessageBx("開支數不可大於請購數,請修正")
                Exit Sub
            End If
        End If
        If Mid(Trim(lblAccno.Text), 1, 5) = "21302" And Session("flow") <> "Y" Then   '代收款還要檢查已收入金額是否足夠
            sqlstr = "SELECT sum(amt) as amt  FROM BGF050 WHERE left(accno,16)='" & Mid(Trim(lblAccno.Text), 1, 16) & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
            sumRtot = Master.ADO.nz(TempDataSet.Tables("temp").Rows(0).Item(0), 0)
            sqlstr = "SELECT sum(totuse) as amt2  FROM BGF010 WHERE accyear=" & Trim(lblYear.Text) & " and left(accno,16)='" & Mid(Trim(lblAccno.Text), 1, 16) & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
            If sumRtot - Master.ADO.nz(TempDataSet.Tables("temp").Rows(0).Item(0), 0) - Master.Models.ValComa(txtAmt1.Text) < 0 Then
                MessageBx("代收款收入不足開支,請退回")
                Exit Sub
            End If
        End If

        strClose = "Y"  '已開支完畢   (直接開支不可再分次開支)
        'If Val(txtAmt1.Text) < Val(lblUseableAmt.Text) Then
        '    If MsgBox("是否要將請購餘額留待下次開支?", MsgBoxStyle.DefaultButton1 + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
        '        strClose = "N"   '尚可開支
        '    End If
        'End If

        '資料處理(UPDATE BGF010->TOTuse,totPER)
        If strClose = "Y" Then
            sqlstr = "update bgf010 set totper = totper - " & Master.Models.ValComa(lblUseableAmt.Text)
        Else
            sqlstr = "update bgf010 set totper = totper - " & Master.Models.ValComa(txtAmt1.Text)
        End If
        sqlstr = sqlstr & ", totuse = totuse + " & Master.Models.ValComa(txtAmt1.Text) & " where accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & lblYear.Text & "  " & lblAccno.Text)
            Exit Sub
        End If

        '資料處理(UPDATE BGF020->date2)
        'GenUpdsql("amt1", txtAmt1.Text, "N")  '以主計審核金額當請購金額
        If Trim(lblRemark.Text) <> Trim(txtRemark.Text) Then Master.ADO.GenUpdsql("remark", txtRemark.Text, "U")
        If Trim(lblSubject.Text) <> Trim(txtSubject.Text) Then Master.ADO.GenUpdsql("SUBJECT", txtSubject.Text, "U")
        Master.ADO.GenUpdsql("date2", UserDate, "D")  '以LOGIN日期當審核日
        If strClose = "Y" Then
            Master.ADO.GenUpdsql("closemark", "Y", "T")
        End If
        sqlstr = "update BGF020 set " & Master.ADO.genupdfunc & " where bgno='" & strBGNo & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
        Else
            MessageBx("更新失敗" & sqlstr)
            Exit Sub
        End If

        '資料處理(新增一筆至BGF030)
        sqlstr = "SELECT max(REL) as rel  FROM BGF030 WHERE BGNO='" & strBGNo & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)

        intRel = 0
        If Not IsDBNull(TempDataSet.Tables("TEMP").Rows(0).Item(0)) Then
            intRel = TempDataSet.Tables("TEMP").Rows(0).Item(0)
        End If
        intRel += 1
        TempDataSet = Nothing
        sqlstr = Master.ADO.GenInsFunc          'clear the insert fields 
        Master.ADO.GenInsSql("BGNO", strBGNo, "T")
        Master.ADO.GenInsSql("rel", intRel, "N")
        Master.ADO.GenInsSql("date3", UserDate, "D")
        Master.ADO.GenInsSql("date4", UserDate, "D")
        Master.ADO.GenInsSql("USEAMT", txtAmt1.Text, "N")
        Master.ADO.GenInsSql("REMARK", txtRemark.Text, "U")
        If Len(Trim(txtMark.Text)) > 0 Then
            Master.ADO.GenInsSql("MARK", txtMark.Text, "U")
        End If
        Master.ADO.GenInsSql("NO_1_NO", 0, "N")
        sqlstr = "insert into BGF030 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("資料寫入BGF030失敗" & sqlstr)
            Exit Sub
        End If

        'MessageBx("直接開支完成")
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        TabContainer1.ActiveTabIndex = 1  '指定Tab頁籤
    End Sub
#End Region
End Class