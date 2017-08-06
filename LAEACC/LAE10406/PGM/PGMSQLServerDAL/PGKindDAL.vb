Imports System.Data.SqlClient
Imports System.Configuration

Public Class PGKindDAL

    Private Const USP_QUERY As String = "usp_pptname_query"
    Private Const USP_INSERT As String = "usp_pptname_insert"
    Private Const USP_UPDATE As String = "usp_pptname_update"
    Private Const USP_DELETE As String = "usp_pptname_delete"

    Private Const PARAM_KINDNO As String = "@kindno"
    Private Const PARAM_KINDNO2 As String = "@kindno2"
    Private Const PARAM_NAME As String = "@name"
    Private Const PARAM_UNIT As String = "@unit"
    Private Const PARAM_MATERIAL As String = "@material"
    Private Const PARAM_USEYEAR As String = "@useyear"
    Private Const PARAM_RETURN As String = "@retval"

    Public Shared CONNECTION_STRING As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString
    '查詢財物分類資料
    Public Shared Function QueryPGKind(ByVal kindInfo As PGKindInfo) As DataTable

        Dim ds As DataSet
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryParameters(params, kindInfo)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY, "財物分類", params)
        Return ds.Tables(0)
        'Dim sdr As SqlDataReader, kindList As New ArrayList
        'sdr = SqlHelper.ExecuteReader(DALConfig.CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY)
        'While sdr.Read
        '    Dim kind As PGKindInfo
        '    Dim unit, material As String
        '    '可能為Null的欄位必須額外判斷,否則Null轉成String會出現錯誤
        '    If IsDBNull(sdr("unit")) Then
        '        unit = ""
        '    Else
        '        unit = sdr("unit")
        '    End If
        '    If IsDBNull(sdr("material")) Then
        '        material = ""
        '    Else
        '        material = sdr("material")
        '    End If
        '    kind = New PGKindInfo(sdr("kindno"), sdr("name"), unit, material, sdr("year"))
        '    kindList.Add(kind)
        'End While
        'Return kindList
    End Function

    '新增財物分類
    Public Shared Function Insert(ByVal kindInfo As PGKindInfo) As Integer
        Dim params As SqlParameter()
        params = GetParameters()
        SetParameters(params, kindInfo)
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_INSERT, params)
        '傳回1代表新增成功,-1代表財物編號重複,-2代表編號長度不合法,-3代表父節點還沒建立不允許建立子節點
        Return params(5).Value
    End Function

    '修改財物分類
    Public Shared Function Update(ByVal kindInfo As PGKindInfo) As Integer
        Dim params As SqlParameter()
        params = GetParameters()
        SetParameters(params, kindInfo)
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_UPDATE, params)
        '傳回1代表修改成功,-1代表財物編號不存在
        Return params(5).Value
    End Function

    '刪除財物分類
    Public Shared Function Delete(ByVal kindInfo As PGKindInfo) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter(PARAM_KINDNO, SqlDbType.VarChar, 6), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = kindInfo.KindNo
        params(1).Direction = ParameterDirection.ReturnValue

        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_DELETE, params)
        '傳回1代表刪除成功,-1代表財物編號不存在,-2代表財物編號不是樹葉節點
        Return params(1).Value
    End Function

    Private Shared Function GetParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_INSERT)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_KINDNO, SqlDbType.VarChar, 6), New SqlParameter(PARAM_NAME, SqlDbType.NVarChar, 30) _
                    , New SqlParameter(PARAM_UNIT, SqlDbType.NVarChar, 6), New SqlParameter(PARAM_MATERIAL, SqlDbType.NVarChar, 10), New SqlParameter(PARAM_USEYEAR, SqlDbType.Int), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
            parms(5).Direction = ParameterDirection.ReturnValue
            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_INSERT, parms)
        End If
        Return parms
    End Function

    Private Shared Function GetQueryParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_QUERY)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_KINDNO, SqlDbType.VarChar, 6), New SqlParameter(PARAM_NAME, SqlDbType.NVarChar, 30) _
                    , New SqlParameter(PARAM_UNIT, SqlDbType.NVarChar, 6), New SqlParameter(PARAM_MATERIAL, SqlDbType.NVarChar, 10), New SqlParameter(PARAM_USEYEAR, SqlDbType.Int), _
                    New SqlParameter(PARAM_KINDNO2, SqlDbType.VarChar, 6)}
            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_QUERY, parms)
        End If
        Return parms
    End Function

    Private Shared Sub SetQueryParameters(ByVal parms As SqlParameter(), ByVal kindInfo As PGKindInfo)
        SetParameters(parms, kindInfo)
        If kindInfo.KindNo2 = String.Empty Then
            parms(5).Value = DBNull.Value
        Else
            parms(5).Value = kindInfo.KindNo2
        End If
    End Sub

    Private Shared Sub SetParameters(ByVal parms As SqlParameter(), ByVal kindInfo As PGKindInfo)
        If kindInfo.KindNo = String.Empty Then
            parms(0).Value = DBNull.Value
        Else
            parms(0).Value = kindInfo.KindNo
        End If
        If kindInfo.Name = String.Empty Then
            parms(1).Value = DBNull.Value
        Else
            parms(1).Value = kindInfo.Name
        End If
        If kindInfo.Unit = String.Empty Then
            parms(2).Value = DBNull.Value
        Else
            parms(2).Value = kindInfo.Unit
        End If
        If kindInfo.Material = String.Empty Then
            parms(3).Value = DBNull.Value
        Else
            parms(3).Value = kindInfo.Material
        End If
        If kindInfo.UseYear = String.Empty Then
            parms(4).Value = DBNull.Value
        Else
            parms(4).Value = kindInfo.UseYear
        End If
    End Sub
End Class
