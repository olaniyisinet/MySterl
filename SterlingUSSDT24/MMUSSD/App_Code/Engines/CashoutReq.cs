using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CashoutReq
/// </summary>
public class CashoutReq
{
    public string BeneficiaryMobile { get; set; }
    public string BeneficiaryDetails { get; set; }
    public string OriginatorMobile { get; set; }
    public decimal Amount { get; set; }
    public string FromNuban { get; set; }
    public string Appid { get; set; }
    public int currency { get; set; }
}