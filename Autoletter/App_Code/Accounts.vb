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



    Public Function connstr() As String
        'Return "provider=sqloledb.1;user id=axecreditportal;password=axe*1234;initial catalog=STERLREPOSITORY;data source=10.0.41.115"
        Return "provider=sqloledb.1;user id=biztalkadmin;password=Sterling123;initial catalog=autoletterT24;data source=10.0.20.152"
    End Function

    Public Function sqlconn() As String
        'Return "user id=axecreditportal;password=axe*1234;initial catalog=STERLREPOSITORY;data source=10.0.41.115"
        'Return "user id=biztalkadmin;password=Sterling123;initial catalog=autoletterT24;data source=10.0.20.152"
        Return "user id=appusr;password=(#usr4*);initial catalog=autoletterT24;data source=10.0.0.156,1490"
    End Function

End Class
