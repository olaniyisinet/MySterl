using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for DepositResponse
/// </summary>
public class DepositResponse
{
    public DepositResponse() { }
    public string Status;
    public string StatusMessage;
    public string ResponseCode;
    
    public DepositResponse(string sts, string stsmsg, string responsecode)
    {
        Status = sts;
        StatusMessage = stsmsg;
        ResponseCode = responsecode;
    }
}