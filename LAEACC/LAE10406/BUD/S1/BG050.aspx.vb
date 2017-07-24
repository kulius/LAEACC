Imports System.Data
Imports System.Data.SqlClient

Public Class BG050
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


        lblNoYear.Text = Session("sYear")

        If Session("UnitTitle").indexof("嘉南") < 0 Then
            lblMark.Visible = False    '嘉南才要顯示憑證補辦攔位
            txtMark.Visible = False
            cboMark.Visible = False
        Else
            '將憑證補辦片語置combobox   
            sqlstr = "SELECT psstr  FROM psname where unit='mark' order by seq"
        End If

        txtNO.Focus()
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
#End Region



#Region "@共用底層副程式@"
    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        '開啟查詢
        Dim sqlstr, qstr, strD, strC As String
        'sqlstr = "SELECT a.BGNO, a.REL, b.ACCYEAR, b.ACCNO, b.DATE1, a.DATE3, a.USEAMT, a.remark," & _
        '         " b.date2, a.autono, b.kind, b.AMT1, b.amt2, b.amt3, b.useableamt, b.subject," & _
        '         " CASE WHEN len(b.accno)=17 THEN c.accname+'('+d.accname+')'" & _
        '             " WHEN len(b.accno)<>17 THEN c.accname END AS accname" & _
        '         " FROM BGF030 a INNER JOIN BGF020 b ON a.BGNO = b.BGNO INNER JOIN ACCNAME c ON b.ACCNO = c.ACCNO " & _
        '         " LEFT OUTER JOIN accname d ON left(b.accno,16)=d.accno and len(b.accno)=17" & _
        '         " WHERE c.account_NO = '" & Session("USERID") & "' AND a.date4 is null" & _
        '         " AND b.ACCYEAR >= '" & CDbl(Now.Year) - 1911 & "'" & _
        '         " ORDER BY b.accno, a.BGNO"

        sqlstr = "SELECT a.BGNO, a.REL, b.ACCYEAR, b.ACCNO, b.DATE1, a.DATE3, a.USEAMT, a.remark," & _
         " b.date2, a.autono, b.kind, b.AMT1, b.amt2, b.amt3, b.useableamt, b.subject," & _
         " CASE WHEN len(b.accno)=17 THEN c.accname+'('+d.accname+')'" & _
             " WHEN len(b.accno)<>17 THEN c.accname END AS accname" & _
         " FROM BGF030 a INNER JOIN BGF020 b ON a.BGNO = b.BGNO INNER JOIN ACCNAME c ON b.ACCNO = c.ACCNO " & _
         " LEFT OUTER JOIN accname d ON left(b.accno,16)=d.accno and len(b.accno)=17" & _
         " WHERE (c.account_NO = '" & Session("USERID") & "' OR c.account_NO = '') AND a.date4 is null" & _
         " ORDER BY b.accno, a.BGNO"

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
            txtKey1.Text = lblBgno.Text
        End If
        Dim loadkey As String = lblBgno.Text

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

        sqlstr = "SELECT a.BGNO, a.REL, b.ACCYEAR, b.ACCNO, b.DATE1, a.DATE3, a.USEAMT, a.remark," & _
                 " b.date2, a.autono, b.kind, b.AMT1, b.amt2, b.amt3, b.useableamt, b.subject," & _
                 " CASE WHEN len(b.accno)=17 THEN c.accname+'('+d.accname+')'" & _
                     " WHEN len(b.accno)<>17 THEN c.accname END AS accname" & _
                 " FROM BGF030 a INNER JOIN BGF020 b ON a.BGNO = b.BGNO INNER JOIN ACCNAME c ON b.ACCNO = c.ACCNO " & _
                 " LEFT OUTER JOIN accname d ON left(b.accno,16)=d.accno and len(b.accno)=17" & _
                 " WHERE a.BGNO = '" & strKey1 & "' and (c.account_NO = '" & Session("USERID") & "' or c.account_NO = '') AND a.date4 is null"


        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            Dim intI, SumUp As Integer
            Dim strI, strColumn1, strColumn2 As String

            lblBgno.Text = Trim(objDR99("BGNO").ToString)   '不允許修改bgno,accyear,accno
            lblYear.Text = Trim(objDR99("accyear").ToString)
            lblAccno.Text = Trim(objDR99("accno").ToString)
            lblAccname.Text = Trim(objDR99("accname").ToString)
            lblDate1.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE1").ToShortDateString.ToString))
            lblDate2.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE2").ToShortDateString.ToString))
            lblDate3.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE3").ToShortDateString.ToString))
            If Trim(objDR99("KIND").ToString) = "1" Then
                rdbKind1.Checked = True
            Else
                rdbKind2.Checked = True
            End If
            txtRemark.Text = Trim(objDR99("REMARK").ToString)
            lblRemark.Text = txtRemark.Text  '用來判定remark是否有異動,要update remark
            txtUseAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useamt"), 0), 0)
            lblUseAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useamt"), 0), 0)
            lblUseableAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useableamt"), 0), 0)
            lblAmt1.Text = FormatNumber(Master.ADO.nz(objDR99("amt1"), 0), 0)
            lblAmt2.Text = FormatNumber(Master.ADO.nz(objDR99("AMT2"), 0), 0)
            lblAmt3.Text = FormatNumber(Master.ADO.nz(objDR99("AMT2"), 0), 0)
            txtSubject.Text = Trim(objDR99("SUBJECT").ToString)
            lblSubject.Text = Trim(objDR99("SUBJECT").ToString) '用來判定subject是否有異動,要update subject
            lblMark.Text = ""   '憑證應補事項
            lblkey.Text = Trim(objDR99("autono").ToString)
            lblBgamt.Text = FormatNumber(Master.Controller.QueryBGAmt(Trim(objDR99("ACCYEAR").ToString), Trim(objDR99("ACCNO").ToString)), 0)
            lblUnUseamt.Text = FormatNumber(Master.Controller.QueryUnUseAmt(Trim(objDR99("accyear").ToString), Trim(objDR99("accno").ToString), Session("sSeason")) + Master.Models.ValComa(lblUseAmt.Text), 0)
            lblUsedAmt.Text = FormatNumber(Master.Controller.QueryUsedAmt(Trim(objDR99("BGNO").ToString)) - Master.Models.ValComa(lblUseAmt.Text), 0) '已開支(要扣除此筆開支)
            lblGrade6.Text = ""

            'KEY、異動人員及日期*****
            txtKey1.Text = Trim(objDR99("BGNO").ToString)
        Else
            TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤
            MessageBx("查無此推算號碼!!!")
        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標

    End Sub


