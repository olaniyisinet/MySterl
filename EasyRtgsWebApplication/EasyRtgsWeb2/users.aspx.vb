Imports System.Data.SqlClient
Imports System.Data

Partial Class users
    Inherits System.Web.UI.Page
    Private esy As New easyrtgs

    Function checkStatus(ByVal id As Integer) As String
        Dim r As New easyrtgs



        Return r.getUserStatus(id)

    End Function



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Session("role") Is Nothing Then
                Response.Redirect("Default.aspx")
            ElseIf Session("role").ToString().Trim().ToLower() <> "regionalchannelcoordinator" AndAlso Session("role").ToString().Trim().ToLower() <> "customercarecreator" AndAlso Session("role").ToString().Trim().ToLower() <> "Administrator" Then
                Response.Redirect("Default.aspx")
           
            End If

            setAdditionButtonText(Session("role").ToString())

            configureRoleToCreate(Session("role").ToString())

            configureBranchToCreate(Session("role").ToString())

        End If


        If Session("role").ToString().Trim().CompareTo("administrator") = 0 OrElse Session("role").ToString().Trim().ToLower().CompareTo("regionalchannelcoordinator") = 0 OrElse Session("role").ToString().Trim().ToLower().CompareTo("customercarecreator") = 0 Then
            If Request.QueryString("action") = "del" Then
                Dim conn As New SqlConnection(esy.conn1())

                conn.Open()
                Dim cmd As New SqlCommand
                cmd.CommandText = "delete from administrators where username='" & Request.QueryString("username") & "'"
                cmd.Connection = conn
                cmd.ExecuteNonQuery()
                Label4.Text = "Admin removed"

                Dim log As New easyrtgs
                log.InsertLog(Session("name") & " removed an Administrator (" & Request.QueryString("username") & ")")
                TextBox1.Text = ""



                Dim da As New SqlDataAdapter("select * from  Administrators", conn)
                Dim ds As New Data.DataSet
                da.Fill(ds)


                DataGrid1.DataSource = ds
                DataGrid1.DataBind()
                DataGrid1.Focus()



                conn.Close()




            End If


            Label1.Text = "Welcome " & Session("name") & " |  " & Session("type") & "  |    <a href='admin.aspx'>Home</a>   |   <a href='logout.aspx'>Logout</a>"

        Else

            Response.Redirect("error.aspx")

        End If
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim conn As New SqlConnection(esy.conn1())
        conn.Open()
        Label4.Text = ""




        Dim da As New SqlDataAdapter("select * from  Administrators", conn)
        Dim ds As New Data.DataSet
        da.Fill(ds)

        Dim cmd As New SqlCommand
        cmd.Connection = conn
        cmd.CommandText = "select * from  Administrators"
        Dim rs As SqlDataReader
        rs = cmd.ExecuteReader()
        If rs.HasRows Then

            DataGrid1.Visible = True

            DataGrid1.DataSource = ds
            DataGrid1.DataBind()
            DataGrid1.Focus()

        Else
            Label4.Text = "No Administrator was found."
            DataGrid1.Visible = False

            DataGrid1.Focus()
        End If



        conn.Close()
    End Sub


    Protected Sub DataGrid1_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles DataGrid1.PageIndexChanged
        DataGrid1.CurrentPageIndex = e.NewPageIndex
        Dim conn As New SqlConnection(esy.conn1())
        conn.Open()
        Dim da As New SqlDataAdapter("select * from  Administrators", conn)
        Dim ds As New Data.DataSet
        da.Fill(ds)
        DataGrid1.DataSource = ds
        DataGrid1.DataBind()
        conn.Close()

    End Sub

    Protected Sub Button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddAdmin.Click
        lblAdminUser.Text = String.Empty
        Dim username As String = TextBox1.Text
        If Not String.IsNullOrEmpty(username) Then
            Dim name As String = String.Empty
            Dim role As String = Session("role").ToString() 'easyrtgs.ROLE_REGIONAL_CHANNEL_COORDINATOR
            Dim branch As String = ConfigurationManager.AppSettings("DEFAULT_BRANCH_CODE").ToString()
            Dim branchName As String = "HEAD OFFICE BRANCH"
            Dim email As String = String.Empty
            Dim creatorUsername As String = Session("uname").ToString()
            Dim tellerid As String = String.Empty

            Dim srv As New EwsService.ServiceSoapClient
            Using srv
                Dim ds As DataSet = srv.GetADDetails(username)
                If ds IsNot Nothing Then
                    If ds.Tables.Count > 0 Then
                        If ds.Tables(0).Rows.Count > 0 Then
                            Dim r As DataRow = ds.Tables(0).Rows(0)
                            name = r("fullname").ToString()
                            email = r("email").ToString()

                        Else
                            Gadget.LogException(New Exception("Could not retrieve values for GetADDetails(" & username & ")"))
                        End If
                    Else
                        Gadget.LogException(New Exception("Could not retrieve values for GetADDetails(" & username & ")"))
                    End If
                Else
                    Gadget.LogException(New Exception("Could not retrieve values for GetADDetails(" & username & ")"))
                End If
            End Using

            Dim g As New Gadget()

            Dim isCreated As Boolean = False
            isCreated = g.CreateUser(username, role, branch, String.Empty, "Active", creatorUsername)


            If isCreated Then
                lblAdminUser.Text = "Admin added"
                Dim log As New easyrtgs
                'log.InsertLog(Session("name") & " created an Administrator (" & TextBox1.Text.Replace("'", "`") & ")")
                log.InsertLog(String.Format("{0} created an Administrator {1}", Session("name"), TextBox1.Text.Replace("'", "`")))
                TextBox1.Text = ""
                Try
                    GridView1.DataBind()
                Catch ex As Exception

                End Try
            Else
                lblAdminUser.Text = "<font color=red>Could not create the admin officer</font>"
            End If

        Else
            lblAdminUser.Text = "Enter admin username."
        End If


    End Sub

    Protected Sub ddlBranch_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlBranch.DataBound
        Dim d As New ListItem("--Pick Branch--", "0")
        ddlBranch.Items.Insert(0, d)

    End Sub

    Protected Sub btnAddNew_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNew.Click
        Label5.Text = String.Empty
        If rblRole.SelectedIndex <> -1 AndAlso Not String.IsNullOrEmpty(txtUsername.Text) AndAlso ddlBranch.SelectedIndex > 0 AndAlso rblStatus.SelectedIndex <> -1 Then
            Dim username As String = txtUsername.Text
            Dim role As String = rblRole.SelectedValue.ToString()
            Dim branch As String = ddlBranch.SelectedValue.ToString()
            Dim branchName As String = ddlBranch.SelectedItem.Text
            Dim creatorUsername As String = Session("uname").ToString()
            Dim status As String = rblStatus.SelectedValue.ToString()

            Dim creatorRole As String = Session("role").ToString().Trim().ToLower()
            If creatorRole = "administrator" And (role.Trim().ToLower() = "trops" OrElse role.Trim().ToLower() = "treasury") Then
                Label5.Text = "<font color='red'>You cannot create a user with this role.</font>"
                Exit Sub
            ElseIf creatorRole = "trops" And (role.Trim().ToLower() = "inputter" OrElse role.Trim().ToLower() = "authorizer") Then
                Label5.Text = "<font color='red'>You cannot create a user with this role.</font>"
                Exit Sub
            End If
            Dim g As New Gadget()
            Dim isCreated As Boolean = False
            isCreated = g.CreateUser(username, role, branch, branchName, status, creatorUsername)
            If isCreated Then
                Label5.Text = "Successfully Modified User."
                Try
                    GridView1.DataBind()
                Catch ex As Exception
                    Gadget.LogException(ex)
                End Try
            Else
                Label5.Text = "<font color=red>Could not Modify User.</font>"
            End If
        Else
            Label5.Text = "<font color=red>Please select the username, role, branch and status.</font>"
        End If

    End Sub

    Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged
        Dim username As String = String.Empty
        username = GridView1.SelectedDataKey.Values("username").ToString()
        txtUsername.Text = username

    End Sub

    Private Function getAdminCreationSql(ByVal role As String) As String
        Dim sql As String = String.Empty

        If role.ToLower().Trim() = "regionalchannelcoordinator" Then
            sql = "INSERT INTO  tblusers ( username , name , type , Branch  , email , Status , Created_By , Date_created , tellerID )  VALUES (@username ,@name ,@type ,@Branch ,@email  ,@Status ,@Created_By ,@Date_created ,@tellerID)"
        Else
            sql = "INSERT INTO  tblusers ( username , name , type , Branch  , email , Status , Created_By , Date_created , tellerID )  VALUES (@username ,@name ,@type ,@Branch ,@email  ,@Status ,@Created_By ,@Date_created ,@tellerID)"
        End If


        Return sql
    End Function

    Private Sub setAdditionButtonText(ByVal rolename As String)
        If rolename.Trim().ToLower() = "regionalchannelcoordinator" Then
            btnAddAdmin.Text = "Add Regional Coordinator"
        ElseIf rolename.Trim().ToLower() = "customercarecreator" Then
            btnAddAdmin.Text = "Add New Customer Care Admin"
        ElseIf rolename.Trim().ToLower() = "administrator" Then
            btnAddAdmin.Text = "Add Administrator"
        End If
    End Sub

    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView1.RowCommand
        lblGridStatus.Text = ""
        Dim commandName As String = e.CommandName.ToString().ToLower().Trim()
        If commandName = "disable" Then
            Dim id As Long = Convert.ToInt64(e.CommandArgument)
            Dim g As New Gadget()
            Dim status As Boolean = g.DeleteUserById(id, Session("uname").ToString())
            If status = True Then
                lblGridStatus.Text = "Successfully Disabled User"
                Try
                    Dim log As New easyrtgs
                    log.InsertLog(Session("name") & " disabled a regional channel coordinator whose record id on table tblusers is (" & id.ToString() & " >>SUCCESSFUL)")

                Catch ex As Exception

                End Try
                Try
                    GridView1.DataBind()
                Catch ex As Exception
                    Gadget.LogException(ex)
                End Try

            Else
                Dim log As New easyrtgs
                log.InsertLog("Attempt by " & Session("name") & " to disable a regional channel coordinator whose record id on table tblusers is (" & id.ToString() & ") >>FAILED")

                lblGridStatus.Text = "<font color=red>Could not disable user</font>"
            End If



        End If

    End Sub

    Private Sub configureRoleToCreate(ByVal role As String)
        If role.Trim().ToLower() = "customercarecreator" Then
            'We want to remove the other roles.
            'And add only the CustomerCare role
            Try
                rblRole.Items.Clear()

                'Then add customer care role directly
                rblRole.Items.Add(New ListItem("CUSTOMER CARE", "CustomerCare"))

            Catch ex As Exception
                Gadget.LogException(ex)
            End Try

        Else
            'We don't have any special needs for other roles
        End If
    End Sub

    Private Sub configureBranchToCreate(ByVal role As String)
        If role.Trim().ToLower() = "customercarecreator" Then
            'Make the branch list un editable and default to the head office
            ddlBranch.Enabled = False
            ddlBranch.SelectedValue = "NG0020006"
        Else
            'We don't have any special needs for other roles
        End If

    End Sub

End Class
