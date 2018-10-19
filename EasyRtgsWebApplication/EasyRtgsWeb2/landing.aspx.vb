Imports System.Net

Partial Class landing
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim role As String = String.Empty
        If Session("role") IsNot Nothing Then
            role = Session("role")
            ShowOptionsForLogonRole(role)
            lblFullnameAndUsername.Text = Session("name").ToString()
        Else
            DoLogout()
        End If
    End Sub

    Private Sub ShowOptionsForLogonRole(ByVal role As String)

        Dim mrole As String = role.Trim().ToLower()
        Select Case mrole
            Case "trops"
                pnlTROPS.Visible = True
                pnlFTO.Visible = False
            Case "inputter"
                'pnlTROPS.Visible = False
                'pnlFTO.Visible = True
                ''22-April-2018
                btnCustomerTransfers_Click(Nothing, Nothing)
            Case Else
                DoLogout()
        End Select
    End Sub

    Private Sub DoLogout()
        Server.Transfer("logout.aspx")
    End Sub

    Protected Sub btnCustomerTransfers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCustomerTransfers.Click
        Session("transtype") = easyrtgs.TRANSTYPE_INTERBANK
        Server.Transfer("main.aspx")
    End Sub

    Protected Sub btnCbnTransfers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCbnTransfers.Click
        Session("transtype") = easyrtgs.TRANSTYPE_CBN
        Server.Transfer("main.aspx")
    End Sub


    Protected Sub rblTropsSelection_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblTropsSelection.SelectedIndexChanged
        ' ShowOptionsForLogonRole(Session("role").ToString())
        ResetForm(Session("role").ToString())

        Dim transtype As String = rblTropsSelection.SelectedValue.ToString()
        Select Case transtype.Trim().ToLower()
            Case "cbn"
                Session("transtype") = easyrtgs.TRANSTYPE_CBN
            Case "interbank"
                Session("transtype") = easyrtgs.TRANSTYPE_INTERBANK
            Case Else
                Return
        End Select
        lblTropSelection.Text = rblTropsSelection.SelectedItem.Text
        lblTropSelection.Visible = True
        rblTropsMessageType.Visible = True
        lblTropsMessage.Visible = True
    End Sub


    Protected Sub rblTropsMessageType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblTropsMessageType.SelectedIndexChanged
        ShowTheFullMt202Types(False)
        ShowInterbankMt202Types(False)
        Dim messageType As String = rblTropsMessageType.SelectedValue.ToString()
        Select Case messageType.Trim().ToLower()
            Case "202"
                Session("messagetype") = "MT202"
                'Check the transfer type to be sure which radio button list of MT202 variants to show
                If Session("transtype").ToString().ToLower() = easyrtgs.TRANSTYPE_CBN.ToLower() Then
                    ShowTheFullMt202Types(True)
                ElseIf Session("transtype").ToString().ToLower() = easyrtgs.TRANSTYPE_INTERBANK.ToLower() Then
                    'pnlMt202Interbank
                    ShowInterbankMt202Types(True)
                End If

            Case "103"
                Session("messagetype") = "MT103"
                'The transfer type and the message type have been set in the appropriate session bariables.
                Server.Transfer("tropsmain.aspx")
            Case Else
                Return
        End Select
    End Sub

    Private Sub ShowTheFullMt202Types(ByVal show As Boolean)
        pnlMT202Types.Visible = show
    End Sub

    Protected Sub rblMT202type_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblMT202type.SelectedIndexChanged

        Select Case rblMT202type.SelectedValue.ToString()
            Case easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR
                Session("variant") = easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR
                Server.Transfer("tropsmain.aspx")
            Case easyrtgs.MESSAGE_TYPE_VARIANT_SDF
                Session("variant") = easyrtgs.MESSAGE_TYPE_VARIANT_SDF
                Server.Transfer("tropsmain.aspx")
            Case easyrtgs.MESSAGE_TYPE_VARIANT_RESERVATION
                Session("variant") = easyrtgs.MESSAGE_TYPE_VARIANT_RESERVATION
                Server.Transfer("tropsmain.aspx")
            Case Else
                'Throw New ArgumentException("Invalid Message Type Variant")
        End Select
    End Sub

    Private Sub ResetForm(ByVal role As String)
        lblTropsMessage.Visible = False
        rblTropsMessageType.Visible = False
        rblTropsMessageType.SelectedIndex = -1 'deselect
        pnlMT202Types.Visible = False
        rblMT202type.SelectedIndex = -1 'deselect
    End Sub

    'Private Sub ShowInterbankMt202Types(ByVal show As Boolean)
    '    If show = True Then
    '        Session("variant") = easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR

    '        Server.Transfer("tropsmain.aspx")
    '    Else
    '        pnlMt202Interbank.Visible = show
    '    End If
    'End Sub
    'Private Sub ShowInterbankMt202Types(ByVal show As Boolean)
    '    If show = True Then
    '        Session("variant") = easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR
    '        Try
    '            Server.Transfer("tropsmain.aspx")
    '        Catch ex As Exception
    '            Gadget.LogException(ex)
    '        End Try

    '    Else
    '        pnlMt202Interbank.Visible = show
    '    End If
    'End Sub
    Private Sub ShowInterbankMt202Types(ByVal show As Boolean)
        If show = False Then
            pnlMt202Interbank.Visible = show
            Exit Sub


        End If
        Session("variant") = easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR

        Response.Redirect("tropsmain.aspx")

    End Sub
    Protected Sub rblMt202InterbankRegular_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblMt202InterbankRegular.SelectedIndexChanged
        Select Case rblMt202InterbankRegular.SelectedValue.ToString()
            Case easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR
                Session("variant") = easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR

                Server.Transfer("tropsmain.aspx")

            Case Else
                Throw New ArgumentException("Invalid Message Type Variant")
        End Select
    End Sub
End Class

