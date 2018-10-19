
Partial Class AdminLogin
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        If TextBox1.Text.Contains("'") Or TextBox1.Text.Contains("&") Or TextBox1.Text.Contains("=") Or TextBox2.Text.Contains("=") Or TextBox2.Text.Contains("'") Then

            Label1.Text = "Injection attack detected!. Sorry, We are Protected."
            Exit Sub

        End If

        Dim es As New easyrtgs
        If es.CheckAdminUser(TextBox1.Text) = "True" Then

        Else

            Label1.Text = "You are not an Administrator"
            Exit Sub

        End If

        Dim l As New ldap
        If l.login(TextBox1.Text, TextBox2.Text) Then

            Session("role") = es.getRole(TextBox1.Text)

            Session("name") = TextBox1.Text


            Response.Redirect("admin.aspx")




        End If

    End Sub
End Class
