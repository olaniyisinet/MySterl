using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Tj.Cryptography;

/// <summary>
/// Summary description for Gadget
/// </summary>
public class Gadget
{
    public Gadget()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string newOTP()
    {
        Thread.Sleep(50);
        return GenerateRndNumber(6);
    }
    public string GenerateRndNumber(int cnt)
    {
        string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        Random rand1 = new Random();
        string txt = "";
        for (int j = 0; j < cnt; j++)
            txt += key2[rand1.Next(0, 9)];
        return txt;
    }
    //to encrpt
    public string enkrypt(string strEnk)
    {
        SymCryptography cryptic = new SymCryptography();
        cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";

        return cryptic.Encrypt(strEnk);

    }
}