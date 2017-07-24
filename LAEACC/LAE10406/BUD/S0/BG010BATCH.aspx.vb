Imports System.Data
Imports System.Data.SqlClient

Public Class BG010BATCH
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
    Dim TempDataSet As DataSet
    Dim myDataSet As DataSet

    Dim sqlstr As String
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        UCBase1.Visible = False
        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤
        txtAreaA.Visible = False
        cboAreaA.Visible = False

        If Session("UnitTitle").indexof("嘉南") >= 0 Then
            ViewState("isArea") = True
            txtAreaA.Visible = True
            cboAreaA.Visible = True
            
            '將管理處置combobox 
            sqlstr = "SELECT area, area+areaname as areaname  FROM area "
        End If
        If Session("UnitTitle").indexof("彰化") >= 0 Then
            txtUnitS.Text = "1"
            txtUnitE.Text = "399"
        End If


        dtpDate1a.Text = Session("UserDate")

        '將所有可請購科目置combobox 
        Call PutAccnoToCbo()

        '將單位片語置combobox   
        sqlstr = "SELECT psstr  FROM psname where unit='" & Session("UserUnit") & "' and seq<>9999 order by psstr"
        
        '將單位置combobox 
        sqlstr = "SELECT unit, unit+shortname as unitname  FROM unittable "
        Master.Controller.objDropDownListOptionEX(cboUnitS, DNS_ACC, sqlstr, "unit", "unitname", 0)

        '將單位置combobox 
        sqlstr = "SELECT unit, unit+shortname as unitname  FROM unittable "
        Master.Controller.objDropDownListOptionEX(cboUnitE, DNS_ACC, sqlstr, "unit", "unitname", 0)

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

            ''資料查詢*****
            'FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            AddDefaultFirstRecord()
        End If
    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
    Sub AddDefaultFirstRecord()
        'creating dataTable   
        Dim dt As New DataTable()
        Dim dr As DataRow
        dt.TableName = "AuthorBooks"
        dt.Columns.Add(New DataColumn("BGNO", GetType(String)))
        dt.Columns.Add(New DataColumn("DATE1", GetType(String)))
        dt.Columns.Add(New DataColumn("ACCYEAR", GetType(String)))
        dt.Columns.Add(New DataColumn("ACCNO", GetType(String)))
        dt.Columns.Add(New DataColumn("ACCNAME", GetType(String)))
        dt.Columns.Add(New DataColumn("REMARK", GetType(String)))
        dt.Columns.Add(New DataColumn("amt1", GetType(String)))
        dt.Columns.Add(New DataColumn("SUBJECT", GetType(String)))
        'dr = dt.NewRow()
        'dt.Rows.Add(dr)
        'saving databale into viewstate   
        ViewState("AuthorBooks") = dt
        'bind Gridview  
        DataGridView.DataSource = dt
        DataGridView.DataBind()
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
    ''點選資訊
    Sub DataGridView_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGridView.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號


        Select Case e.CommandName
            Case "btnDelete"
                If ViewState("AuthorBooks") IsNot Nothing Then
                    'get datatable from view state   
                    Dim dtCurrentTable As DataTable = DirectCast(ViewState("AuthorBooks"), DataTable)
                    Dim drCurrentRow As DataRow = Nothing

                    For RowI = 0 To dtCurrentTable.Rows.Count - 1
                        If dtCurrentTable.Rows(RowI)(0).ToString() = txtID.Text Then
                            dtCurrentTable.Rows(RowI).Delete()
                            Exit For
                        End If
                    Next
                    dtCurrentTable.AcceptChanges()

                    ViewState("AuthorBooks") = dtCurrentTable
                    DataGridView.DataSource = dtCurrentTable
                    DataGridView.DataBind()
                End If

        End Select
    End Sub
    ''資料分頁
    'Sub DataGridView_PageIndexChanged(ByVal sender As Object, ByVal e As DataGridPageChangedEventArgs) Handles DataGridView.PageIndexChanged
    '    DataGridView.CurrentPageIndex = e.NewPageIndex

    '    FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '資料查詢
    'End Sub
    ''自選排序
    'Sub DataGridView_SortCommand(ByVal sender As Object, ByVal e As DataGridSortCommandEventArgs) Handles DataGridView.SortCommand
    '    ViewState("MyOrder") = e.SortExpression
    '    ViewState("MySort") = IIf(ViewState("MySort") = "" Or ViewState("MySort") = "ASC", "DESC", "ASC")

    '    FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '資料查詢
    'End Sub


