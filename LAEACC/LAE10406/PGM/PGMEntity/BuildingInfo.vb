'建物主檔
<Serializable()> Public Class BuildingInfo

    Private mPRNO As String
    Private mPRNO2 As String
    Private mACNO As String
    Private mACNO2 As String
    Private mPDate As String
    Private mBuilNo As String
    Private mLandNo1 As String
    Private mLandNo2 As String
    Private mLandArea As String
    Private mPart As String
    Private mArea1 As String
    Private mArea2 As String
    Private mArea3 As String
    Private mUseYear As String
    Private mAMT As String
    Private mEndAMT As String
    Private mTotalAddDel As String '增減值的總和
    Private mNetAmt As Decimal    '淨值= 原值 + 增減值的總和 - 已提折舊值
    Private mDepreciation As String       '已提折舊值
    Private mDepreciationRatio As Decimal   '折舊率 = 已提折舊值 / (原值+增減值-殘值)
    Private mStru As String
    Private mTax As String
    Private mAddr As String
    Private mKind As String
    Private mBorrow As String
    Private mRemark As String
    Private mUseMode As String
    Private mEndDate As String


    '此建構子適合新增或修改一筆資料到資料庫
    Public Sub New(ByVal prNO As String, ByVal acNo As String, ByVal pDate As String, ByVal builNo As String, ByVal landNo1 As String _
    , ByVal landNo2 As String, ByVal landArea As String, ByVal part As String, ByVal area1 As String, ByVal area2 As String, ByVal area3 As String _
    , ByVal useYear As String, ByVal amt As String, ByVal endDate As String, ByVal endAmt As String, ByVal stru As String, ByVal tax As String, ByVal addr As String _
    , ByVal kind As String, ByVal usemode As String, ByVal borrow As String, ByVal remark As String)
        mPRNO = prNO
        mACNO = acNo
        mPDate = pDate
        mBuilNo = builNo
        mLandNo1 = landNo1
        mLandNo2 = landNo2
        mLandArea = landArea
        mPart = part
        mArea1 = area1
        mArea2 = area2
        mArea3 = area3
        mUseYear = useYear
        mAMT = amt
        mEndAMT = endAmt
        mStru = stru
        mTax = tax
        mAddr = addr
        mKind = kind
        mBorrow = borrow
        mRemark = remark
        mUseMode = usemode
        mEndDate = endDate
    End Sub

    '此建構子適合下查詢條件到資料庫
    Public Sub New(ByVal prNO As String, ByVal prNO2 As String, ByVal kind As String, ByVal acno As String, ByVal acno2 As String)
        mPRNO = prNO
        mPRNO2 = prNO2
        mKind = kind
        mACNO = acno
        mACNO2 = acno2
    End Sub


    '此建構子適合從資料庫刪除一筆資料
    Public Sub New(ByVal prNO As String)
        mPRNO = prNO
    End Sub

    '此建構子適合從資料庫查詢唯一資料
    Public Sub New(ByVal prNO As String, ByVal prNO2 As String)
        mPRNO = prNO
        mPRNO2 = prNO2
    End Sub

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

    Public ReadOnly Property BuilNo() As String
        Get
            Return mBuilNo
        End Get
    End Property

    Public ReadOnly Property LandNo1() As String
        Get
            Return mLandNo1
        End Get
    End Property

    Public ReadOnly Property LandNo2() As String
        Get
            Return mLandNo2
        End Get
    End Property

    Public ReadOnly Property LandArea() As String
        Get
            Return mLandArea
        End Get
    End Property

    Public ReadOnly Property PDate() As String
        Get
            Return mPDate
        End Get
    End Property


    Public ReadOnly Property Part() As String
        Get
            Return mPart
        End Get
    End Property

    Public ReadOnly Property Area1() As String
        Get
            Return mArea1
        End Get
    End Property

    Public ReadOnly Property Area2() As String
        Get
            Return mArea2
        End Get
    End Property

    Public ReadOnly Property Area3() As String
        Get
            Return mArea3
        End Get
    End Property

    Public ReadOnly Property Stru() As String
        Get
            Return mStru
        End Get
    End Property

    Public ReadOnly Property Tax() As String
        Get
            Return mTax
        End Get
    End Property

    Public ReadOnly Property Addr() As String
        Get
            Return mAddr
        End Get
    End Property

    Public ReadOnly Property UseYear() As String
        Get
            Return mUseYear
        End Get
    End Property

    Public ReadOnly Property AMT() As String
        Get
            Return mAMT
        End Get
    End Property

    Public ReadOnly Property EndAMT() As String
        Get
            Return mEndAMT
        End Get
    End Property

    Public ReadOnly Property TotalAddDel() As String
        Get
            Return mTotalAddDel
        End Get
    End Property

    Public ReadOnly Property Depreciation() As String
        Get
            Return mDepreciation
        End Get
    End Property

    Public ReadOnly Property NetAmt() As String
        Get
            Dim a, b, c As Decimal
            If IsNumeric(mAMT) Then a = CDec(mAMT)
            If IsNumeric(mTotalAddDel) Then b = CDec(mTotalAddDel)
            If IsNumeric(mDepreciation) Then c = CDec(mDepreciation)
            mNetAmt = a + b - c
            Return Format(mNetAmt, "0.00")
        End Get
    End Property

    Public ReadOnly Property DepreciationRatio() As String
        Get
            Dim a, b, c, d As Decimal
            If IsNumeric(mAMT) Then a = CDec(mAMT)
            If IsNumeric(mTotalAddDel) Then b = CDec(mTotalAddDel)
            If IsNumeric(mDepreciation) Then c = CDec(mDepreciation)
            If IsNumeric(mEndAMT) Then d = CDec(mEndAMT)
            mDepreciationRatio = c / (a + b - d)
            Return Format(mDepreciationRatio, "0.00%")
        End Get
    End Property

    Public ReadOnly Property Kind() As String
        Get
            Return mKind
        End Get
    End Property

    Public ReadOnly Property EndDate() As String
        Get
            Return mEndDate
        End Get
    End Property

    Public ReadOnly Property Borrow() As String
        Get
            Return mBorrow
        End Get
    End Property

    Public ReadOnly Property UseMode() As String
        Get
            Return mUseMode
        End Get
    End Property

    Public ReadOnly Property AcNo() As String
        Get
            Return mACNO
        End Get
    End Property

    Public ReadOnly Property AcNo2() As String
        Get
            Return mACNO2
        End Get
    End Property

    Public ReadOnly Property Remark() As String
        Get
            Return mRemark
        End Get
    End Property

End Class