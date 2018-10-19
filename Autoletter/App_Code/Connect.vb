Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Xml
Imports System.Text
Imports System.Data.SqlClient
Imports System

Public Class Connect
    Public conn As SqlConnection
    Public sql As SqlCommand
    Public num_rows As Integer
    Public returnValue As Integer
    Public Sub New(ByVal query As String)
        Me.conn = New SqlConnection()
        Me.conn.ConnectionString = ConfigurationManager.ConnectionStrings("dbConn").ConnectionString
        Me.conn.Open()
        Me.sql = New SqlCommand()
        Me.sql.Connection = conn
        Me.sql.CommandText = query
        'new ErrorLog(query);
        Me.sql.CommandType = CommandType.Text
    End Sub
    Public Sub addparam(ByVal key As String, ByVal val As Object)
        Dim prm As SqlParameter = Me.sql.Parameters.AddWithValue(key, val)
    End Sub
    Public Function query(ByVal tblName As String) As DataSet
        Dim res As New SqlDataAdapter()
        res.SelectCommand = Me.sql
        res.TableMappings.Add("Table", tblName)
        Dim ds As New DataSet()
        res.Fill(ds)
        num_rows = ds.Tables(tblName).Rows.Count
        Me.close()
        Return ds
    End Function

    Public Function query() As Integer
        Dim prm As New SqlParameter()
        prm.SqlDbType = SqlDbType.Int
        prm.Direction = ParameterDirection.ReturnValue
        Me.sql.Parameters.Add(prm)
        returnValue = 0
        Dim j As Integer = 0
        Try
            j = sql.ExecuteNonQuery()
            returnValue = Convert.ToInt32(prm.Value)
            close()
        Catch ex As Exception
            ' Dim e As New ErrorLog(ex)
        End Try
        Return j
    End Function
    Public Function [select]() As DataSet
        Dim res As New SqlDataAdapter()
        res.SelectCommand = Me.sql
        res.TableMappings.Add("Table", "recs")
        Dim ds As New DataSet()
        res.Fill(ds)
        num_rows = ds.Tables("recs").Rows.Count
        Me.close()
        Return ds
    End Function
    Public Function delete() As Integer
        Dim j As Integer = 0
        Try
            j = sql.ExecuteNonQuery()
            close()
        Catch ex As Exception
            ' Dim e As New ErrorLog(ex)
        End Try
        Return j
    End Function
    Public Function update() As Integer
        Dim j As Integer = 0
        Try
            j = sql.ExecuteNonQuery()
            close()
        Catch ex As Exception
            ' Dim e As New ErrorLog(ex)
        End Try
        Return j
    End Function
    Public Function insert() As Integer
        Me.sql.CommandText += "; select @@IDENTITY "
        Dim j As Integer = 0
        Try
            j = Convert.ToInt32(sql.ExecuteScalar())
            close()
        Catch ex As Exception
            ' Dim e As New ErrorLog(ex)
        End Try
        Return j
    End Function
    Public Function selectScalar() As String
        Dim j As String = ""
        Try
            j = Convert.ToString(sql.ExecuteScalar())
            close()
        Catch ex As Exception
            ' Dim e As New ErrorLog(ex)
        End Try
        Return j
    End Function
    Public Sub close()
        Me.conn.Close()
    End Sub
End Class