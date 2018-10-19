
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



            'If Not esy.getSessionStatus(Session("uname"), Session.SessionID) = True Then
            '    Session.Abandon()
            '    Response.Redirect("default.aspx")

            'End If


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
                    Dim bankCode As String = rs("beneficiary_bank").ToString
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

                End While

            End Using

          
        End If


    End Sub






    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Label1.Text = ans.Value
        Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())

            If ans.Value.Contains("rue") Then

                Dim trans As Data.SqlClient.SqlTransaction
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

                                ' Label16.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction was Authorized" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"

                            End If

                            If Session("role") = "Treasury" Then
                                'trans = conn.BeginTransaction()
                                'cmd.Transaction = trans
                                cmd.CommandText = "update transactions set Treasury_Approval=@name, approved_date=@date, status='Approved'  where transactionID=@tid"
                                cmd.Parameters.AddWithValue("@name", Session("name"))
                                cmd.Parameters.AddWithValue("@date", Now.ToString("yyyy-MM-dd hh:mm tt"))
                                cmd.Parameters.AddWithValue("@tid", Server.HtmlEncode(Request.QueryString("tid")))


                                If cmd.ExecuteNonQuery() > 0 Then
                                    cmd.Parameters.Clear()
									If SetupPaymentEntries(Server.HtmlEncode(Request.QueryString("tid"))) Then
										'cmd.CommandText = "UPDATE TransactTemp2 set status='Approved'  where transactionID=@tid"
										'cmd.Parameters.AddWithValue("@tid", Server.HtmlEncode(Request.QueryString("tid")))
										'If cmd.ExecuteNonQuery() > 0 Then
										'    Try
										'        trans.Commit()
										'    Catch ex As Exception
										'        trans.Rollback()
										'    End Try

										'End If
									Else
										'TODO: Please show an error message here.
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
                                "Customer: " & Label5.Text & "<br>" & "Amount: " & Label7.Text & "<br>" & "Account Number: " & Label4.Text & "<br><br>Thank You"

                                Try
                                    Dim mail As New easyrtgs
									mail.sendmail(System.Configuration.ConfigurationManager.AppSettings("mailHost"), mail.getAuthorizerEmailbyName(Label11.Text), mail.getTreasuryMail(esy.checkisIMAL(Request.QueryString("tid"))), "3RD PARTY TRANSACTION APPROVAL (TRANSACTION ID: " & Request.QueryString("tid") & ")", body)

								Catch ex2 As Exception
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
                    "Customer: " & Label5.Text & "<br>" & "Amount: " & Label7.Text & "<br>" & "Account Number: " & Label4.Text & "<br><br>Thank You"

                    Try
                        Dim mail As New easyrtgs
						mail.sendmail(System.Configuration.ConfigurationManager.AppSettings("mailHost"), mail.getAuthorizerEmailbyName(Label11.Text), mail.getTreasuryMail(esy.checkisIMAL(Request.QueryString("tid"))), "3RD PARTY TRANSACTION APPROVAL (TRANSACTION ID: " & Request.QueryString("tid") & ")", body)

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
			Dim isReversed As Boolean = SetupReversal(Server.HtmlEncode(Request.QueryString("tid")))

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

    Sub complete(ByVal sender As Object, ByVal e As System.EventArgs)

        If esy.UpdatetxnAsPay(Server.HtmlEncode(Request.QueryString("tid"))) = "Pay" Then

            Response.Redirect("PayResponse.aspx")
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
            If Session("role") <> "Treasury" Then
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
		Dim isReversed As Boolean = False
		If esy.UpdatetxnForReversal(Server.HtmlEncode(transid)) = "Reverse" Then
			'Get the data about this transaction using the transaction id transid
			'Dim rtgsObj As New EasyRtgTransaction(Convert.ToInt64(Server.HtmlEncode(transid)))
			Dim rtgsObj As New EasyRtgTransaction(Server.HtmlEncode(transid))
			Dim isStaff As Boolean = IIf(rtgsObj.Customer_account.Trim().Split("-"c)(3) = "9", True, False)
			Dim vatRate As Decimal = 0.05D

			'If isStaff Then
			'    vatRate = 1D
			'Else
			'    vatRate = 0.05D
			'End If

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


	Private Function SetupPaymentEntries(ByVal transid As String) As Boolean
		Dim isSetup As Boolean = False
		Dim newStatus As String = "Approved"
		Dim vatRate As Decimal = 0.05D
		Dim rtgsObj As New EasyRtgTransaction(Server.HtmlEncode(transid))
		Dim isStaff As Boolean = IIf(rtgsObj.Customer_account.Trim().Split("-"c)(3) = "9", True, False) 'if the ledger code is ledger 9, then the customer is a staff.

		'If isStaff Then
		'    vatRate = 0D
		'Else
		'    vatRate = 0.05
		'End If

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
						cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", rtgsObj.Uploaded_by)
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
						cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", rtgsObj.Uploaded_by)
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
						cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", rtgsObj.Uploaded_by)
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
