Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class AC050R
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


        Dim sqlstr As String
        sqlstr = "SELECT max(date_2) as date_2  from acf010 where books='Y'"
        myDatasetS = Master.ADO.openmember(DNS_ACC, "acf010s", sqlstr)
        If myDatasetS.Tables("acf010s").Rows.Count = 0 Then
            lblMsg.Text = "沒有資料"
            Exit Sub
        Else
            nudMonth.Text = Month(myDatasetS.Tables("acf010s").Rows(0).Item("date_2"))
            lblYear.Text = Year(myDatasetS.Tables("acf010s").Rows(0).Item("date_2"))
        End If
        btnSure.Enabled = True

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



    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        btnSure.Enabled = False   '防止user再按
        Dim sqlstr, strUpdate, retstr, strJ, strDates As String
        Dim intI, intJ As Integer
        Dim smm As Integer = nudMonth.text
        If smm < 1 Then Exit Sub
        Dim up As String = Format(smm - 1, "00")   '以上月餘額為填入數
        strDates = Master.Models.FullDate(lblYear.Text & "/" & smm & "/1")


        lblMsg.Text = "acf010處理中請稍待"   '清books即可
        sqlstr = "update acf010 set books =' ' where date_2>='" & strDates & "' and books='Y'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf010處理 error " & sqlstr)

        lblMsg.Text = "acf070處理中請稍待"
        sqlstr = "delete from acf070 where date_2>='" & strDates & "'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf070處理 error " & sqlstr)

        lblMsg.Text = "acf050處理中請稍待"
        If up = "00" Then
            strUpdate = ""
            For intJ = smm To 12
                strJ = Format(intJ, "00")
                strUpdate &= " deamt" & strJ & "= beg_debit, cramt" & strJ & "= beg_credit,"
            Next
        Else
            strUpdate = ""
            For intJ = smm To 12
                strJ = Format(intJ, "00")
                strUpdate &= " deamt" & strJ & "= deamt" & up & ", cramt" & strJ & "= cramt" & up & ","
            Next
        End If
        sqlstr = "update acf050 set " & Master.Models.cutright1(strUpdate, ",") & " where accyear=" & Val(lblYear.Text)
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf050處理 error " & sqlstr)

        lblMsg.Text = "acf060處理中請稍待"
        If up = "00" Then
            strUpdate = ""
            For intJ = smm To 12
                strJ = Format(intJ, "00")
                strUpdate &= " deamt" & strJ & "= beg_debit, cramt" & strJ & "= beg_credit," & _
                             " account" & strJ & "=abs(beg_debit-beg_credit)," & _
                             " act" & strJ & "= 0, sub" & strJ & "=0, trans" & strJ & "=0,"
            Next
        Else
            strUpdate = ""
            For intJ = smm To 12
                strJ = Format(intJ, "00")
                strUpdate &= " deamt" & strJ & "= deamt" & up & ", cramt" & strJ & "= cramt" & up & _
                             ", account" & strJ & "=account" & up & ", act" & strJ & "=act" & up & _
                             ", sub" & strJ & "=sub" & up & ", trans" & strJ & "=trans" & up & ","
            Next
        End If
        sqlstr = "update acf060 set " & Master.Models.cutright1(strUpdate, ",") & " where accyear=" & Val(lblYear.Text)
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf060處理 error " & sqlstr)


        lblMsg.Text = "acf100處理中請稍待"
        If up = "00" Then
            strUpdate = ""
            For intJ = smm To 12
                strJ = Format(intJ, "00")
                strUpdate &= " qtyin" & strJ & "= qty_beg, qtyout" & strJ & "= 0,"
            Next
        Else
            strUpdate = ""
            For intJ = smm To 12
                strJ = Format(intJ, "00")
                strUpdate &= " qtyin" & strJ & "= qtyin" & up & ", qtyout" & strJ & "= qtyout" & up & ","
            Next
        End If
        sqlstr = "update acf100 set " & Master.Models.cutright1(strUpdate, ",") & " where accyear=" & Val(lblYear.Text)
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf100處理 error " & sqlstr)

        lblMsg.Text = "處理完成"

    End Sub

   
End Class