':::::::::::::::::::
'::: 系統物件控制 ::
'::: 系統加解密 ::::
'::: 檔案處理   ::::
':::::::::::::::::::
Imports System.IO
Imports System.Security.Cryptography
Imports System.Data
Imports System.Data.SqlClient

Public Class Controller
    Inherits System.Web.UI.Page

#Region "資料庫共用變數"
    Dim objDASC As SqlDataAdapter
    Dim objDSSC As DataSet

    '資料庫共用變數
    Dim objConSC As SqlConnection
    Dim objCmdSC As SqlCommand
    Dim objDRSC As SqlDataReader
    Dim strSQLSC As String

    '資料庫連線字串
    Dim DNS_SYS As String = ConfigurationManager.ConnectionStrings("DNS_SYS").ConnectionString
    Dim DNS_ACC As String = ConfigurationManager.ConnectionStrings("DNS_ACC").ConnectionString

    Public ADO As New ADO
#End Region
#Region "固定公用變數"
    Dim I As Integer '累進變數
#End Region

#Region "Models副程式"

#End Region

#Region "@控制項@"
#Region "內容動態產生及驗證"
    'DropDownList
    ''' <summary>
    ''' DropDownList自動產生項目選項[資料庫-直接執行查詢]
    ''' </summary>
    ''' <param name="objDropDownListID">指定產生DropDownList選項的ID</param>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strSQL">SQL語法</param>
    ''' <param name="strRowID">指定代碼欄位</param>
    ''' <param name="strRowName">指定中文欄位</param>
    ''' <param name="intAppoint">指定預設選項位置(可省略)</param>
    Sub objDropDownListOptionEX(ByVal objDropDownListID As DropDownList, ByVal strDNS As String, ByVal strSQL As String, ByVal strRowID As String, ByVal strRowName As String, Optional ByVal intAppoint As Integer = 0)
        Try
            objDropDownListID.Items.Clear()


            '開啟產生
            objConSC = New SqlConnection(strDNS)
            objConSC.Open()

            strSQLSC = strSQL

            objCmdSC = New SqlCommand(strSQLSC, objConSC)
            objDRSC = objCmdSC.ExecuteReader

            I = 0
            objDropDownListID.Items.Insert(I, New ListItem) : objDropDownListID.Items.Item(I).Value = "" : objDropDownListID.Items.Item(I).Text = "--請選擇--" : I = 1
            Do While objDRSC.Read
                With objDropDownListID
                    .Items.Insert(I, New ListItem)
                    .Items.Item(I).Value = objDRSC(strRowID).ToString
                    .Items.Item(I).Text = objDRSC(strRowName).ToString
                End With

                I += 1
            Loop

            objDRSC.Close()    '關閉連結
            objConSC.Close()
            objCmdSC.Dispose() '手動釋放資源
            objConSC.Dispose()
            objConSC = Nothing '移除指標


            '--- 物件初始化 ---
            objDropDownListID.Items.Item(intAppoint).Selected = True '指定預設選項
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' DropDownList自動產生項目選項[資料庫]
    ''' </summary>
    ''' <param name="objDropDownListID">指定產生DropDownList選項的ID</param>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTableName">查詢資料表</param>
    ''' <param name="strRowID">代碼欄位</param>
    ''' <param name="strRowName">中文欄位(例：AAA,BBB,CCC)</param>
    ''' <param name="strWhere">查詢條件(可省略)</param>
    ''' <param name="strOrderBy">排序(可省略)</param>
    ''' <param name="intAppoint">指定預設選項位置(可省略)</param>
    Sub objDropDownListOptionDB(ByVal objDropDownListID As DropDownList, ByVal strDNS As String, ByVal strTableName As String, ByVal strRowID As String, ByVal strRowName As String, Optional ByVal strWhere As String = "", Optional ByVal strOrderBy As String = "", Optional ByVal intAppoint As Integer = 0)
        Try
            objDropDownListID.Items.Clear()


            '開啟產生
            objConSC = New SqlConnection(strDNS)
            objConSC.Open()

            strSQLSC = "SELECT " & strRowID & " AS strID, " & strRowName & " AS strName FROM " & strTableName
            If strWhere <> "" Then strSQLSC &= " WHERE " & strWhere
            If strOrderBy <> "" Then strSQLSC &= " ORDER BY " & strOrderBy

            objCmdSC = New SqlCommand(strSQLSC, objConSC)
            objDRSC = objCmdSC.ExecuteReader

            I = 0
            objDropDownListID.Items.Insert(I, New ListItem) : objDropDownListID.Items.Item(I).Value = "" : objDropDownListID.Items.Item(I).Text = "--請選擇--" : I = 1
            Do While objDRSC.Read
                '選項值
                With objDropDownListID
                    .Items.Insert(I, New ListItem)
                    .Items.Item(I).Value = objDRSC("strID")
                End With


                '選項文字
                Dim strOptionText As String = ""
                Dim strIArray() As String = strRowName.Split(",") '將字串存入陣列

                For J As Integer = 0 To strIArray.Length - 1 Step 1
                    Try
                        strOptionText &= " " & objDRSC(strIArray(J)) & "(" & objDRSC("strID") & ")"
                    Catch ex As Exception
                        strOptionText &= " " & objDRSC("strName") & "(" & objDRSC("strID") & ")"
                    End Try
                Next

                objDropDownListID.Items.Item(I).Text = strOptionText


                I += 1
            Loop

            objDRSC.Close()    '關閉連結
            objConSC.Close()
            objCmdSC.Dispose() '手動釋放資源
            objConSC.Dispose()
            objConSC = Nothing '移除指標


            '--- 物件初始化 ---
            objDropDownListID.Items.Item(intAppoint).Selected = True '指定預設選項
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' DropDownList自動產生項目選項[自訂]
    ''' </summary>
    ''' <param name="objDropDownListID">指定產生DropDownList選項的ID</param>
    ''' <param name="strItemsArray">自訂選項，例：AAA,BBB,</param>
    ''' <param name="strValueArray">自訂選項值，例：123,456,</param>
    ''' <param name="intAppoint">指定預設選項位置(可省略)</param>
    Sub objDropDownListOptionGG(ByVal objDropDownListID As DropDownList, ByVal strItemsArray As String, ByVal strValueArray As String, Optional ByVal intAppoint As Integer = 0)
        Try
            objDropDownListID.Items.Clear()


            Dim strVArray() As String = strValueArray.Split(",")
            Dim strIArray() As String = strItemsArray.Split(",")

            For I As Integer = 0 To strVArray.Length - 2 Step 1
                With objDropDownListID
                    .Items.Insert(I, New ListItem)
                    .Items.Item(I).Value = strVArray(I)
                    .Items.Item(I).Text = strIArray(I)
                End With
            Next


            '--- 物件初始化 ---
            objDropDownListID.Items.Item(intAppoint).Selected = True '指定預設選項
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' DropDownList項目選項驗證
    ''' </summary>
    ''' <param name="objDropDownList">指定產生DropDownList選項的ID</param>
    ''' <param name="strFindValue">項目選項驗證值</param>
    Sub objDropDownListOptionCK(ByVal objDropDownList As DropDownList, ByVal strFindValue As String)
        Dim Item As ListItem = objDropDownList.Items.FindByValue(strFindValue)
        If Item IsNot Nothing Then
            objDropDownList.SelectedValue = strFindValue
        Else
            objDropDownList.Items.Clear()
        End If
    End Sub

    'RadioButtonList
    ''' <summary>
    ''' RadioButtonList自動產生項目選項[資料庫]
    ''' </summary>
    ''' <param name="objRadioButtonListID">指定產生RadioButtonList選項的ID</param>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTableName">查詢資料表</param>
    ''' <param name="strRowID">代碼欄位</param>
    ''' <param name="strRowName">中文欄位(例：AAA,BBB,CCC)</param>
    ''' <param name="strWhere">查詢條件(可省略)</param>
    ''' <param name="strOrderBy">排序(可省略)</param>
    ''' <param name="intAppoint">指定預設選項位置(可省略)</param>
    Sub objRadioButtonListOptionDB(ByVal objRadioButtonListID As RadioButtonList, ByVal strDNS As String, ByVal strTableName As String, ByVal strRowID As String, ByVal strRowName As String, Optional ByVal strWhere As String = "", Optional ByVal strOrderBy As String = "", Optional ByVal intAppoint As Integer = 0)
        Try
            objRadioButtonListID.Items.Clear()


            '開啟產生
            objConSC = New SqlConnection(strDNS)
            objConSC.Open()

            strSQLSC = "SELECT " & strRowID & ", " & strRowName & " FROM " & strTableName
            If strWhere <> "" Then strSQLSC &= " WHERE " & strWhere
            If strOrderBy <> "" Then strSQLSC &= " ORDER BY " & strOrderBy

            objCmdSC = New SqlCommand(strSQLSC, objConSC)
            objDRSC = objCmdSC.ExecuteReader

            I = 0
            objRadioButtonListID.Items.Insert(I, New ListItem) : objRadioButtonListID.Items.Item(I).Value = "" : objRadioButtonListID.Items.Item(I).Text = "未分類" : I = 1
            Do While objDRSC.Read
                '選項值
                With objRadioButtonListID
                    .Items.Insert(I, New ListItem)
                    .Items.Item(I).Value = objDRSC(strRowID)
                End With

                '選項文字
                Dim strOptionText As String = ""
                Dim strIArray() As String = strRowName.Split(",") '將字串存入陣列

                For J As Integer = 0 To strIArray.Length - 1 Step 1
                    strOptionText &= " " & objDRSC(strIArray(J))
                Next

                objRadioButtonListID.Items.Item(I).Text = strOptionText

                I += 1
            Loop

            objDRSC.Close()    '關閉連結        
            objConSC.Close()
            objCmdSC.Dispose() '手動釋放資源
            objConSC.Dispose()
            objConSC = Nothing '移除指標


            '--- 物件初始化 ---
            objRadioButtonListID.Items.Item(intAppoint).Selected = True '指定預設選項
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' RadioButtonList自動產生項目選項[自訂]
    ''' </summary>
    ''' <param name="objRadioButtonListID">指定產生RadioButtonList選項的ID</param>
    ''' <param name="strItemsArray">自訂選項，例：AAA,BBB,</param>
    ''' <param name="strValueArray">自訂選項值，例：123,456,</param>
    ''' <param name="intAppoint">指定預設選項位置(可省略)</param>
    Sub objRadioButtonListOptionGG(ByVal objRadioButtonListID As RadioButtonList, ByVal strItemsArray As String, ByVal strValueArray As String, Optional ByVal intAppoint As Integer = 0)
        Try
            objRadioButtonListID.Items.Clear()


            Dim strVArray() As String = strValueArray.Split(",")
            Dim strIArray() As String = strItemsArray.Split(",")

            For I As Integer = 0 To strVArray.Length - 2 Step 1
                With objRadioButtonListID
                    .Items.Insert(I, New ListItem)
                    .Items.Item(I).Value = strVArray(I)
                    .Items.Item(I).Text = strIArray(I)
                End With
            Next


            '--- 物件初始化 ---
            objRadioButtonListID.Items.Item(intAppoint).Selected = True '指定預設選項
        Catch ex As Exception

        End Try
    End Sub

    'DataGrid
    ''' <summary>
    ''' DataGrid樣式表
    ''' </summary>
    ''' <param name="objDataGridID">指定產生DataGrid列表的ID</param>
    ''' <param name="Type">指定位置類型(I:首頁 M:主列表 S:次列表)</param>
    Sub objDataGridStyle(ByVal objDataGridID As DataGrid, ByVal Type As String)
        '產生位置(I:)
        Select Case Type
            Case "I" '首頁
                With objDataGridID
                    .HeaderStyle.BackColor = Drawing.Color.FromName("#E7CDFF")
                    .HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                    .HeaderStyle.Font.Bold = True
                    .HeaderStyle.Height = 30
                    .ItemStyle.Height = 25
                    .ItemStyle.Font.Size = 14
                    .ItemStyle.Font.Bold = True
                    .AlternatingItemStyle.Height = 25
                    .AutoGenerateColumns = False
                End With
            Case "M" '主列表
                With objDataGridID
                    .BorderColor = Drawing.Color.FromName("#999933")
                    .HeaderStyle.BackColor = Drawing.Color.FromName("#e0ffff")
                    .HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                    .HeaderStyle.Font.Bold = True
                    .HeaderStyle.Height = 30
                    .ItemStyle.BackColor = Drawing.Color.FromName("#ffffff")
                    .ItemStyle.Height = 25
                    .ItemStyle.Font.Size = 14
                    .ItemStyle.Font.Bold = True
                    .AlternatingItemStyle.BackColor = Drawing.Color.FromName("#FACECE")
                    .AlternatingItemStyle.Height = 25
                    .PagerStyle.Font.Size = 14
                    .PagerStyle.Mode = PagerMode.NumericPages
                    .PagerStyle.HorizontalAlign = HorizontalAlign.Center
                    .PagerStyle.Position = PagerPosition.TopAndBottom
                    .AutoGenerateColumns = False
                End With
            Case "S" '次列表
                With objDataGridID
                    .HeaderStyle.BackColor = Drawing.Color.FromName("#f0fff0")
                    .HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                    .HeaderStyle.Font.Bold = True
                    .HeaderStyle.Height = 30
                    .ItemStyle.BackColor = Drawing.Color.FromName("#ffffff")
                    .ItemStyle.Height = 25
                    .ItemStyle.Font.Size = 14
                    .ItemStyle.Font.Bold = True
                    .AlternatingItemStyle.BackColor = Drawing.Color.FromName("#FACECE")
                    .AlternatingItemStyle.Height = 25
                    .PagerStyle.Font.Size = 14
                    .PagerStyle.Mode = PagerMode.NumericPages
                    .PagerStyle.HorizontalAlign = HorizontalAlign.Center
                    .PagerStyle.Position = PagerPosition.TopAndBottom
                    .AutoGenerateColumns = False
                End With
        End Select
    End Sub
    ''' <summary>
    ''' DataGrid查詢列表
    ''' </summary>
    ''' <param name="objDataGridID">指定產生DataGrid列表的ID</param>
    ''' <param name="objLabelCount">指定產生數量Label的ID</param>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strSQL">SQL語法</param>
    ''' <param name="strDataSet">指定DataSet名稱</param>
    Sub objDataGrid(ByVal objDataGridID As DataGrid, ByVal objLabelCount As Label, ByVal strDNS As String, ByVal strSQL As String, ByVal strDataSet As String)
        Try
            objConSC = New SqlConnection(strDNS)
            objConSC.Open()

            objDASC = New SqlDataAdapter(strSQL, objConSC)
            objDSSC = New DataSet()

            objDASC.Fill(objDSSC, strDataSet)
            objDataGridID.DataSource = objDSSC.Tables(strDataSet).DefaultView
            objLabelCount.Text = FormatNumber(objDSSC.Tables(strDataSet).DefaultView.Count, 0, -1, 0, -1)
            Try
                objDataGridID.DataBind()
            Catch ex As Exception
                objDataGridID.CurrentPageIndex = 0
                objDataGridID.DataBind()
            End Try

            objDSSC.Clear()    '關閉連結
            objConSC.Close()
            objDASC.Dispose()  '手動釋放資源
            objDSSC.Dispose()
            objConSC.Dispose()
            objConSC = Nothing '移除指標
        Catch ex As Exception

        End Try
    End Sub

    'Sort
    Function objSort(ByVal Type As String) As String
        Dim strString As String = ""

        Select Case Type
            Case "ASC"
                strString = "<div class='fa fa-lg fa-fw fa-sort-amount-asc'></div>"
            Case "DESC"
                strString = "<div class='fa fa-lg fa-fw fa-sort-amount-desc'></div>"
        End Select

        Return strString
    End Function
#End Region
#End Region

#Region "內容輸入防呆驗證"
    ''' <summary>
    ''' TextBox 欄位輸入判斷
    ''' </summary>
    ''' <param name="objTextBox">TextBox 陣列</param>
    ''' <param name="Type">判斷模式(0:必輸入 1:必數字)</param>
    ''' <param name="strTextName">需驗證欄位名稱(例：AAA,BBB,CCC,)</param>
    ''' <returns>TextBox 未符合驗證欄位名稱(String)</returns>    
    Function TextBox_Input(ByVal objTextBox() As TextBox, ByVal Type As Integer, ByVal strTextName As String) As String
        Dim strString As String = ""
        Dim strName() As String = strTextName.Split(",") '欄位名稱

        For I As Integer = 0 To objTextBox.Length - 1 Step 1
            Select Case Type
                Case 0
                    If objTextBox(I).Text = "" Then strString = strName(I) : Exit For
                Case 1
                    If objTextBox(I).Text <> "" Then If IsNumeric(objTextBox(I).Text) = False Then strString = strName(I) : Exit For
            End Select
        Next

        Return strString
    End Function

    ''' <summary>
    ''' DropDownList 欄位選擇判斷
    ''' </summary>
    ''' <param name="objDropDownList">DropDownList 陣列</param>
    ''' <param name="strTextName">需驗證欄位名稱(例：AAA,BBB,CCC,)</param>
    ''' <returns>DropDownList 未符合驗證欄位名稱(String)</returns>
    Function DropDownList_Input(ByVal objDropDownList() As DropDownList, ByVal strTextName As String) As String
        Dim strString As String = ""
        Dim strName() As String = strTextName.Split(",") '欄位名稱

        For I As Integer = 0 To objDropDownList.Length - 1 Step 1
            If objDropDownList(I).SelectedValue = "" Or objDropDownList(I).SelectedValue Is Nothing Then strString = strName(I) : Exit For
        Next

        Return strString
    End Function
