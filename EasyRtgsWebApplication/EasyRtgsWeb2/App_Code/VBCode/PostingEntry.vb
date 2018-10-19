Imports System.Data.SqlClient
Imports Microsoft.VisualBasic

Public Class PostingEntry
    Public Sub New(ByVal connString As String)
        Me.ConnectionString = connString
        TransactionID = ""
        Status = ""
        ErrorText = ""
        EntryDate = DateTime.Now
        MFNo = String.Empty
        CustomerName = String.Empty
        Amount = 0
        Officer = String.Empty
        Supervisor = String.Empty
        CrCommAcct = String.Empty
        DrCustAcct = String.Empty
        expl_code = 0
        Remarks = String.Empty
        Source = String.Empty
        TransRef = String.Empty
    End Sub
    Public Sub New()
        Me.ConnectionString = String.Empty
        TransactionID = ""
        Status = ""
        ErrorText = ""
        EntryDate = DateTime.Now
        MFNo = String.Empty
        CustomerName = String.Empty
        Amount = 0
        Officer = String.Empty
        Supervisor = String.Empty
        CrCommAcct = String.Empty
        DrCustAcct = String.Empty
        expl_code = 0
        Remarks = String.Empty
        Source = String.Empty
        TransRef = String.Empty
    End Sub
    Public Property ConnectionString As String
    Public Property TransactionID As String
    Public Property Status As String
    Public Property ErrorText As String
    Public Property EntryDate As DateTime
    Public Property MFNo As String
    Public Property CustomerName As String
    Public Property Amount As Decimal
    Public Property Officer As String
    Public Property Supervisor As String
    Public Property CrCommAcct As String
    Public Property DrCustAcct As String
    Public Property expl_code As Integer
    Public Property Remarks As String
    Public Property Source As String
    Public Property TransRef As String

    Private _ID As Long
    'This is readonly because it is an auto-generated field on the table TransactTemp2
    Public ReadOnly Property ID() As Long
        Get
            Return _ID
        End Get

    End Property

    ''' <summary>
    ''' Save a record into table TransactTemp2.
    ''' </summary>
    ''' <returns>Returns the auto-id. And a negative value if there is an error. It throws an <code>ArgumentException</code> if the connectionstring property is not set.</returns>
    Public Function Save() As Long
        Dim idValue As Long = -1
        If String.IsNullOrEmpty(ConnectionString) Then
            Throw New ArgumentException("The connection string is null or empty")
        End If
        Dim sql As String = "INSERT INTO TRANSACTTEMP2(TransactionID,Status,ErrorText,EntryDate,MFNo,CustomerName,Amount,Officer,Supervisor,CrCommAcct,DrCustAcct,expl_code,Remarks,Source,TransRef) OUTPUT INSERTED.ID VALUES(@TransactionID,@Status,@ErrorText,@EntryDate,@MFNo,@CustomerName,@Amount,@Officer,@Supervisor,@CrCommAcct,@DrCustAcct,@expl_code,@Remarks,@Source,@TransRef)"
        Dim cn As New SqlConnection(ConnectionString)
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddWithValue("@TransactionID", TransactionID)
                cmd.Parameters.AddWithValue("@Status", Status)
                cmd.Parameters.AddWithValue("@ErrorText", ErrorText)
                cmd.Parameters.AddWithValue("@EntryDate", EntryDate)
                cmd.Parameters.AddWithValue("@MFNo", MFNo)
                cmd.Parameters.AddWithValue("@CustomerName", CustomerName)
                cmd.Parameters.AddWithValue("@Amount", Amount)
                cmd.Parameters.AddWithValue("@Officer", Officer)
                cmd.Parameters.AddWithValue("@Supervisor", Supervisor)
                cmd.Parameters.AddWithValue("@CrCommAcct", CrCommAcct)
                cmd.Parameters.AddWithValue("@DrCustAcct", DrCustAcct)
                cmd.Parameters.AddWithValue("@expl_code", expl_code)
                cmd.Parameters.AddWithValue("@Remarks", Remarks)
                cmd.Parameters.AddWithValue("@Source", Source)
                cmd.Parameters.AddWithValue("@TransRef", TransRef)
                idValue = CType(cmd.ExecuteScalar(), Long) 'ExecuteScalar will return the auto-generated ID of the table bcos the INSERT statement has the OUTPUT INSERTED.<ID_COLUMN_NAME> clause
                _ID = idValue
            Catch ex As Exception
                Gadget.LogException(ex)
                idValue = -1
            End Try
        End Using

        Return idValue
    End Function

    Public Shared Function SaveInTransaction(ByVal EntryList As List(Of PostingEntry), ByVal ConnectionString As String) As Boolean
        Dim isAllSaved As Boolean = False
        If String.IsNullOrEmpty(ConnectionString) Then
            Throw New ArgumentException("The connection string is null or empty")
        End If
        Dim sql As String = "INSERT INTO TRANSACTTEMP2(TransactionID,Status,ErrorText,EntryDate,MFNo,CustomerName,Amount,Officer,Supervisor,CrCommAcct,DrCustAcct,expl_code,Remarks,Source,TransRef) OUTPUT INSERTED.ID VALUES(@TransactionID,@Status,@ErrorText,@EntryDate,@MFNo,@CustomerName,@Amount,@Officer,@Supervisor,@CrCommAcct,@DrCustAcct,@expl_code,@Remarks,@Source,@TransRef)"
        Dim isEntrySaved As Boolean = True
        Dim cmd As New SqlCommand(sql)
        Dim cn As New SqlConnection(ConnectionString)
        Dim trans As SqlTransaction
        Using cn
            Try
                cn.Open()
                trans = cn.BeginTransaction()
                cmd.Connection = cn
                cmd.Transaction = trans
                Using trans
                    Try
                        For Each e As PostingEntry In EntryList
                            cmd.Parameters.AddWithValue("@TransactionID", e.TransactionID)
                            cmd.Parameters.AddWithValue("@Status", e.Status)
                            cmd.Parameters.AddWithValue("@ErrorText", e.ErrorText)
                            cmd.Parameters.AddWithValue("@EntryDate", e.EntryDate)
                            cmd.Parameters.AddWithValue("@MFNo", e.MFNo)
                            cmd.Parameters.AddWithValue("@CustomerName", e.CustomerName)
                            cmd.Parameters.AddWithValue("@Amount", e.Amount)
                            cmd.Parameters.AddWithValue("@Officer", e.Officer)
                            cmd.Parameters.AddWithValue("@Supervisor", e.Supervisor)
                            cmd.Parameters.AddWithValue("@CrCommAcct", e.CrCommAcct)
                            cmd.Parameters.AddWithValue("@DrCustAcct", e.DrCustAcct)
                            cmd.Parameters.AddWithValue("@expl_code", e.expl_code)
                            cmd.Parameters.AddWithValue("@Remarks", e.Remarks)
                            cmd.Parameters.AddWithValue("@Source", e.Source)
                            cmd.Parameters.AddWithValue("@TransRef", e.TransRef)
                            IIf(cmd.ExecuteNonQuery() > 0, isEntrySaved = isEntrySaved And True, isEntrySaved = isEntrySaved And False)
                            cmd.Parameters.Clear()
                        Next

                        If isEntrySaved Then
                            trans.Commit()
                            isAllSaved = True
                        Else
                            trans.Rollback()
                            isAllSaved = False
                        End If
                    Catch ex As Exception
                        trans.Rollback()
                        Gadget.LogException(ex)

                    End Try

                End Using
            Catch ex As Exception
                Gadget.LogException(ex)

            End Try
        End Using



        Return isAllSaved
    End Function


End Class
