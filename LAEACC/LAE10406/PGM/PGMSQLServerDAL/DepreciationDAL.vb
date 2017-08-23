Imports System.Data.SqlClient
Imports System.Configuration
Public Class DepreciationDAL
    Public Shared CONNECTION_STRING As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString
    Private Const USP_QUERY As String = "usp_pptf030_query"
    Private Const USP_QUERY2 As String = "usp_pptf030_query2"
    Private Const USP_INSERT As String = "usp_pptf030_insert"
    Private Const USP_UPDATE As String = "usp_pptf030_update"
    Private Const USP_DELETE As String = "usp_pptf030_delete"
    Private Const USP_DEPRECIATE_BUILDING As String = "usp_pptf030_depreciate_building"
    Private Const USP_DEPRECIATE_GENERAL As String = "usp_pptf030_depreciate_general"
    Private Const USP_PREDEPRECIATE As String = "usp_pptf030_PreDepreciate"

    Private Const PARAM_ID As String = "@id"
    Private Const PARAM_PRNO As String = "@prno"
    Private Const PARAM_PRNO2 As String = "@prno2"
    Private Const PARAM_ACNO As String = "@acno"
    Private Const PARAM_ACNO2 As String = "@acno2"
    Private Const PARAM_NAME As String = "@name"
    Private Const PARAM_DEPRECIATIONDATE As String = "@depreciationDate"
    Private Const PARAM_DEPRECIATIONDATE2 As String = "@depreciationDate2"
    Private Const PARAM_AMT As String = "@amt"
    Private Const PARAM_AMT2 As String = "@amt2"
    Private Const PARAM_REMARK As String = "@remark"
    Private Const PARAM_RETURN As String = "@retval"


    '查詢財物折舊資料
    Public Shared Function Query(ByVal info As DepreciationInfo) As DataTable
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
        dt.TableName = "財物折舊檔"
        Return dt
    End Function

    '查詢財物折舊資料 (每級會計科目都會小計)
    Public Shared Function Query2(ByVal info As DepreciationInfo) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryParameters(params, info)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY2, params)
        dt = ds.Tables(0)
        '新增兩個欄位,淨值和折舊率
        dt.Columns.Add("NetAmt", GetType(Decimal), "originalAmt + totalAdddel -depreciation")
        '使用IIF避免除以零的錯誤
        'dt.Columns.Add("DepreciationRatio", GetType(Decimal), "depreciation / iif((amt + totalAdddel - endamt)=0,1,(amt + totalAdddel - endamt))")
        '主任說折舊率 = (現在年份 - 購買年份 ) / 使用年限 , 但很多使用年限 =0 年者,因此乾脆不要顯示折舊率
        dt.TableName = "財物折舊檔"
        Return dt
    End Function


    '新增財物折舊資料
    Public Shared Function Insert(ByVal info As DepreciationInfo, ByRef id As String) As Integer
        Dim params As SqlParameter(), result As Integer
        params = GetInsertParameters()
        SetInsertParameters(params, info)
        params(0).Direction = ParameterDirection.Output
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_INSERT, params)
        result = params(5).Value
        '傳回1代表新增成功,-1代表財物編號不存在
        If result = 1 Then id = params(0).Value '剛新增資料的識別碼,若新增失敗,此參數值為null,所以必須額外判斷
        Return result
    End Function

    '修改財物折舊資料
    Public Shared Function Update(ByVal info As DepreciationInfo) As Integer
        Dim params As SqlParameter()
        params = GetInsertParameters()
        SetInsertParameters(params, info)
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_UPDATE, params)
        '傳回1代表修改成功,-1代表財物編號不存在
        Return params(5).Value
    End Function

    '刪除財物折舊資料
    Public Shared Function Delete(ByVal info As DepreciationInfo) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter(PARAM_ID, SqlDbType.Int), New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = info.ID
        params(1).Value = info.PRNO
        params(2).Direction = ParameterDirection.ReturnValue

        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_DELETE, params)
        '傳回1代表刪除成功,-1代表財物編號不存在
        Return params(2).Value
    End Function

    '自動提列折舊, method=0 按年提列, method=1 按月提列,kind=0 一般財物, kind=1 建物
    Public Shared Function Depreciate(ByVal pdate As String, ByVal method As Integer, ByVal kind As Integer) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter("@pdate", SqlDbType.SmallDateTime), New SqlParameter("@method", SqlDbType.TinyInt), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = pdate
        params(1).Value = method
        params(2).Direction = ParameterDirection.ReturnValue
        If kind = 0 Then
            SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_DEPRECIATE_GENERAL, params)
        Else
            SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_DEPRECIATE_BUILDING, params)
        End If

        '傳回值代表總共折舊了幾筆資料
        Return params(2).Value
    End Function

    '預提下年度折舊, result 會回傳總共折舊了幾筆資料
    Public Shared Function PreDepreciate(ByVal pdate As String, ByRef result As Integer) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter("@pdate", SqlDbType.SmallDateTime), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = pdate
        params(1).Direction = ParameterDirection.ReturnValue

        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_PREDEPRECIATE, params)
        dt = ds.Tables(0)
        dt.TableName = "預提折舊"

        '傳回值代表總共折舊了幾筆資料
        result = params(1).Value
        Return dt
    End Function

    Private Shared Function GetQueryParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_QUERY)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_PRNO2, SqlDbType.Char, 10), New SqlParameter(PARAM_ACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_ACNO2, SqlDbType.VarChar, 17), New SqlParameter(PARAM_NAME, SqlDbType.NVarChar, 30) _
              , New SqlParameter(PARAM_DEPRECIATIONDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_DEPRECIATIONDATE2, SqlDbType.SmallDateTime) _
              , New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_AMT2, SqlDbType.Decimal, 9), New SqlParameter(PARAM_REMARK, SqlDbType.NVarChar, 30)}
            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_QUERY, parms)
        End If
        Return parms
    End Function

    Private Shared Function GetInsertParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_INSERT)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_ID, SqlDbType.Int), New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_DEPRECIATIONDATE, SqlDbType.SmallDateTime) _
              , New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_REMARK, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
            parms(5).Direction = ParameterDirection.ReturnValue
            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_INSERT, parms)
        End If
        Return parms
    End Function

    Private Shared Sub SetQueryParameters(ByVal parms As SqlParameter(), ByVal info As DepreciationInfo)
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
            If .DepreciationDate = String.Empty Then
                parms(5).Value = DBNull.Value
            Else
                parms(5).Value = .DepreciationDate
            End If
            If .DepreciationDate2 = String.Empty Then
                parms(6).Value = DBNull.Value
            Else
                parms(6).Value = .DepreciationDate2
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

    Private Shared Sub SetInsertParameters(ByVal parms As SqlParameter(), ByVal info As DepreciationInfo)
        With info
            parms(0).Value = .ID
            parms(1).Value = .PRNO
            parms(2).Value = .DepreciationDate
            parms(3).Value = .AMT
            If .Remark = String.Empty Then
                parms(4).Value = DBNull.Value
            Else
                parms(4).Value = .Remark
            End If

        End With
    End Sub
End Class
