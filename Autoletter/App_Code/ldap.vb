Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Collections.Specialized
Imports System.DirectoryServices
Imports System

Public Class ldap
    Public username As String
    Public fullname As String
    Public firstname As String
    Public lastname As String
    Public email As String
    Public mobile As String
    Public pager As String
    Public sip As String
    Public title As String
    Public cug As String
    Public err As String
    Public department As String
    Public grade As String

    Public Function login(ByVal username As String, ByVal password As String) As Boolean
        Dim path As String = "LDAP://sterlingbank"
        Dim entry As New DirectoryEntry(path, "sterlingbank\" & username, password)
        Try
            Dim native As [Object] = entry.NativeObject
            Return True
        Catch ex As Exception
            err = ex.Message
            Return False
        End Try
    End Function
    Protected Function GetProperty(ByVal searchResult As SearchResult, ByVal PropertyName As String) As String
        If searchResult.Properties.Contains(PropertyName) Then
            Return searchResult.Properties(PropertyName)(0).ToString()
        Else
            Return String.Empty
        End If
    End Function

    Public Function searchUsers(ByVal xusername As String) As DataTable
        Dim dt As New DataTable("sr")
        Dim col As DataColumn

        col = dt.Columns.Add("username", Type.[GetType]("System.String"))
        col = dt.Columns.Add("fullname", Type.[GetType]("System.String"))
        col = dt.Columns.Add("email", Type.[GetType]("System.String"))
        col = dt.Columns.Add("firstname", Type.[GetType]("System.String"))
        col = dt.Columns.Add("lastname", Type.[GetType]("System.String"))

        Dim row As DataRow
        Try
            Dim _path As String = "LDAP://sterlingbank"
            Dim _filterAttribute As String = xusername
            Dim dSearch As New DirectorySearcher(_path)
            dSearch.Filter = "(&(objectClass=user)(cn=*" & _filterAttribute & "*))"
            For Each sResultSet As SearchResult In dSearch.FindAll()
                row = dt.NewRow()
                row("username") = GetProperty(sResultSet, "sAMAccountName")
                'username
                row("fullname") = GetProperty(sResultSet, "cn")
                ' full Name
                row("email") = GetProperty(sResultSet, "mail")
                'Email
                row("firstname") = GetProperty(sResultSet, "givenName")
                ' First Name
                row("lastname") = GetProperty(sResultSet, "sn")
                If row("email") IsNot "" Then
                    dt.Rows.Add(row)
                End If
            Next

        Catch ex As Exception
        End Try
        Dim v As DataView = dt.DefaultView
        v.Sort = "fullname"
        Return v.ToTable()
    End Function

    Public Function searchAgents(ByVal xusername As String) As DataTable
        Dim dt As New DataTable("sr")
        Dim col As DataColumn

        col = dt.Columns.Add("username", Type.[GetType]("System.String"))
        col = dt.Columns.Add("fullname", Type.[GetType]("System.String"))
        col = dt.Columns.Add("email", Type.[GetType]("System.String"))
        col = dt.Columns.Add("firstname", Type.[GetType]("System.String"))
        col = dt.Columns.Add("lastname", Type.[GetType]("System.String"))
        col = dt.Columns.Add("description", Type.[GetType]("System.String"))

        Dim row As DataRow
        Try
            Dim _path As String = "LDAP://sterlingbank"
            Dim _filterAttribute As String = xusername
            Dim dSearch As New DirectorySearcher(_path)
            dSearch.Filter = "(&(objectClass=user)(cn=*" & _filterAttribute & "*)(description=DSE Officer))"
            For Each sResultSet As SearchResult In dSearch.FindAll()
                row = dt.NewRow()
                row("username") = GetProperty(sResultSet, "sAMAccountName")
                'username
                row("fullname") = GetProperty(sResultSet, "cn")
                ' full Name
                row("email") = GetProperty(sResultSet, "mail")
                'Email
                row("firstname") = GetProperty(sResultSet, "givenName")
                ' First Name
                row("lastname") = GetProperty(sResultSet, "sn")
                row("description") = GetProperty(sResultSet, "description")
                If row("email") IsNot "" Then
                    dt.Rows.Add(row)
                End If
            Next

        Catch ex As Exception
        End Try
        Dim v As DataView = dt.DefaultView
        v.Sort = "fullname"
        Return v.ToTable()
    End Function
    Public Function GetInfo(ByVal xusername As String) As Staff
        Dim s As New Staff()
        s.username = "0"
        Try
            Dim _path As String = "LDAP://sterlingbank"
            Dim _filterAttribute As String = xusername
            Dim dSearch As New DirectorySearcher(_path)
            dSearch.Filter = "(&(objectClass=user)(sAMAccountName=" & _filterAttribute & "))"
            For Each sResultSet As SearchResult In dSearch.FindAll()
                s.username = GetProperty(sResultSet, "sAMAccountName")
                'username
                s.fullName = GetProperty(sResultSet, "cn")
                ' full Name
                s.firstName = GetProperty(sResultSet, "givenName")
                ' First Name
                s.lastName = GetProperty(sResultSet, "sn")
                ' Last Name
                s.jobTitle = GetProperty(sResultSet, "title")
                ' Company
                s.communicator = GetProperty(sResultSet, "msRTCSIP-PrimaryUserAddress")
                'communicator
                s.mobile = GetProperty(sResultSet, "mobile")
                'City
                s.email = GetProperty(sResultSet, "mail")
                'Email
                s.deptName = GetProperty(sResultSet, "department")
                'department
                s.grade = GetProperty(sResultSet, "extensionAttribute1")
                'grade
                s.gender = GetProperty(sResultSet, "extensionAttribute3")
                'gender
                s.mstatus = GetProperty(sResultSet, "extensionAttribute4")
                'marital status
                s.cardnumber = GetProperty(sResultSet, "employeeID")
                'staff num
                s.country = GetProperty(sResultSet, "co")
                'Email
                s.stateName = GetProperty(sResultSet, "st")
                'Email
                s.unitName = GetProperty(sResultSet, "division")
                'Email
                s.region = GetProperty(sResultSet, "streetAddress")
                'Email
                s.officeLocation = GetProperty(sResultSet, "extensionAttribute2")
                'Email
                'Email
                s.description = GetProperty(sResultSet, "description")

                s.tellerid = GetProperty(sResultSet, "extensionAttribute5")
            Next
        Catch ex As Exception
            err = ex.Message
        End Try

        Return s
    End Function


End Class