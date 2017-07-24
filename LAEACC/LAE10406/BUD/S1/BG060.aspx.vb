Imports System.Data
Imports System.Data.SqlClient

Public Class BG060
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
    Dim TempDataSet, myDataSet2 As DataSet
    Dim mydataset As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        ViewState("MyOrder") = "bgcno"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
        Master.Controller.objDataGridStyle(DataGrid2, "S")
        UCBase1.Visible = False
        'UCBase1.SetButtons_Visible()                         '初始化控制鍵
        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        PutAccnoToCbo()
        LoadGridFunc()
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
        Master.Controller.objDataGridStyle(DataGrid2, "S")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            '其他預設值*****
            'SetControls(0) '設定所有輸入控制項的唯讀狀態

            ''資料查詢*****
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
                'FlagMoveSeat(0, e.Item.ItemIndex)
                TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
        End Select
    End Sub
    Sub DataGrid2_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid2.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號


        Select Case e.CommandName
            Case "btnShow"
                '開啟查詢
                objCon99 = New SqlConnection(DNS_ACC)
                objCon99.Open()

                Dim sqlstr, qstr, strD, strC As String
                sqlstr = "SELECT b.BGNO, b.ACCYEAR, b.ACCNO, b.KIND, c.REMARK, abs(c.USEAMT) as useamt, " & _
                 "CASE WHEN len(b.accno)=17 THEN d.accname+'('+e.accname+')' " & _
                     " WHEN len(b.accno)<>17 THEN d.accname END AS accname " & _
                 "FROM BGF040 a " & _
                 "INNER JOIN BGF020 b ON a.bgno = b.BGNO " & _
                 "INNER JOIN BGF030 c ON b.BGNO = c.BGNO " & _
                 "INNER JOIN ACCNAME d ON b.ACCNO = d.ACCNO " & _
                 "LEFT OUTER JOIN accname e ON LEFT(b.ACCNO, 16) = e.ACCNO and len(b.accno)=17 " & _
                 "WHERE a.bgcno = " & ViewState("intBGCNO") & " and a.accyear=" & Session("sYear") & _
                 " and a.BGNO= '" & txtID.Text & "'"

                objCmd99 = New SqlCommand(sqlstr, objCon99)
                objDR99 = objCmd99.ExecuteReader

                If objDR99.Read Then

                    lblBgno.Text = Trim(objDR99("BGNO").ToString)  'same to doubleclick

                    ViewState("strBGNo") = Trim(objDR99("BGNO").ToString)
                    If Trim(objDR99("kind").ToString) = "3" Then
                        rdbKind1.Checked = True
                        rdbKind2.Checked = False
                    Else
                        rdbKind1.Checked = False
                        rdbKind2.Checked = True
                    End If
                    lblKind.Text = Trim(objDR99("kind").ToString)
                    lblaccyear.Text = Trim(objDR99("accyear").ToString)
                    lblAccno.Text = Trim(objDR99("accno").ToString)
                    'cboAccno.SelectedValue = Mid("0" + LTrim(Str(Trim(objDR99("accyear").ToString))), 1, 3) + Mid(Trim(objDR99("accno").ToString) + Space(17), 1, 17) '設定科目選定值
                    cboAccno.SelectedValue = Format(CInt(Trim(objDR99("accyear").ToString)), "000") + Mid(Trim(objDR99("accno").ToString) + Space(17), 1, 17) '設定科目選定值
                    txtRemark.Text = Trim(objDR99("remark").ToString)
                    txtUseAmt.Text = Trim(objDR99("useamt").ToString)
                    lblUseAmt.Text = Format(Master.ADO.nz(objDR99("useamt"), 0), "###,###,###.#")

                End If

                objDR99.Close()    '關閉連結
                objCon99.Close()
                objCmd99.Dispose() '手動釋放資源
                objCon99.Dispose()
                objCon99 = Nothing '移除指標


        End Select
    End Sub
    '自選排序
    Sub DataGridView_SortCommand(ByVal sender As Object, ByVal e As DataGridSortCommandEventArgs) Handles DataGridView.SortCommand
        ViewState("MyOrder") = e.SortExpression
        ViewState("MySort") = IIf(ViewState("MySort") = "" Or ViewState("MySort") = "ASC", "DESC", "ASC")

        LoadGridFunc()
    End Sub
