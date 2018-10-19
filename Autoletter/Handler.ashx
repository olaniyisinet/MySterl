<%@ WebHandler Language="VB" Class="Handler" %>

Imports System
Imports System.Web

Public Class Handler : Implements IHttpHandler
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        For i As Integer = 0 To context.Request.Files.Count

            Dim file = context.Request.Files(i)

            file.SaveAs(HttpContext.Current.Server.MapPath("~/photobook/" & context.Request.QueryString("name") & ".jpg"))

        Next
 
        
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class