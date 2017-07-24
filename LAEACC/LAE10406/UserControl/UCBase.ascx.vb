Public Class UCBase
    Inherits System.Web.UI.UserControl

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString

#Region "UserControl物件控制"
    ''' <summary>
    ''' Toolbar 控制項的 Click 事件引數。
    ''' </summary>
    Public Class ClickEventArgs
        Inherits System.EventArgs
        Dim FCommandName As String = String.Empty

        ''' <summary>
        ''' 按鈕命令。
        ''' </summary>
        Public Property CommandName() As String
            Get
                Return FCommandName
            End Get
            Set(ByVal value As String)
                FCommandName = value
            End Set
        End Property
    End Class

    ''' <summary>
    ''' 按下工具列按鈕引發的事件。
    ''' </summary>
    Public Event Click(ByVal sender As Object, ByVal e As ClickEventArgs)

    ''' <summary>
    ''' 引發 Click 事件。
    ''' </summary>
    Public Overloads Sub RaiseClickEvent(ByVal e As ClickEventArgs)
        RaiseEvent Click(Me, e)
    End Sub

    ''' <summary>
    ''' 所有按鈕的事件處理函式[需俢改相關命名]
    ''' </summary>
    Protected Sub btnton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        btnFirst.Click, btnPrior.Click, btnNext.Click, btnLast.Click, _
        btnSave.Click, btnCancelEdit.Click, btnCopy.Click, btnAddNew.Click, btnEdit.Click, btnDelete.Click, _
        btnPrint.Click
        Dim oEventArgs As New ClickEventArgs()

        '取得作用按鈕的 CommandName
        oEventArgs.CommandName = CType(sender, Button).CommandName

        '引發 Click 事件
        RaiseClickEvent(oEventArgs)
    End Sub
#End Region
#Region "類別模組"
    '資料庫
    Dim ADO As New ADO
    '系統
    Dim Controller As New Controller
    Dim Models As New Models
    '自訂
    Dim ACC As New ACC
#End Region
#Region "頁面變數"
    '底層變數*****
    Dim FStatus As Byte '資料狀態
    Dim ProgramID As String '取得目前作業的程式名稱

    Dim FAllowAddNew As Boolean = True '允許更新
    Dim FAllowEdit As Boolean = True '允許修改
    Dim FAllowDelete As Boolean = False '允許刪除

    Dim FileName As String = ""
#End Region

#Region "Page及功能操作"
#Region "Page"
    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        '檔案名稱
        FileName = System.IO.Path.GetFileName(Request.PhysicalPath)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            MyTableStatus = 0 '預設狀態為瀏覽模式
            SetButtons() '所有按鈕的狀態
            SetPageButtons(FileName) '該功能之控制按鍵
            SetControls(0) '設定所有輸入控制項的唯讀狀態
        End If
    End Sub
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

    End Sub
#End Region

#Region "Navigation"
    '第一筆
    Protected Sub btnFirst_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFirst.Click
        SetButtons()

        '按下btnFirst按鈕
        If BeforeScroll() Then
            '找出指定資料表的第一筆記錄
            BeforeLoad()
            FlagMoveSeat(1)
            AfterLoad()

            MyTableStatus = 0
            AfterScroll()
        End If
    End Sub

    '上一筆
    Protected Sub btnPrior_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrior.Click
        SetButtons()

        '按下btnPrior按鈕
        If BeforeScroll() Then
            '找出指定資料表的上一筆記錄
            BeforeLoad()
            FlagMoveSeat(2)
            AfterLoad()

            MyTableStatus = 0
            AfterScroll()
        End If
    End Sub

    '下一筆
    Protected Sub btnNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNext.Click
        SetButtons()

        '按下btnNext按鈕
        If BeforeScroll() Then
            '找出指定資料表的第一筆記錄
            BeforeLoad()
            FlagMoveSeat(3)
            AfterLoad()

            MyTableStatus = 0
            AfterScroll()
        End If
    End Sub

    '最末筆
    Protected Sub btnLast_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLast.Click
        SetButtons()

        '按下btnLast按鈕
        If BeforeScroll() Then
            '找出指定資料表的第一筆記錄
            BeforeLoad()
            FlagMoveSeat(4)
            AfterLoad()

            MyTableStatus = 0
            AfterScroll()
        End If
    End Sub
#End Region

#Region "Modify"
    '存檔
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        '按下btnSave按鈕
        If BeforeEndEdit() Then
            Dim PrevTableStatus As String = MyTableStatus
            Dim SaveStatus As Boolean = True

            Try
                '判斷程序為新增或修改
                If PrevTableStatus = "1" Then SaveStatus = InsertData() '新增
                If PrevTableStatus = "2" Then SaveStatus = UpdateData() '修改

                If SaveStatus Then
                    MyTableStatus = 0
                    SetControls(0)
                    FillData()
                    AfterEndEdit()
                End If
            Catch ex As Exception
                MyTableStatus = PrevTableStatus
            End Try
        End If
    End Sub

    '取消
    Protected Sub btnCancelEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelEdit.Click
        '按下btnCancelEdit按鈕
        If BeforeCancelEdit() Then
            FillData()

            MyTableStatus = 0
            SetControls(0)
            AfterCancelEdit()
        End If

        SetButtons()
    End Sub

    '複製
    Protected Sub btnCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopy.Click
        MyTableStatus = 1
        SetControls(1)
        AfterAddNew()

        SetButtons()
    End Sub

    '新增
    Public Sub btnAddNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNew.Click
        '按下btnAddNew按鈕
        If BeforeAddNew() Then
            MyTableStatus = 1
            SetControls(1)
            AfterAddNew()
        End If

        SetButtons()
    End Sub

    '修改
    Public Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        '按下btnEdit按鈕
        If BeforeEdit() Then
            MyTableStatus = 2
            SetControls(2)
            AfterEdit()
        End If

        SetButtons()
    End Sub

    '刪除
    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        '刪除指定資料表的記錄
        DeleteData()

        BeforeLoad()
        FillData()
        AfterLoad()

        MyTableStatus = 0
        SetControls(0)
        AfterDelete()

        SetButtons()
    End Sub
#End Region
#End Region

#Region "共用底層副程式"
#Region "設定狀態及屬性"
    '設定所有按鍵狀態
    Sub SetButtons()
        'FStatus:
        ' 0:瀏覽模式
        ' 1:新增模式
        ' 2:修改模式        
        btnFirst.Enabled = (FStatus = 0)
        btnPrior.Enabled = (FStatus = 0)
        btnNext.Enabled = (FStatus = 0)
        btnLast.Enabled = (FStatus = 0)

        btnSave.Enabled = ((FStatus = 1) Or (FStatus = 2))
        btnCancelEdit.Enabled = ((FStatus = 1) Or (FStatus = 2))
        btnCopy.Enabled = (FStatus = 0) And (AllowAddNew)
        btnAddNew.Enabled = (FStatus = 0) And (AllowAddNew)
        btnEdit.Enabled = (FStatus = 0) And (AllowEdit)
        btnDelete.Enabled = (FStatus = 0)

        btnLast.Enabled = (FStatus = 0)
    End Sub
    Sub SetButtons_Visible(Optional vis As String = "")
        btnCopy.Visible = False
        btnPrint.Visible = False
        'btnEdit.Visible = False
    End Sub
    '設定該功能之控制按鍵
    Sub SetPageButtons(ByVal PageID As String, Optional PageID2 As String = "")

    End Sub

    '取得或設定資料表狀態的屬性
    Public Property MyTableStatus() As Byte
        Get
            MyTableStatus = FStatus
        End Get

        Set(ByVal value As Byte)
            FStatus = value
        End Set
    End Property

    '取得或設定允許新增記錄的屬性
    Public Property AllowAddNew() As Boolean
        Get
            AllowAddNew = FAllowAddNew
        End Get

        Set(ByVal value As Boolean)
            FAllowAddNew = value
            SetButtons()
        End Set
    End Property

    '取得或設定允許修改記錄的屬性
    Public Property AllowEdit() As Boolean
        Get
            AllowEdit = FAllowEdit
        End Get

        Set(ByVal value As Boolean)
            FAllowEdit = value
            SetButtons()
        End Set
    End Property

    '取得或設定允許刪除記錄的屬性
    Public Property AllowDelete() As Boolean
        Get
            AllowDelete = FAllowDelete
        End Get

        Set(ByVal value As Boolean)
            FAllowDelete = value
            SetButtons()
        End Set
    End Property
#End Region

#Region "建立虛擬方法"
    '執行[FillData()]之前
    Public Overridable Sub BeforeLoad()
        '虛擬方法，供繼承者實作
        '載入資料之前(執行FillData方法之前)
    End Sub
    '執行[FillData()]之後
    Public Overridable Sub AfterLoad()
        '虛擬方法，供繼承者實作
        '載入資料之後(執行FillData方法之後)
    End Sub

    '執行[移動記錄]之前
    Public Overridable Function BeforeScroll() As Boolean
        '虛擬方法，供繼承者實作，預設值為True
        '移動記錄之前
        Return True
    End Function
    '執行[移動記錄]之後
    Public Overridable Sub AfterScroll()
        '虛擬方法，供繼承者實作
        '移動記錄之後
    End Sub

    '在使用者按下[還原]之前
    Public Overridable Function BeforeCancelEdit() As Boolean
        '虛擬方法，供繼承者實作，預設值為True
        '執行MyBS物件的CancelEdit方法之前
        Return True
    End Function
    '在使用者按下[還原]之後
    Public Overridable Sub AfterCancelEdit()
        '虛擬方法，供繼承者實作
        '執行MyBS物件的CancelEdit()方法之後
    End Sub

    '在使用者按下[新增]之前
    Public Overridable Function BeforeAddNew() As Boolean
        '虛擬方法，供繼承者實作，預設值為True
        '按btnAddNew按鈕之前
        Return True
    End Function
    '在使用者按下[新增]之後
    Public Overridable Sub AfterAddNew()
        '虛擬方法，供繼承者實作
        '按btnAddNew按鈕之後
    End Sub

    '在使用者按下[修改]之前
    Public Overridable Function BeforeEdit() As Boolean
        '虛擬方法，供繼承者實作, 預設值為True
        '按btnEdit按鈕之前
        Return True
    End Function
    '在使用者按下[修改]之後
    Public Overridable Sub AfterEdit()
        '虛擬方法，供繼承者實作
        '按了btnEdit按鈕之後
    End Sub

    '儲存資料之後，執行[EndEdit()]之前
    Public Overridable Function BeforeEndEdit() As Boolean
        '虛擬方法，供繼承者實作，預設值為True
        '儲存資料之後(執行EndEdit方法之前)
        Return True
    End Function
    '儲存資料之後，執行[EndEdit()]之後
    Public Overridable Sub AfterEndEdit()
        '虛擬方法，供繼承者實作
        '儲存資料之後(執行EndEdit方法之後)
    End Sub

    '執行[DeleteData()]之前
    Public Overridable Function BeforeDelete() As Boolean
        '虛擬方法，供繼承者實作，預設值為True
        '刪除記錄前要處理的事情
        Return True
    End Function
    '執行[DeleteData()]之後
    Public Overridable Sub AfterDelete()
        '虛擬方法，供繼承者實作
        '刪除記錄後要處理的事情
    End Sub
#End Region

#Region "系統資料控制處理"
    '系統載入資料
    Public Overridable Sub FillData()
        '虛擬方法，供繼承者實作
        '載入資料
    End Sub

    '系統新增資料
    Public Overridable Function InsertData() As Boolean
        '虛擬方法，供繼承者實作，預設值為True
        '用來執行新增記錄的程式碼
        Return True
    End Function

    '系統更新資料
    Public Overridable Function UpdateData() As Boolean
        '虛擬方法，供繼承者實作，預設值為True
        '用來執行更新記錄的程式碼
        Return True
    End Function

    '系統刪除資料
    Public Overridable Sub DeleteData()
        '虛擬方法，供繼承者實作，預設值為True
        '用來執行刪除記錄的程式碼
    End Sub

    '輸入控制項的 ReadOnly 屬性及 Enabled 屬性
    Public Overridable Sub SetControls(ByVal btnStatus As Integer)
        'btnStatus:
        ' 0:一般模式
        ' 1:新增模式
        ' 2:修改模式

        '虛擬方法，供繼承者實作
        '設定所有輸入物件的ReadOnly或Enabled屬性
    End Sub

    '移動DataGridView指標
    Public Overridable Sub FlagMoveSeat(ByVal strStatus As Integer)
        'strStatus:
        ' 0:隨機點選
        ' 1:第一筆
        ' 2:上一筆
        ' 3:下一筆
        ' 4:最未筆
    End Sub
#End Region
#End Region
End Class