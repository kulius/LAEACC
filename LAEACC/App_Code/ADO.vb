'::::::::::::::::::
'::: 資料庫處理 :::
'::::::::::::::::::
Imports System.Data
Imports System.Data.SqlClient

Public Class ADO
    Dim objDADB As SqlDataAdapter
    Dim objDSDB As DataSet

    '資料庫共用變數
    Dim objConDB As SqlConnection
    Dim objCmdDB As SqlCommand
    Dim objDRDB As SqlDataReader
    Dim strSQLDB As String

    '組合SQL
    Dim UpdSqlValue As String, InsField, InsValue As String

#Region "ADO副程式"

#End Region

#Region "資料測試"
    ''' <summary>
    ''' 資料庫連結字串測試
    ''' </summary>
    ''' <param name="DNS">連結字串</param>
    ''' <returns>是否成功連結</returns>
    Function dbConnectionTest(ByVal strDNS As String) As Boolean
        Dim blnCheck As Boolean = False

        Try
            objConDB = New SqlConnection(strDNS)
            objConDB.Open()
            objConDB.Close()   '關閉連結
            objConDB.Dispose()
            objConDB = Nothing '移除指標

            blnCheck = True

        Catch ex As Exception
            blnCheck = False
        End Try

        Return blnCheck
    End Function
#End Region

#Region "資料處理"
    ''' <summary>
    ''' 執行SQL語法
    ''' </summary>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strSQL">SQL語法</param>
    ''' <returns>True:執行成功 False:執行失敗</returns>    
    Function dbExecute(ByVal strDNS As String, ByVal strSQL As String) As Boolean
        Dim blnCheck As Boolean = False

        Try
            objConDB = New SqlConnection(strDNS)
            objConDB.Open()

            strSQLDB = strSQL

            objCmdDB = New SqlCommand(strSQLDB, objConDB)
            objCmdDB.ExecuteNonQuery()

            objConDB.Close()   '關閉連結
            objCmdDB.Dispose() '手動釋放資源
            objConDB.Dispose()
            objConDB = Nothing '移除指標


            '--傳回訊息--
            blnCheck = True
        Catch ex As Exception
            blnCheck = False
        End Try

        Return blnCheck
    End Function

    Function runsql(ByVal strDNS As String, ByVal strSQL As String) As String
        Dim blnCheck As String = "sqlno"

        Try
            objConDB = New SqlConnection(strDNS)
            objConDB.Open()

            strSQLDB = strSQL

            objCmdDB = New SqlCommand(strSQLDB, objConDB)
            objCmdDB.ExecuteNonQuery()

            objConDB.Close()   '關閉連結
            objCmdDB.Dispose() '手動釋放資源
            objConDB.Dispose()
            objConDB = Nothing '移除指標


            '--傳回訊息--
            blnCheck = "sqlok"
        Catch ex As Exception
            blnCheck = "sqlno"
        End Try

        Return blnCheck
    End Function

    ''' <summary>
    ''' 新增資料
    ''' </summary>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTable">寫入資料表</param>
    ''' <param name="strRow">寫入欄位</param>
    ''' <param name="strValue">寫入資料</param>
    ''' <returns>True:寫入成功 False:寫入失敗</returns>    
    Function dbInsert(ByVal strDNS As String, ByVal strTable As String, ByVal strRow As String, ByVal strValue As String) As Boolean
        Dim blnCheck As Boolean = False

        Try
            objConDB = New SqlConnection(strDNS)
            objConDB.Open()

            strSQLDB = "INSERT INTO " & strTable & " WITH (HOLDLOCK) (" & strRow & ")"
            strSQLDB &= " VALUES (" & strValue & ")"

            objCmdDB = New SqlCommand(strSQLDB, objConDB)
            objCmdDB.ExecuteNonQuery()

            objConDB.Close()   '關閉連結
            objCmdDB.Dispose() '手動釋放資源
            objConDB.Dispose()
            objConDB = Nothing '移除指標


            '--傳回訊息--
            blnCheck = True
        Catch ex As Exception
            blnCheck = False
        End Try

        Return blnCheck
    End Function

    ''' <summary>
    ''' 修改資料
    ''' </summary>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTable">修改資料表</param>
    ''' <param name="strEdit">修改欄位</param>
    ''' <param name="strWhere">修改條件值</param>
    ''' <returns>True:修改成功 False:修改失敗</returns>    
    Function dbEdit(ByVal strDNS As String, ByVal strTable As String, ByVal strEdit As String, ByVal strWhere As String) As Boolean
        Dim blnCheck As Boolean = False

        Try
            objConDB = New SqlConnection(strDNS)
            objConDB.Open()

            strSQLDB = "UPDATE " & strTable & " SET"
            strSQLDB &= " " & strEdit
            strSQLDB &= " WHERE " & strWhere

            objCmdDB = New SqlCommand(strSQLDB, objConDB)
            objCmdDB.ExecuteNonQuery()

            objConDB.Close()   '關閉連結
            objCmdDB.Dispose() '手動釋放資源
            objConDB.Dispose()
            objConDB = Nothing '移除指標


            '--傳回訊息--
            blnCheck = True
        Catch ex As Exception
            blnCheck = False
        End Try

        Return blnCheck
    End Function

    ''' <summary>
    ''' 刪除資料
    ''' </summary>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTable">刪除資料表</param>
    ''' <param name="strWhere">刪除條件值</param>
    ''' <returns>True:刪除成功 False:刪除失敗</returns>    
    Function dbDelete(ByVal strDNS As String, ByVal strTable As String, ByVal strWhere As String) As Boolean
        Dim blnCheck As Boolean = False

        Try
            objConDB = New SqlConnection(strDNS)
            objConDB.Open()

            strSQLDB = "DELETE FROM " & strTable
            strSQLDB &= " WHERE " & strWhere

            objCmdDB = New SqlCommand(strSQLDB, objConDB)
            objCmdDB.ExecuteNonQuery()

            objConDB.Close()   '關閉連結
            objCmdDB.Dispose() '手動釋放資源
            objConDB.Dispose()
            objConDB = Nothing '移除指標


            '--傳回訊息--
            blnCheck = True
        Catch ex As Exception
            blnCheck = False
        End Try

        Return blnCheck
    End Function
