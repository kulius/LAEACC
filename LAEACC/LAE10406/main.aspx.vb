Imports System.IO
Public Class main
    Inherits Page

#Region "@資料庫共用變數@"
    Dim strCSQL As String '查詢數量
    Dim strSSQL As String '查詢資料
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_PGM As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString
#End Region

#Region "Page及功能操作"
    Protected Sub Page_Init(sender As Object, e As EventArgs)
        '++ 物件初始化 ++
        'Button*****
        txtSystemButton.Text = Me.Master.strOS_Button(Session("POWERUSERID")) '可操作系統權限

        'Cookies*****
        If Not Request.Cookies("ORGN") Is Nothing Then
            txtOrgName.Text = Request.Cookies("ORGN").Value
        End If

        'Button*****
        btnCreateReportTable.Visible = IIf(Request.Url.Host = "127.0.0.1" Or Request.Url.Host = "localhost", True, False)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            '預載程序*****
            LoginLogoutData() '登入登出記錄
        End If
    End Sub
#End Region


#Region "副程式"
    '近幾筆登入登出記錄
    Public Sub LoginLogoutData()
        strSSQL = "SELECT TOP 10 * FROM b_sys_log"
        strSSQL &= " WHERE user_id <> ''"
        strSSQL &= " AND user_id = '" & Session("USERID") & "'"
        strSSQL &= " ORDER BY date DESC, time DESC"

        Me.Master.Controller.objDataGridStyle(DataGridView, "I")
        Me.Master.Controller.objDataGrid(DataGridView, txtCount, Me.Master.DNS_SYS, strSSQL, "登入登出記錄檔")
    End Sub
#End Region

    Protected Sub btnCreateReportTable_Click(sender As Object, e As EventArgs) Handles btnCreateReportTable.Click
        Dim path As String = Request.PhysicalApplicationPath + "App_Data\Z01ACCTable.txt"
        Dim objReader As New StreamReader(path)

        Dim sLine As String = ""
        Dim sSql As String = ""
        'Do
        '    sLine = objReader.ReadLine()
        '    If Not sLine Is Nothing Then

        '        If sLine.Trim = "GO" Then
        '            Master.ADO.runsql(DNS_ACC, sSql)
        '            sSql = ""
        '        Else
        '            sSql = sSql + sLine + vbNewLine
        '        End If
        '    End If
        'Loop Until sLine Is Nothing
        'objReader.Close()

        'path = Request.PhysicalApplicationPath + "App_Data\Z02SYSTable.txt"
        'Dim objReader1 As New StreamReader(path)

        'sLine = ""
        'sSql = ""
        'Do
        '    sLine = objReader1.ReadLine()
        '    If Not sLine Is Nothing Then

        '        If sLine.Trim = "GO" Then
        '            Master.ADO.runsql(DNS_SYS, sSql)
        '            sSql = ""
        '        Else
        '            sSql = sSql + sLine + vbNewLine
        '        End If
        '    End If
        'Loop Until sLine Is Nothing
        'objReader1.Close()

        path = Request.PhysicalApplicationPath + "App_Data\Z03PGMTable.txt"
        Dim objReader2 As New StreamReader(path)

        sLine = ""
        sSql = ""
        Do
            sLine = objReader2.ReadLine()
            If Not sLine Is Nothing Then

                If sLine.Trim = "GO" Then
                    Master.ADO.runsql(DNS_PGM, sSql)
                    sSql = ""
                Else
                    sSql = sSql + sLine + vbNewLine
                End If
            End If
        Loop Until sLine Is Nothing
        objReader2.Close()
    End Sub
End Class