
Partial Class PayResponse
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("name") = "" Then

            Response.Redirect("default.aspx")

        Else
            'Label1.Text = Session("cusPay")
            'Label2.Text = Session("charges")

        End If
    End Sub
End Class
