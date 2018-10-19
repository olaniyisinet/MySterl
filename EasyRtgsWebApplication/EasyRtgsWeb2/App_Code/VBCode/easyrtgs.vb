Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports BankCore
Imports BankCore.t24

Public Class easyrtgs



    Public Const TRANSTYPE_INTERBANK As String = "interbank"
    Public Const MESSAGETYPE_MT103 As String = "mt103"
    Public Const MESSAGETYPE_MT202 As String = "mt202"
    Public Const TRANSTYPE_CBN As String = "cbn"
    Public Const MESSAGE_TYPE_VARIANT_RESERVATION As String = "RESERVATION"
    Public Const MESSAGE_TYPE_VARIANT_SDF As String = "SDF"
    Public Const MESSAGE_TYPE_VARIANT_REGULAR As String = "REGULAR"
    Public Const MESSAGE_TYPE_VARIANT_CASH_RETURN As String = "CASH_RETURN"
    Public Const TREASURY_STATUS As String = "Authorized"
    Public Const TREASURY_PENDING_STATUS As String = "Authorized-Pending"
    Public Const SERVICE_MANAGER_STATUS As String = "Uploaded"
    Public Const CEMP_STATUS As String = "Pending"
    Public Const ROLE_REGIONAL_CHANNEL_COORDINATOR As String = "regionalchannelcoordinator"
    Public Const ROLE_INPUTTER_FTO As String = "Inputter"
    Public Const ROLE_SERVICE_MANAGER As String = "Authorizer"
    Public Const ROLE_TREASURY As String = "Treasury"
    Public Const ROLE_ADMINISTRATOR As String = "Administrator"
    Public Const ROLE_ICO As String = "ICO"
    Public Const ROLE_CUSTOMERCARE As String = "CustomerCare"
    Public Const ROLE_TROPS As String = "TROPS"
    Public Const ROLE_CUSTOMERCARE_CREATOR As String = "CustomerCareCreator"
    Public Const POST_FROM_SUSPENSE_TO_INCOME_STATUS As String = "Pay"
    Public Const POST_TO_SWIFT_ALLIANCE_STATUS As String = "Paid"

    Public Function getSessionStatus(ByVal username As String, ByVal sessionID As String) As Boolean


        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand

            cmd.Connection = conn
            cmd.CommandText = "select * from Sessions where username=@uname and sessionID=@sessionID"
            cmd.Parameters.AddWithValue("@uname", username)
            cmd.Parameters.AddWithValue("@sessionID", sessionID)

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader()



            If rs.HasRows Then
                s = "t"
            Else
                s = "f"

            End If

        End Using



        If s = "t" Then
            Return True
        Else
            Return False

        End If
    End Function
    Public Function getExplCode() As String



        Return "949"



    End Function
    Public Function CreateSessionStatus(ByVal username As String, ByVal sessionID As String) As Boolean


        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand


            cmd.Connection = conn
            cmd.CommandText = "Insert into Sessions(username,sessionID) values(@username,@sessionID)"
            cmd.Parameters.AddWithValue("@username", username)
            cmd.Parameters.AddWithValue("@sessionID", sessionID)

            Try

                cmd.ExecuteNonQuery()
                s = "t"
            Catch ex As Exception
                s = "f"
                Gadget.LogException(ex)
            End Try
        End Using

        If s = "t" Then
            Return True
        Else
            Return False

        End If



    End Function


    Public Function DeleteSessionStatus(ByVal username As String) As Boolean


        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand


            cmd.Connection = conn
            cmd.CommandText = "delete from  Sessions where username=@username"
            cmd.Parameters.AddWithValue("@username", username)
            Try

                cmd.ExecuteNonQuery()
                s = "t"
            Catch ex As Exception
                s = "f"
                Gadget.LogException(ex)
            End Try
        End Using

        If s = "t" Then
            Return True
        Else
            Return False

        End If




    End Function


    Public Function getBanks() As Data.DataSet


        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand


            cmd.Connection = conn
            cmd.CommandText = "select bank_code,bank_name, nuban, alternateAccount as sdf from tbl_banks where statusflag=1"

            Dim da As New Data.SqlClient.SqlDataAdapter
            da.SelectCommand = cmd
            Dim ds As New Data.DataSet
            da.Fill(ds)
            Return ds
        End Using




    End Function


    Public Function getName(ByVal username As String) As String

        : Dim s As String = "" : Using conn As New SqlConnection(connUser())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from tblusers where username='" & username & "'"
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("name")


            End While
        End Using


        Return s



    End Function

    Public Function getRole(ByVal username As String) As String
        Dim s As String = ""
        Using conn As New SqlConnection(connUser())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            'cmd.CommandText = "select * from tblusers where username='" & username & "'"
            cmd.CommandText = "select * from tblusers where username=@username and Status='Active'"
            cmd.Parameters.AddWithValue("@username", username)

            Dim rs As SqlDataReader
            Try
                rs = cmd.ExecuteReader()


                Using rs
                    While rs.Read

                        s = rs("type")


                    End While
                    rs.Close()
                End Using
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try


        End Using


        Return s


    End Function
    ''' <summary>
    ''' Return the email address of the person who matches the branch and the role
    ''' if you do not want the branch, then enter empty string for it
    ''' </summary>
    ''' <param name="branch">the branch</param>
    ''' <param name="role">the role</param>
    ''' <returns>The email address</returns>
    ''' <remarks></remarks>
    Public Function getAuthorizerEmail(ByVal branch As String, Optional ByVal role As String = "Authorizer") As String
        Dim sql As String = String.Empty
        'sql = "select * from tblusers where branch='" & branch & "' AND status='Active' AND type='Authorizer'"
        sql = "select top 1 * from tblusers where branch=@branch AND status='Active' AND type=@type and email <> '' and email is not null"
        Dim s As String = "" : Using conn As New SqlConnection(connUser())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = sql
            If Not String.IsNullOrEmpty(branch) Then
                cmd.Parameters.AddWithValue("@branch", branch)
                cmd.Parameters.AddWithValue("@type", role)
            Else
                sql = "select top 1 * from tblusers where status='Active' AND type=@type and email <> '' and email is not null"
                cmd.CommandText = sql
                cmd.Parameters.AddWithValue("@type", role)
            End If

            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader()

            While rs.Read()

                s = rs("email").ToString()

            End While
        End Using


        Return s



    End Function

    Public Function sendmail(ByVal host As String, ByVal toAddress As String, ByVal cc As String, ByVal subject As String, ByVal body As String) As Boolean

        Dim mailHost As New Net.Mail.SmtpClient
        mailHost.Host = host
        Dim mailMessage As New Net.Mail.MailMessage
        Dim nc As New Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("mailUserName"), System.Configuration.ConfigurationManager.AppSettings("mailUserPwd"))
        mailHost.Credentials = nc
        mailMessage.IsBodyHtml = True
        mailMessage.To.Add(toAddress)
        mailMessage.From = New Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings("AppMail"))
        If Not String.IsNullOrEmpty(cc) Then
            ' mailMessage.CC.Add(cc)
        End If

        mailMessage.Subject = subject
        mailMessage.Body = body
        Try
            mailHost.Send(mailMessage)
            Return True

        Catch
            Return False
        End Try


    End Function
    Public Function getEmail(ByVal username As String) As String

        : Dim s As String = "" : Using conn As New SqlConnection(connUser())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from tblusers where username=@username" ' & username & "'"
            Dim rs As SqlDataReader
            cmd.Parameters.AddWithValue("@username", username)
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("email")


            End While
        End Using


        Return s



    End Function

    Public Function getTxnStatus(ByVal tid As String) As String

        : Dim s As String = "" : Using conn As New SqlConnection(conn1())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from transactions where transactionid='" & tid & "'"
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("status")


            End While
        End Using


        Return s



    End Function

    Function getAuthorizerEmailbyName(ByVal name As String) As String
        : Dim s As String = "" : Using conn As New SqlConnection(connUser())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from tblusers where name='" & name & "' and status='Active'"
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("email")


            End While
        End Using

        Return s


    End Function
    Function getEmailbyName(ByVal name As String) As String
        : Dim s As String = "" : Using conn As New SqlConnection(connUser())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from tblusers where name='" & name & "' and status='Active'"
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("email")


            End While
        End Using

        Return s


    End Function
	Public Function getRTGSSuspense202(ByVal isIMAL As Boolean, Optional ByVal accttype As String = "BANKS") As String

		Dim s As String = ""
		Using conn As New SqlConnection(conn1())
			conn.Open()
			Dim cmd As New SqlCommand
			cmd.Connection = conn
			cmd.CommandText = "select * from Admin where isActive=1"
			Dim rs As SqlDataReader
			rs = cmd.ExecuteReader

			While rs.Read
				If isIMAL Then
					s = rs("iMALRTGSSuspense_202")
				Else
					s = rs("RTGSSuspense_202")
				End If
			End While

			If Not String.IsNullOrEmpty(s) Then
				Dim acctype As String = ConfigurationManager.AppSettings("ACCT_TYPE")
				If accttype.ToLower().Trim().CompareTo("banks") = 0 Then
					Return s
				End If

				If s.Trim().ToLower().Contains("-") AndAlso acctype.Trim().ToLower() = "banks" Then
					'get the nuban.

					Dim acctObj As IAccount
					Dim t24 As New t24.T24Bank()
					Dim bracode As String = String.Empty
					Dim cusnum As String = String.Empty
					Dim curcode As String = String.Empty
					Dim ledcode As String = String.Empty
					Dim subacctcode As String = String.Empty

					Dim acctparts() As String = s.Split("-")
					If acctparts IsNot Nothing Then
						If acctparts.Length = 5 Then

							bracode = acctparts(0)
							cusnum = acctparts(1)
							curcode = acctparts(2)
							ledcode = acctparts(3)
							subacctcode = acctparts(4)

							Dim ds As DataSet = Nothing
							Try

								acctObj = t24.GetAccountInfoByAccountNumber(New Util1s().getAccountNo(bracode, cusnum, curcode, ledcode, subacctcode))
								If (acctObj IsNot Nothing) Then
									s = acctObj.AccountNumberRepresentations(Account.NUBAN).Representation
								Else
									s = String.Empty
									Throw New ArgumentException("Could not return the NUBAN equivalent.")
								End If

							Catch ex As Exception

							End Try


						End If
					End If

				Else
					Return s 'Return the value like that.
				End If
			End If
		End Using


		Return s

	End Function

	Public Function getRTGSSuspense(ByVal isIMAL As Boolean, ByVal Optional accttype As String = "BANKS") As String
		Dim s As String = ""

		Using conn As SqlConnection = New SqlConnection(conn1())
			conn.Open()
			Dim cmd As SqlCommand = New SqlCommand()
			cmd.Connection = conn
			cmd.CommandText = "select * from Admin where isActive=1"
			Dim rs As SqlDataReader
			rs = cmd.ExecuteReader()

			While rs.Read()

				If isIMAL Then
					s = rs("iMALRTGSSuspense")
				Else
					s = rs("RTGSSuspense")
				End If
			End While

			If Not String.IsNullOrEmpty(s) Then
				Dim acctype As String = ConfigurationManager.AppSettings("ACCT_TYPE")
				If accttype.ToLower().Trim().CompareTo("banks") = 0 Then Return s

				If s.Trim().ToLower().Contains("-") AndAlso acctype.Trim().ToLower() = "banks" Then
					Dim acctObj As IAccount
					Dim t24 As t24.T24Bank = New t24.T24Bank()
					Dim bracode As String = String.Empty
					Dim cusnum As String = String.Empty
					Dim curcode As String = String.Empty
					Dim ledcode As String = String.Empty
					Dim subacctcode As String = String.Empty
					Dim acctparts As String() = s.Split("-")

					If acctparts IsNot Nothing Then

						If acctparts.Length = 5 Then
							bracode = acctparts(0)
							cusnum = acctparts(1)
							curcode = acctparts(2)
							ledcode = acctparts(3)
							subacctcode = acctparts(4)
							Dim ds As DataSet = Nothing

							Try
								acctObj = t24.GetAccountInfoByAccountNumber(New Util1s().getAccountNo(bracode, cusnum, curcode, ledcode, subacctcode))

								If (acctObj IsNot Nothing) Then
									s = acctObj.AccountNumberRepresentations(Account.NUBAN).Representation
								Else
									s = String.Empty
									Throw New ArgumentException("Could not return the NUBAN equivalent.")
								End If

							Catch ex As Exception
							End Try
						End If
					End If
				Else
					Return s
				End If
			End If
		End Using

		Return s
	End Function


	'Public Function getRTGSSuspense(Optional ByVal accttype As String = "BANKS") As String

	'    Dim s As String = ""
	'    Using conn As New SqlConnection(conn1())
	'        conn.Open()
	'        Dim cmd As New SqlCommand
	'        cmd.Connection = conn
	'        cmd.CommandText = "select * from Admin where isActive=1"
	'        Dim rs As SqlDataReader
	'        rs = cmd.ExecuteReader



	'        While rs.Read

	'            s = rs("RTGSSuspense")


	'        End While
	'        If Not String.IsNullOrEmpty(s) Then
	'            Dim acctype As String = ConfigurationManager.AppSettings("ACCT_TYPE")
	'            If accttype.ToLower().Trim().CompareTo("banks") = 0 Then
	'                Return s
	'            End If

	'            If s.Trim().ToLower().Contains("-") AndAlso acctype.Trim().ToLower() = "banks" Then
	'                'get the nuban.

	'                Dim acctObj As IAccount
	'                Dim t24 As New t24.T24Bank()
	'                Dim bracode As String = String.Empty
	'                Dim cusnum As String = String.Empty
	'                Dim curcode As String = String.Empty
	'                Dim ledcode As String = String.Empty
	'                Dim subacctcode As String = String.Empty

	'                Dim acctparts() As String = s.Split("-")
	'                If acctparts IsNot Nothing Then
	'                    If acctparts.Length = 5 Then

	'                        bracode = acctparts(0)
	'                        cusnum = acctparts(1)
	'                        curcode = acctparts(2)
	'                        ledcode = acctparts(3)
	'                        subacctcode = acctparts(4)

	'                        Dim ds As DataSet = Nothing
	'                        Try

	'                            acctObj = t24.GetAccountInfoByAccountNumber(New Util1s().getAccountNo(bracode, cusnum, curcode, ledcode, subacctcode))
	'                            If (acctObj IsNot Nothing) Then
	'                                s = acctObj.AccountNumberRepresentations(Account.NUBAN).Representation
	'                            Else
	'                                s = String.Empty
	'                                Throw New ArgumentException("Could not return the NUBAN equivalent.")
	'                            End If

	'                        Catch ex As Exception

	'                        End Try


	'                    End If
	'                End If

	'            Else
	'                Return s 'Return the value like that.
	'            End If
	'        End If
	'    End Using


	'    Return s



	'End Function

	Public Function getVATAccount(ByVal isIMAL As Boolean) As String

		Dim s As String = "" : Using conn As New SqlConnection(conn1())
			conn.Open()
			Dim cmd As New SqlCommand
			cmd.Connection = conn
			cmd.CommandText = "select * from Admin where isActive=1"
			Dim rs As SqlDataReader
			rs = cmd.ExecuteReader



			While rs.Read
				If isIMAL Then
					s = rs("iMALVATAccount")
				Else
					s = rs("VATAccount")
				End If


			End While
		End Using


		Return s



	End Function

	Public Function getRTGSAccount(ByVal isIMAL As Boolean) As String

		Dim s As String = "" : Using conn As New SqlConnection(conn1())
			conn.Open()
			Dim cmd As New SqlCommand
			cmd.Connection = conn
			cmd.CommandText = "select * from Admin where isActive=1"
			Dim rs As SqlDataReader
			rs = cmd.ExecuteReader

			While rs.Read
				If isIMAL Then
					s = rs("iMALRTGSAccount")
				Else
					s = rs("RTGSAccount")
				End If
			End While
		End Using


		Return s



	End Function
	Public Function getPLAccount(ByVal isINAL As Boolean) As String

		Dim s As String = "" : Using conn As New SqlConnection(conn1())
			conn.Open()
			Dim cmd As New SqlCommand
			cmd.Connection = conn
			cmd.CommandText = "select * from Admin where isActive=1"
			Dim rs As SqlDataReader
			rs = cmd.ExecuteReader

			While rs.Read
				If isINAL Then
					s = rs("iMALPLAccount")
				Else
					s = rs("PLAccount")
				End If

			End While
		End Using


		Return s



	End Function

	Public Function getCustomerCharges() As String

        Dim s As String = String.Empty
        Using conn As New SqlConnection(conn1())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from Admin where isActive=1"
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader()

            If rs.HasRows Then
                While rs.Read()

                    's = rs("ChargesCustomer")
                    s = rs("ChargesCustomer2")

                End While
            End If


        End Using


        Return s



    End Function

    Public Function getStaffCharges() As String

        : Dim s As String = "" : Using conn As New SqlConnection(conn1())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from Admin where isActive=1"
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                's = rs("ChargesStaff").ToString
                s = rs("ChargesStaff2").ToString



            End While
        End Using


        Return s



    End Function

	Public Function getRTGSMail(ByVal isIMAL As Boolean) As String

		Dim s As String = "" : Using conn As New SqlConnection(conn1())
			conn.Open()
			Dim cmd As New SqlCommand
			cmd.Connection = conn
			cmd.CommandText = "select * from Admin where isActive=1"
			Dim rs As SqlDataReader
			rs = cmd.ExecuteReader



			While rs.Read
				If isIMAL Then
					s = rs("iMALRTGSEmail").ToString
				Else
					s = rs("RTGSEmail").ToString
				End If
			End While
		End Using

		Return s
	End Function

	Public Function getTreasuryMail(ByVal isIMAL As Boolean) As String

		Dim s As String = "" : Using conn As New SqlConnection(conn1())
			conn.Open()
			Dim cmd As New SqlCommand
			cmd.Connection = conn
			cmd.CommandText = "select * from Admin where isActive=1"
			Dim rs As SqlDataReader
			rs = cmd.ExecuteReader

			While rs.Read
				If isIMAL Then
					s = rs("iMALTreasuryEMail").ToString
				Else
					s = rs("TreasuryEMail").ToString
				End If
			End While
		End Using

		Return s
	End Function



	Public Function checkisIMAL(ByVal tid As String) As Boolean

		Dim s As Boolean = False : Using conn As New SqlConnection(conn1())
			conn.Open()
			Dim cmd As New SqlCommand
			cmd.Connection = conn
			cmd.CommandText = "select distinct [customer_account] from transactions where TransactionID = '" + tid + "'"
			Dim rs As SqlDataReader
			rs = cmd.ExecuteReader

			While rs.Read
				Dim account As String = rs("customer_account").ToString

				If account.StartsWith("05") Then

					s = True
				Else
					s = False

				End If
			End While
		End Using

		Return s
	End Function

	Public Function getBranch(ByVal username As String) As String

        : Dim s As String = "" : Using conn As New SqlConnection(connUser())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from tblusers where username='" & username & "'"
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("branch").ToString


            End While
        End Using


        Return s



    End Function

    Public Function InsertLog(ByVal action As String) As String

        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "Insert into logs(action,date) values(@action,@date)"
            cmd.Parameters.AddWithValue("@action", action)
            cmd.Parameters.AddWithValue("@date", Date.Now.ToString("yyyy-MM-dd hh:mm tt"))
            cmd.ExecuteNonQuery()

        End Using


        Return "Action was logged"



    End Function
    Public Function TransactionAudit(ByVal action As String, ByVal date_time As DateTime, ByVal tid As String) As Boolean


        Dim s As String = "" : Dim f As New IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory & "\logs\" & tid & ".txt", True)

        Try


            f.WriteLine(Now & ": " & action)
            f.Close()

            s = "t"


        Catch ex As Exception

            If ex.Message.Contains("KEY") Then

                s = "f"
            End If

            s = "f"


        End Try

        'End Using

        If s = "t" Then
            Return True

        Else
            Return False

        End If
    End Function
    Public Function createAudit(ByVal action As String, ByVal date_time As DateTime) As Boolean


        Dim s As String = ""
        Dim f As IO.StreamWriter = Nothing

        Try
            f = New IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory & "\logs\" & Year(Now) & "-" & Month(Now) & "-" & Day(Now) & Second(Now) & "_Audit.txt", True)


            f.WriteLine(Now & ": " & action)


            s = "t"


        Catch ex As Exception

            If ex.Message.Contains("KEY") Then

                s = "f"
            End If

            s = "f"

        Finally
            If f IsNot Nothing Then
                f.Close()
            End If
        End Try

        'End Using

        If s = "t" Then
            Return True

        Else
            Return False

        End If
    End Function


    Public Function Errorlog(ByVal action As String, ByVal date_time As DateTime) As Boolean


        Dim s As String = "" : Dim f As New IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory & "\Errorlogs\" & Year(Now) & "-" & Month(Now) & "-" & Day(Now) & ".txt", True)

        Try
            '  :  Dim s As String = "" : using conn   As  New SqlConnection(conn1())
            'conn.Open()
            'Dim cmd As New SqlCommand
            'cmd.Connection = conn
            'cmd.CommandText = "insert into log values('" & action & "','" & date_time & "')"
            'cmd.ExecuteNonQuery()
            'End Using



            f.WriteLine(Now & ": " & action)
            f.Close()

            s = "t"


        Catch ex As Exception

            If ex.Message.Contains("KEY") Then

                s = "f"
            End If

            s = "f"


        End Try

        'End Using

        If s = "t" Then
            Return True

        Else
            Return False

        End If
    End Function

    ''' <summary>
    ''' Added Atomicity to this operation to make it update two tables at the same time: transactions and TransactTemp2 
    ''' </summary>
    ''' <param name="tid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdatetxnAsPay(ByVal tid As String) As String

        Dim s As String = ""
        Dim trans As Data.SqlClient.SqlTransaction

        Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            'quickly get the transaction remark
            Dim cmdGetRemark As New SqlCommand("select distinct Remarks from TransactTemp2 where Transactionid=@transid", conn)
            cmdGetRemark.Parameters.AddWithValue("@transid", tid)
            Dim result As Object = cmdGetRemark.ExecuteScalar()
            Dim legacyRemark As String = String.Empty
            Dim sasAMLCompliantRemark As String = String.Empty


            If result IsNot Nothing Then
                legacyRemark = CType(result, String)
            End If
            Try
                sasAMLCompliantRemark = EasyRtgTransaction.BuildNfiuCompliantRemark_T24(tid, legacyRemark)
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try

            trans = conn.BeginTransaction()

            Using trans

                Dim isTransOk As Boolean = False
                Dim isTempOk As Boolean = False

                Dim cmd As New Data.SqlClient.SqlCommand()
                cmd.Connection = conn
                cmd.Transaction = trans
                cmd.CommandText = "update transactions set status='Pay' where transactionid=@tid"
                cmd.Parameters.AddWithValue("@tid", tid)
                If cmd.ExecuteNonQuery() > 0 Then
                    isTransOk = True
                End If
                cmd.Parameters.Clear()
                cmd.CommandText = "UPDATE TRANSACTTEMP2 SET STATUS='Pay',Remarks=@remark where  transactionid=@tid"
                cmd.Parameters.AddWithValue("@tid", tid)
                cmd.Parameters.AddWithValue("@remark", sasAMLCompliantRemark)
                If cmd.ExecuteNonQuery() > 0 Then
                    isTempOk = True
                End If

                Try
                    If isTransOk And isTempOk Then
                        trans.Commit()

                        Return "Pay"
                    Else
                        trans.Rollback()
                    End If

                Catch ex As Exception
                    trans.Rollback()
                End Try


            End Using

        End Using
        Return "Failed"

    End Function
    Public Function UpdatetxnAsSwiftReady(ByVal tid As String) As String



        Dim s As String = ""
        Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "update transactions set status='SwiftReady' where transactionid=@tid"
            cmd.Parameters.AddWithValue("@tid", tid)
            cmd.ExecuteNonQuery()


            Return "SwiftReady"
        End Using






    End Function

    'Public Function UpdatetxnForReversal(ByVal tid As String) As String



    '    Dim s As String = ""
    '    Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
    '        conn.Open()
    '        Dim cmd As New Data.SqlClient.SqlCommand
    '        cmd.Connection = conn
    '        cmd.CommandText = "update transactions set status='Reverse' where transactionid=@tid"
    '        cmd.Parameters.AddWithValue("@tid", tid)
    '        If cmd.ExecuteNonQuery() >= 0 Then
    '            Return "Reverse"
    '        End If


    '        Return "Reverse"
    '    End Using






    'End Function

    'If the transaction already has entries in the TransactTemp2 table, first delete these
    Public Function UpdatetxnForReversal(ByVal tid As String) As String

        Dim isTransupdated As Boolean = False
        Dim isDeleted As Boolean = False

        Dim s As String = ""
        Dim trans As SqlTransaction = Nothing
        Using conn As New Data.SqlClient.SqlConnection(sqlconn1())

            conn.Open()
            trans = conn.BeginTransaction()
            Using trans
                Dim cmd As New Data.SqlClient.SqlCommand
                cmd.Connection = conn
                cmd.Transaction = trans
                cmd.CommandText = "update transactions set status='Reverse' where transactionid=@tid"
                cmd.Parameters.AddWithValue("@tid", tid)



                If cmd.ExecuteNonQuery() >= 0 Then
                    isTransupdated = True
                Else
                    isTransupdated = False
                End If

                Dim cmdDelete As New SqlCommand()

                cmdDelete.Connection = conn
                cmdDelete.Transaction = trans
                cmdDelete.CommandText = "delete from TransactTemp2 where TransactionID=@tid"
                cmdDelete.Parameters.AddWithValue("@tid", tid)
                If cmdDelete.ExecuteNonQuery() >= 0 Then
                    isDeleted = True
                Else
                    isDeleted = False
                End If

                If isTransupdated AndAlso isDeleted Then
                    trans.Commit()
                    s = "Reverse"
                Else
                    trans.Rollback()
                    s = String.Empty
                End If


                Return s
            End Using



            Return "Reverse"
        End Using






    End Function
    Public Function UpdateTxnStatus(ByVal tid As String, ByVal status As String) As String


        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "update transactions set status=@status where transactionid=@tid"
            cmd.Parameters.AddWithValue("@tid", tid)
            cmd.Parameters.AddWithValue("@status", status)
            cmd.ExecuteNonQuery()


            Return status
        End Using

    End Function
    Public Function UpdatetxnAsApproved(ByVal tid As String) As String



        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "update transactions set status='Approved' where transactionid=@tid"
            cmd.Parameters.AddWithValue("@tid", tid)
            cmd.ExecuteNonQuery()


            Return "Approved"
        End Using






    End Function
    Public Function UpdatetxnAsAuthorize(ByVal tid As String) As String



        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "update transactions set status='Authorize' where transactionid=@tid"
            cmd.Parameters.AddWithValue("@tid", tid)
            cmd.ExecuteNonQuery()


            Return "Authorize"
        End Using






    End Function


    Public Function UpdatetxnAsAuthorized(ByVal tid As String) As String



        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "update transactions set status='Authorized' where transactionid=@tid"
            cmd.Parameters.AddWithValue("@tid", tid)
            cmd.ExecuteNonQuery()

            Dim authby As String = ""
            Dim inputby As String = ""
            Dim cmd2 As New Data.SqlClient.SqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select * from transactions where transactionid=@tid"
            cmd2.Parameters.AddWithValue("@tid", tid)

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd2.ExecuteReader
            While rs.Read
                authby = rs("authorized_by")
                inputby = rs("uploaded_by")
            End While

            Try
                sendmail(System.Configuration.ConfigurationManager.AppSettings("mail"), getAuthorizerEmailbyName(authby), getEmailbyName(inputby), "Third Party Transaction (" & tid & ") Treated.", "Hello All, This Transaction with Transaction ID: " & tid & ", inputted by " & inputby & ", authorized by " & authby & " has been Paid by the system. Log on to the portal to view the full status. <br> Thank You")
            Catch

            End Try



            Return "Paid"
        End Using






    End Function

    Public Function UpdatetxnAsPaid(ByVal tid As String) As String



        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "update transactions set status='Paid' where transactionid=@tid"
            cmd.Parameters.AddWithValue("@tid", tid)
            cmd.ExecuteNonQuery()

            Dim authby As String = ""
            Dim inputby As String = ""
            Dim cmd2 As New Data.SqlClient.SqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select * from transactions where transactionid=@tid"
            cmd2.Parameters.AddWithValue("@tid", tid)

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd2.ExecuteReader
            While rs.Read
                authby = rs("authorized_by")
                inputby = rs("uploaded_by")
            End While

            Try
                sendmail(System.Configuration.ConfigurationManager.AppSettings("mail"), getAuthorizerEmailbyName(authby), getEmailbyName(inputby), "Third Party Transaction (" & tid & ") Treated.", "Hello All, This Transaction with Transaction ID: " & tid & ", inputted by " & inputby & ", authorized by " & authby & " has been Paid by the system. Log on to the portal to view the full status. <br> Thank You")
            Catch

            End Try



            Return "Paid"
        End Using






    End Function

    Public Function UpdatetxnAsDiscarded(ByVal tid As String, ByVal reason As String) As String




        Dim s As String = ""
        Dim trans As Data.SqlClient.SqlTransaction

        Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            trans = conn.BeginTransaction
            Using trans
                Dim cmd As New Data.SqlClient.SqlCommand
                cmd.Connection = conn
                cmd.Transaction = trans
                cmd.CommandText = "update transactions set status='Discarded - [" & reason & "]' where transactionid=@tid"
                cmd.Parameters.AddWithValue("@tid", tid)
                cmd.ExecuteNonQuery()
                cmd.Parameters.Clear()


                cmd.CommandText = "update TransactTemp2 set status='Discarded - [" & reason & "]' where transactionid=@tid"
                cmd.Parameters.AddWithValue("@tid", tid)
                cmd.ExecuteNonQuery()

                Try
                    Try
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        Return "Discarded Failed"
                    End Try

                    Dim authby As String = ""
                    Dim inputby As String = ""
                    Dim cmd2 As New Data.SqlClient.SqlCommand
                    cmd2.Connection = conn
                    cmd2.CommandText = "select * from transactions where transactionid=@tid"
                    cmd2.Parameters.AddWithValue("@tid", tid)
                    Dim rs As Data.SqlClient.SqlDataReader
                    rs = cmd2.ExecuteReader
                    While rs.Read
                        authby = rs("authorized_by").ToString
                        inputby = rs("uploaded_by").ToString
                    End While

                    Try
						sendmail(System.Configuration.ConfigurationManager.AppSettings("mail"), getRTGSMail(checkisIMAL(tid)) & "," & getTreasuryMail(checkisIMAL(tid)) & "," & getAuthorizerEmailbyName(authby), getEmailbyName(inputby), "Third Party Transaction (" & tid & ") was discarded.", "Hello All,<br> This Transaction with Transaction ID: " & tid & ", inputted by " & inputby & ", authorized by " & authby & " has been DISCARDED  due to " & reason & ". Log on to the portal to view the full status. <br> Thank You")
					Catch

                    End Try
                Catch ex As Exception

                End Try



            End Using


            'Return "Discarded"
        End Using

    End Function

    Public Function UpdatetxnAsFailed(ByVal tid As String) As String




        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "update transactions set status='Failed' where transactionid=@tid"
            cmd.Parameters.AddWithValue("@tid", tid)

            cmd.ExecuteNonQuery()

            Dim authby As String = ""
            Dim inputby As String = ""
            Dim cmd2 As New Data.SqlClient.SqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select * from transactions where transactionid=@tid"
            cmd2.Parameters.AddWithValue("@tid", tid)
            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd2.ExecuteReader
            While rs.Read
                authby = rs("authorized_by")
                inputby = rs("uploaded_by")
            End While

            Try
                sendmail(System.Configuration.ConfigurationManager.AppSettings("mail"), getAuthorizerEmailbyName(authby), getEmailbyName(inputby), "Third Party Transaction (" & tid & ") Failed.", "Hello All, This Transaction with Transaction ID: " & tid & ", inputted by " & inputby & ", authorized by " & authby & " has failed in the system. Confirm that both the Transaction and Charges were both sucessful. Log on to the portal to view the full status. <br> Thank You")
            Catch

            End Try



            Return "Failed"
        End Using






    End Function

    Public Function getUserStatus(ByVal id As Integer) As String
        : Dim s As String = "" : Using conn As New SqlConnection(connUser())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from tblusers where id=" & id
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader


            While rs.Read
                If rs("status") = "Active" Then
                    s = "<img src='images\up.png' title='Active user' width='25'>"
                Else
                    s = "<img src='images\down.png' title='InActive user' width='25'>"
                End If
            End While


            Return s
        End Using


    End Function

    Public Function sqlconn1() As String
        Return ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString

    End Function

    Public Function conn1() As String
        Return ConfigurationManager.ConnectionStrings("easyRtgsOleConn").ToString

    End Function

    Public Function connUser() As String
        ' Return ConfigurationManager.ConnectionStrings("easyRtgsOleBulktraConn").ToString
        Return ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString
    End Function

    Public Function SqlconnUser() As String
        Return ConfigurationManager.ConnectionStrings("bulktraConn").ToString


    End Function
    Public Function validateAccount(ByVal acctnum As String) As Boolean
        Dim isvalid As Boolean = False

        Dim acct As IAccount = Nothing
        Dim t24 As T24Bank
        Try
            If acctnum.Trim().ToUpper().StartsWith("PL") And acctnum.Trim().Length >= 3 Then
                Dim intacctnumeric As String = acctnum.Trim().Substring(2)
                Dim num As Integer = Convert.ToInt32(intacctnumeric)
                If num > 50000 Then
                    Return True
                Else
                    Return False
                End If
            Else
                t24 = New T24Bank()
                acct = t24.GetAccountInfoByAccountNumber(acctnum)
                If acct IsNot Nothing Then
                    isvalid = True

                End If
            End If

        Catch ex As Exception
            Gadget.LogException(ex)
            Throw ex
        End Try

        Return isvalid
    End Function

    Public Function UpdateAdminSettings(ByVal RTGSAccount As String, ByVal PLAccount As String, ByVal CustomerCharges As String, ByVal StaffCharges As String, ByVal RTGSmail As String, ByVal TreasuryMail As String, ByVal RTGSSuspense As String, ByVal RTGSSuspense_202 As String) As String
        'Validate the account numbers first
        Dim accts As String() = New String() {RTGSAccount, PLAccount, RTGSSuspense, RTGSSuspense_202}
        For Each act As String In accts
            If Not validateAccount(act) Then
                Return String.Format("Account: {0} is INVALID", act)
            End If
        Next
        : Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn1())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "update admin set RTGSAccount=@rtgsacc,PLAccount=@placc,ChargesCustomer=@cuscharges,ChargesStaff=@staffcharges,RTGSEmail=@rtgsmail,TreasuryEMail=@trmail, RTGSSuspense=@rtgsSus"

            cmd.Parameters.AddWithValue("@rtgsacc", RTGSAccount)
            cmd.Parameters.AddWithValue("@placc", PLAccount)
            cmd.Parameters.AddWithValue("@cuscharges", CustomerCharges)
            cmd.Parameters.AddWithValue("@staffcharges", StaffCharges)
            cmd.Parameters.AddWithValue("@rtgsmail", RTGSmail)
            cmd.Parameters.AddWithValue("@trmail", TreasuryMail)
            cmd.Parameters.AddWithValue("@rtgssus", RTGSSuspense)


            Try
                cmd.ExecuteNonQuery()
                s = "Updated"
            Catch ex As Exception

                s = "Update Failed"
                s = ex.Message

            End Try



            Return s
        End Using


    End Function

    Public Function CheckAdminUser(ByVal username As String) As String
        : Dim s As String = "" : Using conn As New SqlConnection(conn1())
            conn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from Administrators where username='" & username & "'"

            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader()



            If rs.HasRows Then
                s = "True"
            Else
                s = "False"
            End If



            Return s
        End Using


    End Function

    Function SanitizeRemarks(ByVal remark As String) As String
        Dim newRemark As String = remark

        If remark.Length > 200 Then
            Dim partOne As String = remark.Substring(0, 184)
            Dim partTwo As String = remark.Substring(remark.Length - 16, 16)
            newRemark = partOne & partTwo
        End If

        newRemark = newRemark.Replace(ChrW(38), "-").Replace(ChrW(62), "").Replace(ChrW(60), "").Replace(ChrW(39), "").Replace(ChrW(34), "").Replace("'", "")
        Return newRemark
    End Function

    Function SaveCommentForTransid(ByVal comment As String, ByVal transid As String) As Boolean
        Dim status As Boolean = False
        Dim cn As SqlConnection = New SqlConnection(sqlconn1())
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand("spSaveCommentForTransID", cn)
                cmd.CommandType = Data.CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@transid", transid)
                cmd.Parameters.AddWithValue("@comment", comment)
                If cmd.ExecuteNonQuery() >= 0 Then
                    'saved else
                    status = True

                End If
            Catch ex As Exception
                status = False
            End Try
        End Using
        Return status
    End Function

    Function getBranchMT202AccountNumberByCode(Optional ByVal branchcode As String = "NG0020006") As String
        'As per SWR, the ledger on BANKS (a core banking application) is ledger code 303 s
        'such that the account number is of the format: {branchcode}/0/NGN/303/0
        'For compatibility reasons, we'll get the nuban format of this account

        'TODO: SIT notes:- Remove the hardcoding of baracode = 900 (T24 company code: NG0020006 - HEAD OFFICE BRANCH)
        Dim acctnum As String = String.Empty
        Dim acctObj As IAccount = Nothing
        Dim t24 As BankCore.Bank
        Dim ds As New DataSet
        Try
            Dim g As New Gadget
            Dim br As BranchDetail = g.getMt202BranchDetails(branchcode)
            If br IsNot Nothing Then

            End If
            Dim cusnum As String = br.Cusnum '"0"
            Dim curcode As String = br.CurrencyCode '"1"
            Dim ledcode As String = br.LedgerCode '"303"
            Dim subacctcode As String = br.SubAccountCode '"0"

            Dim util As New BankCore.Util1s
            'acctnum = util.getAccountNo(branchcode, cusnum, curcode, ledcode, subacctcode)
            acctnum = Util1s.GetInternalAccount(branchcode, curcode, ledcode)
            t24 = New BankCore.t24.T24Bank()
            acctObj = t24.GetAccountInfoByAccountNumber(acctnum)
            If acctObj IsNot Nothing Then
                acctnum = acctObj.AccountNumberRepresentations(Account.NUBAN).Representation
            Else
                acctnum = String.Empty
            End If


        Catch ex As Exception
            Gadget.LogException(ex)
        End Try




        'TODO Remove this as soon as possible.
        If String.IsNullOrEmpty(acctnum) Then
            acctnum = "223/0/1/303/0"
        End If
        Return acctnum
    End Function


End Class

