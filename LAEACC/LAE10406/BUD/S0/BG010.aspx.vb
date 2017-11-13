Imports System.Data
Imports System.Data.SqlClient

Public Class BG010
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
    Dim TempDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
        Master.Controller.objDataGridStyle(dtgPay000, "S")

        UCBase1.SetButtons_Visible()                         '初始化控制鍵
        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        If Session("ORG") = "chian" Then
            ViewState("isArea") = True
            'gbxArea.Visible = True
            'gbxAreaA.Visible = True
            ''將管理處置combobox 
            'sqlstr = "SELECT area, area+areaname as areaname  FROM area "
            'AreaDs = ws1.openmember("", "area", sqlstr)
            'If AreaDs.Tables("area").Rows.Count = 0 Then
            '    cboAreaA.Text = "尚無受款人片語"
            'Else
            '    cboAreaA.DataSource = AreaDs.Tables("area")
            '    cboAreaA.DisplayMember = "areaname"  '顯示欄位
            '    cboAreaA.ValueMember = "area"     '欄位值
            '    'cboareaa.SelectionLength = 60
            'End If
        End If
        PutAccnoToCbo()
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            '其他預設值*****
            SetControls(0) '設定所有輸入控制項的唯讀狀態

            '資料查詢*****
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            txtRemarka.Attributes.Add("maxlength", "50")
            txtRemarka.Attributes.Add("onkeyup", "return ismaxlength(this)")
            PutAccnoToCbo()
        End If
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
        Master.Controller.objDataGridStyle(dtgPay000, "S")
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
                TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
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

    '點選資訊
    Sub dtgPay000_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles dtgPay000.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號
        Dim txtYEAR As Label = e.Item.FindControl("年度") '記錄編號

        Select Case e.CommandName
            Case "btnShow"
                '查詢資料及控制*****

                ViewState("MyStatus") = 1
                SetControls(1)
                UCBase1.btnAddNew_Click(sender, e)

                find_pay000(txtID.Text, txtYEAR.Text)
                'FlagMoveSeat(0, e.Item.ItemIndex)
                'TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
        End Select
    End Sub
#End Region
    '#Region "按鍵選項"
    '    '查詢
    '    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
    '        Dim strMySearch As String = ""

    '        '開啟查詢*****
    '        '項目
    '        If S_ACCYEAR.SelectedValue <> "" Then strMySearch &= " AND a.ACCYEAR LIKE '%" & S_ACCYEAR.SelectedValue & "%'"
    '        If S_BGNO.Text <> "" Then strMySearch &= " AND a.BGNO LIKE '%" & S_BGNO.Text & "%'"


    '        '初始化*****
    '        UCBase1.SetButtons()                         '初始化控制鍵

    '        ViewState("MySearch") = strMySearch          '查詢值記錄
    '        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '開啟查詢
    '    End Sub

    '    '清除條件
    '    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
    '        '還原預設值*****
    '        S_ACCYEAR.SelectedIndex = -1
    '        S_BGNO.Text = ""

    '        '初始化*****
    '        UCBase1.SetButtons() '初始化控制鍵
    '    End Sub
    '#End Region

