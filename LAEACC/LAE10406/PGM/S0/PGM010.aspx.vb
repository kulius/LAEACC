Imports System.Data
Imports System.Data.SqlClient

Public Class PGM010
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

        ViewState("MyOrder") = "KindNo"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        UCBase1.SetButtons_Visible()                         '初始化控制鍵

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤



        sqlstr = "SELECT * FROM CODE where kind='使用年限' " & _
                " order by CAST(CODENO AS integer)"
        Master.Controller.objDropDownListOptionEX(cboQueryUseyear, DNS_PGM, sqlstr, "codeno", "codename", 0)
        Master.Controller.objDropDownListOptionEX(cboQueryUseyear2, DNS_PGM, sqlstr, "codeno", "codename", 0)
        Master.Controller.objDropDownListOptionEX(cboUseYear, DNS_PGM, sqlstr, "codeno", "codename", 0)

        sqlstr = "SELECT * FROM CODE where kind='報廢原因' " & _
        " order by CODENO"
        Master.Controller.objDropDownListOptionEX(cboQueryDiscardMode, DNS_PGM, sqlstr, "codeno", "codename", 0)
        Master.Controller.objDropDownListOptionEX(cboEndRemark, DNS_PGM, sqlstr, "codeno", "codename", 0)

        Dim strcount As String = "" : For I As Integer = 1 To 100 Step 1 : strcount &= I & "," : Next '新增數量
        Master.Controller.objDropDownListOptionGG(cboAddCount, "--請選擇--," & strcount, "," & strcount)
        Dim strWhoKeep As String = "個人保管,單位保管"
        Master.Controller.objDropDownListOptionGG(cboWhoKeep, "--請選擇--," & strWhoKeep, "," & strWhoKeep)
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
        '檢查與查詢相關的欄位是否輸入正確
        Dim prno, prno2, acno, acno2, bgno, bgno2, spdate, spdate2 As String
        Dim senddate, senddate2, keepunit, keepunit2, keepemp, keepemp2 As String
        Dim srdate, srdate2, amt, amt2, useyear, useyear2, uses, specremark As String
        Dim rdate, rdate2, enddate, enddate2, pdate, pdate2 As DateTime
        Dim name, comefrom, model, queryDiscardMode As String
        prno = Trim(txtQueryPrNo.Text)
        prno2 = Trim(txtQueryPrNo2.Text)
        acno = Trim(txtQueryAcNo.Text)
        acno2 = Trim(txtQueryAcNo2.Text)
        bgno = Trim(txtQueryBgAcNo.Text)
        bgno2 = Trim(txtQueryBgAcNo2.Text)
        spdate = Trim(txtQueryPDate.Text)
        spdate2 = Trim(txtQueryPDate2.Text)
        senddate = Trim(txtQueryEndDate.Text)
        senddate2 = Trim(txtQueryEndDate2.Text)
        keepunit = Trim(txtQueryKeepUnit.Text)
        keepunit2 = Trim(txtQueryKeepUnit2.Text)
        keepemp = Trim(txtQueryEmpNo.Text)
        keepemp2 = Trim(txtQueryEmpNo2.Text)
        srdate = Trim(txtQueryRevisedDate.Text)
        srdate2 = Trim(txtQueryRevisedDate2.Text)
        amt = Trim(txtQueryAmt.Text)
        amt2 = Trim(txtQueryAmt2.Text)
        useyear = Trim(cboQueryUseyear.Text)
        useyear2 = Trim(cboQueryUseyear2.Text)
        uses = Trim(txtQueryUses.Text)
        specremark = Trim(txtQuerySpecRemark.Text)
        name = Trim(txtQueryName.Text)
        comefrom = Trim(txtQueryComeFrom.Text)
        model = Trim(txtQueryModel.Text)
        queryDiscardMode = cboQueryDiscardMode.SelectedIndex

        '檢查是否至少輸入一項查詢條件
        If prno = "" And prno2 = "" And acno = "" And acno2 = "" And bgno = "" And bgno2 = "" And spdate = "" And spdate2 = "" _
        And senddate = "" And senddate2 = "" And keepunit = "" And keepunit2 = "" And keepemp = "" And keepemp2 = "" _
        And srdate = "" And srdate2 = "" And amt = "" And amt2 = "" And useyear = "" And useyear2 = "" And uses = "" And specremark = "" _
        And name = "" And comefrom = "" And model = "" Then
            MessageBx("至少要輸入一項查詢條件")
            Exit Sub
        End If
        '檢查各欄位是否輸入有效值
        If prno > prno2 And (prno <> "" And prno2 <> "") Then
            MessageBx("起始財物編號必須小於終止財物編號")
            Exit Sub
        End If
        If acno > acno2 And (acno <> "" And acno2 <> "") Then
            MessageBx("起始會計科目編號必須小於終止會計科目編號")
            Exit Sub
        End If
        If bgno > bgno2 And (bgno <> "" And bgno2 <> "") Then
            MessageBx("起始預算科目編號必須小於終止預算科目編號")
            Exit Sub
        End If
        If keepunit > keepunit2 And (keepunit <> "" And keepunit2 <> "") Then
            MessageBx("起始單位代碼必須小於終止單位代碼")
            Exit Sub
        End If
        If keepemp > keepemp2 And (keepemp <> "" And keepemp2 <> "") Then
            MessageBx("起始員工代號必須小於終止員工代號")
            Exit Sub
        End If
        If Not IsNumeric(amt) And amt <> "" Then
            MessageBx("起始原值只能輸入數字")
            Exit Sub
        End If
        If Not IsNumeric(amt2) And amt2 <> "" Then
            MessageBx("終止原值只能輸入數字")
            Exit Sub
        End If
        If amt > amt2 And (amt <> "" And amt2 <> "") Then
            MessageBx("起始原值必須小於終止原值")
            Exit Sub
        End If
        If Not IsNumeric(useyear) And useyear <> "" Then
            MessageBx("起始使用年限只能輸入數字")
            Exit Sub
        End If
        If Not IsNumeric(useyear2) And useyear2 <> "" Then
            MessageBx("終止使用年限只能輸入數字")
            Exit Sub
        End If
        If useyear > useyear2 And (useyear <> "" And useyear2 <> "") Then
            MessageBx("起始使用年限必須小於終止使用年限")
            Exit Sub
        End If

        If pdate > pdate2 And (spdate <> "" And spdate2 <> "") Then
            MessageBx("起始購入日期必須小於終止購入日期")
            Exit Sub
        End If

        If enddate > enddate2 And (senddate <> "" And senddate2 <> "") Then
            MessageBx("起始報廢日期必須小於終止報廢日期")
            Exit Sub
        End If

        If rdate > rdate2 And (srdate <> "" And srdate2 <> "") Then
            MessageBx("起始修改日期必須小於終止修改日期")
            Exit Sub
        End If



        'btnQuery.Enabled = False

        Dim info As New PGMainInfo(prno, prno2, name, acno, acno2, spdate, spdate2, keepemp, keepemp2 _
        , keepunit, keepunit2, useyear, useyear2, amt, amt2, bgno, bgno2, comefrom, model _
        , senddate, senddate2, uses, specremark, srdate, srdate2, queryDiscardMode, "dummy")

        Dim PGMainDAL As PGMainDAL = New PGMainDAL
        Dim mDT As DataTable = New DataTable
        Dim mDV As DataView = New DataView
        mDT = PGMainDAL.Query(info)
        mDV.Table = mDT
        mDV.Sort = "prno"
        DataGridView.DataSource = mDV
        DataGridView.DataBind()

        lbl_GrdCount.Text = mDV.Count
        If mDV.Count = 0 Then
            MessageBx("找不到資料，請重新設定查詢條件")
        End If
        TabContainer1.ActiveTabIndex = 0

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

        Dim kindno, name, unit, material, useyear, rowFilter As String
        'Dim length As Integer
        'kindno = Trim(txtKindNo.Text)
        'length = Len(kindno)
        'name = Trim(txtName.Text)
        'unit = Trim(cboUnit.Text)
        'material = Trim(cboMaterial.Text)
        'useyear = Trim(cboUseYear.Text)
        'If Not IsNumeric(kindno) Then
        '    MessageBx("編號必須輸入不大於6位的數字")
        '    PrevTableStatus = "0"
        'End If
        'If length <> 1 And length <> 3 And length <> 6 Then
        '    MessageBx("編號長度必須是 1 位或 3 位或是 6 位數字")
        '    PrevTableStatus = "0"
        'End If
        'If name = String.Empty Then
        '    MessageBx("名稱不可空白")
        '    PrevTableStatus = "0"
        'End If
        'If Not IsNumeric(useyear) Or useyear.IndexOf("-") >= 0 Then
        '    MessageBx("使用年限必須輸入正整數")
        '    PrevTableStatus = "0"
        'End If

        Dim pgKind As New PGKindInfo(kindno, name, Unit, material, useyear)

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

        sqlstr = "SELECT * from PPTF010 " & _
         "where  PRNO ='" & strKey1 & "'"


        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then

            Dim kindNo As String, no As String, prno As String
            prno = objDR99("prno").ToString
            kindNo = Microsoft.VisualBasic.Left(prno, 6)
            no = Mid(prno, 7)
            txtPrNo.Text = kindNo
            cboAddCount.SelectedIndex = 0
            txtNo.Text = no
            txtNo2.Text = no
            txtName.Text = objDR99("name").ToString
            txtUnit.Text = objDR99("prno").ToString
            txtMaterial.Text = objDR99("material").ToString
            cboUseYear.Text = objDR99("useyear").ToString
            txtAcNo.Text = objDR99("acno").ToString
            txtQty.Text = objDR99("qty").ToString
            txtAmt.Text = objDR99("amt").ToString
            txtEndAmt.Text = objDR99("endamt").ToString
            txtBgAcNo.Text = objDR99("bgacno").ToString
            txtBgAcName.Text = objDR99("bgname").ToString
            txtComeFrom.Text = objDR99("comefrom").ToString
            txtMadeDate.Text = objDR99("madedate").ToString
            txtModel.Text = objDR99("model").ToString
            txtPlace.Text = objDR99("place").ToString
            Dim keepemp As String = objDR99("keepempno").ToString & ""
            If keepemp = String.Empty Then
                cboWhoKeep.SelectedIndex = 1
                txtWhoKeep.Text = objDR99("keepunit").ToString & ""
            Else
                cboWhoKeep.SelectedIndex = 0
                txtWhoKeep.Text = objDR99("keepempno").ToString & ""
            End If
            txtUses.Text = objDR99("uses").ToString
            txtBorrow.Text = objDR99("borrow").ToString
            txtPDate.Text = objDR99("pdate").ToString
            txtEndDate.Text = objDR99("enddate").ToString
            cboEndRemark.Text = objDR99("endremk").ToString
            txtRemark.Text = objDR99("remark").ToString
            txtSpecRemark.Text = objDR99("spec_remark").ToString
            'lblNetAmt.Text = "增減值 = " & objDR99("totalAddDel").ToString & " , 淨值 = " & objDR99("netAmt").ToString & " , 折舊率 = " & Format(objDR99("depreciationRatio").ToString, "0.00%")
            'btnAddReviseOK.Text = "確定修改"
            'txtPrNo.Enabled = False
            'btnPrNo.Enabled = False
            'cboAddCount.Enabled = False
            'txtNo.Enabled = False
            'lblNetAmt.Visible = True
            'btnGetPrNo.Enabled = False
            'btnEndDate.Enabled = True
            'txtEndDate.Enabled = True
            'cboEndRemark.Enabled = True
            'btnDeleteOK.Enabled = True

            lblkey.Text = Trim(objDR99("prno").ToString)
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