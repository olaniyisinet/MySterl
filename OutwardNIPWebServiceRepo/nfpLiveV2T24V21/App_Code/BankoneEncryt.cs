using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for BankoneEncryt
/// </summary>
public class BankoneEncryt
{
    public string EncryptTripleDES(string plainText)
    {
        byte[] byt = System.Text.Encoding.UTF8.GetBytes(plainText);
        string mdo = Convert.ToBase64String(byt);
        byte[] result;
        byte[] dataToEncrypt = System.Text.Encoding.UTF8.GetBytes(plainText);

        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        byte[] keyB = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes("CIC9XRNBWPDAYQFEVKEWAZMVHXHBZCIU"));
        hashmd5.Clear();

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

        tdes.IV = new byte[8];
        tdes.Key = keyB;
        tdes.Mode = CipherMode.CBC;
        tdes.Padding = PaddingMode.PKCS7;
        //TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider { Key = keyB, Mode = CipherMode.CBC, IV = new byte[8], Padding = PaddingMode.PKCS7 };
        using (ICryptoTransform cTransform = tdes.CreateEncryptor())
        {
            result = cTransform.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            tdes.Clear();
        }

        return Convert.ToBase64String(result, 0, result.Length);
    }
}