
Partial Class schedule
    Inherits System.Web.UI.Page
    Private chk2 As New bulktra
    Private enc As New CyclopsDES
    Private bno As String
    Private bnk As New bank.bank


    Function getname(cusnum As String, bracode As String) As String
        Dim bnk As New NewBanks.banks
        Dim ds As New Data.DataSet
        ds = bnk.getAccountFullInfo(cusnum)
        Return ds.Tables(0).Rows(0).Item("CUS_SHO_NAME").ToString()

        'Return bnk.DATCONV_getCustomerName(cusnum, bracode)
    End Function

    Function getimalname(cusnum As String, bracode As String) As String
        Dim bnk As New NewImal.Service
        Dim ds As New Data.DataSet

        ds = bnk.GetAccountByAccountNumber(cusnum)
        Return ds.Tables(0).Rows(0).Item("CUSTOMERNAME").ToString()

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            bno = enc.TripleDESDecode(Request.QueryString("bid").Replace(" ", "+"), "bulktra!@#")
            If bno.Contains("'") Or bno.Contains("--") Then
                Session.Abandon()

                Response.Redirect("default.aspx")

            End If

            If (Session("name") = "" And Session("type") <> "Inputter") Then
                Response.Redirect("default.aspx")

            Else

                label2.Text = "Welcome " & Session("name") & " |  " & Session("type") & "  |    <a href='main.aspx'>Home</a>   |   <a href='logout.aspx'>Logout</a>"

                Dim conn As New Data.SqlClient.SqlConnection(chk2.sqlconn())
                conn.Open()
                Dim da As New Data.SqlClient.SqlDataAdapter
                Dim cmd2 As New Data.SqlClient.SqlCommand
                cmd2.Connection = conn
                cmd2.CommandText = "select * from  trans where batch_id=@bid"
                cmd2.Parameters.AddWithValue("@bid", bno)
                da.SelectCommand = cmd2

                Dim ds As New Data.DataSet
                da.Fill(ds)
                conn.Close()

                conn.Open()

                Dim cmd As New Data.SqlClient.SqlCommand
                cmd.Connection = conn
                cmd.CommandText = "select * from  trans where  batch_id=@bid"
                cmd.Parameters.AddWithValue("@bid", bno)

                Dim rs As Data.SqlClient.SqlDataReader
                rs = cmd.ExecuteReader
                If rs.HasRows Then

                    Label3.Text = "Contents of Batch [" & bno & "]"
                    DataGrid1.DataSource = ds
                    DataGrid1.DataBind()

                Else
                    Label3.Text = "This batch is empty"
                End If
                conn.Close()
            End If
        Catch ex As Exception

            chk2.createErrorLog(ex.Message, Now)


        End Try

    End Sub

   
    Sub export()
        export2(Session("bno") & ".xls", DataGrid1)
       
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
       
    End Sub

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
        export()

    End Sub
End Class
