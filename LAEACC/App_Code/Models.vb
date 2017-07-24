'::::::::::::::::::
':::參數驗證 ::::::
':::系統參數 ::::::
'::::::::::::::::::
Public Class Models

#Region "Models副程式"

#End Region

#Region "伺服器日期及時間"
    ''' <summary>
    ''' 伺服器日期(1011231)
    ''' </summary>
    ''' <returns>民國年月日(1011231)</returns>
    Function NowDate() As String
        Dim dtDay As Date
        Dim strYear, strMonth, strDay As String
        Dim strString As String

        dtDay = Today
        strYear = Year(dtDay) - 1911
        strMonth = Month(dtDay)
        strDay = Microsoft.VisualBasic.Day(dtDay)

        '日期補數
        strYear = IIf(Len(strYear) = 2, "0" & strYear, strYear)
        strMonth = IIf(Len(strMonth) = 1, "0" & strMonth, strMonth)
        strDay = IIf(Len(strDay) = 1, "0" & strDay, strDay)

        strString = strYear & strMonth & strDay

        Return strString
    End Function

    ''' <summary>
    ''' 伺服器時間(23:59:59)
    ''' </summary>
    ''' <returns>24小時制(23:59:59)</returns>
    Function NowTime() As String
        Dim strHour, strMinute, strSecond As String
        Dim strString As String

        strHour = Hour(Now)
        strMinute = Minute(Now)
        strSecond = Second(Now)

        '時間補數
        strHour = IIf(Len(strHour) = 1, "0" & strHour, strHour)
        strMinute = IIf(Len(strMinute) = 1, "0" & strMinute, strMinute)
        strSecond = IIf(Len(strSecond) = 1, "0" & strSecond, strSecond)

        strString = strHour & ":" & strMinute & ":" & strSecond

        Return strString
    End Function
