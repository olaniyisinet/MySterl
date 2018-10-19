Imports System.Data
Imports System.Collections.Generic
Imports System.Text


Public Class Accounts



    Public cusname As String
    Public fullacct As String
    Public cusnum As String
    Public bracode As String
    Public curcode As String
    Public ledcode As String
    Public subacctcode As String

    Public Sub setRecs(dr As DataRow)
        cusname = dr("cusname").ToString()
        bracode = dr("bracode").ToString()
        cusnum = dr("cusnum").ToString()
        curcode = dr("curcode").ToString()
        ledcode = dr("ledcode").ToString()
        subacctcode = dr("subacctcode").ToString()
    End Sub


End Class
