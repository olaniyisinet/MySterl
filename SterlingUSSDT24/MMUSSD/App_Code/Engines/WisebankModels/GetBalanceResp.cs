using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GetBalanceResp
/// </summary>
public class GetBalanceResp
{
    public bool result { get; set; }
    public string errorMessage { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal ValueBalance { get; set; }

}