#End Region


#Region "按鍵選項"
    '查詢
#End Region



#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        '開啟查詢
        'Dim sqlstr, qstr, strD, strC As String
        'sqlstr = "SELECT a.BGNO, a.REL, b.ACCYEAR, b.ACCNO, b.DATE1, a.DATE3, a.USEAMT, a.remark," & _
        '         " b.date2, a.autono, b.kind, b.AMT1, b.amt2, b.amt3, b.useableamt, b.subject," & _
        '         " CASE WHEN len(b.accno)=17 THEN c.accname+'('+d.accname+')'" & _
        '             " WHEN len(b.accno)<>17 THEN c.accname END AS accname" & _
        '         " FROM BGF030 a INNER JOIN BGF020 b ON a.BGNO = b.BGNO INNER JOIN ACCNAME c ON b.ACCNO = c.ACCNO " & _
        '         " LEFT OUTER JOIN accname d ON left(b.accno,16)=d.accno and len(b.accno)=17" & _
        '         " WHERE c.account_NO = '" & Session("USERID") & "' AND a.date4 is null" & _
        '         " ORDER BY b.accno, a.BGNO"

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
            'FindData(id.Text)
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



        blnCheck = True
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
        'ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作刪除資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
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

        Master.Controller.Main_Control(objMainTextBox, objMainDropDownList, objMainRadioButtonList, Status)
        Master.Controller.TextBox_Control(objTextBox, Status) : Master.Controller.TextBox_Clear(objTextBox, Status)
        Master.Controller.DropDownList_Control(objDropDownList, Status) : Master.Controller.DropDownList_Clear(objDropDownList, Status)
        Master.Controller.RadioButtonList_Control(objRadioButtonList, Status) : Master.Controller.RadioButtonList_Clear(objRadioButtonList, Status)


        ''自訂項目*****        
        If Status = "1" Then
            'PutAccnoToCbo()
        End If


        '其他控制項
        Select Case Status
            Case 0 '一般模式


            Case 1 '新增模式
                TabContainer1.ActiveTabIndex = 1

            Case 2 '修改模式


            Case 3 '複製模式
                TabContainer1.ActiveTabIndex = 1
        End Select
    End Sub
#End Region