#End Region

#Region "資料查詢"
    Function openmember(ByVal strDNS As String, ByVal membername As String, ByVal sqlstr As String) As System.Data.DataSet

        objConDB = New SqlConnection(strDNS)
        objConDB.Open()

        objDADB = New SqlDataAdapter(sqlstr, objConDB)
        objDSDB = New DataSet()

        objDADB.Fill(objDSDB, membername)
        Return objDSDB

        objDSDB.Clear()    '關閉連結
        objConDB.Close()
        objDADB.Dispose()  '手動釋放資源
        objDSDB.Dispose()
        objConDB.Dispose()
        objConDB = Nothing '移除指標
    End Function

    ''' <summary>
    ''' 是否有相同關鍵值資料
    ''' </summary>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTable">查詢資料表</param>
    ''' <param name="strWhere">查詢條件值</param>
    ''' <returns>True:有相同資料 False:無相同資料</returns>
    Function dbDataCheck(ByVal strDNS As String, ByVal strTable As String, ByVal strWhere As String) As Boolean
        Dim blnCheck As Boolean = False

        '開啟查詢
        objConDB = New SqlConnection(strDNS)
        objConDB.Open()

        strSQLDB = "SELECT * FROM " & strTable
        strSQLDB &= " WHERE " & strWhere

        objCmdDB = New SqlCommand(strSQLDB, objConDB)
        objDRDB = objCmdDB.ExecuteReader()

        If objDRDB.Read Then blnCheck = True

        objDRDB.Close()    '關閉連結
        objConDB.Close()
        objCmdDB.Dispose() '手動釋放資源
        objConDB.Dispose()
        objConDB = Nothing '移除指標

        Return blnCheck
    End Function

    ''' <summary>
    ''' 取得單一欄位
    ''' </summary>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTable">查詢資料表</param>
    ''' <param name="strRow">欄位值名稱</param>
    ''' <param name="strWhere">查詢條件值</param>
    ''' <param name="strOrderBy">自定排序</param>
    ''' <returns>傳回單一欄位值</returns>
    Function dbGetRow(ByVal strDNS As String, ByVal strTable As String, ByVal strRow As String, ByVal strWhere As String, Optional ByVal strOrderBy As String = "") As String
        Dim strString As String = ""

        '開啟查詢
        objConDB = New SqlConnection(strDNS)
        objConDB.Open()

        strSQLDB = "SELECT " & strRow & " AS Row FROM " & strTable
        If strWhere <> "" Then strSQLDB &= " WHERE " & strWhere
        If strOrderBy <> "" Then strSQLDB &= " ORDER BY " & strOrderBy

        objCmdDB = New SqlCommand(strSQLDB, objConDB)
        objDRDB = objCmdDB.ExecuteReader()

        If objDRDB.Read Then strString = objDRDB("Row").ToString

        objDRDB.Close()    '關閉連結
        objConDB.Close()
        objCmdDB.Dispose() '手動釋放資源
        objConDB.Dispose()
        objConDB = Nothing '移除指標

        Return strString
    End Function
#End Region

