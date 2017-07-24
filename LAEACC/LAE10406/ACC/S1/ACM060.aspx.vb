Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class ACM060
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
        Dim dd As String = Format(Day(dtpDate), "00")

        Param.Add("SYear", SYear)
        Param.Add("mm", mm)
        Param.Add("up", up)
        Param.Add("dd", dd)


        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        LoadGridFunc()

        Master.PrintFR("ACM060應付款項明細表", Session("ORG"), DNS_ACC, Param)
    End Sub

    Sub LoadGridFunc()
        Dim sqlstr, retstr, tempStr1, tempStr2 As String
        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "00")  '本月
        Dim up As String = Format(Month(dtpDate) - 1, "00") '上月

        '先清空acm010 
        sqlstr = "delete from acm010"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '將acf060四級至七級科目全丟入acm010

        '實收數
        If up = "00" Then
            tempStr1 = " a.act" & mm & " as amt2, "
            tempStr2 = " a.sub" & mm & " as amt4, "
        Else
            tempStr1 = " a.act" & mm & " - a.act" & up & " as amt2, "
            tempStr2 = " a.sub" & mm & " - a.sub" & up & " as amt4, "
        End If
        sqlstr = "INSERT INTO ACM010  SELECT a.accno, b.accname, " & _
                   "a.account" & mm & " as amt1, " & tempStr1 & _
                   " a.act" & mm & " as amt3, " & tempStr2 & " a.sub" & mm & " as amt5, " & _
                   "0 as amt6, 0 AS amt7 from ACF060 a " & _
                   "left outer join ACCNAME B ON a.ACCNO = B.ACCNO " & _
                   "WHERE a.ACCYEAR = " & SYear & " AND LEN(a.ACCNO) >= 5 and " & _
                   "len(a.accno)<>17 and substring(a.accno,1,3)='212'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)


        '各欄數值計算
        sqlstr = " Update ACM010 SET amt7 = amt1 - amt3 - amt5"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("update acm010 error " & sqlstr)

        sqlstr = " delete from ACM010 where amt1=0 and amt2=0 and amt3=0 and amt4=0 and amt5=0 and amt6=0"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '合計
        sqlstr = "INSERT INTO ACM010 " & _
                 "SELECT '9' AS accno,'合             計' as accname, SUM(amt1) AS amt1, " & _
                 "SUM(amt2) AS amt2, SUM(amt3) AS amt3, SUM(amt4) AS amt4, " & _
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, sum(amt7) AS amt7 from acm010 " & _
                 "where len(accno)=5 "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("合計 error " & sqlstr)
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
        Dim dd As String = Format(Day(dtpDate), "00")

        Param.Add("SYear", SYear)
        Param.Add("mm", mm)
        Param.Add("up", up)
        Param.Add("dd", dd)


        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        LoadGridFunc()

        Master.PrintFRxls("ACM060應付款項明細表Excel", Session("ORG"), DNS_ACC, Param)
    End Sub
End Class