Imports System.Data
Imports System.Data.SqlClient
Imports FastReport.Export.Pdf
Imports System
Imports System.IO
Imports System.IO.File
Imports System.Web.UI
Public Class BGP020
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

        ViewState("UserId") = Session("UserId")
        BtnPrint.Enabled = False
        BtnPrint1.Enabled = False
        btnExport.Enabled = False
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤

        TabContainer1.Enabled = False
        nudYear.Text = Session("sYear")
        vxtStartNo.Text = "4"    '起值
        vxtEndNo.Text = "59"    '迄值
        If Mid(Session("UserUnit"), 1, 2) = "05" Then
            cboUser.Visible = True
            sqlstr = "SELECT b.staff_no as userid, b.staff_no+USERTABLE.USERNAME as username FROM USERTABLE right outer JOIN" & _
                    " (SELECT STAFF_NO FROM ACCNAME WHERE STAFF_NO IS NOT NULL AND STAFF_NO <> '    ' GROUP BY STAFF_NO) b " & _
                    " ON USERTABLE.USERID = b.STAFF_NO" & _
                    " WHERE USERTABLE.USERNAME IS NOT NULL" & _
                    " order by usertable.userid"

            Master.Controller.objDropDownListOptionEX(cboUser, DNS_ACC, sqlstr, "userid", "username", 0)

            cboUser.Items.Add(New ListItem("全部", "全部"))
            cboUser.SelectedIndex = -1
            cboUser.Items.FindByValue("全部").Selected = True
        End If

        Dim trigger As New PostBackTrigger()
        trigger.ControlID = BtnPrint.ID.ToString()
        trigger.ControlID = BtnPrint1.ID.ToString()
        trigger.ControlID = btnExport.ID.ToString()
        UpdatePanel1.Triggers.Add(trigger)
    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        'Dim path As String = "/LAE10406/FRPrint.aspx"
        'Me.BtnPrint.Attributes.Add("onclick", "window.open('" + path + "?id='+" & nudYear.ClientID & ".value)")
        If Not IsPostBack Then

        End If
    End Sub
    Sub LoadGridFunc()

        If Mid(Session("UserUnit"), 1, 2) = "05" Then
            ViewState("UserId") = cboUser.SelectedValue
        End If

        '丟當年度所有預算科目to Grid1 
        Dim sqlstr, qstr, strD, strC As String
        'sqlstr = "SELECT a.accno,b.accname as accname, " & _
        '         "a.bg1 as bg1, a.bg2 as bg2, a.bg3 as bg3, a.bg4 as bg4, a.bg5 as bg5 " & _
        '         "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO " & _
        '         " WHERE a.accyear=" & nudYear.Text & " and a.accno>='" & _
        '          vxtStartNo.Text & "' and a.accno<='" & vxtEndNo.Text & "'"

        sqlstr = "SELECT a.accno,CASE WHEN len(a.accno)=17 THEN c.accname+'-'+b.accname  WHEN len(a.accno)<17 THEN  b.accname END AS accname , " & _
         "a.bg1 + a.bg2 + a.bg3 + a.bg4 + a.bg5 as bg0, a.bg1 as bg1, a.bg2 as bg2, a.bg3 as bg3, a.bg4 as bg4, a.bg5 as bg5 " & _
         "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO   " & _
         " WHERE a.accyear=" & nudYear.Text & " and a.accno>='" & _
          vxtStartNo.Text & "' and a.accno<='" & vxtEndNo.Text & "'"

        If ViewState("UserId") = "全部" Then
            sqlstr = sqlstr & " order by a.accno"
        Else
            sqlstr = sqlstr & " and b.STAFF_NO = '" & Trim(ViewState("UserId")) & "' order by a.accno"
        End If
        mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        DataGridView.DataSource = mydataset
        DataGridView.DataBind()

        Dim strAccno, tAccno As String
        Dim intG As Integer
        Dim retstr As String
        Dim dBG1, dBG2, dBG3, dBG4, dBG5 As Decimal

        sqlstr = "delete BGP020 where userid='" & ViewState("UserId") & "'"    '先清空tempfile 
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("刪除BGp020失敗" & sqlstr)
            Exit Sub
        End If
        '將datagrid逐筆insert to bgp020 
        For intI As Integer = 0 To mydataset.Tables("BGF010").Rows.Count - 1
            strAccno = mydataset.Tables("BGF010").Rows(intI).Item("accno")
            dBG1 = mydataset.Tables("BGF010").Rows(intI).Item("bg1")
            dBG2 = mydataset.Tables("BGF010").Rows(intI).Item("bg2")
            dBG3 = mydataset.Tables("BGF010").Rows(intI).Item("bg3")
            dBG4 = mydataset.Tables("BGF010").Rows(intI).Item("bg4")
            dBG5 = mydataset.Tables("BGF010").Rows(intI).Item("bg5")
            Master.ADO.GenInsSql("accno", strAccno, "T")
            Master.ADO.GenInsSql("bg1", dBG1, "N")
            Master.ADO.GenInsSql("bg2", dBG2, "N")
            Master.ADO.GenInsSql("bg3", dBG3, "N")
            Master.ADO.GenInsSql("bg4", dBG4, "N")
            Master.ADO.GenInsSql("bg5", dBG5, "N")
            Master.ADO.GenInsSql("userid", ViewState("UserId"), "T")
            sqlstr = "insert into BGP020 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            intG = Master.Models.Grade(strAccno)
            For intJ As Integer = 2 To intG - 1
                If intJ = 7 Then
                    tAccno = Trim(Mid(strAccno, 1, 16))
                End If
                If intJ = 6 Then
                    tAccno = Trim(Mid(strAccno, 1, 9))
                End If
                If intJ = 5 Then
                    tAccno = Trim(Mid(strAccno, 1, 7))
                End If
                If intJ = 4 Then
                    tAccno = Trim(Mid(strAccno, 1, 5))
                End If
                If intJ = 3 Then
                    tAccno = Trim(Mid(strAccno, 1, 3))
                End If
                If intJ = 2 Then
                    tAccno = Trim(Mid(strAccno, 1, 2))
                End If
                If intJ = Master.Models.Grade(tAccno) And intJ <> intG Then
                    sqlstr = "SELECT * from BGP020 where accno='" & tAccno & "' and userid='" & ViewState("UserId") & "'"
                    TempDataSet = Master.ADO.openmember(DNS_ACC, "bgp020", sqlstr)
                    If TempDataSet.Tables("bgp020").Rows.Count = 0 Then   '原無該科目餘額則新增一筆
                        Master.ADO.GenInsSql("accno", tAccno, "T")
                        Master.ADO.GenInsSql("userid", ViewState("UserId"), "T")
                        sqlstr = "insert into bgp020 " & Master.ADO.GenInsFunc
                        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                        If retstr <> "sqlok" Then
                            MessageBx("insert  BGp020失敗" & sqlstr)
                            Exit Sub
                        End If
                    End If
                    sqlstr = "update bgp020 set bg1=bg1+" & dBG1 & ", bg2=bg2+" & dBG2 & ", bg3=bg3+" & dBG3 & _
                            ", bg4=bg4+" & dBG4 & ", bg5=bg5+" & dBG5 & _
                            " where accno='" & tAccno & "'"
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                End If
            Next
        Next

    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