#Region "SQL 組合"
    Function cutright1(ByVal instr As String, ByVal compstr As String)
        instr = Trim(instr)
        If Len(instr) > 0 Then
            Return IIf(Right(instr, 1) = compstr, Left(instr, Len(instr) - 1), instr)
        Else
            Return instr
        End If
    End Function

    Public Property GenInsFunc()
        Get
            Dim InsField1, InsValue1 As String
            InsField1 = cutright1(InsField, ",")
            InsValue1 = cutright1(InsValue, ",")
            InsField = "" : InsValue = ""
            Return "(" & InsField1 & ") values (" & InsValue1 & ")"
        End Get
        Set(ByVal Value)

        End Set
    End Property

    Sub GenInsSql(ByRef FieldName As String, ByVal FieldValue As String, ByVal FieldType As String)
        Select Case FieldType
            Case Is = "T"
                If FieldValue <> "" Then
                    InsField &= FieldName & ","
                    InsValue &= "'" & FieldValue & "',"
                Else
                    InsField &= FieldName & ","
                    InsValue &= "'',"
                End If
            Case Is = "U"
                If FieldValue <> "" Then
                    InsField &= FieldName & ","
                    InsValue &= "N'" & FieldValue & "',"
                Else
                    InsField &= FieldName & ","
                    InsValue &= "'',"
                End If
            Case Is = "D"
                If FieldValue <> "" Then
                    Dim yy As Integer, mmdd As String
                    InsField &= FieldName & ","
                    yy = Mid(FieldValue, 1, InStr(FieldValue, "-") - 1)
                    If yy < 1000 Then yy = yy + 1911
                    mmdd = Mid(FieldValue, InStr(FieldValue, "-"))
                    InsValue &= "'" & yy & mmdd & "',"
                End If
            Case Is = "N", "R"
                If FieldValue <> "" Then
                    InsField &= FieldName & ","
                    InsValue &= Val(Replace(FieldValue, ",", "")) & ","
                End If
            Case Is = "H"
                If FieldValue <> "" Then
                    Dim yy As Integer, mmdd As String
                    InsField &= FieldName & ","
                    yy = Mid(FieldValue, 1, InStr(FieldValue, "/") - 1)
                    If yy < 1000 Then yy = yy + 1911
                    mmdd = Mid(FieldValue, InStr(FieldValue, "/"))
                    InsValue &= "'" & yy & mmdd & "',"
                End If
        End Select
    End Sub

    Function nz(ByVal inputvalue, ByVal nullvalue)
        Return IIf(IsDBNull(inputvalue), nullvalue, inputvalue)
    End Function

    Public Property genupdfunc()
        Get
            Dim UpdSqlValue1 As String
            UpdSqlValue1 = cutright1(UpdSqlValue, ",")
            UpdSqlValue = ""
            Return UpdSqlValue1
        End Get
        Set(ByVal Value)

        End Set
    End Property

    Sub GenUpdsql(ByVal FieldName As String, ByVal FieldValue As String, ByVal FieldType As String)
        Dim yy, atp As Integer

        Select Case FieldType

            Case "T"
                If nz(FieldValue, "") = "" Then
                    UpdSqlValue &= FieldName & "= ' ',"   'put space(1) to textfield
                Else
                    UpdSqlValue &= FieldName & "='" & Replace(FieldValue, "'", "''") & "', "
                End If
            Case "U"
                If nz(FieldValue, "") = "" Then
                    UpdSqlValue &= FieldName & "= ' ',"   'put space(1) to textfield
                Else
                    UpdSqlValue &= FieldName & "=N'" & Replace(FieldValue, "'", "''") & "', "
                End If
            Case "N", "R"
                UpdSqlValue &= FieldName & "=" & Val(Replace(FieldValue, ",", "")) & ", "
            Case "D"
                If Len(Trim(FieldValue)) = 0 Or Mid(FieldValue, 1, 4) = "0001" Then
                    UpdSqlValue &= FieldName & "= null, "
                Else
                    atp = InStr(FieldValue, "-")
                    'yy = FieldValue.leftLeft(FieldValue, atp - 1)
                    yy = Mid(FieldValue, 1, atp - 1)

                    If yy < 1000 Then yy += 1911
                    Dim indate As Date
                    If IsDate(yy & Mid(FieldValue, atp)) Then
                        indate = CDate(yy & Mid(FieldValue, atp))
                        If Format(indate, "HH:mm") = "00:00" Then
                            UpdSqlValue &= FieldName & "='" & Format(indate, "yyyy/MM/dd") & "', "
                        Else
                            UpdSqlValue &= FieldName & "='" & Format(indate, "yyyy/MM/dd HH:mm") & "', "
                        End If
                    End If
                End If

        End Select


    End Sub
#End Region
End Class