using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for ComputerIBSFeeVat
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class ComputerIBSFeeVat : System.Web.Services.WebService {
    AccountService acs = new AccountService();
    Transaction t = new Transaction();
    [WebMethod]
    public string CalculateIBSFeeVat (decimal amt) {

        try
        {
            if (acs.getNIPFee(amt))
            {
                return acs.NIPfee.ToString() + ":" + acs.NIPvat.ToString();
            }
            else
            {
                
            }
        }
        catch (Exception ex)
        {
            new ErrorLog("Unable to compute VAT and FEE " + ex);
        }
        return "0x" + ":" + "0x";
    }

   
    
}

