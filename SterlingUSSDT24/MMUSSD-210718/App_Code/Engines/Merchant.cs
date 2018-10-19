using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Merchant
/// </summary>
public class Merchant : BaseEngine
{
    Gadget g = new Gadget();
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    public string ResponseDescr(string code)
    {
        string txt = "";
        switch (code)
        {
            case "00": txt = "Approved or Completed"; break;
            case "01": txt = "Status unknown, please wait for settlement report "; break;
            case "03": txt = "Invalid sender"; break;
            case "05": txt = "Do not honor"; break;
            case "06": txt = "Dormant account"; break;
            case "07": txt = "Invalid account"; break;
            case "08": txt = "Account name mismatch"; break;
            case "09": txt = "Request processing in progress"; break;
            case "12": txt = "Invalid transaction"; break;
            case "13": txt = "Invalid amount"; break;
            case "14": txt = "Invalid Batch Number"; break;
            case "15": txt = "Invalid Session or Record ID"; break;
            case "16": txt = "Unknown Bank Code"; break;
            case "17": txt = "Invalid Channel"; break;
            case "18": txt = "Wrong Method Call"; break;
            case "21": txt = "No action taken"; break;
            case "25": txt = "Unable to locate record"; break;
            case "26": txt = "Duplicate record"; break;
            case "30": txt = "Wrong destination account format"; break;
            case "34": txt = "Suspected fraud"; break;
            case "35": txt = "Contact sending bank"; break;
            case "51": txt = "No sufficient funds"; break;
            case "57": txt = "Transaction not permitted to sender"; break;
            case "58": txt = "Transaction not permitted on channel"; break;
            case "61": txt = "Transfer Limit Exceeded"; break;
            case "63": txt = "Security violation"; break;
            case "65": txt = "Exceeds withdrawal frequency"; break;
            case "68": txt = "Response received too late"; break;
            case "91": txt = "Beneficiary Bank not available"; break;
            case "92": txt = "Routing Error"; break;
            case "94": txt = "Duplicate Transaction"; break;
            case "96": txt = "Corresponding Bank is currently offline."; break;
            case "97": txt = "Timeout waiting for response from destination."; break;
        }
        return txt;
    }
    /////////////////////// NIBSS
    public string SaveMerchReq(UReq req)
    {
        Gadget g = new Gadget();
        string RequestorID = System.Web.Configuration.WebConfigurationManager.AppSettings["RequestorID"].ToString();
        //*822*22*MerchantCode*Amt#
        //*shortcode*amount*merchantcode#.
        //get account name by mobile
        string AcctName = "";
        string resp = "0";
        try
        {
            char[] delm = { '*', '#' };
            string msg = req.Msg.Trim();
            string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            addParam("MCODE", s[2], req);
            addParam("AMOUNT", s[3], req);

            string bankCode = req.Msisdn.Substring(8, 3);
            string NewSessionid = g.newSessionGlobal(bankCode, 1);

            //call nibss service to validate merchate
            Merchanttxn trx = new Merchanttxn();
            trx.SessionID = NewSessionid;// "000001100913103301000000000001";
            trx.RequestorID = RequestorID;
            trx.MerchantCode = s[2];//s[2] contains the merchant code
            trx.Amount = s[3];
            trx.PayerPhoneNumber = req.Msisdn;
            
            //create the prepatyment request
            trx.CreatePrePaymentRequest();
            //send to NIBSS
            trx.sendPrePaymentEnquiRequest();
            addParam("FinInstitutions", trx.FinancialInstitutions, req);
            addParam("RESP", trx.ResponseCode, req);
            if(trx.ResponseCode =="00")
            {
                //check if merchant name is the same as the name of the customer for trxn
                resp = trx.MerchantName;
                addParam("MERCHNAME", trx.MerchantName, req);
            }
            else
            {
                resp = ResponseDescr(trx.ResponseCode);
            }
        }
        catch
        {
            resp = "Error has occured";
        }
        return resp;
    }
    public string DisplaySummary(UReq req)
    {
        string resp = ""; string NibsRsp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        NibsRsp = prm["RESP"];
        if (NibsRsp == "00")
        {
            resp = "You are paying " + prm["MERCHNAME"] + " the sum of " + getRealMoney(decimal.Parse(prm["AMOUNT"])) + "%0A1.Yes%0A2.No";
            return "Merchant Payment%0A" + resp;
        }
        else
        {
            if (NibsRsp == "" || NibsRsp == null)
            {
                resp = "No Response returned from NIBSS";
            }
            else
            {
                resp = ResponseDescr(NibsRsp);
            }
        }
        return resp;
    }
    public string SaveSummary(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("SUMMARY", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    public string GetAcctByMobile(UReq req)
    {
        string nuban = "";
        Gadget g = new Gadget();
        nuban = g.GetAccountsByMobileNo2(req.Msisdn);
        //do get account full info
        EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        DataSet ds = ws.getAccountFullInfo(nuban);
        if(ds.Tables[0].Rows.Count>0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            addParam("BVN", dr["BVN"].ToString(), req);
        }
        try
        {
            addParam("FROMNUBAN", nuban, req);
            req.next_op = 715;
            return "0";
        }
        catch
        {
            return "0";
        }
        return "0";
    }

    public void insert(int id, string nuban, string sessionid)
    {
        string sql = "insert into tbl_USSD_account_id(acctid,nuban,sessionid) " +
            "values (@id,@nu,@sid)";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@id", id);
        cn.AddParam("@nu", nuban);
        cn.AddParam("@sid", sessionid);
        cn.Insert();
    }
    public string SaveFromAccount(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("FROMNUBAN", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string CollectUSSDPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter your USSD PIN";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 4 digits";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 digits and not alphanumeric";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The PIN you supplied does not correspond with the PIN you registered.  Kindly enter the correct USSD PIN";
            return "Authentication:%0A" + resp;
        }
        resp = "Enter your USSD PIN";
        return "Authentication:%0A" + resp;
    }
    public string SavetxnPIN(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 716;
            return "9";
        }
        //check if entry is numeric
        try
        {
            long PIN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 716;
            return "9";
        }
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);
        //check the registration table to confirm if PIN entered is correct
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetCusRegPIN");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@auth", req.Msg);
        DataSet ds = c.Select("rec");
        string resp = "0";
        if (ds.Tables[0].Rows.Count > 0)
        {

            try
            {
                addParam("PIN", req.Msg, req);

            }
            catch
            {
                resp = "0";
            }
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 716;
            return "9";
        }