#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        '開啟查詢
        strSSQL = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.amt1, a.remark,"
        strSSQL &= "CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')' "
        strSSQL &= " WHEN len(a.accno)<>17 THEN b.accname END AS accname, a.kind, a.subject FROM BGF020 a "
        strSSQL &= " INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO"
        strSSQL &= " LEFT JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO and len(a.accno)=17"
        strSSQL &= " WHERE a.CLOSEMARK <> 'Y' and a.date2 is null"
        strSSQL &= strSearch
        strSSQL &= IIf(Session("USERID") = "admin", "", " and b.STAFF_NO = '" & Session("USERID") & "'")
        strSSQL &= " ORDER BY a.bgno DESC"

        lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
        Master.Controller.objDataGrid(DataGridView, lbl_GrdCount, DNS_ACC, strSSQL, "查詢資料檔")

        strSSQL = "select a.*, b.accname from pay000 a left outer join accname b " & _
                        "on a.accno=b.accno where a.bgno is null and left(a.unit,3)='" & Mid(Session("UserUnit"), 1, 3) & _
                        "'  order by a.accyear desc,a.batno asc"
        Master.Controller.objDataGrid(dtgPay000, lbl_dtgPay000GrdCount, DNS_ACC, strSSQL, "查詢資料檔")



        '判斷是否有值可供選擇*****
        If DataGridView.Items.Count > 0 Then
            Dim txtID As Label = DataGridView.Items(0).FindControl("id")
            txtKey1.Text = txtID.Text
            FindData(txtID.Text)
            FlagMoveSeat(0, 0)
            'FlagMoveSeat(0, DataGridView.Items.Count - 1)
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


        '防呆*****
        '--必輸入--
        'TextBox
        Dim objTextBox() As TextBox = {txtAmta}
        Dim strTextBox As String = "請購金額"
        strMessage = Master.Controller.TextBox_Input(objTextBox, 0, strTextBox)
        If strMessage <> "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & strMessage & "】未輸入!!');", True) : Exit Sub

        ''DropDownList
        'Dim objDropDownList() As DropDownList = {cboAccno}
        'Dim strDropDownList As String = "請購科目"
        'strMessage = Master.Controller.DropDownList_Input(objDropDownList, strDropDownList)
        'If strMessage <> "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & strMessage & "】未選擇!!');", True) : Exit Sub

        '--必數字--
        'TextBox
        Dim objTextBoxN() As TextBox = {txtAmta}
        Dim strTextBoxN As String = "請購金額"
        strMessage = Master.Controller.TextBox_Input(objTextBoxN, 1, strTextBoxN)
        If strMessage <> "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & strMessage & "】必數字!!');", True) : Exit Sub

        '-- 不可重複(只有新增才需判斷) --
        Dim strRow As String = ""
        If PrevTableStatus = "1" Then
            'lblNo.Text = Master.Controller.AutoNumber(Mid(Session("DATE"), 1, 3), 5, DNS_ACC, "BGF020", "BGNO", "BGNO LIKE '" & Mid(Session("DATE"), 1, 3) & "%'") '請購編號                
            'strRow = Master.ADO.dbGetRow(DNS_ACC, "BGF020", "BGNO", "BGNO = '" & lblNo.Text & "'")
            'blnCheck = IIf(strRow <> "", True, False) : If blnCheck = True Then MsgBox("【請購編號】，已存在!!") : Exit Sub
            txtKey1.Text = lblNo.Text
        End If

        '判斷程序為新增或修改*****
        If PrevTableStatus = "1" Then SaveStatus = InsertData() : ViewState("FileKey") = txtKey1.Text '新增
        If PrevTableStatus = "2" Then SaveStatus = UpdateData() : ViewState("FileKey") = txtKey1.Text '修改

        If SaveStatus = True Then
            ViewState("MyStatus") = 0
            SetControls(0)
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            UCBase1.SetButtons()
            FindData(lblNo.Text) '直接查詢值
            TabContainer1.ActiveTabIndex = 1 '指定Tab頁籤
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

        Dim keyvalue, sqlstr, retstr, updstr, skind, TDC, sDC, J As String  'skind 1:收入傳票 2:支出傳票 sDC 1:借方金額 2:貸方金額
        Dim BYear, I As Integer '請購的預算年度
        Dim Tamt, TBGAmt As Decimal
        Dim BAccno, strMsg As String '請購科目        
        Dim strAccno As String


        '重新取得請購編號
        Dim intBgNo As String = Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "B") '目前編號數
        Dim strBgNo As String = Format(CInt(Session("sYear")), "000") + Format(CInt(intBgNo), "00000") '重新給號


        '檢查
        Try
            If Master.Models.ValComa(txtAmta.Text) = 0 Then
                MessageBx("金額不可為0")
                Exit Function
            End If

            BYear = Mid(cboAccno.SelectedValue, 1, 3)
            BAccno = Trim(Mid(cboAccno.SelectedValue, 5, 17))

            strAccno = Format(Val(BYear), "000") & " " & Mid(BAccno + Space(17), 1, 17)

            TBGAmt = Master.Controller.QueryBGAmt(BYear, BAccno)
            If Mid(BAccno, 1, 1) = "4" Then   '99/12/6 update 
                If Master.Models.ValComa(txtAmta.Text) > 0 Then   '收入科目  正數表示收入傳票, 貸方金額
                    skind = "1"
                    sDC = "2"
                Else
                    skind = "2"
                    sDC = "1"
                End If
            Else
                If Master.Models.ValComa(txtAmta.Text) > 0 Then  '支出科目  正數表示支出傳票, 借方金額
                    skind = "2"
                    sDC = "1"
                Else
                    skind = "1"
                    sDC = "2"
                End If
            End If

            If TBGAmt - Master.Models.ValComa(txtAmta.Text) < 0 And Session("flow") <> "Y" And skind <> "1" Then
                MessageBx("預算餘額不足")
                Exit Function
            End If

            '資料處理(新增一筆至BGF020 & UPDATE BGF010->TOTPER)
            Master.ADO.GenInsSql("BGNO", strBgNo, "T")
            Master.ADO.GenInsSql("accyear", BYear, "N")
            Master.ADO.GenInsSql("accno", BAccno, "T")
            Master.ADO.GenInsSql("KIND", skind, "T")
            Master.ADO.GenInsSql("DC", sDC, "T")
            Master.ADO.GenInsSql("DATE1", dtpDate1a.Text, "D")
            Master.ADO.GenInsSql("REMARK", txtRemarka.Text, "U")
            Master.ADO.GenInsSql("AMT1", txtAmta.Text, "N")
            Master.ADO.GenInsSql("useableAMT", txtAmta.Text, "N")
            Master.ADO.GenInsSql("subject", txtSubjecta.Text, "U")
            Master.ADO.GenInsSql("CLOSEMARK", " ", "T")
            If ViewState("isArea") Then
                Master.ADO.GenInsSql("area", txtAreaA.Text, "T")
            End If
            sqlstr = "insert into BGF020 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.dbExecute(DNS_ACC, sqlstr)
            If retstr = "True" Then
                'Call LoadGridFunc()
                'MsgBox("新增成功")
            Else
                MessageBx("新增失敗" + sqlstr)
                Exit Function
            End If
        Catch ex As Exception
            MessageBx("資料錯誤,無法新增")
            Exit Function
        End Try

        'UPDATE BGF010->TOTPER+AMT 
        sqlstr = "update BGF010 set TOTPER = TOTPER + " & Master.Models.ValComa(txtAmta.Text) & " where ACCYEAR=" & BYear & " AND ACCNO='" & BAccno & "'"
        retstr = Master.ADO.dbExecute(DNS_ACC, sqlstr)
        If retstr <> "True" Then
            MessageBx("更新BGF010失敗")
        End If
        If Session("ORG") <> "ptia" Then '彰化
            MessageBx("作業完成,請購編號=" & strBgNo & "可繼續新增請購")
        Else
            sqlstr = "SELECT * FROM BGF010 WHERE accyear=" & Session("sYear") & " and accno='" & BAccno & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
            strMsg = ""
            If TempDataSet.Tables(0).Rows.Count > 0 Then
                Dim intUseableBg As Integer = 0
                With TempDataSet.Tables(0).Rows(0)
                    Select Case Session("sSeason")
                        Case Is = 1
                            intUseableBg = .Item("bg1") + .Item("up1")
                        Case Is = 2
                            intUseableBg = .Item("bg1") + .Item("bg2") + .Item("up1") + .Item("up2")
                        Case Is = 3
                            intUseableBg = .Item("bg1") + .Item("bg2") + .Item("bg3") + .Item("up1") + .Item("up2") + .Item("up3")
                        Case Is = 4
                            intUseableBg = .Item("bg1") + .Item("bg2") + .Item("bg3") + .Item("bg4") + .Item("up1") + .Item("up2") + .Item("up3") + .Item("up4")
                    End Select
                    strMsg = "預算分配累計數=" + FormatNumber(intUseableBg, 0) + vbCrLf + "    本件請購數=" + txtAmta.Text + vbCrLf + _
                             "    累計執行數=" + FormatNumber(.Item("totper") + .Item("totuse"), 0) + vbCrLf + "        剩餘數=" + FormatNumber(intUseableBg - (.Item("totper") + .Item("totuse")), 0)
                End With
            End If
            MessageBx("作業完成,請購編號=" + strBgNo + vbCrLf + strMsg & vbCrLf & "可繼續新增請購")
        End If
        If txtBatNo.Text <> "" Then   'And txtPayno.Text <> ""
            TabContainer1.ActiveTabIndex = 2   '指定Tab頁籤
            'pay000填入bgno
            sqlstr = "update pay000 set bgno = '" & strBgNo & _
                     "' where batno='" & txtBatNo.Text & "' and accyear=" & txtByear.Text
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        End If
        'If ComeFromCBGf020 = True And txtCbgno.Text <> "" Then
        '    TabControl1.SelectedIndex = 4
        '    'cbgf020填入usebgno
        '    sqlstr = "update cbgf020 set usebgno = '" & Format(sYear, "000") + Format(intNo, "00000") & "' where bgno='" & txtCbgno.Text & "'"
        '    retstr = ws1.runsql(mastconn, sqlstr)
        '    Call Cbgf020REmove()   '次預算remove datagrid record  & 清空單據編號
        'End If

        'txtUserid.Text = ""
        'Call Button1_Click(New System.Object, New System.EventArgs)   'query 新請購號
        Call PutAccnoToCbo()
        cboAccno.SelectedValue = strAccno
        'lblUpRecord.Text = "上筆" & Format(sYear, "000") + Format(intNo, "00000") & "已完成新增 " & _
        '                   vbCrLf & strAccno & " " & txtRemarka.Text & "  $" & FormatNumber(Val(txtAmta.Text), 0)

        'FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        'TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        lblNo.Text = strBgNo
        blnCheck = True
        Return blnCheck
    End Function
    '修改
    Public Function UpdateData() As Boolean
        Dim blnCheck As Boolean = False

        Dim keyvalue, sqlstr, retstr, updstr, TDC, sKind, sDc As String
        Dim tBgamt As Decimal

        keyvalue = Trim(lblNo.Text)
        If Master.Models.ValComa(txtAmta.Text) > Master.Models.ValComa(lblAMT.Text) And Mid(lblAccno.Text, 1, 1) <> "4" Then
            tBgamt = Master.Controller.QueryBGAmt(Val(lblYear.Text), lblAccno.Text)
            If tBgamt + Master.Models.ValComa(lblAMT.Text) - Master.Models.ValComa(txtAmta.Text) < 0 And Session("flow") <> "Y" Then
                MessageBx("預算餘額不足")
                Exit Function
            End If
        End If

        If Master.Models.ValComa(txtAmta.Text) <> Master.Models.ValComa(lblAMT.Text) Then
            sqlstr = "update BGF010 set TOTPER = TOTPER - " & Master.Models.ValComa(lblAMT.Text) & " + " & Master.Models.ValComa(txtAmta.Text) & _
                     " where ACCYEAR=" & Trim(lblYear.Text) & " AND ACCNO='" & Trim(lblAccno.Text) & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("更新BGF010失敗" & sqlstr)
            End If
        End If
        
        If Mid(lblAccno.Text, 1, 1) = "4" Then   '99/12/6 update 
            If Master.Models.ValComa(txtAmta.Text) > 0 Then   '收入科目  正數表示收入傳票, 貸方金額
                sKind = "1"
                sDc = "2"
            Else
                sKind = "2"
                sDc = "1"
            End If
        Else
            If Master.Models.ValComa(txtAmta.Text) > 0 Then  '支出科目  正數表示支出傳票, 借方金額
                sKind = "2"
                sDc = "1"
            Else
                sKind = "1"
                sDc = "2"
            End If
        End If
        Master.ADO.GenUpdsql("kind", sKind, "T")
        Master.ADO.GenUpdsql("DC", sDc, "T")
        Master.ADO.GenUpdsql("date1", dtpDate1a.Text, "D")
        Master.ADO.GenUpdsql("remark", txtRemarka.Text, "U")
        Master.ADO.GenUpdsql("subject", txtSubjecta.Text, "U")
        Master.ADO.GenUpdsql("amt1", txtAmta.Text, "N")
        Master.ADO.GenUpdsql("useableamt", txtAmta.Text, "N")
        If ViewState("isArea") Then
            Master.ADO.GenUpdsql("area", txtAreaA.Text, "T")
        End If
        sqlstr = "update BGF020 set " & Master.ADO.genupdfunc & " where bgno='" & keyvalue & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            blnCheck = True
        Else
            MessageBx("更新失敗" & sqlstr)
        End If

        MessageBx("更新完成")

        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False

        Dim keyvalue, sqlstr, retstr As String

        keyvalue = Trim(lblNo.Text)


        sqlstr = "select * from bgf030 where bgno='" & keyvalue & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "temp", sqlstr)
        If TempDataSet.Tables("temp").Rows.Count > 0 Then
            MessageBx("此筆已有分次開支資料:" & TempDataSet.Tables("temp").Rows(0).Item("remark") & _
            FormatNumber(TempDataSet.Tables("temp").Rows(0).Item("useamt"), 0))
            Exit Sub
        End If
        sqlstr = "update BGF010 set TOTPER = TOTPER - " & Master.Models.ValComa(txtAmta.Text) & _
                 " where ACCYEAR=" & Trim(lblYear.Text) & " AND ACCNO='" & Trim(cboAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("更新BGF010失敗" & sqlstr)
            Exit Sub
        End If

        '清PAY000.BGNO   bgno已有年度
        sqlstr = "update PAY000 set BGNO=NULL where BGNO='" & keyvalue & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        sqlstr = "delete from BGF020 where bgno='" & keyvalue & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            SaveStatus = True
        End If

        If SaveStatus = True Then
            ViewState("MyStatus") = 0
            SetControls(0)
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            FlagMoveSeat(1)
            TabContainer1.ActiveTabIndex = 0
        End If

        '異動後初始化*****
        'ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作刪除資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
        Master.ACC.SystemOperate(Session("USERID"), strPage, "BGF020", keyvalue, "D", IIf(SaveStatus = True, "S", "F")) '系統操作歷程記錄
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

        End If


        '其他控制項
        Select Case Status
            Case 0 '一般模式
                cboAccno.Visible = False
                lblAccno.Visible = True
                lblAccname.Visible = True
                lblGrade6.Visible = True
                lblkind.Visible = True

            Case 1 '新增模式
                TabContainer1.ActiveTabIndex = 1

                lblYear.Text = Mid(Session("DATE"), 1, 3)

                dtpDate1a.Text = Session("DATE") '請購日期
                lblNo.Text = "存檔後產生" 'String.Format(Session("sYear"), "000") + Format(Master.Controller.QueryNO(Session("sYear"), "B") + 1, "00000") 'vbdataio.vb 讀取請購編號

                ViewState("ComeFromPay000") = False
                txtBatNo.Text = ""
                txtByear.Text = ""
                txtAmta.Text = ""
                txtSubjecta.Text = ""
                lblkind.Visible = False

                '預算科目
                cboAccno.Visible = True
                lblAccno.Visible = False
                lblAccname.Visible = False
                lblGrade6.Visible = False
                lblAMT.Visible = False

            Case 2 '修改模式
                cboAccno.Visible = False
                lblAccno.Visible = True
                lblAccname.Visible = True
                lblGrade6.Visible = True
                lblAMT.Visible = True
                lblkind.Visible = True
                txtSubjecta.Text = ""

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

        strSQL99 = "SELECT * FROM BGF020"
        strSQL99 &= " WHERE BGNO = '" & strKey1 & "'"

        objCmd99 = New SqlCommand(strSQL99, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            'KEY、異動人員及日期*****
            txtKey1.Text = Trim(objDR99("BGNO").ToString)



            '文字欄位值*****
            lblYear.Text = Trim(objDR99("ACCYEAR").ToString)
            'UNIT.Text = Trim(objDR99("UNIT").ToString)
            'UNITNAME.Text = Master.ADO.dbGetRow(DNS_SYS, "unit", "unit_name", "unit_id = '" & Trim(objDR99("UNIT").ToString) & "'")
            dtpDate1a.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE1").ToShortDateString.ToString))
            lblNo.Text = Trim(objDR99("BGNO").ToString)
            txtAmta.Text = Trim(objDR99("amt1").ToString)
            txtRemarka.Text = Trim(objDR99("REMARK").ToString)
            lblAMT.Text = Trim(objDR99("amt1").ToString)
            txtSubjecta.Text = Trim(objDR99("SUBJECT").ToString)
            lblAccno.Text = Trim(objDR99("ACCNO").ToString)
            lblAccname.Text = Master.ADO.dbGetRow(DNS_ACC, "ACCNAME", "ACCNAME", "ACCNO = '" & Trim(objDR99("ACCNO").ToString) & "'")
            lblkind.Text = "收支:" + Trim(objDR99("kind").ToString)


            '非文字物件*****
            'DropDownList
            '顯示值
            '預算科目
            'Master.Controller.objDropDownListOptionCK(ACCNO, Trim(objDR99("ACCNO").ToString))


            If Trim(objDR99("DATE1").ToString) <> "" And Trim(objDR99("DATE2").ToString) <> "" Then
                UCBase1.FindControl("btnDelete").Visible = False
            End If
        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標

    End Sub

    '由編號找資料(有差勤系統)
    Function find_pay000(ByVal strKey1 As String, ByVal strKey2 As String)

        txtBatNo.Text = strKey1
        txtByear.Text = strKey2


        ViewState("ComeFromPay000") = True

        strSQL99 = "SELECT * FROM pay000 WHERE accyear=" & strKey2 & " and batno='" & strKey1 & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", strSQL99)

        If TempDataSet.Tables(0).Rows.Count > 0 Then
            If Master.ADO.nz(TempDataSet.Tables(0).Rows(0).Item("bgno"), "") <> "" Then
                MsgBox("本單據已推算" & TempDataSet.Tables(0).Rows(0).Item("bgno"))
                TabContainer1.ActiveTabIndex = 2   '指定Tab頁籤
                Exit Function
            End If
            With TempDataSet.Tables(0).Rows(0)
                ViewState("strAccno") = Format(Val(Mid(Master.ADO.nz(.Item("payno"), ""), 1, 3)), "000") & " " & Mid(Master.ADO.nz(.Item("accno"), "") & Space(17), 1, 17)
                'strAccno = Mid(nz(.Item("payno"), ""), 1, 3) & " " & Mid(nz(.Item("accno"), "") & Space(17), 1, 17)
                Try
                    cboAccno.SelectedValue = ViewState("strAccno")     '差勤會計科目
                Catch ex As Exception

                End Try

                txtRemarka.Text = Master.ADO.nz(.Item("BATNO"), 0) & Master.ADO.nz(.Item("remark"), "") '& "  " & nz(.Item("name1"), "")
                txtAmta.Text = Master.ADO.nz(.Item("amt"), 0)
                txtSubjecta.Text = Master.ADO.nz(.Item("name1"), "")
                TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
            End With
        Else
            MsgBox("無此單據")
            TabContainer1.ActiveTabIndex = 2   '指定Tab頁籤
            Exit Function
        End If
    End Function
#End Region

#Region "物件選擇異動值"
    '會計科目下拉式
    Sub PutAccnoToCbo()
        Dim sqlstr, StrBg, strSeasonBg As String
        Select Case Session("sSeason")
            Case Is = 1
                StrBg = "Ltrim(str(a.bg1+a.up1-a.totuse))"
                strSeasonBg = "Ltrim(str(a.bg1+a.up1-a.totper-a.totuse))"
            Case Is = 2
                StrBg = "Ltrim(str(a.bg1+a.bg2+a.up1+a.up2-a.totuse))"
                strSeasonBg = "Ltrim(str(a.bg1+a.bg2+a.up1+a.up2-a.totuse-a.totper))"
            Case Is = 3
                StrBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.up1+a.up2+a.up3-a.totuse))"
                strSeasonBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.up1+a.up2+a.up3-a.totuse-a.totper))"
            Case Is = 4
                StrBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totuse))"
                strSeasonBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totuse-a.totper))"
        End Select

        sqlstr = "SELECT right('0'+ltrim(str(a.accyear)),3) + ' ' +left(a.accno+space(17),17) as bgf010key, " & _
                 "CASE WHEN len(a.accno)=17 THEN " & _
                    "right('0'+ltrim(str(a.accyear)),3) +' '+left(a.accno+space(17),17)+c.accname+'-'+b.accname" & _
                    "+' 年預餘:'+Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totper-a.totuse))+" & _
                    "+' 季預餘:'+" & strSeasonBg & "+' 季支餘:'+" & StrBg & _
                 " WHEN len(a.accno)<17 THEN " & _
                    "right('0'+ltrim(str(a.accyear)),3) +' '+left(a.accno+space(17),17)+b.accname" & _
                    "+' 年預餘:'+Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totper-a.totuse))+" & _
                    "+' 季預餘:'+" & strSeasonBg & "+' 季支餘:'+" & StrBg & _
                 " END AS bgf010data " & _
                 "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO " & _
                 "WHERE a.ctrl<>'Y' AND b.STAFF_NO = '" & Session("USERID") & "' order by a.accyear,a.accno"

        Master.Controller.objDropDownListOptionEX(cboAccno, DNS_ACC, sqlstr, "bgf010key", "bgf010data", 0)

    End Sub
    '六級餘額查詢
    Protected Sub btnGrade6_Click(sender As Object, e As EventArgs) Handles btnGrade6.Click
        If cboAccno.SelectedValue = "" Then Exit Sub

        '六級餘額查詢
        Dim BYear, I As Integer '請購的預算年度
        Dim BAccno As String '請購科目
        BYear = Mid(cboAccno.SelectedValue, 1, 3)
        BAccno = Trim(Mid(cboAccno.SelectedValue, 5, 17))
        If Len(BAccno) >= 7 Then
            Dim sqlstr As String
            sqlstr = "SELECT sum(bg1+bg2+bg3+bg4+up1+up2+up3+up4-totper-totuse) as balance FROM bgf010 " & _
                     " WHERE accyear=" & BYear & " and left(accno,9)='" & Mid(BAccno, 1, 9) & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "temp", sqlstr)
            If TempDataSet.Tables("temp").Rows.Count = 0 Then
                lblGrade6.Text = "0"
            Else
                lblGrade6.Text = FormatNumber(TempDataSet.Tables("temp").Rows(0).Item(0), 0)
            End If            
        Else
            lblGrade6.Text = "請選擇大於六級科目"            
        End If

        lblGrade6.Visible = True
    End Sub

    Protected Sub btnAddRemark_Click(sender As Object, e As EventArgs) Handles btnAddRemark.Click
        If txtRemarka.Text <> "" Then
            txtRemarka.Text = Trim(txtRemarka.Text)

            strSQL99 = "insert into psname (unit, seq, psstr) values ('" & Session("UserUnit") & "', 0, '" & txtRemarka.Text & "')"
            Master.ADO.dbExecute(DNS_ACC, strSQL99)
        End If
    End Sub

    Protected Sub btnAddSubject_Click(sender As Object, e As EventArgs) Handles btnAddSubject.Click
        If txtSubjecta.Text <> "" Then
            txtSubjecta.Text = Trim(txtSubjecta.Text)

            strSQL99 = "insert into psname (unit, seq, psstr) values ('" & Session("UserUnit") & "', 9999, '" & txtSubjecta.Text & "')"
            Master.ADO.dbExecute(DNS_ACC, strSQL99)

        End If

    End Sub

    Protected Sub btnUserid_Click(sender As Object, e As EventArgs) Handles btnUserid.Click

        Dim intLens As Integer = 0
        Dim strUserid As String
        If Len(Trim(txtUserid.Text)) > 0 Then
            strSQL99 = "select * from usertable where userid='" & Format(Val(txtUserid.Text), "0000") & "'"
            strUserid = Format(Val(txtUserid.Text), "0000")
            tempdataset = Master.ADO.openmember(DNS_ACC, "temp", strSQL99)
            If TempDataSet.Tables("temp").Rows.Count > 0 Then
                intLens = txtRemarka.Text.IndexOf("  ")
                If intLens > 0 Then
                    txtRemarka.Text = Mid(txtRemarka.Text, 1, intLens) & "  " & tempdataset.Tables("temp").Rows(0).Item("username")
                Else
                    txtRemarka.Text = Trim(txtRemarka.Text) & "  " & tempdataset.Tables("temp").Rows(0).Item("username")
                End If
                txtAmta.Focus()
            Else
                MsgBox("無此編號")
            End If
        End If
    End Sub
