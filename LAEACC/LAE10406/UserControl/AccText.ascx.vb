Public Class AccText
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Function get_accno() As String
        Dim a1 As String = Format(ACCNO1.Text, " ")
        Dim a2 As String = Format(ACCNO2.Text, "    ")
        Dim a3 As String = Format(ACCNO3.Text, "  ")
        Dim a4 As String = Format(ACCNO4.Text, "  ")
        Dim a5 As String = Format(ACCNO5.Text, "       ")
        Dim a6 As String = Format(ACCNO6.Text, " ")

        Return a1 & a2 & a3 & a4 & a5 & a6
    End Function

End Class