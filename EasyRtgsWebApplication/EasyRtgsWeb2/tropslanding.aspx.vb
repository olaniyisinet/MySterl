Imports System.Net

Partial Class main
    Inherits System.Web.UI.Page
    Private tid As String = ""
    Private ess As New easyrtgs
    Private ledcode As String = ""
    Private instruction As String = ""

    Public Function generateRef() As String
        Dim ref As String = ConfigurationManager.AppSettings("REF_PREFIX") & DateTime.Now.ToString("yyMMddHHmmss") & GenerateRndNumber(1)
        Return ref
    End Function



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        If Session("uname") = "" Then

            Response.Redirect("default.aspx")

        Else
            Label1.Text = Session("name")

          

        End If






        If Session("role") = "Authorizer" Then

            Response.Redirect("mytransactions.aspx")
        End If


        If Session("role") = "Treasury" Then

            Response.Redirect("mytransactions.aspx")
        End If



        If Session("role") = "Administrator" Then

            Response.Redirect("admin.aspx")
        End If

    End Sub
    Public Function GenerateRndNumber(ByVal cnt As Integer) As String
        Dim key2 As String() = {"0", "1", "2", "3", "4", "5", _
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
    

    Function getAccountName(ByVal accno As String) As String
        Dim g As New Gadget()
        Return g.getAccountName(accno)

        
    End Function



    Function getOldAcc(ByVal nuban As String) As String
        Dim g As New Gadget()
        Return g.getOldAcct(nuban)
     


    End Function



    Sub showprg()


    End Sub


    
End Class
