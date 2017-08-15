Imports System.Data.SqlClient
Imports System.Configuration
Public Class TransferDAL
    Public Shared CONNECTION_STRING As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString
    Private Const USP_QUERY As String = "usp_pptf040_query"

    Private Const PARAM_PRNO As String = "@prno"

    '查詢財物交接資料
    Public Shared Function Query(ByVal info As TransferInfo) As DataTable
        Dim ds As DataSet
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10)}
        params(0).Value = info.PRNO

        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY, params)
        ds.Tables(0).TableName = "財物交接"
        Return ds.Tables(0)
    End Function
End Class
