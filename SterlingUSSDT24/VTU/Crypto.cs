using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace VTU.CRYPTO
{

    public static class CryptoSterling
    {
        public static string IBS_Encrypt(string val)
        {
            string sharedkeyval = "000000010000001000000101000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100001100000000110000010100000111000010110000110100011100";
            string sharedvectorval = "0000000100000010000000110000010100000111000010110000110100000100";
          
            sharedkeyval = BinaryToString(sharedkeyval);
            sharedvectorval = BinaryToString(sharedvectorval);
            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toEncrypt = Encoding.UTF8.GetBytes(val);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string IBS_Decrypt(string val)
        {
            string sharedkeyval = "000000010000001000000101000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100001100000000110000010100000111000010110000110100011100";
            string sharedvectorval = "0000000100000010000000110000010100000111000010110000110100000100";
           // string sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100011111";
            //string sharedvectorval = "0000000100000010000000110000010100000111000010110000110100011111";
            sharedkeyval = BinaryToString(sharedkeyval);
            sharedvectorval = BinaryToString(sharedvectorval);

            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string BinaryToString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                throw new ArgumentNullException("binary");

            if ((binary.Length % 8) != 0)
                throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string section = binary.Substring(i, 8);
                int ascii = 0;
                try
                {
                    ascii = Convert.ToInt32(section, 2);
                }
                catch
                {
                    throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
                }
                builder.Append((char)ascii);
            }
            return builder.ToString();
        }

        public static string GetCrypt(string value)
        {
            string hash = "";
            SHA512 alg = SHA512.Create();
            byte[] result = alg.ComputeHash(Encoding.UTF8.GetBytes(value));
            hash = Encoding.UTF8.GetString(result);
            return hash;
        }
    }
}
