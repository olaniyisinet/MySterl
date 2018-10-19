Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports comtest

Public Class FailedSwift
    Private _swiftStatus As String
    Private _sendersReference As String
    Public ReadOnly Property SendersReference As String
        Get
            Return _sendersReference
        End Get
    End Property
    Public Property OrigtBranch As String
    Public Property SendersAccount As String
    Public Property SwiftMessage As String
    Public ReadOnly Property SwiftStatus As String
        Get
            Return _swiftStatus
        End Get
    End Property

    Public Sub New(ByVal sendersReference As String)
        Me._sendersReference = sendersReference
        BuildObj(Me._sendersReference)

    End Sub

    Public Sub New()

    End Sub
    Public Function GetFailedSendersReferences() As List(Of FailedSwift)
        Dim lst As New List(Of FailedSwift)
        Dim fileList() As String
        Dim rtgsHost As String = ConfigurationManager.AppSettings("SWIFT_HOST")
        Dim failedFolder As String = ConfigurationManager.AppSettings("RTGS_FAILED_FOLDER")
        Dim usr As String = ConfigurationManager.AppSettings("RTGS_USR")
        Dim pwd As String = ConfigurationManager.AppSettings("RTGS_PWD")

        Try

            Using svc = New Service
                fileList = svc.FtpDownloadFileList(rtgsHost, failedFolder, usr, pwd)

            End Using

            If fileList.Length = 1 Then
                Dim sql As String = "SELECT TransactionID FROM transactions WHERE status in ('SwiftInProcess', 'SwiftFailed') order by [date] desc"
                Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
                Using cn
                    Try
                        cn.Open()
                        Dim cmd As New SqlCommand(sql, cn)
                        Dim dr As SqlDataReader = cmd.ExecuteReader()
                        If dr IsNot Nothing AndAlso dr.HasRows Then
                            Using dr
                                While dr.Read
                                    Dim fs As FailedSwift
                                    fs = New FailedSwift(dr("TransactionID").ToString())
                                    lst.Add(fs)
                                End While
                            End Using
                        End If
                    Catch exs As Exception

                    End Try
                End Using
            End If
        Catch ex As Exception
            Dim sql As String = "SELECT TransactionID FROM transactions WHERE status in ('SwiftInProcess', 'SwiftFailed') order by [date] desc"
            Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            Using cn
                Try
                    cn.Open()
                    Dim cmd As New SqlCommand(sql, cn)
                    Dim dr As SqlDataReader = cmd.ExecuteReader()
                    If dr IsNot Nothing AndAlso dr.HasRows Then
                        Using dr
                            While dr.Read
                                Dim fs As FailedSwift
                                fs = New FailedSwift(dr("TransactionID").ToString())
                                lst.Add(fs)
                            End While
                        End Using
                    End If
                Catch exs As Exception

                End Try
            End Using
        End Try



        Return lst
    End Function
    Private Sub BuildObj(ByVal sendersReference As String)
        Dim sql As String = "select * from transactions where TransactionID=@transid"
        Dim cn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
        Using cn
            Try
                cn.Open()
                Dim cmd As New SqlCommand()
                cmd.CommandType = Data.CommandType.Text
                cmd.CommandText = sql
                cmd.Connection = cn
                cmd.Parameters.AddWithValue("@transid", sendersReference)
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                If dr IsNot Nothing AndAlso dr.HasRows Then
                    Using dr
                        While dr.Read
                            _sendersReference = dr("TransactionID").ToString
                            _swiftStatus = dr("Status").ToString
                            SwiftMessage = dr("mt103_text").ToString
                            SendersAccount = dr("Customer_account").ToString
                            OrigtBranch = dr("Branch").ToString

                        End While
                    End Using

                End If
            Catch ex As Exception
                Throw
            End Try
        End Using
    End Sub


End Class
