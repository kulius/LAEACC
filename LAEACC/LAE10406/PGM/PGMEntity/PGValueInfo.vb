
'財物增減值
<Serializable()> Public Class PGValueInfo

    Private mID As String
    Private mPRNO As String
    Private mPRNO2 As String
    Private mACNO As String
    Private mACNO2 As String
    Private mName As String
    Private mAddDelDate As String '增減日期
    Private mAddDelDate2 As String
    Private mAMT As String
    Private mAMT2 As String
    Private mRemark As String
    Private mPurchaseDate As String '購入日期
    Private mOriginalAmt As String
    Private mEndAmt As String
    Private mTotalAddDel As String
    Private mNetAmt As String
    Private mDepreciation As String
    Private mUseYear As String
    Private mKeepEmpName As String
    Private mKeepUnitName As String
    Private mEndDate As String
    Private mEndRemark As String


    '此建構子適合從資料庫查詢資料要送到client
    Public Sub New(ByVal id As String, ByVal prNO As String, ByVal acNo As String, ByVal addDelDate As String, ByVal amt As String, ByVal remark As String, ByVal name As String, ByVal purchaseDate As String, _
    ByVal originalAmt As String, ByVal endAmt As String, ByVal totalAdddel As String, ByVal netAmt As String, ByVal depreciation As String, ByVal useYear As String, _
    ByVal keepEmpName As String, ByVal keepUnitName As String, ByVal endDate As String, ByVal endRemark As String)
        mID = id
        mPRNO = prNO
        mACNO = acNo
        mAddDelDate = addDelDate
        mAMT = amt
        mRemark = remark
        mName = name
        mPurchaseDate = purchaseDate
        mOriginalAmt = originalAmt
        mEndAmt = endAmt
        mTotalAddDel = totalAdddel
        mNetAmt = netAmt
        mDepreciation = depreciation
        mUseYear = useYear
        mKeepEmpName = KeepEmpName
        mKeepUnitName = KeepUnitName
        mEndDate = EndDate
        mEndRemark = EndRemark
    End Sub

    '此建構子適合下查詢條件到資料庫
    Public Sub New(ByVal prno As String, ByVal prno2 As String, ByVal acno As String, ByVal acno2 As String, ByVal name As String, ByVal addDelDate As String, ByVal addDelDate2 As String, ByVal amt As String, ByVal amt2 As String, ByVal remark As String)
        mPRNO = prno
        mPRNO2 = prno2
        mACNO = acno
        mACNO2 = acno2
        mName = name
        mAddDelDate = addDelDate
        mAddDelDate2 = addDelDate2
        mAMT = amt
        mAMT2 = amt2
        mRemark = remark
    End Sub

    '此建構子適合從資料庫查詢單一財物編號的資料
    Public Sub New(ByVal prNO As String, ByVal prNO2 As String)
        mPRNO = prNO
        mPRNO2 = prNO2
    End Sub

    '此建構子適合從資料庫刪除一筆資料
    Public Sub New(ByVal id As String, ByVal prNO As String, ByVal dummy As Object)
        mPRNO = prNO
        mID = id
    End Sub

    '此建構子適合新增或修改一筆資料到資料庫
    Public Sub New(ByVal id As String, ByVal prNO As String, ByVal addDelDate As String, ByVal amt As String, ByVal remark As String)
        mID = id
        mPRNO = prNO
        mAddDelDate = addDelDate
        mAMT = amt
        mRemark = remark
    End Sub

    Public ReadOnly Property ID() As String
        Get
            Return mID
        End Get
    End Property

    Public ReadOnly Property PRNO() As String
        Get
            Return mPRNO
        End Get
    End Property

    Public ReadOnly Property PRNO2() As String
        Get
            Return mPRNO2
        End Get
    End Property

    Public ReadOnly Property ACNO() As String
        Get
            Return mACNO
        End Get
    End Property

    Public ReadOnly Property ACNO2() As String
        Get
            Return mACNO2
        End Get
    End Property

    Public ReadOnly Property Name() As String
        Get
            Return mName
        End Get
    End Property

    Public ReadOnly Property AddDelDate() As String
        Get
            Return mAddDelDate
        End Get
    End Property

    Public ReadOnly Property AddDelDate2() As String
        Get
            Return mAddDelDate2
        End Get
    End Property

    Public ReadOnly Property AMT() As String
        Get
            Return mAMT
        End Get
    End Property

    Public ReadOnly Property AMT2() As String
        Get
            Return mAMT2
        End Get
    End Property

    Public ReadOnly Property PurchaseDate() As String
        Get
            Return mPurchaseDate
        End Get
    End Property

    Public ReadOnly Property OriginalAmt() As String
        Get
            Return mOriginalAmt
        End Get
    End Property

    Public ReadOnly Property EndAmt() As String
        Get
            Return mEndAmt
        End Get
    End Property

    Public ReadOnly Property TotalAddDel() As String
        Get
            Return mTotalAddDel
        End Get
    End Property

    Public ReadOnly Property NetAmt() As String
        Get
            Return mNetAmt
        End Get
    End Property

    Public ReadOnly Property Depreciation() As String
        Get
            Return mDepreciation
        End Get
    End Property

    Public ReadOnly Property UseYear() As String
        Get
            Return mUseYear
        End Get
    End Property

    Public ReadOnly Property KeepEmpName() As String
        Get
            Return mKeepEmpName
        End Get
    End Property

    Public ReadOnly Property KeepUnitName() As String
        Get
            Return mKeepUnitName
        End Get
    End Property

    Public ReadOnly Property EndDate() As String
        Get
            Return mEndDate
        End Get
    End Property

    Public ReadOnly Property EndRemark() As String
        Get
            Return mEndRemark
        End Get
    End Property

    Public ReadOnly Property Remark() As String
        Get
            Return mRemark
        End Get
    End Property
End Class
