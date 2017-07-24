Imports System.Data
Imports System.Data.SqlClient

Public Class BG888
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
    Dim TempDataSet, AccnoDataSet As DataSet
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


        If Session("sSeason") = 1 Then
            MessageBx("請以舊年度年底日期辦理")
            TabContainer1.Enabled = False
        End If
        lblNoYear.Text = Format(CInt(Session("sYear")), "000")
        lblSeason.Text = "第" & Session("sSeason") & "季"

        TabContainer1.Enabled = True
        '將下年度應付科目置combo
        sqlstr = "SELECT accno as accno, accno+'  '+accname as accname FROM ACCNAME " & _
                 "WHERE STAFF_NO = '" & Trim(Session("UserId")) & "' AND left(accno,5)='21202' and substring(accno,10,3)='" & _
                 Format(CInt(Session("sYear")), "000") & "' order by accno"
        AccnoDataSet = Master.ADO.openmember(DNS_ACC, "accno", sqlstr)
        If AccnoDataSet.Tables(0).Rows.Count = 0 Then
            cboAccno.Text = "尚無下年度應付科目"
            MessageBx("無下年度應付科目(2-1202-  -  -yyyxxxx),請先請主計單位建立")
        Else
            Master.Controller.objDropDownListOptionEX(cboAccno, DNS_ACC, sqlstr, "accno", "accname", 0)
        End If



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
        sqlstr = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.date2, a.amt1, a.remark," & _
                 " a.amt2, a.amt3, a.useableamt, a.kind,a.subject," & _
                 " CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')'" & _
                     " WHEN len(a.accno)<>17 THEN b.accname END AS accname" & _
                 " FROM BGF020 a INNER JOIN ACCNAME b ON  a.ACCNO = b.ACCNO" & _
                 " LEFT OUTER JOIN accname c ON left(a.accno,16)=c.accno and len(a.accno)=17" & _
                 " WHERE b.STAFF_NO = '" & Trim(Session("UserId")) & "' AND a.CLOSEMARK <> 'Y' and a.date2 is not null" & _
                 " and left(b.accno,1)='5' and a.accyear=" & Session("sYear") & " ORDER BY a.BGNO"

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

        sqlstr = "SELECT a.bgno, a.accyear, a.accno, a.date1, a.date2, a.amt1, a.remark," & _
                 " a.amt2, a.amt3, a.useableamt, a.kind,a.subject," & _
                 " CASE WHEN len(a.accno)=17 THEN b.accname+'('+c.accname+')'" & _
                     " WHEN len(a.accno)<>17 THEN b.accname END AS accname" & _
                 " FROM BGF020 a INNER JOIN ACCNAME b ON  a.ACCNO = b.ACCNO" & _
                 " LEFT OUTER JOIN accname c ON left(a.accno,16)=c.accno and len(a.accno)=17" & _
                 " WHERE b.STAFF_NO = '" & Trim(Session("UserId")) & "' AND a.CLOSEMARK <> 'Y' and a.date2 is not null" & _
                 " and left(b.accno,1)='5' and a.accyear=" & Session("sYear") & " " & _
                 " and a.bgno = '" & strKey1 & "'"


        objCmd99 = New SqlCommand(sqlstr, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            Dim intI, SumUp As Integer
            Dim strI, strColumn1, strColumn2, strBGNo As String

            lblkey.Text = Trim(objDR99("bgno").ToString) 'keep the old keyvalue
            lblBgno.Text = Trim(objDR99("bgno").ToString) '不允許修改bgno,accyear,accno
            strBGNo = Trim(objDR99("bgno").ToString)
            lblBgamt.Text = FormatNumber(Master.Controller.QueryBGAmt(objDR99("accyear").ToString, objDR99("accno").ToString), 1)
            lblunUseamt.Text = FormatNumber(Master.Controller.QueryUnUseAmt(objDR99("accyear").ToString, objDR99("accno").ToString, Session("sSeason")), 1)
            'lblUsedAmt.Text = FormatNumber(QueryUsedAmt(strBGNo), 1) '本筆已開支額
            lblYear.Text = Trim(objDR99("accyear").ToString)
            lblAccno.Text = Trim(objDR99("accno").ToString)
            lblAccname.Text = Trim(objDR99("accname").ToString)
            lblDate1.Text = Master.Models.ShortDate(Master.ADO.nz(objDR99("date1"), ""))
            lblDate2.Text = Master.Models.ShortDate(Master.ADO.nz(objDR99("date2"), ""))
            txtRemark.Text = Trim(objDR99("remark").ToString)
            txtSubject.Text = Trim(objDR99("subject").ToString)
            SumUp = Master.ADO.nz(objDR99("useableamt"), 0)
            txtUseAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useableamt"), 0))  '開支要以可支用金額 
            lblUseableAmt.Text = FormatNumber(Master.ADO.nz(objDR99("useableamt"), 0), 1)
            lblAmt1.Text = FormatNumber(Master.ADO.nz(objDR99("amt1"), 0), 1)
            lblAmt2.Text = FormatNumber(Master.ADO.nz(objDR99("amt2"), 0), 1)
            lblAmt3.Text = FormatNumber(Master.ADO.nz(objDR99("amt3"), 0), 1)
            lblkey.Text = Trim(objDR99("bgno").ToString)
            lblNewBgno.Text = Format(CInt(Session("sYear")) + 1, "000") + Format(Master.Controller.QueryNO(CInt(Session("sYear")) + 1, "B") + 1, "00000") 'vbdataio.vb 讀取new year請購編號





            txtKey1.Text = Trim(objDR99("bgno").ToString)

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
        'If ValComa(txtUseAmt.Text) = 0 Then Exit Sub
        Dim strRemark As String
        'If ValComa(txtUseAmt.Text) = 0 Then
        '    If MsgBox("確定本筆不開支", MsgBoxStyle.YesNo) <> MsgBoxResult.Yes Then Exit Sub
        'End If
        If Master.Models.ValComa(lblunUseamt.Text) < Master.Models.ValComa(txtUseAmt.Text) And Session("flow") <> "Y" Then   '不可溢支
            MessageBx("本季開支餘額不足,請退回")
            Exit Sub
        End If
        If Master.Models.ValComa(lblUseableAmt.Text) < Master.Models.ValComa(txtUseAmt.Text) And Mid(lblAccno.Text, 1, 1) <> "4" Then  '收入不必控制
            MessageBx("開支金額超過請購金額,請退回")
            Exit Sub
        End If
        Call UnitEndUse() '業務單位一次開支

        TabContainer1.ActiveTabIndex = 0
        txtNO.Text = ""
    End Sub

    Sub UnitEndUse()  '業務單位一次開支(最後一次開支)
        Dim keyvalue, sqlstr, retstr, updstr As String
        Dim intRel As Integer '開支次序
        Dim strClose As String '請購科目

        '舊年度資料處理(UPDATE BGF010->TOTuse,totPER)
        sqlstr = "update bgf010 set totper = totper - " & Master.Models.ValComa(lblUseableAmt.Text) & _
                 ", totuse = totuse + " & Master.Models.ValComa(txtUseAmt.Text) & _
                 " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("預算檔bgf010更新錯誤" & sqlstr)
            Exit Sub
        End If

        '舊年度資料處理(UPDATE BGF020)
        Master.ADO.GenUpdsql("kind", "3", "T") '轉帳借方
        Master.ADO.GenUpdsql("CLOSEMARK", "Y", "T")
        Master.ADO.GenUpdsql("subject", txtSubject.Text, "U")
        'GenInsSql("REMARK", Trim(txtRemark.Text) & "保留數轉帳", "U")
        sqlstr = "update BGF020 set " & Master.ADO.genupdfunc & " where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            'myDataSet.Tables("BGF020").Rows.RemoveAt(lastpos)
        Else
            MessageBx("更新失敗" & sqlstr)
            Exit Sub
        End If

        '舊年度資料處理(新增一筆至BGF030)
        If Val(txtUseAmt.Text) = 0 Then Exit Sub '開支金額=0   不新增BGF030
        sqlstr = "SELECT max(REL) as rel  FROM BGF030 WHERE BGNO='" & lblBgno.Text & "'"
        TempDataSet = Master.ADO.openmember(DNS_ACC, "TEMP", sqlstr)
        intRel = 0
        If TempDataSet.Tables("TEMP").Rows.Count > 0 And Not IsDBNull(TempDataSet.Tables("TEMP").Rows(0).Item(0)) Then
            intRel = TempDataSet.Tables("TEMP").Rows(0).Item(0)
        End If
        intRel += 1
        TempDataSet = Nothing

        sqlstr = Master.ADO.GenInsFunc
        Master.ADO.GenInsSql("BGNO", lblBgno.Text, "T")
        Master.ADO.GenInsSql("rel", intRel, "N")
        Master.ADO.GenInsSql("date3", Session("UserDate"), "D")
        Master.ADO.GenInsSql("date4", Session("UserDate"), "D")  '也將主計審核日填上
        Master.ADO.GenInsSql("USEAMT", txtUseAmt.Text, "N")
        Master.ADO.GenInsSql("REMARK", Trim(txtRemark.Text) & "保留數轉帳", "U")
        Master.ADO.GenInsSql("NO_1_NO", 0, "N")   '不可置0  否則會出現在開傳票畫面上,so 以1取代   '100/1/5 update = 0 
        sqlstr = "insert into BGF030 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            'MsgBox("保留作業完成,  " + strBGNo)
            MessageBx("資料寫入BGF030失敗" + sqlstr)
            Exit Sub
        End If



        '增入新年度資料
        '新年度資料處理(新增一筆至bgf010 & BGF020 & UPDATE BGF010->TOTPER)
        '取用new請購編號
        Dim intNo As Integer
        intNo = Master.Controller.RequireNO("", CInt(Session("sYear")) + 1, "B")
        Master.ADO.GenInsSql("BGNO", Format(CInt(Session("sYear")) + 1, "000") + Format(intNo, "00000"), "T")
        Master.ADO.GenInsSql("accyear", CInt(Session("sYear")) + 1, "N")
        Master.ADO.GenInsSql("accno", cboAccno.SelectedValue, "T")
        Master.ADO.GenInsSql("KIND", "2", "T")
        Master.ADO.GenInsSql("DC", "1", "T")
        Master.ADO.GenInsSql("DATE1", Session("UserDate"), "D")
        Master.ADO.GenInsSql("DATE2", Session("UserDate"), "D")
        Master.ADO.GenInsSql("REMARK", txtRemark.Text, "U")
        Master.ADO.GenInsSql("AMT1", Master.Models.ValComa(txtUseAmt.Text), "N")
        Master.ADO.GenInsSql("useableAMT", Master.Models.ValComa(txtUseAmt.Text), "N")
        Master.ADO.GenInsSql("subject", txtSubject.Text, "U")
        Master.ADO.GenInsSql("CLOSEMARK", " ", "T")
        sqlstr = "insert into BGF020 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("新增失敗" + sqlstr)
            Exit Sub
        End If

        '新增一筆至保留原因檔bgf888
        sqlstr = Master.ADO.GenInsFunc
        Master.ADO.GenInsSql("BGNO", lblBgno.Text, "T")  '記錄舊年度請購編號
        Master.ADO.GenInsSql("dateopen", txtDateopen.Text, "T")  '權責發生日
        Master.ADO.GenInsSql("dateclose", txtDateclose.Text, "T") '預計完工日
        Master.ADO.GenInsSql("reason", txtReason.Text, "T")       '保留原因
        Master.ADO.GenInsSql("newbgno", Format(CInt(Session("sYear")) + 1, "000") + Format(intNo, "00000"), "T")  '新請購編號
        sqlstr = "insert into BGF888 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)


        'insert  BGF010
        Master.ADO.GenInsSql("accyear", CInt(Session("sYear")) + 1, "N")
        Master.ADO.GenInsSql("accno", cboAccno.SelectedValue, "T")
        Master.ADO.GenInsSql("DC", "2", "T")
        Master.ADO.GenInsSql("systemdate", Format(Now(), "yyyy/MM/dd HH:mm:ss"), "D")
        sqlstr = "insert into BGF010 " & Master.ADO.GenInsFunc    '假若資料已存在,則不會insert
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        'UPDATE BGF010->TOTPER+AMT 
        sqlstr = "update BGF010 set bg1=bg1+" & Master.Models.ValComa(txtUseAmt.Text) & ", TOTPER = TOTPER + " & Master.Models.ValComa(txtUseAmt.Text) & _
                 " where ACCYEAR=" & CInt(Session("sYear")) + 1 & " AND ACCNO='" & cboAccno.SelectedValue & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        MessageBx("作業完成,請購編號=" + Format(CInt(Session("sYear")) + 1, "000") + Format(intNo, "00000"))
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        TabContainer1.ActiveTabIndex = 0   '指定Tab頁籤
    End Sub


#End Region




    Protected Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        If txtNO.Text = "" Then Exit Sub
        Dim strNO As String
        Dim i As Integer
        strNO = lblNoYear.Text & Format(Val(txtNO.Text), "00000")

        FindData(strNO)               '查詢主檔
        TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤

    End Sub
End Class