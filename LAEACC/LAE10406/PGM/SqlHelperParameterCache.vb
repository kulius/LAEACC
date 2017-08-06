Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections
Imports System.Data
Imports System.Data.SqlClient


Public Class SqlHelperParameterCache
    ' Methods
    Private Sub New()
    End Sub

    Public Shared Sub CacheParameterSet(ByVal connectionString As String, ByVal commandText As String, ByVal ParamArray commandParameters As SqlParameter())
        Dim str As String = (connectionString & ":" & commandText)
        SqlHelperParameterCache.paramCache.Item(str) = commandParameters
    End Sub

    Private Shared Function CloneParameters(ByVal originalParameters As SqlParameter()) As SqlParameter()
        Dim num2 As Short = CShort((originalParameters.Length - 1))
        Dim parameterArray As SqlParameter() = New SqlParameter((num2 + 1) - 1) {}
        Dim num3 As Short = num2
        Dim i As Short = 0
        Do While (i <= num3)
            parameterArray(i) = DirectCast(DirectCast(originalParameters(i), ICloneable).Clone, SqlParameter)
            i = CShort((i + 1))
        Loop
        Return parameterArray
    End Function

    Private Shared Function DiscoverSpParameterSet(ByVal connectionString As String, ByVal spName As String, ByVal includeReturnValueParameter As Boolean, ByVal ParamArray parameterValues As Object()) As SqlParameter()
        Dim parameterArray As SqlParameter()
        Dim connection As New SqlConnection(connectionString)
        Dim command As New SqlCommand(spName, connection)
        Try
            connection.Open()
            command.CommandType = CommandType.StoredProcedure
            SqlCommandBuilder.DeriveParameters(command)
            If Not includeReturnValueParameter Then
                command.Parameters.RemoveAt(0)
            End If
            parameterArray = New SqlParameter(((command.Parameters.Count - 1) + 1) - 1) {}
            command.Parameters.CopyTo(DirectCast(parameterArray, Array), 0)
        Finally
            command.Dispose()
            connection.Dispose()
        End Try
        Return parameterArray
    End Function

    Public Shared Function GetCachedParameterSet(ByVal connectionString As String, ByVal commandText As String) As SqlParameter()
        Dim str As String = (connectionString & ":" & commandText)
        Dim originalParameters As SqlParameter() = DirectCast(SqlHelperParameterCache.paramCache.Item(str), SqlParameter())
        If (originalParameters Is Nothing) Then
            Return Nothing
        End If
        Return SqlHelperParameterCache.CloneParameters(originalParameters)
    End Function

    Public Shared Function GetSpParameterSet(ByVal connectionString As String, ByVal spName As String) As SqlParameter()
        Return SqlHelperParameterCache.GetSpParameterSet(connectionString, spName, False)
    End Function

    Public Shared Function GetSpParameterSet(ByVal connectionString As String, ByVal spName As String, ByVal includeReturnValueParameter As Boolean) As SqlParameter()
        Dim str As String = StringType.FromObject(ObjectType.AddObj((connectionString & ":" & spName), Interaction.IIf(includeReturnValueParameter, ":include ReturnValue Parameter", "")))
        Dim originalParameters As SqlParameter() = DirectCast(SqlHelperParameterCache.paramCache.Item(str), SqlParameter())
        If (originalParameters Is Nothing) Then
            SqlHelperParameterCache.paramCache.Item(str) = SqlHelperParameterCache.DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter, New Object(0 - 1) {})
            originalParameters = DirectCast(SqlHelperParameterCache.paramCache.Item(str), SqlParameter())
        End If
        Return SqlHelperParameterCache.CloneParameters(originalParameters)
    End Function


    ' Fields
    Private Shared paramCache As Hashtable = Hashtable.Synchronized(New Hashtable)
End Class


