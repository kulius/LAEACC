Imports System.Data
Imports System.Data.SqlClient

Public Class BGF010UPD
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

        nudYear.Text = Year(Session("Userdate"))
        vxtStartNo.Text = "1"   '起值
        vxtEndNo.Text = "9"      '迄值


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


    
    Protected Sub BtnSearch_Click(sender As Object, e As EventArgs) Handles BtnSearch.Click
        Dim sqlstr, qstr, retstr, strAccno, strBookAccno As String
        Dim intJ, intI, intSum, intCount, intYear As Integer
        lblFinish1.Text = ""
        lblFinish2.Text = ""
        BtnSearch.Enabled = False



        '處理預算科目預算請購數
        intCount = 0  '計算更正預算科目個數
        sqlstr = "SELECT A.*, B.SUMAMT AS SUMAMT, B.SUMUSED as sumused FROM BGF010 A " & _
                 "LEFT OUTER JOIN (SELECT ACCNO, SUM(USEABLEAMT) AS SUMAMT, SUM(SUMUSED) AS SUMUSED " & _
                 "FROM (SELECT x.*, y.SUMUSED FROM BGF020 x LEFT OUTER JOIN " & _
                 "(SELECT BGNO, SUM(USEAMT) AS SUMUSED FROM  BGF030 GROUP BY BGNO) Y " & _
                 "ON X.BGNO = Y.BGNO WHERE x.ACCYEAR =" & nudYear.Text & " AND x.ACCNO >= '" & vxtStartNo.Text & "'" & _
                 " AND X.ACCNO<='" & vxtEndNo.Text & "' AND x.CLOSEMARK <> 'Y') C GROUP BY   ACCNO) B " & _
                 "ON A.ACCNO = B.ACCNO " & _
                 "WHERE (A.ACCYEAR =" & nudYear.Text & ") AND (A.ACCNO >= '" & vxtStartNo.Text & "' AND A.ACCNO<='" & vxtEndNo.Text & "')"

        mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        With mydataset.Tables("BGF010")
            For intI = 0 To (.Rows.Count - 1)
                intYear = .Rows(intI).Item("Accyear")
                strAccno = .Rows(intI).Item("ACCNO")
                If IsDBNull(.Rows(intI).Item("SUMAMT")) Then
                    intSum = 0
                Else
                    intSum = Master.ADO.nz(.Rows(intI).Item("SUMAMT"), 0) - Master.ADO.nz(.Rows(intI).Item("sumused"), 0) '多次開支->已開支數
                End If
                If .Rows(intI).Item("TOTPER") <> intSum Then
                    sqlstr = "update BGF010 set TOTPER = " & intSum & " where ACCYEAR=" & intYear & " and  accno='" & strAccno & "'"
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    intCount += 1
                End If
            Next
        End With
        lblFinish1.Text = "預算請購數更正完成件數=" & Str(intCount)
        mydataset.Clear()

        '處理預算科目開支數
        intCount = 0  '計算更正預算科目個數
        sqlstr = "SELECT a.ACCYEAR, a.ACCNO, a.TotUse, b.SUMAMT from bgf010 a left outer join " & _
                 "(SELECT ACCYEAR,ACCNO,SUM(USEAMT) AS SUMAMT FROM " & _
                 "(SELECT BGF020.ACCYEAR AS ACCYEAR,BGF020.ACCNO AS ACCNO,BGF030.USEAMT AS USEAMT FROM BGF030 INNER JOIN BGF020 ON BGF030.BGNO=BGF020.BGNO) derivedtbl" & _
                 " WHERE ACCYEAR=" & nudYear.Text & " AND ACCNO>='" & vxtStartNo.Text & "' AND ACCNO<='" & vxtEndNo.Text & _
                 "' group by accyear,accno) b on a.accyear=b.accyear and a.accno=b.accno " & _
                 " WHERE a.ACCYEAR=" & nudYear.Text & " AND a.ACCNO>='" & vxtStartNo.Text & "' AND a.ACCNO<='" & vxtEndNo.Text & "' ORDER BY a.ACCNO"

        mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        With mydataset.Tables("BGF010")
            For intI = 0 To (.Rows.Count - 1)
                intYear = .Rows(intI).Item("accyear")
                strAccno = .Rows(intI).Item("ACCNO")
                If IsDBNull(.Rows(intI).Item("SUMAMT")) Then
                    intSum = 0
                Else
                    intSum = .Rows(intI).Item("SUMAMT")
                End If
                If .Rows(intI).Item("TOTUSE") <> intSum Then
                    sqlstr = "update BGF010 set TOTUSE = " & intSum & " where ACCYEAR=" & intYear & " and accno='" & strAccno & "'"
                    retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
                    intCount += 1
                End If
            Next
        End With
        lblFinish2.Text = "開支數更正完成件數=" & Str(intCount)
        BtnSearch.Enabled = True
    End Sub
End Class