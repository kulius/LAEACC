Imports System.Data
Imports System.Data.SqlClient

Public Class BGF010
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
        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        '先由會計科目找異動可否控制碼
        sqlstr = "SELECT * FROM ACCNAME WHERE ACCNO='5'"
        mydataset = Master.ADO.openmember(DNS_ACC, "ACCNAME", sqlstr)
        If mydataset.Tables(0).Rows.Count <= 0 Then
            MessageBx("貴會會計科目檔少了科目=5的科目,請主計人員先行補上 ")
        End If
        If Mid(Master.ADO.nz(mydataset.Tables(0).Rows(0).Item("account_no"), ""), 1, 1) = "Y" Then
            ViewState("strYes") = "Y"
        Else
            ViewState("strYes") = "N"
        End If

        If Mid(Session("UserUnit"), 1, 2) = "05" Then
            sqlstr = "SELECT accno,accno+accname as accnamet from accname where outyear=0 "
        Else
            sqlstr = "SELECT accno,accno+accname as accnamet from accname where staff_no='" & Session("USERID") & "' and outyear=0"
        End If

        Master.Controller.objDropDownListOptionEX(cboAccno, DNS_ACC, sqlstr, "accno", "accnamet", 0)

        nudYear.Text = Session("sYear")
        vxtStartNo.text = "1"    '起值
        vxtEndNo.Text = "59"      '迄值
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
    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim strMySearch As String = ""

        '開啟查詢*****        
        '項目
        If Mid(Session("UserUnit"), 1, 2) = "05" Then
            sqlstr = "SELECT accno,accno+accname as accnamet from accname where outyear=0 "
            If vxtStartNo.Text <> "" Then sqlstr &= " AND ACCNO LIKE '" & vxtStartNo.Text & "%'"
        Else
            sqlstr = "SELECT accno,accno+accname as accnamet from accname where staff_no='" & Session("USERID") & "' and outyear=0"
            If vxtStartNo.Text <> "" Then sqlstr &= " AND ACCNO LIKE '" & vxtStartNo.Text & "%'"
        End If

        Master.Controller.objDropDownListOptionEX(cboAccno, DNS_ACC, sqlstr, "accno", "accnamet", 0)


        '初始化*****
        UCBase1.SetButtons()                         '初始化控制鍵

        ViewState("MySearch") = strMySearch          '查詢值記錄
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '開啟查詢
        TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤
    End Sub

    '清除條件
    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        '還原預設值*****
        nudYear.Text = Session("sYear")
        'vxtStartNo.Text = ""
        'vxtEndNo.Text = ""

        '初始化*****
        UCBase1.SetButtons() '初始化控制鍵
    End Sub
#End Region

