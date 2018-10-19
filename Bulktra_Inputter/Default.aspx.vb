
Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim login As New ldap
        Dim utype As New bulktra

        Label1.Text = ""

        'Dim access As New GAL.Service
        'If access.ApplicationAccess(1, TextBox1.Text) = "0" Then
        '    Label1.Text = "This application is no longer available by this time."
        '    Exit Sub

        'End If

        If login.login(TextBox1.Text.Trim(), TextBox2.Text.Trim()) = True Then

            If utype.checkUsrType(Server.HtmlEncode(TextBox1.Text.Trim())) = True Then

                Session("uname") = TextBox1.Text.Trim()
                Session("name") = utype.getName(TextBox1.Text.Trim())
                Session("type") = utype.getTypex(TextBox1.Text.Trim())
                Session("email") = utype.getEmail(TextBox1.Text.Trim())
                Session("branch") = utype.getBranch(TextBox1.Text.Trim())
                Session("pwd") = TextBox2.Text.Trim()
                Session("tellerid") = utype.getTellerID(TextBox1.Text.Trim)
                Session("bracode") = Session("branch").ToString.Split("*")(0)
                Session("braname") = Session("branch").ToString.Split("*")(1)




                Session.Timeout = 60

                Response.Redirect("main.aspx")

            Else

                Label1.Text = "User must be an Inputter"

            End If

        Else

            Label1.Text = "Wrong userID or Password or Inactive account. Try again."



        End If
    End Sub

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
       
    End Sub
End Class