#End Region
#Region "日期、時間字串轉換"
    ''' <summary>
    ''' 字串日期轉日期(1011231->101-12-31)
    ''' </summary>
    ''' <param name="strValue">日期字串(1011231)</param>
    ''' <returns>轉換後日期(101-12-31)</returns>    
    Function strStrToDate(ByVal strValue As String) As String
        Dim strString As String = "" '傳回變數值
        Dim strYear, strMonth, strDay As String

        If strValue <> "" Then
            strYear = Mid(strValue, 1, 3)
            strMonth = Mid(strValue, 4, 2)
            strDay = Mid(strValue, 6, 2)

            strString = strYear & "-" & strMonth & "-" & strDay
        End If

        Return strString
    End Function

    ''' <summary>
    ''' 民國年轉字串西元年(101-12-31->2012-12-31)
    ''' </summary>
    ''' <param name="strValue">日期字串(101-12-31)</param>
    ''' <returns>轉換後日期(2012-12-31)</returns>
    Function strDateChinessToAD(ByVal strValue As String) As String
        Dim strString As String = "" '傳回變數值
        Dim strYear, strMonth, strDay As String
        Dim strDateArray() As String

        If strValue <> "" And InStr(strValue, "-") <> 0 Then
            strDateArray = strValue.Split("-") '字串分離

            strYear = Int(strDateArray(0)) + 1911
            strMonth = Int(strDateArray(1))
            strDay = Int(strDateArray(2))

            strYear = IIf(Len(strYear) = 3, "0" & strYear, strYear)
            strMonth = IIf(Len(strMonth) = 1, "0" & strMonth, strMonth)
            strDay = IIf(Len(strDay) = 1, "0" & strDay, strDay)

            strString = strYear & "-" & strMonth & "-" & strDay
        End If

        Return strString
    End Function
    Function strDateChinessToAD1(ByVal strValue As String) As String
        Dim strString As String = "" '傳回變數值
        Dim strYear, strMonth, strDay As String
        Dim strDateArray() As String

        If strValue <> "" And InStr(strValue, "-") <> 0 Then
            strDateArray = strValue.Split("-") '字串分離

            strYear = Int(strDateArray(0)) + 1911
            strMonth = Int(strDateArray(1))
            strDay = Int(strDateArray(2))

            strYear = IIf(Len(strYear) = 3, "0" & strYear, strYear)
            strMonth = IIf(Len(strMonth) = 1, "0" & strMonth, strMonth)
            strDay = IIf(Len(strDay) = 1, "0" & strDay, strDay)

            strString = strYear & "/" & strMonth & "/" & strDay
        End If

        Return strString
    End Function

    ''' <summary>
    ''' 西元年轉字串民國年(2012-12-31->101-12-31)
    ''' </summary>
    ''' <param name="strValue">日期字串(2012-12-31)</param>
    ''' <param name="strKind">字串樣式(/)(可省略)</param>
    ''' <returns>轉換後日期(101-12-31)</returns>
    Function strDateADToChiness(ByVal strValue As String, Optional ByVal strKind As String = "/") As String
        Dim strString As String = "" '傳回變數值
        Dim strYear, strMonth, strDay As String
        Dim strDateArray() As String
        If strValue <> "" Then
            strValue = FormatDateTime(strValue, DateFormat.ShortDate)
        End If

        If strValue <> "" And InStr(strValue, strKind) <> 0 Then
            strDateArray = strValue.Split(strKind) '字串分離

            strYear = Int(strDateArray(0)) - 1911
            strMonth = Int(strDateArray(1))
            strDay = Int(strDateArray(2))

            strYear = IIf(Len(strYear) = 2, "0" & strYear, strYear)
            strMonth = IIf(Len(strMonth) = 1, "0" & strMonth, strMonth)
            strDay = IIf(Len(strDay) = 1, "0" & strDay, strDay)

            strString = strYear & "-" & strMonth & "-" & strDay
        End If

        Return strString
    End Function

    ''' <summary>
    ''' 西元年取會計年度(2012-12-31->101)
    ''' </summary>
    ''' <param name="strValue">日期字串(2012-12-31)</param>
    ''' <param name="strKind">字串樣式(/)(可省略)</param>
    ''' <returns>轉換後年度(101)</returns>
    Function strDateADToChinessYear(ByVal strValue As String, Optional ByVal strKind As String = "/") As String
        Dim strString As String = "" '傳回變數值
        Dim strYear, strMonth, strDay As String
        Dim strDateArray() As String

        If strValue <> "" And InStr(strValue, strKind) <> 0 Then
            strDateArray = strValue.Split(strKind) '字串分離

            strYear = Int(strDateArray(0)) - 1911
            strMonth = Int(strDateArray(1))
            strDay = Int(strDateArray(2))

            strYear = IIf(Len(strYear) = 2, "0" & strYear, strYear)
            strMonth = IIf(Len(strMonth) = 1, "0" & strMonth, strMonth)
            strDay = IIf(Len(strDay) = 1, "0" & strDay, strDay)

            strString = strYear
        End If

        Return strString
    End Function

    ''' <summary>
    ''' 西元年取季(2012-01-31->1, 2012-06-30->2)
    ''' </summary>
    ''' <param name="strValue">日期字串(2012-12-31)</param>
    ''' <param name="strKind">字串樣式(/)(可省略)</param>
    ''' <returns>取季(1,2,3,4)</returns>
    Function strDateADToSeason(ByVal strValue As String, Optional ByVal strKind As String = "/") As String
        Dim strString As String = "" '傳回變數值
        Dim strYear, strMonth, strDay As String
        Dim strDateArray() As String

        If strValue <> "" And InStr(strValue, strKind) <> 0 Then
            strDateArray = strValue.Split(strKind) '字串分離

            strYear = Int(strDateArray(0)) - 1911
            strMonth = Int(strDateArray(1))
            strDay = Int(strDateArray(2))

            strYear = IIf(Len(strYear) = 2, "0" & strYear, strYear)
            strMonth = IIf(Len(strMonth) = 1, "0" & strMonth, strMonth)
            strDay = IIf(Len(strDay) = 1, "0" & strDay, strDay)


            Select Case strMonth
                Case Is <= 3
                    strString = 1
                Case Is <= 6
                    strString = 2
                Case Is <= 9
                    strString = 3
                Case Is <= 12
                    strString = 4
            End Select
        End If
        Return strString

    End Function

    ''' <summary>
    ''' 民國年取季(101-12-31-> 4)
    ''' </summary>
    ''' <param name="strValue">日期字串(101-12-31)</param>
    ''' <returns>取季(1,2,3,4)</returns>
    Function strDateChinessToSeason(ByVal strValue As String) As String
        Dim strString As String = "" '傳回變數值
        Dim strYear, strMonth, strDay As String
        Dim strDateArray() As String

        If strValue <> "" And InStr(strValue, "-") <> 0 Then
            strDateArray = strValue.Split("-") '字串分離

            strYear = Int(strDateArray(0)) + 1911
            strMonth = Int(strDateArray(1))
            strDay = Int(strDateArray(2))

            strYear = IIf(Len(strYear) = 3, "0" & strYear, strYear)
            strMonth = IIf(Len(strMonth) = 1, "0" & strMonth, strMonth)
            strDay = IIf(Len(strDay) = 1, "0" & strDay, strDay)

            Select Case CInt(strMonth)
                Case Is <= 3
                    strString = 1
                Case Is <= 6
                    strString = 2
                Case Is <= 9
                    strString = 3
                Case Is <= 12
                    strString = 4
            End Select
        End If

        Return strString
    End Function

    Function ShortDate(ByVal indate As String)   '輸出用的日期格式 yy/MM/dd
        If indate = "" Then Return ""
        Dim strOutDate As String
        strOutDate = Format(Year(indate), 0) & "/" & Format(CDate(indate), "MM/dd")
        Return strOutDate
    End Function

    Function RShortDate(ByVal indate As String)   '輸出用的日期格式 yy/MM/dd
        If indate = "" Then Return ""
        Dim strOutDate As String
        Dim sYear As Integer
        If Year(indate) > 1000 Then
            sYear = Year(indate) - 1911
        Else
            sYear = Year(indate)
        End If
        strOutDate = Format(sYear, 0) & "/" & Format(CDate(indate), "MM/dd")
        Return strOutDate
    End Function

    Function FormatAccno(ByVal accno As String)
        Dim i As Integer
        If Grade(accno) = 8 Then Return Mid(accno, 1, 1) & "-" & Mid(accno, 2, 4) & "-" & Mid(accno, 6, 2) & "-" & Mid(accno, 8, 2) & "-" & Mid(accno, 10, 7) & "-" & Mid(accno, 17, 1)
        If Grade(accno) = 7 Then Return Mid(accno, 1, 1) & "-" & Mid(accno, 2, 4) & "-" & Mid(accno, 6, 2) & "-" & Mid(accno, 8, 2) & "-" & Mid(accno, 10, 7)
        If Grade(accno) = 6 Then Return Mid(accno, 1, 1) & "-" & Mid(accno, 2, 4) & "-" & Mid(accno, 6, 2) & "-" & Mid(accno, 8, 2)
        If Grade(accno) = 5 Then Return Mid(accno, 1, 1) & "-" & Mid(accno, 2, 4) & "-" & Mid(accno, 6, 2)
        If Grade(accno) = 1 Then
            Return Mid(accno, 1, 1)
        Else
            Return Mid(accno, 1, 1) & "-" & Mid(accno, 2, 4)
        End If
    End Function
#End Region

#Region "數字轉換大寫"
    ''' <summary>
    ''' 數字轉換大寫-中文
    ''' </summary>
    ''' <param name="ktotal">數字</param>
    ''' <returns>零壹貳參肆伍陸柒捌玖</returns>
    Function NumberToChinese(ByVal ktotal As String, Optional kspace As Integer = 0) As String
        Dim i As Integer = 0
        Dim c As String = ""
        Dim d As String = ""
        Dim ctotal As String = ""

        ctotal = Right("000000000" + ktotal, 9)
        For i = 1 To Len(ctotal)
            d = Mid(ctotal, i, 1)
            If d <> "0" Then
                c += Mid("零壹貳參肆伍陸柒捌玖", Val(d) + 1, 1)
                If kspace = 0 Then
                    Select Case i
                        Case 1 : c += "億"
                        Case 2 : c += "仟"
                        Case 3 : c += "佰"
                        Case 4 : c += "拾"
                        Case 5 : c += "萬"
                        Case 6 : c += "仟"
                        Case 7 : c += "佰"
                        Case 8 : c += "拾"
                        Case 9 : c += ""
                    End Select
                Else
                    c = c + Space(1)
                End If
            ElseIf d = "0" And i = 5 Then
                c += "萬"
            End If
        Next

        If ktotal < 10000 Then c = Mid(c, 2, Len(c) - 1)

        NumberToChinese = c
    End Function
    ''' <summary>
    ''' 數字轉換大寫-簡中文
    ''' </summary>
    ''' <param name="ktotal">數字</param>
    ''' <returns>零一二三四五六七八九</returns>
    Function NumberToChineseSort(ByVal ktotal As String, Optional kspace As Integer = 0) As String
        Dim i As Integer = 0
        Dim c As String = ""
        Dim d As String = ""
        Dim ctotal As String = ""

        ctotal = Right("000000000" + ktotal, 9)
        For i = 1 To Len(ctotal)
            d = Mid(ctotal, i, 1)
            If d <> "0" Then
                c += Mid("零一二三四五六七八九", Val(d) + 1, 1)
                If kspace = 0 Then
                    Select Case i
                        Case 1 : c += "億"
                        Case 2 : c += "仟"
                        Case 3 : c += "佰"
                        Case 4 : c += "拾"
                        Case 5 : c += "萬"
                        Case 6 : c += "仟"
                        Case 7 : c += "佰"
                        Case 8 : c += "十"
                        Case 9 : c += ""
                    End Select
                Else
                    c = c + Space(1)
                End If
            ElseIf d = "0" And i = 5 Then
                c += "萬"
            End If
        Next

        If ktotal < 10000 Then c = Mid(c, 2, Len(c) - 1)

        NumberToChineseSort = c
    End Function
#End Region

#Region "文字處理"
#Region "字串補碼"
    ''' <summary>
    ''' 左補(0)
    ''' </summary>
    ''' <param name="intValue">字串總碼數</param>
    ''' <param name="strValue">字串值</param>
    ''' <returns>傳回補齊字串(String)</returns>    
    Function strZero(ByVal intValue As Integer, ByVal strValue As String) As String
        Dim strString As String = strValue

        '開啟補值
        For I As Integer = Len(strString) To intValue - 1 Step 1
            strString = "0" & strString
        Next

        Return strString
    End Function
    ''' <summary>
    ''' 右補(空白)
    ''' </summary>
    ''' <param name="intValue">字串總碼數</param>
    ''' <param name="strValue">字串值</param>
    ''' <returns>傳回補齊字串(String)</returns>    
    Function strEmpty(ByVal intValue As Integer, ByVal strValue As String) As String
        Dim strString As String = strValue

        '開啟補值
        For I As Integer = Len(strString) To intValue - 1 Step 1
            strString = strString & " "
        Next

        Return strString
    End Function
#End Region

#Region "顏色"
    ''' <summary>
    ''' 登入、登出之顏色(I:藍 O:紅)
    ''' </summary>
    ''' <param name="strValue">判斷字串</param>
    ''' <returns>文字顏色</returns>    
    Function strIOColor(ByVal strValue As String) As String
        Dim strString As String = ""

        Select Case strValue
            Case "I"
                strString = "<span style='color:#00c;'><b>系統登入</b></span>"
            Case "O"
                strString = "<span style='color:#c00;'><b>系統登出</b></span>"
        End Select

        Return strString
    End Function
    ''' <summary>
    ''' 新增、修改、刪除之顏色(I:藍 U:綠 D:紅)
    ''' </summary>
    ''' <param name="strValue">判斷字串</param>
    ''' <returns>文字顏色</returns>    
    Function strIUDColor(ByVal strValue As String) As String
        Dim strString As String = ""

        Select Case strValue
            Case "A"
                strString = "<span style='color:#0300FA;'><b>新增</b></span>"
            Case "I"
                strString = "<span style='color:#0300FA;'><b>新增</b></span>"
            Case "M"
                strString = "<span style='color:#FA00F7;'><b>修改</b></span>"
            Case "U"
                strString = "<span style='color:#FA00F7;'><b>修改</b></span>"
            Case "D"
                strString = "<span style='color:#FA8000;'><b>刪除</b></span>"
        End Select

        Return strString
    End Function
    ''' <summary>
    ''' 是、否之顏色(Y:藍 N:紅)
    ''' </summary>
    ''' <param name="strValue">判斷字串</param>
    ''' <returns>文字顏色</returns>    
    Function strYNColor(ByVal strValue As String) As String
        Dim strString As String = ""

        Select Case strValue
            Case "Y"
                strString = "<span style='color:#00f;'><b>是</b></span>"
            Case "N"
                strString = "<span style='color:#f00;'><b>否</b></span>"
        End Select

        Return strString
    End Function
    ''' <summary>
    ''' 成功、失敗之顏色(S:成功 F:失敗)
    ''' </summary>
    ''' <param name="strValue">判斷字串</param>
    ''' <returns>文字顏色</returns>    
    Function strSFColor(ByVal strValue As String) As String
        Dim strString As String = ""

        Select Case strValue
            Case "S"
                strString = "<span style='color:#00f;'><b>成功</b></span>"
            Case "F"
                strString = "<span style='color:#f00;'><b>失敗</b></span>"
        End Select

        Return strString
    End Function
    ''' <summary>
    ''' 覆核、未覆核之顏色(Y:覆核 N:未覆核)
    ''' </summary>
    ''' <param name="strValue">判斷字串</param>
    ''' <returns>文字顏色</returns>    
    Function strCKColor(ByVal strValue As String) As String
        Dim strString As String = ""

        Select Case strValue
            Case "Y"
                strString = "<span style='color:#00f;'><b>核</b></span>"
            Case "N"
                strString = "<span style='color:#f00;'><b>未核</b></span>"
        End Select

        Return strString
    End Function
