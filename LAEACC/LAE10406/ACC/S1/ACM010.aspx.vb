Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class ACM010
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
    Dim strSQL99, retstr As String
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
    Dim mydataset, mydataset2, myDatasetS, myDatasetT As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱

        dtpDateS.Text = Session("LastDay")

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

    Protected Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click
        Session("LastDay") = dtpDateS.Text

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("SUnitTitle", Session("SUnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號

        Param.Add("dtpDateS", Master.Models.strDateChinessToAD(dtpDateS.Text))    '結束會計科目

        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "00")  '本月
        Dim up As String = Format(Month(dtpDate) - 1, "00") '上月
        Dim dd As String = DateTime.DaysInMonth(dtpDate.Year, dtpDate.Month)

        Param.Add("SYear", SYear)
        Param.Add("mm", mm)
        Param.Add("up", up)
        Param.Add("dd", dd)


        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        LoadGridFunc()

        Master.PrintFR("ACM010總分類帳彙總表", Session("ORG"), DNS_ACC, Param)
    End Sub

    Sub LoadGridFunc()
        Dim sqlstr, retstr, tempStr As String
        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "00")  '本月
        Dim up As String = Format(Month(dtpDate) - 1, "00") '上月

        '先清空acm010 
        sqlstr = "delete from acm010"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '將acf050四級科目丟入acm010
        If up = "00" Then
            tempStr = " a.beg_debit AS amt1, a.beg_credit AS amt2, "
        Else
            tempStr = " a.DEAMT" & up & " AS amt1, a.CRAMT" & up & " AS amt2, "
        End If

        sqlstr = "INSERT INTO ACM010  SELECT a.ACCNO, b.ACCNAME," & tempStr & _
                 " a.DEAMT" & mm & " AS amt3, a.CRAMT" & mm & " AS amt4, " & _
                 " a.DEAMT" & mm & " AS amt5, a.CRAMT" & mm & " AS amt6, 0 AS amt7 " & _
                 "FROM ACF050 a left outer join ACCNAME b ON a.ACCNO = b.ACCNO " & _
                 "WHERE a.ACCYEAR = " & SYear & " AND LEN(a.ACCNO) = 5"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '各欄數值計算(本月總額)
        sqlstr = " Update ACM010 SET amt3 = amt3 - amt1, amt4 = amt4 - amt2"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("update acm010 error " & sqlstr)
        '各欄數值計算(本月餘額)
        sqlstr = "Update ACM010 SET amt5 = amt5 - amt6, amt6 = 0 WHERE amt5 > amt6"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        sqlstr = "Update ACM010 SET amt6 = amt6 - amt5, amt5 = 0 WHERE amt6 > amt5"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        sqlstr = "Update ACM010 SET amt6 = 0, amt5 = 0 WHERE amt6 = amt5"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        '各欄數值計算(上月餘額)
        sqlstr = "Update ACM010 SET amt2 = amt2 - amt1, amt1 = 0 WHERE amt2 > amt1"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        sqlstr = "Update ACM010 SET amt1 = amt1 - amt2, amt2 = 0 WHERE amt1 > amt2"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        sqlstr = "Update ACM010 SET amt1 = 0, amt2 = 0 WHERE amt1 = amt2"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("update acm010 error " & sqlstr)

        '統計三級
        sqlstr = "INSERT INTO ACM010 select a.accno, b.accname, a.amt1, a.amt2, a.amt3, " & _
                 "a.amt4, a.amt5, a.amt6, a.amt7 from " & _
                 "(SELECT substring(accno, 1, 3) AS accno, SUM(amt1) AS amt1, " & _
                 "SUM(amt2) AS amt2, SUM(amt3) AS amt3, SUM(amt4) AS amt4, " & _
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, 0 AS amt7 from acm010 " & _
                 "GROUP BY substring(accno, 1, 3)) a left outer join ACCNAME b ON a.accno = b.ACCNO"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("統計三級 error " & sqlstr)

        '統計二級
        sqlstr = "INSERT INTO ACM010 select a.accno, b.accname, a.amt1, a.amt2, a.amt3, " & _
                 "a.amt4, a.amt5, a.amt6, a.amt7 from " & _
                 "(SELECT substring(accno, 1, 2) AS accno, SUM(amt1) AS amt1, " & _
                 "SUM(amt2) AS amt2, SUM(amt3) AS amt3, SUM(amt4) AS amt4, " & _
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, 0 AS amt7 from acm010 " & _
                 "where len(accno)=3 " & _
                 "GROUP BY substring(accno, 1, 2)) a left outer join ACCNAME b ON a.accno = b.ACCNO"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("統計二級 error " & sqlstr)
        '統計一級
        sqlstr = "INSERT INTO ACM010 select a.accno, b.accname, a.amt1, a.amt2, a.amt3, " & _
                 "a.amt4, a.amt5, a.amt6, a.amt7 from " & _
                 "(SELECT substring(accno, 1, 1) AS accno, SUM(amt1) AS amt1, " & _
                 "SUM(amt2) AS amt2, SUM(amt3) AS amt3, SUM(amt4) AS amt4, " & _
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, 0 AS amt7 from acm010 " & _
                 "where len(accno)=2 " & _
                 "GROUP BY substring(accno, 1, 1)) a left outer join ACCNAME b ON a.accno = b.ACCNO"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("統計一級 error " & sqlstr)
        '合計
        sqlstr = "INSERT INTO ACM010 " & _
                 "SELECT '9' AS accno,'合             計' as accname, SUM(amt1) AS amt1, " & _
                 "SUM(amt2) AS amt2, SUM(amt3) AS amt3, SUM(amt4) AS amt4, " & _
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, 0 AS amt7 from acm010 " & _
                 "where len(accno)=1 "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("合計 error " & sqlstr)
        '丟入dataset 
        'sqlstr = "SELECT * FROM acm010 order by accno"
        'myds = Master.ADO.openmember(DNS_ACC, "acm010", sqlstr)
    End Sub

    Protected Sub BtnPrintExcel_Click(sender As Object, e As EventArgs) Handles BtnPrintExcel.Click
        Session("LastDay") = dtpDateS.Text

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("SUnitTitle", Session("SUnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號

        Param.Add("dtpDateS", Master.Models.strDateChinessToAD(dtpDateS.Text))    '結束會計科目

        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "0")  '本月
        Dim up As String = Format(Month(dtpDate) - 1, "00") '上月
        Dim dd As String = DateTime.DaysInMonth(dtpDate.Year, dtpDate.Month)

        Param.Add("SYear", SYear)
        Param.Add("mm", mm)
        Param.Add("up", up)
        Param.Add("dd", dd)


        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        LoadGridFunc()

        Master.PrintFRxls("ACM010總分類帳彙總表Excel", Session("ORG"), DNS_ACC, Param)
    End Sub
End Class