Imports Microsoft.VisualBasic

Public Class autoletter
    Public Function sqlconn() As String
        'Return "data source=10.0.41.115; user id=axecreditportal;password=axe*1234;initial catalog=STERLREPOSITORY"
        'Return "data source=10.0.0.156,1490; user id=appusr;password=(#usr4*);initial catalog=autoletterT24"
        Return "data source=10.0.41.101; user id=sa;password=tylent;initial catalog=autoletterT24"
        'Return ConfigurationManager.ConnectionStrings("connStringActive").ConnectionString
    End Function

    Public Function getCustomerCount(ByVal cusnum As String) As Integer

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn



        cmd.CommandText = "select count from count where cusnum=@uname"
        cmd.Parameters.AddWithValue("@uname", cusnum)
        Dim da As New Data.SqlClient.SqlDataAdapter
        Dim ds As New Data.DataSet

        da.SelectCommand = cmd
        da.Fill(ds)
        cmd.Parameters.Clear()


        Dim s As String = ""

        If ds.Tables(0).Rows.Count > 0 Then
            For Each dr As Data.DataRow In ds.Tables(0).Rows
                s = Val(dr("count").ToString) + 1
                UpdateCountData(cusnum)
            Next

        Else
            s = 100
            CreateCountData(cusnum, s)

        End If
        conn.Close()


        Return s


    End Function

    Public Function CreateCountData(ByVal cusnum As String, ByVal count As Integer) As Boolean

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        Dim cmd2 As New Data.SqlClient.SqlCommand
        cmd.CommandText = "insert into count(cusnum,count) values (@id,@cnt)"
        cmd.Parameters.AddWithValue("@id", cusnum)
        cmd.Parameters.AddWithValue("@cnt", count)

        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception

        End Try

        conn.Close()


        Return True



    End Function

    Public Function UpdateCountData(ByVal cusnum As String) As Boolean

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        Dim cmd2 As New Data.SqlClient.SqlCommand
        cmd.CommandText = "update count set count=count+1 where cusnum=@id"
        cmd.Parameters.AddWithValue("@id", cusnum)
        cmd.ExecuteNonQuery()
        conn.Close()


        Return True



    End Function

    Public Function CreateBAInfo(BRA_CODE As String, CUS_NUM As String, CUR_CODE As String, LED_CODE As String, SUB_ACCT_CODE As String, BUS_AMT As String, DIS_AMT As String, VAL_DATE As String, MAT_DATE As String, GRA_DAYS As String, INT_RATE As String, CUS_SHO_NAME As String, ADD_LINE1 As String, ADD_LINE2 As String, EMAIL As String, MOB_NUM As String, ALT_CUR_CODE As String, CURRENCY As String, ISSUER_NAME As String, REFNO As String) As Boolean
        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        Dim cmd2 As New Data.SqlClient.SqlCommand
        cmd.CommandText = "insert into BA(BRA_CODE, CUS_NUM, CUR_CODE, LED_CODE, SUB_ACCT_CODE, BUS_AMT, DIS_AMT, VAL_DATE, MAT_DATE, GRA_DAYS, INT_RATE, CUS_SHO_NAME, ADD_LINE1, ADD_LINE2,EMAIL,ALT_CUR_CODE, CURRENCY, ISSUER_NAME, REFNO) values( @bc,@cn,@cc,@lc,@sa,@bus,@dis,@val,@mat,@gra,@int,@cus,@add1,@add2,@email,@alt,@cur,@iss,@ref)"
        cmd.Parameters.AddWithValue("@bc", BRA_CODE)
        cmd.Parameters.AddWithValue("@cn", CUS_NUM)
        cmd.Parameters.AddWithValue("@cc", CUR_CODE)
        cmd.Parameters.AddWithValue("@lc", LED_CODE)
        cmd.Parameters.AddWithValue("@sa", SUB_ACCT_CODE)
        cmd.Parameters.AddWithValue("@bus", BUS_AMT)
        cmd.Parameters.AddWithValue("@dis", DIS_AMT)
        cmd.Parameters.AddWithValue("@val", VAL_DATE)
        cmd.Parameters.AddWithValue("@mat", MAT_DATE)
        cmd.Parameters.AddWithValue("@gra", GRA_DAYS)
        cmd.Parameters.AddWithValue("@int", INT_RATE)
        cmd.Parameters.AddWithValue("@cus", CUS_SHO_NAME)
        cmd.Parameters.AddWithValue("@add1", ADD_LINE1)
        cmd.Parameters.AddWithValue("@add2", ADD_LINE2)
        cmd.Parameters.AddWithValue("@email", EMAIL)
        'cmd.Parameters.AddWithValue("@mob_num", MOB_NUM)
        cmd.Parameters.AddWithValue("@alt", ALT_CUR_CODE)
        cmd.Parameters.AddWithValue("@cur", CURRENCY)
        cmd.Parameters.AddWithValue("@iss", ISSUER_NAME)
        cmd.Parameters.AddWithValue("@ref", REFNO)
        cmd.ExecuteNonQuery()
        conn.Close()


        Return True



    End Function

    Public Function UpdateBA(ByVal id As Integer, issuerName As String) As Boolean

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        Dim cmd2 As New Data.SqlClient.SqlCommand
        cmd.CommandText = "update BA set ISSUER_NAME=@in where id=@id"
        cmd.Parameters.AddWithValue("@id", id)
        cmd.Parameters.AddWithValue("@in", issuerName)
        cmd.ExecuteNonQuery()
        conn.Close()


        Return True



    End Function

    Public Function DeleteBA(ByVal ref As String) As Boolean

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        Dim cmd2 As New Data.SqlClient.SqlCommand
        cmd.CommandText = "delete from BA where REFNO=@id"
        cmd.Parameters.AddWithValue("@id", ref)
        cmd.ExecuteNonQuery()
        conn.Close()


        Return True



    End Function
	
	Public Function AuditAction(ByVal user As String, ByVal action As String) As Boolean

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        Dim cmd2 As New Data.SqlClient.SqlCommand
        cmd.CommandText = "insert into Audit_Log([User],[Activity],[DateTime]) values (@user,@action,@time)"
        cmd.Parameters.AddWithValue("@user", user)
        cmd.Parameters.AddWithValue("@action", action)
        cmd.Parameters.AddWithValue("@time", DateTime.Now)
        Try
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            'System.IO.File.AppendAllText(AppSettings("errorlog").ToString() & Year(Now) & Month(Now) & Day(Now) & ".txt", Now & ex.Message & vbCrLf)
        End Try

        conn.Close()


        Return True
    End Function

    Public Function sendmail(ByVal toAddress As String, ByVal cc As String, ByVal subject As String, ByVal body As String, ByVal attachment2 As String) As Boolean
        'toAddress = "Rabiu.Adetayo@Sterlingbankng.com"
        Dim mailHost As New Net.Mail.SmtpClient
        mailHost.Host = "172.18.2.11"
        Dim ma As New Net.Mail.Attachment(attachment2)

        Dim mailMessage As New Net.Mail.MailMessage
        Dim nc As New Net.NetworkCredential("spservice", "Kinder$$098")
        mailHost.Credentials = nc
        mailMessage.IsBodyHtml = True
        mailMessage.To.Add(toAddress)
        mailMessage.From = New Net.Mail.MailAddress("ebusiness@sterlingbankng.com")
        ' mailMessage.CC.Add(cc)
        mailMessage.Subject = subject
        mailMessage.Body = body
        mailMessage.Attachments.Add(ma)


        Try
            mailHost.Send(mailMessage)
            Return True

        Catch
            Return False
        End Try


    End Function

    Public Function sendSMS(ByVal phone As String, ByVal message As String) As Boolean
        Try
            'TODO Remove this line
            ''phone = "08029963787"
            Dim proxy As New EWSService.Service()
            Dim resp = proxy.SendSMS(message, phone)

            If resp = "00" Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try

    End Function

    Public Function CheckIssuerName(ref As String) As Boolean

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        cmd.CommandText = "select * from BA where REFNO =@tref " 'and ISSUER_NAME=''"
        cmd.Parameters.AddWithValue("@tref", ref)
        Dim da As Data.SqlClient.SqlDataReader
        da = cmd.ExecuteReader
        If da.HasRows Then
            conn.Close()

            Return True

        Else
            conn.Close()

            Return False
        End If

    End Function

    Public Function getAll() As Data.DataSet

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        cmd.CommandText = "SELECT     p.staffID, p.Surname, p.Firstname, p.Othernames, p.CurrentGrade, p.EntryGrade, p.StaffGroup, p.CurrentLocation, p.PreviousFunction, p.CurrentFunction, p.TimeInCurrentFunction, " & _
                   " p.LastPromotionDate, p.Supervisor, p.GroupHead,o.Education,o.PersonalGoals,o.job,o.Achievements,o.whyadmityou,o.AdditionalInfo FROM         PersonalInfo AS p INNER JOIN  OtherInfo AS o ON p.staffID = o.staffID"
        Dim da As New Data.SqlClient.SqlDataAdapter
        da.SelectCommand = cmd
        Dim ds As New Data.DataSet
        da.Fill(ds)
        conn.Close()

        Return ds
    End Function

    Public Function getBA(ByVal ref As String) As Data.DataSet

        Dim conn As New Data.SqlClient.SqlConnection(sqlconn())
        conn.Open()
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = conn
        cmd.CommandText = "SELECT * from BA where REFNO=@sn"
        cmd.Parameters.AddWithValue("@sn", ref)

        Dim da As New Data.SqlClient.SqlDataAdapter
        da.SelectCommand = cmd
        Dim ds As New Data.DataSet
        da.Fill(ds)
        conn.Close()

        Return ds
    End Function

End Class
