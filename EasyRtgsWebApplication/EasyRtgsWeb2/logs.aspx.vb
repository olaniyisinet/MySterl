
Partial Class logs
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim esy As New easyrtgs

        Dim conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())
        conn.Open()


        Dim cmd As New Data.SqlClient.SqlCommand("select * from logs where action like @action", conn)
        cmd.Parameters.AddWithValue("@action", "%" & TextBox1.Text & "%")
        Dim da As New Data.SqlClient.SqlDataAdapter

        da.SelectCommand = cmd

        Dim ds As New Data.DataSet
        da.Fill(ds)
        DataGrid1.DataSource = ds
        DataGrid1.DataBind()
        conn.Close()
    End Sub
End Class
