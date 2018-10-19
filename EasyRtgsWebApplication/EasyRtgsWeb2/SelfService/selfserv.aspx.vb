Imports System.Data.SqlClient

Partial Class selfserv
    Inherits System.Web.UI.Page

    Protected Sub btnGetEntryForDate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGetEntryForDate.Click
        Try
            Dim dateString As String = txtDateUnpostedEntries.Text
            If Not String.IsNullOrEmpty(dateString) Then
                Dim entrydate As DateTime = Convert.ToDateTime(dateString)
                Session("entrydate") = entrydate
                sqlDsUnpostedTransactions.DataBind()
                gvUnpostedTransactions.DataBind()
            End If
        Catch ex As Exception
            Gadget.LogException(ex)
        End Try
    End Sub

    Protected Sub gvUnpostedTransactions_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvUnpostedTransactions.RowCommand
        lblResultStatus.Text = String.Empty

        Dim index As Integer = Convert.ToInt32(e.CommandArgument.ToString())

        Dim id As Long
        'Dim sn As Long
        Dim sn As String
        Dim SN_index As Long = 2 'This is the index of the columns that contains the SN.
        Dim Status_index As Long = 3
        Dim oldStatus As String = String.Empty
        Dim username As String = Convert.ToString(Session("uname")) 'get the username of the user currently logged on.
        'get the account number of the requesrt.
        Dim idColumn As Integer = 1
        Dim c As String = e.CommandSource.ToString()
        Dim commandName As String = e.CommandName.ToLower().Trim()
        Try
            If (c.Trim().ToLower().CompareTo("system.web.ui.webcontrols.button") = 0) Then
                id = Convert.ToInt64(gvUnpostedTransactions.DataKeys(index).Value)
                sn = Convert.ToString(gvUnpostedTransactions.Rows(index).Cells(SN_index).Text)
                oldStatus = Convert.ToString(gvUnpostedTransactions.Rows(index).Cells(Status_index).Text)

                Dim result As Boolean = False
                If commandName = "already posted" Then
                    result = MarkEntryAsAlreadyPosted(id, sn, username, oldStatus)
                ElseIf commandName = "post entry" Then
                    result = RepostEntry(id, sn, username, oldStatus)
                End If

                If result = True Then

                    lblResultStatus.Text = "<font color=blue>" & e.CommandName & " Action Successfully Performed.</font>"
                Else

                    lblResultStatus.Text = "<font color=red>" & e.CommandName & " Action Failed.</font>"
                End If
                gvUnpostedTransactions.DataBind()
            End If
        Catch ex As Exception
            Gadget.LogException(ex)
        End Try

    End Sub

    Public Function RepostEntry(ByVal entryID As Long, ByVal transactionID As String, ByVal userid As String, ByVal oldstatus As String) As Boolean
        Dim reposted As Boolean = False
        Dim sql As String = "UPDATE transacttemp2 set Status=@newstatus, TransRef=NULL where ID=@ID and TransactionID=@TransactionID and Status =@oldstatus"
        Dim newstatus As String = GetFreshStatus(oldstatus)
        Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddWithValue("@ID", entryID)
                cmd.Parameters.AddWithValue("@TransactionID", transactionID)
                cmd.Parameters.AddWithValue("@newstatus", newstatus)
                cmd.Parameters.AddWithValue("@oldstatus", oldstatus)
                Dim updateCount As Integer = cmd.ExecuteNonQuery()
                If updateCount >= 0 Then
                    reposted = True
                    Try
                        Dim logmessage As New StringBuilder

                        logmessage.Append("User: ")
                        logmessage.Append(userid)
                        logmessage.Append(" Marked Transaction[ID:")
                        logmessage.Append(entryID)
                        logmessage.Append(", SN: ")
                        logmessage.Append(transactionID)
                        logmessage.Append("]")
                        logmessage.AppendLine(" For Reposting as status {0}")
                        Dim esy As New easyrtgs()
                        Dim msg As String = String.Format(logmessage.ToString(), newstatus)
                        esy.createAudit(msg, DateTime.Now)

                    Catch ex As Exception

                    End Try

                Else
                    reposted = False
                End If
            Catch ex As Exception
                reposted = False
                Gadget.LogException(ex)
            End Try
        End Using
        Return reposted
    End Function
    Public Function MarkEntryAsAlreadyPosted(ByVal entryID As Long, ByVal TransactionID As String, ByVal userid As String, ByVal oldstatus As String) As Boolean
        Dim reposted As Boolean = False
        Dim newstatus As String = String.Empty
        newstatus = GetSuccessfulStatus(oldstatus)
        Dim sql As String = "UPDATE transacttemp2 set Status=@status where ID=@ID and TransactionID=@TransactionID and Status=@oldstatus"
        Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddWithValue("@ID", entryID)
                cmd.Parameters.AddWithValue("@TransactionID", TransactionID)
                cmd.Parameters.AddWithValue("@status", newstatus)
                cmd.Parameters.AddWithValue("@oldstatus", oldstatus)
                Dim updateCount As Integer = cmd.ExecuteNonQuery()
                If updateCount >= 0 Then
                    reposted = True
                    Try
                        Dim logmessage As New StringBuilder

                        logmessage.Append("User: ")
                        logmessage.Append(userid)
                        logmessage.Append(" Marked Transaction[ID:")
                        logmessage.Append(entryID)
                        logmessage.Append(", SN: ")
                        logmessage.Append(TransactionID)
                        logmessage.Append("]")
                        logmessage.AppendLine(" As {0}")
                        Dim msg As String = String.Format(logmessage.ToString(), newstatus)
                        Dim esy As New easyrtgs
                        esy.createAudit(msg, Now)

                    Catch ex As Exception
                        Gadget.LogException(ex)
                    End Try
                Else
                    reposted = False
                End If
            Catch ex As Exception
                reposted = False
                Gadget.LogException(ex)
            End Try
        End Using
        Return reposted
    End Function

    Private Function GetFreshStatus(ByVal oldstatus As String) As String
        Dim status As String = String.Empty
        If Not String.IsNullOrEmpty(oldstatus) Then
            If oldstatus.Trim().Contains("-") Then
                Dim parts() As String = oldstatus.Split("-")
                'Based on our current format for status, the status is {Fresh_status-Transaction_Status}. See Jobs and web app code in this same project
                If parts IsNot Nothing Then
                    If parts.Length > 1 Then
                        status = parts(0).Trim().ToLower()
                       
                    End If
                End If
            End If
        End If
        Return status
    End Function

    Private Function GetSuccessfulStatus(ByVal oldstatus As String) As String
        Dim status As String = String.Empty
        If Not String.IsNullOrEmpty(oldstatus) Then
            If oldstatus.Trim().Contains("-") Then
                Dim parts() As String = oldstatus.Split("-")
                'Based on our current format for status, the status is {Fresh_status-Transaction_Status}. See Jobs and web app code in this same project
                If parts IsNot Nothing Then
                    If parts.Length > 1 Then
                        status = parts(0).Trim().ToLower()
                        Select Case status
                            Case "pay"
                                Return "Paid"
                            Case "reverse"
                                Return "Reversed"
                            Case "authorize"
                                Return "Authorized"
                        End Select
                    End If
                End If
            End If
        End If
        Return status
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("entrydate") = DateTime.Now.ToString("dd-MMM-yyyy")
    End Sub
End Class
