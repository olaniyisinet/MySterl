
Partial Class myuploads
    Inherits System.Web.UI.Page
    Private chk As New bulktra
    Private enc As New CyclopsDES

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Session("name") = "" And Session("type") <> "Inputter") Then
            Response.Redirect("default.aspx")

        Else

            label2.Text = "Welcome " & Session("name") & " |  " & Session("type") & "  |    <a href='main.aspx'>Home</a>   |   <a href='logout.aspx'>Logout</a>"

            Dim conn As New Data.SqlClient.SqlConnection(chk.sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand("select * from  batches where uploaded_by =@name order by batch_date desc", conn)
            cmd.Parameters.AddWithValue("@name", Session("name"))
            Dim da As New Data.SqlClient.SqlDataAdapter
            da.SelectCommand = cmd

            Dim ds As New Data.DataSet
            da.Fill(ds)

            conn.Close()
            conn.Open()

            Dim cmd2 As New Data.SqlClient.SqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select * from  batches where uploaded_by =@name"
            cmd2.Parameters.AddWithValue("@name", Session("name"))

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd2.ExecuteReader
            If rs.HasRows Then
                DataGrid1.DataSource = ds
                DataGrid1.DataBind()

            Else
                Label3.Text = "You do not have any uploads in your history"
            End If
            conn.Close()
        End If

    End Sub

    Function getbid(bno As String) As String
        Return enc.TripleDESEncode(bno, "bulktra!@#")

    End Function

    Protected Sub DataGrid1_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles DataGrid1.PageIndexChanged
        DataGrid1.CurrentPageIndex = e.NewPageIndex
 Dim conn As New Data.SqlClient.SqlConnection(chk.sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand("select * from  batches where uploaded_by =@name order by batch_date desc", conn)
        cmd.Parameters.AddWithValue("@name", Session("name"))
        Dim da As New Data.SqlClient.SqlDataAdapter
        da.SelectCommand = cmd

        Dim ds As New Data.DataSet
        da.Fill(ds)

        DataGrid1.DataSource = ds
        DataGrid1.DataBind()
        conn.Close()

    End Sub

End Class
