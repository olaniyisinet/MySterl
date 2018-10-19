using System;
using System.IO;

public partial class v1_0 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Machine mach = new Machine();
        UService usv = new UService();
        UResp resp = new UResp();
        Stream reqdata;
        reqdata = Request.InputStream;
        int reqlen = Convert.ToInt32(reqdata.Length);
        byte[] data = new byte[reqlen];
        try
        {
            reqdata.Read(data, 0, reqlen);
        }
        catch (IOException ex)
        {
            new ErrorLog("IO:" + ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            new ErrorLog("NULL:" + ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            new ErrorLog("RANGE:" + ex.Message);
        }
        catch (Exception ex)
        {
            new ErrorLog("EX:" + ex.Message);
        }

        string xmltext = System.Text.Encoding.ASCII.GetString(data);
        xmltext = xmltext.Replace("*355*5", "*822");
        xmltext = xmltext.Replace("*3550*5", "*822");//test etisalat
        xmltext = xmltext.Replace("*859*5", "*822");// test airtel
        //new ErrorLog("This is from test USSD " + xmltext);
        //string xmltext = "<?xml version=\"1.0\" ?><ussd><msisdn>2348171109992</msisdn><sessionid>161229115502412</sessionid><type>1</type><msg>*822*5#</msg><network>1</network></ussd>";
        //string xmltext = "<?xml version=\"1.0\" ?><ussd><msisdn>2347066707201</msisdn><sessionid>11448</sessionid><type>1</type><msg>*822*100#</msg><network>4</network></ussd>";
        UReq req = usv.readRequest(xmltext);
        if (!req.ok)
        {
            resp.Msg = "Request Error";
            resp.Mtype = 6;
        }  

        switch (req.Mtype)
        {
            case 1: //new request
                UMenu.GetMenu(ref req);
                usv.saveState(req);
                resp.Msg = mach.doNextOperation(ref req);
                ///////////////////this was added to enable it suit the test
                if (req.Msg == "*822#")
                {
                    resp.Mtype = 2;
                }
                else if (req.Msg.Contains("822*9") || req.Msg.Contains("822*14"))
                {
                    resp.Mtype = 2;
                }
                else if (req.Msg.Contains("*822*6"))
                {
                    resp.Mtype = 3;
                }
                else if (req.Msg == "99")
                {
                    resp.Mtype = 3;
                }
                else
                {
                    resp.Mtype = 3;
                }
                ///////////////////////////////////////////
                break;
            case 2: 
                resp.Msg = mach.doNextOperation(ref req); 
                resp.Mtype = mach.Mtype;
                if (resp.Mtype == 3) goto case 3;
                break;
            case 3: //release 
                resp.Mtype = 3;
                mach.endSession(req);
                break;
            case 4: //time out
                mach.endSession(req);
                break;
            case 6: //release
                resp.Msg = "Thank you for banking with Sterling!";
                resp.Mtype = 6;
                mach.endSession(req);
                break;
            case 10: // charge
                break;
        }
        usv.saveTrnx(req, resp);
        usv.saveState(req);
        Response.Write(usv.writeResponse(resp));
    }

}