        return resp;
    }
    public string SubmitRec(UReq req)
    {
        string resp = ""; int summary = 0; string nuban = ""; int PassCode = 0;
        Merchanttxn trx = new Merchanttxn();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string bankCode = req.Msisdn.Substring(8, 3);
        string NewSessionid = g.newSessionGlobal(bankCode, 1);
        string RequestorID = System.Web.Configuration.WebConfigurationManager.AppSettings["RequestorID"].ToString();

        trx.SessionID = NewSessionid;
        trx.RequestorID = RequestorID;
        trx.PayerPhoneNumber = req.Msisdn;
        
        trx.PayerBVN = prm["BVN"];
        trx.Amount = prm["AMOUNT"];
        trx.MerchantCode = prm["MCODE"];
        trx.MandateCode = DateTime.Now.ToString("yyyyMMddhhmmss");
        trx.pin = prm["PIN"];
        trx.debitacct = prm["FROMNUBAN"];

        //insert mandate into the mandate table
        InsertMandate(trx.SessionID, "000001", trx.MandateCode, decimal.Parse(trx.Amount), "", trx.debitacct, trx.PayerBVN, trx.MerchantCode);
        if (req.Network =="1")
        {
            trx.Telco = "ETISALAT";
        }
        else if(req.Network =="2")
        {
            trx.Telco = "GLO";
        }
        else if (req.Network == "3")
        {
            trx.Telco = "AIRTEL";
        }
        else if (req.Network == "4")
        {
            trx.Telco = "MTN";
        }

        trx.CreatePaymentRequest();
        //send
        trx.sendPaymentRequest();
        if (trx.ResponseCode == "00")
        {
            resp = "Merchant Payment%0ATransaction was successful";
            string sql = "Insert into tbl_USSD_Merchant (sessionid,mobile,MerchantCode,amt,Nuban,MerchantName,ReferenceCode,NIBSSResponse) " +
                                           " values(@sid,@mb,@mc,@am,@nu,@mn,@refcode,@resp)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@mc", prm["MCODE"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@nu", prm["FROMNUBAN"]);
            cn.AddParam("@mn", prm["MERCHNAME"]);
            cn.AddParam("@refcode", trx.ReferenceCode);
            cn.AddParam("@resp", trx.ResponseCode);
            Convert.ToInt32(cn.Insert());
        }
        else
        {
            resp = "Transaction was not successful";
            string sql = "Insert into tbl_USSD_Merchant (sessionid,mobile,MerchantCode,amt,Nuban,MerchantName,ReferenceCode,NIBSSResponse) " +
                                           " values(@sid,@mb,@mc,@am,@nu,@mn,@refcode,@resp)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@mc", prm["MCODE"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@nu", prm["FROMNUBAN"]);
            cn.AddParam("@mn", prm["MERCHNAME"]);
            cn.AddParam("@refcode", trx.ReferenceCode);
            cn.AddParam("@resp", trx.ResponseCode);
            Convert.ToInt32(cn.Insert());
        }

        return resp;
    }
    //get customer account by merchant 
    public void InsertMandate(string sid,string dcode, string mandateRef, decimal amt, string accname, string nuban, string bvn, string mcode)
    {
        string sql = "";
        sql = "insert into tbl_nip_mandate(SessionID,DestinationInstitutionCode,ChannelCode,MandateReferenceNumber,Amount,DebitAccountName,DebitAccountNumber, " +
            " DebitBankVerificationNumber,DebitKYCLevel,BeneficiaryAccountName,BeneficiaryAccountNumber,BeneficiaryBankVerificationNumber,BeneficiaryKYCLevel) " +
            " values(@sid,@dcode,8,@mn,@amt,@debi_acc_name,@debit_acc_num,@dbvn,@bkyc,@ban,@banNum,@bbvn,@bkycl)";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_nip");
        cn.SetSQL(sql);
        cn.AddParam("@sid", sid);
        cn.AddParam("@dcode", dcode);
        cn.AddParam("@mn", mandateRef);
        cn.AddParam("@amt", amt);
        cn.AddParam("@debi_acc_name", accname);
        cn.AddParam("@debit_acc_num", nuban);
        cn.AddParam("@dbvn", bvn);
        cn.AddParam("@bkyc", "");
        cn.AddParam("@ban", "");
        cn.AddParam("@banNum", mcode);
        cn.AddParam("@bbvn", "");
        cn.AddParam("@bkycl", "");
        cn.Execute();
    }
    public string isUserRegActive(UReq req)
    {
        //check if you are registered and have pin set
        try
        {
            addParam("Tid", req.Msg, req);
            string sql = ""; int activated = -1; string nuban = "";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                activated = int.Parse(dr["Activated"].ToString());

                if (activated == 0)
                {
                    //redirect to set PIN
                    req.next_op = 115;
                    return "0";
                }
            }
            else
            {
                //redirect to register and set PIN
                req.next_op = 122;
                return "0";
            }
        }
        catch
        {

        }
        return "0";
    }
}