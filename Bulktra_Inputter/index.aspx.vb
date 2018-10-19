
Partial Class index
    Inherits System.Web.UI.Page
    Function getInputterUrl() As String
        Return System.Configuration.ConfigurationManager.AppSettings("InputterUrl")

    End Function

    Function getHOPUrl() As String
        Return System.Configuration.ConfigurationManager.AppSettings("AuthorizerUrl")

    End Function

    Function getApproverUrl() As String
        Return System.Configuration.ConfigurationManager.AppSettings("ApproverUrl")

    End Function
End Class
