
Partial Class admin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (Session("role") = "Administrator" Or Session("role") = "ICO") Then

            Response.Redirect("default.aspx")

        Else

            Label1.Text = Session("name") & " | " & Session("role")


        End If

    End Sub
End Class
