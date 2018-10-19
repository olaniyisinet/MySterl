Imports System.Net
Imports System.Data.SqlClient
Imports BankCore
Imports BankCore.t24

Partial Class main
    Inherits System.Web.UI.Page
    Private tid As String = ""
    Private ess As New easyrtgs
    Private ledcode As String = ""
    Private instruction As String = ""

    Public Function generateRef() As String
        Dim ref As String = ConfigurationManager.AppSettings("REF_PREFIX") & DateTime.Now.ToString("yyMMddHHmmss") & GenerateRndNumber(1)
        Return ref
    End Function

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init



    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        If Session("uname") = "" Then

            Response.Redirect("default.aspx")

        Else
            Label1.Text = Session("name")



        End If



        'If Not ess.getSessionStatus(Session("uname"), Session.SessionID) = True Then
        '    Session.Abandon()
        '    Response.Redirect("default.aspx")

        'End If



        If Session("role") = "Authorizer" Then

            Response.Redirect("mytransactions.aspx")
        End If


        If Session("role") = "Treasury" Then

            Response.Redirect("mytransactions.aspx")
        End If



        If Session("role") = "Administrator" Then

            Response.Redirect("admin.aspx")
        End If

    End Sub
    Public Function GenerateRndNumber(ByVal cnt As Integer) As String
        Dim key2 As String() = {"0", "1", "2", "3", "4", "5", _
         "6", "7", "8", "9"}
        Dim rand1 As New Random()
        Dim txt As String = ""
        For j As Integer = 0 To cnt - 1
            txt += key2(rand1.[Next](0, 9))
        Next
        Return txt
    End Function

    Public Function newSessionId(ByVal bankcode As String) As String
        System.Threading.Thread.Sleep(50)
        Return (Convert.ToString("232") & bankcode) + DateTime.Now.ToString("yyMMddHHmmss") + GenerateRndNumber(12)
    End Function


    Function ret() As Boolean
        Return True

    End Function

    Function getAccountName(ByVal accno As String) As String
        Dim sqlstr As String = IO.File.ReadAllText(Server.MapPath("./accnamesql.txt")).Replace("%nuban%", accno)
        Dim t24 As New T24Bank()
        Dim acct As IAccount = Nothing
        Dim ds As New Data.DataSet()
        Dim accname As String = ""

        acct = t24.GetAccountInfoByAccountNumber(accno)
        If acct IsNot Nothing Then
            Return acct.CustomerName
        Else
            Throw New Exception("Could not retrieve account name because account object was NULL.")

        End If



    End Function
    



    Function getOldAcc(ByVal nuban As String) As String
        Dim ds As New Data.DataSet
        Dim t24 As New T24Bank()
        Dim acct As IAccount = Nothing
        Dim oldacc As String = ""

        acct = t24.GetAccountInfoByAccountNumber(nuban)
        If acct IsNot Nothing Then
            If ds.Tables(0).Rows.Count > 0 Then
                
                oldacc = acct.AccountNumberRepresentations(Account.BANKS).Representation
                If Not String.IsNullOrEmpty(oldacc) Then
                    Return oldacc
                Else

                    oldacc = acct.AccountNumberRepresentations(Account.NUBAN).Representation
                End If
            End If
        End If




        Return oldacc

    End Function



    Sub showprg()


    End Sub









    Protected Sub btnReject_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReject.Click
        If String.IsNullOrEmpty(txtCustomerCareComment.Text) Then
            WriteMsg("Please state the reason for rejection in the comment box above.", True)
            Return
        End If

        Dim transid As String = gvPendingCustomerCare.SelectedDataKey.Value
        Dim comment As String = txtCustomerCareComment.Text
        Dim userid As String = Session("uname")
        Dim isApproval As Boolean = False
        Dim rejectionStatus As Boolean = PerformAuthorization(transid, comment, userid, isApproval)

        If rejectionStatus Then
            'State the comment
            Try
                gvPendingCustomerCare.DataBind()
                dtVwTransaction.DataBind()
            Catch ex As Exception

            End Try

            WriteMsg("Successfully rejected", False)
        Else
            'State the comment
            WriteMsg("Could not reject", True)
        End If

    End Sub

    Private Function PerformAuthorization(ByVal transid As String, ByVal comment As String, ByVal userid As String, ByVal isApproval As Boolean) As Boolean
        'get the EasyTransaction object
        Dim rtgsObj As New EasyRtgTransaction(transid)
        Dim status As Boolean = False
        Dim statusName As String = ""
        Dim sql As String = "UPDATE transactions SET CustCareApproveReject=@approveReject, CustCareApprovalDate=GETDATE(), CustCareApproverUserid=@userid, CustCareComment=@comment, Status=@status WHERE TransactionID=@transid"
        Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddWithValue("@transid", transid)
                cmd.Parameters.AddWithValue("@comment", comment)
                cmd.Parameters.AddWithValue("@userid", userid)
                If isApproval Then
                    'get the status from the postconfirmation status field
                    statusName = rtgsObj.PostConfirmationStatus
                    cmd.Parameters.AddWithValue("@approveReject", "APPROVE-1")
                Else
                    statusName = "CustomerCareRejection"
                    cmd.Parameters.AddWithValue("@approveReject", "REJECT-0")
                End If

                cmd.Parameters.AddWithValue("@status", statusName)

                Dim i As Integer = cmd.ExecuteNonQuery()

                If i >= 0 Then
                    status = True
                Else
                    status = False
                End If
            Catch ex As Exception
                Gadget.LogException(ex)
            Finally
                If cn IsNot Nothing Then cn.Close()
            End Try
        End Using
        Return status
    End Function

    Private Sub WriteMsg(ByVal msg As String, ByVal isErrorMsg As Boolean)
        lblMsg.Text = msg
        If isErrorMsg Then
            lblMsg.ForeColor = Drawing.Color.Red
        Else
            lblMsg.ForeColor = Drawing.Color.White
        End If
    End Sub

    Protected Sub btnApprove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApprove.Click
        Dim transid As String = gvPendingCustomerCare.SelectedDataKey.Value
        Dim comment As String = txtCustomerCareComment.Text
        Dim userid As String = Session("uname")
        Dim isApproval As Boolean = True
        Dim isErrorMsg As Boolean = False
        Dim rejectionStatus As Boolean = PerformAuthorization(transid, comment, userid, isApproval)
        Dim mail As New easyrtgs
        If rejectionStatus Then
            'State the comment
            isErrorMsg = False
            ''Send a mail to the initiator and the HOP
            Dim transObj As New EasyRtgTransaction(transid)
            Dim branch As String = Session("branch")
            Dim host As String = String.Empty
            Dim authorizerEmail As String = String.Empty
            Dim custcareOfficerEmail As String = Session("email")
            Dim subject As String = "3RD PARTY TRANSACTION AUTHORIZATION REQUEST (TRANSACTION ID: " & tid & ")"
            Dim body As String = String.Empty

            body = GetEmailBody(transObj)



            host = System.Configuration.ConfigurationManager.AppSettings("mailHost")
            authorizerEmail = mail.getAuthorizerEmail(branch)

            'Save the response on the table: tblCustCareResponses
            Dim responderName As String = txtResponderName.Text
            Dim customerResponse As String = txtCustomerResponse.Text
            Dim custCareComment As String = txtCustomerCareComment.Text
            Dim custofficeruserid As String = Session("uname").ToString()
            Dim easyObj As New EasyRtgTransaction(transid)
            Dim amt As Decimal = Convert.ToDecimal(easyObj.amount)
            Dim acctNum As String = easyObj.Customer_account

            Dim custCareObj As New CustomerCareOps
            Dim issaved As Boolean = custCareObj.saveCustomerCareResponse(transid, responderName, customerResponse, custCareComment, custofficeruserid, amt)

            mail.sendmail(host, authorizerEmail, custcareOfficerEmail, subject, body)

            Try
                gvPendingCustomerCare.DataBind()
                dtVwTransaction.DataBind()
            Catch ex As Exception

            End Try

            WriteMsg("Successfully approved", isErrorMsg)
        Else
            'State the comment
            isErrorMsg = True
            WriteMsg("Could not approve", isErrorMsg)
        End If
    End Sub

    Private Function GetEmailBody(ByVal transObj As EasyRtgTransaction) As String
        Dim bodyBuilder As New StringBuilder()
        'TODO: implement a good and well-formatted email body here later.
        Dim body As String = String.Empty
        bodyBuilder.AppendLine("Good day,")
        bodyBuilder.AppendLine(String.Format("Transaction ID: {0}", transObj.TransactionID))
        bodyBuilder.AppendLine(String.Format("Transaction Amount: {0}", transObj.amount))
        bodyBuilder.AppendLine("has been successfully approved")
        bodyBuilder.AppendLine()
        bodyBuilder.AppendLine("Yours sincerely")
        body = bodyBuilder.ToString()
        Return (body)
    End Function

    Protected Sub gvPendingCustomerCare_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvPendingCustomerCare.SelectedIndexChanged
        pnlRequestDetails.Visible = False
        If gvPendingCustomerCare.SelectedIndex <> -1 Then
            pnlRequestDetails.Visible = True
        Else
            pnlRequestDetails.Visible = False
        End If
    End Sub

    Protected Sub dtVwTransaction_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtVwTransaction.DataBound
        If dtVwTransaction.Rows.Count > 0 Then

            pnlRequestDetails.Visible = True
        Else
            pnlRequestDetails.Visible = False
        End If


    End Sub
End Class

