Imports System.Data.SqlClient

Partial Class mytransactions
    Inherits System.Web.UI.Page
    Private esy As New easyrtgs
    Private strsql As String = ""

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Button1.Visible = False


        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_SERVICE_MANAGER).ToString().Trim().ToLower() Then 'Service Manager
            Button1.Visible = True
            Button1.Text = "View Authorized Txns"
            Label4.Text = "Pending Transactions"
        End If
        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_TREASURY).ToString().Trim().ToLower() Then 'Treasury
            Button1.Visible = True
            Button1.Text = "View Approved Txns"
            Label4.Text = "Pending Transactions"
        End If

        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_INPUTTER_FTO).ToString().Trim().ToLower() Then 'FTO
            Label4.Text = "My Uploaded Transactions"
        End If



        'load authorized/approved transactions
        Dim conn As New SqlConnection(esy.conn1())
        conn.Open()
        Dim strsql As String = ""
        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_SERVICE_MANAGER).ToString().Trim().ToLower() Then
            Label3.Text = "My Authorized Transactions"

            strsql = "select transactionID,[messagetypeID],customer_name,customer_account,amount,charges,status,Uploaded_date ,(select messageType from tblMessageTypes where id=messagetypeID) type      ,(select messageVariant from tblMessageVariant where id=[messageVariantID]) variant ,[requestingBranch]    from transactions where authorized_by='" & Session("name") & "' order by authorized_date desc"
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = strsql
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader
            If rs.HasRows Then

                rs.Close()
                Dim da As New SqlDataAdapter(strsql, conn)
                Dim ds As New Data.DataSet
                da.Fill(ds)
                DataGrid2.DataSource = ds
                DataGrid2.DataBind()
                DataGrid2.Focus()




            Else
                Label4.Text = "No transactions"

            End If
            conn.Close()

        End If
        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_TREASURY).ToString().Trim().ToLower() Then
            Label3.Text = "My Approved Transactions"
            strsql = "select transactionID,[messagetypeID],customer_name,customer_account,amount,charges,Case Status when 'Uploaded' THEN 'Pending With SM' WHEN 'CustomerCare' THEN 'Pending With Customer Care' WHEN 'Authorize' THEN 'Pending Payment CUSTOMER-to-SUSPENSE(Authorize)' WHEN 'Authorized' THEN 'Pending With TROPS(Authorized)'   WHEN 'SwiftInProcess' THEN 'Completed By TROPS (SwiftInProcess)'  WHEN 'SwiftFailed' then 'Completed By TROPS (SwiftFailed)' WHEN 'Paid' then 'Successfully Paid SUSPENSE-to-RTGS Account' when 'Pay' then 'Pending Payment SUSPENSE-to-RTGS Account' when 'Reverse' then 'Pending Reversal SUSPENSE-to-CUSTOMER' when 'Reversed' then 'Reversal Completed' when 'Approved' then 'Approved By TROPS' when 'CustomerCareRejection' then 'Customer Care Rejected'else  Status END  status,Uploaded_date ,(select messageType from tblMessageTypes where id=messagetypeID) type      ,(select messageVariant from tblMessageVariant where id=[messageVariantID]) variant ,[requestingBranch]   from transactions where Treasury_Approval='" & Session("name") & "' order by approved_date desc"
            Dim cmd As New SqlCommand
            cmd.Connection = conn
            cmd.CommandText = strsql
            Dim rs As SqlDataReader
            rs = cmd.ExecuteReader
            If rs.HasRows Then
                rs.Close()

                Dim da As New SqlDataAdapter(strsql, conn)
                Dim ds As New Data.DataSet
                da.Fill(ds)
                DataGrid2.DataSource = ds
                DataGrid2.DataBind()
                DataGrid2.Focus()
            Else
                Label4.Text = "No transactions"
            End If
            conn.Close()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Session("uname") = "" Then
            Response.Redirect("default.aspx")

        End If


        If Not esy.getSessionStatus(Session("uname"), Session.SessionID) = True Then
            Session.Abandon()
            Response.Redirect("default.aspx")

        End If




        Label1.Text = Session("name")
        Label2.Text = Session("role")

        Dim conn As New SqlConnection(esy.conn1())



        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_INPUTTER_FTO).ToString().Trim().ToLower() Then
            strsql = "select transactionID,[messagetypeID],customer_name,customer_account,amount,charges,Case Status when 'Uploaded' THEN 'Pending With SM' WHEN 'CustomerCare' THEN 'Pending With Customer Care' WHEN 'Authorize' THEN 'Pending Payment CUSTOMER-to-SUSPENSE(Authorize)' WHEN 'Authorized' THEN 'Pending With TROPS(Authorized)'   WHEN 'SwiftInProcess' THEN 'Completed By TROPS (SwiftInProcess)'  WHEN 'SwiftFailed' then 'Completed By TROPS (SwiftFailed)' WHEN 'Paid' then 'Successfully Paid SUSPENSE-to-RTGS Account' when 'Pay' then 'Pending Payment SUSPENSE-to-RTGS Account' when 'Reverse' then 'Pending Reversal SUSPENSE-to-CUSTOMER' when 'Reversed' then 'Reversal Completed' when 'Approved' then 'Approved By TROPS' when 'CustomerCareRejection' then 'Customer Care Rejected' else  Status END  status,Uploaded_date ,(select messageType from tblMessageTypes where id=messagetypeID) type      ,(select messageVariant from tblMessageVariant where id=[messageVariantID]) variant ,[requestingBranch]    from transactions where Uploaded_by='" & Session("name") & "' order by uploaded_date desc"
        End If

        'Status value for the Service Manager to treat = Uploaded
        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_SERVICE_MANAGER).ToString().Trim().ToLower() Then
            strsql = "select transactionID,[messagetypeID],customer_name,customer_account,amount,charges,Case Status when 'Uploaded' THEN 'Pending With SM' WHEN 'CustomerCare' THEN 'Pending With Customer Care' WHEN 'Authorize' THEN 'Pending Payment CUSTOMER-to-SUSPENSE(Authorize)' WHEN 'Authorized' THEN 'Pending With TROPS(Authorized)'   WHEN 'SwiftInProcess' THEN 'Completed By TROPS (SwiftInProcess)'  WHEN 'SwiftFailed' then 'Completed By TROPS (SwiftFailed)' WHEN 'Paid' then 'Successfully Paid SUSPENSE-to-RTGS Account' when 'Pay' then 'Pending Payment SUSPENSE-to-RTGS Account' when 'Reverse' then 'Pending Reversal SUSPENSE-to-CUSTOMER' when 'Reversed' then 'Reversal Completed' when 'Approved' then 'Approved By TROPS' when 'CustomerCareRejection' then 'Customer Care Rejected' else  Status END  status,Uploaded_date ,(select messageType from tblMessageTypes where id=messagetypeID) type      ,(select messageVariant from tblMessageVariant where id=[messageVariantID]) variant ,[requestingBranch]    from transactions where status='" & easyrtgs.SERVICE_MANAGER_STATUS & "' and branch='" & Session("branch") & "' order by uploaded_date desc"
        End If

        'Status value for the Treasury to treat = Authorized
        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_TREASURY).ToString().Trim().ToLower() Then
            strsql = "select transactionID,[messagetypeID],customer_name,customer_account,amount,charges,Case Status when 'Uploaded' THEN 'Pending With SM' WHEN 'CustomerCare' THEN 'Pending With Customer Care' WHEN 'Authorize' THEN 'Pending Payment CUSTOMER-to-SUSPENSE(Authorize)' WHEN 'Authorized' THEN 'Pending With TROPS(Authorized)'   WHEN 'SwiftInProcess' THEN 'Completed By TROPS (SwiftInProcess)'  WHEN 'SwiftFailed' then 'Completed By TROPS (SwiftFailed)' WHEN 'Paid' then 'Successfully Paid SUSPENSE-to-RTGS Account' when 'Pay' then 'Pending Payment SUSPENSE-to-RTGS Account' when 'Reverse' then 'Pending Reversal SUSPENSE-to-CUSTOMER' when 'Reversed' then 'Reversal Completed' when 'Approved' then 'Approved By TROPS' when 'CustomerCareRejection' then 'Customer Care Rejected' else  Status END  status,Uploaded_date ,(select messageType from tblMessageTypes where id=messagetypeID) type      ,(select messageVariant from tblMessageVariant where id=[messageVariantID]) variant ,[requestingBranch]    from transactions where status='" & easyrtgs.TREASURY_STATUS & "' order by Authorized_date desc"

        End If

        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_TROPS).ToString().Trim().ToLower() Then
            strsql = "select transactionID,[messagetypeID],customer_name,customer_account,amount,charges,Case Status when 'Uploaded' THEN 'Pending With SM' WHEN 'CustomerCare' THEN 'Pending With Customer Care' WHEN 'Authorize' THEN 'Pending Payment CUSTOMER-to-SUSPENSE(Authorize)' WHEN 'Authorized' THEN 'Pending With TROPS(Authorized)'   WHEN 'SwiftInProcess' THEN 'Completed By TROPS (SwiftInProcess)'  WHEN 'SwiftFailed' then 'Completed By TROPS (SwiftFailed)' WHEN 'Paid' then 'Successfully Paid SUSPENSE-to-RTGS Account' when 'Pay' then 'Pending Payment SUSPENSE-to-RTGS Account' when 'Reverse' then 'Pending Reversal SUSPENSE-to-CUSTOMER' when 'Reversed' then 'Reversal Completed' when 'Approved' then 'Approved By TROPS' when 'CustomerCareRejection' then 'Customer Care Rejected' else  Status END  status,Uploaded_date ,(select messageType from tblMessageTypes where id=messagetypeID) type      ,(select messageVariant from tblMessageVariant where id=[messageVariantID]) variant ,[requestingBranch]    from transactions where status='" & easyrtgs.TREASURY_STATUS & "' order by Authorized_date desc"
        End If

        SetHomeLinkUrl(Session("role").ToString())
        Using conn
            Try
                conn.Open()
                Dim cmd As New SqlCommand
                cmd.Connection = conn
                cmd.CommandText = strsql
                Dim rs As SqlDataReader
                rs = cmd.ExecuteReader()
                If rs.HasRows Then
                    rs.Close()
                    Dim da As New SqlDataAdapter(strsql, conn)

                    Dim ds As New Data.DataSet
                    da.Fill(ds)
                    DataGrid1.DataSource = ds
                    DataGrid1.DataBind()

                Else

                    Label4.Text = "No Pending Transactions"
                End If

            Catch ex As Exception

            End Try
        End Using
        
    End Sub

    Protected Sub DataGrid1_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles DataGrid1.PageIndexChanged
        DataGrid1.CurrentPageIndex = e.NewPageIndex
        Dim conn As New SqlConnection(esy.conn1())
        Using conn
            Try
                conn.Open()


                Dim da As New SqlDataAdapter(strsql, conn)


                Dim ds As New Data.DataSet
                da.Fill(ds)
                DataGrid1.DataSource = ds
                DataGrid1.DataBind()
                conn.Close()
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try

        End Using
       
    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Label3.Visible = True

        Dim conn As New SqlConnection(esy.conn1())
        Using conn
            Try
                conn.Open()
                Dim strsql As String = ""
                If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_SERVICE_MANAGER).ToString().Trim().ToLower() Then
                    Label3.Text = "My Authorized Transactions"
                    strsql = "select transactionID,[messagetypeID],customer_name,customer_account,amount,charges,Case Status when 'Uploaded' THEN 'Pending With SM' WHEN 'CustomerCare' THEN 'Pending With Customer Care' WHEN 'Authorize' THEN 'Pending Payment CUSTOMER-to-SUSPENSE(Authorize)' WHEN 'Authorized' THEN 'Pending With TROPS(Authorized)'   WHEN 'SwiftInProcess' THEN 'Completed By TROPS (SwiftInProcess)'  WHEN 'SwiftFailed' then 'Completed By TROPS (SwiftFailed)' WHEN 'Paid' then 'Successfully Paid SUSPENSE-to-RTGS Account' when 'Pay' then 'Pending Payment SUSPENSE-to-RTGS Account' when 'Reverse' then 'Pending Reversal SUSPENSE-to-CUSTOMER' when 'Reversed' then 'Reversal Completed' when 'Approved' then 'Approved By TROPS' when 'CustomerCareRejection' then 'Customer Care Rejected' else  Status END  status,Uploaded_date ,(select messageType from tblMessageTypes where id=messagetypeID) type      ,(select messageVariant from tblMessageVariant where id=[messageVariantID]) variant ,[requestingBranch]  from transactions where authorized_by='" & Session("name") & "' order by authorized_date desc"
                End If
                If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_TREASURY).ToString().Trim().ToLower() Then
                    Label3.Text = "My Approved Transactions"
                    strsql = "select transactionID,[messagetypeID],customer_name,customer_account,amount,charges,Case Status when 'Uploaded' THEN 'Pending With SM' WHEN 'CustomerCare' THEN 'Pending With Customer Care' WHEN 'Authorize' THEN 'Pending Payment CUSTOMER-to-SUSPENSE(Authorize)' WHEN 'Authorized' THEN 'Pending With TROPS(Authorized)'   WHEN 'SwiftInProcess' THEN 'Completed By TROPS (SwiftInProcess)'  WHEN 'SwiftFailed' then 'Completed By TROPS (SwiftFailed)' WHEN 'Paid' then 'Successfully Paid SUSPENSE-to-RTGS Account' when 'Pay' then 'Pending Payment SUSPENSE-to-RTGS Account' when 'Reverse' then 'Pending Reversal SUSPENSE-to-CUSTOMER' when 'Reversed' then 'Reversal Completed' when 'Approved' then 'Approved By TROPS' when 'CustomerCareRejection' then 'Customer Care Rejected' else  Status END  status,Uploaded_date ,(select messageType from tblMessageTypes where id=messagetypeID) type      ,(select messageVariant from tblMessageVariant where id=[messageVariantID]) variant ,[requestingBranch]  from transactions where Treasury_Approval='" & Session("name") & "' order by approved_date desc"
                End If

                Dim cmd As New SqlCommand
                cmd.Connection = conn
                cmd.CommandText = strsql
                Dim rs As SqlDataReader
                rs = cmd.ExecuteReader
                If rs.HasRows Then

                    rs.Close()
                    Dim da As New SqlDataAdapter(strsql, conn)
                    Dim ds As New Data.DataSet
                    da.Fill(ds)
                    DataGrid2.DataSource = ds
                    DataGrid2.DataBind()
                    DataGrid2.Focus()




                Else
                    Label4.Text = "No transactions"

                End If
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try


        End Using
        
    End Sub


    Protected Sub DataGrid2_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles DataGrid2.PageIndexChanged
        DataGrid2.CurrentPageIndex = e.NewPageIndex
        Dim conn As New SqlConnection(esy.conn1())
        conn.Open()
        Dim strsql As String = ""
        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_SERVICE_MANAGER).ToString().Trim().ToLower() Then
            strsql = "select *  from transactions where authorized_by='" & Session("name") & "' order by authorized_date desc"
        End If
        If Session("role").ToString().Trim().ToLower() = (easyrtgs.ROLE_TREASURY).ToString().Trim().ToLower() Then
            strsql = "select *  from transactions where Treasury_Approval='" & Session("name") & "' order by approved_date desc"
        End If
        Dim da As New SqlDataAdapter(strsql, conn)
        Dim ds As New Data.DataSet
        da.Fill(ds)
        DataGrid2.DataSource = ds
        DataGrid2.DataBind()
        conn.Close()
        DataGrid2.Focus()

    End Sub

    Private Sub SetHomeLinkUrl(ByVal rolename As String)
        Select Case rolename.Trim().ToLower()
            Case (easyrtgs.ROLE_TROPS).Trim().ToLower(), (easyrtgs.ROLE_SERVICE_MANAGER).Trim().ToLower()
                hypHome.NavigateUrl = "~/trops/trops_landing.aspx"
                hypMyTransactions.NavigateUrl = "~/mytransactions.aspx"
                hypFundingReport.NavigateUrl = "~/funding.aspx"
                hypSwiftManager.NavigateUrl = "~/swift.aspx"
                hypSelfService.NavigateUrl = "~/SelfService/selfserv.aspx"
                hypLogout.NavigateUrl = "~/logout.aspx"
            Case (easyrtgs.ROLE_TREASURY).Trim().ToLower()
                hypHome.NavigateUrl = "mytransactions.aspx"
            Case Else
                hypHome.NavigateUrl = "main.aspx"
        End Select

    End Sub

End Class
