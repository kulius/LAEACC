
Imports System.Data.SqlClient


Public Class PGMainDAL

    Private Const USP_QUERY As String = "usp_pptf010_query"
    Private Const USP_QUERY_IR As String = "usp_pptf010_query_ir"
    Private Const USP_QUERY_PRNOLIST As String = "usp_pptf010_query_prnolist"
    Private Const USP_QUERY_SPARE As String = "usp_pptf010_query_spare"
    Private Const USP_QUERY_STA As String = "usp_pptf010_query_sta"
    Private Const USP_QUERY_STA2 As String = "usp_pptf010_query_sta2"
    Private Const USP_QUERY_TRANSFERLIST As String = "usp_pptf010_query_transferlist"
    Private Const USP_QUERY_CATALOG As String = "usp_pptf010_query_catalog"
    '財產報告表用
    Private Const USP_QUERY_CATALOG2 As String = "usp_pptf010_query_catalog2"
    Private Const USP_QUERY_CAN_DISCARD As String = "usp_pptf010_query_canDiscard"
    Private Const USP_INSERT As String = "usp_pptf010_insert"
    Private Const USP_UPDATE As String = "usp_pptf010_update"
    Private Const USP_DELETE As String = "usp_pptf010_delete"
    Private Const USP_Get_PRNO As String = "usp_pptf010_get_prno"
    Private Const USP_TRANSFER As String = "usp_pptf010_transfer"
    Private Const USP_DISCARD As String = "usp_pptf010_discard"

    Private Const PARAM_PRNO As String = "@prno"
    Private Const PARAM_PRNO_LIST As String = "@prno_list"
    Private Const PARAM_PRNO2 As String = "@prno2"
    Private Const PARAM_NAME As String = "@name"
    Private Const PARAM_ACNO As String = "@acno"
    Private Const PARAM_ACNO2 As String = "@acno2"
    Private Const PARAM_PDATE As String = "@pdate"
    Private Const PARAM_PDATE2 As String = "@pdate2"
    Private Const PARAM_UNIT As String = "@unit"
    Private Const PARAM_QTY As String = "@qty"
    Private Const PARAM_KEEPEMPNO As String = "@keepempno"
    Private Const PARAM_KEEPEMPNO2 As String = "@keepempno2"
    Private Const PARAM_KEEPUNIT As String = "@keepunit"
    Private Const PARAM_KEEPUNIT2 As String = "@keepunit2"
    Private Const PARAM_USEYEAR As String = "@useyear"
    Private Const PARAM_USEYEAR2 As String = "@useyear2"
    Private Const PARAM_AMT As String = "@amt"
    Private Const PARAM_AMT2 As String = "@amt2"
    Private Const PARAM_ENDAMT As String = "@endamt"
    Private Const PARAM_BGACNO As String = "@bgacno"
    Private Const PARAM_BGACNO2 As String = "@bgacno2"
    Private Const PARAM_BGNAME As String = "@bgname"
    Private Const PARAM_COMEFROM As String = "@comefrom"
    Private Const PARAM_MODEL As String = "@model"
    Private Const PARAM_ENDDATE As String = "@enddate"
    Private Const PARAM_ENDDATE2 As String = "@enddate2"
    Private Const PARAM_ENDREMARK As String = "@endremk"
    Private Const PARAM_BORROW As String = "@borrow"
    Private Const PARAM_MATERIAL As String = "@material"
    Private Const PARAM_USES As String = "@uses"
    Private Const PARAM_MADEDATE As String = "@madedate"
    Private Const PARAM_REMARK As String = "@remark"
    Private Const PARAM_PLACE As String = "@place"
    Private Const PARAM_SPEC_REMARK As String = "@spec_remark"
    Private Const PARAM_REVISED_EMPNO As String = "@revised_empno"
    Private Const PARAM_REVISED_DATE As String = "@revised_date"
    Private Const PARAM_REVISED_DATE2 As String = "@revised_date2"
    Private Const PARAM_QUERY_DISCARD_MODE As String = "@query_discard_mode"
    Private Const PARAM_DEPRECIATIONDATE As String = "@depreciationDate"
    Private Const PARAM_DEPRECIATIONDATE2 As String = "@depreciationDate2"
    Private Const PARAM_RETURN As String = "@retval"

    Public Shared CONNECTION_STRING As String = ConfigurationManager.ConnectionStrings("DNS_PGM").ConnectionString
    '查詢財物主檔資料,以ArrayList型態傳回,list裡面每個物件都是 PGMainInfo
    '注意: winform 配合 remoting 好像不適合如此做,速度有點慢,直接傳回datatable會比較快
    Public Shared Function QueryArrayList(ByVal info As PGMainInfo) As ArrayList
        Dim sdr As SqlDataReader, result As New ArrayList, pgMain As PGMainInfo
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryParameters(params, info)
        sdr = SqlHelper.ExecuteReader(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY, params)
        While sdr.Read

            pgMain = New PGMainInfo(sdr("prno"), sdr("name"), sdr("acno") & "", "", sdr("pdate"), _
            sdr("unit") & "", sdr("qty"), sdr("keepempno") & "", sdr("keepempname") & "", sdr("keepunit") & "", sdr("keepunitname") & "" _
            , sdr("useyear"), sdr("amt"), sdr("endamt"), sdr("totalAddDel"), sdr("depreciation"), sdr("bgacno") & "" _
            , sdr("bgname") & "", sdr("comefrom") & "", sdr("model") & "", sdr("enddate") & "", sdr("endremk") & "", sdr("borrow") & "" _
            , sdr("material") & "", sdr("uses") & "", sdr("madedate") & "", sdr("remark") & "", sdr("place") & "", sdr("spec_remark") & "" _
            , sdr("revised_date") & "", sdr("revised_empno") & "", sdr("revisedempname") & "")

            result.Add(pgMain)
        End While
        Return result
    End Function

    '查詢財物主檔資料
    Public Overloads Shared Function Query(ByVal info As PGMainInfo) As DataTable
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
        dt.TableName = "財物主檔"
        Return dt
    End Function

    '查詢財物主檔資料,水利工程明細分戶卡
    '@year int,           --提列折舊的最後年度,要用西元年
    '@prefix varchar(10), --水利工程財物編號的開頭
    '@year_diff int,      --購入日期距提列折舊的最後年度超過幾年前就不要查詢出來
    '@is_show_discard bit, --是否要顯示已報廢的水利工程
    '@min_year int output  --傳回結果中的最小年份,西元年
    Public Shared Function QueryIR(ByVal year As Integer, ByVal prefix As String, ByVal yearDiff As Integer, ByVal isShowDiscard As Boolean, ByRef minYear As Integer) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter("@year", SqlDbType.Int), New SqlParameter("@prefix", SqlDbType.VarChar, 10), New SqlParameter("@year_diff", SqlDbType.Int), New SqlParameter("@is_show_discard", SqlDbType.Bit), New SqlParameter("@min_year", SqlDbType.Int)}
        params(0).Value = year
        params(1).Value = prefix
        params(2).Value = yearDiff
        params(3).Value = isShowDiscard
        params(4).Direction = ParameterDirection.Output
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_IR, params)
        dt = ds.Tables(0)
        '新增兩個欄位,淨值和累積折舊
        dt.Columns.Add("aDepreciation", GetType(Decimal), "bDepreciation + Depreciation")
        dt.Columns.Add("NetAmt", GetType(Decimal), "amt - aDepreciation")
        dt.TableName = "工程明細卡"
        If Not IsDBNull(params(4).Value) Then minYear = params(4).Value
        Return dt
    End Function

    '查詢財物主檔資料,移交清冊
    Public Shared Function QueryTransferList(ByVal info As PGMainInfo, ByVal transferDate As String, ByVal transferDate2 As String) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetTransferListParameters()
        SetTransferListParameters(params, info, transferDate, transferDate2)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_TRANSFERLIST, params)
        dt = ds.Tables(0)
        dt.TableName = "移交清冊"
        Return dt
    End Function

    '查詢財物主檔資料(不會篩選報廢的資料)
    Public Shared Function QueryNodiscard(ByVal info As PGMainInfo) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryParameters(params, info)
        params(25).Value = 1 '只查詢未報廢者
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY, params)
        dt = ds.Tables(0)
        '新增兩個欄位,淨值和折舊率
        dt.Columns.Add("NetAmt", GetType(Decimal), "amt + totalAdddel -depreciation")
        '使用IIF避免除以零的錯誤
        'dt.Columns.Add("DepreciationRatio", GetType(Decimal), "depreciation / iif((amt + totalAdddel - endamt)=0,1,(amt + totalAdddel - endamt))")
        '主任說折舊率 = (現在年份 - 購買年份 ) / 使用年限 , 但很多使用年限 =0 年者,因此乾脆不要顯示折舊率
        dt.TableName = "財物主檔"
        Return dt
    End Function

    '查詢財物主檔資料,只查詢備品,也就是acno is null
    Public Shared Function QuerySpare(ByVal info As PGMainInfo) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryParameters(params, info)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_SPARE, params)
        dt = ds.Tables(0)
        dt.TableName = "財物主檔"
        Return dt
    End Function

    '查詢財產目錄
    Public Shared Function QueryCatalog(ByVal year As Integer, ByVal mode As Integer) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter("@year", SqlDbType.Int), New SqlParameter("@mode", SqlDbType.TinyInt)}
        params(0).Value = year
        params(1).Value = mode
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_CATALOG, params)
        dt = ds.Tables(0)
        dt.TableName = "財產目錄"
        Return dt
    End Function
    '查詢財產報告表
    Public Shared Function QueryCatalog2(ByVal year As Integer, ByVal mode As Integer) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter("@year", SqlDbType.Int), New SqlParameter("@mode", SqlDbType.TinyInt)}
        params(0).Value = year
        params(1).Value = mode
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_CATALOG2, params)
        dt = ds.Tables(0)
        dt.TableName = "財產報告"
        Return dt
    End Function

    '查詢財物主檔資料,要列出小計和總計資料,因為是group by acno, 所以若有篩選到acno為null者,就會造成兩筆總計,一個總計是針對全部的總計,另一個總計是針對acno=null的小計
    Public Shared Function QuerySta(ByVal info As PGMainInfo, ByVal depreciationDate As String, ByVal depreciationDate2 As String) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetQueryStaParameters()
        SetQueryStaParameters(params, info, depreciationDate, depreciationDate2)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_STA, params)
        dt = ds.Tables(0)
        dt.TableName = "財物主檔"
        Return dt
    End Function

    '查詢財物主檔資料(每級會計科目都會小計),要列出小計和總計資料,因為是group by acno, 所以若有篩選到acno為null者,就會造成兩筆總計,一個總計是針對全部的總計,另一個總計是針對acno=null的小計
    Public Shared Function QuerySta2(ByVal info As PGMainInfo, ByVal depreciationDate As String, ByVal depreciationDate2 As String) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = GetQueryParameters()
        SetQueryStaParameters(params, info, depreciationDate, depreciationDate2)
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_STA2, params)
        dt = ds.Tables(0)
        dt.TableName = "財物主檔"
        Return dt
    End Function

    '查詢財物主檔資料,只查詢符合 prno List 中的資料
    'discardMode = 0 不限,=1只查詢未報廢者,=2只查詢已報廢者
    Public Overloads Shared Function Query(ByVal prnoList As String, ByVal discardMode As Integer) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter("@prnoList", SqlDbType.VarChar, 1500), New SqlParameter("@query_discard_mode", SqlDbType.TinyInt)}
        params(0).Value = prnoList
        params(1).Value = discardMode
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_PRNOLIST, params)
        dt = ds.Tables(0)
        '新增兩個欄位,淨值和折舊率
        dt.Columns.Add("NetAmt", GetType(Decimal), "amt + totalAdddel -depreciation")
        '使用IIF避免除以零的錯誤
        'dt.Columns.Add("DepreciationRatio", GetType(Decimal), "depreciation / iif((amt + totalAdddel - endamt)=0,1,(amt + totalAdddel - endamt))")
        '主任說折舊率 = (現在年份 - 購買年份 ) / 使用年限 , 但很多使用年限 =0 年者,因此乾脆不要顯示折舊率
        dt.TableName = "財物主檔"
        Return dt
    End Function

    '查詢可報廢財物清單
    Public Shared Function QueryCanDiscard(ByVal mode As Integer) As DataTable
        Dim ds As DataSet, dt As DataTable
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter("@mode", SqlDbType.TinyInt)}
        params(0).Value = mode
        ds = SqlHelper.ExecuteDataset(CONNECTION_STRING, CommandType.StoredProcedure, USP_QUERY_CAN_DISCARD, params)
        dt = ds.Tables(0)
        '新增兩個欄位,淨值和折舊率
        dt.Columns.Add("NetAmt", GetType(Decimal), "amt + totalAdddel -depreciation")
        '使用IIF避免除以零的錯誤
        'dt.Columns.Add("DepreciationRatio", GetType(Decimal), "depreciation / iif((amt + totalAdddel - endamt)=0,1,(amt + totalAdddel - endamt))")
        '主任說折舊率 = (現在年份 - 購買年份 ) / 使用年限 , 但很多使用年限 =0 年者,因此乾脆不要顯示折舊率
        dt.TableName = "財物主檔"
        Return dt
    End Function

    '新增財物主檔 (可能有多筆 prno ~ prno2)
    Public Shared Function Insert(ByVal info As PGMainInfo) As Integer
        Dim params As SqlParameter()
        params = GetInsertParameters()
        SetInsertParameters(params, info)
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_INSERT, params)
        '傳回1代表新增成功,-1代表財物編號區間至少有一個編號已經存在資料庫
        Return params(26).Value
    End Function

    '修改財物主檔
    Public Shared Function Update(ByVal info As PGMainInfo) As Integer
        Dim params As SqlParameter()
        params = GetInsertParameters()
        SetInsertParameters(params, info)
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_UPDATE, params)
        '傳回1代表修改成功,-1代表財物編號不存在
        Return params(26).Value
    End Function

    '刪除財物主檔
    Public Shared Function Delete(ByVal info As PGMainInfo) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = info.PRNO
        params(1).Direction = ParameterDirection.ReturnValue

        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_DELETE, params)
        '傳回1代表刪除成功,-1代表財物編號不存在
        Return params(1).Value
    End Function

    '財物交接作業
    Public Shared Function Transfer(ByVal prNoList As String, ByVal transferDate As String, ByVal keepEmpNo As String, ByVal keepUnit As String, ByVal revisedEmpNo As String) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter(PARAM_PRNO_LIST, SqlDbType.VarChar, 1000), New SqlParameter(PARAM_PDATE, SqlDbType.SmallDateTime) _
        , New SqlParameter(PARAM_KEEPEMPNO, SqlDbType.VarChar, 4), New SqlParameter(PARAM_KEEPUNIT, SqlDbType.VarChar, 4), New SqlParameter(PARAM_REVISED_EMPNO, SqlDbType.VarChar, 4), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = prNoList
        params(1).Value = transferDate
        If keepEmpNo = String.Empty Then params(2).Value = DBNull.Value Else params(2).Value = keepEmpNo
        If keepUnit = String.Empty Then params(3).Value = DBNull.Value Else params(3).Value = keepUnit
        params(4).Value = revisedEmpNo
        params(5).Direction = ParameterDirection.ReturnValue

        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_TRANSFER, params)
        '傳回1代表交接成功,-1代表至少有一個財物編號不存在,-2代表至少有一個財物編號已經報廢了
        Return params(5).Value
    End Function

    '財物報廢作業
    Public Shared Function Discard(ByVal prNoList As String, ByVal endDate As String, ByVal endRemark As String, ByVal revisedEmpNo As String) As Integer
        Dim params As SqlParameter()
        params = New SqlParameter() {New SqlParameter(PARAM_PRNO_LIST, SqlDbType.VarChar, 1000), New SqlParameter(PARAM_ENDDATE, SqlDbType.SmallDateTime) _
        , New SqlParameter(PARAM_ENDREMARK, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_REVISED_EMPNO, SqlDbType.VarChar, 4), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
        params(0).Value = prNoList
        params(1).Value = endDate
        params(2).Value = endRemark
        params(3).Value = revisedEmpNo
        params(4).Direction = ParameterDirection.ReturnValue
        SqlHelper.ExecuteNonQuery(CONNECTION_STRING, CommandType.StoredProcedure, USP_DISCARD, params)
        '傳回1代表報廢成功,-1代表至少有一個財物編號不存在,-2代表至少有一個財物編號已經報廢了
        Return params(4).Value
    End Function

    '取得此財務分類編號可用的序號起始號碼,count代表總共需要幾個沒被佔用的號碼
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
            parms = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_PRNO2, SqlDbType.Char, 10), New SqlParameter(PARAM_NAME, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_ACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_PDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_UNIT, SqlDbType.NVarChar, 6) _
                , New SqlParameter(PARAM_QTY, SqlDbType.Int), New SqlParameter(PARAM_KEEPEMPNO, SqlDbType.VarChar, 4), New SqlParameter(PARAM_KEEPUNIT, SqlDbType.VarChar, 4) _
                , New SqlParameter(PARAM_USEYEAR, SqlDbType.Int), New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_ENDAMT, SqlDbType.Decimal, 9) _
                , New SqlParameter(PARAM_BGACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_BGNAME, SqlDbType.NVarChar, 50), New SqlParameter(PARAM_COMEFROM, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_MODEL, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_ENDDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_ENDREMARK, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_BORROW, SqlDbType.NVarChar, 50), New SqlParameter(PARAM_MATERIAL, SqlDbType.NVarChar, 10), New SqlParameter(PARAM_USES, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_MADEDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_REMARK, SqlDbType.NVarChar, 50), New SqlParameter(PARAM_PLACE, SqlDbType.NVarChar, 10) _
                , New SqlParameter(PARAM_SPEC_REMARK, SqlDbType.NVarChar, 100), New SqlParameter(PARAM_REVISED_EMPNO, SqlDbType.VarChar, 4), New SqlParameter(PARAM_RETURN, SqlDbType.Int)}
            parms(26).Direction = ParameterDirection.ReturnValue
            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_INSERT, parms)
        End If
        Return parms
    End Function

    Private Shared Function GetQueryParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_QUERY)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_PRNO2, SqlDbType.Char, 10), New SqlParameter(PARAM_NAME, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_ACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_ACNO2, SqlDbType.VarChar, 17), New SqlParameter(PARAM_PDATE, SqlDbType.SmallDateTime) _
                , New SqlParameter(PARAM_PDATE2, SqlDbType.SmallDateTime), New SqlParameter(PARAM_KEEPEMPNO, SqlDbType.VarChar, 4), New SqlParameter(PARAM_KEEPEMPNO2, SqlDbType.VarChar, 4) _
                , New SqlParameter(PARAM_KEEPUNIT, SqlDbType.VarChar, 4), New SqlParameter(PARAM_KEEPUNIT2, SqlDbType.VarChar, 4), New SqlParameter(PARAM_USEYEAR, SqlDbType.Int) _
                , New SqlParameter(PARAM_USEYEAR2, SqlDbType.Int), New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_AMT2, SqlDbType.Decimal, 9) _
                , New SqlParameter(PARAM_BGACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_BGACNO2, SqlDbType.VarChar, 17), New SqlParameter(PARAM_COMEFROM, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_MODEL, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_ENDDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_ENDDATE2, SqlDbType.SmallDateTime) _
                , New SqlParameter(PARAM_USES, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_SPEC_REMARK, SqlDbType.NVarChar, 100), New SqlParameter(PARAM_REVISED_DATE, SqlDbType.DateTime) _
                , New SqlParameter(PARAM_REVISED_DATE2, SqlDbType.DateTime), New SqlParameter(PARAM_QUERY_DISCARD_MODE, SqlDbType.TinyInt)}

            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_QUERY, parms)
        End If
        Return parms
    End Function

    Private Shared Function GetQueryStaParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_QUERY_STA)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_PRNO2, SqlDbType.Char, 10), New SqlParameter(PARAM_NAME, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_ACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_ACNO2, SqlDbType.VarChar, 17), New SqlParameter(PARAM_PDATE, SqlDbType.SmallDateTime) _
                , New SqlParameter(PARAM_PDATE2, SqlDbType.SmallDateTime), New SqlParameter(PARAM_KEEPEMPNO, SqlDbType.VarChar, 4), New SqlParameter(PARAM_KEEPEMPNO2, SqlDbType.VarChar, 4) _
                , New SqlParameter(PARAM_KEEPUNIT, SqlDbType.VarChar, 4), New SqlParameter(PARAM_KEEPUNIT2, SqlDbType.VarChar, 4), New SqlParameter(PARAM_USEYEAR, SqlDbType.Int) _
                , New SqlParameter(PARAM_USEYEAR2, SqlDbType.Int), New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_AMT2, SqlDbType.Decimal, 9) _
                , New SqlParameter(PARAM_BGACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_BGACNO2, SqlDbType.VarChar, 17), New SqlParameter(PARAM_COMEFROM, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_MODEL, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_ENDDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_ENDDATE2, SqlDbType.SmallDateTime) _
                , New SqlParameter(PARAM_USES, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_SPEC_REMARK, SqlDbType.NVarChar, 100), New SqlParameter(PARAM_REVISED_DATE, SqlDbType.DateTime) _
                , New SqlParameter(PARAM_REVISED_DATE2, SqlDbType.DateTime), New SqlParameter(PARAM_QUERY_DISCARD_MODE, SqlDbType.TinyInt), New SqlParameter(PARAM_DEPRECIATIONDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_DEPRECIATIONDATE2, SqlDbType.SmallDateTime)}

            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_QUERY_STA, parms)
        End If
        Return parms
    End Function

    Private Shared Function GetTransferListParameters() As SqlParameter()
        Dim parms As SqlParameter() = SqlHelperParameterCache.GetCachedParameterSet(CONNECTION_STRING, USP_QUERY_TRANSFERLIST)

        If parms Is Nothing Then
            parms = New SqlParameter() {New SqlParameter(PARAM_PRNO, SqlDbType.Char, 10), New SqlParameter(PARAM_PRNO2, SqlDbType.Char, 10), New SqlParameter(PARAM_NAME, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_ACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_ACNO2, SqlDbType.VarChar, 17), New SqlParameter(PARAM_PDATE, SqlDbType.SmallDateTime) _
                , New SqlParameter(PARAM_PDATE2, SqlDbType.SmallDateTime), New SqlParameter(PARAM_KEEPEMPNO, SqlDbType.VarChar, 4), New SqlParameter(PARAM_KEEPEMPNO2, SqlDbType.VarChar, 4) _
                , New SqlParameter(PARAM_KEEPUNIT, SqlDbType.VarChar, 4), New SqlParameter(PARAM_KEEPUNIT2, SqlDbType.VarChar, 4), New SqlParameter(PARAM_USEYEAR, SqlDbType.Int) _
                , New SqlParameter(PARAM_USEYEAR2, SqlDbType.Int), New SqlParameter(PARAM_AMT, SqlDbType.Decimal, 9), New SqlParameter(PARAM_AMT2, SqlDbType.Decimal, 9) _
                , New SqlParameter(PARAM_BGACNO, SqlDbType.VarChar, 17), New SqlParameter(PARAM_BGACNO2, SqlDbType.VarChar, 17), New SqlParameter(PARAM_COMEFROM, SqlDbType.NVarChar, 30) _
                , New SqlParameter(PARAM_MODEL, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_ENDDATE, SqlDbType.SmallDateTime), New SqlParameter(PARAM_ENDDATE2, SqlDbType.SmallDateTime) _
                , New SqlParameter(PARAM_USES, SqlDbType.NVarChar, 30), New SqlParameter(PARAM_SPEC_REMARK, SqlDbType.NVarChar, 100), New SqlParameter(PARAM_REVISED_DATE, SqlDbType.DateTime) _
                , New SqlParameter(PARAM_REVISED_DATE2, SqlDbType.DateTime), New SqlParameter(PARAM_QUERY_DISCARD_MODE, SqlDbType.TinyInt), New SqlParameter("@transfer_date", SqlDbType.SmallDateTime), New SqlParameter("@transfer_date2", SqlDbType.SmallDateTime)}

            SqlHelperParameterCache.CacheParameterSet(CONNECTION_STRING, USP_QUERY_TRANSFERLIST, parms)
        End If
        Return parms
    End Function


    Private Shared Sub SetInsertParameters(ByVal parms As SqlParameter(), ByVal info As PGMainInfo)
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
            If .Name = String.Empty Then
                parms(2).Value = DBNull.Value
            Else
                parms(2).Value = .Name
            End If
            If .ACNO = String.Empty Then
                parms(3).Value = DBNull.Value
            Else
                parms(3).Value = .ACNO
            End If
            If .PDate = String.Empty Then
                parms(4).Value = DBNull.Value
            Else
                parms(4).Value = .PDate
            End If
            If .Unit = String.Empty Then
                parms(5).Value = DBNull.Value
            Else
                parms(5).Value = .Unit
            End If
            If .QTY = String.Empty Then
                parms(6).Value = DBNull.Value
            Else
                parms(6).Value = .QTY
            End If
            If .KeepEmpNO = String.Empty Then
                parms(7).Value = DBNull.Value
            Else
                parms(7).Value = .KeepEmpNO
            End If
            If .KeepUnit = String.Empty Then
                parms(8).Value = DBNull.Value
            Else
                parms(8).Value = .KeepUnit
            End If
            If .UseYear = String.Empty Then
                parms(9).Value = DBNull.Value
            Else
                parms(9).Value = .UseYear
            End If
            If .AMT = String.Empty Then
                parms(10).Value = DBNull.Value
            Else
                parms(10).Value = .AMT
            End If
            If .EndAMT = String.Empty Then
                parms(11).Value = DBNull.Value
            Else
                parms(11).Value = .EndAMT
            End If
            If .BGACNO = String.Empty Then
                parms(12).Value = DBNull.Value
            Else
                parms(12).Value = .BGACNO
            End If
            If .BGName = String.Empty Then
                parms(13).Value = DBNull.Value
            Else
                parms(13).Value = .BGName
            End If
            If .ComeFrom = String.Empty Then
                parms(14).Value = DBNull.Value
            Else
                parms(14).Value = .ComeFrom
            End If
            If .Model = String.Empty Then
                parms(15).Value = DBNull.Value
            Else
                parms(15).Value = .Model
            End If
            If .EndDate = String.Empty Then
                parms(16).Value = DBNull.Value
            Else
                parms(16).Value = .EndDate
            End If
            If .EndRemark = String.Empty Then
                parms(17).Value = DBNull.Value
            Else
                parms(17).Value = .EndRemark
            End If
            If .Borrow = String.Empty Then
                parms(18).Value = DBNull.Value
            Else
                parms(18).Value = .Borrow
            End If
            If .Material = String.Empty Then
                parms(19).Value = DBNull.Value
            Else
                parms(19).Value = .Material
            End If
            If .Uses = String.Empty Then
                parms(20).Value = DBNull.Value
            Else
                parms(20).Value = .Uses
            End If
            If .MadeDate = String.Empty Then
                parms(21).Value = DBNull.Value
            Else
                parms(21).Value = .MadeDate
            End If
            If .Remark = String.Empty Then
                parms(22).Value = DBNull.Value
            Else
                parms(22).Value = .Remark
            End If
            If .Place = String.Empty Then
                parms(23).Value = DBNull.Value
            Else
                parms(23).Value = .Place
            End If
            If .SpecRemark = String.Empty Then
                parms(24).Value = DBNull.Value
            Else
                parms(24).Value = .SpecRemark
            End If
            If .RevisedEmpNO = String.Empty Then
                parms(25).Value = DBNull.Value
            Else
                parms(25).Value = .RevisedEmpNO
            End If
        End With

    End Sub

    Private Shared Sub SetQueryParameters(ByVal parms As SqlParameter(), ByVal info As PGMainInfo)
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
            If .Name = String.Empty Then
                parms(2).Value = DBNull.Value
            Else
                parms(2).Value = .Name
            End If
            If .ACNO = String.Empty Then
                parms(3).Value = DBNull.Value
            Else
                parms(3).Value = .ACNO
            End If
            If .ACNO2 = String.Empty Then
                parms(4).Value = DBNull.Value
            Else
                parms(4).Value = .ACNO2
            End If
            If .PDate = String.Empty Then
                parms(5).Value = DBNull.Value
            Else
                parms(5).Value = .PDate
            End If
            If .PDate2 = String.Empty Then
                parms(6).Value = DBNull.Value
            Else
                parms(6).Value = .PDate2
            End If
            If .KeepEmpNO = String.Empty Then
                parms(7).Value = DBNull.Value
            Else
                parms(7).Value = .KeepEmpNO
            End If
            If .KeepEmpNO2 = String.Empty Then
                parms(8).Value = DBNull.Value
            Else
                parms(8).Value = .KeepEmpNO2
            End If
            If .KeepUnit = String.Empty Then
                parms(9).Value = DBNull.Value
            Else
                parms(9).Value = .KeepUnit
            End If
            If .KeepUnit2 = String.Empty Then
                parms(10).Value = DBNull.Value
            Else
                parms(10).Value = .KeepUnit2
            End If
            If .UseYear = String.Empty Then
                parms(11).Value = DBNull.Value
            Else
                parms(11).Value = .UseYear
            End If
            If .UseYear2 = String.Empty Then
                parms(12).Value = DBNull.Value
            Else
                parms(12).Value = .UseYear2
            End If
            If .AMT = String.Empty Then
                parms(13).Value = DBNull.Value
            Else
                parms(13).Value = .AMT
            End If
            If .AMT2 = String.Empty Then
                parms(14).Value = DBNull.Value
            Else
                parms(14).Value = .AMT2
            End If
            If .BGACNO = String.Empty Then
                parms(15).Value = DBNull.Value
            Else
                parms(15).Value = .BGACNO
            End If
            If .BGACNO2 = String.Empty Then
                parms(16).Value = DBNull.Value
            Else
                parms(16).Value = .BGACNO2
            End If
            If .ComeFrom = String.Empty Then
                parms(17).Value = DBNull.Value
            Else
                parms(17).Value = .ComeFrom
            End If
            If .Model = String.Empty Then
                parms(18).Value = DBNull.Value
            Else
                parms(18).Value = .Model
            End If
            If .EndDate = String.Empty Then
                parms(19).Value = DBNull.Value
            Else
                parms(19).Value = .EndDate
            End If
            If .EndDate2 = String.Empty Then
                parms(20).Value = DBNull.Value
            Else
                parms(20).Value = .EndDate2
            End If
            If .Uses = String.Empty Then
                parms(21).Value = DBNull.Value
            Else
                parms(21).Value = .Uses
            End If
            If .SpecRemark = String.Empty Then
                parms(22).Value = DBNull.Value
            Else
                parms(22).Value = .SpecRemark
            End If
            If .RevisedDate = String.Empty Then
                parms(23).Value = DBNull.Value
            Else
                parms(23).Value = .RevisedDate
            End If
            If .RevisedDate2 = String.Empty Then
                parms(24).Value = DBNull.Value
            Else
                parms(24).Value = .RevisedDate2
            End If
            parms(25).Value = .QueryDiscardMode

        End With

    End Sub

    Private Shared Sub SetQueryStaParameters(ByVal parms As SqlParameter(), ByVal info As PGMainInfo, ByVal depreciationDate As String, ByVal depreciationDate2 As String)
        SetQueryParameters(parms, info)
        If depreciationDate = String.Empty Then
            parms(26).Value = DBNull.Value
        Else
            parms(26).Value = depreciationDate
        End If
        If depreciationDate2 = String.Empty Then
            parms(27).Value = DBNull.Value
        Else
            parms(27).Value = depreciationDate2
        End If
    End Sub

    Private Shared Sub SetTransferListParameters(ByVal parms As SqlParameter(), ByVal info As PGMainInfo, ByVal transferDate As String, ByVal transferDate2 As String)
        SetQueryParameters(parms, info)
        parms(26).Value = transferDate
        parms(27).Value = transferDate2
    End Sub

End Class
