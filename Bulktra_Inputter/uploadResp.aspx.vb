
Partial Class uploadResp
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Session("name") = "" Then
            Response.Redirect("default.aspx")

        End If
        Label2.Text = "Welcome, " & Session("name") & "    |   <a href='main.aspx'>Home</a>    |    <a href='myuploads.aspx'>My Uploads</a>    |   <a href='Bulktraformats.zip'>Download Excel Format</a>     |     <a href='logout.aspx'>signout</a>"

    End Sub
End Class
