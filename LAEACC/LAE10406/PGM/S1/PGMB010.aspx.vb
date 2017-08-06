Imports System.Data
Imports System.Data.SqlClient

Public Class PGMB010
    Inherits System.Web.UI.Page

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_PGM As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString

#Region "@資料庫變數@"
    Dim strCSQL As String '查詢數量
    Dim strSSQL As String '查詢資料

    '程序專用*****
    Dim objCon99 As SqlConnection
    Dim objCmd99 As SqlCommand
    Dim objDR99 As SqlDataReader
    Dim strSQL99 As String

    Dim strPage As String = ""    '表單編號
    Dim I As Integer              '累進變數
    Dim strMessage As String = "" '訊息字串
    Dim strIRow, strIValue, strUValue, strWValue As String '資料處理參數(新增欄位；新增資料；異動資料；條件)

    Dim sqlstr As String
    Dim TempDataSet As DataSet
    Dim mydataset, mydataset2 As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱

        txtQueryKindNo.Attributes.Add("onchange", "return ismaxlength(this)")
        txtQueryKindNo2.Attributes.Add("onchange", "return ismaxlength(this)")

        ViewState("MyOrder") = "a.KindNo"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        UCBase1.SetButtons_Visible()                         '初始化控制鍵

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤


        sqlstr = "SELECT * FROM CODE where kind='單位' " & _
                " order by codeno"
        Master.Controller.objDropDownListOptionEX(cboQueryUnit, DNS_PGM, sqlstr, "codeno", "codename", 0)
        Master.Controller.objDropDownListOptionEX(cboUnit, DNS_PGM, sqlstr, "codeno", "codename", 0)

        sqlstr = "SELECT * FROM CODE where kind='使用年限' " & _
                " order by CAST(CODENO AS integer)"
        Master.Controller.objDropDownListOptionEX(cboQueryUseyear, DNS_PGM, sqlstr, "codeno", "codename", 0)
        Master.Controller.objDropDownListOptionEX(cboUseYear, DNS_PGM, sqlstr, "codeno", "codename", 0)

        sqlstr = "SELECT * FROM CODE where kind='材質' " & _
                " order by codeno"
        Master.Controller.objDropDownListOptionEX(cboQueryMaterial, DNS_PGM, sqlstr, "codeno", "codename", 0)
        Master.Controller.objDropDownListOptionEX(cboMaterial, DNS_PGM, sqlstr, "codeno", "codename", 0)
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
            'FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
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

