Imports System.Data.SqlClient
Imports System.Configuration
Public Class PGValueDAL
    Public Shared CONNECTION_STRING As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString
    Private Const USP_QUERY As String = "usp_pptf020_query"
    Private Const USP_INSERT As String = "usp_pptf020_insert"
    Private Const USP_UPDATE As String = "usp_pptf020_update"
    Private Const USP_DELETE As String = "usp_pptf020_delete"

    Private Const PARAM_ID As String = "@id"
    Private Const PARAM_PRNO As String = "@prno"
    Private Const PARAM_PRNO2 As String = "@prno2"
    Private Const PARAM_ACNO As String = "@acno"
    Private Const PARAM_ACNO2 As String = "@acno2"
    Private Const PARAM_NAME As String = "@name"
    Private Const PARAM_ADDDELDATE As String = "@addDelDate"
    Private Const PARAM_ADDDELDATE2 As String = "@addDelDate2"
    Private Const PARAM_AMT As String = "@amt"
    Private Const PARAM_AMT2 As String = "@amt2"
    Private Const PARAM_REMARK As String = "@remark"
    Private Const PARAM_RETURN As String = "@retval"


    '查詢財物增減資料
    Public Shared Function Query(ByVal info As PGValueInfo) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryParameters(params, info)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY, params)
        dt = ds.Tables(0)
        '新增兩個欄位,淨值和折舊率
        dt.Columns.Add("NetAmt", GetType(Decimal), "originalAmt + totalAdddel -depreciation")
        '使用IIF避免除以零的錯誤
        'dt.Columns.Add("DepreciationRatio", GetType(Decimal), "depreciation / iif((amt + totalAdddel - endamt)=0,1,(amt + totalAdddel - endamt))")
        '主任說折舊率 = (現在年份 - 購買年份 ) / 使用年限 , 但很多使用年限 =0 年者,因此乾脆不要顯示折舊率
        dt.TableName = "財物增減值"
        Return dt

    End Function

    '新增財物增減資料
    Public Shared Function Insert(ByVal info As PGValueInfo, ByRef id As String) As Integer
        Dim params As SqlParameter()
        params = GetInsertParameters()
        SetInsertParameters(params, info)
        params(0).Direction = ParameterDirection.Output
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_INSERT, params)
        '傳回1代表新增成功,-1代表財物編號不存在
        id = params(0).Value ''剛新增資料的識別碼
        Return params(5).Value
    End Function

    '修改財物增減資料
    Public Shared Function Update(ByVal info As PGValueInfo) As Integer
        Dim params As SqlParameter()
        params = GetInsertParameters()
        SetInsertParameters(params, info)
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_UPDATE, params)
        '傳回1代表修改成功,-1代表財物編號不存在
        Return params(5).Value
    End Function

    '刪除財物增減資料
    Public Shared Function Delete(ByVal info As PGValueInfo) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter(PARAM_ID, SqlDbType.Int), New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = info.ID
        params(1).Value = info.PRNO
        params(2).Direction = ParameterDirection.ReturnValue

        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_DELETE, params)
        '傳回1代表刪除成功,-1代表財物編號不存在
        Return params(2).Value
    End Function

    Private Shared Function GetQueryParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_QUERY)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_PRNO2, SqlDbType.Char, 10), New SqlParameter(PARAM_ACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_ACNO2, SqlDbType.VarChar, 17), New SqlParameter(PARAM_NAME, SqlDbType.NVarChar, 30) _
              , New SqlParameter(PARAM_ADDDELDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_ADDDELDATE2, SqlDbType.SmallDateTime) _
              , New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_AMT2, SqlDbType.Decimal, 9), New SqlParameter(PARAM_REMARK, SqlDbType.NVarChar, 30)}
            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_QUERY, parms)
        End If
        Return parms
    End Function

    Private Shared Function GetInsertParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_INSERT)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_ID, SqlDbType.Int), New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_ADDDELDATE, SqlDbType.SmallDateTime) _
              , New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_REMARK, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
            parms(5).Direction = ParameterDirection.ReturnValue
            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_INSERT, parms)
        End If
        Return parms
    End Function

    Private Shared Sub SetQueryParameters(ByVal parms As SqlParameter(), ByVal info As PGValueInfo)
        With info
            If .PRNO = String.Empty Then
                parms(0).Value = DBNull.Value
            Else
                parms(0).Value = .PRNO
            End If
            If .PRNO2 = String.Empty Then
                parms(1).Value = DBNull.Value
            Else
                parms(1).Value = .PRNO2
            End If
            If .ACNO = String.Empty Then
                parms(2).Value = DBNull.Value
            Else
                parms(2).Value = .ACNO
            End If
            If .ACNO2 = String.Empty Then
                parms(3).Value = DBNull.Value
            Else
                parms(3).Value = .ACNO2
            End If
            If .Name = String.Empty Then
                parms(4).Value = DBNull.Value
            Else
                parms(4).Value = .Name
            End If
            If .AddDelDate = String.Empty Then
                parms(5).Value = DBNull.Value
            Else
                parms(5).Value = .AddDelDate
            End If
            If .AddDelDate2 = String.Empty Then
                parms(6).Value = DBNull.Value
            Else
                parms(6).Value = .AddDelDate2
            End If
            If .AMT = String.Empty Then
                parms(7).Value = DBNull.Value
            Else
                parms(7).Value = .AMT
            End If
            If .AMT2 = String.Empty Then
                parms(8).Value = DBNull.Value
            Else
                parms(8).Value = .AMT2
            End If
            If .Remark = String.Empty Then
                parms(9).Value = DBNull.Value
            Else
                parms(9).Value = .Remark
            End If

        End With
    End Sub

    Private Shared Sub SetInsertParameters(ByVal parms As SqlParameter(), ByVal info As PGValueInfo)
        With info
            parms(0).Value = .ID
            parms(1).Value = .PRNO
            parms(2).Value = .AddDelDate
            parms(3).Value = .AMT
            If .Remark = String.Empty Then
                parms(4).Value = DBNull.Value
            Else
                parms(4).Value = .Remark
            End If

        End With
    End Sub
End Class
