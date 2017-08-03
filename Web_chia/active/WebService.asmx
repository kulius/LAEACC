<%@ WebService Language="VB" Class="WebService" %>

Imports System
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports LAEACC

<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<System.Web.Script.Services.ScriptService()> _
Public Class WebService
    Inherits System.Web.Services.WebService

    '--資料庫共用變數--
    Dim WSDNS As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString
    
#Region "類別模組"
    '資料庫
    Dim ADO As New ADO
    '系統
    Dim Controller As New Controller
    Dim Models As New Models
#End Region
#Region "資料庫共用變數"
    Dim WSobjCon As SqlConnection
    Dim WSobjCmd As SqlCommand
    Dim WSobjDR As SqlDataReader
    Dim WSstrSQL As String
#End Region
    
    
    '++ 查詢專用[資料未含特殊代號] ++
    
    '** 查詢摘要 **
    <WebMethod(EnableSession:=True)> _
    Public Function GetCompletionListRemark(ByVal prefixText As String, ByVal count As Integer) As String()
        If count = 0 Then
            count = 10
        End If
        
        '防呆
        If prefixText.Equals("%#)*)*)*DDD") Then
            Return New String(0) {}
        End If
                
        Dim i As Integer = 0
        Dim items As List(Of String) = New List(Of String)(count)
        
        
        '開啟查詢
        WSobjCon = New SqlConnection(WSDNS)
        WSobjCon.Open()
        
        If Trim(prefixText) = "" Then
            WSstrSQL = "SELECT psstr  FROM psname where unit='" & HttpContext.Current.Session("UserUnit") & "' and  seq<>9999 "
        Else
            WSstrSQL = "SELECT psstr  FROM psname where unit='" & HttpContext.Current.Session("UserUnit") & "' and  seq<>9999 and psstr LIKE '%" & prefixText & "%'"
        End If

        WSobjCmd = New SqlCommand(WSstrSQL, WSobjCon)
        WSobjDR = WSobjCmd.ExecuteReader

        Do While WSobjDR.Read
            items.Add(Trim(WSobjDR("psstr")))
            System.Math.Min(System.Threading.Interlocked.Increment(i), i - 1)
        Loop

        WSobjCon.Close()
        
        
        Return items.ToArray
    End Function
    
    '** 查詢付款人 **
    <WebMethod(EnableSession:=True)> _
    Public Function GetCompletionListSubject(ByVal prefixText As String, ByVal count As Integer) As String()
        If count = 0 Then
            count = 10
        End If
        
        '防呆
        If prefixText.Equals("%#)*)*)*DDD") Then
            Return New String(0) {}
        End If
                
        Dim i As Integer = 0
        Dim items As List(Of String) = New List(Of String)(count)
        
        
        '開啟查詢
        WSobjCon = New SqlConnection(WSDNS)
        WSobjCon.Open()
        If Trim(prefixText) = "" Then
            WSstrSQL = "SELECT psstr  FROM psname where unit='" & HttpContext.Current.Session("UserUnit") & "' and  seq=9999 "
        Else
            WSstrSQL = "SELECT psstr  FROM psname where unit='" & HttpContext.Current.Session("UserUnit") & "' and  seq=9999 and psstr LIKE '" & prefixText & "%'"
        End If
        
        

        WSobjCmd = New SqlCommand(WSstrSQL, WSobjCon)
        WSobjDR = WSobjCmd.ExecuteReader

        Do While WSobjDR.Read
            items.Add(Trim(WSobjDR("psstr").ToString))
            System.Math.Min(System.Threading.Interlocked.Increment(i), i - 1)
        Loop

        WSobjCon.Close()
        
        
        Return items.ToArray
    End Function
    
    '** 查詢傳票會計科目 **
    <WebMethod(EnableSession:=True)> _
    Public Function GetAC010Accno(ByVal prefixText As String, ByVal count As Integer) As String()
        If count = 0 Then
            count = 10
        End If
        
        '防呆
        If prefixText.Equals("%#)*)*)*DDD") Then
            Return New String(0) {}
        End If
        
        prefixText = prefixText.Replace("-", "")
        prefixText = prefixText.Replace("_", " ")
                
        Dim i As Integer = 0
        Dim items As List(Of String) = New List(Of String)(count)
        
        
        '開啟查詢
        WSobjCon = New SqlConnection(WSDNS)
        WSobjCon.Open()
        
       
        'WSstrSQL = "SELECT psstr  FROM psname where unit='" & HttpContext.Current.Session("UserUnit") & "' and  seq<>9999 and psstr LIKE '%" & prefixText & "%'"
        If Trim(prefixText) = "" Then
            WSstrSQL = "SELECT top 10 rtrim(accno) as accno, rtrim(left(accno+space(17),17)+accname) as accname FROM accname where belong<>'B' and outyear=0  order by accno"
        Else
            WSstrSQL = "SELECT top 10 rtrim(accno) as accno, rtrim(left(accno+space(17),17)+accname) as accname FROM accname where belong<>'B' and outyear=0 and left(accno+space(17),17)+accname LIKE '" & Trim(prefixText) & "%' order by accno"
        End If
        

        WSobjCmd = New SqlCommand(WSstrSQL, WSobjCon)
        WSobjDR = WSobjCmd.ExecuteReader

        Do While WSobjDR.Read
            items.Add(Trim(WSobjDR("accname").ToString))
            System.Math.Min(System.Threading.Interlocked.Increment(i), i - 1)
        Loop

        WSobjCon.Close()
        
        
        Return items.ToArray
    End Function
    
    '** 查詢傳票摘要 **
    <WebMethod(EnableSession:=True)> _
    Public Function GetAC010Remark(ByVal prefixText As String, ByVal count As Integer) As String()
        If count = 0 Then
            count = 10
        End If
        
        '防呆
        If prefixText.Equals("%#)*)*)*DDD") Then
            Return New String(0) {}
        End If
                
        Dim i As Integer = 0
        Dim items As List(Of String) = New List(Of String)(count)
        
        
        '開啟查詢
        WSobjCon = New SqlConnection(WSDNS)
        WSobjCon.Open()
        
       
        'WSstrSQL = "SELECT psstr  FROM psname where unit='" & HttpContext.Current.Session("UserUnit") & "' and  seq<>9999 and psstr LIKE '%" & prefixText & "%'"
        If Trim(prefixText) = "" Then
            WSstrSQL = "SELECT psstr  FROM psname where left(unit,3)='050' order by psstr"
        Else
            WSstrSQL = "SELECT psstr  FROM psname where left(unit,3)='050' and psstr LIKE '%" & prefixText & "%' order by psstr"
        End If
        

        WSobjCmd = New SqlCommand(WSstrSQL, WSobjCon)
        WSobjDR = WSobjCmd.ExecuteReader

        Do While WSobjDR.Read
            items.Add(Trim(WSobjDR("psstr")))
            System.Math.Min(System.Threading.Interlocked.Increment(i), i - 1)
        Loop

        WSobjCon.Close()
        
        
        Return items.ToArray
    End Function
    
    '** 查詢出納摘要 **
    <WebMethod(EnableSession:=True)> _
    Public Function GetPayRemark(ByVal prefixText As String, ByVal count As Integer) As String()
        If count = 0 Then
            count = 10
        End If
        
        '防呆
        If prefixText.Equals("%#)*)*)*DDD") Then
            Return New String(0) {}
        End If
                
        Dim i As Integer = 0
        Dim items As List(Of String) = New List(Of String)(count)
        
        
        '開啟查詢
        WSobjCon = New SqlConnection(WSDNS)
        WSobjCon.Open()
        
        If Trim(prefixText) = "" Then
            WSstrSQL = "SELECT DISTINCT TOP 20 psstr FROM psname where unit='0403' order by psstr DESC "
        Else
            WSstrSQL = "SELECT DISTINCT TOP 20 psstr FROM psname where unit='0403' and psstr LIKE '%" & prefixText & "%' order by psstr DESC"
        End If
                      '
        WSobjCmd = New SqlCommand(WSstrSQL, WSobjCon)
        WSobjDR = WSobjCmd.ExecuteReader

        Do While WSobjDR.Read
            items.Add(Trim(WSobjDR("psstr")))
            System.Math.Min(System.Threading.Interlocked.Increment(i), i - 1)
        Loop

        WSobjCon.Close()
        
        
        Return items.ToArray
    End Function
  
    
    '** 主計系統Api串接程序 **
    ''' <summary>
    ''' 於主計系統中，直接產生一筆推算資料
    ''' </summary>
    ''' <param name="NowDate">請購(推算)日期</param>
    ''' <param name="AccNo">會計科目代號(請購科目)</param>
    ''' <param name="ReMark">請購事由</param>
    ''' <param name="Amt">請購(推算)金額</param>
    ''' <param name="Subject">受款人(可空白)</param>
    ''' <returns>儲存狀態(True：成功 False：失敗)</returns>
    <WebMethod()> _
    Public Function GoBGF020(ByVal NowDate As String, ByVal AccNo As String, _
                     ByVal ReMark As String, ByVal Amt As String, ByVal Subject As String) As String
        Dim blnCheck As Boolean = False
        Dim strKind, strDC As String
        
        '取得請購編號
        Dim intBgNo As String = Controller.RequireNO(WSDNS, Mid(NowDate, 1, 3), "B") '目前編號數
        Dim strBgNo As String = Format(CInt(Mid(NowDate, 1, 3)), "000") + Format(CInt(intBgNo), "00000") '重新給號
        
        
        '防呆檢查
        If NowDate = "" Then Return "" : Exit Function
        If AccNo = "" Then Return "" : Exit Function
        If ReMark = "" Then Return "" : Exit Function
        If Amt = "" Or Amt = "0" Then Return "" : Exit Function
        
        
        '判斷傳票狀態
        If Mid(AccNo, 1, 1) = "4" Then
            If CDbl(Amt) > 0 Then   '收入科目  正數表示收入傳票, 貸方金額
                strKind = "1" : strDC = "2"
            Else
                strKind = "2" : strDC = "1"
            End If
        Else
            If CDbl(Amt) > 0 Then  '支出科目  正數表示支出傳票, 借方金額
                strKind = "2" : strDC = "1"
            Else
                strKind = "1" : strDC = "2"
            End If
        End If
        
        
        '資料處理(新增一筆至BGF020 & UPDATE BGF010->TOTPER)
        ADO.GenInsSql("BGNO", strBgNo, "T")
        ADO.GenInsSql("ACCYEAR", Mid(NowDate, 1, 3), "N")
        ADO.GenInsSql("ACCNO", AccNo, "T")
        ADO.GenInsSql("KIND", strKind, "T")
        ADO.GenInsSql("DC", strDC, "T")
        ADO.GenInsSql("DATE1", Models.strStrToDate(NowDate), "D")
        ADO.GenInsSql("REMARK", ReMark, "U")
        ADO.GenInsSql("AMT1", Amt, "N")
        ADO.GenInsSql("USEABLEAMT", Amt, "N")
        ADO.GenInsSql("SUBJECT", Subject, "U")
        ADO.GenInsSql("CLOSEMARK", " ", "T")
        
        WSstrSQL = "INSERT INTO BGF020 " & ADO.GenInsFunc
        blnCheck = ADO.dbExecute(WSDNS, WSstrSQL)
        If blnCheck = False Then Return "" : Exit Function '新增失敗傳回
        
        
        'UPDATE BGF010->TOTPER+AMT 
        WSstrSQL = "UPDATE BGF010 SET"
        WSstrSQL &= " TOTPER = TOTPER + " & Models.ValComa(Amt)
        WSstrSQL &= " WHERE ACCYEAR = '" & Mid(NowDate, 1, 3) & "'"
        WSstrSQL &= " AND ACCNO = '" & AccNo & "'"
        
        blnCheck = ADO.dbExecute(WSDNS, WSstrSQL)
        If blnCheck = False Then Return "" : Exit Function '新增失敗傳回
        
        
        Return strBgNo
    End Function
End Class
