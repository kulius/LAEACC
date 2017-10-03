Imports System.Data
Imports System.Data.SqlClient

Public Class AC030
    Inherits System.Web.UI.Page

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString

    Public strBackColor As String = "" '開立傳票區背景顏色

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
        Master.Controller.objDataGridStyle(dtgSource, "M")
        Master.Controller.objDataGridStyle(dtgTarget, "S")

        UCBase1.Visible = False                 '初始化控制鍵

        'Focus*****
        strBackColor = "LightBlue"
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        Dim s1 As String = Request.QueryString("cvalue")

        lblYear.Text = Format(CInt(Session("sYear")), "000")
        If s1 = "AC030" Then
            ViewState("ac010kind") = "開立傳票"
            gbxModify.Visible = False
            gbxCreate.Visible = True
            btnFirstScreen.Visible = True

        Else
            ViewState("ac010kind") = "修改傳票"
            gbxModify.Visible = True
            gbxCreate.Visible = False
            btnFirstScreen.Visible = False
            txtOldNo.Focus()
        End If

        TabContainer1.Visible = False

        If Session("UnitTitle").indexof("彰化") >= 0 Then btnIntCopy.Visible = False '彰化傳票印一份botton不顯示



        'Call Begin_Data()

    End Sub

    Private Sub Begin_Data()
        Dim DAryAccno(8, 5) As String  '宣告DEBIT 9*6 array
        Dim DAryRemark(8, 5) As String
        Dim DAryAmt(8, 5) As Decimal
        Dim DAryCode(8, 5) As String
        Dim DAryOther(8, 5) As String
        Dim DAryQty(8, 5) As Decimal
        Dim CAryAccno(8, 5) As String  '宣告CREDIT 9*6 array
        Dim CAryRemark(8, 5) As String
        Dim CAryAmt(8, 5) As Decimal
        Dim CAryCode(8, 5) As String
        Dim CAryOther(8, 5) As String
        Dim CAryQty(8, 5) As Decimal


        For intI = 0 To 8
            For intJ = 0 To 5
                DAryAccno(intI, intJ) = ""
                DAryRemark(intI, intJ) = ""
                DAryAmt(intI, intJ) = 0
                DAryCode(intI, intJ) = ""
                DAryOther(intI, intJ) = ""
                DAryQty(intI, intJ) = 0
                CAryAccno(intI, intJ) = ""
                CAryRemark(intI, intJ) = ""
                CAryAmt(intI, intJ) = 0
                CAryCode(intI, intJ) = ""
                CAryOther(intI, intJ) = ""
                CAryQty(intI, intJ) = 0
            Next
        Next
        Session("DAryAccno") = DAryAccno
        Session("DAryRemark") = DAryRemark
        Session("DAryAmt") = DAryAmt
        Session("DAryCode") = DAryCode
        Session("DAryOther") = DAryOther
        Session("DAryQty") = DAryQty
        Session("CAryAccno") = CAryAccno
        Session("CAryRemark") = CAryRemark
        Session("CAryAmt") = CAryAmt
        Session("CAryCode") = CAryCode
        Session("CAryOther") = CAryOther
        Session("CAryQty") = CAryQty

        ViewState("DebitPage") = 0
        ViewState("CreditPage") = 0
        btnPageUp.Enabled = False
        btnPageDown.Enabled = True
        btnDebit.Visible = False
        btnCredit.Visible = True
        lblDC.Text = "轉帳借方"
        lblPage.Text = ViewState("DebitPage") + 1   '99/11/16 update 
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(dtgSource, "M")
        Master.Controller.objDataGridStyle(dtgTarget, "S")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        txtRemark1.Attributes.Add("maxlength", "170") '70
        txtRemark1.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark2.Attributes.Add("maxlength", "170")
        txtRemark2.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark3.Attributes.Add("maxlength", "170")
        txtRemark3.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark4.Attributes.Add("maxlength", "170")
        txtRemark4.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark5.Attributes.Add("maxlength", "170")
        txtRemark5.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark6.Attributes.Add("maxlength", "170")
        txtRemark6.Attributes.Add("onkeyup", "return ismaxlength(this)")

        txtAmt2.Attributes.Add("OnBlur", "return sumamt()")
        txtAmt3.Attributes.Add("OnBlur", "return sumamt()")
        txtAmt4.Attributes.Add("OnBlur", "return sumamt()")
        txtAmt5.Attributes.Add("OnBlur", "return sumamt()")
        txtAmt6.Attributes.Add("OnBlur", "return sumamt()")
        If Not IsPostBack Then
            '其他預設值*****
            'SetControls(0) '設定所有輸入控制項的唯讀狀態

            '資料查詢*****
            'FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            Call Begin_Data()
            AddDefaultFirstRecord()

            btnFinish.Attributes.Add("onclick", "this.disabled = true;this.value = '請稍候..';" + Page.ClientScript.GetPostBackEventReference(btnFinish, ""))

        End If
    End Sub
    Sub AddDefaultFirstRecord()
        'creating dataTable   
        Dim dt As New DataTable()
        Dim dr As DataRow
        dt.TableName = "dtgTarget"
        dt.Columns.Add(New DataColumn("BGNO", GetType(String)))
        dt.Columns.Add(New DataColumn("ACCNO", GetType(String)))
        dt.Columns.Add(New DataColumn("REMARK", GetType(String)))
        dt.Columns.Add(New DataColumn("useamt", GetType(String)))
        dt.Columns.Add(New DataColumn("SUBJECT", GetType(String)))
        dt.Columns.Add(New DataColumn("kind", GetType(String)))
        dt.Columns.Add(New DataColumn("autono", GetType(String)))
        dt.Columns.Add(New DataColumn("dc", GetType(String)))
        'dr = dt.NewRow()
        'dt.Rows.Add(dr)
        'saving databale into viewstate   
        ViewState("dtgTarget") = dt
        'bind Gridview  
        dtgTarget.DataSource = dt
        dtgTarget.DataBind()
    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
    'UserControl控制項
    Protected Sub UCBase1_Click(ByVal sender As Object, ByVal e As UCBase.ClickEventArgs) Handles UCBase1.Click
        ''ViewState("MyStatus")：目前按鍵狀態
        'Select Case e.CommandName
        '    Case "First"      '第一筆
        '        FlagMoveSeat(1, 0)
        '    Case "Prior"      '上一筆
        '        FlagMoveSeat(2, ViewState("ItemIndex"))
        '    Case "Next"       '下一筆
        '        FlagMoveSeat(3, ViewState("ItemIndex"))
        '    Case "Last"       '最末筆
        '        FlagMoveSeat(4, DataGridView.Items.Count - 1)

        '    Case "Save"       '存檔
        '        SaveData(ViewState("MyStatus"))
        '    Case "CancelEdit" '還原
        '        ResetData()
        '    Case "Copy"       '複製
        '        ViewState("MyStatus") = 1
        '        SetControls(3)
        '    Case "AddNew"     '新增       
        '        ViewState("MyStatus") = 1
        '        SetControls(1)
        '    Case "Edit"       '修改
        '        ViewState("MyStatus") = 2
        '        SetControls(2)
        '    Case "Delete"     '刪除
        '        DeleteData()
        'End Select
    End Sub
