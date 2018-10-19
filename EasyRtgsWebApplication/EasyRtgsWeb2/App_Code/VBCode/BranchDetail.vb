Imports Microsoft.VisualBasic
Imports BankCore
Imports System.Data.SqlClient

Public Class BranchDetail
    Public Sub New()

    End Sub

    Public Property BranchCode As String
    Public Property Cusnum As String
    Public Property CurrencyCode As String
    Public Property LedgerCode As String
    Public Property SubAccountCode As String
    Public Property Name As String

    Public Shared Function GetBranches() As List(Of IBranch)
        Dim l As List(Of IBranch)
        Dim t24 As New t24.T24Bank
        l = t24.Branches
        Return l
    End Function

    Public Shared Function GetBrancheDetails() As List(Of BranchDetail)
        Dim l As New List(Of BranchDetail)
        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("branchConn").ConnectionString)
        Using cn
            Try
                cn.Open()
                Dim r As SqlDataReader = Nothing
                Dim cmd As New SqlCommand("select T24BRANCHID, T24BRANCHID+ '-'+ COMPANYNAME as COMPANYNAME from BranchInfo", cn)
                r = cmd.ExecuteReader()
                Using r
                    If r.HasRows Then
                        While r.Read()
                            Dim b As New BranchDetail()
                            b.BranchCode = r("T24BRANCHID").ToString()
                            b.Name = r("COMPANYNAME").ToString()
                            l.Add(b)
                        End While
                    End If
                End Using
                
            Catch ex As Exception
                Gadget.LogException(ex)
            End Try
        End Using
        Return l
    End Function
End Class
