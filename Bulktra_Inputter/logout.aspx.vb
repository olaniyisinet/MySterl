
Partial Class logout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Session.Abandon()
        Response.Redirect("default.aspx")
    End Sub
End Class
