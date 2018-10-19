
Partial Class menu
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Response.Redirect("main.aspx")

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Session("name")) Then
            Response.Redirect("default.aspx")

        End If
        Label2.Text = Session("name") & " <b>[" & Session("job") & "]</b>"

 
    End Sub

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Response.Redirect("ba.aspx")

    End Sub
End Class
