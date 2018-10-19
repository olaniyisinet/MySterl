
Partial Class swift_full
    Inherits System.Web.UI.Page
    Private status As String = ""
    Private esy As New easyrtgs

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Button2.Visible = False
    

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("name") = "" Then

            Response.Redirect("default.aspx")

        Else
            If Session("role") = "Administrator" Then
                Button2.Visible = True
               
            End If

            If Session("role") = "Treasury" Then
                Button2.Visible = True

            End If

            If Session("role") = "Inputter" Then
                  Button2.Visible = False
             
            End If

            If Session("role") = "Authorizer" Then
                Button2.Visible = False

            End If


            'If Not esy.getSessionStatus(Session("uname"), Session.SessionID) = True Then
            '    Session.Abandon()
            '    Response.Redirect("default.aspx")

            'End If


            If Request.QueryString("tid") Is Nothing Then

                Response.Redirect("main.aspx")

            End If

 
                If esy.getTxnStatus(Request.QueryString("tid")) = "Failed" Or esy.getTxnStatus(Request.QueryString("tid")) = "Discarded" Then

                    Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "This Transaction Failed or Discarded" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                Button2.Visible = False

                End If


               

                If esy.getTxnStatus(Request.QueryString("tid")) = "Authorized" Then

                Button2.Visible = False


            End If

            If esy.getTxnStatus(Request.QueryString("tid")) = "Uploaded" Then

                Button2.Visible = False


            End If

            If esy.getTxnStatus(Request.QueryString("tid")) = "SwiftReady" Then

                Button2.Visible = False


            End If
            If esy.getTxnStatus(Request.QueryString("tid")) = "SwiftInProcess" Then

                Button2.Visible = False


            End If
            If esy.getTxnStatus(Request.QueryString("tid")) = "SwiftComplete" Then

                Button2.Visible = False


            End If

            End If

            Label1.Text = Session("name")
            Label3.Text = Request.QueryString("tid")


            Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())

                conn.Open()
                Dim cmd As New Data.SqlClient.SqlCommand
                cmd.Connection = conn
                cmd.CommandText = "select * from transactions where transactionID=@tid"
                cmd.Parameters.AddWithValue("@tid", Request.QueryString("tid"))

                Dim rs As Data.SqlClient.SqlDataReader
                rs = cmd.ExecuteReader
                While rs.Read
                    Label20.Text = Request.QueryString("tid")

                    Label4.Text = rs("Customer_account").ToString
                    Label5.Text = rs("Customer_name").ToString
                    Label7.Text = FormatNumber(rs("amount").ToString, 2)
                    Label8.Text = rs("remarks").ToString
                    Label9.Text = rs("uploaded_by").ToString
                    Label10.Text = Convert.ToDateTime(rs("Uploaded_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
                    Label11.Text = rs("Authorized_by").ToString

                    If Not rs("Authorized_date").ToString = String.Empty Then
                        Label15.Text = Convert.ToDateTime(rs("Authorized_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
                    End If

                    Label12.Text = rs("Treasury_approval").ToString
                    Label17.Text = FormatNumber(rs("charges").ToString, 2)

                    If Not rs("Approved_date").ToString = String.Empty Then
                        Label13.Text = Convert.ToDateTime(rs("Approved_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
                    End If

                    Label14.Text = "<br><a href='Instructions/" & Request.QueryString("tid") & "." & rs("Instruction").ToString & "' target='_blank'>Download Mandate here [right click and Save As]</a><br/><br/><embed src='Instructions/" & Request.QueryString("tid") & "." & rs("Instruction").ToString & "' width='600' height='600' />"

                    Label18.Text = rs("beneficiary").ToString
                    Label19.Text = rs("beneficiary_bank").ToString



                End While

            End Using

 

    End Sub






     

    Sub reverseAllEntries(ByVal sender As Object, ByVal e As System.EventArgs)

        If esy.UpdatetxnForReversal(Server.HtmlEncode(Request.QueryString("tid"))) = "Reverse" Then
            Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='370'>" & "Transaction has been marked for reversal." & "</td><td valign='middle' width='30'><img src='images\up.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Button2.Visible = False

            esy.InsertLog(Session("name") & " Marked transaction with REF: " & Request.QueryString("tid") & " for reversal of entries on " & Now)

        Else

            Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction could not be reversed now." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub

        End If


    End Sub
     


    Sub swiftPush(ByVal sender As Object, ByVal e As System.EventArgs)

        If esy.UpdatetxnAsSwiftReady(Server.HtmlEncode(Request.QueryString("tid"))) = "SwiftReady" Then
            Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='370'>" & "Transaction has been pushed to Swift." & "</td><td valign='middle' width='30'><img src='images\up.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Button2.Visible = False

            esy.InsertLog(Session("name") & " Pushed transaction with REF: " & Request.QueryString("tid") & " to SWIFT on " & Now)

        Else

            Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction could not be completed now." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub

        End If


    End Sub




     
End Class
