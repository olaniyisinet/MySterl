Imports Microsoft.VisualBasic
Imports System.Data.SqlClient

Public Class CustomerCareOps

    Function saveCustomerCareResponse(ByVal transid As String, ByVal responderName As String, ByVal customerResponse As String, ByVal custCareComment As String, ByVal custofficeruserid As String, ByVal amt As Decimal) As Boolean
        Dim status As Boolean = False
        'TODO: Implement please
        Dim phone As String = String.Empty
        Dim custcareofficer As String = custofficeruserid
        Dim amount As Decimal = amt
        Dim reason As String = custCareComment

        Dim cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString())
        Using cn
            Try
                Dim sqlBuilder As New StringBuilder
                sqlBuilder.AppendLine("INSERT INTO [tblCustCareResponses]")
                sqlBuilder.AppendLine("([transactionid] ,[requestingCustomerPhone] ,[custcareofficerUserid]  ,")
                sqlBuilder.AppendLine("responderName, [amount], [reason] ,[entrydate])    VALUES(")
                sqlBuilder.AppendLine("@transactionid,@requestingCustomerPhone ,@custcareofficerUserid,@responderName")
                sqlBuilder.AppendLine(",@amount  ,@reason    ,@entrydate)")
                Dim sql As String = sqlBuilder.ToString()
                sqlBuilder.Clear()


                Dim cmd As New SqlCommand(sql, cn)
                cn.Open()

                cmd.Parameters.AddWithValue("@transactionid", transid)
                cmd.Parameters.AddWithValue("@requestingCustomerPhone", phone)
                cmd.Parameters.AddWithValue("@custcareofficerUserid", custcareofficer)
                cmd.Parameters.AddWithValue("@responderName", responderName)
                cmd.Parameters.AddWithValue("@responderTelNum", responderName)
                cmd.Parameters.AddWithValue("@amount", amount)
                cmd.Parameters.AddWithValue("@reason", reason)
                cmd.Parameters.AddWithValue("@entrydate", DateTime.Now)
                Dim retcount As Integer = cmd.ExecuteNonQuery()


                cn.Close()

                If retcount > 0 Then
                    status = True
                Else
                    status = False
                End If
            Catch ex As Exception
                Gadget.LogException(ex)
                status = False
            End Try
        End Using
        Return status
    End Function

End Class
