Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class AC050
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

        dtpDate.Enabled = True
        btnSure.Enabled = True
        dtpDate.Text = Session("UserDate")
        If Month(Now) = 1 Then    'for year beginning,And Microsoft.VisualBasic.DateAndTime.Day(Now) < 10 let's the date=yy/12/31  
            dtpDate.Text = CStr(Year(Now) - 1) + "/12/31"
        End If

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '++ 控制頁面初始化 ++
        If Not IsPostBack Then
            'btnSure.Attributes.Add("onclick", "this.disabled = true;this.value = '處理中..';" + Page.ClientScript.GetPostBackEventReference(btnSure, ""))
        End If
    End Sub

    Public Sub MessageBx(ByVal sMessage As String)
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & sMessage & "】');", True)
    End Sub
#End Region



    Protected Sub btnSure_Click(sender As Object, e As EventArgs) Handles btnSure.Click
        btnSure.Enabled = False   '防止user再按
        Dim sqlstr, qstr, sortstr As String
        Dim intI, int1, int2, int3 As Integer
        int1 = 0
        int2 = 0
        int3 = 0
        dtpDate.Enabled = False
        btnSure.Enabled = False
        'System.Threading.Thread.Sleep(3000)

        Master.Models.FullDate(dtpDate.Text)
        sqlstr = "SELECT acf010.*, chf020.accno as bankaccno from acf010 LEFT OUTER JOIN chf020 ON acf010.bank=chf020.bank " & _
                 "where acf010.no_2_no<>0 and acf010.books<>'Y' and acf010.date_2<='" & Master.Models.FullDate(dtpDate.Text) & "'"
        myDatasetS = Master.ADO.openmember(DNS_ACC, "acf010s", sqlstr)
        If myDatasetS.Tables("acf010s").Rows.Count = 0 Then
            lblNo.Text = "已全過完帳,沒有資料待處理"
            Exit Sub
        End If
        lblNo.Text = "共有總帳" & myDatasetS.Tables("acf010s").Rows.Count & "筆待處理,處理中請稍待"
        'ProgressBar1.Visible = True
        'ProgressBar1.Maximum = myDatasetS.Tables("acf010s").Rows.Count
        For intI = 0 To myDatasetS.Tables("acf010s").Rows.Count - 1
            'ProgressBar1.Value = intI + 1   '顯示作業進度
            ViewState("sdate") = Master.Models.FullDate(myDatasetS.Tables("acf010s").Rows(intI).Item("date_2").ToShortDateString)   '將重要欄位置public變數
            ViewState("sYear") = Val(Mid(ViewState("sdate"), 1, 4)) - 1911 'Year(sdate) - 1911  because 2008/2/29 error
            If Mid(ViewState("sdate"), 5, 5) = "/2/29" Or Mid(ViewState("sdate"), 5, 6) = "/02/29" Then ' "2008/2/29" "2012/2/29"
                ViewState("sMm") = 2
            Else
                ViewState("sMm") = Month(ViewState("sdate"))
            End If
            ViewState("sNo") = myDatasetS.Tables("acf010s").Rows(intI).Item("no_2_no")
            ViewState("sKind") = myDatasetS.Tables("acf010s").Rows(intI).Item("kind")
            ViewState("sItem") = myDatasetS.Tables("acf010s").Rows(intI).Item("item")
            ViewState("sSeq") = myDatasetS.Tables("acf010s").Rows(intI).Item("seq")
            ViewState("sAmt") = myDatasetS.Tables("acf010s").Rows(intI).Item("amt")
            ViewState("sAccno") = myDatasetS.Tables("acf010s").Rows(intI).Item("accno")
            ViewState("sDC") = myDatasetS.Tables("acf010s").Rows(intI).Item("dc")
            ViewState("sBankAccno") = myDatasetS.Tables("acf010s").Rows(intI).Item("bankaccno").ToString
            ViewState("sAutono") = myDatasetS.Tables("acf010s").Rows(intI).Item("autono")
            '統計過帳張數
            If ViewState("sItem") = "1" And ViewState("sSeq") = "1" Then   '轉帳傳票
                If ViewState("sKind") = "3" Then int3 += 1
            End If
            If ViewState("sItem") = "9" Then     '收支傳票
                If ViewState("sKind") = "1" Then int1 += 1
                If ViewState("sKind") = "2" Then int2 += 1
            End If
            Call Upd_ACf070()   '產生日計檔
            Call Upd_AMT()      '過帳 UPDATE acf050 & acf060 & acf100 
            Call Upd_ACf010()   'update acf010->books = "Y"
        Next
        lblNo.Text = "共有總帳" & myDatasetS.Tables("acf010s").Rows.Count & "筆已處理"

        lblKind1.Text = lblKind1.Text & int1
        lblKind2.Text = lblKind2.Text & int2
        lblKind3.Text = lblKind3.Text & int3 & "    過帳完成"
        'myDatasetT = Nothing
        'myDatasetS = Nothing
        'btnExit.Text = "結束"
        'btnExit.Enabled = True
    End Sub

    Sub Upd_ACf070()   '產生日計檔
        '必須要將該日所有傳票,按相同之四級科目,依照銀行存款 專戶存款 or 轉帳之金額統計至acf070,以便於列印日計表及總分類帳
        Dim intJ, keyvalue As Integer
        Dim strJ, retstr As String
        sqlstr = "SELECT * from acf070 where date_2='" & ViewState("sdate") & "' and accno='" & ViewState("sAccno") & "'"
        mydataset = Master.ADO.openmember(DNS_ACC, "acf070s", sqlstr)
        If mydataset.Tables("acf070s").Rows.Count = 0 Then   '原無該日餘額則新增一筆
            Master.ADO.GenInsSql("date_2", ViewState("sdate"), "H")
            Master.ADO.GenInsSql("accno", ViewState("sAccno"), "T")
            sqlstr = "insert into acf070 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            sqlstr = "SELECT * from acf070 where date_2='" & ViewState("sdate") & "' and accno='" & ViewState("sAccno") & "'"
            mydataset = Master.ADO.openmember(DNS_ACC, "acf070s", sqlstr)
        End If
        keyvalue = mydataset.Tables("acf070s").Rows(0).Item("autono")
        If ViewState("sDC") = "1" Then '借方  由acf010 update acf070,acf010->借方各依銀行存款 專戶存款 or 轉帳,分別加入各欄金額
            If ViewState("sKind") = "3" Then   '轉帳
                Master.ADO.GenUpdsql("deamt3", mydataset.Tables("acf070s").Rows(0).Item("deamt3") + ViewState("sAmt"), "N")
            Else    '非轉帳時,要依該科目支出時是以銀行存款or專戶存款支出
                Select Case ViewState("sBankAccno")
                    Case "11102"
                        Master.ADO.GenUpdsql("deamt1", mydataset.Tables("acf070s").Rows(0).Item("deamt1") + ViewState("sAmt"), "N")
                    Case "11103"
                        Master.ADO.GenUpdsql("deamt2", mydataset.Tables("acf070s").Rows(0).Item("deamt2") + ViewState("sAmt"), "N")
                End Select
            End If
        Else             '貸方
            If ViewState("sKind") = "4" Then   '轉帳
                Master.ADO.GenUpdsql("cramt3", mydataset.Tables("acf070s").Rows(0).Item("cramt3") + ViewState("sAmt"), "N")
            Else     '非轉帳時,要依該科目收入時是以銀行存款or專戶存款收入
                Select Case ViewState("sBankAccno")
                    Case "11102"
                        Master.ADO.GenUpdsql("cramt1", mydataset.Tables("acf070s").Rows(0).Item("cramt1") + ViewState("sAmt"), "N")
                    Case "11103"
                        Master.ADO.GenUpdsql("cramt2", mydataset.Tables("acf070s").Rows(0).Item("cramt2") + ViewState("sAmt"), "N")
                End Select
            End If
        End If

        sqlstr = "update acf070 set " & Master.ADO.genupdfunc & " where autono=" & keyvalue
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then
            MessageBx("acf070 error sqlstr=" & sqlstr)
        End If
    End Sub

    Sub Upd_AMT()   '過帳
        Dim intK As Integer
        Call UPD_ACF050()   'acf010是總帳科目(四級)五位 
        If ViewState("sItem") = 1 Then  '表示有明細科目資料acf020待過帳
            sqlstr = "SELECT * from acf020 where no_2_no=" & ViewState("sNo") & " and accyear=" & ViewState("sYear") & " and kind='" & ViewState("sKind") & "' and seq='" & ViewState("sSeq") & "'"
            myDatasetT = Master.ADO.openmember(DNS_ACC, "acf020s", sqlstr)
            If myDatasetT.Tables("acf020s").Rows.Count = 0 Then
                MessageBx("acf020沒有明細科目資料,請檢查kind=" & ViewState("sKind") & ",號數=" & ViewState("sNo"))
                Exit Sub
            End If
            For intK = 0 To myDatasetT.Tables("acf020s").Rows.Count - 1
                ViewState("sAmt") = myDatasetT.Tables("acf020s").Rows(intK).Item("amt")
                ViewState("sAccno") = myDatasetT.Tables("acf020s").Rows(intK).Item("accno")
                ViewState("sDC") = myDatasetT.Tables("acf020s").Rows(intK).Item("dc")
                ViewState("sCotn_Code") = myDatasetT.Tables("acf020s").Rows(intK).Item("cotn_code")
                ViewState("sMat_qty") = Master.ADO.nz(myDatasetT.Tables("acf020s").Rows(intK).Item("Mat_qty"), 0)
                If Master.Models.Grade(ViewState("sAccno")) = 8 Then
                    If Mid(ViewState("sAccno"), 1, 3) = "113" Or Mid(ViewState("sAccno"), 1, 3) = "151" Or Mid(ViewState("sAccno"), 1, 3) = "212" Then
                        Call Upd_ACf060()  '應收 應付 催收 update acf060 
                    Else
                        Call UPD_ACF050()
                    End If
                    If Mid(ViewState("sAccno"), 1, 3) = "114" Or Mid(ViewState("sAccno"), 1, 3) = "112" Or (Mid(ViewState("sAccno"), 1, 2) = "13" And Mid(ViewState("sAccno"), 1, 5) <> "13701" And Mid(ViewState("sAccno"), 5, 1) <> "2") Then
                        Call Upd_ACf100()  '材料 update acf100 
                    End If
                    ViewState("sAccno") = Trim(Mid(ViewState("sAccno"), 1, 16))  '過完八級後,將科目設為七級
                End If

                If Master.Models.Grade(ViewState("sAccno")) = 7 Then
                    If Mid(ViewState("sAccno"), 1, 3) = "113" Or Mid(ViewState("sAccno"), 1, 3) = "151" Or Mid(ViewState("sAccno"), 1, 3) = "212" Then
                        Call Upd_ACf060()  '應收 應付 催收 update acf060 
                    Else
                        Call UPD_ACF050()
                    End If
                    If Mid(ViewState("sAccno"), 1, 3) = "114" Or Mid(ViewState("sAccno"), 1, 3) = "112" Or (Mid(ViewState("sAccno"), 1, 2) = "13" And Mid(ViewState("sAccno"), 1, 5) <> "13701" And Mid(ViewState("sAccno"), 5, 1) <> "2") Then
                        Call Upd_ACf100()  '材料 update acf100 
                    End If
                    ViewState("sAccno") = Trim(Mid(ViewState("sAccno"), 1, 9))   '過完七級後,將科目設為6級
                End If

                If Master.Models.Grade(ViewState("sAccno")) = 6 Then
                    If Mid(ViewState("sAccno"), 1, 3) = "113" Or Mid(ViewState("sAccno"), 1, 3) = "151" Or Mid(ViewState("sAccno"), 1, 3) = "212" Then
                        Call Upd_ACf060()  '應收 應付 催收 update acf060 
                    Else
                        Call UPD_ACF050()
                    End If
                    If Mid(ViewState("sAccno"), 1, 3) = "114" Or Mid(ViewState("sAccno"), 1, 3) = "112" Or (Mid(ViewState("sAccno"), 1, 2) = "13" And Mid(ViewState("sAccno"), 1, 5) <> "13701" And Mid(ViewState("sAccno"), 5, 1) <> "2") Then
                        Call Upd_ACf100()  '材料 update acf100 
                    End If
                    ViewState("sAccno") = Trim(Mid(ViewState("sAccno"), 1, 7))   '過完6級後,將科目設為5級
                End If

                If Master.Models.Grade(ViewState("sAccno")) = 5 Then
                    If Mid(ViewState("sAccno"), 1, 3) = "113" Or Mid(ViewState("sAccno"), 1, 3) = "151" Or Mid(ViewState("sAccno"), 1, 3) = "212" Then
                        Call Upd_ACf060()  '應收 應付 催收 update acf060 
                    Else
                        Call UPD_ACF050()              '過至5級即可,因4級已由acf010過完帳
                    End If
                    If Mid(ViewState("sAccno"), 1, 3) = "114" Or Mid(ViewState("sAccno"), 1, 3) = "112" Or (Mid(ViewState("sAccno"), 1, 2) = "13" And Mid(ViewState("sAccno"), 1, 5) <> "13701" And Mid(ViewState("sAccno"), 5, 1) <> "2") Then
                        Call Upd_ACf100()  '材料 update acf100 
                    End If
                End If

                '應收 應付 催收要加過4級
                If Mid(ViewState("sAccno"), 1, 3) = "113" Or Mid(ViewState("sAccno"), 1, 3) = "151" Or Mid(ViewState("sAccno"), 1, 3) = "212" Then
                    ViewState("sAccno") = Trim(Mid(ViewState("sAccno"), 1, 5))   '將科目設為4級
                    Call Upd_ACf060()  '應收 應付 催收 update acf060 
                End If

            Next
        End If
    End Sub


    Sub UPD_ACF050()  'update acf050四級科目 & acf100 
        Dim intJ, keyvalue As Integer
        Dim strJ, retstr, fieldname As String
        sqlstr = "SELECT * from acf050 where accyear=" & ViewState("sYear") & " and accno='" & ViewState("sAccno") & "'"
        mydataset = Master.ADO.openmember(DNS_ACC, "acf050s", sqlstr)
        If mydataset.Tables("acf050s").Rows.Count = 0 Then   '原無該科目餘額則新增一筆
            Master.ADO.GenInsSql("accyear", ViewState("sYear"), "N")
            Master.ADO.GenInsSql("accno", ViewState("sAccno"), "T")
            sqlstr = "insert into acf050 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            sqlstr = "SELECT * from acf050 where accyear=" & ViewState("sYear") & " and accno='" & ViewState("sAccno") & "'"
            mydataset = Master.ADO.openmember(DNS_ACC, "acf050s", sqlstr)
        End If
        keyvalue = mydataset.Tables("acf050s").Rows(0).Item("autono")
        For intJ = ViewState("sMm") To 12
            strJ = Format(intJ, "00")
            If ViewState("sDC") = "1" Then '借方  update acf050由交易月份起至12月止,在借方各加入傳票金額
                Master.ADO.GenUpdsql("deamt" & strJ, mydataset.Tables("acf050s").Rows(0).Item("deamt" & strJ) + ViewState("sAmt"), "N")
            Else              '貸方  update acf050由交易月份起至12月止,在貸方各加入傳票金額
                Master.ADO.GenUpdsql("cramt" & strJ, mydataset.Tables("acf050s").Rows(0).Item("cramt" & strJ) + ViewState("sAmt"), "N")
            End If
        Next
        sqlstr = "update acf050 set " & Master.ADO.genupdfunc & " where autono=" & keyvalue
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf050 error accno=" & ViewState("sAccno"))
    End Sub

    Sub Upd_ACf060()   '應收 應付 催收 update acf060
        Dim intJ, keyvalue As Integer
        Dim strJ, retstr, fieldname As String
        sqlstr = "SELECT * from acf060 where accyear=" & ViewState("sYear") & " and accno='" & ViewState("sAccno") & "'"
        mydataset = Master.ADO.openmember(DNS_ACC, "acf060s", sqlstr)
        If mydataset.Tables("acf060s").Rows.Count = 0 Then   '原無該科目餘額則新增一筆
            Master.ADO.GenInsSql("accyear", ViewState("sYear"), "N")
            Master.ADO.GenInsSql("accno", ViewState("sAccno"), "T")
            sqlstr = "insert into acf060 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            sqlstr = "SELECT * from acf060 where accyear=" & ViewState("sYear") & " and accno='" & ViewState("sAccno") & "'"
            mydataset = Master.ADO.openmember(DNS_ACC, "acf060s", sqlstr)
        End If
        keyvalue = mydataset.Tables("acf060s").Rows(0).Item("autono")
        For intJ = ViewState("sMm") To 12
            strJ = Format(intJ, "00")
            strJ = Format(intJ, "00")
            If ViewState("sDC") = "1" Then '借方  update acf060由交易月份起至12月止,在借方各加入傳票金額
                Master.ADO.GenUpdsql("deamt" & strJ, mydataset.Tables("acf060s").Rows(0).Item("deamt" & strJ) + ViewState("sAmt"), "N")
            Else              '貸方  update acf060由交易月份起至12月止,在貸方各加入傳票金額
                Master.ADO.GenUpdsql("cramt" & strJ, mydataset.Tables("acf060s").Rows(0).Item("cramt" & strJ) + ViewState("sAmt"), "N")
            End If
            Select Case ViewState("sCotn_Code")
                Case "1"
                    If (Mid(ViewState("sAccno"), 1, 1) = "1" And ViewState("sDC") = "1") Or (Mid(ViewState("sAccno"), 1, 1) = "2" And ViewState("sDC") = "2") Then '應收數增加,應付數增加
                        Master.ADO.GenUpdsql("account" & strJ, mydataset.Tables("acf060s").Rows(0).Item("account" & strJ) + ViewState("sAmt"), "N")
                    Else
                        Master.ADO.GenUpdsql("account" & strJ, mydataset.Tables("acf060s").Rows(0).Item("account" & strJ) - ViewState("sAmt"), "N")
                    End If

                Case "2"
                    If (Mid(ViewState("sAccno"), 1, 1) = "1" And ViewState("sDC") = "2") Or (Mid(ViewState("sAccno"), 1, 1) = "2" And ViewState("sDC") = "1") Then '實收數增加,實付數增加
                        Master.ADO.GenUpdsql("act" & strJ, mydataset.Tables("acf060s").Rows(0).Item("act" & strJ) + ViewState("sAmt"), "N")
                    Else
                        Master.ADO.GenUpdsql("act" & strJ, mydataset.Tables("acf060s").Rows(0).Item("act" & strJ) - ViewState("sAmt"), "N")
                    End If

                Case "3"
                    If (Mid(ViewState("sAccno"), 1, 1) = "1" And ViewState("sDC") = "2") Or (Mid(ViewState("sAccno"), 1, 1) = "2" And ViewState("sDC") = "1") Then '減免數增加,減免數增加
                        Master.ADO.GenUpdsql("sub" & strJ, mydataset.Tables("acf060s").Rows(0).Item("sub" & strJ) + ViewState("sAmt"), "N")
                    Else
                        Master.ADO.GenUpdsql("sub" & strJ, mydataset.Tables("acf060s").Rows(0).Item("sub" & strJ) - ViewState("sAmt"), "N")
                    End If

                Case "4"
                    If (Mid(ViewState("sAccno"), 1, 1) = "1" And ViewState("sDC") = "2") Or (Mid(ViewState("sAccno"), 1, 1) = "2" And ViewState("sDC") = "1") Then '減免數增加,減免數增加
                        Master.ADO.GenUpdsql("trans" & strJ, mydataset.Tables("acf060s").Rows(0).Item("trans" & strJ) + ViewState("sAmt"), "N")
                    Else
                        Master.ADO.GenUpdsql("trans" & strJ, mydataset.Tables("acf060s").Rows(0).Item("trans" & strJ) - ViewState("sAmt"), "N")
                    End If

            End Select
        Next
        sqlstr = "update acf060 set " & Master.ADO.genupdfunc & " where autono=" & keyvalue
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf060 error accno=" & ViewState("sAccno"))
    End Sub

    Sub Upd_ACf100()   '材料 update acf100
        Dim intJ, keyvalue As Integer
        Dim strJ, retstr, fieldname As String
        sqlstr = "SELECT * from acf100 where accyear=" & ViewState("sYear") & " and accno='" & ViewState("sAccno") & "'"
        mydataset = Master.ADO.openmember(DNS_ACC, "acf100s", sqlstr)
        If mydataset.Tables("acf100s").Rows.Count = 0 Then   '原無該科目餘額則新增一筆
            Master.ADO.GenInsSql("accyear", ViewState("sYear"), "N")
            Master.ADO.GenInsSql("accno", ViewState("sAccno"), "T")
            sqlstr = "insert into acf100 " & Master.ADO.GenInsFunc
            retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            sqlstr = "SELECT * from acf100 where accyear=" & ViewState("sYear") & " and accno='" & ViewState("sAccno") & "'"
            mydataset = Master.ADO.openmember(DNS_ACC, "acf100s", sqlstr)
        End If
        keyvalue = mydataset.Tables("acf100s").Rows(0).Item("autono")
        For intJ = ViewState("sMm") To 12
            strJ = Format(intJ, "00")
            If ViewState("sDC") = "1" Then '借方  update acf050由交易月份起至12月止,在借方各加入傳票金額
                Master.ADO.GenUpdsql("qtyin" & strJ, mydataset.Tables("acf100s").Rows(0).Item("qtyin" & strJ) + ViewState("sMat_qty"), "N")
            Else              '貸方  update acf050由交易月份起至12月止,在貸方各加入傳票金額
                Master.ADO.GenUpdsql("qtyout" & strJ, mydataset.Tables("acf100s").Rows(0).Item("qtyout" & strJ) + ViewState("sMat_qty"), "N")
            End If
        Next
        sqlstr = "update acf100 set " & Master.ADO.genupdfunc & " where autono=" & keyvalue
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf100 error accno=" & ViewState("sAccno"))
    End Sub

    Sub Upd_ACf010()   'update acf010->books = "Y"
        Dim strJ, retstr As String
        sqlstr = "update acf010 set books = 'Y' where autono=" & ViewState("sAutono")
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        If retstr <> "sqlok" Then MessageBx("acf010 error no_2_no=" & ViewState("sNo"))
    End Sub
End Class