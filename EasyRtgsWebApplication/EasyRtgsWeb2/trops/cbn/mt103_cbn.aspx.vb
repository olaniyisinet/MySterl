Imports System.Net
Imports System.Data
Imports BankCore
Imports BankCore.T24
Imports System.Data.SqlClient

Partial Class mt103_cbn
    Inherits System.Web.UI.Page
    Private tid As String = ""
    Private ess As New easyrtgs
    Private ledcode As String = ""
    Private instruction As String = ""
    Private t24 As New T24Bank

    'Public Function generateRef() As String
    '    Dim ref As String = ConfigurationManager.AppSettings("REF_PREFIX") & DateTime.Now.ToString("yyMMddHHmmss") & GenerateRndNumber(1)
    '    Return ref
    'End Function

    'Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
    '    Dim rnd As New System.Random(Second(Now))

    '    tid = generateRef()
    '    TextBox6.Visible = False
    '    DropDownList1.Items.Insert(0, New ListItem("--Please select--", "000"))
    '    For Each dr As Data.DataRow In ess.getBanks().Tables(0).Rows
    '        DropDownList1.Items.Add(New ListItem(dr("bank_name").ToString, dr("bank_code").ToString))
    '    Next


    'End Sub

    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load




    '    If Session("uname") = "" Then

    '        Server.Transfer("default.aspx")

    '    Else
    '        Label1.Text = Session("name")

    '        Label4.Text = "N " & ess.getCustomerCharges()
    '        Label5.Text = "N " & ess.getStaffCharges()


    '    End If

    '    ''Create this session variable on first load
    '    'If Not IsPostBack Then
    '    '    Session("doNameQuery") = True
    '    'End If




    '    Dim role As String = Session("role").ToString()
    '    Dim transferType As String = Session("transtype").ToString()
    '    ModifyUserInterfaceByRoleAndTransferType(role, transferType)


    '    If Session("role") = "Authorizer" Then

    '        Server.Transfer("mytransactions.aspx")
    '    End If


    '    If Session("role") = "Treasury" Then

    '        Server.Transfer("mytransactions.aspx")
    '    End If



    '    If Session("role") = "Administrator" Then

    '        Server.Transfer("admin.aspx")
    '    End If

    'End Sub
    'Public Function GenerateRndNumber(ByVal cnt As Integer) As String
    '    Dim key2 As String() = {"0", "1", "2", "3", "4", "5",
    '     "6", "7", "8", "9"}
    '    Dim rand1 As New Random()
    '    Dim txt As String = ""
    '    For j As Integer = 0 To cnt - 1
    '        txt += key2(rand1.[Next](0, 9))
    '    Next
    '    Return txt
    'End Function

    'Public Function newSessionId(ByVal bankcode As String) As String
    '    System.Threading.Thread.Sleep(50)
    '    Return (Convert.ToString("232") & bankcode) + DateTime.Now.ToString("yyMMddHHmmss") + GenerateRndNumber(12)
    'End Function


    'Function ret() As Boolean
    '    Return True

    'End Function

    Sub benefName(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim bank_code As String = DropDownList1.SelectedValue
        Dim accountNum As String = TextBox1.Text

        Dim gad As New Gadget
        Dim name As String = String.Empty
        Dim errorMsg As String = String.Empty
        Dim response As String = gad.DoNameQueryNew(bank_code, accountNum, "new")

        Dim parts() As String = response.Split(":")
        If parts(0) = "00" Then
            name = parts(1)
        Else
            name = String.Empty
            errorMsg = parts(1)
        End If
        'The dictionary will not contain the keys "RESPONSECODE" or "RESPONSETEXT" if the 


        If Not String.IsNullOrEmpty(name) Then

            txtCustName103.Visible = True
            txtCustName103.Text = name

        Else
            LinkButton1.Visible = True
            Label2.ForeColor = Drawing.Color.Red
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>Error: " & "Could not retrieve the name of this account: Message: " & errorMsg & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
        End If

    End Sub




    Sub show2(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim inputAcct As String = TextBox1.Text
        Dim ds As DataSet = New DataSet()
        Dim outputNuban As String = String.Empty
        Dim acctName As String = String.Empty
        Dim availBal As String = String.Empty
        Dim oldAcctNumberFull As String = String.Empty
        Dim sep As String = "-"
        Dim g As New Gadget()
        Label2.Text = ""



        Try
            'If Not IsPostBack Then
            Dim ac As String = ""
            '   Dim account As String=  TextBox5.Text & textbox6.Text & TextBox7.Text & textbox8.Text & TextBox9.Text
            Dim acctObj As IAccount = Nothing



            If inputAcct.Trim.Length = 10 Then
                ac = inputAcct.Trim()

                acctObj = t24.GetAccountInfoByAccountNumber(inputAcct)

            Else
                ac = inputAcct.Trim()

                'Check that the account number has the correct separator
                If inputAcct.Contains("/") Then
                    Dim acnparts() As String = inputAcct.Split("/")

                    Dim bracode As String = acnparts(0)
                    Dim cusnum As String = acnparts(1)
                    Dim curcode As String = acnparts(2)
                    Dim ledcode As String = acnparts(3)
                    Dim subacctcode As String = acnparts(4)

                    Dim paddedBanksFormatAcctNumber As String = New Util1s().getAccountNo(bracode, cusnum, curcode, ledcode, subacctcode)
                    acctObj = t24.GetAccountInfoByAccountNumber(paddedBanksFormatAcctNumber)
                Else
                    Label2.Text = "<font color=red>Please use this character: / to separate the branch code, customer number, currency code, ledger code and subacctcode.</font>"
                    Exit Sub
                    'acctObj = t24.GetAccountInfoByAccountNumber(inputAcct)
                End If




            End If


            outputNuban = acctObj.AccountNumberRepresentations(Account.NUBAN).Representation
            acctName = acctObj.CustomerName
            Dim cle_bal As Decimal = 0
            Dim for_amt As Decimal = 0
            Dim bal_lim As Decimal = 0
            Dim tot_blo_fund As Decimal = 0
            Dim risk_lim As Decimal = 0





            'availBal = acctObj.UsableBal.ToString()
            availBal = acctObj.UsableBal
            oldAcctNumberFull = outputNuban 'acctObj.AccountNumberRepresentations(Account.BANKS).Representation
            'Dim acctparts() As String = oldAcctNumberFull.Split(sep)
            'Session("acc") = oldAcctNumberFull
            'Session("ledcode") = acctparts(3)
            'Dim acctparts() As String = oldAcctNumberFull
            Session("acc") = oldAcctNumberFull
            Session("ledcode") = acctObj.ProductCode
            Session.Timeout = 30
            lblBalance103.Text = FormatNumber(availBal, 2)
            txtCustName103.Text = acctName
            Session("bal") = FormatNumber(availBal, 2)
            Session("cname") = txtCustName103.Text
            Session.Timeout = 10

            Button3.Value = "Get Customer Data"
            Button3.Disabled = False
            'End If

        Catch ex As Exception
            Gadget.LogException(ex)


            Label2.ForeColor = Drawing.Color.Red
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>" & ex.Message() & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"


        End Try



    End Sub

    Private Sub mt103_cbn_Init(sender As Object, e As EventArgs) Handles Me.Init
        tid = EasyRtgTransaction.generateRef()
        TextBox6.Visible = False
        DropDownList1.Items.Insert(0, New ListItem("--Please select--", "000"))
        For Each dr As Data.DataRow In ess.getBanks().Tables(0).Rows
            DropDownList1.Items.Add(New ListItem(dr("bank_name").ToString, dr("bank_code").ToString))
        Next
    End Sub

    Private Sub mt103_cbn_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Session("uname") = "" Then

                Server.Transfer("../../default.aspx")

            Else
                lblUserFullName.Text = Session("name")
                Label4.Text = "N " & ess.getCustomerCharges()
                Label5.Text = "N " & ess.getStaffCharges()


            End If

            'Get the default bank code
            Dim defaultBankCode As String = ConfigurationManager.AppSettings("CBN_TRXN_DEFAULT_BANK_CODE").Trim().ToLower()
            Dim disableBankChange As String = ConfigurationManager.AppSettings("CBN_TRXN_DISABLE_BANK_CHANGE").Trim().ToLower()
            Dim enableNameQuery As String = ConfigurationManager.AppSettings("CBN_TRXN_ENABLE_NAME_ENQUIRY").Trim().ToLower()
            If (enableNameQuery = "true") Then
                Session("enableNameQuery") = True
            ElseIf (enableNameQuery = "false") Then
                Session("enableNameQuery") = False
            End If

            DropDownList1.SelectedValue = defaultBankCode

            If (disableBankChange = "true") Then
                DropDownList1.Enabled = False
            ElseIf (disableBankChange = "false") Then
                DropDownList1.Enabled = True
            End If

            If (Session("enableNameQuery") = False) Then
                txtBenName103.Enabled = True
                txtBenAccount103.ReadOnly = False
            Else
                txtBenName103.Enabled = False
                txtBenAccount103.ReadOnly = True
            End If
        End If
    End Sub

    Protected Sub txtBenAccount103_TextChanged(sender As Object, e As EventArgs) Handles txtBenAccount103.TextChanged
        'TODO: Name Enquiry disabled
        If Session("enableNameQuery") = True Then
            Label6.Text = ""
            Dim acctLen As Integer = txtBenAccount103.Text.Length
            Dim bankCode As String = DropDownList1.SelectedValue.ToString()
            Dim bdet As BankDetail = BankDetail.GetBankDetailByCode(bankCode)
            'Chceck whether the application is running in the offline mode
            Dim isOffline As String = ConfigurationManager.AppSettings("APP_MODE").ToString()
            If isOffline.ToLower().Trim() = "offline" Then
                'txtBenName103.Text = "JOHN SMITH"
                If bdet.IsNameEnquirySupported Then
                    If acctLen = 10 Then
                        benefName(sender, e)
                    Else
                        Label6.Text = "Please check the beneficiary account number because it is not 10 digits long."
                    End If

                Else

                    txtBenName103.Visible = True  'Make this visible so that it can be inputted directly
                    txtBenName103.ReadOnly = False

                End If
            Else
                If bdet.IsNameEnquirySupported Then
                    If acctLen = 10 Then
                        benefName(sender, e)
                    Else
                        Label6.Text = "Please check the beneficiary account number because it is not 10 digits long."
                    End If

                Else

                    txtBenName103.Visible = True  'Make this visible so that it can be inputted directly
                    txtBenName103.ReadOnly = False

                End If
            End If
        End If

    End Sub



    Protected Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        lblSubmissionText.Text = String.Empty
        Dim trans As SqlTransaction = Nothing
        'Perform token validation here
        Dim username As String = txtUsername103.Text
        Dim token As String = txtToken103.Text

        If ConfigurationManager.AppSettings("APP_MODE").ToString().ToLower() = "online" Then
            Dim tokenValUtil As New TokenValidatorUtil()
            Dim response As TokenResponseHolder = tokenValUtil.ValidateToken(username, token.Trim())
            If Not response.IsValid Then
                Dim responseMsg As String = response.ResponseMessage
                lblSubmissionText.Visible = True
                lblSubmissionText.Text = "<font color=red>" & responseMsg & "</font>"
                Exit Sub
            End If
        End If

        If String.IsNullOrEmpty(TextBox1.Text) Then
            Label2.Text = String.Empty
            Label2.Text = "Please enter the account number."
            Exit Sub
        End If

        'Check that the account is a naira account
        Dim t24Obj As New T24Bank()
        Dim acctPrincipal As IAccount = t24Obj.GetAccountInfoByAccountNumber(TextBox1.Text)
        If acctPrincipal.CurrencyCode.Trim().ToUpper() <> "NGN" Then
            Label2.Text = String.Empty
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "You supplied a " & acctPrincipal.CurrencyCode.Trim().ToUpper() & " account. Please enter a Naira(NGN) account number only." & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub
        End If



        If String.IsNullOrEmpty(txtBenName103.Text) Then
            Label2.ForeColor = Drawing.Color.Red
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Missing Beneficiary Name." & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub
        End If


        If Session("bal") Is Nothing Then
            Label2.ForeColor = Drawing.Color.Red
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Incomplete fields." & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub
        End If

        If Session("cname") Is Nothing Then
            Label2.ForeColor = Drawing.Color.Red
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Incomplete Fields." & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub

            Exit Sub
        End If

        If tid Is Nothing Then
            Server.Transfer("mt103_cbn.aspx")
        End If

        Dim es As New easyrtgs

        Dim charges As Decimal = 0D
        Dim vatRate As Decimal = 0.05D

        If CheckBox1.Checked Then
            If TextBox6.Text = String.Empty Then

                charges = 0
            Else
                charges = Val(TextBox6.Text)

            End If

        Else

            acctPrincipal = t24Obj.GetAccountInfoByAccountNumber(TextBox1.Text)
            Dim isstaff As Boolean = False
            If acctPrincipal IsNot Nothing Then
                isstaff = acctPrincipal.AccountType.Trim().ToLower().Contains("staff")
            End If

            If isstaff Then
                charges = Convert.ToDecimal(es.getStaffCharges())
            Else
                charges = Convert.ToDecimal(es.getCustomerCharges())
            End If
        End If

        If String.IsNullOrEmpty(txtBenAccount103.Text) Then
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & String.Format("Beneficiary Account Number is empty") & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub
        End If




        'Check the balance with the transaction
        Dim transAmt As Decimal = Convert.ToDecimal(TextBox3.Text)
        Dim bal As Decimal = acctPrincipal.UsableBal 'Convert.ToDecimal(lblBalance103.Text)
        Dim vat As Decimal = 0D
        If Not String.IsNullOrEmpty(TextBox6.Text) Then
            vat = vatRate * charges
        Else
            vat = vatRate * charges
        End If

        If bal < (vat + charges + transAmt) Then
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & String.Format("Account balance: {0} is not sufficient for transaction charges {1}.", bal.ToString("0,000.00"), (vat + charges).ToString("0,000.00")) & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub
        End If



        If Val(TextBox3.Text) < 0 Then
            Label2.ForeColor = Drawing.Color.Red
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Amount cannot be a Negative value." & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub

        End If


        If Val(Session("bal").ToString().Replace(",", "")) < Val(Val(TextBox3.Text) + charges + (charges * vatRate)) Then


            Label2.ForeColor = Drawing.Color.Red
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='470'>" & "Customer's Balance cannot handle this Transaction. N " & FormatNumber(Val(TextBox3.Text) + charges, 2) & " is Required.</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Exit Sub


        End If


        Try

            Button5.Enabled = False


        Catch ex1 As Exception
            Gadget.LogException(ex1)
            Label2.ForeColor = Drawing.Color.Red
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Error: Cannot post this transaction." & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Button5.Text = "Post Transaction"
            Button5.Enabled = True

            Exit Sub

        End Try



        Dim conn As New Data.SqlClient.SqlConnection(es.sqlconn1())
        Dim statusName As String = easyrtgs.TREASURY_PENDING_STATUS 'Set it to Authorize-Pending so that when the financial entry (which starts as "Authorize" is posted successfully, this will become "Authorized" and then will go to Treasury straight away (see mytransactions.aspx for the Status code that shows pending transactions for Treasury)
        Dim newstatusName As String = statusName
        Dim msgtype As String = String.Empty
        Dim msgvariant As String = String.Empty
        Dim transactionType As String = easyrtgs.TRANSTYPE_CBN
        msgtype = easyrtgs.MESSAGETYPE_MT103.ToString()
        If Session("variant") IsNot Nothing Then
            msgvariant = Session("variant").ToString()
        Else
            msgvariant = String.Empty ' Session("variant").ToString()
        End If


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
        t.Customer_name = Server.HtmlEncode(Session("cname").Trim.Replace("'", "`"))
        t.Customer_account = Session("acc")
        t.amount = Server.HtmlEncode(FormatNumber(TextBox3.Text.Trim, 2))
        t.charges = Server.HtmlEncode(FormatNumber(charges, 2))
        t.Remarks = Server.HtmlEncode(tid & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-"))
        t.status = statusName
        t.Uploaded_by = Session("name")
        t.Uploaded_date = DateTime.Now
        t.Branch = Session("branch")
        t.Instruction = instruction
        t.Beneficiary = Server.HtmlEncode(txtBenName103.Text)
        t.Beneficiary_Bank = Server.HtmlEncode(DropDownList1.SelectedItem.Text & ":" & DropDownList1.SelectedItem.Value)
        t.Beneficiary_account = Server.HtmlEncode(txtBenAccount103.Text)
        t.date = DateTime.Now
        t.MessagetypeID = msgTypeID ' Message type id for mt103
        t.MessageVariantID = msgVarID 'Message variant id for mt103
        t.RequiresCustCareApproval = 0 ' Initiated by TROPS, does not go for customer care approval.
        t.MailSent = 0
        t.RequestingBranch = Session("branch")
        t.RequestingBranchAccount = ""
        t.PostConfirmationStatus = ""
        t.TransactionType = transactionType ' easyrtgs.TRANSTYPE_CBN

		Dim transactionID As String = t.Save(False)

        If (transactionID <> "-1") Then 'It was saved successfully.
            'Create a List of PostingEntry
            Dim entryList As New List(Of PostingEntry)
            Dim isAllEntriesInserted As Boolean = False
            Dim principalEntry As New PostingEntry()
            principalEntry.TransactionID = t.TransactionID
            principalEntry.EntryDate = DateTime.Now
            principalEntry.ErrorText = String.Empty
            principalEntry.CustomerName = Server.HtmlEncode(Session("cname").Trim.Replace("'", "`"))
            principalEntry.Amount = Convert.ToDecimal(Server.HtmlEncode(FormatNumber(TextBox3.Text.Trim, 2)))
            principalEntry.Officer = Session("name")
            principalEntry.Supervisor = Session("name")
			principalEntry.CrCommAcct = esy.getRTGSSuspense(esy.checkisIMAL(transactionID))
			principalEntry.DrCustAcct = Session("acc").ToString()
            principalEntry.expl_code = esy.getExplCode()
            principalEntry.Remarks = esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(tid & "-" & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-"))))
            principalEntry.Source = "EasyRTGS"
            principalEntry.MFNo = Session("branch")
            principalEntry.Status = "Authorize"
            entryList.Add(principalEntry)


            'T
            Dim chargeEntry As New PostingEntry()
            chargeEntry.TransactionID = t.TransactionID
            chargeEntry.EntryDate = DateTime.Now
            chargeEntry.ErrorText = String.Empty
            chargeEntry.CustomerName = Server.HtmlEncode(Session("cname").Trim.Replace("'", "`"))
            'chargeEntry.Amount = Convert.ToDecimal(Server.HtmlEncode(FormatNumber(TextBox6.Text.Trim, 2)))
            chargeEntry.Amount = Convert.ToDecimal(Server.HtmlEncode(FormatNumber(charges, 2)))
            chargeEntry.Officer = Session("name")
            chargeEntry.Supervisor = Session("name")
			chargeEntry.CrCommAcct = esy.getRTGSSuspense(esy.checkisIMAL(transactionID))
			chargeEntry.DrCustAcct = Session("acc").ToString()
            chargeEntry.expl_code = esy.getExplCode()
            chargeEntry.Remarks = esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(tid & "-" & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-"))))
            chargeEntry.Source = "EasyRTGS"
            chargeEntry.MFNo = Session("branch")
            chargeEntry.Status = "Authorize"
            entryList.Add(chargeEntry)


            Dim vatEntry As New PostingEntry()
            vatEntry.TransactionID = t.TransactionID
            vatEntry.EntryDate = DateTime.Now
            vatEntry.ErrorText = String.Empty
            vatEntry.CustomerName = Server.HtmlEncode(Session("cname").Trim.Replace("'", "`"))
            'vatEntry.Amount = Convert.ToDecimal(Server.HtmlEncode(FormatNumber(TextBox6.Text.Trim, 2) * vatRate))
            vatEntry.Amount = Convert.ToDecimal(Server.HtmlEncode(FormatNumber(charges, 2) * vatRate))
            vatEntry.Officer = Session("name")
            vatEntry.Supervisor = Session("name")
			vatEntry.CrCommAcct = esy.getRTGSSuspense(esy.checkisIMAL(transactionID))
			vatEntry.DrCustAcct = Session("acc").ToString()
            vatEntry.expl_code = esy.getExplCode()
            vatEntry.Remarks = esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(tid & "-" & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-"))))
            vatEntry.Source = "EasyRTGS"
            vatEntry.MFNo = Session("branch")
            vatEntry.Status = "Authorize"
            entryList.Add(vatEntry)

            isAllEntriesInserted = PostingEntry.SaveInTransaction(entryList, es.conn1())

            If Not isAllEntriesInserted Then
                Dim etran As New EasyRtgTransaction(t.TransactionID)
                etran.Delete()
                Button5.Enabled = True
            Else
                esy.createAudit(tid & " was posted by " & Session("name"), Now)
                esy.TransactionAudit(tid & " was posted by " & Session("name"), Now, tid)


                'notify authorizer
                Dim body As String = ""
                body = "Sir/Madam" & "<br>" & "<br>" & "This mail is a request for you to authorize a Third Party Transfer Transaction . Details of this transaction are as follows;" & "<br>" &
                "Customer: " & txtCustName103.Text & "<br>" & "Amount: " & FormatNumber(TextBox3.Text, 2) & "<br>" & " Account Number: " & TextBox1.Text.Trim() & "<br><br>" & "Click on the link below to authorize this transaction. <br><br><a href='http://" & System.Configuration.ConfigurationManager.AppSettings("serverip") & "/" & System.Configuration.ConfigurationManager.AppSettings("AppVirDir") & "/default.aspx?action=authorize&tid=" & tid & "'>Authorize this Transaction</a> <br><br>Thank You"




                Label2.ForeColor = Drawing.Color.Green
                Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction was posted.(" & tid & ")" & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
                Button5.Text = "Post Transaction"
                Button5.Enabled = True
                TextBox1.Text = ""
                txtCustName103.Text = ""
                TextBox3.Text = ""
                TextBox4.Text = ""
                lblBalance103.Text = ""
                Button5.Enabled = True
                Server.Transfer("../../postsuccess.aspx", False)

            End If
        Else 'The main transaction could not be saved. Notify the user.
            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Submission Failed." & "</td><td valign='middle' width='30'><img src='../../images/bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
            Button5.Enabled = True

            Exit Sub
        End If



    End Sub
    Protected Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            TextBox6.Visible = True
        Else
            TextBox6.Visible = False
        End If
    End Sub
	Protected Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

	End Sub
End Class