#End Region

#Region "物件選擇異動值"
    

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤
    End Sub

    Protected Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        If txtNO.Text = "" Then Exit Sub
        Dim strBGNO As String
        Dim i As Integer
        strBGNO = lblNoYear.Text & Format(Val(txtNO.Text), "00000")

        TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
        FindData(strBGNO)               '查詢主檔    
        txtNO.Text = "" : txtNO.Focus()
    End Sub

    Protected Sub btnGrade6_Click(sender As Object, e As EventArgs) Handles btnGrade6.Click
        '六級開支餘額查詢  
        Dim BYear, I As Integer '請購的預算年度
        Dim BAccno As String '請購科目
        BYear = lblYear.Text
        BAccno = lblAccno.Text
        If Len(BAccno) >= 7 Then
            Dim tempdataset As DataSet
            Dim sqlstr As String = ""
            If Session("sSeason") = 1 Then
                sqlstr = "SELECT sum(bg1+up1-totuse) as balance FROM bgf010 "
            End If
            If Session("sSeason") = 2 Then
                sqlstr = "SELECT sum(bg1+bg2+up1+up2-totuse) as balance FROM bgf010 "
            End If
            If Session("sSeason") = 3 Then
                sqlstr = "SELECT sum(bg1+bg2+bg3+up1+up2+up3-totuse) as balance FROM bgf010 "
            End If
            If Session("sSeason") = 4 Then
                sqlstr = "SELECT sum(bg1+bg2+bg3+bg4+up1+up2+up3+up4-totuse) as balance FROM bgf010 "
            End If
            sqlstr += " WHERE accyear=" & BYear & " and left(accno,9)='" & Mid(BAccno, 1, 9) & "'"
            tempdataset = Master.ADO.openmember(DNS_ACC, "temp", sqlstr)
            If tempdataset.Tables("temp").Rows.Count = 0 Then
                lblGrade6.Text = "0"
            Else
                lblGrade6.Text = FormatNumber(tempdataset.Tables("temp").Rows(0).Item(0), 0)
            End If
        Else
            lblGrade6.Text = "請選擇大於六級科目"
        End If
    End Sub

