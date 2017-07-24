Imports System.Data
Imports System.Data.SqlClient

Public Class BG040
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

    Dim isArea As Boolean = False
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

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤
        txtNO.Text = "" : txtNO.Focus()

        If Session("UnitTitle").indexof("嘉南") >= 0 Then
            isArea = True
            gbxArea.Visible = True
            txtArea.Visible = True
        End If

        lblNoYear.Text = Session("sYear")
        lblSeason.Text = "第" & Session("sSeason") & "季"

        btnSure2.Enabled = True
        btnZero.Enabled = True
        'End If
        'MsgBox("單位:" & TransPara.TransP("unittitle"))

        '將單位受款人置combobox   seq=9999為受款人專用
        sqlstr = "SELECT psstr  FROM psname where unit='" & Session("UserUnit") & "' and seq=9999 order by psstr"
        


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
        sqlstr = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.date2, a.amt1, a.remark,"
        If isArea Then
            sqlstr = sqlstr & " a.amt2, a.amt3, a.useableamt, a.kind,a.subject, a.area,"
        Else
            sqlstr = sqlstr & " a.amt2, a.amt3, a.useableamt, a.kind,a.subject,"
        End If
        sqlstr = sqlstr & " CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')'" & _
                     " WHEN len(a.accno)<>17 THEN b.accname END AS accname" & _
                 " FROM BGF020 a INNER JOIN ACCNAME b ON  a.ACCNO = b.ACCNO" & _
                 " LEFT OUTER JOIN accname c ON left(a.accno,16)=c.accno and len(a.accno)=17" & _
                 " WHERE b.STAFF_NO = '" & Session("USERID") & "' AND a.CLOSEMARK <> 'Y' and a.date2 is not null" & _
                 " ORDER BY a.BGNO"

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

        sqlstr = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.date2, a.amt1, a.remark,"
        If isArea Then
            sqlstr = sqlstr & " a.amt2, a.amt3, a.useableamt, a.kind,a.subject, a.area,"
        Else
            sqlstr = sqlstr & " a.amt2, a.amt3, a.useableamt, a.kind,a.subject,"
        End If

        sqlstr = sqlstr & " CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')'" & _
                     " WHEN len(a.accno)<>17 THEN b.accname END AS accname" & _
                 " FROM BGF020 a INNER JOIN ACCNAME b ON  a.ACCNO = b.ACCNO" & _
                 " LEFT OUTER JOIN accname c ON left(a.accno,16)=c.accno and len(a.accno)=17" & _
                 " WHERE BGNO = '" & strKey1 & "' AND a.CLOSEMARK <> 'Y' and a.date2 is not null"

        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            Dim SumUp As Integer

            lblkey.Text = Trim(objDR99("BGNO").ToString)  'keep the old keyvalue
            lblBgno.Text = Trim(objDR99("BGNO").ToString)   '不允許修改bgno,accyear,accno

            lblBgamt.Text = FormatNumber(Master.Controller.QueryBGAmt(Trim(objDR99("ACCYEAR").ToString), Trim(objDR99("ACCNO").ToString)), 0)
            lblUnUseamt.Text = FormatNumber(Master.Controller.QueryUnUseAmt(Trim(objDR99("ACCYEAR").ToString), Trim(objDR99("ACCNO").ToString), Session("sSeason")), 0)
            lblUsedAmt.Text = Master.Controller.QueryUsedAmt(Trim(objDR99("BGNO").ToString))
            lblYear.Text = Trim(objDR99("ACCYEAR").ToString)
            lblAccno.Text = Trim(objDR99("ACCNO").ToString)
            lblAccname.Text = Trim(objDR99("ACCNAME").ToString)
            lblDate1.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE1").ToShortDateString.ToString))
            lblDate2.Text = Master.Models.strDateADToChiness(Trim(objDR99("DATE2").ToShortDateString.ToString))
            If Trim(objDR99("KIND").ToString) = "1" Then
                rdbKind1.Checked = True
            Else
                rdbKind2.Checked = True
            End If
            txtRemark.Text = Trim(objDR99("REMARK").ToString)
            SumUp = Master.ADO.nz(objDR99("useableamt"), 0)
            If Master.ADO.nz(objDR99("useableamt"), 0) - Master.Models.ValComa(lblUsedAmt.Text) < 0 Then
                txtUseAmt.Text = 0
            Else
                txtUseAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useableamt"), 0) - Master.Models.ValComa(lblUsedAmt.Text), 0) '開支要以可支用金額-本筆已開支額,because 可能有一次請購多次開支
            End If
            lblUseableAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useableamt"), 0), 0)
            lblAmt1.Text = FormatNumber(Master.ADO.nz(objDR99("AMT1"), 0), 0)
            lblAmt2.Text = FormatNumber(Master.ADO.nz(objDR99("AMT2"), 0), 0)
            lblAmt3.Text = FormatNumber(Master.ADO.nz(objDR99("AMT2"), 0), 0)
            txtSubject.Text = Trim(objDR99("SUBJECT").ToString)
            lblSubject.Text = Trim(objDR99("SUBJECT").ToString)
            lblkey.Text = Trim(objDR99("BGNO").ToString)
            If isArea Then txtArea.Text = Trim(objDR99("area").ToString)
            lblGrade6.Text = ""

            'KEY、異動人員及日期*****
            txtKey1.Text = Trim(objDR99("BGNO").ToString)
        End If

        objDR99.Close()    '關閉連結
        objCon99.Close()
        objCmd99.Dispose() '手動釋放資源
        objCon99.Dispose()
        objCon99 = Nothing '移除指標

    End Sub


