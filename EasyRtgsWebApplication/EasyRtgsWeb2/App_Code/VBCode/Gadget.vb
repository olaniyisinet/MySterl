
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
Imports System.Data.SqlClient
Imports namEnqWs
Imports System.Xml
Imports System.Data
Imports BankCore.t24
Imports BankCore
Imports com.sbpws.utility


Public Class Gadget
    Public Function RemoveSepFromAccountNumber(ByVal acctnum As String, ByVal sepCharToRemove As String, ByVal replacementChar As String) As String
        Return acctnum.Replace(sepCharToRemove, replacementChar)
    End Function
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

    Public Function GetBank() As DataSet
        Dim ds As New DataSet()
        Dim sql As String = "select bank_code, bank_name from tbl_banks where statusFlag=1"
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString
        Dim cn As New SqlConnection(connectionString)
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand(sql, cn)
                Dim cmdAdapter As New SqlDataAdapter(cmd)
                Using cmdAdapter
                    cmdAdapter.Fill(ds)
                    Return ds
                End Using

            Catch ex As Exception
                LogException(ex)
            End Try
        End Using
        Return ds
    End Function

    Public Function GetBankNameFromCode(ByVal bankcode As String) As String
        Dim bankname As String = String.Empty
        Dim sql As String = "SELECT bank_name from tbl_banks where bank_code=@bankcode"
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString
        Dim cn As New SqlConnection(connectionString)
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddWithValue("@bankcode", bankcode)
                Dim dr As SqlDataReader = cmd.ExecuteReader
                Using dr
                    If dr IsNot Nothing AndAlso dr.HasRows Then
                        While dr.Read
                            bankname = dr("bank_name").ToString()
                        End While
                    End If
                End Using

            Catch ex As Exception

            End Try
        End Using
        Return bankname
    End Function
    Public Function DoNameEnquiryOutput(ByVal bank_code As String, ByVal accountNum As String) As String
        Dim name As String = String.Empty
        If Not String.IsNullOrEmpty(accountNum) AndAlso Not String.IsNullOrEmpty(bank_code) AndAlso accountNum.Length = 10 Then
            Dim request As String = String.Empty
            request = BuildXmlMessage(bank_code, accountNum)
            SaveXmlRequest(request)
            request = Encryption.Encrypt(request)
            Dim response As String = String.Empty
            Dim appId As Integer = Convert.ToInt32(ConfigurationManager.AppSettings("APP_ID").ToString)
            Try
                Using ibsWs = New IBSServices
                    response = ibsWs.IBSBridge(request, appId)
                End Using
            Catch ex As Exception
                LogException(ex)
                response = ""

            End Try

            If Not String.IsNullOrEmpty(response) Then
                response = Encryption.Decrypt(response)
                Dim dict As Dictionary(Of String, String) = parseXmlResponse(response)
                If dict IsNot Nothing AndAlso dict.Keys.Count = 2 Then
                    name = "<?xml version='1.0' encoding='UTF-8' ?><response><ResponseCode>" & dict("RESPONSECODE") & "</ResponseCode><ResponseText>" & dict("RESPONSETEXT") & "</ResponseText></response>"
                    Return name
                    'If dict("RESPONSECODE") = "00" Then
                    '    name = "<NAME>" & dict("RESPONSECODE") & "</NAME><TEXT>" & dict("RESPONSETEXT") & "</TEXT>"
                    'Else
                    '    name = String.Empty '  dict("RESPONSETEXT")
                    'End If
                End If
            Else
                name = String.Empty
            End If

        End If
        Return name
    End Function
    ''' <summary>
    ''' Performs name enquiry.
    ''' The response format from this method is 
    ''' </summary>
    ''' <param name="bank_code"></param>
    ''' <param name="accountNum"></param>
    ''' <param name="flag"></param>
    ''' <returns></returns>
    Public Function DoNameQueryNew(ByVal bank_code As String, ByVal accountNum As String, Optional ByVal flag As String = "old") As String
        Dim response As String = String.Empty

        If flag.Trim().ToLower() = "old" Then
            response = OldDoNameQueryNew(bank_code, accountNum)
        ElseIf flag.Trim().ToLower() = "new" Then
            response = NewDoNameQueryNew(bank_code, accountNum)
        End If

        Return response
    End Function

    Private Function NewDoNameQueryNew(bank_code As String, accountNum As String) As String
        Dim name As String = String.Empty
        Dim response As String = String.Empty
        Dim SessionID As String = CreateSessionID()
        Dim DestinationBankCode As String = GetNfpBankCode(bank_code)
        Dim ChannelCode As String = ConfigurationManager.AppSettings("ChannelCode").ToString()
        Dim AccountNumber As String = accountNum
        Dim srv As NfpLiveRef.NewIBSSoapClient = New NfpLiveRef.NewIBSSoapClient()
        Try
            Using (srv)
                response = srv.NameEnquiry(SessionID, DestinationBankCode, ChannelCode, AccountNumber)
            End Using

        Catch ex As Exception
            LogException(ex)
            response = "99:Exception while calling name enquiry operation"
        End Try


        Return response

    End Function

    Private Function CreateSessionID() As String
        Dim sessionId As String = String.Empty
        Dim randomLength As Integer = ConfigurationManager.AppSettings("RandomLength").ToString() '12
        Dim senderBankCode As String = ConfigurationManager.AppSettings("SenderBankCode").ToString()
        Dim timeComponent As String = DateTime.Now.ToString("yyMMddHHmmss")
        Dim serialRandComponent As String = GetRandomComponent(randomLength)

        sessionId = senderBankCode & timeComponent & serialRandComponent

        Return sessionId
    End Function

    Private Function GetRandomComponent(randomLength As Integer) As String
        Dim randStr As String = String.Empty
        Dim randBuilder As New StringBuilder()
        'TODO: get a random sequence of unique numbers of length "randomLength"
        Dim r As Random = New Random()
        Dim n As Integer = r.Next(0, 10) ' between 0 and 9
        randBuilder.Append(n)
        n = r.Next(1, 9) ' between 1 and 8
        randBuilder.Append(n)
        n = r.Next(2, 8) ' between 2 and 7
        randBuilder.Append(n)
        n = r.Next(3, 7) ' between 3 and 6
        randBuilder.Append(n)
        n = r.Next(4, 6) ' between 2 and 5
        randBuilder.Append(n)
        n = r.Next(5, 10) ' between 5 and 9
        randBuilder.Append(n)
        n = r.Next(6, 9) ' between 6 and 8
        randBuilder.Append(n)

        n = r.Next(7, 10) ' between 7 and 9
        randBuilder.Append(n)

        n = r.Next(2, 10) ' between 2 and 9
        randBuilder.Append(n)

        ''
        n = r.Next(1, 4) ' between 1 and 3
        randBuilder.Append(n)

        n = r.Next(4, 10) ' between 4 and 9
        randBuilder.Append(n)

        n = r.Next(3, 8) ' between 3 and 7
        randBuilder.Append(n)

        randStr = randBuilder.ToString()

        Return randStr
    End Function

    Private Function GetNfpBankCode(bank_code As String) As String
        Dim nfpCode As String = String.Empty
        Dim sql As String = "SELECT [nfpBankCode] from tbl_banks where bank_code=@bankcode"
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString
        Dim cn As New SqlConnection(connectionString)
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddWithValue("@bankcode", bank_code)
                Dim dr As SqlDataReader = cmd.ExecuteReader
                If dr.HasRows Then
                    dr.Read()
                    nfpCode = dr(0).ToString()
                End If
            Catch ex As Exception
                LogException(ex)
            End Try
        End Using
        Return nfpCode
    End Function

    Private Function OldDoNameQueryNew(bank_code As String, accountNum As String) As String
        Dim name As String = String.Empty
        Dim key As String = ConfigurationManager.AppSettings("ENCRYPTION_KEY").ToString()
        Dim vector As String = ConfigurationManager.AppSettings("ENCRYPTION_VECTOR").ToString()
        Dim reqTypeId As String = ConfigurationManager.AppSettings("NAME_QUERY_REQ_ID").ToString()
        Dim appid As String = ConfigurationManager.AppSettings("APP_ID").ToString()


        Dim msg As String = XMLTool.GetHeader(DateTime.Now.ToString("yyMMddHHmmss"), reqTypeId)
        msg &= XMLTool.AddTagWithValue("ReferenceID", getReferenceNumber())
        msg &= XMLTool.AddTagWithValue("ToAccount", accountNum)
        msg &= XMLTool.AddTagWithValue("DestinationBankCode", bank_code)
        msg &= XMLTool.GetFooter()

        SaveXmlRequest(msg)

        Dim ws As WSPlumber = New WSPlumber()
        Dim obj As Object = ws.CallIBS(msg, key, vector, appid)
        Dim x As String = String.Empty
        If obj IsNot Nothing Then
            x = CType(obj, String)
        End If

        If Not String.IsNullOrEmpty(x) Then
            Dim r As Response = New Response()
            r.ID = XMLTool.GetNodeData(x, "ReferenceID")
            r.Type = XMLTool.GetNodeData(x, "RequestType")
            r.Code = XMLTool.GetNodeData(x, "ResponseCode")
            r.Text = XMLTool.GetNodeData(x, "ResponseText")

            If r.Code.Trim() = "00" Then
                name = r.Text
            Else
                name = ""
            End If

        End If
    End Function

    Public Function DoNameQueryNewNIP(ByVal bank_code As String, ByVal accountNum As String) As String
        Dim name As String = String.Empty

        Dim key As String = ConfigurationManager.AppSettings("ENCRYPTION_KEY").ToString()
        Dim vector As String = ConfigurationManager.AppSettings("ENCRYPTION_VECTOR").ToString()
        Dim reqTypeId As String = ConfigurationManager.AppSettings("NAME_QUERY_REQ_ID").ToString()
        Dim appid As String = ConfigurationManager.AppSettings("APP_ID").ToString()

        ' Dim request As String = BuildXmlMessage(bank_code, accountNum)
        'SaveXmlRequest(request)

        'RequestType = 105
        'ToAccount = 3017858442
        'DestinationBankCode = 11

        Dim msg As String = XMLTool.GetHeader(DateTime.Now.ToString("yyMMddHHmmss"), reqTypeId)
        msg &= XMLTool.AddTagWithValue("ReferenceID", getReferenceNumber())
        msg &= XMLTool.AddTagWithValue("ToAccount", accountNum)
        msg &= XMLTool.AddTagWithValue("DestinationBankCode", bank_code)
        msg &= XMLTool.GetFooter()

        SaveXmlRequest(msg)

        Dim ws As WSPlumber = New WSPlumber()
        Dim obj As Object = ws.CallIBS(msg, key, vector, appid)
        Dim x As String = String.Empty
        If obj IsNot Nothing Then
            x = CType(obj, String)
        End If

        If Not String.IsNullOrEmpty(x) Then
            Dim r As Response = New Response()
            r.ID = XMLTool.GetNodeData(x, "ReferenceID")
            r.Type = XMLTool.GetNodeData(x, "RequestType")
            r.Code = XMLTool.GetNodeData(x, "ResponseCode")
            r.Text = XMLTool.GetNodeData(x, "ResponseText")

            If r.Code.Trim() = "00" Then
                name = r.Text
            Else
                name = ""
            End If

        End If




        Return name
    End Function
    Public Function DoNameEnquiry(ByVal bank_code As String, ByVal accountNum As String) As String
        Dim name As String = String.Empty
        If Not String.IsNullOrEmpty(accountNum) AndAlso Not String.IsNullOrEmpty(bank_code) AndAlso accountNum.Length = 10 Then
            Dim request As String = String.Empty
            request = BuildXmlMessage(bank_code, accountNum)
            SaveXmlRequest(request)
            request = Encryption.Encrypt(request)
            Dim response As String = String.Empty
            Dim appId As Integer = Convert.ToInt32(ConfigurationManager.AppSettings("APP_ID").ToString)
            Try
                Using ibsWs = New IBSServices
                    response = ibsWs.IBSBridge(request, appId)
                End Using
            Catch ex As Exception
                response = ""

            End Try

            If Not String.IsNullOrEmpty(response) Then
                response = Encryption.Decrypt(response)
                Dim dict As Dictionary(Of String, String) = parseXmlResponse(response)
                If dict IsNot Nothing AndAlso dict.Keys.Count = 2 Then
                    If dict("RESPONSECODE") = "00" Then
                        name = dict("RESPONSETEXT")
                    Else
                        name = String.Empty '  dict("RESPONSETEXT")
                    End If
                End If
            Else
                name = String.Empty
            End If

        End If
        Return name
    End Function

    Private Function BuildXmlMessage(ByVal bank_code As String, ByVal accountNum As String) As String

        '<?xml version="1.0" encoding="UTF-8" ?>
        '<IBSRequest>
        '<ReferenceID>1</ReferenceID>
        '<RequestType>105</RequestType>
        '<ToAccount>1234567890</ToAccount>
        '<DestinationBankCode>012</DestinationBankCode>
        '</IBSRequest>
        Dim refID As Long = getReferenceNumber()

        Dim xmlBuf As New StringBuilder
        xmlBuf.Append("<?xml version='1.0' encoding='UTF-8' ?>")
        xmlBuf.Append("<IBSRequest>")
        xmlBuf.Append(String.Format("<ReferenceID>{0}</ReferenceID>", refID))
        xmlBuf.Append("<RequestType>105</RequestType>")
        xmlBuf.Append(String.Format("<ToAccount>{0}</ToAccount>", accountNum))
        xmlBuf.Append(String.Format("<DestinationBankCode>{0}</DestinationBankCode>", bank_code))
        xmlBuf.Append("</IBSRequest>")

        Return xmlBuf.ToString()
    End Function

    'Private Function getReferenceNumber() As Long
    '    Dim rand As New Random()
    '    Dim randNum As Long = 89236 ' rand.Next()
    '    Return randNum
    'End Function
    Private Function getReferenceNumber() As Long
        Dim rand As New Random()
        Dim refLen As Integer = Convert.ToInt32(ConfigurationManager.AppSettings("REF_LEN").ToString())
        Dim timestring = CStr(CLng(DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalMilliseconds))
        Dim timelen As Integer = timestring.Length

        Dim randNum As Long = CLng(timestring.Substring(timelen - refLen))
        'rand.Next()
        Return randNum
    End Function
    Public Function parseXmlResponse(ByVal response As String) As Dictionary(Of String, String)
        '<?xml version="1.0" encoding="UTF-8">
        '<IBSResponse>
        '<SessionID>232044131009130545625742550477
        '</SessionID>
        '<ReferenceID>1</ReferenceID>
        '<RequestType>105</RequestType>
        '<ResponseCode>00</ResponseCode>
        '<ResponseText>OLOORO OMOTAYO SEGUN</ResponseText>
        '</IBSResponse>
        Dim dictResponse As New Dictionary(Of String, String)
        Dim xmlDoc As New XmlDocument()
        If (response.Contains("&")) Then
            response = response.Replace("&", " AND ")
        End If

        Try
            xmlDoc.LoadXml(response.Trim())

            dictResponse.Add("RESPONSECODE", xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText)
            dictResponse.Add("RESPONSETEXT", xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText)

        Catch ex As Exception
            Dim esy As New easyrtgs

            esy.createAudit(ex.Message & " inside parseXmlResponse: The response value to be parsed is: " & response, Now)
            esy.TransactionAudit(ex.Message & " inside parseXmlResponse: The response value to be parsed is: " & response, Now, "")
            LogException(ex)
        End Try

        Return dictResponse
    End Function

    Public Sub SaveXmlRequest(ByVal xml As String)
        'Dim filename As String = HttpContext.Current.Server.MapPath("xml_requests/" & DateTime.Now.Year & "_" & DateTime.Now.Month & "_" & DateTime.Now.Day & "_" & DateTime.Now.Millisecond & ".txt")
        'File.WriteAllText(filename, xml)
    End Sub
    Private Shared Function GetLogFileNameForToday() As String
        Dim filename As String = String.Empty
        Dim ext As String = ".txt"
        Dim year As String = DateTime.Now.Year
        Dim month As String = DateTime.Now.Month
        Dim day As String = DateTime.Now.Day

        filename = year & "_" & month & "_" & day & ext
        Return filename
    End Function
    Public Const LOG_FILE_NAME As String = "~\logs\errors\"

    Public Const MSG_LOG_FILE_NAME As String = "~\logs\msgs\"
    Public Shared Sub LogException(ByVal ex As Exception)
        If ex IsNot Nothing Then
            Dim targetSite As String = "" 'ex.TargetSite.Name
            Dim message As String = "" 'ex.Message
            Dim stackTrace As String = "" ' ex.StackTrace
            Dim source As String = "" 'ex.Source
            Dim trace As String = "" 'ex.StackTrace

            Try
                message = ex.Message
                targetSite = ex.TargetSite.Name
                stackTrace = ex.StackTrace
                source = ex.Source
                trace = ex.StackTrace
            Catch exc As Exception

            End Try

            Dim innerException As Exception = ex.InnerException

            File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(LOG_FILE_NAME & GetLogFileNameForToday()), _
                               "[Date]>> " & Date.Now & vbCrLf & _
                               "[Method Name]>> " & targetSite & vbCrLf & _
                               "[Message]>> " & message & vbCrLf & _
                               "[Source]>> " & source & vbCrLf & _
                               "[Stack Trace]>> " & trace & vbCrLf)

            If innerException IsNot Nothing Then
                LogException(innerException)
            Else
                File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(LOG_FILE_NAME & GetLogFileNameForToday()), vbCrLf & vbCrLf)
            End If
        Else
            File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(LOG_FILE_NAME & GetLogFileNameForToday()), _
                               "[Date]>> " & Date.Today & vbCrLf & _
                               "[Method Name]>> " & "Exception is NOTHING" & vbCrLf & _
                               "[Message]>> " & "Exception is NOTHING" & vbCrLf & _
                               "[Source]>> " & "Exception is NOTHING" & vbCrLf & _
                               "[Stack Trace]>> " & "Exception is NOTHING" & vbCrLf)
        End If
    End Sub

    Public Function GetBranch() As DataSet

        'Dim ds As New DataSet()
        'Dim srv As New bank.banks()
        'Using srv
        '    ds = srv.getBranchList()

        'End Using
        'Return ds
        'Dim t24 As New T24Bank


        'Dim listOfIBranch As List(Of IBranch) = t24.Branches
        'Dim ds As DataSet = constructDataSetFromListOfIBranch(listOfIBranch)
        'Return ds

        Dim ds As New DataSet()
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("branchConn").ConnectionString)
        Using cn
            Try
                cn.Open()

                Dim cmd As New SqlCommand("select t24branchid as BRACODE, companyname + '-' + t24branchid  as BRANAME from BranchInfo ", cn)

                Dim adapter As New SqlDataAdapter(cmd)

                adapter.Fill(ds)


            Catch ex As Exception
                LogException(ex)
            End Try
        End Using
        Return ds
    End Function

    Function getMt202BranchDetails(ByVal branchcode As String) As BranchDetail
        Dim bdets As BranchDetail = Nothing
        Dim sql As String = "select * from tblMtMessageValues where messageTypeID=(select id from tblMessageTypes where messageType='MT202') and keyName in ('ledgercode', 'currencycode', 'subaccountcode', 'customernumber')"
        Dim cn As New Connect(sql)
        Dim ds As DataSet = cn.query("dets")
        If ds IsNot Nothing Then
            If ds.Tables.Count > 0 Then
                bdets = New BranchDetail
                bdets.BranchCode = branchcode

                If ds.Tables(0).Rows.Count > 0 Then
                    For Each dr As DataRow In ds.Tables(0).Rows
                        Select Case dr("keyName").ToString().Trim().ToLower()
                            Case "ledgercode"
                                bdets.LedgerCode = dr("keyValue").ToString()
                            Case "customernumber"
                                bdets.Cusnum = dr("keyValue").ToString()
                            Case "subaccountcode"
                                bdets.SubAccountCode = dr("keyValue").ToString()
                            Case "currencycode"
                                bdets.CurrencyCode = dr("keyValue").ToString()
                        End Select
                    Next
                End If
            End If
        End If
        Return bdets
    End Function


    Public Function formatAmountForSwift(ByVal amt As String) As String
        Dim fm As String = String.Empty
        'TODO:
        'first clean up the input
        amt = amt.Replace(",", String.Empty)
        If amt.Contains(".") Then
            amt = amt.Replace(".", ",")
        End If
        fm = amt

        Return fm
    End Function

    Public Function UpdateTransactTempSupervisor(ByVal transid As String, ByVal supervisorname As Object) As String
        Dim sql As String = "UPDATE TransactTemp2 SET Supervisor=@SUPERVISOR WHERE TransactionID=@TRANSID"
        Dim cn As New Connect(sql)
        cn.addparam("@SUPERVISOR", supervisorname)
        cn.addparam("@TRANSID", transid)
        Dim i As Integer = cn.query()
        If i >= 0 Then
            Return True
        Else
            Return False
        End If
    End Function


    Shared Function GetSendersCorrespondent() As String

        Dim ftpOutfolder As String = String.Empty


        Dim account = ConfigurationManager.AppSettings("SenderAccount")
        Dim bic = ConfigurationManager.AppSettings("SenderBIC")

        Dim sendersCorr As String = account & vbCrLf & bic
        Return sendersCorr

    End Function

    Function GetCorrespondentBankBIC(ByVal benbank As String) As String()
        Dim retvalArr As String() = Nothing
        If benbank.Contains(":") Then
            benbank = benbank.Split(":"c)(1).Trim()
        Else
            'only old data will not contain : Therefore, let us return a default bank code
            benbank = "070"
        End If
        Dim Sql As String = "SELECT bic, nuban from tbl_banks where bank_code=@benbank" ' & benbank & "'"
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand(Sql, cn)
                cmd.Parameters.AddWithValue("@benbank", benbank)
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                If dr IsNot Nothing AndAlso dr.HasRows Then
                    retvalArr = New String(2) {}
                    While dr.Read
                        retvalArr(0) = dr("bic").ToString().Trim()
                        retvalArr(1) = dr("nuban").ToString().Trim()

                    End While
                End If
            Catch ex As Exception
                Throw ex
            Finally

            End Try
        End Using
        Return retvalArr
    End Function

    'Public Function GetDataRow(ByVal ds As DataSet) As DataRow
    '    Dim dr As DataRow = Nothing
    '    If (ds IsNot Nothing) Then
    '        If ds.Tables.Count > 0 Then
    '            If ds.Tables(0).Rows.Count > 0 Then
    '                dr = ds.Tables(0).Rows(0)
    '            End If
    '        End If
    '    End If
    '    Return dr
    'End Function

    Function Format24HourTime(ByVal twentyfourhrstring As String) As Integer
        Dim hr As Integer = Convert.ToInt32(twentyfourhrstring)
        If hr > 12 Then
            twentyfourhrstring = (hr - 12).ToString()
        End If
        Dim hrRes As Integer = Convert.ToInt32(twentyfourhrstring)
        Return hrRes
    End Function

    Private Function constructDataSetFromListOfIBranch(ByVal listOfIBranch As List(Of IBranch)) As DataSet
        Dim ds As New DataSet()
        If listOfIBranch IsNot Nothing Then
            If listOfIBranch.Count > 0 Then

                Dim dt As New DataTable("BranchInfo")
                Dim dcBraname As New DataColumn("BRANAME")
                dcBraname.DataType = Type.GetType("System.String")

                Dim dcBracode As New DataColumn("BRACODE")
                dcBracode.DataType = Type.GetType("System.String")

                dt.Columns.Add(dcBraname)
                dt.Columns.Add(dcBracode)
                dt.AcceptChanges()

                For Each bra As IBranch In listOfIBranch

                    Dim dr As DataRow = dt.NewRow()
                    dr.Item("BRACODE") = bra.BranchCode
                    dr.Item("BRANAME") = String.Format("{0} - {1}", bra.BranchCode, bra.BranchName)
                  
                    dt.Rows.Add(dr)
                    dt.AcceptChanges()

                Next
                ds.Tables.Add(dt)
                ds.AcceptChanges()
            End If
        End If
        Return ds
    End Function

    Function getOldAcct(ByVal nuban As String) As String
        Dim oldacct As String = String.Empty
        Dim t24 As New T24Bank()
        Dim acct As IAccount = Nothing
        acct = t24.GetAccountInfoByAccountNumber(nuban)
        If acct IsNot Nothing Then
            oldacct = acct.AccountNumberRepresentations(Account.BANKS).Representation
            If Not String.IsNullOrEmpty(oldacct) Then
                Return oldacct
            Else
                oldacct = acct.AccountNumberRepresentations(Account.NUBAN).Representation
            End If
        End If
        Return oldacct
    End Function

    Function getAccountName(ByVal accno As String) As String
        Dim sqlstr As String = IO.File.ReadAllText(HttpContext.Current.Server.MapPath("./accnamesql.txt")).Replace("%nuban%", accno)
        Dim t24 As New T24Bank()
        Dim acct As IAccount = Nothing
        Dim acctname As String = String.Empty
        acct = t24.GetAccountInfoByAccountNumber(accno)
        If acct IsNot Nothing Then
            acctname = acct.CustomerName
        Else
            Throw New Exception("Could not return the account name because the account object returned null")
        End If
        Return acctname
       
    End Function

    Public Function deleteAccountingEntries(ByVal transid As String) As Boolean
        Dim status As Boolean = False
        Dim cn As New Connect("delete from TransactTemp2 where TransactionID=@id")
        cn.addparam("@id", transid)
        Dim deletionCount As Integer = 0
        deletionCount = cn.query()
        If deletionCount >= 0 Then
            status = True
        Else
            status = False
        End If
        Return status

    End Function
    Public Shared Function ExpandBIC(ByVal bic As String, Optional ByVal pattern As Boolean = True) As String
        Dim details As String = String.Empty
        Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("novelTradeConString").ToString)
        Dim cmd As SqlCommand = Nothing
        Dim dr As SqlDataReader = Nothing
        Dim sql As String = String.Empty

        Try
            cn.Open()
            If pattern Then
                sql = "SELECT TOP 1 FI_NAME, DEPT, CITY FROM tblSwiftBicCodes WHERE BIC_CODE LIKE '" & bic & "%' OR SUBSTRING(BIC_CODE, 1, 8) LIKE '" & bic & "%'"
            Else
                sql = "SELECT TOP 1 FI_NAME, DEPT, CITY FROM tblSwiftBicCodes WHERE BIC_CODE='" & bic & "' OR SUBSTRING(BIC_CODE, 1, 8)='" & bic & "'"
            End If
            cmd = New SqlCommand(sql, cn)
            dr = cmd.ExecuteReader()
            If dr.HasRows Then
                While dr.Read
                    details = details & dr("FI_NAME").ToString & vbCrLf
                    details = details & dr("DEPT").ToString & vbCrLf
                    details = details & dr("CITY").ToString
                    details = details & vbCrLf
                End While
            End If
        Catch ex As Exception
            details = "<N/A-Contact Admin>"
        Finally
            If dr IsNot Nothing Then dr.Close()
            If cn IsNot Nothing Then cn.Close()
        End Try

        Return details

    End Function

    Public Function GetDataRow(ByVal ds As DataSet) As DataRow
        Dim dr As DataRow = Nothing
        If (ds IsNot Nothing) Then
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    dr = ds.Tables(0).Rows(0)
                End If
            End If
        End If
        Return dr
    End Function

    Public Function CreateUser(ByVal username As String, ByVal role As String, ByVal bracode As String, ByVal branchName As String, ByVal status As String, ByVal creatorUsername As String) As Boolean

        Dim success_status As Boolean = False
        Dim bname As String = bracode.Trim() & "*" & branchName
        Dim sql As String = String.Empty
        Dim sqlBuilder As New StringBuilder()
        Dim conCheck As New Connect("select count(*) from tblusers where username=@username")
        conCheck.addparam("@username", username)
        Dim count As Integer = conCheck.selectScalar()
        If (count = 0) Then
            sqlBuilder.Append("INSERT INTO  tblusers( username , name , type, Branch , email , Status , Created_By , Date_created  ) ")
            sqlBuilder.Append("VALUES (@username,@name,@type,@Branch,@email,@Status,@Created_By,@Date_created)")
            sql = sqlBuilder.ToString()

            Dim srv As New EwsService.ServiceSoapClient()

            Dim name As String = String.Empty
            Dim email As String = String.Empty

            Dim ds As DataSet = srv.GetADDetails(username)
            name = ds.Tables(0).Rows(0)("fullname").ToString()
            email = ds.Tables(0).Rows(0)("email").ToString()

            Dim connInsert As Connect
            connInsert = New Connect(sql)
            connInsert.addparam("@username", username)
            connInsert.addparam("@name", name)
            connInsert.addparam("@type", role)
            connInsert.addparam("@Branch", bname)
            connInsert.addparam("@email", email)
            'conn.addparam("@Status", "Active")
            connInsert.addparam("@Status", status)
            connInsert.addparam("@Created_By", creatorUsername)
            connInsert.addparam("@Date_created", DateTime.Now)


            Dim i As Integer = connInsert.insert()
            If i > 0 Then
                success_status = True
            Else
                success_status = False
            End If
        Else
            Dim connUpdate As New Connect("update tblusers set status=@status, Branch=@Branch, type=@type where username=@username")
            connUpdate.addparam("@username", username)
            connUpdate.addparam("@status", status)
            connUpdate.addparam("@Branch", bname)
            connUpdate.addparam("@type", role)
            Dim updateCount As Integer = connUpdate.update()
            If updateCount > 0 Then
                success_status = True
            Else
                success_status = False
            End If
        End If


        Return success_status
    End Function
    Public Function SetUserStatusById(ByVal id As Long, ByVal usernameOfActionInitiator As String, ByVal status As String) As Boolean
        Dim sql As String = "update tblusers set status=@status where id=@id and username<>@username"
        Dim retstatus As Boolean = False

        Dim cn As New Connect(sql)
        cn.addparam("@id", id)
        cn.addparam("@status", status)
        cn.addparam("@username", usernameOfActionInitiator)
        Dim count As Integer = cn.update()

        If count > 0 Then
            retstatus = True
        Else
            retstatus = False
        End If
        Return retstatus
    End Function
    Public Function DeleteUserById(ByVal id As Long, ByVal usernameOfActionInitiator As String) As Boolean
        Return SetUserStatusById(id, usernameOfActionInitiator, "Inactive")
    End Function
    Public Function EnableUserById(ByVal id As Long, ByVal usernameOfActionInitiator As String) As Boolean
        Return SetUserStatusById(id, usernameOfActionInitiator, "Active")
    End Function

End Class

