Imports System.Data.SqlClient
Imports System.IO

Public Class index
    Inherits System.Web.UI.Page

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString

#Region "類別模組"
    '資料庫
    Dim ADO As New ADO
    '系統
    Dim Controller As New Controller
    Dim Models As New Models

    '自訂
    Dim ACC As New ACC
#End Region
#Region "資料庫共用變數"
    Dim objCon As SqlConnection
    Dim objCmd As SqlCommand
    Dim objDR As SqlDataReader
    Dim strSQL As String
#End Region
#Region "共用變數"
    Dim I As Integer '累進變數
    Dim strMessage As String = "" '訊息字串
    Dim strIRow, strIValue, strUValue, strWValue As String '資料處理參數(新增欄位；新增資料；異動資料；條件)
#End Region

#Region "Page及功能操作"
    Private Sub index_Init(sender As Object, e As EventArgs) Handles Me.Init        
        If Request("t") = "out" Then ACC.SystemLoginAndLogout(Session("USERID"), "O", "S", "登出成功") : Session.Clear() : Response.Write("<script>top.location.href='index.aspx';</script>") '系統登出

        '++ 物件初始化 ++
        'TextBox*****
        txtNowDate.Text = Models.strStrToDate(Models.NowDate()) '當日日期

        '定月報日期
        Dim yy, mm, dd As Integer
        yy = CDbl(Mid(txtNowDate.Text, 1, 3))
        mm = CDbl(Mid(txtNowDate.Text, 5, 2))
        dd = CDbl(Mid(txtNowDate.Text, 8, 2))

        If mm = 1 And dd <= 10 Then
            txtNowDate.Text = yy - 1 & "-12-30" '元月時為上年年底
        End If


        Dim sqlstr As String = "select * FROM  a_sys_name ORDER BY SORT"

        Controller.objDropDownListOptionEX(cbos_system_id, DNS_SYS, sqlstr, "s_system_id", "s_system_name", 0)

        'From*****
        '是否有記住登入帳號
        If Not Request.Cookies("UNAME") Is Nothing Then
            txtUserName.Text = Request.Cookies("UNAME").Value
            If Not Request.Cookies("USYS") Is Nothing Then
                cbos_system_id.SelectedValue = Request.Cookies("USYS").Value
            End If
            txtRemember.Checked = True
        End If

        'Other*****
        Dim strLae As String = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "s_unit_id", "default_value = 'Y'") '系統安裝水利會

        txtLogo.Text = ACC.objLaeLogo("i", strLae)
        txtUserName.Focus()

        'Button*****
        btnCreateReportTable.Visible = IIf(Request.Url.Host = "127.0.0.1" Or Request.Url.Host = "localhost", True, False)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            '預載程序*****   
            InformationData()             '重要訊息發佈
        End If
    End Sub

    '登入
    Protected Sub butSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        '防呆*****
        'TextBox
        Dim objTextBox() As TextBox = {txtUserName, txtPassword, txtNowDate}
        Dim strTextBox As String = "帳號,密碼,日期,"
        strMessage = Controller.TextBox_Input(objTextBox, 0, strTextBox)
        If strMessage <> "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & strMessage & "】未輸入!!');history.back();", True) : Exit Sub


        '記住我的帳號*****
        'Cookies
        If txtRemember.Checked = True Then
            Response.Cookies("UNAME").Value = txtUserName.Text    '記住帳號
            Response.Cookies("USYS").Value = cbos_system_id.SelectedValue    '記住帳號
            Response.Cookies("UNAME").Expires = Now.AddDays(10)   '有效期限
            Response.Cookies("USYS").Expires = Now.AddDays(10)   '有效期限
        Else
            Response.Cookies("UNAME").Expires = Now.AddDays(-365) '有效期限
            Response.Cookies("USYS").Expires = Now.AddDays(-365) '有效期限
        End If


        '開啟登入*****
        Dim strUnit As String = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "default_value", "") '系統使用單位

        LoginBasic(strUnit) '一般登入
    End Sub

    '重填
    Protected Sub butReset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReset.Click
        '清空欄位值
        txtUserName.Text = ""
        txtPassword.Text = ""
        txtUserName.Focus()
    End Sub
