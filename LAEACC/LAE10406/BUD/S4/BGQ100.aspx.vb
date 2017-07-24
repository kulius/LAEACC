Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class BGQ100
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

    Dim TempDataSet, mydataset, mydataset2, mydataset3 As DataSet
    Dim sqlstr As String

#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        'ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

        'DataGrid*****

        Master.Controller.objDataGridStyle(DataGrid1, "M")
        Master.Controller.objDataGridStyle(DataGrid2, "S")
        Master.Controller.objDataGridStyle(DataGridQuery, "S")



        ' UCBase1.Visible = False
        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        TabContainer1.Enabled = False

        ViewState("UserId") = Session("UserId")
        nudYear.Text = Session("sYear")
        nudQyear.Text = Session("sYear")

        ViewState("UserUnit") = Session("UserUnit")
        ViewState("intUnitLen") = Len(Session("UserUnit"))


    End Sub

    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGrid1, "M")
        Master.Controller.objDataGridStyle(DataGrid2, "S")
        Master.Controller.objDataGridStyle(DataGridQuery, "S")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then

        End If
    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
    'UserControl控制項
    'Protected Sub UCBase1_Click(ByVal sender As Object, ByVal e As UCBase.ClickEventArgs) Handles UCBase1.Click
    '    'ViewState("MyStatus")：目前按鍵狀態
    '    Select Case e.CommandName
    '        Case "First"      '第一筆
    '            FlagMoveSeat(1, 0)
    '        Case "Prior"      '上一筆
    '            FlagMoveSeat(2, ViewState("ItemIndex"))
    '        Case "Next"       '下一筆
    '            FlagMoveSeat(3, ViewState("ItemIndex"))
    '        Case "Last"       '最末筆
    '            FlagMoveSeat(4, DataGridView.Items.Count - 1)

    '        Case "Save"       '存檔
    '            SaveData(ViewState("MyStatus"))
    '        Case "CancelEdit" '還原
    '            ResetData()
    '        Case "Copy"       '複製
    '            ViewState("MyStatus") = 1
    '            SetControls(3)
    '        Case "AddNew"     '新增       
    '            ViewState("MyStatus") = 1
    '            SetControls(1)
    '        Case "Edit"       '修改
    '            ViewState("MyStatus") = 2
    '            SetControls(2)
    '        Case "Delete"     '刪除
    '            DeleteData()
    '    End Select
    'End Sub
