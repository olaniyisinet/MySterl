
Partial Class test
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
        '     Label1.Text = getOldAcc(TextBox1.Text)
        Label1.Text = TextBox1.Text.Split("-")(1).Trim

    End Sub
    Function getOldAcc(ByVal nuban As String) As String
        Dim ds As New Data.DataSet
        Dim bnk As New bank.bank
        Dim oldacc As String = ""

        oldacc = bnk.getAccountFromNUBAN(nuban)

        Return oldacc

    End Function

End Class
