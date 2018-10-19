
Partial Class postsuccess
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("name") = "" Then

            Response.Redirect("default.aspx")

        Else
            Label1.Text = Session("name")

        End If

        If Session("role").ToString().ToLower().CompareTo("trops") = 0 Then
            hypLinkBranch.Visible = False
            hypLinkTrops.Visible = True
            hypHome.NavigateUrl = "~/trops/trops_landing.aspx"
            hypLogout.NavigateUrl = "~/logout.aspx"
            hypMyTransactions.NavigateUrl = "~/mytransactions.aspx"
            hypLinkTrops.Visible = True
            hypLinkTrops.NavigateUrl = "~/trops/trops_landing.aspx"
        ElseIf Session("role").ToString().ToLower().CompareTo("inputter") = 0 Then
            hypHome.NavigateUrl = "main.aspx"
            hypLinkBranch.Visible = True
            hypLinkTrops.Visible = False
        Else
            hypHome.NavigateUrl = "main.aspx"
        End If

    End Sub
End Class
