Imports BankCore.t24
Imports BankCore

Partial Class api
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.QueryString("action") = "load" Then
            Dim t24 As New T24Bank()
            Dim acct As IAccount = Nothing
            acct = t24.GetAccountInfoByAccountNumber(Request.QueryString("acc"))
            Response.Clear()
            Response.Write(acct.AccountNumberRepresentations(Account.NUBAN).Representation)
            'For Each dr As Data.DataRow In bnk.getAccount(Request.QueryString("acc")).Tables(0).Rows

            '    Response.Write(dr("FN"))

            'Next


        End If
    End Sub
End Class
