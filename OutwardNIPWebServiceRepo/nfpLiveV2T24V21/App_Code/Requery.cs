using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Xml;

/// <summary>
/// Summary description for Requery
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Requery : System.Web.Services.WebService {

    public Requery () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string requery(string sessionid, string channelcode, string destBankCode)
    {
        string ResponseCode = "err";
        //destBankCode = sessionid;
        //destBankCode = destBankCode.Substring(0, 6);
        try
        {
        //    string req = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
        //"<TSQuerySingleRequest>" +
        //"<DestinationBankCode>" + destBankCode + "</DestinationBankCode>" +
        //"<ChannelCode>" + channelcode + "</ChannelCode>" +
        //"<SessionID>" + sessionid + "</SessionID>" +
        //"</TSQuerySingleRequest>";
            string req = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
       "<TSQuerySingleRequest>" +
       "<SourceInstitutionCode>" + "000001" + "</SourceInstitutionCode>" +
       "<ChannelCode>" + channelcode + "</ChannelCode>" +
       "<SessionID>" + sessionid + "</SessionID>" +
       "</TSQuerySingleRequest>";

            //new ErrorLog("Log Inward Requery " + req);
            //callwebservice
            SSM ssm = new SSM();
            req = ssm.enkrypt(req);
            ///Nibbslive.NIPInterface nip = new Nibbslive.NIPInterface();
            NibssInward.NIPTSQInterface nip = new NibssInward.NIPTSQInterface();
            string resp = nip.txnstatusquerysingleitem(req);
            //new ErrorLog("The Raw TSQuery Response from NIBSS " + resp);
            Mylogger1.Info("The Raw TSQuery Response from NIBSS " + resp);
            SSM ssm1 = new SSM();
            resp = ssm1.dekrypt(resp);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);
            
            //string DestinationBankCode = xmlDoc.GetElementsByTagName("DestinationBankCode").Item(0).InnerText;
            string SourceInstitutionCode = xmlDoc.GetElementsByTagName("SourceInstitutionCode").Item(0).InnerText;
            string ChannelCode = xmlDoc.GetElementsByTagName("ChannelCode").Item(0).InnerText;
            string SessionID = xmlDoc.GetElementsByTagName("SessionID").Item(0).InnerText;
            ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
            ResponseCode = ResponseCode.Trim();

            //new ErrorLog("Response on Inward Re-query for session ID " + SessionID + " is ==> " + ResponseCode);
            Mylogger1.Info("Response on Inward Re-query for session ID " + SessionID + " is ==> " + ResponseCode);
        }
        catch (Exception ex)
        {
            //ResponseCode = ex.Message;
            //new ErrorLog("Error occured during TQuery " + ex);
            Mylogger1.Error("Error occured during TQuery " , ex);
            ResponseCode = "??";
        }

        return ResponseCode;
    }
    
}
