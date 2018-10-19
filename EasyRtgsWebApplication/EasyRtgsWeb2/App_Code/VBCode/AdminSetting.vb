Imports Microsoft.VisualBasic
Imports System.Data.SqlClient

Public Class AdminSetting
    Public Property ID As Integer
    Public Property RTGSAccount As String
    Public Property PLAccount As String
    Public Property ChargesCustomerAmount As Decimal
    Public Property ChargesStaffAmount As Decimal
    Public Property RTGSEmail As String
    Public Property TreasuryEmail As String
    Public Property RTGSSuspense As String
    Public Property VATAccount As String
    Public Property VATAmount As Decimal
    Public Property IsActive As Boolean
    Public Property CutOffTimeIn24HourFormat As Integer


    Public Sub New()

    End Sub

    Public Function Save() As Boolean
        Dim isSaved As Boolean = False
        '
        Dim sqlSaveBankDetail As String = "spSaveAdminSetting"
        Using cn = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            Dim cmd As New SqlCommand(sqlSaveBankDetail, cn)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@rtgsAccount", RTGSAccount)
            cmd.Parameters.AddWithValue("@plAccount", PLAccount)
            cmd.Parameters.AddWithValue("@chargesCustomerAmount", ChargesCustomerAmount)
            cmd.Parameters.AddWithValue("@chargesStaffAmount", ChargesStaffAmount)
            cmd.Parameters.AddWithValue("@rtgsEmail", RTGSEmail)
            cmd.Parameters.AddWithValue("@treasuryEmail", TreasuryEmail)
            cmd.Parameters.AddWithValue("@rtgsSuspense", RTGSSuspense)
            cmd.Parameters.AddWithValue("@VATAccount", VATAccount)
            cmd.Parameters.AddWithValue("@VATAmount", VATAmount)
            cmd.Parameters.AddWithValue("@isActive", IsActive)
            Dim outParam = New SqlParameter("@id", Data.SqlDbType.Int)
            outParam.Direction = Data.ParameterDirection.Output
            cmd.Parameters.Add(outParam)

            If cmd.ExecuteNonQuery() > 0 Then
                If Not IsDBNull(cmd.Parameters("@id").Value) Then
                    ID = cmd.Parameters("@id").Value
                End If
                isSaved = True
            End If

        End Using

        Return isSaved
    End Function
    ''' <summary>
    ''' can return nothing
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetActiveAdminSettings() As AdminSetting
        Dim ad As AdminSetting = New AdminSetting()
        Dim sql As String = "SELECT id      ,RTGSAccount      ,PLAccount      ,ChargesCustomer      ,ChargesStaff      ,RTGSEmail      ,TreasuryEmail      ,RTGSSuspense      ,VATAccount      ,VATAmount      ,isActive, CutOffTimeIn24HourFormat  FROM Admin WHERE isActive=1 and role='all'"

        Using cn = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            Dim cmd As New SqlCommand(sql, cn)
            cmd.CommandType = Data.CommandType.Text


            Dim dr As SqlDataReader = cmd.ExecuteReader()

            If dr IsNot Nothing AndAlso dr.HasRows Then
                Using dr

                    While dr.Read


                        ad.ID = dr("id").ToString()
                        ad.RTGSAccount = dr("RTGSAccount").ToString
                        ad.PLAccount = dr("PLAccount").ToString
                        ad.ChargesCustomerAmount = dr("ChargesCustomer").ToString
                        ad.ChargesStaffAmount = dr("ChargesStaff").ToString
                        ad.RTGSEmail = dr("RTGSEmail").ToString
                        ad.TreasuryEmail = dr("TreasuryEmail").ToString
                        ad.RTGSSuspense = dr("RTGSSuspense").ToString
                        ad.VATAccount = dr("VATAccount").ToString()
                        ad.VATAmount = dr("VATAmount").ToString
                        If Not IsDBNull(dr("isActive")) AndAlso Convert.ToBoolean(dr("isActive").ToString) = True Then
                            ad.IsActive = True
                        Else
                            ad.IsActive = False
                        End If
                        If Not IsDBNull(dr("CutOffTimeIn24HourFormat")) Then
                            ad.CutOffTimeIn24HourFormat = Convert.ToInt32(dr("CutOffTimeIn24HourFormat").ToString)
                        End If

                    End While
                    Return ad
                End Using

            End If
        End Using
        Return ad
    End Function



    Public Shared Function DeleteByID(ByVal adminID As Integer) As Boolean
        Dim isDeleted As Boolean = False
        Dim sql As String = "spDeleteAdminSettingByID"

        Using cn = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            Dim cmd As New SqlCommand(sql, cn)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@adminID", adminID)
            If cmd.ExecuteNonQuery() >= 0 Then
                isDeleted = True
            End If

        End Using
        Return isDeleted
    End Function

    Public Function Delete() As Boolean
        Dim isDeleted As Boolean = False
        Dim sql As String = "spDeleteAdmin"

        Using cn = New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            Dim cmd As New SqlCommand(sql, cn)
            cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@id", ID)
            If cmd.ExecuteNonQuery() >= 0 Then
                isDeleted = True
            End If

        End Using
        Return isDeleted
    End Function

    Public Shared Function GetAllAdminSetting() As List(Of AdminSetting)
        Dim list As New List(Of AdminSetting)

        Using cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()
            'Dim sql As String = "spGetAllAdminSettings"
            Dim sql As String = "select * from Admin"
            Dim cmd As New SqlCommand()
            cmd.Connection = cn
            cmd.CommandText = sql
            'cmd.CommandType = Data.CommandType.StoredProcedure
            cmd.CommandType = Data.CommandType.Text
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Using dr
                While dr.Read

                    Dim ad As AdminSetting = MaterializeAdminSetting(dr)
                    list.Add(ad)
                End While

                dr.Close()
            End Using
        End Using
        Return list
    End Function

    Shared Function GetAdminSettingForRole(ByVal role As String) As AdminSetting
        Dim adminSettingObj As AdminSetting = Nothing
        Dim sql As String = "select * from Admin where role=@role"
        Using cn As New SqlConnection(ConfigurationManager.ConnectionStrings("easyRtgsConn").ToString)
            cn.Open()

            Dim cmd As New SqlCommand()
            cmd.Connection = cn
            cmd.CommandText = sql
            cmd.CommandType = Data.CommandType.Text
            cmd.Parameters.AddWithValue("@role", role)
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Using dr
                If dr.HasRows Then
                    While dr.Read()

                        Dim ad As AdminSetting = MaterializeAdminSetting(dr)
                        Return ad 'There should be just one anyway
                    End While
                Else
                    Return Nothing
                End If
                

                dr.Close()
            End Using
        End Using
        Return adminSettingObj
    End Function

    Private Shared Function MaterializeAdminSetting(ByVal dr As SqlDataReader) As AdminSetting
        Dim ad As New AdminSetting
        ad.ID = dr("id").ToString()
        ad.RTGSAccount = IIf(Not IsDBNull(dr("RTGSAccount")), dr("RTGSAccount").ToString(), "")
        ad.PLAccount = IIf(Not IsDBNull(dr("PLAccount")), dr("PLAccount").ToString(), "")
        ad.ChargesCustomerAmount = IIf(Not IsDBNull(dr("ChargesCustomer")), dr("ChargesCustomer").ToString(), "")
        ad.ChargesStaffAmount = IIf(Not IsDBNull(dr("ChargesStaff")), dr("ChargesStaff").ToString(), "")
        ad.RTGSEmail = IIf(Not IsDBNull(dr("RTGSEmail")), dr("RTGSEmail").ToString(), "")
        ad.TreasuryEmail = IIf(Not IsDBNull(dr("TreasuryEmail")), dr("TreasuryEmail").ToString(), "")
        ad.RTGSSuspense = IIf(Not IsDBNull(dr("RTGSSuspense")), dr("RTGSSuspense").ToString(), "")
        ad.VATAccount = IIf(Not IsDBNull(dr("VATAccount")), dr("VATAccount").ToString(), "")
        ad.VATAmount = IIf(Not IsDBNull(dr("VATAmount")), dr("VATAmount").ToString(), "0")
        ad.CutOffTimeIn24HourFormat = IIf(Not IsDBNull(dr("CutOffTimeIn24HourFormat")), Convert.ToInt32(dr("CutOffTimeIn24HourFormat").ToString()), 0)

        If Not IsDBNull(dr("isActive")) Then
            If Convert.ToBoolean(dr("isActive").ToString()) = True Then
                ad.IsActive = True
            Else
                ad.IsActive = False
            End If

        Else
            ad.IsActive = False
        End If
        Return ad
    End Function

End Class