#End Region

#Region "顯示內容清空"
    ''' <summary>
    ''' TextBox 的顯示內容
    ''' </summary>
    ''' <param name="objTextBox">TextBox 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub TextBox_Clear(ByVal objTextBox() As TextBox, ByVal Status As Integer)
        For I As Integer = 0 To objTextBox.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objTextBox(I)
                        .Text = ""
                    End With
                Case 1 '新增模式
                    With objTextBox(I)
                        .Text = ""
                    End With
                Case 2 '修改模式

                Case 3 '複製模式

                Case 9 '顯示模式

            End Select
        Next
    End Sub

    ''' <summary>
    ''' DropDownList 的顯示內容
    ''' </summary>
    ''' <param name="objDropDownList">DropDownList 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub DropDownList_Clear(ByVal objDropDownList() As DropDownList, ByVal Status As Integer)
        For I As Integer = 0 To objDropDownList.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objDropDownList(I)
                        .SelectedIndex = -1
                        .Items.Clear()
                    End With
                Case 1 '新增模式
                    With objDropDownList(I)
                        .SelectedIndex = -1
                        .Items.Clear()
                    End With
                Case 2 '修改模式

                Case 3 '複製模式

                Case 9 '顯示模式

            End Select
        Next
    End Sub

    ''' <summary>
    ''' RadioButtonList 的顯示內容
    ''' </summary>
    ''' <param name="objRadioButtonList">RadioButtonList 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub RadioButtonList_Clear(ByVal objRadioButtonList() As RadioButtonList, ByVal Status As Integer)
        For I As Integer = 0 To objRadioButtonList.Length - 1 Step 1
            Select Case Status
                Case 2 '修改模式

                Case 3 '複製模式

                Case Else
                    With objRadioButtonList(I)
                        .SelectedIndex = 0
                    End With
            End Select
        Next
    End Sub