#End Region

#Region "其他"
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
#End Region

    Protected Sub btnPayno_Click(sender As Object, e As EventArgs) Handles btnPayno.Click
        find_pay000(txtBatNo.Text, txtByear.Text)
    End Sub

    Protected Sub btnPaynoC_Click(sender As Object, e As EventArgs) Handles btnPaynoC.Click
        TabContainer1.ActiveTabIndex = 0
        ViewState("ComeFromPay000") = False
    End Sub

    Protected Sub btnPay000_Click(sender As Object, e As EventArgs) Handles btnPay000.Click

        ViewState("MyStatus") = 1
        SetControls(1)
        TabContainer1.ActiveTabIndex = 2   'page4
        ViewState("ComeFromPay000") = True
    End Sub

    '複製至事由
    Private Sub cboAccno_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboAccno.SelectedIndexChanged
        Select Case Session("ORG")
            Case "ttia" : Exit Sub '台東會不用
            Case "ptia" : Exit Sub '屏東會不用
        End Select

        '防呆
        If cboAccno.SelectedValue = "~Z" Or cboAccno.SelectedValue = "" Then Exit Sub

        Dim strAccNo As String = Trim(Mid(cboAccno.SelectedValue, 5, 17))

        If Mid(strAccNo, 1, 5) = "13701" Then
            If Len(strAccNo) = 16 Then
                txtRemarka.Text = Master.ADO.dbGetRow(DNS_ACC, "ACCNAME", "ACCNAME", "ACCNO = '" & Mid(strAccNo, 1, 16) & "'")
            Else
                txtRemarka.Text = Master.ADO.dbGetRow(DNS_ACC, "ACCNAME", "ACCNAME", "ACCNO = '" & Mid(strAccNo, 1, 16) & "'") & "-"
                txtRemarka.Text &= Master.ADO.dbGetRow(DNS_ACC, "ACCNAME", "ACCNAME", "ACCNO = '" & strAccNo & "'")
            End If
        Else
            txtRemarka.Text = ""
        End If
    End Sub
End Class