using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for SetPrimaryAcct
/// </summary>
public class SetPrimaryAcct : BaseEngine
{
    public string SaveRequest(UReq req)
    {
        //*822*0# or from Menu
        string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            //Continue process
            DataRow dr = ds.Tables[0].Rows[0];
            nuban = dr["NUBAN"].ToString();
            addParam("NUBAN", nuban, req);
        }
        else
        {
            //redirect to register and set PIN
            req.next_op = 122;
            return "0";
        }
        return "0";
    }

    public string CheckNoofAccts(UReq req)
    {
        //this is to check how many accounts exist for the cutomer
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            addParam("AcctCnt", "1", req);
            req.next_op = 527;
            return "0";
        }
        else
        {
            req.next_op = 522;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb(UReq req)
    {

        string primeAcct = GetPrimaryAccount(req.Msisdn);
        if (string.IsNullOrEmpty(primeAcct))
        {
            //Get most used account for transfers
            string favAcct = GetMostUsedAccount(req.Msisdn);
            if (!string.IsNullOrEmpty(favAcct))
            {
                //Order by most used account
                Sterling.MSSQL.Connect cc = new Sterling.MSSQL.Connect("mssqlconn");
                cc.SetProcedure("spd_getAcctListWithPrime");
                cc.AddParam("@mob", req.Msisdn);
                cc.AddParam("@sessionid", req.SessionID);
                cc.AddParam("@account", favAcct.Trim());
                int ccn = cc.ExecuteProc();
                if (ccn > 0)
                {
                    req.next_op = 523;
                    return "0";
                }
                else
                {
                    return "0";
                }
            }

            //Use default list
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getAcctList");
            c.AddParam("@mob", req.Msisdn);
            c.AddParam("@sessionid", req.SessionID);
            int cn = c.ExecuteProc();
            if (cn > 0)
            {
                req.next_op = 523;
                return "0";
            }
            else
            {
                return "0";
            }
        }
        else
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetProcedure("spd_getAcctListWithPrime");
            c.AddParam("@mob", req.Msisdn);
            c.AddParam("@sessionid", req.SessionID);
            c.AddParam("@account", primeAcct.Trim());
            int cn = c.ExecuteProc();
            if (cn > 0)
            {
                req.next_op = 523;
                return "0";
            }
            else
            {
                return "0";
            }
        }
        
       
        return "0";
    }
    public string ListCustomerAccts(UReq req)
    {
        Gadget g = new Gadget(); int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            List<AccountList> lb = g.GetListofAcctsByPage(page.ToString(), req.SessionID);
            if (lb.Count <= 0)
            {
                removeParam("NXTPAG", req);
                lb = g.GetListofAcctsByPage("0", req.SessionID);
            }
            
            foreach (AccountList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.Nuban;                    
                }
                else
                {
                    int acctListNo = Convert.ToInt16(item.TransRate);
                    resp += "%0A" + acctListNo + " " + item.Nuban;
                }
            }

            if (cntfirst < 5)
            {
                resp += "";
            }
            else
            {
                resp += "%0A0 Next";
            }
        }
        catch
        {
        }
        return "Select Default Account%0A" + resp;
    }
    public int SaveAcctSelected(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "0")
        {
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }

        addParam("ITEMSELECT", req.Msg, req);
        addParam("A", "2", req);
        return 1;
    }

    public string DisplaySummary(UReq req)
    {
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        Gadget g = new Gadget();
        string nuban = ""; int flag = 0; int ITEMSELECT = 0;
        flag = int.Parse(prm["A"]);
        if (flag == 1)
        {
            nuban = prm["NUBAN"];
        }
        else if (flag == 2)
        {
            ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
            nuban = g.GetNubanByListID(ITEMSELECT, req);
        }

        resp = "Are you sure you want to set " + nuban + " as your default account?%0A1.Yes%0A2.No";


        return resp;
    }

    public string SaveSummary(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 527;
            }
            else
            {
                var regexItem = new Regex("^[0-9]*$");
                if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
                {
                    //continue: no issues
                }
                else
                {
                    //redirect to summary page
                    req.next_op = 525;
                    resp = "0";
                }
            }
        }
        catch
        {

        }

        return resp;
    }

    public string DoSubmit(UReq req)
    {
        Gadget g = new Gadget();
        string resp = ""; int summary = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int k = 0;
        try
        {
            int acctCount = Convert.ToInt32(prm["AcctCnt"]);
            if (acctCount == 1)
            {
                resp = "There is no need to set a default account , you only have one account profiled.";
                return resp;
            }

            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Operation cancelled. Please dial *822*0# or select from *822# menu to try again.";
                return resp;
            }
            else
            {
                string nuban = ""; int ITEMSELECT = 0;
                //Get selected acount from list
                ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
                nuban = g.GetNubanByListID(ITEMSELECT, req);

                //Set default account
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
                c.SetProcedure("spd_SetDefaultAcct");
                c.AddParam("@Mobile", req.Msisdn);
                c.AddParam("@NUBAN", nuban);
                c.AddParam("@SessionID", req.SessionID);
                c.ExecuteProc();
                int? cnt = c.returnValue;
                if (cnt > 0)
                {
                    //call EACBS that Kayode will conclude so you can proceeed and get response to respond to the users
                    resp = nuban + " has been saved as your Primary account";
                }
                else
                {
                    resp = "We were unable to set our default account. Please try again later.%0AThank you.";
                }
            }
        }
        catch (Exception ex)
        {
            resp = "Error occurred during processing";
            return resp;
        }

        return resp;
    }
}