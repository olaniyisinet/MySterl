using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for SwitchCBA
/// </summary>
public class SwitchCBA
{
    public SwitchCBA()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int IsNubanT24(string nuban)
    {
        int cbaType = 99;
     
        return cbaType;
    }

    public int GetCBATypeByStartingDigits(string nuban)
    {
        new ErrorLog("got to cba type");
        int cbatype = 88;
        if (nuban.StartsWith("05"))
        {
            cbatype = 2;//imal
        }
        else if (nuban.StartsWith("99"))
        {
            cbatype = 3;//bankone
        }
        else
        {
            cbatype = 1;//t24
        }
        return cbatype;
    }


}