Imports System.Data
Imports System.Data.SqlClient

Public Class BUD_ACCNAME
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

        UCBase1.SetButtons_Visible()                         '初始化控制鍵

        If Session("UnitTitle").indexof("嘉南") >= 0 Then ViewState("blnArea") = True

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤


        '初始化                
        vxtStartNo.Text = "1"    '起值
        vxtEndNo.Text = "9"      '迄值
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            '其他預設值*****
            SetControls(0) '設定所有輸入控制項的唯讀狀態

            '資料查詢*****
            DataGridView.PageSize = 50
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

#End Region
#Region "按鍵選項"
    '查詢


    '清除條件

#End Region



#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        '開啟查詢
        Dim sqlstr, qstr As String
        sqlstr = "SELECT   *  FROM  accname where accno between '" & vxtStartNo.Text & "' and '" & vxtEndNo.Text & "'"
        If txtQaccname.Text <> "" Then qstr = " and accname like '%" & Trim(txtQaccname.Text) & "%'"
        If txtQunit.Text <> "" Then qstr = qstr + " and unit like '%" & Trim(txtQunit.Text) & "%'"
        If txtQstaff_no.Text <> "" Then qstr = qstr + " and staff_no like '%" & Trim(txtQstaff_no.Text) & "%'"
        If txtQaccount_no.Text <> "" Then qstr = qstr + " and account_no like '%" & Trim(txtQaccount_no.Text) & "%'"
        If txtQaccount_no.Text <> "" Then qstr = qstr + " and account_no like '%" & Trim(txtQaccount_no.Text) & "%'"
        If ViewState("blnArea") = True Then
            If txtQArea.Text <> "" Then qstr = qstr + " and area = '" & Trim(txtQArea.Text) & "'"
        End If
        If ChkBelong.Checked Then
            qstr = qstr + " and belong <> 'B'"
        End If
        sqlstr = sqlstr + qstr

        lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
        Master.Controller.objDataGrid(DataGridView, lbl_GrdCount, DNS_ACC, sqlstr, "查詢資料檔")

        '判斷是否有值可供選擇*****
        If DataGridView.Items.Count > 0 Then
            Dim txtID As Label = DataGridView.Items(0).FindControl("id")
            txtKey1.Text = txtID.Text
            'FindData(txtID.Text)
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
            'lblNo.Text = Master.Controller.AutoNumber(Mid(Session("DATE"), 1, 3), 5, DNS_ACC, "BGF020", "BGNO", "BGNO LIKE '" & Mid(Session("DATE"), 1, 3) & "%'") '請購編號                
            'strRow = Master.ADO.dbGetRow(DNS_ACC, "BGF020", "BGNO", "BGNO = '" & lblNo.Text & "'")
            'blnCheck = IIf(strRow <> "", True, False) : If blnCheck = True Then MsgBox("【請購編號】，已存在!!") : Exit Sub
            txtKey1.Text = lblkey.Text
        End If
        Dim loadkey As String = lblkey.Text

        '判斷程序為新增或修改*****
        If PrevTableStatus = "1" Then SaveStatus = InsertData() : ViewState("FileKey") = txtKey1.Text '新增
        If PrevTableStatus = "2" Then SaveStatus = UpdateData() : ViewState("FileKey") = txtKey1.Text '修改

        If SaveStatus = True Then
            ViewState("MyStatus") = 0
            SetControls(0)
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            TabContainer1.ActiveTabIndex = 1
            UCBase1.SetButtons()
            'FindData(loadkey) '直接查詢值
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

        Dim keyvalue, sqlstr, retstr, updstr As String

        Master.ADO.GenInsSql("accno", vxtAccno.Text, "T")
        Master.ADO.GenInsSql("accname", txtAccname.Text, "U")
        Master.ADO.GenInsSql("belong", txtBelong.Text.ToUpper, "T")
        Master.ADO.GenInsSql("bank", txtBank.Text, "T")
        Master.ADO.GenInsSql("unit", txtUnit.Text, "T")
        Master.ADO.GenInsSql("staff_no", txtStaff_no.Text, "T")
        Master.ADO.GenInsSql("account_no", txtAccount_no.Text, "T")
        Master.ADO.GenInsSql("bookaccno", vxtBook.Text, "T")
        If ViewState("blnArea") = True Then Master.ADO.GenInsSql("area", txtArea.Text, "T")
        sqlstr = "insert into accname " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        If retstr = "sqlok" Then
            blnCheck = True
        Else
            MessageBx("新增失敗")
        End If



        Return blnCheck
    End Function
    '修改
    Public Function UpdateData() As Boolean
        Dim blnCheck As Boolean = False

        Dim keyvalue, sqlstr, retstr, updstr As String

        keyvalue = lblAccno.Text

        Master.ADO.GenUpdsql("accno", vxtAccno.Text, "T")
        Master.ADO.GenUpdsql("accname", Trim(txtAccname.Text), "U")
        Master.ADO.GenUpdsql("belong", Trim(txtBelong.Text.ToUpper), "T")
        Master.ADO.GenUpdsql("bank", Trim(txtBank.Text), "T")
        Master.ADO.GenUpdsql("unit", Trim(txtUnit.Text), "T")
        Master.ADO.GenUpdsql("staff_no", Trim(txtStaff_no.Text), "T")
        Master.ADO.GenUpdsql("account_no", Trim(txtAccount_no.Text), "T")
        Master.ADO.GenUpdsql("bookaccno", vxtBook.Text, "T")
        If ViewState("blnArea") = True Then Master.ADO.GenUpdsql("area", Trim(txtArea.Text), "T")
        sqlstr = "update accname set " & Master.ADO.genupdfunc & " where accno='" & keyvalue & "'"

        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            blnCheck = True
        Else
            MessageBx("更新失敗")
        End If

        keyvalue = Trim(lblkey.Text)


        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False


        Dim keyvalue, sqlstr, retstr As String
        keyvalue = lblAccno.Text

        sqlstr = "delete from accname where accno='" & keyvalue & "'"
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
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作刪除資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
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
        Dim objDropDownList() As DropDownList = {cboAccno}
        Dim objRadioButtonList() As RadioButtonList = {}

        'Master.Controller.Main_Control(objMainTextBox, objMainDropDownList, objMainRadioButtonList, Status)
        'Master.Controller.TextBox_Control(objTextBox, Status) : Master.Controller.TextBox_Clear(objTextBox, Status)
        'Master.Controller.DropDownList_Control(objDropDownList, Status) : Master.Controller.DropDownList_Clear(objDropDownList, Status)
        'Master.Controller.RadioButtonList_Control(objRadioButtonList, Status) : Master.Controller.RadioButtonList_Clear(objRadioButtonList, Status)



        '其他控制項
        Select Case Status
            Case 0 '一般模式

            Case 1 '新增模式
                TabContainer1.ActiveTabIndex = 1

            Case 2 '修改模式
                TabContainer1.ActiveTabIndex = 1


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
        Data_Load(strKey1) '載入資料
    End Sub
    '請購推算檔
    Sub Data_Load(ByVal strKey1 As String)
        Dim intI, SumUp As Integer
        Dim strI, strColumn1, strColumn2 As String

        '開啟查詢
        objCon99 = New SqlConnection(DNS_ACC)
        objCon99.Open()

        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT   *  FROM  accname where " & _
                 "  accno = '" & strKey1 & "'"

        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            lblAccno.Text = Trim(objDR99("accno").ToString)
            vxtAccno.Text = Trim(objDR99("accno").ToString)
            txtAccname.Text = Trim(objDR99("accname").ToString)
            txtBelong.Text = Trim(objDR99("belong").ToString)
            txtUnit.Text = Trim(objDR99("unit").ToString)
            txtBank.Text = Trim(objDR99("bank").ToString)
            txtStaff_no.Text = Trim(objDR99("staff_no").ToString)
            txtAccount_no.Text = Trim(objDR99("account_no").ToString)
            vxtBook.Text = Trim(objDR99("bookaccno").ToString)
            If ViewState("blnArea") = True Then
                txtArea.Text = Trim(objDR99("area").ToString)
            End If

            'ViewState("intBGCNO") = Trim(objDR99("BGCNO").ToString)
            'lblBgno.Text = Trim(objDR99("BGCNO").ToString)
            'lblYear.Text = Trim(objDR99("ACCYEAR").ToString)
            'lblDatec.Text = Master.Models.strStrToDate(Trim(objDR99("DATEC").ToString))

        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標

    End Sub

