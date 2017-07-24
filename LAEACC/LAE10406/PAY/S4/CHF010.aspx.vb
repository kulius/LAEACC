Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class CHF010
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


        TxtStartNo.Text = ""    '起值
        TxtEndNo.Text = "Z9999"      '迄值

        dtpStartDate.Text = Session("DATE")
        dtpEndDate.Text = Session("DATE")

        txtStartBank.Text = "01"




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


    '清除條件

#End Region



#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        'ViewState("Syear") = nudYear.Text
        'ViewState("Sfile") = IIf(rdbFile1.Checked = True, "1", "2")
        'If rdbFile3.Checked = True Then ViewState("Sfile") = "3"
        'If Val(TxtEndNo.Text) = 0 Then TxtEndNo.Text = TxtStartNo.Text
        'ViewState("Sno") = CInt(Trim(TxtStartNo.Text))
        'ViewState("Eno") = CInt(Trim(TxtEndNo.Text))
        'If ViewState("Eno") < ViewState("Sno") Then ViewState("Eno") = ViewState("Sno")
        'If rdbKind1.Checked = True Then ViewState("Skind") = "1"
        'If rdbKind2.Checked = True Then ViewState("Skind") = "2"
        'If rdbKind3.Checked = True Then ViewState("Skind") = "3"
        'ViewState("Ekind") = ViewState("Skind")
        'If ViewState("Skind") = "3" Then ViewState("Ekind") = "4"

        If rdbKind1.Checked = True Then ViewState("Skind") = "1"
        If rdbKind2.Checked = True Then ViewState("Skind") = "2"

        Dim sqlstr, qstr As String
        sqlstr = "SELECT isnull(date_2,'') as date_2,* FROM chf010 where kind='" & ViewState("Skind") & "' and bank='" & txtStartBank.Text & "' and "
        '已領 or 未領
        If rdbCheck1.Checked Then  '已領
            sqlstr = sqlstr & " date_2 is not null and date_2 >='" & Master.Models.FullDate(dtpStartDate.Text) & "' and date_2 <='" & Master.Models.FullDate(dtpEndDate.Text) & "'"
        Else '未領()
            sqlstr = sqlstr & " date_2 is null and date_1 >='" & Master.Models.FullDate(dtpStartDate.Text) & "' and date_1 <='" & Master.Models.FullDate(dtpEndDate.Text) & "'"
        End If
        sqlstr = sqlstr & " and chkno>='" & TxtStartNo.Text & "' and chkno<='" & TxtEndNo.Text & "'"
        If txtQremark.Text <> "" Then qstr = " and remark like '%" & Trim(txtQremark.Text) & "%'"
        If txtQchkname.Text <> "" Then qstr = qstr + " and chkname like '%" & Trim(txtQchkname.Text) & "%'"
        If txtQamt.Text <> "" Then qstr = qstr + " and amt =" & Master.Models.ValComa(txtQamt.Text)
        sqlstr = sqlstr & qstr
        '排序
        If rdbSortBank.Checked Then  '銀行順
            If rdbCheck1.Checked Then
                sqlstr = sqlstr & " order by bank, date_2"  '已領
            Else
                sqlstr = sqlstr & " order by bank, chkno"  '未領
            End If
        Else                         '日期順
            If rdbCheck1.Checked Then
                sqlstr = sqlstr & " order by date_2, bank"   '已領
            Else
                sqlstr = sqlstr & " order by date_1, bank"   '未領
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
        Dim sqlstr, retstr, updstr As String

        Master.ADO.GenInsSql("accyear", Trim(txtYear.Text), "N")
        Master.ADO.GenInsSql("kind", Trim(txtKind.Text), "T")
        Master.ADO.GenInsSql("bank", Trim(txtBank.Text), "T")
        Master.ADO.GenInsSql("chkno", txtChkno.Text, "T")
        Master.ADO.GenInsSql("date_1", Trim(txtDate1.Text), "D")
        Master.ADO.GenInsSql("date_2", Trim(txtDate2.Text), "D")
        Master.ADO.GenInsSql("chkname", txtChkname.Text, "U")
        Master.ADO.GenInsSql("remark", Trim(txtRemark.Text), "U")
        Master.ADO.GenInsSql("amt", Trim(txtamt.Text), "N")
        Master.ADO.GenInsSql("start_no", txtStart_no.Text, "N")
        Master.ADO.GenInsSql("End_no", txtEnd_no.Text, "N")
        Master.ADO.GenInsSql("no_1_no", txtNO1.Text, "T")

        sqlstr = "insert into chf010 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        If retstr = "sqlok" Then
            MessageBx("新增成功")
            blnCheck = True
        Else
            MessageBx("新增full  " & sqlstr)
        End If

        'Dim sqlstr, retstr, updstr As String
        'Master.ADO.GenInsSql("accyear", txtaccyear.Text, "T")
        'Master.ADO.GenInsSql("kind", txtkind.Text, "U")
        'Master.ADO.GenInsSql("cont_no", txtcont_no.Text, "T")
        'sqlstr = "insert into Acfno " & Master.ADO.GenInsFunc
        'retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        'If retstr = "sqlok" Then
        '    MessageBx("新增成功")
        '    blnCheck = True
        'Else
        '    MessageBx("新增full  " & sqlstr)
        'End If

        Return blnCheck
    End Function
    '修改
    Public Function UpdateData() As Boolean
        Dim blnCheck As Boolean = False

        Dim sqlstr, retstr, updstr, KeyValue As String
        Dim Rmark As Integer
        KeyValue = lblkey.Text

        Master.ADO.GenUpdsql("accyear", Trim(txtYear.Text), "N")
        Master.ADO.GenUpdsql("kind", Trim(txtKind.Text), "T")
        Master.ADO.GenUpdsql("bank", Trim(txtBank.Text), "T")
        Master.ADO.GenUpdsql("chkno", txtChkno.Text, "T")
        Master.ADO.GenUpdsql("date_1", Trim(txtDate1.Text), "D")
        Master.ADO.GenUpdsql("date_2", Trim(txtDate2.Text), "D")
        Master.ADO.GenUpdsql("chkname", txtChkname.Text, "U")
        Master.ADO.GenUpdsql("remark", Trim(txtRemark.Text), "U")
        Master.ADO.GenUpdsql("amt", Trim(txtamt.Text), "N")
        Master.ADO.GenUpdsql("start_no", txtStart_no.Text, "N")
        Master.ADO.GenUpdsql("End_no", txtEnd_no.Text, "N")
        Master.ADO.GenUpdsql("no_1_no", txtNO1.Text, "T")
        sqlstr = "update chf010 set " & Master.ADO.genupdfunc & " where autono=" & KeyValue
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("修改成功")
            blnCheck = True
        Else
            MessageBx("修改full  " & sqlstr)
        End If

        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False


        Dim keyvalue, sqlstr, retstr As String

        keyvalue = Trim(lblkey.Text)

        sqlstr = "delete from chf010 where autono=" & keyvalue
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

        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT   isnull(date_2,'') as date_2 ,* " & _
         " FROM  chf010 " & _
         " WHERE autono ='" & strKey1 & "'"

        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then

            txtYear.Text = Master.ADO.nz(objDR99("accyear"), 0)
            txtKind.Text = Master.ADO.nz(objDR99("kind"), "")
            txtBank.Text = Master.ADO.nz(objDR99("bank"), "")
            txtChkno.Text = Master.ADO.nz(objDR99("chkno"), "")
            txtDate1.Text = Master.Models.strDateADToChiness(Trim(objDR99("date_1").ToShortDateString.ToString))
            txtDate2.Text = Master.Models.strDateADToChiness(Trim(objDR99("date_2").ToShortDateString.ToString))
            txtChkname.Text = Master.ADO.nz(objDR99("chkname"), " ")
            txtamt.Text = FormatNumber(Master.ADO.nz(objDR99("amt"), 0), 2)
            txtRemark.Text = Master.ADO.nz(objDR99("remark"), "")
            txtStart_no.Text = Master.ADO.nz(objDR99("start_no"), 0)
            txtEnd_no.Text = Master.ADO.nz(objDR99("end_no"), 0)
            txtNO1.Text = Master.ADO.nz(objDR99("no_1_no"), 0)


            lblkey.Text = Trim(objDR99("autono").ToString)
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
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
    End Sub


    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        '丟選定之當年度預算科目所有之開支to Grid2 
        Dim sqlstr, qstr, strD, strC As String
        Dim i, TOTBG, TOTUSE As Integer

        If rdbKind1.Checked = True Then ViewState("Skind") = "1"
        If rdbKind2.Checked = True Then ViewState("Skind") = "2"
        sqlstr = "SELECT accyear as 年,kind as 收支,chkno as 支票號,RIGHT('0'+CAST(CONVERT(CHAR(8),date_1,112)-19110000 AS VARCHAR(8)),7) as 開票日,RIGHT('0'+CAST(CONVERT(CHAR(8),date_2,112)-19110000 AS VARCHAR(8)),7) as 收付日, "
        sqlstr = sqlstr & " chkname as 受款人,amt as 金額,remark as  摘要,start_no as 傳票起號,end_no as 傳票迄號,no_1_no as 製票號 FROM chf010 where kind='" & ViewState("Skind") & "' and bank='" & txtStartBank.Text & "' and "
        '已領 or 未領
        If rdbCheck1.Checked Then  '已領
            sqlstr = sqlstr & " date_2 is not null and date_2 >='" & Master.Models.FullDate(dtpStartDate.Text) & "' and date_2 <='" & Master.Models.FullDate(dtpEndDate.Text) & "'"
        Else '未領()
            sqlstr = sqlstr & " date_2 is null and date_1 >='" & Master.Models.FullDate(dtpStartDate.Text) & "' and date_1 <='" & Master.Models.FullDate(dtpEndDate.Text) & "'"
        End If
        sqlstr = sqlstr & " and chkno>='" & TxtStartNo.Text & "' and chkno<='" & TxtEndNo.Text & "'"
        If txtQremark.Text <> "" Then qstr = " and remark like '%" & Trim(txtQremark.Text) & "%'"
        If txtQchkname.Text <> "" Then qstr = qstr + " and chkname like '%" & Trim(txtQchkname.Text) & "%'"
        If txtQamt.Text <> "" Then qstr = qstr + " and amt =" & Master.Models.ValComa(txtQamt.Text)
        sqlstr = sqlstr & qstr
        '排序
        If rdbSortBank.Checked Then  '銀行順
            If rdbCheck1.Checked Then
                sqlstr = sqlstr & " order by bank, date_2"  '已領
            Else
                sqlstr = sqlstr & " order by bank, chkno"  '未領
            End If
        Else                         '日期順
            If rdbCheck1.Checked Then
                sqlstr = sqlstr & " order by date_2, bank"   '已領
            Else
                sqlstr = sqlstr & " order by date_1, bank"   '未領
            End If
        End If


        mydataset = Master.ADO.openmember(DNS_ACC, "Export", sqlstr)

        Master.ExportDataTableToExcel(mydataset.Tables("Export"))
        'Dim excel As New ExcelPackage()
        'Dim sheet As ExcelWorksheet = excel.Workbook.Worksheets.Add("sheet1")

        'sheet.Cells("A1").LoadFromDataTable(mydataset.Tables("Export"), True)
        'sheet.Cells.Style.Font.Size = 11



        'Dim ms As New MemoryStream()
        'excel.SaveAs(ms)
        'Dim fileName As String = HttpUtility.UrlEncode("Excel.xlsx")

        'Response.ContentType = "application/vnd.ms-excel"
        ''Response.ContentType = "application/download"; //也可以设置成download    
        'Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", fileName))

        'Response.Buffer = True
        'Response.Clear()
        'Response.BinaryWrite(ms.GetBuffer())
        'Response.[End]()
    End Sub
End Class