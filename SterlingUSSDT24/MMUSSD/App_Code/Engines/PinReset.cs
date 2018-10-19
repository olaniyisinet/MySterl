using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for PinReset
/// </summary>
public class PinReset : BaseEngine
{
    public PinReset()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string CollectNUBAN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
            return resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your account number:";
            return "Collect NUBAN:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 10 digits";
            return "Collect NUBAN:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 10 digits and not alphanumeric";
            return "Collect NUBAN:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter the account number you registered for USSD service";
            return "Collect NUBAN:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            //resp = "You need an active Card to complete your PIN reset request. To request for an Instant Card, please call +23470078375464 or visit a Branch close to you.";
            //resp = "To complete your PIN reset, kindly go to the nearest branch to request for a debit card.%0AThank you";
            resp = "Your phone number and account number do not match. Kindly visit the nearest Sterling branch to update your details";
            return resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            // resp = "The account number you entered is incorrect";
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

        resp = "Kindly enter your account number";
        return "Collect NUBAN:%0A" + resp;
    }
    public string SaveNuban(UReq req)
    {
        int cnt = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        IMALEngine iMALEngine = new IMALEngine();
        string nuban = req.Msg.Trim();
        //check if entry is numeric
        try
        {
            long acctNo = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7001;
            return "9";
        }

        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 10 || req.Msg.Length > 10)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7001;
            return "9";
        }

        //check if the account number is profiled
        string sql = ""; string resp = "0"; string nubanRs = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                //activated = int.Parse(dr["Activated"].ToString());
                string regisNuban = dr["nuban"].ToString().Trim();
                if (req.Msg.Trim() != regisNuban)
                {
                    cnt = -1;
                    continue;
                }
                else
                {
                    //Nuban matches one of customer's profile
                    //cnt = 111;
                    addParam("NUBAN", req.Msg.Trim(), req);
                    break;
                    //return "0";
                }
            }

            if (cnt == -1)
            {
                //cnt = -1;
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                wrongCnt++;
                removeParam("wrongCnt", req);
                addParam("wrongCnt", wrongCnt.ToString(), req);
                req.next_op = 7001;
                return "9";
            }

        }
        else
        {
            //redirect to account not found
            cnt = -2;
            addParam("status", cnt.ToString(), req);
            req.next_op = 7009;
            return "0";
        }

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

            //Continue
        }
        else if (nuban.StartsWith("11"))
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
            if (bank1.status != "True")
            {
                cnt = 104;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 7000;
                return "0";
            }
            bank1.phoneNumber = ConvertMobile234(bank1.phoneNumber);
            if (bank1.phoneNumber != req.Msisdn.Replace("234", "0"))
            {
                cnt = 105;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 7000;
                return "0";
            }

            return "0";
        }

        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
        string response = cardws.GetActiveCards(nuban);
        if (response.StartsWith("00"))
        {
            //this person doesn't have an active card.
            cnt = 111;
            addParam("Bypass", cnt.ToString(), req);
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 7002;
            return "0";
        }


        return resp;
    }

    public string DoPINReset(UReq req)
    {
        string resp = ""; int status = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        int bypass = RunCount(prm["Bypass"]);
        string smsMessage = "";
        int k = 0;
        try
        {
            if (prms.Contains("status"))
            {
                status = int.Parse(prm["status"]);
                if (status == -1)
                {
                    resp = "Sorry, the account number entered does not exist in our system.  Kindly ensure you enter the correct account number";
                    return resp;
                }
                else if (status == -2)
                {
                    //resp = "Sorry, the BVN entered does not exist.  Kindly ensure you enter the correct BVN";
                    resp = "Sorry, this phone number is not profiled on this service.";
                    return resp;
                }
                else if (status == -3)
                {
                    resp = "Oops! The BVN you have entered does not match what we have in our records. Please enter the correct BVN and try again.";
                    return resp;
                }
            }
            else
            {
                //proceed to reset the PIN for the customer
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
                c.SetProcedure("spd_USSD_PINRESET");
                c.AddParam("@mobile", req.Msisdn);
                c.AddParam("@custauthid", req.Msg);
                c.AddParam("@nuban", nuban);
                int cn = c.Update();
                if (cn > 0)
                {
                    //EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
                    //var ds = ws.getAccountFullInfo(nuban);
                    //DataRow dr = ds.Tables[0].Rows[0];
                    //string custName = dr["NAME_LINE1"].ToString();
                    if (bypass == 111)
                    {
                        //string maskedMobNo = MaskDetails(req.Msisdn, 9, 4, 2);
                        //smsMessage = "Dear " + custName + ", your PIN reset on " + maskedMobNo + " was successful" + Environment.NewLine + "Not you? Please call +23470078375464 or send an email to customercare@sterlingbankg.com";
                        // resp = "Great! PIN reset completed. Enjoy magic banking with *822#”";
                        smsMessage = "Hello, your PIN reset was successful! If this isn't you, please contact our one customer centre on 070078375464 immediately ";
                        //SendSms(smsMessage, req.Msisdn);
                        resp = "Great! PIN reset successful. Continue to enjoy the Magic of *822#";
                    }
                    else
                    {
                        //smsMessage = "Dear " + custName + ", your PIN reset was successful." + Environment.NewLine + "Not you? Please contact our one customer centre on +23470078375464  or via customercare@sterlingbankng.com immediately";
                        //resp = "Great! Your PIN reset was successful. Continue to enjoy the benefits of *822#";
                        smsMessage = "Hello, your PIN reset was successful! If this isn't you, please contact our one customer centre on 070078375464 immediately";
                        //SendSms(smsMessage, req.Msisdn);
                        resp = "Great! Your PIN reset was successful. Continue to enjoy the Magic of *822#";
                    }

                    return resp;
                }
                else
                {
                    resp = "PIN Reset was not successful. Kindly try again later";
                    return resp;
                }
            }
        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system";
        }
        return resp;
    }

    public string CollectBVN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
            return resp;
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
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry, an error occurred treating your request";
            return "Bank Verification Number: " + resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            resp = "This phone number doesn't match your BVN profile";
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
        int wrongCnt = RunCount(prm["wrongCnt"]);

        string nuban = prm["NUBAN"];

        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 11 || req.Msg.Length > 11)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7003;
            return "9";
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
            req.next_op = 7003;
            return "9";
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
        string bvn = prm["BVN"];
        bvn = removeSpareParams(bvn);
        nuban = removeSpareParams(nuban);
        string bvnRs = ""; string PhoneNumber = "";
        EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        //ValBVN.ServiceSoapClient ws = new ValBVN.ServiceSoapClient();
        //DataSet ds = ws.GetBVN(bvn);

        try
        {
            if (nuban.StartsWith("05"))
            {
                IMALEngine iMALEngine = new IMALEngine();
                ImalDetails imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
                if (imalinfo.BVN != bvn)
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 7002;
                    return "0";
                }

                return resp;
            }
            else if (nuban.StartsWith("11"))
            {
                //Treat as BankOne
                ValBVN.ServiceSoapClient wsOne = new ValBVN.ServiceSoapClient();
                var dsOne = wsOne.GetBVN(bvn);
                if (dsOne.Tables[0].Rows.Count > 0)
                {
                    var phone = dsOne.Tables[0].Rows[0]["PhoneNumber"].ToString();
                    if (ConvertMobile234(phone) != req.Msisdn)
                    {
                        cnt = 104;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 7002;
                        return "0";
                    }
                }
                else
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 7002;
                    return "0";
                }

                return "0";
            }

            DataSet ds = ws.getAccountFullInfo(nuban);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                bvnRs = dr["BVN"].ToString();
                PhoneNumber = dr["MOB_NUM"].ToString();

                if (bvnRs == bvn && PhoneNumber == req.Msisdn.Replace("234", "0"))
                {
                    cnt = RunCount(prm["Bypass"]);
                    if (cnt == 111)
                    {
                        //Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
                        //c.SetProcedure("spd_CheckCusRegPIN");
                        //c.AddParam("@mob", req.Msisdn);
                        //DataSet dss = c.Select("rec");
                        //if (dss.Tables[0].Rows.Count > 0)
                        //{
                        //    req.next_op = 7012;
                        //}
                        //else
                        //{
                        //    req.next_op = 115;
                        //    return "0";
                        //}
                        req.next_op = 7005;//Maybe Direct to enter current pin?? 
                        return "0";
                    }
                }
                else
                {
                    cnt = -3;
                    addParam("status", cnt.ToString(), req);
                    req.next_op = 7009;
                    return "0";
                }
            }
            else
            {
                cnt = -1;
                addParam("status", cnt.ToString(), req);
                req.next_op = 7009;
                return "0";
            }
        }
        catch (Exception)
        {
            cnt = 103;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 7002;
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
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
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
            req.next_op = 7010;
            return "0";
        }
        if (req.Msg.Length < 6 || req.Msg.Length > 6)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7010;
            return "0";
        }

        //Check if BankOne Customer
        if (nuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = nuban;
            string bankOneReq = bank1.createReqForCardValidation();
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.CardIssuanceCardValidation(encrptdReq);
            bool isBankOne = bank1.readRespForCardValidation(getBankOneResp);
            if (bank1.status != "True")
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 7010;
                return "0";
            }
            return "0";
        }

        nuban = removeSpareParams(nuban);

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
            req.next_op = 7010;
            return "0";
        }

        return resp;
    }

    public string SetUserPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
            return resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter any 4 digit of your choice";
            return "PIN Reset: " + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of the PIN is not less than or greater than 4 digits";
            return "PIN Reset: " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 numeric digits and not alphanumeric";
            return "PIN Reset: " + resp;
        }

        resp = "Kindly enter any 4 digits of your choice";
        return "PIN Reset: " + resp;
    }
    public string SaveCusPIN(UReq req)
    {
        int cnt = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        string nuban = prm["NUBAN"];
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7006;
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
            req.next_op = 7006;
            return "9";
        }

        //Check to ensure PIN 1 is different from PIN 2
        string pin1 = req.Msg;
        string PIN2 = getUSSDPin2(req.Msisdn, nuban);
        if (pin1 == PIN2)
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 7006;
            return "9";
        }

        string resp = "0";
        try
        {
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
    public string ConfirmSetUserPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
            return resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter any 4 digit of your choice";
            return "Confirm PIN Reset: " + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of the PIN is not less than or greater than 4 digits";
            return "Confirm PIN Reset: " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 numeric digits and not alphanumeric";
            return "Confirm PIN Reset: " + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The confirmation PIN did not match the intial PIN you entered.  Kindly ensure they are the same";
            return "Confirm PIN Reset: " + resp;
        }
        return "";
    }
    public string SaveConfrimPIN(UReq req)
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
            req.next_op = 7007;
            return "0";
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
            req.next_op = 7007;
            return "0";
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
            req.next_op = 7007;
            return "0";
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

    //***********************Flow for Resetting PIN 2**************************************
    //

    public string CollectNUBAN2ndPin(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
            return resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your account number:";
            return "Collect NUBAN for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 10 digits";
            return "Collect NUBAN for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 10 digits and not alphanumeric";
            return "Collect NUBAN for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter the account number you registered for USSD service";
            return "Collect NUBAN for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            //resp = "You need an active Card to complete your PIN reset request. To request for an Instant Card, please call +23470078375464 or visit a Branch close to you.";
            //resp = "To complete your PIN reset, kindly go to the nearest branch to request for a debit card.%0AThank you";
            resp = "Your phone number and account number do not match. Kindly visit the nearest Sterling branch to update your details";
            return resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            // resp = "The account number you entered is incorrect";
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

        resp = "Kindly enter your account number";
        return "Collect NUBAN for PIN 2 reset:%0A" + resp;
    }
    public string SaveNuban2ndPin(UReq req)
    {
        int cnt = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        IMALEngine iMALEngine = new IMALEngine();
        string nuban = req.Msg.Trim();
        //check if entry is numeric
        try
        {
            long acctNo = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7021;
            return "9";
        }

        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 10 || req.Msg.Length > 10)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7021;
            return "9";
        }

        //check if the account number is profiled
        string sql = ""; string resp = "0"; string nubanRs = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                //activated = int.Parse(dr["Activated"].ToString());
                string regisNuban = dr["nuban"].ToString().Trim();
                if (req.Msg.Trim() != regisNuban)
                {
                    cnt = -1;
                    continue;
                }
                else
                {
                    //Nuban matches one of customer's profile
                    //cnt = 111;
                    cnt = 0;
                    addParam("NUBAN", req.Msg.Trim(), req);
                    break;
                    //return "0";
                }
            }

            if (cnt == -1)
            {
                //cnt = -1;
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                wrongCnt++;
                removeParam("wrongCnt", req);
                addParam("wrongCnt", wrongCnt.ToString(), req);
                req.next_op = 7021;
                return "9";
            }

        }
        else
        {
            //redirect to account not found
            cnt = -2;
            addParam("status", cnt.ToString(), req);
            req.next_op = 7029;
            return "0";
        }

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

            //Continue
        }
        else if (nuban.StartsWith("11"))
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
            if (bank1.status != "True")
            {
                cnt = 104;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 7020;
                return "0";
            }
            bank1.phoneNumber = ConvertMobile234(bank1.phoneNumber);
            if (bank1.phoneNumber != req.Msisdn.Replace("234", "0"))
            {
                cnt = 105;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 7020;
                return "0";
            }

            return "0";
        }

        CardPinService.CardsSoapClient cardws = new CardPinService.CardsSoapClient();
        string response = cardws.GetActiveCards(nuban);
        if (response.StartsWith("00"))
        {
            //this person doesn't have an active card.
            cnt = 111;
            addParam("Bypass", cnt.ToString(), req);
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 7022;
            return "0";
        }


        return resp;
    }

    public string DoPINReset2ndPin(UReq req)
    {
        string resp = ""; int status = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        int bypass = RunCount(prm["Bypass"]);
        string smsMessage = "";
        int k = 0;
        try
        {
            if (prms.Contains("status"))
            {
                status = int.Parse(prm["status"]);
                if (status == -1)
                {
                    resp = "Sorry, the account number entered does not exist in our system.  Kindly ensure you enter the correct account number";
                    return resp;
                }
                else if (status == -2)
                {
                    //resp = "Sorry, the BVN entered does not exist.  Kindly ensure you enter the correct BVN";
                    resp = "Sorry, this phone number is not profiled on this service.";
                    return resp;
                }
                else if (status == -3)
                {
                    resp = "Oops! The BVN you have entered does not match what we have in our records. Please enter the correct BVN and try again.";
                    return resp;
                }
            }
            else
            {
                //proceed to reset the PIN for the customer
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
                c.SetProcedure("spd_SetCustPin2");//2ndPin
                c.AddParam("@mobile", req.Msisdn);
                c.AddParam("@custauthid", req.Msg);
                c.AddParam("@nuban", nuban);
                int cn = c.Update();
                if (cn > 0)
                {
                    
                    if (bypass == 111)
                    {
                        //string maskedMobNo = MaskDetails(req.Msisdn, 9, 4, 2);
                        //smsMessage = "Dear " + custName + ", your PIN reset on " + maskedMobNo + " was successful" + Environment.NewLine + "Not you? Please call +23470078375464 or send an email to customercare@sterlingbankg.com";
                        // resp = "Great! PIN reset completed. Enjoy magic banking with *822#”";
                        smsMessage = "Hello, your PIN 2 reset was successful! If this isn't you, please contact our one customer centre on 070078375464 immediately ";
                        //SendSms(smsMessage, req.Msisdn);
                        resp = "Great! PIN 2 reset successful. Continue to enjoy the Magic of *822#";
                    }
                    else
                    {
                        //smsMessage = "Dear " + custName + ", your PIN reset was successful." + Environment.NewLine + "Not you? Please contact our one customer centre on +23470078375464  or via customercare@sterlingbankng.com immediately";
                        //resp = "Great! Your PIN reset was successful. Continue to enjoy the benefits of *822#";
                        smsMessage = "Hello, your PIN 2 reset was successful! If this isn't you, please contact our one customer centre on 070078375464 immediately";
                        //SendSms(smsMessage, req.Msisdn);
                        resp = "Great! Your PIN 2 reset was successful. Continue to enjoy the Magic of *822#";
                    }

                    return resp;
                }
                else
                {
                    resp = "PIN 2 Reset was not successful. Kindly try again later";
                    return resp;
                }
            }
        }
        catch (Exception ex)
        {
            resp = "ERROR: Could not contact core banking system";
        }
        return resp;
    }

    public string CollectBVN2ndPin(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
            return resp;
        }
        if (cnt == 0 || cnt == 111)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your BVN";
            return "Bank Verification Number for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of BVN is not less than or greater than 11 digits";
            return "Bank Verification Number for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 11 digits and not alphanumeric";
            return "Bank Verification Number for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Oops! The BVN you have entered does not match what we have in our records. Please enter the correct BVN and try again.";
            return "Bank Verification Number for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry, an error occurred treating your request";
            return "Bank Verification Number for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            resp = "This phone number doesn't match your BVN profile";
            return "Bank Verification Number for PIN 2 reset:%0A" + resp;
        }
        resp = "Kindly enter your BVN";
        return "Bank Verification Number for PIN 2 reset:%0A" + resp;
    }
    public string SaveBVN2ndPin(UReq req)
    {
        string prms = getParams(req);
        int cnt = 0;
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);

        string nuban = prm["NUBAN"];

        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length < 11 || req.Msg.Length > 11)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7023;
            return "9";
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
            req.next_op = 7023;
            return "9";
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
    public string ValidateBVN2ndPin(UReq req)
    {
        string resp = "0"; string prms = getParams(req); int cnt = 0;
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        string bvn = prm["BVN"]; string bvnRs = ""; string PhoneNumber = "";
        EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        //ValBVN.ServiceSoapClient ws = new ValBVN.ServiceSoapClient();
        //DataSet ds = ws.GetBVN(bvn);

        try
        {
            if (nuban.StartsWith("05"))
            {
                IMALEngine iMALEngine = new IMALEngine();
                ImalDetails imalinfo = iMALEngine.GetImalDetailsByNuban(nuban);
                if (imalinfo.BVN != bvn)
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 7022;
                    return "0";
                }

                return resp;
            }
            else if (nuban.StartsWith("11"))
            {
                //Treat as BankOne
                ValBVN.ServiceSoapClient wsOne = new ValBVN.ServiceSoapClient();
                var dsOne = wsOne.GetBVN(bvn);
                if (dsOne.Tables[0].Rows.Count > 0)
                {
                    var phone = dsOne.Tables[0].Rows[0]["PhoneNumber"].ToString();
                    if (ConvertMobile234(phone) != req.Msisdn)
                    {
                        cnt = 104;
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 7022;
                        return "0";
                    }
                }
                else
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 7022;
                    return "0";
                }

                return "0";
            }

            nuban = removeSpareParams(nuban);

            DataSet ds = ws.getAccountFullInfo(nuban);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                bvnRs = dr["BVN"].ToString();
                PhoneNumber = dr["MOB_NUM"].ToString();

                if (bvnRs == bvn && PhoneNumber == ConvertMobile080(req.Msisdn))
                {
                    cnt = RunCount(prm["Bypass"]);
                    if (cnt == 111)
                    {
                        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
                        c.SetProcedure("spd_CheckCusRegPIN");
                        c.AddParam("@mob", req.Msisdn);
                        DataSet dss = c.Select("rec");
                        if (dss.Tables[0].Rows.Count > 0)
                        {
                            req.next_op = 7032;
                            return "0";

                        }
                        else
                        {
                            req.next_op = 115;
                            return "0";
                        }
                        req.next_op = 7025;//Maybe Direct to enter current pin?? 
                        return "0";
                    }
                }
                else
                {
                    cnt = -3;
                    addParam("status", cnt.ToString(), req);
                    req.next_op = 7029;
                    return "0";
                }
            }
            else
            {
                cnt = -1;
                addParam("status", cnt.ToString(), req);
                req.next_op = 7029;
                return "0";
            }
        }
        catch (Exception)
        {
            cnt = 103;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 7022;
            return "0";
        }

        return resp;
    }

    public string CollectLast6Digits2ndPin(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
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

        resp = "Enter last 6 digits of your card:";
        return resp;
    }
    public string SaveLastSixDigits2ndPin(UReq req)
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
            req.next_op = 7030;
            return "0";
        }
        if (req.Msg.Length < 6 || req.Msg.Length > 6)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7030;
            return "0";
        }

        //Check if BankOne Customer
        if (nuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = nuban;
            string bankOneReq = bank1.createReqForCardValidation();
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.CardIssuanceCardValidation(encrptdReq);
            bool isBankOne = bank1.readRespForCardValidation(getBankOneResp);
            if (bank1.status != "True")
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 7030;
                return "0";
            }
            return "0";
        }

        nuban = removeSpareParams(nuban);

        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_PostCard");
        c.SetProcedure("getCrdDet_From6Digits");
        c.AddParam("@pan", req.Msg);
        c.AddParam("@nuban", nuban);
        DataSet dss = c.Select("rec");
        if (dss != null && dss.Tables.Count > 0 && dss.Tables[0].Rows.Count > 0)
        {
            //last 6 digits match with the nuban
            //continue
            removeParam("cnt", req);
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            wrongCnt++;
            removeParam("wrongCnt", req);
            addParam("wrongCnt", wrongCnt.ToString(), req);
            req.next_op = 7030;
            return "0";
        }

        return resp;
    }

    public string SetUserPIN2ndPin(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
            return resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter any 4 digit of your choice";
            return "PIN 2 Reset:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of the PIN is not less than or greater than 4 digits";
            return "PIN 2 Reset:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 numeric digits and not alphanumeric";
            return "PIN 2 Reset:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure your PIN 2 is different from PIN 1";
            return "PIN 2 Reset:%0A" + resp;
        }

        resp = "Kindly enter any 4 digits of your choice";
        return "PIN 2 Reset:%0A" + resp;
    }
    public string SaveCusPIN2ndPin(UReq req)
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
            req.next_op = 7026;
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
            req.next_op = 7026;
            return "9";
        }
        string resp = "0";
        try
        {
            
            string nuban = prm["NUBAN"];
            nuban = removeSpareParams(nuban);

            string PIN1 = getUSSDPin1(req.Msisdn.Trim(), nuban);
            string PIN2 = req.Msg;
            if (PIN1 == PIN2)//redirect to 
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 7026;
                return "9";
            }

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
    public string ConfirmSetUserPIN2ndPin(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        if (wrongCnt == 3)
        {
            removeParam("wrongCnt", req);
            resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
            return resp;
        }
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter any 4 digit of your choice";
            return "Confirm PIN 2 Reset:" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of the PIN is not less than or greater than 4 digits";
            return "Confirm PIN 2 Reset: " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 numeric digits and not alphanumeric";
            return "Confirm PIN 2 Reset: " + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The confirmation PIN did not match the intial PIN you entered.  Kindly ensure they are the same";
            return "Confirm PIN 2 Reset: " + resp;
        }
        return "";
    }
    public string SaveConfrimPIN2ndPin(UReq req)
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
            req.next_op = 7027;
            return "0";
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
            req.next_op = 7027;
            return "0";
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
            req.next_op = 7027;
            return "0";
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
    //______________________________________________________
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
            resp = "Enter your current USSD PIN";
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
            req.next_op = 7012;
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
            req.next_op = 7012;
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
                req.next_op = 7012;
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
}