#End Region

#Region "物件選擇異動值"

#End Region

    Protected Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        TabContainer1.Enabled = True
        TabContainer1.ActiveTabIndex = 0

        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
    End Sub

    Protected Sub btnAutoAdd_Click(sender As Object, e As EventArgs) Handles btnAutoAdd.Click
        If txtEaccno.Text = "" Or txtSEngno.Text = "" Or txtEEngno.Text = "" Then Exit Sub
        Dim tempds As DataSet
        sqlstr = "select engno, engname FROM enf010 where engno>='" & Trim(txtSEngno.Text) & "' and engno<='" & Trim(txtEEngno.Text) & "'"
        tempds = Master.ADO.openmember(DNS_ACC, "enf010", sqlstr)
        Dim intI, intJ As Integer
        intJ = 0
        Dim retstr As String
        For intI = 0 To (tempds.Tables(0).Rows.Count - 1)
            With tempds.Tables(0).Rows(intI)
                If Master.ADO.nz(.Item("engname"), "") <> "" Then
                    Master.ADO.GenInsSql("accno", Mid(Trim(txtEaccno.Text) & Space(9), 1, 9) & .Item("engno"), "T")
                    Master.ADO.GenInsSql("accname", .Item("engname"), "U")
                    Master.ADO.GenInsSql("staff_no", txtStaffno.Text, "T")
                    Master.ADO.GenInsSql("account_no", txtAccountno.Text, "T")
                    Master.ADO.GenInsSql("bookaccno", Mid(Trim(txtEaccno.Text) & Space(9), 1, 9) & .Item("engno"), "T")
                    sqlstr = "insert into accname " & Master.ADO.GenInsFunc
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    If retstr = "sqlok" Then intJ += 1
                End If
            End With
        Next
        MessageBx("處理完畢,  新增" & Str(intJ) & "筆")
        TabContainer1.ActiveTabIndex = 0
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
    End Sub

    Protected Sub btnAutoExit_Click(sender As Object, e As EventArgs) Handles btnAutoExit.Click
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click

    End Sub

    Protected Sub btnBook_Click(sender As Object, e As EventArgs) Handles btnBook.Click
        Dim sqlstr, qstr, retstr, strAccno, strBookAccno As String
        Dim intJ, intI, intLen, intCount As Integer
        sqlstr = "UPDATE ACCNAME SET BOOKACCNO=ACCNO where accno<>bookaccno AND belong<>'B'"   '先將會計科目全搬入記帳科目
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        lblFinish.Text = ""
        lblError.Text = ""

        '將所有會計科目置combobox   
        Dim i As Integer
        sqlstr = "SELECT accno,bookaccno FROM ACCNAME WHERE BELONG<>'B'"
        mydataset = Master.ADO.openmember(DNS_ACC, "ACCNAME", sqlstr)
        Master.Controller.objDropDownListOptionEX(cboAccno, DNS_ACC, sqlstr, "accno", "bookaccno", 0)

        '處理預算科目
        intCount = 0  '計算更正預算科目個數
        sqlstr = "SELECT accno,bookaccno FROM accname where belong='B'"
        mydataset = Master.ADO.openmember(DNS_ACC, "accnamet", sqlstr)
        For intI = 0 To (mydataset.Tables("accnamet").Rows.Count - 1)
            strAccno = mydataset.Tables("accnamet").Rows(intI).Item(0)
            strBookAccno = ""
            For intJ = 7 To 4 Step -1
                Select Case intJ
                    Case 7
                        intLen = 16
                    Case 6
                        intLen = 9
                    Case 5
                        intLen = 7
                    Case 4
                        intLen = 5
                End Select
                cboAccno.SelectedValue = Mid(strAccno, 1, intLen)
                If Mid(cboAccno.SelectedValue, 1, 5) = Mid(strAccno, 1, 5) Then 'value=1 表示找到會計科目第一筆(1:資產);不是1,表示找到了.
                    strBookAccno = cboAccno.SelectedValue
                    lblFinish.Text = strBookAccno & "  TO " & strAccno
                    Exit For
                End If
                If intJ = 4 And strBookAccno = "" Then lblError.Text += "錯誤:" & strAccno
            Next
            If strBookAccno <> mydataset.Tables("accnamet").Rows(intI).Item(1) Then
                sqlstr = "update accname set bookaccno = " & strBookAccno & " where accno='" & strAccno & "'"
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                intCount += 1
            End If
        Next
        mydataset.Clear()
        lblFinish.Text = "更正完成件數=" & Str(intCount)
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
    End Sub

    Protected Sub btbChange_Click(sender As Object, e As EventArgs) Handles btbChange.Click
        If Len(Trim(txtNewNo.Text)) = 0 Or Len(Trim(txtOldNo.Text)) = 0 Then
            MessageBx("請輸入員工編號")
            Exit Sub
        End If
        'If MsgBox("科目介於" & vxtStartNo.getTrimData() & "至" & vxtEndNo.getTrimData() & "修正" & txtOldNo.Text & "為" & txtNewNo.Text, MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
        Dim retstr As String
        If rdoStaff.Checked Then
            sqlstr = "update accname set staff_no='" & txtNewNo.Text & "' where staff_no='" & txtOldNo.Text & "'"
        Else
            sqlstr = "update accname set account_no='" & txtNewNo.Text & "' where account_no='" & txtOldNo.Text & "'"
        End If
        sqlstr &= " and accno between '" & vxtStartNo.Text & "' and '" & vxtEndNo.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then MessageBx("處理完成")
        TabContainer1.ActiveTabIndex = 0
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        'End If
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        TabContainer1.ActiveTabIndex = 0
    End Sub
End Class