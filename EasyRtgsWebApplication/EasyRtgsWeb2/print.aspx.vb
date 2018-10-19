Imports System.Data.SqlClient

Partial Class print
    Inherits System.Web.UI.Page
    Private esy As New easyrtgs
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("name") = "" Then

            Response.Redirect("default.aspx")

        Else
            Dim esy As New easyrtgs

            Dim conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from transactions where transactionID='" & Request.QueryString("tid") & "'"
            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader
            While rs.Read
                Label17.Text = Request.QueryString("tid")
                Label4.Text = rs("Customer_account").ToString
                Label5.Text = rs("Customer_name").ToString
                Label7.Text = FormatNumber(rs("amount").ToString, 2)
                Label8.Text = rs("remarks").ToString
                Label9.Text = rs("uploaded_by").ToString
                Label10.Text = rs("Uploaded_date").ToString
                Label11.Text = rs("Authorized_by").ToString
                Label15.Text = rs("Authorized_date").ToString
                Label12.Text = rs("Treasury_approval").ToString
                Label16.Text = FormatNumber(rs("charges").ToString, 2)

                Label13.Text = rs("Approved_date").ToString
                Label14.Text = "<img src='Instructions/" & Request.QueryString("tid") & ".jpg' width='500' />"

            End While

            conn.Close()

            LoadCustomerCareFields(Request.QueryString("tid"))

        End If
    End Sub
    Private Sub LoadCustomerCareFields(ByVal transactionID As String)
        Try
            Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())

                conn.Open()
                Dim cmd As New Data.SqlClient.SqlCommand
                cmd.Connection = conn
                cmd.CommandText = "select *  ,name from tblCustCareResponses,tblusers where tblCustCareResponses.custcareofficerUserid=tblusers.username and transactionID=@tid"
                cmd.Parameters.AddWithValue("@tid", transactionID)
                Dim reader As SqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    lblCustCareComment.Visible = True
                    'hypPrintCustomerCareComment.Visible = True
                    lblCustCareConfirmAvailability.Visible = False
                    While reader.Read()
                        If Not IsDBNull(reader("entrydate")) Then
                            lblCustCareConfirmationDate.Text = Convert.ToDateTime(reader("entrydate").ToString()).ToString("dd-MMM-yyyy")
                        End If

                        If Not IsDBNull(reader("name")) Then
                            lblCustCareOfficer.Text = reader("name").ToString()
                        End If

                        If Not IsDBNull(reader("reason")) Then
                            lblCustCareComment.Text = reader("reason").ToString()
                        End If

                        lblCustCareConfirmAvailability.Text = String.Empty
                        'hypPrintCustomerCareComment.NavigateUrl = hypPrintCustomerCareComment.NavigateUrl & "?transid=" & transactionID
                    End While
                Else
                    lblCustCareConfirmationDate.Text = "N/A"
                    lblCustCareOfficer.Text = "N/A"
                    lblCustCareConfirmAvailability.Visible = True
                    lblCustCareConfirmAvailability.Text = "N/A"
                    lblCustCareComment.Text = String.Empty
                    lblCustCareComment.Visible = False
                    'hypPrintCustomerCareComment.Visible = False
                End If
            End Using
        Catch ex As Exception
            Gadget.LogException(ex)
        End Try

    End Sub
End Class
