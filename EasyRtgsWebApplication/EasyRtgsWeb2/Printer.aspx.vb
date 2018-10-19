Imports System.Data.SqlClient

Partial Class Printer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'If Session("ctrlex") Is Nothing Then
        '    btnPrintTelex.Visible = False
        '    txtPrintAreaExt.Visible = False
        '    lblErrorExpanded.Visible = True
        'Else
        '    btnPrintTelex.Visible = True
        '    txtPrintAreaExt.Visible = True
        '    lblErrorExpanded.Visible = False

        '    Dim ctrl As Control = CType(Session("ctrlex"), Control)
        '    If TypeOf (ctrl) Is TextBox Then
        '        Dim text As String = CType(ctrl, TextBox).Text
        '        If Not String.IsNullOrEmpty(text) Then
        '            txtPrintAreaExt.Text = text
        '            'PrintHelper.PrintWebControl(txtPrintAreaExt)
        '        End If
        '    End If
        'End If


        'If Session("ctrl") Is Nothing Then
        '    btnPrint.Visible = False
        '    txtPrintArea.Visible = False
        '    lblError.Visible = True
        'Else
        '    btnPrint.Visible = True
        '    lblError.Visible = False
        '    txtPrintArea.Visible = True
        '    Dim ctrl As Control = CType(Session("ctrl"), Control)
        '    If TypeOf (ctrl) Is TextBox Then
        '        Dim text As String = CType(ctrl, TextBox).Text
        '        If Not String.IsNullOrEmpty(text) Then
        '            txtPrintArea.Text = text
        '            'PrintHelper.PrintWebControl(txtPrintArea)
        '        End If
        '    End If

        'End If

        Dim transid As String = Request.QueryString("transid")
        Dim esy As New easyrtgs()
        Using conn As New Data.SqlClient.SqlConnection(esy.sqlconn1())

            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select *  ,name from tblCustCareResponses,tblusers where tblCustCareResponses.custcareofficerUserid=tblusers.username and transactionID=@tid"
            cmd.Parameters.AddWithValue("@tid", transid)
            Dim reader As SqlDataReader = cmd.ExecuteReader()
            If reader.HasRows Then
                While reader.Read()
                    If Not IsDBNull(reader("reason")) Then
                        txtPrintArea.Text = reader("reason").ToString()
                        PrintHelper.PrintWebControl(txtPrintArea)
                        txtPrintArea.Visible = True
                    End If
                End While
            End If
         
        End Using

        'Select Case transid
        '    Case "en"
        '        pnlEncrypted.Visible = True
        '        pnlExpanded.Visible = False

        '        txtPrintArea.Visible = True
        '        btnPrint.Visible = False

        '        txtPrintAreaExt.Visible = False
        '        btnPrintTelex.Visible = False

        '        PrintHelper.PrintWebControl(txtPrintArea)
        '    Case "tel"
        '        pnlEncrypted.Visible = False
        '        pnlExpanded.Visible = True


        '        txtPrintArea.Visible = False
        '        btnPrint.Visible = False

        '        txtPrintAreaExt.Visible = True
        '        btnPrintTelex.Visible = False

        '        PrintHelper.PrintWebControl(txtPrintAreaExt)
        'End Select
    End Sub

    Protected Sub btnPrint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrint.Click
        Dim ctrl As Control = CType(Session("ctrl"), Control)
        If TypeOf (ctrl) Is TextBox Then
            Dim text As String = CType(ctrl, TextBox).Text
            If Not String.IsNullOrEmpty(text) Then
                txtPrintArea.Text = text
                PrintHelper.PrintWebControl(txtPrintArea)
            End If
        End If
    End Sub

    Protected Sub btnPrintTelex_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintTelex.Click
        Dim ctrl As Control = CType(Session("ctrlex"), Control)
        If TypeOf (ctrl) Is TextBox Then
            Dim text As String = CType(ctrl, TextBox).Text
            If Not String.IsNullOrEmpty(text) Then
                txtPrintAreaExt.Text = text
                PrintHelper.PrintWebControl(txtPrintAreaExt)
            End If
        End If
    End Sub
End Class
