using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using VTU.PL;

public class GoRechargeEngine : BaseEngine
{
    //*******************REGISTRATION**********************************
    //*822*1*Nuban and Fom menu
    public string SaveRequest(UReq req)
    {
        //*822*1*0005969437#
        string resp = "0"; string nuban = ""; string ledger_code = ""; int activated = -1; int cnt = 0; int trnxLimit = 0;
        IMALEngine iMALEngine = new IMALEngine();
        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();

        try
        {
            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            char[] delm = { '*', '#' };
            string msg = req.Msg.Trim();
            string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            if (s[0] == "1" && s.Length == 1)
            {
                //redirect to enter nuban
                req.next_op = 122;
                return "0";
            }

            nuban = s[2];

            //Check if BankOne Customer
            if (nuban.StartsWith("11"))
            {
                //Treat as BankOne
                BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
                //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
                BankOneClass bank1 = new BankOneClass();
                bank1.accountNumber = nuban;
                string bankOneReq = bank1.createRequestForNameEnq();
                string encrptdReq = EncryptTripleDES(bankOneReq);
                string getBankOneResp = bankOne_WS.BankOneGetAccountName(encrptdReq);
                bool isBankOne = bank1.readResponseForNameEnq(getBankOneResp);
                if (bank1.status != "True")
                {
                    cnt = 104;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }
                if (bank1.phoneNumber != req.Msisdn.Replace("234", "0"))
                {
                    cnt = 105;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }

                Sterling.MSSQL.Connect cc = new Sterling.MSSQL.Connect();
                cc.SetProcedure("spd_getRegisteredUserProfile");
                cc.AddParam("@mobile", req.Msisdn.Trim());
                DataSet bankOne = cc.Select("rec");
                if (bankOne.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = bankOne.Tables[0].Rows[0];
                    activated = int.Parse(dr["Activated"].ToString());
                    nuban = dr["nuban"].ToString();
                    if (activated == 1)
                    {
                        req.next_op = 4040;
                        return "0";
                    }
                    else
                    {
                        //Continue
                        addParam("NUBAN", nuban, req);
                        req.next_op = 124;
                        return "0";
                    }
                }
            }

            //Treat as Imal customer
            if (nuban.StartsWith("05"))
            {
                var regexItem = new Regex("^[0-9-]*$");
                if (!regexItem.IsMatch(nuban))
                {
                    cnt = 101;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }
                if (nuban.Length != 10)
                {
                    cnt = 100;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }
                var isMobValid = iMALEngine.validateMobileAndNuban(nuban, req.Msisdn);
                if (!isMobValid)
                {
                    cnt = 103;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }
                ImalDetails imalinfo = new ImalDetails();
                imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
                if (imalinfo.Status == -1)
                {
                    cnt = 106;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }

                if (imalinfo.Status == 0)
                {
                    cnt = 104;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }

                Sterling.MSSQL.Connect cc = new Sterling.MSSQL.Connect();
                cc.SetProcedure("spd_getRegisteredUserProfile");
                cc.AddParam("@mobile", req.Msisdn.Trim());
                DataSet dsImal = cc.Select("rec");
                if (dsImal.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsImal.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = dsImal.Tables[0].Rows[i];
                        activated = int.Parse(dr["Activated"].ToString());
                        string dbNuban = dr["nuban"].ToString();
                        trnxLimit = int.Parse(dr["TrnxLimit"].ToString());
                        if (activated == 1 && nuban == dbNuban)
                        {
                            if (trnxLimit != 0)
                            {
                                //Continue to upgrade account.
                                break;
                            }
                            else
                            {
                                req.next_op = 4040;
                                return "0";
                            }
                        }
                        if (nuban == dbNuban)
                        {
                            break;
                        }
                    }
                }

                //Check for resticted ledger codes

                //Check for active card on account.
                string imalResponse = cardws.GetActiveCards(nuban);
                if (imalResponse.StartsWith("00"))
                {
                    if (trnxLimit == 1)
                    {
                        req.next_op = 4290;
                        return "0";
                    }
                    cnt = 102;
                    addParam("Bypass", cnt.ToString(), req);
                    addParam("NUBAN", nuban, req);
                    req.next_op = 126;
                    return "0";
                }
                else
                {
                    //has active card on account
                    addParam("NUBAN", nuban, req);
                    req.next_op = 124;
                    return "0";
                }
            }


            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds1 = c.Select("rec");
            if (ds1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds1.Tables[0].Rows[i];
                    activated = int.Parse(dr["Activated"].ToString());
                    string dbNuban = dr["nuban"].ToString();
                    trnxLimit = int.Parse(dr["TrnxLimit"].ToString());
                    if (activated == 1 && nuban == dbNuban)
                    {
                        if (trnxLimit != 0)
                        {
                            //Continue to upgrade account.
                        }
                        else
                        {
                            req.next_op = 4040;
                            return "0";
                        }
                    }
                    else
                    {
                        //Continue
                    }
                }
            }
            //else
            //{
            //    //redirect to register and set PIN
            //    req.next_op = 122;
            //    return "0";
            //}



