Imports System.Data.SqlClient
Imports System.Configuration
Public Class BuildingDAL
    Public Shared CONNECTION_STRING As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString
    Private Const USP_QUERY As String = "usp_pptf050_query"
    Private Const USP_QUERY_STA As String = "usp_pptf050_query_sta"
    Private Const USP_INSERT As String = "usp_pptf050_insert"
    Private Const USP_UPDATE As String = "usp_pptf050_update"
    Private Const USP_DELETE As String = "usp_pptf050_delete"
    Private Const USP_Get_PRNO As String = "usp_pptf050_get_prno"

    Private Const PARAM_PRNO As String = "@prno"
    Private Const PARAM_PRNO2 As String = "@prno2"
    Private Const PARAM_ACNO As String = "@acno"
    Private Const PARAM_ACNO2 As String = "@acno2"
    Private Const PARAM_BUILNO As String = "@builno"
    Private Const PARAM_LANDNO1 As String = "@landno1"
    Private Const PARAM_LANDNO2 As String = "@landno2"
    Private Const PARAM_LANDAREA As String = "@landarea"
    Private Const PARAM_PART As String = "@part"
    Private Const PARAM_AREA1 As String = "@area1"
    Private Const PARAM_AREA2 As String = "@area2"
    Private Const PARAM_AREA3 As String = "@area3"
    Private Const PARAM_STRU As String = "@stru"
    Private Const PARAM_TAX As String = "@tax"
    Private Const PARAM_ADDR As String = "@addr"
    Private Const PARAM_KIND As String = "@kind"
    Private Const PARAM_USEMODE As String = "@usemode"
    Private Const PARAM_PDATE As String = "@pdate"
    Private Const PARAM_USEYEAR As String = "@useyear"
    Private Const PARAM_AMT As String = "@amt"
    Private Const PARAM_ENDAMT As String = "@endamt"
    Private Const PARAM_ENDDATE As String = "@enddate"
    Private Const PARAM_BORROW As String = "@borrow"
    Private Const PARAM_REMARK As String = "@remark"
    Private Const PARAM_RETURN As String = "@retval"

    '查詢建物主檔資料
    Public Shared Function Query(ByVal info As BuildingInfo) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryParameters(params, info)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY, params)
        dt = ds.Tables(0)
        '新增兩個欄位,淨值和折舊率
        dt.Columns.Add("NetAmt", GetType(Decimal), "amt + totalAdddel -depreciation")
        '使用IIF避免除以零的錯誤
        'dt.Columns.Add("DepreciationRatio", GetType(Decimal), "depreciation / iif((amt + totalAdddel - endamt)=0,1,(amt + totalAdddel - endamt))")
        '主任說折舊率 = (現在年份 - 購買年份 ) / 使用年限 , 但很多使用年限 =0 年者,因此乾脆不要顯示折舊率
        dt.TableName = "建物主檔"
        Return dt
    End Function

    '查詢建物主檔資料,要列出小計和總計資料,因為是group by acno, 所以若有篩選到acno為null者,就會造成兩筆總計,一個總計是針對全部的總計,另一個總計是針對acno=null的小計
    Public Shared Function QuerySta(ByVal info As BuildingInfo) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryParameters(params, info)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_STA, params)
        dt = ds.Tables(0)
        dt.TableName = "建物主檔"
        Return dt
    End Function

    '新增建物主檔
    Public Shared Function Insert(ByVal info As BuildingInfo) As Integer
        Dim params As SqlParameter()
        params = GetInsertParameters()
        SetInsertParameters(params, info)
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_INSERT, params)
        '傳回1代表新增成功,-1代表財物編號已經存在資料庫
        Return params(22).Value
    End Function

    '修改建物主檔
    Public Shared Function Update(ByVal info As BuildingInfo) As Integer
        Dim params As SqlParameter()
        params = GetInsertParameters()
        SetInsertParameters(params, info)
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_UPDATE, params)
        '傳回1代表修改成功,-1代表財物編號不存在
        Return params(22).Value
    End Function

    '刪除建物主檔
    Public Shared Function Delete(ByVal info As BuildingInfo) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = info.PRNO
        params(1).Direction = ParameterDirection.ReturnValue

        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_DELETE, params)
        '傳回1代表刪除成功,-1代表財物編號不存在
        Return params(1).Value
    End Function

    '取得此建物分類編號可用的序號起始號碼,count代表總共需要幾個沒被佔用的號碼
    Public Shared Function GetPrNo(ByVal kindno As String, ByVal count As Integer) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter("@kindno", SqlDbType.VarChar, 6), New SqlParameter("@count", SqlDbType.Int), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = kindno
        params(1).Value = count
        params(2).Direction = ParameterDirection.ReturnValue

        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_Get_PRNO, params)
        '傳回值代表起始號碼, 若傳回 -1 代表此財物分類編號沒有可用的連續號碼,傳回 -2 代表此財物分類編號不存在
        Return params(2).Value
    End Function

    Private Shared Function GetInsertParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_INSERT)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_BUILNO, SqlDbType.VarChar, 28), New SqlParameter(PARAM_LANDNO1, SqlDbType.VarChar, 6) _
                , New SqlParameter(PARAM_LANDNO2, SqlDbType.VarChar, 20), New SqlParameter(PARAM_LANDAREA, SqlDbType.Decimal, 5), New SqlParameter(PARAM_PART, SqlDbType.VarChar, 12) _
                , New SqlParameter(PARAM_AREA1, SqlDbType.Decimal, 5), New SqlParameter(PARAM_AREA2, SqlDbType.Decimal, 5), New SqlParameter(PARAM_AREA3, SqlDbType.Decimal, 5) _
                , New SqlParameter(PARAM_STRU, SqlDbType.NVarChar, 20), New SqlParameter(PARAM_TAX, SqlDbType.VarChar, 21), New SqlParameter(PARAM_ADDR, SqlDbType.NVarChar, 50) _
                , New SqlParameter(PARAM_PDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_USEYEAR, SqlDbType.Int) _
                , New SqlParameter(PARAM_KIND, SqlDbType.NVarChar, 20), New SqlParameter(PARAM_USEMODE, SqlDbType.NVarChar, 12), New SqlParameter(PARAM_ACNO, SqlDbType.VarChar, 17) _
                , New SqlParameter(PARAM_ENDAMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_ENDDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_BORROW, SqlDbType.NVarChar, 50) _
                , New SqlParameter(PARAM_REMARK, SqlDbType.NVarChar, 50), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
            parms(22).Direction = ParameterDirection.ReturnValue
            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_INSERT, parms)
        End If
        Return parms
    End Function

    Private Shared Function GetQueryParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_QUERY)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_PRNO2, SqlDbType.Char, 10), New SqlParameter(PARAM_KIND, SqlDbType.NVarChar, 20) _
            , New SqlParameter(PARAM_ACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_ACNO2, SqlDbType.VarChar, 17)}

            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_QUERY, parms)
        End If
        Return parms
    End Function


    Private Shared Sub SetInsertParameters(ByVal parms As SqlParameter(), ByVal info As BuildingInfo)
        With info
            If .PRNO = String.Empty Then
                parms(0).Value = DBNull.Value
            Else
                parms(0).Value = .PRNO
            End If
            If .BuilNo = String.Empty Then
                parms(1).Value = DBNull.Value
            Else
                parms(1).Value = .BuilNo
            End If
            If .LandNo1 = String.Empty Then
                parms(2).Value = DBNull.Value
            Else
                parms(2).Value = .LandNo1
            End If
            If .LandNo2 = String.Empty Then
                parms(3).Value = DBNull.Value
            Else
                parms(3).Value = .LandNo2
            End If
            If .LandArea = String.Empty Then
                parms(4).Value = DBNull.Value
            Else
                parms(4).Value = .LandArea
            End If
            If .Part = String.Empty Then
                parms(5).Value = DBNull.Value
            Else
                parms(5).Value = .Part
            End If
            If .Area1 = String.Empty Then
                parms(6).Value = DBNull.Value
            Else
                parms(6).Value = .Area1
            End If
            If .Area2 = String.Empty Then
                parms(7).Value = DBNull.Value
            Else
                parms(7).Value = .Area2
            End If
            If .Area3 = String.Empty Then
                parms(8).Value = DBNull.Value
            Else
                parms(8).Value = .Area3
            End If
            If .Stru = String.Empty Then
                parms(9).Value = DBNull.Value
            Else
                parms(9).Value = .Stru
            End If
            If .Tax = String.Empty Then
                parms(10).Value = DBNull.Value
            Else
                parms(10).Value = .Tax
            End If
            If .Addr = String.Empty Then
                parms(11).Value = DBNull.Value
            Else
                parms(11).Value = .Addr
            End If
            If .PDate = String.Empty Then
                parms(12).Value = DBNull.Value
            Else
                parms(12).Value = .PDate
            End If
            If .AMT = String.Empty Then
                parms(13).Value = DBNull.Value
            Else
                parms(13).Value = .AMT
            End If
            If .UseYear = String.Empty Then
                parms(14).Value = DBNull.Value
            Else
                parms(14).Value = .UseYear
            End If
            If .Kind = String.Empty Then
                parms(15).Value = DBNull.Value
            Else
                parms(15).Value = .Kind
            End If
            If .UseMode = String.Empty Then
                parms(16).Value = DBNull.Value
            Else
                parms(16).Value = .UseMode
            End If
            If .AcNo = String.Empty Then
                parms(17).Value = DBNull.Value
            Else
                parms(17).Value = .AcNo
            End If
            If .EndAMT = String.Empty Then
                parms(18).Value = DBNull.Value
            Else
                parms(18).Value = .EndAMT
            End If
            If .EndDate = String.Empty Then
                parms(19).Value = DBNull.Value
            Else
                parms(19).Value = .EndDate
            End If
            If .Borrow = String.Empty Then
                parms(20).Value = DBNull.Value
            Else
                parms(20).Value = .Borrow
            End If
            If .Remark = String.Empty Then
                parms(21).Value = DBNull.Value
            Else
                parms(21).Value = .Remark
            End If

        End With

    End Sub

    Private Shared Sub SetQueryParameters(ByVal parms As SqlParameter(), ByVal info As BuildingInfo)
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
            If .Kind = String.Empty Then
                parms(2).Value = DBNull.Value
            Else
                parms(2).Value = .Kind
            End If
            If .AcNo = String.Empty Then
                parms(3).Value = DBNull.Value
            Else
                parms(3).Value = .AcNo
            End If
            If .AcNo2 = String.Empty Then
                parms(4).Value = DBNull.Value
            Else
                parms(4).Value = .AcNo2
            End If
        End With

    End Sub
End Class
