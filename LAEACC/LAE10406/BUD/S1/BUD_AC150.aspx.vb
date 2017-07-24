Imports System.Data
Imports System.Data.SqlClient

Public Class BUD_AC150
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
    Dim mydatasetT As DataSet

#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        ViewState("MyOrder") = "a.BGNO"  '預設排序欄位

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGrid1, "M")

        nudYear.Text = Session("sYear")
        '將科目置combobox
        sqlstr = "SELECT accno, left(accno+space(17),17)+accname as accname FROM accname where belong<>'B' order by accno"

        Master.Controller.objDropDownListOptionEX(cboAccno, DNS_ACC, sqlstr, "accno", "accname", 0)


        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤



    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGrid1, "M")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then

        End If
    End Sub

    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
#End Region
#Region "@DataGridView@"


#End Region


    Protected Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        Dim sqlstr As String
        Dim intI As Double
        Dim intD, intC As Decimal
        Dim bm As DataTable
        '先將查詢科目置datagrid
        sqlstr = "SELECT accname.accno, accname.accname, acf050.beg_debit, acf050.beg_credit, acf050.deamt12, acf050.cramt12," & _
                 " 0 as acf020d, 0 as acf020c, 0 as balanced, 0 as balancec FROM  accname inner join acf050 " & _
                 " on accname.accno=acf050.accno WHERE accname.accno like '" & cboAccno.SelectedValue & "%' " & _
                 " and acf050.accyear=" & nudYear.Text
        mydataset = Master.ADO.openmember(DNS_ACC, "acf050", sqlstr)



        bm = mydataset.Tables("acf050")

        '再由datagrid逐一計算未過帳部份
        For intI = 0 To bm.Rows.Count - 1
            'bm.Position = intI
            sqlstr = "SELECT sum(acf020.amt) as amt  FROM  acf020 inner join acf010 " & _
                     "ON ACF010.ACCYEAR = ACF020.ACCYEAR AND ACF010.NO_1_NO = ACF020.NO_1_NO AND " & _
                     "ACF010.KIND = ACF020.KIND AND ACF010.ITEM = '1' " & _
                     "WHERE acf020.accyear>=" & nudYear.Text & " and acf020.accno='" & _
                     bm.Rows(intI)("accno").ToString() & "' and acf020.dc='1' and ACF010.BOOKS <> 'Y'"
            mydatasetT = Master.ADO.openmember(DNS_ACC, "acf020", sqlstr)
            If mydatasetT.Tables("acf020").Rows.Count > 0 Then
                bm.Rows(intI)("acf020d") = IIf(IsDBNull(mydatasetT.Tables("acf020").Rows(0).Item("amt")), 0, mydatasetT.Tables("acf020").Rows(0).Item("amt"))
            End If
            sqlstr = "SELECT sum(acf020.amt) as amt  FROM  acf020 inner join acf010 " & _
                     "ON ACF010.ACCYEAR = ACF020.ACCYEAR AND ACF010.NO_1_NO = ACF020.NO_1_NO AND " & _
                     "ACF010.KIND = ACF020.KIND AND ACF010.ITEM = '1' " & _
                     "WHERE acf020.accyear>=" & nudYear.Text & " and acf020.accno='" & _
                     bm.Rows(intI)("accno").ToString() & "' and acf020.dc='2' and ACF010.BOOKS <> 'Y'"
            mydatasetT = Master.ADO.openmember(DNS_ACC, "acf020", sqlstr)
            If mydatasetT.Tables("acf020").Rows.Count > 0 Then
                bm.Rows(intI)("acf020c") = IIf(IsDBNull(mydatasetT.Tables("acf020").Rows(0).Item("amt")), 0, mydatasetT.Tables("acf020").Rows(0).Item("amt"))
            End If
        Next
        For intI = 0 To bm.Rows.Count - 1
            intD = bm.Rows(intI)("deamt12") + bm.Rows(intI)("acf020d")
            intC = bm.Rows(intI)("cramt12") + bm.Rows(intI)("acf020c")
            If intD > intC Then bm.Rows(intI)("balanced") = intD - intC
            If intC > intD Then bm.Rows(intI)("balancec") = intC - intD
        Next



        DataGrid1.DataSource = mydataset.Tables("acf050")
        DataGrid1.DataBind()
    End Sub

    Protected Sub BtnSearch1_Click(sender As Object, e As EventArgs) Handles BtnSearch1.Click
        Dim sqlstr As String
        Dim intI As Double
        Dim intD, intC As Decimal
        Dim bm As DataTable
        '先將查詢科目置datagrid
        sqlstr = "SELECT accname.accno, accname.accname, acf050.beg_debit, acf050.beg_credit, acf050.deamt12, acf050.cramt12," & _
                 " 0 as acf020d, 0 as acf020c, 0 as balanced, 0 as balancec FROM  accname inner join acf050 " & _
                 " on accname.accno=acf050.accno WHERE accname.accno like '" & vxtAccno.Text & "%' " & _
                 " and acf050.accyear=" & nudYear.Text
        mydataset = Master.ADO.openmember(DNS_ACC, "acf050", sqlstr)



        bm = mydataset.Tables("acf050")

        '再由datagrid逐一計算未過帳部份
        For intI = 0 To bm.Rows.Count - 1
            'bm.Position = intI
            sqlstr = "SELECT sum(acf020.amt) as amt  FROM  acf020 inner join acf010 " & _
                     "ON ACF010.ACCYEAR = ACF020.ACCYEAR AND ACF010.NO_1_NO = ACF020.NO_1_NO AND " & _
                     "ACF010.KIND = ACF020.KIND AND ACF010.ITEM = '1' " & _
                     "WHERE acf020.accyear>=" & nudYear.Text & " and acf020.accno='" & _
                     bm.Rows(intI)("accno").ToString() & "' and acf020.dc='1' and ACF010.BOOKS <> 'Y'"
            mydatasetT = Master.ADO.openmember(DNS_ACC, "acf020", sqlstr)
            If mydatasetT.Tables("acf020").Rows.Count > 0 Then
                bm.Rows(intI)("acf020d") = IIf(IsDBNull(mydatasetT.Tables("acf020").Rows(0).Item("amt")), 0, mydatasetT.Tables("acf020").Rows(0).Item("amt"))
            End If
            sqlstr = "SELECT sum(acf020.amt) as amt  FROM  acf020 inner join acf010 " & _
                     "ON ACF010.ACCYEAR = ACF020.ACCYEAR AND ACF010.NO_1_NO = ACF020.NO_1_NO AND " & _
                     "ACF010.KIND = ACF020.KIND AND ACF010.ITEM = '1' " & _
                     "WHERE acf020.accyear>=" & nudYear.Text & " and acf020.accno='" & _
                     bm.Rows(intI)("accno").ToString() & "' and acf020.dc='2' and ACF010.BOOKS <> 'Y'"
            mydatasetT = Master.ADO.openmember(DNS_ACC, "acf020", sqlstr)
            If mydatasetT.Tables("acf020").Rows.Count > 0 Then
                bm.Rows(intI)("acf020c") = IIf(IsDBNull(mydatasetT.Tables("acf020").Rows(0).Item("amt")), 0, mydatasetT.Tables("acf020").Rows(0).Item("amt"))
            End If
        Next
        For intI = 0 To bm.Rows.Count - 1
            intD = bm.Rows(intI)("deamt12") + bm.Rows(intI)("acf020d")
            intC = bm.Rows(intI)("cramt12") + bm.Rows(intI)("acf020c")
            If intD > intC Then bm.Rows(intI)("balanced") = intD - intC
            If intC > intD Then bm.Rows(intI)("balancec") = intC - intD
        Next



        DataGrid1.DataSource = mydataset.Tables("acf050")
        DataGrid1.DataBind()
    End Sub
End Class