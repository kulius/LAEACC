Imports System.Data
Imports System.Data.SqlClient

Public Class PGMB050
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
    Dim mDT As DataTable = New DataTable
    Dim mDV As DataView = New DataView
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱

        txtQueryPrNo.Attributes.Add("onchange", "return ismaxlength(this)")
        txtQueryPrNo2.Attributes.Add("onchange", "return ismaxlength(this)")
        txtPrNo.Attributes.Add("onchange", "return ismaxlength(this)")


        ViewState("MyOrder") = "prno"  '預設排序欄位

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        Dim s1 As String = Request.QueryString("cvalue")

        If s1 = "PGMC010" Then
            TabContainer1.ActiveTabIndex = 2 '指定Tab頁籤
        End If

        If s1 = "PGMC020" Then
            TabContainer1.ActiveTabIndex = 3 '指定Tab頁籤
        End If
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        UCBase1.SetButtons_Visible()                         '初始化控制鍵



        Dim str1 As String = "一般財物,建物,"
        Master.Controller.objDropDownListOptionGG(cboAutoDepreciationData, "--請選擇--," & str1, "," & str1)

        Dim str2 As String = "按年提列,按月提例,"
        Master.Controller.objDropDownListOptionGG(cboAutoDepreciationMode, "--請選擇--," & str2, "," & str2)

        lblNextYear.Text = Now.Year - 1911 + 1


        'sqlstr = "SELECT * FROM CODE where kind='單位' " & _
        '        " order by codeno"
        'Master.Controller.objDropDownListOptionEX(cboQueryUnit, DNS_PGM, sqlstr, "codeno", "codename", 0)
        'Master.Controller.objDropDownListOptionEX(cboUnit, DNS_PGM, sqlstr, "codeno", "codename", 0)

        'sqlstr = "SELECT * FROM CODE where kind='建物類別' " & _
        '        " order by CODENO"
        'Master.Controller.objDropDownListOptionEX(cboQueryKind, DNS_PGM, sqlstr, "codeno", "codename", 0)
        'Master.Controller.objDropDownListOptionEX(cboKind, DNS_PGM, sqlstr, "codeno", "codename", 0)

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
        Dim prno, prno2, acno, acno2, sDepreciationDate, sDepreciationDate2, name As String
        Dim amt, amt2, remark As String
        Dim depreciationDate, depreciationDate2 As DateTime

        prno = Trim(txtQueryPrNo.Text)
        prno2 = Trim(txtQueryPrNo2.Text)
        acno = Trim(txtQueryAcNo.Text)
        acno2 = Trim(txtQueryAcNo2.Text)
        sDepreciationDate = Trim(txtQueryDepreciationDate.Text)
        sDepreciationDate2 = Trim(txtQueryDepreciationDate2.Text)
        amt = Trim(txtQueryAmt.Text)
        amt2 = Trim(txtQueryAmt2.Text)
        remark = Trim(txtQueryRemark.Text)
        name = Trim(txtQueryName.Text)


        '檢查是否至少輸入一項查詢條件
        If prno = "" And prno2 = "" And acno = "" And acno2 = "" And sDepreciationDate = "" And sDepreciationDate2 = "" _
        And amt = "" And amt2 = "" And remark = "" And name = "" Then
            MessageBx("至少要輸入一項查詢條件")
            Exit Sub
        End If
        '檢查各欄位是否輸入有效值
        If prno > prno2 And (prno <> "" And prno2 <> "") Then
            MessageBx("起始財物編號必須小於終止財物編號")
            txtQueryPrNo.Focus()
            Exit Sub
        End If
        If acno > acno2 And (acno <> "" And acno2 <> "") Then
            MessageBx("起始會計科目編號必須小於終止會計科目編號")
            txtQueryAcNo.Focus()
            Exit Sub
        End If
        If Not IsNumeric(amt) And amt <> "" Then
            MessageBx("起始金額只能輸入數字")
            txtQueryAmt.Focus()
            Exit Sub
        End If
        If Not IsNumeric(amt2) And amt2 <> "" Then
            MessageBx("終止金額只能輸入數字")
            txtQueryAmt2.Focus()
            Exit Sub
        End If
        If amt > amt2 And (amt <> "" And amt2 <> "") Then
            MessageBx("起始金額必須小於終止金額")
            txtQueryAmt.Focus()
            Exit Sub
        End If

        

        Dim info As New DepreciationInfo(prno, prno2, acno, acno2, name, sDepreciationDate, sDepreciationDate2, amt, amt2, remark)

        If chkIsSumAllGrade.Checked Then
            mDT = DepreciationDAL.Query2(info)
        Else
            mDT = DepreciationDAL.Query(info)
        End If
        mDV.Table = mDT
        mDV.Sort = "seq,acno,prno,pdate"
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

        Dim prno, sDepreciationDate, amt, remark As String
        Dim name, purchaseDate, sEndDate, endRemark As String
        Dim depreciationDate As DateTime

        prno = Trim(txtPrNo.Text)
        sDepreciationDate = Trim(txtDepreciationDate.Text)
        amt = Trim(txtAmt.Text)
        remark = Trim(txtRemark.Text)

        name = Trim(txtName.Text)
        purchaseDate = Trim(txtPurchaseDate.Text)
        sEndDate = Trim(txtEndDate.Text)
        endRemark = lblEndRemark.Text

        If Not IsNumeric(prno) Or Len(prno) <> 10 Then
            MessageBx("財物編號必須輸入10位數字")
            txtPrNo.Focus()
            Exit Sub
        End If
        If sEndDate <> String.Empty Then
            MessageBx("財物已經報廢了，不可執行折舊作業")
            txtPrNo.Focus()
            Exit Sub
        End If
        If Not IsNumeric(amt) Then
            MessageBx("金額必須輸入整數")
            txtAmt.Focus()
            Exit Sub
        End If

        If sDepreciationDate <> "" Then
            sDepreciationDate = sDateCDToAD(sDepreciationDate)
            depreciationDate = CDate(sDepreciationDate)
            If depreciationDate > Now Then
                MessageBx("提列日期不可大於今天")
                txtDepreciationDate.Focus()
                PrevTableStatus = "0"
            End If
        Else
            MessageBx("提列日期不得為空")
            txtDepreciationDate.Focus()
            PrevTableStatus = "0"
        End If

        Dim info As DepreciationInfo
        info = New DepreciationInfo(Session("mid"), prno, Session("mACNO"), sDepreciationDate, amt, remark, name, purchaseDate, Session("mOriginalAmt"), Session("mEndAmt"), Session("mTotalAddDel"), Session("mNetAmt"), Session("mDepreciation"), Session("mUseYear"), Session("mKeepEmpName"), Session("mKeepUnitName"), sEndDate, endRemark)



      


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
    Public Function InsertData(info As DepreciationInfo) As Boolean
        Dim blnCheck As Boolean = False
        Dim result As Integer
        result = DepreciationDAL.Insert(info, Session("mid"))
        Select Case result
            Case 1
                MessageBx("新增成功")
                blnCheck = True
                'mDepreciation = CDec(mDepreciation) + CDec(amt)
                'mNetAmt = CDec(mOriginalAmt) + CDec(mTotalAddDel) - CDec(mDepreciation)
                'txtDepreciation.Text = mDepreciation
                'txtNetAmt.Text = mNetAmt
                'AddToGrid(info)
                'If mIsClearDataAfterAdd Then ClearData()
            Case -1
                MessageBx("新增失敗，原因是 此筆財物編號不存在於財物主檔或是建物主檔中，或是此筆財物編號的會記科目欄位沒有資料，因此不可提列折舊")
            Case -2
                MessageBx("新增失敗，原因是 此筆財物編號和提列日期已經存在折舊檔中，必須修改提列日期")
            Case Else
                MessageBx("未知的傳回值")
        End Select

        Return blnCheck
    End Function
    '修改
    Public Function UpdateData(info As DepreciationInfo) As Boolean
        Dim blnCheck As Boolean = False
        Dim result As Integer
        result = DepreciationDAL.Update(info)
        Select Case result
            Case 1
                MessageBx("修改成功")
                blnCheck = True
                'mDepreciation = CDec(mDepreciation) - CDec(mAmt) + CDec(amt)
                'mNetAmt = CDec(mOriginalAmt) + CDec(mTotalAddDel) - CDec(mDepreciation)
                'txtDepreciation.Text = mDepreciation
                'txtNetAmt.Text = mNetAmt
                'ReviseToGrid(info)
                'If mIsClearDataAfterAdd Then ClearData()
            Case -1
                MessageBx("修改失敗，原因是 找不到此筆資料")
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


        If Session("mid") Then
            MessageBx("這是加總過的資料，不可刪除")
            Exit Sub
        End If
        Dim prno As String = txtPrNo.Text
        Dim id As String = Session("mid")
        Dim info As DepreciationInfo


        info = New DepreciationInfo(id, prno, False)
        result = DepreciationDAL.Delete(info)
        Select Case result
            Case 1
                SaveStatus = True
            Case -1
                MessageBx("刪除失敗，原因是 找不到此筆資料 ")
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
        Dim intI, SumUp As Integer
        Dim strI, strColumn1, strColumn2 As String

        '開啟查詢
        objCon99 = New SqlConnection(DNS_PGM)
        objCon99.Open()

        Dim sqlstr, qstr, strD, strC As String

        sqlstr = "SELECT * from PPTF030 " & _
         "where  ID ='" & strKey1 & "'"


        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            Session("mid") = objDR99("ID").ToString
            lblkey.Text = objDR99("ID").ToString
            txtPrNo.Text = objDR99("prno").ToString
            txtDepreciationDate.Text = sDateADToCD(objDR99("pdate").ToString)
            txtAmt.Text = objDR99("amt").ToString
            txtRemark.Text = objDR99("remark").ToString

        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標


        Dim prno As String = txtPrNo.Text

        Query(prno)
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
        Dim prno As String = txtPrNo.Text
        Query(prno)
    End Sub

    Protected Sub btnDepreciationQuery_Click(sender As Object, e As EventArgs) Handles btnDepreciationQuery.Click
        Dim prno As String = Trim(txtPrNo.Text)
        If prno = String.Empty Then
            MessageBx("必須輸入財物編號")
            txtPrNo.Focus()
            Exit Sub
        End If
        If Len(prno) <> 10 Then
            MessageBx("財物編號必須為10碼")
            txtPrNo.Focus()
            Exit Sub
        End If


        Dim info As New DepreciationInfo(prno, prno)
        mDT = DepreciationDAL.Query(info)
        mDV.Table = mDT
        mDV.Sort = "prno"
        DataGridView.DataSource = mDV
        DataGridView.DataBind()

        lbl_GrdCount.Text = mDV.Count

        If mDV.Count = 0 Then
            MessageBx("查無資料")
            '使大部分控制項失效
            'DisableControl()
        Else
            '使大部分控制項生效,並選擇第一筆資料
            'EnableControl()
        End If
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Sub Query(ByVal prno As String)
        If prno = String.Empty Then
            txtName.Text = ""
            txtWhoKeep.Text = ""
            txtPurchaseDate.Text = ""
            txtEndDate.Text = ""
            lblEndRemark.Text = ""
            txtOriginalAmt.Text = ""
            txtEndAmt.Text = ""
            txtTotalAddDel.Text = ""
            txtNetAmt.Text = ""
            txtDepreciation.Text = ""
            txtUseYear.Text = ""
        Else
            Dim dt As DataTable
            Dim info As New PGMainInfo(prno, prno)
            dt = PGMainDAL.Query(info)

            If dt.Rows.Count = 0 Then '在財物主檔找不到資料,還要去建物主檔找

                Dim info2 As New BuildingInfo(prno, prno)
                dt = BuildingDAL.Query(info2)

                If dt.Rows.Count = 0 Then
                    txtName.Text = ""
                    txtWhoKeep.Text = ""
                    txtPurchaseDate.Text = ""
                    txtEndDate.Text = ""
                    lblEndRemark.Text = ""
                    txtOriginalAmt.Text = ""
                    txtEndAmt.Text = ""
                    txtTotalAddDel.Text = ""
                    txtNetAmt.Text = ""
                    txtDepreciation.Text = ""
                    txtUseYear.Text = ""
                    MessageBx("找不到此筆財物編號的資料")

                Else '在建物主檔找到資料

                    Dim dr As DataRow = dt.Rows(0)
                    txtName.Text = dr("kind") '在這使用建物的 kind 欄位來當作財物名稱
                    txtWhoKeep.Text = ""

                    txtPurchaseDate.Text = sDateADToCD(dr("pdate").ToString)
                    txtEndDate.Text = sDateADToCD(dr("enddate").ToString)
                    lblEndRemark.Text = ""
                    txtOriginalAmt.Text = dr("amt")
                    txtEndAmt.Text = dr("endamt")
                    txtTotalAddDel.Text = dr("totalAddDel")
                    txtNetAmt.Text = dr("netamt")
                    txtDepreciation.Text = dr("depreciation")
                    txtUseYear.Text = dr("useyear")
                    Session("mKeepEmpName") = ""
                    Session("mKeepUnitName") = ""
                    Session("mACNO") = dr("acno") & ""
                    Session("mOriginalAmt") = dr("amt")
                    Session("mEndAmt") = dr("endamt")
                    Session("mTotalAddDel") = dr("totalAddDel")
                    Session("mNetAmt") = dr("netamt")
                    Session("mDepreciation") = dr("depreciation")
                    Session("mUseYear") = dr("useyear")
                End If

            Else '在財物主檔找到資料
                Dim dr As DataRow = dt.Rows(0)
                Dim keepEmp As String = dr("keepempno") & ""
                txtName.Text = dr("name")
                If keepEmp = String.Empty Then
                    txtWhoKeep.Text = dr("keepunit") & " " & dr("keepUnitName")
                    Session("mKeepEmpName") = ""
                    Session("mKeepUnitName") = dr("keepUnitName") & ""
                Else
                    txtWhoKeep.Text = keepEmp & " " & dr("keepempname")
                    Session("mKeepEmpName") = dr("keepempname") & ""
                    Session("mKeepUnitName") = ""
                End If
                Session("mACNO") = dr("acno") & ""
                txtPurchaseDate.Text = sDateADToCD(dr("pdate").ToString)
                txtEndDate.Text = sDateADToCD(dr("enddate").ToString)
                lblEndRemark.Text = dr("endremk") & ""
                txtOriginalAmt.Text = dr("amt")
                txtEndAmt.Text = dr("endamt")
                txtTotalAddDel.Text = dr("totalAddDel")
                txtNetAmt.Text = dr("netamt")
                txtDepreciation.Text = dr("depreciation")
                txtUseYear.Text = dr("useyear")
                Session("mOriginalAmt") = dr("amt")
                Session("mEndAmt") = dr("endamt")
                Session("mTotalAddDel") = dr("totalAddDel")
                Session("mNetAmt") = dr("netamt")
                Session("mDepreciation") = dr("depreciation")
                Session("mUseYear") = dr("useyear")
            End If

        End If
    End Sub

    Protected Sub btnAutoDepreciation_Click(sender As Object, e As EventArgs) Handles btnAutoDepreciation.Click
        Dim sDepreciationDate, sDepreciationDate2 As String
        Dim depreciationDate As DateTime
        Dim method, kind As Integer
        Dim year, month As Integer

        sDepreciationDate = Trim(txtAutoDepreciationDate.Text)
        method = cboAutoDepreciationMode.SelectedIndex - 1 '按年或按月提列
        kind = cboAutoDepreciationData.SelectedIndex - 1 '提列財物或是建物

        If sDepreciationDate <> "" Then
            sDepreciationDate = sDateCDToAD(sDepreciationDate)
            depreciationDate = CDate(sDepreciationDate)
            If depreciationDate.Year > Now.Year Then
                MessageBx("提列日期不可大於今年")
                txtAutoDepreciationDate.Focus()
                Exit Sub
            End If

            If method = 1 And depreciationDate.Year = Now.Year And depreciationDate.Month > Now.Month Then '按月提列
                MessageBx("按月提列者其提列日期不可大於本月")
                txtAutoDepreciationDate.Focus()
                Exit Sub
            End If
        Else
            MessageBx("提列日期不得為空")
            txtAutoDepreciationDate.Focus()
            Exit Sub
        End If


        Dim result As Integer
        result = DepreciationDAL.Depreciate(sDepreciationDate, method, kind)
        MessageBx("總共提列了 " & result & " 筆財物的折舊資料，現在要查詢折舊明細資料")

        '建立出日期區間以便等下要查詢折舊明細資料
        year = depreciationDate.Year
        month = depreciationDate.Month
        If method = 0 Then '按年提列
            sDepreciationDate = year & "/1/1"
            sDepreciationDate2 = year & "/12/31"
        Else
            sDepreciationDate = year & "/" & month & "/1"
            sDepreciationDate2 = year & "/" & month & "/" & DateTime.DaysInMonth(year, month)
        End If

        Dim info As New DepreciationInfo(sDepreciationDate, sDepreciationDate2, "")

        mDT = DepreciationDAL.Query(info)
        mDV.Table = mDT
        mDV.Sort = "acno,prno"
        DataGridView.DataSource = mDV
        DataGridView.DataBind()

        lbl_GrdCount.Text = mDV.Count
        If mDV.Count = 0 Then
            MessageBx("找不到資料，請重新設定查詢條件")
        End If
        TabContainer1.ActiveTabIndex = 0



    End Sub

    Protected Sub btnPreAutoDepreciation_Click(sender As Object, e As EventArgs) Handles btnPreAutoDepreciation.Click
        btnPreAutoDepreciation.Enabled = False
        Dim result As String, dt As DataTable
        Dim sDepreciationDate As String
        sDepreciationDate = (Now.Year + 1) & "/1/1"
        mDT = DepreciationDAL.PreDepreciate(sDepreciationDate, result)
        MsgBox("總共提列了 " & result & " 筆財物的折舊資料，現在要查詢折舊明細資料")

        mDV.Table = mDT
        mDV.Sort = "acno,prno"
        DataGrid1.DataSource = mDV
        DataGrid1.DataBind()

        'Dim dv As DataView = dt.DefaultView
        'dv.Sort = "acno,prno"
        'DataGrid1.DataSource = mDV
        'DataGrid1.DataBind()

        Label2.Text = mDV.Count


        If mDV.Count = 0 Then
            MessageBx("查無資料")
            btnCopyPreDepreciation.Enabled = False
        Else
            btnCopyPreDepreciation.Enabled = True
        End If
        btnPreAutoDepreciation.Enabled = True
    End Sub

    Protected Sub btnCopyPreDepreciation_Click(sender As Object, e As EventArgs) Handles btnCopyPreDepreciation.Click
        'Dim dt As DataTable, dv As DataView
        'dv = dgPreAutoDepreciation.DataSource
        'dt = dv.Table
        'CopyAllGridData(dgPreAutoDepreciation, dt, dv, gMsgTitle)
        'MsgBox("複製成功，您可以將這些資料貼到其他應用程式作後續處理，譬如Excel")
        Dim sw As New System.IO.StringWriter
        Dim hw As New System.Web.UI.HtmlTextWriter(sw)

        'GridView1.EditIndex = False 

        Response.Clear()
        Response.AppendHeader("Content-Disposition", "attachment; filename=P.xls")
        Response.ContentType = "application/vnd.ms-excel"

        DataGrid1.RenderControl(hw)
        Response.Write(sw.ToString())
        Response.End()

        'GridView1.EditIndex = True 

    End Sub
End Class