#End Region
#Region "@DataGridView@"


#End Region
#Region "按鍵選項"

#End Region



#Region "@共用底層副程式@"

#End Region

#Region "@資料查詢@"


#End Region

#Region "物件選擇異動值"

#End Region



  
    Protected Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        TabContainer1.Enabled = True
        If Mid(Session("UserUnit"), 1, 2) = "05" Then
            ViewState("UserId") = cboUser.SelectedValue
        End If
        Call LoadGridFunc()
        BtnPrint.Enabled = True
        BtnPrint1.Enabled = True
        btnExport.Enabled = True
    End Sub

    Protected Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click
        'Dim path As String = Request.PhysicalApplicationPath + "LAE10406\FRPrint.aspx
        If Mid(Session("UserUnit"), 1, 2) = "05" Then
            ViewState("UserId") = cboUser.SelectedValue
        End If

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", ViewState("UserId"))    '使用者代號
        Param.Add("nudYear", nudYear.Text)  '報表年度
        Param.Add("vxtStartNo", vxtStartNo.Text)    '起始會計科目
        Param.Add("vxtEndNo", vxtEndNo.Text)    '結束會計科目
        Param.Add("nudGrade", nudGrade.Text)    '幾級科目

        Master.PrintFR("BGP020預算分配表", Session("ORG"), DNS_ACC, Param)
    End Sub

    Protected Sub BtnPrint1_Click(sender As Object, e As EventArgs) Handles BtnPrint1.Click
        If Mid(Session("UserUnit"), 1, 2) = "05" Then
            ViewState("UserId") = cboUser.SelectedValue
        End If

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", ViewState("UserId"))    '使用者代號
        Param.Add("nudYear", nudYear.Text)  '報表年度
        Param.Add("vxtStartNo", vxtStartNo.Text)    '起始會計科目
        Param.Add("vxtEndNo", vxtEndNo.Text)    '結束會計科目
        Param.Add("nudGrade", nudGrade.Text)    '幾級科目

        Master.PrintFR("BGP020預算分配表加封面", Session("ORG"), DNS_ACC, Param)
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        If Mid(Session("UserUnit"), 1, 2) = "05" Then
            ViewState("UserId") = cboUser.SelectedValue
        End If

        '丟當年度所有預算科目to Grid1 
        sqlstr = "SELECT a.accno AS 預算科目," & _
         "CASE WHEN len(a.accno)=17 THEN c.accname+'-'+b.accname  WHEN len(a.accno)<17 THEN  b.accname END AS 名稱," & _
         "Convert(Varchar(32),CONVERT(money,a.bg1 + a.bg2 + a.bg3 + a.bg4 + a.bg5),1) as 核定預算數," & _
         "Convert(Varchar(32),CONVERT(money,a.bg1),1) as 第一季分配," & _
         "Convert(Varchar(32),CONVERT(money,a.bg2),1) as 第二季分配," & _
         "Convert(Varchar(32),CONVERT(money,a.bg3),1) as 第三季分配," & _
         "Convert(Varchar(32),CONVERT(money,a.bg4),1) as 第四季分配數," & _
         "Convert(Varchar(32),CONVERT(money,a.bg5),1) as 保留數 " & _
         "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO   " & _
         " WHERE a.accyear=" & nudYear.Text & " and a.accno>='" & _
          vxtStartNo.Text & "' and a.accno<='" & vxtEndNo.Text & "'"

        If ViewState("UserId") = "全部" Then
            sqlstr = sqlstr & " order by a.accno"
        Else
            sqlstr = sqlstr & " and b.STAFF_NO = '" & Trim(ViewState("UserId")) & "' order by a.accno"
        End If

        mydataset = Master.ADO.openmember(DNS_ACC, "Export", sqlstr)
        Master.ExportDataTableToExcel(mydataset.Tables("Export"))
    End Sub
End Class