#End Region





    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        If Master.Models.ValComa(txtUseAmt.Text) = 0 Then Exit Sub
        Dim retstr, sqlstr As String
        If Master.Models.ValComa(txtUseAmt.Text) > Master.Models.ValComa(lblUseAmt.Text) And Mid(lblAccno.Text, 1, 1) <> "4" Then  '主計開支金額>業務單位開支金額
            If Master.Models.ValComa(txtUseAmt.Text) - Master.Models.ValComa(lblUseAmt.Text) > (Master.Models.ValComa(lblUseableAmt.Text) - Master.Models.ValComa(lblUsedAmt.Text)) Then
                MessageBx("開支金額超過請購金額,請退回")
                Exit Sub
            End If
        End If

        '資料處理updte bgf030->date4
        Master.ADO.GenUpdsql("date4", Session("UserDate"), "D")
        If txtRemark.Text <> lblRemark.Text Then Master.ADO.GenUpdsql("remark", txtRemark.Text, "U")
        If Master.Models.ValComa(lblUseAmt.Text) <> Master.Models.ValComa(txtUseAmt.Text) Then Master.ADO.GenUpdsql("useamt", txtUseAmt.Text, "N")
        If Len(Trim(txtMark.Text)) > 0 Then
            Master.ADO.GenUpdsql("MARK", txtMark.Text, "U")
        End If
        sqlstr = "update BGF030 set " & Master.ADO.genupdfunc & " where autono=" & lblkey.Text
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            'FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        Else
            MessageBx("更新bgf030失敗" & sqlstr)
            Exit Sub
        End If

        '資料處理updte bgf020->subject
        If lblSubject.Text <> txtSubject.Text Then
            sqlstr = "update BGF020 set subject = '" & txtSubject.Text & "' where bgno='" & lblBgno.Text & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("更新bgf020受款人欄失敗" & sqlstr)
                Exit Sub
            End If
        End If

        '資料處理(UPDATE BGF010->TOTuse) 
        If Master.Models.ValComa(txtUseAmt.Text) <> Master.Models.ValComa(lblUseAmt.Text) Then  '主計開支金額<>業務單位開支金額
            sqlstr = "update bgf010 set totuse = totuse + " & Master.Models.ValComa(txtUseAmt.Text) & " - " & Master.Models.ValComa(lblUseAmt.Text) & _
                     " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("預算檔bgf010更新錯誤" & sqlstr)
                Exit Sub
            End If
        End If
        'MessageBx("核准開支 成功")
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤
        txtNO.Text = ""
        txtNO.Focus()
    End Sub

    Protected Sub btnReturn_Click(sender As Object, e As EventArgs) Handles btnReturn.Click
        '退件:退至主計請購審核完成階段(業務單位未開支)
        Dim keyvalue, sqlstr, retstr, updstr As String
        Dim BYear, intRel As Integer '請購的預算年度
        Dim BAccno, strClose As String '請購科目

        '資料處理(UPDATE BGF010->TOTuse要扣除開支數,totPER要加上請購數)
        sqlstr = "update bgf010 set totper = totper + " & Master.Models.ValComa(lblAmt1.Text) & _
                 ", totuse = totuse - " & Master.Models.ValComa(lblUseAmt.Text) & _
                 " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
            Exit Sub
        End If

        '資料處理(UPDATE BGF020->closemark<>'Y' 表示未支畢)
        sqlstr = "update BGF020 set closemark = '' where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("更新BGF020失敗" & sqlstr)
            Exit Sub
        End If

        '資料處理(刪除BGF030,刪除開支資料)
        sqlstr = "DELETE FROM BGF030 WHERE autono=" & lblkey.Text
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("BGF030資料刪除失敗" + sqlstr)
            Exit Sub
        End If

        'MessageBx("退回業務單位 成功")
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤
        txtNO.Text = ""
        txtNO.Focus()
    End Sub
End Class