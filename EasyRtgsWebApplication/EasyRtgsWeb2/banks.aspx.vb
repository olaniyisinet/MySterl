
Partial Class banks
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim sr As New IO.StreamReader(Server.MapPath("./config/banks.txt"))

        Do

            DropDownList1.Items.Add(sr.ReadLine)


        Loop Until sr.EndOfStream





    End Sub
End Class