#End Region

#Region "ReadOnly 屬性及 Enabled 屬性"
    ''' <summary>
    ''' 主項目的控制項屬性
    ''' </summary>
    ''' <param name="objTextBox">主項目 TextBox 陣列</param>
    ''' <param name="objDropDownList">主項目 DropDownList 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub Main_Control(ByVal objTextBox() As TextBox, ByVal objDropDownList() As DropDownList, ByVal objRadioButtonList() As RadioButtonList, ByVal Status As Integer)
        'TextBox *****
        For I As Integer = 0 To objTextBox.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objTextBox(I)
                        .Text = ""
                        .ReadOnly = True
                        .ForeColor = Drawing.Color.Blue
                        .BackColor = Drawing.Color.WhiteSmoke
                    End With
                Case 1 '新增模式
                    With objTextBox(I)
                        .Text = ""
                        .ReadOnly = False
                        .ForeColor = Drawing.Color.Blue
                        .BackColor = Drawing.Color.White
                    End With
                Case 2 '修改模式
                    With objTextBox(I)
                        .ReadOnly = True
                        .ForeColor = Drawing.Color.Blue
                        .BackColor = Drawing.Color.WhiteSmoke
                    End With
                Case 3 '複製模式
                    With objTextBox(I)
                        .ReadOnly = False
                        .ForeColor = Drawing.Color.Blue
                        .BackColor = Drawing.Color.White
                    End With
                Case 9 '顯示模式
                    With objTextBox(I)
                        .ReadOnly = True
                        .ForeColor = Drawing.Color.Blue
                        .BackColor = Drawing.Color.WhiteSmoke
                    End With
            End Select
        Next

        'DropDownList *****
        For I As Integer = 0 To objDropDownList.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objDropDownList(I)
                        .SelectedIndex = -1
                        .Enabled = False
                        .ForeColor = Drawing.Color.Blue
                    End With
                Case 1 '新增模式
                    With objDropDownList(I)
                        .Items.Clear()
                        .SelectedIndex = -1
                        .Enabled = True
                        .ForeColor = Drawing.Color.Blue
                    End With
                Case 2 '修改模式
                    With objDropDownList(I)
                        .Enabled = False
                        .ForeColor = Drawing.Color.Blue
                    End With
                Case 3 '複製模式
                    With objDropDownList(I)
                        .Enabled = True
                        .ForeColor = Drawing.Color.Blue
                    End With
                Case 9 '顯示模式
                    With objDropDownList(I)
                        .Enabled = False
                        .ForeColor = Drawing.Color.Blue
                    End With
            End Select
        Next

        'RadioButtonList *****
        For I As Integer = 0 To objRadioButtonList.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objRadioButtonList(I)
                        .Enabled = False
                    End With
                Case 1 '新增模式
                    With objRadioButtonList(I)
                        .Items(0).Selected = True
                        .Enabled = True
                    End With
                Case 2 '修改模式
                    With objRadioButtonList(I)
                        .Enabled = False
                    End With
                Case 3 '複製模式
                    With objRadioButtonList(I)
                        .Enabled = True
                    End With
                Case 9 '顯示模式
                    With objRadioButtonList(I)
                        .Enabled = False
                    End With
            End Select
        Next
    End Sub

    ''' <summary>
    ''' TextBox 的控制項屬性
    ''' </summary>
    ''' <param name="objTextBox">TextBox 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub TextBox_Control(ByVal objTextBox() As TextBox, ByVal Status As Integer)
        For I As Integer = 0 To objTextBox.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objTextBox(I)
                        .Text = ""
                        .ReadOnly = True
                        .BackColor = Drawing.Color.WhiteSmoke
                    End With
                Case 1 '新增模式
                    With objTextBox(I)
                        .Text = ""
                        .ReadOnly = False
                        .BackColor = Drawing.Color.White
                    End With
                Case 2 '修改模式
                    With objTextBox(I)
                        .ReadOnly = False
                        .BackColor = Drawing.Color.White
                    End With
                Case 3 '複製模式
                    With objTextBox(I)
                        .ReadOnly = False
                        .BackColor = Drawing.Color.White
                    End With
                Case 9 '顯示模式
                    With objTextBox(I)
                        .ReadOnly = True
                        .BackColor = Drawing.Color.WhiteSmoke
                    End With
            End Select
        Next
    End Sub

    ''' <summary>
    ''' DropDownList 的控制項屬性
    ''' </summary>
    ''' <param name="objDropDownList">DropDownList 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub DropDownList_Control(ByVal objDropDownList() As DropDownList, ByVal Status As Integer)
        For I As Integer = 0 To objDropDownList.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objDropDownList(I)
                        .SelectedIndex = -1
                        .Enabled = False
                    End With
                Case 1 '新增模式
                    With objDropDownList(I)
                        .SelectedIndex = -1
                        .Enabled = True
                    End With
                Case 2 '修改模式
                    With objDropDownList(I)
                        .Enabled = True
                    End With
                Case 3 '複製模式
                    With objDropDownList(I)
                        .Enabled = True
                    End With
                Case 9 '顯示模式
                    With objDropDownList(I)
                        .Enabled = False
                    End With
            End Select
        Next
    End Sub

    ''' <summary>
    ''' RadioButtonList 的控制項屬性
    ''' </summary>
    ''' <param name="objRadioButtonList">RadioButtonList 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub RadioButtonList_Control(ByVal objRadioButtonList() As RadioButtonList, ByVal Status As Integer)
        For I As Integer = 0 To objRadioButtonList.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objRadioButtonList(I)
                        .Items(0).Selected = True
                        .Enabled = False
                    End With
                Case 1 '新增模式
                    With objRadioButtonList(I)
                        .Items(0).Selected = True
                        .Enabled = True
                    End With
                Case 2 '修改模式
                    With objRadioButtonList(I)
                        .Enabled = True
                    End With
                Case 3 '複製模式
                    With objRadioButtonList(I)
                        .Enabled = True
                    End With
                Case 9 '顯示模式
                    With objRadioButtonList(I)
                        .Enabled = False
                    End With
            End Select
        Next
    End Sub

    ''' <summary>
    ''' Label 的控制項屬性
    ''' </summary>
    ''' <param name="objLabel">Label 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub Label_Control(ByVal objLabel() As Label, ByVal Status As Integer)
        For I As Integer = 0 To objLabel.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objLabel(I)
                        .Text = IIf(IsNumeric(objLabel(I).Text) = True, "0", "")
                    End With
                Case 1 '新增模式
                    With objLabel(I)
                        .Text = IIf(IsNumeric(objLabel(I).Text) = True, "0", "")
                    End With
                Case 2 '修改模式

                Case 3 '複製模式
                    With objLabel(I)
                        .Text = IIf(IsNumeric(objLabel(I).Text) = True, "0", "")
                    End With
                Case 9 '顯示模式

            End Select
        Next
    End Sub

    ''' <summary>
    ''' DataGrid 的控制項屬性
    ''' </summary>
    ''' <param name="objDataGrid">DataGrid 陣列</param>
    ''' <param name="Status">控制模式(0:一般模式 1:新增模式 2:修改模式 3:複製模式 9:顯示模式)</param>
    Sub DataGrid_Control(ByVal objDataGrid() As DataGrid, ByVal Status As Integer)
        For I As Integer = 0 To objDataGrid.Length - 1 Step 1
            Select Case Status
                Case 0 '一般模式
                    With objDataGrid(I)
                        .DataBind()
                    End With
                Case 1 '新增模式
                    With objDataGrid(I)
                        .DataBind()
                    End With
                Case 2 '修改模式

                Case 3 '複製模式
                    With objDataGrid(I)
                        .DataBind()
                    End With
                Case 9 '顯示模式

            End Select
        Next
    End Sub