#End Region
#Region "@DataGridView@"
    '點選資訊
    Sub DataGrid2_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid2.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號


        Select Case e.CommandName
            Case "btnShow"
                '查詢資料及控制*****
                FindData(txtID.Text)               '查詢主檔
                TabContainer1.ActiveTabIndex = 2   '指定Tab頁籤
        End Select
    End Sub

    Sub DataGridQuery_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGridQuery.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號

        txtQbgno.Text = txtID.Text
        TabContainer1.ActiveTabIndex = 3
        btnGoQuery.Visible = True
        Call btnQbgno_Click(New System.Object, New System.EventArgs)


        Select Case e.CommandName
            Case "btnShow"

        End Select
    End Sub

    Sub DataGrid2_SortCommand(ByVal sender As Object, ByVal e As DataGridSortCommandEventArgs) Handles DataGrid2.SortCommand
        ViewState("MyOrder") = e.SortExpression
        ViewState("MySort") = IIf(ViewState("MySort") = "" Or ViewState("MySort") = "ASC", "DESC", "ASC")

        PutGridToGrid2(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch")) '資料查詢
    End Sub

#End Region
#Region "@DataGrid1@"

    '點選資訊
    Sub DataGrid1_ItemCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid1.ItemCommand
        '關鍵值*****
        Dim txtID As Label = e.Item.FindControl("id") '記錄編號

        sqlstr = "SELECT a.accyear,a.accno, " & _
         "CASE WHEN len(a.accno)=17 THEN c.accname+'－'+b.accname " & _
             " WHEN len(a.accno)>9 THEN d.accname+'－'+b.accname " & _
             " WHEN len(a.accno)<=9 THEN b.accname END AS accname, " & _
             "a.unit, a.bg1+a.bg2+a.bg3+a.bg4+a.bg5+a.up1+a.up2+a.up3+a.up4 as bgamt, a.totper, a.totUSE, a.ctrl, a.flow  " & _
             "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO " & _
             "INNER JOIN ACCNAME d ON LEFT(a.ACCNO, 9) = d.ACCNO " & _
             "WHERE a.accyear=" & nudYear.Text & " and b.STAFF_NO = '" & Trim(ViewState("UserId")) & "' and a.accno = '" & Trim(txtID.Text) & "'"

        mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)

        lblAccno1.Text = mydataset.Tables("BGF010").Rows(0).Item("accno").ToString
        lblAccname1.Text = mydataset.Tables("BGF010").Rows(0).Item("accname").ToString
        lblBgamt.Text = FormatNumber(mydataset.Tables("BGF010").Rows(0).Item("bgamt"), 0)
        lblUsed.Text = FormatNumber(mydataset.Tables("BGF010").Rows(0).Item("totuse"), 0)
        lblUnuse.Text = FormatNumber(mydataset.Tables("BGF010").Rows(0).Item("totper"), 0)
        lblNet.Text = FormatNumber(mydataset.Tables("BGF010").Rows(0).Item("bgamt") - mydataset.Tables("BGF010").Rows(0).Item("totuse") - mydataset.Tables("BGF010").Rows(0).Item("totper"), 0)



        Select Case e.CommandName
            Case "btnShow1"

                PutGridToGrid2(ViewState("MyOrder"), ViewState("MySort"), ViewState("MySearch"))
                TabContainer1.ActiveTabIndex = 1   '指定Tab頁籤
        End Select
    End Sub


#End Region

#Region "按鍵選項"

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

        strSQL99 = "SELECT bgf020.bgno, bgf020.accyear, BGF020.accno, bgf020.date1, bgf020.date2, bgf020.amt1, bgf020.remark, " & _
                 "bgf020.amt2, bgf020.amt3, bgf020.useableamt, ACCNAME.ACCNAME AS ACCNAME, bgf020.kind,bgf020.subject, bgf020.closemark, " & _
                 "bgf030.rel, bgf030.date3, bgf030.date4, isnull(bgf030.useamt,0) as useamt, bgf030.no_1_no,bgf030.autono, bgf030.remark as remark3 " & _
                 "FROM BGF020 left outer JOIN bgf030 on bgf020.bgno=bgf030.bgno inner join ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                 " WHERE BGF020.ACCYEAR=" & nudYear.Text & " AND BGF020.accno='" & lblAccno1.Text & "'" & _
                 " and bgf020.BGNO = '" & strKey1 & "'"


        objCmd99 = New SqlCommand(strSQL99, objCon99)
        objDR99 = objCmd99.ExecuteReader

        If objDR99.Read Then
            Dim intI, SumUp As Integer
            Dim strI, strColumn1, strColumn2 As String

            lblBgno.Text = Trim(objDR99("bgno").ToString)
            lblRel.Text = Format(Master.ADO.nz(objDR99("Rel"), 0), "##")
            lblYear.Text = Trim(objDR99("accyear").ToString)
            lblAccno.Text = Trim(objDR99("accno").ToString)
            lblAccname.Text = Trim(objDR99("accname").ToString)
            lblDate1.Text = Master.Models.strDateADToChiness(objDR99("date1").ToString)
            lblDate2.Text = Master.Models.strDateADToChiness(objDR99("date2").ToString)
            lblDate3.Text = Master.Models.strDateADToChiness(objDR99("date3").ToString)
            lblDate4.Text = Master.Models.strDateADToChiness(objDR99("date4").ToString)
            lblautono.Text = Trim(objDR99("autono").ToString)
            'for account officer
            If lblDate4.Text <> "" Then
                btnEmptyDate4.Enabled = True    '允許取消開支
                btnDelete.Enabled = True        '允許刪除
                btnEmptyDate2.Enabled = False
            Else
                btnEmptyDate4.Enabled = False
                btnDelete.Enabled = False       '不允許刪除
                If lblDate2.Text <> "" And lblDate3.Text = "" Then
                    btnEmptyDate2.Enabled = True   '允許取消審核
                Else
                    btnEmptyDate2.Enabled = False
                End If
            End If
            'for user 
            If lblDate3.Text <> "" And lblDate4.Text = "" Then
                btnEmptyDate3.Enabled = True
                btnDelete.Enabled = True        '允許刪除
            Else
                btnEmptyDate3.Enabled = False
            End If
            If Trim(objDR99("kind").ToString) = "1" Then
                rdbKind1.Checked = True
            Else
                rdbKind2.Checked = True
            End If
            lblRemark.Text = Trim(objDR99("remark").ToString)
            lblRemark3.Text = Trim(objDR99("remark3").ToString)
            lblUseAmt.Text = Format(Master.ADO.nz(objDR99("useamt"), 0), "###,###,###")  '開支要金額
            lblAmt1.Text = Format(Master.ADO.nz(objDR99("amt1"), 0), "###,###,###")
            lblAmt2.Text = Format(Master.ADO.nz(objDR99("amt2"), 0), "###,###,###")
            lblAmt3.Text = Format(Master.ADO.nz(objDR99("amt3"), 0), "###,###,###")
            lblSubject.Text = Trim(objDR99("SUBJECT").ToString)
            lblNo_1_no.Text = Format(Master.ADO.nz(objDR99("no_1_no"), 0), "#####")
            lblkey.Text = Trim(objDR99("bgno").ToString)
            txtRemark.Text = lblRemark.Text
            txtRemark3.Text = lblRemark3.Text
            txtSubject.Text = lblSubject.Text
            txtNo_1_no.Text = lblNo_1_no.Text


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

