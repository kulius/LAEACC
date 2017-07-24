Imports System.Data
Imports System.Data.SqlClient

Public Class PAY_ACF010
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
        TxtStartNo.Text = "1"    '起值
        TxtEndNo.Text = "15000"      '迄值



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
        ViewState("Syear") = nudYear.Text
        ViewState("Sfile") = IIf(rdbFile1.Checked = True, "1", "2")
        If rdbFile3.Checked = True Then ViewState("Sfile") = "3"
        If Val(TxtEndNo.Text) = 0 Then TxtEndNo.Text = TxtStartNo.Text
        ViewState("Sno") = CInt(Trim(TxtStartNo.Text))
        ViewState("Eno") = CInt(Trim(TxtEndNo.Text))
        If ViewState("Eno") < ViewState("Sno") Then ViewState("Eno") = ViewState("Sno")
        If rdbKind1.Checked = True Then ViewState("Skind") = "1"
        If rdbKind2.Checked = True Then ViewState("Skind") = "2"
        If rdbKind3.Checked = True Then ViewState("Skind") = "3"
        ViewState("Ekind") = ViewState("Skind")
        If ViewState("Skind") = "3" Then ViewState("Ekind") = "4"

        Dim sqlstr, qstr, sortstr As String
        sqlstr = "SELECT acf010.*, isnull(date_1, '') as date_1_n, isnull(date_2, '') as date_2_n FROM  acf010 where accyear=" & ViewState("Syear") & " and kind>='" & ViewState("Skind") & "' and kind<='" & ViewState("Ekind") & "' and "
        Select Case ViewState("Sfile")
            Case "1"   '未處理
                sqlstr = sqlstr & "no_1_no>=" & ViewState("Sno") & " and no_1_no<=" & ViewState("Eno") & " and date_2 is null"
                sortstr = " order by accyear, kind, no_1_no, seq, item"    '排序
            Case "2"   '已處理
                sqlstr = sqlstr & "no_2_no>=" & ViewState("Sno") & " and no_2_no<=" & ViewState("Eno") & " and date_2 is not null"
                sortstr = " order by accyear, kind, no_2_no, seq, item"
            Case "3"  '全部
                sqlstr = sqlstr & "no_1_no>=" & ViewState("Sno") & " and no_1_no<=" & ViewState("Eno")
                sortstr = " order by accyear, kind, no_1_no, seq, item"
        End Select
        If txtQremark.Text <> "" Then qstr = " and remark like '%" & Trim(txtQremark.Text) & "%'"
        If txtQaccno.Text <> "" Then qstr = qstr + " and accno like '%" & Trim(txtQaccno.Text) & "%'"
        If txtQarea.Text <> "" Then qstr = qstr + " and area='" & Trim(txtQarea.Text) & "'"
        If Master.Models.ValComa(txtQpayseq.Text) <> 0 Then qstr = qstr + " and payseq=" & Master.Models.ValComa(txtQpayseq.Text)
        If Master.Models.ValComa(txtQamt.Text) <> 0 Then qstr = qstr + " and amt =" & Master.Models.ValComa(txtQamt.Text)
        sqlstr = sqlstr + qstr + sortstr


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
        Master.ADO.GenInsSql("no_1_no", Trim(txtNo1.Text), "N")
        Master.ADO.GenInsSql("no_2_no", Trim(txtNo2.Text), "N")
        Master.ADO.GenInsSql("seq", Trim(txtSeq.Text), "T")
        Master.ADO.GenInsSql("item", Trim(txtItem.Text), "T")
        Master.ADO.GenInsSql("date_1", txtDate1.Text, "D")
        Master.ADO.GenInsSql("date_2", txtDate2.Text, "D")
        Master.ADO.GenInsSql("dc", Trim(txtDC.Text), "T")
        Master.ADO.GenInsSql("accno", vxtAccno.Text, "T")
        Master.ADO.GenInsSql("remark", Trim(txtRemark.Text), "U")
        Master.ADO.GenInsSql("amt", txtamt.Text, "N")
        Master.ADO.GenInsSql("act_amt", Trim(txtActamt.Text), "N")
        Master.ADO.GenInsSql("bank", Trim(txtBank.Text), "T")
        Master.ADO.GenInsSql("chkno", Trim(txtchkno.Text), "T")
        'Master.ADO.GenInsSql("payseq", Trim(txtPayseq.Text), "N")
        'Master.ADO.GenInsSql("area", Trim(txtArea.Text), "T")
        Master.ADO.GenInsSql("books", Trim(txtBooks.Text), "T")

        sqlstr = "insert into acf010 " & Master.ADO.GenInsFunc
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
        Master.ADO.GenUpdsql("no_1_no", Trim(txtNo1.Text), "N")
        Master.ADO.GenUpdsql("no_2_no", Trim(txtNo2.Text), "N")
        Master.ADO.GenUpdsql("seq", Trim(txtSeq.Text), "T")
        Master.ADO.GenUpdsql("item", Trim(txtItem.Text), "T")
        Master.ADO.GenUpdsql("date_1", txtDate1.Text, "D")
        Master.ADO.GenUpdsql("date_2", txtDate2.Text, "D")
        Master.ADO.GenUpdsql("dc", Trim(txtDC.Text), "T")
        Master.ADO.GenUpdsql("accno", vxtAccno.Text, "T")
        Master.ADO.GenUpdsql("remark", Trim(txtRemark.Text), "U")
        Master.ADO.GenUpdsql("amt", Trim(txtamt.Text), "N")
        Master.ADO.GenUpdsql("act_amt", Trim(txtActamt.Text), "N")
        Master.ADO.GenUpdsql("bank", Trim(txtBank.Text), "T")
        Master.ADO.GenUpdsql("chkno", Trim(txtchkno.Text), "T")
        'Master.ADO.GenUpdsql("payseq", Trim(txtPayseq.Text), "N")
        'Master.ADO.GenUpdsql("area", Trim(txtArea.Text), "T")
        Master.ADO.GenUpdsql("books", Trim(txtBooks.Text), "T")
        sqlstr = "update acf010 set " & Master.ADO.genupdfunc & " where autono=" & KeyValue
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

        sqlstr = "delete from acf010 where autono=" & keyvalue
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
        sqlstr = "SELECT   * ,* " & _
         " FROM  acf010 " & _
         " WHERE autono ='" & strKey1 & "'"

        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then

            'txtaccyear.Text = Trim(objDR99("accyear").ToString)
            'txtkind.Text = Trim(objDR99("kind").ToString)
            'txtcont_no.Text = Trim(objDR99("cont_no").ToString)
            Dim strDate1 As String = Trim(objDR99("date_1").ToString)
            Dim strDate2 As String = Trim(objDR99("date_2").ToString)

            If strDate1 <> "" Then strDate1 = Master.Models.strDateADToChiness(CDate(strDate1).ToShortDateString)
            If strDate2 <> "" Then strDate2 = Master.Models.strDateADToChiness(CDate(strDate2).ToShortDateString)

            txtYear.Text = Master.ADO.nz(objDR99("accyear"), 0)
            txtKind.Text = objDR99("kind").ToString
            txtNo1.Text = Master.ADO.nz(objDR99("no_1_no"), 0)
            txtNo2.Text = Master.ADO.nz(objDR99("no_2_no"), 0)
            txtSeq.Text = Master.ADO.nz(objDR99("seq"), 0)
            txtItem.Text = Master.ADO.nz(objDR99("item"), 0)
            txtDate1.Text = strDate1
            txtDate2.Text = strDate2
            txtDC.Text = objDR99("dc").ToString
            vxtAccno.Text = objDR99("accno").ToString
            txtRemark.Text = objDR99("remark").ToString
            txtamt.Text = Format(objDR99("amt"), "##0")
            txtActamt.Text = Format(objDR99("act_amt"), "##0")
            txtBank.Text = objDR99("bank").ToString
            txtchkno.Text = objDR99("chkno").ToString
            'txtPayseq.Text = Master.ADO.nz(objDR99("payseq"), 0)
            'txtArea.Text = objDR99("area").ToString
            txtBooks.Text = objDR99("books").ToString
            lblAccname.Text = " "

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


    Protected Sub btnAccname_Click(sender As Object, e As EventArgs) Handles btnAccname.Click
        Dim sqlstr As String
        Dim mydataset2 As DataSet
        sqlstr = "SELECT ACCNAME FROM ACCNAME WHERE ACCNO = '" & vxtAccno.Text & "'"
        mydataset2 = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If mydataset2.Tables("accname").Rows.Count = 0 Then
            lblAccname.Text = "無此代號"
        Else
            lblAccname.Text = mydataset2.Tables("accname").Rows(0).Item(0)
        End If
        mydataset2 = Nothing
    End Sub
End Class