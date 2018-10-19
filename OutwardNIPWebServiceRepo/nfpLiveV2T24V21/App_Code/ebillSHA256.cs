using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for ebillSHA256
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class ebillSHA256 : System.Web.Services.WebService {

    [WebMethod]
    public string getSHA256(string txt) {
        SSM1 ssm = new SSM1();
        string HashHolder = "";string HashValue="";
        HashHolder = txt;
        HashValue = ssm.sha256(HashHolder);

        return HashValue;
    }


}