#Region "@資料查詢@"
    ''查詢資料
    'Sub FindData(ByVal strKey1 As String)
    '    '防呆*****
    '    If strKey1 = "" Then Exit Sub

    '    '設定關鍵值*****        
    '    txtKey1.Text = strKey1 : ViewState("FileKey") = strKey1

    '    '資料查詢*****
    '    BGF020_Load(strKey1) '載入資料
    'End Sub
    ''請購推算檔
    'Sub BGF020_Load(ByVal strKey1 As String)
    '    '開啟查詢
    '    objCon99 = New SqlConnection(DNS_ACC)
    '    objCon99.Open()

    '    sqlstr = "SELECT a.BGNO, a.REL, b.ACCYEAR, b.ACCNO, b.DATE1, a.DATE3, a.USEAMT, a.remark," & _
    '             " b.date2, a.autono, b.kind, b.AMT1, b.amt2, b.amt3, b.useableamt, b.subject," & _
    '             " CASE WHEN len(b.accno)=17 THEN c.accname+'('+d.accname+')'" & _
    '                 " WHEN len(b.accno)<>17 THEN c.accname END AS accname" & _
    '             " FROM BGF030 a INNER JOIN BGF020 b ON a.BGNO = b.BGNO INNER JOIN ACCNAME c ON b.ACCNO = c.ACCNO " & _
    '             " LEFT OUTER JOIN accname d ON left(b.accno,16)=d.accno and len(b.accno)=17" & _
    '             " WHERE a.BGNO = '" & strKey1 & "' and c.account_NO = '" & Session("USERID") & "' AND a.date4 is null"


    '    objCmd99 = New SqlCommand(sqlstr, objCon99)
    '    objDR99 = objCmd99.ExecuteReader

    '    If objDR99.Read Then
    '        Dim intI, SumUp As Integer
    '        Dim strI, strColumn1, strColumn2 As String

    '        lblBgno.Text = Trim(objDR99("BGNO").ToString)   '不允許修改bgno,accyear,accno
    '        lblYear.Text = Trim(objDR99("accyear").ToString)
    '        lblAccno.Text = Trim(objDR99("accno").ToString)
    '        lblAccname.Text = Trim(objDR99("accname").ToString)
    '        lblDate1.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE1").ToShortDateString.ToString))
    '        lblDate2.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE2").ToShortDateString.ToString))
    '        lblDate3.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE3").ToShortDateString.ToString))
    '        If Trim(objDR99("KIND").ToString) = "1" Then
    '            rdbKind1.Checked = True
    '        Else
    '            rdbKind2.Checked = True
    '        End If
    '        txtRemark.Text = Trim(objDR99("REMARK").ToString)
    '        lblRemark.Text = txtRemark.Text  '用來判定remark是否有異動,要update remark
    '        txtUseAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useamt"), 0), 0)
    '        lblUseAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useamt"), 0), 0)
    '        lblUseableAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useableamt"), 0), 0)
    '        lblAmt1.Text = FormatNumber(Master.ADO.nz(objDR99("amt1"), 0), 0)
    '        lblAmt2.Text = FormatNumber(Master.ADO.nz(objDR99("AMT2"), 0), 0)
    '        lblAmt3.Text = FormatNumber(Master.ADO.nz(objDR99("AMT2"), 0), 0)
    '        txtSubject.Text = Trim(objDR99("SUBJECT").ToString)
    '        lblSubject.Text = Trim(objDR99("SUBJECT").ToString) '用來判定subject是否有異動,要update subject
    '        lblMark.Text = ""   '憑證應補事項
    '        lblkey.Text = Trim(objDR99("autono").ToString)
    '        lblBgamt.Text = FormatNumber(Master.Controller.QueryBGAmt(Trim(objDR99("ACCYEAR").ToString), Trim(objDR99("ACCNO").ToString)), 0)
    '        lblUnUseamt.Text = FormatNumber(Master.Controller.QueryUnUseAmt(Trim(objDR99("accyear").ToString), Trim(objDR99("accno").ToString), Session("sSeason")) + Master.Models.ValComa(lblUseAmt.Text), 0)
    '        lblUsedAmt.Text = FormatNumber(Master.Controller.QueryUsedAmt(Trim(objDR99("BGNO").ToString)) - Master.Models.ValComa(lblUseAmt.Text), 0) '已開支(要扣除此筆開支)
    '        lblGrade6.Text = ""
    '        'KEY、異動人員及日期*****
    '        txtKey1.Text = Trim(objDR99("BGNO").ToString)

    '    End If

    '    objDR99.Close()    '關閉連結
    '    objCon99.Close()
    '    objCmd99.Dispose() '手動釋放資源
    '    objCon99.Dispose()
    '    objCon99 = Nothing '移除指標

    'End Sub


#End Region

#Region "物件選擇異動值"

    '會計科目下拉式
    Sub PutAccnoToCbo()
        Dim sqlstr, StrBg, strSeasonBg As String
        Select Case Session("sSeason")
            Case Is = 1
                StrBg = "Ltrim(str(a.bg1+a.up1-a.totuse))"
                strSeasonBg = "Ltrim(str(a.bg1+a.up1-a.totper-a.totuse))"
            Case Is = 2
                StrBg = "Ltrim(str(a.bg1+a.bg2+a.up1+a.up2-a.totuse))"
                strSeasonBg = "Ltrim(str(a.bg1+a.bg2+a.up1+a.up2-a.totuse-a.totper))"
            Case Is = 3
                StrBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.up1+a.up2+a.up3-a.totuse))"
                strSeasonBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.up1+a.up2+a.up3-a.totuse-a.totper))"
            Case Is = 4
                StrBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totuse))"
                strSeasonBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totuse-a.totper))"
        End Select
        sqlstr = "SELECT right('0'+ltrim(str(a.accyear)),3) + ' ' +left(a.accno+space(17),17) as bgf010key, " & _
                 "CASE WHEN len(a.accno)=17 THEN " & _
                    "right('0'+ltrim(str(a.accyear)),3) +' '+left(a.accno+space(17),17)+c.accname+'-'+b.accname" & _
                    "+' 年預餘:'+Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totper-a.totuse))+" & _
                    "+' 季預餘:'+" & strSeasonBg & "+' 季支餘:'+" & StrBg & _
                 " WHEN len(a.accno)<17 THEN " & _
                    "right('0'+ltrim(str(a.accyear)),3) +' '+left(a.accno+space(17),17)+b.accname" & _
                    "+' 年預餘:'+Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totper-a.totuse))+" & _
                    "+' 季預餘:'+" & strSeasonBg & "+' 季支餘:'+" & StrBg & _
                 " END AS bgf010data " & _
                 "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO " & _
                 "WHERE a.ctrl<>'Y' AND b.STAFF_NO = '" & Session("USERID") & "' order by a.accyear,a.accno"

        Master.Controller.objDropDownListOptionEX(cboAccno, DNS_ACC, sqlstr, "bgf010key", "bgf010data", 0)

    End Sub

