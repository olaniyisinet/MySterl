using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ValidateWiseBankCardReq
/// </summary>
public class ValidateWiseBankCardReq
{
    public string institutionCode { get; set; }
    public string accountNumber { get; set; }
    public string cardPan { get; set; }
}