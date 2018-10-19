using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public class BaseEngine
{
    public decimal Totaldone;
    public int RunCount(string s)
    {
        int k = 0;
        try
        {
            k = Convert.ToInt32(s);
        }
        catch
        {
        }
        return k;
    }
    public static string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    public void addParam(string key, string val, UReq req)
    {
        string sql = "select params from tbl_USSD_reqstate " +
            " where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        string prm = cn.SelectScalar();
        newParam(prm + key, val, req);
    }
    public void newParam(string key, string val, UReq req)
    {
        string sql = "update tbl_USSD_reqstate set  params = @prm, lastupdated= getdate() " +
            " where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@prm", key + ":" + val + ";");
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        cn.Update();
    }
    public void saveParam(string prms, UReq req)
    {
        string sql = "update tbl_USSD_reqstate set  params = @prm, lastupdated= getdate() " +
            " where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@prm", prms);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        cn.Update();
    }
    public void removeParam(string key, UReq req)
    {
        string sql = "select params from tbl_USSD_reqstate " +
            " where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        string prms = cn.SelectScalar();
        NameValueCollection prm = splitParam(prms);
        prms = prms.Replace(key + ":" + prm[key] + ";", "");
        saveParam(prms, req);
    }
    public string getParams(UReq req)
    {
        string sql = "select params from tbl_USSD_reqstate " +
            " where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        return cn.SelectScalar();
    }
    public void setOpType(int typeID, UReq req)
    {
        string sql = "update tbl_USSD_reqstate set  op_type = @typ, lastupdated= getdate() " +
            " where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@typ", typeID);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        cn.Update();
    }
    public string getOpType(UReq req)
    {
        string sql = "select op_type from tbl_USSD_reqstate " +
            " where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        return cn.SelectScalar();
    }
    public void setStatusflag(int typeID, UReq req)
    {
        string sql = "update tbl_USSD_reqstate set  statusflag = @typ, lastupdated= getdate() " +
            " where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@typ", typeID);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        cn.Update();

        //at this point when status is 2
        //i need to encrypt data sent in by customer
        //
        if (typeID == 2)
        {
            string prms = getParams(req);
            if (prms != "")
            {
                prms = Gadget.enkrypt(prms);
                saveParam(prms, req);
            }
        }
    }
    public NameValueCollection splitParam(string prm)
    {
        char[] sep1 = { ';' };
        char[] sep2 = { ':' };
        NameValueCollection nvc = new NameValueCollection();
        string[] tmp = prm.Split(sep1, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < tmp.Length; i++)
        {
            string[] tmp2 = tmp[i].Split(sep2, StringSplitOptions.None);
            nvc.Add(tmp2[0], tmp2[1]);
        }
        return nvc;
    }

    public int SaveActivation(object obj)
    {
        UReq req = (UReq)obj;
        addParam("ACTVCODE", req.Msg, req);
        setOpType(1, req);
        return 0;
    }
    public int SaveDOB(object obj)
    {
        UReq req = (UReq)obj;
        addParam("DOB", req.Msg, req);
        return 0;
    }
    public int SavePIN(object obj)
    {
        UReq req = (UReq)obj;
        addParam("PIN", req.Msg, req);
        return 0;
    }
    public int SavePINnew(object obj)
    {
        UReq req = (UReq)obj;
        addParam("NEWPIN", req.Msg, req);
        return 0;
    }
    public int SavePINconfirm(object obj)
    {
        UReq req = (UReq)obj;
        addParam("CONFIRMPIN", req.Msg, req);
        return 0;
    }
    public int SaveAmount(object obj)
    {
        UReq req = (UReq)obj;
        addParam("AMOUNT", req.Msg, req);
        return 0;
    }
    public int SaveNetwork(object obj)
    {
        Gadget g = new Gadget();
        UReq req = (UReq)obj;
        if (Convert.ToInt16(req.Msg) > 4)
        {
            req.next_op = req.op - 1;
            return 0;
        }

        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = ""; int ITEMSELECT = 0;
        ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
        nuban = g.GetNubanByListID(ITEMSELECT, req);

        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_CheckCusRegPIN");
        c.AddParam("@mob", req.Msisdn);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                int activated = int.Parse(dr["Activated"].ToString());
                string regisNuban = dr["nuban"].ToString().Trim();
                if (nuban != regisNuban)
                {
                    //cnt = -1;
                    continue;
                }
                else
                {
                    //Nuban matches one of customer's profile
                    nuban = regisNuban;
                    if (activated == 0)
                    {
                        cnt = -1;
                        continue;
                    }
                    if (activated == 1)
                    {
                        cnt = 1;
                        break;
                    }
                }

                if (cnt == -1)
                {
                    CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                    string response = cardws.GetActiveCards(nuban);
                    if (response.StartsWith("00"))
                    {
                        addParam("NUBAN", nuban, req);
                        cnt = 102;
                        addParam("Bypass", cnt.ToString(), req);
                        req.next_op = 126;
                        return 0;
                    }
                    else
                    {
                        addParam("NUBAN", nuban, req);
                        req.next_op = 124;
                        return 0;
                    }
                }
            }
        }
        else
        {
            //customer is okay to continue
        }

        addParam("NETWORK", req.Msg, req);
        return 0;
    }

    public int SaveExistingCustomer(object obj)
    {
        UReq req = (UReq)obj;
        addParam("CUSTSTATUS", req.Msg, req);
        return 0;
    }

    public int SaveAccountMobile(object obj)
    {
        int resp = 0; int cnt = 0;
        Gadget g = new Gadget();
        string frmNuban = "";
        //Msg  =   *822*500*080355904203
        UReq req = (UReq)obj;
        try
        {
            char[] sep = { '*', '#' };
            string[] bits = req.Msg.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            //Check if customer balance is ok
            int noOfAccts = g.GetNoofAccts(req.Msisdn.Trim());
            if (noOfAccts > 1)
            {
                //check will happen at SaveAcctSelected3
            }
            else
            {
                frmNuban = getDefaultAccount(req.Msisdn.Trim());
                decimal cusbal = 0; decimal amtval = 0;
                amtval = decimal.Parse(bits[1]);
                if (frmNuban.StartsWith("05"))
                {
                    IMALEngine iMALEngine = new IMALEngine();
                    ImalDetails imalinfo = new ImalDetails();
                    imalinfo = iMALEngine.GetImalDetailsByNuban(frmNuban);
                    cusbal = Convert.ToDecimal(imalinfo.AvailBal);
                    if (imalinfo.Status == 0)
                    {
                        req.next_op = 4190;
                        return 0;
                    }
                    if (cusbal > amtval)
                    {
                        //proceed
                        var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                        DataRow dr1 = ds1.Tables[0].Rows[0];
                        int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                        int activated = int.Parse(dr1["Activated"].ToString());
                       
                        if (trnxLimit == 1 && activated == 1)
                        {
                            Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                            getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                            getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                            if (limitedTuple.Item2 < amtval + Totaldone)
                            {
                                CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                                string response = cardws.GetActiveCards(frmNuban);
                                if (response.StartsWith("00"))
                                {
                                    req.next_op = 4290;
                                    return 0;
                                }
                                else
                                {
                                    cnt = 999;
                                    addParam("cnt", cnt.ToString(), req);
                                    addParam("NUBAN", frmNuban, req);
                                    req.next_op = 124;
                                    return 0;
                                }
                            }
                        }

                        if (trnxLimit == 0)
                        {
                            //600 for proper registered account
                            //Minimum ,Maximum per trns, Max per day
                            Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                            getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                            if (airtimeTuple.Item2 < amtval)
                            {
                                //Reroute to amount field
                                cnt = 104;
                                int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                                addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 1;
                                return 3;
                            }
                            if (airtimeTuple.Item3 < amtval + Totaldone)
                            {
                                //reroute to amount field
                                cnt = 103;
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 1;
                                return 3;
                            }
                        }
                    }
                    else
                    {
                        cnt = 102;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 1;
                        return 3;
                    }

                }
                else if (frmNuban.StartsWith("11"))
                {
                    BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
                    //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
                    BankOneClass bank1 = new BankOneClass();
                    bank1.accountNumber = frmNuban;
                    string bankOneReq = bank1.createReqForBalanceEnq();
                    var Bank1ref = LogBankOneRequest(req.SessionID, frmNuban, bankOneReq);
                    string encrptdReq = EncryptTripleDES(bankOneReq);
                    string getBankOneResp = bankOne_WS.BankOneBalanceEnquiry(encrptdReq);
                    bool isBankOne = bank1.readRespForBalEnq(getBankOneResp);
                    LogBankOneResponse(Bank1ref, getBankOneResp, bank1.status);
                    if (bank1.availableBal < amtval)
                    {
                        cnt = 102;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 1;
                        return 3;
                    }
                    else
                    {
                        //proceed
                        var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                        DataRow dr1 = ds1.Tables[0].Rows[0];
                        int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                        if (trnxLimit == 0)
                        {
                            //600 for proper registered account
                            //Minimum ,Maximum per trns, Max per day
                            Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                            getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                            if (airtimeTuple.Item2 < amtval)
                            {
                                //Reroute to amount field
                                cnt = 104;
                                int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                                addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 1;
                                return 3;
                            }
                            if (airtimeTuple.Item3 < amtval + Totaldone)
                            {
                                //reroute to amount field
                                cnt = 103;
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 1;
                                return 3;
                            }
                        }
                    }
                }
                else
                {
                    EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
                    DataSet dss = ws.getAccountFullInfo(frmNuban);
                    if (dss.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = dss.Tables[0].Rows[0];
                        string restFlag = dr["REST_FLAG"].ToString();
                        if (restFlag == "TRUE")
                        {
                            //Check for restriction code
                            var restCode = dss.Tables[1].Rows[0]["RestrCode"].ToString();
                            var restrictedCodes = ConfigurationManager.AppSettings["RestrictedCodes"].Split(',');

                            var isRestricted = restrictedCodes.Contains(restCode, StringComparer.OrdinalIgnoreCase);
                            if (isRestricted)
                            {
                                cnt = 99;
                                addParam("RestFlag", cnt.ToString(), req);
                                req.next_op = 419;
                                return 0;
                            }
                        }
                        cusbal = decimal.Parse(dr["UsableBal"].ToString());
                        if (cusbal > amtval)
                        {
                            //proceed
                            var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
                            DataRow dr1 = ds1.Tables[0].Rows[0];
                            int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                            int enrollType = int.Parse(dr1["EnrollType"].ToString());
                            int activated = int.Parse(dr1["Activated"].ToString());
                            if (enrollType == 1 && activated == 0)
                            {
                                //700 for auto enrolled account
                                //Minimum ,Maximum per trns, Max per day
                                Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(700);
                                getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                                if (airtimeTuple.Item3 < amtval + Totaldone)
                                {
                                    CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                                    string response = cardws.GetActiveCards(frmNuban);
                                    if (response.StartsWith("00"))
                                    {
                                        addParam("NUBAN", frmNuban, req);
                                        cnt = 102;
                                        addParam("Bypass", cnt.ToString(), req);
                                        addParam("cnt", cnt.ToString(), req);
                                        req.next_op = 126;
                                        return 0;
                                    }
                                    else
                                    {
                                        cnt = 999;
                                        addParam("cnt", cnt.ToString(), req);
                                        addParam("NUBAN", frmNuban, req);
                                        req.next_op = 124;
                                        return 0;
                                    }
                                }
                            }

                            if (trnxLimit == 1 && activated == 1)
                            {
                                Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                                getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                                getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                                if (limitedTuple.Item2 < amtval + Totaldone)
                                {
                                    CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                                    string response = cardws.GetActiveCards(frmNuban);
                                    if (response.StartsWith("00"))
                                    {
                                        req.next_op = 4290;
                                        return 0;
                                    }
                                    else
                                    {
                                        cnt = 999;
                                        addParam("cnt", cnt.ToString(), req);
                                        addParam("NUBAN", frmNuban, req);
                                        req.next_op = 124;
                                        return 0;
                                    }
                                }
                            }
                            if (trnxLimit == 0)
                            {
                                //600 for proper registered account
                                //Minimum ,Maximum per trns, Max per day
                                Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(600);
                                getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                                if (airtimeTuple.Item2 < amtval)
                                {
                                    //Reroute to amount field
                                    cnt = 104;
                                    int maxAmtPerTrans = Convert.ToInt16(airtimeTuple.Item2);
                                    addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                    addParam("cnt", cnt.ToString(), req);
                                    req.next_op = 1;
                                    return 3;
                                }
                                if (airtimeTuple.Item3 < amtval + Totaldone)
                                {
                                    //reroute to amount field
                                    cnt = 103;
                                    addParam("cnt", cnt.ToString(), req);
                                    req.next_op = 1;
                                    return 3;
                                }
                            }
                        }
                        else
                        {
                            cnt = 102;
                            //removeParam("Amt", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 1;
                            return 3;
                        }
                    }
                    else
                    {
                        req.next_op = 4190;
                        return 0;
                    }
                }

            }

            addParam("AMOUNT", bits[1], req);
            addParam("MOBILE", bits[2], req);
            resp = 0;
        }
        catch
        {
            resp = 1;
        }
        return resp;
    }
    public int SaveConfirm(object obj)
    {
        UReq req = (UReq)obj;
        addParam("CONFIRM", req.Msg, req);
        return 0;
    }
    public int SaveFromAcct(object obj)
    {
        UReq req = (UReq)obj;
        if (Convert.ToInt16(req.Msg) > 2)
        {
            req.next_op = req.op - 10;
            return 0;
        }
        addParam("FROMACCT", req.Msg, req);
        return 0;
    }
    public int SaveToPhone(object obj)
    {
        //i need to add 234
        UReq req = (UReq)obj;
        addParam("TOPHONE", req.Msg, req);
        return 0;
    }
    public int SaveFName(object obj)
    {
        UReq req = (UReq)obj;
        addParam("FNAME", req.Msg, req);
        return 0;
    }
    public int SaveLName(object obj)
    {
        UReq req = (UReq)obj;
        addParam("LNAME", req.Msg, req);
        return 0;
    }
    public int SaveEmail(object obj)
    {
        UReq req = (UReq)obj;
        addParam("EMAIL", req.Msg, req);
        return 0;
    }
    public int SaveBank(object obj)
    {
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        if (req.Msg == "9")
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            int page = RunBankPage(prm["BANKPAGE"]);
            page++;
            removeParam("BANKPAGE", req);
            addParam("BANKPAGE", page.ToString(), req);
            return 9;
        }
        addParam("TOBANK", req.Msg, req);
        return 1;
    }
    public int SaveMNO(object obj)
    {
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        if (req.Msg == "9")
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            int page = RunBankPage(prm["MNOPAGE"]);
            page++;
            removeParam("MNOPAGE", req);
            addParam("MNOPAGE", page.ToString(), req);
            return 9;
        }
        addParam("TOMNO", req.Msg, req);
        return 1;
    }
    public int SaveToAcct(object obj)
    {
        UReq req = (UReq)obj;
        addParam("TOACCT", req.Msg, req);
        return 0;
    }
    public int SaveToNUBAN(object obj)
    {
        UReq req = (UReq)obj;
        addParam("TONUBAN", req.Msg, req);
        return 0;
    }
    public int SaveCashOutSource(object obj)
    {
        UReq req = (UReq)obj;
        addParam("CASHOUTVIA", req.Msg, req);
        return 0;
    }
    public int SaveAgent(object obj)
    {
        UReq req = (UReq)obj;
        addParam("FROMAGENT", req.Msg, req);
        return 0;
    }

    public string PaintBankList(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = RunBankPage(prm["BANKPAGE"]);
            BankService bx = new BankService();
            List<Bank> lb = bx.GetBanksByPage(page.ToString());
            if (lb.Count <= 0)
            {
                removeParam("BANKPAGE", req);
                lb = bx.GetBanksByPage("0");
            }
            resp = "Enter Bank Code";
            foreach (Bank bank in lb)
            {
                resp += "%0A" + bank.TransRate + " " + bank.BankShort;
            }
            resp += "%0A9 Next Page";
        }
        catch
        {
        }
        return resp;
    }

    public string PaintMNOList(UReq req)
    {
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = RunMNOPage(prm["MNOPAGE"]);
            MNOService bx = new MNOService();
            List<MNO> lb = bx.GetMOByPage(page.ToString());
            if (lb.Count <= 0)
            {
                removeParam("MNOPAGE", req);
                lb = bx.GetMOByPage("0");
            }
            resp = "Enter MNO Code";
            foreach (MNO mo in lb)
            {
                resp += "%0A" + mo.MNOID + " " + mo.MNOName;
            }
            resp += "%0A9 Next Page";
        }
        catch
        {
        }
        return resp;
    }

    private string RunGetBank(string s)
    {
        BankService bx = new BankService();
        return bx.GetBankName(s);
    }

    private string RunGetMNO(string s)
    {
        MNOService bx = new MNOService();
        return bx.GetMOName(s);
    }

    private int RunBankPage(string s)
    {
        int k = 0;
        try
        {
            k = Convert.ToInt32(s);
        }
        catch
        {
        }
        return k;
    }

    private int RunMNOPage(string s)
    {
        int k = 0;
        try
        {
            k = Convert.ToInt32(s);
        }
        catch
        {
        }
        return k;
    }

    private string RunFromSource(string src)
    {
        string resp = "";
        switch (src)
        {
            case "1":
                resp = "WALLET"; break;
            case "2":
                resp = "BANK"; break;

        }
        return resp;
    }

    private string RunCheckMobile(string msisdn)
    {
        if (msisdn == "080") return "0";
        return "1";
    }

    private string RunFee(UReq req, string amount, int typ)
    {
        string fee = "";
        switch (typ)
        {
            case 1:
                fee = "0"; break;
            case 2:
                fee = "100"; break;
        }


        addParam("FEE", fee, req);
        return fee;
    }

    public string SendSms(string message, string mobileNo)
    {
        string resp = "";
        SMSAPIInfoBip sms = new SMSAPIInfoBip();
        string smsStatus = sms.insertIntoInfobip(message, mobileNo);
        resp = smsStatus;
        return resp;
    }

    public string ConvertMobile234(string mobile)
    {
        char[] trima = { '+', ' ' };
        mobile = mobile.Trim(trima);
        if (mobile.Length == 13 && mobile.StartsWith("234"))
        {
            return mobile;
        }
        if (mobile.Length >= 10)
        {
            mobile = "234" + mobile.Substring(mobile.Length - 10, 10);
            return mobile;
        }
        return mobile;
    }

    public static string ConvertMobile080(string mobile)
    {
        char[] trima = { '+', ' ' };
        mobile = mobile.Trim(trima);
        if (mobile.Length == 11 && mobile.StartsWith("0"))
        {
            return mobile;
        }
        if (mobile.Length >= 10)
        {
            mobile = "0" + mobile.Substring(mobile.Length - 10, 10);
            return mobile;
        }
        return mobile;
    }

    public string MaskDetails(string detail, int setLength, int firstPart, int secondPart)
    {
        if (detail.Length > setLength)
            return detail.Substring(0, firstPart) + string.Empty.PadLeft(detail.Length - setLength, '*') + detail.Substring(detail.Length - secondPart, secondPart);
        return detail;
    }

    public DataSet getRegisteredProfile(string mobile, string nuban)
    {
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        string sql = "select * from Go_Registered_Account where  Mobile  = @Mobile and Activated in (1,0) and statusflag = 1 and NUBAN = @nuban Order by Activated desc";
        c.SetSQL(sql);
        c.AddParam("@Mobile", mobile);
        c.AddParam("@nuban", nuban);
        DataSet ds = c.Select("rec");
        return ds;
    }
    public bool getTotalTransDonePerday(decimal amt, string mobile, string sessionid, decimal Maxperday)
    {
        DataSet ds = new DataSet();
        bool ok = false;
        string sql = "select ISNULL(SUM(amt),0) as totalTOday,ISNULL(count(amt),0) as cnt from tbl_USSD_transfers " +
            " where mobile =@mob " +
            " and CONVERT(Varchar(20), dateadded,102) = CONVERT(Varchar(20), GETDATE(),102) and statusflag = 1 ";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@mob", mobile);
        ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            Totaldone = decimal.Parse(dr["totalTOday"].ToString());
            //totlcnt = int.Parse(dr["cnt"].ToString());
            if (Totaldone + amt > Maxperday)
            {
                ok = true;
            }
            else
            {
                ok = false;
            }
        }
        else
        {
            ok = false;
        }

        return ok;
    }
    public Tuple<decimal, decimal> getMINMAXamt()
    {
        string sql = "select minamt,maxamt from tbl_USSD_AMTRANGE where statusflag=1";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        DataSet ds = c.Select("rec");
        var Minimum = decimal.Parse(ds.Tables[0].Rows[0]["minamt"].ToString());//Minimum
        var Maximum = decimal.Parse(ds.Tables[0].Rows[0]["maxamt"].ToString());//Maximum
        return Tuple.Create(Minimum, Maximum);
    }
    public Tuple<decimal, decimal> GetLimitMINMAXamt()
    {
        string sql = "select minamt,maxamt from tbl_USSD_AMTRANGE_Bypass where EnrollProp=1";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        DataSet ds = c.Select("rec");
        var Minimum = decimal.Parse(ds.Tables[0].Rows[0]["minamt"].ToString());//Minimum
        var Maximum = decimal.Parse(ds.Tables[0].Rows[0]["maxamt"].ToString());//Maximum
        return Tuple.Create(Minimum, Maximum);
    }
    public bool getTotalAirtimeDonePerday(decimal amt, string mobile, string sessionid, decimal Maxperday)
    {
        DataSet ds = new DataSet();
        bool ok = false;
        string sql = "select isnull((select SUM(Amount) from [dbo].[Go_Request] where Convert(date,[RequestDate]) = Convert(date,GETDATE()) AND Mobile  = @Mobile AND RequestStatus <> 2),0)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@Mobile", mobile);
        ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            //DataRow dr = ds.Tables[0].Rows[0][0];
            Totaldone += decimal.Parse(ds.Tables[0].Rows[0][0].ToString());

            if (Totaldone + amt > Maxperday)
            {
                ok = true;
            }
            else
            {
                ok = false;
            }
        }
        else
        {
            ok = false;
        }

        return ok;
    }
    public Tuple<decimal, decimal, decimal> getConfigAccount(int code)
    {
        string sql = "select * from tbl_USSD_Accounts where Code=@code ";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@code", code);
        DataSet ds = c.Select("rec");
        var Minimum = decimal.Parse(ds.Tables[0].Rows[0]["var1"].ToString());//Minimum
        var Maximum = decimal.Parse(ds.Tables[0].Rows[0]["var2"].ToString());//Maximum
        var Max_per_day = decimal.Parse(ds.Tables[0].Rows[0]["var3"].ToString());//Max per day
        return Tuple.Create(Minimum, Maximum, Max_per_day);
        //try
        //{

        //    DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, null).Tables[0];
        //    return dt;
        //}
        //catch (Exception ex)
        //{
        //    ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
        //    return null;
        //}
    }

    public string getDefaultAccount(string mobileNo)
    {
        string nuban = "";
        //string sql = "select NUBAN from Go_Registered_Account where mobile =@mb and Activated = 1 and StatusFlag=1";
        string sql = "select * from Go_Registered_Account where mobile = @mobile and Activated in (1, 0) and StatusFlag = 1 order by Activated desc";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mobile", mobileNo);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            nuban = dr["NUBAN"].ToString();
        }
        else
        {
            nuban = "";
        }

        return nuban;
    }
    public string getDefaultImalAcct(string mobileNo)
    {
        string nuban = "";
        //string sql = "select NUBAN from Go_Registered_Account where mobile =@mb and Activated = 1 and StatusFlag=1";
        string sql = "select NUBAN from Go_Registered_Account where mobile = @mobile and Activated in (1, 0) and StatusFlag = 1 and NUBAN like '05%' order by Activated desc";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mobile", mobileNo);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            nuban = dr["NUBAN"].ToString();
        }
        else
        {
            nuban = "";
        }

        return nuban;
    }

    public string GetFirstNameByMobileNo(string mobileNo)
    {
        EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        Gadget g = new Gadget();
        string firstName = "";
        string frmNuban = "";
        frmNuban = getDefaultAccount(mobileNo);
        if (frmNuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = iMALEngine.GetImalDetailsByNuban(frmNuban);
            firstName = imalinfo.CustomerName;
        }
        else if (frmNuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = frmNuban;
            string bankOneReq = bank1.createRequestForNameEnq();
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.BankOneGetAccountName(encrptdReq);
            bool isBankOne = bank1.readResponseForNameEnq(getBankOneResp);
            firstName = bank1.accountName;   
        }
        else
        {
            var ds = ws.getAccountFullInfo(frmNuban);
            firstName = ds.Tables[0].Rows[0]["NAME_LINE1"].ToString();
        }

        return firstName;

    }

    public DataSet getImalAcctsByMobile(string mobileNo)
    {
        string nuban = getDefaultImalAcct(mobileNo);
        DataSet result = null;

        string sql = @"select cif_sub_no from imal.amf where ADDITIONAL_REFERENCE ='" + nuban + "'";
        Sterling.Oracle.Connect c = new Sterling.Oracle.Connect("conn_imal");
        c.SetSQL(sql);
        try
        {
            var ds = c.Select();
            var custId = ds.Tables[0].Rows[0]["cif_sub_no"].ToString();

            string sqlData = @"SELECT AMF.ADDITIONAL_REFERENCE as Nuban, amf.cv_avail_bal as Balance, amf.gl_code as LedgerCode FROM IMAL.CIF INNER JOIN IMAL.AMF ON CIF.CIF_NO = AMF.CIF_SUB_NO
                WHERE amf.gl_code in (210801,210153,210804,210805,210201,210101) and CIF.cif_no = '" + custId + "'";
            Sterling.Oracle.Connect cc = new Sterling.Oracle.Connect("conn_imal");
            cc.SetSQL(sqlData);
            var dss = cc.Select();
            result = dss;
        }
        catch
        {

        }

        return result;

    }

    public bool checkForPin2(string nuban, string mobile, UReq req)
    {
        bool resp = false;
        //check the registration table to confirm if PIN entered is correct
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_CheckForPin2");
        c.AddParam("@mob", mobile);
        c.AddParam("@nuban", nuban);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            resp = true;
        }
        else
        {
            CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
            string response = cardws.GetActiveCards(nuban);
            if (response.StartsWith("00"))
            {
                //this person doesn't have an active card.
                int cnt = 111;
                addParam("cnt", cnt.ToString(), req);                
            }
            resp = false;
        }
        return resp;

    }

    public long LogBankOneRequest(string sessionId, string accountNo,string request)
    {
        long refid = 0;
        string sql = "insert into tbl_USSD_BankOne_Logs (SessionID,AccountNumber,Request) values (@ssid,@acctNo,@req)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@ssid", sessionId);
        c.AddParam("@acctNo", accountNo ?? "");
        c.AddParam("@req", request);
        try
        {
            refid = Convert.ToInt64(c.Insert());
        }
        catch
        {
            refid = -1;
        }

        return refid;
    }

    public void LogBankOneResponse(long refid, string response, string responseCode)
    {
        string sql = "Update tbl_USSD_BankOne_Logs  set ResponseCode=@respcode, Response= @resp, Dateprocessed=@date where Refid = @refid";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@resp", response);
        c.AddParam("@respcode", responseCode ?? "");
        c.AddParam("@date", DateTime.Now);
        c.AddParam("@refid", refid);
        c.Update();
    }
    //***************For BankOne Customers*******
    public static string EncryptTripleDES(string plainText)
    {
        byte[] byt = System.Text.Encoding.UTF8.GetBytes(plainText);
        string mdo = Convert.ToBase64String(byt);
        byte[] result;
        byte[] dataToEncrypt = System.Text.Encoding.UTF8.GetBytes(plainText);

        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        String keyString = "CIC9XRNBWPDAYQFEVKEWAZMVHXHBZCIU";
        byte[] keyB = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(keyString));
        hashmd5.Clear();

        var tdes = new TripleDESCryptoServiceProvider
        {
            Key = keyB,
            Mode = CipherMode.CBC,
            IV = new byte[8],
            Padding = PaddingMode.PKCS7
        };

        using (ICryptoTransform cTransform = tdes.CreateEncryptor())
        {
            result = cTransform.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            tdes.Clear();
        }
        return Convert.ToBase64String(result, 0, result.Length);
    }

   
}