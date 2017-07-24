Imports System.Data
Imports System.Data.SqlClient

Public Class ACCBG
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

        nudYear.Text = Session("sYear")
        vxtStartNo.Text = "1"    '起值
        vxtEndNo.Text = "59"      '迄值

        If Session("UnitTitle").indexof("中") >= 0 And Mid(Session("Userunit"), 1, 2) = "05" Then
            btnTrans.Enabled = False '台中主計人員不要預算自動轉入
        End If


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
        ' If nudYear.SelectedValue <> "" Then strMySearch &= " AND a.ACCYEAR LIKE '%" & S_ACCYEAR.SelectedValue & "%'"


        '初始化*****
        UCBase1.SetButtons()                         '初始化控制鍵

        ViewState("MySearch") = strMySearch          '查詢值記錄
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '開啟查詢
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

        sqlstr = "SELECT a.bg1+a.bg2+a.bg3+a.bg4+a.bg5 as bgnet, a.*, b.accname  FROM  ACCBG a LEFT OUTER JOIN accname b" & _
         " ON a.accno = b.accno WHERE accyear=" & nudYear.Text & " and a.accno>='" & _
         Trim(vxtStartNo.Text) & "' and a.accno<='" & Trim(vxtEndNo.Text) & "' ORDER BY a.accyear,a.accno"

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

        Master.ADO.GenInsSql("accyear", txtAccYear.Text, "N")
        Master.ADO.GenInsSql("accno", vxtAccno.Text, "T")
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
        Master.ADO.GenInsSql("deamt", txtDeamt.Text, "N")
        Master.ADO.GenInsSql("cramt", txtCramt.Text, "N")
        sqlstr = "insert into ACCBG " & Master.ADO.GenInsFunc
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

        Master.ADO.GenUpdsql("accyear", txtAccYear.Text, "N")
        Master.ADO.GenUpdsql("accno", vxtAccno.Text, "T")
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
        Master.ADO.GenUpdsql("deamt", txtDeamt.Text, "N")
        Master.ADO.GenUpdsql("cramt", txtCramt.Text, "N")
        sqlstr = "update ACCBG set " & Master.ADO.genupdfunc & " where autono=" & keyvalue
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            blnCheck = True
            MessageBx("更新成功")
        End If


        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False

        Dim keyvalue, sqlstr, retstr As String
        Dim tempds As DataSet

        keyvalue = Trim(lblkey.Text)

        sqlstr = "delete from ACCBG where autono=" & keyvalue
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
        Dim objDropDownList() As DropDownList = {}
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

        sqlstr = "SELECT a.bg1+a.bg2+a.bg3+a.bg4+a.bg5 as bgnet, a.*, b.accname  FROM  ACCBG a LEFT OUTER JOIN accname b" & _
                 " ON a.accno = b.accno " & _
                 " WHERE autono = '" & strKey1 & "'"

        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then


            txtAccYear.Text = objDR99("ACCYEAR").ToString
            vxtAccno.Text = objDR99("accno").ToString
            lblAccname.Text = IIf(IsDBNull(objDR99("accname").ToString), " ", objDR99("accname").ToString)

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
                lbl.Text = Master.ADO.nz(objDR99(strColumn1), 0) + Master.ADO.nz(objDR99(strColumn2), 0)

                SumUp += Master.ADO.nz(objDR99(strColumn2), 0)
            Next
            lblSumBg.Text = objDR99("bgnet").ToString
            lblSumUp.Text = FormatNumber(SumUp, 2)
            lblSumNet.Text = FormatNumber(Master.ADO.nz(objDR99("bgnet"), 0) + SumUp, 2)
            txtDeamt.Text = FormatNumber(objDR99("deamt").ToString)
            txtCramt.Text = FormatNumber(objDR99("cramt").ToString)
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


    Protected Sub btnTrans_Click(sender As Object, e As EventArgs) Handles btnTrans.Click
        Dim retstr As String
        Dim tempDs As DataSet
        'sqlstr = "select * from accbg where accyear=" & txtYear.Text
        'tempDs = Master.ADO.openmember(DNS_ACC, "bgf020", sqlstr)
        'If tempDs.Tables(0).Rows.Count > 0 Then
        '    If MsgBox("你確定要刪除accbg 年度=" & txtYear.Text & ",再由bgp020轉入嗎?", MsgBoxStyle.YesNo) <> MsgBoxResult.Yes Then
        '        Exit Sub
        '    End If
        'End If
        'tempDs = Nothing
        sqlstr = "delete from accbg where accyear=" & txtYear.Text
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        sqlstr = "INSERT INTO ACCBG " & _
                 "SELECT " & txtYear.Text & " AS accyear, accno, bg1, bg2, bg3, bg4, bg5, 0 AS up1, 0 AS up2, 0 AS up3, " & _
                 " 0 AS up4, 0 AS up5,'' AS remark, 0 AS deamt, 0 AS cramt FROM BGP020 WHERE userid = '全部' and len(accno)<=9 order by accno"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("轉入完成")
        End If
        btnTrans.Enabled = False

        'btnCancel.Text = "結束"
    End Sub

    Protected Sub btnRunAcu_Click(sender As Object, e As EventArgs) Handles btnRunAcu.Click
        Dim retstr, strTemp As String

        '先delete   accbg 
        sqlstr = "delete from accbg where accyear=" & txtAcuYear.Text & " and len(accno)<=5 "
        If rdoR.Checked Then
            strTemp = " and left(accno,1)='4' "
        End If
        If rdoP.Checked Then
            strTemp = " and left(accno,1)='5' "
        End If
        If rdoRP.Checked Then
            strTemp = " and left(accno,1)>='4' "
        End If
        sqlstr += strTemp
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '統計至四級
        sqlstr = "INSERT INTO ACCBG select a.accyear, a.accno, a.bg1, a.bg2, a.bg3, a.bg4, a.bg5, " & _
                 "a.up1, a.up2, a.up3, a.up4, a.up5, '' as remark, a.deamt, a.cramt from " & _
                 "(SELECT accyear, substring(accno, 1, 5) AS accno,  " & _
                 "SUM(bg1) AS bg1, SUM(bg2) AS bg2, SUM(bg3) AS bg3, SUM(bg4) AS bg4, sum(bg5) as bg5, " & _
                 "SUM(up1) AS up1, SUM(up2) AS up2, SUM(up3) AS up3, SUM(up4) AS up4, sum(up5) as up5, " & _
                 "SUM(deamt) AS deamt, SUM(cramt) AS cramt from accbg " & _
                 "where accyear=" & txtAcuYear.Text & " and len(accno)=7 " & strTemp & _
                 "GROUP BY accyear, substring(accno, 1, 5)) a "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("統計四級 error " & sqlstr)
            Exit Sub
        End If
        '統計至三級
        sqlstr = "INSERT INTO ACCBG select a.accyear, a.accno, a.bg1, a.bg2, a.bg3, a.bg4, a.bg5, " & _
                 "a.up1, a.up2, a.up3, a.up4, a.up5, '' as remark, a.deamt, a.cramt from " & _
                 "(SELECT accyear, substring(accno, 1, 3) AS accno,  " & _
                 "SUM(bg1) AS bg1, SUM(bg2) AS bg2, SUM(bg3) AS bg3, SUM(bg4) AS bg4, sum(bg5) as bg5, " & _
                 "SUM(up1) AS up1, SUM(up2) AS up2, SUM(up3) AS up3, SUM(up4) AS up4, sum(up5) as up5, " & _
                 "SUM(deamt) AS deamt, SUM(cramt) AS cramt from accbg " & _
                 "where accyear=" & txtAcuYear.Text & " and len(accno)=5 " & strTemp & _
                 "GROUP BY accyear, substring(accno, 1, 3)) a "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("統計三級 error " & sqlstr)
            Exit Sub
        End If        '統計至二級
        sqlstr = "INSERT INTO ACCBG select a.accyear, a.accno, a.bg1, a.bg2, a.bg3, a.bg4, a.bg5, " & _
                 "a.up1, a.up2, a.up3, a.up4, a.up5, '' as remark, a.deamt, a.cramt from " & _
                 "(SELECT accyear, substring(accno, 1, 2) AS accno,  " & _
                 "SUM(bg1) AS bg1, SUM(bg2) AS bg2, SUM(bg3) AS bg3, SUM(bg4) AS bg4, sum(bg5) as bg5, " & _
                 "SUM(up1) AS up1, SUM(up2) AS up2, SUM(up3) AS up3, SUM(up4) AS up4, sum(up5) as up5, " & _
                 "SUM(deamt) AS deamt, SUM(cramt) AS cramt from accbg " & _
                 "where accyear=" & txtAcuYear.Text & " and len(accno)=3 " & strTemp & _
                 "GROUP BY accyear, substring(accno, 1, 2)) a "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("統計二級 error " & sqlstr)
            Exit Sub
        End If
        MessageBx("作業完成")
    End Sub

    Protected Sub btnTransTo_Click(sender As Object, e As EventArgs) Handles btnTransTo.Click
        Dim retstr, tempDate, strRemark As String
        Dim decAmt As Decimal
        'If MsgBox("你確定要刪除accbgbook 年度=" & txtYearTo.Text & IIf(rdbRev.Checked, "收入", "收入") & _
        '   ",再由accbg轉入嗎?", MsgBoxStyle.YesNo) <> MsgBoxResult.Yes Then
        '    Exit Sub
        'End If
        '先清該年度收入or收入資料
        sqlstr = "delete from accbgbook where accyear=" & txtYearTo.Text & " and left(accno,1)='" & IIf(rdbRev.Checked, "4", "5") & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        '由accbg逐筆按四季資料insert至accbgbook
        sqlstr = "SELECT * from accbg where accyear=" & txtYearTo.Text & " and left(accno,1)='" & IIf(rdbRev.Checked, "4", "5") & _
                 "' and len(accno)>=5"
        mydataset = Master.ADO.openmember(DNS_ACC, "ACCBG", sqlstr)
        With mydataset.Tables("accbg")
            For intI As Integer = 0 To .Rows.Count - 1
                For intQ As Integer = 1 To 4
                    If intQ = 1 Then
                        tempDate = txtYearTo.Text & "/1/1"
                        decAmt = Master.ADO.nz(.Rows(intI).Item("bg1"), 0) + Master.ADO.nz(.Rows(intI).Item("up1"), 0)
                        strRemark = "第一季預算分配數"
                    End If
                    If intQ = 2 Then
                        tempDate = txtYearTo.Text & "/4/1"
                        decAmt = Master.ADO.nz(.Rows(intI).Item("bg2"), 0) + Master.ADO.nz(.Rows(intI).Item("up2"), 0)
                        strRemark = "第二季預算分配數"
                    End If
                    If intQ = 3 Then
                        tempDate = txtYearTo.Text & "/7/1"
                        decAmt = Master.ADO.nz(.Rows(intI).Item("bg3"), 0) + Master.ADO.nz(.Rows(intI).Item("up3"), 0)
                        strRemark = "第三季預算分配數"
                    End If
                    If intQ = 4 Then
                        tempDate = txtYearTo.Text & "/10/1"
                        decAmt = Master.ADO.nz(.Rows(intI).Item("bg4"), 0) + Master.ADO.nz(.Rows(intI).Item("up4"), 0)
                        strRemark = "第四季預算分配數"
                    End If
                    If decAmt <> 0 Then   '有預算分配數才insert 
                        Master.ADO.GenInsSql("accyear", txtYearTo.Text, "N")
                        Master.ADO.GenInsSql("accno", .Rows(intI).Item("accno"), "T")
                        Master.ADO.GenInsSql("date_2", tempDate, "D")
                        Master.ADO.GenInsSql("kind", " ", "T")
                        Master.ADO.GenInsSql("NO_2_NO", 0, "N")
                        Master.ADO.GenInsSql("DC", " ", "T")
                        Master.ADO.GenInsSql("REMARK", strRemark, "T")
                        Master.ADO.GenInsSql("amt", decAmt, "N")
                        Master.ADO.GenInsSql("COTN_CODE", " ", "T")
                        sqlstr = "insert into ACCBGBOOK " & Master.ADO.GenInsFunc
                        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    End If
                Next
            Next
        End With

        MessageBx("轉出完成")
        'btnTransTo.Enabled = False
        'btnCancelTo.Text = "結束"
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        sqlstr = "SELECT  a.accyear as 年度,a.accno as 會計科目, b.accname as 會計科目名稱, "
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.bg1),1) as 第一季預算數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.bg2),1) as 第二季預算數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.bg3),1) as 第三季預算數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.bg4),1) as 第四季預算數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.bg5),1) as 預算保留數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.up1),1) as 第一季變動數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.up2),1) as 第二季變動數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.up3),1) as 第三季變動數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.up4),1) as 第四季變動數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.up5),1) as 保留變動數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.deamt),1) as 借方增加,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.cramt),1) as 貸方增加,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.bg1+a.bg2+a.bg3+a.bg4+a.bg5),1) as 全年預算數,"
        sqlstr &= " Convert(Varchar(24),CONVERT(money,a.up1+a.up2+a.up3+a.up4+a.up5),1) as 全年變動數"

        sqlstr &= " FROM  ACCBG a"
        sqlstr &= " LEFT OUTER JOIN accname b ON a.accno = b.accno"

        sqlstr &= " WHERE accyear=" & nudYear.Text & ""
        sqlstr &= " and a.accno>='" & Trim(vxtStartNo.Text) & "'"
        sqlstr &= " and a.accno<='" & Trim(vxtEndNo.Text) & "'"
        sqlstr &= " ORDER BY a.accyear,a.accno"

        mydataset = Master.ADO.openmember(DNS_ACC, "Export", sqlstr)

        Master.ExportDataTableToExcel(mydataset.Tables("Export"))
    End Sub
End Class