Imports System.Data
Imports System.Data.SqlClient

Public Class Schild
    Inherits System.Web.UI.MasterPage

    '資料庫連線字串
    Public DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString

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
#End Region

#Region "Page及功能操作"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region
End Class