Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports SwiftLib
Imports System.Data
Imports BankCore.t24
Imports BankCore

Public Class MtMessageDetails
    Public Sub New()

    End Sub
    Public Property ID As Integer
        Get
            Return _id
        End Get
        Set(ByVal value As Integer)
            _id = value
        End Set
    End Property
    Public Property MessageVariantID As Integer
        Get
            Return _messageVariantID
        End Get
        Set(ByVal value As Integer)
            _messageVariantID = value
        End Set
    End Property

    Public ReadOnly Property MessageVariantName As String
        Get
            If _messageVariantID <> 0 Then
                Dim variantName As String = GetMessageVariantNameByID(_messageVariantID)
                Return variantName
            Else
                Return String.Empty
            End If
        End Get

    End Property
    Public Property MessageTypeID As Integer
        Get
            Return _messageTypeID
        End Get
        Set(ByVal value As Integer)
            _messageTypeID = value
        End Set
    End Property

    Public ReadOnly Property MessageTypeName As String
        Get
            Dim messageTypeID As Integer = _messageTypeID
            Dim mname As String = String.Empty
            'Get the message type name fromthe id and return
            mname = GetMessageTypeNameByID(messageTypeID)
            Return mname
        End Get
    End Property

    Public Property KeyName As String
        Get
            Return _keyName
        End Get
        Set(ByVal value As String)
            _keyName = value
        End Set
    End Property


    Public Property KeyValue As String
        Get
            Return _keyValue
        End Get
        Set(ByVal value As String)
            _keyValue = value
        End Set
    End Property

    Public Property DateAdded As DateTime
        Get
            Return _dateAdded
        End Get
        Set(ByVal value As DateTime)
            _dateAdded = value
        End Set
    End Property

    Public Property CreatedBy As String
        Get
            Return _createdBy
        End Get
        Set(ByVal value As String)
            _createdBy = value
        End Set
    End Property

    Public _id As Integer
    Private _messageTypeID As Integer
    Private _messageVariantID As Integer
    Private _keyName As String
    Private _keyValue As String
    Private _dateAdded As DateTime
    Private _createdBy As String

    Public Function GetMessageTypeNameByID(ByVal messageTypeID As Integer) As String
        Dim name As String = String.Empty
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand("select messageType from tblMessageTypes where id=@id", cn)
                cmd.Parameters.AddWithValue("@id", messageTypeID)
                name = CType(cmd.ExecuteScalar(), String)
                cn.Close()
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try
        End Using
        Return name
    End Function

    Public Function GetMessageTypeIDByName(ByVal messageTypeName As String) As Integer
        Dim msgTypeID As Integer = 0
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand("select id from tblMessageTypes where messageType=@mn", cn)
                cmd.Parameters.AddWithValue("@mn", messageTypeName)
                msgTypeID = CType(cmd.ExecuteScalar(), Integer)
                cn.Close()
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try
        End Using
        Return msgTypeID
    End Function

    Public Function GetMessageVariantNameByID(ByVal messageVariantID As Integer) As String
        Dim name As String = String.Empty
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand("select messageVariant from tblMessageVariant where id=@id", cn)
                cmd.Parameters.AddWithValue("@id", messageVariantID)
                name = CType(cmd.ExecuteScalar(), String)
                cn.Close()
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try
        End Using
        Return name
    End Function


    Public Function GetMessageVariantIDByName(ByVal messageVariantName As String) As Integer
        Dim nameID As Integer = 0
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand("select id from tblMessageVariant where messageVariant=@messageVariant", cn)
                cmd.Parameters.AddWithValue("@messageVariant", messageVariantName)
                nameID = CType(cmd.ExecuteScalar(), Integer)

                cn.Close()
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try
        End Using
        Return nameID
    End Function

    Function generateSwiftMt202Message(ByVal transid As String) As String
        Dim t24 As New T24Bank()
        Dim acct As IAccount = Nothing

        Dim robj As New EasyRtgTransaction(transid)
        Dim mt103 As String = String.Empty
        'Get the variant of the MT202
        Dim variantName As String = String.Empty

        variantName = robj.MessageVariantName
        Dim f72Code As String = String.Empty
        Dim f72Narration As String = String.Empty
        Dim f72Prefix As String = String.Empty
        Dim remark As String = String.Empty

        Select Case variantName
            Case easyrtgs.MESSAGE_TYPE_VARIANT_RESERVATION
                f72Code = "CODTYPTR"
                f72Narration = "002"
                f72Prefix = "RESCREDI"

            Case easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR
                f72Code = "CODTYPTR"
                f72Narration = "001"
                f72Prefix = "BNF"

            Case easyrtgs.MESSAGE_TYPE_VARIANT_SDF
                f72Code = "CODTYPTR"
                f72Narration = "032"
                f72Prefix = "BNF"

            Case Else
                f72Code = "CODTYPTR"
                f72Narration = "001"
                f72Prefix = "BNF"

        End Select

        Dim g As New Gadget()
        'TODO: implement generating the MT
        Dim mt202 As Message = Nothing
        mt202 = New Mt202Message()


        Dim f20 As Field = New Field20(robj.TransactionID)
        Dim f21 As Field = New Field21(robj.TransactionID)
        Dim f32a As Field = New Field32A(robj.date.ToString("yyMMdd") & "NGN" & g.formatAmountForSwift(robj.amount.ToString()))
        Dim f53a As Field = New Field53A() 'New Field53A()
        Dim f58a As Field = New Field58A() 'New Field58A()
        Dim f72 As Field = New Field72()


        Dim f53Content As String = String.Empty
        Dim nuban As String = String.Empty
        nuban = robj.Customer_account
        'If robj.Customer_account.Contains("-") Then
        '    'robj.Customer_account = robj.Customer_account.Replace("-", String.Empty)
        '    Dim acctParts() As String = Nothing

        '    acctParts = robj.Customer_account.Split("-")
        '    If acctParts IsNot Nothing Then
        '        Dim bracode As String = acctParts(0)
        '        Dim cusnum As String = acctParts(1)
        '        Dim curcode As String = acctParts(2)
        '        Dim ledcode As String = acctParts(3)
        '        Dim subacctcode As String = acctParts(4)

        '        'get the nuban equivalent

        '        Dim bserv As New bank.banks()

        '        Dim dsAccount As DataSet = bserv.getNuban(bracode, cusnum, curcode, ledcode, subacctcode)
        '        If dsAccount IsNot Nothing Then
        '            If dsAccount.Tables.Count > 0 Then
        '                If dsAccount.Tables(0).Rows.Count > 0 Then
        '                    nuban = dsAccount.Tables(0).Rows(0)("MAP_ACC_NO").ToString()
        '                End If
        '            End If
        '        End If
        '    End If

        'End If
        If Not String.IsNullOrEmpty(nuban) Then
            If robj.Customer_name.Trim().ToUpper() = "NULL" Then
                f53Content = "/" & nuban.ToUpper()
            Else
                f53Content = "/" & nuban.ToUpper() & vbCrLf & robj.Customer_name.ToUpper()
            End If
        Else
            If robj.Customer_name.Trim().ToUpper() = "NULL" Then
                f53Content = "/" & robj.Customer_account.ToUpper()
            Else
                f53Content = "/" & robj.Customer_account.ToUpper() & vbCrLf & robj.Customer_name.ToUpper()
            End If

        End If

        f53a.Content = f53Content.ToUpper()

        Dim f58aContent As String = String.Empty
        f58aContent = "/" & robj.Beneficiary_account.ToUpper() & vbCrLf & BankDetail.GetBankDetailByCode(robj.Beneficiary_Bank.Split(":")(1).ToUpper()).Bic
        f58aContent = f58aContent.ToUpper()
        f58a.Content = f58aContent.ToUpper()

        Dim f72Content As String = String.Empty
        remark = robj.Remarks
        f72Content = BuildField72Content(f72Code, f72Narration, f72Prefix, remark) '"/CODTYPTR/001" & vbCrLf & "/BNF/" & robj.Remarks.ToUpper()
        f72.Content = f72Content.ToUpper()


        mt202.AddField(f20)
        mt202.AddField(f21)
        mt202.AddField(f32a)
        mt202.AddField(f53a)
        mt202.AddField(f58a)
        mt202.AddField(f72)

        mt202.MessagePriorityNumber = GetMessagePriorityNumber(robj.MessagetypeID)
        Dim isvalid As Boolean = False
        mt202.SourceTerminal = String.Empty
        mt202.SourceTerminal = "NAMENGLA".PadRight(12, "X")
        Dim bankcode As String = robj.Beneficiary_Bank.Split(":")(1)
        Dim bdet As BankDetail = BankDetail.GetBankDetailByCode(bankcode)
        mt202.DestinationTerminal = bdet.Bic.PadRight(12, "X") '.Empty 'Get the destination terminal address from the beneficiary bankl

        isvalid = mt202.Validate()

        If isvalid Then
            'This is invalid.
        End If

        mt103 = mt202.Render(Renderable.RenderingMode.Telex)

        Return mt103

    End Function

    Private Function GetMessagePriorityNumber(messagetypeID As Integer) As String
        Dim messagePriorityNumber As String = ""
        Dim Sql As String = "select priorityNumber from tblSwiftMessageSettings where messageID=@id"
        Dim esy As New easyrtgs
        Try
            Using cn = New SqlConnection(esy.sqlconn1())
                cn.Open()
                Dim cmd As New SqlCommand(Sql, cn)
                cmd.CommandType = Data.CommandType.Text
                cmd.Parameters.AddWithValue("@id", messagetypeID)
                messagePriorityNumber = CType(cmd.ExecuteScalar(), String)
            End Using
        Catch ex As Exception
            Gadget.LogException(ex)
        End Try

        Return messagePriorityNumber
    End Function

    Function generateSwiftMt103Message(ByVal transid As String) As String
        Dim robj As New EasyRtgTransaction(transid)
        Dim mt103Text As String = String.Empty
        'Get the variant of the MT202
        Dim variantName As String = String.Empty

        variantName = robj.MessageVariantName
        Dim f72Code As String = String.Empty
        Dim f72Narration As String = String.Empty
        Dim f72Prefix As String = String.Empty
        Dim remark As String = String.Empty

        Select Case variantName
            Case easyrtgs.MESSAGE_TYPE_VARIANT_RESERVATION
                f72Code = "CODTYPTR"
                f72Narration = "002"
                f72Prefix = "RESCREDI"

            Case easyrtgs.MESSAGE_TYPE_VARIANT_REGULAR
                f72Code = "CODTYPTR"
                f72Narration = "001"
                f72Prefix = "BNF"

            Case easyrtgs.MESSAGE_TYPE_VARIANT_SDF
                f72Code = "CODTYPTR"
                f72Narration = "032"
                f72Prefix = "BNF"

            Case Else
                f72Code = "CODTYPTR"
                f72Narration = "001"
                f72Prefix = "BNF"

        End Select

        Dim g As New Gadget()
        'TODO: implement generating the MT
        Dim mt103Obj As Message = Nothing
        mt103Obj = New RtgsMt103Message()
        Dim dr As SqlDataReader = Nothing



        mt103Text = BuildMt103Text(robj)


        Return mt103Text

    End Function
    Private Function BuildField72Content(ByVal f72Code As String, ByVal f72Narration As String, ByVal f72Prefix As String, ByVal transRemark As String) As String
        Dim content As String = String.Empty

        ':72:/CODTYPTR/001
        '/BNF/CASHSWAP STERLING HEAD OFFICE
        If Not String.IsNullOrEmpty(f72Prefix) Then
            content = String.Format("/{0}/{1}" & vbCrLf & "/{2}/{3}", f72Code, f72Narration, f72Prefix, transRemark.ToUpper())
        Else
            content = String.Format("/{0}/{1}" & vbCrLf & "/{2}{3}", f72Code, f72Narration, f72Prefix, transRemark.ToUpper()) 'if the prefix is null, remove the forward slash.
        End If


        Return content
    End Function
    Private Function SanitizeForSwift(ByVal content As String) As String
        Dim sanitize As String = String.Empty

        If Not Field.ContainsInvalidCharacter(content) Then
            sanitize = content
        Else
            sanitize = CleanInvalidCharacters(content)
        End If
        Return sanitize
    End Function
    Public Function GetNuban(ByVal sterlingaccount As String, ByVal sep As String) As String
        Return sterlingaccount
    End Function
    Public Function NubanNumberRetrieval(ByVal BankAccNum As String, ByVal sep As String) As String
        Dim result As String = String.Empty
        Try
            Dim bracode, cusnum, curcode, ledcode, subacctcode As String
            Dim t24 As New T24Bank()
            Dim acct As IAccount = Nothing
            Dim utObj As New Util1s()

            'Use the webservice call
            Dim acctparts() As String = BankAccNum.Split(sep)
            If acctparts IsNot Nothing Then
                If acctparts.Length = 5 Then
                    bracode = acctparts(0)
                    cusnum = acctparts(1)
                    curcode = acctparts(2)
                    ledcode = acctparts(3)
                    subacctcode = acctparts(4)


                    Dim oldref As String = utObj.getAccountNo(bracode, cusnum, curcode, ledcode, subacctcode)
                    acct = t24.GetAccountInfoByAccountNumber(oldref)
                    result = acct.AccountNumberRepresentations(Account.NUBAN).Representation
                    Return result
                End If
            Else
                acct = t24.GetAccountInfoByAccountNumber(BankAccNum)
                result = acct.AccountNumberRepresentations(Account.NUBAN).Representation
                Return result
            End If
        Catch ex As Exception
            Throw ex
        Finally

        End Try

        Return result
    End Function
    Private Function CleanInvalidCharacters(ByVal content As String) As String
        Dim invalidChars() As Char = Field.invalidCharacterArray
        Dim cleaned As String = String.Empty
        Dim strbuf As New StringBuilder
        For Each c As Char In invalidChars
            content = content.Replace("&amp;", "&").Replace("&", "AND").Replace(c, "")
        Next
        cleaned = content
        Return cleaned
    End Function
    Private Function BuildMt103Text(ByVal r As EasyRtgTransaction) As String
        Dim dr As SqlDataReader = Nothing
        Dim text As String = String.Empty
        'If dr.HasRows Then
        'variables for use in the mt103 for rtgs
        Dim sendersRef_f20 As String
        Dim bankOpCode_f23B As String
        Dim instructionCode_23E As String
        Dim transactionType_26T As String
        Dim valDateCurAmt_32A As String
        Dim orderingCust_50K As String
        Dim sendersCorrespondent_53A As String
        Dim acctWithInstitution_57A As String
        Dim detailOfCharges_71A As String
        Dim benCust_59 As String = String.Empty
        Dim senderToReceiverInfo_72 As String
        Dim amt As String = ""
        Dim beneAccount As String = ""
        Dim beneName As String = ""
        Dim remarks As String = ""
        Dim rows As Integer = 5
        Dim cols As Integer = 35
        Dim linePrefix As String = ConfigurationManager.AppSettings("Line Prefix For Field72").ToString()
        Dim appMode As String = ConfigurationManager.AppSettings("APP_MODE").ToString()
        Dim UseTestAddress As String = String.Empty

        If (appMode.Trim().ToLower() = "offline") Then
            UseTestAddress = "true"
        Else
            UseTestAddress = "false"
        End If

        Dim gObj As New Gadget
        'Using dr
        'While dr.Read
        sendersRef_f20 = r.TransactionID ' dr("transactionid").ToString()
        bankOpCode_f23B = "CRED"
        instructionCode_23E = "SDVA"
        transactionType_26T = "001"
        amt = r.amount

        If amt.Contains(",") Then
            amt = amt.Replace(",", String.Empty) 'first remove any comma characters
        End If

        If amt.Contains(".") Then
            amt = amt.Replace(".", ",")
        End If

        'TODO: Uncomment the line below
        valDateCurAmt_32A = Convert.ToDateTime(r.date).ToString("yyMMdd") & "NGN" & amt
        'valDateCurAmt_32A = "140101" & "NGN" & amt
        beneAccount = r.Beneficiary_account 'dr("Beneficiary_account").ToString()
        beneName = r.Beneficiary 'dr("Beneficiary").ToString
        orderingCust_50K = "/" & GetNuban(r.Customer_account, "-") & vbCrLf & FormatSwift(SanitizeForSwift(r.Customer_name), 4, 35)
        sendersCorrespondent_53A = Gadget.GetSendersCorrespondent()
        Dim retval() As String = gObj.GetCorrespondentBankBIC(r.Beneficiary_Bank)
        Dim nuban As String = retval(1)
        Dim bic As String = retval(0)
        acctWithInstitution_57A = "/" & nuban & vbCrLf & bic
        If Not String.IsNullOrEmpty(beneAccount) Then
            benCust_59 = "/" & beneAccount
        Else
            benCust_59 = "/" & "0"
        End If

        If Not String.IsNullOrEmpty(beneName) Then
            benCust_59 &= vbCrLf
            benCust_59 = benCust_59 & FormatSwift(SanitizeForSwift(beneName), 4, 35)
        Else
            'must always have a name.
        End If

        detailOfCharges_71A = "OUR"
        If Not IsDBNull(r.Remarks) AndAlso Not String.IsNullOrEmpty(r.Remarks.Trim()) Then
            remarks = r.Remarks.ToString().Replace(sendersRef_f20, String.Empty)
        Else
            remarks = ""
        End If


        If Not String.IsNullOrEmpty(remarks) Then
            'remarks = "/CODTYPTR/001" & vbCrLf & FormatSwift("/" & SanitizeForSwift(remarks), rows, cols, linePrefix, True)
            remarks = "/CODTYPTR/001" & vbCrLf & FormatSwift(linePrefix & SanitizeForSwift(remarks), rows, cols, linePrefix, True)
        Else
            remarks = "/CODTYPTR/001" & vbCrLf & linePrefix & "RTGS"
        End If

        senderToReceiverInfo_72 = remarks.ToUpper()

        Dim m As Message = New RtgsMt103Message
        m.AddField(New Field20(sendersRef_f20.ToUpper.Trim))
        m.AddField(New Field23B(bankOpCode_f23B.ToUpper.Trim))
        m.AddField(New Field23E(instructionCode_23E.ToUpper.Trim))
        m.AddField(New Field26T(transactionType_26T.ToUpper.Trim))
        m.AddField(New Field32A(valDateCurAmt_32A.ToUpper.Trim))
        m.AddField(New Field50K(orderingCust_50K.ToUpper.Trim))
        Dim f As Field = New Field53A(sendersCorrespondent_53A.ToUpper.Trim)
        m.AddField(f)
        m.AddField(New Field57A(acctWithInstitution_57A.ToUpper.Trim))
        m.AddField(New Field59(benCust_59.ToUpper.Trim))
        m.AddField(New Field71A(detailOfCharges_71A.ToUpper.Trim))
        If Not String.IsNullOrEmpty(remarks.ToUpper.Trim) Then
            Dim f72 As Field = New Field72(senderToReceiverInfo_72.ToUpper.Trim)
            m.AddField(f72)
        Else
            'if the remark is empty or null, then we do not include field 72.
        End If

        If Not String.IsNullOrEmpty(UseTestAddress) AndAlso UseTestAddress.Trim.ToLower().CompareTo("true") = 0 Then

            m.SourceTerminal = "NAMENGL0XXXX"

        Else

            m.SourceTerminal = "NAMENGLAXXXX"

        End If
        m.DestinationTerminal = bic.PadRight(12, "X"c)

        Dim novelconstring As String = ConfigurationManager.ConnectionStrings("novelTradeConString").ToString()
        m.DbConnectionString = novelconstring
        Try
            Dim rtgsDs As New DataSet
            Dim sqlRts As String = "select mt103_text from transactions where TransactionID=@ref"
            Dim mt103 As String = String.Empty
            m.MessagePriorityNumber = GetMessagePriorityNumber(r.MessagetypeID)
            If m.Validate() Then
                mt103 = m.Render()
                text = mt103
                Return text

            Else
                mt103 = m.Render()
                Return mt103
            End If
        Catch ex As Exception
            Gadget.LogException(ex)
        End Try

        ' End While

        ' End Using


        'End If
        Return text
    End Function

    ''' <summary>
    ''' Formats the remark to conform to the number of rows and columns and the line prefix supplied.
    ''' It does not add the line prefix to the first line. That means that if you want the line prefix on the first line of the resulting
    ''' formatted string, then you should add it prior to calling this function.
    ''' This function truncates to exactly LESS_THAN_OR_EQUAL_TO rows as the number of rows that you will have in the formatted output.
    ''' </summary>
    ''' <param name="remarks">the string you want to format</param>
    ''' <param name="rows">the number of rows you'd ideally like to have.</param>
    ''' <param name="cols">then umber of columns to appear in the formatted output.</param>
    ''' <param name="linePrefix">the string to prepend to each line except the first line.</param>
    ''' <returns></returns>
    ''' <remarks>throws an invalid argument exception if the string to format is empty or null.</remarks>
    Private Function FormatSwift(ByVal remarks As String, ByVal rows As Integer, ByVal cols As Integer, Optional ByVal linePrefix As String = "", Optional ByVal isRemarks As Boolean = False) As String
        Dim formattedString As String = String.Empty
        Dim content As New StringBuilder
        Dim numLinesFormated As Integer = 0
        Dim lines() As String = Field.GetLinesFromContent(remarks)
        If lines IsNot Nothing Then
            If lines.Length > 1 Then 'combine into a single line

                For Each line As String In lines
                    content.Append(line)
                Next
                remarks = content.ToString()
            End If

            content.Clear()
            Dim len As Integer = remarks.Length
            Dim remnant As Integer = len
            Dim startIndex As Integer = 0
            Dim count As Integer = len
            Dim storedPrefix As String = linePrefix

            For i As Integer = 0 To rows - 1

                If remnant <= 0 Then
                    Exit For
                End If

                If Not String.IsNullOrEmpty(linePrefix) Then
                    If i <> 0 Then
                        content.Append(linePrefix)
                        If cols - linePrefix.Length <= remnant Then
                            count = cols - linePrefix.Length
                        Else
                            count = remnant
                        End If
                    Else
                        count = cols
                    End If
                Else
                    count = cols
                End If
                If startIndex + count <= remarks.Length - 1 Then
                    content.AppendLine(remarks.Substring(startIndex, count))
                Else
                    content.AppendLine(remarks.Substring(startIndex))
                End If
                numLinesFormated += 1
                If isRemarks Then
                    If numLinesFormated >= 1 Then
                        linePrefix = "//"
                    End If
                End If

                remnant -= count
                startIndex += count
            Next
        Else
            Throw New ArgumentException("The remark is invalid. Either empty string or null.")
        End If
        formattedString = content.ToString

        Return formattedString

    End Function



End Class
