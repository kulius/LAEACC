Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing


Public Class PAY070
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
    Dim mydataset, mydataset2 As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱
        Dim myds As DataSet
        sqlstr = "SELECT date_2 from chf020 where date_2 is not null"
        myds = Master.ADO.openmember(DNS_ACC, "chf020", sqlstr)
        If myds.Tables("chf020").Rows.Count > 0 Then
            dtpDateS.Text = Master.Models.strDateADToChiness(Trim(myds.Tables("chf020").Rows(0).Item("date_2").ToShortDateString.ToString))
            dtpDateE.Text = Master.Models.strDateADToChiness(Trim(myds.Tables("chf020").Rows(0).Item("date_2").ToShortDateString.ToString))
        Else
            dtpDateS.Text = Session("userdate")
            dtpDateE.Text = Session("userdate")
        End If
        txtNo.Text = "1"


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
        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", ViewState("UserId"))    '使用者代號

        Param.Add("dtpDateS", Master.Models.FullDate(dtpDateS.Text))
        Param.Add("dtpDateE", Master.Models.FullDate(dtpDateE.Text))
        Param.Add("dtpDate", Mid(Master.Models.FullDate(dtpDateS.Text), 1, 4) + "-01-01")
        Param.Add("txtNo", txtNo.Text)


        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期


        Master.PrintFR("PAY070現金出納登記簿", Session("ORG"), DNS_ACC, Param)

        'Dim savePath As String = Server.MapPath("~/ExcelUploads/") + Guid.NewGuid().ToString() + ".xlsx"
        'Dim savePathpdf As String = Server.MapPath("~/ExcelUploads/") + Guid.NewGuid().ToString() + ".pdf"
        'Dim filename As String = "推算簿.pdf"
        'Response.Clear()
        'Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        'Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", filename))

        'Dim newFile As New FileInfo(savePath)
        'Dim package As New ExcelPackage(newFile)
        'Dim ws = package.Workbook.Worksheets.Add("sheet1")

        'ws.Cells.Style.Font.Size = 11
        'ws.Cells.Style.Font.Name = "新細明體"
        'ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center
        'ws.View.ShowGridLines = False '是否顯示格線
        'ws.PrinterSettings.PaperSize = ePaperSize.A4
        'ws.PrinterSettings.Orientation = eOrientation.Landscape
        ''ws.PrinterSettings.LeftMargin = 20 / 2.54
        ''ws.PrinterSettings.TopMargin = 10 / 2.54
        ''ws.PrinterSettings.RightMargin = 5 / 2.54
        ''ws.PrinterSettings.BottomMargin = 5 / 2.54
        ''sheet.Cells[DataRowNumber, j + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);


        'Dim intR As Integer = 0  'control record number
        'Dim intD, i As Integer
        'Dim retstr, strRemark, strAccno, strAccname As String
        'Dim bgamt, used, unuse, TotUnuse, TotUsed, subUnuse, subUsed As Decimal
        'Dim strFilter As String = Trim(txtFilter.Text)



        'Dim PageRow As Integer = 27

        'If Mid(Session("UserUnit"), 1, 2) = "05" Or Mid(Session("UserUnit"), 1, 2) = "08" Then
        '    ViewState("UserId") = cboUser.SelectedValue
        'End If

        ''丟當年度所有預算科目to mydataset 逐科目列印
        'Dim sqlstr, qstr, strD, strC As String
        'sqlstr = "SELECT a.accyear,a.accno, " & _
        '         "CASE WHEN len(a.accno)=17 THEN c.accname+'－'+b.accname " & _
        '             " WHEN len(a.accno)>9 THEN d.accname+'－'+b.accname " & _
        '             " WHEN len(a.accno)<=9 THEN b.accname END AS accname, " & _
        ' "a.unit, a.bg1+a.bg2+a.bg3+a.bg4+a.bg5+a.up1+a.up2+a.up3+a.up4 as bgamt, a.totper, a.totUSE, a.ctrl  " & _
        ' "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO " & _
        ' "INNER JOIN ACCNAME d ON LEFT(a.ACCNO, 9) = d.ACCNO " & _
        ' "WHERE a.accyear=" & nudYear.Text & " and b.STAFF_NO = '" & Trim(ViewState("UserId")) & _
        ' "' and a.accno>='" & vxtStartNo.Text & "' and a.accno<='" & vxtEndNo.Text & _
        ' "' order by a.accno"
        'mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        'Dim PageNo As Integer = 0
        'For intD = 0 To mydataset.Tables("bgf010").Rows.Count - 1
        '    strAccno = mydataset.Tables("bgf010").Rows(intD).Item("accno")
        '    strAccname = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("accname"), "")
        '    bgamt = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("bgamt"), 0)
        '    used = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("totuse"), 0)
        '    unuse = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("totper"), 0)
        '    TotUnuse = 0 : TotUsed = 0 : subUnuse = 0 : subUsed = 0
        '    intR = 0
        '    '找資料
        '    '丟選定之當年度預算科目所有之開支to Grid2 
        '    sqlstr = "SELECT bgf020.bgno, bgf020.accyear, BGF020.accno, bgf020.date1, bgf020.date2, bgf020.amt1, bgf020.remark, " & _
        '             "bgf020.amt2, bgf020.amt3, bgf020.useableamt, ACCNAME.ACCNAME AS ACCNAME, bgf020.kind,bgf020.subject, bgf020.closemark, " & _
        '             "bgf030.date3, bgf030.date4, bgf030.useamt, bgf030.no_1_no  " & _
        '             "FROM BGF020 left outer JOIN bgf030 on bgf020.bgno=bgf030.bgno inner join ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
        '             " WHERE BGF020.ACCYEAR=" & nudYear.Text & " AND BGF020.accno='" & strAccno & "'" & _
        '             " and bgf020.date1 <='" & Master.Models.FullDate(dtpDateE.Text) & "'"
        '    If strFilter <> "" Then
        '        If rdoLikeAll.Checked Then
        '            sqlstr = sqlstr & " and bgf020.remark like '%" & strFilter & "%'"
        '        Else
        '            sqlstr = sqlstr & " and bgf020.remark like '" & strFilter & "%'"
        '        End If
        '    End If
        '    sqlstr = sqlstr & " ORDER BY BGF020.bgno"
        '    mydataset2 = Master.ADO.openmember(DNS_ACC, "BGF030", sqlstr)
        '    Dim sRow As Integer = 0
        '    For PageCnt As Integer = 1 To 999    '頁次
        '        sRow = PageNo * PageRow
        '        Dim text1 = "預算科目: " & Master.Models.FormatAccno(strAccno) & " " & strAccname
        '        ws.Cells(sRow + 1, 1).Value = text1
        '        ws.Cells(sRow + 1, 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left

        '        Dim text2 = "總預算:" & Format(bgamt, "###,###,###,###.#") & _
        '                              "   請購中:" & Format(unuse, "###,###,###,###.#") & _
        '                              "   已開支:" & Format(used, "###,###,###,###.#") & _
        '                              "   預算餘額:" & Format(bgamt - unuse - used, " ###,###,###,###.#")
        '        ws.Cells(sRow + 2, 1).Value = text2
        '        ws.Cells(sRow + 2, 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left

        '        Dim text3 = "列印日期:" & Master.Models.ShortDate(Session("userdate")) & _
        '                                "  頁次:" & PageNo + 1
        '        ws.Cells(sRow + 2, 8).Value = text3
        '        ws.Cells(sRow + 2, 8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right

        '        Dim range = ws.Cells(sRow + 3, 1, sRow + 27, 8)
        '        range.Style.Font.Size = 11
        '        range.Style.Font.Name = "標楷體"
        '        Dim table = ws.Tables.Add(range, "Table" + CStr(PageNo))
        '        table.ShowTotal = False
        '        table.TableStyle = TableStyles.Light18


        '        ws.Column(1).Width = 20 / 2
        '        ws.Column(2).Width = 20 / 2
        '        ws.Column(3).Width = 101 / 2
        '        ws.Column(4).Width = 28 / 2
        '        ws.Column(5).Width = 20 / 2
        '        ws.Column(6).Width = 28 / 2
        '        ws.Column(7).Width = 30 / 2
        '        ws.Column(8).Width = 13 / 2
        '        ws.Column(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left
        '        ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
        '        ws.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
        '        ws.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left

        '        ws.Cells(sRow + 3, 1).Value = "請購編號"
        '        ws.Cells(sRow + 3, 2).Value = "請購日期"
        '        ws.Cells(sRow + 3, 3).Value = "　　　　摘　　　　　要"
        '        ws.Cells(sRow + 3, 4).Value = "請購金額　."
        '        ws.Cells(sRow + 3, 5).Value = "開支日期"
        '        ws.Cells(sRow + 3, 6).Value = "開支金額　."
        '        ws.Cells(sRow + 3, 7).Value = " 受 款 人"
        '        ws.Cells(sRow + 3, 8).Value = "傳票"
        '        i = 3
        '        With mydataset2.Tables("bgf030")
        '            Do While i < PageRow
        '                If intR > .Rows.Count - 1 Then
        '                    If subUnuse <> 0 Or subUsed <> 0 Then
        '                        i += 1
        '                        If i > PageRow Then Exit Do
        '                        ws.Cells(sRow + i, 3).Value = "    合  計"
        '                        ws.Cells(sRow + i, 4).Value = Format(subUnuse, "###,###,###,###.#") '直接取用bgf010數據unuse 
        '                        ws.Cells(sRow + i, 6).Value = Format(subUsed, "###,###,###,###.#")
        '                    End If
        '                    If TotUnuse <> 0 Or TotUsed <> 0 Then
        '                        i += 1
        '                        If i > PageRow Then Exit Do
        '                        ws.Cells(sRow + i, 3).Value = "    累  計"
        '                        ws.Cells(sRow + i, 4).Value = Format(TotUnuse, "###,###,###,###.#") '直接取用bgf010數據unuse 
        '                        ws.Cells(sRow + i, 6).Value = Format(TotUsed, "###,###,###,###.#")
        '                    End If
        '                    PageCnt = 1000
        '                    Exit Do
        '                End If
        '                If Master.ADO.nz(.Rows(intR)("useamt"), 0) = 0 Then
        '                    TotUnuse += Master.ADO.nz(.Rows(intR)("useableamt"), 0)
        '                Else
        '                    TotUsed += Master.ADO.nz(.Rows(intR)("useamt"), 0)
        '                End If
        '                sqlstr = Master.ADO.nz(.Rows(intR)("date1"), "")
        '                sqlstr = dtpDateS.Text
        '                If Master.Models.FullDate(mydataset2.Tables("bgf030").Rows(intR)("date1")) < Master.Models.FullDate(dtpDateS.Text) Then  '小於起印日前
        '                    '不印
        '                Else
        '                    If Master.ADO.nz(.Rows(intR)("useamt"), 0) = 0 Then
        '                        subUnuse += Master.ADO.nz(.Rows(intR)("useableamt"), 0)
        '                    Else
        '                        subUsed += Master.ADO.nz(.Rows(intR)("useamt"), 0)
        '                    End If
        '                    i += 1
        '                    If i > PageRow Then Exit Do
        '                    ws.Cells(sRow + i, 1).Value = Master.ADO.nz(.Rows(intR)("bgno"), "")
        '                    ws.Cells(sRow + i, 2).Value = Master.Models.RShortDate(Master.ADO.nz(.Rows(intR)("date1"), ""))
        '                    ws.Cells(sRow + i, 3).Value = Master.ADO.nz(.Rows(intR)("remark"), "")
        '                    ws.Cells(sRow + i, 4).Value = Format(Master.ADO.nz(.Rows(intR)("useableamt"), 0), "###,###,###,###.#")
        '                    ws.Cells(sRow + i, 5).Value = Master.Models.RShortDate(Master.ADO.nz(.Rows(intR)("date3"), ""))
        '                    ws.Cells(sRow + i, 6).Value = Format(Master.ADO.nz(.Rows(intR)("useamt"), 0), "###,###,###,###.#")
        '                    ws.Cells(sRow + i, 7).Value = Mid(Master.ADO.nz(.Rows(intR)("subject"), ""), 1, 7)  '7個字
        '                    ws.Cells(sRow + i, 8).Value = Mid(Master.ADO.nz(.Rows(intR)("no_1_no"), 0), 1, 7)  '7個字

        '                End If
        '                intR += 1
        '            Loop
        '        End With
        '        PageNo += 1
        '        ws.Row(PageNo * PageRow).PageBreak = True

        '    Next
        'Next
        'package.Save()
        'Response.WriteFile(savePathpdf)
        'Response.Flush()
        'Response.[End]()







        'Dim filename As String = "test.xlsx"
        'Response.Clear()
        'Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        'Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", filename))

        'Dim outputDir As DirectoryInfo

        'Dim newFile As New FileInfo("D:\test.xlsx")
        'If newFile.Exists Then
        '    newFile.Delete()
        '    ' ensures we create a new workbook
        '    newFile = New FileInfo("D:\test.xlsx")
        'End If


        'Using package As New ExcelPackage(newFile)

        '    Dim ws = package.Workbook.Worksheets.Add("Sample2")

        '    ws.PrinterSettings.PaperSize = ePaperSize.A4
        '    ws.PrinterSettings.Orientation = eOrientation.Landscape

        '    Dim PageNo As Integer = 0
        '    Dim PageRow As Integer = 27

        '    For PageCnt As Integer = 1 To 10
        '        Dim startRow = PageNo * PageRow + 2
        '        Dim startColumn = 1
        '        Dim endRow = PageNo * PageRow + 27
        '        Dim endColumn = 7

        '        ws.Cells(PageNo * PageRow + 1, 1).Value = PageNo
        '        Dim range = ws.Cells(startRow, startColumn, endRow, endColumn)
        '        Dim table = ws.Tables.Add(range, "Q" + PageNo.ToString)
        '        table.TableStyle = TableStyles.Light2
        '        ws.Row(PageNo * PageRow + 27).PageBreak = True
        '        PageNo = PageNo + 1
        '    Next



        '    package.Save()
        'End Using

        'Response.WriteFile("D:\test.xlsx")
        'Response.Flush()
        'Response.[End]()

        'Dim intR As Integer = 0  'control record number
        'Dim intD, i As Integer
        'Dim retstr, strRemark, strAccno, strAccname As String
        'Dim bgamt, used, unuse, TotUnuse, TotUsed, subUnuse, subUsed As Decimal
        'Dim strFilter As String = Trim(txtFilter.Text)

        'Dim filename As String = "test.xlsx"

        'Response.Clear()
        'Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        'Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", filename))


        'Dim workbook As New XSSFWorkbook()
        'Dim sheet1 As ISheet = workbook.CreateSheet("Sheet1")

        'sheet1.PrintSetup.PaperSize = 9
        'sheet1.PrintSetup.Landscape = True


        'Dim PageRow As Integer = 27

        'If Mid(Session("UserUnit"), 1, 2) = "05" Or Mid(Session("UserUnit"), 1, 2) = "08" Then
        '    ViewState("UserId") = cboUser.SelectedValue
        'End If

        ''丟當年度所有預算科目to mydataset 逐科目列印
        'Dim sqlstr, qstr, strD, strC As String
        'sqlstr = "SELECT a.accyear,a.accno, " & _
        '         "CASE WHEN len(a.accno)=17 THEN c.accname+'－'+b.accname " & _
        '             " WHEN len(a.accno)>9 THEN d.accname+'－'+b.accname " & _
        '             " WHEN len(a.accno)<=9 THEN b.accname END AS accname, " & _
        ' "a.unit, a.bg1+a.bg2+a.bg3+a.bg4+a.bg5+a.up1+a.up2+a.up3+a.up4 as bgamt, a.totper, a.totUSE, a.ctrl  " & _
        ' "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO " & _
        ' "INNER JOIN ACCNAME d ON LEFT(a.ACCNO, 9) = d.ACCNO " & _
        ' "WHERE a.accyear=" & nudYear.Text & " and b.STAFF_NO = '" & Trim(ViewState("UserId")) & _
        ' "' and a.accno>='" & vxtStartNo.Text & "' and a.accno<='" & vxtEndNo.Text & _
        ' "' order by a.accno"
        'mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        'Dim PageNo As Integer = 0
        'For intD = 0 To mydataset.Tables("bgf010").Rows.Count - 1
        '    strAccno = mydataset.Tables("bgf010").Rows(intD).Item("accno")
        '    strAccname = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("accname"), "")
        '    bgamt = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("bgamt"), 0)
        '    used = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("totuse"), 0)
        '    unuse = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("totper"), 0)
        '    TotUnuse = 0 : TotUsed = 0 : subUnuse = 0 : subUsed = 0
        '    intR = 0
        '    '找資料
        '    '丟選定之當年度預算科目所有之開支to Grid2 
        '    sqlstr = "SELECT bgf020.bgno, bgf020.accyear, BGF020.accno, bgf020.date1, bgf020.date2, bgf020.amt1, bgf020.remark, " & _
        '             "bgf020.amt2, bgf020.amt3, bgf020.useableamt, ACCNAME.ACCNAME AS ACCNAME, bgf020.kind,bgf020.subject, bgf020.closemark, " & _
        '             "bgf030.date3, bgf030.date4, bgf030.useamt, bgf030.no_1_no  " & _
        '             "FROM BGF020 left outer JOIN bgf030 on bgf020.bgno=bgf030.bgno inner join ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
        '             " WHERE BGF020.ACCYEAR=" & nudYear.Text & " AND BGF020.accno='" & strAccno & "'" & _
        '             " and bgf020.date1 <='" & Master.Models.FullDate(dtpDateE.Text) & "'"
        '    If strFilter <> "" Then
        '        If rdoLikeAll.Checked Then
        '            sqlstr = sqlstr & " and bgf020.remark like '%" & strFilter & "%'"
        '        Else
        '            sqlstr = sqlstr & " and bgf020.remark like '" & strFilter & "%'"
        '        End If
        '    End If
        '    sqlstr = sqlstr & " ORDER BY BGF020.bgno"
        '    mydataset2 = Master.ADO.openmember(DNS_ACC, "BGF030", sqlstr)
        '    For PageCnt As Integer = 1 To 999    '頁次

        '        'Dim page As New FPPage
        '        Dim text1 As String = "預算科目: " & Master.Models.FormatAccno(strAccno) & " " & strAccname
        '        'page.AddText(text1)
        '        sheet1.CreateRow(PageNo * PageRow + 0).CreateCell(0).SetCellValue(text1)
        '        Dim text2 As String = "總預算:" & Format(bgamt, "###,###,###,###.#") & _
        '                              "   請購中:" & Format(unuse, "###,###,###,###.#") & _
        '                              "   已開支:" & Format(used, "###,###,###,###.#") & _
        '                              "   預算餘額:" & Format(bgamt - unuse - used, " ###,###,###,###.#")
        '        'page.AddText(text2)
        '        sheet1.CreateRow(PageNo * PageRow + 1).CreateCell(0).SetCellValue(text2)

        '        Dim text3 As String = "列印日期:" & Master.Models.ShortDate(Session("userdate")) & _
        '                                "  頁次:" & PageNo + 1
        '        sheet1.CreateRow(PageNo * PageRow + 1).CreateCell(5).SetCellValue(text3)
        '        'Dim table0 As New FPTable(0, 10, 260, 7 * PageRow, PageRow, 8)
        '        'table0.Font.Name = "標楷體"
        '        'table0.Font.Size = 11
        '        'table0.SetLineColor(Color.DarkBlue)
        '        'table0.OutlineThicken(4)
        '        sheet1.SetColumnWidth(0, 20 * 256)
        '        sheet1.SetColumnWidth(1, 20 * 256)
        '        sheet1.SetColumnWidth(2, 101 * 256)
        '        sheet1.SetColumnWidth(3, 28 * 256)
        '        sheet1.SetColumnWidth(4, 20 * 256)
        '        sheet1.SetColumnWidth(5, 28 * 256)
        '        sheet1.SetColumnWidth(6, 30 * 256)
        '        sheet1.SetColumnWidth(7, 13 * 256)

        '        sheet1.CreateRow(PageNo * PageRow + 2)
        '        sheet1.GetRow(PageNo * PageRow + 2).CreateCell(0).SetCellValue("請購編號")
        '        sheet1.GetRow(PageNo * PageRow + 2).CreateCell(1).SetCellValue("請購日期")
        '        sheet1.GetRow(PageNo * PageRow + 2).CreateCell(2).SetCellValue("　　　　摘　　　　　要")
        '        sheet1.GetRow(PageNo * PageRow + 2).CreateCell(3).SetCellValue("請購金額")
        '        sheet1.GetRow(PageNo * PageRow + 2).CreateCell(4).SetCellValue("開支日期")
        '        sheet1.GetRow(PageNo * PageRow + 2).CreateCell(5).SetCellValue("開支金額")
        '        sheet1.GetRow(PageNo * PageRow + 2).CreateCell(6).SetCellValue(" 受 款 人")
        '        sheet1.GetRow(PageNo * PageRow + 2).CreateCell(7).SetCellValue("傳票")

        '        'table0.ColumnStyles(3).HAlignment = StringAlignment.Near '整欄左靠
        '        'table0.ColumnStyles(4).HAlignment = StringAlignment.Far  '整欄右靠
        '        'table0.ColumnStyles(6).HAlignment = StringAlignment.Far  '整欄右靠
        '        'table0.ColumnStyles(7).HAlignment = StringAlignment.Near '整欄左靠
        '        'table0.Texts2D(1, 1).Text = "請購編號"
        '        'table0.Texts2D(1, 2).Text = "請購日期"
        '        'table0.Texts2D(1, 3).Text = "　　　　摘　　　　　要"
        '        'table0.Texts2D(1, 4).Text = "請購金額　."
        '        'table0.Texts2D(1, 5).Text = "開支日期"
        '        'table0.Texts2D(1, 6).Text = "開支金額　."
        '        'table0.Texts2D(1, 7).Text = " 受 款 人"
        '        'table0.Texts2D(1, 8).Text = "傳票"
        '        i = 1
        '        'With mydataset2.Tables("bgf030")
        '        '    Do While i < PageRow
        '        '        If intR > .Rows.Count - 1 Then
        '        '            If subUnuse <> 0 Or subUsed <> 0 Then
        '        '                i += 1
        '        '                If i > PageRow Then Exit Do
        '        '                table0.Texts2D(i, 3).Text = "    合  計"
        '        '                table0.Texts2D(i, 4).Text = Format(subUnuse, "###,###,###,###.#") '直接取用bgf010數據unuse 
        '        '                table0.Texts2D(i, 6).Text = Format(subUsed, "###,###,###,###.#")
        '        '            End If
        '        '            If TotUnuse <> 0 Or TotUsed <> 0 Then
        '        '                i += 1
        '        '                If i > PageRow Then Exit Do
        '        '                table0.Texts2D(i, 3).Text = "    累  計"
        '        '                table0.Texts2D(i, 4).Text = Format(TotUnuse, "###,###,###,###.#") '直接取用bgf010數據unuse 
        '        '                table0.Texts2D(i, 6).Text = Format(TotUsed, "###,###,###,###.#")
        '        '            End If
        '        '            PageCnt = 1000
        '        '            Exit Do
        '        '        End If
        '        '        If nz(.Rows(intR)("useamt"), 0) = 0 Then
        '        '            TotUnuse += nz(.Rows(intR)("useableamt"), 0)
        '        '        Else
        '        '            TotUsed += nz(.Rows(intR)("useamt"), 0)
        '        '        End If
        '        '        sqlstr = nz(.Rows(intR)("date1"), "")
        '        '        sqlstr = dtpDateS.Value
        '        '        If nz(.Rows(intR)("date1"), "") < dtpDateS.Value Then  '小於起印日前
        '        '            '不印
        '        '        Else
        '        '            If nz(.Rows(intR)("useamt"), 0) = 0 Then
        '        '                subUnuse += nz(.Rows(intR)("useableamt"), 0)
        '        '            Else
        '        '                subUsed += nz(.Rows(intR)("useamt"), 0)
        '        '            End If
        '        '            i += 1
        '        '            If i > PageRow Then Exit Do
        '        '            table0.Texts2D(i, 1).Text = nz(.Rows(intR)("bgno"), "")
        '        '            table0.Texts2D(i, 2).Text = ShortDate(nz(.Rows(intR)("date1"), ""))
        '        '            table0.Texts2D(i, 3).Text = nz(.Rows(intR)("remark"), "")
        '        '            table0.Texts2D(i, 4).Text = Format(nz(.Rows(intR)("useableamt"), 0), "###,###,###,###.#")
        '        '            table0.Texts2D(i, 5).Text = ShortDate(nz(.Rows(intR)("date3"), ""))
        '        '            table0.Texts2D(i, 6).Text = Format(nz(.Rows(intR)("useamt"), 0), "###,###,###,###.#")
        '        '            table0.Texts2D(i, 7).Text = Mid(nz(.Rows(intR)("subject"), ""), 1, 7)  '7個字
        '        '            table0.Texts2D(i, 8).Text = Mid(nz(.Rows(intR)("no_1_no"), 0), 1, 7)  '7個字
        '        '            table0.Texts2D(i, 3).Font.Size = 9
        '        '        End If
        '        '        intR += 1
        '        '    Loop
        '        'End With
        '        sheet1.SetRowBreak(PageNo * PageRow + PageRow - 1)
        '        PageNo += 1
        '        'page.Add(table0)
        '        'doc.AddPage(page)
        '    Next
        'Next






        ''sheet1.CreateRow(0).CreateCell(0).SetCellValue("This is a Sample")
        ''Dim x As Integer = 1
        ''For i As Integer = 1 To 15
        ''    Dim row As IRow = sheet1.CreateRow(i)
        ''    For j As Integer = 0 To 14
        ''        row.CreateCell(j).SetCellValue(System.Math.Max(System.Threading.Interlocked.Increment(x), x - 1))
        ''    Next
        ''Next
        ''sheet1.SetRowBreak(15)


        ''For i As Integer = 16 To 30
        ''    Dim row As IRow = sheet1.CreateRow(i)
        ''    For j As Integer = 0 To 14
        ''        row.CreateCell(j).SetCellValue(System.Math.Max(System.Threading.Interlocked.Increment(x), x - 1))
        ''    Next
        ''Next

        'Using f = File.Create("D:\test.xlsx")
        '    workbook.Write(f)
        'End Using
        'Response.WriteFile("D:\test.xlsx")
        'Response.Flush()
        'Response.[End]()

        ''Dim intR As Integer = 0  'control record number
        ''Dim intD, i As Integer
        ''Dim retstr, strRemark, strAccno, strAccname As String
        ''Dim bgamt, used, unuse, TotUnuse, TotUsed, subUnuse, subUsed As Decimal
        ''Dim strFilter As String = Trim(txtFilter.Text)
        ' ''Dim printer As FPPrinter = FPPrinter.SharedPrinter
        ' ''Dim doc As New FPDocument("推算簿列印")
        ' ''doc.DefaultPageSettings.PaperKind = Printing.PaperKind.A4
        ' ''doc.DefaultPageSettings.Landscape = True
        ' ''doc.DefaultFont = New Font("新細明體", 11) '標楷體
        ' ''doc.SetDefaultPageMargin(20, 10, 5, 5)   'left top right bottom



        ''Dim PageRow As Integer = 25

        ''If Mid(Session("UserUnit"), 1, 2) = "05" Or Mid(Session("UserUnit"), 1, 2) = "08" Then
        ''    ViewState("UserId") = cboUser.SelectedValue
        ''End If

        ' ''丟當年度所有預算科目to mydataset 逐科目列印
        ''Dim sqlstr, qstr, strD, strC As String
        ''sqlstr = "SELECT a.accyear,a.accno, " & _
        ''         "CASE WHEN len(a.accno)=17 THEN c.accname+'－'+b.accname " & _
        ''             " WHEN len(a.accno)>9 THEN d.accname+'－'+b.accname " & _
        ''             " WHEN len(a.accno)<=9 THEN b.accname END AS accname, " & _
        '' "a.unit, a.bg1+a.bg2+a.bg3+a.bg4+a.bg5+a.up1+a.up2+a.up3+a.up4 as bgamt, a.totper, a.totUSE, a.ctrl  " & _
        '' "FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO " & _
        '' "INNER JOIN ACCNAME d ON LEFT(a.ACCNO, 9) = d.ACCNO " & _
        '' "WHERE a.accyear=" & nudYear.Text & " and b.STAFF_NO = '" & Trim(ViewState("UserId")) & _
        '' "' and a.accno>='" & vxtStartNo.Text & "' and a.accno<='" & vxtEndNo.Text & _
        '' "' order by a.accno"
        ''mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        ''Dim PageNo As Integer = 0
        ''For intD = 0 To mydataset.Tables("bgf010").Rows.Count - 1
        ''    strAccno = mydataset.Tables("bgf010").Rows(intD).Item("accno")
        ''    strAccname = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("accname"), "")
        ''    bgamt = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("bgamt"), 0)
        ''    used = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("totuse"), 0)
        ''    unuse = Master.ADO.nz(mydataset.Tables("bgf010").Rows(intD).Item("totper"), 0)
        ''    TotUnuse = 0 : TotUsed = 0 : subUnuse = 0 : subUsed = 0
        ''    intR = 0
        ''    '找資料
        ''    '丟選定之當年度預算科目所有之開支to Grid2 
        ''    sqlstr = "SELECT bgf020.bgno, bgf020.accyear, BGF020.accno, bgf020.date1, bgf020.date2, bgf020.amt1, bgf020.remark, " & _
        ''             "bgf020.amt2, bgf020.amt3, bgf020.useableamt, ACCNAME.ACCNAME AS ACCNAME, bgf020.kind,bgf020.subject, bgf020.closemark, " & _
        ''             "bgf030.date3, bgf030.date4, bgf030.useamt, bgf030.no_1_no  " & _
        ''             "FROM BGF020 left outer JOIN bgf030 on bgf020.bgno=bgf030.bgno inner join ACCNAME ON BGF020.ACCNO = ACCNAME.ACCNO " & _
        ''             " WHERE BGF020.ACCYEAR=" & nudYear.Text & " AND BGF020.accno='" & strAccno & "'" & _
        ''             " and bgf020.date1 <='" & Master.Models.FullDate(dtpDateE.Text) & "'"
        ''    If strFilter <> "" Then
        ''        If rdoLikeAll.Checked Then
        ''            sqlstr = sqlstr & " and bgf020.remark like '%" & strFilter & "%'"
        ''        Else
        ''            sqlstr = sqlstr & " and bgf020.remark like '" & strFilter & "%'"
        ''        End If
        ''    End If
        ''    sqlstr = sqlstr & " ORDER BY BGF020.bgno"
        ''    mydataset2 = Master.ADO.openmember(DNS_ACC, "BGF030", sqlstr)
        ''    For PageCnt As Integer = 1 To 999    '頁次
        ''        Dim page As New FPPage
        ''        Dim text1 As New FPText("預算科目: " & FormatAccno(strAccno) & " " & strAccname, 0, 0)
        ''        page.AddText(text1)
        ''        Dim text2 As New FPText("總預算:" & Format(bgamt, "###,###,###,###.#") & _
        ''                              "   請購中:" & Format(unuse, "###,###,###,###.#") & _
        ''                              "   已開支:" & Format(used, "###,###,###,###.#") & _
        ''                              "   預算餘額:" & Format(bgamt - unuse - used, " ###,###,###,###.#"), 0, 5)
        ''        page.AddText(text2)
        ''        PageNo += 1
        ''        Dim text3 As New FPText("列印日期:" & ShortDate(TransPara.TransP("userdate")) & _
        ''                                "  頁次:" & PageNo, 210, 5)
        ''        page.AddText(text3)
        ''        Dim table0 As New FPTable(0, 10, 260, 7 * PageRow, PageRow, 8)
        ''        table0.Font.Name = "標楷體"
        ''        table0.Font.Size = 11
        ''        table0.SetLineColor(Color.DarkBlue)
        ''        table0.OutlineThicken(4)
        ''        table0.ColumnStyles(1).Width = 20
        ''        table0.ColumnStyles(2).Width = 20
        ''        table0.ColumnStyles(3).Width = 101
        ''        table0.ColumnStyles(4).Width = 28
        ''        table0.ColumnStyles(5).Width = 20
        ''        table0.ColumnStyles(6).Width = 28
        ''        table0.ColumnStyles(7).Width = 30
        ''        table0.ColumnStyles(8).Width = 13
        ''        table0.ColumnStyles(3).HAlignment = StringAlignment.Near '整欄左靠
        ''        table0.ColumnStyles(4).HAlignment = StringAlignment.Far  '整欄右靠
        ''        table0.ColumnStyles(6).HAlignment = StringAlignment.Far  '整欄右靠
        ''        table0.ColumnStyles(7).HAlignment = StringAlignment.Near '整欄左靠
        ''        table0.Texts2D(1, 1).Text = "請購編號"
        ''        table0.Texts2D(1, 2).Text = "請購日期"
        ''        table0.Texts2D(1, 3).Text = "　　　　摘　　　　　要"
        ''        table0.Texts2D(1, 4).Text = "請購金額　."
        ''        table0.Texts2D(1, 5).Text = "開支日期"
        ''        table0.Texts2D(1, 6).Text = "開支金額　."
        ''        table0.Texts2D(1, 7).Text = " 受 款 人"
        ''        table0.Texts2D(1, 8).Text = "傳票"
        ''        i = 1
        ''        With mydataset2.Tables("bgf030")
        ''            Do While i < PageRow
        ''                If intR > .Rows.Count - 1 Then
        ''                    If subUnuse <> 0 Or subUsed <> 0 Then
        ''                        i += 1
        ''                        If i > PageRow Then Exit Do
        ''                        table0.Texts2D(i, 3).Text = "    合  計"
        ''                        table0.Texts2D(i, 4).Text = Format(subUnuse, "###,###,###,###.#") '直接取用bgf010數據unuse 
        ''                        table0.Texts2D(i, 6).Text = Format(subUsed, "###,###,###,###.#")
        ''                    End If
        ''                    If TotUnuse <> 0 Or TotUsed <> 0 Then
        ''                        i += 1
        ''                        If i > PageRow Then Exit Do
        ''                        table0.Texts2D(i, 3).Text = "    累  計"
        ''                        table0.Texts2D(i, 4).Text = Format(TotUnuse, "###,###,###,###.#") '直接取用bgf010數據unuse 
        ''                        table0.Texts2D(i, 6).Text = Format(TotUsed, "###,###,###,###.#")
        ''                    End If
        ''                    PageCnt = 1000
        ''                    Exit Do
        ''                End If
        ''                If nz(.Rows(intR)("useamt"), 0) = 0 Then
        ''                    TotUnuse += nz(.Rows(intR)("useableamt"), 0)
        ''                Else
        ''                    TotUsed += nz(.Rows(intR)("useamt"), 0)
        ''                End If
        ''                sqlstr = nz(.Rows(intR)("date1"), "")
        ''                sqlstr = dtpDateS.Value
        ''                If nz(.Rows(intR)("date1"), "") < dtpDateS.Value Then  '小於起印日前
        ''                    '不印
        ''                Else
        ''                    If nz(.Rows(intR)("useamt"), 0) = 0 Then
        ''                        subUnuse += nz(.Rows(intR)("useableamt"), 0)
        ''                    Else
        ''                        subUsed += nz(.Rows(intR)("useamt"), 0)
        ''                    End If
        ''                    i += 1
        ''                    If i > PageRow Then Exit Do
        ''                    table0.Texts2D(i, 1).Text = nz(.Rows(intR)("bgno"), "")
        ''                    table0.Texts2D(i, 2).Text = ShortDate(nz(.Rows(intR)("date1"), ""))
        ''                    table0.Texts2D(i, 3).Text = nz(.Rows(intR)("remark"), "")
        ''                    table0.Texts2D(i, 4).Text = Format(nz(.Rows(intR)("useableamt"), 0), "###,###,###,###.#")
        ''                    table0.Texts2D(i, 5).Text = ShortDate(nz(.Rows(intR)("date3"), ""))
        ''                    table0.Texts2D(i, 6).Text = Format(nz(.Rows(intR)("useamt"), 0), "###,###,###,###.#")
        ''                    table0.Texts2D(i, 7).Text = Mid(nz(.Rows(intR)("subject"), ""), 1, 7)  '7個字
        ''                    table0.Texts2D(i, 8).Text = Mid(nz(.Rows(intR)("no_1_no"), 0), 1, 7)  '7個字
        ''                    table0.Texts2D(i, 3).Font.Size = 9
        ''                End If
        ''                intR += 1
        ''            Loop
        ''        End With
        ''        page.Add(table0)
        ''        doc.AddPage(page)
        ''    Next
        ''Next
    End Sub
End Class