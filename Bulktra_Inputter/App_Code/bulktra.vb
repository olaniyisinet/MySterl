Imports Microsoft.VisualBasic

Public Class bulktra
    Public Function connstr() As String
        'Return "provider=sqloledb.1;user id=bulktra;password=(45*tra#;initial catalog=bulktraDBT24;data source=10.0.0.204,1490"
        Return "provider=sqloledb.1;user id=sa;password=tylent;initial catalog=bulktraDB;data source=10.0.41.101"
    End Function

    Public Function sqlconn() As String
        'Return "user id=bulktra;password=(45*tra#;initial catalog=bulktraDBT24;data source=10.0.0.204,1490"
        Return "user id=sa;password=tylent;initial catalog=bulktraDB;data source=10.0.41.101"
    End Function


    Public Function checkUsrType(ByVal username As String) As Boolean

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
                conn.Open()
                Dim cmd As New Data.SqlClient.SqlCommand
                cmd.Connection = conn
                cmd.CommandText = "Select * from users where username=@uname And status=@status"
                cmd.Parameters.AddWithValue("@uname", username)
                cmd.Parameters.AddWithValue("@status", "Active")

                Dim rs As Data.SqlClient.SqlDataReader
                rs = cmd.ExecuteReader

            s = "t"

            While rs.Read
                If rs("type") = "Inputter" Then
                    s = "t"

                Else
                    s = "f"

                End If
            End While
        End Using


            If s = "t" Then
                Return True
            Else
                Return False


            End If

    End Function


    Public Function CheckCurCodeCount(ByVal batch_id As String) As Boolean

        Dim i As Integer = 0
        Dim j As Integer = 0

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "Select count(cur_code) from transTemp where batch_id=@bid"
            cmd.Parameters.AddWithValue("@bid", batch_id)

            i = cmd.ExecuteScalar()

            Dim cmd2 As New Data.sqlclient.sqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "Select count(cur_code) from transTemp where batch_id=@bid"
            cmd2.Parameters.AddWithValue("@bid", batch_id)

            j = cmd2.ExecuteScalar()
        End Using

        If i = j Then

            Return True

        Else
            Return False


        End If

    End Function

    Public Function CheckBatchAmountMatch(ByVal batch_id As String) As Boolean

        Dim batchAcc As Decimal = 0
        Dim transAmt As Decimal = 0

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "Select sum(tra_amt) from transTemp where batch_id=@bid And deb_cre_ind=@dci"
            cmd.Parameters.AddWithValue("@bid", batch_id)
            cmd.Parameters.AddWithValue("@dci", "1")

         

            batchAcc = FormatNumber(Val(cmd.ExecuteScalar), 2)

            Dim cmd2 As New Data.sqlclient.sqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "Select sum(tra_amt) from transTemp where batch_id=@bid And deb_cre_ind=@dci"
            cmd2.Parameters.AddWithValue("@bid", batch_id)
            cmd2.Parameters.AddWithValue("@dci", "2")

            transAmt = FormatNumber(Val(cmd2.ExecuteScalar), 2)



        End Using

        If transAmt = batchAcc Then
            Return True
        Else
            Return False

        End If


    End Function



    Public Function getTotalDRCR(ByVal batch_id As String) As String

        Dim batchAcc As String

        Dim transAmt As String
        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "Select sum(tra_amt) from transTemp where batch_id=@bid And deb_cre_ind=@dci"
            cmd.Parameters.AddWithValue("@bid", batch_id)
            cmd.Parameters.AddWithValue("@dci", "1")

            batchAcc = Val(cmd.ExecuteScalar)

            Dim cmd2 As New Data.sqlclient.sqlCommand
            cmd2.Connection = conn
            'cmd2.CommandText = "Select sum(tra_amt) from transTemp where batch_id='" & batch_id & "'  AND deb_cre_ind='2'"

            cmd2.CommandText = "Select sum(tra_amt) from transTemp where batch_id=@bid And deb_cre_ind=@dci"
            cmd2.Parameters.AddWithValue("@bid", batch_id)
            cmd2.Parameters.AddWithValue("@dci", "2")

            transAmt = Val(cmd2.ExecuteScalar)



        End Using

        If transAmt = batchAcc Then

            Return "Credit Total Amount: " & FormatNumber(transAmt, 2) & ", Debit Total Amount: " & FormatNumber(batchAcc, 2)
        Else

            Return "Credit Total Amount: " & FormatNumber(transAmt, 2) & " is not equal to Debit Total Amount: " & FormatNumber(batchAcc, 2)

        End If






    End Function


    Public Function createErrorLog(ByVal action As String, ByVal date_time As DateTime) As Boolean

        Dim s As String = ""

        Dim f As New IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory & "\ErrorLogs\" & Year(Now) & "-" & Month(Now) & "-" & Day(Now) & ".txt", True)

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

        'conn.Close()

        If s = "t" Then
            Return True

        Else
            Return False

        End If
    End Function
    Public Function ChkDuplicates(ByVal amount As String, ByVal recs As Integer) As Integer

        Dim c As Integer = 0

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select count(*) from batches where Total_amt=@amount  AND batch_id like  @bid AND status=@status"
            cmd.Parameters.AddWithValue("@bid", "%" & Day(Today) & Year(Today) & Month(Today) & "%")
            cmd.Parameters.AddWithValue("@amount", amount)
            cmd.Parameters.AddWithValue("@status", "uploaded")




            c = cmd.ExecuteScalar



        End Using

        Return c


    End Function

    Public Function getTotalCusnum(ByVal batch_id As String) As String

        Dim totcn1 As String = ""


        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()



            Dim cmd2 As New Data.sqlclient.sqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select sum(cast(cus_num as bigint)) from trans  where batch_id=@bid "
            cmd2.Parameters.AddWithValue("@bid", batch_id)
            totcn1 = cmd2.ExecuteScalar.ToString




        End Using
        Return totcn1


    End Function

    Public Function getTotalCusnum2(ByVal batch_id As String) As String
        Dim totcn1 As String = ""


        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()

          


            Dim cmd2 As New Data.SqlClient.SqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select sum(cast(cus_num as bigint)) from transTemp  where batch_id=@bid "
            cmd2.Parameters.AddWithValue("@bid", batch_id)
            totcn1 = cmd2.ExecuteScalar.ToString




        End Using
        Return totcn1


    End Function

    Public Function CheckTotalCusnum(ByVal name As String, ByVal batchId As String) As String


        Dim out As Integer = 0

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()



            Dim cmd2 As New Data.SqlClient.SqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select * from batches  where uploaded_by=@bid and status=@status and batch_id like @date"
            cmd2.Parameters.AddWithValue("@bid", name)
            cmd2.Parameters.AddWithValue("@status", "uploaded")
            cmd2.Parameters.AddWithValue("@date", "%" & Day(Today) & Year(Today) & Month(Today) & "%")

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd2.ExecuteReader
            While rs.Read
                If getTotalCusnum(rs("batch_id")) = getTotalCusnum2(batchId) Then

                    out = out + 1
                End If

            End While


        End Using
        Return out


    End Function

    Public Function getTotalCR(ByVal batch_id As String) As String


        Dim transAmt As String = ""


        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()


            Dim cmd2 As New Data.sqlclient.sqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select sum(tra_amt) from transTemp where batch_id=@bid AND deb_cre_ind=@dci"
            cmd2.Parameters.AddWithValue("@bid", batch_id)
            cmd2.Parameters.AddWithValue("@dci", "2")
            transAmt = cmd2.ExecuteScalar.ToString


        End Using

        Return transAmt


    End Function

    Public Function getTotalDR(ByVal batch_id As String) As String

        Dim transAmt As String = ""

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()

            Dim cmd2 As New Data.SqlClient.SqlCommand
            cmd2.Connection = conn
            cmd2.CommandText = "select sum(tra_amt) from transTemp where batch_id=@bid AND deb_cre_ind=@dci"
            cmd2.Parameters.AddWithValue("@bid", batch_id)
            cmd2.Parameters.AddWithValue("@dci", "1")
            transAmt = cmd2.ExecuteScalar.ToString

        End Using

        Return transAmt

    End Function

    Function getbalance(ByVal acc As String) As String

        Dim bnk As New NewBanks.banks
        Dim curbal As String = ""
        Dim chck As New SBPSwitch.sbpswitch
        Dim ac As New Integer
        Dim benk As New NewImal.Service
        Dim bc As String

        ac = chck.getInternalBankID(acc)

        If Not acc = "0" Then

            If ac <> 0 Then
                bc = Convert.ToString(ac)

            Else

            End If
        End If


        Dim accbal As New Data.DataSet

        If bc = "1" Then
            accbal = bnk.getAccountFullInfo(acc)
            curbal = accbal.Tables(0).Rows(0).Item("UsableBal").ToString()

            Return FormatNumber(curbal, 2).ToString
        Else
            accbal = benk.GetAccountByAccountNumber(acc)
            curbal = accbal.Tables(0).Rows(0).Item("AVAIL_BAL").ToString()

            Return FormatNumber(curbal, 2).ToString
        End If
        'For Each dr As Data.DataRow In accbal.Tables(0).Rows
        '    curbal = Val(dr("cle_bal"))
        'Next






        'Dim bnk As New bank.bank
        'Dim bnk As New NewBanks.banks


    End Function

    Function getbalanceimal(ByVal acc As String) As String

        Dim bnk As New NewImal.Service
        Dim curbal As String = ""
        Dim accbal As New Data.DataSet
        accbal = bnk.GetAccountByAccountNumber(acc)

        'For Each dr As Data.DataRow In accbal.Tables(0).Rows
        '    curbal = Val(dr("cle_bal"))
        'Next

        curbal = accbal.Tables(0).Rows(0).Item("AVAIL_BAL").ToString()

        Return FormatNumber(curbal, 2).ToString


    End Function
    Public Function getAccNoofBatch(ByVal batchID As String) As String

        Dim bnk As New bank.Account

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from transTemp where batch_id=@bid and deb_cre_ind=@dci"
            cmd.Parameters.AddWithValue("@bid", batchID)
            cmd.Parameters.AddWithValue("@dci", "1")

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader

            If rs.HasRows Then

                While rs.Read

                    ' s = rs("bra_code") & rs("cus_num") & rs("cur_code") & rs("led_code") & rs("sub_acct_code")
                    s = rs("sub_acct_code")


                End While

            Else

                s = ""


            End If
        End Using

        Return s


    End Function

    Public Function getAccNoofBatchImal(ByVal batchID As String) As String

        Dim bnk As New NewImal.Service

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from transTemp where batch_id=@bid and deb_cre_ind=@dci"
            cmd.Parameters.AddWithValue("@bid", batchID)
            cmd.Parameters.AddWithValue("@dci", "1")

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader

            If rs.HasRows Then

                While rs.Read

                    ' s = rs("bra_code") & rs("cus_num") & rs("cur_code") & rs("led_code") & rs("sub_acct_code")
                    s = rs("sub_acct_code")


                End While

            Else

                s = ""


            End If
        End Using

        Return s


    End Function
    Public Function CheckBatchAccCountOK(ByVal batch_id As String) As Boolean

        Dim c As Integer = 0
        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select count(*) from transTemp where batch_id=@bid AND deb_cre_ind=@dci"
            cmd.Parameters.AddWithValue("@bid", batch_id)
            cmd.Parameters.AddWithValue("@dci", "1")

            c = cmd.ExecuteScalar()


        End Using


        If c = 1 Then
            Return True
        ElseIf c = 0 Then
            Return False

        Else

            Return False

        End If

    End Function

    Public Function CheckCHRGBatchAccCountOK(ByVal batch_id As String) As Boolean
        Dim c As Integer = 0


        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select count(*) from transTemp where batch_id=@bid AND deb_cre_ind=@dci"
            cmd.Parameters.AddWithValue("@bid", batch_id)
            cmd.Parameters.AddWithValue("@dci", "2")

            c = cmd.ExecuteScalar()




        End Using

        If c = 1 Then
            Return True
        ElseIf c = 0 Then
            Return False

        Else

            Return False

        End If

    End Function

    Public Function getBatchExplCode(ByVal batchid As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from batchType where batch_id=@bid"
            cmd.Parameters.AddWithValue("@bid", batchid)

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("expl_code")


            End While
        End Using


        Return s


    End Function
    Public Function getExplCodes(ByVal owner As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from explcode  where owner=@owner"
            cmd.Parameters.AddWithValue("@owner", owner)

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = s & rs("expl_code") & "-" & rs("schedule_type") & "*"


            End While
        End Using


        Return s.Trim("*")



    End Function
    Public Function getUnits() As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from units order by unit"

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = s & rs("unit") & "*"


            End While
        End Using


        Return s.Trim("*")



    End Function

    Public Function getExplCode(ByVal batch_type As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from explcode where schedule_type=@btype"
            cmd.Parameters.AddWithValue("@btype", batch_type)

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("expl_code")


            End While
        End Using


        Return s



    End Function

    Public Function getBatchType(ByVal batchid As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from batchType where batch_id=@bid"
            cmd.Parameters.AddWithValue("@bid", batchid)

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("batch_type")


            End While
        End Using


        Return s



    End Function
    Public Function getName(ByVal username As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from users where username=@uname"
            cmd.Parameters.AddWithValue("@uname", username)

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("name")


            End While
        End Using


        Return s



    End Function


    Public Function getTellerID(ByVal username As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from users where username=@uname"
            cmd.Parameters.AddWithValue("@uname", username)

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("tellerID").ToString


            End While
        End Using


        Return s



    End Function

    Public Function getTypex(ByVal username As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from users where username=@uname"
            cmd.Parameters.AddWithValue("@uname", username)

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("type")


            End While
        End Using


        Return s



    End Function

    Public Function getEmail(ByVal username As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from users where username=@uname"
            cmd.Parameters.AddWithValue("@uname", username)

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("email")


            End While
        End Using


        Return s



    End Function

    Public Function getBranch(ByVal username As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from users where username=@uname"
            cmd.Parameters.AddWithValue("@uname", username)

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("branch")


            End While
        End Using


        Return s



    End Function
    Public Function getAuthorizerEmail(ByVal branch As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from users where branch=@branch AND status='Active' AND type=@type"
            cmd.Parameters.AddWithValue("@branch", branch)
            cmd.Parameters.AddWithValue("@type", "Authorizer")

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = s & rs("email").ToString & ","


            End While
        End Using


        Return s.Trim(",")



    End Function

    Public Function getApproverEmail(ByVal branch As String) As String

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from users where branch=@branch AND status='Active' AND type=@type"
            cmd.Parameters.AddWithValue("@branch", branch)
            cmd.Parameters.AddWithValue("@type", "Approver")

            Dim rs As Data.sqlclient.sqlDataReader
            rs = cmd.ExecuteReader



            While rs.Read

                s = rs("email")


            End While
        End Using


        Return s



    End Function

    Public Function createBatch(ByVal batchID As String, ByVal batch_title As String, ByVal batch_date As Date, ByVal total_amount As Double, ByVal status As String, ByVal uploadedBy As String, ByVal authorizedBy As String, ByVal approvedBy As String) As Boolean

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn


            cmd.CommandText = "INSERT INTO batches(batch_id,batch_title,batch_date,Total_Amt,Status,Uploaded_By,Authorized_By,Approved_by,Authorized_date,Approved_date) VALUES(@bid,@btitle,@bdate,@totAmt,@status,@uploadBy,@AuthBy,@ApprBy,@authDate,@ApprDate)"


            cmd.Parameters.AddWithValue("@bid", batchID)
            cmd.Parameters.AddWithValue("@btitle", batch_title)
            cmd.Parameters.AddWithValue("@bdate", batch_date.ToString("yyyy-MM-dd hh:mm tt"))
            cmd.Parameters.AddWithValue("@totAmt", total_amount)
            cmd.Parameters.AddWithValue("@status", status)
            cmd.Parameters.AddWithValue("@uploadBy", uploadedBy)
            cmd.Parameters.AddWithValue("@AuthBy", authorizedBy)
            cmd.Parameters.AddWithValue("@ApprBy", approvedBy)
            cmd.Parameters.AddWithValue("@AuthDate", "")
            cmd.Parameters.AddWithValue("@ApprDate", "")

            Try
                cmd.ExecuteNonQuery()
                s = "t"

            Catch ex As Exception

                If ex.Message.Contains("KEY") Then
                    s = "f"

                End If



            End Try

        End Using

        If s = "t" Then
            Return True

        Else
            Return False

        End If

    End Function

    Public Function createBatchSuspense(ByVal batchID As String, ByVal suspenseAcc As String) As Boolean

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn

            cmd.CommandText = "INSERT INTO batch_suspense(batch_id,Branch_Suspense) values(@bid,@sus)"
            cmd.Parameters.AddWithValue("@bid", batchID)
            cmd.Parameters.AddWithValue("@sus", suspenseAcc)


            Try
                cmd.ExecuteNonQuery()
                s = "t"

            Catch ex As Exception

                If ex.Message.Contains("KEY") Then
                    s = "f"

                End If



            End Try

        End Using

        If s = "t" Then
            Return True

        Else
            Return False

        End If

    End Function


    Public Function createBatchUnit(ByVal batchID As String, ByVal unit As String) As Boolean

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn

            cmd.CommandText = "INSERT INTO batchUnit(batch_id,unit) values(@bid,@unit)"
            cmd.Parameters.AddWithValue("@bid", batchID)
            cmd.Parameters.AddWithValue("@unit", unit)


            Try
                cmd.ExecuteNonQuery()
                s = "t"

            Catch ex As Exception

                If ex.Message.Contains("KEY") Then
                    s = "f"

                End If



            End Try

        End Using

        If s = "t" Then
            Return True

        Else
            Return False

        End If

    End Function

    Public Function getBatchSuspense(ByVal branch As String) As String

        Dim bc As Array
        bc = Split(branch, "*")

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.sqlclient.sqlCommand
            cmd.Connection = conn
            cmd.CommandText = "select * from branches where branch_code =@bc and Branch_Name=@bn"
            cmd.Parameters.AddWithValue("@bc", bc(0))
            cmd.Parameters.AddWithValue("@bn", bc(1))

            Dim rs As Data.SqlClient.SqlDataReader
            rs = cmd.ExecuteReader
            While rs.Read
                s = rs("branch_suspense").ToString


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
        mailMessage.CC.Add(cc)
        mailMessage.Subject = subject
        mailMessage.Body = body
        Try
            mailHost.Send(mailMessage)
            Return True

        Catch
            Return False
        End Try


    End Function
    Public Function createAudit(ByVal action As String, ByVal date_time As DateTime) As Boolean

        Dim s As String = "" : Using conn As New Data.SqlClient.SqlConnection(sqlconn())
            conn.Open()
            Dim cmd As New Data.SqlClient.SqlCommand
            cmd.Connection = conn
            cmd.CommandText = "INSERT INTO  AUDIT_log(action,date_time)  VALUES(@action,@date)"
            cmd.Parameters.AddWithValue("@action", action)

            cmd.Parameters.AddWithValue("@date", date_time.ToString("yyyy-MM-dd hh:mm tt"))


            Try
                cmd.ExecuteNonQuery()
                s = "f"


            Catch ex As Exception

                If ex.Message.Contains("KEY") Then

                    s = "f"
                End If



            End Try

        End Using

        If s = "t" Then
            Return True

        Else
            Return False

        End If
    End Function
End Class


