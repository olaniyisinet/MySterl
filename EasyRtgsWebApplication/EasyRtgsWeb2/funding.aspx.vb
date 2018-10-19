
Partial Class funding
    Inherits System.Web.UI.Page

    Sub export()
        export2("Exported" & ".xls", DataGrid1)
        Button2.Text = "Export to Excel"

    End Sub

    Sub export2(ByVal fn As String, ByVal dg As DataGrid)
        Response.ClearContent()
        Response.AddHeader("content-disposition", "attachment; filename=" & Server.MapPath("./temp2/exported_" & fn))
        Response.ContentType = "application/excel"
        Dim sw As New System.IO.StringWriter()
        Dim htw As New HtmlTextWriter(sw)
        dg.RenderControl(htw)
        Response.Write(sw.ToString())
        Response.[End]()
        Button2.Text = "Export to Excel"

    End Sub

    Protected Sub Button2_Click(sender As Object, e As System.EventArgs) Handles Button2.Click
        export()
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Session("role") = "Treasury" Then
        Else
            Response.Redirect("main.aspx")

        End If
    End Sub

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
        'Dim esy As New easyrtgs

        'Dim conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())
        'conn.Open()

        'Dim cmd As New Data.SqlClient.SqlCommand
        'cmd.Connection = conn

        'cmd.CommandText = "select * from transactions where date >= @date1 and date <= @date2 and status like @status"
        'cmd.Parameters.AddWithValue("@date1", TextBox1.Text)
        'cmd.Parameters.AddWithValue("@date2", TextBox2.Text)

        'cmd.Parameters.AddWithValue("@status", "%" & DropDownList1.SelectedItem.Value & "%")

        'Dim da As New Data.SqlClient.SqlDataAdapter
        'da.SelectCommand = cmd



        'Dim ds As New Data.DataSet
        'da.Fill(ds)
        'DataGrid1.DataSource = ds
        'DataGrid1.DataBind()
        'conn.Close()

        'Label5.Text = "From: " & TextBox1.Text & " , To: " & TextBox2.Text & " ,  " & ds.Tables(0).Rows.Count & " Records"
        Dim esy As New easyrtgs

        Dim conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())
        conn.Open()

        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        Dim sqlBuilder As New StringBuilder()

        Dim sql As String = String.Empty
        '"select * from transactions where date >= @date1 and date <= @date2"
        'sql = " and status like @status"
        sqlBuilder.Append("select * from transactions where date >= @date1 and date <= @date2")
        If DropDownList1.SelectedIndex <> 0 Then

            sqlBuilder.Append(" and status like @status")
            cmd.Parameters.AddWithValue("@status", "%" & DropDownList1.SelectedItem.Value & "%")
        End If

        sql = sqlBuilder.ToString()

        cmd.CommandText = sql

        cmd.Parameters.AddWithValue("@date1", TextBox1.Text)
        cmd.Parameters.AddWithValue("@date2", TextBox2.Text)



        Dim da As New Data.SqlClient.SqlDataAdapter
        da.SelectCommand = cmd



        Dim ds As New Data.DataSet
        da.Fill(ds)
        DataGrid1.DataSource = ds
        DataGrid1.DataBind()
        conn.Close()

        Label5.Text = "From: " & TextBox1.Text & " , To: " & TextBox2.Text & " ,  " & ds.Tables(0).Rows.Count & " Records"



    End Sub
End Class
