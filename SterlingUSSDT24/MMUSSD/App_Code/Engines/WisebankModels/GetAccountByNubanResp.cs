using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GetAccountByNubanResp
/// </summary>
public class GetAccountByNubanResp
{

    public bool result { get; set; }
    public string errorMessage { get; set; }
    public string AccountNo { get; set; }
    public string AccountTitle { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal BookBalance { get; set; }
    public string CustomerId { get; set; }
    public string NubanId { get; set; }
    public string BvnId { get; set; }
    public string PhoneNumber { get; set; }

}