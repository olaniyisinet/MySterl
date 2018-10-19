
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Text
Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System

Public Class Gadget
    Public Function getStatus(ByVal input As String) As String
        Dim str As String = "<span"
        Select Case input
            Case "0"
                str += " style='color:orange;'>PENDING"
                Exit Select
            Case "1"
                str += " style='color:GREEN;'>APPROVED"
                Exit Select
            Case "2"
                str += " style='color:RED;'>REJECTED"
                Exit Select
        End Select
        Return str & "</span>"
    End Function

    Public Function getStatus2(ByVal input As String) As String
        Dim str As String = "<span"
        Select Case input
            Case "0"
                str += " style='color:orange;'>PENDING"
                Exit Select
            Case "1"
                str += " style='color:GREEN;'>APPROVED BY CUSTOMER"
                Exit Select
            Case "2"
                str += " style='color:RED;'>DECLINED BY CUSTOMER"
                Exit Select
        End Select
        Return str & "</span>"
    End Function

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

    Public Function checkDate(ByVal year As String, ByVal month As String, ByVal day As String) As DateTime
        'date checker
        Dim datechecker As New DateTime()
        Dim y As Integer = 0
        Dim m As Integer = 0
        Dim d As Integer = 0
        y = Convert.ToInt32(year)
        m = Convert.ToInt32(month)
        d = Convert.ToInt32(day)
        Try
            datechecker = New DateTime(y, m, d)
        Catch ex As Exception
            Dim j As Integer = DateTime.Now.Year - 90
            datechecker = New DateTime(j, 1, 1)
        End Try
        Return datechecker
    End Function
    Public Function checkDate(ByVal dob As String) As DateTime
        'date checker
        Dim datechecker As New DateTime()
        Try
            Dim sep As Char() = {"-"c}
            Dim dt As String() = dob.Split(sep)
            Dim y As Integer = Convert.ToInt32(dt(0))
            Dim m As Integer = Convert.ToInt32(dt(1))
            Dim d As Integer = Convert.ToInt32(dt(2))
            datechecker = New DateTime(y, m, d)
        Catch ex As Exception
            Dim j As Integer = DateTime.Now.Year - 90
            datechecker = New DateTime(j, 1, 1)
        End Try
        Return datechecker
    End Function


    Public Function generatePassword() As String
        Dim key1 As String() = {"b", "c", "d", "f", "g", "h", _
         "j", "k", "m", "n", "p", "q", _
         "r", "t", "v", "w", "x", "y", _
         "z"}
        Dim key2 As String() = {"1", "2", "3", "4", "5", "6", _
         "7", "8", "9"}
        Dim rand1 As New Random()
        Dim txt As String = ""
        For i As Integer = 0 To 6
            txt += key1(rand1.[Next](0, 18))
        Next
        For j As Integer = 0 To 2
            txt += key2(rand1.[Next](0, 8))
        Next
        Return txt
    End Function

    Public Function generateRandomCode(ByVal c As Integer) As String
        Dim key1 As String() = {"b", "c", "d", "f", "g", "h", _
         "j", "k", "m", "n", "p", "q", _
         "r", "t", "v", "w", "x", "y", _
         "z"}
        Dim rand1 As New Random()
        Dim txt As String = ""
        For i As Integer = 0 To c - 1
            txt += key1(rand1.[Next](0, 18))
        Next
        Return txt
    End Function

    Public Function checkEmail(ByVal emailAddress As String) As Boolean
        Dim patternStrict As String = "^(([^<>()[\]\\.,;:\s@\""]+" & "(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@" & "((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}" & "\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+" & "[a-zA-Z]{2,}))$"
        Dim reStrict As New Regex(patternStrict)
        Dim isStrictMatch As Boolean = reStrict.IsMatch(emailAddress)
        Return isStrictMatch

    End Function

    Public Function mobile234(ByVal mobile As String) As String
        If mobile.StartsWith("0") Then
            mobile = mobile.Substring(1, mobile.Length - 1)
            mobile = "234" & mobile
        End If
        ' do something
        If mobile.StartsWith("+") Then
        End If
        Return mobile
    End Function
    Public Function printMoney(ByVal amt As Decimal) As String
        Return amt.ToString("#,###,###,###,##0.00")
    End Function
    Public Function printDate(ByVal dt As DateTime) As String
        Dim dtm As String = dt.ToString("MMM d, yyyy h:mm tt")
        If dtm = "Jan 1, 0001 12:00 AM" OrElse dtm = "Jan 1, 1900 12:00 AM" Then
            dtm = "not set"
        End If
        dtm = dtm.Replace(" 12:00 AM", "")
        Return dtm
    End Function
    Public Function makeMoney(ByVal amt As String) As Decimal
        Dim mny As Decimal
        Try
            mny = Convert.ToDecimal(amt)
        Catch ex As Exception
            mny = CDec(0)
        End Try
        Return mny
    End Function

    Public Function checkFileType(ByVal filename As String) As Boolean
        Dim ext As String = Path.GetExtension(filename).ToLower()
        Dim allowed As String = ".pdf.jpeg.jpg"
        If allowed.Contains(ext) Then
            Return True
        Else
            Return False
        End If
    End Function




    Public Function responseCodes(ByVal code As String) As String
        Dim txt As String = "Unknown Code"
        Select Case code
            Case "00"
                txt = "Approved or completed successfully"
                Exit Select
            Case "03"
                txt = "Invalid sender"
                Exit Select
            Case "05"
                txt = "Do not honor"
                Exit Select
            Case "06"
                txt = "Dormant account"
                Exit Select
            Case "07"
                txt = "Invalid account"
                Exit Select
            Case "08"
                txt = "Account name mismatch"
                Exit Select
            Case "09"
                txt = "Request processing in progress"
                Exit Select
            Case "12"
                txt = "Invalid transaction"
                Exit Select
            Case "13"
                txt = "Invalid amount"
                Exit Select
            Case "21"
                txt = "No action taken"
                Exit Select
            Case "25"
                txt = "Unable to locate record"
                Exit Select
            Case "26"
                txt = "Duplicate record"
                Exit Select
            Case "30"
                txt = "Format error"
                Exit Select
            Case "34"
                txt = "Suspected fraud"
                Exit Select
            Case "35"
                txt = "Contact sending bank"
                Exit Select
            Case "51"
                txt = "No sufficient funds"
                Exit Select
            Case "57"
                txt = "Transaction not permitted to sender"
                Exit Select
            Case "58"
                txt = "Transaction not permitted on channel"
                Exit Select
            Case "61"
                txt = "Transfer Limit Exceeded"
                Exit Select
            Case "63"
                txt = "Security violation"
                Exit Select
            Case "65"
                txt = "Exceeds withdrawal frequency"
                Exit Select
            Case "68"
                txt = "Response received too late"
                Exit Select
            Case "91"
                txt = "Beneficiary Bank not available"
                Exit Select
            Case "92"
                txt = "Routing Error"
                Exit Select
            Case "94"
                txt = "Duplicate Transaction"
                Exit Select
            Case "96"
                txt = "System malfunction"
                Exit Select
        End Select
        Return txt
    End Function
    Public Function RemoveSpecialChars(ByVal str As String) As String
        Dim chars As String() = New String() {",", ".", "/", "!", "@", "#", _
         "$", "%", "^", "&", "*", "'", _
         """", ";", "-", "_", "(", ")", _
         ":", "|", "[", "]"}
        For i As Integer = 0 To chars.Length - 1
            If str.Contains(chars(i)) Then
                str = str.Replace(chars(i), " ")
            End If
        Next
        Return str
    End Function
    Public Function getMime(ByVal input As String) As String
        Select Case input
            Case ".jpg"
                input = "image/jpeg"
                Exit Select
            Case ".jpeg"
                input = "image/jpeg"
                Exit Select
            Case ".pdf"
                input = "application/pdf"
                Exit Select
            Case ".gif"
                input = "image/gif"
                Exit Select
            Case ".png"
                input = "image/png"
                Exit Select
            Case ".tif"
                input = "image/tiff"
                Exit Select
            Case ".tiff"
                input = "image/tiff"
                Exit Select
            Case ".zip"
                input = "application/zip"
                Exit Select
            Case ".zipx"
                input = "application/zip"
                Exit Select
        End Select
        Return input
    End Function

    Public Function validatenum(ByVal number As String) As Boolean
        Dim ichar As Boolean = True
        For i As Integer = 0 To number.Length - 1
            If Not Char.IsNumber(number(i)) Then
                ichar = False
            End If
        Next
        Return ichar
    End Function

    Public Function validateName(ByVal name As String) As Boolean
        Dim patternStrict As String = "\d"
        Dim reStrict As New Regex(patternStrict)
        Dim isStrictMatch As Boolean = reStrict.IsMatch(name)
        Return isStrictMatch
    End Function
End Class