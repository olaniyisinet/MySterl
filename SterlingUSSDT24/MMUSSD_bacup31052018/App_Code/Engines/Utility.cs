using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Utility
/// </summary>
public class Utility
{
    public string FormatMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
}