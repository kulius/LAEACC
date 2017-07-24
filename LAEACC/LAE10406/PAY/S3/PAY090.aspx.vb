Imports System.Data
Imports System.Data.SqlClient

Public Class PAY090
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

        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")

        'Focus*****
        TabContainer1.ActiveTabIndex = 0 '指定Tab頁籤




        Dim sqlstr, qstr, strD, strC As String
        Dim intI, PageNo As Integer
        sqlstr = "SELECT max(date_2) as date_2 FROM  CHF020"
        mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        If mydataset.Tables("chf020").Rows.Count = 0 Or IsDBNull(mydataset.Tables("chf020").Rows(0).Item("date_2")) Then
            lblMsg.Text = "尚未作帳"
            MessageBx("已開帳完成")
            btnSure.Visible = False
        Else
            ViewState("sdate") = mydataset.Tables("chf020").Rows(0).Item("date_2")
            lblDate2.Text = Master.Models.strDateADToChiness(Trim(mydataset.Tables("chf020").Rows(0).Item("date_2").ToShortDateString.ToString))
            ViewState("SYear") = Year(ViewState("sdate")) - 1911   '年度
            PageNo = Master.Controller.QueryNO(Val(ViewState("SYear")), "7")    '\accservice\service1.asmx
            txtPageNo.Text = PageNo + 1
        End If




    End Sub
    Protected Sub Page_SaveStateComplete(sender As Object, e As System.EventArgs) Handles Me.SaveStateComplete
        '++ 物件初始化 ++
        'DataGrid*****
        Master.Controller.objDataGridStyle(DataGridView, "M")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            '其他預設值*****

        End If
    End Sub
    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
 
