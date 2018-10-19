Imports System.Data
Imports BankCore.t24
Imports BankCore
Imports System.Data.SqlClient

Partial Class full
    Inherits System.Web.UI.Page
    Private status As String = ""
    Private esy As New easyrtgs

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Button2.Visible = False
        Button3.Visible = False
        Button4.Visible = False

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("name") = "" Then

            Response.Redirect("default.aspx")

        Else
            If Session("role") = "Administrator" Then
                Button1.Visible = False
                Button2.Visible = False
                Button3.Visible = False

            End If

            If Session("role") = "Inputter" Then
                Button1.Visible = False
                Button2.Visible = False
                Button3.Visible = False

            End If




            If Request.QueryString("tid") Is Nothing Then

                Response.Redirect("main.aspx")

            End If


            If Session("role") = "Authorizer" Then
                If Not (esy.getTxnStatus(Request.QueryString("tid")) = "Authorized" Or esy.getTxnStatus(Request.QueryString("tid")) = "Approved" Or esy.getTxnStatus(Request.QueryString("tid")) = "Paid" Or esy.getTxnStatus(Request.QueryString("tid")) = "Discarded" Or esy.getTxnStatus(Request.QueryString("tid")) = "Failed" Or esy.getTxnStatus(Request.QueryString("tid")) = "Pay") Then
                    Button1.Text = "Authorize Transaction"
                    Button3.Visible = True
                    status = "Authorized"
                    Label16.ForeColor = Drawing.Color.Red

                    Label16.Text = "Note that clicking the [Authorize] button will Debit the customer's account to credit Treasury Ops."
                Else
                    Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction has been Authorized" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                    Button1.Visible = False

                End If
            End If

            If Session("role") = "Treasury" Then
                If esy.getTxnStatus(Request.QueryString("tid")) = "Paid" Then

                    Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction has been processed" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                    Button2.Visible = False
                    'Button3.Visible = True
                    Button3.Visible = False 'The discard button should not be visible when the Treasury user is logged on.

                End If


                If esy.getTxnStatus(Request.QueryString("tid")) = "Pay" Then

                    Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction is awaiting processing." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                    Button2.Visible = False
                    Button3.Visible = True


                End If


                If esy.getTxnStatus(Request.QueryString("tid")) = "Failed" Or esy.getTxnStatus(Request.QueryString("tid")) = "Discarded" Then

                    Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "This Transaction Failed or Discarded" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                    Button2.Visible = True
                    Button1.Visible = False
                End If


                If Not esy.getTxnStatus(Request.QueryString("tid")) = "Approved" Then

                    Button1.Text = "Approve Transaction"
                    status = "Approved"
                Else
                    Button2.Visible = True
                    Button1.Visible = False

                    Label16.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "This Transaction has been Approved. Click [Pay] button to complete Transaction." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"

                End If

                If esy.getTxnStatus(Request.QueryString("tid")) = "Authorized" Then
                    Button4.Visible = True 'button for reversals
                    '                    Button3.Visible = True
                    Button3.Visible = False 'Discarding a request should not be done on the Treasury.

                End If
            End If

            Label1.Text = Session("name")
            Label3.Text = Request.QueryString("tid")
            Dim transid As String = Request.QueryString("tid").ToString()
            LoadDetailsForm(transid)

            LoadSwiftMessage(transid)
            LoadAccountEntries(transid)
        End If


    End Sub






    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        'New code. Ensure that the token is tested before proceeding
        lblTokenError.Text = String.Empty
        Dim token As New TokenValidatorUtil()
        Dim tokenResponse As TokenResponseHolder = Nothing
        Dim appMode As String = ConfigurationManager.AppSettings("APP_MODE").ToString().Trim().ToLower()
        If appMode = "online" Then
            tokenResponse = token.ValidateToken(txtUsername.Text, txtToken.Text)
            If Not tokenResponse.IsValid Then
                lblTokenError.Text = tokenResponse.ResponseMessage
                Exit Sub
            End If
        End If



        Label1.Text = ans.Value
        Dim transidval As String = String.Empty
        transidval = Request.QueryString("tid").ToString()
        Dim rtgsObj As New EasyRtgTransaction(transidval)
        Dim amount As String = Convert.ToDecimal(rtgsObj.amount).ToString("0,000.00")

        Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())

            If ans.Value.Contains("rue") Then

                Dim trans As Data.SqlClient.SqlTransaction = Nothing
                Using conn
                    conn.Open()

                    Try
                        Using trans
                            Dim cmd As New Data.SqlClient.SqlCommand

                            cmd.Connection = conn


                            If Session("role") = "Authorizer" Then
                                trans = conn.BeginTransaction()
                                cmd.Transaction = trans
                                Dim isTransactionsUpdateOk As Boolean = False
                                Dim isTransactTemp2UdateOk As Boolean = False
                                cmd.CommandText = "update transactions set Authorized_by=@name, authorized_date=@date, status='Authorize'  where transactionID=@tid"
                                cmd.Parameters.AddWithValue("@name", Session("name"))
                                cmd.Parameters.AddWithValue("@date", Now.ToString("yyyy-MM-dd hh:mm tt"))
                                cmd.Parameters.AddWithValue("@tid", Server.HtmlEncode(Request.QueryString("tid")))
                                If cmd.ExecuteNonQuery() > 0 Then
                                    isTransactionsUpdateOk = True
                                End If
                                cmd.Parameters.Clear()
                                'audit trail
                                esy.createAudit(Request.QueryString("tid") & " was submitted for authorization by " & Session("name"), Now)
                                esy.TransactionAudit(Request.QueryString("tid") & " was submitted for authorization by " & Session("name"), Now, Request.QueryString("tid"))

                                cmd.CommandText = "update TransactTemp2 set Status='Authorize' where TransactionID=@tid"
                                cmd.Parameters.AddWithValue("@tid", Server.HtmlEncode(Request.QueryString("tid")))
                                If cmd.ExecuteNonQuery() > 0 Then
                                    isTransactTemp2UdateOk = True
                                End If

                                If isTransactionsUpdateOk And isTransactTemp2UdateOk Then
                                    Try
                                        trans.Commit()
                                        Button1.Visible = False
                                        Button3.Visible = True
                                    Catch ex As Exception
                                        trans.Rollback()
                                    End Try

                                    Response.Redirect("payResponse.aspx")
                                Else
                                    trans.Rollback()
                                End If


                            End If

                            If Session("role") = "Treasury" Then

                                cmd.CommandText = "update transactions set Treasury_Approval=@name, approved_date=@date, status='Approved'  where transactionID=@tid"
                                cmd.Parameters.AddWithValue("@name", Session("name"))
                                cmd.Parameters.AddWithValue("@date", Now.ToString("yyyy-MM-dd hh:mm tt"))
                                cmd.Parameters.AddWithValue("@tid", Server.HtmlEncode(Request.QueryString("tid")))


                                If cmd.ExecuteNonQuery() > 0 Then
                                    cmd.Parameters.Clear()
                                    Dim isPaymentSetup As Boolean = SetupPaymentEntries(Server.HtmlEncode(Request.QueryString("tid")))
                                    If isPaymentSetup Then
                                        Try
                                            gvAccountingEntries.DataBind()
                                        Catch ex As Exception
                                            Gadget.LogException(ex)
                                        End Try

                                    End If


                                Else
                                    trans.Rollback()
                                End If



                                Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction was Approved" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"


                                'audit trail
                                esy.createAudit(Request.QueryString("tid") & " was approved by " & Session("name"), Now)
                                esy.TransactionAudit(Request.QueryString("tid") & " was approved by " & Session("name"), Now, Request.QueryString("tid"))


                                'notify Treasury and HOP
                                Dim body As String = ""
                                body = "Sir/Madam," & "<br>" & "<br>" & "This mail is to notify you that this Transaction has been Approved. Details of this transaction are as follows;" & "<br>" & _
                                "Customer: " & Label5.Text & "<br>" & "Amount: " & amount & "<br>" & "Account Number: " & Label4.Text & "<br><br>Thank You"

                                Try
                                    Dim mail As New easyrtgs
                                    Dim host As String = System.Configuration.ConfigurationManager.AppSettings("mailHost")
                                    Dim toaddr As String = mail.getAuthorizerEmailbyName(Label11.Text)
									Dim ccAddr As String = mail.getTreasuryMail(mail.checkisIMAL(Request.QueryString("tid")))
									If String.IsNullOrEmpty(toaddr) Then
                                        'Get the name of the uploader
                                        Dim r As New EasyRtgTransaction(Request.QueryString("tid"))
                                        toaddr = mail.getAuthorizerEmailbyName(r.Uploaded_by)
                                        If String.IsNullOrEmpty(toaddr) Then
                                            toaddr = ccAddr
                                        End If
                                    End If
                                    Dim subject As String = "3RD PARTY TRANSACTION APPROVAL (TRANSACTION ID: " & Request.QueryString("tid") & ")"
                                    mail.sendmail(host, toaddr, ccAddr, subject, body)

                                Catch ex2 As Exception
                                    Gadget.LogException(ex2)
                                End Try
                                'pay button visible
                                Button2.Visible = True
                                Button1.Visible = False

                            End If
                        End Using
                    Catch ex As Exception

                    End Try

                End Using




            End If

        End Using


    End Sub


    Sub btn1(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim transid As String = Request.QueryString("tid").ToString()
        Dim amount As String = String.Empty
        Dim rtgs As New EasyRtgTransaction(transid)
        amount = rtgs.amount

        Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())

            If ans.Value = "True" Then

                conn.Open()
                Dim cmd As New Data.SqlClient.SqlCommand
                cmd.Connection = conn
                If Session("role") = "Authorizer" Then
                    cmd.CommandText = "update transactions set Authorized_by=@name, authorized_date=@date, status='Authorize'  where transactionID=@tid"
                    cmd.Parameters.AddWithValue("@name", Session("name"))
                    cmd.Parameters.AddWithValue("@date", Now.ToString("yyyy-MM-dd hh:mm tt"))
                    cmd.Parameters.AddWithValue("@tid", Server.HtmlEncode(Request.QueryString("tid")))
                    cmd.ExecuteNonQuery()
                    'audit trail
                    esy.createAudit(Request.QueryString("tid") & " was submitted for authorization by " & Session("name"), Now)
                    esy.TransactionAudit(Request.QueryString("tid") & " was submitted for authorization by " & Session("name"), Now, Request.QueryString("tid"))


                    Button1.Visible = False
                    Button3.Visible = True
                    Response.Redirect("payResponse.aspx")


                    ' Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction was Authorized" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"

                End If

                If Session("role") = "Treasury" Then
                    cmd.CommandText = "update transactions set Treasury_Approval=@name, approved_date=@date, status='Approved'  where transactionID=@tid"
                    cmd.Parameters.AddWithValue("@name", Session("name"))
                    cmd.Parameters.AddWithValue("@date", Now.ToString("yyyy-MM-dd hh:mm tt"))
                    cmd.Parameters.AddWithValue("@tid", Server.HtmlEncode(Request.QueryString("tid")))


                    cmd.ExecuteNonQuery()


                    Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction was Approved" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"


                    'audit trail
                    esy.createAudit(Request.QueryString("tid") & " was approved by " & Session("name"), Now)
                    esy.TransactionAudit(Request.QueryString("tid") & " was approved by " & Session("name"), Now, Request.QueryString("tid"))


                    'notify Treasury and HOP
                    Dim body As String = ""
                    body = "Sir/Madam," & "<br>" & "<br>" & "This mail is to notify you that this Transaction has been Approved. Details of this transaction are as follows;" & "<br>" & _
                    "Customer: " & Label5.Text & "<br>" & "Amount: " & amount & "<br>" & "Account Number: " & Label4.Text & "<br><br>Thank You"

                    Try
                        Dim mail As New easyrtgs
						mail.sendmail(System.Configuration.ConfigurationManager.AppSettings("mailHost"), mail.getAuthorizerEmailbyName(Label11.Text), mail.getTreasuryMail(mail.checkisIMAL(Request.QueryString("tid"))), "3RD PARTY TRANSACTION APPROVAL (TRANSACTION ID: " & Request.QueryString("tid") & ")", body)

					Catch ex2 As Exception
                    End Try
                    'pay button visible
                    Button2.Visible = True
                    Button1.Visible = False

                End If

            End If


        End Using


    End Sub
    Sub reverseAllEntries(ByVal sender As Object, ByVal e As System.EventArgs)
        If String.IsNullOrEmpty(txtComment.Text) Then
            Label16.Text = "<font color=red>Please enter a comment for rejections.</font>"
            Exit Sub
        End If
        Dim comment As String = txtComment.Text

        If esy.SaveCommentForTransid(comment, Request.QueryString("tid")) Then
            Dim rtgs As New EasyRtgTransaction(Request.QueryString("tid"))
            Dim mtname As String = rtgs.MessageTypeName.ToLower()

            Dim isReversed As Boolean = False
            isReversed = SetupReversal(Server.HtmlEncode(Request.QueryString("tid")))
            'If mtname.Trim().ToLower().CompareTo(easyrtgs.MESSAGETYPE_MT202) = 0 Then
            '    'Change the status to Reversed on the TransactTemp2
            '    Dim sql As String = "update TransactTemp2 set Status='Reversed' where TransactionID=@tid"
            '    Dim cn As New Connect(sql)
            '    cn.addparam("@tid", rtgs.TransactionID)
            '    Dim i As Integer = cn.query()
            '    If i >= 0 Then
            '        isReversed = True

            '    End If
            'Else
            '    isReversed = SetupReversal(Server.HtmlEncode(Request.QueryString("tid")))

            'End If

            If isReversed Then
                Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='370'>" & "Transaction has been marked for reversal." & "</td><td valign='middle' width='30'><img src='images\up.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                Button2.Visible = False
                Try
                    esy.InsertLog(Session("name") & " Marked transaction with REF: " & Request.QueryString("tid") & " for reversal of entries on " & Now)
                Catch ex As Exception

                End Try
                Response.Redirect("mytransactions.aspx")
            Else

                Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction could not be reversed now." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                Exit Sub
            End If

        Else
            Label16.Text = "<font color=red>Could not save the comment for the reversal. Reversal failed!</font>"
        End If

    End Sub
    'Sub reverseAllEntries(ByVal sender As Object, ByVal e As System.EventArgs)
    '    If String.IsNullOrEmpty(txtComment.Text) Then
    '        Label16.Text = "<font color=red>Please enter a comment for rejections.</font>"
    '        Exit Sub
    '    End If
    '    Dim comment As String = txtComment.Text

    '    If esy.SaveCommentForTransid(comment, Request.QueryString("tid")) Then
    '        Dim rtgs As New EasyRtgTransaction(Request.QueryString("tid"))
    '        Dim mtname As String = rtgs.MessageTypeName.ToLower()

    '        Dim isReversed As Boolean = False
    '        If mtname.Trim().ToLower().CompareTo(easyrtgs.MESSAGETYPE_MT202) = 0 Then
    '            'Change the status to Reversed on the TransactTemp2
    '            Dim sql As String = "update TransactTemp2 set Status='Reversed' where TransactionID=@tid"
    '            Dim cn As New Connect(sql)
    '            cn.addparam("@tid", rtgs.TransactionID)
    '            Dim i As Integer = cn.query()
    '            If i >= 0 Then
    '                isReversed = True

    '            End If
    '        Else
    '            isReversed = SetupReversal(Server.HtmlEncode(Request.QueryString("tid")))

    '        End If

    '        If isReversed Then
    '            Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='370'>" & "Transaction has been marked for reversal." & "</td><td valign='middle' width='30'><img src='images\up.png' width='20' title='close' onclick='hide();'></td></tr></table>"
    '            Button2.Visible = False
    '            Try
    '                esy.InsertLog(Session("name") & " Marked transaction with REF: " & Request.QueryString("tid") & " for reversal of entries on " & Now)
    '            Catch ex As Exception

    '            End Try
    '            Response.Redirect("mytransactions.aspx")
    '        Else

    '            Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction could not be reversed now." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
    '            Exit Sub
    '        End If

    '    Else
    '        Label16.Text = "<font color=red>Could not save the comment for the reversal. Reversal failed!</font>"
    '    End If

    'End Sub

    Sub complete(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim isValid As Boolean = False
        Dim username As String = String.Empty
        Dim token As String = String.Empty
        username = txtUsername.Text
        token = txtToken.Text
        Label16.Text = String.Empty

        Dim tokenVal As New TokenValidatorUtil()


        If ConfigurationManager.AppSettings("APP_MODE").Trim().ToLower() <> "offline" Then
            Dim tokenResponse As TokenResponseHolder = tokenVal.ValidateToken(username, token)
            If Not tokenResponse.IsValid Then
                Label16.Text = String.Format("The token authentication failed. Error message: {0}", tokenResponse.ResponseMessage)
                Exit Sub
            End If
        End If


        If esy.UpdatetxnAsPay(Server.HtmlEncode(Request.QueryString("tid"))) = "Pay" Then
            Dim url As String = String.Empty
            url = String.Format("PayResponse.aspx")
            Server.Transfer(url)
            'Update the Supervisor Column
        Else
            Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction could not be completed now." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub
        End If

    End Sub




    Protected Sub Button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button3.Click
        If String.IsNullOrEmpty(txtComment.Text) Then
            Label16.Text = "<font color=red>Please enter a comment for rejections.</font>"
            Exit Sub
        End If
        Dim comment As String = txtComment.Text
        If esy.SaveCommentForTransid(comment, Request.QueryString("tid")) Then
            If Session("role").ToString().Trim().ToLower <> easyrtgs.ROLE_TREASURY.Trim().ToLower() Then
                esy.UpdatetxnAsDiscarded(Request.QueryString("tid"), reason.Value)
                esy.TransactionAudit(Request.QueryString("tid") & " was discarded by " & Session("name"), Now, Request.QueryString("tid"))

                'Label16.Text = "<table id='output' bgcolor='#ccffcc' width='600'><tr><td valign='middle' width='570'>" & "Transaction was Discarded - [Please reverse the money/charges back to the customer's account]" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                Button3.Visible = False
            Else
                Label16.Text = "<font color=red> Rejection failed because a Treasury user should not perform this action!</font>"
            End If

        Else
            Label16.Text = "<font color=red>Could not save the comment for the rejection. Rejection failed!</font>"
        End If



    End Sub

    Private Function ConvertToBankName(ByVal p1 As String) As String
        Dim gadgetObj As New Gadget
        Return gadgetObj.GetBankNameFromCode(p1)
    End Function
    Private Function SetupReversal(ByVal transid As String) As Boolean
        Dim t24 As New T24Bank()
        Dim acct As IAccount = Nothing

        Dim isReversed As Boolean = False
        If esy.UpdatetxnForReversal(Server.HtmlEncode(transid)) = "Reverse" Then
            'Get the data about this transaction using the transaction id transid
            'Dim rtgsObj As New EasyRtgTransaction(Convert.ToInt64(Server.HtmlEncode(transid)))
            Dim rtgsObj As New EasyRtgTransaction(Server.HtmlEncode(transid))
            If rtgsObj.Customer_account.Length = 10 Then

                Dim ds As DataSet = New DataSet
                Dim nuban As String = String.Empty 'srv.getAccountByNUBAN(rtgsObj.Customer_account)
                nuban = rtgsObj.Customer_account
                If Not String.IsNullOrEmpty(nuban) Then
                    acct = t24.GetAccountInfoByAccountNumber(nuban)

                End If

            End If
            Dim isStaff As Boolean = False 'IIf(rtgsObj.Customer_account.Trim().Split("-"c)(3) = "9", True, False)
            Dim vatRate As Decimal = 0.05D

            If Not String.IsNullOrEmpty(acct.AccountType) Then
                isStaff = acct.AccountType.Trim().ToLower().Contains("staff")
            Else
                Throw New Exception("Could not determine whether the customer account number is a staff account because the AccountType field is empty or NULL.")
            End If


            Dim conn As Data.SqlClient.SqlConnection = New Data.SqlClient.SqlConnection(esy.sqlconn1())
            Dim isSavedPrincipal As Boolean = False
            Dim isSavedCharges As Boolean = False
            Dim issavedVat As Boolean = False
            Using conn
                conn.Open()
                Dim trans As Data.SqlClient.SqlTransaction = conn.BeginTransaction
                Using trans
                    Dim sql As String = "spSavePostingEntries"
                    Dim cmdInsertTransactTemp As New Data.SqlClient.SqlCommand(sql, conn)

                    cmdInsertTransactTemp.Transaction = trans
                    Try
                        cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
                        cmdInsertTransactTemp.Parameters.AddWithValue("@transid", transid)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(rtgsObj.Customer_name.Trim.Replace("'", "`")))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(rtgsObj.amount, 2))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", rtgsObj.Uploaded_by)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", rtgsObj.Uploaded_by)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getRTGSSuspense(esy.checkisIMAL(transid)))
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", rtgsObj.Customer_account)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(transid & "-" & rtgsObj.Remarks.Trim.Replace("'", "`").Replace("&", "-")))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", rtgsObj.Branch)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Status", "Reverse")



                        If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
                            isSavedPrincipal = True
                        End If

                        cmdInsertTransactTemp.Parameters.Clear()

                        'Save the charges
                        cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
                        cmdInsertTransactTemp.Parameters.AddWithValue("@transid", transid)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(rtgsObj.Customer_name.Trim.Replace("'", "`")))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(rtgsObj.charges, 2))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", rtgsObj.Uploaded_by)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", rtgsObj.Uploaded_by)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getRTGSSuspense(esy.checkisIMAL(transid)))
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", rtgsObj.Customer_account)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(transid & "-" & rtgsObj.Remarks.Trim.Replace("'", "`").Replace("&", "-")))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", rtgsObj.Branch)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Status", "Reverse")

                        If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
                            isSavedCharges = True

                        End If

                        cmdInsertTransactTemp.Parameters.Clear()

                        'Save the charges
                        cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
                        cmdInsertTransactTemp.Parameters.AddWithValue("@transid", transid)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(rtgsObj.Customer_name.Trim.Replace("'", "`")))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(rtgsObj.charges, 2))) * vatRate)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", rtgsObj.Uploaded_by)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", rtgsObj.Uploaded_by)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getRTGSSuspense(esy.checkisIMAL(transid)))
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", rtgsObj.Customer_account)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(transid & "-" & rtgsObj.Remarks.Trim.Replace("'", "`").Replace("&", "-")))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", rtgsObj.Branch)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Status", "Reverse")

                        If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
                            issavedVat = True

                        End If

                        If isSavedCharges And isSavedPrincipal And issavedVat Then
                            Try
                                trans.Commit()
                                isReversed = True
                            Catch ex As Exception
                                isReversed = False
                            End Try

                        Else
                            trans.Rollback()
                            isReversed = False
                        End If
                    Catch ex As Exception
                        trans.Rollback()
                        isReversed = False

                    End Try

                End Using

            End Using

        End If


        Return isReversed
    End Function
    'Private Function SetupPaymentEntries(ByVal transid As String) As Boolean
    '    Dim isSetup As Boolean = False
    '    Dim newStatus As String = "Approved"
    '    Dim vatRate As Decimal = 0.05D
    '    Dim rtgsObj As New EasyRtgTransaction(Server.HtmlEncode(transid))

    '    'Check if the type of the transaction is for MT202 and do something else
    '    Dim msgTypeId As Integer = rtgsObj.MessagetypeID

    '    Dim msgTypeName As String = rtgsObj.MessageTypeName
    '    Dim isMt202 As Boolean = False
    '    Dim isMt103ByTrops As Boolean = False
    '    Select Case msgTypeName.Trim().ToLower()
    '        Case "mt202"
    '            isMt202 = True
    '        Case Else
    '            If msgTypeName.Trim().ToLower().CompareTo(easyrtgs.MESSAGETYPE_MT103.ToLower().Trim()) = 0 Then
    '                isMt103ByTrops = True
    '            End If
    '            isMt202 = False
    '    End Select



    '    Dim supervisorName As String = Session("name")


    '    isSetup = createAccountingEntries(rtgsObj, newStatus, transid, supervisorName)






    '    Return isSetup
    'End Function
    Private Function SetupPaymentEntries(ByVal transid As String) As Boolean
        Dim isSetup As Boolean = False
        Dim newStatus As String = "Approved"
        Dim vatRate As Decimal = 0.05D
        Dim rtgsObj As New EasyRtgTransaction(Server.HtmlEncode(transid))

        'Check if the type of the transaction is for MT202 and do something else
        Dim msgTypeId As Integer = rtgsObj.MessagetypeID

        Dim msgTypeName As String = rtgsObj.MessageTypeName
        Dim isMt202 As Boolean = False
        Dim isMt103ByTrops As Boolean = False
        Select Case msgTypeName.Trim().ToLower()
            Case "mt202"
                isMt202 = True
            Case Else
                If msgTypeName.Trim().ToLower().CompareTo(easyrtgs.MESSAGETYPE_MT103.ToLower().Trim()) = 0 Then
                    isMt103ByTrops = True
                End If
                isMt202 = False
        End Select



        Dim supervisorName As String = Session("name")

        ''22-April-2018. Since we are also crediting the suspense first even for TROPS-initiated transactions, then let's set up
        '' accounting entries nevertheless. Hence we are commenting out the lines below.
        'If Not isMt103ByTrops Then
        '    isSetup = createAccountingEntries(rtgsObj, newStatus, transid, supervisorName)
        'End If
        isSetup = createAccountingEntries(rtgsObj, newStatus, transid, supervisorName)

        Return isSetup
    End Function
    Private Sub LoadDetailsForm(ByVal transactionID As String)

        lblTransactionType.Text = ""
        Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())

            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from transactions where transactionID=@tid"

            cmd.Parameters.AddWithValue("@tid", transactionID)

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader
            While rs.Read
                If Not IsDBNull(rs("Status")) Then
                    Dim status As String = String.Empty
                    status = rs("Status").ToString()
                    If status.Trim().ToLower().CompareTo("pay") = 0 OrElse status.Trim().ToLower().CompareTo("approve") = 0 Then
                        Server.Transfer("logout.aspx") 'You are trying to approve something that has already been approved or paid.
                    End If
                End If
                Label20.Text = transactionID
                lblHasFinancialEntry.Text = IIf(Not IsDBNull(rs("HasFinancialEntry")), IIf(Convert.ToBoolean(rs("HasFinancialEntry")), "Yes", "No"), "Yes")
                Label4.Text = rs("Customer_account").ToString
                Label5.Text = rs("Customer_name").ToString
                Label7.Text = FormatNumber(rs("amount").ToString, 2)
                Label8.Text = rs("remarks").ToString()
                Label9.Text = rs("uploaded_by").ToString()
                Label10.Text = Convert.ToDateTime(rs("Uploaded_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
                Label11.Text = rs("Authorized_by").ToString

                If Not rs("Authorized_date").ToString = String.Empty Then
                    Label15.Text = Convert.ToDateTime(rs("Authorized_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
                Else
                    Label15.Text = "N/A"
                End If

                Label12.Text = rs("Treasury_approval").ToString
                Label17.Text = FormatNumber(rs("charges").ToString, 2)

                If Not rs("Approved_date").ToString = String.Empty Then
                    Label13.Text = Convert.ToDateTime(rs("Approved_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
                Else
                    Label13.Text = "N/A"
                End If

                Label14.Text = "<br><a href='Instructions/" & transactionID & "." & rs("Instruction").ToString & "' target='_blank'>Download Mandate here [right click and Save As]</a><br/><br/><embed src='Instructions/" & transactionID & "." & rs("Instruction").ToString & "' width='600' height='600' />"

                Label18.Text = rs("beneficiary").ToString()
                Dim bankCode As String = rs("beneficiary_bank").ToString()
                Try

                    If bankCode.Contains(":") Then
                        bankCode = bankCode.Split(":")(1)
                    End If
                Catch ex As Exception

                End Try

                Label19.Text = ConvertToBankName(bankCode)
                lblBenAcctNum.Text = rs("Beneficiary_account").ToString()


                If Session("role") = "Authorizer" Then
                    Label11.Text = "N/A"
                    Label12.Text = "N/A"
                    Label13.Text = "N/A"
                    Label15.Text = "N/A"
                End If

                Dim messageTypeName As String = String.Empty
                If Not IsDBNull(rs("messagetypeID")) Then
                    Dim msgDet As New MtMessageDetails
                    messageTypeName = msgDet.GetMessageTypeNameByID(Convert.ToInt32(rs("messagetypeID").ToString()))
                    lblTransactionType.Text = messageTypeName
                Else
                    lblTransactionType.Text = "N/A"
                End If

                LoadCustomerCareFields(transactionID)

            End While

        End Using
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
                    txtCustCareComment.Visible = True
                    hypPrintCustomerCareComment.Visible = True
                    lblCustCareConfirmAvailability.Visible = False
                    While reader.Read()
                        If Not IsDBNull(reader("entrydate")) Then
                            lblCustCareConfirmationDate.Text = Convert.ToDateTime(reader("entrydate").ToString()).ToString("dd-MMM-yyyy")
                        End If

                        If Not IsDBNull(reader("name")) Then
                            lblCustCareOfficer.Text = reader("name").ToString()
                        End If

                        If Not IsDBNull(reader("reason")) Then
                            txtCustCareComment.Text = reader("reason").ToString()
                        End If

                        lblCustCareConfirmAvailability.Text = String.Empty
                        hypPrintCustomerCareComment.NavigateUrl = hypPrintCustomerCareComment.NavigateUrl & "?transid=" & transactionID
                    End While
                Else
                    lblCustCareConfirmationDate.Text = "N/A"
                    lblCustCareOfficer.Text = "N/A"
                    lblCustCareConfirmAvailability.Visible = True
                    lblCustCareConfirmAvailability.Text = "N/A"
                    txtCustCareComment.Text = String.Empty
                    txtCustCareComment.Visible = False
                    hypPrintCustomerCareComment.Visible = False
                End If
            End Using
        Catch ex As Exception
            Gadget.LogException(ex)
        End Try

    End Sub

    'Private Sub LoadDetailsForm(ByVal transactionID As String)

    '    lblTransactionType.Text = ""
    '    Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())

    '        conn.Open()
    '        Dim cmd As New Data.SqlClient.SqlCommand
    '        cmd.Connection = conn
    '        cmd.CommandText = "select * from transactions where transactionID=@tid"
    '        cmd.Parameters.AddWithValue("@tid", transactionID)

    '        Dim rs As Data.SqlClient.SqlDataReader
    '        rs = cmd.ExecuteReader
    '        While rs.Read
    '            If Not IsDBNull(rs("Status")) Then
    '                Dim status As String = String.Empty
    '                status = rs("Status").ToString()
    '                If status.Trim().ToLower().CompareTo("pay") = 0 OrElse status.Trim().ToLower().CompareTo("approve") = 0 Then
    '                    Server.Transfer("logout.aspx") 'You are trying to approve something that has already been approved or paid.
    '                End If
    '            End If
    '            Label20.Text = transactionID

    '            Label4.Text = rs("Customer_account").ToString
    '            Label5.Text = rs("Customer_name").ToString
    '            Label7.Text = FormatNumber(rs("amount").ToString, 2)
    '            Label8.Text = rs("remarks").ToString()
    '            Label9.Text = rs("uploaded_by").ToString()
    '            Label10.Text = Convert.ToDateTime(rs("Uploaded_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
    '            Label11.Text = rs("Authorized_by").ToString

    '            If Not rs("Authorized_date").ToString = String.Empty Then
    '                Label15.Text = Convert.ToDateTime(rs("Authorized_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
    '            Else
    '                Label15.Text = "N/A"
    '            End If

    '            Label12.Text = rs("Treasury_approval").ToString()
    '            Label17.Text = FormatNumber(rs("charges").ToString, 2)

    '            If Not rs("Approved_date").ToString = String.Empty Then
    '                Label13.Text = Convert.ToDateTime(rs("Approved_date").ToString).ToString("dd-MM-yyyy hh:mm tt")
    '            Else
    '                Label13.Text = "N/A"
    '            End If

    '            Label14.Text = "<br><a href='Instructions/" & transactionID & "." & rs("Instruction").ToString & "' target='_blank'>Download Mandate here [right click and Save As]</a><br/><br/><embed src='Instructions/" & transactionID & "." & rs("Instruction").ToString & "' width='600' height='600' />"

    '            Label18.Text = rs("beneficiary").ToString()
    '            Dim bankCode As String = rs("beneficiary_bank").ToString()
    '            Try

    '                If bankCode.Contains(":") Then
    '                    bankCode = bankCode.Split(":")(1)
    '                End If
    '            Catch ex As Exception

    '            End Try

    '            Label19.Text = ConvertToBankName(bankCode)
    '            lblBenAcctNum.Text = rs("Beneficiary_account").ToString()


    '            If Session("role") = "Authorizer" Then
    '                Label11.Text = "N/A"
    '                Label12.Text = "N/A"
    '                Label13.Text = "N/A"
    '                Label15.Text = "N/A"
    '            End If

    '            Dim messageTypeName As String = String.Empty
    '            If Not IsDBNull(rs("messagetypeID")) Then
    '                Dim msgDet As New MtMessageDetails
    '                messageTypeName = msgDet.GetMessageTypeNameByID(Convert.ToInt32(rs("messagetypeID").ToString()))
    '                lblTransactionType.Text = messageTypeName
    '            Else
    '                lblTransactionType.Text = "N/A"
    '            End If

    '        End While

    '    End Using
    'End Sub

    Private Sub LoadAccountEntries(ByVal transid As String)
        'TODO: This is not needed. It is the SqlDataSource that handles this part for the GridView
        'Throw New NotImplementedException
    End Sub

    Private Sub LoadSwiftMessage(ByVal transid As String)
        Dim easyRtgsObj As New EasyRtgTransaction(transid)
        Dim mt103 As String = easyRtgsObj.mt103_text
        If Not String.IsNullOrEmpty(mt103) Then
            pnlMT202Details.Visible = True
            DisplayMT202Message(mt103)
        Else
            Dim msgDet As New MtMessageDetails()
            'Check the type of the message if it an MT103, then do not bother to generate it.
            If easyRtgsObj.MessageTypeName.Trim().ToLower() = easyrtgs.MESSAGETYPE_MT202.Trim().ToLower() Then 'This is for MT202 message types
                pnlMT202Details.Visible = True
                mt103 = msgDet.generateSwiftMt202Message(transid)
                easyRtgsObj.mt103_text = mt103
                DisplayMT202Message(mt103)
            ElseIf easyRtgsObj.MessageTypeName.Trim().ToLower() = easyrtgs.MESSAGETYPE_MT103.Trim().ToLower() Then 'This is for MT103 message types
                mt103 = msgDet.generateSwiftMt103Message(transid)
                easyRtgsObj.mt103_text = mt103
                pnlMT202Details.Visible = True
                DisplayMT202Message(mt103) 'if not a MT202, then just display the SWIFT message in the supplied box for it.
            Else 'This will be the case for the normal branch transaction which does not set the message type.
                pnlMT202Details.Visible = True
                If Session("role") = "Authorizer" Then 'If it is the HOP
                    txtSwiftMessage.Visible = False
                End If
            End If

        End If
    End Sub

    Private Sub DisplayMT202Message(ByVal mt103 As String)
        'TODO: Implement
        txtSwiftMessage.Text = mt103
    End Sub

    Private Sub ClearForm()
        txtSwiftMessage.Text = String.Empty

    End Sub

    Private Function createAccountingEntries(ByVal rtgsObj As EasyRtgTransaction, ByVal newStatus As String, ByVal transid As String, ByVal supervisorName As String) As Boolean
        Dim isSetup As Boolean = False
        Dim vatRate As Decimal = 0.05D
        Dim esy As New easyrtgs()
        If esy.UpdateTxnStatus(Server.HtmlEncode(transid), newStatus) = newStatus Then
            'Get the data about this transaction using the transaction id transid

            Dim conn As Data.SqlClient.SqlConnection = New Data.SqlClient.SqlConnection(esy.sqlconn1())
            Dim isSavedPrincipal As Boolean = False
            Dim isSavedCharges As Boolean = False
            Dim isSavedVat As Boolean = False
            Using conn
                conn.Open()
                Dim trans As Data.SqlClient.SqlTransaction = conn.BeginTransaction
                Using trans
                    Dim sql As String = "spSavePostingEntries"
                    Dim cmdInsertTransactTemp As New Data.SqlClient.SqlCommand(sql, conn)

                    cmdInsertTransactTemp.Transaction = trans
                    Try
                        cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
                        cmdInsertTransactTemp.Parameters.AddWithValue("@transid", transid)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(rtgsObj.Customer_name.Trim.Replace("'", "`")))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(rtgsObj.amount, 2))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", rtgsObj.Uploaded_by)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", supervisorName)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getRTGSAccount(esy.checkisIMAL(transid))) 'esy.getRTGSAccount()
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", esy.getRTGSSuspense(esy.checkisIMAL(transid)))
						cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(transid & "-" & rtgsObj.Remarks.Trim.Replace("'", "`").Replace("&", "-")))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", rtgsObj.Branch)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Status", newStatus)



                        If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
                            isSavedPrincipal = True
                        End If

                        cmdInsertTransactTemp.Parameters.Clear()

                        'Save the charges
                        cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
                        cmdInsertTransactTemp.Parameters.AddWithValue("@transid", transid)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(rtgsObj.Customer_name.Trim.Replace("'", "`")))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(rtgsObj.charges, 2))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", rtgsObj.Uploaded_by)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", supervisorName)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getPLAccount(esy.checkisIMAL(transid)))
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", esy.getRTGSSuspense(esy.checkisIMAL(transid)))
						cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(transid & "-" & rtgsObj.Remarks.Trim.Replace("'", "`").Replace("&", "-")))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", rtgsObj.Branch)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Status", newStatus)

                        If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
                            isSavedCharges = True

                        End If


                        cmdInsertTransactTemp.Parameters.Clear()

                        'Save the charges
                        cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
                        cmdInsertTransactTemp.Parameters.AddWithValue("@transid", transid)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(rtgsObj.Customer_name.Trim.Replace("'", "`")))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(rtgsObj.charges, 2))) * vatRate)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", rtgsObj.Uploaded_by)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", supervisorName)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getVATAccount(esy.checkisIMAL(transid)))
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", esy.getRTGSSuspense(esy.checkisIMAL(transid)))
						cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(transid & "-" & rtgsObj.Remarks.Trim.Replace("'", "`").Replace("&", "-")))))
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", rtgsObj.Branch)
                        cmdInsertTransactTemp.Parameters.AddWithValue("@Status", newStatus)

                        If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
                            isSavedVat = True

                        End If

                        If isSavedCharges And isSavedPrincipal And isSavedVat Then
                            Try
                                trans.Commit()
                                isSetup = True
                            Catch ex As Exception
                                isSetup = False
                            End Try

                        Else
                            trans.Rollback()
                            isSetup = False
                        End If
                    Catch ex As Exception
                        trans.Rollback()
                        isSetup = False

                    End Try

                End Using

            End Using

        End If
        Return isSetup
    End Function

End Class
