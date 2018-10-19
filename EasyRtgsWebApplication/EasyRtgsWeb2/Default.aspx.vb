
Partial Class _Default
    Inherits System.Web.UI.Page
    Private esy As New easyrtgs


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
         
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim access As New GAL.Service

        Dim login As New ldap
        Dim utype As New easyrtgs


        If login.login(TextBox1.Text.Trim(), TextBox2.Text.Trim()) = True Then


            Session("role") = utype.getRole(TextBox1.Text.Trim())
            Dim roles = utype.getRole(TextBox1.Text.Trim())
            If ConfigurationManager.AppSettings("APP_MODE").Trim().ToLower() <> "offline" Then

                If access.ApplicationAccess(5, TextBox1.Text) = "0" Then
                    Label1.Text = "This application is no longer available by this time."
                    Exit Sub

                End If
            End If

            Session("name") = utype.getName(TextBox1.Text.Trim())
            Session("cname") = Session("name").ToString()

            'Time Check for 2:00pm
            Dim isCutOffTimeOkay As Boolean = False
            Dim role As String = Session("role").ToString() 'set the session once and for all.


            'Check if the role is empty string which may mean the person is not profiled at all or the profile is inactive
            If String.IsNullOrEmpty(role.Trim()) Then
                Label1.Text = "You have not been profiled or your profile is inactive."
                Exit Sub
            End If
            isCutOffTimeOkay = CheckCutOffTime(role)


            If ConfigurationManager.AppSettings("APP_MODE").Trim().ToLower() = "offline" Then
                isCutOffTimeOkay = True
            End If

            If Not isCutOffTimeOkay Then
                Label1.Text = "You are not allowed to use this Portal after 2:00pm"
                Button1.Visible = False
                Exit Sub
            End If


            If Request.QueryString("action") = "authorize" Or Request.QueryString("action") = "approve" Then
                Session("uname") = TextBox1.Text.Trim()
                Session("name") = utype.getName(TextBox1.Text.Trim())
                Session("cname") = Session("name").ToString()
                role = utype.getRole(TextBox1.Text.Trim())
                Session("email") = utype.getEmail(TextBox1.Text.Trim())
                Session("branch") = utype.getBranch(TextBox1.Text.Trim())
                Session("pwd") = TextBox2.Text.Trim()

                Session.Timeout = 60



                'Response.Redirect("full.aspx?tid=" & Request.QueryString("tid"))
                Server.Transfer("full.aspx?tid=" & Request.QueryString("tid"))

            End If



            Dim sid As String = Session.SessionID


            Session("uname") = TextBox1.Text.Trim()
            Session("name") = utype.getName(TextBox1.Text.Trim())
            role = utype.getRole(TextBox1.Text.Trim())
            Session("email") = utype.getEmail(TextBox1.Text.Trim())
            Session("branch") = utype.getBranch(TextBox1.Text.Trim())
            Session("pwd") = TextBox2.Text.Trim()
            Session.Timeout = 60





            utype.DeleteSessionStatus(TextBox1.Text.Trim)

            utype.CreateSessionStatus(TextBox1.Text.Trim, sid)


            If role = easyrtgs.ROLE_INPUTTER_FTO Then
                esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
                Server.Transfer("landing.aspx")
            End If

            If role = easyrtgs.ROLE_SERVICE_MANAGER Then
                esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
                Server.Transfer("mytransactions.aspx")
            End If


            If role = easyrtgs.ROLE_TREASURY Then
                esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
                Response.Redirect("mytransactions.aspx")
            End If

            If role = easyrtgs.ROLE_ADMINISTRATOR Then
                esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
                Server.Transfer("admin.aspx")
            End If

            If role.Trim().ToLower() = easyrtgs.ROLE_REGIONAL_CHANNEL_COORDINATOR Then
                esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
                Server.Transfer("users.aspx")
            End If

            If role = easyrtgs.ROLE_ICO Then
                esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
                Server.Transfer("admin.aspx")
            End If

            ''The roles below are new roles from 2015-August-21
            If role = easyrtgs.ROLE_CUSTOMERCARE Then
                esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
                Server.Transfer("custcare.aspx")
            End If

            If role = easyrtgs.ROLE_TROPS Then
                esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
                '' 8th March 2018. Changed the landing page to trops_landing.aspx
                'Server.Transfer("landing.aspx")
                Server.Transfer("~/trops/trops_landing.aspx")
            End If

            If role = easyrtgs.ROLE_CUSTOMERCARE_CREATOR Then
                esy.createAudit(String.Format("{0} Logged on as {1} successfully from  {2}", TextBox1.Text, easyrtgs.ROLE_CUSTOMERCARE_CREATOR, Request.UserHostAddress), Now)
                Server.Transfer("users.aspx")
            End If
        Else

            esy.createAudit(TextBox1.Text & " Failed Logon from  " & Request.UserHostAddress, Now)


            Label1.Text = "Wrong userID or Password or Inactive account. Try again."



        End If

    End Sub
    'Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
    '    'tempLogin()


    '    Dim access As New GAL.Service






    '    If TextBox1.Text.Contains("'") Or TextBox1.Text.Contains("&") Or TextBox1.Text.Contains("=") Or TextBox2.Text.Contains("=") Or TextBox2.Text.Contains("'") Then

    '        Label1.Text = "Injection attack detected!."
    '        Exit Sub

    '    End If




    '    Dim login As New ldap
    '    Dim utype As New easyrtgs




    '    If login.login(TextBox1.Text.Trim(), TextBox2.Text.Trim()) = True Then


    '        Session("role") = utype.getTypex(TextBox1.Text.Trim())
    '        If ConfigurationManager.AppSettings("APP_MODE").Trim().ToLower() <> "offline" Then
    '            If access.ApplicationAccess(5, TextBox1.Text) = "0" Then
    '                Label1.Text = "This application is no longer available by this time."
    '                Exit Sub

    '            End If
    '        End If

    '        Session("name") = utype.getName(TextBox1.Text.Trim())
    '        Session("cname") = Session("name").ToString()

    '        'Time Check for 2:00pm
    '        Dim isCutOffTimeOkay As Boolean = False
    '        Dim role As String = Session("role").ToString() 'set the session once and for all.

    '        isCutOffTimeOkay = CheckCutOffTime(role)

    '        'If Not isCutOffTimeOkay Then
    '        '    Label1.Text = "You are not allowed to use this Portal after 2:00pm"
    '        '    Button1.Visible = False
    '        '    Exit Sub
    '        'End If


    '        If Request.QueryString("action") = "authorize" Or Request.QueryString("action") = "approve" Then
    '            Session("uname") = TextBox1.Text.Trim()
    '            Session("name") = utype.getName(TextBox1.Text.Trim())
    '            Session("cname") = Session("name").ToString()
    '            role = utype.getTypex(TextBox1.Text.Trim())
    '            Session("email") = utype.getEmail(TextBox1.Text.Trim())
    '            Session("branch") = utype.getBranch(TextBox1.Text.Trim())
    '            Session("pwd") = TextBox2.Text.Trim()

    '            Session.Timeout = 60



    '            'Response.Redirect("full.aspx?tid=" & Request.QueryString("tid"))
    '            Server.Transfer("full.aspx?tid=" & Request.QueryString("tid"))

    '        End If



    '        Dim sid As String = Session.SessionID


    '        Session("uname") = TextBox1.Text.Trim()
    '        Session("name") = utype.getName(TextBox1.Text.Trim())
    '        role = utype.getTypex(TextBox1.Text.Trim())
    '        Session("email") = utype.getEmail(TextBox1.Text.Trim())
    '        Session("branch") = utype.getBranch(TextBox1.Text.Trim())
    '        Session("pwd") = TextBox2.Text.Trim()
    '        Session.Timeout = 60





    '        utype.DeleteSessionStatus(TextBox1.Text.Trim)

    '        utype.CreateSessionStatus(TextBox1.Text.Trim, sid)


    '        If role = "Inputter" Then
    '            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
    '            Server.Transfer("landing.aspx")
    '        End If

    '        If role = "Authorizer" Then
    '            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
    '            Server.Transfer("mytransactions.aspx")
    '        End If


    '        If role = "Treasury" Then
    '            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
    '            Response.Redirect("mytransactions.aspx")
    '        End If

    '        If role = "Administrator" Then
    '            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
    '            Server.Transfer("admin.aspx")
    '        End If


    '        If role = "ICO" Then
    '            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
    '            Server.Transfer("admin.aspx")
    '        End If

    '        ''The roles below are new roles from 2015-August-21
    '        If role = "CustomerCare" Then
    '            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
    '            Server.Transfer("custcare.aspx")
    '        End If

    '        If role = "TROPS" Then
    '            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)
    '            Server.Transfer("landing.aspx")
    '        End If
    '    Else

    '        esy.createAudit(TextBox1.Text & " Failed Logon from  " & Request.UserHostAddress, Now)


    '        Label1.Text = "Wrong userID or Password or Inactive account. Try again."



    '    End If

    'End Sub


    Sub tempLogin()


        Dim utype1 As New easyrtgs
        Dim sid As String = Session.SessionID



        If TextBox1.Text = "Input" Then
            Session("role") = "Inputter"
            Session("uname") = TextBox1.Text.Trim()
            Session("name") = "TempUser"
            Session("email") = "okpaguu@sterlingbankng.com"
            Session("branch") = "Temp Branch"
            Session("pwd") = "123"

            utype1.DeleteSessionStatus(TextBox1.Text.Trim)

            utype1.CreateSessionStatus(TextBox1.Text.Trim, sid)



        End If


        If TextBox1.Text = "Auth" Then
            Session("role") = "Authorizer"
            Session("uname") = TextBox1.Text.Trim()
            Session("name") = "TempUser"
            Session("email") = "okpaguu@sterlingbankng.com"
            Session("branch") = "Temp Branch"
            Session("pwd") = "123"

            utype1.DeleteSessionStatus(TextBox1.Text.Trim)

            utype1.CreateSessionStatus(TextBox1.Text.Trim, sid)

        End If




        If TextBox1.Text = "Treasury" Then
            Session("role") = "Treasury"
            Session("uname") = TextBox1.Text.Trim()
            Session("name") = "TempUser"
            Session("email") = "okpaguu@sterlingbankng.com"
            Session("branch") = "Temp Branch"
            Session("pwd") = "123"


            utype1.DeleteSessionStatus(TextBox1.Text.Trim)

            utype1.CreateSessionStatus(TextBox1.Text.Trim, sid)

        End If


        If Session("role") = "Inputter" Then
            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)

            Response.Redirect("main.aspx")
        End If

        If Session("role") = "Authorizer" Then
            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)

            Response.Redirect("mytransactions.aspx")
        End If


        If Session("role") = "Treasury" Then
            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)

            Response.Redirect("mytransactions.aspx")
        End If

        If Session("role") = "Administrator" Then
            esy.createAudit(TextBox1.Text & " Logged on successfully from  " & Request.UserHostAddress, Now)

            Response.Redirect("admin.aspx")
        End If
    End Sub
    Private Function CheckCutOffTime(ByVal role As String) As Boolean
        Dim isOkay As Boolean = False
        Dim admSetting As AdminSetting = AdminSetting.GetActiveAdminSettings()
        Dim cutOffTimeIn24HourFormat As String = admSetting.CutOffTimeIn24HourFormat
        Dim hour As Integer = Convert.ToInt32(cutOffTimeIn24HourFormat.Substring(0, 2))
        Dim minute As Integer = Convert.ToInt32(cutOffTimeIn24HourFormat.Substring(2, 2))
        Dim second As Integer = 0
        Dim date1 As New Date(Microsoft.VisualBasic.DateAndTime.Year(Now), Microsoft.VisualBasic.DateAndTime.Month(Now), Microsoft.VisualBasic.DateAndTime.Day(Now), hour, minute, second)
        Dim date3 As Date = Date.Now

        'There are a number of roles that do not have a cut off time. For example
        Dim loweredRoleName As String = role.Trim().ToLower()

        admSetting = AdminSetting.GetAdminSettingForRole(loweredRoleName)
        If admSetting IsNot Nothing Then
            cutOffTimeIn24HourFormat = admSetting.CutOffTimeIn24HourFormat.ToString().PadLeft(4, "0")
            'hour, minute,second
            hour = Convert.ToInt32(cutOffTimeIn24HourFormat.Substring(0, 2))
            minute = Convert.ToInt32(cutOffTimeIn24HourFormat.Substring(2, 2))
            second = 0
            date1 = New Date(Microsoft.VisualBasic.DateAndTime.Year(Now), Microsoft.VisualBasic.DateAndTime.Month(Now), Microsoft.VisualBasic.DateAndTime.Day(Now), hour, minute, second)

        Else
            Throw New Exception("There is no Admin record for this role")
        End If


        'TODO: Remove the time limit comment later
        If (date3 > date1 And Not (role.ToString().ToLower().Contains("administrator") Or role.ToString().ToLower().Contains("treasury") Or role.ToString().ToLower().Contains("trops"))) Then

            Label1.Text = "You are not allowed to use this Portal after 2:00pm"
            Button1.Visible = False
            isOkay = False
        Else
            isOkay = True
        End If

        Return isOkay
    End Function
    'Private Function CheckCutOffTime(ByVal role As String) As Boolean
    '    Dim isOkay As Boolean = False
    '    Dim admSetting As AdminSetting = AdminSetting.GetActiveAdminSettings()
    '    Dim cutOffTimeIn24HourFormat As Integer = admSetting.CutOffTimeIn24HourFormat
    '    Dim twentyfourhrstring As String = cutOffTimeIn24HourFormat.ToString().Substring(0, 2)
    '    Dim g As New Gadget()
    '    Dim hr As Integer = g.Format24HourTime(twentyfourhrstring)
    '    Dim date1 As New Date(Microsoft.VisualBasic.DateAndTime.Year(Now), Microsoft.VisualBasic.DateAndTime.Month(Now), Microsoft.VisualBasic.DateAndTime.Day(Now), Convert.ToInt32(twentyfourhrstring), 0, 0)
    '    Dim date3 As Date = Date.Now

    '    'There are a number of roles that do not have a cut off time. For example
    '    Select Case role.Trim().ToLower()
    '        Case "trops"
    '            admSetting = AdminSetting.GetAdminSettingForRole(role)
    '            If admSetting IsNot Nothing Then
    '                cutOffTimeIn24HourFormat = admSetting.CutOffTimeIn24HourFormat
    '                hr = g.Format24HourTime(cutOffTimeIn24HourFormat.ToString().Substring(0, 2))
    '                date1 = New Date(Microsoft.VisualBasic.DateAndTime.Year(Now), Microsoft.VisualBasic.DateAndTime.Month(Now), Microsoft.VisualBasic.DateAndTime.Day(Now), hr, 0, 0)

    '            Else
    '                Throw New Exception("There is no Admin record for this role")
    '            End If

    '        Case Else

    '    End Select
    '    'TODO: Remove the time limit comment later
    '    If (date3 > date1 And Not (role.ToString.Contains("Administrator") Or role.ToString.Contains("Treasury"))) Then

    '        'Label1.Text = "You are not allowed to use this Portal after 2:00pm"
    '        'Button1.Visible = False
    '        isOkay = False
    '    Else
    '        isOkay = True
    '    End If

    '    Return isOkay
    'End Function

    Protected Sub btnGen103_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGen103.Click
        Dim mt As String = String.Empty
        Dim sendersRef As String = "SRS1603040854078"
        Dim mdet As New MtMessageDetails()

        mt = mdet.generateSwiftMt103Message(sendersRef)
        txtMtResult.Text = mt
    End Sub

    Protected Sub btnTestNfiu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTestNfiu.Click
        Dim es As New easyrtgs()
        es.UpdatetxnAsPay("SRS1602231550403")

    End Sub
End Class
