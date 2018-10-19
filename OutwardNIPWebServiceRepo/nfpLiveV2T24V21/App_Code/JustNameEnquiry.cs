using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for JustNameEnquiry
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class JustNameEnquiry : System.Web.Services.WebService {

    [WebMethod]
    public string NameEnquiry(string SessionID, string DestinationBankCode, string ChannelCode, string AccountNumber)
    {
        string msg = "";
        string responsecodeVal = "";
        string AcctNameval = "";
        TR_SingleNameEnquiry sne = new TR_SingleNameEnquiry();
        sne.SessionID = SessionID;
        sne.DestinationInstitutionCode = DestinationBankCode;
        sne.ChannelCode = ChannelCode;
        sne.AccountNumber = AccountNumber;
        sne.createRequest();

        if (!sne.sendRequest()) //unsuccessful request
        {
            responsecodeVal = sne.ResponseCode;
            AcctNameval = "";
            msg = responsecodeVal + ":" + AcctNameval;
            new ErrorLog(msg);
        }
        else
        {
            responsecodeVal = sne.ResponseCode;
            AcctNameval = sne.AccountName;
            msg = responsecodeVal + ":" + AcctNameval;
            new ErrorLog(msg);
        }
        return msg;
    }
    
}

