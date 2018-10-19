using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Text;

public class Gadget
{
    public string GenerateRndNumber(int cnt)
    {
        string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        Random rand1 = new Random();
        string txt = "";
        for (int j = 0; j < cnt; j++)
            txt += key2[rand1.Next(0, 9)];
        return txt;
    }

    public string unhash(string text1)
    {
        string text2 = "";
        for (int i = 0; i < text1.Length; i++)
        {
            text2 += unhashtable(text1.Substring(i,1));
        }
        return text2;
    }

    public string hash(string text1)
    {
        string text2 = "";
        for (int i = 0; i < text1.Length; i++)
        {
            text2 += hashtable(text1.Substring(i,1));
        }
        return text2;
    }

    private string hashtable(string s)
    {
        s = s.ToUpper();
        string chrLetter = "";
        switch (s)
        {
            case "A": chrLetter = "M"; break;
            case "B": chrLetter = "C"; break;
            case "C": chrLetter = "O"; break;
            case "D": chrLetter = "P"; break;
            case "E": chrLetter = "R"; break;
            case "F": chrLetter = "N"; break;
            case "G": chrLetter = "Y"; break;
            case "H": chrLetter = "W"; break;
            case "I": chrLetter = "A"; break;
            case "J": chrLetter = "F"; break;
            case "K": chrLetter = "E"; break;
            case "L": chrLetter = "J"; break;
            case "M": chrLetter = "L"; break;
            case "N": chrLetter = "T"; break;
            case "O": chrLetter = "K"; break;
            case "P": chrLetter = "U"; break;
            case "Q": chrLetter = "5"; break;
            case "R": chrLetter = "7"; break;
            case "S": chrLetter = "2"; break;
            case "T": chrLetter = "6"; break;
            case "U": chrLetter = "0"; break;
            case "V": chrLetter = "3"; break;
            case "W": chrLetter = "9"; break;
            case "X": chrLetter = "8"; break;
            case "Y": chrLetter = "1"; break;
            case "Z": chrLetter = "4"; break;
            case "0": chrLetter = "D"; break;
            case "1": chrLetter = "G"; break;
            case "2": chrLetter = "I"; break;
            case "3": chrLetter = "X"; break;
            case "4": chrLetter = "Z"; break;
            case "5": chrLetter = "Q"; break;
            case "6": chrLetter = "H"; break;
            case "7": chrLetter = "S"; break;
            case "8": chrLetter = "V"; break;
            case "9": chrLetter = "B"; break;
            default: chrLetter = s; break;
        }
        return chrLetter;
    }

    private string unhashtable(string s)
    {
        s = s.ToUpper();
        string chrLetter = "";
        switch (s)
        {
            case "A": chrLetter = "I"; break;
            case "B": chrLetter = "9"; break;
            case "C": chrLetter = "B"; break;
            case "D": chrLetter = "0"; break;
            case "E": chrLetter = "K"; break;
            case "F": chrLetter = "J"; break;
            case "G": chrLetter = "1"; break;
            case "H": chrLetter = "6"; break;
            case "I": chrLetter = "2"; break;
            case "J": chrLetter = "L"; break;
            case "K": chrLetter = "O"; break;
            case "L": chrLetter = "M"; break;
            case "M": chrLetter = "A"; break;
            case "N": chrLetter = "F"; break;
            case "O": chrLetter = "C"; break;
            case "Q": chrLetter = "5"; break;
            case "V": chrLetter = "8"; break;
            case "P": chrLetter = "D"; break;
            case "R": chrLetter = "E"; break;
            case "S": chrLetter = "7"; break;
            case "T": chrLetter = "N"; break;
            case "U": chrLetter = "P"; break;
            case "W": chrLetter = "H"; break;
            case "X": chrLetter = "3"; break;
            case "Y": chrLetter = "G"; break;
            case "Z": chrLetter = "4"; break;
            case "0": chrLetter = "U"; break;
            case "1": chrLetter = "Y"; break;
            case "2": chrLetter = "S"; break;
            case "3": chrLetter = "V"; break;
            case "4": chrLetter = "Z"; break;
            case "5": chrLetter = "Q"; break;
            case "6": chrLetter = "T"; break;
            case "7": chrLetter = "R"; break;
            case "8": chrLetter = "X"; break;
            case "9": chrLetter = "W"; break;
            default: chrLetter = s; break;
        }
        return chrLetter;
    }
}
