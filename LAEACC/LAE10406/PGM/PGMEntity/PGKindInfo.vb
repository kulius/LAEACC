<Serializable()> Public Class PGKindInfo

    Private mKindNo As String
    Private mKindNo2 As String '查詢時,起訖區間的終止編號
    Private mName As String
    Private mUnit As String
    Private mMaterial As String
    Private mUseYear As String

    Public IsValidNode As Boolean  '是否為有效節點

    Public Sub New()

    End Sub

    Public Sub New(ByVal kindno As String)
        mKindNo = kindno
    End Sub

    Public Sub New(ByVal kindno As String, ByVal name As String, ByVal unit As String, ByVal material As String, ByVal useyear As String)
        mKindNo = kindno
        mName = name
        mUnit = unit
        mMaterial = material
        mUseYear = useyear
    End Sub

    Public Sub New(ByVal kindno As String, ByVal name As String, ByVal unit As String, ByVal material As String, ByVal useyear As String, ByVal kindno2 As String)
        Me.New(kindno, name, unit, material, useyear)
        mKindNo2 = kindno2
    End Sub

    Public Sub New(ByVal kindno As String, ByVal name As String, ByVal unit As String, ByVal material As String, ByVal useyear As String, ByVal isNodeValid As Boolean)
        Me.New(kindno, name, unit, material, useyear)
        IsValidNode = isNodeValid
    End Sub

    Public ReadOnly Property KindNo() As String
        Get
            Return mKindNo
        End Get
    End Property

    Public ReadOnly Property KindNo2() As String
        Get
            Return mKindNo2
        End Get
    End Property

    Public ReadOnly Property Name() As String
        Get
            Return mName
        End Get
    End Property

    Public ReadOnly Property Unit() As String
        Get
            Return mUnit
        End Get
    End Property

    Public ReadOnly Property Material() As String
        Get
            Return mMaterial
        End Get
    End Property

    Public ReadOnly Property UseYear() As String
        Get
            Return mUseYear
        End Get
    End Property
End Class
