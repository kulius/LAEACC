Imports LAEACC
Imports Microsoft.AspNet.Identity
Imports System.Web.Configuration
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports CrystalDecisions.Web
Imports FastReport.Export.OoXML
Imports FastReport.Export.ExportBase

Public Class SiteMaster
    Inherits MasterPage

    '資料庫連線字串
    Public DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Public DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString

#Region "類別模組"
    '資料庫
    Public ADO As New ADO
    '系統
    Public Controller As New Controller
    Public Models As New Models
    '自訂
    Public ACC As New ACC
#End Region
#Region "資料庫共用變數"
    Dim objCon As SqlConnection
    Dim objCmd As SqlCommand
    Dim objDR As SqlDataReader
    Dim strSQL As String

    Dim objCon1 As SqlConnection
    Dim objCmd1 As SqlCommand
    Dim objDR1 As SqlDataReader
    Dim strSQL1 As String
#End Region

#Region "Page及功能操作"
    Protected Sub Page_Init(sender As Object, e As EventArgs)
        If Session("USERID") = "" Then Response.Redirect("/index.aspx") '導至首頁
        If Request("s") <> "" Then Session("SYSID") = Request("s") : Response.Write("<script>top.location.href='/LAE10406/main.aspx';</script>") '載入選擇系統

        '++ 物件初始化 ++
        txtLogo.Text = ACC.objLaeLogo("m", Session("ORG")) '顯示Logo
        txtSystemLink.Text = strOS_Link(Session("POWERUSERID")) '可操作系統權限


        'Cookies*****
        '顯示登入資訊
        If Not Request.Cookies("NAME") Is Nothing Then
            txtOrgName1.Text = Request.Cookies("ORGN").Value            
            txtName.Text = Request.Cookies("NAME").Value
            txtUnit.Text = Request.Cookies("NNAME").Value
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

        End If
    End Sub
#End Region

#Region "副程式"
#Region "特殊程序"

#End Region

