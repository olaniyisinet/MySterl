Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports BankCore

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class easyrtgsapi
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function doNameEnquiry(ByVal bankCode As String, ByVal accountno As String) As String
        Dim response As String = String.Empty
        Dim gad As New Gadget
        Dim name As String = gad.DoNameEnquiryOutput(bankCode, accountno)
        Dim dict As Dictionary(Of String, String) = gad.parseXmlResponse(name)


        If dict.ContainsKey("RESPONSECODE") AndAlso dict.ContainsKey("RESPONSETEXT") Then
            Dim code As String = dict("RESPONSECODE")
            Dim nameval As String = dict("RESPONSETEXT")

            If Not String.IsNullOrEmpty(code) AndAlso code.Trim().CompareTo("00") = 0 Then

                Return nameval

            Else
                Return "-1"
            End If
        Else
            Return "-1"
        End If
        Return response
    End Function

    <WebMethod(Description:="Returns a <code>DataSet</code> that has two columns: <code>bank_code</code> and <code>bank_name</code>")>
    Public Function GetBeneficiaryBanks() As DataSet

        Dim e As New easyrtgs
        Return e.getBanks()
    End Function
    <WebMethod(Description:="Submits a new RTGS request.<br />" &
        "Response Format: <br />" &
        "00:Failure:{Trans_Ref} <br />" &
        "11:Success:{Error_Message} <br />" &
        "Where {Trans_Ref} is the transaction ID of format SRSXXXXXXXXXXX <br />" &
        "And {Error_Message} is the reason why the transaction failed <br />" &
               "<code>customername</code> - Customer Name <br />" &
               "<code>customerAccount</code> - Customer Account <br />" &
               "<code>transferAmount</code> - The amount to be transferred <br />" &
                "<code>narration</code> - The transaction narration<br />" &
                "<code>beneficiaryName</code> - The name of the beneficiary. After doing name enquiry on IBS.<br />" &
                "<code>beneficiaryAccount</code> - The account number of the beneficiary.<br />" &
                 "<code>beneficiaryBankName</code> - The name of the beneficiary bank <seealso>GetBeneficiaryBanks</seealso>.<br />" &
                 "<code>beneficiaryBankCode</code> - The code of the beneficiary bank <seealso>GetBeneficiaryBanks</seealso>.<br />")>
    Public Function SubmitRtgs(ByVal customername As String,
                               ByVal customerAccount As String,
                               ByVal transferAmount As Decimal,
                               ByVal narration As String,
                               ByVal beneficiaryName As String,
                               ByVal beneficiaryAccount As String,
                               ByVal beneficiaryBankName As String,
                               ByVal beneficiaryBankCode As String) As String

		Dim isIMAL As Boolean = False
		Dim tid As String = EasyRtgTransaction.generateRef()
        Dim t24Obj As BankCore.t24.T24Bank = New BankCore.t24.T24Bank()
        Dim response As String = "{0}:{1}:{2}"

        'Validate some input that are important for us to get right
        Dim bank As BankDetail = New BankDetail()
        Try
            bank = BankDetail.GetBankDetailByCode(beneficiaryBankCode)
            If bank Is Nothing Then
                response = String.Format(response, "00", "Failure", "Could not validate bank code. Ask Administrator for valid bank codes")
                Return response
            End If
            If String.IsNullOrEmpty(bank.Bank_code) Then
                response = String.Format(response, "00", "Failure", "Invalid bank code. Ask Administrator for valid bank codes")
                Return response
            End If
        Catch ex As Exception
            Gadget.LogException(ex)
        End Try

		If customerAccount.StartsWith("05") Then
			isIMAL = True
		Else
			isIMAL = False
		End If

		Dim statusName As String = easyrtgs.TREASURY_PENDING_STATUS 'Set it to Authorize-Pending so that when the financial entry (which starts as "Authorize" is posted successfully, this will become "Authorized" and then will go to Treasury straight away (see mytransactions.aspx for the Status code that shows pending transactions for Treasury)
        Dim newstatusName As String = statusName
        Dim msgtype As String = String.Empty
        Dim msgvariant As String = String.Empty
        Dim transactionType As String = easyrtgs.TRANSTYPE_CBN
        msgtype = easyrtgs.MESSAGETYPE_MT103.ToString()
        msgvariant = String.Empty


        Dim esy As New easyrtgs
        Dim msgTypeID As Integer = 0
        Dim msgVarID As Integer = 0

        Dim mgdetOb As New MtMessageDetails
        msgTypeID = mgdetOb.GetMessageTypeIDByName(msgtype)
        msgVarID = mgdetOb.GetMessageVariantIDByName(msgvariant)


        'Refactoring
        Dim g As New Gadget()
        Dim t As New EasyRtgTransaction(tid)
        t.TransactionID = tid
        t.Customer_name = customername
        t.Customer_account = customerAccount
        t.amount = transferAmount
        Dim es As New easyrtgs

        Dim charges As Decimal = 0D
        Dim vatRate As Decimal = 0.05D


        Dim acctPrincipal As IAccount = t24Obj.GetAccountInfoByAccountNumber(customerAccount)
        Dim isstaff As Boolean = False
        If acctPrincipal IsNot Nothing Then
            isstaff = acctPrincipal.AccountType.Trim().ToLower().Contains("staff")
        Else
            response = String.Format(response, "00", "Failure", "Customer Account Not Exist or CBA is down")
            Return response
        End If

        If isstaff Then
            charges = Convert.ToDecimal(es.getStaffCharges())
        Else
            charges = Convert.ToDecimal(es.getCustomerCharges())
        End If

        t.CustomerEmail = acctPrincipal.Email
        t.charges = FormatNumber(charges, 2)
        t.Remarks = tid & " " & narration.Trim.Replace("'", "`").Replace("&", "-")
        t.status = statusName
        t.Uploaded_by = "IBS_USER"
        t.Uploaded_date = DateTime.Now
        t.Branch = "NG0020001"
        t.Instruction = "PDF"
        t.Beneficiary = beneficiaryName
        t.Beneficiary_Bank = beneficiaryBankName & ":" & beneficiaryBankCode
        t.Beneficiary_account = beneficiaryAccount
        t.date = DateTime.Now
        t.MessagetypeID = msgTypeID ' Message type id for mt103
        t.MessageVariantID = msgVarID 'Message variant id for mt103
        t.RequiresCustCareApproval = 0 ' Initiated by TROPS, does not go for customer care approval.
        t.MailSent = 0
        t.RequestingBranch = "NG0020001"
        t.RequestingBranchAccount = ""
        t.PostConfirmationStatus = ""
        t.TransactionType = easyrtgs.TRANSTYPE_INTERBANK


        Dim transactionID As String = t.Save(False)

        If (transactionID <> "-1") Then
            Dim entryList As New List(Of PostingEntry)
            Dim isAllEntriesInserted As Boolean = False
            Dim principalEntry As New PostingEntry()
            principalEntry.TransactionID = t.TransactionID
            principalEntry.EntryDate = DateTime.Now
            principalEntry.ErrorText = String.Empty
            principalEntry.CustomerName = t.Customer_name
            principalEntry.Amount = Convert.ToDecimal(FormatNumber(t.amount, 2))
            principalEntry.Officer = "IBS_USER"
            principalEntry.Supervisor = "IBS_USER"
			principalEntry.CrCommAcct = esy.getRTGSSuspense(isIMAL)
			principalEntry.DrCustAcct = t.Customer_account
            principalEntry.expl_code = esy.getExplCode()
            principalEntry.Remarks = esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(t.TransactionID & "-" & t.Remarks.Trim.Replace("'", "`").Replace("&", "-"))))
            principalEntry.Source = "IBS"
            principalEntry.MFNo = "NG0020001"
            principalEntry.Status = "Authorize"
            entryList.Add(principalEntry)

            Dim chargeEntry As New PostingEntry()
            chargeEntry.TransactionID = t.TransactionID
            chargeEntry.EntryDate = DateTime.Now
            chargeEntry.ErrorText = String.Empty
            chargeEntry.CustomerName = t.Customer_name
            chargeEntry.Amount = Convert.ToDecimal(FormatNumber(charges, 2))
            chargeEntry.Officer = "IBS_USER"
            chargeEntry.Supervisor = "IBS_USER"
			chargeEntry.CrCommAcct = esy.getRTGSSuspense(isIMAL)
			chargeEntry.DrCustAcct = t.Customer_account
            chargeEntry.expl_code = esy.getExplCode()
            chargeEntry.Remarks = esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(tid & "-" & t.Remarks.Trim.Replace("'", "`").Replace("&", "-"))))
            chargeEntry.Source = "EasyRTGS"
            chargeEntry.MFNo = "NG0020001"
            chargeEntry.Status = "Authorize"
            entryList.Add(chargeEntry)


            Dim vatEntry As New PostingEntry()
            vatEntry.TransactionID = t.TransactionID
            vatEntry.EntryDate = DateTime.Now
            vatEntry.ErrorText = String.Empty
            vatEntry.CustomerName = t.Customer_name.Trim.Replace("'", "`")
            vatEntry.Amount = Convert.ToDecimal(Server.HtmlEncode(FormatNumber(charges, 2) * vatRate))
            vatEntry.Officer = "IBS_USER"
            vatEntry.Supervisor = "IBS_USER"
			vatEntry.CrCommAcct = esy.getRTGSSuspense(isIMAL)
			vatEntry.DrCustAcct = t.Customer_account
            vatEntry.expl_code = esy.getExplCode()
            vatEntry.Remarks = esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(tid & "-" & t.Remarks.Trim.Replace("'", "`").Replace("&", "-"))))
            vatEntry.Source = "EasyRTGS"
            vatEntry.MFNo = "NG0020001"
            vatEntry.Status = "Authorize"
            entryList.Add(vatEntry)

            isAllEntriesInserted = PostingEntry.SaveInTransaction(entryList, es.conn1())

            If Not isAllEntriesInserted Then
                Dim etran As New EasyRtgTransaction(t.TransactionID)
                etran.Delete()
                '{0}:{1}:{2}
                response = String.Format(response, "00", "Failure", "Could not save accounting entries. Deleted")
                Return response
            Else
                esy.createAudit(tid & " was posted by " & "IBS_USER", Now)
                esy.TransactionAudit(tid & " was posted by " & "IBS_USER", Now, tid)


                'notify authorizer
                Dim body As String = ""
                body = "Sir/Madam" & "<br>" & "<br>" & "This mail is a request for you to authorize a Third Party Transfer Transaction . Details of this transaction are as follows;" & "<br>" &
                "Customer: " & t.Customer_name & "<br>" & "Amount: " & FormatNumber(t.amount, 2) & "<br>" & " Account Number: " & t.Customer_account & "<br><br>" & "Click on the link below to authorize this transaction. <br><br><a href='http://" & System.Configuration.ConfigurationManager.AppSettings("serverip") & "/" & System.Configuration.ConfigurationManager.AppSettings("AppVirDir") & "/default.aspx?action=authorize&tid=" & tid & "'>Authorize this Transaction</a> <br><br>Thank You"

                Dim host As String = ConfigurationManager.AppSettings("MailHost").ToString()
                Dim toAddress As String = ""
                Try
                    Dim Mail As New easyrtgs()

					Mail.sendmail(host, Mail.getRTGSMail(esy.checkisIMAL(Mail.checkisIMAL(t.TransactionID))), t.CustomerEmail, "3RD PARTY TRANSACTION AUTHORIZATION REQUEST (TRANSACTION ID: " & t.TransactionID & ")", body)

				Catch ex As Exception
                    Gadget.LogException(ex)
                End Try
                response = String.Format(response, "11", "Success", t.TransactionID)
                Return response



            End If

        End If

        'if it was successful, it would have been returned before executing up to this line
        response = String.Format(response, "00", "Failure", "No Operation Occurred")
        Return response
    End Function

End Class