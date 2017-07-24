Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class BGQ100_bak
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
        ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

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
        lblBgamt.Text = mydataset.Tables("BGF010").Rows(0).Item("bgamt").ToString
        lblUsed.Text = mydataset.Tables("BGF010").Rows(0).Item("totuse").ToString
        lblUnuse.Text = mydataset.Tables("BGF010").Rows(0).Item("totper").ToString
        lblNet.Text = mydataset.Tables("BGF010").Rows(0).Item("bgamt") - mydataset.Tables("BGF010").Rows(0).Item("totuse") - mydataset.Tables("BGF010").Rows(0).Item("totper")



        Select Case e.CommandName
            Case "btnShow1"

                PutGridToGrid2()
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

            'for user 

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
         "WHERE a.accyear=" & nudYear.Text & " and b.STAFF_NO = '" & Trim(ViewState("UserId")) & "' order by a.accno"
        mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        DataGrid1.DataSource = mydataset.Tables("BGF010")
        DataGrid1.DataBind()
    End Sub

    Sub PutGridToGrid2()
        '丟選定之當年度預算科目所有之開支to Grid2 
        Dim sqlstr, qstr, strD, strC As String
        Dim i, TOTBG, TOTUSE As Integer

        sqlstr = "SELECT bgf020.bgno, bgf020.accyear, BGF020.accno, bgf020.date1, bgf020.date2, bgf020.amt1, bgf020.remark, " & _
                 "bgf020.amt2, bgf020.amt3, bgf020.useableamt, ACCNAME.ACCNAME AS ACCNAME, bgf020.kind,bgf020.subject, bgf020.closemark, " & _
                 "bgf030.rel, bgf030.date3, bgf030.date4, isnull(bgf030.useamt,0) as useamt, bgf030.no_1_no,bgf030.autono, bgf030.remark as remark3 " & _
                 "FROM BGF020 left outer JOIN bgf030 on bgf020.bgno=bgf030.bgno inner join ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
                 " WHERE BGF020.ACCYEAR=" & nudYear.Text & " AND BGF020.accno='" & lblAccno1.Text & "' ORDER BY BGF020.bgno"
        mydataset = Master.ADO.openmember(DNS_ACC, "BGF030", sqlstr)
        DataGrid2.DataSource = mydataset.Tables("BGF030")
        DataGrid2.DataBind()
    End Sub







    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        TabContainer1.ActiveTabIndex = 1
    End Sub











    Protected Sub btnQbgno_Click(sender As Object, e As EventArgs) Handles btnQbgno.Click
        If txtQbgno.Text = "" Or Len(txtQbgno.Text) <> 8 Then
            MessageBx("請輸入8碼請購編號")
            Exit Sub
        End If
        sqlstr = "SELECT a.*, b.* from bgf020 a left outer join bgf030 b on a.bgno=b.bgno " & _
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
                 "bgf030.rel, bgf030.date3, bgf030.date4, isnull(bgf030.useamt,0) as useamt, bgf030.no_1_no " & _
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



    End Sub
End Class