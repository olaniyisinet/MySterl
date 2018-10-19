Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class BankDetail
    Public Property Bank_code As String
    Public Property Bank_Name As String
    Public Property Status_Flag As Integer
    Public Property Bic As String
    Public Property Nuban As String
    Public Property IsNameEnquirySupported As Boolean
    Public Property AlternateAccount As String

    Public Sub New()
        Bank_code = String.Empty
        Bank_Name = String.Empty
        Status_Flag = -1
        Bic = String.Empty
        Nuban = String.Empty
        IsNameEnquirySupported = False
    End Sub
    Public Function Save() As Boolean
        Dim isSaved As Boolean = False
        '
        Dim sqlSaveBankDetail As String = "spSaveBankDetail"
        Using cn = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            Dim cmd As New SqlCommand(sqlSaveBankDetail, cn)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@Bank_code", Bank_code)
            cmd.Parameters.AddWithValue("@Bank_Name", Bank_Name)
            cmd.Parameters.AddWithValue("@Status_Flag", Status_Flag)
            cmd.Parameters.AddWithValue("@Bic", Bic)
            cmd.Parameters.AddWithValue("@Nuban", Nuban)
            If cmd.ExecuteNonQuery() > 0 Then
                isSaved = True
            Else
                isSaved = False
            End If

        End Using

        Return isSaved
    End Function

    Public Shared Function GetBankDetailByCode(ByVal bankCode As String) As BankDetail
        Dim bd As New BankDetail()
        Dim sql As String = "spReadBankDetailByCode"
        Dim esy As New easyrtgs
        Using cn = New SqlConnection(esy.sqlconn1())
            cn.Open()
            Dim cmd As New SqlCommand(sql, cn)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@Bank_code", bankCode)

            Dim dr As SqlDataReader = cmd.ExecuteReader()
            If dr.HasRows Then
                While dr.Read()
                    bd.Bank_code = dr("bank_code").ToString()
                    bd.Bank_Name = dr("bank_name").ToString()
                    bd.Status_Flag = dr("statusflag").ToString()
                    bd.Bic = dr("bic").ToString()
                    bd.Nuban = dr("nuban").ToString()
                    If Not IsDBNull(dr("supportsNameQuery")) Then
                        If Not String.IsNullOrEmpty(dr("supportsNameQuery").ToString()) Then
                            Dim nameQueryVal As Integer = -1
                            nameQueryVal = Convert.ToInt32(dr("supportsNameQuery"))
                            Select Case nameQueryVal
                                Case 0
                                    bd.IsNameEnquirySupported = False
                                Case 1
                                    bd.IsNameEnquirySupported = True
                                Case Else
                                    bd.IsNameEnquirySupported = False
                            End Select
                        Else
                            bd.IsNameEnquirySupported = False
                        End If
                    End If
                    'bd.IsNameEnquirySupported=IIf(dr("supportsNameQuery").ToString()
                    If Not IsDBNull(dr("alternateAccount")) Then
                        bd.AlternateAccount = dr("alternateAccount").ToString()
                    Else
                        bd.AlternateAccount = String.Empty
                    End If

                End While
            End If
        End Using
        Return bd
    End Function

    Public Shared Function GetBankDetailByBankName(ByVal bankname As String) As BankDetail
        Dim bd As New BankDetail()
        Dim sql As String = "spReadBankDetailByName"
        Dim esy As New easyrtgs
        Using cn = New SqlConnection(esy.sqlconn1())
            cn.Open()
            Dim cmd As New SqlCommand(sql, cn)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@Bank_Name", bankname)

            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Using dr
                If dr IsNot Nothing AndAlso dr.HasRows Then

                    While dr.Read
                        bd.Bank_code = dr("bank_code").ToString()
                        bd.Bank_Name = dr("bank_name").ToString()
                        bd.Status_Flag = dr("statusflag").ToString()
                        bd.Bic = dr("bic").ToString()
                        bd.Nuban = dr("nuban").ToString()
                    End While
                End If
            End Using

        End Using
        Return bd
    End Function

    Public Shared Function DeleteByBankCode(ByVal bc As String) As Boolean
        Dim isDeleted As Boolean = False
        Dim sql As String = "spDeleteBankDetail"

        Using cn = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            Dim cmd As New SqlCommand(sql, cn)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@bank_code", bc)
            If cmd.ExecuteNonQuery() >= 0 Then
                isDeleted = True
            End If

        End Using
        Return isDeleted
    End Function

    Public Function Delete() As Boolean
        Dim isDeleted As Boolean = False
        Dim sql As String = "spDeleteBankDetail"

        Using cn = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            Dim cmd As New SqlCommand(sql, cn)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@bank_code", Bank_code)
            If cmd.ExecuteNonQuery() >= 0 Then
                isDeleted = True
            End If

        End Using
        Return isDeleted
    End Function

    Public Shared Function GetAllBankDetails() As List(Of BankDetail)
        Dim list As New List(Of BankDetail)

        Using cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            Dim sql As String = "spGetAllBankDetails"
            Dim cmd As New SqlCommand()
            cmd.Connection = cn
            cmd.CommandText = sql
            cmd.CommandType = Data.CommandType.StoredProcedure
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Using dr
                While dr.Read
                    Dim bd As New BankDetail
                    bd.Bank_code = dr("bank_code").ToString()
                    bd.Bank_Name = dr("bank_name").ToString
                    bd.Status_Flag = dr("statusflag").ToString
                    bd.Bic = dr("bic").ToString
                    bd.Nuban = dr("nuban").ToString

                    list.Add(bd)
                End While

                dr.Close()
            End Using
        End Using
        Return list
    End Function

    Public Shared Function getCbnBankCode() As String
        'NAMENGLA
        Dim bcode As String = String.Empty
        Dim sql As String = "SELECT bank_code , bank_name  , statusflag  , bic      , nuban  FROM tbl_banks  where bic='CBNINGLA'"
        Dim cn As New Connect(sql)
        Dim ds As DataSet = Nothing
        ds = cn.query("rs")
        If ds.Tables.Count > 0 Then
            If ds.Tables(0).Rows.Count > 0 Then
                If Not IsDBNull(ds.Tables(0).Rows(0)("bank_code")) Then
                    bcode = ds.Tables(0).Rows(0)("bank_code").ToString()
                Else
                    Return String.Empty
                End If

            End If
        End If
        Return bcode
    End Function

    Public Shared Function getSterlingBankCode() As String
        '
        Dim bcode As String = String.Empty
        Dim sql As String = "SELECT bank_code , bank_name  , statusflag  , bic      , nuban  FROM tbl_banks  where bic='NAMENGLA'"
        Dim cn As New Connect(sql)
        Dim ds As DataSet = Nothing
        ds = cn.query("rs")
        If ds.Tables.Count > 0 Then
            If ds.Tables(0).Rows.Count > 0 Then
                If Not IsDBNull(ds.Tables(0).Rows(0)("bank_code")) Then
                    bcode = ds.Tables(0).Rows(0)("bank_code").ToString()
                Else
                    Return String.Empty
                End If

            End If
        End If
        Return bcode
    End Function

End Class
