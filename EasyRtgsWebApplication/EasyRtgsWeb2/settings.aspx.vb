Imports System.IO

Partial Class swift
    Inherits System.Web.UI.Page
    Private esy As New easyrtgs
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not (Session("role") = "Administrator" Or Session("role") = "Treasury") Then
                Response.Redirect("error.aspx")

            End If

            Label1.Text = Session("name")
            Label2.Text = Session("role")

        Catch ex As Exception
            esy.Errorlog(ex.ToString, Now)


        End Try



    End Sub

    'Protected Sub DataGrid2_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles DataGrid2.PageIndexChanged
    '    DataGrid2.CurrentPageIndex = e.NewPageIndex
    '    Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())
    '        conn.Open()
    '        Dim strsql As String = "select * from transactions where " & DropDownList1.SelectedValue & " like @out and status not in ('Authorized','Failed','Uploaded') order by id desc"
    '        Dim cmd As New Data.SqlClient.SqlCommand(strsql, conn)
    '        cmd.Parameters.AddWithValue("@out", "%" & txtBankName.Text.Trim & "%")
    '        Dim da As New Data.SqlClient.SqlDataAdapter(cmd)
    '        Dim ds As New Data.DataSet
    '        da.Fill(ds)
    '        DataGrid2.DataSource = ds
    '        DataGrid2.DataBind()
    '        DataGrid2.Focus()


    '    End Using

    'End Sub

    'Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

    '    'Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())
    '    '    conn.Open()
    '    '    Dim strsql As String = "select * from transactions where " & DropDownList1.SelectedValue & " like @out and status not in ('Authorized','Failed','Uploaded') order by id desc"
    '    '    Dim cmd As New Data.SqlClient.SqlCommand(strsql, conn)
    '    '    cmd.Parameters.AddWithValue("@out", "%" & txtBankName.Text.Trim & "%")
    '    '    Dim da As New Data.SqlClient.SqlDataAdapter(cmd)
    '    '    Dim ds As New Data.DataSet
    '    '    da.Fill(ds)
    '    '    DataGrid2.DataSource = ds
    '    '    DataGrid2.DataBind()
    '    '    DataGrid2.Focus()

    '    'End Using

    'End Sub

    Protected Sub btnSaveBank_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveBank.Click
        clearmessage()
        Dim bankname As String = txtBankName.Text
        Dim bankcode As String = txtBankCode.Text
        Dim bic As String = txtBic.Text
        Dim nuban As String = txtNuban.Text
        Dim isActive As Integer = 1

        Dim mybank As New BankDetail
        mybank.Bank_Name = bankname
        mybank.Bank_code = bankcode
        mybank.Bic = bic
        mybank.Nuban = nuban
        mybank.Status_Flag = isActive
        If mybank.Save() Then
            'saved
            lblSuccess.Text = "Successfully Saved Bank Detail"
            gvBank.DataBind()
        Else
            'not saved
            lblFail.Text = "Could Not Save Bank Detail"
        End If


    End Sub

    Private Sub clearmessage()
        lblFail.Text = String.Empty
        lblSuccess.Text = String.Empty
    End Sub

    Protected Sub odsBankDetail_Deleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsBankDetail.Deleting
        Dim paramsFromPage As IDictionary = e.InputParameters

        paramsFromPage.Remove("bc") 'the parameter from the DeleteParameter declaration of the object datasource
        paramsFromPage.Add("bc", Convert.ToString(paramsFromPage("bank_code")))
        paramsFromPage.Remove("bank_code") ' the parameter fro, the datakey names of the GridView
    End Sub

    Protected Sub btnSaveAdmin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAdmin.Click
        lblAdminFail.Text = String.Empty
        lblAdminSuccess.Text = String.Empty

        Dim adm As AdminSetting
        adm = New AdminSetting
        adm.RTGSAccount = txtRtgsAccount.Text
        adm.PLAccount = txtPLAccount.Text
        adm.ChargesCustomerAmount = Convert.ToDecimal(txtCustomerAmount.Text)
        adm.ChargesStaffAmount = Convert.ToDecimal(txtChargeStaffAmount.Text)
        adm.RTGSEmail = txtRtgsEmail.Text
        adm.TreasuryEmail = txtTreasuryEmail.Text
        adm.RTGSSuspense = txtRtgSuspenseAccount.Text
        adm.VATAccount = txtVATAccount.Text
        adm.VATAmount = 5
        adm.IsActive = IIf(rblActiveSet.SelectedValue = "1", True, False)
        Try
            If adm.Save() Then
                lblAdminSuccess.Text = "Successfully Saved/Updated Admin Entry With ID: " & adm.ID
                gvAdminSettings.DataBind()
                odsAdmin.DataBind()
            Else
                lblAdminFail.Text = "Could not save admin entry."
            End If
        Catch ex As Exception
            lblAdminFail.Text = "There was an error: " & ex.Message
        End Try


    End Sub

    Protected Sub odsAdmin_Deleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsAdmin.Deleting
        Dim paramdict As IDictionary = e.InputParameters
        paramdict.Remove("adminID")
        paramdict.Add("adminID", paramdict("ID"))
        paramdict.Remove("ID")
    End Sub

    Protected Sub downloadFile(ByVal sender As Object, ByVal e As EventArgs)
        If sender IsNot Nothing Then
            If TypeOf (sender) Is LinkButton Then
                Dim bt As LinkButton = CType(sender, LinkButton)
                Dim sendersRef As String = bt.Text
                Dim fs As New FailedSwift(sendersRef)
                Dim swiftMsg As String = fs.SwiftMessage
                If Not String.IsNullOrEmpty(swiftMsg) Then
                    Dim filename As String = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) & ".txt"
                    Dim fullPath As String = Server.MapPath("~/tempfiles/" & filename)
                    Dim destFilename As String = String.Empty
                    Try

                        File.WriteAllText(fullPath, swiftMsg)
                        If File.Exists(fullPath) Then
                            OpenFileForDownload(fullPath)
                        End If
                    Catch ex As Exception

                    Finally
                        'Try
                        '    If File.Exists(fullPath) Then
                        '        File.Delete(fullPath)
                        '    End If
                        'Catch ex As Exception

                        'End Try

                    End Try


                End If
            End If
        End If

    End Sub

    Public Sub OpenFileForDownload(ByVal fileName As String)
        Try

            Dim myfile As New FileInfo(fileName)

            If myfile.Exists Then
                Response.ClearContent()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & myfile.Name)
                Response.AddHeader("Content-Length", myfile.Length.ToString())
                Response.ContentType = "text/xml"
                Response.TransmitFile(myfile.FullName)
                Response.End()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        '{ /* Do nothing */ 
    End Sub

    Public Overrides Property EnableEventValidation() As Boolean
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)

        End Set
    End Property
End Class
