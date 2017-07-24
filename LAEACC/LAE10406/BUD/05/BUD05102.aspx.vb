Imports System.Data
Imports System.Data.SqlClient

Public Class BUD05102
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

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤
        UPDATE_ID.Focus()
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
        UCBase1.Visible = False
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
    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim strMySearch As String = ""

        '開啟查詢*****
        '項目
        If S_ACCYEAR.SelectedValue <> "" Then strMySearch &= " AND a.ACCYEAR LIKE '%" & S_ACCYEAR.SelectedValue & "%'"
        If S_BGNO.Text <> "" Then strMySearch &= " AND a.BGNO LIKE '%" & S_BGNO.Text & "%'"


        '初始化*****
        UCBase1.SetButtons()                         '初始化控制鍵
        ViewState("MySearch") = strMySearch          '查詢值記錄
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '開啟查詢
    End Sub

    '清除條件
    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        '還原預設值*****
        S_ACCYEAR.SelectedIndex = -1
        S_BGNO.Text = ""

        '初始化*****
        UCBase1.SetButtons() '初始化控制鍵
    End Sub
#End Region



#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        '開啟查詢
        strSSQL = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.amt1, a.remark,"
        strSSQL &= "CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')' "
        strSSQL &= " WHEN len(a.accno)<>17 THEN b.accname END AS accname, a.kind, a.subject FROM BGF020 a "
        strSSQL &= " INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO"
        strSSQL &= " LEFT JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO and len(a.accno)=17"
        strSSQL &= " WHERE a.CLOSEMARK <> 'Y' and a.date2 is null"
        strSSQL &= strSearch
        strSSQL &= IIf(Session("USERID") = "admin", "", " and b.STAFF_NO = '" & Session("USERID") & "'")
        strSSQL &= " ORDER BY " & strOrder & " " & strSortType

        lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
        Master.Controller.objDataGrid(DataGridView, lbl_GrdCount, DNS_ACC, strSSQL, "查詢資料檔")


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


        '防呆*****
        '--必輸入--
        'TextBox
        Dim objTextBox() As TextBox = {USEABLEAMT}
        Dim strTextBox As String = "請購金額"
        strMessage = Master.Controller.TextBox_Input(objTextBox, 0, strTextBox)
        If strMessage <> "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & strMessage & "】未輸入!!');", True) : Exit Sub

        'DropDownList
        Dim objDropDownList() As DropDownList = {ACCNO}
        Dim strDropDownList As String = "請購科目"
        strMessage = Master.Controller.DropDownList_Input(objDropDownList, strDropDownList)
        If strMessage <> "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & strMessage & "】未選擇!!');", True) : Exit Sub

        '--必數字--
        'TextBox
        Dim objTextBoxN() As TextBox = {USEABLEAMT}
        Dim strTextBoxN As String = "請購金額"
        strMessage = Master.Controller.TextBox_Input(objTextBoxN, 1, strTextBoxN)
        If strMessage <> "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & strMessage & "】必數字!!');", True) : Exit Sub

        '-- 不可重複(只有新增才需判斷) --
        Dim strRow As String = ""
        If PrevTableStatus = "1" Then
            BGNO.Text = Master.Controller.AutoNumber(Mid(Session("DATE"), 1, 3), 5, DNS_ACC, "BGF020", "BGNO", "BGNO LIKE '" & Mid(Session("DATE"), 1, 3) & "%'") '請購編號                
            strRow = Master.ADO.dbGetRow(DNS_ACC, "BGF020", "BGNO", "BGNO = '" & BGNO.Text & "'")
            blnCheck = IIf(strRow <> "", True, False) : If blnCheck = True Then MsgBox("【請購編號】，已存在!!") : Exit Sub
            txtKey1.Text = BGNO.Text
        End If
        Dim loadkey As String = BGNO.Text

        '判斷程序為新增或修改*****
        If PrevTableStatus = "1" Then SaveStatus = InsertData() : ViewState("FileKey") = txtKey1.Text '新增
        If PrevTableStatus = "2" Then SaveStatus = UpdateData() : ViewState("FileKey") = txtKey1.Text '修改

        If SaveStatus = True Then
            ViewState("MyStatus") = 0
            SetControls(0)
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            UCBase1.SetButtons()
            FindData(loadkey) '直接查詢值
        End If


        '異動後初始化*****
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作" & IIf(PrevTableStatus = 1, "新增", "修改") & "資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
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

        Dim skind, sdc As String

        If Mid(ACCNO.Text, 1, 1) = "4" Then   '99/12/6 update 
            If USEABLEAMT.Text > 0 Then   '收入科目  正數表示收入傳票, 貸方金額
                skind = "1"
                sdc = "2"
            Else
                skind = "2"
                sdc = "1"
            End If
        Else
            If USEABLEAMT.Text > 0 Then  '支出科目  正數表示支出傳票, 借方金額
                skind = "2"
                sdc = "1"
            Else
                skind = "1"
                sdc = "2"
            End If
        End If

        strIRow = "BGNO, ACCYEAR, ACCNO, KIND,"
        strIRow &= "DC, DATE1, REMARK, USEABLEAMT,AMT1,"
        strIRow &= "CLOSEMARK,UNIT,"
        strIRow &= "UPDATE_ID,UPDATE_DATE"

        strIValue = "'" & BGNO.Text & "','" & ACCYEAR.Text & "','" & ACCNO.Text & "','" & skind & "',"
        strIValue &= "'" & sdc & "','" & Master.Models.strDateChinessToAD(DATE1.Text) & "','" & REMARK.Text & "','" & USEABLEAMT.Text & "','" & USEABLEAMT.Text & "',"
        strIValue &= "'" & "" & "','" & UNIT.Text & "',"
        strIValue &= "'" & Session("USERID") & "','" & Master.Models.NowDate & "'"

        blnCheck = Master.ADO.dbInsert(DNS_ACC, "BGF020", strIRow, strIValue) '開啟儲存

        Return blnCheck
    End Function
    '修改
    Public Function UpdateData() As Boolean
        Dim blnCheck As Boolean = False

        'ACCNAME[會計科目]*****
        strUValue = "ACCNO = '" & ACCNO.SelectedValue & "',"
        strUValue &= "REMARK = '" & REMARK.Text.Trim() & "',"
        'strUValue &= "SUBJECT = '" & SUBJECT.Text & "',"
        strUValue &= "USEABLEAMT = '" & USEABLEAMT.Text & "'"

        strWValue = "BGNO = '" & txtKey1.Text & "'"

        blnCheck = Master.ADO.dbEdit(DNS_ACC, "BGF020", strUValue, strWValue) '開啟儲存

        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False

        ''開啟儲存*****
        Dim strKey1 As String = txtKey1.Text

        SaveStatus = Master.ADO.dbDelete(DNS_ACC, "BGF020", "BGNO = '" & strKey1 & "'") 'BGF020[請購推算檔]

        If SaveStatus = True Then
            ViewState("MyStatus") = 0
            SetControls(0)
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            FlagMoveSeat(1)
            TabContainer1.ActiveTabIndex = 0
        End If

        '異動後初始化*****
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('" & "操作刪除資料" & IIf(SaveStatus = True, "成功", "失敗") & "');", True)
        Master.ACC.SystemOperate(Session("USERID"), strPage, "BGF020", strKey1, "D", IIf(SaveStatus = True, "S", "F")) '系統操作歷程記錄
    End Sub

    '輸入控制項的 ReadOnly 屬性及 Enabled 屬性
    Public Sub SetControls(ByVal Status As Integer)
        '頁面控制項ID陣列***** 
        'Status: 0:一般模式 1:新增模式 2:修改模式 3:複製模式
        Dim objMainTextBox() As TextBox = {}
        Dim objMainDropDownList() As DropDownList = {}
        Dim objMainRadioButtonList() As RadioButtonList = {}
        Dim objTextBox() As TextBox = {USEABLEAMT, REMARK}
        Dim objDropDownList() As DropDownList = {ACCNO}
        Dim objRadioButtonList() As RadioButtonList = {}

        Master.Controller.Main_Control(objMainTextBox, objMainDropDownList, objMainRadioButtonList, Status)
        Master.Controller.TextBox_Control(objTextBox, Status) : Master.Controller.TextBox_Clear(objTextBox, Status)
        Master.Controller.DropDownList_Control(objDropDownList, Status) : Master.Controller.DropDownList_Clear(objDropDownList, Status)
        Master.Controller.RadioButtonList_Control(objRadioButtonList, Status) : Master.Controller.RadioButtonList_Clear(objRadioButtonList, Status)


        '自訂項目*****        
        If Status = "0" Or Status = "1" Then
            'DropDownList            
            Dim strYear As String = "" : For I As Integer = Mid(Master.Models.NowDate, 1, 3) To 80 Step -1 : strYear &= Master.Models.strZero(3, I) & "," : Next '年度
            Master.Controller.objDropDownListOptionGG(S_ACCYEAR, "--請選擇--," & strYear, "," & strYear)
        End If


        '其他控制項
        Select Case Status
            Case 0 '一般模式
                UPDATE_ID.Text = ""              '異動人員
                UPDATE_DATE.Text = ""            '異動日期

            Case 1 '新增模式
                TabContainer1.ActiveTabIndex = 1
                UPDATE_ID.Text = Master.ADO.dbGetRow(DNS_SYS, "users", "name", "user_id = '" & Session("USERID") & "'")
                UPDATE_DATE.Text = Master.Models.strStrToDate(Master.Models.NowDate)

                ACCYEAR.Text = Mid(Session("DATE"), 1, 3)
                UNIT.Text = Session("SID")
                UNITNAME.Text = Master.ADO.dbGetRow(DNS_SYS, "unit", "unit_name", "unit_id = '" & Session("SID") & "'")

                DATE1.Text = Session("DATE") '請購日期
                BGNO.Text = Master.Controller.AutoNumber(Mid(Session("DATE"), 1, 3), 5, DNS_ACC, "BGF020", "BGNO", "BGNO LIKE '" & Mid(Session("DATE"), 1, 3) & "%'") '請購編號                
                BUD1.Text = 0 : BUD2.Text = 0 : BUD3.Text = 0 : BUD4.Text = 0

                '預算科目
                strSQL99 = "select a.*,isnull(a.accno+b.accname,'') as ACCNAME from bgf010 a " & _
                            "inner join accname b on a.ACCNO=b.ACCNO where a.accyear='" & ACCYEAR.Text & "'"
                Master.Controller.objDropDownListOptionEX(ACCNO, DNS_ACC, strSQL99, "ACCNO", "ACCNAME", 0)

            Case 2 '修改模式
                UPDATE_ID.Text = Response.Cookies("NAME").Value
                UPDATE_DATE.Text = Master.Models.strStrToDate(Master.Models.NowDate)


            Case 3 '複製模式
                TabContainer1.ActiveTabIndex = 1
                UPDATE_ID.Text = Master.ADO.dbGetRow(DNS_SYS, "users", "name", "user_id = '" & Session("USERID") & "'")
                UPDATE_DATE.Text = Master.Models.strStrToDate(Master.Models.NowDate)
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
        BGF020_Load(strKey1) '載入資料
    End Sub
    '請購推算檔
    Sub BGF020_Load(ByVal strKey1 As String)
        '開啟查詢
        objCon99 = New SqlConnection(DNS_ACC)
        objCon99.Open()

        strSQL99 = "SELECT * FROM BGF020"
        strSQL99 &= " WHERE BGNO = '" & strKey1 & "'"

        objCmd99 = New SqlCommand(strSQL99, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            'KEY、異動人員及日期*****
            txtKey1.Text = Trim(objDR99("BGNO").ToString)
            UPDATE_ID.Text = Master.ADO.dbGetRow(DNS_SYS, "users", "name", "user_id = '" & Trim(objDR99("UPDATE_ID").ToString) & "'")
            UPDATE_DATE.Text = Master.Models.strStrToDate(Trim(objDR99("UPDATE_DATE").ToString))


            '文字欄位值*****
            ACCYEAR.Text = Trim(objDR99("ACCYEAR").ToString)
            UNIT.Text = Trim(objDR99("UNIT").ToString)
            UNITNAME.Text = Master.ADO.dbGetRow(DNS_SYS, "unit", "unit_name", "unit_id = '" & Trim(objDR99("UNIT").ToString) & "'")
            DATE1.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE1").ToShortDateString.ToString))
            BGNO.Text = Trim(objDR99("BGNO").ToString)
            USEABLEAMT.Text = Trim(objDR99("USEABLEAMT").ToString)
            REMARK.Text = Trim(objDR99("REMARK").ToString)


            '非文字物件*****
            'DropDownList
            '顯示值
            '預算科目
            strSQL99 = "select a.*,isnull(a.accno+b.accname,'') as ACCNAME from bgf010 a " & _
                        "inner join accname b on a.ACCNO=b.ACCNO where a.accyear='" & ACCYEAR.Text & "'"
            Master.Controller.objDropDownListOptionEX(ACCNO, DNS_ACC, strSQL99, "ACCNO", "ACCNAME", 0)

            Master.Controller.objDropDownListOptionCK(ACCNO, Trim(objDR99("ACCNO").ToString))
        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標


        '資料初始化*****
        TotalMoney(ACCNO.SelectedValue) '查詢總經費、季餘額、己開支數、年度餘額
    End Sub
#End Region

#Region "物件選擇異動值"
    '依會計科目 => 總經費、季餘額、己開支數、年度餘額
    Sub TotalMoney(ByVal strAccNo As String)
        Dim sqlstr, StrBg, strSeasonBg, sSeason As String
        '取
        sSeason = Master.Models.strDateChinessToSeason(DATE1.Text)

        StrBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totuse))"
        strSeasonBg = "Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totuse-a.totper))"

        Select Case sSeason
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

        '開啟查詢
        objCon99 = New SqlConnection(DNS_ACC)
        objCon99.Open()

        strSQL99 = "SELECT a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4 as bud1, " + strSeasonBg + " as bud2" & _
            ",a.totper+a.totuse as bud3,Ltrim(str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totuse-a.totper)) as bud4 " & _
            " fROM bgf010 a where  ACCYEAR = '" & ACCYEAR.Text & "' and ACCNO = '" & strAccNo & "'"

        objCmd99 = New SqlCommand(strSQL99, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            '文字欄位值*****
            BUD1.Text = FormatNumber(Trim(objDR99("BUD1").ToString), 2)
            BUD2.Text = FormatNumber(Trim(objDR99("BUD2").ToString), 2)
            BUD3.Text = FormatNumber(Trim(objDR99("BUD3").ToString), 2)
            BUD4.Text = FormatNumber(Trim(objDR99("BUD4").ToString), 2)
        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標
    End Sub
    Protected Sub ACCNO_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ACCNO.SelectedIndexChanged
        If ACCNO.SelectedValue = "" Then Exit Sub

        TotalMoney(ACCNO.SelectedValue)
    End Sub
#End Region
    '審查
    Protected Sub btnCheck_Click(sender As Object, e As EventArgs) Handles btnCheck.Click
        Dim blnCheck As Boolean = False

        'ACCNAME[會計科目]*****
        strUValue = "DATE2 = '" & Date.Now.ToShortDateString & "'"
        strWValue = "BGNO = '" & txtKey1.Text & "'"

        blnCheck = Master.ADO.dbEdit(DNS_ACC, "BGF020", strUValue, strWValue) '開啟儲存

        If blnCheck = True Then
            ViewState("MyStatus") = 0
            SetControls(0)
            FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
            TabContainer1.ActiveTabIndex = 0
        End If
    End Sub
End Class