#Region "@主要SQL FILL INSERT DELETE UPDATE FIND@"
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        Dim kindno, kindno2, name, unit, material, useyear As String
        kindno = Trim(txtQueryKindNo.Text)
        kindno2 = Trim(txtQueryKindNo2.Text)
        name = Trim(txtQueryName.Text)
        unit = Trim(cboQueryUnit.Text)
        material = Trim(cboQueryMaterial.Text)
        useyear = Trim(cboQueryUseyear.Text)

        Dim sqlstr, qstr, sortstr As String
        sqlstr = "SELECT * from PPTName  " & _
                 "where 1=1 "

        qstr = " and (kindno >= '" & kindno & "' or '" & kindno & "'='')"
        qstr += " and (kindno <= '" & kindno2 & "' or '" & kindno2 & "'='')"
        qstr += " and (name like '%" & name & "%'  or '" & name & "'='')"
        qstr += " and (unit like '%" & unit & "%' or '" & unit & "'='')"
        qstr += " and (material like '%" & material & "%' or '" & material & "'='')"
        qstr += " and (useyear=" & IIf(useyear = "", 0, useyear) & " or '" & useyear & "'='')"

        sortstr = ""

        If strOrder <> "" Then
            sortstr = IIf(strOrder = "", sortstr, " ORDER BY " & strOrder & " " & strSortType)
        End If

        sqlstr = sqlstr + qstr + sortstr

        lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
        Master.Controller.objDataGrid(DataGridView, lbl_GrdCount, DNS_PGM, sqlstr, "查詢資料檔")

        '判斷是否有值可供選擇*****
        If DataGridView.Items.Count > 0 Then
            Dim txtID As Label = DataGridView.Items(0).FindControl("id")
            txtKey1.Text = txtID.Text
            FindData(txtID.Text)
        End If
    End Sub
    '存檔
    Public Sub SaveData(ByVal PrevTableStatus As Integer)
        Dim SaveStatus As Boolean = False
        Dim blnCheck As Boolean = False

        Dim kindno, name, unit, material, useyear, rowFilter As String
        Dim length As Integer
        kindno = Trim(txtKindNo.Text)
        length = Len(kindno)
        name = Trim(txtName.Text)
        unit = Trim(cboUnit.Text)
        material = Trim(cboMaterial.Text)
        useyear = Trim(cboUseYear.Text)
        If Not IsNumeric(kindno) Then
            MessageBx("編號必須輸入不大於6位的數字")
            PrevTableStatus = "0"
        End If
        If length <> 1 And length <> 3 And length <> 6 Then
            MessageBx("編號長度必須是 1 位或 3 位或是 6 位數字")
            PrevTableStatus = "0"
        End If
        If name = String.Empty Then
            MessageBx("名稱不可空白")
            PrevTableStatus = "0"
        End If
        If Not IsNumeric(useyear) Or useyear.IndexOf("-") >= 0 Then
            MessageBx("使用年限必須輸入正整數")
            PrevTableStatus = "0"
        End If

        Dim pgKind As New PGKindInfo(kindno, name, unit, material, useyear)

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
        If PrevTableStatus = "1" Then SaveStatus = InsertData(pgKind) : ViewState("FileKey") = txtKey1.Text '新增
        If PrevTableStatus = "2" Then SaveStatus = UpdateData(pgKind) : ViewState("FileKey") = txtKey1.Text '修改

        If SaveStatus = True Then
            ViewState("MyStatus") = 0
            SetControls(0)
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            TabContainer1.ActiveTabIndex = 0
            UCBase1.SetButtons()
            FindData(loadkey) '直接查詢值
        End If


        '異動後初始化*****
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作" & IIf(PrevTableStatus = 1, "新增", "修改") & "資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
        Master.ACC.SystemOperate(Session("USERID"), strPage, "PGMB010", ViewState("FileKey"), IIf(PrevTableStatus = 1, "I", "U"), IIf(SaveStatus = True, "S", "F")) '系統操作歷程記錄
    End Sub

    '新增
    Public Function InsertData(pgKind As PGKindInfo) As Boolean
        Dim blnCheck As Boolean = False
        Dim result As Integer
        Dim PKG As PGKindDAL = New PGKindDAL

        result = PKG.Insert(pgKind)
        Select Case result
            Case 1
                MessageBx("新增成功")
                blnCheck = True
            Case -1
                MessageBx("新增失敗，原因是 編號已經存在")
            Case -2
                MessageBx("新增失敗，原因是 編號長度必須是 1 位或 3 位或是 6 位數字")
            Case -3
                MessageBx("新增失敗，原因是 此編號之上沒有母編號，必須先建立母編號之後才可以新增子編號")
            Case Else
                MessageBx("未知的傳回值")
        End Select

        Return blnCheck
    End Function
    '修改
    Public Function UpdateData(pgKind As PGKindInfo) As Boolean
        Dim blnCheck As Boolean = False
        Dim result As Integer
        Dim PKG As PGKindDAL = New PGKindDAL

        result = PKG.Update(pgKind)
        Select Case result
            Case 1
                MessageBx("修改成功")
                blnCheck = True
            Case -1
                MessageBx("修改失敗，原因是 找不到編號")
            Case Else
                MessageBx("未知的傳回值")
        End Select


        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False
        Dim result As Integer
        Dim keyvalue As String
        Dim PKG As PGKindDAL = New PGKindDAL

        keyvalue = Trim(lblkey.Text)

        Dim pgkind As PGKindInfo = New PGKindInfo(keyvalue)
        result = PKG.Delete(pgkind)

        Select Case result
            Case 1
                SaveStatus = True
            Case -1
                MsgBox("刪除失敗，原因是 找不到編號")
            Case -2
                MsgBox("刪除失敗，原因是 此編號之下仍有其他子編號，必須先刪除所有子編號才可以刪除母編號")
            Case -3
                MsgBox("刪除失敗，原因是 財物主檔有資料參考到此編號")
            Case Else
                MsgBox("未知的傳回值")
        End Select

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


    '請購推算檔
    Sub Data_Load(ByVal strKey1 As String)
        Dim intI, SumUp As Integer
        Dim strI, strColumn1, strColumn2 As String

        '開啟查詢
        objCon99 = New SqlConnection(DNS_PGM)
        objCon99.Open()

        Dim sqlstr, qstr, strD, strC As String

        sqlstr = "SELECT * from PPTName " & _
         "where  kindno ='" & strKey1 & "'"


        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            txtKindNo.Text = objDR99("KindNo").ToString
            txtName.Text = objDR99("Name").ToString
            cboUnit.Text = objDR99("Unit").ToString
            cboMaterial.Text = objDR99("Material").ToString
            cboUseYear.Text = objDR99("UseYear").ToString
            lblkey.Text = Trim(objDR99("kindno").ToString)
        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標

    End Sub
    Sub FindData(ByVal strKey1 As String)
        '防呆*****
        If strKey1 = "" Then Exit Sub

        '設定關鍵值*****        
        txtKey1.Text = strKey1 : ViewState("FileKey") = strKey1

        '資料查詢*****
        Data_Load(strKey1) '載入資料
    End Sub
    '還原
    Public Sub ResetData()
        ViewState("MyStatus") = 0
        SetControls(0)
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        FlagMoveSeat(0, ViewState("ItemIndex"))
    End Sub

#End Region
   

    

    Protected Sub btnQuery_Click(sender As Object, e As EventArgs) Handles btnQuery.Click
        ViewState("MyOrder") = ""
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
    End Sub
End Class