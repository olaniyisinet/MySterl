Imports BankCore.t24
Imports BankCore

Partial Class audit
    Inherits System.Web.UI.Page

    Protected Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim es As New easyrtgs
        Dim s As String = ""
		Dim isIMAL As Boolean = False

		If CheckBox1.Checked Then
			isIMAL = True
		Else
			isIMAL = False
		End If

		s = es.UpdateAdminSettings(txtAcctCbn.Text, txtAcctCharge.Text, txtCustomerCharge.Text, txtStaffCharge.Text, txtRtgsEmailAddress.Text, txtTreasuryEmail.Text, txtAcctSuspense.Text, txtAcctSuspense_202.Text)
        If s = "Updated" Then
            Label4.Text = "Saved Successfully."

			If txtAcctCbn.Text = es.getRTGSAccount(isIMAL) Then
				updateLog(es.getRTGSAccount(isIMAL))

			End If

			If txtAcctCharge.Text = es.getPLAccount(isIMAL) Then
				updateLog(es.getPLAccount(isIMAL))

			End If

			If txtCustomerCharge.Text = es.getCustomerCharges Then
                updateLog(es.getCustomerCharges)

            End If


            If txtStaffCharge.Text = es.getStaffCharges Then
                updateLog(es.getStaffCharges)

            End If

			If txtRtgsEmailAddress.Text = es.getRTGSMail(isIMAL) Then
				updateLog(es.getRTGSMail(isIMAL))

			End If

			If txtTreasuryEmail.Text = es.getTreasuryMail(isIMAL) Then
				updateLog(es.getTreasuryMail(isIMAL))

			End If

			If txtAcctSuspense.Text = es.getRTGSSuspense(isIMAL) Then
				updateLog(es.getRTGSSuspense(isIMAL))

			End If

			If txtAcctSuspense_202.Text = es.getRTGSSuspense202(isIMAL) Then
				updateLog(es.getRTGSSuspense202(isIMAL))
			End If


			IO.File.WriteAllText(Server.MapPath("./config/banks.txt"), TextBox8.Text)


			txtAcctCbn.Text = es.getRTGSAccount(isIMAL)

			txtAcctCharge.Text = es.getPLAccount(isIMAL)

			txtCustomerCharge.Text = es.getCustomerCharges


            txtStaffCharge.Text = es.getStaffCharges

			txtRtgsEmailAddress.Text = es.getRTGSMail(isIMAL)

			txtTreasuryEmail.Text = es.getTreasuryMail(isIMAL)

			txtAcctSuspense.Text = es.getRTGSSuspense(isIMAL)

			txtAcctSuspense_202.Text = es.getRTGSSuspense202(isIMAL)

		Else
            Label4.Text = "Saving Failed."


        End If

    End Sub


    Sub updateLog(ByVal acct As String)
        Dim es As New easyrtgs

        es.InsertLog(Session("name") & " updated a field to " & acct)

    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim es As New easyrtgs

		txtAcctCbn.Text = es.getRTGSAccount(False)

		txtAcctCharge.Text = es.getPLAccount(False)

		txtCustomerCharge.Text = es.getCustomerCharges


        txtStaffCharge.Text = es.getStaffCharges

		txtRtgsEmailAddress.Text = es.getRTGSMail(False)

		txtTreasuryEmail.Text = es.getTreasuryMail(False)

		txtAcctSuspense.Text = es.getRTGSSuspense(False)

		txtAcctSuspense_202.Text = es.getRTGSSuspense202(False)

		TextBox8.Text = IO.File.ReadAllText(Server.MapPath("./config/banks.txt"))


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("role") <> "Administrator" Then
            Response.Redirect("error.aspx")
        End If

    End Sub



	Protected Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
		Dim es As New easyrtgs
		If CheckBox1.Checked Then
			txtAcctCbn.Text = es.getRTGSAccount(True)

			txtAcctCharge.Text = es.getPLAccount(True)

			txtCustomerCharge.Text = es.getCustomerCharges


			txtStaffCharge.Text = es.getStaffCharges

			txtRtgsEmailAddress.Text = es.getRTGSMail(True)

			txtTreasuryEmail.Text = es.getTreasuryMail(True)

			txtAcctSuspense.Text = es.getRTGSSuspense(True)

			txtAcctSuspense_202.Text = es.getRTGSSuspense202(False)
		Else
			txtAcctCbn.Text = es.getRTGSAccount(False)

			txtAcctCharge.Text = es.getPLAccount(False)

			txtCustomerCharge.Text = es.getCustomerCharges


			txtStaffCharge.Text = es.getStaffCharges

			txtRtgsEmailAddress.Text = es.getRTGSMail(False)

			txtTreasuryEmail.Text = es.getTreasuryMail(False)

			txtAcctSuspense.Text = es.getRTGSSuspense(False)

			txtAcctSuspense_202.Text = es.getRTGSSuspense202(False)
		End If
	End Sub
End Class
