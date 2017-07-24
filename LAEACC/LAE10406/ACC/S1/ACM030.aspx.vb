Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class ACM030
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

        Master.PrintFR("ACM030支出明細表", Session("ORG"), DNS_ACC, Param)
    End Sub

    Sub LoadGridFunc()
        Dim sqlstr, retstr, tempStr, strGrade7 As String
        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "00")  '本月
        Dim up As String = Format(Month(dtpDate) - 1, "00") '上月
        Dim myds As DataSet
        Dim sea As Integer = Master.Models.strDateChinessToSeason(dtpDateS.Text)      '第幾季

        If rdbprint1.Checked Then
            strGrade7 = "Y"
        Else
            strGrade7 = "N"
        End If

        Dim strBg As String = "a.bg1 + a.up1 "
        Dim inti As Integer
        'check accbg 因為要從accbg開始找資料,所以accbg必須完整
        sqlstr = "select accno from acf050 where accyear=" & SYear & " And Len(ACCNO) between 5 And 9 " & _
        " and (accno not in (select accno from accbg where accyear=" & SYear & " And Len(ACCNO) between 5 And 9)) " & _
        " and left(accno,1)='5'"
        myds = Master.ADO.openmember(DNS_ACC, "acf050", sqlstr)
        For inti = 0 To myds.Tables(0).Rows.Count - 1
            tempStr = Master.ADO.nz(myds.Tables(0).Rows(inti).Item(0), "")
            If tempStr <> "" Then
                sqlstr = "insert into accbg (accyear,accno) values (" & SYear & ", '" & tempStr & "')"
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            End If
        Next

        '先清空acm010 
        sqlstr = "delete from acm010"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '將acf050,accbg四五六級科目丟入acm010
        '預算數
        If sea > 1 Then
            For inti = 2 To sea
                strBg &= " + a.bg" & Format(inti, "0") & " + a.up" & Format(inti, "0")
            Next
        End If
        strBg &= " as amt2, "
        '實收數
        If up = "00" Then
            tempStr = " b.deamt01 - b.cramt01 AS amt3, "
        Else
            tempStr = " b.deamt" & mm & " - b.cramt" & mm & " - (b.deamt" & up & " - b.cramt" & up & ") AS amt3, "
        End If
        '先由accbg找資料
        sqlstr = "INSERT INTO ACM010  SELECT a.accno, c.accname, " & _
                   "a.bg1+a.bg2+a.bg3+a.bg4+a.bg5+a.up1+a.up2+a.up3+a.up4+a.up5 AS amt1, " & strBg & _
                   tempStr & " b.deamt" & mm & " - b.cramt" & mm & " as amt4, " & _
                   "0 AS amt5, 0 as amt6, a.up1+a.up2+a.up3+a.up4+a.up5 as amt7 from ACCBG a " & _
                   "left outer join acf050 b ON a.accyear = b.accyear and a.accno=b.accno " & _
                   "left outer join ACCNAME c ON a.ACCNO = c.ACCNO " & _
                   "WHERE a.ACCYEAR = " & SYear & " AND LEN(a.ACCNO) >= 5 and substring(a.accno,1,1)='5'"
        If strGrade7.ToUpper = "Y" Then
            sqlstr = sqlstr & " and len(a.accno)<=16"    '取七級
        Else
            sqlstr = sqlstr & " and len(a.accno)<=9"     '取六級
        End If
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '消除null  (當年度皆未有實支數時,amt3 amt4 is null), =null無法作數學運算 
        sqlstr = "update acm010 set amt3=0 where amt3 is null"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        sqlstr = "update acm010 set amt4=0 where amt4 is null"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        sqlstr = " delete from ACM010 where amt1=0 and amt2=0 and amt3=0 and amt4=0 and amt5=0 and amt6=0"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If strGrade7.ToUpper = "Y" Then
            '計算應收數AMT5
            sqlstr = " update acm010 set amt5=a.amt from " & _
            "(select left(accno,16) as accno, sum(amt) as amt from acf020 where accyear=" & SYear & " and left(accno,1)='5'" & _
            " and cotn_code='A' and dc='1'  and no_2_no > 0 GROUP by left(accno,16)) a where acm010.accno=a.accno "
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            '貸方為減少
            sqlstr = " update acm010 set amt5=amt5-a.amt from " & _
            "(select left(accno,16) as accno, sum(amt) as amt from acf020 where accyear=" & SYear & " and left(accno,1)='5'" & _
            " and cotn_code='A' and dc='2' and no_2_no > 0 GROUP by left(accno,16)) a where acm010.accno=a.accno "
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        End If
        '計算應收數AMT5
        sqlstr = " update acm010 set amt5=a.amt from " & _
        "(select left(accno,9) as accno, sum(amt) as amt from acf020 where accyear=" & SYear & " and left(accno,1)='5'" & _
        " and cotn_code='A' and dc='1'  and no_2_no > 0 GROUP by left(accno,9)) a where acm010.accno=a.accno "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        '貸方為減少
        sqlstr = " update acm010 set amt5=amt5-a.amt from " & _
        "(select left(accno,9) as accno, sum(amt) as amt from acf020 where accyear=" & SYear & " and left(accno,1)='5'" & _
        " and cotn_code='A' and dc='2' and no_2_no > 0 GROUP by left(accno,9)) a where acm010.accno=a.accno "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        If strGrade7.ToUpper = "Y" Then
            'update 6級
            sqlstr = "update ACM010 set amt5=a.amt from (select left(accno,9) as accno, sum(amt5) as amt from acm010 " & _
                     "where len(accno)>9 GROUP BY left(accno,9)) a where acm010.accno=a.accno"
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        End If
        'update 5級
        sqlstr = "update ACM010 set amt5=a.amt from (select left(accno,7) as accno, sum(amt5) as amt from acm010 " & _
                 "where len(accno)=9 GROUP BY left(accno,7)) a where acm010.accno=a.accno"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        'update 4級
        sqlstr = "update ACM010 set amt5=a.amt from (select left(accno,5) as accno, sum(amt5) as amt from acm010 " & _
                 "where len(accno)=7 GROUP BY left(accno,5)) a where acm010.accno=a.accno"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '未支出之分配數
        sqlstr = " Update ACM010 SET amt3 = amt3 - amt5, amt4=amt4-amt5 "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '統計三級
        sqlstr = "INSERT INTO ACM010 select a.accno, b.accname, a.amt1, a.amt2, a.amt3, " & _
                 "a.amt4, a.amt5, a.amt6, a.amt7 from " & _
                 "(SELECT substring(accno, 1, 3) AS accno, SUM(amt1) AS amt1, " & _
                 "SUM(amt2) AS amt2, SUM(amt3) AS amt3, SUM(amt4) AS amt4, " & _
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, SUM(amt7) AS amt7 from acm010 where len(accno)=5 " & _
                 "GROUP BY substring(accno, 1, 3)) a left outer join ACCNAME b ON a.accno = b.ACCNO"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("統計三級 error " & sqlstr)

        '統計二級
        sqlstr = "INSERT INTO ACM010 select a.accno, b.accname, a.amt1, a.amt2, a.amt3, " & _
                 "a.amt4, a.amt5, a.amt6, a.amt7 from " & _
                 "(SELECT substring(accno, 1, 2) AS accno, SUM(amt1) AS amt1, " & _
                 "SUM(amt2) AS amt2, SUM(amt3) AS amt3, SUM(amt4) AS amt4, " & _
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, SUM(amt7) AS amt7 from acm010 " & _
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
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, SUM(amt7) AS amt7 from acm010 " & _
                 "where len(accno)=1 "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("合計 error " & sqlstr)

        '各欄數值計算(未收入分配數)
        sqlstr = " Update ACM010 SET amt6 = amt2 - amt4 - amt5"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("update acm010 error " & sqlstr)

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

        Master.PrintFRxls("ACM030支出明細表Excel", Session("ORG"), DNS_ACC, Param)
    End Sub
End Class