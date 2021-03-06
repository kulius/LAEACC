﻿Module PGMModule
    ''' <summary>
    ''' 民國年轉字串西元年(101-12-31->2012-12-31)
    ''' </summary>
    ''' <param name="strValue">日期字串(101-12-31)</param>
    ''' <returns>轉換後日期(2012-12-31)</returns>
    Function sDateCDToAD(ByVal strValue As String) As String
        Dim strString As String = "" '傳回變數值

        If strValue <> "" Then
            Dim yy As Integer, mmdd As String
            yy = Mid(strValue, 1, InStr(strValue, "-") - 1)
            If yy < 1000 Then yy = yy + 1911
            mmdd = Mid(strValue, InStr(strValue, "-"))
            strString = yy & mmdd
        End If

        Return strString
    End Function
    ''' <summary>
    ''' 西元年轉字串民國年(2012/12/31->101-12-31)
    ''' </summary>
    ''' <param name="strValue">日期字串(2012/12/311)</param>
    ''' <returns>轉換後日期(101-12-31)</returns>
    Function sDateADToCD(ByVal strValue As String) As String
        Dim strString As String = "" '傳回變數值

        If strValue <> "" Then
            If InStr(strValue, " ") > 0 Then strValue = Mid(strValue, 1, InStr(strValue, " "))
            Dim yy As Integer, mmdd As String
            yy = Mid(strValue, 1, InStr(strValue, "/") - 1)
            If yy > 1000 Then yy = yy - 1911
            mmdd = Mid(strValue, InStr(strValue, "/"))
            strString = (yy & mmdd).Replace("/", "-")
        End If

        Return strString
    End Function
End Module