#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        '開啟查詢
        'strSSQL = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.amt1, a.remark,"
        'strSSQL &= "CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')' "
        'strSSQL &= " WHEN len(a.accno)<>17 THEN b.accname END AS accname, a.kind, a.subject FROM BGF020 a "
        'strSSQL &= " INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO"
        'strSSQL &= " LEFT JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO and len(a.accno)=17"
        'strSSQL &= " WHERE a.CLOSEMARK <> 'Y' and a.date2 is null"
        'strSSQL &= strSearch
        'strSSQL &= IIf(Session("USERID") = "admin", "", " and b.STAFF_NO = '" & Session("USERID") & "'")
        'strSSQL &= " ORDER BY " & strOrder & " " & strSortType

        sqlstr = "SELECT a.bg1+a.bg2+a.bg3+a.bg4+a.bg5 as bgnet, a.*, " & _
                 "CASE WHEN len(a.accno) = 17 THEN c.ACCNAME+'-'+b.accname " & _
                 " WHEN len(a.accno) <> 17 THEN b.accname END AS accname" & _
                 " FROM  BGF010 a" & _
                 " LEFT OUTER JOIN accname b ON a.accno = b.accno" & _
                 " left outer join accname c ON LEFT(a.ACCNO, 16) = c.ACCNO and len(a.accno)=17 " & _
                 " WHERE accyear=" & nudYear.Text & " and a.accno>='" & _
                  Trim(vxtStartNo.Text) & "' and a.accno<='" & Trim(vxtEndNo.Text) & "'"
        If Mid(Session("UserUnit"), 1, 2) = "05" Then   '主計人員
            sqlstr = sqlstr & " ORDER BY a.accyear,a.accno"
        Else
            sqlstr = sqlstr & " and b.STAFF_NO = '" & Session("USERID") & _
                     "' ORDER BY a.accyear,a.accno"

            If ViewState("strYes") <> "Y" Then
                UCBase1.Visible = False '不可異動
                lblMsgMod.Text = "目前主計室控制業務單位不可修改預算資料"
            Else
                UCBase1.Visible = True '可異動
            End If
        End If

        lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
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
            TabContainer1.ActiveTabIndex = 0
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

        Dim keyvalue, sqlstr, retstr, updstr, TDC As String

        sqlstr = "SELECT accno FROM bgf010 where accyear=" & txtAccYear.Text & " and accno='" & cboAccno.SelectedValue & "'"
        psDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If psDataSet.Tables("accname").Rows.Count > 0 Then
            MessageBx("資料重複, " & "accyear=" & txtAccYear.Text & " and accno='" & cboAccno.SelectedValue & "'")
            Exit Function
        End If
        TDC = IIf(Mid(cboAccno.SelectedValue, 1, 1) > "1" And Mid(cboAccno.SelectedValue, 1, 1) < "5", "2", "1")
        Master.ADO.GenInsSql("accyear", txtAccYear.Text, "N")
        Master.ADO.GenInsSql("accno", cboAccno.SelectedValue, "T")
        Master.ADO.GenInsSql("bg1", txtBg1.Text, "N")
        Master.ADO.GenInsSql("bg2", txtBg2.Text, "N")
        Master.ADO.GenInsSql("bg3", txtBg3.Text, "N")
        Master.ADO.GenInsSql("bg4", txtBg4.Text, "N")
        Master.ADO.GenInsSql("bg5", txtBg5.Text, "N")
        Master.ADO.GenInsSql("up1", txtUp1.Text, "N")
        Master.ADO.GenInsSql("up2", txtUp2.Text, "N")
        Master.ADO.GenInsSql("up3", txtUp3.Text, "N")
        Master.ADO.GenInsSql("up4", txtUp4.Text, "N")
        Master.ADO.GenInsSql("up5", txtUp5.Text, "N")
        Master.ADO.GenInsSql("DC", TDC, "T")
        Master.ADO.GenInsSql("flow", txtFlow.Text, "T")
        Master.ADO.GenInsSql("ctrl", txtCtrl.Text, "T")
        Master.ADO.GenInsSql("unit", txtUnit.Text, "T")
        Master.ADO.GenInsSql("engno", txtEngno.Text, "T")
        Master.ADO.GenInsSql("totper", 0, "N")   '新增時總請購及開支=0
        Master.ADO.GenInsSql("totuse", 0, "N")
        Master.ADO.GenInsSql("systemdate", Format(Now(), "yyyy-MM-dd HH:mm:ss"), "D")
        sqlstr = "insert into BGF010 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        If retstr = "sqlok" Then
            blnCheck = True
            MessageBx("新增成功")
        Else
            MessageBx("新增失敗" + sqlstr)
        End If

        Return blnCheck
    End Function
    '修改
    Public Function UpdateData() As Boolean
        Dim blnCheck As Boolean = False

        Dim keyvalue, sqlstr, retstr, updstr, TDC As String

        keyvalue = Trim(lblkey.Text)
        TDC = IIf(Mid(cboAccno.SelectedValue, 1, 1) > "1" And Mid(cboAccno.SelectedValue, 1, 1) < "5", "2", "1")
        'RecMove1.GenUpdsql("accyear", txtAccYear.Text, "N")
        'RecMove1.GenUpdsql("accno", cboAccno.SelectedValue, "T")
        Master.ADO.GenUpdsql("bg1", txtBg1.Text, "N")
        Master.ADO.GenUpdsql("bg2", txtBg2.Text, "N")
        Master.ADO.GenUpdsql("bg3", txtBg3.Text, "N")
        Master.ADO.GenUpdsql("bg4", txtBg4.Text, "N")
        Master.ADO.GenUpdsql("bg5", txtBg5.Text, "N")
        Master.ADO.GenUpdsql("up1", txtUp1.Text, "N")
        Master.ADO.GenUpdsql("up2", txtUp2.Text, "N")
        Master.ADO.GenUpdsql("up3", txtUp3.Text, "N")
        Master.ADO.GenUpdsql("up4", txtUp4.Text, "N")
        Master.ADO.GenUpdsql("up5", txtUp5.Text, "N")
        Master.ADO.GenUpdsql("unit", txtUnit.Text, "T")
        Master.ADO.GenUpdsql("DC", TDC, "T")
        Master.ADO.GenUpdsql("flow", txtFlow.Text, "T")
        Master.ADO.GenUpdsql("ctrl", txtCtrl.Text, "T")
        Master.ADO.GenUpdsql("engno", txtEngno.Text, "T")
        Master.ADO.GenUpdsql("totper", txtTotper.Text, "N")
        Master.ADO.GenUpdsql("totuse", txtTotuse.Text, "N")
        Master.ADO.GenUpdsql("systemdate", Format(Now(), "yyyy-MM-dd HH:mm:ss"), "D")

        sqlstr = "update BGF010 set " & Master.ADO.genupdfunc & " where autono=" & keyvalue
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            blnCheck = True
            MessageBx("更新成功")
        Else
            MessageBx("更新失敗" & sqlstr)
        End If

        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False

        Dim keyvalue, sqlstr, retstr As String
        Dim tempds As DataSet

        keyvalue = Trim(lblkey.Text)

        sqlstr = "select count(*) as recno from bgf020 where accyear=" & lblYear.Text & " and accno='" & lblAccno.Text & "'"
        tempds = Master.ADO.openmember(DNS_ACC, "recno", sqlstr)
        If tempds.Tables(0).Rows(0).Item(0) > 0 Then  '該科目已有開支資料
            MsgBox("該科目已有開支資料,筆數=" & tempds.Tables(0).Rows(0).Item(0))
            Exit Sub
        End If
        sqlstr = "delete from BGF010 where autono=" & keyvalue
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

                Dim intI, SumUp As Integer
                Dim strI, strColumn1, strColumn2 As String

                txtAccYear.Text = Session("sYear")
                lblYear.Text = Session("sYear")
                Master.Controller.objDropDownListOptionCK(cboAccno, "")
                lblAccno.Text = cboAccno.SelectedValue
                txtUnit.Text = ""
                txtTotper.Text = "0"
                txtTotuse.Text = "0"
                txtFlow.Text = ""
                txtCtrl.Text = ""
                txtEngno.Text = ""
                lblkey.Text = ""

                SumUp = 0
                Dim box As TextBox
                Dim lbl As Label
                For intI = 1 To 5
                    strI = Format(intI, "0")
                    strColumn1 = "bg" & strI
                    strColumn2 = "up" & strI
                    box = TabPanel2.FindControl("txtbg" & strI)
                    box.Text = "0"
                    box = TabPanel2.FindControl("txtup" & strI)
                    box.Text = "0"
                    lbl = TabPanel2.FindControl("lblNet" & strI)
                    lbl.Text = "0"
                    SumUp += 0
                Next
                lblSumBg.Text = "0.00"
                lblSumUp.Text = "0.00"
                lblSumNet.Text = "0.00"

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

        sqlstr = "SELECT a.bg1+a.bg2+a.bg3+a.bg4+a.bg5 as bgnet, a.*, " & _
         "CASE WHEN len(a.accno) = 17 THEN c.ACCNAME+'-'+b.accname " & _
         " WHEN len(a.accno) <> 17 THEN b.accname END AS accname" & _
         " FROM  BGF010 a" & _
         " LEFT OUTER JOIN accname b ON a.accno = b.accno" & _
         " left outer join accname c ON LEFT(a.ACCNO, 16) = c.ACCNO and len(a.accno)=17 " & _
         " WHERE autono = '" & strKey1 & "'"

        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            txtAccYear.Text = Trim(objDR99("ACCYEAR").ToString)
            lblYear.Text = txtAccYear.Text
            Master.Controller.objDropDownListOptionCK(cboAccno, Trim(objDR99("ACCNO").ToString))
            lblAccno.Text = cboAccno.SelectedValue
            txtUnit.Text = Trim(objDR99("UNIT").ToString)
            txtTotper.Text = Format(Master.ADO.nz(objDR99("totper"), 0), "###,###,##0")
            txtTotuse.Text = Format(Master.ADO.nz(objDR99("totuse"), 0), "###,###,##0")
            txtFlow.Text = Trim(objDR99("flow").ToString)
            txtCtrl.Text = Trim(objDR99("ctrl").ToString)
            txtEngno.Text = Trim(objDR99("engno").ToString)
            lblkey.Text = Trim(objDR99("autono").ToString)

            SumUp = 0
            Dim box As TextBox
            Dim lbl As Label
            For intI = 1 To 5
                strI = Format(intI, "0")
                strColumn1 = "bg" & strI
                strColumn2 = "up" & strI
                box = TabPanel2.FindControl("txtbg" & strI)
                box.Text = FormatNumber(Master.ADO.nz(objDR99(strColumn1), 0), 0)
                box = TabPanel2.FindControl("txtup" & strI)
                box.Text = FormatNumber(Master.ADO.nz(objDR99(strColumn2), 0), 0)
                lbl = TabPanel2.FindControl("lblNet" & strI)
                lbl.Text = FormatNumber(Master.ADO.nz(objDR99(strColumn1), 0) + Master.ADO.nz(objDR99(strColumn2), 0), 2)

                SumUp += FormatNumber(Master.ADO.nz(objDR99(strColumn2), 0), 2)
            Next
            lblSumBg.Text = FormatNumber(objDR99("bgnet").ToString, 2)
            lblSumUp.Text = FormatNumber(SumUp, 2)
            lblSumNet.Text = FormatNumber(Master.ADO.nz(objDR99("bgnet"), 0) + SumUp, 2)
            lbldate.Text = objDR99("systemdate").ToString
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


    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        txtBg1.Text = "0"
        txtBg2.Text = "0"
        txtBg3.Text = "0"
        txtBg4.Text = "0"
    End Sub

    Protected Sub btnFour_Click(sender As Object, e As EventArgs) Handles btnFour.Click

        Dim sumbg, sbg As Integer
        sumbg = Master.Models.ValComa(txtBg1.Text) + Master.Models.ValComa(txtBg2.Text) + Master.Models.ValComa(txtBg3.Text) + Master.Models.ValComa(txtBg4.Text) - Master.Models.ValComa(txtBg5.Text)
        sbg = Math.Round(sumbg / 4, 0)
        txtBg1.Text = FormatNumber(sbg, 0)
        txtBg2.Text = FormatNumber(sbg, 0)
        txtBg3.Text = FormatNumber(sbg, 0)
        txtBg4.Text = FormatNumber(sumbg - (sbg * 3), 0)
    End Sub
End Class