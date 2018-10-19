
Partial Class swift
    Inherits System.Web.UI.Page
    Private esy As New easyrtgs
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not (Session("role") = "Administrator" Or Session("role") = "Treasury") Then
                Response.Redirect("error.aspx")

            End If

            Label1.Text = Session("name")
            Label2.Text = Session("role")

        Catch ex As Exception
            esy.Errorlog(ex.ToString, Now)


        End Try



    End Sub

    Protected Sub DataGrid2_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles DataGrid2.PageIndexChanged
        DataGrid2.CurrentPageIndex = e.NewPageIndex
        Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())
            conn.Open()
            Dim strsql As String = "select * from transactions where " & DropDownList1.SelectedValue & " like @out and status not in ('Authorized','Failed','Uploaded') order by id desc"
            Dim cmd As New Data.SqlClient.SqlCommand(strsql, conn)
            cmd.Parameters.AddWithValue("@out", "%" & TextBox1.Text.Trim & "%")
            Dim da As New Data.SqlClient.SqlDataAdapter(cmd)
            Dim ds As New Data.DataSet
            da.Fill(ds)
            DataGrid2.DataSource = ds
            DataGrid2.DataBind()
            DataGrid2.Focus()


        End Using

    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())
            conn.Open()
            Dim strsql As String = "select * from transactions where " & DropDownList1.SelectedValue & " like @out and status not in ('Authorized','Failed','Uploaded') order by id desc"
            Dim cmd As New Data.SqlClient.SqlCommand(strsql, conn)
            cmd.Parameters.AddWithValue("@out", "%" & TextBox1.Text.Trim & "%")
            Dim da As New Data.SqlClient.SqlDataAdapter(cmd)
            Dim ds As New Data.DataSet
            da.Fill(ds)
            DataGrid2.DataSource = ds
            DataGrid2.DataBind()
            DataGrid2.Focus()

        End Using

    End Sub

End Class
