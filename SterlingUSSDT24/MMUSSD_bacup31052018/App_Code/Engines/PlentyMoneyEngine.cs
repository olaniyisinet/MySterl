using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PlentyMoney
/// </summary>
public class PlentyMoneyEngine : BaseEngine
{
    public string DoValidatePickType(UReq req)
    {
        //at this point i expect the options to be 1 or 2
        //any other response should be error code 99
        string resp = "";
        try
        {
            switch(req.Msg)
            {
                case "1":
                case "2":
                    resp = req.Msg;
                    break;
                default:
                    resp = "99";
                    break;
            }
        }
        catch 
        {
            resp = "99";
        }
        return resp;
    }
}