#End Region

#Region "字串補數"
    ''' <summary>
    ''' 左補(0)
    ''' </summary>
    ''' <param name="intValue">需補位數</param>
    ''' <param name="strValue">字串</param>
    ''' <returns>補碼字串(String)</returns>
    Function strZero(ByVal intValue As Integer, ByVal strValue As String) As String
        Dim strString As String = strValue

        '開啟補數
        For I As Integer = Len(strString) To intValue - 1 Step 1
            strString = "0" & strString
        Next

        Return strString
    End Function
    ''' <summary>
    ''' 右補(空白)
    ''' </summary>
    ''' <param name="intValue">需補位數</param>
    ''' <param name="strValue">字串</param>
    ''' <returns>補碼字串(String)</returns>
    Function strEmpty(ByVal intValue As Integer, ByVal strValue As String) As String
        Dim strString As String = strValue

        '開啟補數
        For I As Integer = Len(strString) To intValue - 1 Step 1
            strString = strString & " "
        Next

        Return strString
    End Function
#End Region

#Region "系統編號"
    '查詢編號(取得號數控制檔的編號)
    Function QueryNO(ByVal noyear As Integer, ByVal kind As String)
        Dim tempdataset As DataSet
        Dim sqlstr As String
        sqlstr = "SELECT cont_no FROM acfno WHERE accyear=" & noyear & " and kind='" & kind & "'"

        '開啟查詢
        objConSC = New SqlConnection(DNS_ACC)
        objConSC.Open()


        objCmdSC = New SqlCommand(sqlstr, objConSC)
        objDRSC = objCmdSC.ExecuteReader

        If objDRSC.Read Then
            Return Trim(objDRSC("cont_no").ToString)
        Else
            If kind = "3" Or kind = "6" Then
                Return 1
            Else
                Return 0
            End If

        End If

        objDRSC.Close()    '關閉連結
        objConSC.Close()
        objCmdSC.Dispose() '手動釋放資源
        objConSC.Dispose()
        objConSC = Nothing '移除指標
    End Function
    '取用編號(取得號數控制檔的編號+1)
    Function RequireNO(ByVal connstr As String, ByVal noyear As Integer, ByVal kind As String)  '取用編號(取得號數控制檔的編號+1)
        Dim tempdataset As DataSet
        Dim sqlstr, MaxNo As String
        Dim cnt As Integer
        sqlstr = "SELECT cont_no FROM acfno WHERE accyear=" & noyear & " and kind='" & kind & "'"

        tempdataset = ADO.openmember(connstr, "acfno", sqlstr)

        'If MaxNo = "" Then '空號
        If tempdataset.Tables("acfno").Rows.Count = 0 Then
            If kind = "3" Or kind = "6" Or kind = "C" Then '轉帳傳票每年由2號起
                cnt = 2
            Else
                cnt = 1                      '收支傳票每年由1號起 
            End If
            sqlstr = "insert into acfno (accyear, kind, cont_no) values (" & noyear & ", '" & kind & "', " & cnt & ")"
        Else
            cnt = tempdataset.Tables("acfno").Rows(0).Item(0) + 1
            sqlstr = "update acfno set cont_no = cont_no+1 WHERE accyear=" & noyear & " and kind='" & kind & "'"
        End If
        ADO.dbExecute(connstr, sqlstr)
        Return cnt
    End Function

    '支票號加1
    Public Function AddCheckNo(ByVal chkno As String)
        Dim intI As Integer
        Dim strI As String
        If Trim(chkno) = "" Then Return ""
        strI = Mid(chkno, 4, 7)    '前三碼預留給英文字
        intI = Len(strI)
        Return Mid(chkno, 1, 3) & Right(Format(Val(strI) + 1, "0000000"), intI)
    End Function

    ''' <summary>
    ''' 系統自動編號
    ''' </summary>
    ''' <param name="Type">種類前置代碼</param>
    ''' <param name="Length">數字位數長度</param>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTable">資料表</param>
    ''' <param name="strRow">編號欄位名</param>
    ''' <param name="strWhere">條件值(可省略)</param>
    ''' <returns>系統編號(String)</returns>
    Function AutoNumber(ByVal Type As String, ByVal Length As String, ByVal strDNS As String, ByVal strTable As String, ByVal strRow As String, Optional strWhere As String = "1=1") As String
        Dim strString As String = ""

        '開啟查詢
        objConSC = New SqlConnection(strDNS)
        objConSC.Open()

        strSQLSC = "SELECT ISNULL(MAX(SUBSTRING(" & strRow & "," & (Type.Length + 1).ToString & ",20)), 0) As strMax FROM " & strTable
        strSQLSC &= " WHERE " & strWhere

        objCmdSC = New SqlCommand(strSQLSC, objConSC)
        objDRSC = objCmdSC.ExecuteReader()

        If objDRSC.Read Then strString = IIf(objDRSC("strMax").ToString = "", "1", CInt(objDRSC("strMax").ToString) + 1)

        strString = Type & strZero(Length, strString)

        objConSC.Close()   '關閉連結
        objCmdSC.Dispose() '手動釋放資源
        objConSC.Dispose()
        objConSC = Nothing '移除指標

        Return strString
    End Function

    ''' <summary>
    ''' 最大值、最小值計算(條件式)
    ''' </summary>
    ''' <param name="strDNS">資料庫連結字串</param>
    ''' <param name="strTable">查詢資料表</param>
    ''' <param name="strType">判斷種類(Max：最大值 Min：最小值)</param>
    ''' <param name="strMaxRow">欲判斷欄位</param>
    ''' <param name="strAsRow">判斷後欄位名(預設：strData)(可省略)</param>
    ''' <param name="strWhere">查詢條件值(可省略)</param>
    ''' <returns>傳回判斷值(String)</returns>
    Function dbMaxMin(ByVal strDNS As String, ByVal strTable As String, ByVal strType As String, ByVal strMaxRow As String, Optional strAsRow As String = "strData", Optional ByVal strWhere As String = "1=1") As String
        Dim strString As String = ""

        '開啟查詢
        objConSC = New SqlConnection(strDNS)
        objConSC.Open()

        strSQLSC = "SELECT " & strType & "(" & strMaxRow & ") AS " & strAsRow & " FROM " & strTable
        strSQLSC &= " WHERE " & strWhere

        objCmdSC = New SqlCommand(strSQLSC, objConSC)
        objDRSC = objCmdSC.ExecuteReader()

        If objDRSC.Read Then strString = objDRSC(strAsRow).ToString

        objDRSC.Close()    '關閉連結
        objConSC.Close()
        objCmdSC.Dispose() '手動釋放資源
        objConSC.Dispose()
        objConSC = Nothing '移除指標

        Return strString
    End Function

    '系統單號
    Function ACC_AutoNo(ByVal DNS As String, ByVal strPage As String, ByVal strTable As String, ByVal strRow As String) As String
        Dim strString As String = ""
        Dim strMaxNo As String = dbMaxMin(DNS, strTable, "Max", strRow, "strMax", strRow & " LIKE '" & strPage & "%'")
        Dim intMaxNo As Integer = 1
        If strMaxNo <> "" Then intMaxNo = Mid(strMaxNo, 5, Len(strMaxNo) - 4) + 1

        strString = strPage & strZero(6, intMaxNo)

        Return strString
    End Function