#End Region
#Region "@DataGridView@"
    '點選資訊
    Sub dtgSource_SortCommand(ByVal sender As Object, ByVal e As DataGridSortCommandEventArgs) Handles dtgSource.SortCommand
        ViewState("MyOrder") = e.SortExpression
        ViewState("MySort") = IIf(ViewState("MySort") = "" Or ViewState("MySort") = "ASC", "DESC", "ASC")

        LoadGridFunc(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '資料查詢
    End Sub

    Sub dtgSource_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles dtgSource.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號
        Dim bgno As Label = e.Item.FindControl("請購編號") '記錄編號
        Dim accno As Label = e.Item.FindControl("科目") '記錄編號
        Dim remark As Label = e.Item.FindControl("摘要") '記錄編號
        Dim subject As Label = e.Item.FindControl("受款人") '記錄編號
        Dim useamt As Label = e.Item.FindControl("金額") '記錄編號
        Dim kind As Label = e.Item.FindControl("kind") '記錄編號
        Dim autono As Label = e.Item.FindControl("autono") '記錄編號
        Dim dc As Label = e.Item.FindControl("dc") '記錄編號


        Select Case e.CommandName
            Case "btnShow"
                '查詢資料及控制*****
                'FindData(txtID.Text)               '查詢主檔
                'FlagMoveSeat(0, e.Item.ItemIndex)
                'TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤

                If ViewState("dtgTarget") IsNot Nothing Then
                    'get datatable from view state   
                    Dim dtCurrentTable As DataTable = DirectCast(ViewState("dtgTarget"), DataTable)
                    Dim drCurrentRow As DataRow = Nothing


                    If accno.Text = "" Then Exit Sub
                    'If dtCurrentTable.Rows.Count < 5 Then    '只置五筆記錄
                    '    If dtCurrentTable.Rows.Count = 0 Then    '記錄第一筆會計科目以便控制四級科目相同
                    '        ViewState("strAccno4") = accno.Text.Substring(0, 5)
                    '    Else
                    '        If accno.Text.Substring(0, 5) <> ViewState("strAccno4") Then
                    '            MessageBx("四級科目不相同")
                    '            Exit Sub
                    '        End If
                    '    End If

                    drCurrentRow = dtCurrentTable.NewRow()

                    drCurrentRow("bgno") = bgno.Text
                    drCurrentRow("accno") = accno.Text
                    drCurrentRow("remark") = remark.Text
                    drCurrentRow("subject") = subject.Text
                    drCurrentRow("useamt") = useamt.Text
                    drCurrentRow("kind") = kind.Text
                    drCurrentRow("autono") = autono.Text
                    If ViewState("sFile") = "1" Then
                        drCurrentRow("dc") = dc.Text
                        'If Mid(bmS.Current("ACCNO"), 1, 5) = "21302" Then
                        '    If bmS.Current("dc") = "1" Then nr("dc") = "2"
                        '    If bmS.Current("dc") = "2" Then nr("dc") = "1"
                        'End If
                    End If

                    dtCurrentTable.Rows.Add(drCurrentRow)

                    'End If
                    ViewState("dtgTarget") = dtCurrentTable
                    'Bind Gridview with latest Row  
                    dtgTarget.DataSource = dtCurrentTable
                    dtgTarget.DataBind()
                End If


        End Select
    End Sub
    Sub dtgTarget_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles dtgTarget.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號


        Select Case e.CommandName
            Case "btnDelete"
                If ViewState("dtgTarget") IsNot Nothing Then
                    'get datatable from view state   
                    Dim dtCurrentTable As DataTable = DirectCast(ViewState("dtgTarget"), DataTable)
                    Dim drCurrentRow As DataRow = Nothing

                    For RowI = 0 To dtCurrentTable.Rows.Count - 1
                        If dtCurrentTable.Rows(RowI)(0).ToString() = txtID.Text Then
                            dtCurrentTable.Rows(RowI).Delete()
                            Exit For
                        End If
                    Next
                    dtCurrentTable.AcceptChanges()

                    ViewState("dtgTarget") = dtCurrentTable
                    dtgTarget.DataSource = dtCurrentTable
                    dtgTarget.DataBind()
                End If

        End Select
    End Sub

#End Region
#Region "按鍵選項"
    '查詢
    'Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
    '    Dim strMySearch As String = ""

    '    '開啟查詢*****
    '    '項目
    '    ' If nudYear.SelectedValue <> "" Then strMySearch &= " AND a.ACCYEAR LIKE '%" & S_ACCYEAR.SelectedValue & "%'"


    '    '初始化*****
    '    UCBase1.SetButtons()                         '初始化控制鍵

    '    ViewState("MySearch") = strMySearch          '查詢值記錄
    '    FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '開啟查詢
    'End Sub


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

        'sqlstr = "SELECT a.bg1+a.bg2+a.bg3+a.bg4+a.bg5 as bgnet, a.*, " & _
        '         "CASE WHEN len(a.accno) = 17 THEN c.ACCNAME+'-'+b.accname " & _
        '         " WHEN len(a.accno) <> 17 THEN b.accname END AS accname" & _
        '         " FROM  BGF010 a" & _
        '         " LEFT OUTER JOIN accname b ON a.accno = b.accno" & _
        '         " left outer join accname c ON LEFT(a.ACCNO, 16) = c.ACCNO and len(a.accno)=17 " & _
        '         " WHERE accyear=" & nudYear.Text & " and a.accno>='" & _
        '          Trim(vxtStartNo.Text) & "' and a.accno<='" & Trim(vxtEndNo.Text) & "'"
        'If Mid(Session("UserUnit"), 1, 2) = "05" Then   '主計人員
        '    sqlstr = sqlstr & " ORDER BY a.accyear,a.accno"
        'Else
        '    sqlstr = sqlstr & " and b.STAFF_NO = '" & Session("USERID") & _
        '             "' ORDER BY a.accyear,a.accno"

        '    If ViewState("strYes") <> "Y" Then
        '        UCBase1.Visible = False '不可異動
        '        lblMsgMod.Text = "目前主計室控制業務單位不可修改預算資料"
        '    Else
        '        UCBase1.Visible = True '可異動
        '    End If
        'End If

        'lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
        'Master.Controller.objDataGrid(DataGridView, lbl_GrdCount, DNS_ACC, sqlstr, "查詢資料檔")

        ''判斷是否有值可供選擇*****
        'If DataGridView.Items.Count > 0 Then
        '    Dim txtID As Label = DataGridView.Items(0).FindControl("id")
        '    txtKey1.Text = txtID.Text
        '    FindData(txtID.Text)
        'End If
    End Sub
    '移動DataGridView指標
    Public Sub FlagMoveSeat(ByVal Status As Integer, Optional ByVal ItemIndex As Integer = 0)
        'Dim myItemIndex As Integer = 0

        'Try
        '    'Status: 0:隨機點選 1:第一筆 2:上一筆 3:下一筆 4:最未筆
        '    Select Case Status
        '        Case 1
        '            myItemIndex = 0
        '        Case 2
        '            myItemIndex = ItemIndex - 1
        '        Case 3
        '            myItemIndex = ItemIndex + 1
        '        Case 4
        '            myItemIndex = DataGridView.Items.Count - 1
        '        Case 0
        '            myItemIndex = ItemIndex
        '    End Select

        '    myItemIndex = DataGridView.Items.Item(myItemIndex).ItemIndex


        '    '關鍵值*****
        '    Dim id As Label = DataGridView.Items.Item(myItemIndex).FindControl("id") '記錄編號


        '    '查詢資料及控制*****
        '    FindData(id.Text)
        '    ViewState("MyStatus") = 0
        '    ViewState("ItemIndex") = myItemIndex
        'Catch ex As Exception

        'End Try
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


        Return blnCheck
    End Function
    '修改
    Public Function UpdateData() As Boolean
        Dim blnCheck As Boolean = False


        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False


        '異動後初始化*****
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作刪除資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
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
        'Data_Load(strKey1) '載入資料
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


    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        If rdbFile1.Checked Then ViewState("sFile") = "1"
        If rdbFile2.Checked Then ViewState("sFile") = "2"
        If rdbFile3.Checked Then ViewState("sFile") = "3"

        If rdbFile3.Checked Then '空白資料
            ViewState("sFile") = "3"
            vxtAccno1.Focus()
            Call clsScreen()
            'If Not IsDBNull(myDatasetS) Then myDatasetS.Clear()
        End If
        lblFile.Text = IIf(ViewState("sFile") = "1", "請輸入預算轉帳編號", "請輸入保證品工程編號")
        TabContainer1.ActiveTabIndex = IIf(ViewState("sFile") = "3", 1, 0)
        TabContainer1.Visible = True
        vxtAccno1.Focus()
        If ViewState("sFile") <= "2" Then Call LoadGridFunc()
        If ViewState("ac010kind") = "開立傳票" Then
            gbxCreate.Visible = False
        End If

    End Sub

    Sub LoadGridFunc(Optional ByVal strOrder As String = "", Optional ByVal strSortType As String = "", Optional ByVal strSearch As String = "")
        '將bgf030->no_1_no=0置入source datagrid 
        Dim sqlstr, qstr, sortstr As String
        If ViewState("sFile") = "1" Then   '資料來源:預算資料(取no_1_no=0 and 已開支(date4開支日期<>null) 借方kind='3'  貸方kind='4')
            sqlstr = "SELECT bgf030.bgno, abs(BGF030.useamt) as useamt, bgf030.remark,bgf030.autono, " & _
                     "bgf020.subject, accname.bookaccno as accno, bgf020.kind as kind, bgf020.dc as dc  " & _
                    "FROM BGF030  LEFT OUTER JOIN BGF020 ON BGF030.bgno=BGF020.bgno " & _
                    "LEFT OUTER JOIN ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                    "WHERE BGF030.date4 is not null and year(bgf030.date4)=" & CInt(Session("sYear")) + 1911 & _
                    " and BGF030.no_1_no = 0 "  'INCLUDE 收支
            sqlstr += IIf(strOrder = "", " order by bgno", " ORDER BY " & strOrder & " " & strSortType)
            ' " and BGF030.no_1_no = 0 and BGF020.kind>='3' order by bgf030.bgno"
        Else    '資料來源:保證品資料 (取no_1_no=0 and 收入或支出資料決定借方貸方傳票)23102存入保證品,對方科目15303保管品 
            sqlstr = "SELECT bailf020.engno as bgno,'23102' as accno, bailf020.rp as kind, " & _
                     " enf010.engname as remark, bailf020.amt as useamt, " & _
                     " bailf020.cop as subject, bailf020.autono as autono,'' as dc " & _
                     " FROM bailf020  LEFT OUTER JOIN enf010 ON bailf020.engno=enf010.engno " & _
                     " WHERE bailf020.no_1_no = 0 and year(bailf020.rpdate)=" & _
                      CInt(Session("sYear")) + 1911 & " "
            sqlstr += IIf(strOrder = "", " order by bgno", " ORDER BY " & strOrder & " " & strSortType)
        End If
        Master.Controller.objDataGrid(dtgSource, lbl_dtgSourceGrdCount, DNS_ACC, sqlstr, "查詢資料檔")
        'myDatasetS = ws1.openmember("", "ac010s", sqlstr)
        'dtgSource.DataSource = myDatasetS
        'dtgSource.DataMember = "ac010s"
        'bmS = Me.BindingContext(myDatasetS, "ac010s")
        'myDatasetT = myDatasetS.Copy()
        'myDatasetT.Clear()
        'dtgTarget.DataSource = myDatasetT
        'dtgTarget.DataMember = "ac010s"
        'bmT = Me.BindingContext(myDatasetT, "ac010s")
        'TabControl1.SelectedIndex = 0
        If ViewState("dtgTarget") IsNot Nothing Then
            'get datatable from view state   
            Dim dtCurrentTable As DataTable = DirectCast(ViewState("dtgTarget"), DataTable)
            dtCurrentTable.Clear()
            ViewState("dtgTarget") = dtCurrentTable
            dtgTarget.DataSource = dtCurrentTable
            dtgTarget.DataBind()
        End If

        lblUseNO.Text = Str(Master.Controller.QueryNO(Session("sYear"), "3"))
        lblNo_1_no.Text = Val(lblUseNO.Text) + 1

    End Sub

    Protected Sub btnOldNo_Click(sender As Object, e As EventArgs) Handles btnOldNo.Click
        If txtOldNo.Text = "" Then Exit Sub
        Call Begin_Data()

        Dim DAryAccno(8, 5) As String  '宣告DEBIT 9*6 array
        Dim DAryRemark(8, 5) As String
        Dim DAryAmt(8, 5) As Decimal
        Dim DAryCode(8, 5) As String
        Dim DAryOther(8, 5) As String
        Dim DAryQty(8, 5) As Decimal
        Dim CAryAccno(8, 5) As String  '宣告CREDIT 9*6 array
        Dim CAryRemark(8, 5) As String
        Dim CAryAmt(8, 5) As Decimal
        Dim CAryCode(8, 5) As String
        Dim CAryOther(8, 5) As String
        Dim CAryQty(8, 5) As Decimal

        DAryAccno = DirectCast(Session("DAryAccno"), String(,))
        DAryRemark = DirectCast(Session("DAryRemark"), String(,))
        DAryAmt = DirectCast(Session("DAryAmt"), Decimal(,))
        DAryCode = DirectCast(Session("DAryCode"), String(,))
        DAryOther = DirectCast(Session("DAryOther"), String(,))
        DAryQty = DirectCast(Session("DAryQty"), Decimal(,))
        CAryAccno = DirectCast(Session("CAryAccno"), String(,))
        CAryRemark = DirectCast(Session("CAryRemark"), String(,))
        CAryAmt = DirectCast(Session("CAryAmt"), Decimal(,))
        CAryCode = DirectCast(Session("CAryCode"), String(,))
        CAryOther = DirectCast(Session("CAryOther"), String(,))
        CAryQty = DirectCast(Session("CAryQty"), Decimal(,))



        Dim SumAmt As Decimal = 0
        Dim intI As Integer
        Dim strI, sqlstr As String
        Dim tempdataset As DataSet
        '總帳
        sqlstr = "SELECT * from acf010 where accyear=" & Session("sYear") & " and kind>='3'" & _
                 " and no_1_no=" & txtOldNo.Text & " order by dc, item"
        tempdataset = Master.ADO.openmember(DNS_ACC, "ac010s", sqlstr)
        If tempdataset.Tables("ac010s").Rows.Count <= 0 Then
            MessageBx("無此傳票")
            txtOldNo.Focus()
            Exit Sub
        End If
        If tempdataset.Tables("ac010s").Rows(0)("no_2_no") <> 0 Then
            MessageBx("此傳票已作帳,傳票號=" & tempdataset.Tables("ac010s").Rows(0)("no_2_no"))
            txtOldNo.Focus()
            Exit Sub
        End If
        lblNo_1_no.Text = txtOldNo.Text                         '原製票編號
        With tempdataset.Tables("ac010s")
            Dim intPage As Integer
            lblDate1.Text = Master.Models.strDateADToChiness(.Rows(0).Item("date_1"))  '原製票日期
            For intI = 0 To .Rows.Count - 1
                intPage = .Rows(intI)("seq") - 1
                If .Rows(intI)("dc") = "1" Then
                    DAryAccno(intPage, 0) = .Rows(intI)("accno")
                    DAryRemark(intPage, 0) = .Rows(intI)("remark")
                    DAryAmt(intPage, 0) = .Rows(intI)("amt")
                Else
                    CAryAccno(intPage, 0) = .Rows(intI)("accno")
                    CAryRemark(intPage, 0) = .Rows(intI)("remark")
                    CAryAmt(intPage, 0) = .Rows(intI)("amt")
                End If
            Next
        End With
        '明細帳
        sqlstr = "SELECT * from acf020 where accyear=" & Session("sYear") & " and kind>='3'" & _
                 " and no_1_no=" & txtOldNo.Text & " order by dc, item"
        tempdataset = Master.ADO.openmember(DNS_ACC, "ac010s", sqlstr)
        With tempdataset.Tables("ac010s")
            Dim intPage, intItem As Integer
            For intI = 0 To .Rows.Count - 1
                intPage = .Rows(intI)("seq") - 1
                intItem = .Rows(intI)("item") - 1
                If .Rows(intI)("dc") = "1" Then
                    DAryAccno(intPage, intItem) = .Rows(intI)("accno")
                    DAryRemark(intPage, intItem) = .Rows(intI)("remark")
                    DAryAmt(intPage, intItem) = .Rows(intI)("amt")
                    DAryCode(intPage, intItem) = .Rows(intI)("cotn_code").ToString
                    DAryOther(intPage, intItem) = .Rows(intI)("other_accno").ToString
                    DAryQty(intPage, intItem) = Format(.Rows(intI)("mat_qty"), "0")
                Else
                    CAryAccno(intPage, intItem) = .Rows(intI)("accno")
                    CAryRemark(intPage, intItem) = .Rows(intI)("remark")
                    CAryAmt(intPage, intItem) = .Rows(intI)("amt")
                    CAryCode(intPage, intItem) = .Rows(intI)("cotn_code").ToString
                    CAryOther(intPage, intItem) = .Rows(intI)("other_accno").ToString
                    CAryQty(intPage, intItem) = Format(.Rows(intI)("mat_qty"), "0")
                End If
            Next
        End With
        tempdataset = Nothing
        Session("DAryAccno") = DAryAccno
        Session("DAryRemark") = DAryRemark
        Session("DAryAmt") = DAryAmt
        Session("DAryCode") = DAryCode
        Session("DAryOther") = DAryOther
        Session("DAryQty") = DAryQty
        Session("CAryAccno") = CAryAccno
        Session("CAryRemark") = CAryRemark
        Session("CAryAmt") = CAryAmt
        Session("CAryCode") = CAryCode
        Session("CAryOther") = CAryOther
        Session("CAryQty") = CAryQty

        ViewState("DebitPage") = 0
        ViewState("CreditPage") = 0
        Call Load_Debit()
        TabContainer1.Visible = True
        TabContainer1.ActiveTabIndex = 1
    End Sub


    Sub enableOther()   '相關科目
        vxtOther2.Visible = True
        vxtOther3.Visible = True
        vxtOther4.Visible = True
        vxtOther5.Visible = True
        vxtOther6.Visible = True
        btnF42.Visible = True
        btnF43.Visible = True
        btnF44.Visible = True
        btnF45.Visible = True
        btnF46.Visible = True
    End Sub

    Sub disableOther()
        vxtOther2.Visible = False
        vxtOther3.Visible = False
        vxtOther4.Visible = False
        vxtOther5.Visible = False
        vxtOther6.Visible = False
        btnF42.Visible = False
        btnF43.Visible = False
        btnF44.Visible = False
        btnF45.Visible = False
        btnF46.Visible = False
    End Sub

    Protected Sub vxtAccno2_TextChanged(sender As Object, e As EventArgs)
        If Len(vxtAccno2.Text) > 17 Then
            lblAccName2.Text = vxtAccno2.Text.Substring(17)
            vxtAccno2.Text = vxtAccno2.Text.Substring(0, 16)
        End If
    End Sub
    Protected Sub vxtAccno3_TextChanged(sender As Object, e As EventArgs)
        If Len(vxtAccno3.Text) > 17 Then
            lblAccName3.Text = vxtAccno3.Text.Substring(17)
            vxtAccno3.Text = vxtAccno3.Text.Substring(0, 16)
        End If
    End Sub
    Protected Sub vxtAccno4_TextChanged(sender As Object, e As EventArgs)
        If Len(vxtAccno4.Text) > 17 Then
            lblAccName4.Text = vxtAccno4.Text.Substring(17)
            vxtAccno4.Text = vxtAccno4.Text.Substring(0, 16)
        End If
    End Sub
    Protected Sub vxtAccno5_TextChanged(sender As Object, e As EventArgs)
        If Len(vxtAccno5.Text) > 17 Then
            lblAccName5.Text = vxtAccno5.Text.Substring(17)
            vxtAccno5.Text = vxtAccno5.Text.Substring(0, 16)
        End If
    End Sub
    Protected Sub vxtAccno6_TextChanged(sender As Object, e As EventArgs)
        If Len(vxtAccno6.Text) > 17 Then
            lblAccName6.Text = vxtAccno6.Text.Substring(17)
            vxtAccno6.Text = vxtAccno6.Text.Substring(0, 16)
        End If
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        If lblDC.Text = "轉帳借方" Then
            Call Save_Debit()
            If ViewState("checkError") = True Then Exit Sub
        Else
            Call Save_Credit()
            If ViewState("checkError") = True Then Exit Sub
        End If

        Dim SumDebit, SumCredit As Decimal

        Dim DAryAccno(8, 5) As String  '宣告DEBIT 9*6 array
        Dim DAryRemark(8, 5) As String
        Dim DAryAmt(8, 5) As Decimal
        Dim DAryCode(8, 5) As String
        Dim DAryOther(8, 5) As String
        Dim DAryQty(8, 5) As Decimal
        Dim CAryAccno(8, 5) As String  '宣告CREDIT 9*6 array
        Dim CAryRemark(8, 5) As String
        Dim CAryAmt(8, 5) As Decimal
        Dim CAryCode(8, 5) As String
        Dim CAryOther(8, 5) As String
        Dim CAryQty(8, 5) As Decimal

        DAryAccno = DirectCast(Session("DAryAccno"), String(,))
        DAryRemark = DirectCast(Session("DAryRemark"), String(,))
        DAryAmt = DirectCast(Session("DAryAmt"), Decimal(,))
        DAryCode = DirectCast(Session("DAryCode"), String(,))
        DAryOther = DirectCast(Session("DAryOther"), String(,))
        DAryQty = DirectCast(Session("DAryQty"), Decimal(,))
        CAryAccno = DirectCast(Session("CAryAccno"), String(,))
        CAryRemark = DirectCast(Session("CAryRemark"), String(,))
        CAryAmt = DirectCast(Session("CAryAmt"), Decimal(,))
        CAryCode = DirectCast(Session("CAryCode"), String(,))
        CAryOther = DirectCast(Session("CAryOther"), String(,))
        CAryQty = DirectCast(Session("CAryQty"), Decimal(,))

        Dim box As TextBox
        Dim lbl As Label


        '檢查借貸平衡
        For intI = 0 To 8  '共9頁
            SumDebit += DAryAmt(intI, 0)   '0代表第一項(總帳項)
            SumCredit += CAryAmt(intI, 0)
        Next
        If SumDebit <> SumCredit Then
            MessageBx("借貸不平衡" & SumDebit & "<>" & SumCredit)
            Exit Sub
        End If

        '修改傳票應先刪除原資料
        Dim retstr As String
        If ViewState("ac010kind") = "修改傳票" Then
            sqlstr = "DELETE acf010 where accyear=" & Session("sYear") & " and kind>='3' and no_1_no=" & txtOldNo.Text
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then MessageBx("ACF010刪除失敗" & sqlstr)
            sqlstr = "DELETE acf020 where accyear=" & Session("sYear") & " and kind>='3' and no_1_no=" & txtOldNo.Text
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then MessageBx("ACF020刪除失敗" & sqlstr)
        End If

        '寫入資料表acf010傳票總帳檔 & 寫入資料表acf020傳票明細
        Dim intNo As Integer
        Dim strdate1 As String
        If ViewState("ac010kind") = "修改傳票" Then
            intNo = Val(txtOldNo.Text)                 '製票編號
            strdate1 = lblDate1.Text                   '製票日期
        Else
            intNo = Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "3")    '\accservice\service1.asmx
            strdate1 = Session("UserDate")         '製票日期
        End If

        Dim IsError As Boolean = False  '計錄insert是否有失敗
        For intI = 0 To 8
            If DAryAccno(intI, 0) = "" Then Exit For
            '總帳項
            Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
            Master.ADO.GenInsSql("kind", "3", "T")
            Master.ADO.GenInsSql("no_1_no", intNo, "N")
            Master.ADO.GenInsSql("no_2_no", 0, "N")
            Master.ADO.GenInsSql("SEQ", CType(intI + 1, String), "T")
            Master.ADO.GenInsSql("item", "1", "T")  '總帳項為1
            Master.ADO.GenInsSql("date_1", strdate1, "D")   '製票日期
            Master.ADO.GenInsSql("systemdate", Format(Now(), "yyyy-MM-dd"), "D")   '系統日期(藉以記錄user實際update日期)
            Master.ADO.GenInsSql("dc", "1", "T")
            Master.ADO.GenInsSql("accno", DAryAccno(intI, 0), "T")
            Master.ADO.GenInsSql("remark", DAryRemark(intI, 0), "U")   '摘要
            Master.ADO.GenInsSql("amt", DAryAmt(intI, 0), "N")
            Master.ADO.GenInsSql("act_amt", 0, "N")
            Master.ADO.GenInsSql("bank", "  ", "T")
            Master.ADO.GenInsSql("books", " ", "T")                 '過帳碼
            sqlstr = "insert into acf010 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("ACF010新增失敗" & sqlstr)
                IsError = True
            End If
            '明細帳項
            For intJ = 1 To 5
                If DAryAccno(intI, intJ) = "" Then Exit For
                Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
                Master.ADO.GenInsSql("kind", "3", "T")                    '借方傳票kind="3"
                Master.ADO.GenInsSql("no_1_no", intNo, "N")
                Master.ADO.GenInsSql("no_2_no", 0, "N")
                Master.ADO.GenInsSql("SEQ", CType(intI + 1, String), "T")    '頁次
                Master.ADO.GenInsSql("item", CType(intJ + 1, String), "T")   '項次
                Master.ADO.GenInsSql("dc", "1", "T")
                Master.ADO.GenInsSql("accno", DAryAccno(intI, intJ), "T")
                Master.ADO.GenInsSql("remark", DAryRemark(intI, intJ), "U")
                Master.ADO.GenInsSql("amt", DAryAmt(intI, intJ), "N")
                Master.ADO.GenInsSql("cotn_code", DAryCode(intI, intJ), "T")
                Master.ADO.GenInsSql("mat_qty", DAryQty(intI, intJ), "N")                '材料數量
                If DAryAmt(intI, intJ) <> 0 And DAryQty(intI, intJ) <> 0 Then Master.ADO.GenInsSql("mat_pric", DAryAmt(intI, intJ) / DAryQty(intI, intJ), "N") '材料單價
                Master.ADO.GenInsSql("other_accno", DAryOther(intI, intJ), "T")          '相關科目
                sqlstr = "insert into acf020 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                If retstr <> "sqlok" Then
                    MessageBx("ACF020新增失敗" & sqlstr)
                    IsError = True
                End If
            Next
        Next
        For intI = 0 To 8
            If CAryAccno(intI, 0) = "" Then Exit For
            '總帳項
            Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
            Master.ADO.GenInsSql("kind", "4", "T")
            Master.ADO.GenInsSql("no_1_no", intNo, "N")
            Master.ADO.GenInsSql("no_2_no", 0, "N")
            Master.ADO.GenInsSql("SEQ", CType(intI + 1, String), "T")
            Master.ADO.GenInsSql("item", "1", "T")  '總帳項為1
            Master.ADO.GenInsSql("date_1", strdate1, "D")   '製票日期
            Master.ADO.GenInsSql("systemdate", Format(Now(), "yyyy-MM-dd"), "D")   '系統日期(藉以記錄user實際update日期)
            Master.ADO.GenInsSql("dc", "2", "T")
            Master.ADO.GenInsSql("accno", CAryAccno(intI, 0), "T")
            Master.ADO.GenInsSql("remark", CAryRemark(intI, 0), "U")   '摘要
            Master.ADO.GenInsSql("amt", CAryAmt(intI, 0), "N")
            Master.ADO.GenInsSql("act_amt", 0, "N")
            Master.ADO.GenInsSql("bank", "  ", "T")
            Master.ADO.GenInsSql("books", " ", "T")                 '過帳碼
            sqlstr = "insert into acf010 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then MessageBx("ACF010新增失敗" & sqlstr)
            '明細帳項
            For intJ = 1 To 5
                If CAryAccno(intI, intJ) = "" Then Exit For
                Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
                Master.ADO.GenInsSql("kind", "4", "T")                    '借方傳票kind="3"
                Master.ADO.GenInsSql("no_1_no", intNo, "N")
                Master.ADO.GenInsSql("no_2_no", 0, "N")
                Master.ADO.GenInsSql("SEQ", CType(intI + 1, String), "T")    '頁次
                Master.ADO.GenInsSql("item", CType(intJ + 1, String), "T")   '項次
                Master.ADO.GenInsSql("dc", "2", "T")
                Master.ADO.GenInsSql("accno", CAryAccno(intI, intJ), "T")
                Master.ADO.GenInsSql("remark", CAryRemark(intI, intJ), "U")
                Master.ADO.GenInsSql("amt", CAryAmt(intI, intJ), "N")
                Master.ADO.GenInsSql("cotn_code", CAryCode(intI, intJ), "T")
                Master.ADO.GenInsSql("mat_qty", CAryQty(intI, intJ), "N")               '材料數量
                If CAryAmt(intI, intJ) <> 0 And CAryQty(intI, intJ) <> 0 Then Master.ADO.GenInsSql("mat_pric", CAryAmt(intI, intJ) / CAryQty(intI, intJ), "N") '材料單價
                Master.ADO.GenInsSql("other_accno", CAryOther(intI, intJ), "T")          '相關科目
                sqlstr = "insert into acf020 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                If retstr <> "sqlok" Then
                    MessageBx("ACF020新增失敗" & sqlstr)
                    IsError = True
                End If
            Next
        Next

        If IsError Then   '新增失敗
            sqlstr = "delete from acf010 where accyear=" & Session("sYear") & " and no_1_no=" & intNo & " and kind>='3' " & _
                     " and no_2_no=0 and date_2='" & Master.Models.FullDate(strdate1) & "'"   '以確切dele方才的insert 防止刪除原有資料 
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("刪除 ACF010失敗" & sqlstr)
            End If
            sqlstr = "delete from acf020 where accyear=" & Session("sYear") & " and no_1_no=" & intNo & " and kind>='3' " & _
                     " and no_2_no=0 "   '以確切dele方才的insert 防止刪除原有資料 
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("刪除 ACF020失敗" & sqlstr)
            End If
            Exit Sub
        End If

        '回寫編號至傳票內容來源
        Dim bmT As DataTable
        If ViewState("dtgTarget") IsNot Nothing Then
            bmT = DirectCast(ViewState("dtgTarget"), DataTable)
        Else
            MessageBx("ViewState 中無選取資料")
            Exit Sub
        End If

        If ViewState("ac010kind") = "開立傳票" Then
            If ViewState("sFile") = "1" Then   '回寫編號至BGF030->no_1_no,並清空傳票內容選入區
                Dim intAutono As Integer
                For intI = 0 To bmT.Rows.Count - 1
                    intAutono = bmT.Rows(intI)("autono")
                    sqlstr = "UPDATE bgf030 SET no_1_no = " & intNo & " WHERE autono = " & intAutono
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                Next
            Else
                If ViewState("sFile") = "2" Then
                    '回寫編號至bailf020->no_1_no,並清空傳票內容選入區
                    Dim intAutono As Integer
                    For intI = 0 To bmT.Rows.Count - 1
                        intAutono = bmT.Rows(intI)("autono")
                        sqlstr = "UPDATE bailf020 SET no_1_no = " & intNo & " WHERE autono = " & intAutono
                        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    Next
                End If
            End If
            lblUseNO.Text = Str(intNo)     '顯示實際使用編號
        Else
            TabContainer1.Visible = False
        End If


        If btnIntCopy.Text = "傳票印一份" Then
            PrintTransSlip(Session("sYear"), intNo, Session("UnitTitle"), 0) '傳票印一份
            'PrintTransSlip(sYear, intNo, TransPara.TransP("UnitTitle"), 0)     '列印傳票 at printslip.vb
        Else
            PrintTransSlip(Session("sYear"), intNo, Session("UnitTitle"), 1) '傳票印一份
            PrintTransSlip(Session("sYear"), intNo, Session("UnitTitle"), 2) '傳票印一份
            'PrintTransSlip(sYear, intNo, TransPara.TransP("UnitTitle"), 1)     '列印傳票 at printslip.vb
            'PrintTransSlip(sYear, intNo, TransPara.TransP("UnitTitle"), 2)     '列印傳票 at printslip.vb
        End If
        Call Begin_Data()  'clear array
        Call clsScreen()               '清傳票螢幕
        If ViewState("ac010kind") = "開立傳票" Then
            If ViewState("sFile") = "3" Then   '由空白搜尋時  轉至空白傳票頁
                TabContainer1.ActiveTabIndex = 1  '回空白傳票頁               
            Else
                LoadGridFunc()
                TabContainer1.ActiveTabIndex = 0  '回傳票來源頁datagrid PAGE 1
                txtNo.Focus()
            End If
        End If
    End Sub

    Protected Sub btnReturn_Click(sender As Object, e As EventArgs) Handles btnReturn.Click
        If ViewState("ac010kind") = "開立傳票" Then
            TabContainer1.ActiveTabIndex = 0
        Else
            TabContainer1.Visible = False
        End If
    End Sub

    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click

        Dim SumAmt As Decimal = 0
        Dim intI As Integer
        Dim strI, sqlstr As String
        Dim tempdataset As DataSet

        Dim bmT As DataTable
        If ViewState("dtgTarget") IsNot Nothing Then 
            bmT = DirectCast(ViewState("dtgTarget"), DataTable)
        Else
            MessageBx("ViewState 中無選取資料")
            Exit Sub
        End If

        'Call Begin_Data()  'clear array
        ViewState("intD") = 0  'debit record number 
        ViewState("intC") = 0   'credit record number 
        ViewState("strAccnoD") = ""
        ViewState("strAccnoC") = ""

        For intI = 0 To bmT.Rows.Count - 1
            ViewState("strAccno") = bmT.Rows(intI)("accno").ToString()
            If ViewState("sFile") <> "1" And bmT.Rows(intI)("accno").ToString() = "23102" Then '保證品
                If bmT.Rows(intI)("kind").ToString() = "2" Then  '保證品支出(De:2-3102 Cr:1-5303)
                    Call Add_debit(bmT.Rows(intI))
                    ViewState("strAccno") = "15303"
                    Call Add_credit(bmT.Rows(intI))
                Else                               '保證品收入"1"(De:1-5303 Cr:2-3102)
                    Call Add_credit(bmT.Rows(intI))
                    ViewState("strAccno") = "15303"
                    Call Add_debit(bmT.Rows(intI))
                End If
            End If
            If ViewState("sFile") = "1" Then   '預算資料
                If Mid(bmT.Rows(intI)("accno").ToString(), 1, 5) = "21302" Then '代收款
                    If bmT.Rows(intI)("kind").ToString() = "2" Then  '支出在借方
                        Call Add_debit(bmT.Rows(intI))
                    Else
                        Call Add_credit(bmT.Rows(intI))
                    End If
                Else
                    If bmT.Rows(intI)("dc").ToString() = "1" Then
                        Call Add_debit(bmT.Rows(intI))
                    Else
                        Call Add_credit(bmT.Rows(intI))
                    End If
                End If
            End If
        Next
        Call clsScreen()
        ViewState("DebitPage") = 0
        Call Load_Debit()
        TabContainer1.ActiveTabIndex = 1

       
    End Sub
    Sub Add_debit(dr As DataRow)
        Dim DAryAccno(8, 5) As String  '宣告DEBIT 9*6 array
        Dim DAryRemark(8, 5) As String
        Dim DAryAmt(8, 5) As Decimal
        Dim DAryCode(8, 5) As String
        Dim DAryOther(8, 5) As String
        Dim DAryQty(8, 5) As Decimal

        DAryAccno = DirectCast(Session("DAryAccno"), String(,))
        DAryRemark = DirectCast(Session("DAryRemark"), String(,))
        DAryAmt = DirectCast(Session("DAryAmt"), Decimal(,))
        DAryCode = DirectCast(Session("DAryCode"), String(,))
        DAryOther = DirectCast(Session("DAryOther"), String(,))
        DAryQty = DirectCast(Session("DAryQty"), Decimal(,))

        If ViewState("intD") > 5 Then  '0,1,2,3,4,5 六項一頁
            ViewState("DebitPage") += 1
            ViewState("intD") = 0
        End If
        If ViewState("strAccnoD") <> "" And Mid(ViewState("strAccno"), 1, 5) <> Mid(ViewState("strAccnoD"), 1, 5) Then  '四級不同換頁
            ViewState("DebitPage") += 1
            ViewState("intD") = 0
        End If
        ViewState("strAccnoD") = ViewState("strAccno")
        If ViewState("intD") = 0 Then   '第一項要加總帳項
            DAryAccno(ViewState("DebitPage"), ViewState("intD")) = Mid(ViewState("strAccno"), 1, 5)
            DAryRemark(ViewState("DebitPage"), ViewState("intD")) = RTrim(dr("remark")) & "  " & dr("subject").ToString
            If ViewState("strAccno") = "15303" Then
                DAryRemark(ViewState("DebitPage"), ViewState("intD")) = RTrim(dr("remark")) & "保證品收入" & "  " & dr("subject").ToString
            End If
            If ViewState("strAccno") = "23102" Then
                DAryRemark(ViewState("DebitPage"), ViewState("intD")) = RTrim(dr("remark")) & "保證品退還" & "  " & dr("subject").ToString
            End If
            DAryAmt(ViewState("DebitPage"), ViewState("intD")) = dr("useamt")
            ViewState("intD") = 1
        End If
        DAryAccno(ViewState("DebitPage"), ViewState("intD")) = ViewState("strAccno")
        If ViewState("strAccno") = "15303" Then
            DAryRemark(ViewState("DebitPage"), ViewState("intD")) = RTrim(dr("remark")) & "保證品收入" & "  " & dr("subject").ToString
        Else
            If ViewState("strAccno") = "23102" Then
                DAryRemark(ViewState("DebitPage"), ViewState("intD")) = RTrim(dr("remark")) & "保證品退還" & "  " & dr("subject").ToString
            Else
                DAryRemark(ViewState("DebitPage"), ViewState("intD")) = RTrim(dr("remark")) & "  " & dr("subject").ToString
            End If
        End If
        DAryAmt(ViewState("DebitPage"), ViewState("intD")) = dr("useamt")
        If ViewState("intD") > 1 Then DAryAmt(ViewState("DebitPage"), 0) += dr("useamt") '第二項要加至總帳金額
        ViewState("intD") += 1

        Session("DAryAccno") = DAryAccno
        Session("DAryRemark") = DAryRemark
        Session("DAryAmt") = DAryAmt
        Session("DAryCode") = DAryCode
        Session("DAryOther") = DAryOther
        Session("DAryQty") = DAryQty
    End Sub

    Sub Add_credit(dr As DataRow)
        Dim CAryAccno(8, 5) As String  '宣告CREDIT 9*6 array
        Dim CAryRemark(8, 5) As String
        Dim CAryAmt(8, 5) As Decimal
        Dim CAryCode(8, 5) As String
        Dim CAryOther(8, 5) As String
        Dim CAryQty(8, 5) As Decimal


        CAryAccno = DirectCast(Session("CAryAccno"), String(,))
        CAryRemark = DirectCast(Session("CAryRemark"), String(,))
        CAryAmt = DirectCast(Session("CAryAmt"), Decimal(,))
        CAryCode = DirectCast(Session("CAryCode"), String(,))
        CAryOther = DirectCast(Session("CAryOther"), String(,))
        CAryQty = DirectCast(Session("CAryQty"), Decimal(,))

        If ViewState("intC") > 5 Then  '0,1,2,3,4,5 六項一頁
            ViewState("CreditPage") += 1
            ViewState("intC") = 0
        End If
        If ViewState("strAccnoC") <> "" And Mid(ViewState("strAccno"), 1, 5) <> Mid(ViewState("strAccnoC"), 1, 5) Then  '四級不同換頁
            ViewState("CreditPage") += 1
            ViewState("intC") = 0
        End If
        ViewState("strAccnoC") = ViewState("strAccno")
        If ViewState("intC") = 0 Then   '第一項要加總帳項
            CAryAccno(ViewState("CreditPage"), ViewState("intC")) = Mid(ViewState("strAccno"), 1, 5)
            CAryRemark(ViewState("CreditPage"), ViewState("intC")) = RTrim(dr("remark")) & "  " & dr("subject").ToString
            If ViewState("strAccno") = "15303" Then
                CAryRemark(ViewState("CreditPage"), ViewState("intC")) = RTrim(dr("remark")) & "保證品退還" & "  " & dr("subject").ToString
            End If
            If ViewState("strAccno") = "23102" Then
                CAryRemark(ViewState("CreditPage"), ViewState("intC")) = RTrim(dr("remark")) & "保證品收入" & "  " & dr("subject").ToString
            End If
            CAryAmt(ViewState("CreditPage"), ViewState("intC")) = dr("useamt")
            ViewState("intC") = 1
        End If
        CAryAccno(ViewState("CreditPage"), ViewState("intC")) = ViewState("strAccno")
        If ViewState("strAccno") = "15303" Then
            CAryRemark(ViewState("CreditPage"), ViewState("intC")) = RTrim(dr("remark")) & "保證品退還" & "  " & dr("subject").ToString
        Else
            If ViewState("strAccno") = "23102" Then
                CAryRemark(ViewState("CreditPage"), ViewState("intC")) = RTrim(dr("remark")) & "保證品收入" & "  " & dr("subject").ToString
            Else
                CAryRemark(ViewState("CreditPage"), ViewState("intC")) = RTrim(dr("remark")) & "  " & dr("subject").ToString
            End If
        End If
        CAryAmt(ViewState("CreditPage"), ViewState("intC")) = dr("useamt")
        If ViewState("intC") > 1 Then CAryAmt(ViewState("CreditPage"), 0) += dr("useamt") '第二項要加至總帳金額
        ViewState("intC") += 1

        'Session("DAryAccno") = DAryAccno
        'Session("DAryRemark") = DAryRemark
        'Session("DAryAmt") = DAryAmt
        'Session("DAryCode") = DAryCode
        'Session("DAryOther") = DAryOther
        'Session("DAryQty") = DAryQty
        Session("CAryAccno") = CAryAccno
        Session("CAryRemark") = CAryRemark
        Session("CAryAmt") = CAryAmt
        Session("CAryCode") = CAryCode
        Session("CAryOther") = CAryOther
        Session("CAryQty") = CAryQty

    End Sub
    Protected Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        If txtNo.Text = "" Then Exit Sub
        Dim strBGNO As String
        Dim i As Integer
        If ViewState("sFile") = "1" Then   '請購編號
            strBGNO = Format(CInt(Session("sYear")), "000") & Format(Val(txtNo.Text), "00000")
        Else
            strBGNO = Format(CInt(Session("sYear")), "000") & Format(Val(txtNo.Text), "0000")
        End If

        Dim sqlstr, qstr, sortstr As String
        If ViewState("sFile") = "1" Then   '資料來源:預算資料(取no_1_no=0 and 已開支(date4開支日期<>null) 借方kind='3'  貸方kind='4')
            sqlstr = "SELECT bgf030.bgno, abs(BGF030.useamt) as useamt, bgf030.remark,bgf030.autono, " & _
                     "bgf020.subject, accname.bookaccno as accno, bgf020.kind as kind, bgf020.dc as dc  " & _
                    "FROM BGF030  LEFT OUTER JOIN BGF020 ON BGF030.bgno=BGF020.bgno " & _
                    "LEFT OUTER JOIN ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                    "WHERE BGF030.date4 is not null and year(bgf030.date4)=" & CInt(Session("sYear")) + 1911 & _
                    " and BGF030.no_1_no = 0 and BGF030.bgno='" + strBGNO + "' order by bgf030.bgno"  'INCLUDE 收支
            ' " and BGF030.no_1_no = 0 and BGF020.kind>='3' order by bgf030.bgno"
        Else    '資料來源:保證品資料 (取no_1_no=0 and 收入或支出資料決定借方貸方傳票)23102存入保證品,對方科目15303保管品 
            sqlstr = "SELECT bailf020.engno as bgno,'23102' as accno, bailf020.rp as kind, " & _
                     " enf010.engname as remark, bailf020.amt as useamt, " & _
                     " bailf020.cop as subject, bailf020.autono as autono ,'' as dc " & _
                     " FROM bailf020  LEFT OUTER JOIN enf010 ON bailf020.engno=enf010.engno " & _
                     " WHERE bailf020.no_1_no = 0 and bailf020.engno='" + strBGNO + "' and year(bailf020.rpdate)=" & _
                      CInt(Session("sYear")) + 1911 & " ORDER BY bailf020.engno"
        End If
 

        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count > 0 Then

            Dim bgno As String = TempDataSet.Tables("accname").Rows(0).Item("bgno")
            Dim accno As String = TempDataSet.Tables("accname").Rows(0).Item("accno")
            Dim remark As String = TempDataSet.Tables("accname").Rows(0).Item("remark")
            Dim useamt As String = TempDataSet.Tables("accname").Rows(0).Item("useamt")
            Dim subject As String = TempDataSet.Tables("accname").Rows(0).Item("subject")
            Dim kind As String = TempDataSet.Tables("accname").Rows(0).Item("kind")
            Dim autono As String = TempDataSet.Tables("accname").Rows(0).Item("autono")
            Dim dc As String = TempDataSet.Tables("accname").Rows(0).Item("dc")

            If ViewState("dtgTarget") IsNot Nothing Then
                'get datatable from view state   
                Dim dtCurrentTable As DataTable = DirectCast(ViewState("dtgTarget"), DataTable)
                Dim drCurrentRow As DataRow = Nothing


                If accno = "" Then Exit Sub
                'If dtCurrentTable.Rows.Count < 5 Then    '只置五筆記錄
                '    If dtCurrentTable.Rows.Count = 0 Then    '記錄第一筆會計科目以便控制四級科目相同
                '        ViewState("strAccno4") = accno.Substring(0, 5)
                '    Else
                '        If accno.Substring(0, 5) <> ViewState("strAccno4") Then
                '            MessageBx("四級科目不相同")
                '            Exit Sub
                '        End If
                '    End If

                drCurrentRow = dtCurrentTable.NewRow()

                drCurrentRow("bgno") = bgno
                drCurrentRow("accno") = accno
                drCurrentRow("remark") = remark
                drCurrentRow("subject") = subject
                drCurrentRow("useamt") = useamt
                drCurrentRow("kind") = kind
                drCurrentRow("autono") = autono
                If ViewState("sFile") = "1" Then
                    drCurrentRow("dc") = dc
                    'If Mid(bmS.Current("ACCNO"), 1, 5) = "21302" Then
                    '    If bmS.Current("dc") = "1" Then nr("dc") = "2"
                    '    If bmS.Current("dc") = "2" Then nr("dc") = "1"
                    'End If
                End If

                dtCurrentTable.Rows.Add(drCurrentRow)

                'End If
                ViewState("dtgTarget") = dtCurrentTable
                'Bind Gridview with latest Row  
                dtgTarget.DataSource = dtCurrentTable
                dtgTarget.DataBind()
            End If
        End If

        txtNo.Text = ""
        txtNo.Focus()


    End Sub

    Protected Sub btnFirstScreen_Click(sender As Object, e As EventArgs) Handles btnFirstScreen.Click
        If ViewState("ac010kind") = "開立傳票" Then
            gbxCreate.Visible = True
        End If
    End Sub

    Private Sub clsScreen()    '清傳票螢幕
        Dim intI As Integer
        Dim strI As String
        Dim box As TextBox
        Dim lbl As Label

        For intI = 1 To 6
            strI = CType(intI, String)
            box = TabPanel2.FindControl("vxtaccno" & strI)
            box.Text = ""
            box = TabPanel2.FindControl("txtAmt" & strI)
            box.Text = ""

            If intI > 1 Then
                box = TabPanel2.FindControl("txtremark" & strI)
                box.Text = ""
                box = TabPanel2.FindControl("txtcode" & strI)
                box.Text = ""
                lbl = TabPanel2.FindControl("lblaccname" & strI)
                lbl.Text = ""
                box = TabPanel2.FindControl("txtQty" & strI)
                box.Text = ""
                box = TabPanel2.FindControl("vxtother" & strI)
                box.Text = ""
            End If

        Next
        'gbxQty.Visible = False
        Call disableOther()
    End Sub

    Sub Load_Debit()  '將借方array data put to screen 
        Dim strI As String

        Dim DAryAccno(8, 5) As String  '宣告DEBIT 9*6 array
        Dim DAryRemark(8, 5) As String
        Dim DAryAmt(8, 5) As Decimal
        Dim DAryCode(8, 5) As String
        Dim DAryOther(8, 5) As String
        Dim DAryQty(8, 5) As Decimal
        'Dim CAryAccno(8, 5) As String  '宣告CREDIT 9*6 array
        'Dim CAryRemark(8, 5) As String
        'Dim CAryAmt(8, 5) As Decimal
        'Dim CAryCode(8, 5) As String
        'Dim CAryOther(8, 5) As String
        'Dim CAryQty(8, 5) As Decimal

        DAryAccno = DirectCast(Session("DAryAccno"), String(,))
        DAryRemark = DirectCast(Session("DAryRemark"), String(,))
        DAryAmt = DirectCast(Session("DAryAmt"), Decimal(,))
        DAryCode = DirectCast(Session("DAryCode"), String(,))
        DAryOther = DirectCast(Session("DAryOther"), String(,))
        DAryQty = DirectCast(Session("DAryQty"), Decimal(,))
        'CAryAccno = DirectCast(ViewState("CAryAccno"), String(,))
        'CAryRemark = DirectCast(ViewState("CAryRemark"), String(,))
        'CAryAmt = DirectCast(ViewState("CAryAmt"), Decimal(,))
        'CAryCode = DirectCast(ViewState("CAryCode"), String(,))
        'CAryOther = DirectCast(ViewState("CAryOther"), String(,))
        'CAryQty = DirectCast(ViewState("CAryQty"), Decimal(,))

        Dim box As TextBox
        Dim lbl As Label


        For intI = 0 To 5
            strI = CType(intI + 1, String)
            box = TabPanel2.FindControl("vxtaccno" & strI)
            box.Text = DAryAccno(ViewState("DebitPage"), intI)
            box = TabPanel2.FindControl("txtRemark" & strI)
            box.Text = DAryRemark(ViewState("DebitPage"), intI)
            box = TabPanel2.FindControl("txtAmt" & strI)
            box.Text = DAryAmt(ViewState("DebitPage"), intI)

            If intI > 0 Then
                box = TabPanel2.FindControl("txtcode" & strI)
                box.Text = DAryCode(ViewState("DebitPage"), intI)
                box = TabPanel2.FindControl("vxtother" & strI)
                box.Text = DAryOther(ViewState("DebitPage"), intI)
                box = TabPanel2.FindControl("txtqty" & strI)
                box.Text = DAryQty(ViewState("DebitPage"), intI)
                lbl = TabPanel2.FindControl("lblaccname" & strI)
                lbl.Text = ""
            End If
            '相關科目
            If intI = 1 And Mid(vxtAccno2.Text, 1, 5) = "31102" Then
                Call enableOther()
            End If
        Next
        Dim strAccno As String
        For intI = 1 To 6   'show會計科目名稱
            strI = CType(intI, String)
            box = TabPanel2.FindControl("vxtaccno" & strI)
            strAccno = box.Text
            If strAccno <> "" Then
                sqlstr = "SELECT * from accname where accno='" & strAccno & "'"
                TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
                If tempdataset.Tables("accname").Rows.Count > 0 Then
                    lbl = TabPanel2.FindControl("lblaccname" & strI)
                    lbl.Text = TempDataSet.Tables("accname").Rows(0).Item("accname")
                End If
            End If
        Next
        lblPage.Text = ViewState("DebitPage") + 1
        If ViewState("sFile") = "3" Then  '空白傳票 cursor 在總帳科目上    99/11/11 update 
            vxtAccno1.Focus()
        End If
    End Sub

    Sub Load_Credit()   '將貸方array data put to screen 

        Dim CAryAccno(8, 5) As String  '宣告CREDIT 9*6 array
        Dim CAryRemark(8, 5) As String
        Dim CAryAmt(8, 5) As Decimal
        Dim CAryCode(8, 5) As String
        Dim CAryOther(8, 5) As String
        Dim CAryQty(8, 5) As Decimal

        CAryAccno = DirectCast(Session("CAryAccno"), String(,))
        CAryRemark = DirectCast(Session("CAryRemark"), String(,))
        CAryAmt = DirectCast(Session("CAryAmt"), Decimal(,))
        CAryCode = DirectCast(Session("CAryCode"), String(,))
        CAryOther = DirectCast(Session("CAryOther"), String(,))
        CAryQty = DirectCast(Session("CAryQty"), Decimal(,))

        Dim box As TextBox
        Dim lbl As Label

        Dim strI As String
        For intI = 0 To 5
            strI = CType(intI + 1, String)
            box = TabPanel2.FindControl("vxtaccno" & strI)
            box.Text = CAryAccno(ViewState("CreditPage"), intI)
            box = TabPanel2.FindControl("txtRemark" & strI)
            box.Text = CAryRemark(ViewState("CreditPage"), intI)
            box = TabPanel2.FindControl("txtAmt" & strI)
            box.Text = CAryAmt(ViewState("CreditPage"), intI)

            If intI > 0 Then
                box = TabPanel2.FindControl("txtcode" & strI)
                box.Text = CAryCode(ViewState("CreditPage"), intI)
                box = TabPanel2.FindControl("vxtother" & strI)
                box.Text = CAryOther(ViewState("CreditPage"), intI)
                box = TabPanel2.FindControl("txtqty" & strI)
                box.Text = CAryQty(ViewState("CreditPage"), intI)
                lbl = TabPanel2.FindControl("lblaccname" & strI)
                lbl.Text = ""
            End If
            '相關科目
            If intI = 1 And Mid(vxtAccno2.Text, 1, 5) = "31102" Then
                Call enableOther()
            End If
        Next
        Dim strAccno As String
        For intI = 1 To 6   'show會計科目名稱
            strI = CType(intI, String)
            box = TabPanel2.FindControl("vxtaccno" & strI)
            strAccno = box.Text
            If strAccno <> "" Then
                sqlstr = "SELECT * from accname where accno='" & strAccno & "'"
                TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
                If TempDataSet.Tables("accname").Rows.Count > 0 Then
                    lbl = TabPanel2.FindControl("lblaccname" & strI)
                    lbl.Text = TempDataSet.Tables("accname").Rows(0).Item("accname")
                End If
            End If
        Next
        lblPage.Text = ViewState("CreditPage") + 1
        If ViewState("sFile") = "3" Then  '空白傳票 cursor 在總帳科目上    99/11/11 update 
            vxtAccno1.Focus()
        End If
    End Sub

    Sub Save_Debit()  '將借方screen put array data
        Dim strI As String
        Dim DAryAccno(8, 5) As String  '宣告DEBIT 9*6 array
        Dim DAryRemark(8, 5) As String
        Dim DAryAmt(8, 5) As Decimal
        Dim DAryCode(8, 5) As String
        Dim DAryOther(8, 5) As String
        Dim DAryQty(8, 5) As Decimal

        DAryAccno = DirectCast(Session("DAryAccno"), String(,))
        DAryRemark = DirectCast(Session("DAryRemark"), String(,))
        DAryAmt = DirectCast(Session("DAryAmt"), Decimal(,))
        DAryCode = DirectCast(Session("DAryCode"), String(,))
        DAryOther = DirectCast(Session("DAryOther"), String(,))
        DAryQty = DirectCast(Session("DAryQty"), Decimal(,))

        Dim box As TextBox
        Dim lbl As Label

        vxtAccno1.Text = Mid(vxtAccno2.Text, 1, 5) '由第二項科目設定第一項科目
        DAryAmt(ViewState("DebitPage"), 0) = 0 '總帳金額
        For intI = 0 To 5
            strI = CType(intI + 1, String)
            box = TabPanel2.FindControl("vxtaccno" & strI)
            DAryAccno(ViewState("DebitPage"), intI) = box.Text.Trim
            box = TabPanel2.FindControl("txtRemark" & strI)
            DAryRemark(ViewState("DebitPage"), intI) = box.Text.Trim
            If Trim(Mid(DAryAccno(ViewState("DebitPage"), intI), 1, 1)) = "" Then '科目不完整時,不可有金額
                DAryAmt(ViewState("DebitPage"), intI) = 0
            Else
                box = TabPanel2.FindControl("txtAmt" & strI)
                DAryAmt(ViewState("DebitPage"), intI) = Master.Models.ValComa(box.Text)
            End If
            If intI > 0 Then
                box = TabPanel2.FindControl("txtcode" & strI)
                DAryCode(ViewState("DebitPage"), intI) = box.Text.ToUpper
                box = TabPanel2.FindControl("vxtother" & strI)
                DAryOther(ViewState("DebitPage"), intI) = box.Text.Trim
                box = TabPanel2.FindControl("txtQty" & strI)
                DAryQty(ViewState("DebitPage"), intI) = Master.Models.ValComa(box.Text)
                DAryAmt(ViewState("DebitPage"), 0) += DAryAmt(ViewState("DebitPage"), intI)   '總帳金額由明細帳加總
            Else
                DAryAmt(ViewState("DebitPage"), 0) = 0
            End If
        Next

        Session("DAryAccno") = DAryAccno
        Session("DAryRemark") = DAryRemark
        Session("DAryAmt") = DAryAmt
        Session("DAryCode") = DAryCode
        Session("DAryOther") = DAryOther
        Session("DAryQty") = DAryQty
        'ViewState("CAryAccno") = CAryAccno
        'ViewState("CAryRemark") = CAryRemark
        'ViewState("CAryAmt") = CAryAmt
        'ViewState("CAryCode") = CAryCode
        'ViewState("CAryOther") = CAryOther
        'ViewState("CAryQty") = CAryQty

        Call Check_data()
        'If checkError = True Then
        '    Exit Sub
        'End If
    End Sub

    Sub Save_Credit()  '將貸方screen put array data
        Dim strI As String
        Dim CAryAccno(8, 5) As String  '宣告CREDIT 9*6 array
        Dim CAryRemark(8, 5) As String
        Dim CAryAmt(8, 5) As Decimal
        Dim CAryCode(8, 5) As String
        Dim CAryOther(8, 5) As String
        Dim CAryQty(8, 5) As Decimal

        CAryAccno = DirectCast(Session("CAryAccno"), String(,))
        CAryRemark = DirectCast(Session("CAryRemark"), String(,))
        CAryAmt = DirectCast(Session("CAryAmt"), Decimal(,))
        CAryCode = DirectCast(Session("CAryCode"), String(,))
        CAryOther = DirectCast(Session("CAryOther"), String(,))
        CAryQty = DirectCast(Session("CAryQty"), Decimal(,))

        Dim box As TextBox
        Dim lbl As Label

        vxtAccno1.Text = Mid(vxtAccno2.Text, 1, 5)   '由第二項科目設定第一項科目
        CAryAmt(ViewState("CreditPage"), 0) = 0 '總帳金額
        For intI = 0 To 5
            strI = CType(intI + 1, String)
            box = TabPanel2.FindControl("vxtaccno" & strI)
            CAryAccno(ViewState("CreditPage"), intI) = box.Text.Trim
            box = TabPanel2.FindControl("txtRemark" & strI)
            CAryRemark(ViewState("CreditPage"), intI) = box.Text.Trim

            If Trim(Mid(CAryAccno(ViewState("CreditPage"), intI), 1, 1)) = "" Then '科目不完整時,不可有金額
                CAryAmt(ViewState("CreditPage"), intI) = 0
            Else
                box = TabPanel2.FindControl("txtAmt" & strI)
                CAryAmt(ViewState("CreditPage"), intI) = Master.Models.ValComa(box.Text)
            End If

            If intI > 0 Then
                box = TabPanel2.FindControl("txtcode" & strI)
                CAryCode(ViewState("CreditPage"), intI) = box.Text.ToUpper
                box = TabPanel2.FindControl("vxtother" & strI)
                CAryOther(ViewState("CreditPage"), intI) = box.Text.Trim
                box = TabPanel2.FindControl("txtQty" & strI)
                CAryQty(ViewState("CreditPage"), intI) = Master.Models.ValComa(box.Text)
                CAryAmt(ViewState("CreditPage"), 0) += CAryAmt(ViewState("CreditPage"), intI)   '總帳金額由明細帳加總
            Else
                CAryAmt(ViewState("CreditPage"), 0) = 0
            End If
        Next

        Session("CAryAccno") = CAryAccno
        Session("CAryRemark") = CAryRemark
        Session("CAryAmt") = CAryAmt
        Session("CAryCode") = CAryCode
        Session("CAryOther") = CAryOther
        Session("CAryQty") = CAryQty
        'ViewState("DAryAccno") = DAryAccno
        'ViewState("DAryRemark") = DAryRemark
        'ViewState("DAryAmt") = DAryAmt
        'ViewState("DAryCode") = DAryCode
        'ViewState("DAryOther") = DAryOther
        'ViewState("DAryQty") = DAryQty
        Call Check_data()
        'If checkError = True Then
        '    Exit Sub
        'End If
    End Sub

    Private Sub Check_data()  '檢查螢幕資料合理性
        Dim SumAmt As Decimal = 0
        Dim strI, strOther, strCode As String
        Dim box As TextBox
        Dim lbl As Label
        ViewState("checkError") = False   '不合理時給true 
        '檢查資料
        vxtAccno1.Text = Mid(vxtAccno2.Text, 1, 5)
        If txtRemark1.Text = "" Then txtRemark1.Text = txtRemark2.Text
        ViewState("strAccno4") = vxtAccno1.Text.Trim
        For intI = 2 To 6
            strI = CType(intI, String)
            box = TabPanel2.FindControl("txtcode" & strI)
            strCode = box.Text.ToUpper
            box = TabPanel2.FindControl("vxtAccno" & strI)
            ViewState("strAccno") = box.Text.Trim
            box = TabPanel2.FindControl("txtamt" & strI)
            If Mid(ViewState("strAccno"), 1, 1) = "" Or Master.Models.ValComa(box.Text) = 0 Then  '科目空白or金額=0
                Exit For
            End If
            If Mid(ViewState("strAccno"), 1, 5) <> ViewState("strAccno4") Then
                MessageBx("四級科目不相同")
                ViewState("checkError") = True
                Exit Sub
            End If
            '固定資產及材料要有數量


            If Mid(ViewState("strAccno"), 1, 3) = "114" Or Mid(ViewState("strAccno"), 1, 3) = "112" Or _
               (Mid(ViewState("strAccno"), 1, 2) = "13" And Mid(ViewState("strAccno"), 1, 5) <> "13701" And Mid(ViewState("strAccno"), 1, 5) <> "13201" And Mid(ViewState("strAccno"), 5, 1) <> "2") Then
                If Master.Models.ValComa(DirectCast(TabPanel2.FindControl("txtqty" & strI), TextBox).Text) = 0 Then
                    ViewState("checkError") = True
                    'DirectCast(TabPanel2.FindControl("txtqty" & strI), TextBox).Focus()
                    MessageBx("請輸入數量")
                    Exit Sub
                End If
            End If

            sqlstr = "SELECT accno FROM accname WHERE belong<>'B' AND outyear=0 and ACCNO LIKE '" & ViewState("strAccno") & "%'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
            If TempDataSet.Tables("accname").Rows.Count = 0 Then
                MessageBx("無此科目")
                ViewState("checkError") = True
                'DirectCast(TabPanel2.FindControl("txtaccno" & strI), TextBox).Focus()
                Exit Sub
            Else
                If strCode <> "E" Then    '內容別='E' 是收補助款或退回補助款  可由七級記帳
                    If TempDataSet.Tables("accname").Rows.Count > 1 Then
                        If Master.Models.Grade(ViewState("strAccno")) < Master.Models.Grade(TempDataSet.Tables("accname").Rows(1).Item(0)) Then  '找級數 grade() at vbdataid.vb
                            MessageBx("請輸入至最明細科目")
                            ViewState("checkError") = True
                            Exit Sub
                        End If
                    End If
                End If
            End If
            'other_accno
            strOther = DirectCast(TabPanel2.FindControl("vxtother" & strI), TextBox).Text
            sqlstr = "SELECT accno FROM accname WHERE belong<>'B' AND outyear=0 and ACCNO LIKE '" & strOther & "%'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
            If TempDataSet.Tables("accname").Rows.Count = 0 Then
                MessageBx("無此相關科目")
                'DirectCast(TabPanel2.FindControl("vxtother" & strI), TextBox).Focus()
                ViewState("checkError") = True
                Exit Sub
            End If
            If Mid(ViewState("strAccno"), 1, 5) = "31102" And strOther = "" Then
                MessageBx("請注意第" & Str(intI) & "項相關科目未輸入")
            End If
            TempDataSet = Nothing

            If Mid(ViewState("strAccno4"), 1, 3) = "212" Or Mid(ViewState("strAccno4"), 1, 3) = "113" Or Mid(ViewState("strAccno4"), 1, 3) = "151" Then
                If strCode < "1" Or strCode > "4" Then
                    MessageBx("請輸入內容別")
                    ViewState("checkError") = True
                    'DirectCast(TabPanel2.FindControl("txtcode" & strI), TextBox).Focus()
                    Exit Sub
                End If
            End If
            SumAmt += Master.Models.ValComa(DirectCast(TabPanel2.FindControl("txtAmt" & strI), TextBox).Text) '合計總帳金額
        Next
        txtAmt1.Text = FormatNumber(SumAmt, 2)
        If SumAmt = 0 And ViewState("strAccno4") <> "" Then
            MessageBx("金額不可=0")
            Exit Sub
            ViewState("checkError") = True
        End If

    End Sub

    Protected Sub btnPageUp_Click(sender As Object, e As EventArgs) Handles btnPageUp.Click
        If lblDC.Text = "轉帳借方" Then
            Call Save_Debit()  '先儲存本頁資料
            If ViewState("checkError") = True Then Exit Sub '資料檢查有問題
            If ViewState("DebitPage") >= 1 Then
                ViewState("DebitPage") -= 1
                Call Load_Debit()
                If ViewState("DebitPage") = 0 Then btnPageUp.Enabled = False
            End If
        Else
            Call Save_Credit()
            If ViewState("checkError") = True Then Exit Sub
            If ViewState("CreditPage") >= 1 Then
                ViewState("CreditPage") -= 1
                Call Load_Credit()
                If ViewState("CreditPage") = 0 Then btnPageUp.Enabled = False
            End If
        End If
        btnPageDown.Enabled = True
    End Sub

    Protected Sub btnPageDown_Click(sender As Object, e As EventArgs) Handles btnPageDown.Click

        Dim CAryRemark(8, 5) As String
        Dim DAryRemark(8, 5) As String



        If lblDC.Text = "轉帳借方" Then
            Call Save_Debit()  '先儲存本頁資料
            CAryRemark = DirectCast(Session("CAryRemark"), String(,))
            DAryRemark = DirectCast(Session("DAryRemark"), String(,))

            If ViewState("checkError") = True Then Exit Sub '資料檢查有問題
            If ViewState("DebitPage") < 8 Then
                ViewState("DebitPage") += 1
                Call Load_Debit()
                If Trim(txtRemark1.Text) = "" Then
                    If txtRemark1.Text = "" Then txtRemark1.Text = DAryRemark(ViewState("DebitPage") - 1, 0) '下一頁時, copy 上一頁摘要
                End If
                If ViewState("DebitPage") = 8 Then btnPageDown.Enabled = False
            End If
        Else
            Call Save_Credit()  '先儲存本頁資料
            CAryRemark = DirectCast(Session("CAryRemark"), String(,))
            DAryRemark = DirectCast(Session("DAryRemark"), String(,))

            If ViewState("checkError") = True Then Exit Sub '資料檢查有問題
            If ViewState("CreditPage") < 8 Then
                ViewState("CreditPage") += 1
                Call Load_Credit()
                If Trim(txtRemark1.Text) = "" Then
                    If txtRemark1.Text = "" Then txtRemark1.Text = DAryRemark(ViewState("CreditPage") - 1, 0) '下一頁時, copy 上一頁摘要
                End If
                If ViewState("CreditPage") = 8 Then btnPageDown.Enabled = False
            End If
        End If
        btnPageUp.Enabled = True
    End Sub

    Protected Sub btnDebit_Click(sender As Object, e As EventArgs) Handles btnDebit.Click        
        Call Save_Credit()
        If ViewState("checkError") = True Then Exit Sub
        lblDC.Text = "轉帳借方"
        btnDebit.Visible = False
        btnCredit.Visible = True
        Call Load_Debit()
        btnPageUp.Enabled = True
        btnPageDown.Enabled = True
        If ViewState("DebitPage") = 0 Then btnPageUp.Enabled = False
        If ViewState("DebitPage") = 8 Then btnPageDown.Enabled = False

        strBackColor = "LightBlue"
    End Sub

    Protected Sub btnCredit_Click(sender As Object, e As EventArgs) Handles btnCredit.Click
        Call Save_Debit()
        If ViewState("checkError") = True Then Exit Sub
        lblDC.Text = "轉帳貸方"
        btnDebit.Visible = True
        btnCredit.Visible = False
        Call Load_Credit()

        Dim DAryAmt(8, 5) As Decimal
        DAryAmt = DirectCast(Session("DAryAmt"), Decimal(,))
        Dim DAryRemark(8, 5) As String
        DAryRemark = DirectCast(Session("DAryRemark"), String(,))

        btnPageUp.Enabled = True
        btnPageDown.Enabled = True
        If ViewState("CreditPage") = 0 Then   '第一頁時
            btnPageUp.Enabled = False
            If Trim(txtRemark1.Text) = "" Then
                If txtRemark1.Text = "" Then txtRemark1.Text = DAryRemark(0, 0) '第一頁時, copy 借方第一頁摘要
                If txtRemark2.Text = "" Then txtRemark2.Text = DAryRemark(0, 1)
                If txtRemark3.Text = "" Then txtRemark3.Text = DAryRemark(0, 2)
                If txtRemark4.Text = "" Then txtRemark4.Text = DAryRemark(0, 3)
                If txtRemark5.Text = "" Then txtRemark5.Text = DAryRemark(0, 4)
                If txtRemark6.Text = "" Then txtRemark6.Text = DAryRemark(0, 5)
                If Master.Models.ValComa(txtAmt1.Text) = 0 Then txtAmt1.Text = DAryAmt(0, 0)
                If Master.Models.ValComa(txtAmt2.Text) = 0 Then txtAmt2.Text = DAryAmt(0, 1)
                If Master.Models.ValComa(txtAmt3.Text) = 0 Then txtAmt3.Text = DAryAmt(0, 2)
                If Master.Models.ValComa(txtAmt4.Text) = 0 Then txtAmt4.Text = DAryAmt(0, 3)
                If Master.Models.ValComa(txtAmt5.Text) = 0 Then txtAmt5.Text = DAryAmt(0, 4)
                If Master.Models.ValComa(txtAmt6.Text) = 0 Then txtAmt6.Text = DAryAmt(0, 5)
            End If
        End If
        If ViewState("CreditPage") = 8 Then btnPageDown.Enabled = False

        strBackColor = "LightPink"
    End Sub

    Sub PrintTransSlip(ByVal sYear As Integer, ByVal No1 As Integer, ByVal orgName As String, ByVal intCopy As Integer)
        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號

        Param.Add("No1", No1)    '使用者代號
        Param.Add("sYear", sYear)    '使用者代號
        Param.Add("intCopy", intCopy)    '使用者代號

        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        If intCopy = 2 Then
            Master.PrintFR1("AC030轉帳傳票", Session("ORG"), DNS_ACC, Param)
        Else
            Master.PrintFR("AC030轉帳傳票", Session("ORG"), DNS_ACC, Param)
        End If

    End Sub

    Protected Sub btnCopy1_Click(sender As Object, e As EventArgs) Handles btnCopy1.Click
        If vxtAccno2.Text.Trim = "" Then vxtAccno2.Text = vxtAccno1.Text.Trim        
        If Session("unittitle").indexof("臺中") = 0 Then
            vxtAccno2.Text = vxtAccno1.Text
        End If
        txtRemark2.Text = txtRemark1.Text
        If Master.Models.ValComa(txtAmt2.Text) = 0 Then txtAmt2.Text = txtAmt1.Text
    End Sub

    Protected Sub btnCopy5_Click(sender As Object, e As EventArgs) Handles btnCopy5.Click
        If vxtAccno2.Text.Trim = "" Then vxtAccno2.Text = vxtAccno1.Text.Trim
        If Session("unittitle").indexof("臺中") = 0 Then
            vxtAccno2.Text = vxtAccno1.Text
            vxtAccno3.Text = vxtAccno1.Text
            vxtAccno4.Text = vxtAccno1.Text
            vxtAccno5.Text = vxtAccno1.Text
            vxtAccno6.Text = vxtAccno1.Text
        End If

        txtRemark2.Text = txtRemark1.Text
        txtRemark3.Text = txtRemark1.Text
        txtRemark4.Text = txtRemark1.Text
        txtRemark5.Text = txtRemark1.Text
        txtRemark6.Text = txtRemark1.Text
        If Master.Models.ValComa(txtAmt2.Text) = 0 Then txtAmt2.Text = txtAmt1.Text
    End Sub

    Protected Sub btnF42_Click(sender As Object, e As EventArgs) Handles btnF42.Click

        If vxtOther2.Text.Trim = "" Then
            Exit Sub
        End If
        sqlstr = "SELECT accname FROM accname WHERE ACCNO = '" & vxtOther2.Text.Trim & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count > 0 Then
            txtRemark2.Text = TempDataSet.Tables("accname").Rows(0).Item(0) + txtRemark2.Text
        End If
        TempDataSet = Nothing
    End Sub

    Protected Sub btnF43_Click(sender As Object, e As EventArgs) Handles btnF43.Click
        If vxtOther3.Text.Trim = "" Then
            Exit Sub
        End If
        sqlstr = "SELECT accname FROM accname WHERE ACCNO = '" & vxtOther3.Text.Trim & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count > 0 Then
            txtRemark3.Text = TempDataSet.Tables("accname").Rows(0).Item(0) + txtRemark3.Text
        End If
        TempDataSet = Nothing
    End Sub

    Protected Sub btnF44_Click(sender As Object, e As EventArgs) Handles btnF44.Click
        If vxtOther4.Text.Trim = "" Then
            Exit Sub
        End If
        sqlstr = "SELECT accname FROM accname WHERE ACCNO = '" & vxtOther4.Text.Trim & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count > 0 Then
            txtRemark4.Text = TempDataSet.Tables("accname").Rows(0).Item(0) + txtRemark4.Text
        End If
        TempDataSet = Nothing
    End Sub

    Protected Sub btnF45_Click(sender As Object, e As EventArgs) Handles btnF45.Click
        If vxtOther5.Text.Trim = "" Then
            Exit Sub
        End If
        sqlstr = "SELECT accname FROM accname WHERE ACCNO = '" & vxtOther5.Text.Trim & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count > 0 Then
            txtRemark5.Text = TempDataSet.Tables("accname").Rows(0).Item(0) + txtRemark5.Text
        End If
        TempDataSet = Nothing
    End Sub

    Protected Sub btnF46_Click(sender As Object, e As EventArgs) Handles btnF46.Click
        If vxtOther6.Text.Trim = "" Then
            Exit Sub
        End If
        sqlstr = "SELECT accname FROM accname WHERE ACCNO = '" & vxtOther6.Text.Trim & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count > 0 Then
            txtRemark6.Text = TempDataSet.Tables("accname").Rows(0).Item(0) + txtRemark6.Text
        End If
        TempDataSet = Nothing
    End Sub

   

    Protected Sub btnOther_Click(sender As Object, e As EventArgs) Handles btnOther.Click
        Call enableOther()
    End Sub

    Protected Sub vxtAccno1_TextChanged(sender As Object, e As EventArgs) Handles vxtAccno1.TextChanged, vxtAccno2.TextChanged, vxtAccno3.TextChanged, vxtAccno4.TextChanged, vxtAccno5.TextChanged, vxtAccno6.TextChanged
        Dim txt1 As TextBox
        txt1 = DirectCast(sender, TextBox)
        If txt1.Text.Trim() = "" Then
            Exit Sub
        End If

        Dim strObjectName As String
        strObjectName = txt1.ID
        sqlstr = "SELECT accname FROM accname WHERE ACCNO = '" & txt1.Text.Trim() & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count <= 0 Then
            MessageBx("無此科目")
            txt1.Focus()
            Exit Sub
        Else

            DirectCast(TabPanel2.FindControl("lblaccname" & Mid(strObjectName, 9, 1)), Label).Text = TempDataSet.Tables("accname").Rows(0).Item(0)

        End If
        '由第二項科目設定相關科目
        If Mid(strObjectName, 9, 1) = "2" Then '由第二項科目設定
            If Mid(txt1.Text.Trim(), 1, 5) = "31102" Then
                Call enableOther()
            End If
        End If
    End Sub

    Protected Sub vxtOther2_TextChanged(sender As Object, e As EventArgs) Handles vxtOther2.TextChanged, vxtOther3.TextChanged, vxtOther4.TextChanged, vxtOther5.TextChanged, vxtOther6.TextChanged
        Dim txt1 As TextBox
        txt1 = DirectCast(sender, TextBox)
        If txt1.Text.Trim() = "" Then
            Exit Sub
        End If
        Dim strAccno As String
        sqlstr = "SELECT accname FROM accname WHERE ACCNO = '" & txt1.Text.Trim() & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count <= 0 Then
            MessageBx("無此科目")
            txt1.Focus()
            Exit Sub
        Else
            Dim strObjectName As String
            strObjectName = txt1.ID
            DirectCast(TabPanel2.FindControl("lblOtherName" & Mid(strObjectName, 9, 1)), Label).Text = TempDataSet.Tables("accname").Rows(0).Item(0)
        End If
    End Sub

    Protected Sub btnIntCopy_Click(sender As Object, e As EventArgs) Handles btnIntCopy.Click
        If btnIntCopy.Text = "傳票印一份" Then
            btnIntCopy.Text = "傳票印二份"
        Else
            btnIntCopy.Text = "傳票印一份"
        End If
    End Sub

End Class