#End Region



    Protected Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        TabContainer1.Enabled = True
        Call LoadGridFunc()

    End Sub

    Sub LoadGridFunc()
        '丟當年度所有預算科目to Grid1 
        Dim sqlstr, qstr, strD, strC As String

        sqlstr = "SELECT a.accyear,a.accno, " & _
                 "CASE WHEN len(a.accno)=17 THEN c.accname+'－'+b.accname " & _
                     " WHEN len(a.accno)>9 THEN d.accname+'－'+b.accname " & _
                     " WHEN len(a.accno)<=9 THEN b.accname END AS accname, " & _
         "a.unit, a.bg1+a.bg2+a.bg3+a.bg4+a.bg5+a.up1+a.up2+a.up3+a.up4 as bgamt, a.totper, a.totUSE, a.ctrl, a.flow  " & _
         "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO " & _
         "INNER JOIN ACCNAME d ON LEFT(a.ACCNO, 9) = d.ACCNO " & _
         "WHERE a.accyear=" & nudYear.Text & " and left(b.unit," & ViewState("intUnitLen") & ") = '" & Trim(ViewState("UserUnit")) & "' order by a.accno"

        mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        DataGrid1.DataSource = mydataset.Tables("BGF010")
        DataGrid1.DataBind()
    End Sub

    Sub PutGridToGrid2(ByVal strOrder As String, Optional ByVal strSortType As String = "ASC", Optional ByVal strSearch As String = "")
        '丟選定之當年度預算科目所有之開支to Grid2 
        Dim sqlstr, qstr, strD, strC As String
        Dim i, TOTBG, TOTUSE As Integer

        sqlstr = "SELECT bgf020.bgno, bgf020.accyear, BGF020.accno, bgf020.date1, bgf020.date2, bgf020.amt1, bgf020.remark, " & _
                 "bgf020.amt2, bgf020.amt3, bgf020.useableamt, ACCNAME.ACCNAME AS ACCNAME, bgf020.kind,bgf020.subject, bgf020.closemark, " & _
                 "bgf030.rel, bgf030.date3, bgf030.date4, isnull(bgf030.useamt, 0) as useamt, bgf030.no_1_no, bgf030.autono, bgf030.remark as remark3 " & _
                 "FROM BGF020 left outer JOIN bgf030 on bgf020.bgno=bgf030.bgno inner join ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                 " WHERE BGF020.ACCYEAR=" & nudYear.Text & " AND BGF020.accno='" & lblAccno1.Text & "' "

        sqlstr += IIf(strOrder = "", " ORDER BY BGF020.bgno", " ORDER BY " & strOrder & " " & strSortType)

        mydataset = Master.ADO.openmember(DNS_ACC, "BGF030", sqlstr)
        DataGrid2.DataSource = mydataset.Tables("BGF030")
        DataGrid2.DataBind()
    End Sub

    Protected Sub btnEmptyDate2_Click(sender As Object, e As EventArgs) Handles btnEmptyDate2.Click
        Dim retstr As String
        sqlstr = "update BGF020 set date2 = null where bgno='" & lblkey.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消主計審核" & lblkey.Text)
        Else
            MessageBx("取消主計審核,更新bgf020失敗" & sqlstr)
        End If
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnEmptyDate4_Click(sender As Object, e As EventArgs) Handles btnEmptyDate4.Click
        Dim retstr As String
        sqlstr = "update BGF030 set date4 = null where bgno='" & lblkey.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消主計開支" & lblkey.Text)
        Else
            MessageBx("取消主計開支更新bgf030失敗" & sqlstr)
        End If
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnModAmt_Click(sender As Object, e As EventArgs) Handles btnModAmt.Click
        If Master.Models.ValComa(lblUseAmt.Text) = 0 Then
            MessageBx("未開支資料,不開放修改")
            Exit Sub
        End If
        gbxModAmt.Visible = True
        txtUseAmt.Text = FormatNumber(Master.Models.ValComa(lblUseAmt.Text), 0)
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        TabContainer1.ActiveTabIndex = 1
    End Sub

    Protected Sub btnEmptyDate3_Click(sender As Object, e As EventArgs) Handles btnEmptyDate3.Click
        Dim retstr As String
        sqlstr = "update BGF020 set closemark = '' where bgno='" & lblkey.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("取消單位開支更新bgf020失敗" & sqlstr)
            Exit Sub
        End If

        sqlstr = "update BGF010 set totuse = totuse - " & Master.Models.ValComa(lblUseAmt.Text) & ", totper=totper + " & Master.Models.ValComa(lblAmt1.Text) & _
                 " where accyear=" & lblYear.Text & " and accno='" & lblAccno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("取消單位開支更新bgf010失敗" & sqlstr)
            Exit Sub
        End If

        sqlstr = "delete from BGF030 where autono=" & lblautono.Text
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消單位開支" & lblkey.Text)
        Else
            MessageBx("取消單位開支更新bgf030失敗" & sqlstr)
        End If
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim retstr As String
        sqlstr = "update BGF010 set totuse = totuse - " & Master.Models.ValComa(lblUseAmt.Text) & _
                 " where accyear=" & lblYear.Text & " and accno='" & lblAccno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("取消單位開支更新bgf010失敗" & sqlstr)
        sqlstr = "select count(*) as cnt from bgf030 where bgno='" & lblkey.Text & "'"  '計算開支有幾筆
        TempDataSet = Master.ADO.openmember(DNS_ACC, "BGF030", sqlstr)
        If TempDataSet.Tables("bgf030").Rows(0).Item(0) > 1 Then   '多筆開支
            sqlstr = "update BGF020 set closemark = '', useableamt = useableamt + " & Master.Models.ValComa(lblUseAmt.Text) & _
                     " where bgno='" & lblkey.Text & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then MessageBx("更新bgf020失敗" & sqlstr)
        Else
            sqlstr = "delete from BGF020 where bgno='" & lblkey.Text & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then MessageBx("刪除bgf020 失敗" & sqlstr)
        End If
        sqlstr = "delete from BGF030 where autono=" & lblautono.Text
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已刪除完成 " & lblkey.Text)
        Else
            MessageBx("刪除bgf030失敗 " & sqlstr)
        End If
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnModAccno_Click(sender As Object, e As EventArgs) Handles btnModAccno.Click
        If Master.Models.ValComa(lblUseAmt.Text) = 0 Then
            MessageBx("未開支資料,不開放修改")
            Exit Sub
        End If
        gbxModAccno.Visible = True
        txtAccno.Text = lblAccno.Text
    End Sub

    Protected Sub btnModOther_Click(sender As Object, e As EventArgs) Handles btnModOther.Click
        Dim retstr As String = ""
        If Trim(lblRemark.Text) <> Trim(txtRemark.Text) Or Trim(lblSubject.Text) <> Trim(txtSubject.Text) Then
            If Trim(lblRemark.Text) <> Trim(txtRemark.Text) Then Master.ADO.GenUpdsql("remark", txtRemark.Text, "U")
            If Trim(lblSubject.Text) <> Trim(txtSubject.Text) Then Master.ADO.GenUpdsql("subject", txtSubject.Text, "U")
            sqlstr = "update BGF020 set " & Master.ADO.genupdfunc & " where bgno='" & lblkey.Text & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr = "sqlok" Then
                MessageBx("更新完成")
            End If
        End If
        If Trim(lblRemark3.Text) <> Trim(txtRemark3.Text) Or Master.Models.ValComa(lblNo_1_no.Text) <> Master.Models.ValComa(txtNo_1_no.Text) Then
            If Master.Models.ValComa(lblautono.Text) <> 0 Then  '已開支
                If Trim(lblRemark3.Text) <> Trim(txtRemark3.Text) Then Master.ADO.GenUpdsql("remark", txtRemark3.Text, "U")
                'If Trim(lblSubject.Text) <> Trim(txtSubject.Text) Then GenUpdsql("subject", txtSubject.Text, "U")
                If Master.Models.ValComa(lblNo_1_no.Text) <> Master.Models.ValComa(txtNo_1_no.Text) Then Master.ADO.GenUpdsql("NO_1_NO", txtNo_1_no.Text, "N")
                sqlstr = "update BGF030 set " & Master.ADO.genupdfunc & " where autono=" & lblautono.Text
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            End If
        End If
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnSureAccno_Click(sender As Object, e As EventArgs) Handles btnSureAccno.Click
        Dim retstr As String
        sqlstr = "select * from accname where accno='" & txtAccno.Text & "'"
        mydataset2 = Master.ADO.openmember(DNS_ACC, "accname", sqlstr)
        If mydataset2.Tables(0).Rows.Count = 0 Then
            '    If MsgBox(txtAccno.Text & Master.ADO.nz(mydataset2.Tables(0).Rows(0).Item("accname"), "") & vbCrLf & "是否正確?", MsgBoxStyle.YesNo) <> MsgBoxResult.Yes Then
            '        Exit Sub
            '    End If
            'Else
            MessageBx("無此科目" & txtAccno.Text)
            Exit Sub
        End If
        gbxModAccno.Visible = False
        TabContainer1.ActiveTabIndex = 0
        txtAccno.Text = Replace(txtAccno.Text, "-", "")
        lblAccno.Text = Replace(lblAccno.Text, "-", "")
        If lblAccno.Text = txtAccno.Text Then
            Exit Sub
        End If
        sqlstr = "update BGF020 set accno='" & txtAccno.Text & "' where bgno='" & lblBgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("修改bgf020錯誤" & sqlstr)
            Exit Sub
        End If
        sqlstr = "update bgf010 set totuse = totuse - " & Master.Models.ValComa(lblUseAmt.Text) & _
                 " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("bgf010原科目開支累計數扣除錯誤,請檢查" & sqlstr)
        End If
        sqlstr = "update bgf010 set totuse = totuse + " & Master.Models.ValComa(lblUseAmt.Text) & _
                 " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(txtAccno.Text) & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("bgf010新科目開支累計數加入錯誤,請檢查" & sqlstr)
        End If

        MessageBx("修改完成")
    End Sub

    Protected Sub btnCancelAccno_Click(sender As Object, e As EventArgs) Handles btnCancelAccno.Click
        gbxModAccno.Visible = False
    End Sub

    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        gbxModAmt.Visible = False
        TabContainer1.ActiveTabIndex = 0
        If Master.Models.ValComa(txtUseAmt.Text) = Master.Models.ValComa(lblUseAmt.Text) Then
            Exit Sub
        End If
        Dim retstr As String
        If Master.Models.ValComa(lblUseAmt.Text) <> Master.Models.ValComa(txtUseAmt.Text) Then
            sqlstr = "update BGF030 set useamt=" & Master.Models.ValComa(txtUseAmt.Text) & " where autono=" & lblautono.Text
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            sqlstr = "update bgf010 set totuse = totuse + " & Master.Models.ValComa(txtUseAmt.Text) & " - " & Master.Models.ValComa(lblUseAmt.Text) & _
             " WHERE accyear=" & Trim(lblYear.Text) & " AND ACCNO = '" & Trim(lblAccno.Text) & "'"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            If retstr <> "sqlok" Then
                MessageBx("預算檔bgf010更新錯誤" & sqlstr)
            End If
        End If
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        gbxModAmt.Visible = False
    End Sub

    Protected Sub btnQbgno_Click(sender As Object, e As EventArgs) Handles btnQbgno.Click
        If txtQbgno.Text = "" Or Len(txtQbgno.Text) <> 8 Then
            MessageBx("請輸入8碼請購編號")
            Exit Sub
        End If
        sqlstr = "SELECT a.*, b.*,b.autono as bgf030autono from bgf020 a left outer join bgf030 b on a.bgno=b.bgno " & _
                 " where a.bgno='" & txtQbgno.Text & "'"
        mydataset2 = Master.ADO.openmember(DNS_ACC, "BGF020", sqlstr)
        If mydataset2.Tables(0).Rows.Count > 0 Then
            With mydataset2.Tables(0)
                lblQyear.Text = Master.ADO.nz(.Rows(0).Item("accyear"), 0)
                lblQAccno.Text = Master.ADO.nz(.Rows(0).Item("accno"), "")
                lblQRemark.Text = Master.ADO.nz(.Rows(0).Item("remark"), "")
                lblQAmt1.Text = FormatNumber(Master.ADO.nz(.Rows(0).Item("amt1"), 0), 0)
                lblQAmt2.Text = FormatNumber(Master.ADO.nz(.Rows(0).Item("amt2"), 0), 0)
                lblQAmt3.Text = FormatNumber(Master.ADO.nz(.Rows(0).Item("amt3"), 0), 0)
                lblQUseAmt.Text = FormatNumber(Master.ADO.nz(.Rows(0).Item("useamt"), 0), 0)
                lblQDate1.Text = Master.Models.ShortDate(Master.ADO.nz(.Rows(0).Item("date1"), ""))
                lblQDate2.Text = Master.Models.ShortDate(Master.ADO.nz(.Rows(0).Item("date2"), ""))
                lblQDate3.Text = Master.Models.ShortDate(Master.ADO.nz(.Rows(0).Item("date3"), ""))
                lblQDate4.Text = Master.Models.ShortDate(Master.ADO.nz(.Rows(0).Item("date4"), ""))
                lblQautono.Text = Master.ADO.nz(.Rows(0).Item("bgf030autono"), "")
            End With
            sqlstr = "SELECT a.*, b.USERNAME FROM ACCNAME a LEFT OUTER JOIN USERTABLE b ON a.STAFF_NO = b.USERID WHERE a.ACCNO ='" & lblQAccno.Text & "'"
            mydataset2 = Master.ADO.openmember(DNS_ACC, "BGF020", sqlstr)
            If mydataset2.Tables(0).Rows.Count > 0 Then
                lblQUserId.Text = Master.ADO.nz(mydataset2.Tables(0).Rows(0).Item("username"), "")
                lblQAccname.Text = Master.ADO.nz(mydataset2.Tables(0).Rows(0).Item("accname"), "")
            End If
        End If
    End Sub

    Protected Sub btnGoQuery_Click(sender As Object, e As EventArgs) Handles btnGoQuery.Click
        TabContainer1.ActiveTabIndex = 4
    End Sub

    Protected Sub btnQsearch_Click(sender As Object, e As EventArgs) Handles btnQsearch.Click
        'If Trim(txtQremark.Text) = "" And Master.Models.ValComa(txtQamt.Text) = 0 Then Exit Sub
        sqlstr = "SELECT bgf020.bgno, bgf020.accyear, BGF020.accno, bgf020.date1, bgf020.date2, bgf020.amt1, bgf020.remark, " & _
                 "bgf020.amt2, bgf020.amt3, bgf020.useableamt, ACCNAME.ACCNAME AS ACCNAME, bgf020.kind,bgf020.subject, bgf020.closemark, " & _
                 "bgf030.rel, bgf030.date3, bgf030.date4, isnull(bgf030.useamt, 0) as useamt, bgf030.no_1_no " & _
                 "FROM BGF020 left outer JOIN bgf030 on bgf020.bgno=bgf030.bgno inner join ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                 " WHERE BGF020.ACCYEAR=" & nudQyear.Text
        If Trim(txtQremark.Text) <> "" Then
            sqlstr = sqlstr & " AND BGF020.remark like '%" & Trim(txtQremark.Text) & "%'"
        End If
        If Master.Models.ValComa(txtQamt.Text) <> 0 Then
            sqlstr = sqlstr & " AND (BGF020.amt1=" & Master.Models.ValComa(txtQamt.Text) & " or bgf030.useamt=" & Master.Models.ValComa(txtQamt.Text) & ")"
        End If
        If Mid(Session("Userunit"), 1, 2) <> "05" Then sqlstr += " and accname.staff_no='" & Session("Userid") & "'"
        sqlstr += " ORDER BY BGF020.bgno"
        mydataset3 = Master.ADO.openmember(DNS_ACC, "BGF030", sqlstr)

        DataGridQuery.DataSource = mydataset3.Tables("BGF030")
        DataGridQuery.DataBind()

    End Sub

    Protected Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click

        btnPrint1_Click(Nothing, Nothing)
    End Sub

    Protected Sub btnEmptyDate_Click(sender As Object, e As EventArgs) Handles btnEmptyDate.Click
        '4
        Dim retstr As String
        sqlstr = "update BGF030 set date4 = null where bgno='" & lblkey.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消主計開支" & lblkey.Text)
        Else
            MessageBx("取消主計開支更新bgf030失敗" & sqlstr)
        End If

        '3

        sqlstr = "update BGF020 set closemark = '' where bgno='" & lblkey.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("取消單位開支更新bgf020失敗" & sqlstr)
            Exit Sub
        End If

        sqlstr = "update BGF010 set totuse = totuse - " & Master.Models.ValComa(lblUseAmt.Text) & ", totper=totper + " & Master.Models.ValComa(lblAmt1.Text) & _
                 " where accyear=" & lblYear.Text & " and accno='" & lblAccno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("取消單位開支更新bgf010失敗" & sqlstr)
            Exit Sub
        End If

        sqlstr = "delete from BGF030 where autono=" & lblautono.Text
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消單位開支" & lblkey.Text)
        Else
            MessageBx("取消單位開支更新bgf030失敗" & sqlstr)
        End If

        '2
        sqlstr = "update BGF020 set date2 = null where bgno='" & lblkey.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消主計審核" & lblkey.Text)
        Else
            MessageBx("取消主計審核,更新bgf020失敗" & sqlstr)
        End If

        Call LoadGridFunc()

        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnQEmptyDate_Click(sender As Object, e As EventArgs) Handles btnQEmptyDate.Click
        '4
        Dim retstr As String
        sqlstr = "update BGF030 set date4 = null where bgno='" & lblkey.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消主計開支" & lblkey.Text)
        Else
            MessageBx("取消主計開支更新bgf030失敗" & sqlstr)
        End If

        '3

        sqlstr = "update BGF020 set closemark = '' where bgno='" & txtQbgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("取消單位開支更新bgf020失敗" & sqlstr)
            Exit Sub
        End If

        sqlstr = "update BGF010 set totuse = totuse - " & Master.Models.ValComa(lblQUseAmt.Text) & ", totper=totper + " & Master.Models.ValComa(lblQAmt1.Text) & _
                 " where accyear=" & lblQyear.Text & " and accno='" & lblQAccno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("取消單位開支更新bgf010失敗" & sqlstr)
            Exit Sub
        End If

        sqlstr = "delete from BGF030 where autono=" & lblQautono.Text
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消單位開支" & lblQautono.Text)
        Else
            MessageBx("取消單位開支更新bgf030失敗" & sqlstr)
        End If

        '2
        sqlstr = "update BGF020 set date2 = null where bgno='" & txtQbgno.Text & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr = "sqlok" Then
            MessageBx("已取消主計審核" & txtQbgno.Text)
        Else
            MessageBx("取消主計審核,更新bgf020失敗" & sqlstr)
        End If

        Call LoadGridFunc()

        Call btnQbgno_Click(sender, e)

        TabContainer1.ActiveTabIndex = 4
    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs) Handles btnExcel.Click
        '丟選定之當年度預算科目所有之開支to Grid2 
        Dim sqlstr, qstr, strD, strC As String
        Dim i, TOTBG, TOTUSE As Integer

        sqlstr = "SELECT bgf020.bgno as 請購編號, bgf020.accyear as 年度, BGF020.accno as 科目,ACCNAME.ACCNAME AS 科目名稱, " & _
                 " bgf020.amt1 as 請購金額, isnull(bgf030.useamt, 0) as 開支金額, bgf020.remark as 摘要, bgf020.kind as 收支,bgf020.subject as 受款人,  " & _
                 " RIGHT('0'+CAST(CONVERT(CHAR(8),bgf020.date1,112)-19110000 AS VARCHAR(8)),7) as 請購日期 ,RIGHT('0'+CAST(CONVERT(CHAR(8),bgf020.date2,112)-19110000 AS VARCHAR(8)),7) as 主計審核 ,RIGHT('0'+CAST(CONVERT(CHAR(8),bgf030.date3,112)-19110000 AS VARCHAR(8)),7) as 單位開支,RIGHT('0'+CAST(CONVERT(CHAR(8),bgf030.date4,112)-19110000 AS VARCHAR(8)),7) as 主計開支 , bgf030.no_1_no as 傳票編號 " & _
                 " FROM BGF020 left outer JOIN bgf030 on bgf020.bgno=bgf030.bgno inner join ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                 " WHERE BGF020.ACCYEAR=" & nudYear.Text & " AND BGF020.accno='" & lblAccno1.Text & "' ORDER BY BGF020.bgno"
        mydataset = Master.ADO.openmember(DNS_ACC, "BGF030", sqlstr)



        Dim excel As New ExcelPackage()
        Dim sheet As ExcelWorksheet = excel.Workbook.Worksheets.Add("sheet1")

        sheet.Cells("A1").LoadFromDataTable(mydataset.Tables("BGF030"), True)
        sheet.Cells.Style.Font.Size = 11



        Dim ms As New MemoryStream()
        excel.SaveAs(ms)
        Dim fileName As String = HttpUtility.UrlEncode("Excel.xlsx")

        Response.ContentType = "application/vnd.ms-excel"
        'Response.ContentType = "application/download"; //也可以设置成download    
        Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", fileName))

        Response.Buffer = True
        Response.Clear()
        Response.BinaryWrite(ms.GetBuffer())
        Response.[End]()

    End Sub

    Protected Sub btnBack1_Click(sender As Object, e As EventArgs) Handles btnBack1.Click
        TabContainer1.ActiveTabIndex = 0
    End Sub

    Protected Sub btnPrint1_Click(sender As Object, e As EventArgs) Handles btnPrint1.Click
        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)

        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", ViewState("UserId"))    '使用者代號
        Param.Add("nudYear", nudYear.Text)    '起始會計科目
        Param.Add("vxtStartNo", lblAccno1.Text)    '起始會計科目
        Param.Add("vxtEndNo", lblAccno1.Text)    '結束會計科目


        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期


        Master.PrintFR("BGQ010推算簿列印", Session("ORG"), DNS_ACC, Param)
    End Sub
End Class