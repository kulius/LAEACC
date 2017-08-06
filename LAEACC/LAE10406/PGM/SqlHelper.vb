Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Runtime.CompilerServices
Imports System.Xml


Public Class SqlHelper
    ' Methods
    Private Sub New()
    End Sub

    Private Shared Sub AssignParameterValues(ByVal commandParameters As SqlParameter(), ByVal parameterValues As Object())
        If Not ((commandParameters Is Nothing) And (parameterValues Is Nothing)) Then
            If (commandParameters.Length <> parameterValues.Length) Then
                Throw New ArgumentException("Parameter count does not match Parameter Value count.")
            End If
            Dim num3 As Short = CShort((commandParameters.Length - 1))
            Dim i As Short = 0
            Do While (i <= num3)
                commandParameters(i).Value = RuntimeHelpers.GetObjectValue(parameterValues(i))
                i = CShort((i + 1))
            Loop
        End If
    End Sub

    Private Shared Sub AttachParameters(ByVal command As SqlCommand, ByVal commandParameters As SqlParameter())
        Dim parameter As SqlParameter
        For Each parameter In commandParameters
            If ((parameter.Direction = ParameterDirection.InputOutput) And (parameter.Value Is Nothing)) Then
                parameter.Value = Nothing
            End If
            command.Parameters.Add(parameter)
        Next
    End Sub

    Public Shared Function ExecuteDataset(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String) As DataSet
        Return SqlHelper.ExecuteDataset(connection, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteDataset(ByVal connection As SqlConnection, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As DataSet
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteDataset(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String) As DataSet
        Return SqlHelper.ExecuteDataset(transaction, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteDataset(ByVal transaction As SqlTransaction, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As DataSet
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteDataset(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String) As DataSet
        'Return SqlHelper.ExecuteDataset(connectionString, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteDataset(ByVal connectionString As String, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As DataSet
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteDataset(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As DataSet
        Dim command As New SqlCommand
        Dim dataSet As New DataSet

        SqlHelper.PrepareCommand(command, connection, Nothing, commandType, commandText, commandParameters)
        Dim adapter As SqlDataAdapter = New SqlDataAdapter(command)
        adapter.Fill(dataSet)

        command.Parameters.Clear()
        Return dataSet
    End Function

    Public Shared Function ExecuteDataset(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As DataSet
        Dim command As New SqlCommand
        Dim dataSet As New DataSet
        SqlHelper.PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters)
        Dim adapter As SqlDataAdapter = New SqlDataAdapter(command)
        adapter.Fill(dataSet)
        command.Parameters.Clear()
        Return dataSet
    End Function

    Public Shared Function ExecuteDataset(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As DataSet
        Using connection As SqlConnection = New SqlConnection(connectionString)
            connection.Open()
            Return SqlHelper.ExecuteDataset(connection, commandType, commandText, commandParameters)
        End Using
    End Function

    Public Shared Function ExecuteDataset(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String, ByVal tableName As String, ByVal ParamArray commandParameters As SqlParameter()) As DataSet
        Using connection As SqlConnection = New SqlConnection(connectionString)
            connection.Open()
            Dim command As New SqlCommand
            Dim dataSet As New DataSet
            SqlHelper.PrepareCommand(command, connection, Nothing, commandType, commandText, commandParameters)
            Dim adapter As SqlDataAdapter = New SqlDataAdapter(command)
            adapter.Fill(dataSet, tableName)
            'New SqlDataAdapter(command).Fill(dataSet, tableName)
            command.Parameters.Clear()
            Return dataSet
        End Using
    End Function

    Public Shared Function ExecuteNonQuery(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String) As Integer
        Return SqlHelper.ExecuteNonQuery(connection, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteNonQuery(ByVal connection As SqlConnection, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As Integer
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteNonQuery(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String) As Integer
        Return SqlHelper.ExecuteNonQuery(transaction, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteNonQuery(ByVal transaction As SqlTransaction, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As Integer
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteNonQuery(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String) As Integer
        Return SqlHelper.ExecuteNonQuery(connectionString, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteNonQuery(ByVal connectionString As String, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As Integer
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteNonQuery(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As Integer
        Dim command As New SqlCommand
        SqlHelper.PrepareCommand(command, connection, Nothing, commandType, commandText, commandParameters)
        Dim num2 As Integer = command.ExecuteNonQuery
        command.Parameters.Clear()
        Return num2
    End Function

    Public Shared Function ExecuteNonQuery(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As Integer
        Dim command As New SqlCommand
        SqlHelper.PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters)
        Dim num2 As Integer = command.ExecuteNonQuery
        command.Parameters.Clear()
        Return num2
    End Function

    Public Shared Function ExecuteNonQuery(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As Integer
        Using connection As SqlConnection = New SqlConnection(connectionString)
            connection.Open()
            Return SqlHelper.ExecuteNonQuery(connection, commandType, commandText, commandParameters)
        End Using
    End Function

    Public Shared Function ExecuteReader(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String) As SqlDataReader
        Return SqlHelper.ExecuteReader(connection, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteReader(ByVal connection As SqlConnection, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As SqlDataReader
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteReader(StringType.FromInteger(4), spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteReader(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String) As SqlDataReader
        Return SqlHelper.ExecuteReader(transaction, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteReader(ByVal transaction As SqlTransaction, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As SqlDataReader
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteReader(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String) As SqlDataReader
        Return SqlHelper.ExecuteReader(connectionString, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteReader(ByVal connectionString As String, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As SqlDataReader
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteReader(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As SqlDataReader
        Return SqlHelper.ExecuteReader(connection, Nothing, commandType, commandText, commandParameters, SqlConnectionOwnership.External)
    End Function

    Public Shared Function ExecuteReader(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As SqlDataReader
        Return SqlHelper.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External)
    End Function

    Public Shared Function ExecuteReader(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As SqlDataReader
        Dim reader As SqlDataReader
        Dim connection As New SqlConnection(connectionString)
        connection.Open()
        Try
            reader = SqlHelper.ExecuteReader(connection, Nothing, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal)
        Catch exception1 As Exception
            ProjectData.SetProjectError(exception1)
            Dim exception As Exception = exception1
            connection.Dispose()
            Throw exception
        End Try
        Return reader
    End Function

    Private Shared Function ExecuteReader(ByVal connection As SqlConnection, ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String, ByVal commandParameters As SqlParameter(), ByVal connectionOwnership As SqlConnectionOwnership) As SqlDataReader
        Dim reader As SqlDataReader
        Dim command As New SqlCommand
        SqlHelper.PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters)
        If (connectionOwnership = SqlConnectionOwnership.External) Then
            reader = command.ExecuteReader
        Else
            reader = command.ExecuteReader(CommandBehavior.CloseConnection)
        End If
        command.Parameters.Clear()
        Return reader
    End Function

    Public Shared Function ExecuteScalar(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String) As Object
        Return SqlHelper.ExecuteScalar(connection, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteScalar(ByVal connection As SqlConnection, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As Object
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteScalar(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String) As Object
        Return SqlHelper.ExecuteScalar(transaction, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteScalar(ByVal transaction As SqlTransaction, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As Object
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteScalar(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String) As Object
        Return SqlHelper.ExecuteScalar(connectionString, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteScalar(ByVal connectionString As String, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As Object
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteScalar(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As Object
        Dim command As New SqlCommand
        SqlHelper.PrepareCommand(command, connection, Nothing, commandType, commandText, commandParameters)
        Dim objectValue As Object = RuntimeHelpers.GetObjectValue(command.ExecuteScalar)
        command.Parameters.Clear()
        Return objectValue
    End Function

    Public Shared Function ExecuteScalar(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As Object
        Dim command As New SqlCommand
        SqlHelper.PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters)
        Dim objectValue As Object = RuntimeHelpers.GetObjectValue(command.ExecuteScalar)
        command.Parameters.Clear()
        Return objectValue
    End Function

    Public Shared Function ExecuteScalar(ByVal connectionString As String, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As Object
        Using connection As SqlConnection = New SqlConnection(connectionString)
            connection.Open()
            Return SqlHelper.ExecuteScalar(connection, commandType, commandText, commandParameters)
        End Using
    End Function

    Public Shared Function ExecuteXmlReader(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String) As XmlReader
        Return SqlHelper.ExecuteXmlReader(connection, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteXmlReader(ByVal connection As SqlConnection, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As XmlReader
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteXmlReader(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String) As XmlReader
        Return SqlHelper.ExecuteXmlReader(transaction, commandType, commandText, Nothing)
    End Function

    Public Shared Function ExecuteXmlReader(ByVal transaction As SqlTransaction, ByVal spName As String, ByVal ParamArray parameterValues As Object()) As XmlReader
        If ((Not parameterValues Is Nothing) And (parameterValues.Length > 0)) Then
            Dim spParameterSet As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName)
            SqlHelper.AssignParameterValues(spParameterSet, parameterValues)
            Return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, spParameterSet)
        End If
        Return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName)
    End Function

    Public Shared Function ExecuteXmlReader(ByVal connection As SqlConnection, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As XmlReader
        Dim command As New SqlCommand
        SqlHelper.PrepareCommand(command, connection, Nothing, commandType, commandText, commandParameters)
        Dim reader2 As XmlReader = command.ExecuteXmlReader
        command.Parameters.Clear()
        Return reader2
    End Function

    Public Shared Function ExecuteXmlReader(ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter()) As XmlReader
        Dim command As New SqlCommand
        SqlHelper.PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters)
        Dim reader2 As XmlReader = command.ExecuteXmlReader
        command.Parameters.Clear()
        Return reader2
    End Function

    Public Shared Function GetCommandTimeout() As Integer
        Dim str As String = "300"
        If (StringType.StrCmp(str, String.Empty, False) = 0) Then
            str = "300"
        End If
        Return IntegerType.FromString(str)
    End Function

    Private Shared Sub PrepareCommand(ByVal command As SqlCommand, ByVal connection As SqlConnection, ByVal transaction As SqlTransaction, ByVal commandType As CommandType, ByVal commandText As String, ByVal commandParameters As SqlParameter())
        If (connection.State <> ConnectionState.Open) Then
            connection.Open()
        End If
        command.CommandTimeout = SqlHelper.mCommandTimeout
        command.Connection = connection
        command.CommandText = commandText
        If (Not transaction Is Nothing) Then
            command.Transaction = transaction
        End If
        command.CommandType = commandType
        If (Not commandParameters Is Nothing) Then
            SqlHelper.AttachParameters(command, commandParameters)
        End If
    End Sub


    ' Fields
    Private Shared mCommandTimeout As Integer = SqlHelper.GetCommandTimeout

    ' Nested Types
    Private Enum SqlConnectionOwnership
        ' Fields
        External = 1
        Internal = 0
    End Enum
End Class


