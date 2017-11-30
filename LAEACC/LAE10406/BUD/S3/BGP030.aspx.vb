Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml
Imports OfficeOpenXml.Style
Imports OfficeOpenXml.Table
Imports System.IO
Imports System.Xml
Imports System.Drawing

Public Class BGP030
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
    Dim mydataset, mydataset2 As DataSet
    Dim psDataSet As DataSet
#End Region

#Region "@Page及控制功能操作@"
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        strPage = System.IO.Path.GetFileName(Request.PhysicalPath) '取得檔案名稱

        ViewState("UserId") = Session("UserId")
        ViewState("UserUnit") = Session("UserUnit")


        nudYear.Text = Session("sYear")
        If Mid(ViewState("UserUnit"), 1, 2) = "05" Or Mid(ViewState("UserUnit"), 1, 2) = "08" Then
            cboUser.Visible = True
            sqlstr = "SELECT b.staff_no as userid, b.staff_no+USERTABLE.USERNAME as username FROM USERTABLE right outer JOIN" & _
                    " (SELECT STAFF_NO FROM ACCNAME WHERE STAFF_NO IS NOT NULL AND STAFF_NO <> '    ' GROUP BY STAFF_NO) b " & _
                    " ON USERTABLE.USERID = b.STAFF_NO" & _
                    " WHERE USERTABLE.USERNAME IS NOT NULL" & _
                    " order by usertable.userid"

            Master.Controller.objDropDownListOptionEX(cboUser, DNS_ACC, sqlstr, "userid", "username", 0)

            cboUser.Items.Add(New ListItem("全部", "全部"))
        End If


        vxtStartNo.Text = "1"    '起值
        vxtEndNo.Text = "59"    '迄值

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




    Protected Sub btnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click
        If Mid(ViewState("UserUnit"), 1, 2) = "05" Or Mid(ViewState("UserUnit"), 1, 2) = "08" Then
            ViewState("UserId") = cboUser.SelectedValue
        End If

        Dim Param As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Param.Add("UnitTitle", Session("UnitTitle"))    '水利會名稱
        Param.Add("UserUnit", Session("UserUnit"))  '使用者單位代號
        Param.Add("UserId", ViewState("UserId"))    '使用者代號
        Param.Add("nudYear", nudYear.Text)  '報表年度
        Param.Add("vxtStartNo", vxtStartNo.Text)    '起始會計科目
        Param.Add("vxtEndNo", vxtEndNo.Text)    '結束會計科目
        Param.Add("rdoSumYes", rdoSumYes.Checked)    '幾級科目

        Param.Add("sSeason", Session("sSeason"))    '第幾季
        Param.Add("UserDate", Session("UserDate"))    '登入日期

        CreateBGP030()

        Master.PrintFR("BGP030預算執行統計表", Session("ORG"), DNS_ACC, Param)
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click        
        sqlstr = "select"
        sqlstr &= " cast(a.accno as nvarchar) + cast(a.accname as nvarchar) as 預算科目及名稱,"
        sqlstr &= " Convert(Varchar(32),CONVERT(money,a.bgamt),1) as 預算總額,"
        sqlstr &= " Convert(Varchar(32),CONVERT(money,b.bg1 + b.bg2 + b.bg3 + b.bg4 + b.bg5),1) as 分配數額,"
        sqlstr &= " Convert(Varchar(32),CONVERT(money,a.totper),1) as 請購中金額,"
        sqlstr &= " Convert(Varchar(32),CONVERT(money,a.totUSE),1) as 已開支金額,"
        sqlstr &= " Convert(Varchar(32),CONVERT(money,a.bgamt - a.totUSE),1) as 分配數餘額,"
        sqlstr &= " Convert(Varchar(32),CONVERT(money,b.bg1 + b.bg2 + b.bg3 + b.bg4 + b.bg5 - b.TotUse),1) as 預算餘額"

        sqlstr &= " from BGP030 a"
        sqlstr &= " left join BGF010 b on a.accyear = b.ACCYEAR AND a.accno = b.accno"

        sqlstr &= " where 1=1"
        If cboUser.SelectedValue <> "" Then sqlstr &= " and a.userid = '" & cboUser.SelectedValue & "'"
        If cboUser.SelectedValue = "" Then sqlstr &= " and a.unit like '%" & Session("SID") & "%'"
        sqlstr &= " and a.accyear = '" & nudYear.Text & "'"
        sqlstr &= " and a.accno>='" & Trim(vxtStartNo.Text) & "'"
        sqlstr &= " and a.accno<='" & Trim(vxtEndNo.Text) & "'"
        sqlstr &= " ORDER BY a.accyear, a.accno"

        mydataset = Master.ADO.openmember(DNS_ACC, "Export", sqlstr)

        Master.ExportDataTableToExcel(mydataset.Tables("Export"))
    End Sub

    Sub CreateBGP030()
        Dim intR As Integer = 0  'control record number
        Dim intD, i As Integer
        Dim retstr, UnitName As String
        Dim sum2, sum3, sum4, sum5 As Decimal
        sum2 = 0 : sum3 = 0 : sum4 = 0 : sum5 = 0
        Dim amt2, amt3, amt4, amt5 As Decimal
        amt2 = 0 : amt3 = 0 : amt4 = 0 : amt5 = 0

        Dim tempAccno As String = ""
        Dim isFirst As Boolean = True
        Dim tempGrade As Integer = 0
        Dim sumBGamt, sumTotper, sumTotuse, sumSubBgamt As Decimal

        If Mid(ViewState("UserUnit"), 1, 2) = "05" Or Mid(ViewState("UserUnit"), 1, 2) = "08" Then
            ViewState("UserId") = cboUser.SelectedValue
        End If

        Dim strBg As String = ""
        Dim sSeason As Integer = Session("sSeason")
        Select Case sSeason
            Case Is = 1
                strBg = ", a.bg1+a.up1 as subbgamt "
            Case Is = 2
                strBg = ", a.bg1+a.bg2+a.up1+a.up2 as subbgamt "
            Case Is = 3
                strBg = ", a.bg1+a.bg2+a.bg3+a.up1+a.up2+a.up3 as subbgamt "
            Case Is = 4
                strBg = ", a.bg1+a.bg2+a.bg3+a.bg4+a.up1+a.up2+a.up3+a.up4 as subbgamt "
        End Select
        '丟當年度所有預算科目to mydataset 逐科目列印
        Dim sqlstr, qstr, strD, strC As String
        sqlstr = "SELECT a.accyear,a.accno, " & _
                 "CASE WHEN len(a.accno)=17 THEN c.accname+'－'+b.accname " & _
                     " WHEN len(a.accno)>9 THEN d.accname+'－'+b.accname " & _
                     " WHEN len(a.accno)<=9 THEN b.accname END AS accname, " & _
                     "a.unit, a.bg1+a.bg2+a.bg3+a.bg4+a.bg5+a.up1+a.up2+a.up3+a.up4 as bgamt, " & _
                     " a.totper, a.totUSE, a.ctrl, 'Y' as sumcode "
        sqlstr += strBg
        sqlstr += " FROM bgf010 a INNER JOIN ACCNAME b ON a.ACCNO = b.ACCNO INNER JOIN ACCNAME c ON LEFT(a.ACCNO, 16) = c.ACCNO " & _
         "INNER JOIN ACCNAME d ON LEFT(a.ACCNO, 9) = d.ACCNO WHERE a.accyear=" & nudYear.Text
        If Trim(ViewState("UserId")) <> "全部" Then
            sqlstr += " and b.STAFF_NO = '" & Trim(ViewState("UserId")) & "'"
        End If
        sqlstr += " and a.accno>='" & vxtStartNo.Text & "' and a.accno<='" & vxtEndNo.Text & _
                  "' order by a.accno"
        mydataset = Master.ADO.openmember(DNS_ACC, "BGF010", sqlstr)
        'labInfo1.Text = sqlstr

        '要統計    'first sum 8 grade
        If rdoSumYes.Checked Then
            Dim cutLen As Integer = 0
            Dim sumGrade As Integer = 0
            With mydataset.Tables(0)
                For intCycle As Integer = 7 To 4 Step -1
                    Select Case intCycle
                        Case 7
                            cutLen = 16
                            tempGrade = 7
                        Case 6
                            cutLen = 9
                            tempGrade = 6
                        Case 5
                            cutLen = 7
                            tempGrade = 5
                        Case 4
                            cutLen = 5
                            tempGrade = 4
                    End Select
                    intR = 0
                    isFirst = True
                    sumBGamt = 0 : sumTotper = 0 : sumTotuse = 0 : sumSubBgamt = 0
                    For intR = 0 To .Rows.Count - 1
                        If .Rows(intR).Item("sumcode") = "Y" And Trim(.Rows(intR).Item("accno")) <> Trim(Mid(.Rows(intR).Item("accno"), 1, cutLen)) Then
                            If isFirst Then
                                tempAccno = Trim(Mid(.Rows(intR).Item("accno"), 1, cutLen))
                                isFirst = False
                            End If
                            If Trim(Mid(.Rows(intR).Item("accno"), 1, cutLen)) <> tempAccno Then
                                If sumBGamt + sumTotper + sumTotuse <> 0 Then '要寫入一筆統計資料

                                    sqlstr = "SELECT * from accname where accno='" & tempAccno & "'"
                                    TempDataSet = Master.ADO.openmember(DNS_ACC, "user", sqlstr)
                                    If TempDataSet.Tables(0).Rows.Count > 0 And Master.Models.Grade(tempAccno) = tempGrade Then
                                        '有該統計科目時才寫入統計資料
                                        Dim nr As DataRow
                                        nr = mydataset.Tables(0).NewRow()
                                        nr("accyear") = nudYear.Text
                                        nr("accno") = tempAccno
                                        nr("accname") = Master.ADO.nz(TempDataSet.Tables(0).Rows(0).Item("accname"), "")
                                        nr("bgamt") = sumBGamt
                                        nr("totper") = sumTotper
                                        nr("totuse") = sumTotuse
                                        nr("ctrl") = ""
                                        nr("sumcode") = " "
                                        nr("subbgamt") = sumSubBgamt
                                        mydataset.Tables(0).Rows.Add(nr) '增行至dataset
                                    End If
                                    sumBGamt = 0 : sumTotper = 0 : sumTotuse = 0 : sumSubBgamt = 0

                                    tempAccno = Trim(Mid(.Rows(intR).Item("accno"), 1, cutLen))
                                End If
                            End If
                            sumBGamt += Master.ADO.nz(.Rows(intR).Item("bgamt"), 0)
                            sumTotper += Master.ADO.nz(.Rows(intR).Item("totper"), 0)
                            sumTotuse += Master.ADO.nz(.Rows(intR).Item("totuse"), 0)
                            sumSubBgamt += Master.ADO.nz(.Rows(intR).Item("subbgamt"), 0)
                        End If
                    Next
                    If sumBGamt + sumTotper + sumTotuse <> 0 Then 'end 要寫入一筆統計資料
                        sqlstr = "SELECT * from accname where accno='" & tempAccno & "'"
                        TempDataSet = Master.ADO.openmember(DNS_ACC, "user", sqlstr)
                        If TempDataSet.Tables(0).Rows.Count > 0 And Master.Models.Grade(tempAccno) = tempGrade Then
                            '有該統計科目時才寫入統計資料
                            Dim nr As DataRow
                            nr = mydataset.Tables(0).NewRow()
                            nr("accyear") = nudYear.Text
                            nr("accno") = tempAccno
                            nr("accname") = Master.ADO.nz(TempDataSet.Tables(0).Rows(0).Item("accname"), "")
                            nr("bgamt") = sumBGamt
                            nr("totper") = sumTotper
                            nr("totuse") = sumTotuse
                            nr("ctrl") = ""
                            nr("sumcode") = " "
                            nr("subbgamt") = sumSubBgamt
                            mydataset.Tables(0).Rows.Add(nr) '增行至dataset
                        End If
                        sumBGamt = 0 : sumTotper = 0 : sumTotuse = 0 : sumSubBgamt = 0
                    End If
                Next
            End With
        End If


        sqlstr = "delete BGP030 where userid='" & ViewState("UserId") & "'"    '先清空tempfile 
        retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
        'Label2.Text = sqlstr
        If retstr <> "sqlok" Then
            MessageBx("刪除BGp030失敗" & sqlstr)
            Exit Sub
        End If
        '將datagrid逐筆insert to bgp020 
        For intI As Integer = 0 To mydataset.Tables(0).Rows.Count - 1
            With mydataset.Tables(0)
                Master.ADO.GenInsSql("accno", .Rows(intI).Item("accno"), "T")
                Master.ADO.GenInsSql("accname", .Rows(intI).Item("accname"), "T")
                Master.ADO.GenInsSql("accyear", .Rows(intI).Item("accyear"), "N")
                Master.ADO.GenInsSql("unit", .Rows(intI).Item("unit").ToString, "T")
                Master.ADO.GenInsSql("bgamt", .Rows(intI).Item("bgamt"), "N")
                Master.ADO.GenInsSql("totper", .Rows(intI).Item("totper"), "N")
                Master.ADO.GenInsSql("totUSE", .Rows(intI).Item("totUSE"), "N")
                Master.ADO.GenInsSql("ctrl", .Rows(intI).Item("ctrl"), "T")
                Master.ADO.GenInsSql("sumcode", .Rows(intI).Item("sumcode"), "T")
                Master.ADO.GenInsSql("subbgamt", .Rows(intI).Item("subbgamt"), "N")

                Master.ADO.GenInsSql("userid", ViewState("UserId"), "T")
                sqlstr = "insert into BGP030 " & Master.ADO.GenInsFunc
                retstr = Master.ADO.runsql(DNS_ACC, sqlstr)
            End With
        Next

    End Sub
    

    'Protected Sub btnCreateReportTable_Click(sender As Object, e As EventArgs) Handles btnCreateReportTable.Click
    '    Dim path As String = Request.PhysicalApplicationPath + "App_Data\Z01CreateReportTable.txt"
    '    Dim objReader As New StreamReader(path)

    '    Dim sLine As String = ""
    '    Dim sSql As String = ""
    '    Do
    '        sLine = objReader.ReadLine()
    '        If Not sLine Is Nothing Then

    '            If sLine.Trim = "GO" Then
    '                Master.ADO.runsql(DNS_ACC, sSql)
    '                sSql = ""
    '            Else
    '                sSql = sSql + sLine + vbNewLine
    '            End If
    '        End If
    '    Loop Until sLine Is Nothing
    '    objReader.Close()
    'End Sub
End Class