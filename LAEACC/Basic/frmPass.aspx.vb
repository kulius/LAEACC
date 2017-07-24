Public Class frmPass
    Inherits System.Web.UI.Page

#Region "共用變數"
    Dim I As Integer '累進變數
    Dim strMessage As String = "" '訊息字串
    Dim strIRow, strIValue, strUValue, strWValue As String '資料處理參數(新增欄位；新增資料；異動資料；條件)
#End Region

#Region "@Page及功能操作@"
    Private Sub frmPass_Init(sender As Object, e As EventArgs) Handles Me.Init
        '++ 物件初始化 ++
        'Focus*****
        O_PASS_WORD.Focus()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    '存檔
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        '防呆
        'TextBox
        Dim objTextBox() As TextBox = {O_PASS_WORD, N_PASS_WORD, C_PASS_WORD}
        Dim strTextBox As String = "舊密碼,新密碼,確認密碼,"
        strMessage = Me.Master.Controller.TextBox_Input(objTextBox, 0, strTextBox)
        If strMessage <> "" Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('【" & strMessage & "】未輸入!!');", True) : Exit Sub

        '其他
        If N_PASS_WORD.Text <> C_PASS_WORD.Text Then ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('新密碼與確認密碼輸入不相同!!');", True) : Exit Sub


        '開啟儲存*****
        'users[帳號記錄表]
        strUValue = "password = '" & C_PASS_WORD.Text & "',"
        strUValue &= "update_date = '" & Me.Master.Models.NowDate & "'"

        strWValue = "user_id = '" & Session("POWERUSERID") & "'"

        Me.Master.ADO.dbEdit(Me.Master.DNS_SYS, "users", strUValue, strWValue)


        '異動後初始化*****
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "訊息", "alert('操作個人化修改資料成功');", True) '系統訊息        
    End Sub
#End Region
End Class