#End Region

#Region "物件選擇異動值"
    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        Dim strRemark As String
        If Master.Models.ValComa(txtUseAmt.Text) = 0 Then
            'If MsgBox("確定本筆不開支", MsgBoxStyle.YesNo) <> MsgBoxResult.Yes Then Exit Sub
            MessageBx("開支金額不得為0，若不開支請點選 本筆不再開支")
        End If
        If Mid(lblAccno.Text, 1, 1) <> "4" Then '收入不必控制
            If Master.Models.ValComa(lblUnUseamt.Text) < Master.Models.ValComa(txtUseAmt.Text) And Session("flow") <> "Y" Then   '不可溢支
                MessageBx("本季開支餘額不足,請退回")
                Exit Sub
            End If
            If Master.Models.ValComa(lblUseableAmt.Text) < Master.Models.ValComa(txtUseAmt.Text) Then
                MessageBx("開支金額超過請購金額,請退回")
                Exit Sub
            End If
        End If
        If Mid(Trim(lblAccno.Text), 1, 5) = "21302" And Session("flow") <> "Y" Then   '代收款還要檢查已收入金額是否足夠
            sqlstr = "SELECT sum(amt) as amt  FROM BGF050 WHERE left(accno,16)='" & Mid(Trim(lblAccno.Text), 1, 16) & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
            Dim sumRtot As Decimal = Master.ADO.nz(TempDataSet.Tables("temp").Rows(0).Item(0), 0)
            sqlstr = "SELECT sum(totuse) as amt2  FROM BGF010 WHERE accyear=" & Trim(lblYear.Text) & " and left(accno,16)='" & Mid(Trim(lblAccno.Text), 1, 16) & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
            If sumRtot - Master.ADO.nz(TempDataSet.Tables("temp").Rows(0).Item(0), 0) - Master.Models.ValComa(txtUseAmt.Text) < 0 Then
                MessageBx("代收款收入不足開支,請退回")
                Exit Sub
            End If
        End If

        Call UnitEndUse() '業務單位一次開支

        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤
        txtNO.Text = "" : txtNO.Focus()
    End Sub
    Protected Sub btnSure2_Click(sender As Object, e As EventArgs) Handles btnSure2.Click
        If Master.Models.ValComa(txtUseAmt.Text) = 0 Then Exit Sub
        If Mid(lblAccno.Text, 1, 1) <> "4" Then '收入不必控制
            If Master.Models.ValComa(lblUnUseamt.Text) < Master.Models.ValComa(txtUseAmt.Text) And Session("flow") <> "Y" Then
                MessageBx("本季開支餘額不足,請退回")
                Exit Sub
            End If
            If Master.Models.ValComa(lblUseableAmt.Text) < Master.Models.ValComa(txtUseAmt.Text) + Master.Models.ValComa(lblUsedAmt.Text) Then
                MessageBx("開支金額超過請購金額,請退回")
                Exit Sub
            End If
        End If
        If Mid(Trim(lblAccno.Text), 1, 5) = "21302" And Session("flow") <> "Y" Then   '代收款還要檢查已收入金額是否足夠
            sqlstr = "SELECT sum(amt) as amt  FROM BGF050 WHERE accno='" & Mid(Trim(lblAccno.Text), 1, 16) & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
            Dim sumRtot As Decimal = Master.ADO.nz(TempDataSet.Tables("temp").Rows(0).Item(0), 0)
            sqlstr = "SELECT sum(totuse) as amt2  FROM BGF010 WHERE accyear=" & Trim(lblYear.Text) & " and accno='" & Mid(Trim(lblAccno.Text), 1, 16) & "'"
            TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
            If sumRtot - Master.ADO.nz(TempDataSet.Tables("temp").Rows(0).Item(0), 0) - Master.Models.ValComa(txtUseAmt.Text) < 0 Then
                MessageBx("代收款收入不足開支,請退回")
                Exit Sub
            End If
        End If

        If Master.Models.ValComa(lblUseableAmt.Text) = Master.Models.ValComa(txtUseAmt.Text) + Master.Models.ValComa(lblUsedAmt.Text) Then  '本次開支+已開支額=請購金額
            Call UnitEndUse() '最後一次開支
        Else
            Call UnitMidUse() '分次開支
        End If

        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤
        txtNO.Text = "" : txtNO.Focus()
    End Sub
    Protected Sub btnZero_Click(sender As Object, e As EventArgs) Handles btnZero.Click
        Dim keyvalue, sqlstr, retstr, updstr As String
        Dim BYear, intRel As Integer '請購的預算年度
        Dim BAccno, strClose As String '請購科目

        '資料處理(UPDATE BGF010->TOTuse,totPER)
        sqlstr = "update bgf010 set totper = totper - (" & _
                  Master.Models.ValComa(lblUseableAmt.Text) & " - " & Master.Models.ValComa(lblUsedAmt.Text) & ")" & _
                 " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
            Exit Sub
        End If

        '資料處理(UPDATE BGF020)
        sqlstr = "update BGF020 set closemark = 'Y' where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then

        Else
            MessageBx("更新失敗" & sqlstr)
        End If
        MessageBx("本筆不再開支，更新成功")
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
    End Sub

    Sub UnitEndUse()  '業務單位一次開支(最後一次開支)
        Dim keyvalue, sqlstr, retstr, updstr As String
        Dim BYear, intRel As Integer '請購的預算年度
        Dim BAccno, strClose As String '請購科目

        '資料處理(UPDATE BGF010->TOTuse,totPER)
        sqlstr = "update bgf010 set totper = totper - (" & _
                  Master.Models.ValComa(lblUseableAmt.Text) & " - " & Master.Models.ValComa(lblUsedAmt.Text) & ")" & _
                 ", totuse = totuse + " & Master.Models.ValComa(txtUseAmt.Text) & _
                 " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
            Exit Sub
        End If

        '資料處理(UPDATE BGF020)
        Master.ADO.GenUpdsql("CLOSEMARK", "Y", "T")
        If lblSubject.Text <> txtSubject.Text Then Master.ADO.GenUpdsql("subject", txtSubject.Text, "U")
        If isArea Then Master.ADO.GenUpdsql("area", txtArea.Text, "T")
        sqlstr = "update BGF020 set " & Master.ADO.genupdfunc & " where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
        Else
            MessageBx("更新失敗" & sqlstr)
            Exit Sub
        End If

        '資料處理(新增一筆至BGF030)
        If Val(txtUseAmt.Text) = 0 Then Exit Sub '開支金額=0   不新增BGF030
        sqlstr = "SELECT max(REL) as rel  FROM BGF030 WHERE BGNO='" & lblBgno.Text & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
        intRel = 0
        If TempDataSet.Tables("TEMP").Rows.Count > 0 And Not IsDBNull(TempDataSet.Tables("TEMP").Rows(0).Item(0)) Then
            intRel = TempDataSet.Tables("TEMP").Rows(0).Item(0)
        End If
        intRel += 1
        TempDataSet = Nothing

        sqlstr = Master.ADO.GenInsFunc  '清insert字串
        Master.ADO.GenInsSql("BGNO", lblBgno.Text, "T")
        Master.ADO.GenInsSql("rel", intRel, "N")
        Master.ADO.GenInsSql("date3", Session("UserDate"), "D")
        Master.ADO.GenInsSql("USEAMT", txtUseAmt.Text, "N")
        Master.ADO.GenInsSql("REMARK", txtRemark.Text, "U")
        Master.ADO.GenInsSql("NO_1_NO", 0, "N")
        sqlstr = "insert into BGF030 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then

        Else
            MessageBx("資料寫入BGF030失敗 " + sqlstr)
            '回復資料
            sqlstr = "update BGF020 set closemark=' ' where bgno='" & lblBgno.Text & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            'bgf010金額要扣除
            sqlstr = "update bgf010 set totper = totper + (" & _
                              Master.Models.ValComa(lblUseableAmt.Text) & " - " & Master.Models.ValComa(lblUsedAmt.Text) & ")" & _
                             ", totuse = totuse - " & Master.Models.ValComa(txtUseAmt.Text) & _
                             " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            Exit Sub
        End If

        MessageBx("開支作業完成,  " + lblBgno.Text)
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))       
    End Sub

    Sub UnitMidUse()   '業務單位分次開支
        Dim keyvalue, sqlstr, retstr, updstr As String
        Dim BYear, intRel As Integer '請購的預算年度
        Dim BAccno, strClose As String '請購科目

        '資料處理(UPDATE BGF010->TOTuse,totPER)
        sqlstr = "update bgf010 set totper = totper - " & Master.Models.ValComa(txtUseAmt.Text) & _
                 ", totuse = totuse + " & Master.Models.ValComa(txtUseAmt.Text) & _
                 " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
            Exit Sub
        End If

        '資料處理(UPDATE BGF020) 
        If lblSubject.Text <> txtSubject.Text Then
            sqlstr = "update BGF020 set SUBJECT = '" & txtSubject.Text & "' where bgno='" & lblBgno.Text & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr = "sqlok" Then
            Else
                MessageBx("更新失敗" & sqlstr)
                Exit Sub
            End If
        End If

        '資料處理(新增一筆至BGF030)   same UnitEndUse()
        sqlstr = "SELECT max(REL) as rel  FROM BGF030 WHERE BGNO='" & lblBgno.Text & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
        intRel = 0
        If TempDataSet.Tables("TEMP").Rows.Count > 0 And Not IsDBNull(TempDataSet.Tables("TEMP").Rows(0).Item(0)) Then
            intRel = TempDataSet.Tables("TEMP").Rows(0).Item(0)
        End If
        intRel += 1
        TempDataSet = Nothing

        Master.ADO.GenInsSql("BGNO", lblBgno.Text, "T")
        Master.ADO.GenInsSql("rel", intRel, "N")
        Master.ADO.GenInsSql("date3", Session("UserDate"), "D")
        Master.ADO.GenInsSql("USEAMT", txtUseAmt.Text, "N")
        Master.ADO.GenInsSql("REMARK", txtRemark.Text, "U")
        Master.ADO.GenInsSql("NO_1_NO", 0, "N")
        sqlstr = "insert into BGF030 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then

        Else
            MessageBx("資料寫入BGF030失敗" + sqlstr)
            Exit Sub
        End If
        MessageBx("分次開支作業完成,  " + lblBgno.Text)
        FillData(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤
    End Sub

    Protected Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        If txtNO.Text = "" Then Exit Sub
        Dim strBGNO As String
        Dim i As Integer
        Dim blnCheck As Boolean = False
        strBGNO = lblNoYear.Text & Format(Val(txtNO.Text), "00000")

        '是否可查詢
        blnCheck = Master.ADO.dbDataCheck(DNS_ACC, "BGF020", "BGNO = '" & strBGNO & "' AND CLOSEMARK <> 'Y' AND DATE2 IS NOT NULL")

        If blnCheck = True Then
            FindData(strBGNO)               '查詢主檔
            TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
        Else
            ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('查無請購編號【" & txtNO.Text & "】!!');", True)
            txtNO.Text = "" : txtNO.Focus()
            TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤
        End If
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

    Protected Sub btnAddSubject_Click(sender As Object, e As EventArgs) Handles btnAddSubject.Click
        If txtSubject.Text <> "" Then
            txtSubject.Text = Trim(txtSubject.Text)
            Dim ii As Integer
            ii = MsgBox("將 " & txtSubject.Text & "增入片語檔", MsgBoxStyle.OkCancel)
            If ii = 1 Then  ' click the ok botton
                sqlstr = "insert into psname (unit, seq, psstr) values ('" & Session("UserUnit") & "', 9999, '" & txtSubject.Text & "')"
                Master.ADO.runsql(DNS_ACC, sqlstr)
            End If
        End If
    End Sub





#End Region
End Class