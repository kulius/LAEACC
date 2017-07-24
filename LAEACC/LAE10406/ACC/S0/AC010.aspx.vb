Imports System.Data
Imports System.Data.SqlClient

Public Class AC010
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
        Master.Controller.objDataGridStyle(dtgSource, "M")
        Master.Controller.objDataGridStyle(dtgTarget, "S")

        UCBase1.Visible = False                 '初始化控制鍵

        'Focus*****
        Session("BackColor") = "LightGreen" '開立傳票區背景顏色
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        Dim s1 As String = Request.QueryString("cvalue")

        'ac010kind = TransPara.TransP("ac010kind")  '定開立傳票 or 修改傳票
        If s1 = "AC010" Then
            ViewState("ac010kind") = "開立傳票"
            gbxModify.Visible = False
            gbxCreate.Visible = True
        Else
            ViewState("ac010kind") = "修改傳票"
            gbxModify.Visible = True
            gbxCreate.Visible = False
            txtOldNo.Focus()
        End If
        If Session("UnitTitle").indexof("彰化") >= 0 Then
            btnIntCopy.Visible = False '彰化傳票印一份botton不顯示
            dtgSource.AllowPaging = False
        End If


        TabContainer1.Visible = False
        dtpDate.Text = Session("UserDate")

        Dim sqlstr As String
        Dim i As Integer
        sqlstr = "SELECT bank,bank + bankname + '(' + Replace(Convert(Varchar(18),CONVERT(money,str(balance+day_income-day_pay-unpay,14)),1),'.00','') + ')' as cbank FROM chf020"
        mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)

        Master.Controller.objDropDownListOptionEX(cboBank, DNS_ACC, sqlstr, "bank", "cbank", 0)

        If Not Request.Cookies("autoprint") Is Nothing Then
            If Request.Cookies("autoprint").Value = "Y" Then
                autoprint1.Checked = True
            Else
                autoprint2.Checked = True
            End If
        Else
            autoprint1.Checked = True
        End If

        ''將單位片語置combobox   
        'sqlstr = "SELECT psstr  FROM psname where left(unit,3)='050' order by psstr"
        'psDataSet = ws1.openmember("", "psname", sqlstr)
        'If psDataSet.Tables("psname").Rows.Count = 0 Then
        '    cboRemark.Text = "尚無片語"
        'Else
        '    cboRemark.DataSource = psDataSet.Tables("psname")
        '    cboRemark.DisplayMember = "psstr"  '顯示欄位
        '    cboRemark.ValueMember = "psstr"     '欄位值
        '    cboRemark.SelectionLength = 60
        'End If
        ''將科目置combobox
        'sqlstr = "SELECT accno, left(accno+space(17),17)+accname as accname FROM accname where belong<>'B' and outyear=0 order by accno"
        'accnoDataset = ws1.openmember("", "accname", sqlstr)
        'If accnoDataset.Tables("accname").Rows.Count = 0 Then
        '    cboAccno.Text = "尚無可請購科目"
        'Else
        '    cboAccno.DataSource = accnoDataset.Tables("accname")
        '    cboAccno.DisplayMember = "accname"  '顯示欄位
        '    cboAccno.ValueMember = "accno"     '欄位值
        'End If

    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(dtgSource, "M")
        Master.Controller.objDataGridStyle(dtgTarget, "S")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        txtRemark1.Attributes.Add("maxlength", "70")
        txtRemark1.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark2.Attributes.Add("maxlength", "70")
        txtRemark2.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark3.Attributes.Add("maxlength", "70")
        txtRemark3.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark4.Attributes.Add("maxlength", "70")
        txtRemark4.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark5.Attributes.Add("maxlength", "70")
        txtRemark5.Attributes.Add("onkeyup", "return ismaxlength(this)")
        txtRemark6.Attributes.Add("maxlength", "70")
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
            AddDefaultFirstRecord()
            btnFinish.Attributes.Add("onclick", "this.disabled = true;this.value = '請稍候..';" + Page.ClientScript.GetPostBackEventReference(btnFinish, ""))

            '將請購編號加入Enter
            txtNo.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { window.document.getElementById('" + btnNo.ClientID + "').foucs(); }")
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
        ViewState("MySort") = IIf(ViewState("MySort") = "" Or ViewState("MySort") = "ASC", "ASC", "DESC")

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
                    If dtCurrentTable.Rows.Count < 5 Then    '只置五筆記錄
                        If dtCurrentTable.Rows.Count = 0 Then    '記錄第一筆會計科目以便控制四級科目相同
                            ViewState("strAccno4") = accno.Text.Substring(0, 5)
                        Else
                            If accno.Text.Substring(0, 5) <> ViewState("strAccno4") Then
                                MessageBx("四級科目不相同")
                                Exit Sub
                            End If
                        End If

                        drCurrentRow = dtCurrentTable.NewRow()

                        drCurrentRow("bgno") = bgno.Text
                        drCurrentRow("accno") = accno.Text
                        drCurrentRow("remark") = remark.Text
                        drCurrentRow("subject") = subject.Text
                        drCurrentRow("useamt") = useamt.Text
                        drCurrentRow("kind") = kind.Text
                        drCurrentRow("autono") = autono.Text

                        dtCurrentTable.Rows.Add(drCurrentRow)

                    End If
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

    '資料分頁
    Sub dtgSource_PageIndexChanged(ByVal sender As Object, ByVal e As DataGridPageChangedEventArgs) Handles dtgSource.PageIndexChanged
        dtgSource.CurrentPageIndex = e.NewPageIndex

        LoadGridFunc("BGF030.BGNO", "", "", False) '資料查詢
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
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"
        ViewState("sYear") = Mid(dtpDate.Text, 1, 3)



        '傳票資料來源
        If rdbFile1.Checked Then ViewState("sFile") = "1"
        If rdbFile2.Checked Then ViewState("sFile") = "2"
        If rdbFile3.Checked Then ViewState("sFile") = "3"
        '傳票總類(1收2支)
        ViewState("sKind") = IIf(rdbKind1.Checked, "1", "2")

        lblYear.Text = Format(CInt(ViewState("sYear")), "000")
        If ViewState("sFile") = "1" Then lblFile.Text = "請輸入請購編號:"
        If ViewState("sFile") = "2" Then lblFile.Text = "請輸入保證金編號:"
        If ViewState("sKind") = "1" Then TabPanel2.BackColor = System.Drawing.Color.RosyBrown 'Thistle 'MistyRose
        If ViewState("sKind") = "2" Then TabPanel2.BackColor = System.Drawing.Color.DarkSeaGreen
        TabContainer1.Visible = True
        If ViewState("sFile") = "3" Then
            TabContainer1.ActiveTabIndex = 1   '由空白開始開傳票時,直接至傳票畫面
            'If Not IsDBNull(myDatasetS) Then myDatasetS.Clear() '清空傳票來源
        Else
            Call LoadGridFunc()
        End If
        txtNo.Focus()
    End Sub

    Sub LoadGridFunc(Optional ByVal strOrder As String = "", Optional ByVal strSortType As String = "", Optional ByVal strSearch As String = "", Optional ByVal strclear As Boolean = True)
        '將bgf030->no_1_no=0置入source datagrid 
        Dim sqlstr, qstr, sortstr As String
        If ViewState("sFile") = "1" Then   '資料來源:預算資料(取no_1_no=0 and 已開支(date4開支日期<>null) and 收入或支出資料決定於開立何種傳票)
            sqlstr = "SELECT BGF030.*, bgf020.subject, accname.bookaccno as accno, bgf020.kind as kind " & _
                   "FROM BGF030  LEFT OUTER JOIN BGF020 ON BGF030.bgno=BGF020.bgno " & _
                   "LEFT OUTER JOIN ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                   "WHERE BGF030.date4 is not null and year(bgf030.date4)=" & CInt(ViewState("sYear")) + 1911 & _
                   " and BGF030.no_1_no = 0 and (BGF020.kind='" & ViewState("sKind") & "' or bgf020.dc='"
            If ViewState("sFile") = "1" Then sqlstr += "2') "
            If ViewState("sFile") = "2" Then sqlstr += "1') "
            sqlstr += IIf(strOrder = "", " order by bgf030.bgno", " ORDER BY " & strOrder & " " & strSortType)
        Else    '資料來源:保證金資料 (取no_1_no=0 and 收入或支出資料決定於開立何種傳票)
            If Session("unittitle").indexof("臺中") >= 0 Then   '要將工程代號置摘要
                sqlstr = "SELECT bailf010.engno as bgno,'23101' as accno,bailf010.engno+enf010.engname as remark, "
            Else
                sqlstr = "SELECT bailf010.engno as bgno,'23101' as accno, enf010.engname as remark, "
            End If
            sqlstr &= " bailf010.amt as useamt, bailf010.cop as subject, bailf010.kind as kind, bailf010.autono as autono " & _
                     " FROM bailf010  LEFT OUTER JOIN enf010 ON bailf010.engno=enf010.engno" & _
                     " where bailf010.rp='" & ViewState("sKind") & "' and bailf010.no_1_no = 0" & _
                     " and year(bailf010.rpdate)=" & CInt(ViewState("sYear")) + 1911

            sqlstr += IIf(strOrder = "", " order by bailf010.engno", " ORDER BY " & strOrder & " " & strSortType)
        End If

        'lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
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

        If ViewState("dtgTarget") IsNot Nothing And strclear = True Then
            'get datatable from view state   
            Dim dtCurrentTable As DataTable = DirectCast(ViewState("dtgTarget"), DataTable)
            dtCurrentTable.Clear()
            ViewState("dtgTarget") = dtCurrentTable
            dtgTarget.DataSource = dtCurrentTable
            dtgTarget.DataBind()
        End If

        TabContainer1.ActiveTabIndex = 0
        lblUseNO.Text = Str(Master.Controller.QueryNO(ViewState("sYear"), ViewState("sKind")))
        lblNOkind.Text = " 上張編號:" & IIf(ViewState("sKind") = "1", "收", "支")
    End Sub

    Protected Sub btnOldNo_Click(sender As Object, e As EventArgs) Handles btnOldNo.Click
        lblMOdMsg.Text = ""
        If txtOldNo.Text = "" Then Exit Sub
        Dim strAccno As String
        Call clsScreen()
        Dim sYear As String = Mid(dtpDate.Text, 1, 3)
        ViewState("sYear") = Mid(dtpDate.Text, 1, 3)
        Dim sKind As String = IIf(rdbKind3.Checked, "1", "2")
        ViewState("sKind") = IIf(rdbKind3.Checked, "1", "2")
        If sKind = "1" Then TabPanel1.BackColor = System.Drawing.Color.RosyBrown 'Thistle 'MistyRose
        If sKind = "2" Then TabPanel1.BackColor = System.Drawing.Color.DarkSeaGreen
        Dim SumAmt As Decimal = 0
        Dim intI As Integer
        Dim strI, sqlstr As String
        Dim tempdataset As DataSet
        '總帳
        sqlstr = "SELECT * from acf010 where accyear=" & sYear & " and kind='" & sKind & _
                 "' and no_1_no=" & txtOldNo.Text & " order by item"
        tempdataset = Master.ADO.openmember(DNS_ACC, "ac010s", sqlstr)
        If tempdataset.Tables("ac010s").Rows.Count <= 0 Then
            MessageBx("無此傳票")
            Exit Sub
        End If
        If tempdataset.Tables("ac010s").Rows(0)("no_2_no") <> 0 Then
            MessageBx("此傳票已作帳,傳票號=" & tempdataset.Tables("ac010s").Rows(0)("no_2_no"))
            Exit Sub
        End If
        If Master.ADO.nz(tempdataset.Tables("ac010s").Rows(1)("chkno"), "") <> "" Then   '支票號寫在item='9'
            MessageBx("此傳票已開支票=" & tempdataset.Tables("ac010s").Rows(1)("chkno"))
            Exit Sub
        End If
        lblDate1.Text = Master.Models.strDateADToChiness(tempdataset.Tables("ac010s").Rows(0)("date_1"))         '製票日期
        cboBank.SelectedValue = tempdataset.Tables("ac010s").Rows(0)("bank")   '設定科目銀行選定值
        txtSubAmt.Text = tempdataset.Tables("ac010s").Rows(0)("amt") - tempdataset.Tables("ac010s").Rows(0)("act_amt")
        txtRemark1.Text = tempdataset.Tables("ac010s").Rows(0)("remark")
        vxtAccno1.Text = tempdataset.Tables("ac010s").Rows(0)("accno")
        txtAmt1.Text = FormatNumber(tempdataset.Tables("ac010s").Rows(0)("amt"), 2)
        strAccno = tempdataset.Tables("ac010s").Rows(0)("accno")
        '固定資產及材料要有數量
        If Mid(strAccno, 1, 3) = "114" Or Mid(strAccno, 1, 3) = "112" Or _
           (Mid(strAccno, 1, 2) = "13" And Mid(strAccno, 1, 5) <> "13701" And Mid(strAccno, 1, 5) <> "13201" And Mid(strAccno, 5, 1) <> "2") Then
            gbxQty.Visible = True
        End If
        '明細帳
        sqlstr = "SELECT * from acf020 where accyear=" & sYear & " and kind='" & sKind & "' and no_1_no=" & _
                 txtOldNo.Text & " order by item"
        tempdataset = Master.ADO.openmember(DNS_ACC, "ac010s", sqlstr)
        Dim box As TextBox
        Dim lbl As Label

        With tempdataset.Tables("ac010s")
            For intI = 0 To .Rows.Count - 1
                strI = CType(intI + 2, String)
                box = TabPanel2.FindControl("txtRemark" & strI)
                box.Text = Trim(.Rows(intI)("remark"))
                box = TabPanel2.FindControl("txtAmt" & strI)
                box.Text = FormatNumber(.Rows(intI)("amt"), 2)
                box = TabPanel2.FindControl("txtQty" & strI)
                box.Text = Format(.Rows(intI)("mat_qty"), "###,###,###.######")
                box = TabPanel2.FindControl("txtcode" & strI)
                box.Text = .Rows(intI)("cotn_code")
                box = TabPanel2.FindControl("vxtaccno" & strI)
                box.Text = .Rows(intI)("accno")

                If Not IsDBNull(tempdataset.Tables("ac010s").Rows(0)("other_accno")) Then
                    box = TabPanel2.FindControl("vxtother" & strI)
                    box.Text = .Rows(intI)("other_accno")
                End If
            Next
        End With
        strI = Master.ADO.nz(tempdataset.Tables("ac010s").Rows(0)("other_accno"), "")
        If strI <> "" Then
            Call enableOther()
        End If
        tempdataset = Nothing
        TabContainer1.Visible = True
        TabContainer1.ActiveTabIndex = 1
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

            If intI > 1 Then
                box = TabPanel2.FindControl("txtcode" & strI)
                box.Text = ""
            End If

            box = TabPanel2.FindControl("txtremark" & strI)
            box.Text = ""
            box = TabPanel2.FindControl("txtAmt" & strI)
            box.Text = ""

            If intI >= 2 Then
                lbl = TabPanel2.FindControl("lblAccname" & strI)
                lbl.Text = ""
            End If
            If intI >= 2 Then
                box = TabPanel2.FindControl("txtQty" & strI)
                box.Text = ""

            End If

            If intI >= 2 Then
                box = TabPanel2.FindControl("vxtother" & strI)
                box.Text = ""

            End If
        Next
        txtSubAmt.Text = ""
        gbxQty.Visible = False
        Call disableOther()
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

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

        Dim intI, intJ, SumAmt, AryAmt(5) As Decimal
        Dim strI, strJ, strAccno, strOther, AryRemark(5), retstr, strCode As String
        Dim strAccno4 As String
        Dim box As TextBox
        Dim AryCode() = {" ", " ", " ", " ", " "}  '將傳票螢幕資料置入array
        Dim AryAccno() = {"", "", "", "", ""}
        Dim AryOther() = {"", "", "", "", ""}
        Dim AryQty() = {"", "", "", "", ""}
        Dim tempdataset As DataSet

        '檢查資料

        SumAmt = 0
        'vxtAccno1.setData(Mid(vxtAccno2.getTrimData(), 1, 5))   '由第二項科目設定第一項科目
        vxtAccno1.Text = vxtAccno2.Text.Substring(0, 5)
        If txtRemark1.Text = "" Then txtRemark1.Text = txtRemark2.Text
        strAccno4 = vxtAccno1.Text.Trim

        If strAccno4 = "" Then Exit Sub '都沒有資料

        For intI = 2 To 6
            strI = CType(intI, String)
            strAccno = CType(TabPanel2.FindControl("vxtAccno" & strI), TextBox).Text.Trim
            'If Mid(strAccno, 1, 1) = "" Or Master.Models.ValComa(FindControl(Me, "txtamt" & strI).Text) = 0 Then  '科目空白or金額=0
            If Mid(strAccno, 1, 1) = "" Or Master.Models.ValComa(CType(TabPanel2.FindControl("txtamt" & strI), TextBox).Text.Trim) = 0 Then  '科目空白or金額=0
                Exit For
            End If
            strCode = CType(TabPanel2.FindControl("txtcode" & strI), TextBox).Text.ToUpper
            AryAccno(intI - 2) = strAccno
            If Mid(strAccno, 1, 5) <> strAccno4 Then
                MessageBx("四級科目不相同")
                Exit Sub
            End If
            '檢查數量
            If Mid(strAccno, 1, 3) = "114" Or Mid(strAccno, 1, 3) = "112" Or _
               (Mid(strAccno, 1, 2) = "13" And Mid(strAccno, 1, 5) <> "13701" And Mid(strAccno, 1, 5) <> "13201" And Mid(strAccno, 5, 1) <> "2") Then
                'If Master.Models.ValComa(FindControl(Me, "txtQty" & strI).Text) = 0 Then
                If Master.Models.ValComa(CType(TabPanel2.FindControl("txtQty" & strI), TextBox).Text) = 0 Then
                    MessageBx("請輸入數量")
                    gbxQty.Visible = True
                    Exit Sub
                End If
            End If
            sqlstr = "SELECT accno FROM accname WHERE belong<>'B' AND ACCNO LIKE '" & strAccno & "%'"
            tempdataset = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
            If tempdataset.Tables("accname").Rows.Count = 0 Then
                MessageBx("無此科目")
                CType(TabPanel2.FindControl("txtaccno" & strI), TextBox).Focus()
                Exit Sub
            Else
                If strCode <> "E" Then     '內容別='E' 是收補助款或退回補助款  可由七級記帳
                    If tempdataset.Tables("accname").Rows.Count > 1 Then
                        If Master.Models.Grade(strAccno) < Master.Models.Grade(tempdataset.Tables("accname").Rows(1).Item(0)) Then  '找級數 grade() at vbdataid.vb
                            MessageBx("請輸入至最明細科目")
                            Exit Sub
                        End If
                    End If
                End If
            End If
            'other_accno
            strOther = CType(TabPanel2.FindControl("vxtother" & strI), TextBox).Text.Trim
            AryOther(intI - 2) = strOther
            sqlstr = "SELECT accno FROM accname WHERE belong<>'B' AND ACCNO LIKE '" & strOther & "%'"
            tempdataset = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
            If tempdataset.Tables("accname").Rows.Count = 0 Then
                MessageBx("無此相關科目")
                CType(TabPanel2.FindControl("vxtother" & strI), TextBox).Focus()
                Exit Sub
            End If
            If Mid(strAccno, 1, 5) = "31102" And strOther = "" Then
                MessageBx("請注意第" & Str(intI) & "項相關科目未輸入")
            End If
            tempdataset = Nothing

            AryCode(intI - 2) = strCode
            If Mid(strAccno4, 1, 3) = "212" Or Mid(strAccno4, 1, 3) = "113" Or Mid(strAccno4, 1, 3) = "151" Then
                If AryCode(intI - 2) < "1" Or AryCode(intI - 2) > "4" Then
                    MessageBx("請輸入內容別")
                    CType(TabPanel2.FindControl("txtcode" & strI), TextBox).Focus()
                    Exit Sub
                End If
            End If
            AryRemark(intI - 2) = CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text
            AryAmt(intI - 2) = Master.Models.ValComa(CType(TabPanel2.FindControl("txtAmt" & strI), TextBox).Text)
            AryQty(intI - 2) = Master.Models.ValComa(CType(TabPanel2.FindControl("txtqty" & strI), TextBox).Text)
            SumAmt += AryAmt(intI - 2) '合計總帳金額
        Next
        If SumAmt <= 0 Then
            MessageBx("金額不可=0")
            Exit Sub
        End If
        If SumAmt - Master.Models.ValComa(txtSubAmt.Text) < 0 Then
            MessageBx("實收付金額不可小於0")
            Exit Sub
        End If

        If ViewState("ac010kind") = "修改傳票" Then
            sqlstr = "DELETE acf010 where accyear=" & ViewState("sYear") & " and kind='" & ViewState("sKind") & "' and no_1_no=" & txtOldNo.Text
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then MessageBx("ACF010刪除失敗" & sqlstr)
            sqlstr = "DELETE acf020 where accyear=" & ViewState("sYear") & " and kind='" & ViewState("sKind") & "' and no_1_no=" & txtOldNo.Text
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then MessageBx("ACF020刪除失敗" & sqlstr)
        End If

        '寫入資料表acf010傳票總帳檔
        Dim strDc As String
        Dim intNo As Integer
        Dim strdate1 As String
        If ViewState("ac010kind") = "修改傳票" Then
            intNo = Val(txtOldNo.Text)
            strdate1 = lblDate1.Text   '原製票日期
        Else
            intNo = Master.Controller.RequireNO(DNS_ACC, ViewState("sYear"), ViewState("sKind"))    '\accservice\service1.asmx
            strdate1 = dtpDate.Text  '製票日期
        End If
        For intI = 1 To 2
            Master.ADO.GenInsSql("accyear", ViewState("sYear"), "N")
            Master.ADO.GenInsSql("kind", ViewState("sKind"), "T")
            Master.ADO.GenInsSql("no_1_no", intNo, "N")
            Master.ADO.GenInsSql("no_2_no", 0, "N")
            Master.ADO.GenInsSql("SEQ", "0", "T")   '頁次='0'
            Master.ADO.GenInsSql("item", IIf(intI = 1, "1", "9"), "T")  '總帳項為1,9
            Master.ADO.GenInsSql("date_1", strdate1, "D")   '製票日期
            Master.ADO.GenInsSql("systemdate", Format(Now(), "yyyy-MM-dd"), "D")   '系統日期(藉以記錄user實際update日期)
            If (ViewState("sKind") = "1" And intI = 2) Or (ViewState("sKind") = "2" And intI = 1) Then
                strDc = "1"      '收入傳票9項為借方,支出傳票時1項為借方
            Else
                strDc = "2"      '收入傳票1項為貸方,支出傳票時9項為貸方
            End If
            Master.ADO.GenInsSql("dc", strDc, "T")
            If intI = 1 Then
                strAccno = vxtAccno1.Text.Trim
            Else
                sqlstr = "SELECT accno FROM chf020 WHERE bank = '" & cboBank.SelectedValue & "'"
                tempdataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
                If tempdataset.Tables("chf020").Rows.Count > 0 Then
                    strAccno = tempdataset.Tables("chf020").Rows(0).Item(0)    '第9項會計科目由chf020該銀行記錄之會計科目決定
                End If
                tempdataset = Nothing
            End If
            Master.ADO.GenInsSql("accno", strAccno, "T")
            Master.ADO.GenInsSql("remark", Trim(txtRemark1.Text), "U")   '摘要
            Master.ADO.GenInsSql("amt", SumAmt, "N")
            Master.ADO.GenInsSql("act_amt", SumAmt - Val(txtSubAmt.Text), "N")
            Master.ADO.GenInsSql("bank", cboBank.SelectedValue, "T")
            Master.ADO.GenInsSql("books", " ", "T")                 '過帳碼
            sqlstr = "insert into acf010 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("ACF010新增失敗" & sqlstr)
                If intI = 2 Then   '已經insert總帳項1
                    sqlstr = "delete from acf010 where accyear=" & ViewState("sYear") & " and no_1_no=" & intNo & " and kind='" & ViewState("sKind") & _
                             "' and no_2_no=0 and date_2='" & Master.Models.FullDate(strdate1) & "'"   '以確切dele方才的insert 防止刪除原有資料 
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    If retstr <> "sqlok" Then
                        MessageBx("刪除 ACF010失敗" & sqlstr)
                    End If
                End If
                Exit Sub    '不再新增acf020 
            End If
        Next

        '寫入資料表acf020傳票明細
        For intI = 0 To 4
            If AryAccno(intI) = "" Or AryAmt(intI) = 0 Then Exit For
            Master.ADO.GenInsSql("accyear", ViewState("sYear"), "N")
            Master.ADO.GenInsSql("kind", ViewState("sKind"), "T")                    '收入傳票kind="1",支出傳票kind="2"
            Master.ADO.GenInsSql("no_1_no", intNo, "N")
            Master.ADO.GenInsSql("no_2_no", 0, "N")
            Master.ADO.GenInsSql("SEQ", "0", "T")                       '收入,支出傳票無頁次
            Master.ADO.GenInsSql("item", CType(intI + 2, String), "T")
            Master.ADO.GenInsSql("dc", IIf(ViewState("sKind") = "1", "2", "1"), "T") '收入傳票時1-6項為貸方,支出傳票時1-6項為借方
            Master.ADO.GenInsSql("accno", AryAccno(intI), "T")
            Master.ADO.GenInsSql("remark", AryRemark(intI), "U")
            Master.ADO.GenInsSql("amt", AryAmt(intI), "N")
            Master.ADO.GenInsSql("cotn_code", AryCode(intI), "T")
            Master.ADO.GenInsSql("mat_qty", AryQty(intI), "N")                 '材料數量
            If AryAmt(intI) <> 0 And AryQty(intI) <> 0 Then Master.ADO.GenInsSql("mat_pric", AryAmt(intI) / AryQty(intI), "N") '材料單價
            Master.ADO.GenInsSql("other_accno", AryOther(intI), "T")           '相關科目
            sqlstr = "insert into acf020 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("ACF020新增失敗" & sqlstr)
                If intI >= 2 Then   '已經insert acf020 
                    sqlstr = "delete from acf020 where accyear=" & ViewState("sYear") & " and no_1_no=" & intNo & " and kind='" & ViewState("sKind") & _
                              "' and no_2_no=0 "   '以確切dele方才的insert 防止刪除原有資料 
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    If retstr <> "sqlok" Then
                        MessageBx("刪除 ACF020失敗" & sqlstr)
                    End If
                    sqlstr = "delete from acf010 where accyear=" & ViewState("sYear") & " and no_1_no=" & intNo & " and kind='" & ViewState("sKind") & _
                                        "' and no_2_no=0 and date_2='" & Master.Models.FullDate(strdate1) & "'"   '以確切dele方才的insert 防止刪除原有資料 
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    If retstr <> "sqlok" Then
                        MessageBx("刪除 ACF020失敗" & sqlstr)
                    End If
                End If
                Exit Sub
            End If
        Next

        '回寫編號至傳票來源
        If ViewState("ac010kind") = "開立傳票" Then
            Dim bmT As DataTable
            If ViewState("dtgTarget") IsNot Nothing Then
                bmT = DirectCast(ViewState("dtgTarget"), DataTable)
            Else
                MessageBx("ViewState 中無選取資料")
                Exit Sub
            End If
            If ViewState("sFile") = "1" Then   '回寫編號至BGF030->no_1_no,並清空傳票內容選入區
                Dim intAutono As Integer
                For intI = 0 To bmT.Rows.Count - 1
                    intAutono = bmT.Rows(intI)("autono").ToString()
                    sqlstr = "UPDATE bgf030 SET no_1_no = " & intNo & " WHERE autono = " & intAutono
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                Next

            Else
                If ViewState("sFile") = "2" Then
                    '回寫編號至bailf010->no_1_no,並清空傳票內容選入區
                    Dim intAutono As Integer
                    For intI = 0 To bmT.Rows.Count - 1
                        intAutono = bmT.Rows(intI)("autono").ToString()
                        sqlstr = "UPDATE bailf010 SET no_1_no = " & intNo & " WHERE autono = " & intAutono
                        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    Next
                End If
            End If
            bmT.Clear()
            lblUseNO.Text = Str(intNo)     '顯示實際使用編號
            lblNOkind.Text = " 上張編號:" & IIf(ViewState("sKind") = "1", "收", "支")
            Call LoadGridFunc()
            'MessageBx("新增傳票" + Str(intNo) + "完成")
        Else
            TabContainer1.Visible = False
            lblMOdMsg.Text = txtOldNo.Text & "號傳票已修正完成"
            txtOldNo.Text = ""
        End If

        '列印傳票
        If btnIntCopy.Text = "傳票印一份" Then
            PrintIncomeSlip(ViewState("sYear"), ViewState("sKind"), intNo, Session("UnitTitle"), 0) '傳票印一份
        Else
            PrintIncomeSlip(ViewState("sYear"), ViewState("sKind"), intNo, Session("UnitTitle"), 1) '傳票印二份 (正本)
            PrintIncomeSlip(ViewState("sYear"), ViewState("sKind"), intNo, Session("UnitTitle"), 2) '傳票印二份 (副本)
        End If

        Call clsScreen()               '清傳票螢幕
        If ViewState("ac010kind") = "開立傳票" Then
            If ViewState("sFile") = "3" Then   '由空白搜尋時  轉至空白傳票頁
                TabContainer1.ActiveTabIndex = 1  '回空白傳票頁
            Else
                TabContainer1.ActiveTabIndex = 0  '回傳票來源頁datagrid PAGE 1
                txtNo.Focus()
            End If
        End If
    End Sub

    Protected Sub btnReturn_Click(sender As Object, e As EventArgs) Handles btnReturn.Click
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

        If ViewState("ac010kind") = "開立傳票" Then
            TabContainer1.ActiveTabIndex = 0
        Else
            TabContainer1.Visible = False
        End If
    End Sub

    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        Call clsScreen()
        Dim SumAmt As Decimal = 0
        Dim intI As Integer
        Dim strI, strRemark, sqlstr, sbank, strAccno As String
        Dim tempdataset As DataSet
        Dim bmT As DataTable
        If ViewState("dtgTarget") IsNot Nothing Then
            'get datatable from view state   
        bmT = DirectCast(ViewState("dtgTarget"), DataTable)
        Else
            MessageBx("ViewState 中無選取資料")
            Exit Sub
        End If


        lblNo_1_no.Text = Val(lblUseNO.Text) + 1
        lblNowNO.Text = "製票編號:" & IIf(ViewState("sKind") = "1", "收", "支")
        TabContainer1.ActiveTabIndex = 1
        If bmT.Rows.Count = 0 Then Exit Sub
        vxtAccno1.Text = bmT.Rows(0)("accno").ToString().Substring(0, 5)
        '固定資產及材料要有數量
        strAccno = bmT.Rows(0)("accno").ToString().Substring(0, 5)
        If Mid(strAccno, 1, 3) = "114" Or Mid(strAccno, 1, 3) = "112" Or _
           (Mid(strAccno, 1, 2) = "13" And strAccno <> "13701" And strAccno <> "13201" And Mid(strAccno, 5, 1) <> "2") Then
            gbxQty.Visible = True
        End If
        '受贈公積要有相關科目
        If Mid(strAccno, 1, 5) = "31102" Then
            Call enableOther()
        End If
        '總帳
        txtRemark1.Text = bmT.Rows(0)("remark").ToString() & "  " & IIf(bmT.Rows(0)("subject").ToString() = "", "  ", bmT.Rows(0)("subject").ToString())
        
        For intI = 0 To bmT.Rows.Count - 1
            strI = CType(intI + 2, String)
            strRemark = bmT.Rows(intI)("remark").ToString()

            CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = strRemark
            If IsDBNull(bmT.Rows(intI)("subject")) = False Then
                If InStr(bmT.Rows(intI)("remark"), bmT.Rows(intI)("subject")) = 0 Then
                    If ViewState("sFile") = "2" Then   '保證金資料
                        If ViewState("sKind") = "1" Then '收入傳票
                            Select Case bmT.Rows(intI)("kind").ToString
                                Case "1"
                                    CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "履約金收入" & "  " & bmT.Rows(intI)("subject").ToString
                                Case "2"
                                    CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "押標金收入" & "  " & bmT.Rows(intI)("subject").ToString
                                Case "3"
                                    CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "保固金收入" & "  " & bmT.Rows(intI)("subject").ToString
                                Case "4"
                                    CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "差額保證金收入" & "  " & bmT.Rows(intI)("subject").ToString
                            End Select
                        Else   '支出傳票
                            Select Case bmT.Rows(intI)("kind").ToString
                                Case "1"
                                    CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "履約金退還" & "  " & bmT.Rows(intI)("subject").ToString
                                Case "2"
                                    CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "押標金退還" & "  " & bmT.Rows(intI)("subject").ToString
                                Case "3"
                                    CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "保固金退還" & "  " & bmT.Rows(intI)("subject").ToString
                                Case "4"
                                    CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "差額保證金退還" & "  " & bmT.Rows(intI)("subject").ToString
                            End Select
                        End If
                    Else
                        CType(TabPanel2.FindControl("txtRemark" & strI), TextBox).Text = bmT.Rows(intI)("remark").ToString & "  " & bmT.Rows(intI)("subject").ToString
                    End If
                End If
            End If
            CType(TabPanel2.FindControl("txtAmt" & strI), TextBox).Text = FormatNumber(Master.ADO.nz(bmT.Rows(intI)("useamt").ToString.Replace("-", ""), 0), 2)
            CType(TabPanel2.FindControl("vxtaccno" & strI), TextBox).Text = bmT.Rows(intI)("accno").ToString  'function findcontrol at vbdataio.vb
            SumAmt += Master.ADO.nz(bmT.Rows(intI)("useamt").ToString.Replace("-", ""), 0)  '合計總帳金額

        Next
        If ViewState("sFile") = "2" Then '保證金資料 
            txtRemark1.Text = txtRemark2.Text   '抓明細第一行摘要置總帳行摘要
        End If
        txtAmt1.Text = FormatNumber(SumAmt, 2)

        '設定銀行

        sbank = ""
        sqlstr = "SELECT bank FROM accname WHERE ACCNO = '" & bmT.Rows(0)("accno").ToString() & "'"
        tempdataset = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If tempdataset.Tables("accname").Rows.Count > 0 Then
            sbank = Master.ADO.nz(tempdataset.Tables("accname").Rows(0).Item(0), "")
        End If
        '科目未設銀行時,依app.config定義
        If Trim(sbank) = "" Then
            If ViewState("sKind") = "1" Then
                'sbank = CHIA.Utility.AppInfo.GetAppSettings("收入傳票銀行代碼", "")
            Else
                sbank = "01"
            End If
        End If
        Try
            cboBank.SelectedValue = sbank    '設定科目銀行選定值
        Catch ex As Exception

        End Try

        tempdataset = Nothing
        lblDate1.Text = dtpDate.Text
    End Sub

    Protected Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        ViewState("sYear") = Mid(dtpDate.Text, 1, 3)
        '傳票資料來源
        If rdbFile1.Checked Then ViewState("sFile") = "1"
        If rdbFile2.Checked Then ViewState("sFile") = "2"
        If rdbFile3.Checked Then ViewState("sFile") = "3"
        '傳票總類(1收2支)
        ViewState("sKind") = IIf(rdbKind1.Checked, "1", "2")

        lblYear.Text = Format(CInt(ViewState("sYear")), "000")
        If ViewState("sFile") = "1" Then lblFile.Text = "請輸入請購編號:"
        If ViewState("sFile") = "2" Then lblFile.Text = "請輸入保證金編號:"
        'If ViewState("sKind") = "1" Then TabPanel2.BackColor = System.Drawing.Color.RosyBrown 'Thistle 'MistyRose
        'If ViewState("sKind") = "2" Then TabPanel2.BackColor = System.Drawing.Color.DarkSeaGreen
        TabContainer1.Visible = True
        'If ViewState("sFile") = "3" Then
        '    TabContainer1.ActiveTabIndex = 1   '由空白開始開傳票時,直接至傳票畫面
        '    'If Not IsDBNull(myDatasetS) Then myDatasetS.Clear() '清空傳票來源
        'Else
        '    Call LoadGridFunc()
        'End If


        If txtNo.Text = "" Then Exit Sub
        Dim strBGNO As String
        Dim i As Integer
        strBGNO = Format(CInt(ViewState("sYear")), "000") & Format(Val(txtNo.Text), "00000")

        Dim sqlstr, qstr, sortstr As String
        If ViewState("sFile") = "1" Then   '資料來源:預算資料(取no_1_no=0 and 已開支(date4開支日期<>null) and 收入或支出資料決定於開立何種傳票)
            sqlstr = "SELECT BGF030.*, bgf020.subject, accname.bookaccno as accno, bgf020.kind as kind " & _
                   "FROM BGF030  LEFT OUTER JOIN BGF020 ON BGF030.bgno=BGF020.bgno " & _
                   "LEFT OUTER JOIN ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                   "WHERE BGF030.date4 is not null and year(bgf030.date4)=" & CInt(ViewState("sYear")) + 1911 & _
                   " and BGF030.no_1_no = 0 and (BGF020.kind='" & ViewState("sKind") & "' OR bgf020.dc='"
            If ViewState("sFile") = "1" Then sqlstr += "2')" 'INCLUDE 預算轉帳
            If ViewState("sFile") = "2" Then sqlstr += "1')"
            sqlstr = sqlstr + " and BGF030.bgno='" + strBGNO + "'"
        Else    '資料來源:保證金資料 (取no_1_no=0 and 收入或支出資料決定於開立何種傳票)
            If Session("unittitle").indexof("臺中") >= 0 Then   '要將工程代號置摘要
                sqlstr = "SELECT bailf010.engno as bgno,'23101' as accno,bailf010.engno+enf010.engname as remark, "
            Else
                sqlstr = "SELECT bailf010.engno as bgno,'23101' as accno, enf010.engname as remark, "
            End If
            sqlstr &= " bailf010.amt as useamt, bailf010.cop as subject, bailf010.kind as kind, bailf010.autono as autono " & _
                     " FROM bailf010  LEFT OUTER JOIN enf010 ON bailf010.engno=enf010.engno" & _
                     " where bailf010.rp='" & ViewState("sKind") & "' and bailf010.no_1_no = 0" & _
                     " and year(bailf010.rpdate)=" & CInt(ViewState("sYear")) + 1911
            sqlstr = sqlstr + " and bailf010.engno='" + strBGNO + "'"
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

            If ViewState("dtgTarget") IsNot Nothing Then
                'get datatable from view state   
                Dim dtCurrentTable As DataTable = DirectCast(ViewState("dtgTarget"), DataTable)
                Dim drCurrentRow As DataRow = Nothing


                If accno = "" Then Exit Sub
                If dtCurrentTable.Rows.Count < 5 Then    '只置五筆記錄
                    If dtCurrentTable.Rows.Count = 0 Then    '記錄第一筆會計科目以便控制四級科目相同
                        ViewState("strAccno4") = accno.Substring(0, 5)
                    Else
                        If accno.Substring(0, 5) <> ViewState("strAccno4") Then
                            MessageBx("四級科目不相同")
                            Exit Sub
                        End If
                    End If

                    drCurrentRow = dtCurrentTable.NewRow()

                    drCurrentRow("bgno") = bgno
                    drCurrentRow("accno") = accno
                    drCurrentRow("remark") = remark
                    drCurrentRow("subject") = subject
                    drCurrentRow("useamt") = useamt
                    drCurrentRow("kind") = kind
                    drCurrentRow("autono") = autono

                    dtCurrentTable.Rows.Add(drCurrentRow)

                End If
                ViewState("dtgTarget") = dtCurrentTable
                'Bind Gridview with latest Row  
                dtgTarget.DataSource = dtCurrentTable
                dtgTarget.DataBind()
            End If
        End If

        txtNo.Text = ""
        txtNo.Focus()


    End Sub

    Protected Sub btnIntCopy_Click(sender As Object, e As EventArgs) Handles btnIntCopy.Click
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

        If btnIntCopy.Text = "傳票印一份" Then
            btnIntCopy.Text = "傳票印二份"
        Else
            btnIntCopy.Text = "傳票印一份"
        End If
    End Sub

    Protected Sub btnCopy1_Click(sender As Object, e As EventArgs) Handles btnCopy1.Click
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

        If vxtAccno2.Text.Trim = "" Then vxtAccno2.Text = vxtAccno1.Text.Trim
        If Session("unittitle").indexof("臺中") = 0 Then
            vxtAccno2.Text = vxtAccno1.Text
        End If
        txtRemark2.Text = txtRemark1.Text
        If Master.Models.ValComa(txtAmt2.Text) = 0 Then txtAmt2.Text = txtAmt1.Text
    End Sub

    Protected Sub btnCopy5_Click(sender As Object, e As EventArgs) Handles btnCopy5.Click
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

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

    Protected Sub BtnPrint2_Click(sender As Object, e As EventArgs) Handles BtnPrint2.Click
        Dim intCopy As Integer
        If btnIntCopy.Text = "傳票印一份" Then
            intCopy = 0
        Else
            intCopy = 1
        End If

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號

        Param.Add("No1", PNo1.Text)    '使用者代號
        Param.Add("No2", PNo2.Text)    '使用者代號
        Param.Add("sYear", ViewState("sYear"))    '使用者代號
        Param.Add("intCopy", intCopy)    '使用者代號

        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        If ViewState("sKind") = "2" Then
            Master.PrintFR("AC010支出傳票", Session("ORG"), DNS_ACC, Param)
        Else
            Master.PrintFR("AC010收入傳票", Session("ORG"), DNS_ACC, Param)
        End If
    End Sub


    Protected Sub btnOther_Click(sender As Object, e As EventArgs) Handles btnOther.Click
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

        Call enableOther()
    End Sub

    Protected Sub vxtAccno1_TextChanged(sender As Object, e As EventArgs) Handles vxtAccno1.TextChanged, vxtAccno3.TextChanged, vxtAccno4.TextChanged, vxtAccno5.TextChanged, vxtAccno6.TextChanged
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

        Dim txt1 As TextBox
        txt1 = DirectCast(sender, TextBox)
        If txt1.Text.Trim() = "" Then
            Exit Sub
        End If

        Dim strObjectName, strAccno As String
        sqlstr = "SELECT accname,bank FROM accname WHERE ACCNO = '" & txt1.Text.Trim() & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count <= 0 Then
            'MessageBx("無此科目")
            'txt1.Focus()
            Exit Sub
        Else
            strObjectName = txt1.ID
            DirectCast(TabPanel2.FindControl("lblaccname" & Mid(strObjectName, 9, 1)), Label).Text = TempDataSet.Tables("accname").Rows(0).Item(0)
            If Mid(strObjectName, 9, 1) = "2" Then '由第二項科目設定銀行設定值
                If Trim(TempDataSet.Tables("accname").Rows(0)("bank")) <> "" Then
                    cboBank.SelectedValue = TempDataSet.Tables("accname").Rows(0)("bank")   '設定銀行設定值
                    'Else
                    '    If sKind = "1" Then cboBank.SelectedValue = "04" '彰化水利會收入皆以04 
                    '    If sKind = "2" Then cboBank.SelectedValue = "01" '彰化水利會支出皆以01
                End If
                '由第二項科目設定固定資產及材料要有數量
                strAccno = Mid(vxtAccno2.Text, 1, 5)
                If Mid(strAccno, 1, 3) = "114" Or Mid(strAccno, 1, 3) = "112" Or _
                   (Mid(strAccno, 1, 2) = "13" And Mid(strAccno, 1, 5) <> "13701" And Mid(strAccno, 1, 5) <> "13201" And Mid(strAccno, 5, 1) <> "2") Then
                    gbxQty.Visible = True
                End If
                '由第二項科目設定相關科目
                If Mid(strAccno, 1, 5) = "31102" Then
                    Call enableOther()
                End If
            End If
        End If


    End Sub

    Protected Sub vxtOther2_TextChanged(sender As Object, e As EventArgs) Handles vxtOther2.TextChanged, vxtOther3.TextChanged, vxtOther4.TextChanged, vxtOther5.TextChanged, vxtOther6.TextChanged, vxtAccno2.TextChanged
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

        Dim txt1 As TextBox
        txt1 = DirectCast(sender, TextBox)
        If txt1.Text.Trim() = "" Then
            Exit Sub
        End If

        Dim strObjectName, strAccno As String
        sqlstr = "SELECT accname FROM accname WHERE ACCNO = '" & txt1.Text.Trim() & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If TempDataSet.Tables("accname").Rows.Count <= 0 Then
            MessageBx("無此科目")
            txt1.Focus()
            Exit Sub
        Else
            strObjectName = txt1.ID
            DirectCast(TabPanel2.FindControl("lblOtherName" & Mid(strObjectName, 9, 1)), Label).Text = TempDataSet.Tables("accname").Rows(0).Item(0)
        End If

    End Sub



    Protected Sub autoprintset_Click(sender As Object, e As EventArgs) Handles autoprintset.Click
        If autoprint1.Checked Then
            Response.Cookies("autoprint").Value = "Y" : Response.Cookies("autoprint").Expires = Now.AddDays(3)
        Else
            Response.Cookies("autoprint").Value = "N" : Response.Cookies("autoprint").Expires = Now.AddDays(3)
        End If
    End Sub
    '列印傳票
    Sub PrintIncomeSlip(ByVal sYear As Integer, ByVal sKind As String, ByVal No1 As Integer, ByVal orgName As String, ByVal intCopy As Integer)

        If autoprint2.Checked Then
            Master.ADO.GenInsSql("NO_1_NO", No1, "N")
            Master.ADO.GenInsSql("KIND", sKind, "T")
            sqlstr = "insert into ACF010PRINT " & Master.ADO.GenInsFunc
            Master.ADO.dbExecute(DNS_ACC, sqlstr)
            Exit Sub
        End If


        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號

        Param.Add("No1", No1)    '使用者代號
        Param.Add("No2", No1)    '使用者代號
        Param.Add("sYear", sYear)    '使用者代號
        Param.Add("intCopy", intCopy)    '使用者代號

        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        If intCopy = 2 Then
            If sKind = "2" Then
                Master.PrintFR1("AC010支出傳票", Session("ORG"), DNS_ACC, Param)
            Else
                Master.PrintFR1("AC010收入傳票", Session("ORG"), DNS_ACC, Param)
            End If
        Else
            If sKind = "2" Then
                Master.PrintFR("AC010支出傳票", Session("ORG"), DNS_ACC, Param)
            Else
                Master.PrintFR("AC010收入傳票", Session("ORG"), DNS_ACC, Param)
            End If
        End If



    End Sub

    Protected Sub BtnPrint3_Click(sender As Object, e As EventArgs) Handles BtnPrint3.Click

        '列印傳票
        If btnIntCopy1.Text = "傳票印一份" Then
            PrintIncomeSlip1(ViewState("sYear"), 0) '傳票印一份
        Else
            PrintIncomeSlip1(ViewState("sYear"), 1) '傳票印二份 (正本)
            PrintIncomeSlip1(ViewState("sYear"), 2) '傳票印二份 (副本)
        End If

        '清空ACF010PRINT 
        'sqlstr = "delete from ACF010PRINT WHERE (KIND = 1)"
        'Master.ADO.runsql(DNS_ACC, sqlstr)

        'Dim myds As DataSet
        'Dim PNo1 As Integer = 0
        'Dim PNo2 As Integer = 0

        'sqlstr = "SELECT  ISNULL(MIN(NO_1_NO), 0) AS pno1, ISNULL(MAX(NO_1_NO), 0) AS pno2 FROM  ACF010PRINT  WHERE (KIND = 1) "
        'myds = Master.ADO.openmember(DNS_ACC, "ACF010PRINT", sqlstr)
        'PNo1 = myds.Tables("ACF010PRINT").Rows(0).Item("pno1")
        'PNo2 = myds.Tables("ACF010PRINT").Rows(0).Item("pno2")

        'If PNo1 <> 0 Then
        '    Dim Param1 As Dictionary(Of String, String) = New Dictionary(Of String, String)
        '    Param1.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        '    Param1.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        '    Param1.Add("UserId", Session("UserId"))    '使用者代號

        '    Param1.Add("No1", PNo1)    '使用者代號
        '    Param1.Add("No2", PNo2)    '使用者代號
        '    Param1.Add("sYear", ViewState("sYear"))    '使用者代號
        '    Param1.Add("intCopy", intCopy)    '使用者代號

        '    Param1.Add("sSeason", Session("sSeason"))    '第幾季
        '    Param1.Add("UserDate", Session("UserDate"))    '登入日期

        '    Master.PrintFR("AC010收入傳票", Session("ORG"), DNS_ACC, Param1)

        '    '清空ACF010PRINT 
        '    sqlstr = "delete from ACF010PRINT WHERE (KIND = 1)"
        '    Master.ADO.runsql(DNS_ACC, sqlstr)
        'End If

    End Sub
    Sub PrintIncomeSlip1(ByVal sYear As Integer, ByVal intCopy As Integer)
        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號


        Param.Add("sYear", sYear)    '使用者代號
        Param.Add("intCopy", intCopy)    '使用者代號

        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        If intCopy = 2 Then
            Master.PrintFR1("AC010收入支出連續列印", Session("ORG"), DNS_ACC, Param)
        Else
            Master.PrintFR("AC010收入支出連續列印", Session("ORG"), DNS_ACC, Param)
        End If


    End Sub

    Protected Sub btnIntCopy1_Click(sender As Object, e As EventArgs) Handles btnIntCopy1.Click
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

        If btnIntCopy1.Text = "傳票印一份" Then
            btnIntCopy1.Text = "傳票印二份"
        Else
            btnIntCopy1.Text = "傳票印一份"
        End If
    End Sub

    Protected Sub btnF42_Click(sender As Object, e As EventArgs) Handles btnF42.Click
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

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
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

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
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

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
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

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
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"

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

    Protected Sub rdbKind1_CheckedChanged(sender As Object, e As EventArgs) Handles rdbKind1.CheckedChanged, rdbKind2.CheckedChanged
        '初始化
        If rdbKind1.Checked = True Then Session("BackColor") = "LightPink"
        If rdbKind2.Checked = True Then Session("BackColor") = "LightGreen"
        '傳票總類(1收2支)
        ViewState("sKind") = IIf(rdbKind1.Checked, "1", "2")
        If ViewState("sKind") = "1" Then TabPanel2.BackColor = System.Drawing.Color.RosyBrown 'Thistle 'MistyRose
        If ViewState("sKind") = "2" Then TabPanel2.BackColor = System.Drawing.Color.DarkSeaGreen
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        '清空ACF010PRINT 
        sqlstr = "delete from ACF010PRINT WHERE (KIND = 1)"
        Master.ADO.runsql(DNS_ACC, sqlstr)
    End Sub
End Class