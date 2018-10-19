Imports Microsoft.VisualBasic
Imports System



Public Class CyclopsDES
    Public Function TripleDESEncode(ByVal value As String, ByVal key As String) As String
        Dim des As New Security.Cryptography.TripleDESCryptoServiceProvider
        des.IV = New Byte(7) {}
        Dim pdb As New Security.Cryptography.PasswordDeriveBytes(key, New Byte(-1) {})
        des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, New Byte(7) {})
        Dim ms As New IO.MemoryStream((value.Length * 2) - 1)
        Dim encStream As New Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(), Security.Cryptography.CryptoStreamMode.Write)
        Dim plainBytes As Byte() = Text.Encoding.UTF8.GetBytes(value)
        encStream.Write(plainBytes, 0, plainBytes.Length)
        encStream.FlushFinalBlock()
        Dim encryptedBytes(CInt(ms.Length - 1)) As Byte
        ms.Position = 0
        ms.Read(encryptedBytes, 0, CInt(ms.Length))
        encStream.Close()
        Return Convert.ToBase64String(encryptedBytes)
    End Function

    Public Function TripleDESDecode(ByVal value As String, ByVal key As String) As String
        Dim des As New Security.Cryptography.TripleDESCryptoServiceProvider
        des.IV = New Byte(7) {}
        Dim pdb As New Security.Cryptography.PasswordDeriveBytes(key, New Byte(-1) {})
        des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, New Byte(7) {})
        Dim encryptedBytes As Byte() = Convert.FromBase64String(value)
        Dim ms As New IO.MemoryStream(value.Length)
        Dim decStream As New Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(), Security.Cryptography.CryptoStreamMode.Write)
        decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
        decStream.FlushFinalBlock()
        Dim plainBytes(CInt(ms.Length - 1)) As Byte
        ms.Position = 0
        ms.Read(plainBytes, 0, CInt(ms.Length))
        decStream.Close()
        Return Text.Encoding.UTF8.GetString(plainBytes)
    End Function
End Class