#End Region

    '去除逗號
    Function ValComa(ByVal lblNum As String)
        If IsDBNull(lblNum) Or lblNum = "" Then
            Return 0
        Else
            Return Val(Replace(lblNum, ",", ""))
        End If
    End Function
    '判斷會計科目階層
    Function Grade(ByVal accno As String)
        Select Case Len(Trim(accno))
            Case Is = 1
                Return 1
            Case Is = 2
                Return 2
            Case Is = 3
                Return 3
            Case Is = 5
                Return 4
            Case Is <= 7
                Return 5
            Case Is <= 9
                Return 6
            Case Is <= 16
                Return 7
            Case Is = 17
                Return 8
        End Select
    End Function

    Function FullDate(ByVal indate As String)
        If indate = "" Then Return ""
        Dim strOutDate As String
        strOutDate = Year(indate)
        If strOutDate <= 1000 Then strOutDate = strOutDate + 1911
        strOutDate &= "/" & Format(CDate(indate), "MM/dd")
        Return strOutDate
    End Function

    Function genLongDate(ByVal Sdate As String)
        If Len(Trim(Sdate)) = 0 Or Mid(Sdate, 1, 4) = "0001" Then
            Return "null"
        Else
            Dim mmdd As String
            Dim yy As Integer
            yy = Mid(Sdate, 1, InStr(Sdate, "/") - 1)
            If yy < 1000 Then yy = yy + 1911
            mmdd = Mid(Sdate, InStr(Sdate, "/"))
            Return yy & mmdd
        End If
    End Function

    Function cutright1(ByVal instr As String, ByVal compstr As String)
        instr = Trim(instr)
        If Len(instr) > 0 Then
            Return IIf(Right(instr, 1) = compstr, Left(instr, Len(instr) - 1), instr)
        Else
            Return instr
        End If
    End Function

   
    


#End Region
End Class