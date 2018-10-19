Imports Microsoft.VisualBasic

Public Class TokenResponseHolder
    Private _isValid As Boolean = False
    Private _responseMessage As String = String.Empty
    Private _responseCode As String = String.Empty

    Public Sub New()

    End Sub

    Public Property IsValid As Boolean
        Get
            Return _isValid
        End Get
        Set(ByVal value As Boolean)
            _isValid = value
        End Set
    End Property


    Public Property ResponseMessage As String

        Get
            Return _responseMessage
        End Get
        Set(ByVal value As String)
            _responseMessage = value
        End Set
    End Property

    Public Property ResponseCode As String
        Get
            Return _responseCode
        End Get
        Set(ByVal value As String)
            _responseCode = value
        End Set
    End Property



End Class