            DataSet ds = new DataSet();
            ds = ws.getAccountFullInfo(nuban);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                ledger_code = dr["T24_LED_CODE"].ToString();
                //Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
                c.SetProcedure("spd_getRestrictedLedgCode");
                c.AddParam("@led_code", int.Parse(ledger_code));
                DataSet dss = c.Select("rec");
                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    //Acct is restricted
                    req.next_op = 419;
                    return "0";
                }
                else
                {
                    //Continue
                }
            }
            else
            {
                //Invalid nuban
                req.next_op = 4190;
                return "0";
            }


            string response = cardws.GetActiveCards(nuban);
            if (response.StartsWith("00"))
            {
                if (trnxLimit == 1)
                {
                    req.next_op = 4290;
                    return "0";
                }
                cnt = 102;
                addParam("Bypass", cnt.ToString(), req);
                addParam("NUBAN", nuban, req);
                //addParam("cnt", cnt.ToString(), req);
                req.next_op = 126;
                return "0";
            }
            else
            {
                //has active card on account
                addParam("NUBAN", nuban, req);
                req.next_op = 124;
                return "0";
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string CollectNuban(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //int cnt = RunCount(prm["cnt"]);
        try
        {
            int wrongCnt = RunCount(prm["wrongCnt"]);
            int cnt = RunCount(prm["cnt"]);
            if (wrongCnt == 3)
            {
                removeParam("wrongCnt", req);
                resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your registration. Thank you";
                return resp;
            }
            if (cnt == 0)
            {
                //first entrance
                removeParam("cnt", req);
                resp = "Kindly enter your account number:";
                return resp;
            }
            else if (cnt == 100)
            {
                removeParam("cnt", req);
                resp = "Kindly ensure that the required length is not less than or greater than 10 digits";
                return "Collect account number: %0A" + resp;
            }
            else if (cnt == 101)
            {
                removeParam("cnt", req);
                resp = "Kindly ensure you enter only 10 digits and not alphanumeric";
                return "Collect account number:%0A" + resp;
            }
            else if (cnt == 102)
            {
                removeParam("cnt", req);
                //resp = "To complete your registration, kindly go to the nearest Sterling branch to request for a debit card.%0AThank you";
                resp = "You need an active Card to complete your Registration. To request for an Instant Card, please call +23470078375464 or visit a Branch close to you.";
                return resp;
            }
            else if (cnt == 103)
            {
                removeParam("cnt", req);
                resp = "Your phone number and account number do not match. Kindly visit the nearest branch or call +23470078375464 to update your details ";
                return resp;
            }
            else if (cnt == 104)
            {
                removeParam("cnt", req);
                resp = "The account number you entered is not profiled with us.";
                return resp;
            }
            else if (cnt == 105)
            {
                removeParam("cnt", req);
                resp = "Your phone number and account number do not match. Kindly visit the nearest BankOne branch to update your details";
                return resp;
            }
            else if (cnt == 106)
            {
                removeParam("cnt", req);
                resp = "Sorry, an error occurred while getting your details";
                return resp;
            }

        }
        catch
        {
            resp = "Kindly enter your account number:";
        }

        //resp = "Kindly enter your account number";
        return resp;
    }
    public string SaveNUBAN(UReq req)
    {
        string resp = "0"; string nuban = ""; string ledger_code = ""; int cnt = 0; int activated = 0; string mobile = "";
        int trnxLimit = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        IMALEngine iMALEngine = new IMALEngine();
        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
        try
        {
            nuban = req.Msg;
            if (req.Msg.Length != 10)
            {
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                wrongCnt++;
                removeParam("wrongCnt", req);
                addParam("wrongCnt", wrongCnt.ToString(), req);
                req.next_op = 122;
                return "0";
            }

            try
            {
                long acctno = long.Parse(req.Msg);
            }
            catch
            {
                cnt = 101;
                addParam("cnt", cnt.ToString(), req);
                wrongCnt++;
                removeParam("wrongCnt", req);
                addParam("wrongCnt", wrongCnt.ToString(), req);
                req.next_op = 122;
                return "0";
            }

            try //bank one case
            {
                BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
                //string testBank = "<BankOneDeposit><Authentication><Key>22948DD9-A03D-42C1-88D8-3A297D0F4F51</Key></Authentication><DepositRequest><SourceAccountNumber>0063888910</SourceAccountNumber><SourceAccountName>OLUMIDE AJAYI</SourceAccountName><DestinationAccountNumber>1100015937</DestinationAccountNumber><DestinationAccountName>Onigbinde Simeon</DestinationAccountName><DestinationPhoneNumber>08169768689</DestinationPhoneNumber><Amount>10000</Amount><ReferenceNumber>RECH08723</ReferenceNumber><Narration>Test</Narration><TransactionType>2</TransactionType></DepositRequest></BankOneDeposit>";
                //string encrData = EncryptTripleDES(testBank);
                //string resss = bankOne_WS.BankOneDeposit(encrData);       

                //string testBank = "<BankOneWithdrawal><Authentication><Key>22948DD9-A03D-42C1-88D8-3A297D0F4F51</Key></Authentication><WithdrawalRequest><SourceAccountNumber>1100015937</SourceAccountNumber><SourceAccountName>Onigbinde Simeon</SourceAccountName><DestinationAccountNumber>1100015937</DestinationAccountNumber><DestinationAccountName>Olumide Ajayi</DestinationAccountName><DestinationPhoneNumber>08130057119</DestinationPhoneNumber><Amount>500</Amount><ReferenceNumber>RECH28830</ReferenceNumber><Narration>Test</Narration><TransactionType>2</TransactionType></WithdrawalRequest></BankOneWithdrawal>";
                //string encrData = EncryptTripleDES(testBank);
                //string resss = bankOne_WS.BankOneWithdrawal(encrData);

                //Check if BankOne Customer
                if (nuban.StartsWith("11"))
                {
                    //Treat as BankOne                            
                    BankOneClass bank1 = new BankOneClass();
                    bank1.accountNumber = nuban;
                    //string bankOneReq = bank1.createRequestForNameEnq();
                    string bankOneReq = bank1.createReqForBalanceEnq();
                    // string encrptdReq = EncryptTripleDES(bankOneReq);
                    string encrptdReq = EncryptTripleDES(bankOneReq);
                    // string getBankOneResp = bankOne_WS.BankOneGetAccountName(encrptdReq);
                    string getBankOneResp = bankOne_WS.BankOneBalanceEnquiry(encrptdReq);
                    // bool isBankOne = bank1.readResponseForNameEnq(getBankOneResp);
                    bool isBankOne = bank1.readRespForBalEnq(getBankOneResp);
                    if (!isBankOne)
                    {
                        cnt = 104;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 122;
                        return "0";
                    }
                    if (bank1.phoneNumber != req.Msisdn.Replace("234", "0"))
                    {
                        cnt = 105;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 122;
                        return "0";
                    }

                    //Continue 
                    addParam("NUBAN", nuban, req);
                    req.next_op = 124;
                    return "0";
                }
            }
            catch (Exception ex)
            {

            }

            //Treat as Imal customer
            if (nuban.StartsWith("05"))
            {
                var isMobValid = iMALEngine.validateMobileAndNuban(nuban, req.Msisdn);
                if (!isMobValid)
                {
                    cnt = 103;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }
                ImalDetails imalinfo = new ImalDetails();
                imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
                if (imalinfo.Status == -1)
                {
                    cnt = 106;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }

                if (imalinfo.Status == 0)
                {
                    cnt = 104;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }

                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
                c.SetProcedure("spd_getRegisteredUserProfile");
                c.AddParam("@mobile", req.Msisdn.Trim());
                DataSet dsImal = c.Select("rec");
                if (dsImal.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsImal.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = dsImal.Tables[0].Rows[i];
                        activated = int.Parse(dr["Activated"].ToString());
                        string dbNuban = dr["nuban"].ToString();
                        trnxLimit = int.Parse(dr["TrnxLimit"].ToString());
                        if (activated == 1 && nuban == dbNuban)
                        {
                            if (trnxLimit != 0)
                            {
                                //Continue to upgrade account.
                                break;
                            }
                            else
                            {
                                req.next_op = 4040;
                                return "0";
                            }
                        }
                        if (nuban == dbNuban)
                        {
                            break;
                        }
                    }
                }

                //Check for resticted ledger codes

                //Check for active card on account.
                string imalResponse = cardws.GetActiveCards(nuban);
                if (imalResponse.StartsWith("00"))
                {
                    if (trnxLimit == 1)
                    {
                        req.next_op = 4290;
                        return "0";
                    }
                    cnt = 102;
                    addParam("Bypass", cnt.ToString(), req);
                    addParam("NUBAN", nuban, req);
                    req.next_op = 126;
                    return "0";
                }
                else
                {
                    //has active card on account
                    addParam("NUBAN", nuban, req);
                    req.next_op = 124;
                    return "0";
                }
            }



            Sterling.MSSQL.Connect cc = new Sterling.MSSQL.Connect();
            cc.SetProcedure("spd_getRegisteredUserProfile");
            cc.AddParam("@mobile", req.Msisdn.Trim());
            DataSet DS = cc.Select("rec");
            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
            {

                for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = DS.Tables[0].Rows[i];
                    activated = int.Parse(dr["Activated"].ToString());
                    string dbNuban = dr["nuban"].ToString();
                    trnxLimit = int.Parse(dr["TrnxLimit"].ToString());
                    if (activated == 1 && nuban == dbNuban)
                    {
                        if (trnxLimit != 0)
                        {
                            //Continue to upgrade account. 
                            break;
                        }
                        else
                        {
                            req.next_op = 4040;
                            return "0";
                        }
                    }
                    if (nuban == dbNuban)
                    {
                        break;
                    }
                }
            }

            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            DataSet ds = new DataSet();
            ds = ws.getAccountFullInfo(nuban);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                mobile = dr["MOB_NUM"].ToString();
                mobile = mobile.StartsWith("0") && mobile.Length == 11 ? "234" + mobile.Substring(1, 10) : mobile;
                if (req.Msisdn != mobile)
                {
                    cnt = 103;
                    addParam("cnt", cnt.ToString(), req);
                    wrongCnt++;
                    removeParam("wrongCnt", req);
                    addParam("wrongCnt", wrongCnt.ToString(), req);
                    req.next_op = 122;
                    return "0";
                }

                ledger_code = dr["T24_LED_CODE"].ToString();
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
                c.SetProcedure("spd_getRestrictedLedgCode");
                c.AddParam("@led_code", int.Parse(ledger_code));
                DataSet dss = c.Select("rec");
                if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
                {
                    //Acct is restricted 
                    req.next_op = 419;
                    return "0";
                }
                else
                {
                    //Continue
                }
            }
            else
            {
                //Invalid nuban
                req.next_op = 4190;
                return "0";
            }

            string response = cardws.GetActiveCards(nuban);
            if (response.StartsWith("00"))
            {
                if (trnxLimit == 1)
                {
                    cnt = 1;
                    addParam("NoCard", cnt.ToString(), req);
                    req.next_op = 4290;
                    return "0";
                }
                addParam("NUBAN", nuban, req);
                cnt = 102;
                addParam("Bypass", cnt.ToString(), req);
                req.next_op = 126;
                return "0";
            }

            addParam("NUBAN", nuban, req);

        }
        catch (Exception ex)
        {
            resp = "0";
        }
        return resp;
    }
    public string CollectBVN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        int bypass = RunCount(prm["Bypass"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your registration. Thank you";
            return resp;
        }
        if (bypass == 102 && cnt == 102)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your BVN to register this account for this service and upgrade your transaction limit";
            return "Bank Verification Number: " + resp;
        }
        if (bypass == 102)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your BVN to be registered for this service";
            return "Bank Verification Number: " + resp;
        }
        if (cnt == 0 || cnt == 111)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your BVN";
            return "Bank Verification Number: " + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of BVN is not less than or greater than 11 digits";
            return "Bank Verification Number: " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 11 digits and not alphanumeric";
            return "Bank Verification Number: " + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Oops! The BVN you have entered does not match what we have in our records. Please enter the correct BVN and try again.";
            return "Bank Verification Number: " + resp;
        }

        resp = "Kindly enter your BVN";
        return "Bank Verification Number: " + resp;
    }
    public string SaveBVN(UReq req)
    {
        string prms = getParams(req);
        int cnt = 0;
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        int wrongCnt = RunCount(prm["wrongCnt"]);

        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 11 || req.Msg.Length > 11)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 126;
            return "0";
        }
        //check if entry is numeric
        try
        {
            long BVN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 126;
            return "0";
        }

        //Check if BankOne Customer
        if (req.Msg.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            BankOneClass bank1 = new BankOneClass
            {
                accountNumber = nuban
            };
            string bankOneReq = bank1.createRequestForNameEnq();
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.BankOneGetAccountName(encrptdReq);
            bool isBankOne = bank1.readResponseForNameEnq(getBankOneResp);
            if (bank1.BVN != req.Msg)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 126;
                return "0";
            }

            //Continue 
            addParam("BVN", req.Msg, req);
            return "0";
        }

        string resp = "0";
        try
        {
            addParam("BVN", req.Msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string ValidateBVN(UReq req)
    {
        string resp = "0"; string prms = getParams(req); int cnt = 0;
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        string bvn = prm["BVN"]; string bvnRs = ""; string PhoneNumber = "";
        EACBS.banksSoapClient ws = new EACBS.banksSoapClient();

        if (nuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
            if (imalinfo.BVN != bvn)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 126;
                return "0";
            }

            return resp;
        }
        //ValBVN.ServiceSoapClient ws = new ValBVN.ServiceSoapClient();
        //DataSet ds = ws.GetBVN(bvn);
        DataSet ds = ws.getAccountFullInfo(nuban);
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            bvnRs = dr["BVN"].ToString();
            PhoneNumber = dr["MOB_NUM"].ToString();
            if (PhoneNumber.StartsWith("0"))
            {
                PhoneNumber = ConvertMobile234(PhoneNumber);
            }
            if (bvnRs == bvn && PhoneNumber == req.Msisdn)
            {

            }
            else
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 126;
                return "0";
            }
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 126;
            return "0";
        }
        return resp;
    }
    public string CollectLast6Digits(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your registration. Thank you";
            return resp;
        }
        if (cnt == 999)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter last 6 digits of your card linked to your account to upgrade your USSD limit:";
            return resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter last 6 digits of your card:";
            return resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is exactly 6 digits";
            return resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is not alphanumeric";
            return resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Oops! You have entered the last 6 digits of your Debit Card wrongly. Please check your Card and try again.";
            return resp;
        }
        else if (cnt == 111)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter last 6 digits of your card to activate your profile:";
            return resp;
        }

        resp = "Enter last 6 digits of your card:";
        return resp;
    }
    public string SaveLastSixDigits(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        int wrongCnt = RunCount(prm["wrongCnt"]);

        try
        {
            long last6digits = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 124;
            return "0";
        }
        if (req.Msg.Length != 6)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 124;
            return "0";
        }

        //Check if BankOne Customer
        if (nuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = nuban;
            bank1.last6Digits = req.Msg.Trim();
            string bankOneReq = bank1.createReqForCardValidation();
            string encrptdReq = EncryptTripleDES(bankOneReq);
            var getBankOneResp = bankOne_WS.CardIssuanceCardValidation(encrptdReq);
            bool isBankOne = bank1.readRespForCardValidation(getBankOneResp);
            if (!isBankOne)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 124;
                return "0";
            }
            return "0";
        }

        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_PostCard");
        c.SetProcedure("getCrdDet_From6Digits");
        c.AddParam("@pan", req.Msg);
        c.AddParam("@nuban", nuban);
        DataSet dss = c.Select("rec");
        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
        {
            //last 6 digits match with the nuban
            //continue
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 124;
            return "0";
        }

        return resp;
    }
    public string SetUserPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int bypass = RunCount(prm["Bypass"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your registration. Thank you";
            return resp;
        }
        if (cnt == 0 && bypass == 101)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter any 4-digit USSD PIN of choice";
            return "Set your USSD PIN: %0A" + resp;
        }
        if (cnt == 0 && bypass == 102)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter any 4-digit USSD PIN of choice";
            return "Set USSD PIN for this account: %0A" + resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            //resp = "Set your USSD PIN";
            //return "Set PIN: " + resp;
            return "Enter any 4-digit USSD PIN of choice: ";
        }

        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the length of the PIN is exactly 4 digits";
            return "Set PIN: " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4-digit PIN and not alphanumeric";
            return "Set PIN: " + resp;
        }

        //resp = "Set your USSD PIN";
        return "Set PIN: " + resp;
    }
    public string ConfirmSetUserPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your registration. Thank you";
            return resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            //resp = "Enter any 4 digit USSD PIN of choice";
            return "Confirm USSD PIN: ";
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the length of the PIN is exactly 4 digits";
            return "Confirm USSD PIN:%0A " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4-digit PIN and not alphanumeric";
            return "Confirm USSD PIN:%0A " + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The confirmation PIN did not match the initial PIN you entered.  Kindly ensure they are the same";
            return "Confirm USSD PIN:%0A " + resp;
        }

        return "";
    }
    public string SaveCusPIN(UReq req)
    {
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 116;
            return "9";
        }
        //check if entry is numeric
        try
        {
            int pin = int.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 116;
            return "9";
        }
        string resp = "0";
        try
        {
            //encrypt at this stage
            //encrypt the value
            string EncryptPIN = "";
            Encrypto EnDeP = new Encrypto();
            req.Msg = EnDeP.Encrypt(req.Msg);
            cnt = 0;
            addParam("SavePIN", req.Msg, req);
            addParam("cnt", cnt.ToString(), req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveConfrimPIN(UReq req)
    {
        int cnt = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 120;
            return "9";
        }
        //check if entry is numeric
        try
        {
            int pin = int.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 120;
            return "9";
        }
        //do get the display info
        string resp = "0";
        string Initial_PIN = prm["SavePIN"];

        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);

        if (Initial_PIN == req.Msg)
        {
            resp = "0";
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 120;
            return "9";
        }

        try
        {
            addParam("SaveConfrimPIN", req.Msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    public string DoSelfReg(UReq req)
    {
        string resp = "";
        try
        {
            char[] delm = new char[] { '*' };
            string msg = req.Msg.Trim().TrimStart().TrimEnd();
            string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);

            //string nuban = s[2].Replace("#", string.Empty).TrimEnd();
            string smsMessage = "";
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            int bypass = RunCount(prm["Bypass"]);
            string nuban = prm["NUBAN"];
            string SavePIN = prm["SavePIN"];
            //resp = USSD.DoActivation(req.Msisdn, nuban);
            RegCustomers CusReg = new RegCustomers();
            resp = CusReg.DoActivate(req.Msisdn, nuban, SavePIN, bypass);
            SMSAPIInfoBip SendSms = new SMSAPIInfoBip();

            //Message for customer who registered without active card
            if (resp.StartsWith("Great") && (bypass == 102))
            {
                //EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
                //var ds = ws.getAccountFullInfo(nuban);
                //DataRow dr = ds.Tables[0].Rows[0];
                //string custName = dr["NAME_LINE1"].ToString();
                //string smsMessage = "Dear " + custName + ", you are now registered for magic banking. Simply dial *822*Amount# to buy airtime on the go!" + Environment.NewLine + "More Info? Call +23470078375464";
                //resp = "You are now registered for magic banking. Simply dial *822*Amount# to buy airtime on the go!";
                smsMessage = "You are now registered for our Magic Banking! Enjoy instant transfers, airtime top-up and a lot more with *822#.  More info? Call 070078375464";
                SendSms.insertIntoInfobip(smsMessage, req.Msisdn);
                //SendSms(smsMessage, req.Msisdn);
                resp = "Great! You are now registered for Magic Banking. Buy airtime on the go with *822*amount#.";
            }

            //Message for customer who tries to do transaction but hasn't set USSD pin for that account. //102-transaction //101 -first registration
            if (resp.StartsWith("Great") && (bypass == 101 || bypass == 111))
            {
                
                if (bypass == 101)
                {
                    smsMessage = "You are now registered for our Magic Banking! Enjoy instant transfers, airtime top-up and a lot more with *822#.  More info? Call 070078375464";
                    //smsMessage = "Dear " + custName + ", you are now registered for magic banking. Simply dial *822*Amount# to buy airtime on the go!" + Environment.NewLine + "More Info? Call +23470078375464";
                    //SendSms(smsMessage, req.Msisdn);
                    SendSms.insertIntoInfobip(smsMessage, req.Msisdn);
                    resp = "You are now registered for magic banking. Simply dial *822*Amount# to buy airtime on the go!";
                }
                string maskedNuban = MaskDetails(nuban, 9, 0, 4);
                if (bypass == 102)
                {
                    smsMessage = "You are now registered for our Magic Banking! Enjoy instant transfers, airtime top-up and a lot more with *822#.  More info? Call 070078375464";
                    //string smsMessage = "Dear " + custName + ", your " + maskedNuban + " is now registered for magic banking. Simply dial *822*Amount# to buy airtime on the go!" + Environment.NewLine + "More Info? Call +23470078375464";
                    SendSms.insertIntoInfobip(smsMessage, req.Msisdn);
                    resp = "Your account is now fully registered for magic banking.";
                }
            }
        }
        catch
        {
            resp = "ERROR: Request string not supported!%0ADial *822*1*NUBAN# to register.";
        }
        return resp;
    }

    //***************AIRTIME FOR SELF************************************

    //airtime self from menu
    public string CollectAmt(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int amtLimit = RunCount(prm["MaxAmtPerTrans"]);

        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter airtime amount";
            return "Airtime for self:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 100 naira";
            return "Airtime for self:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Airtime for self:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            var firstName = "";
            try
            {
                firstName = GetFirstNameByMobileNo(req.Msisdn.Trim());
                resp = "Dear " + firstName + ", you do not have sufficient balance for this transaction. Please credit your account and try again. Thank you.";
                return resp;
            }
            catch
            {
                resp = "You currently do not have sufficient balance for this transaction.";
            }
            return "Airtime for self:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry you have exceeded your airtime purchase limit for today.";
            return "Airtime for self:%0A" + resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            if (amtLimit == 0)
            {
                resp = "The amount entered is more than maximum amount allowed per transaction";
            }
            else
            {
                resp = "Please enter an amount lower than N" + amtLimit.ToString("#,##");
            }
            return "Airtime for self:%0A" + resp;
        }
        resp = "Enter airtime amount";
        return "Airtime for self:%0A" + resp;
    }
    public string SaveAirtimeSelfAMt(UReq req)
    {
        int cnt = 0; string resp = ""; Gadget g = new Gadget(); string frmNuban = "";
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 1;
            return "2";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (decimal.Parse(req.Msg) < 100)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 1;
            return "2";
        }

        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            //Check if customer balance is ok
            int noOfAccts = g.GetNoofAccts(req.Msisdn.Trim());
            if (noOfAccts > 1)
            {
                //check will happen at SaveAcctSelected
            }
            else
            {
                frmNuban = getDefaultAccount(req.Msisdn.Trim());

                decimal cusbal = 0; decimal amtval = 0;
                amtval = decimal.Parse(req.Msg.Trim());
                if (frmNuban.StartsWith("05"))
                {
                    IMALEngine iMALEngine = new IMALEngine();
                    ImalDetails imalinfo = new ImalDetails();
                    imalinfo = iMALEngine.GetImalDetailsByNuban(frmNuban);
                    cusbal = Convert.ToDecimal(imalinfo.AvailBal);
                    if (imalinfo.Status == 0)
                    {
                        req.next_op = 4190;
                        return "0";
                    }
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
                                    return "0";
                                }
                                else
                                {
                                    cnt = 999;
                                    addParam("cnt", cnt.ToString(), req);
                                    addParam("NUBAN", frmNuban, req);
                                    req.next_op = 124;
                                    return "0";
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
                                    return "0";
                                }
                                else
                                {
                                    cnt = 999;
                                    addParam("cnt", cnt.ToString(), req);
                                    addParam("NUBAN", frmNuban, req);
                                    req.next_op = 124;
                                    return "0";
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
                                return "2";
                            }
                            if (airtimeTuple.Item3 < amtval + Totaldone)
                            {
                                //reroute to amount field
                                cnt = 103;
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 1;
                                return "2";
                            }
                        }
                    }
                    else
                    {
                        cnt = 102;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 1;
                        return "2";
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
                                return "0";
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
                                        return "0";
                                    }
                                    else
                                    {
                                        cnt = 999;
                                        addParam("cnt", cnt.ToString(), req);
                                        addParam("NUBAN", frmNuban, req);
                                        req.next_op = 124;
                                        return "0";
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
                                        return "0";
                                    }
                                    else
                                    {
                                        cnt = 999;
                                        addParam("cnt", cnt.ToString(), req);
                                        addParam("NUBAN", frmNuban, req);
                                        req.next_op = 124;
                                        return "0";
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
                                    return "2";
                                }
                                if (airtimeTuple.Item3 < amtval + Totaldone)
                                {
                                    //reroute to amount field
                                    cnt = 103;
                                    addParam("cnt", cnt.ToString(), req);
                                    req.next_op = 1;
                                    return "2";
                                }
                            }
                        }
                        else
                        {
                            cnt = 102;
                            //removeParam("Amt", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 1;
                            return "2";
                        }
                    }
                    else
                    {
                        req.next_op = 4190;
                        return "0";
                    }
                }
            }

        }
        else
        {
            //redirect to register and set PIN
            req.next_op = 122;
            return "0";
        }

        //save it
        try
        {
            addParam("Amt", req.Msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string CheckNoofAccts(UReq req)
    {
        //this is to check how many number of accounts exist for the cutomer
        //if 1 then redirect to display summary which is 91130 0
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            req.next_op = 5501;
            return "0";
        }
        else
        {
            req.next_op = 5401;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb(UReq req)
    {
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_getAcctList");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            req.next_op = 5402;
            return "0";
        }
        else
        {
            return "0";
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
            //resp = "Select Biller Item";
            foreach (AccountList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.Nuban;
                }
                else
                {
                    resp += "%0A" + item.TransRate + " " + item.Nuban;
                }
            }
            resp += "%0A99 Next";
        }
        catch
        {
        }
        return "Select Account%0A" + resp;
    }
    public int SaveAcctSelected(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "99")
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
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
                //string prms = getParams(req);
                //NameValueCollection prm = splitParam(prms);
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }

        string nuban = g.GetNubanByListID(int.Parse(req.Msg), req);
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_CheckCusRegPIN");
        c.AddParam("@mob", req.Msisdn);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                //activated = int.Parse(dr["Activated"].ToString());
                string regisNuban = dr["nuban"].ToString().Trim();
                if (nuban != regisNuban)
                {
                    continue;
                }
                else
                {
                    //Nuban matches one of customer's profile
                    nuban = regisNuban;
                    int activated = int.Parse(dr["Activated"].ToString());
                    if (activated == 0)
                    {
                        //Customer doesnt need pin to buy airtime for self
                    }
                }
            }
        }
        else
        {
            //customer is okay to continue
        }

        string amount = prm["Amt"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(amount);
        if (nuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = new ImalDetails();
            imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
            cusbal = Convert.ToDecimal(imalinfo.AvailBal);
            if (imalinfo.Status == 0)
            {
                req.op = 4190;
                return 0;
            }
            if (cusbal > amtval)
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
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
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            addParam("NUBAN", nuban, req);
                            cnt = 102;
                            addParam("Bypass", cnt.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 126;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
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
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            cnt = 111;
                            addParam("NoCard", cnt.ToString(), req);
                            req.op = 4290;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
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
                        removeParam("Amt", req);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 1;
                        return 2;
                    }
                    if (airtimeTuple.Item3 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("Amt", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 1;
                        return 2;
                    }
                }
            }
            else
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.op = 1;
                return 2;
            }

        }
        else
        {

            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            DataSet dss = ws.getAccountFullInfo(nuban);
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
                        req.op = 419;
                        return 0;
                    }
                }
                cusbal = decimal.Parse(dr["UsableBal"].ToString());
                if (cusbal > amtval)
                {
                    //proceed
                    var ds1 = getRegisteredProfile(req.Msisdn, nuban);
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
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                addParam("NUBAN", nuban, req);
                                cnt = 102;
                                addParam("Bypass", cnt.ToString(), req);
                                addParam("cnt", cnt.ToString(), req);
                                req.op = 126;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
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
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                cnt = 111;
                                addParam("NoCard", cnt.ToString(), req);
                                req.op = 4290;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
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
                            removeParam("Amt", req);
                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 1;
                            return 2;
                        }
                        if (airtimeTuple.Item3 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            removeParam("Amt", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 1;
                            return 2;
                        }
                    }
                }
                else
                {
                    cnt = 102;
                    removeParam("Amt", req);
                    addParam("cnt", cnt.ToString(), req);
                    req.op = 1;
                    return 2;
                }
            }
            else
            {
                req.op = 4190;
                return 0;
            }
        }

        addParam("ITEMSELECT", req.Msg, req);
        addParam("A", "2", req);
        return 1;
    }

    public string SubmitAirtimeSelf(UReq req)
    {
        Gadget g = new Gadget(); string frmNuban = "";
        string resp = ""; int flag = 0; int ITEMSELECT = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string Amt = prm["Amt"];
        flag = int.Parse(prm["A"]);
        if (flag == 1)
        {
            frmNuban = "";
        }
        else if (flag == 2)
        {
            ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
            frmNuban = g.GetNubanByListID(ITEMSELECT, req);
        }

        try
        {
            resp = USSD.DoBuySelf(req.Msisdn, Amt, req.Network, req.SessionID, frmNuban);
        }
        catch
        {
            resp = "ERROR: Request string not supported!%0ADial *822*1*NUBAN# to register.";
        }
        return resp;
    }

    //From short string *822*500#
    //IsUserRegis_BuySef(SterlingEngine)
    public string CheckNoofAccts1(UReq req)
    {
        //store the message
        addParam("msg", req.Msg, req);
        //this is to check how many number of accounts exist for the cutomer
        //if 1 then redirect to display summary which is 91130 0
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            req.next_op = 100;
            return "0";
        }
        else
        {
            req.next_op = 102;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb1(UReq req)
    {
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_getAcctList");//Editted to include autoenroll
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            req.next_op = 103;
            return "0";
        }
        else
        {
            return "0";
        }
        return "0";
    }
    public string ListCustomerAccts1(UReq req)
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
            //resp = "Select Biller Item";
            foreach (AccountList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.Nuban;
                }
                else
                {
                    resp += "%0A" + item.TransRate + " " + item.Nuban;
                }
            }
            resp += "%0A99 Next";
        }
        catch
        {
        }
        return "Select Account%0A" + resp;
    }
    public int SaveAcctSelected1(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "99")
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
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
                //string prms = getParams(req);
                //NameValueCollection prm = splitParam(prms);
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }

        string nuban = g.GetNubanByListID(int.Parse(req.Msg), req);
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_CheckCusRegPIN");
        c.AddParam("@mob", req.Msisdn);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                //activated = int.Parse(dr["Activated"].ToString());
                string regisNuban = dr["nuban"].ToString().Trim();
                if (nuban != regisNuban)
                {
                    continue;
                }
                else
                {
                    //Nuban matches one of customer's profile
                    nuban = regisNuban;
                    int activated = int.Parse(dr["Activated"].ToString());
                    if (activated == 0)
                    {
                        //customer doesn't need pin to buy airtime for self                       
                    }
                }
            }
        }
        else
        {
            //customer is okay to continue
        }

        string amount = prm["Amt"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(amount);
        if (nuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = new ImalDetails();
            imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
            cusbal = Convert.ToDecimal(imalinfo.AvailBal);
            if (imalinfo.Status == 0)
            {
                req.op = 4190;
                return 0;
            }
            if (cusbal > amtval)
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
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
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            addParam("NUBAN", nuban, req);
                            cnt = 102;
                            addParam("Bypass", cnt.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 126;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
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
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            cnt = 111;
                            addParam("NoCard", cnt.ToString(), req);
                            req.op = 4290;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
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
                        removeParam("Amt", req);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 1;
                        return 2;
                    }
                    if (airtimeTuple.Item3 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("Amt", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 1;
                        return 2;
                    }
                }
            }
            else
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.op = 1;
                return 2;
            }

        }
        else
        {

            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            DataSet dss = ws.getAccountFullInfo(nuban);
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
                        req.op = 419;
                        return 0;
                    }
                }
                cusbal = decimal.Parse(dr["UsableBal"].ToString());
                if (cusbal > amtval)
                {
                    //proceed
                    var ds1 = getRegisteredProfile(req.Msisdn, nuban);
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
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                addParam("NUBAN", nuban, req);
                                cnt = 102;
                                addParam("Bypass", cnt.ToString(), req);
                                addParam("cnt", cnt.ToString(), req);
                                req.op = 126;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
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
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                cnt = 111;
                                addParam("NoCard", cnt.ToString(), req);
                                req.op = 4290;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
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
                            removeParam("Amt", req);
                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 1;
                            return 2;
                        }
                        if (airtimeTuple.Item3 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            removeParam("Amt", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 1;
                            return 2;
                        }
                    }
                }
                else
                {
                    cnt = 102;
                    removeParam("Amt", req);
                    addParam("cnt", cnt.ToString(), req);
                    req.op = 1;
                    return 2;
                }
            }
            else
            {
                req.op = 4190;
                return 0;
            }
        }

        addParam("ITEMSELECT", req.Msg, req);
        addParam("A", "2", req);
        return 1;
    }

    public string DoBuySelf(UReq req)
    {
        string resp = "";
        try
        {

            string prms = getParams(req); string Reqmsg = "";
            NameValueCollection prm = splitParam(prms);
            Reqmsg = prm["msg"];
            Gadget g = new Gadget(); int ITEMSELECT = 0;
            char[] delm = { '*' }; int flag = 0; string frmNuban = "";
            //string msg = req.Msg.Trim();
            string msg = Reqmsg.Trim();
            string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            string Amt = s[1].Replace("#", string.Empty).TrimEnd();


            flag = int.Parse(prm["A"]);
            if (flag == 1)
            {
                frmNuban = "";
            }
            else if (flag == 2)
            {
                ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
                frmNuban = g.GetNubanByListID(ITEMSELECT, req);
            }

            resp = USSD.DoBuySelf(req.Msisdn, Amt, req.Network, req.SessionID, frmNuban);
        }
        catch (Exception ex)
        {
            resp = "ERROR: We are sorry! Service improvement is currently ongoing";
        }
        return resp;
    }

    //*******************AIRTIME FOR OTHERS******************************
    //Airtime others from menu
    public string CollectAmtOthers(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int amtLimit = RunCount(prm["MaxAmtPerTrans"]);
        if (cnt == 0)
        {
            //check if the customer is registered
            Gadget g = new Gadget();
            int val = g.ConfirmUserStatus(req.Msisdn);
            if (val == 3)
            {
                //first entrance
                removeParam("cnt", req);
                resp = "Enter airtime amount";
                return "Airtime for Beneficiary:%0A" + resp;
            }
            else
            {
                removeParam("cnt", req);
                resp = "Enter your NUBAN";
                return "" + resp;
            }
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 100 naira";
            return "Airtime for Beneficiary:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Airtime for Beneficiary:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            var firstName = "";
            try
            {
                firstName = GetFirstNameByMobileNo(req.Msisdn.Trim());
                resp = "Dear " + firstName + ", you do not have sufficient balance for this transaction. Please credit your account and try again. Thank you.";
                return resp;
            }
            catch
            {
                resp = "You currently do not have sufficient balance for this transaction.";
            }
            return "Airtime for Beneficiary:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry you have exceeded your airtime purchase limit for today.";
            return "Airtime for Beneficiary:%0A" + resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            if (amtLimit == 0)
            {
                resp = "The amount entered is more than maximum amount allowed per transaction";
            }
            else
            {
                resp = "Please enter an amount lower than N" + amtLimit.ToString("#,##");
            }
            return "Airtime for Beneficiary:%0A" + resp;
        }
        resp = "Enter airtime amount";
        return "Airtime for Beneficiary:%0A" + resp;
    }
    public string SaveAirtimeOthersAMt(UReq req)
    {
        int cnt = 0; string resp = "";
        Gadget g = new Gadget();
        string frmNuban = "";
        //if (req.Msg.Length == 10)
        //{
        //    addParam("NUBAN", req.Msg, req);
        //    req.next_op = 115;
        //    return "0";
        //}
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 1;
            return "3";
        }
        if (decimal.Parse(req.Msg) <= 100)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 1;
            return "3";
        }

        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            //Check if customer balance is ok
            int noOfAccts = g.GetNoofAccts(req.Msisdn.Trim());
            if (noOfAccts > 1)
            {
                //check will happen at SaveAcctSelected2
            }
            else
            {
                frmNuban = getDefaultAccount(req.Msisdn.Trim());

                decimal cusbal = 0; decimal amtval = 0;
                amtval = decimal.Parse(req.Msg.Trim());
                if (frmNuban.StartsWith("05"))
                {
                    IMALEngine iMALEngine = new IMALEngine();
                    ImalDetails imalinfo = new ImalDetails();
                    imalinfo = iMALEngine.GetImalDetailsByNuban(frmNuban);
                    cusbal = Convert.ToDecimal(imalinfo.AvailBal);
                    if (imalinfo.Status == 0)
                    {
                        req.next_op = 4190;
                        return "0";
                    }
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
                                    return "0";
                                }
                                else
                                {
                                    cnt = 999;
                                    addParam("cnt", cnt.ToString(), req);
                                    addParam("NUBAN", frmNuban, req);
                                    req.next_op = 124;
                                    return "0";
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
                                    cnt = 111;
                                    addParam("NoCard", cnt.ToString(), req);
                                    req.next_op = 4290;
                                    return "0";
                                }
                                else
                                {
                                    cnt = 999;
                                    addParam("cnt", cnt.ToString(), req);
                                    addParam("NUBAN", frmNuban, req);
                                    req.next_op = 124;
                                    return "0";
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
                                removeParam("AMOUNT", req);
                                addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 1;
                                return "3";
                            }
                            if (airtimeTuple.Item3 < amtval + Totaldone)
                            {
                                //reroute to amount field
                                cnt = 103;
                                removeParam("AMOUNT", req);
                                addParam("cnt", cnt.ToString(), req);
                                req.next_op = 1;
                                return "3";
                            }
                        }
                    }
                    else
                    {
                        cnt = 102;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 1;
                        return "3";
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
                                return "0";
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
                                        return "0";
                                    }
                                    else
                                    {
                                        cnt = 999;
                                        addParam("cnt", cnt.ToString(), req);
                                        addParam("NUBAN", frmNuban, req);
                                        req.next_op = 124;
                                        return "0";
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
                                        cnt = 111;
                                        addParam("NoCard", cnt.ToString(), req);
                                        req.next_op = 4290;
                                        return "0";
                                    }
                                    else
                                    {
                                        cnt = 999;
                                        addParam("cnt", cnt.ToString(), req);
                                        addParam("NUBAN", frmNuban, req);
                                        req.next_op = 124;
                                        return "0";
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
                                    removeParam("AMOUNT", req);
                                    addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                                    addParam("cnt", cnt.ToString(), req);
                                    req.next_op = 1;
                                    return "3";
                                }
                                if (airtimeTuple.Item3 < amtval + Totaldone)
                                {
                                    //reroute to amount field
                                    cnt = 103;
                                    removeParam("AMOUNT", req);
                                    addParam("cnt", cnt.ToString(), req);
                                    req.next_op = 1;
                                    return "3";
                                }
                            }

                        }
                        else
                        {
                            cnt = 102;
                            //removeParam("Amt", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.next_op = 1;
                            return "3";
                        }
                    }
                    else
                    {
                        req.next_op = 4190;
                        return "0";
                    }
                }
            }

        }
        else
        {
            //redirect to register and set PIN
            req.next_op = 122;
            return "0";
        }

        //save it
        try
        {
            addParam("AMOUNT", req.Msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string CheckNoofAccts2(UReq req)
    {
        //this is to check how many number of accounts exist for the cutomer
        //if 1 then redirect to display summary which is 91130 0
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            req.next_op = 6601;
            return "0";
        }
        else
        {
            req.next_op = 6401;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb2(UReq req)
    {
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_getAcctList");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            req.next_op = 6402;
            return "0";
        }
        else
        {
            return "0";
        }
        return "0";
    }
    public string ListCustomerAccts2(UReq req)
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
            //resp = "Select Biller Item";
            foreach (AccountList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.Nuban;
                }
                else
                {
                    resp += "%0A" + item.TransRate + " " + item.Nuban;
                }
            }
            resp += "%0A99 Next";
        }
        catch
        {
        }
        return "Select Account%0A" + resp;
    }
    public int SaveAcctSelected2(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "99")
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
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
                //string prms = getParams(req);
                //NameValueCollection prm = splitParam(prms);
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }

        string nuban = g.GetNubanByListID(int.Parse(req.Msg), req);
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
                        cnt = 102;
                        addParam("NUBAN", nuban, req);
                        addParam("Bypass", cnt.ToString(), req);
                        req.op = 126;
                        return 0;
                    }
                    else
                    {
                        addParam("NUBAN", nuban, req);
                        req.op = 124;
                        return 0;
                    }
                }

                //    if (activated == 0)
                //    {
                //        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                //        string response = cardws.GetActiveCards(nuban);
                //        if (response.StartsWith("00"))
                //        {
                //            addParam("NUBAN", nuban, req);
                //            cnt = 102;
                //            addParam("Bypass", cnt.ToString(), req);
                //            req.op = 126;
                //            return 0;
                //        }
                //        else
                //        {
                //            addParam("NUBAN", nuban, req);
                //            req.op = 124;
                //            return 0;
                //        }
                //    }
                //}
            }
        }
        else
        {
            //customer is okay to continue
        }
        string amount = prm["AMOUNT"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(amount);
        if (nuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = new ImalDetails();
            imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
            cusbal = Convert.ToDecimal(imalinfo.AvailBal);
            if (imalinfo.Status == 0)
            {
                req.op = 4190;
                return 0;
            }
            if (cusbal > amtval)
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
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
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            addParam("NUBAN", nuban, req);
                            cnt = 102;
                            addParam("Bypass", cnt.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 126;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
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
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            cnt = 111;
                            addParam("NoCard", cnt.ToString(), req);
                            req.op = 4290;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
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
                        removeParam("AMOUNT", req);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 1;
                        return 3;
                    }
                    if (airtimeTuple.Item3 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("AMOUNT", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 1;
                        return 3;
                    }
                }
            }
            else
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.op = 1;
                return 3;
            }

        }
        else
        {

            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            DataSet dss = ws.getAccountFullInfo(nuban);
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
                        req.op = 419;
                        return 0;
                    }
                }
                cusbal = decimal.Parse(dr["UsableBal"].ToString());
                if (cusbal > amtval)
                {
                    //proceed
                    var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                    DataRow dr1 = ds1.Tables[0].Rows[0];
                    int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                    int activated = int.Parse(dr1["Activated"].ToString());
                    int enrollType = int.Parse(dr1["EnrollType"].ToString());
                    if (enrollType == 1 && activated == 0)
                    {
                        //700 for auto enrolled account
                        //Minimum ,Maximum per trns, Max per day
                        Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(700);
                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                        if (airtimeTuple.Item3 < amtval + Totaldone)
                        {
                            CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                addParam("NUBAN", nuban, req);
                                cnt = 102;
                                addParam("Bypass", cnt.ToString(), req);
                                addParam("cnt", cnt.ToString(), req);
                                req.op = 126;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
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
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                cnt = 111;
                                addParam("NoCard", cnt.ToString(), req);
                                req.op = 4290;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
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
                            removeParam("AMOUNT", req);
                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 1;
                            return 3;
                        }
                        if (airtimeTuple.Item3 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            removeParam("AMOUNT", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 1;
                            return 3;
                        }
                    }
                }
                else
                {
                    cnt = 102;
                    removeParam("AMOUNT", req);
                    addParam("cnt", cnt.ToString(), req);
                    req.op = 1;
                    return 3;
                }
            }
            else
            {
                req.op = 4190;
                return 0;
            }
        }

        addParam("ITEMSELECT", req.Msg, req);
        addParam("A", "2", req);
        return 1;
    }
    public string CollectBeneMobile(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter beneficiary mobile number";
            return "Airtime Beneficiary Mobile:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the beneficiary mobile number is not less or more than 11 digits";
            return "Airtime Beneficiary Mobile:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the beneficiary mobile number is not alphanumeric";
            return "Airtime Beneficiary Mobile:%0A" + resp;
        }
        resp = "Enter beneficiary mobile number";
        return "Airtime Beneficiary Mobile:%0A" + resp;
    }
    public string SaveBeneMobile(UReq req)
    {
        int cnt = 0; string resp = "";
        //check if entry is numeric
        try
        {
            long mobile = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 6602;
            return "9";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 11 || req.Msg.Length > 11)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 6602;
            return "9";
        }
        //save it
        try
        {
            addParam("MOBILE", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

    //From short string *822*100*MobileNo#
    //IsUserRegis_BuyOthers(SterlingEngine) -  SaveAccountMobile(BaseEngine)
    public string CheckNoofAccts3(UReq req)
    {
        //this is to check how many number of accounts exist for the cutomer
        //if 1 then redirect to display summary which is 91130 0
        Gadget g = new Gadget();
        int cnt = g.GetNoofAccts(req.Msisdn);
        if (cnt == 1)
        {
            //redirect to the DisplaySummary screen
            addParam("A", "1", req);
            req.next_op = 113;
            return "0";
        }
        else
        {
            req.next_op = 141;
            return "0";
        }
        return "";
    }
    public string FetchAcctsIntodb3(UReq req)
    {
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_getAcctList");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            req.next_op = 142;
            return "0";
        }
        else
        {
            return "0";
        }
        return "0";
    }
    public string ListCustomerAccts3(UReq req)
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
            //resp = "Select Biller Item";
            foreach (AccountList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.Nuban;
                }
                else
                {
                    resp += "%0A" + item.TransRate + " " + item.Nuban;
                }
            }
            resp += "%0A99 Next";
        }
        catch
        {
        }
        return "Select Account%0A" + resp;
    }
    public int SaveAcctSelected3(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "99")
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
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
                //string prms = getParams(req);
                //NameValueCollection prm = splitParam(prms);
                int page = g.RunBillerNextPage(prm["NXTPAG"]);
                page++;
                removeParam("NXTPAG", req);
                addParam("NXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            //string prms = getParams(req);
            //NameValueCollection prm = splitParam(prms);
            int page = g.RunBillerNextPage(prm["NXTPAG"]);
            page++;
            removeParam("NXTPAG", req);
            addParam("NXTPAG", page.ToString(), req);
            return 99;
        }

        string nuban = g.GetNubanByListID(int.Parse(req.Msg), req);
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
                        cnt = 102;
                        addParam("NUBAN", nuban, req);
                        addParam("Bypass", cnt.ToString(), req);
                        req.op = 126;
                        return 0;
                    }
                    else
                    {
                        addParam("NUBAN", nuban, req);
                        req.op = 124;
                        return 0;
                    }
                }
                //    if (activated == 0)
                //    {
                //        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                //        string response = cardws.GetActiveCards(nuban);
                //        if (response.StartsWith("00"))
                //        {
                //            addParam("NUBAN", nuban, req);
                //            cnt = 102;
                //            addParam("Bypass", cnt.ToString(), req);
                //            req.op = 126;
                //            return 0;
                //        }
                //        else
                //        {
                //            addParam("NUBAN", nuban, req);
                //            req.op = 124;
                //            return 0;
                //        }
                //    }
                //}
            }
        }
        else
        {
            //customer is okay to continue
        }
        string amount = prm["AMOUNT"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(amount);
        if (nuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = new ImalDetails();
            imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
            cusbal = Convert.ToDecimal(imalinfo.AvailBal);
            if (imalinfo.Status == 0)
            {
                req.op = 4190;
                return 0;
            }
            if (cusbal > amtval)
            {
                //proceed
                var ds1 = getRegisteredProfile(req.Msisdn, nuban);
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
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            addParam("NUBAN", nuban, req);
                            cnt = 102;
                            addParam("Bypass", cnt.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 126;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
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
                        string response = cardws.GetActiveCards(nuban);
                        if (response.StartsWith("00"))
                        {
                            cnt = 111;
                            addParam("NoCard", cnt.ToString(), req);
                            req.op = 4290;
                            return 0;
                        }
                        else
                        {
                            cnt = 999;
                            addParam("cnt", cnt.ToString(), req);
                            addParam("NUBAN", nuban, req);
                            req.op = 124;
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
                        removeParam("AMOUNT", req);
                        addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 1;
                        return 3;
                    }
                    if (airtimeTuple.Item3 < amtval + Totaldone)
                    {
                        //reroute to amount field
                        cnt = 103;
                        removeParam("AMOUNT", req);
                        addParam("cnt", cnt.ToString(), req);
                        req.op = 1;
                        return 3;
                    }
                }
            }
            else
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.op = 1;
                return 3;
            }

        }
        else
        {

            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            DataSet dss = ws.getAccountFullInfo(nuban);
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
                        req.op = 419;
                        return 0;
                    }
                }
                cusbal = decimal.Parse(dr["UsableBal"].ToString());
                if (cusbal > amtval)
                {
                    //proceed
                    var ds1 = getRegisteredProfile(req.Msisdn, nuban);
                    DataRow dr1 = ds1.Tables[0].Rows[0];
                    int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
                    int activated = int.Parse(dr1["Activated"].ToString());
                    int enrollType = int.Parse(dr1["EnrollType"].ToString());
                    if (enrollType == 1 && activated == 0)
                    {
                        //700 for auto enrolled account
                        //Minimum ,Maximum per trns, Max per day
                        Tuple<decimal, decimal, decimal> airtimeTuple = getConfigAccount(700);
                        getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, airtimeTuple.Item3);
                        if (airtimeTuple.Item3 < amtval + Totaldone)
                        {
                            CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                addParam("NUBAN", nuban, req);
                                cnt = 102;
                                addParam("Bypass", cnt.ToString(), req);
                                addParam("cnt", cnt.ToString(), req);
                                req.op = 126;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
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
                            string response = cardws.GetActiveCards(nuban);
                            if (response.StartsWith("00"))
                            {
                                cnt = 111;
                                addParam("NoCard", cnt.ToString(), req);
                                req.op = 4290;
                                return 0;
                            }
                            else
                            {
                                cnt = 999;
                                addParam("cnt", cnt.ToString(), req);
                                addParam("NUBAN", nuban, req);
                                req.op = 124;
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
                            removeParam("AMOUNT", req);
                            addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 1;
                            return 3;
                        }
                        if (airtimeTuple.Item3 < amtval + Totaldone)
                        {
                            //reroute to amount field
                            cnt = 103;
                            removeParam("AMOUNT", req);
                            addParam("cnt", cnt.ToString(), req);
                            req.op = 1;
                            return 3;
                        }
                    }
                }
                else
                {
                    cnt = 102;
                    removeParam("AMOUNT", req);
                    addParam("cnt", cnt.ToString(), req);
                    req.op = 1;
                    return 3;
                }
            }
            else
            {
                req.op = 4190;
                return 0;
            }
        }

        addParam("ITEMSELECT", req.Msg, req);
        addParam("A", "2", req);
        return 1;
    }

    //---Used by both routes
    public string CollectUSSDPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter your USSD Authentication PIN";
            return "USSD Authentication PIN:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the PIN is not less than or more than 4 digits";
            return "USSD Authentication PIN:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the PIN is not alphanumeric";
            return "USSD Authentication PIN:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Kindly enter your correct USSD PIN";
            return "USSD Authentication PIN:%0A" + resp;
        }
        //if (wrongCnt == 3)
        //{
        //    removeParam("wrongCnt", req);
        //    resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your registration. Thank you";
        //    return resp;
        //}
        resp = "Enter your USSD Authentication PIN";
        return "USSD Authentication PIN:%0A" + resp;
    }
    public string SaveUSSDPIN(UReq req)
    {
        int cnt = 0; string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);

        //check if entry is numeric
        try
        {
            long mobile = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 113;
            return "0";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 113;
            return "0";
        }
        if (req.Msg.Length == 4)
        {
            Encrypto EnDeP = new Encrypto();
            req.Msg = EnDeP.Encrypt(req.Msg);

            string sql = "select custauthid from Go_Registered_Account where Mobile =@mb and custauthid =@pin";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetSQL(sql);
            c.AddParam("@mb", req.Msisdn);
            c.AddParam("@pin", req.Msg.Trim());
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {

            }
            else
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                wrongCnt++;
                removeParam("wrongCnt", req);
                addParam("wrongCnt", wrongCnt.ToString(), req);
                req.next_op = 113;
                return "0";
            }
        }
        //save it
        try
        {
            addParam("PIN", req.Msg, req);

        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string DoBuyOther(UReq req)
    {
        string resp = ""; Gadget g = new Gadget();
        try
        {
            int flag = 0; string frmNuban = ""; int ITEMSELECT = 0;
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            flag = int.Parse(prm["A"]);
            if (flag == 1)
            {
                frmNuban = "";
            }
            else if (flag == 2)
            {
                ITEMSELECT = int.Parse(prm["ITEMSELECT"]);
                frmNuban = g.GetNubanByListID(ITEMSELECT, req);
            }

            resp = USSD.Buy_Friend(prm["MOBILE"], req.Msisdn, prm["AMOUNT"], prm["NETWORK"], req.SessionID, prm["PIN"], frmNuban);
        }
        catch (Exception ex)
        {
            resp = "ERROR: We are sorry! Service improvement is currently ongoing";
        }
        return resp;
    }

    //**************************************************************************







    /////////////////////////////////////////////////////////////
    public string RestrictCode(UReq req)
    {
        string resp = "";
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        cnt = RunCount(prm["RestFlag"]);
        var firstName = "";
        try
        {
            if (cnt == 0)
            {
                resp = "Sorry, this account cannot be registered on USSD.";
                return resp;
            }

            firstName = GetFirstNameByMobileNo(req.Msisdn.Trim());
            resp = "Dear " + firstName + ", this transaction cannot be completed, your account is restricted. Please visit any of our branches close to you or call 070078345464.";
        }
        catch (Exception)
        {
            resp = "There appears to be a restriction on your account. Kindly visit your nearest Sterling branch or call +23470078375464.";
        }
        return resp;
    }
    public string InvalidAcct(UReq req)
    {
        string resp = "";
        resp = "The account number provided isn't a Sterling account. Please try again with a valid account number.";
        return resp;
    }
    public string NoUpgradeAvailable(UReq req)
    {
        //string resp = "Kindly request for a debit card for this account to upgrade your limit.%0ATo request for an Instant Card, please call +23470078375464 or visit a Branch close to you.";
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        cnt = RunCount(prm["NoCard"]);
        string resp = "";
        if (cnt == 0)
        {
            resp = "Dear customer, you have exceeded your limit on USSD.%0AKindly call 070078375464 or visit a branch close to you to request for a debit card for this account to upgrade your limit.";

        }
        else
        {
            resp = "Kindly request for a debit card for this account to upgrade your limit.%0ATo request for an Instant Card, please call +23470078375464 or visit a Branch close to you.";
        }
        return resp;
    }
    public string WrongInput(UReq req)
    {
        string resp = "";
        resp = "Invalid input. Kindly restart your request.";
        return resp;
    }
    public string AlreadyReg(UReq req)
    {
        string resp = "You have already been activated for this service%0ADial *822*Amount# to recharge your phone.%0Ae.g *822*500#";

        return resp;
    }
    //*******************************************************
    public string SaveBankAcct(UReq req)
    {
        string resp = "0";
        try
        {
            addParam("NUBAN", req.Msg, req);
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string SaveRequestNew(UReq req)
    {

        string resp = "0";
        try
        {
            EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
            //check if you are registered and have pin set
            string sql = ""; int activated = -1; string nuban = "";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", req.Msisdn.Trim());
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                activated = int.Parse(dr["Activated"].ToString());
                nuban = dr["nuban"].ToString();
                if (activated == 1)
                {
                    req.next_op = 4040;
                    return "0";
                }
                else
                {
                    addParam("NUBAN", nuban, req);
                }
            }
            else
            {
                //redirect to register and set PIN
                req.next_op = 122;
                return "0";
            }

            //if (activated == 1)
            //{
            //    char[] delm = { '*', '#' };
            //    string msg = req.Msg.Trim();
            //    string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);
            //    if (nuban == s[3].Trim())
            //    {
            //        req.next_op = 122;
            //        return "9";
            //    }
            //    else
            //    {
            //        addParam("AMOUNT", s[2], req);
            //        addParam("TONUBAN", s[3], req);
            //    }
            //}
            //else if (activated == 0)
            //{
            //    //redirect to set PIN
            //    req.next_op = 115;
            //    return "0";
            //}
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }

}
