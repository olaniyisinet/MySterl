Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Net
Imports System.Net.Mail

Public Class Mailer
    Public mailFrom As String = ConfigurationManager.AppSettings("mailsender").ToString()
    Public mailHost As String = ConfigurationManager.AppSettings("mailserver").ToString()
    Public mailBody As String = ""
    Public mailSubject As String = ""
    Public message As New MailMessage()

    Public Sub New()
        Dim address As New MailAddress(mailFrom)
        message.From = address
    End Sub
    Public Sub New(ByVal email As String)
        Dim address As New MailAddress(email)
        message.From = address
    End Sub

    Public Sub addTo(ByVal email As String)
        'test if only one email
        Dim s As String = email
        s = s.Replace(" ", "")
        s = s.Replace(",", " ")
        s = s.Replace(";", " ")
        Dim words As String() = s.Split(" "c)
        Dim g As New Gadget()
        For Each word As String In words
            If g.checkEmail(word) Then
                Dim address As New MailAddress(word)
                message.[To].Add(address)
            End If
        Next
    End Sub

    Public Sub addCC(ByVal email As String)
        Dim s As String = email
        s = s.Replace(" ", "")
        s = s.Replace(",", " ")
        s = s.Replace(";", " ")
        Dim words As String() = s.Split(" "c)
        Dim g As New Gadget()
        For Each word As String In words
            If g.checkEmail(word) Then
                Dim address As New MailAddress(word)
                message.[To].Add(address)
            End If
        Next
    End Sub

    Public Sub addBCC(ByVal email As String)
        Dim s As String = email
        s = s.Replace(" ", "")
        s = s.Replace(",", " ")
        s = s.Replace(";", " ")
        Dim words As String() = s.Split(" "c)
        Dim g As New Gadget()
        For Each word As String In words
            If g.checkEmail(word) Then
                Dim address As New MailAddress(word)
                message.[To].Add(address)
            End If
        Next
    End Sub

    Public Sub sendTheMail(ByVal admin As String)
        Dim smtpClient As New SmtpClient()
        Dim nc As New NetworkCredential("sterlingbank\ebusiness", "ebusiness")
        Try
            message.Subject = mailSubject
            message.Body += mailBody
            message.IsBodyHtml = True
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = nc
            smtpClient.Host = mailHost
            smtpClient.Send(message)
        Catch ex As Exception

        Finally
        End Try

    End Sub
    Public Function sendTheMail() As Boolean
        Dim smtpClient As New SmtpClient()
        Dim nc As New NetworkCredential("sterlingbank\ebusiness", "ebusiness")
        Try
            message.Subject = mailSubject
            message.Body = "<table width='800' border='0' cellspacing='0' cellpadding='2' style='font-family:Verdana;'>"
            message.Body += "<tr><td>&nbsp;</td>"
            message.Body += "<td><div align='right'><img src='http://www.sterlingbankng.com/images/str_logo.png'></div></td>"
            message.Body += "</tr><tr>"
            message.Body += "<td colspan='2'>&nbsp;</td>"
            message.Body += "</tr><tr>"
            message.Body += "<td colspan='2'>"
            message.Body += mailBody
            message.Body += "</td></tr><tr></table>"
            message.IsBodyHtml = True
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess
            smtpClient.UseDefaultCredentials = False
            smtpClient.Credentials = nc
            smtpClient.Host = mailHost
            smtpClient.Send(message)

            Return True
        Catch ex As Exception

            Return False
        End Try

    End Function
End Class