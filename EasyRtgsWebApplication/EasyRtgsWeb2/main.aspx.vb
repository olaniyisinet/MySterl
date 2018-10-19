Imports System.Net
Imports System.Data
Imports BankCore
Imports BankCore.t24
Imports Newtonsoft.Json

Partial Class main
	Inherits System.Web.UI.Page
	Private tid As String = ""
	Private ess As New easyrtgs
	Private ledcode As String = ""
	Private instruction As String = ""
	Private t24 As New T24Bank

	Public Function generateRef() As String
		Dim ref As String = ConfigurationManager.AppSettings("REF_PREFIX") & DateTime.Now.ToString("yyMMddHHmmss") & GenerateRndNumber(1)
		Return ref
	End Function

	Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init


		tid = EasyRtgTransaction.generateRef()
		TextBox6.Visible = False
		DropDownList1.Items.Insert(0, New ListItem("--Please select--", "000"))
		For Each dr As Data.DataRow In ess.getBanks().Tables(0).Rows
			DropDownList1.Items.Add(New ListItem(dr("bank_name").ToString, dr("bank_code").ToString))
		Next


	End Sub
	'Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
	'    Dim rnd As New System.Random(Second(Now))

	'    tid = generateRef()
	'    TextBox6.Visible = False
	'    DropDownList1.Items.Insert(0, New ListItem("--Please select--", "000"))
	'    For Each dr As Data.DataRow In ess.getBanks().Tables(0).Rows
	'        DropDownList1.Items.Add(New ListItem(dr("bank_name").ToString, dr("bank_code").ToString))
	'    Next


	'End Sub

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

		If Session("uname") = "" Then

			Server.Transfer("default.aspx")

		Else
			Label1.Text = Session("name")

			Label4.Text = "N " & ess.getCustomerCharges()
			Label5.Text = "N " & ess.getStaffCharges()
		

		End If

		''Create this session variable on first load
		'If Not IsPostBack Then
		'    Session("doNameQuery") = True
		'End If

		Dim role As String = Session("role").ToString()
		Dim transferType As String = Session("transtype").ToString()
		ModifyUserInterfaceByRoleAndTransferType(role, transferType)


		If Session("role") = "Authorizer" Then

			Server.Transfer("mytransactions.aspx")
		End If


		If Session("role") = "Treasury" Then

			Server.Transfer("mytransactions.aspx")
		End If



		If Session("role") = "Administrator" Then

			Server.Transfer("admin.aspx")
		End If

	End Sub
	Public Function GenerateRndNumber(ByVal cnt As Integer) As String
		Dim key2 As String() = {"0", "1", "2", "3", "4", "5",
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

	Sub benefName(ByVal sender As Object, ByVal e As System.EventArgs)

		Dim bank_code As String = DropDownList1.SelectedValue
		Dim accountNum As String = TextBox7.Text

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

			TextBox5.Visible = True
			TextBox5.Text = name

		Else
			LinkButton1.Visible = True
			Label2.ForeColor = Drawing.Color.Red
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>Error: " & "Could not retrieve the name of this account: Message: " & errorMsg & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
		End If

	End Sub



	Public Sub show2(ByVal sender As Object, ByVal e As System.EventArgs)
		Dim inputAcct As String = TextBox1.Text
		Dim ds As DataSet = New DataSet()
		Dim outputNuban As String = String.Empty
		Dim acctName As String = String.Empty
		Dim availBal As String = String.Empty
		Dim oldAcctNumberFull As String = String.Empty
		Dim sep As String = "-"
		Dim g As Gadget = New Gadget()
		Label2.Text = ""

		Try
			Dim ac As String = ""
            Dim acctObj As IAccount = Nothing
            Dim resp


            If inputAcct.StartsWith("05") Then

				If inputAcct.Trim().Length = 10 Then
					Dim imal As iMalWebservice = New iMalWebservice()
                    '	imal.getAccountDetails(inputAcct)
                    Try
                        resp = JsonConvert.DeserializeObject(Of AccountDetailsResp)(imal.getAccountDetails(inputAcct))
                    Catch ex As Exception
                        Label2.Text = "<font color=red>Invalid IMal Account Number.</font>"
                        Return
                    End Try

                    acctName = resp.name
					Dim cle_bal As Decimal
					Dim for_amt As Decimal
					Dim bal_lim As Decimal
					Dim tot_blo_fund As Decimal
					Dim risk_lim As Decimal
					availBal = resp.availableBalance
					availBal = availBal * -1
					oldAcctNumberFull = inputAcct 'should be modified later
					Session("acc") = inputAcct
					Session("ledcode") = resp.glCode
					Session.Timeout = 30
					Label3.Text = Strings.FormatNumber(availBal, 2)
					TextBox2.Text = acctName
					Session("bal") = Strings.FormatNumber(availBal, 2)
					Session("cname") = TextBox2.Text
					Session.Timeout = 10
					Button3.Value = "Get Customer Data"
					Button3.Disabled = False
					Label24.Text = "Enter Your iMAL Username:"
					Label25.Text = "Enter Your iMAL password:"
				Else
					Label2.Text = "<font color=red>Please enter 10 digits iMAL account number.</font>"
				End If
			Else

				If inputAcct.Trim().Length = 10 Then
					ac = inputAcct.Trim()
					acctObj = t24.GetAccountInfoByAccountNumber(inputAcct)
				Else
					ac = inputAcct.Trim()

					If inputAcct.Contains("/") Then
						Dim acnparts As String() = inputAcct.Split("/")
						Dim bracode As String = acnparts(0)
						Dim cusnum As String = acnparts(1)
						Dim curcode As String = acnparts(2)
						Dim ledcode As String = acnparts(3)
						Dim subacctcode As String = acnparts(4)
						Dim paddedBanksFormatAcctNumber As String = New Util1s().getAccountNo(bracode, cusnum, curcode, ledcode, subacctcode)
						acctObj = t24.GetAccountInfoByAccountNumber(paddedBanksFormatAcctNumber)
					Else
						Label2.Text = "<font color=red>Please use this character: / to separate the branch code, customer number, currency code, ledger code and subacctcode.</font>"
						Return
					End If
				End If

				outputNuban = acctObj.AccountNumberRepresentations(Account.NUBAN).Representation
				acctName = acctObj.CustomerName
				Dim cle_bal As Decimal
				Dim for_amt As Decimal
				Dim bal_lim As Decimal
				Dim tot_blo_fund As Decimal
				Dim risk_lim As Decimal
				availBal = acctObj.UsableBal
				oldAcctNumberFull = outputNuban
				Session("acc") = oldAcctNumberFull
				Session("ledcode") = acctObj.ProductCode
				Session.Timeout = 30
				Label3.Text = Strings.FormatNumber(availBal, 2)
				TextBox2.Text = acctName
				Session("bal") = Strings.FormatNumber(availBal, 2)
				Session("cname") = TextBox2.Text
				Session.Timeout = 10
				Button3.Value = "Get Customer Data"
				Button3.Disabled = False
				Label24.Text = "Enter Your T24 sign on name:"
				Label25.Text = "Please Enter Your Token Password:"
			End If

		Catch ex As Exception
			Gadget.LogException(ex)
			Label2.ForeColor = Drawing.Color.Red
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>" & ex.Message() & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
		End Try
	End Sub


	'Sub show2(ByVal sender As Object, ByVal e As System.EventArgs)
	'    Label2.Text = ""
	'    Dim bnk As New bank.bank


	'    Try
	'        If IsPostBack Then

	'            Dim ac As String = ""
	'            '   Dim account As String=  TextBox5.Text & textbox6.Text & TextBox7.Text & textbox8.Text & TextBox9.Text

	'            If TextBox1.Text.Trim.Length = 10 Then
	'                ac = getOldAcc(TextBox1.Text.Trim)

	'            Else
	'                ac = TextBox1.Text.Trim

	'            End If


	'            Dim nuban As String = ""



	'            For Each dr As Data.DataRow In bnk.getAccount(ac).Tables(0).Rows


	'                Session("acc") = dr("BC") & "-" & dr("CN") & "-" & dr("CC") & "-" & dr("LC") & "-" & dr("SC")
	'                Session("ledcode") = dr("LC")
	'                Session.Timeout = 30


	'                For Each drN As Data.DataRow In bnk.getNuban(dr("BC"), dr("CN"), dr("CC"), dr("LC"), dr("SC")).Tables(0).Rows
	'                    nuban = drN("MAP_ACC_NO").ToString

	'                Next


	'            Next

	'            TextBox2.Text = getAccountName(nuban)


	'            Dim accbal As New Data.DataSet
	'            accbal = bnk.getBalance(ac)

	'            For Each dr As Data.DataRow In accbal.Tables(0).Rows
	'                Label3.Text = FormatNumber(dr("cle_bal"), 2)
	'                Session("bal") = FormatNumber(dr("cle_bal"), 2)
	'                Session("cname") = TextBox2.Text
	'                Session.Timeout = 10
	'            Next

	'            Button3.Value = "Get Customer Data"
	'            Button3.Disabled = False

	'        End If

	'    Catch ex As Exception
	'        Gadget.LogException(ex)
	'        If ex.Message.Contains("TNS") Or ex.Message.Contains("OracleConnection") Then
	'            Label2.ForeColor = Drawing.Color.Red
	'            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>" & "Cannot connect to BANKS" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"

	'        Else

	'            Label2.ForeColor = Drawing.Color.Red
	'            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>" & ex.Message() & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"

	'        End If
	'    End Try



	'End Sub

	Function getAccountName(ByVal accno As String) As String
		Dim sqlstr As String = IO.File.ReadAllText(Server.MapPath("./accnamesql.txt")).Replace("%nuban%", accno)
		Dim acct As IAccount = Nothing
		Dim t24 As New T24Bank()
		Dim ds As New Data.DataSet
		Dim accname As String = ""
		acct = t24.GetAccountInfoByAccountNumber(accno)
		If acct IsNot Nothing Then
			accname = acct.CustomerName
		End If
		'ds = bnk.ReportsPortalDDL(sqlstr)



		Return accname







	End Function
	'Function getOldAcc(ByVal nuban As String) As String
	'    Dim ds As New Data.DataSet
	'    Dim bnk As New bank.bank
	'    Dim oldacc As String = ""



	'    ds = bnk.getNubanAccount(nuban)

	'    For Each dr As Data.DataRow In ds.Tables(0).Rows

	'        oldacc = dr("ACCOUNTNUMBER")
	'    Next

	'    Return oldacc

	'End Function



	Function getOldAcc(ByVal nuban As String) As String
		Dim ds As New Data.DataSet

		Dim oldacc As String = ""

		Dim t24 As New T24Bank()
		Dim acct As IAccount = Nothing

		acct = t24.GetAccountInfoByAccountNumber(nuban)

		If acct IsNot Nothing Then
			oldacc = acct.AccountNumberRepresentations(Account.BANKS).Representation
			If Not String.IsNullOrEmpty(oldacc) Then
				Return oldacc
			Else
				oldacc = acct.AccountNumberRepresentations(Account.NUBAN).Representation
				Return oldacc
			End If
		End If

		'oldacc = bnk.getAccountFromNUBAN(nuban)



		Return oldacc

	End Function



	Sub showprg()


	End Sub


	Protected Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button5.Click
		'New change. First thing is to check for the token
		lblSubmissionText.Text = String.Empty
		Dim token As New TokenValidatorUtil()
		Dim tokenResponse As TokenResponseHolder = token.ValidateToken(txtUsername103.Text, txtToken103.Text)
		Dim appMode As String = ConfigurationManager.AppSettings("APP_MODE").ToString().Trim().ToLower()
		If appMode = "offline" Then
			tokenResponse.IsValid = True
		End If
		If tokenResponse.IsValid Then
			Try
				SubmitMT103Request()
			Catch ex As Exception
				Gadget.LogException(ex)
			End Try
		Else
			lblSubmissionText.Text = tokenResponse.ResponseMessage

		End If

	End Sub

	Protected Sub CheckBox1_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
		If CheckBox1.Checked Then
			TextBox6.Visible = True
		Else
			TextBox6.Visible = False
		End If
	End Sub

	Protected Sub LinkButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LinkButton1.Click
		Button6.Visible = True
		TextBox5.Visible = False
		LinkButton1.Visible = False

	End Sub
	Protected Sub TextBox7_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox7.TextChanged
		'Check whether name enquiry is needed for the selected branch
		If DropDownList1.SelectedIndex = 0 Then
			DropDownList1.Focus()
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>Select a beneficiary bank first.</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
			Exit Sub
		Else
			If Not BankDetail.GetBankDetailByCode(DropDownList1.SelectedValue.ToString().Trim()).IsNameEnquirySupported Then
				'Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>Name enquiry is not supported by this bank.</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
				Exit Sub
			End If
		End If
		Label6.Text = ""
		Dim acctLen As Integer = TextBox7.Text.Length
		If acctLen = 10 Then
			benefName(sender, e)
		Else
			Label6.Text = "Please check the beneficiary account number because it is not 10 digits long."
		End If

	End Sub
	'Protected Sub TextBox7_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox7.TextChanged
	'    If ConfigurationManager.AppSettings("APP_MODE").ToLower() <> "offline" Then
	'        'Check whether name enquiry is needed for the selected branch
	'        If DropDownList1.SelectedIndex = 0 Then
	'            DropDownList1.Focus()
	'            Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>Select a beneficiary bank first.</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
	'            Exit Sub
	'        Else
	'            If Not BankDetail.GetBankDetailByCode(DropDownList1.SelectedValue.ToString().Trim()).IsNameEnquirySupported Then
	'                'Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>Name enquiry is not supported by this bank.</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
	'                Exit Sub
	'            End If
	'        End If
	'        Label6.Text = ""
	'        Dim acctLen As Integer = TextBox7.Text.Length
	'        If acctLen = 10 Then
	'            benefName(sender, e)
	'        Else
	'            Label6.Text = "Please check the beneficiary account number because it is not 10 digits long."
	'        End If
	'    Else
	'        DisableNameEntry(True)
	'        TextBox5.Visible = True
	'        TextBox5.Text = "OSAGBEMI OLUKAYODE"
	'    End If

	'End Sub

	''' <summary>
	''' Check for Empty string.
	''' </summary>
	''' <param name="amount">The amount that was in the request.</param>
	''' <returns>The appropriate status name for the transaction.</returns>
	''' <remarks>if there are any empty string, that means that the request does not have an opinion as regards the status name.</remarks>
	Private Function GetStatusNameForTransAmount(ByVal amount As Decimal) As String
		Dim op As String = ConfigurationManager.AppSettings("CUST_CARE_AMT_OP").ToString()
		Dim thresholdAmt As Decimal = Convert.ToDecimal(ConfigurationManager.AppSettings("CUST_CARE_AMT_LIM").ToString())

		Dim statusName As String = String.Empty
		Select Case op.Trim().ToLower()
			Case ">"
				If amount > thresholdAmt Then
					statusName = "CustomerCare"
				End If
			Case ">="
				If amount >= thresholdAmt Then
					statusName = "CustomerCare"
				End If
			Case "<"
				If amount < thresholdAmt Then
					statusName = "CustomerCare"
				End If
			Case "<="
				If amount <= thresholdAmt Then
					statusName = "CustomerCare"
				End If
			Case Else
				If amount > thresholdAmt Then
					statusName = "CustomerCare"
				End If

		End Select

		Return statusName
	End Function

	Private Sub ModifyUserInterfaceByRoleAndTransferType(ByVal role As String, ByVal transferType As String)
		'TODO: Implement.
		Dim mrole As String = role.Trim().ToLower()
		Dim mtranstype As String = transferType.Trim().ToLower()

		Select Case mtranstype
			Case easyrtgs.TRANSTYPE_CBN.Trim().ToLower()
				DisableNubanCheck(True) 'valRegExpNuban.Enabled = False '
				DisableNameEntry(True) ' TextBox2.ReadOnly = False ' 
				DisableLimitCheck(True) 'hiddenUseAmtLimit.Value = "false" '
			Case easyrtgs.MESSAGETYPE_MT103.Trim().ToLower()
				valRegExpNuban.Enabled = True
				TextBox2.ReadOnly = True
				hiddenUseAmtLimit.Value = "true"
			Case easyrtgs.MESSAGETYPE_MT202.Trim().ToLower()
				valRegExpNuban.Enabled = True
				TextBox2.ReadOnly = True
				hiddenUseAmtLimit.Value = "true"
			Case easyrtgs.TRANSTYPE_INTERBANK
				valRegExpNuban.Enabled = True
				TextBox2.ReadOnly = True
				hiddenUseAmtLimit.Value = "true"
		End Select
	End Sub

	Private Sub DisableNubanCheck(ByVal isnubanCheckDisabled As Boolean)
		valRegExpNuban.Enabled = isnubanCheckDisabled
	End Sub

	Private Sub DisableNameEntry(ByVal isNameEntryDisabled As Boolean)
		'TextBox2.ReadOnly = isNameEntryDisabled

		If isNameEntryDisabled Then
			'Disable NUBAN validator
			RegularExpressionValidator4.Enabled = False
			'Disable lenght for account number
			'
			TextBox7.MaxLength = 25

			TextBox5.ReadOnly = False
		Else
			'Disable NUBAN validator
			RegularExpressionValidator4.Enabled = True
			'Disable lenght for account number
			'
			TextBox7.MaxLength = 10

			TextBox5.ReadOnly = True


		End If

	End Sub

	Private Sub DisableLimitCheck(ByVal isLimitCheckDisabled As Boolean)
		If isLimitCheckDisabled Then

		End If
		hiddenUseAmtLimit.Value = "false"
	End Sub

	Private Sub SubmitMT103Request()

		If Session("acc") Is Nothing Then
			Throw New Exception("The account number is nothing or empty.")
		ElseIf String.IsNullOrEmpty(Session("acc")) Then
			Throw New Exception("The account number is null or empty.")
		End If

		Dim charges As Decimal = 0D
		Dim vatRate As Decimal = 0.05D

		If String.IsNullOrEmpty(TextBox5.Text) Then
			Label2.ForeColor = Drawing.Color.Red
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Missing Beneficiary Name." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
			Exit Sub
		End If

		Dim limit As Decimal = 0D
		limit = Convert.ToDecimal(ConfigurationManager.AppSettings("limit").ToString())

		If ConfigurationManager.AppSettings("APP_MODE").ToLower().Trim() <> "offline" Then
			If Convert.ToDecimal(TextBox3.Text) < limit Then
				CompareValidator1.IsValid = False
				Exit Sub
			End If
		End If

		If Session("bal") Is Nothing Then
			Label2.ForeColor = Drawing.Color.Red
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Incomplete fields." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
			Exit Sub


		End If

		If Session("cname") Is Nothing Then
			Label2.ForeColor = Drawing.Color.Red
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Incomplete Fields." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
			Exit Sub

			Exit Sub
		End If

		If tid Is Nothing Then
			Server.Transfer("main.aspx")


		End If
		Dim es As New easyrtgs



		If CheckBox1.Checked Then
			If TextBox6.Text = String.Empty Then

				charges = 0
			Else
				charges = Val(TextBox6.Text)

			End If

		Else

			Dim t24 As New T24Bank()
			Dim acct As IAccount = t24.GetAccountInfoByAccountNumber(TextBox1.Text)
			Dim isstaff As Boolean = False
			If acct IsNot Nothing Then
				isstaff = acct.AccountType.Trim().ToLower().Contains("staff")
			End If
			'If Session("ledcode") = "9" Then

			'    charges = Convert.ToDecimal(es.getStaffCharges())
			'Else

			'    charges = Convert.ToDecimal(es.getCustomerCharges())

			'End If
			If isstaff Then

				charges = Convert.ToDecimal(es.getStaffCharges())
			Else

				charges = Convert.ToDecimal(es.getCustomerCharges())

			End If
		End If




		If Val(TextBox3.Text) < 0 Then
			Label2.ForeColor = Drawing.Color.Red
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='270'>" & "Amount cannot be a Negative value." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
			Exit Sub

		End If


		If Val(Session("bal").Replace(",", "")) < Val(Val(TextBox3.Text) + charges + (charges * vatRate)) Then


			Label2.ForeColor = Drawing.Color.Red
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='500'><tr><td valign='middle' width='470'>" & "Customer's Balance cannot handle this Transaction. N " & FormatNumber(Val(TextBox3.Text) + charges, 2) & " is Required.</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
			Exit Sub


		End If


		Try

			Button5.Enabled = False

			If Not FileUpload1.FileContent.Length > 1048576 Then
				If FileUpload1.FileName.Contains(".jpg") Or FileUpload1.FileName.Contains(".JPG") Then
					FileUpload1.SaveAs(Server.MapPath("./Instructions/" & tid & ".jpg"))
					instruction = "JPG"
				ElseIf FileUpload1.FileName.Contains(".png") Or FileUpload1.FileName.Contains(".PNG") Then
					FileUpload1.SaveAs(Server.MapPath("./Instructions/" & tid & ".png"))
					instruction = "PNG"

				ElseIf FileUpload1.FileName.Contains(".pdf") Or FileUpload1.FileName.Contains(".PDF") Then
					FileUpload1.SaveAs(Server.MapPath("./Instructions/" & tid & ".pdf"))
					instruction = "PDF"

				Else
					Label2.ForeColor = Drawing.Color.Red
					Label2.Text = "<table id='output' bgcolor='#ccffcc' width='400'><tr><td valign='middle' width='370'>" & "Scanned Instruction must be .JPG/.PNG/.PDF" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
					Button5.Text = "Post Transaction"
					Button5.Enabled = True

					Exit Sub
				End If
			Else
				Label2.ForeColor = Drawing.Color.Red
				Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Scanned Instruction must be less than 1MB" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
				Button5.Text = "Post Transaction"
				Button5.Enabled = True

				Exit Sub
			End If
		Catch ex1 As Exception
			Gadget.LogException(ex1)
			Label2.ForeColor = Drawing.Color.Red
			Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Error: Cannot post this transaction. Uploade PNG, JPEG, or PDF files instead" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
			Button5.Text = "Post Transaction"
			Button5.Enabled = True

			Exit Sub

		End Try


		'Perform token validation here
		Dim username As String = txtUsername103.Text
		Dim token As String = txtToken103.Text

		Dim conn As New Data.SqlClient.SqlConnection(es.sqlconn1())
		Dim statusName As String = "Uploaded"
		Dim newstatusName As String = GetStatusNameForTransAmount(Convert.ToDecimal(TextBox3.Text))

		If Not String.IsNullOrEmpty(newstatusName) And Session("role") <> "TROPS" Then
			statusName = newstatusName
		End If


		Dim msgtype As String = String.Empty
		Dim msgvariant As String = String.Empty


		Dim esy As New easyrtgs
		Dim msgTypeID As Integer = 0
		Dim msgVarID As Integer = 0
		Dim mgdetOb As New MtMessageDetails
		Try

			msgTypeID = mgdetOb.GetMessageTypeIDByName(easyrtgs.MESSAGETYPE_MT103)
			msgVarID = mgdetOb.GetMessageVariantIDByName(String.Empty)
		Catch ex As Exception
			Gadget.LogException(ex)
		End Try




		'audit trail

		Using conn
			conn.Open()
			'Dim trans As Data.SqlClient.SqlTransaction = conn.BeginTransaction()
			Dim trans As Data.SqlClient.SqlTransaction = Nothing
			'Using trans

			Dim cmd As New Data.SqlClient.SqlCommand
			cmd.Connection = conn

			cmd.CommandText = "insert into transactions(transactionid,customer_name,customer_account,amount,charges,remarks,status,uploaded_by,uploaded_date,branch,Instruction,Beneficiary,Beneficiary_bank,Beneficiary_account,date, requiresCustCareApproval, requestingBranch) values" &
			   "(@tid,@cusname,@cusacc,@amount,@charges,@remarks,@uploaded,@uploadby,'" & Now.ToString("yyyy-MM-dd hh:mm tt") & "',@branch,@Instruction,@benef,@benefBank,@benefAcc,@date, @requiresCustCareApproval, @requestingBranch)"



			Try
				'cmd.Transaction = trans
				cmd.Parameters.AddWithValue("@tid", tid)
				cmd.Parameters.AddWithValue("@cusname", Server.HtmlEncode(Session("cname").Trim.Replace("'", "`")))
				cmd.Parameters.AddWithValue("@cusacc", Session("acc"))
				cmd.Parameters.AddWithValue("@amount", Server.HtmlEncode(FormatNumber(TextBox3.Text.Trim, 2)))
				cmd.Parameters.AddWithValue("@charges", Server.HtmlEncode(FormatNumber(charges, 2)))
				cmd.Parameters.AddWithValue("@remarks", Server.HtmlEncode(tid & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-")))
				cmd.Parameters.AddWithValue("@uploadby", Session("name"))
				cmd.Parameters.AddWithValue("@branch", Session("branch"))
				cmd.Parameters.AddWithValue("@Instruction", instruction)
				cmd.Parameters.AddWithValue("@benef", Server.HtmlEncode(TextBox5.Text))
				cmd.Parameters.AddWithValue("@benefBank", Server.HtmlEncode(DropDownList1.SelectedItem.Text & ":" & DropDownList1.SelectedItem.Value))
				cmd.Parameters.AddWithValue("@benefAcc", Server.HtmlEncode(TextBox7.Text))
				cmd.Parameters.AddWithValue("@date", Now.ToString("yyyy-MM-dd"))
				cmd.Parameters.AddWithValue("@uploaded", statusName)
				'cmd.Parameters.AddWithValue("@msgtype", msgTypeID)
				'cmd.Parameters.AddWithValue("@msgvariant", msgVarID)

				If statusName.Trim().ToLower().CompareTo("customercare") = 0 Then
					cmd.Parameters.AddWithValue("@requiresCustCareApproval", 1)
				Else
					cmd.Parameters.AddWithValue("@requiresCustCareApproval", 0)
				End If
				cmd.Parameters.AddWithValue("@requestingBranch", ddlOwnerBranch.SelectedValue.ToString())
				'Using trans
				If cmd.ExecuteNonQuery() > 0 Then
					trans = conn.BeginTransaction()
					Using trans
						Dim nfiuRemarks As String = String.Empty
						Dim legacyRemark As String = String.Empty
						legacyRemark = Server.HtmlEncode(tid & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-"))
						nfiuRemarks = EasyRtgTransaction.BuildNfiuCompliantRemark_T24(tid, legacyRemark)

						Dim isSavedPrincipal As Boolean = False
						Dim isSavedCharges As Boolean = False
						Dim isSavedVat As Boolean = False
						Dim isNextStateUpdated As Boolean = False
						Try
							If newstatusName.Trim().ToLower().CompareTo("customercare") = 0 Then
								Dim sqlUpdate As String = "update transactions set postConfirmationStatus=@newStatus where TransactionID=@tid"
								Dim cmdUpdate As New Data.SqlClient.SqlCommand(sqlUpdate, conn)
								cmdUpdate.Transaction = trans
								cmdUpdate.Parameters.AddWithValue("@tid", tid)
								cmdUpdate.Parameters.AddWithValue("@newStatus", easyrtgs.SERVICE_MANAGER_STATUS)
								If cmdUpdate.ExecuteNonQuery() > 0 Then
									isNextStateUpdated = True
								Else
									isNextStateUpdated = False
								End If
							Else
								isNextStateUpdated = True 'cause i need it in the Commit() check
							End If
						Catch ex As Exception
							Gadget.LogException(ex)
						End Try

						Dim isIMAL As Boolean = False
						Dim account As String = Session("acc").ToString

						If account.StartsWith("05") Then
							isIMAL = True
						Else
							isIMAL = False
						End If


						Dim sql As String = "spSavePostingEntries"
						Dim cmdInsertTransactTemp As New Data.SqlClient.SqlCommand(sql, conn)
						cmdInsertTransactTemp.Transaction = trans
						cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
						cmdInsertTransactTemp.Parameters.AddWithValue("@transid", tid)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(Session("cname").Trim.Replace("'", "`")))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(TextBox3.Text.Trim, 2))))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", Session("name"))
						'cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", Session("name"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", String.Empty)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getRTGSSuspense(isIMAL))
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", Session("acc"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
						'cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(tid & "-" & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-")))))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", nfiuRemarks)
						cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
						cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", Session("branch"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Status", "Pending")

						If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
							isSavedPrincipal = True
						End If

						cmdInsertTransactTemp.Parameters.Clear()

						'Save the charges
						cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
						cmdInsertTransactTemp.Parameters.AddWithValue("@transid", tid)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(Session("cname").Trim.Replace("'", "`")))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(charges, 2))))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", Session("name"))
						'                        cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", Session("name"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", String.Empty)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getRTGSSuspense(isIMAL))
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", Session("acc"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
						'cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(tid & "-" & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-")))))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", nfiuRemarks)
						cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
						cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", Session("branch"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Status", "Pending")

						If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
							isSavedCharges = True
						End If


						cmdInsertTransactTemp.Parameters.Clear()

						'Save the charges
						cmdInsertTransactTemp.CommandType = Data.CommandType.StoredProcedure
						cmdInsertTransactTemp.Parameters.AddWithValue("@transid", tid)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CustomerName", Server.HtmlEncode(Session("cname").Trim.Replace("'", "`")))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Amount", Convert.ToDecimal(Server.HtmlEncode(FormatNumber(charges, 2))) * vatRate) 'calculate the vat on the charge
						cmdInsertTransactTemp.Parameters.AddWithValue("@Officer", Session("name"))
						'                        cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", Session("name"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Supervisor", String.Empty)
						cmdInsertTransactTemp.Parameters.AddWithValue("@CrCommAcct", esy.getRTGSSuspense(isIMAL))
						cmdInsertTransactTemp.Parameters.AddWithValue("@DrCustAcct", Session("acc"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@expl_code", esy.getExplCode())
						'cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", esy.SanitizeRemarks(Convert.ToString(Server.HtmlEncode(tid & "-" & TextBox4.Text.Trim.Replace("'", "`").Replace("&", "-")))))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Remarks", nfiuRemarks)
						cmdInsertTransactTemp.Parameters.AddWithValue("@Source", "EasyRtgs")
						cmdInsertTransactTemp.Parameters.AddWithValue("@Branch", Session("branch"))
						cmdInsertTransactTemp.Parameters.AddWithValue("@Status", "Pending")

						If cmdInsertTransactTemp.ExecuteNonQuery() > 0 Then
							isSavedVat = True
						End If


						If isSavedPrincipal And isSavedCharges And isSavedVat And isNextStateUpdated Then
							Try
								trans.Commit()
								conn.Close()
								conn.Dispose()


								esy.createAudit(tid & " was posted by " & Session("name"), Now)
								esy.TransactionAudit(tid & " was posted by " & Session("name"), Now, tid)


								'notify authorizer
								Dim body As String = ""
								body = "Sir/Madam" & "<br>" & "<br>" & "This mail is a request for you to authorize a Third Party Transfer Transaction . Details of this transaction are as follows;" & "<br>" &
								"Customer: " & TextBox2.Text & "<br>" & "Amount: " & FormatNumber(TextBox3.Text, 2) & "<br>" & " Account Number: " & TextBox1.Text.Trim() & "<br><br>" & "Click on the link below to authorize this transaction. <br><br><a href='http://" & System.Configuration.ConfigurationManager.AppSettings("serverip") & "/" & System.Configuration.ConfigurationManager.AppSettings("AppVirDir") & "/default.aspx?action=authorize&tid=" & tid & "'>Authorize this Transaction</a> <br><br>Thank You"

								Try
									Dim mail As New easyrtgs
									If statusName.Trim().ToLower().CompareTo("customercare") = 0 Then 'if it is the customercare
										mail.sendmail(System.Configuration.ConfigurationManager.AppSettings("mailHost"), mail.getAuthorizerEmailbyName("Customer Care"), Session("email"), "3RD PARTY TRANSACTION CUSTOMER CARE CONFIRMATION REQUEST (TRANSACTION ID: " & tid & ")", body)
									Else
										mail.sendmail(System.Configuration.ConfigurationManager.AppSettings("mailHost"), mail.getAuthorizerEmail(Session("branch")), Session("email"), "3RD PARTY TRANSACTION AUTHORIZATION REQUEST (TRANSACTION ID: " & tid & ")", body)
									End If
								Catch ex As Exception
									Gadget.LogException(ex)
								End Try


								Label2.ForeColor = Drawing.Color.Green
								Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction was posted.(" & tid & ")" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
								Button5.Text = "Post Transaction"
								Button5.Enabled = True
								TextBox1.Text = ""
								TextBox2.Text = ""
								TextBox3.Text = ""
								TextBox4.Text = ""
								Label3.Text = ""

								Server.Transfer("postsuccess.aspx", False)
							Catch ex As Exception
								Gadget.LogException(ex)
								trans.Rollback()
								Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Transaction was posted.(" & tid & ")" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
							End Try
						Else
							trans.Rollback()
						End If
					End Using


				Else
					trans.Rollback()
				End If
				'End Using


			Catch ex As Exception
				Gadget.LogException(ex)
				'trans.Rollback()

				If ex.Message.Contains("duplicate") Then
					Label2.ForeColor = Drawing.Color.Red
					Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Duplicate Transaction ID. Already exist." & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
					Button5.Text = "Post Transaction"
					Button5.Enabled = True

					Exit Sub
				End If

				If ex.Message.Contains("Input String") Then

					Label2.ForeColor = Drawing.Color.Red
					Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & "Error: Cannot Post. Check all fields" & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
					Button5.Text = "Post Transaction"
					Button5.Enabled = True

					Exit Sub

				End If

				Label2.ForeColor = Drawing.Color.Red
				Label2.Text = "<table id='output' bgcolor='#ccffcc' width='300'><tr><td valign='middle' width='270'>" & ex.Message & "</td><td valign='middle' width='30'><img src='images\bad.png' width='20' title='close' onclick='hide();'></td></tr></table>"
				Button5.Text = "Post Transaction"
				Button5.Enabled = True


			End Try

			'End Using

		End Using

	End Sub

	Protected Sub DropDownList1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownList1.SelectedIndexChanged
		'Enable validator
		DisableNameEntry(True)
		Try
			If DropDownList1.SelectedIndex <> 0 Then
				'get the bank code
				Dim bankcode As String = DropDownList1.SelectedValue.ToString()
				'get whether name enquiry is supported for this bank
				Dim bc As BankDetail = BankDetail.GetBankDetailByCode(bankcode)
				If Not bc.IsNameEnquirySupported Then
					DisableNameEntry(False)
				End If
			End If
		Catch ex As Exception
			Gadget.LogException(ex)
			DisableNameEntry(True)
		End Try

	End Sub

	Protected Sub ddlOwnerBranch_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOwnerBranch.DataBound
		ddlOwnerBranch.Items.Insert(0, New ListItem("-Please Select A Branch", "0"))
	End Sub
End Class
