Imports System.Configuration.ConfigurationManager

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
        Try

            Dim login As New ld.ldap
            Dim level As New ldap
            Dim bnk As New bank.banks
            Dim xm As New System.Xml.XmlDocument
            xm.LoadXml(login.GetInfo(TextBox1.Text.Trim))


            'If String.IsNullOrEmpty(xm.GetElementsByTagName("tellerid").Item(0).InnerText) Then

            '    Label1.Text = "Please Update your Teller ID on the Active Dirctory System. <a href='http://activedirectory/DirectoryUpdate/' target='_blank'>Click Here</a>"

            '    Exit Sub

            'End If

            If login.login(TextBox1.Text.Trim, TextBox2.Text.Trim) Then


                ' get bracode of staff from banks

                Dim bracode As String = ""
                Dim userid As String = ""
                Dim username As String = (TextBox1.Text).ToUpper
                Dim tillid As String = ""
                'bracode = bnk.DATCONV_getBracodeFromTellerId(xm.GetElementsByTagName("tellerid").Item(0).InnerText)
                userid = bnk.RetrieveUserID(username)
                tillid = bnk.getBankTellerID(userid)
                'Select Case bracode.Trim
                '    Case "999"
                '        bracode = "900"


                'End Select



                ' for troubleshooting branches that cannot spool. Enter any branch code
                If TextBox1.Text = "odukoyaoo" Then
                    '' Session("branch") = "223"
                    '' bracode = "223"


                End If

                tillid = 123
                'End of troubleshooting

                'If String.IsNullOrEmpty(bracode) Then

                '    Label1.Text = "This TellerID might be Expired or does not exist"

                '    Exit Sub

                'End If

                Session("name") = xm.GetElementsByTagName("fullname").Item(0).InnerText
                Session("xml") = login.GetInfo(TextBox1.Text.Trim)
                Session("sid") = xm.GetElementsByTagName("staffID").Item(0).InnerText
                Session("fname") = xm.GetElementsByTagName("firstname").Item(0).InnerText
                Session("lname") = xm.GetElementsByTagName("lastname").Item(0).InnerText
                ' Session("branch") = bracode
                Session("dept") = xm.GetElementsByTagName("deptName").Item(0).InnerText
                Session("tid") = tillid
                'Session("tid") = 223
                Session.Timeout = 1225


                'If (Session("tid") <> "") Then
                If Not String.IsNullOrEmpty(Session("tid")) Then

                    Response.Redirect("menu.aspx", False)
                    'Else
                    'Label1.Text = "<div class='ui-widget'><div class='ui-state-error ui-corner-all' style='padding: 0 .7em;'><p><span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em;'></span>You must be BO or SBO to access this portal. </p></div></div>"
                    'End If

                Else
                    Label1.Text = "<div class='ui-widget'><div class='ui-state-error ui-corner-all' style='padding: 0 .7em;'><p><span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em;'></span>Wrong Login details. Try again </p></div></div>"

                End If

            End If
        Catch ex As Exception

            If ex.message.contains("teller") Then
                Label1.Text = "<div class='ui-widget'><div class='ui-state-error ui-corner-all' style='padding: 0 .7em;'><p><span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em;'></span><p>Error:[No Teller ID]. Please update your Teller ID on the Active Directory Service.<a href='http://activedirectory/DirectoryUpdate/' target='_blank'>Click Here</a> </p></div></div>"

            Else
                Label1.Text = "<div class='ui-widget'><div class='ui-state-error ui-corner-all' style='padding: 0 .7em;'><p><span class='ui-icon ui-icon-alert' style='float: left; margin-right: .3em;'></span><p>" & ex.message & "</div></div>"
            End If

            'TODO Change path back before de            
            System.IO.File.AppendAllText(AppSettings("errorlog") & Year(Now) & Month(Now) & Day(Now) & ".txt", Now & ex.ToString & vbCrLf)
   
        End Try

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load


    End Sub
End Class


 