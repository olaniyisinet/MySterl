using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GetAccountSByMobileResp
/// </summary>


public class GetAccountSByMobileResp
{
    public Arrayofaccount[] ArrayOfAccounts { get; set; }
    public bool result { get; set; }
    public string errorMessage { get; set; }
}

public class Arrayofaccount
{
    public string AccountNo { get; set; }
    public string AccountTitle { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal BookBalance { get; set; }
    public string CustomerId { get; set; }
    public string Nuban { get; set; }
    public string PhoneNumber { get; set; }
    public string BvnId { get; set; }

}
