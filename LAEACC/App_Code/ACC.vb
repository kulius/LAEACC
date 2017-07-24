'::::::::::::::::::::
'::: ACC自定義底層 ::
'::::::::::::::::::::
Imports System.Data
Imports System.Data.SqlClient

Public Class ACC
    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString
    Dim DNS_PGM As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString

#Region "類別模組"
    '資料庫
    Dim ADO As New ADO
    '系統
    Dim Controller As New Controller
    Dim Models As New Models
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

#Region "ACC副程式"
    '水利會LOGO
    Function objLaeLogo(ByVal type As String, ByVal strLaeNo As String) As String
        Dim strString As String = ""
        Dim strLogoB As String = "" : Dim strLogoS As String = ""
        Dim intWidth As Integer = 0 : Dim intHeight As Integer = 0

        '圖片大小
        Select Case type
            Case "i" '大圖
                strLogoB = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "logo_files_big", "s_unit_id = '" & strLaeNo & "'")
                intWidth = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "logo_files_big_width", "s_unit_id = '" & strLaeNo & "'")
                intHeight = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "logo_files_big_height", "s_unit_id = '" & strLaeNo & "'")

                strString = IIf(strLogoB = "", "<img src='/upload/logo_default_big.png' style='width:569px; height:40px;' />", "<img src='/upload/" & strLogoB & "' style='width:" & intWidth & "px; height:" & intHeight & "px;' />")
            Case "m" '小圖
                strLogoS = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "logo_files_small", "s_unit_id = '" & strLaeNo & "'")
                intWidth = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "logo_files_small_width", "s_unit_id = '" & strLaeNo & "'")
                intHeight = ADO.dbGetRow(DNS_SYS, "a_lae_unit", "logo_files_small_height", "s_unit_id = '" & strLaeNo & "'")

                strString = IIf(strLogoS = "", "<img src='/upload/logo_default_small.png' style='width:25px; height:25px;' />", "<img src='/upload/" & strLogoS & "' style='width:" & intWidth & "px; height:" & intHeight & "px;' />")
        End Select


        Return strString
    End Function
#End Region

#Region "系統操作記錄"
    '系統登入登出記錄
    Sub SystemLoginAndLogout(ByVal USERID As String, _
                             ByVal strStateID As String, ByVal strMessageID As String, ByVal strOperate As String)
        strIRow = "user_id,"
        strIRow &= "state_id,message_id,operate_note,"
        strIRow &= "ip,date,time"

        strIValue = "'" & USERID & "',"
        strIValue &= "'" & strStateID & "','" & strMessageID & "','" & strOperate & "',"
        strIValue &= "'" & HttpContext.Current.Request.ServerVariables("REMOTE_ADDR") & "','" & Models.NowDate & "','" & Models.NowTime & "'"

        ADO.dbInsert(DNS_SYS, "b_sys_log", strIRow, strIValue)
    End Sub

    '系統操作歷程記錄
    Sub SystemOperate(ByVal strUserID As String, ByVal strUnitemID As String, ByVal strTable As String, ByVal strID As String, _
                      ByVal strStateID As String, ByVal strMessageID As String)
        strIRow = "user_id,s_unitem_id,data_table_name,data_id,"
        strIRow &= "state_id,message_id,"
        strIRow &= "ip,date,time"

        strIValue = "'" & strUserID & "','" & strUnitemID & "','" & strTable & "','" & strID & "',"
        strIValue &= "'" & strStateID & "','" & strMessageID & "',"
        strIValue &= "'" & HttpContext.Current.Request.ServerVariables("REMOTE_ADDR") & "','" & Models.NowDate() & "','" & Models.NowTime() & "'"

        ADO.dbInsert(DNS_SYS, "b_sys_operate", strIRow, strIValue)
    End Sub
#End Region

#Region "常用代碼轉換"
#Region "一般文字轉換類"
    '基本*****
    Function strIssueType(ByVal strIssueID As String) As String
        Dim strString As String = ""

        Select Case strIssueID
            Case "A"
                strString = "公告"
            Case "B"
                strString = "異動"
            Case "C"
                strString = "新增"
            Case "E"
                strString = "修正"
        End Select

        Return strString
    End Function
#End Region

#Region "使用者自訂類"
    '單位
    Function strUnitName(ByVal UnitID As String) As String
        '條件項
        strWValue = "unit_id = '" & UnitID & "'"

        '查詢值       
        Dim strString As String = ADO.dbGetRow(DNS_SYS, "unit", "unit_name", strWValue)

        Return strString
    End Function
    '使用者
    Function strStaffName(ByVal StaffID As String) As String
        '條件項
        strWValue = "user_id = '" & StaffID & "'"

        '查詢值       
        Dim strString As String = ADO.dbGetRow(DNS_SYS, "users", "name", strWValue)

        Return strString
    End Function
#End Region

#Region "accdb_db"
    '會計科目
    Function strAccNoToName(ByVal ACCNO As String) As String
        '條件項
        strWValue = "ACCNO = '" & ACCNO & "'"

        '查詢值       
        Dim strString As String = ADO.dbGetRow(DNS_ACC, "ACCNAME", "ACCNAME", strWValue)

        Return strString
    End Function
#End Region
#End Region
End Class