#Region "系統"
    '可操作系統
    Function strOS_Link(ByVal UserID As String) As String
        Dim strString As String = ""

        objCon = New SqlConnection(DNS_SYS)
        objCon.Open()

        strSQL = "select DISTINCT c.s_system_id, d.s_system_name, d.sort "
        strSQL &= " from groups_power a "
        strSQL &= " inner join users_groups b on a.group_id=b.group_id and b.user_id = '" & UserID & "'"
        strSQL &= " INNER JOIN a_sys_nunit_item c on a.s_unitem_id=c.s_unitem_id"
        strSQL &= " INNER JOIN a_sys_name d ON d.s_system_id=c.s_system_id"
        strSQL &= " union "
        strSQL &= " select DISTINCT c.s_system_id, d.s_system_name, d.sort "
        strSQL &= " from groups_power a "
        strSQL &= " inner join unit_groups b on a.group_id=b.group_id and CHARINDEX(b.unit_id, '" & Session("UserUnit") & "')>0 "
        strSQL &= " INNER JOIN a_sys_nunit_item c on a.s_unitem_id=c.s_unitem_id"
        strSQL &= " INNER JOIN a_sys_name d ON d.s_system_id=c.s_system_id"

        objCmd = New SqlCommand(strSQL, objCon)
        objDR = objCmd.ExecuteReader

        strString = "<ul class=""header-dropdown-list hidden-xs"">"
        strString &= "<li>"
        strString &= "<a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown""> <img src=""/active/images/icon/icon_monitor_pc.gif"" /> <span> 權限操作系統 </span> <i class=""fa fa-angle-down""></i> </a>"
        strString &= "<ul class=""dropdown-menu pull-right"">"
        Do While objDR.Read
            Dim strURL As String = "javascript:window.location.href='/LAE10406/main.aspx?s=" & Trim(objDR("s_system_id").ToString) & "';"
            Dim strICon As String = "/active/images/icon/" & Trim(objDR("s_system_id").ToString) & ".gif"

            strString &= "<li" & IIf(Trim(objDR("s_system_id").ToString) = Session("SYSID"), " class='active'", "") & ">"
            strString &= "<a href=" & strURL & "><img src='" & strICon & "' class='flag flag-us'> " & Trim(objDR("s_system_name").ToString) & "(" & Trim(objDR("s_system_id").ToString) & ") </a>"
            strString &= "</li>"
        Loop
        strString &= "</ul>"
        strString &= "</li>"
        strString &= "</ul>"

        objDR.Close() '關閉連結        
        objCon.Close()
        objCmd.Dispose() '手動釋放資源
        objCon.Dispose()
        objCon = Nothing '移除指標


        Return strString
    End Function
    Function strOS_Button(ByVal UserID As String) As String
        Dim strString As String = ""
        Dim I As Integer = 0

        objCon = New SqlConnection(DNS_SYS)
        objCon.Open()

        strSQL = "select DISTINCT c.s_system_id, d.s_system_name, d.sort "
        strSQL &= " from groups_power a "
        strSQL &= " inner join users_groups b on a.group_id=b.group_id and b.user_id = '" & UserID & "'"
        strSQL &= " INNER JOIN a_sys_nunit_item c on a.s_unitem_id=c.s_unitem_id"
        strSQL &= " INNER JOIN a_sys_name d ON d.s_system_id=c.s_system_id"
        strSQL &= " union "
        strSQL &= " select DISTINCT c.s_system_id, d.s_system_name, d.sort "
        strSQL &= " from groups_power a "
        strSQL &= " inner join unit_groups b on a.group_id=b.group_id and CHARINDEX(b.unit_id, '" & Session("UserUnit") & "')>0 "
        strSQL &= " INNER JOIN a_sys_nunit_item c on a.s_unitem_id=c.s_unitem_id"
        strSQL &= " INNER JOIN a_sys_name d ON d.s_system_id=c.s_system_id"

        objCmd = New SqlCommand(strSQL, objCon)
        objDR = objCmd.ExecuteReader

        Do While objDR.Read
            strString &= "<input type='button' class='btn btn-primary' value='" & Trim(objDR("s_system_name").ToString) & "' onclick=""javascript:top.location.href='main.aspx?s=" & Trim(objDR("s_system_id").ToString) & "';"" />"
            strString &= "&nbsp;&nbsp;&nbsp;&nbsp;"

            I += 1
            If I Mod 5 = 0 Then strString &= "<p style='line-height:5px;'></p>"
        Loop

        objDR.Close() '關閉連結        
        objCon.Close()
        objCmd.Dispose() '手動釋放資源
        objCon.Dispose()
        objCon = Nothing '移除指標

        Return strString
    End Function

    '可操作系統權限
    Sub strUserMenuPower(ByVal SystemID As String, ByVal UserID As String)
        Dim strPageID As String = Request.QueryString("cvalue")

        '第一層選單*****
        objCon = New SqlConnection(DNS_SYS)
        objCon.Open()

        'If Session("ORG") = "all" Then
        '    strSQL = "SELECT DISTINCT m.s_unit_id, t1.s_unit_name, t1.s_unit_css, m.sort FROM a_sys_nunit_item m"
        '    strSQL &= " INNER JOIN a_sys_nunit t1 ON m.s_system_id = t1.s_system_id AND m.s_unit_id = t1.s_unit_id "
        '    strSQL &= " INNER JOIN users_power t2 ON m.s_unitem_id = t2.s_unitem_id"
        '    strSQL &= " WHERE m.s_system_id = '" & SystemID & "'"
        '    strSQL &= " AND t2.user_id = '" & UserID & "'"
        '    strSQL &= " union select s_unit_id, s_unit_name, s_unit_css, null from a_sys_nunit WHERE s_system_id = '" & SystemID & "'"
        '    strSQL &= " ORDER BY m.sort, m.s_unit_id ASC"
        'ElseIf Session("ORG") = "ylia" Then
        '    strSQL = "SELECT DISTINCT m.s_unit_id, t1.s_unit_name, t1.s_unit_css, m.sort FROM a_sys_nunit_item m"
        '    strSQL &= " INNER JOIN a_sys_nunit t1 ON m.s_system_id = t1.s_system_id AND m.s_unit_id = t1.s_unit_id "
        '    strSQL &= " INNER JOIN users_power t2 ON m.s_unitem_id = t2.s_unitem_id"
        '    strSQL &= " WHERE m.s_system_id = '" & SystemID & "'"
        '    strSQL &= " AND t2.user_id = '" & UserID & "'"
        '    strSQL &= " union select s_unit_id, s_unit_name, s_unit_css, null from a_sys_nunit WHERE (lae='ylia' or lae='all') and s_system_id = '" & SystemID & "'"
        '    strSQL &= " ORDER BY m.sort, m.s_unit_id ASC"
        'Else
        '    strSQL = "SELECT DISTINCT m.s_unit_id, t1.s_unit_name, t1.s_unit_css, m.sort FROM a_sys_nunit_item m"
        '    strSQL &= " INNER JOIN a_sys_nunit t1 ON m.s_system_id = t1.s_system_id AND m.s_unit_id = t1.s_unit_id "
        '    strSQL &= " INNER JOIN users_power t2 ON m.s_unitem_id = t2.s_unitem_id"
        '    strSQL &= " WHERE m.s_system_id = '" & SystemID & "'"
        '    strSQL &= " AND t2.user_id = '" & UserID & "'"
        '    strSQL &= " union select s_unit_id, s_unit_name, s_unit_css, null from a_sys_nunit WHERE (isnull(lae,'')<>'ylia' or lae='all') and s_system_id = '" & SystemID & "'"
        '    strSQL &= " ORDER BY m.sort, m.s_unit_id ASC"
        'End If

        strSQL = "SELECT DISTINCT m.s_unit_id, t1.s_unit_name, t1.s_unit_css FROM a_sys_nunit_item m"
        strSQL &= " INNER JOIN a_sys_nunit t1 ON m.s_system_id = t1.s_system_id AND m.s_unit_id = t1.s_unit_id "
        strSQL &= " INNER JOIN groups_power t2 ON m.s_unitem_id = t2.s_unitem_id"
        strSQL &= " inner join users_groups t3 on t2.group_id=t3.group_id and t3.user_id = '" & UserID & "'"
        strSQL &= " WHERE m.s_system_id = '" & SystemID & "'"
        strSQL &= " union "
        strSQL &= " SELECT DISTINCT m.s_unit_id, t1.s_unit_name, t1.s_unit_css FROM a_sys_nunit_item m"
        strSQL &= "  INNER JOIN a_sys_nunit t1 ON m.s_system_id = t1.s_system_id AND m.s_unit_id = t1.s_unit_id "
        strSQL &= "  INNER JOIN groups_power t2 ON m.s_unitem_id = t2.s_unitem_id"
        strSQL &= "  inner join unit_groups t3 on t2.group_id=t3.group_id and CHARINDEX(t3.unit_id, '" & Session("UserUnit") & "')>0 "
        strSQL &= "  WHERE m.s_system_id = '" & SystemID & "'"
        strSQL &= " ORDER BY m.s_unit_id"

        objCmd = New SqlCommand(strSQL, objCon)
        objDR = objCmd.ExecuteReader

        '將每個系統皆放入回首頁選項
        Response.Write("<li" & IIf(Path.GetFileName(Request.PhysicalPath) = "", " class='active'", "") & " style='font-size:14px;'>")
        Response.Write("<a href='/LAE10406/main.aspx' title='回首頁'><i class='fa fa-lg fa-fw fa-home'></i> <span class='menu-item-parent' style='font-size:16px;'>回首頁</span></a>")
        Response.Write("</li>")

        Do While objDR.Read
            Response.Write("<li>")
            Response.Write("<a href='#'><i class='fa fa-lg fa-fw " & IIf(Trim(objDR("s_unit_css").ToString) = "", "fa-asterisk", Trim(objDR("s_unit_css").ToString)) & "'></i> <span class='menu-item-parent' style='font-size:16px;'>" & Trim(objDR("s_unit_name").ToString) & "</span></a>")
            Response.Write("<ul>")


            '第二層選單*****
            objCon1 = New SqlConnection(DNS_SYS)
            objCon1.Open()

            'strSQL1 = "SELECT m.s_unitem_id, m.s_item_name,m.sort,m.s_system_id,m.s_unit_id,m.custom_path,m.custom_value FROM a_sys_nunit_item m"
            'strSQL1 &= " INNER JOIN users_power t1 ON m.s_unitem_id = t1.s_unitem_id"
            'strSQL1 &= " WHERE m.s_system_id = '" & SystemID & "'"
            'strSQL1 &= " AND t1.user_id = '" & UserID & "'"
            'strSQL1 &= " AND m.s_unit_id = '" & Trim(objDR("s_unit_id").ToString) & "'"
            'strSQL1 &= " union select s_unitem_id, s_item_name,sort,s_system_id,s_unit_id,custom_path,custom_value from a_sys_nunit_item WHERE s_system_id = '" & SystemID & "' AND s_unit_id = '" & Trim(objDR("s_unit_id").ToString) & "'"
            'strSQL1 &= " ORDER BY m.sort, m.s_unitem_id ASC"

            strSQL1 = "SELECT DISTINCT m.s_unitem_id, m.s_item_name,m.sort,m.s_system_id,m.s_unit_id,m.custom_path,m.custom_value FROM a_sys_nunit_item m"
            strSQL1 &= " INNER JOIN groups_power t1 ON m.s_unitem_id = t1.s_unitem_id"
            strSQL1 &= " inner join users_groups t2 on t1.group_id=t2.group_id and t2.user_id = '" & UserID & "'"
            strSQL1 &= " WHERE m.s_system_id = '" & SystemID & "'"
            strSQL1 &= " AND m.s_unit_id = '" & Trim(objDR("s_unit_id").ToString) & "'"
            strSQL1 &= " union "
            strSQL1 &= " SELECT DISTINCT m.s_unitem_id, m.s_item_name,m.sort,m.s_system_id,m.s_unit_id,m.custom_path,m.custom_value FROM a_sys_nunit_item m"
            strSQL1 &= " INNER JOIN groups_power t1 ON m.s_unitem_id = t1.s_unitem_id"
            strSQL1 &= " inner join unit_groups t2 on t1.group_id=t2.group_id and CHARINDEX(t2.unit_id, '" & Session("UserUnit") & "')>0 "
            strSQL1 &= " WHERE m.s_system_id = '" & SystemID & "'"
            strSQL1 &= " AND m.s_unit_id = '" & Trim(objDR("s_unit_id").ToString) & "'"
            strSQL1 &= " ORDER BY m.sort, m.s_unitem_id ASC"

            objCmd1 = New SqlCommand(strSQL1, objCon1)
            objDR1 = objCmd1.ExecuteReader

            Do While objDR1.Read
                'Dim strURL As String = "/LAE10406/" & Mid(Trim(objDR1("s_unitem_id").ToString), 1, 3) & "/" & Mid(Trim(objDR1("s_unitem_id").ToString), 4, 2) & "/" & Trim(objDR1("s_unitem_id").ToString) & ".aspx"
                Dim strURL As String = ""
                If Trim(objDR1("custom_path").ToString) <> "" Then
                    strURL = "/LAE10406/" & Trim(objDR1("s_system_id").ToString) & "/" & Trim(objDR1("custom_path").ToString) & ".aspx" & If(Trim(objDR1("custom_value").ToString) = "", "", "?cvalue=" & Trim(objDR1("custom_value").ToString))
                Else
                    strURL = "/LAE10406/" & Trim(objDR1("s_system_id").ToString) & "/" & Trim(objDR1("s_unit_id").ToString) & "/" & Trim(objDR1("s_unitem_id").ToString) & ".aspx" & If(Trim(objDR1("custom_value").ToString) = "", "", "?cvalue=" & Trim(objDR1("custom_value").ToString))
                End If
                '依呼叫的程式呈現選單預設值
                If strPageID <> "" Then
                    Response.Write("<li" & IIf(strPageID = Trim(objDR1("custom_value").ToString), " class='active'", "") & ">")
                Else
                    Response.Write("<li" & IIf(Path.GetFileName(Request.PhysicalPath) = Trim(objDR1("s_unitem_id").ToString), " class='active'", "") & ">")
                End If

                Response.Write("<a href='" & strURL & "' style='font-size:16px;'>" & Trim(objDR1("s_item_name").ToString) & "</a>")
                Response.Write("</li>")
            Loop



            objDR1.Close() '關閉連結        
            objCon1.Close()
            objCmd1.Dispose() '手動釋放資源
            objCon1.Dispose()
            objCon1 = Nothing '移除指標


            Response.Write("</ul>")
            Response.Write("</li>")
        Loop

        '將每個系統皆放入回上頁選項
        'Response.Write("<li class='active'>")
        'Response.Write("<a href='javascript: history.go(-1)' title='回上頁'><i class='fa fa-lg fa-fw fa-arrow-left'></i> <span class='menu-item-parent'>回上頁</span></a>")
        'Response.Write("</li>")

        objDR.Close() '關閉連結        
        objCon.Close()
        objCmd.Dispose() '手動釋放資源
        objCon.Dispose()
        objCon = Nothing '移除指標
    End Sub

    '導覽連結
    Function strBreadcrumb(ByVal strPageID As String) As String
        Dim s1 As String = Request.QueryString("cvalue")
        If s1 <> "" Then
            strPageID = s1
        End If
        Dim strString As String = ADO.dbGetRow(DNS_SYS, "a_sys_nunit_item", "s_item_name", "s_unitem_id = '" & strPageID & "'")

        If strString <> "" Then strString = "<li>" & ADO.dbGetRow(DNS_SYS, "a_sys_nunit_item", "s_item_name", "s_unitem_id = '" & strPageID & "'") & "</li>"

        Return strString
    End Function

    Sub PrintFR(ByVal ReportName As String, ByVal UnitPath As String, ByVal ConnStr As String, ByVal param As Dictionary(Of String, String))
        Dim FileName1 As String = Request.PhysicalApplicationPath + "App_Data\" + UnitPath + "\" + ReportName + ".frx"
        Dim FileName2 As String = Request.PhysicalApplicationPath + "App_Data\" + ReportName + ".frx"

        If System.IO.File.Exists(FileName1) Then
            Session("ReportPath") = FileName1
        ElseIf System.IO.File.Exists(FileName2) Then
            Session("ReportPath") = FileName2
        Else
            Exit Sub
        End If

        Session("ReportConnStr") = ConnStr
        Session("ReportParam") = param
        Session("ReportTitle") = ReportName

        Dim path As String = "/LAE10406/FRPrint.aspx"
        Me.Page.Controls.Add(New LiteralControl("<script>var w = window.open('" + path + "','_blank','menubar=no,status=no,scrollbars=no,top=0,left=0,toolbar=no,width=' + document.body.scrollWidth + ',height=' + document.body.scrollHeight); w.focus();</script>"))

        'Dim JavaScript As String = ""
        'JavaScript += "var mount = (screen.availHeight-600)/2;"
        'JavaScript += "var chasm = (screen.availWidth-600)/2;"
        'JavaScript += "window.open('" + path + "');"

        'ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "msg", JavaScript, True)

    End Sub

    Sub PrintFR1(ByVal ReportName As String, ByVal UnitPath As String, ByVal ConnStr As String, ByVal param As Dictionary(Of String, String))
        Dim FileName1 As String = Request.PhysicalApplicationPath + "App_Data\" + UnitPath + "\" + ReportName + ".frx"
        Dim FileName2 As String = Request.PhysicalApplicationPath + "App_Data\" + ReportName + ".frx"

        If System.IO.File.Exists(FileName1) Then
            Session("ReportPath") = FileName1
        ElseIf System.IO.File.Exists(FileName2) Then
            Session("ReportPath") = FileName2
        Else
            Exit Sub
        End If

        Session("ReportConnStr") = ConnStr
        Session("ReportParam1") = param
        Session("ReportTitle") = ReportName

        Dim path As String = "/LAE10406/FRPrint1.aspx"
        Me.Page.Controls.Add(New LiteralControl("<script>var w = window.open('" + path + "','_blank','menubar=no,status=no,scrollbars=no,top=0,left=0,toolbar=no,width=800,height=600'); w.focus();</script>"))

        'Dim JavaScript As String = ""
        'JavaScript += "var mount = (screen.availHeight-600)/2;"
        'JavaScript += "var chasm = (screen.availWidth-600)/2;"
        'JavaScript += "window.open('" + path + "');"

        'ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "msg", JavaScript, True)

    End Sub

    Sub PrintFRExcel(ByVal ReportName As String, ByVal UnitPath As String, ByVal ConnStr As String, ByVal param As Dictionary(Of String, String))
        Dim FileName1 As String = Request.PhysicalApplicationPath + "App_Data\" + UnitPath + "\" + ReportName + ".frx"
        Dim FileName2 As String = Request.PhysicalApplicationPath + "App_Data\" + ReportName + ".frx"

        If System.IO.File.Exists(FileName1) Then
            Session("ReportPath") = FileName1
        ElseIf System.IO.File.Exists(FileName2) Then
            Session("ReportPath") = FileName2
        Else
            Exit Sub
        End If

        Session("ReportConnStr") = ConnStr
        Session("ReportParam") = param
        Session("ReportTitle") = ReportName

        'Dim path As String = "/LAE10406/FRPrint.aspx"
        'Me.Page.Controls.Add(New LiteralControl("<script>var w = window.open('" + path + "','_blank','menubar=no,status=no,scrollbars=no,top=0,left=0,toolbar=no,width=800,height=600'); w.focus();</script>"))

        ' No temp files
        FastReport.Utils.Config.WebMode = True

        ' Set PDF export props
        Dim XLSExport As New FastReport.Export.OoXML.Excel2007Export


        ' Load our report
        Dim report As New FastReport.Report

        Dim evn As New FastReport.EnvironmentSettings()

        report.Load(Session("ReportPath"))
        'Dim ConnStr As String = "Data Source=KULIUSNB\SQL2012;AttachDbFilename=;Initial Catalog=accdb_db;Integrated Security=False;Persist Security Info=True;User ID=acc;Password=acc"
        report.SetParameterValue("ConnStr", Session("ReportConnStr"))


        For Each param1 In param
            report.SetParameterValue(param1.Key, param1.Value)
        Next

        report.Prepare()

        Dim strm As New MemoryStream()

        report.Export(XLSExport, strm)
        report.Dispose()
        XLSExport.Dispose()

        ' Stream the PDF back to the client as an attachment
        Response.ClearContent()
        Response.ClearHeaders()
        Response.Buffer = True

        Response.ContentType = "application/xlsx"
        Response.AddHeader("Content-Disposition", "attachment; filename=Excel_2007.xlsx")
        Response.BinaryWrite(strm.ToArray())

        'Response.ContentType = "application/vnd.ms-excel"
        'Response.AddHeader("Content-Disposition", "attachment; filename=MyExcelFileName.xlsx")
        '直接開啓的寫法
        'Response.AddHeader("Content-Disposition", "inline; filename=some_filename.xlsx")
        'Response.ContentType = "Application/vnd.ms-excel"

        'strm.Position = 0
        'strm.WriteTo(Response.OutputStream)
        'strm.Dispose()

        'Response.[End]()

    End Sub

    'Sub PrintFRExcel(ByVal ReportName As String, ByVal UnitPath As String, ByVal ConnStr As String, ByVal param As Dictionary(Of String, String))
    '    Dim FileName1 As String = Request.PhysicalApplicationPath + "App_Data\" + UnitPath + "\" + ReportName + ".frx"
    '    Dim FileName2 As String = Request.PhysicalApplicationPath + "App_Data\" + ReportName + ".frx"

    '    If System.IO.File.Exists(FileName1) Then
    '        Session("ReportPath") = FileName1
    '    ElseIf System.IO.File.Exists(FileName2) Then
    '        Session("ReportPath") = FileName2
    '    Else
    '        Exit Sub
    '    End If

    '    Session("ReportConnStr") = ConnStr
    '    Session("ReportParam") = param
    '    Session("ReportTitle") = ReportName

    '    Dim path As String = "/LAE10406/FRPrint.aspx"
    '    Me.Page.Controls.Add(New LiteralControl("<script>var w = window.open('" + path + "','_blank','menubar=no,status=no,scrollbars=no,top=0,left=0,toolbar=no,width=800,height=600'); w.focus();</script>"))

    '    'Dim JavaScript As String = ""
    '    'JavaScript += "var mount = (screen.availHeight-600)/2;"
    '    'JavaScript += "var chasm = (screen.availWidth-600)/2;"
    '    'JavaScript += "window.open('" + path + "');"

    '    'ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "msg", JavaScript, True)

    'End Sub
    Function PrintFRxls(ByVal ReportName As String, ByVal UnitPath As String, ByVal ConnStr As String, ByVal param As Dictionary(Of String, String))
        Dim FileName1 As String = Request.PhysicalApplicationPath + "App_Data\" + UnitPath + "\" + ReportName + ".frx"
        Dim FileName2 As String = Request.PhysicalApplicationPath + "App_Data\" + ReportName + ".frx"

        If System.IO.File.Exists(FileName1) Then
            Session("ReportPath") = FileName1
        ElseIf System.IO.File.Exists(FileName2) Then
            Session("ReportPath") = FileName2
        Else
            Exit Function
        End If

        Session("ReportConnStr") = ConnStr
        Session("ReportParam") = param
        Session("ReportTitle") = ReportName

        FastReport.Utils.Config.WebMode = True

        'Dim XLSExport As New FastReport.Export.Xml.XMLExport
        Dim XLSExport As New FastReport.Export.OoXML.Excel2007Export()

        Dim report As New FastReport.Report

        Dim evn As New FastReport.EnvironmentSettings()

        report.Load(Session("ReportPath"))
        report.SetParameterValue("ConnStr", Session("ReportConnStr"))


        For Each param1 In param
            report.SetParameterValue(param1.Key, param1.Value)
        Next

        Dim savePath As String = Server.MapPath("~/ExcelUploads/Export.xls")
        report.Prepare()
        report.Export(XLSExport, savePath)

        Dim filename As String = ReportName + ".xls"
        Response.Clear()
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", filename))

        Response.WriteFile(savePath)
        Response.Flush()
        Response.End()

    End Function

    Public Sub ExportDataTableToExcel(ByVal s_DataTable As DataTable)
        Dim tRowCount As Integer = s_DataTable.Rows.Count
        Dim tColumnCount As Integer = s_DataTable.Columns.Count

        Response.Expires = 0
        Response.Clear()
        Response.Buffer = True
        Response.Charset = "utf-8"
        Response.ContentEncoding = System.Text.Encoding.UTF8
        Response.ContentType = "application/vnd.ms-excel"
        '設定檔名可為中文_#1
        Response.AddHeader("Content-Disposition", "attachment;filename=excel.xls")

        '引用這三個xmlns
        Response.Write("<html xmlns:o='urn:schemas-microsoft-com:office:office'")
        Response.Write("xmlns:x='urn:schemas-microsoft-com:office:excel'")
        Response.Write("xmlns='http://www.w3.org/TR/REC-html40'>")

        Response.Write("<meta http-equiv=Content-Type content=text/html;charset=utf-8>")

        '在head中加入xml定義
        Response.Write("<head>")
        Response.Write("<xml>")
        Response.Write("<x:ExcelWorkbook>")
        Response.Write("<x:ExcelWorksheets>")
        Response.Write("<x:ExcelWorksheet>")
        '設定此Worksheet名稱_#2
        Response.Write("<x:Name>excel</x:Name>")

        '以下針對此工作表進行屬性設定
        Response.Write("<x:WorksheetOptions>")
        Response.Write("<x:FrozenNoSplit/>")

        '設定凍結行號_#3
        Response.Write("<x:SplitHorizontal>1</x:SplitHorizontal>")

        '設定起始行號(TopRowBottomPane)_#4
        Response.Write("<x:TopRowBottomPane>2</x:TopRowBottomPane>")
        Response.Write("<x:ActivePane>2</x:ActivePane>")
        Response.Write("</x:WorksheetOptions>")
        Response.Write("</xml>")
        Response.Write("</head>")

        Response.Write("<body>")
        Response.Write("<Table borderColor=black border=1>")
        Response.Write("<TR>")

        '塞入head
        Dim i As Integer
        For i = 0 To tColumnCount - 1 Step i + 1
            '設定head的背景色_#5
            Response.Write("<TD  bgcolor = #fff8dc>")
            Response.Write(s_DataTable.Columns(i).ColumnName)
            Response.Write("</TD>")
        Next

        Response.Write("</TR>")

        '塞入每一筆資料
        Dim j As Integer
        Dim k As Integer
        For j = 0 To tRowCount - 1 Step j + 1
            Response.Write("<TR>")
            k = 0
            For k = 0 To tColumnCount - 1 Step k + 1
                Response.Write("<TD style='mso-number-format:\\@;'>")
                Response.Write(s_DataTable.Rows(j)(k).ToString())
                Response.Write("</TD>")
            Next
            Response.Write("</TR>")
        Next

        Response.Write("</Table>")
        Response.Write("</body>")
        Response.Write("</html>")
        Response.End()
    End Sub
#End Region
#End Region
End Class
