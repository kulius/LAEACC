Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class ACM080
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

        Master.PrintFR("ACM080固定資產增減表", Session("ORG"), DNS_ACC, Param)
    End Sub

    Sub LoadGridFunc()
        Dim sqlstr, retstr, tempStr1, tempStr2, sdate As String
        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "00")  '本月
        Dim up As String = Format(Month(dtpDate) - 1, "00") '上月


        sdate = SYear + 1911 & "/" & Val(mm) & "/1"
        '先清空acm010 
        sqlstr = "delete from acm070"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '將acf020材料科目置入acm070(kind='1',"2","3","4")    'substring(a.accno,5,1)='2'是備抵折舊
        sqlstr = "INSERT INTO ACM070 " & _
        "SELECT d.ACCNO, c.ACCNAME AS accname, e.KIND, '購入' AS remark, e.UNIT, d.AMT1, " & _
        " d.AMT2, 0 AS amt3, 0 AS amt4  FROM " & _
        "(SELECT A.ACCNO, SUM(A.MAT_QTY) AS AMT1, SUM(A.AMT) AS AMT2 FROM ACF020 A LEFT OUTER JOIN " & _
        " ACF010 b ON A.KIND = b.KIND AND A.NO_1_NO = b.NO_1_NO and a.seq=b.seq and b.item='1' AND A.ACCYEAR = b.ACCYEAR " & _
        " WHERE SUBSTRING(A.ACCNO, 1, 2) = '13' and SUBSTRING(A.ACCNO, 1, 5) <> '13701' and substring(a.accno,5,1)<>'2' AND b.DATE_2 >= '" & sdate & "'" & _
        " AND b.DATE_2 <= '" & Master.Models.strDateChinessToAD(dtpDateS.Text) & "' AND A.KIND = '2'" & _
        " GROUP BY A.ACCNO) d LEFT OUTER JOIN ACF100 e ON d.ACCNO = e.ACCNO AND e.ACCYEAR = " & SYear & _
        " LEFT OUTER JOIN ACCNAME c ON rtrim(left(d.ACCNO,7)) = c.ACCNO"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        sqlstr = "INSERT INTO ACM070 " & _
        "SELECT d.ACCNO, c.ACCNAME AS accname, e.KIND, '科目調整' AS remark, e.UNIT, d.AMT1, " & _
        " d.AMT2, 0 AS amt3, 0 AS amt4  FROM " & _
        "(SELECT A.ACCNO, SUM(A.MAT_QTY) AS AMT1, SUM(A.AMT) AS AMT2 FROM ACF020 A LEFT OUTER JOIN " & _
        " ACF010 b ON A.KIND = b.KIND AND A.NO_1_NO = b.NO_1_NO and a.seq=b.seq and b.item='1' AND A.ACCYEAR = b.ACCYEAR " & _
        " WHERE SUBSTRING(A.ACCNO, 1, 2) = '13' and SUBSTRING(A.ACCNO, 1, 5) <> '13701' and substring(a.accno,5,1)<>'2' AND b.DATE_2 >= '" & sdate & "'" & _
        " AND b.DATE_2 <= '" & Master.Models.strDateChinessToAD(dtpDateS.Text) & "' AND A.KIND = '3'" & _
        " GROUP BY A.ACCNO) d LEFT OUTER JOIN ACF100 e ON d.ACCNO = e.ACCNO AND e.ACCYEAR = " & SYear & _
        " LEFT OUTER JOIN ACCNAME c ON rtrim(left(d.ACCNO,7)) = c.ACCNO"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        sqlstr = "INSERT INTO ACM070 " & _
        "SELECT d.ACCNO, c.ACCNAME AS accname, e.KIND, '報廢' AS remark, e.UNIT, 0 AS amt1, 0 AS amt2, d.AMT3, " & _
        " d.AMT4  FROM " & _
        "(SELECT A.ACCNO, SUM(A.MAT_QTY) AS AMT3, SUM(A.AMT) AS AMT4 FROM ACF020 A LEFT OUTER JOIN " & _
        " ACF010 b ON A.KIND = b.KIND AND A.NO_1_NO = b.NO_1_NO and a.seq=b.seq and b.item='1' AND A.ACCYEAR = b.ACCYEAR " & _
        " WHERE SUBSTRING(A.ACCNO, 1, 2) = '13' and SUBSTRING(A.ACCNO, 1, 5) <> '13701' and substring(a.accno,5,1)<>'2' AND b.DATE_2 >= '" & sdate & "'" & _
        " AND b.DATE_2 <= '" & Master.Models.strDateChinessToAD(dtpDateS.Text) & "' AND A.KIND = '4'" & _
        " GROUP BY A.ACCNO) d LEFT OUTER JOIN ACF100 e ON d.ACCNO = e.ACCNO AND e.ACCYEAR = " & SYear & _
         " LEFT OUTER JOIN ACCNAME c ON rtrim(left(d.ACCNO,7)) = c.ACCNO"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        sqlstr = "INSERT INTO ACM070 " & _
        "SELECT d.ACCNO, c.ACCNAME AS accname, e.KIND, '變賣' AS remark, e.UNIT, 0 AS amt1, 0 AS amt2, d.AMT3, " & _
        " d.AMT4  FROM " & _
        "(SELECT A.ACCNO, SUM(A.MAT_QTY) AS AMT3, SUM(A.AMT) AS AMT4 FROM ACF020 A LEFT OUTER JOIN " & _
        " ACF010 b ON A.KIND = b.KIND AND A.NO_1_NO = b.NO_1_NO and a.seq=b.seq and b.item='1' AND A.ACCYEAR = b.ACCYEAR " & _
        " WHERE SUBSTRING(A.ACCNO, 1, 2) = '13' and SUBSTRING(A.ACCNO, 1, 5) <> '13701' and substring(a.accno,5,1)<>'2' AND b.DATE_2 >= '" & sdate & "'" & _
        " AND b.DATE_2 <= '" & Master.Models.strDateChinessToAD(dtpDateS.Text) & "' AND A.KIND = '1'" & _
        " GROUP BY A.ACCNO) d LEFT OUTER JOIN ACF100 e ON d.ACCNO = e.ACCNO AND e.ACCYEAR = " & SYear & _
        " LEFT OUTER JOIN ACCNAME c ON rtrim(left(d.ACCNO,7)) = c.ACCNO"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)


        '合計
        sqlstr = "INSERT INTO ACM070 " & _
                 "SELECT '9' AS accno,'合        計' as accname,'  ' as kind, '  ' as remark, " & _
                 "'  ' as unit, SUM(amt1) AS amt1, SUM(amt2) AS amt2, SUM(amt3) AS amt3, SUM(amt4) AS amt4 " & _
                 " from acm070"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("合計 error " & sqlstr)
    End Sub
End Class