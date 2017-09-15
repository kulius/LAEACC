Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class ACM000
    Inherits System.Web.UI.Page

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString
    Dim DNS_COA As String = ConfigurationManager.ConnectionStrings("DNS_COA").ConnectionString

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
        If Not ACM010LoadGridFunc() Then
            Exit Sub
        End If
        If Not ACM020LoadGridFunc() Then
            Exit Sub
        End If
        If Not ACM030LoadGridFunc() Then
            Exit Sub
        End If
        If Not ACM040LoadGridFunc() Then
            Exit Sub
        End If
        If Not ACM050LoadGridFunc() Then
            Exit Sub
        End If
        If Not ACM060LoadGridFunc() Then
            Exit Sub
        End If
        MessageBx("己上傳農委會成功")
    End Sub


    Function ACM010LoadGridFunc()
        Dim datacheck As Boolean = True

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
        If retstr <> "sqlok" Then
            MessageBx("合計 error " & sqlstr)
        End If

        'ACM010寫到農委會
        Dim Tdata, Tdata_ok, Tdata_no As Integer
        Tdata = 0 : Tdata_ok = 0 : Tdata_no = 0

        '先清空農委會ACM010 
        sqlstr = "delete from acm010 where unit_name=N'" & Session("SUnitTitle") & "' and fill_years='" & SYear & "' and fill_month ='" & mm & "'"
        retstr = Master.ADO.runsql(DNS_COA, sqlstr)

        sqlstr = "select * from ACM010 "
        myDatasetT = Master.ADO.openmember(DNS_ACC, "ACM010", sqlstr)

        For inti = 0 To myDatasetT.Tables("ACM010").Rows.Count - 1
            With myDatasetT.Tables(0)
                sqlstr = Master.ADO.GenInsFunc
                Master.ADO.GenInsSql("id", System.Guid.NewGuid().ToString(), "T")
                Master.ADO.GenInsSql("unit_name", Session("SUnitTitle"), "U")
                Master.ADO.GenInsSql("fill_years", SYear, "T")
                Master.ADO.GenInsSql("fill_month", mm, "T")
                Master.ADO.GenInsSql("acc_no", Master.Models.FormatAccno(.Rows(inti).Item("ACCNO").ToString), "T")
                Master.ADO.GenInsSql("acc_name", .Rows(inti).Item("ACCNAME").ToString, "U")
                Master.ADO.GenInsSql("last_month_debit_balance", .Rows(inti).Item("AMT1").ToString, "N")
                Master.ADO.GenInsSql("last_month_lender_balance", .Rows(inti).Item("AMT2").ToString, "N")
                Master.ADO.GenInsSql("now_month_debit_total", .Rows(inti).Item("AMT3").ToString, "N")
                Master.ADO.GenInsSql("now_month_lender_total", .Rows(inti).Item("AMT4").ToString, "N")
                Master.ADO.GenInsSql("now_month_debit_balance", .Rows(inti).Item("AMT5").ToString, "N")
                Master.ADO.GenInsSql("now_month_lender_balance", .Rows(inti).Item("AMT6").ToString, "N")
                sqlstr = "insert into ACM010 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_COA, sqlstr)
                Tdata = Tdata + 1
                If retstr <> "sqlok" Then
                    Tdata_no = Tdata_no + 1
                Else
                    Tdata_ok = Tdata_ok + 1
                End If
            End With
        Next

        If Tdata = 0 Then
            MessageBx("上傳ACM010資料有誤，資料筆數為0")
            datacheck = False
        End If

        If Not UploadCOA("ACM010", Tdata, Tdata_ok, Tdata_no) Then
            MessageBx("ACM010上傳農委會ACM000有誤")
            datacheck = False
        End If
        Return datacheck

    End Function

    Function ACM020LoadGridFunc()
        Dim datacheck As Boolean = True

        Dim sqlstr, retstr, tempStr As String
        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "00")  '本月
        Dim up As String = Format(Month(dtpDate) - 1, "00") '上月
        Dim myds As DataSet

        Dim sea As Integer = Master.Models.strDateChinessToSeason(dtpDateS.Text)      '第幾季
        Dim strBg As String = "a.bg1 "
        Dim inti As Integer
        'check accbg 因為要從accbg開始找資料,所以accbg必須完整
        sqlstr = "select accno from acf050 where accyear=" & SYear & " And Len(ACCNO) between 5 And 9 " & _
        " and (accno not in (select accno from accbg where accyear=" & SYear & " And Len(ACCNO) between 5 And 9)) " & _
        " and left(accno,1)='4'"
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
            tempStr = " b.cramt01 - b.deamt01 AS amt3, "
        Else
            tempStr = "( b.cramt" & mm & " - b.deamt" & mm & ") - (b.cramt" & up & " - b.deamt" & up & ") AS amt3, "
        End If
        '先由accbg找資料
        sqlstr = "INSERT INTO ACM010  SELECT a.accno, c.accname, " & _
                   "a.bg1+a.bg2+a.bg3+a.bg4+a.bg5+a.up1+a.up2+a.up3+a.up4+a.up5 AS amt1, " & strBg & _
                   tempStr & " b.cramt" & mm & " - b.deamt" & mm & " as amt4, " & _
                   "0 AS amt5, 0 as amt6, a.up1+a.up2+a.up3+a.up4+a.up5 as amt7 from ACCBG a " & _
                   "left outer join acf050 b ON a.accyear = b.accyear and a.accno=b.accno " & _
                   "left outer join ACCNAME c ON a.ACCNO = c.ACCNO " & _
                   "WHERE a.ACCYEAR = " & SYear & " AND LEN(a.ACCNO) >= 5 and len(a.accno)<=9 and substring(a.accno,1,1)='4'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '消除null  (當年度皆未有實支數時,amt3 amt4 is null), =null無法作數學運算 
        sqlstr = "update acm010 set amt3=0 where amt3 is null"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        sqlstr = "update acm010 set amt4=0 where amt4 is null"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '尚待計算應收數AMT5 (6級)
        sqlstr = " update acm010 set amt5=a.amt from " & _
        "(select left(accno,9) as accno, sum(amt) as amt from acf020 where accyear=" & SYear & " and left(accno,1)='4'" & _
        " and cotn_code='A' and dc='2' and no_2_no > 0 GROUP by left(accno,9) ) a where acm010.accno=a.accno "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        '借方為減少
        sqlstr = " update acm010 set amt5=amt5-a.amt from " & _
        "(select left(accno,9) as accno, sum(amt) as amt from acf020 where accyear=" & SYear & " and left(accno,1)='4'" & _
        " and cotn_code='A' and dc='1' and no_2_no > 0 GROUP by left(accno,9) ) a where acm010.accno=a.accno "
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        'update 5級
        sqlstr = "update ACM010 set amt5=a.amt from (select left(accno,7) as accno, sum(amt5) as amt from acm010 " & _
                 "where len(accno)=9 GROUP BY left(accno,7)) a where acm010.accno=a.accno"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        'update 4級
        sqlstr = "update ACM010 set amt5=a.amt from (select left(accno,5) as accno, sum(amt5) as amt from acm010 " & _
                 "where len(accno)=7 GROUP BY left(accno,5)) a where acm010.accno=a.accno"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)

        '未支出之分配數
        sqlstr = " Update ACM010 SET amt3 = amt3 - amt5, amt4=amt4 - amt5"
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
                 "SUM(amt5) AS amt5, SUM(amt6) AS amt6, SUM(amt7) AS amt7 from acm010 " & _
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

        'ACM020寫到農委會
        Dim Tdata, Tdata_ok, Tdata_no As Integer
        Tdata = 0 : Tdata_ok = 0 : Tdata_no = 0

        '先清空農委會ACM020 
        sqlstr = "delete from acm020 where unit_name=N'" & Session("SUnitTitle") & "' and fill_years='" & SYear & "' and fill_month ='" & mm & "'"
        retstr = Master.ADO.runsql(DNS_COA, sqlstr)

        sqlstr = "select * from ACM010 "
        myDatasetT = Master.ADO.openmember(DNS_ACC, "ACM010", sqlstr)

        For inti = 0 To myDatasetT.Tables("ACM010").Rows.Count - 1
            With myDatasetT.Tables(0)
                sqlstr = Master.ADO.GenInsFunc
                Master.ADO.GenInsSql("id", System.Guid.NewGuid().ToString(), "T")
                Master.ADO.GenInsSql("unit_name", Session("SUnitTitle"), "U")
                Master.ADO.GenInsSql("fill_years", SYear, "T")
                Master.ADO.GenInsSql("fill_month", mm, "T")
                Master.ADO.GenInsSql("acc_no", Master.Models.FormatAccno(.Rows(inti).Item("ACCNO").ToString), "T")
                Master.ADO.GenInsSql("acc_name", .Rows(inti).Item("ACCNAME").ToString, "U")
                Master.ADO.GenInsSql("approved", .Rows(inti).Item("AMT1").ToString, "N")
                Master.ADO.GenInsSql("correct", .Rows(inti).Item("AMT2").ToString, "N")
                Master.ADO.GenInsSql("month_end_distribution", .Rows(inti).Item("AMT3").ToString, "N")
                Master.ADO.GenInsSql("month_end_income", .Rows(inti).Item("AMT4").ToString, "N")
                Master.ADO.GenInsSql("month_end_grand_total", .Rows(inti).Item("AMT5").ToString, "N")
                Master.ADO.GenInsSql("receivable", .Rows(inti).Item("AMT6").ToString, "N")
                Master.ADO.GenInsSql("not_received_distribution", .Rows(inti).Item("AMT7").ToString, "N")
                sqlstr = "insert into ACM020 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_COA, sqlstr)
                Tdata = Tdata + 1
                If retstr <> "sqlok" Then
                    Tdata_no = Tdata_no + 1
                Else
                    Tdata_ok = Tdata_ok + 1
                End If
            End With
        Next

        If Tdata = 0 Then
            MessageBx("上傳ACM020資料有誤，資料筆數為0")
            datacheck = False
        End If

        If Not UploadCOA("ACM020", Tdata, Tdata_ok, Tdata_no) Then
            MessageBx("ACM020上傳農委會ACM000有誤")
            datacheck = False
        End If
        Return datacheck

    End Function

    Function ACM030LoadGridFunc()
        Dim datacheck As Boolean = True
        Dim sqlstr, retstr, tempStr, strGrade7 As String
        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "00")  '本月
        Dim up As String = Format(Month(dtpDate) - 1, "00") '上月
        Dim myds As DataSet
        Dim sea As Integer = Master.Models.strDateChinessToSeason(dtpDateS.Text)      '第幾季

        'If rdbprint1.Checked Then
        '    strGrade7 = "Y"
        'Else
        strGrade7 = "N"
        'End If

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

        'ACM030寫到農委會
        Dim Tdata, Tdata_ok, Tdata_no As Integer
        Tdata = 0 : Tdata_ok = 0 : Tdata_no = 0

        '先清空農委會ACM030 
        sqlstr = "delete from acm030 where unit_name=N'" & Session("SUnitTitle") & "' and fill_years='" & SYear & "' and fill_month ='" & mm & "'"
        retstr = Master.ADO.runsql(DNS_COA, sqlstr)

        sqlstr = "select * from ACM010 "
        myDatasetT = Master.ADO.openmember(DNS_ACC, "ACM010", sqlstr)

        For inti = 0 To myDatasetT.Tables("ACM010").Rows.Count - 1
            With myDatasetT.Tables("ACM010")
                sqlstr = Master.ADO.GenInsFunc
                Master.ADO.GenInsSql("id", System.Guid.NewGuid().ToString(), "T")
                Master.ADO.GenInsSql("unit_name", Session("SUnitTitle"), "U")
                Master.ADO.GenInsSql("fill_years", SYear, "T")
                Master.ADO.GenInsSql("fill_month", mm, "T")
                Master.ADO.GenInsSql("acc_no", Master.Models.FormatAccno(.Rows(inti).Item("ACCNO").ToString), "T")
                Master.ADO.GenInsSql("acc_name", .Rows(inti).Item("ACCNAME").ToString, "U")
                Master.ADO.GenInsSql("approved", .Rows(inti).Item("AMT1").ToString, "N")
                Master.ADO.GenInsSql("correct", .Rows(inti).Item("AMT2").ToString, "N")
                Master.ADO.GenInsSql("month_end_distribution", .Rows(inti).Item("AMT3").ToString, "N")
                Master.ADO.GenInsSql("month_end_income", .Rows(inti).Item("AMT4").ToString, "N")
                Master.ADO.GenInsSql("month_end_grand_total", .Rows(inti).Item("AMT5").ToString, "N")
                Master.ADO.GenInsSql("payable", .Rows(inti).Item("AMT6").ToString, "N")
                Master.ADO.GenInsSql("not_payable_distribution", .Rows(inti).Item("AMT7").ToString, "N")
                sqlstr = "insert into ACM030 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_COA, sqlstr)
                Tdata = Tdata + 1
                If retstr <> "sqlok" Then
                    Tdata_no = Tdata_no + 1
                Else
                    Tdata_ok = Tdata_ok + 1
                End If
            End With
        Next

        If Tdata = 0 Then
            MessageBx("上傳ACM030資料有誤，資料筆數為0")
            datacheck = False
        End If

        If Not UploadCOA("ACM030", Tdata, Tdata_ok, Tdata_no) Then
            MessageBx("ACM030上傳農委會ACM000有誤")
            datacheck = False
        End If
        Return datacheck

    End Function

    Function ACM040LoadGridFunc()
        Dim datacheck As Boolean = True
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
                   " a.act" & mm & " as amt3, " & tempStr2 & " a.sub" & mm & " as amt5, a.trans" & mm & " as amt6, " & _
                   "0 AS amt7 from ACF060 a " & _
                   "left outer join ACCNAME B ON a.ACCNO = B.ACCNO " & _
                   "WHERE a.ACCYEAR = " & SYear & " AND LEN(a.ACCNO) >= 5 and " & _
                   "len(a.accno)<>17 and substring(a.accno,1,3)='113'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)



        '各欄數值計算
        sqlstr = " Update ACM010 SET amt7 = amt1 - amt3 - amt5 - amt6"
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

        'ACM040寫到農委會
        Dim Tdata, Tdata_ok, Tdata_no As Integer
        Dim strAccno, strAccname As String
        Tdata = 0 : Tdata_ok = 0 : Tdata_no = 0

        '先清空農委會ACM040 
        sqlstr = "delete from acm040 where unit_name=N'" & Session("SUnitTitle") & "' and fill_years='" & SYear & "' and fill_month ='" & mm & "'"
        retstr = Master.ADO.runsql(DNS_COA, sqlstr)

        sqlstr = "select * from ACM010 "
        myDatasetT = Master.ADO.openmember(DNS_ACC, "ACM010", sqlstr)

        For inti = 0 To myDatasetT.Tables("ACM010").Rows.Count - 1
            With myDatasetT.Tables("ACM010")
                strAccno = Master.ADO.nz(.Rows(inti).Item("accno"), "")
                strAccname = Master.ADO.nz(.Rows(inti).Item("accname"), "")

                sqlstr = Master.ADO.GenInsFunc
                Master.ADO.GenInsSql("id", System.Guid.NewGuid().ToString(), "T")
                Master.ADO.GenInsSql("unit_name", Session("SUnitTitle"), "U")
                Master.ADO.GenInsSql("fill_years", SYear, "T")
                Master.ADO.GenInsSql("fill_month", mm, "T")
                Master.ADO.GenInsSql("in_years", IIf(Val(Mid(strAccno, 10, 3)) < 60, "", Val(Mid(strAccno, 10, 3))), "T") '取年度
                Master.ADO.GenInsSql("acc_no_name", Master.Models.FormatAccno(strAccno) & strAccname, "U")
                Master.ADO.GenInsSql("acc_no", Master.Models.FormatAccno(.Rows(inti).Item("ACCNO").ToString), "T")
                Master.ADO.GenInsSql("acc_name", .Rows(inti).Item("ACCNAME").ToString, "U")
                Master.ADO.GenInsSql("receivable", .Rows(inti).Item("AMT1").ToString, "N")
                Master.ADO.GenInsSql("month_end_income", .Rows(inti).Item("AMT2").ToString, "N")
                Master.ADO.GenInsSql("month_end_income_total", .Rows(inti).Item("AMT3").ToString, "N")
                Master.ADO.GenInsSql("month_end_reduce", .Rows(inti).Item("AMT4").ToString, "N")
                Master.ADO.GenInsSql("month_end_reduce_total", .Rows(inti).Item("AMT5").ToString, "N")
                Master.ADO.GenInsSql("transfer_urge", .Rows(inti).Item("AMT6").ToString, "N")
                Master.ADO.GenInsSql("balance", .Rows(inti).Item("AMT7").ToString, "N")
                sqlstr = "insert into ACM040 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_COA, sqlstr)
                Tdata = Tdata + 1
                If retstr <> "sqlok" Then
                    Tdata_no = Tdata_no + 1
                Else
                    Tdata_ok = Tdata_ok + 1
                End If
            End With
        Next

        If Tdata = 0 Then
            MessageBx("上傳ACM040資料有誤，資料筆數為0")
            datacheck = False
        End If

        If Not UploadCOA("ACM040", Tdata, Tdata_ok, Tdata_no) Then
            MessageBx("ACM040上傳農委會ACM000有誤")
            datacheck = False
        End If
        Return datacheck

    End Function

    Function ACM050LoadGridFunc()
        Dim datacheck As Boolean = True
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
                   " a.act" & mm & " as amt3," & tempStr2 & " a.sub" & mm & " as amt5, " & _
                   " 0 as amt6, 0 AS amt7 from ACF060 a " & _
                   "left outer join ACCNAME B ON a.ACCNO = B.ACCNO " & _
                   "WHERE a.ACCYEAR = " & SYear & " AND LEN(a.ACCNO) >= 5 and " & _
                   "len(a.accno)<>17 and substring(a.accno,1,3)='151'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)


        '各欄數值計算
        sqlstr = " Update ACM010 SET amt6 = amt1 - amt3 - amt5"
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

        'ACM050寫到農委會
        Dim Tdata, Tdata_ok, Tdata_no As Integer
        Dim strAccno, strAccname As String
        Tdata = 0 : Tdata_ok = 0 : Tdata_no = 0

        '先清空農委會ACM050 
        sqlstr = "delete from acm050 where unit_name=N'" & Session("SUnitTitle") & "' and fill_years='" & SYear & "' and fill_month ='" & mm & "'"
        retstr = Master.ADO.runsql(DNS_COA, sqlstr)

        sqlstr = "select * from ACM010 "
        myDatasetT = Master.ADO.openmember(DNS_ACC, "ACM010", sqlstr)

        For inti = 0 To myDatasetT.Tables("ACM010").Rows.Count - 1
            With myDatasetT.Tables("ACM010")
                strAccno = Master.ADO.nz(.Rows(inti).Item("accno"), "")
                strAccname = Master.ADO.nz(.Rows(inti).Item("accname"), "")

                sqlstr = Master.ADO.GenInsFunc
                Master.ADO.GenInsSql("id", System.Guid.NewGuid().ToString(), "T")
                Master.ADO.GenInsSql("unit_name", Session("SUnitTitle"), "U")
                Master.ADO.GenInsSql("fill_years", SYear, "T")
                Master.ADO.GenInsSql("fill_month", mm, "T")
                Master.ADO.GenInsSql("urge_years", IIf(Val(Mid(strAccno, 10, 3)) < 60, "", Val(Mid(strAccno, 10, 3))), "T") '取年度
                Master.ADO.GenInsSql("acc_no_name", Master.Models.FormatAccno(strAccno) & strAccname, "U")
                Master.ADO.GenInsSql("acc_no", Master.Models.FormatAccno(.Rows(inti).Item("ACCNO").ToString), "T")
                Master.ADO.GenInsSql("acc_name", .Rows(inti).Item("ACCNAME").ToString, "U")
                Master.ADO.GenInsSql("urge", .Rows(inti).Item("AMT1").ToString, "N")
                Master.ADO.GenInsSql("month_end_income", .Rows(inti).Item("AMT2").ToString, "N")
                Master.ADO.GenInsSql("month_end_income_total", .Rows(inti).Item("AMT3").ToString, "N")
                Master.ADO.GenInsSql("month_end_reduce", .Rows(inti).Item("AMT4").ToString, "N")
                Master.ADO.GenInsSql("month_end_reduce_total", .Rows(inti).Item("AMT5").ToString, "N")
                Master.ADO.GenInsSql("balance", .Rows(inti).Item("AMT6").ToString, "N")

                sqlstr = "insert into ACM050 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_COA, sqlstr)
                Tdata = Tdata + 1
                If retstr <> "sqlok" Then
                    Tdata_no = Tdata_no + 1
                Else
                    Tdata_ok = Tdata_ok + 1
                End If
            End With
        Next

        If Tdata = 0 Then
            MessageBx("上傳ACM050資料有誤，資料筆數為0")
            datacheck = False
        End If

        If Not UploadCOA("ACM050", Tdata, Tdata_ok, Tdata_no) Then
            MessageBx("ACM050上傳農委會ACM000有誤")
            datacheck = False
        End If
        Return datacheck
    End Function

    Function ACM060LoadGridFunc()
        Dim datacheck As Boolean = True
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

        'ACM060寫到農委會
        Dim Tdata, Tdata_ok, Tdata_no As Integer
        Dim strAccno, strAccname As String
        Tdata = 0 : Tdata_ok = 0 : Tdata_no = 0

        '先清空農委會ACM050 
        sqlstr = "delete from acm060 where unit_name=N'" & Session("SUnitTitle") & "' and fill_years='" & SYear & "' and fill_month ='" & mm & "'"
        retstr = Master.ADO.runsql(DNS_COA, sqlstr)

        sqlstr = "select * from ACM010 "
        myDatasetT = Master.ADO.openmember(DNS_ACC, "ACM010", sqlstr)

        For inti = 0 To myDatasetT.Tables("ACM010").Rows.Count - 1
            With myDatasetT.Tables("ACM010")
                strAccno = Master.ADO.nz(.Rows(inti).Item("accno"), "")
                strAccname = Master.ADO.nz(.Rows(inti).Item("accname"), "")

                sqlstr = Master.ADO.GenInsFunc
                Master.ADO.GenInsSql("id", System.Guid.NewGuid().ToString(), "T")
                Master.ADO.GenInsSql("unit_name", Session("SUnitTitle"), "U")
                Master.ADO.GenInsSql("fill_years", SYear, "T")
                Master.ADO.GenInsSql("fill_month", mm, "T")
                Master.ADO.GenInsSql("pay_years", IIf(Val(Mid(strAccno, 10, 3)) < 60, "", Val(Mid(strAccno, 10, 3))), "T") '取年度
                Master.ADO.GenInsSql("acc_no_name", Master.Models.FormatAccno(strAccno) & strAccname, "U")
                Master.ADO.GenInsSql("acc_no", Master.Models.FormatAccno(.Rows(inti).Item("ACCNO").ToString), "T")
                Master.ADO.GenInsSql("acc_name", .Rows(inti).Item("ACCNAME").ToString, "U")
                Master.ADO.GenInsSql("payable", .Rows(inti).Item("AMT1").ToString, "N")
                Master.ADO.GenInsSql("month_end_income", .Rows(inti).Item("AMT2").ToString, "N")
                Master.ADO.GenInsSql("month_end_income_total", .Rows(inti).Item("AMT3").ToString, "N")
                Master.ADO.GenInsSql("month_end_reduce", .Rows(inti).Item("AMT4").ToString, "N")
                Master.ADO.GenInsSql("month_end_reduce_total", .Rows(inti).Item("AMT5").ToString, "N")
                Master.ADO.GenInsSql("balance", .Rows(inti).Item("AMT6").ToString, "N")

                sqlstr = "insert into ACM060 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_COA, sqlstr)
                Tdata = Tdata + 1
                If retstr <> "sqlok" Then
                    Tdata_no = Tdata_no + 1
                Else
                    Tdata_ok = Tdata_ok + 1
                End If
            End With
        Next

        If Tdata = 0 Then
            MessageBx("上傳ACM060資料有誤，資料筆數為0")
            datacheck = False
        End If

        If Not UploadCOA("ACM060", Tdata, Tdata_ok, Tdata_no) Then
            MessageBx("ACM060上傳農委會ACM000有誤")
            datacheck = False
        End If
        Return datacheck

    End Function

    Function UploadCOA(ByVal ReportName As String, ByVal data_count As Integer, ByVal success_count As Integer, ByVal fail_count As Integer)
        Dim dtpDate As DateTime = Convert.ToDateTime(Master.Models.strDateChinessToAD1(dtpDateS.Text))
        Dim SYear As String = Mid(dtpDateS.Text, 1, 3)  '年
        Dim mm As String = Format(Month(dtpDate), "0")  '本月

        '檢查農委會有無資料   
        Dim up_no As Integer

        sqlstr = "SELECT *  FROM ACM000 where unit_name=N'" & Session("SUnitTitle") & "' and fill_years='" & SYear & "' and fill_month ='" & mm & "' and file_no ='" & ReportName & "' "
        psDataSet = Master.ADO.openmember(DNS_COA, "ACM000", sqlstr)

        If psDataSet.Tables("ACM000").Rows.Count = 0 Then
            up_no = 0
        Else
            up_no = psDataSet.Tables(0).Rows(0).Item("upload_serial_no")
        End If

        '先清空acm000 
        sqlstr = "delete from acm000 where unit_name=N'" & Session("SUnitTitle") & "' and fill_years='" & SYear & "' and fill_month ='" & mm & "' and file_no ='" & ReportName & "' "
        retstr = Master.ADO.runsql(DNS_COA, sqlstr)

        sqlstr = Master.ADO.GenInsFunc
        Master.ADO.GenInsSql("id", System.Guid.NewGuid().ToString(), "T")
        Master.ADO.GenInsSql("unit_name", Session("SUnitTitle"), "U")
        Master.ADO.GenInsSql("fill_years", SYear, "T")
        Master.ADO.GenInsSql("fill_month", mm, "T")
        Master.ADO.GenInsSql("file_no", ReportName, "T")
        Master.ADO.GenInsSql("upload_serial_no", up_no + 1, "T")
        Master.ADO.GenInsSql("data_count", data_count, "N")
        Master.ADO.GenInsSql("success_count", success_count, "N")
        Master.ADO.GenInsSql("fail_count", fail_count, "N")
        Master.ADO.GenInsSql("upload_date_time", Now.ToString("yyyy/MM/dd HH:mm:ss"), "T")

        sqlstr = "insert into ACM000 " & Master.ADO.GenInsFunc
        retstr = Master.ADO.runsql(DNS_COA, sqlstr)

        If retstr <> "sqlok" Then
            Return False
        Else
            Return True
        End If

    End Function
End Class