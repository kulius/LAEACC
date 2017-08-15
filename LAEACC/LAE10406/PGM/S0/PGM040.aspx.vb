Imports System.Data
Imports System.Data.SqlClient

Public Class PGM040
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

    Private mID As String  '剛新增資料的識別碼,或是剛剛修改資料的識別碼
    '新增或修改時,所查詢出來該筆財物的明細資料
    Private mKeepEmpName, mKeepUnitName, mACNO, mAmt As String
    Private mOriginalAmt, mEndAmt, mTotalAddDel, mNetAmt, mDepreciation, mUseYear As String
    Private mstrToday As String '今天的日期字串
    Private mPreviousQueryInfo As PGValueInfo
#End Region
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱

        txtQueryPrNo.Attributes.Add("onchange", "return ismaxlength(this)")
        txtQueryPrNo2.Attributes.Add("onchange", "return ismaxlength(this)")
        txtPrNo.Attributes.Add("onchange", "return ismaxlength(this)")
        txtWhoKeep.Attributes.Add("onchange", "return ismaxlength2(this)")

        ViewState("MyOrder") = "id"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        'UCBase1.SetButtons_Visible()                         '初始化控制鍵

        'Focus*****
        TabContainer1.ActiveTabIndex = 1 '指定Tab頁籤

        

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
            'SetControls(0) '設定所有輸入控制項的唯讀狀態

            '資料查詢*****
            'FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        End If
    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub

    Sub DataGridView_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGridView.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號


        Select Case e.CommandName
            Case "btnShow"
                '查詢資料及控制*****
                FindData(txtID.Text)               '查詢主檔
                'FlagMoveSeat(0, e.Item.ItemIndex)
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

    Protected Sub btnGetPrNo_Click(sender As Object, e As EventArgs) Handles btnGetPrNo.Click
        Dim prno As String = txtPrNo.Text

        If Len(prno) < 10 Then
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

            MessageBx("財務編號長度必須大於10")
            Exit Sub
        End If


        Dim dt As DataTable
        Dim info As New PGMainInfo(prno, prno)
        dt = PGMainDAL.Query(info)
        If dt.Rows.Count = 0 Then

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
                mKeepEmpName = ""
                mKeepUnitName = ""
                mACNO = dr("acno") & ""
                txtPurchaseDate.Text = sDateADToCD(dr("pdate").ToString)
                txtEndDate.Text = sDateADToCD(dr("enddate").ToString)
                lblEndRemark.Text = ""
                txtOriginalAmt.Text = dr("amt")
                txtEndAmt.Text = dr("endamt")
                txtTotalAddDel.Text = dr("totalAddDel")
                txtNetAmt.Text = dr("netamt")
                txtDepreciation.Text = dr("depreciation")
                txtUseYear.Text = dr("useyear")
                mOriginalAmt = dr("amt")
                mEndAmt = dr("endamt")
                mTotalAddDel = dr("totalAddDel")
                mNetAmt = dr("netamt")
                mDepreciation = dr("depreciation")
                mUseYear = dr("useyear")
            End If

        Else
            Dim dr As DataRow = dt.Rows(0)
            Dim keepEmp As String = dr("keepempno") & ""
            txtName.Text = dr("name")
            If keepEmp = String.Empty Then
                txtWhoKeep.Text = dr("keepunit") & " " & dr("keepUnitName")
                mKeepEmpName = ""
                mKeepUnitName = dr("keepUnitName") & ""
            Else
                txtWhoKeep.Text = keepEmp & " " & dr("keepempname")
                mKeepEmpName = dr("keepempname") & ""
                mKeepUnitName = ""
            End If
            mACNO = dr("acno") & ""
            txtPurchaseDate.Text = sDateADToCD(dr("pdate").ToString)
            txtEndDate.Text = sDateADToCD(dr("enddate").ToString)
            lblEndRemark.Text = dr("endremk") & ""
            txtOriginalAmt.Text = dr("amt")
            txtEndAmt.Text = dr("endamt")
            txtTotalAddDel.Text = dr("totalAddDel")
            txtNetAmt.Text = dr("netamt")
            txtDepreciation.Text = dr("depreciation")
            txtUseYear.Text = dr("useyear")
            mOriginalAmt = dr("amt")
            mEndAmt = dr("endamt")
            mTotalAddDel = dr("totalAddDel")
            mNetAmt = dr("netamt")
            mDepreciation = dr("depreciation")
            mUseYear = dr("useyear")
        End If
    End Sub

    Protected Sub btnAddReviseOK_Click(sender As Object, e As EventArgs) Handles btnAddReviseOK.Click
        If Trim(txtEndDate.Text) <> String.Empty Then
            MessageBx("財物已經報廢了，不可執行增減值作業")
            Exit Sub
        End If

        Dim prno, sAddDelDate, amt, remark As String
        Dim name, purchaseDate, sEndDate, endRemark As String
        Dim addDelDate As DateTime

        prno = Trim(txtPrNo.Text)
        sAddDelDate = Trim(txtAddDelDate.Text)
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
        If Not IsNumeric(amt) Then
            MessageBx("金額必須輸入整數")
            txtAmt.Focus()
            Exit Sub
        End If

        If sAddDelDate <> "" Then
            sAddDelDate = sDateCDToAD(sAddDelDate)
            addDelDate = CDate(sAddDelDate)
        Else
            MessageBx("增減日期不得為空")
            txtAddDelDate.Focus()
        End If

        If addDelDate > Now Then
            MessageBx("增減日期不可大於今天")
            txtAddDelDate.Focus()
        End If

        Dim result As Integer

        Dim info As PGValueInfo
        mID = Trim(lblkey.Text)
        info = New PGValueInfo(mID, prno, mACNO, sAddDelDate, amt, remark, name, purchaseDate, mOriginalAmt, mEndAmt, mTotalAddDel, mNetAmt, mDepreciation, mUseYear, mKeepEmpName, mKeepUnitName, sEndDate, endRemark)

        If txtPrNo.Enabled Then '新增

            result = PGValueDAL.Insert(info, mID)
            Select Case result
                Case 1
                    MessageBx("新增成功")
                    mTotalAddDel = CDec(mTotalAddDel) + CDec(amt)
                    mNetAmt = CDec(mOriginalAmt) + CDec(mTotalAddDel) - CDec(mDepreciation)
                    txtTotalAddDel.Text = mTotalAddDel
                    txtNetAmt.Text = mNetAmt
                Case -1
                    MessageBx("新增失敗，原因是 此筆財物編號不存在於財物主檔中")
                Case Else
                    MessageBx("未知的傳回值")
            End Select

        Else

            result = PGValueDAL.Update(info)
            Select Case result
                Case 1
                    MessageBx("修改成功")
                    mTotalAddDel = CDec(mTotalAddDel) - CDec(mAmt) + CDec(amt)
                    mNetAmt = CDec(mOriginalAmt) + CDec(mTotalAddDel) - CDec(mDepreciation)
                    txtTotalAddDel.Text = mTotalAddDel
                    txtNetAmt.Text = mNetAmt
                Case -1
                    MessageBx("修改失敗，原因是 找不到此筆資料 ")
                Case Else
                    MessageBx("未知的傳回值")
            End Select

        End If
    End Sub

    Protected Sub btnAddDelQuery_Click(sender As Object, e As EventArgs) Handles btnAddDelQuery.Click
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
        btnAddDelQuery.Enabled = False

        Dim mDT As DataTable = New DataTable
        Dim mDV As DataView = New DataView

        Dim info As New PGValueInfo(prno, prno)
        mDT = PGValueDAL.Query(info)
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
        btnAddDelQuery.Enabled = True
    End Sub

    Protected Sub btnQuery_Click(sender As Object, e As EventArgs) Handles btnQuery.Click
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
    End Sub

    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        '先取消選取,以免等下查詢出來造成重複的選取項目

        '檢查與查詢相關的欄位是否輸入正確
        Dim prno, prno2, acno, acno2, sAddDelDate, sAddDelDate2, name As String
        Dim amt, amt2, remark As String
        Dim addDelDate, addDelDate2 As DateTime

        prno = Trim(txtQueryPrNo.Text)
        prno2 = Trim(txtQueryPrNo2.Text)
        acno = Trim(txtQueryAcNo.Text)
        acno2 = Trim(txtQueryAcNo2.Text)
        sAddDelDate = Trim(txtQueryAddDelDate.Text)
        sAddDelDate2 = Trim(txtQueryAddDelDate2.Text)
        amt = Trim(txtQueryAmt.Text)
        amt2 = Trim(txtQueryAmt2.Text)
        remark = Trim(txtQueryRemark.Text)
        name = Trim(txtQueryName.Text)


        '檢查是否至少輸入一項查詢條件
        If prno = "" And prno2 = "" And acno = "" And acno2 = "" And sAddDelDate = "" And sAddDelDate2 = "" _
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


        btnQuery.Enabled = False

        Dim info As New PGValueInfo(prno, prno2, acno, acno2, name, sAddDelDate, sAddDelDate2, amt, amt2, remark)

        Dim mDT As DataTable = New DataTable
        Dim mDV As DataView = New DataView

        mPreviousQueryInfo = info
        mDT = PGValueDAL.Query(info)
        mDV.Table = mDT
        mDV.Sort = "prno"
        DataGridView.DataSource = mDV
        DataGridView.DataBind()

        lbl_GrdCount.Text = mDV.Count
        If mDV.Count = 0 Then
            MessageBx("找不到資料，請重新設定查詢條件")
            '使大部分控制項失效
            'DisableControl()
        Else
            '使大部分控制項生效,並選擇第一筆資料
            ' EnableControl()
        End If
        TabContainer1.ActiveTabIndex = 0


        btnQuery.Enabled = True
    End Sub
    Sub FindData(ByVal strKey1 As String)
        '防呆*****
        If strKey1 = "" Then Exit Sub

        '設定關鍵值*****        
        txtKey1.Text = strKey1 : ViewState("FileKey") = strKey1

        '資料查詢*****
        Data_Load(strKey1) '載入資料
    End Sub

    Sub Data_Load(ByVal strKey1 As String)
        Dim intI, SumUp As Integer
        Dim strI, strColumn1, strColumn2 As String

        '開啟查詢
        objCon99 = New SqlConnection(DNS_PGM)
        objCon99.Open()

        Dim sqlstr, qstr, strD, strC As String

        sqlstr = "SELECT * from pptf020 " & _
         "where  id ='" & strKey1 & "'"


        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then

            mID = Trim(objDR99("id").ToString)
            txtPrNo.Text = Trim(objDR99("prno").ToString) '設定財物編號就會自動去查詢明細
            txtAddDelDate.Text = sDateADToCD(objDR99("pdate").ToString)
            txtAmt.Text = Trim(objDR99("amt").ToString)
            mAmt = Trim(objDR99("amt").ToString) '紀錄原始的增減金額  
            txtRemark.Text = Trim(objDR99("remark").ToString)
            btnAddReviseOK.Text = "確定修改"
            txtPrNo.Enabled = False
            btnDeleteOK.Enabled = True

            lblkey.Text = Trim(objDR99("id").ToString)
        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標

    End Sub

    Protected Sub btnDeleteOK_Click(sender As Object, e As EventArgs) Handles btnDeleteOK.Click
        '如果選到有效資料列,將刪除該資料
        mID = Trim(lblkey.Text)
        If mID <> "" Then
            Dim prno As String = txtPrNo.Text
            Dim id As String = mID
            Dim info As PGValueInfo
            Dim result As Integer

            btnDeleteOK.Enabled = False
            info = New PGValueInfo(id, prno, Nothing)
            result = PGValueDAL.Delete(info)
            Select Case result
                Case 1
                    MessageBx("刪除成功")
                    Exit Sub
                Case -1
                    MessageBx("刪除失敗，原因是 找不到此筆資料 ")
                Case Else
                    MessageBx("未知的傳回值")
            End Select
            btnDeleteOK.Enabled = True
        Else
            MessageBx("請先選擇一筆資料")
        End If
        


    End Sub
End Class