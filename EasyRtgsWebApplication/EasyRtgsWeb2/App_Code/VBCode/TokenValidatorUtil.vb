Imports Microsoft.VisualBasic

Public Class TokenValidatorUtil
    Dim tokenValEngine As New TokenServices.TokenValidatorClient
    Public Enum TokenResponseCodes
        SUCCESS = 1
        USER_DOES_NOT_EXIST = 2
        TOKEN_LOCKED = 3
        INVALID_TOKEN_VALUE = 4
        TOKEN_NEEDS_SYNCHRONISATION = 6
    End Enum
    Public Sub New()
        tokenValEngine = New TokenServices.TokenValidatorClient()
    End Sub
    Public Function ValidateToken(ByVal t24signonname As String, ByVal token As String) As TokenResponseHolder
        Dim response As New TokenResponseHolder()

        Dim isValid As Boolean = False
        Dim authRetCode As Integer = 0
        Dim t24tellerid As String = String.Empty

        Dim srv As New Eacbs.banksSoapClient()
        'TODO: validate the username and the token password together
        Try


            Dim t24userid As String = String.Empty

            t24userid = srv.RetrieveUserID(t24signonname.ToUpper())

            authRetCode = tokenValEngine.validatetoken(t24userid, token)

            response.ResponseCode = authRetCode

            Select Case authRetCode
                Case TokenResponseCodes.SUCCESS
                    isValid = True
                    response.IsValid = isValid
                    response.ResponseMessage = "Success"
                Case TokenResponseCodes.USER_DOES_NOT_EXIST
                    isValid = False
                    response.IsValid = isValid
                    response.ResponseMessage = "User Does Not Exist"
                Case TokenResponseCodes.TOKEN_LOCKED
                    isValid = False
                    response.IsValid = isValid
                    response.ResponseMessage = "Token Locked"
                Case TokenResponseCodes.INVALID_TOKEN_VALUE
                    isValid = False
                    response.IsValid = isValid
                    response.ResponseMessage = "Invalid Token Value"
                Case TokenResponseCodes.TOKEN_NEEDS_SYNCHRONISATION
                    isValid = False
                    response.IsValid = isValid
                    response.ResponseMessage = "Token Needs Synchronisation"
                Case Else
                    isValid = False
                    response.IsValid = isValid
                    response.ResponseMessage = "Error. Unknown Response"

            End Select

        Catch ex As Exception
            Try
                Gadget.LogException(ex)
            Catch exlog As Exception

            End Try

            isValid = False
            response.IsValid = isValid

            authRetCode = -111111111

            response.ResponseCode = t24signonname & "-" & authRetCode 'Since an exception occurred, just return a really large negative number

            response.ResponseMessage = "Application Error. Token Authentication Failed."
        End Try


        Return response
    End Function
    'Public Function ValidateToken(ByVal banksTellerId As String, ByVal token As String) As TokenResponseHolder
    '    Dim response As New TokenResponseHolder()

    '    Dim isValid As Boolean = False
    '    Dim authRetCode As Integer = 0
    '    'TODO: validate the username and the token password together
    '    Try
    '        Dim ws As New Eacbs.banksSoapClient()
    '        Dim t24Tellerid As String = String.Empty
    '        Try
    '            t24Tellerid = ws.getT24UserID(banksTellerId)
    '        Catch ex As Exception
    '            Gadget.LogException(ex)
    '        End Try



    '        authRetCode = tokenValEngine.validatetoken(t24Tellerid, token)

    '        Select Case authRetCode
    '            Case TokenResponseCodes.SUCCESS
    '                isValid = True
    '                response.IsValid = isValid
    '                response.ResponseMessage = "Success"
    '            Case TokenResponseCodes.USER_DOES_NOT_EXIST
    '                isValid = False
    '                response.IsValid = isValid
    '                response.ResponseMessage = "User Does Not Exist"
    '            Case TokenResponseCodes.TOKEN_LOCKED
    '                isValid = False
    '                response.IsValid = isValid
    '                response.ResponseMessage = "Token Locked"
    '            Case TokenResponseCodes.INVALID_TOKEN_VALUE
    '                isValid = False
    '                response.IsValid = isValid
    '                response.ResponseMessage = "Invalid Token Value"
    '            Case TokenResponseCodes.TOKEN_NEEDS_SYNCHRONISATION
    '                isValid = False
    '                response.IsValid = isValid
    '                response.ResponseMessage = "Token Needs Synchronisation"
    '            Case Else
    '                isValid = False
    '                response.IsValid = isValid
    '                response.ResponseMessage = "Error. Unknown Response"

    '        End Select

    '    Catch ex As Exception
    '        Try
    '            Gadget.LogException(ex)
    '        Catch exlog As Exception

    '        End Try

    '        isValid = False
    '        response.IsValid = isValid
    '        response.ResponseMessage = "Application Error. Token Authentication Failed."
    '    End Try


    '    Return response
    'End Function
End Class
