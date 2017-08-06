
'財物主檔
<Serializable()> Public Class PGMainInfo

    Private mPRNO As String
    Private mPRNO2 As String
    Private mName As String
    Private mACNO As String
    Private mACNO2 As String
    Private mACName As String
    Private mPDate As String
    Private mPDate2 As String
    Private mUnit As String
    Private mQTY As String
    Private mKeepEmpNO As String
    Private mKeepEmpNO2 As String
    Private mKeepEmpName As String
    Private mKeepUnit As String
    Private mKeepUnit2 As String
    Private mKeepUnitName As String
    Private mUseYear As String
    Private mUseYear2 As String
    Private mAMT As String       '原值
    Private mAMT2 As String
    Private mEndAMT As String    '預估殘值
    Private mTotalAddDel As String '增減值的總和
    Private mNetAmt As Decimal    '淨值= 原值 + 增減值的總和 - 已提折舊值
    Private mDepreciation As String       '已提折舊值
    Private mDepreciationRatio As Decimal   '折舊率 = 已提折舊值 / (原值+增減值-殘值)
    Private mBGACNO As String
    Private mBGACNO2 As String
    Private mBGName As String
    Private mComeFrom As String
    Private mModel As String
    Private mEndDate As String
    Private mEndDate2 As String
    Private mEndRemark As String
    Private mBorrow As String
    Private mMaterial As String
    Private mUses As String
    Private mMadeDate As String
    Private mRemark As String
    Private mPlace As String
    Private mSpecRemark As String
    Private mRevisedDate As String
    Private mRevisedDate2 As String
    Private mRevisedEmpNO As String
    Private mRevisedEmpName As String
    Private mQueryDiscardMode As Integer = 0 '0-報廢或沒報廢都查詢,1-只查詢沒有報廢者,2-只查詢報廢者


    '此建構子適合從資料庫讀取資料送到client
    Public Sub New(ByVal prNO As String, ByVal name As String, ByVal acNO As String, ByVal acName As String, ByVal pDate As String, ByVal unit As String, _
    ByVal qty As String, ByVal keepEmpNO As String, ByVal keepEmpName As String, ByVal keepUnit As String, ByVal keepUnitName As String, ByVal useYear As String, _
    ByVal amt As String, ByVal endAMT As String, ByVal totalAddDel As String, ByVal depreciation As String, ByVal bgACNO As String, ByVal bgName As String, ByVal comeFrom As String, ByVal model As String, _
    ByVal endDate As String, ByVal endRemark As String, ByVal borrow As String, ByVal material As String, ByVal uses As String, ByVal madeDate As String, _
    ByVal remark As String, ByVal place As String, ByVal specRemark As String, ByVal revisedDate As String, ByVal revisedEmpNO As String, ByVal revisedEmpName As String)

        mPRNO = prNO
        mName = name
        mACNO = acNO
        mACName = acName
        mPDate = pDate
        mUnit = unit
        mQTY = qty
        mKeepEmpNO = keepEmpNO
        mKeepEmpName = keepEmpName
        mKeepUnit = keepUnit
        mKeepUnitName = keepUnitName
        mUseYear = useYear
        mAMT = amt
        mEndAMT = endAMT
        mTotalAddDel = totalAddDel
        mDepreciation = depreciation
        mBGACNO = bgACNO
        mBGName = bgName
        mComeFrom = comeFrom
        mModel = model
        mEndDate = endDate
        mEndRemark = endRemark
        mBorrow = borrow
        mMaterial = material
        mUses = uses
        mMadeDate = madeDate
        mRemark = remark
        mPlace = place
        mSpecRemark = specRemark
        mRevisedDate = revisedDate
        mRevisedEmpNO = revisedEmpNO
        mRevisedEmpName = revisedEmpName
    End Sub

    '此建構子適合新增或修改一筆資料到資料庫
    Public Sub New(ByVal prNO As String, ByVal prNO2 As String, ByVal name As String, ByVal acNO As String, ByVal pDate As String, ByVal unit As String, _
    ByVal qty As String, ByVal keepEmpNO As String, ByVal keepUnit As String, ByVal useYear As String, _
    ByVal amt As String, ByVal endAMT As String, ByVal bgACNO As String, ByVal bgName As String, ByVal comeFrom As String, ByVal model As String, _
    ByVal endDate As String, ByVal endRemark As String, ByVal borrow As String, ByVal material As String, ByVal uses As String, ByVal madeDate As String, _
    ByVal remark As String, ByVal place As String, ByVal specRemark As String, ByVal revisedEmpNO As String)

        mPRNO = prNO
        mPRNO2 = prNO2
        mName = name
        mACNO = acNO
        mPDate = pDate
        mUnit = unit
        mQTY = qty
        mKeepEmpNO = keepEmpNO
        mKeepUnit = keepUnit
        mUseYear = useYear
        mAMT = amt
        mEndAMT = endAMT
        mBGACNO = bgACNO
        mBGName = bgName
        mComeFrom = comeFrom
        mModel = model
        mEndDate = endDate
        mEndRemark = endRemark
        mBorrow = borrow
        mMaterial = material
        mUses = uses
        mMadeDate = madeDate
        mRemark = remark
        mPlace = place
        mSpecRemark = specRemark
        mRevisedEmpNO = revisedEmpNO

    End Sub

    '此建構子適合下查詢條件到資料庫
    Public Sub New(ByVal prNO As String, ByVal prNO2 As String, ByVal name As String, ByVal acNO As String, ByVal acNO2 As String, ByVal pDate As String, ByVal pDate2 As String, _
    ByVal keepEmpNO As String, ByVal keepEmpNO2 As String, ByVal keepUnit As String, ByVal keepUnit2 As String, ByVal useYear As String, ByVal useYear2 As String, _
    ByVal amt As String, ByVal amt2 As String, ByVal bgACNO As String, ByVal bgACNO2 As String, ByVal comeFrom As String, ByVal model As String, _
    ByVal endDate As String, ByVal endDate2 As String, ByVal uses As String, _
      ByVal specRemark As String, ByVal revisedDate As String, ByVal revisedDate2 As String, ByVal queryDiscardMode As String, ByVal dummy As String)

        mPRNO = prNO
        mPRNO2 = prNO2
        mName = name
        mACNO = acNO
        mACNO2 = acNO2
        mPDate = pDate
        mPDate2 = pDate2
        mKeepEmpNO = keepEmpNO
        mKeepEmpNO2 = keepEmpNO2
        mKeepUnit = keepUnit
        mKeepUnit2 = keepUnit2
        mUseYear = useYear
        mUseYear2 = useYear2
        mAMT = amt
        mAMT2 = amt2
        mBGACNO = bgACNO
        mBGACNO2 = bgACNO2
        mComeFrom = comeFrom
        mModel = model
        mEndDate = endDate
        mEndDate2 = endDate2
        mUses = uses
        mSpecRemark = specRemark
        mRevisedDate = revisedDate
        mRevisedDate2 = revisedDate2
        mQueryDiscardMode = queryDiscardMode
    End Sub

    '此建構子適合下列印條件到資料庫
    Public Sub New(ByVal prNO As String, ByVal prNO2 As String, ByVal acNO As String, ByVal acNO2 As String, ByVal pDate As String, ByVal pDate2 As String, _
    ByVal keepEmpNO As String, ByVal keepEmpNO2 As String, ByVal keepUnit As String, ByVal keepUnit2 As String)
        mPRNO = prNO
        mPRNO2 = prNO2
        mACNO = acNO
        mACNO2 = acNO2
        mPDate = pDate
        mPDate2 = pDate2
        mKeepEmpNO = keepEmpNO
        mKeepEmpNO2 = keepEmpNO2
        mKeepUnit = keepUnit
        mKeepUnit2 = keepUnit2
    End Sub

    '此建構子適合下列印條件到資料庫 (列印財產減損單時)
    Public Sub New(ByVal prNO As String, ByVal prNO2 As String, ByVal acNO As String, ByVal acNO2 As String, ByVal pDate As String, ByVal pDate2 As String, _
    ByVal keepEmpNO As String, ByVal keepEmpNO2 As String, ByVal keepUnit As String, ByVal keepUnit2 As String, ByVal endDate As String, ByVal endDate2 As String)
        mPRNO = prNO
        mPRNO2 = prNO2
        mACNO = acNO
        mACNO2 = acNO2
        mPDate = pDate
        mPDate2 = pDate2
        mKeepEmpNO = keepEmpNO
        mKeepEmpNO2 = keepEmpNO2
        mKeepUnit = keepUnit
        mKeepUnit2 = keepUnit2
        mEndDate = endDate
        mEndDate2 = endDate2
        mQueryDiscardMode = 2 '只查詢報廢者
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

    '此建構子適合從資料庫查詢指定日期的報廢清單
    Public Sub New(ByVal endDate As String, ByVal endDate2 As String, ByVal dummy As DateTime)
        mEndDate = endDate
        mEndDate2 = endDate2
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

    Public ReadOnly Property Name() As String
        Get
            Return mName
        End Get
    End Property

    Public ReadOnly Property ACName() As String
        Get
            Return mACName
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

    Public ReadOnly Property PDate() As String
        Get
            Return mPDate
        End Get
    End Property

    Public ReadOnly Property PDate2() As String
        Get
            Return mPDate2
        End Get
    End Property

    Public ReadOnly Property Unit() As String
        Get
            Return mUnit
        End Get
    End Property

    Public ReadOnly Property QTY() As String
        Get
            Return mQTY
        End Get
    End Property

    Public ReadOnly Property KeepEmpNO() As String
        Get
            Return mKeepEmpNO
        End Get
    End Property

    Public ReadOnly Property KeepEmpNO2() As String
        Get
            Return mKeepEmpNO2
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

    Public ReadOnly Property KeepUnit2() As String
        Get
            Return mKeepUnit2
        End Get
    End Property

    Public ReadOnly Property KeepUnitName() As String
        Get
            Return mKeepUnitName
        End Get
    End Property

    Public ReadOnly Property UseYear() As String
        Get
            Return mUseYear
        End Get
    End Property

    Public ReadOnly Property UseYear2() As String
        Get
            Return mUseYear2
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

    Public ReadOnly Property BGACNO() As String
        Get
            Return mBGACNO
        End Get
    End Property

    Public ReadOnly Property BGACNO2() As String
        Get
            Return mBGACNO2
        End Get
    End Property

    Public ReadOnly Property BGName() As String
        Get
            Return mBGName
        End Get
    End Property

    Public ReadOnly Property ComeFrom() As String
        Get
            Return mComeFrom
        End Get
    End Property

    Public ReadOnly Property Model() As String
        Get
            Return mModel
        End Get
    End Property

    Public ReadOnly Property EndDate() As String
        Get
            Return mEndDate
        End Get
    End Property

    Public ReadOnly Property EndDate2() As String
        Get
            Return mEndDate2
        End Get
    End Property

    Public ReadOnly Property EndRemark() As String
        Get
            Return mEndRemark
        End Get
    End Property

    Public ReadOnly Property Borrow() As String
        Get
            Return mBorrow
        End Get
    End Property

    Public ReadOnly Property Material() As String
        Get
            Return mMaterial
        End Get
    End Property

    Public ReadOnly Property Uses() As String
        Get
            Return mUses
        End Get
    End Property

    Public ReadOnly Property MadeDate() As String
        Get
            Return mMadeDate
        End Get
    End Property

    Public ReadOnly Property Remark() As String
        Get
            Return mRemark
        End Get
    End Property

    Public ReadOnly Property Place() As String
        Get
            Return mPlace
        End Get
    End Property

    Public ReadOnly Property SpecRemark() As String
        Get
            Return mSpecRemark
        End Get
    End Property

    Public ReadOnly Property RevisedDate() As String
        Get
            Return mRevisedDate
        End Get
    End Property

    Public ReadOnly Property RevisedDate2() As String
        Get
            Return mRevisedDate2
        End Get
    End Property

    Public ReadOnly Property RevisedEmpNO() As String
        Get
            Return mRevisedEmpNO
        End Get
    End Property

    Public ReadOnly Property RevisedEmpName() As String
        Get
            Return mRevisedEmpName
        End Get
    End Property

    Public ReadOnly Property QueryDiscardMode() As Integer
        Get
            Return mQueryDiscardMode
        End Get
    End Property

End Class