#End Region



    '載入資料
    Public Sub FillData(ByVal strOrder As String, ByVal strSortType As String, Optional ByVal strSearch As String = "")
        'sqlstr = "SELECT * FROM  CHF040 where rdate >='" & Master.Models.FullDate(dtpDateS.Text) & "' order by rdate desc"


        lbl_sort.Text = Master.Controller.objSort(IIf(strSortType = "", "ASC", strSortType))
        Master.Controller.objDataGrid(DataGridView, lbl_GrdCount, DNS_ACC, sqlstr, "查詢資料檔")


    End Sub




    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        Dim sqlstr, retstr, qstr, strD, strC, skind As String
        Dim intI, intJ, SumAmt, err As Integer
        Dim bm As DataTable
        Dim myds As DataSet

        '先將acf020本日收支置grid 
        sqlstr = "SELECT  isnull(DATE_2,'') DATE_2, *, 0.00 as ramt, 0.00 as pamt, '      ' as errmark FROM  CHF020 where balance<>0 or day_income<>0 or day_pay<>0 order by accno,bank"
        mydataset = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        If mydataset.Tables("chf020").Rows.Count = 0 Then
            MsgBox("沒有銀行資料")
            btnSure.Visible = False
            Exit Sub
        Else
            DataGridView.DataSource = mydataset
            DataGridView.DataBind()
            bm = mydataset.Tables("chf020")
        End If

        '再將傳票收支數置data的ramt pamt
        sqlstr = "SELECT sum(amt) as amt, bank,kind from acf010 where date_2='" & Master.Models.FullDate(ViewState("sdate")) & _
                 "' and item='9' and no_2_no<>0 group by bank, kind order by bank, kind"
        myds = Master.ADO.openmember(DNS_ACC, "acf010", sqlstr)
        If myds.Tables("acf010").Rows.Count = 0 Then
            MessageBx("沒有傳票資料")
            btnSure.Visible = False
            Exit Sub
        End If
        Dim sBank As String
        For intI = 0 To myds.Tables("acf010").Rows.Count - 1
            SBank = myds.Tables("acf010").Rows(intI).Item("bank")
            skind = myds.Tables("acf010").Rows(intI).Item("kind")
            SumAmt = myds.Tables("acf010").Rows(intI).Item("amt")
            For intJ = 0 To bm.Rows.Count - 1
                If bm.Rows(intJ)("bank") = sBank Then
                    If skind = "1" Then
                        bm.Rows(intJ)("ramt") = SumAmt
                    Else
                        bm.Rows(intJ)("pamt") = SumAmt
                    End If
                    Exit For
                End If
            Next
        Next

        '判斷收支淨額相同
        err = 0
        For intJ = 0 To bm.Rows.Count - 1
            If bm.Rows(intJ)("day_income") - bm.Rows(intJ)("day_pay") <> bm.Rows(intJ)("ramt") - bm.Rows(intJ)("pamt") Then
                bm.Rows(intJ)("errmark") = "錯誤"
                err = err + 1
            End If
        Next
        If err > 0 Then
            MessageBx("尚有資料未入帳")
            btnSure.Visible = False
            Exit Sub
        End If

        '找銀行轉帳借貸要平衡  (收入傳票借-貸=支出傳票貸-借)
        sqlstr = "SELECT sum(amt) as amt, dc from acf010 where date_2='" & Master.Models.FullDate(ViewState("sdate")) & _
                 "' and no_2_no<>0 and kind<='2' group by dc order by dc"
        myds = Master.ADO.openmember(DNS_ACC, "acf010", sqlstr)
        With myds.Tables("acf010")
            If .Rows.Count > 0 Then
                Dim TotD, TotC As Decimal
                TotD = 0 : TotC = 0
                For intJ = 0 To .Rows.Count - 1
                    If .Rows(intJ).Item("dc") = "1" Then TotD = Master.ADO.nz(.Rows(intJ).Item("amt"), 0)
                    If .Rows(intJ).Item("dc") = "2" Then TotC = Master.ADO.nz(.Rows(intJ).Item("amt"), 0)
                Next
                If TotD <> TotC Then
                    MessageBx("尚有銀行轉帳未入帳" & FormatNumber(TotD - TotC, 2))
                    btnSure.Visible = False
                    DataGridView.DataSource = mydataset
                    DataGridView.DataBind()
                    Exit Sub
                End If
            End If
        End With

        '找各傳票起訖號
        Dim sNo1, eNo1, recno1, sNo2, eNo2, recno2 As String
        sqlstr = "SELECT kind, min(no_2_no) as sno, max(no_2_no) as eno, count(*) as recno from acf010 " & _
                 " where date_2='" & Master.Models.FullDate(ViewState("sdate")) & "' and item='9' and kind<='2' GROUP BY kind "
        myds = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        With myds.Tables("chf020")
            For intI = 0 To .Rows.Count - 1
                If .Rows(intI).Item("kind") = "1" Then
                    sNo1 = Master.ADO.nz(.Rows(intI).Item("sno"), 0)
                    eNo1 = Master.ADO.nz(.Rows(intI).Item("eno"), 0)
                    recno1 = Master.ADO.nz(.Rows(intI).Item("recno"), 0)
                End If
                If .Rows(intI).Item("kind") = "2" Then
                    sNo2 = Master.ADO.nz(.Rows(intI).Item("sno"), 0)
                    eNo2 = Master.ADO.nz(.Rows(intI).Item("eno"), 0)
                    recno2 = Master.ADO.nz(.Rows(intI).Item("recno"), 0)
                End If
            Next
        End With
        txtPageNo.Text = Val(txtPageNo.Text) - 1  '頁次在列印前先減1, because each page will add 1 

        DataGridView.DataSource = mydataset
        DataGridView.DataBind()

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", Session("UserId"))    '使用者代號
        Param.Add("sdate", Master.Models.FullDate(ViewState("sdate")))
        Param.Add("txtPageNo", txtPageNo.Text)
        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期
        Master.PrintFR("PAY090日計表", Session("ORG"), DNS_ACC, Param)

        '記錄頁次編號acfno 
        sqlstr = "update acfno set cont_no=" & txtPageNo.Text & " where accyear=" & ViewState("SYear") & " and kind='7'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("更正頁次編號acfno錯誤" & sqlstr)

        'update chf020 
        sqlstr = "update chf020 set prt_code='Y'"
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MsgBox("update chf020 error " & sqlstr)
        lblMsg.Text = "作業完成"
    End Sub
End Class