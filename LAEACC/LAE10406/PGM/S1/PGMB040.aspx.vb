Imports System.Data
Imports System.Data.SqlClient

Public Class PGMB040
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

        txtQueryPrNo.Attributes.Add("onchange", "return ismaxlength(this)")
        txtQueryPrNo2.Attributes.Add("onchange", "return ismaxlength(this)")
        txtPrNo.Attributes.Add("onchange", "return ismaxlength(this)")


        ViewState("MyOrder") = "prno"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        UCBase1.SetButtons_Visible()                         '初始化控制鍵

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤


        'sqlstr = "SELECT * FROM CODE where kind='單位' " & _
        '        " order by codeno"
        'Master.Controller.objDropDownListOptionEX(cboQueryUnit, DNS_PGM, sqlstr, "codeno", "codename", 0)
        'Master.Controller.objDropDownListOptionEX(cboUnit, DNS_PGM, sqlstr, "codeno", "codename", 0)

        sqlstr = "SELECT * FROM CODE where kind='建物類別' " & _
                " order by CODENO"
        Master.Controller.objDropDownListOptionEX(cboQueryKind, DNS_PGM, sqlstr, "codeno", "codename", 0)
        Master.Controller.objDropDownListOptionEX(cboKind, DNS_PGM, sqlstr, "codeno", "codename", 0)

        'sqlstr = "SELECT * FROM CODE where kind='材質' " & _
        '        " order by codeno"
        'Master.Controller.objDropDownListOptionEX(cboQueryMaterial, DNS_PGM, sqlstr, "codeno", "codename", 0)
        'Master.Controller.objDropDownListOptionEX(cboMaterial, DNS_PGM, sqlstr, "codeno", "codename", 0)
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

        Dim prno, prno2, kind As String

        prno = Trim(txtQueryPrNo.Text)
        prno2 = Trim(txtQueryPrNo2.Text)
        kind = Trim(cboQueryKind.Text)


        '檢查各欄位是否輸入有效值
        If prno > prno2 And (prno <> "" And prno2 <> "") Then
            MessageBx("起始財物編號必須小於終止財物編號")
            txtQueryPrNo.Focus()
            Exit Sub
        End If

        Dim info As New BuildingInfo(prno, prno2, kind, "", "")
        Dim mDT As DataTable = New DataTable
        Dim mDV As DataView = New DataView

        mDT = BuildingDAL.Query(info)
        mDV.Table = mDT
        mDV.Sort = "prno"
        DataGridView.DataSource = mDV
        DataGridView.DataBind()
        lbl_GrdCount.Text = mDV.Count

        If mDV.Count = 0 Then
            MsgBox("找不到資料，請重新設定查詢條件")
        End If

        '判斷是否有值可供選擇*****
        'If DataGridView.Items.Count > 0 Then
        '    Dim txtID As Label = DataGridView.Items(0).FindControl("id")
        '    txtKey1.Text = txtID.Text
        '    FindData(txtID.Text)
        'End If
    End Sub
    '存檔
    Public Sub SaveData(ByVal PrevTableStatus As Integer)
        Dim SaveStatus As Boolean = False
        Dim blnCheck As Boolean = False

        Dim kindno, prno, acno, no, sPDate As String
        Dim builNo, landNo1, landNo2, landArea, part, area1 As String
        Dim area2, area3, endAmt, useyear, stru, tax, remark As String
        Dim addr, amt, kind, borrow, useMode, sEndDate As String
        Dim pDate, endDate As DateTime

        kindno = Trim(txtPrNo.Text)
        builNo = Trim(txtBuilNo.Text)
        acno = Trim(txtAcNo.Text)
        no = Trim(txtNo.Text)
        landNo2 = Trim(txtLandNo2.Text)
        landNo1 = Trim(txtLandNo1.Text)
        sPDate = Trim(txtPDate.Text)
        landArea = Trim(txtLandArea.Text)
        part = Trim(txtPart.Text)
        area1 = Trim(txtArea1.Text)
        area2 = Trim(txtArea2.Text)
        area3 = Trim(txtArea3.Text)
        sEndDate = Trim(txtEndDate.Text)
        stru = Trim(txtStru.Text)
        amt = Trim(txtAmt.Text)
        endAmt = Trim(txtEndAmt.Text)
        useyear = Trim(txtUseYear.Text)
        tax = Trim(txtTax.Text)
        addr = Trim(txtAddr.Text)
        remark = Trim(txtRemark.Text)
        useMode = Trim(txtUseMode.Text)
        borrow = Trim(txtBorrow.Text)
        kind = Trim(cboKind.Text)

        If Not IsNumeric(kindno) Then
            MessageBx("財物編號必須輸入6位數字")
            txtPrNo.Focus()
            PrevTableStatus = "0"
            Exit Sub
        End If
        'If Not gPS3.IsPGKindIDValid(kindno) Then
        '    MessageBx("財物編號不存在，請指定屬於建物的財物編號")
        '    txtPrNo.Focus()
        '    Exit Sub
        'End If
        If addr = "" Then
            MessageBx("必須輸入地址")
            txtAddr.Focus()
            PrevTableStatus = "0"
            Exit Sub
        End If
        If stru = "" Then
            MessageBx("必須輸入房屋構造")
            txtStru.Focus()
            PrevTableStatus = "0"
            Exit Sub
        End If
        If useMode = "" Then
            MessageBx("必須輸入使用現況")
            txtUseMode.Focus()
            PrevTableStatus = "0"
            Exit Sub
        End If
        If kind = "" Then
            MessageBx("必須輸入房屋類別")
            cboKind.Focus()
            PrevTableStatus = "0"
            Exit Sub
        End If
        If (no = "") Then
            MessageBx("必須指定序號")
            txtNo.Focus()
            PrevTableStatus = "0"
            Exit Sub
        End If

        If sPDate <> "" Then
            sPDate = sDateCDToAD(sPDate)
            pDate = CDate(sPDate)
            If pDate > Now Then
                MessageBx("登記日期不可大於今天")
                txtPDate.Focus()
                PrevTableStatus = "0"
            End If
        Else
            MessageBx("登記日期不得為空")
            txtPDate.Focus()
            PrevTableStatus = "0"
        End If

        If sEndDate <> "" Then
            sEndDate = sDateCDToAD(sEndDate)
            endDate = CDate(sEndDate)
            If endDate > Now Then
                MessageBx("報廢日期不可大於今天")
                txtEndDate.Focus()
                PrevTableStatus = "0"
            End If
        End If


        If (Not IsNumeric(landArea)) Or landArea.IndexOf("-") >= 0 Then
            MessageBx("基地面積必須輸入正數")
            txtLandArea.Focus()
            Exit Sub
        End If
        If (Not IsNumeric(area1)) Or area1.IndexOf("-") >= 0 Then
            MessageBx("主物面積必須輸入正數")
            txtArea1.Focus()
            Exit Sub
        End If
        If (Not IsNumeric(area2)) Or area2.IndexOf("-") >= 0 Then
            MessageBx("附屬建物面積必須輸入正數")
            txtArea2.Focus()
            Exit Sub
        End If
        If (Not IsNumeric(area3)) Or area3.IndexOf("-") >= 0 Then
            MessageBx("房屋坪數必須輸入正數")
            txtArea3.Focus()
            Exit Sub
        End If
        If (Not IsNumeric(amt)) Or amt.IndexOf("-") >= 0 Then
            MessageBx("金額必須輸入正數")
            txtAmt.Focus()
            Exit Sub
        End If
        If (Not IsNumeric(endAmt)) Or endAmt.IndexOf("-") >= 0 Then
            MessageBx("預估殘值必須輸入正數")
            txtEndAmt.Focus()
            Exit Sub
        End If
        If (Not IsNumeric(useyear)) Or useyear.IndexOf("-") >= 0 Or useyear.IndexOf(".") >= 0 Then
            MessageBx("使用年限必須輸入正整數")
            txtUseYear.Focus()
            Exit Sub
        End If

        Dim result As Integer
        prno = kindno & no

        Dim info As BuildingInfo
        info = New BuildingInfo(prno, acno, sPDate, builNo, landNo1, landNo2, landArea, part, area1, area2, area3, useyear, amt, sEndDate, endAmt, stru, tax, addr, kind, useMode, borrow, remark)




        '-- 不可重複(只有新增才需判斷) --
        Dim strRow As String = ""
        If PrevTableStatus = "1" Then
            txtKey1.Text = lblkey.Text
        End If
        Dim loadkey As String = lblkey.Text

        '判斷程序為新增或修改*****
        If PrevTableStatus = "1" Then SaveStatus = InsertData(info) : ViewState("FileKey") = txtKey1.Text '新增
        If PrevTableStatus = "2" Then SaveStatus = UpdateData(info) : ViewState("FileKey") = txtKey1.Text '修改

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
    Public Function InsertData(info As BuildingInfo) As Boolean
        Dim blnCheck As Boolean = False
        Dim result As Integer
        result = BuildingDAL.Insert(info)
        Select Case result
            Case 1
                MessageBx("新增成功")
                blnCheck = True
            Case -1
                MessageBx("新增失敗，原因是 此筆財物編號已經存在，必須指定新的編號")
            Case -2
                MessageBx("新增失敗，原因是 在財物主檔至少有一筆財物已經佔用此編號，必須指定新的編號")
            Case Else
                MessageBx("未知的傳回值")
        End Select

        Return blnCheck
    End Function
    '修改
    Public Function UpdateData(info As BuildingInfo) As Boolean
        Dim blnCheck As Boolean = False
        Dim result As Integer
        result = BuildingDAL.Update(info)
        Select Case result
            Case 1
                MessageBx("修改成功")
                blnCheck = True
            Case -1
                MessageBx("修改失敗，原因是 找不到此財物編號 ")
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

        keyvalue = Trim(lblkey.Text)
        Dim info As BuildingInfo
        info = New BuildingInfo(keyvalue)
        result = BuildingDAL.Delete(info)
        Select Case result
            Case 1
                SaveStatus = True
            Case -1
                MessageBx("刪除失敗，原因是 找不到此筆財物編號 ")
            Case Else
                MessageBx("未知的傳回值")
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
        Dim info As New BuildingInfo(strKey1, strKey1)

        Dim mDT As DataTable = New DataTable
        Dim mDV As DataView = New DataView
        Dim dr As DataRow

        mDT = BuildingDAL.Query(info)
        dr = mDT.Rows(0)

        Dim kindNo As String, no As String, prno As String
        prno = dr("prno")
        kindNo = Microsoft.VisualBasic.Left(prno, 6)
        no = Mid(prno, 7)
        txtPrNo.Text = kindNo
        txtNo.Text = no
        txtBuilNo.Text = dr("builno") & ""
        txtAcNo.Text = dr("acno") & ""
        txtLandNo2.Text = dr("land_no2") & ""
        txtLandNo1.Text = dr("land_no1") & ""
        txtPDate.Text = sDateADToCD(dr("pdate").ToString)
        txtLandArea.Text = dr("land_area") & ""
        txtPart.Text = dr("part") & ""
        txtArea1.Text = dr("area1")
        txtArea2.Text = dr("area2")
        txtArea3.Text = dr("area3")
        txtEndDate.Text = sDateADToCD(dr("enddate").ToString)
        txtStru.Text = dr("stru")
        txtAmt.Text = dr("amt")
        txtEndAmt.Text = dr("endamt")
        txtUseYear.Text = dr("useyear")
        txtTax.Text = dr("tax") & ""
        txtAddr.Text = dr("addr")
        txtRemark.Text = dr("remark") & ""
        txtUseMode.Text = dr("usemode")
        txtBorrow.Text = dr("borrow") & ""
        cboKind.Text = dr("kind")
        lblNetAmt.Text = "淨值 = " & dr("netAmt") '& " , 折舊率 = " & Format(dr("depreciationRatio"), "0.00%")
        txtPrNo.Enabled = False
        txtNo.Enabled = False
        txtEndDate.Enabled = True
        lblkey.Text = dr("prno")


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

    Protected Sub btnGetPrNo_Click(sender As Object, e As EventArgs) Handles btnGetPrNo.Click
        If LoadNo() Then MessageBx("成功取得可用之序號")
    End Sub

    '取得此財務分類編號可用的序號
    Private Function LoadNo() As Boolean
        Dim intFrom, intTo As Integer
        Dim strFrom, strTo As String
        Dim count As Integer = 1
        Dim kindNo As String = Trim(txtPrNo.Text)

        If kindNo = String.Empty Or Len(kindNo) <> 6 Then
            txtNo.Text = ""
            Exit Function
        End If

        intFrom = BuildingDAL.GetPrNo(kindNo, count)
        If intFrom < 0 Then
            If intFrom = -1 Then MessageBx("此財物分類編號之下已經沒有可用的序號")
            If intFrom = -2 Then
                MessageBx("此財物分類編號不存在,請重新輸入")
            End If
            txtNo.Text = ""
            txtPrNo.Focus()
            Exit Function
        Else
            strFrom = CStr(intFrom)
            strFrom = New String("0", 4 - Len(strFrom)) & strFrom
            txtNo.Text = strFrom
        End If
    End Function
End Class