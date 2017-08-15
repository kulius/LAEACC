
<Serializable()> Public Class TransferInfo
    Private mID As String
    Private mPrNo As String
    Private mPDate As String
    Private mKeepEmpNo As String
    Private mKeepEmpName As String
    Private mKeepUnit As String
    Private mKeepUnitName As String

    '此建構子適合從資料庫查詢特定財物的交接紀錄
    Public Sub New(ByVal prNO As String)
        mPrNo = prNO
    End Sub

    Public ReadOnly Property ID() As String
        Get
            Return mID
        End Get
    End Property

    Public ReadOnly Property PRNO() As String
        Get
            Return mPrNo
        End Get
    End Property

    Public ReadOnly Property PDate() As String
        Get
            Return mPDate
        End Get
    End Property

    Public ReadOnly Property KeepEmpNO() As String
        Get
            Return mKeepEmpNo
        End Get
    End Property

    Public ReadOnly Property KeepEmpName() As String
        Get
            Return mKeepEmpName
        End Get
    End Property

    Public ReadOnly Property KeepUnit() As String
        Get
            Return mKeepUnit
        End Get
    End Property

    Public ReadOnly Property KeepUnitName() As String
        Get
            Return mKeepUnitName
        End Get
    End Property

End Class