#End Region

#Region "副程式"
    '一般登入
    Sub LoginBasic(ByVal strUnitID As String)
        objCon = New SqlConnection(DNS_SYS)
        objCon.Open()

        strSQL = "SELECT * FROM users"
        strSQL &= " WHERE user_id = @id or employee_id = @id1 "

        objCmd = New SqlCommand(strSQL, objCon)

        '避免SQL injection
        objCmd.Parameters.AddWithValue("@id", txtUserName.Text)
        objCmd.Parameters.AddWithValue("@id1", txtUserName.Text)

        objDR = objCmd.ExecuteReader

        If objDR.Read Then
            '資料驗證*****
            '一般
            If Trim(objDR("password").ToString) <> Trim(txtPassword.Text) Then ACC.SystemLoginAndLogout(txtUserName.Text, "I", "F", "密碼輸入錯誤") : ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('密碼輸入錯誤!!');window.location.href='index.aspx';", True) : Exit Sub
            If Trim(objDR("login").ToString) = "N" Then ACC.SystemLoginAndLogout(txtUserName.Text, "I", "F", "帳號已關閉") : ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('帳號已關閉，若有問題，請洽詢您的系統管理員!!');window.location.href='index.aspx';", True) : Exit Sub

            '特殊
            If Trim(objDR("use_date_start").ToString) <> "" And Trim(objDR("use_date_end").ToString) <> "" Then If Trim(objDR("use_date_start").ToString) > Models.NowDate() Or Trim(objDR("use_date_end").ToString) < Models.NowDate() Then ACC.SystemLoginAndLogout(txtUserName.Text, "I", "F", "帳號目前已超過或未到允許使用時間") : ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('帳號目前已超過或未到允許使用時間!!');window.location.href='index.aspx';", True) : Exit Sub
            If Trim(objDR("ip").ToString) <> "" Then If Trim(objDR("ip").ToString) <> Request.ServerVariables("REMOTE_ADDR") Then ACC.SystemLoginAndLogout(txtUserName.Text, "I", "F", "該帳號登入主機之IP不符") : ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('該帳號登入主機之IP不符!!');window.location.href='index.aspx';", True) : Exit Sub

            '成功登入*****
            '將值寫入Session中            
            Session("POWERUSERID") = Trim(objDR("user_id").ToString)    '帳號     
            Session("USERID") = Trim(objDR("employee_id").ToString)    '帳號
            Session("USERNAME") = Trim(objDR("name").ToString)    '帳號
            Session("DATE") = txtNowDate.Text                      '操作記帳日期
            Session("UserDate") = txtNowDate.Text    '登入日期
            Session("UserUnit") = Trim(objDR("unit_id").ToString)    '單位
            Session("sYear") = Mid(txtNowDate.Text, 1, 3)    '登入年度
            Session("sSeason") = Models.strDateChinessToSeason(txtNowDate.Text)    '登入季
            Session("LastDay") = Session("UserDate")

            Session("UnitTitle") = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "s_unit_name", "default_value = 'Y'") : Response.Cookies("ORGN").Expires = Now.AddDays(3) '使用單位名稱            
            Session("SUnitTitle") = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "s_unit_sname", "default_value = 'Y'")
            Session("SID") = Trim(objDR("unit_id").ToString)       '單位
            Session("PID") = Trim(objDR("post_id").ToString)       '職務
            Session("ORG") = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "s_unit_id", "default_value = 'Y'") '使用單位            


            '其他登入資訊寫入Cookies中
            Response.Cookies("NAME").Value = Trim(objDR("name").ToString) : Response.Cookies("NAME").Expires = Now.AddDays(3) '姓名
            Response.Cookies("ORGN").Value = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "s_unit_name", "default_value = 'Y'") : Response.Cookies("ORGN").Expires = Now.AddDays(3) '使用單位名稱            
            Response.Cookies("NNAME").Value = ADO.dbGetRow(DNS_SYS, "unit", "unit_name", "unit_id = '" & Trim(objDR("unit_id").ToString) & "'") : Response.Cookies("NNAME").Expires = Now.AddDays(3) '單位名稱


            '開啟主頁
            ACC.SystemLoginAndLogout(txtUserName.Text, "I", "S", "登入成功") '寫入登入登出記錄

            Response.Redirect("~/LAE10406/main.aspx?s=" + cbos_system_id.SelectedValue) '導入系統主頁
        Else
            ACC.SystemLoginAndLogout(txtUserName.Text, "I", "F", "查無此帳號") '寫入登入登出記錄
            ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('查無此帳號!!');window.location.href='index.aspx';", True)

            '物件預設值初始化
            txtUserName.Text = ""
            txtPassword.Text = ""
            txtNowDate.Text = Models.strStrToDate(Models.NowDate())
            txtUserName.Focus()
        End If

        objDR.Close()    '關閉連結        
        objCon.Close()
        objCmd.Dispose() '手動釋放資源
        objCon.Dispose()
        objCon = Nothing '移除指標
    End Sub

    '訊息發佈
    Public Sub InformationData()
        Dim strMessage As String = ""


        '開啟查詢
        objCon = New SqlConnection(DNS_SYS)
        objCon.Open()

        strSQL = "SELECT * FROM ("
        strSQL &= " SELECT * FROM news WHERE 1=1 AND issue_date_start = '' AND issue_date_end = ''"
        strSQL &= " UNION"
        strSQL &= " SELECT * FROM news WHERE 1=1 AND issue_date_start >= '" & Models.NowDate() & "' AND issue_date_end = ''"
        strSQL &= " UNION"
        strSQL &= " SELECT * FROM news WHERE 1=1 AND (issue_date_start <> '' OR issue_date_end <> '') AND issue_date_start = '' AND issue_date_end <= '" & Models.NowDate() & "'"
        strSQL &= " UNION"
        strSQL &= " SELECT * FROM news WHERE 1=1 AND (issue_date_start <> '' OR issue_date_end <> '') AND issue_date_start >= '" & Models.NowDate() & "' AND issue_date_end <= '" & Models.NowDate() & "'"
        strSQL &= " ) AS myTable1"
        strSQL &= " ORDER BY update_date DESC"

        objCmd = New SqlCommand(strSQL, objCon)
        objDR = objCmd.ExecuteReader

        Do While objDR.Read
            '發佈訊息*****            
            strMessage &= "<li style='color:#63c;'>"
            strMessage &= "【" & ACC.strIssueType(Trim(objDR("issue_type").ToString)) & "】"
            strMessage &= Trim(objDR("message_info").ToString)
            strMessage &= "</li>"
        Loop

        objDR.Close()    '關閉連結
        objCon.Close()
        objCmd.Dispose() '手動釋放資源
        objCon.Dispose()
        objCon = Nothing '移除指標


        '顯示訊息
        txtMessage.Text = "<ul>" & IIf(strMessage = "", "<li>&nbsp;</li>", strMessage) & "</ul>"
    End Sub

    Protected Sub btnCreateReportTable_Click(sender As Object, e As EventArgs) Handles btnCreateReportTable.Click
        Dim path As String = Request.PhysicalApplicationPath + "App_Data\Z01ACCTable.txt"
        Dim objReader As New StreamReader(path)

        Dim sLine As String = ""
        Dim sSql As String = ""
        Do
            sLine = objReader.ReadLine()
            If Not sLine Is Nothing Then

                If sLine.Trim = "GO" Then
                    ADO.runsql(DNS_ACC, sSql)
                    sSql = ""
                Else
                    sSql = sSql + sLine + vbNewLine
                End If
            End If
        Loop Until sLine Is Nothing
        objReader.Close()

        path = Request.PhysicalApplicationPath + "App_Data\Z02SYSTable.txt"
        Dim objReader1 As New StreamReader(path)

        sLine = ""
        sSql = ""
        Do
            sLine = objReader1.ReadLine()
            If Not sLine Is Nothing Then

                If sLine.Trim = "GO" Then
                    ADO.runsql(DNS_SYS, sSql)
                    sSql = ""
                Else
                    sSql = sSql + sLine + vbNewLine
                End If
            End If
        Loop Until sLine Is Nothing
        objReader1.Close()
    End Sub
#End Region
End Class