#End Region
#Region "按鍵選項"
    '查詢


    '清除條件

#End Region



#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        '開啟查詢
        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT a.bgcno, a.accyear, a.datec, b.KIND, SUM(abs(c.USEAMT)) AS useamt " & _
                 "FROM BGF040 a INNER JOIN BGF020 b ON a.bgno = b.BGNO " & _
                 "INNER JOIN BGF030 c ON a.bgno = c.BGNO " & _
                 "WHERE (a.no_1_no = 0 and a.accyear=" & Session("sYear") & ") GROUP BY  a.bgcno, a.accyear, a.datec, b.KIND " & _
                 "ORDER BY a.bgcno"

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
        Dim retstr, sqlstr, strBgno As String
        Dim intNO As Integer
        If Master.Models.ValComa(txtUseAmt.Text) <= 0 Then
            MessageBx("金額不可為零或負數")
            Exit Function
        End If

        'If myDataSet2.Tables("bgf020").Rows.Count = 0 Then
        ViewState("intBGCNO") = Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "C")     '當第一筆轉帳明細時,才確實取用轉帳編號
        lblBgcno.Text = ViewState("intBGCNO")
        'End If

        intNO = Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "B") '取得請購編號
        Dim BYear, I As Integer '請購的預算年度
        Dim BAccno, Bkind As String '請購科目
        Dim Bamt As Decimal
        BYear = Mid(cboAccno.SelectedValue, 1, 3)
        BAccno = Trim(Mid(cboAccno.SelectedValue, 4, 17))
        strBgno = Format(CInt(Session("sYear")), "000") + Format(intNO, "00000")
        Bkind = IIf(rdbKind1.Checked, "3", "4")

        Bamt = Master.Models.ValComa(txtUseAmt.Text)
        If Bkind = "3" Then  '借方
            If Mid(BAccno, 1, 1) = "3" Or Mid(BAccno, 1, 1) = "4" Then Bamt = -Master.Models.ValComa(txtUseAmt.Text)
        Else  '2負債科目亦同支出處理
            If Mid(BAccno, 1, 1) = "5" Or Mid(BAccno, 1, 1) = "1" Or Mid(BAccno, 1, 1) = "2" Then Bamt = -Master.Models.ValComa(txtUseAmt.Text)
        End If
        'Bamt = IIf(Bkind = Accnokind, ValComa(txtUseAmt.Text), -ValComa(txtUseAmt.Text))  '借方科目在轉帳貸方時,表示減少,反之亦同

        '資料處理(新增一筆至BGF020 & BGF030 & bgf040)
        sqlstr = Master.ADO.GenInsFunc
        Master.ADO.GenInsSql("BGNO", strBgno, "T")
        Master.ADO.GenInsSql("accyear", BYear, "N")
        Master.ADO.GenInsSql("accno", BAccno, "T")
        Master.ADO.GenInsSql("KIND", Bkind, "T")
        Master.ADO.GenInsSql("DC", IIf(Bkind = "3", "1", "2"), "T")
        Master.ADO.GenInsSql("DATE1", lblDatec.Text, "D")
        Master.ADO.GenInsSql("DATE2", lblDatec.Text, "D")
        Master.ADO.GenInsSql("REMARK", txtRemark.Text, "U")
        Master.ADO.GenInsSql("AMT1", Bamt, "N")
        Master.ADO.GenInsSql("useableAMT", Bamt, "N")
        Master.ADO.GenInsSql("subject", " ", "U")
        Master.ADO.GenInsSql("CLOSEMARK", "Y", "T")
        sqlstr = "insert into BGF020 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("新增失敗" + sqlstr)
            Exit Function
        End If

        '資料處理INSERT bgf030-
        Master.ADO.GenInsSql("BGNO", strBgno, "T")
        Master.ADO.GenInsSql("rel", 0, "N")
        Master.ADO.GenInsSql("date3", lblDatec.Text, "D")
        Master.ADO.GenInsSql("date4", lblDatec.Text, "D")
        Master.ADO.GenInsSql("USEAMT", Bamt, "N")
        Master.ADO.GenInsSql("REMARK", txtRemark.Text, "U")
        Master.ADO.GenInsSql("NO_1_NO", 0, "N")
        sqlstr = "insert into BGF030 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("資料寫入BGF030失敗" + sqlstr)
            Exit Function
        End If

        '資料處理INSERT bgf040
        Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
        Master.ADO.GenInsSql("BGCNO", lblBgcno.Text, "N")
        Master.ADO.GenInsSql("BGNO", strBgno, "T")
        Master.ADO.GenInsSql("DATEC", lblDatec.Text, "D")
        Master.ADO.GenInsSql("NO_1_NO", 0, "N")
        sqlstr = "insert into BGF040 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("資料寫入BGF040失敗" + sqlstr)
            Exit Function
        End If


        '資料處理(UPDATE BGF010->TOTuse)
        'If rdbKind1.Checked Then  'debit 
        sqlstr = "update bgf010 set totuse = totuse + " & Bamt & _
                 " WHERE accyear=" & Str(BYear) & " AND ACCNO = '" & Trim(BAccno) & "'"
        'Else
        '    sqlstr = "update bgf010 set totuse = totuse - " & Bamt & _
        '            " WHERE accyear=" & Str(BYear) & " AND ACCNO = '" & Trim(BAccno) & "'"
        'End If
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
        End If
        MessageBx("新增完成,請購編號＝" & strBgno)

        'Call LoadGrid2Func()
        blnCheck = True

        Return blnCheck
    End Function
    '修改
    Public Function UpdateData() As Boolean
        Dim blnCheck As Boolean = False

        Dim keyvalue, sqlstr, retstr, updstr, TDC As String

        keyvalue = Trim(lblkey.Text)
        

        Return blnCheck
    End Function
    '刪除
    Public Sub DeleteData()
        Dim SaveStatus As Boolean = False

        Dim keyvalue, sqlstr, retstr As String
        Dim tempds As DataSet

        keyvalue = Trim(lblkey.Text)


        Dim intNO As Integer

        '資料處理delete bgf020 & bgf030
        sqlstr = "delete from BGF020 where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("BGF020刪除失敗" + sqlstr)
            Exit Sub
        End If
        sqlstr = "delete from BGF030 where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("BGF030刪除失敗" + sqlstr)
            Exit Sub
        End If
        sqlstr = "delete from BGF040 where bgcno='" & lblBgcno.Text & "' and bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("BGF040刪除失敗" + sqlstr)
            Exit Sub
        End If

        '資料處理(UPDATE BGF010->TOTuse)
        '還原舊資料
        If lblKind.Text = "3" Then   'debit 
            sqlstr = "update bgf010 set totuse = totuse - " & Master.Models.ValComa(txtUseAmt.Text) & _
                     " WHERE accyear=" & lblYear.Text & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        Else
            sqlstr = "update bgf010 set totuse = totuse + " & Master.Models.ValComa(txtUseAmt.Text) & _
                    " WHERE accyear=" & lblYear.Text & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        End If
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
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
        Dim objDropDownList() As DropDownList = {cboAccno}
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

                lblYear.Text = Session("sYear")
                lblDatec.Text = Session("UserDate")
                Session("intBGCNO") = Master.Controller.QueryNO(CInt(Session("sYear")), "C") + 1
                lblBgcno.Text = Session("intBGCNO")
                cboAccno.Focus()
                Call LoadGrid2Func()

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
        sqlstr = "SELECT a.bgcno, a.accyear, a.datec, b.KIND, SUM(abs(c.USEAMT)) AS useamt " & _
                 "FROM BGF040 a INNER JOIN BGF020 b ON a.bgno = b.BGNO " & _
                 "INNER JOIN BGF030 c ON a.bgno = c.BGNO " & _
                 "WHERE (a.no_1_no = 0 and a.accyear=" & Session("sYear") & ") " & _
                 " and  a.bgcno = '" & strKey1 & "'  GROUP BY  a.bgcno, a.accyear, a.datec, b.KIND"

        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            lblkey.Text = Trim(objDR99("BGCNO").ToString) 'keep the old keyvalue
            lblBgcno.Text = Trim(objDR99("BGCNO").ToString)  'keep the old keyvalue
            lblDatec.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATEC").ToString))
            lblYear.Text = Trim(objDR99("ACCYEAR").ToString)
            ViewState("intBGCNO") = Trim(objDR99("BGCNO").ToString)
            LoadGrid2Func()

            cboAccno.Text = ""
            lblAccno.Text = ""
            txtRemark.Text = ""
            txtUseAmt.Text = ""
            lblUseAmt.Text = ""

            'lblBgno.Text = Trim(objDR99("BGCNO").ToString)

            'lblDatec.Text = Master.Models.ShortDate(Trim(objDR99("DATEC").ToString))

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
    Sub PutAccnoToCbo()
        Dim sqlstr As String
        sqlstr = "SELECT right('0'+ltrim(str(a.accyear)),3)" & _
                 "+left(a.accno+space(17),17) as bgf010key, " & _
                 "right('0'+ltrim(str(a.accyear)),3)+left(a.accno+space(17),17)+c.accname+'-'+" & _
                 "b.accname+str(a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4-a.totper-a.totuse) as bgf010data " & _
                 "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO " & _
                 "INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO WHERE a.ctrl<>'Y' order by bgf010key"

        Master.Controller.objDropDownListOptionEX(cboAccno, DNS_ACC, sqlstr, "bgf010key", "bgf010data", 0)


    End Sub

    Sub LoadGridFunc()
        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT * FROM (" & _
                 "SELECT a.bgcno, a.accyear, a.datec, b.KIND, SUM(abs(c.USEAMT)) AS useamt " & _
                 "FROM BGF040 a INNER JOIN BGF020 b ON a.bgno = b.BGNO " & _
                 "INNER JOIN BGF030 c ON a.bgno = c.BGNO " & _
                 "WHERE (a.no_1_no = 0 and a.accyear=" & Session("sYear") & ") GROUP BY  a.bgcno, a.accyear, a.datec, b.KIND " & _
                 ") m " & _
                 "ORDER BY " & ViewState("MyOrder") & ViewState("MySort")
        '取正數合計,借貸才能balance
        mydataset = Master.ADO.openmember(DNS_ACC, "bgf040", sqlstr)
        DataGridView.DataSource = mydataset
        DataGridView.DataBind()
    End Sub
    Sub LoadGrid2Func()
        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT b.BGNO, b.ACCYEAR, b.ACCNO, b.KIND, c.REMARK, abs(c.USEAMT) as useamt, " & _
                 "CASE WHEN len(b.accno)=17 THEN d.accname+'('+e.accname+')' " & _
                     " WHEN len(b.accno)<>17 THEN d.accname END AS accname " & _
                 "FROM BGF040 a " & _
                 "INNER JOIN BGF020 b ON a.bgno = b.BGNO " & _
                 "INNER JOIN BGF030 c ON b.BGNO = c.BGNO " & _
                 "INNER JOIN ACCNAME d ON b.ACCNO = d.ACCNO " & _
                 "LEFT OUTER JOIN accname e ON LEFT(b.ACCNO, 16) = e.ACCNO and len(b.accno)=17 " & _
                 "WHERE a.bgcno = " & ViewState("intBGCNO") & " and a.accyear=" & Session("sYear") & _
                 " ORDER BY  b.KIND"

        Master.Controller.objDataGrid(DataGrid2, lbl_GrdCount2, DNS_ACC, sqlstr, "查詢資料檔")

        TabContainer1.ActiveTabIndex = 1
    End Sub

    Sub PutGrid2ToTxt()
        'Dim intI, SumUp As Integer
        'Dim strI, strColumn1, strColumn2 As String
        'If bm2.Position < 0 Then Exit Sub
        'If IsDBNull(bm2.Current("bgno")) Then Exit Sub
        'lastpos = bm2.Position
        'lblBgno.Text = bm2.Current("bgno")   '不允許修改bgno
        'strBGNo = bm2.Current("BGNO")
        'If bm2.Current("kind") = "3" Then
        '    rdbKind1.Checked = True
        'Else
        '    rdbkind2.Checked = True
        'End If
        'lblKind.Text = bm2.Current("kind")
        'lblaccYear.Text = bm2.Current("accyear")
        'lblAccno.Text = nz(bm2.Current("accno"), " ")
        'cboAccno.SelectedValue = Mid("0" + LTrim(Str(bm2.Current("accyear"))), 1, 3) + Mid(bm2.Current("accno") + Space(17), 1, 17) '設定科目選定值
        'txtRemark.Text = nz(bm2.Current("remark"), " ")
        'txtUseAmt.Text = Format(nz(bm2.Current("useamt"), 0), "###,###,###.#")
        'lblUseAmt.Text = Format(nz(bm2.Current("useamt"), 0), "###,###,###.#")
    End Sub

    Protected Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        lblYear.Text = Session("sYear")
        lblDatec.Text = Session("UserDate")
        ViewState("intBGCNO") = Master.Controller.QueryNO(CInt(Session("sYear")), "C") + 1
        lblBgcno.Text = ViewState("intBGCNO")
        rdbKind1.Checked = True
        cboAccno.Focus()
        Call LoadGrid2Func()
    End Sub

    Protected Sub btnReflesh_Click(sender As Object, e As EventArgs) Handles btnReflesh.Click
        Call LoadGridFunc()
    End Sub

    Protected Sub btnInsert_Click(sender As Object, e As EventArgs) Handles btnInsert.Click
        Dim retstr, sqlstr, strBgno As String
        Dim intNO As Integer
        If Master.Models.ValComa(txtUseAmt.Text) <= 0 Then
            MessageBx("金額不可為零或負數")
            Exit Sub
        End If

        sqlstr = "SELECT b.BGNO, b.ACCYEAR, b.ACCNO, b.KIND, c.REMARK, abs(c.USEAMT) as useamt, " & _
         "CASE WHEN len(b.accno)=17 THEN d.accname+'('+e.accname+')' " & _
             " WHEN len(b.accno)<>17 THEN d.accname END AS accname " & _
         "FROM BGF040 a " & _
         "INNER JOIN BGF020 b ON a.bgno = b.BGNO " & _
         "INNER JOIN BGF030 c ON b.BGNO = c.BGNO " & _
         "INNER JOIN ACCNAME d ON b.ACCNO = d.ACCNO " & _
         "LEFT OUTER JOIN accname e ON LEFT(b.ACCNO, 16) = e.ACCNO and len(b.accno)=17 " & _
         "WHERE a.bgcno = " & ViewState("intBGCNO") & " and a.accyear=" & Session("sYear") & _
         " ORDER BY  b.KIND"

        myDataSet2 = Master.ADO.openmember(DNS_ACC, "bgf020", sqlstr)


        If myDataSet2.Tables("bgf020").Rows.Count = 0 Then
            ViewState("intBGCNO") = Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "C")     '當第一筆轉帳明細時,才確實取用轉帳編號
            lblBgcno.Text = ViewState("intBGCNO")
        End If

        intNO = Master.Controller.RequireNO(DNS_ACC, CInt(Session("sYear")), "B") '取得請購編號
        Dim BYear, I As Integer '請購的預算年度
        Dim BAccno, Bkind As String '請購科目
        Dim Bamt As Decimal
        BYear = Mid(cboAccno.SelectedValue, 1, 3)
        BAccno = Trim(Mid(cboAccno.SelectedValue, 4, 17))
        strBgno = Format(CInt(Session("sYear")), "000") + Format(intNO, "00000")
        Bkind = IIf(rdbKind1.Checked, "3", "4")

        Bamt = Master.Models.ValComa(txtUseAmt.Text)
        If Bkind = "3" Then  '借方
            If Mid(BAccno, 1, 1) = "3" Or Mid(BAccno, 1, 1) = "4" Then Bamt = -Master.Models.ValComa(txtUseAmt.Text)
        Else  '2負債科目亦同支出處理
            If Mid(BAccno, 1, 1) = "5" Or Mid(BAccno, 1, 1) = "1" Or Mid(BAccno, 1, 1) = "2" Then Bamt = -Master.Models.ValComa(txtUseAmt.Text)
        End If
        'Bamt = IIf(Bkind = Accnokind, ValComa(txtUseAmt.Text), -ValComa(txtUseAmt.Text))  '借方科目在轉帳貸方時,表示減少,反之亦同

        '資料處理(新增一筆至BGF020 & BGF030 & bgf040)
        sqlstr = Master.ADO.GenInsFunc
        Master.ADO.GenInsSql("BGNO", strBgno, "T")
        Master.ADO.GenInsSql("accyear", BYear, "N")
        Master.ADO.GenInsSql("accno", BAccno, "T")
        Master.ADO.GenInsSql("KIND", Bkind, "T")
        Master.ADO.GenInsSql("DC", IIf(Bkind = "3", "1", "2"), "T")
        Master.ADO.GenInsSql("DATE1", lblDatec.Text, "D")
        Master.ADO.GenInsSql("DATE2", lblDatec.Text, "D")
        Master.ADO.GenInsSql("REMARK", txtRemark.Text, "U")
        Master.ADO.GenInsSql("AMT1", Bamt, "N")
        Master.ADO.GenInsSql("useableAMT", Bamt, "N")
        Master.ADO.GenInsSql("subject", " ", "U")
        Master.ADO.GenInsSql("CLOSEMARK", "Y", "T")
        sqlstr = "insert into BGF020 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("新增失敗" + sqlstr)
            Exit Sub
        End If

        '資料處理INSERT bgf030-
        Master.ADO.GenInsSql("BGNO", strBgno, "T")
        Master.ADO.GenInsSql("rel", 0, "N")
        Master.ADO.GenInsSql("date3", lblDatec.Text, "D")
        Master.ADO.GenInsSql("date4", lblDatec.Text, "D")
        Master.ADO.GenInsSql("USEAMT", Bamt, "N")
        Master.ADO.GenInsSql("REMARK", txtRemark.Text, "U")
        Master.ADO.GenInsSql("NO_1_NO", 0, "N")
        sqlstr = "insert into BGF030 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("資料寫入BGF030失敗" + sqlstr)
            Exit Sub
        End If

        '資料處理INSERT bgf040
        Master.ADO.GenInsSql("accyear", Session("sYear"), "N")
        Master.ADO.GenInsSql("BGCNO", lblBgcno.Text, "N")
        Master.ADO.GenInsSql("BGNO", strBgno, "T")
        Master.ADO.GenInsSql("DATEC", lblDatec.Text, "D")
        Master.ADO.GenInsSql("NO_1_NO", 0, "N")
        sqlstr = "insert into BGF040 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("資料寫入BGF040失敗" + sqlstr)
            Exit Sub
        End If


        '資料處理(UPDATE BGF010->TOTuse)
        'If rdbKind1.Checked Then  'debit 
        sqlstr = "update bgf010 set totuse = totuse + " & Bamt & _
                 " WHERE accyear=" & Str(BYear) & " AND ACCNO = '" & Trim(BAccno) & "'"
        'Else
        '    sqlstr = "update bgf010 set totuse = totuse - " & Bamt & _
        '            " WHERE accyear=" & Str(BYear) & " AND ACCNO = '" & Trim(BAccno) & "'"
        'End If
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
        End If
        MessageBx("新增完成,請購編號＝" & strBgno)

        Call LoadGrid2Func()
    End Sub

    Protected Sub btnModify_Click(sender As Object, e As EventArgs) Handles btnModify.Click
        Dim retstr, sqlstr As String
        Dim intNO As Integer
        Dim mastconn As String = DNS_ACC

        If Master.Models.ValComa(txtUseAmt.Text) <= 0 Then
            MessageBx("金額不可為零或負數")
            Exit Sub
        End If
        Dim BYear, I As Integer '請購的預算年度
        Dim BAccno, Bkind, Accnokind As String '請購科目
        Dim Bamt As Decimal
        BYear = Mid(cboAccno.SelectedValue, 1, 3)
        BAccno = Trim(Mid(cboAccno.SelectedValue, 4, 17))
        Bkind = IIf(rdbKind1.Checked, "3", "4")
        Bamt = Master.Models.ValComa(txtUseAmt.Text)
        If Bkind = "3" Then  '借方
            If Mid(BAccno, 1, 1) = "3" Or Mid(BAccno, 1, 1) = "4" Then Bamt = -Master.Models.ValComa(txtUseAmt.Text)
        Else    '2負債科目亦同支出處理
            If Mid(BAccno, 1, 1) = "5" Or Mid(BAccno, 1, 1) = "2" Or Mid(BAccno, 1, 1) = "1" Then Bamt = -Master.Models.ValComa(txtUseAmt.Text)
        End If

        '資料處理(update BGF020 & BGF030)
        Master.ADO.GenUpdsql("accyear", BYear, "N")
        Master.ADO.GenUpdsql("accno", BAccno, "T")
        Master.ADO.GenUpdsql("KIND", Bkind, "T")
        Master.ADO.GenUpdsql("DC", IIf(Bkind = "3", "1", "2"), "T")
        Master.ADO.GenUpdsql("REMARK", txtRemark.Text, "U")
        Master.ADO.GenUpdsql("AMT1", Bamt, "N")
        Master.ADO.GenUpdsql("useableAMT", Bamt, "N")
        sqlstr = "update BGF020 set " & Master.ADO.genupdfunc & " where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("修改失敗" + sqlstr)
            Exit Sub
        End If

        '資料處理update bgf030-
        Master.ADO.GenUpdsql("USEAMT", Bamt, "N")
        Master.ADO.GenUpdsql("REMARK", txtRemark.Text, "U")
        sqlstr = "update BGF030 set " & Master.ADO.genupdfunc & " where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("作業完成,  " + ViewState("strBGNo"))
        Else
            MessageBx("資料update BGF030失敗" + sqlstr)
            Exit Sub
        End If


        '資料處理(UPDATE BGF010->TOTuse)
        '先還原舊資料
        If lblKind.Text = "3" Then   'debit 
            sqlstr = "update bgf010 set totuse = totuse - " & Master.Models.ValComa(txtUseAmt.Text) & _
                     " WHERE accyear=" & lblaccyear.Text & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        Else
            sqlstr = "update bgf010 set totuse = totuse + " & Master.Models.ValComa(txtUseAmt.Text) & _
                    " WHERE accyear=" & lblaccyear.Text & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        End If
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
        End If

        '再update新資料
        'If rdbKind1.Checked Then  'debit 
        sqlstr = "update bgf010 set totuse = totuse + " & Bamt & _
                 " WHERE accyear=" & Str(BYear) & " AND ACCNO = '" & Trim(BAccno) & "'"
        'Else
        '    sqlstr = "update bgf010 set totuse = totuse - " & Trim(txtUseAmt.Text) & _
        '             " WHERE accyear=" & Str(BYear) & " AND ACCNO = '" & Trim(BAccno) & "'"
        'End If
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
        End If
        Call LoadGrid2Func()
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim retstr, sqlstr As String
        Dim intNO As Integer
        Dim mastconn As String = DNS_ACC

        '資料處理delete bgf020 & bgf030
        sqlstr = "delete from BGF020 where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("BGF020刪除失敗" + sqlstr)
            Exit Sub
        End If
        sqlstr = "delete from BGF030 where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("BGF030刪除失敗" + sqlstr)
            Exit Sub
        End If
        sqlstr = "delete from BGF040 where bgcno='" & lblBgcno.Text & "' and bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("BGF040刪除失敗" + sqlstr)
            Exit Sub
        End If

        '資料處理(UPDATE BGF010->TOTuse)
        '還原舊資料
        If lblKind.Text = "3" Then   'debit 
            sqlstr = "update bgf010 set totuse = totuse - " & Master.Models.ValComa(txtUseAmt.Text) & _
                     " WHERE accyear=" & lblaccyear.Text & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        Else
            sqlstr = "update bgf010 set totuse = totuse + " & Master.Models.ValComa(txtUseAmt.Text) & _
                    " WHERE accyear=" & lblaccyear.Text & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        End If
        retstr = Master.ADO.runsql(mastconn, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
        End If
        Call LoadGrid2Func()
    End Sub
End Class