#End Region

#Region "預算相關"
    Function QueryBGAmt(ByVal noyear As Integer, ByVal accno As String)  '查詢預算餘額(取得全年預算扣除(已請購+已開支)的餘額)
        '業務單位查詢時,請購數並未update至totper,主計單位查詢時,請購數已由業務單位請購數update至totper  
        Dim tempdataset As DataSet
        Dim sqlstr As String
        sqlstr = "SELECT bg1+bg2+bg3+bg4+up1+up2+up3+up4-totper-totuse as balance,flow FROM bgf010 WHERE accyear=" & noyear & " and accno='" & accno & "'"

        tempdataset = ADO.openmember(DNS_ACC, "temp", sqlstr)
        If tempdataset.Tables("temp").Rows.Count = 0 Then
            Session("flow") = "0"
            Return 0
        Else
            Session("flow") = tempdataset.Tables("temp").Rows(0).Item("flow")  '將可否溢支置全域變數
            Return tempdataset.Tables("temp").Rows(0).Item(0)
        End If
        tempdataset = Nothing
    End Function

    Function QueryUnUseAmt(ByVal noyear As Integer, ByVal accno As String, ByVal season As Integer)  '查詢開支餘額(取得當季可開支餘額(已分配數-已開支的餘額)
        '業務單位查詢時,開支數並未update至totuse,主計單位查詢時,開支數已由業務單位開支數update至totuse  
        Dim tempdataset As DataSet
        Dim sqlstr As String
        Dim i As Integer
        Dim ans As Decimal
        If noyear < Session("sYear") Then   'whh 95/5/31 modi 
            season = 4
        End If
        ans = 0
        sqlstr = "SELECT * FROM bgf010 WHERE accyear=" & noyear & " and accno='" & accno & "'"
        tempdataset = ADO.openmember(DNS_ACC, "temp", sqlstr)
        If tempdataset.Tables("temp").Rows.Count = 0 Then
            Session("flow") = "0"
            Return 0
        Else
            For i = 1 To season
                ans = ans + tempdataset.Tables("temp").Rows(0).Item("bg" & LTrim(Str(i))) + tempdataset.Tables("temp").Rows(0).Item("up" & LTrim(Str(i)))
            Next
            Session("flow") = tempdataset.Tables("temp").Rows(0).Item("flow")  '將可否溢支置全域變數
            Return ans - tempdataset.Tables("temp").Rows(0).Item("totuse")
        End If
        '要加讀取2-1302收入總額,作為可開支數 must use 7grade accno
        tempdataset = Nothing
    End Function

    Function QueryUsedAmt(ByVal bgno As String)  '查詢該筆請購已開支總額
        Dim tempdataset As DataSet
        Dim sqlstr As String
        sqlstr = "SELECT sum(useamt) as useamt FROM bgf030 WHERE bgno='" & bgno & "'"
        tempdataset = ADO.openmember(DNS_ACC, "temp", sqlstr)

        If tempdataset.Tables("temp").Rows.Count = 0 Or IsDBNull(tempdataset.Tables("temp").Rows(0).Item(0)) Then
            Return 0
        Else
            Return tempdataset.Tables("temp").Rows(0).Item(0)
        End If
        tempdataset = Nothing
    End Function
#End Region
End Class