#End Region






    Protected Sub cboUnitS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboUnitS.SelectedIndexChanged
        txtUnitS.Text = cboUnitS.SelectedValue
    End Sub

    Protected Sub cboUnitE_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboUnitE.SelectedIndexChanged
        txtUnitE.Text = cboUnitE.SelectedValue
    End Sub

    Sub AddNewRecordRowToGrid()
        ' check view state is not null  
        'If ViewState("dt_bgf020") IsNot Nothing Then
        '    'get datatable from view state   
        '    Dim dtCurrentTable As DataTable = DirectCast(ViewState("AuthorBooks"), DataTable)
        '    Dim drCurrentRow As DataRow = Nothing

        '    If dtCurrentTable.Rows.Count > 0 Then

        '        For i As Integer = 1 To dtCurrentTable.Rows.Count

        '            'add each row into data table  
        '            drCurrentRow = dtCurrentTable.NewRow()
        '            drCurrentRow("AuthorName") = TextBox1.Text
        '            drCurrentRow("BrandName") = TextBox2.Text
        '            drCurrentRow("Warrenty") = TextBox3.Text


        '            drCurrentRow("Price") = TextBox4.Text
        '        Next
        '        'Remove initial blank row  
        '        If dtCurrentTable.Rows(0)(0).ToString() = "" Then
        '            dtCurrentTable.Rows(0).Delete()

        '            dtCurrentTable.AcceptChanges()
        '        End If

        '        'add created Rows into dataTable  
        '        dtCurrentTable.Rows.Add(drCurrentRow)
        '        'Save Data table into view state after creating each row  
        '        ViewState("AuthorBooks") = dtCurrentTable
        '        'Bind Gridview with latest Row  
        '        GridView1.DataSource = dtCurrentTable
        '        GridView1.DataBind()
        '    End If
        'End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If ViewState("AuthorBooks") IsNot Nothing Then
            'get datatable from view state   
            Dim dtCurrentTable As DataTable = DirectCast(ViewState("AuthorBooks"), DataTable)
            Dim drCurrentRow As DataRow = Nothing

            Dim intYear As String = Val(Mid(cboAccno.SelectedValue, 1, 3))            '請購年度
            Dim strAccno As String = Trim(Mid(cboAccno.SelectedValue, 5, 17))    '請購科目
            Dim strAccname As String = Trim(Mid(cboAccno.SelectedItem.ToString, 23, 50)) '請購科目名稱
            If strAccname.IndexOf("年預") > 0 Then
                strAccname = Trim(Mid(strAccname, 1, strAccname.IndexOf("年預")))
            End If
            '由單位檔逐筆增入DATAGRID
            sqlstr = "select * from unittable where unit>='" & txtUnitS.Text & "' and unit<='" & txtUnitE.Text & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "unittable", sqlstr)

            For RowI = 0 To TempDataSet.Tables("unittable").Rows.Count - 1
                With TempDataSet.Tables("unittable").Rows(RowI)
                    'ViewState("strUnitName") = .Item("shortname")
                    'ViewState("strCashier") = .Item("cashier")

                    drCurrentRow = dtCurrentTable.NewRow()
                    drCurrentRow("BGNO") = RowI
                    drCurrentRow("accyear") = intYear
                    drCurrentRow("accno") = strAccno
                    drCurrentRow("remark") = .Item("shortname") & txtRemarka.Text & "  " & .Item("cashier")
                    drCurrentRow("subject") = ""
                    drCurrentRow("amt1") = 0
                    drCurrentRow("accname") = strAccname

                    dtCurrentTable.Rows.Add(drCurrentRow)
                End With
            Next

            ViewState("AuthorBooks") = dtCurrentTable
            'Bind Gridview with latest Row  
            DataGridView.DataSource = dtCurrentTable
            DataGridView.DataBind()
        End If

        'If ViewState("AuthorBooks") IsNot Nothing Then
        '    'get datatable from view state   
        '    Dim dtCurrentTable As DataTable = DirectCast(ViewState("AuthorBooks"), DataTable)
        '    Dim drCurrentRow As DataRow = Nothing

        '    If dtCurrentTable.Rows.Count > 0 Then

        '        For i As Integer = 1 To dtCurrentTable.Rows.Count

        '            'add each row into data table  
        '            drCurrentRow = dtCurrentTable.NewRow()
        '            drCurrentRow("BGNO") = "123"

        '        Next
        '        'Remove initial blank row  
        '        If dtCurrentTable.Rows(0)(0).ToString() = "" Then
        '            dtCurrentTable.Rows(0).Delete()

        '            dtCurrentTable.AcceptChanges()
        '        End If

        '        'add created Rows into dataTable  
        '        dtCurrentTable.Rows.Add(drCurrentRow)
        '        'Save Data table into view state after creating each row  
        '        ViewState("AuthorBooks") = dtCurrentTable
        '        'Bind Gridview with latest Row  
        '        DataGridView.DataSource = dtCurrentTable
        '        DataGridView.DataBind()
        '    End If
        'End If

        'Call LoadGridFunc() '產生空結構

        'btnAddbatch.Enabled = True

        'ViewState("intYear") = Val(Mid(cboAccno.SelectedValue, 1, 3))            '請購年度
        'ViewState("strAccno") = Trim(Mid(cboAccno.SelectedValue, 5, 17))    '請購科目
        'ViewState("strAccname") = Trim(Mid(cboAccno.Text, 23, 50)) '請購科目名稱
        'If ViewState("strAccname").IndexOf("年預") > 0 Then
        '    ViewState("strAccname") = Trim(Mid(ViewState("strAccname"), 1, ViewState("strAccname").IndexOf("年預")))
        'End If
        ''由單位檔逐筆增入DATAGRID
        'sqlstr = "select * from unittable where unit>='" & txtUnitS.Text & "' and unit<='" & txtUnitE.Text & "'"
        'TempDataSet = Master.ADO.openmember(DNS_ACC, "unittable", sqlstr)
        'Dim nr As DataRow
        'For RowI = 0 To TempDataSet.Tables("unittable").Rows.Count - 1
        '    With TempDataSet.Tables("unittable").Rows(RowI)
        '        ViewState("strUnitName") = .Item("shortname")
        '        ViewState("strCashier") = .Item("cashier")
        '        'Call GridAdd()

        '    End With
        'Next
        'DataGridView.DataSource = myDataSet
        'DataGridView.DataMember = "BGF020"
        'DataGridView.DataBind()

        'TabContainer1.ActiveTabIndex = 1
        'MsgBox("請逐筆輸入請購金額")
    End Sub
    Sub LoadGridFunc()
        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT a.*, b.accname from bgf020 a left outer join accname b on a.accno=b.accno where a.accyear=999" '產生空結構
        myDataSet = Master.ADO.openmember(DNS_ACC, "BGF020", sqlstr)
        DataGridView.DataSource = myDataSet
        DataGridView.DataMember = "BGF020"

        TabContainer1.ActiveTabIndex = 1
    End Sub

    Function GridAdd()
        Dim nr As DataRow
        nr = myDataSet.Tables("BGF020").NewRow()
        nr("bgno") = ""
        nr("accyear") = ViewState("intYear")
        nr("accno") = ViewState("strAccno")
        nr("remark") = ViewState("strUnitName") & txtRemarka.Text & "  " & ViewState("strCashier")
        nr("subject") = ViewState("strCashier")
        nr("amt1") = 0
        nr("accname") = ViewState("strAccname")
        'nr("kind") = bm.Current("kind")
        'nr("area") = txtAreaA.Text
        myDataSet.Tables("BGF020").Rows.Add(nr)                          '增行至source grid
    End Function

    Protected Sub btnAddbatch_Click(sender As Object, e As EventArgs) Handles btnAddbatch.Click

    End Sub
End Class