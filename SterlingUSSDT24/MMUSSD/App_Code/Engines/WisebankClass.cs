using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using VTU.ObjectInfo;
using VTU.PL;
using VTU.UTILITIES;

/// <summary>
/// Summary description for WisebankClass
/// </summary>
public class WisebankClass : BaseEngine
{
    //NOTE: some classes are reused in API calls
    public string baseUrl = ConfigurationManager.AppSettings["WisebankBaseUrl"];
    public string userName = ConfigurationManager.AppSettings["WisebankUser"];
    public string password = ConfigurationManager.AppSettings["WisebankPassword"];
    public string institutionCode = ConfigurationManager.AppSettings["WisebankCode"];
    public string jsoncontent = "";


    //..............................API METHOD CALLS................................
    // 
    public GetAccountByNubanResp GetAccountInfoWithAcct(string nuban, string sessionId)
    {
        GetAccountByNubanResp response = new GetAccountByNubanResp();
        string apipath = "WISEBANK/GetAccountByAccountNo?institutionCode=" + institutionCode + "&accountNumber=" + nuban + "";
        GetAccountByNubanReq r = new GetAccountByNubanReq();
        r.accountNumber = nuban;
        r.institutionCode = institutionCode;
        string input = JsonConvert.SerializeObject(r);
        //long refid = SaveRequestToWisebank(sessionId, "GetAccountInfoWithAcct", input);
        string auth = "Basic " + getBase64(userName + ":" + password);

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Authorization", auth);

            var result = client.GetAsync(apipath).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                //jsoncontent = jsoncontent.Replace(@"\", "");
                //jsoncontent = jsoncontent.Replace("\"{", "{");
                //jsoncontent = jsoncontent.Replace("}\"", "}");
                response = JsonConvert.DeserializeObject<GetAccountByNubanResp>(jsoncontent);

            }
            else
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                try
                {
                    response = JsonConvert.DeserializeObject<GetAccountByNubanResp>(jsoncontent);
                }
                catch
                {
                    response = JsonConvert.DeserializeObject<dynamic>(jsoncontent);
                }
            }

        }

        //SaveResponseFromWisebank(refid, jsoncontent, response.result);

        return response;
    }

    public GetAccountSByMobileResp GetAccountsByPhoneNo(string mobile, string sessionId)
    {
        GetAccountSByMobileResp response = new GetAccountSByMobileResp();

        string apipath = "WISEBANK/GetAccountsByPhoneNo?institutionCode=" + institutionCode + "&phoneNumber=" + mobile + "";
        GetAccountsByMobileReq r = new GetAccountsByMobileReq();
        r.phoneNumber = mobile;
        r.institutionCode = institutionCode;
        string input = JsonConvert.SerializeObject(r);
        //long refid = SaveRequestToWisebank(sessionId, "GetAccountsByPhoneNo", input);

        string auth = "Basic " + getBase64(userName + ":" + password);

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Authorization", auth);

            var result = client.GetAsync(apipath).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                //jsoncontent = jsoncontent.Replace(@"\", "");
                //jsoncontent = jsoncontent.Replace("\"{", "{");
                //jsoncontent = jsoncontent.Replace("}\"", "}");
                response = JsonConvert.DeserializeObject<GetAccountSByMobileResp>(jsoncontent);

            }
            else
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                try
                {
                    response = JsonConvert.DeserializeObject<GetAccountSByMobileResp>(jsoncontent);
                }
                catch
                {
                    response = JsonConvert.DeserializeObject<dynamic>(jsoncontent);
                }
            }

        }

        //SaveResponseFromWisebank(refid, jsoncontent, response.result);

        return response;
    }

    public ValidateWiseBankCardResp ValidateCardPan(string nuban, string last6digits, string sessionId)
    {
        ValidateWiseBankCardResp response = new ValidateWiseBankCardResp();

        string apipath = "WISEBANK/ValidateCardPan?institutionCode=" + institutionCode + "&accountNumber=" + nuban + "&cardPan=" + last6digits;
        ValidateWiseBankCardReq r = new ValidateWiseBankCardReq();
        r.accountNumber = nuban;
        r.cardPan = last6digits;
        r.institutionCode = institutionCode;
        string input = JsonConvert.SerializeObject(r);
        //long refid = SaveRequestToWisebank(sessionId, "ValidateCardPan", input);

        string auth = "Basic " + getBase64(userName + ":" + password);

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Authorization", auth);

            var result = client.GetAsync(apipath).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                //jsoncontent = jsoncontent.Replace(@"\", "");
                //jsoncontent = jsoncontent.Replace("\"{", "{");
                //jsoncontent = jsoncontent.Replace("}\"", "}");
                response = JsonConvert.DeserializeObject<ValidateWiseBankCardResp>(jsoncontent);

            }
            else
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                try
                {
                    response = JsonConvert.DeserializeObject<ValidateWiseBankCardResp>(jsoncontent);
                }
                catch
                {
                    response = JsonConvert.DeserializeObject<dynamic>(jsoncontent);
                }
            }

        }

        //SaveResponseFromWisebank(refid, jsoncontent, response.result);
        return response;
    }

    public GetBalanceResp GetAccountBalance(string nuban, string sessionId)
    {
        GetBalanceResp response = new GetBalanceResp();
        string apipath = "WISEBANK/GetBalance?institutionCode=" + institutionCode + "&accountNumber=" + nuban + "";
        GetAccountByNubanReq r = new GetAccountByNubanReq();
        r.accountNumber = nuban;
        r.institutionCode = institutionCode;
        string input = JsonConvert.SerializeObject(r);
        //long refid = SaveRequestToWisebank(sessionId, "GetAccountBalance", input);

        string auth = "Basic " + getBase64(userName + ":" + password);

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Authorization", auth);

            var result = client.GetAsync(apipath).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                //jsoncontent = jsoncontent.Replace(@"\", "");
                //jsoncontent = jsoncontent.Replace("\"{", "{");
                //jsoncontent = jsoncontent.Replace("}\"", "}");
                response = JsonConvert.DeserializeObject<GetBalanceResp>(jsoncontent);

            }
            else
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                try
                {
                    response = JsonConvert.DeserializeObject<GetBalanceResp>(jsoncontent);
                }
                catch
                {
                    response = JsonConvert.DeserializeObject<dynamic>(jsoncontent);
                }
            }

        }
        //SaveResponseFromWisebank(refid, jsoncontent, response.result);

        return response;
    }

    public ValidateWiseBankCardResp CheckForValidCard(string nuban, string sessionId)
    {
        ValidateWiseBankCardResp response = new ValidateWiseBankCardResp();

        string apipath = "WISEBANK/GetValidAccountCard?institutionCode=" + institutionCode + "&accountNumber=" + nuban;
        GetAccountByNubanReq r = new GetAccountByNubanReq();
        r.accountNumber = nuban;
        r.institutionCode = institutionCode;
        string input = JsonConvert.SerializeObject(r);
        //long refid = SaveRequestToWisebank(sessionId, "GetAccountBalance", input);

        string auth = "Basic " + getBase64(userName + ":" + password);

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Authorization", auth);

            var result = client.GetAsync(apipath).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                //jsoncontent = jsoncontent.Replace(@"\", "");
                //jsoncontent = jsoncontent.Replace("\"{", "{");
                //jsoncontent = jsoncontent.Replace("}\"", "}");
                response = JsonConvert.DeserializeObject<ValidateWiseBankCardResp>(jsoncontent);

            }
            else
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                try
                {
                    response = JsonConvert.DeserializeObject<ValidateWiseBankCardResp>(jsoncontent);
                }
                catch
                {
                    response = JsonConvert.DeserializeObject<dynamic>(jsoncontent);
                }
            }

        }
        //SaveResponseFromWisebank(refid, jsoncontent, response.result);

        return response;
    }

    public GetCustomerInfoByNuban GetCustomerDetailsWithAcct(string nuban)
    {
        GetCustomerInfoByNuban response = new GetCustomerInfoByNuban();
        string apipath = "WISEBANK/GetCustomerByAccountNo?institutionCode=" + institutionCode + "&accountNumber=" + nuban + "";
        GetAccountByNubanReq r = new GetAccountByNubanReq();
        r.accountNumber = nuban;
        r.institutionCode = institutionCode;
        string input = JsonConvert.SerializeObject(r);
        //long refid = SaveRequestToWisebank(sessionId, "GetAccountInfoWithAcct", input);
        string auth = "Basic " + getBase64(userName + ":" + password);

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Authorization", auth);

            var result = client.GetAsync(apipath).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                //jsoncontent = jsoncontent.Replace(@"\", "");
                //jsoncontent = jsoncontent.Replace("\"{", "{");
                //jsoncontent = jsoncontent.Replace("}\"", "}");
                response = JsonConvert.DeserializeObject<GetCustomerInfoByNuban>(jsoncontent);

            }
            else
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                try
                {
                    response = JsonConvert.DeserializeObject<GetCustomerInfoByNuban>(jsoncontent);
                }
                catch
                {
                    response = JsonConvert.DeserializeObject<dynamic>(jsoncontent);
                }
            }

        }

        //SaveResponseFromWisebank(refid, jsoncontent, response.result);

        return response;
    }

    public ValidateWiseBankCardResp DisableCard(string nuban)
    {
        ValidateWiseBankCardResp response = new ValidateWiseBankCardResp();
        string apipath = "WISEBANK/DisableCard";//POST /WISEBANK/DisableCard
        GetAccountByNubanReq r = new GetAccountByNubanReq();
        r.accountNumber = nuban;
        r.institutionCode = institutionCode;
        string input = JsonConvert.SerializeObject(r);
        //long refid = SaveRequestToWisebank(sessionId, "GetAccountInfoWithAcct", input);
        string auth = "Basic " + getBase64(userName + ":" + password);

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Authorization", auth);
            var cont = new StringContent(input, System.Text.Encoding.UTF8, "application/json");

            var result = client.PostAsync(apipath, cont).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                //jsoncontent = jsoncontent.Replace(@"\", "");
                //jsoncontent = jsoncontent.Replace("\"{", "{");
                //jsoncontent = jsoncontent.Replace("}\"", "}");
                response = JsonConvert.DeserializeObject<ValidateWiseBankCardResp>(jsoncontent);

            }
            else
            {
                jsoncontent = result.Content.ReadAsStringAsync().Result;
                try
                {
                    response = JsonConvert.DeserializeObject<ValidateWiseBankCardResp>(jsoncontent);
                }
                catch
                {
                    response = JsonConvert.DeserializeObject<dynamic>(jsoncontent);
                }
            }

        }

        return response;
    }
    //..............................Menu operations.....................................
    //
    public string ProcessRequest(UReq req)
    {
        string resp = "0";
        int cnt = 0;
        try
        {
            char[] delm = { '*', '#' };
            string msg = req.Msg.Trim();
            string[] s = msg.Split(delm, StringSplitOptions.RemoveEmptyEntries);

            if (s.Length > 2)
            {
                req.next_op = GetStepFromShortString(msg);
            }

            var custInfo = new GetAccountSByMobileResp();
            custInfo = GetAccountsByPhoneNo(req.Msisdn.Trim(), req.SessionID);
            if (!custInfo.result)
            {
                //mobile is not profiled with Wise bank
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
            }

            req.next_op = 2500;

        }
        catch (Exception e)
        {
            req.next_op = 2500;
        }

        return resp;
    }

    public string GetWiseBankMenu(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            int cnt = RunCount(prm["cnt"]);

            if (cnt == 0)
            {
                //first entrance
                removeParam("cnt", req);
                resp = "1 Registration%0A2 Acct Balance%0A3 Open Acct%0A4 Transfer to Gateway/Sterling%0A5 Transfer to other banks%0A6 Airtime-Self%0A7 Airtime-Others%0A8 Next page";
                return "Gateway Menu%0A" + resp;
            }
            else if (cnt == 100)
            {
                removeParam("cnt", req);
                resp = "Sorry, You are not authorised for this service";
                return resp;
            }

        }
        catch
        {
            resp = "1 Registration%0A2 Acct Balance%0A3 Open Acct%0A4 Transfer to Gateway/Sterling%0A5 Transfer to other banks%0A6 Airtime-Self%0A7 Airtime-Others%0A8 Next page";
            return "Gateway Menu%0A" + resp;

        }

        return resp;
    }

    public string ProcessMenu(UReq req)
    {
        string resp = "0";

        try
        {
            var regexItem = new Regex("^[0-9]*$");
            if (!regexItem.IsMatch(req.Msg) || int.Parse(req.Msg) > 8)
            {
                req.next_op = 2500;
                return resp;
            }
            else
            {
                req.next_op = GetNextOpFromWiseMainMenu(req.Msg);
            }

        }
        catch
        {
            req.next_op = 2500;
        }

        return resp;


    }

    public string GetWiseBankMenu2(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            int cnt = RunCount(prm["cnt"]);

            if (cnt == 0)
            {
                //first entrance
                removeParam("cnt", req);
                resp = "1 Bills Payment%0A2 Buy Data%0A3 Disable Card%0A4 Reset USSD PIN%0A5 Back";
                return "Gateway Bank Menu%0A" + resp;
            }

        }
        catch
        {
            resp = "1 Bills Payment%0A2 Buy Data%0A3 Disable Card%0A4 Reset USSD PIN%0A5 Back";
        }

        return resp;
    }

    public string ProcessMenu2(UReq req)
    {
        string resp = "0";

        try
        {
            var regexItem = new Regex("^[0-9]*$");
            if (!regexItem.IsMatch(req.Msg) || int.Parse(req.Msg) > 4)
            {
                req.next_op = 2503;
                return resp;
            }
            else
            {
                req.next_op = GetNextOpFromWiseMainMenu2(req.Msg);
            }

        }
        catch
        {
            req.next_op = 2503;
        }

        return resp;


    }
    //+++++++++++++++++++++++++++++++++ Registration flow ++++++++++++++++++++++++++++++++++
    //
    public string CollectNuban(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            int cnt = RunCount(prm["cnt"]);

            if (cnt == 0)
            {
                //first entrance
                removeParam("cnt", req);
                resp = "Kindly enter your account number:";
                return "Registration%0A" + resp;
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
                //resp = "Your phone number and account number do not match. Kindly visit the nearest branch or call +23470078375464 to update your details ";
                resp = "This isn't the phone number linked to the account provided. Kindly visit the nearest branch to update your details";
                return resp;
            }
            else if (cnt == 104)
            {
                removeParam("cnt", req);
                resp = "The account number you entered is not profiled with Gateway Bank.";
                return resp;
            }
            else if (cnt == 105)
            {
                removeParam("cnt", req);
                resp = "Sorry, an error occurred while getting your details";
                return resp;
            }

        }
        catch
        {
            resp = "Kindly enter your account number:";
            return "Registration%0A" + resp;

        }

        return resp;
    }
    public string SaveNUBAN(UReq req)
    {
        string resp = "0"; string nuban = ""; int cnt = 0;
        int activated = 0; string mobile = ""; int trnxLimit = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);

        try
        {
            nuban = req.Msg;
            if (req.Msg.Length != 10)
            {
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2510;
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
                req.next_op = 2510;
                return "0";
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

            var accountInfo = new GetAccountByNubanResp();
            accountInfo = GetAccountInfoWithAcct(nuban, req.SessionID);
            if (!accountInfo.result)
            {
                cnt = 104;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2510;
                return "0";
            }

            mobile = accountInfo.PhoneNumber;
            if (req.Msisdn != ConvertMobile234(mobile))
            {
                cnt = 103;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2510;
                return "0";
            }

            var cardCheck = new ValidateWiseBankCardResp();
            //Check if customer has active card.
            cardCheck = CheckForValidCard(nuban, req.SessionID);
            if (!cardCheck.result)
            {
                //Direct to Collect BVN instead of Card pan
                cnt = 102;
                addParam("Bypass", cnt.ToString(), req);
                addParam("NUBAN", nuban, req);
                req.next_op = 2514;
                return "0";
            }
            else
            {
                addParam("NUBAN", nuban, req);
            }
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
        //if (wrongCnt == 3)
        //{
        //    removeParam("wrongCnt", req);
        //    resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your registration. Thank you";
        //    return resp;
        //}
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
    public string ValidateBVN(UReq req)
    {
        string resp = "0";
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        string bvn = req.Msg.Trim();

        if (req.Msg.Length != 11)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2514;
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
            req.next_op = 2514;
            return "0";
        }

        var accountInfo = new GetAccountByNubanResp();
        accountInfo = GetAccountInfoWithAcct(nuban, req.SessionID);

        if (accountInfo.BvnId != bvn)//VALIDATE BVN FROM GATEWAY
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2514;
            return "0";
        }

        cnt = 102;
        addParam("Bypass", cnt.ToString(), req);

        return resp;
    }
    public string CollectLast6Digits(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);

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
            return "Registration:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is exactly 6 digits";
            return "Registration:%0A" + resp;

        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is not alphanumeric";
            return "Registration:%0A" + resp;

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
        return "Registration:%0A" + resp;

    }
    public string SaveLastSixDigits(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];

        try
        {
            long last6digits = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2512;
            return "0";
        }

        if (req.Msg.Length != 6)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2512;
            return "0";
        }

        //Validate last digits
        var response = new ValidateWiseBankCardResp();
        response = ValidateCardPan(nuban, req.Msg, req.SessionID);
        if (!response.result)
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2512;
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
            resp = "Enter any 4-digit USSD PIN of choice: ";
            return "Set PIN: " + resp;
            //return "Enter any 4-digit USSD PIN of choice: ";
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

        resp = "Enter any 4-digit USSD PIN of choice";
        return "Set PIN: " + resp;
    }
    public string ConfirmSetUserPIN(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);

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
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2516;
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
            req.next_op = 2516;
            return "0";
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
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2518;
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
            req.next_op = 2518;
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
            req.next_op = 2518;
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
            resp = DoActivate(req.Msisdn, nuban, SavePIN, bypass, req.SessionID);
            SMSAPIInfoBip SendSms = new SMSAPIInfoBip();

            //Message for customer who registered without active card
            if (resp.StartsWith("Great") && (bypass == 102))
            {
                smsMessage = "You are now registered for our Magic Banking! Enjoy instant transfers, airtime top-up and a lot more with *822*25#.  More info? Call 070078375464";
                // SendSms.insertIntoInfobip(smsMessage, req.Msisdn);
                resp = "Great! You are now registered for Magic Banking.";
            }

            //Message for customer who tries to do transaction but hasn't set USSD pin for that account. //102-transaction //101 -first registration
            if (resp.StartsWith("Great") && (bypass == 101 || bypass == 111))
            {

                if (bypass == 101)
                {
                    smsMessage = "You are now registered for our Magic Banking! Enjoy instant transfers, airtime top-up and a lot more with *822*25#.  More info? Call 070078375464";
                    //SendSms.insertIntoInfobip(smsMessage, req.Msisdn);
                    resp = "You are now registered for magic banking.";
                }
                string maskedNuban = MaskDetails(nuban, 9, 0, 4);
                if (bypass == 102)
                {
                    smsMessage = "You are now registered for our Magic Banking! Enjoy instant transfers, airtime top-up and a lot more with *822*25#.  More info? Call 070078375464";
                    // SendSms.insertIntoInfobip(smsMessage, req.Msisdn);
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
    //-------Referenced in DoSelfReg-------------
    //
    public string DoActivate(string mobile, string nuban, string pin, int bypass, string sessionId)
    {
        string resp = ""; string dbMob = "";

        //Validate account again
        var accountInfo = new GetAccountByNubanResp();
        accountInfo = GetAccountInfoWithAcct(nuban, sessionId);
        if (!accountInfo.result)
        {
            resp = "The account number you entered is not profiled with Gateway Bank.";

            return resp;
        }

        dbMob = accountInfo.PhoneNumber;

        if (ConvertMobile234(dbMob) != mobile)
        {
            resp = "This isn't the phone number linked to the account provided. Kindly visit the nearest branch to update your details";
            return resp;
        }

        Go_Registered_Account g = new Go_Registered_Account();
        g.Mobile = mobile;
        g.NUBAN = nuban;
        g.StatusFlag = Constants.Status_Green.ToString();
        g.TransactionRefID = DateTime.Now.Ticks.ToString();
        g.RefID = "1987";
        RegCustomers rc = new RegCustomers();
        int retval = rc.Activate(g, pin, bypass);
        switch (retval)
        {
            case -99:
                resp = "All connections are currently in use%0A Kindly try again!";
                break;
            case -1:
                resp = "You have already been activated for this service%0ADial *822*25# to use the service";
                break;
            case 1:
                resp = "Great! You are successfully registered for Sterling USSD Service. Please dial *822*25# to experience the magic…";
                break;
        }

        return resp;
    }
    //+++++++++++++++++++++++++++++++++ Account Balance ++++++++++++++++++++++++++++++++++++
    //
    public string SaveAcctBalReq(UReq req)
    {
        string resp = "0";

        int activated = -1; int enrollType = 0; string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            activated = int.Parse(dr["Activated"].ToString());
            enrollType = int.Parse(dr["EnrollType"].ToString());
            nuban = dr["Nuban"].ToString();

            addParam("NUBAN", nuban, req);
        }
        else
        {
            //redirect to register and set PIN (WISEBANK)
            req.next_op = 2510;
            return "0";
        }

        return resp;
    }
    public string GetAccountWiseBalance(UReq req)
    {
        string resp = "";
        string nuban = ""; decimal bal = 0; int cnt = 0;
        Gadget g = new Gadget();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        nuban = prm["NUBAN"];

        var balanceInfo = new GetBalanceResp();
        balanceInfo = GetAccountBalance(nuban, req.SessionID);

        if (!balanceInfo.result)
        {
            resp = "There is no account currently associated with this mobile number.";
        }
        else
        {
            bal = Convert.ToDecimal(balanceInfo.AvailableBalance);
            resp = "Account: " + nuban + " Balance: " + bal.ToString("#,##0.00");

            //Insert charges for balance enquiry 
            g.Insert_Charges(req.SessionID, nuban, 6);
        }

        return resp;
    }
    //+++++++++++++++++++++++++++++++++ Airtime for Self ++++++++++++++++++++++++++++++++++++
    //
    public string SaveAirtimeSelfReq(UReq req)
    {
        string resp = "0";
        int activated = -1; string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            activated = int.Parse(dr["Activated"].ToString());
            nuban = dr["Nuban"].ToString();
            addParam("NUBAN", nuban, req);
            if (activated != 1)
            {
                //redirect to set PIN (WISEBANK)
                req.next_op = 2516;
                return "0";
            }
        }
        else
        {
            //redirect to register and set PIN (WISEBANK)
            req.next_op = 2510;
            return "0";
        }

        return resp;
    }
    public string CollectAirtimeAmount(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);

        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            //resp = "Enter any 4 digit USSD PIN of choice";
            return "Enter your airtime amount:";
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount is not less than 100";
            return "Enter your airtime amount:%0A " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only digits and not alphanumeric";
            return "Enter your airtime amount:%0A " + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Sorry you don't have sufficient balance for this transaction";
            return resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry, we were unable to validate your record";
            return resp;
        }

        return "Enter your airtime amount:";
    }
    public string SaveAirtimeAmount(UReq req)
    {
        string resp = "0";
        int cnt = 0; Gadget g = new Gadget(); string nuban = ""; decimal bal = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2522;
            return "0";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (decimal.Parse(req.Msg) < 100)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2522;
            return "0";
        }

        nuban = prm["NUBAN"];
        decimal amount = decimal.Parse(req.Msg);

        var balanceInfo = new GetBalanceResp();
        balanceInfo = GetAccountBalance(nuban, req.SessionID);

        if (!balanceInfo.result)
        {
            cnt = 103;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2522;
            return "0";
        }
        else
        {
            bal = Convert.ToDecimal(balanceInfo.AvailableBalance);
            if (amount > bal)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2522;
                return "0";
            }

            addParam("AMOUNT", amount.ToString(), req);
        }

        return resp;
    }
    public string DoAirtimeSelf(UReq req)
    {
        string resp = "";

        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            Gadget g = new Gadget();
            string frmNuban = "";
            string Amt = "";
            frmNuban = prm["NUBAN"];
            Amt = prm["AMOUNT"];

            //resp = USSD.DoBuySelf(req.Msisdn, Amt, req.Network, req.SessionID, frmNuban);
            resp = USSD.Buy_Self_Wisebank(req.Msisdn, Amt, req.Network, req.SessionID, frmNuban);

        }
        catch (Exception ex)
        {
            resp = "ERROR: We are sorry! Service improvement is currently ongoing";
        }

        return resp;
    }
    //++++++++++++++++++++++++++++++++ Airtime for others +++++++++++++++++++++++++++++++++++
    //
    public string SaveAirtimeOthersReq(UReq req)
    {
        string resp = "0";
        int activated = -1; string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            activated = int.Parse(dr["Activated"].ToString());
            nuban = dr["Nuban"].ToString();
            addParam("NUBAN", nuban, req);
            if (activated != 1)
            {
                //redirect to set PIN (WISEBANK)
                req.next_op = 2516;
                return "0";
            }
        }
        else
        {
            //redirect to register and set PIN (WISEBANK)
            req.next_op = 2510;
            return "0";
        }
        return resp;
    }
    public string CollectAirtimeAmountOthers(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);

        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            return "Enter the airtime amount:";
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount is not less than 100";
            return "Enter the airtime amount:%0A " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only digits and not alphanumeric";
            return "Enter the airtime amount:%0A " + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Sorry you don't have sufficient balance for this transaction";
            return resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry, we were unable to validate your record";
            return resp;
        }

        return "Enter the airtime amount:";
    }
    public string SaveAirtimeAmountOthers(UReq req)
    {
        string resp = "0";
        int cnt = 0; Gadget g = new Gadget(); string nuban = ""; decimal bal = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2532;
            return "0";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (decimal.Parse(req.Msg) < 100)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2532;
            return "0";
        }

        nuban = prm["NUBAN"];
        decimal amount = decimal.Parse(req.Msg);

        var balanceInfo = new GetBalanceResp();
        balanceInfo = GetAccountBalance(nuban, req.SessionID);

        if (!balanceInfo.result)
        {
            cnt = 103;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2532;
            return "0";
        }
        else
        {
            bal = Convert.ToDecimal(balanceInfo.AvailableBalance);
            if (amount > bal)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2532;
                return "0";
            }

            addParam("AMOUNT", amount.ToString(), req);
        }


        return resp;
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
            req.next_op = 2533;
            return "0";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length != 11)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2533;
            return "0";
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
    public string CollectNetwork(UReq req)
    {
        string resp = "";

        resp = "Enter mobile number network to be topup:%0A1 for MTN%0A2 for GLO%0A3 for Airtel%0A4 for 9Mobile";
        return resp;
    }
    public string SaveNetworkOthers(UReq req)
    {
        string resp = "0";
        try
        {
            long network = long.Parse(req.Msg);
        }
        catch
        {
            req.next_op = 2535;
            return "0";
        }

        if (int.Parse(req.Msg) > 5 || int.Parse(req.Msg) < 1)
        {
            req.next_op = 2535;
            return "0";
        }

        addParam("NETWORK", req.Msg, req);

        return resp;
    }
    public string CollectPIN_AirtimeOthrs(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
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

        resp = "Enter your USSD Authentication PIN";
        return "USSD Authentication PIN:%0A" + resp;
    }
    public string SavePIN_AirtimeOthrs(UReq req)
    {
        int cnt = 0; string resp = "0";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);

        //check if entry is numeric
        try
        {
            long PIN = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2537;
            return "0";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2537;
            return "0";
        }

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
            req.next_op = 2537;
            return "0";
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
    public string DoAirtimeOthers(UReq req)
    {
        string resp = "";
        try
        {
            string frmNuban = "";
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            frmNuban = prm["NUBAN"];

            resp = USSD.Buy_Friend_Wisebank(prm["MOBILE"], req.Msisdn, prm["AMOUNT"], prm["NETWORK"], req.SessionID, prm["PIN"], frmNuban);
        }
        catch (Exception ex)
        {
            resp = "ERROR: We are sorry! Service improvement is currently ongoing";
        }
        return resp;
    }
    //+++++++++++++++++++++++++++++ Transfer to Gateway/Sterling ++++++++++++++++++++++++++++
    //
    public string SaveLocalTxnReq(UReq req)
    {
        string resp = "0";
        int activated = -1; string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            activated = int.Parse(dr["Activated"].ToString());
            nuban = dr["Nuban"].ToString();
            addParam("NUBAN", nuban, req);
            if (activated != 1)
            {
                //redirect to set PIN (WISEBANK)
                req.next_op = 2516;
                return "0";
            }
        }
        else
        {
            //redirect to register and set PIN (WISEBANK)
            req.next_op = 2510;
            return "0";
        }
        return resp;
    }
    public string CollectLocalTxnAmt(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int amtLimit = RunCount(prm["MaxAmtPerTrans"]);
        string acct = prm["NUBAN"];

        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Transfer Amount:";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 0 naira";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            var firstName = "";
            try
            {
                var custDetails = GetCustomerDetailsWithAcct(acct);
                firstName = custDetails.FirstName;
                resp = "Dear " + firstName + ", you do not have sufficient balance for this transaction. Please credit your account and try again. Thank you.";
                return resp;
            }
            catch
            {
                resp = "You currently do not have sufficient balance for this transaction.";
            }

            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry you have exceeded your transfer limit for today.";
            return "Transfer amount:%0A" + resp;
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
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 105)
        {
            removeParam("cnt", req);
            resp = "Sorry, we were unable to validate your record";
            return resp;
        }

        resp = "Enter Transfer amount";
        return "Transfer amount:%0A" + resp;
    }
    public string SaveLocalTxnAmt(UReq req)
    {
        int cnt = 0; string resp = "";
        string frmNuban = ""; Gadget g = new Gadget();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int activated = -1;
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2541;
            return "0";
        }

        if (decimal.Parse(req.Msg) <= 0)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2541;
            return "0";
        }

        frmNuban = prm["NUBAN"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(req.Msg.Trim());

        //if amount is over 20k check if customer has set pin 2
        if (amtval > 20000)
        {
            var isPin2Set = checkForPin2(frmNuban, req.Msisdn, req);
            if (!isPin2Set)
            {
                addParam("Wisebank", "1", req);
                req.next_op = 777;
                return "0";
            }
        }

        var balanceInfo = new GetBalanceResp();
        balanceInfo = GetAccountBalance(frmNuban, req.SessionID);
        if (!balanceInfo.result)
        {
            cnt = 105;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2541;
            return "0";
        }
        else
        {
            cusbal = Convert.ToDecimal(balanceInfo.AvailableBalance);
        }

        if (cusbal > amtval)
        {
            //proceed
            var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
            DataRow dr1 = ds1.Tables[0].Rows[0];
            int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
            activated = int.Parse(dr1["Activated"].ToString());

            if (trnxLimit == 1 && activated == 1)
            {
                Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);

                if (limitedTuple.Item2 < amtval + Totaldone)
                {
                    var cardCheck = new ValidateWiseBankCardResp();
                    //Check if customer has active card.
                    cardCheck = CheckForValidCard(frmNuban, req.SessionID);
                    if (!cardCheck.result)
                    {
                        cnt = 111;
                        addParam("NoCard", cnt.ToString(), req);
                        req.next_op = 4290;
                        return "0";
                    }
                    else
                    {
                        cnt = 999;//direct to enter last 6 digits
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 2512;
                        return "0";
                    }
                }
            }

            if (trnxLimit == 0)
            {
                Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                if (limitedTuple.Item1 < amtval)
                {
                    //Reroute to amount field
                    cnt = 104;
                    int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                    addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 2541;
                    return "0";

                }
                if (limitedTuple.Item2 < amtval + Totaldone)
                {
                    //reroute to amount field
                    cnt = 103;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 2541;
                    return "0";
                }
            }
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2541;
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
    public string CollectBeneMode(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "1.Input Account No%0A2.Select from saved beneficiary";
            return "Enter Beneficiary:%0A" + resp;
        }
        return resp;
    }
    public string SaveBeneMethod(UReq req)
    {
        string resp = "0";
        try
        {
            if (req.Msg == "1")
            {
                req.next_op = 2545;
            }
            else
            {
                var regexItem = new Regex("^[0-9]*$");
                if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
                {
                    //continue: no issues
                    addParam("BeneSelected", "1", req);
                }
                else
                {
                    //redirect to beneficiary selection mode page
                    req.next_op = 2543;
                    resp = "0";
                }
            }
        }
        catch
        {
            //redirect to beneficiary selection mode page
            req.next_op = 2543;
            resp = "0";
        }

        return resp;
    }
    //if beneficiary name search is null or customer selects to input acct no
    //
    public string CollectBeneAcct(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Beneficiary Account:";
            return resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Unable to fetch data with name.%0APlease enter beneficiary account:";
            return resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Please enter a valid beneficiary account number:";
            return resp;
        }

        return resp;
    }
    public string SaveBeneLocal(UReq req)
    {
        string resp = "0"; int cnt = 0;
        try
        {
            var regexItem = new Regex("^[0-9]*$");
            if (regexItem.IsMatch(req.Msg) && req.Msg.Length == 10)
            {
                //continue: no issues
                addParam("TONUBAN", req.Msg, req);
            }
            else
            {
                //redirect to enter beneficiary account
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2545;
                resp = "0";
            }

        }
        catch
        {
            req.next_op = 2545;
            resp = "0";
        }
        return resp;
    }
    //***********************************************************************
    //
    public string CollectBeneName(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Search by Beneficiary Name:";
        }
        else if (cnt == 101)
        {
            resp = "Search by Beneficiary Name:%0APlease ensure your enter only letters.";
        }
        return resp;
    }
    public string FetchBeneIntodb(UReq req)
    {
        string beneName = "";
        var regexItem = new Regex("^[a-zA-Z ]*$");
        if (regexItem.IsMatch(req.Msg) && !string.IsNullOrEmpty(req.Msg))
        {
            //No numbers. Safe to continue
            beneName = req.Msg.Trim();
        }
        else
        {
            //redirect
            addParam("cnt", "101", req);
            req.next_op = 2547;
            return "0";
        }

        //Get Beneficiary list
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetBeneList");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        c.AddParam("@beneName", beneName);
        c.AddParam("@bankID", 99);//99 for intra customers
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            //Continue to display list of beneficiaries matching the name
        }
        else
        {
            //redirect
            removeParam("BeneSelected", req);
            addParam("cnt", "101", req);
            req.next_op = 2545;
            return "0";
        }


        return "0";
    }
    public string ListBeneInfo(UReq req)
    {
        Gadget g = new Gadget(); int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            List<BeneList> lb = g.GetBeneListByPage(page.ToString(), req.SessionID);
            if (lb.Count <= 0)
            {
                removeParam("BENENXTPAG", req);
                lb = g.GetBeneListByPage("0", req.SessionID);
            }

            foreach (BeneList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.BeneName + " (" + item.Nuban + "-" + item.BeneBank + ")";
                }
                else
                {
                    int acctListNo = Convert.ToInt16(item.TransRate);
                    resp += "%0A" + acctListNo + " " + item.BeneName + " (" + item.Nuban + "-" + item.BeneBank + ")";
                }
            }

            if (cntfirst < 2)
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
        return "Select Beneficiary%0A" + resp;
    }
    public int SaveBeneSelected(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "0")
        {
            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            page++;
            removeParam("BENENXTPAG", req);
            addParam("BENENXTPAG", page.ToString(), req);
            return 99;
        }
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
                page++;
                removeParam("BENENXTPAG", req);
                addParam("BENENXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            page++;
            removeParam("BENENXTPAG", req);
            addParam("BENENXTPAG", page.ToString(), req);
            return 99;
        }

        addParam("BENEITEMSELECT", req.Msg, req);
        addParam("B", "1", req);
        return 1;
    }
    public string DisplaySummaryLocal(UReq req)
    {
        Gadget g = new Gadget(); int flag = 0; int ITEMSELECT = 0;
        string resp = ""; string ToNuban = ""; string frmNuban = "";
        string prms = getParams(req); string ToName = ""; decimal amt = 0;
        NameValueCollection prm = splitParam(prms);
        frmNuban = prm["NUBAN"];
        addParam("frmNuban", frmNuban, req);

        //Check if beneficiary was selected.
        int beneFlag = 0; string beneDetails = "";
        beneFlag = RunCount(prm["B"]);
        if (beneFlag != 1)
        {
            ToNuban = prm["TONUBAN"];
        }
        else
        {
            ITEMSELECT = RunCount(prm["BENEITEMSELECT"]);
            beneDetails = g.GetBeneByListID(ITEMSELECT, req);
            char[] sep1 = { '*' };
            string[] beneBits = beneDetails.Split(sep1, StringSplitOptions.RemoveEmptyEntries);
            ToNuban = beneBits[0];
            addParam("TONUBAN", beneBits[0].Trim(), req);
        }

        amt = decimal.Parse(prm["AMOUNT"]);
        //Check if the account is sterling or imal
        if (ToNuban.StartsWith("05"))
        {
            IMALEngine iMALEngine = new IMALEngine();
            ImalDetails imalinfo = iMALEngine.GetImalDetailsByNuban(ToNuban);
            ToName = imalinfo.CustomerName;
        }
        else if (ToNuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = ToNuban;
            string bankOneReq = bank1.createRequestForNameEnq();
            var Bank1ref = LogBankOneRequest(req.SessionID, ToNuban, bankOneReq);
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.BankOneGetAccountName(encrptdReq);
            bool isBankOne = bank1.readResponseForNameEnq(getBankOneResp);
            LogBankOneResponse(Bank1ref, getBankOneResp, bank1.status);
            ToName = bank1.accountName;
        }
        else
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_S2S_GetBeneByNuban");
            c.AddParam("@nuban", ToNuban);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                ToName = dr["CUS_SHOW_NAME"].ToString();
            }
            else
            {
                //Check if toAccount is Wisebank
                var custDetails = GetCustomerDetailsWithAcct(ToNuban);
                if (!custDetails.result)
                {
                    ToName = "Unable to fetch beneficiary account name at this type.";
                }
                else
                {
                    flag = 98;
                    ToName = custDetails.FirstName + " " + custDetails.Surname;
                }

            }
        }

        if (string.IsNullOrEmpty(ToName))
        {
            resp = "An error occured at this time as we are unable to validate the customers details from the core.  Kindly try again later.";
        }
        else
        {
            //adding params for saving beneficiary
            if (flag == 98)//Gateway bank
            {
                addParam("ToName", ToName, req);
                addParam("BankName", "GATEWAY MORTGAGE", req);
                addParam("ToBankCode", "98", req);
            }
            else
            {
                addParam("ToName", ToName, req);
                addParam("BankName", "STERLING", req);
                addParam("ToBankCode", "99", req);
            }

            resp = "You are transferring the sum of " + getRealMoney(amt) + " from " + frmNuban + " to " + ToName + "%0A1.Yes%0A2.No";
        }
        return "Confirmation: " + resp;
    }
    public string SaveSummaryLocal(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            string nuban = prm["NUBAN"];
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
                            cnt = 101;
                            addParam("Bypass", cnt.ToString(), req);
                            req.next_op = 2516;
                            return "0";
                        }
                        if (activated == 1)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                //customer is okay to continue
            }

            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 2557;
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string CollectPINForLocalTxn(UReq req)
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
            resp = "The USSD PIN you provided is incorrect. Please check and try again.";
            return "Authentication:%0A" + resp;
        }
        resp = "Enter your USSD PIN";
        return "Authentication:%0A" + resp;
    }
    public string SavetxnPINForLocalTxn(UReq req)
    {
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2553;
            return "0";
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
            req.next_op = 2553;
            return "0";
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
                //submit request to transfer is amount less than 20K

                int amt = RunCount(prm["AMOUNT"]);
                if (amt < 20000)
                {
                    //check is bene was selected 
                    int flag = RunCount(prm["BeneSelected"]);
                    if (flag == 1)
                    {
                        //Beneficiary was selected. No reply needed
                        req.next_op = 2557;
                        return "1";
                    }
                    else
                    {
                        //Beneficiary 
                        req.next_op = 2557;
                        return "0";
                    }
                }
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
            req.next_op = 2553;
            return "0";
        }

        return resp;
    }
    public string CollectPIN2ForLocalTxn(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter your USSD PIN 2";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 4 digits";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 digits and not alphanumeric";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The USSD PIN 2 you provided is incorrect. Please check and try again.";
            return "Authentication for PIN 2:%0A" + resp;
        }
        resp = "Enter your USSD PIN 2";
        return "Authentication:%0A" + resp;
    }
    public string SavetxnPIN2ForLocalTxn(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2555;
            return "0";
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
            req.next_op = 2555;
            return "0";
        }
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);

        //check the registration table to confirm if PIN entered is correct
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetCusRegPIN2");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@auth", req.Msg);
        DataSet ds = c.Select("rec");
        string resp = "0";
        if (ds.Tables[0].Rows.Count > 0)
        {

            try
            {
                addParam("PIN2", req.Msg, req);
                string prms = getParams(req);
                NameValueCollection prm = splitParam(prms);
                //check is bene was selected 
                int flag = RunCount(prm["BeneSelected"]);
                if (flag == 1)
                {
                    //Beneficiary was selected. No reply needed
                    req.next_op = 2557;
                    return "1";
                }
                else
                {
                    //Beneficiary 
                    req.next_op = 2557;
                    return "0";
                }
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
            req.next_op = 2555;
            return "0";
        }

        return resp;
    }
    public string SubmitLocalTxn(UReq req)
    {

        string resp = ""; int summary = 0; string PIN = ""; string PIN2 = "";
        int k = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            //***************************************************************
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*25# to try again";
                return resp;
            }
            PIN = prm["PIN"];

            try
            {
                PIN2 = prm["PIN2"];
            }
            catch
            {

            }
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,custauthid2,trans_type,sessionid,frmAccount) " +
                " values(@mb,@ta,@am,@pn,@pn2,6,@sid,@frmNuban)";//trans_type = 6 for local transfer
            //string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,frmAccount) " +
            //    " values(@mb,@ta,@am,@pn,1,@sid,@frmNuban)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@pn2", PIN2 ?? "");
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@frmNuban", prm["frmNuban"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            Upd_acct_byTranstype(req.SessionID, 6);
            resp = "Your transaction has been submitted.%0ADo you want to save the beneficiary?%0A1.Yes%0A2.No";

            Gadget g = new Gadget();
            g.Insert_Charges(req.SessionID, prm["frmNuban"], 7); //
        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }
    public string SubmitLocalTxnNoReply(UReq req)
    {

        string resp = ""; int summary = 0; string PIN = ""; string PIN2 = "";
        int k = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            //***************************************************************
            summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*25# to try again";
                return resp;
            }
            PIN = prm["PIN"];

            try
            {
                PIN2 = prm["PIN2"];
            }
            catch
            {

            }
            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,custauthid2,trans_type,sessionid,frmAccount) " +
                " values(@mb,@ta,@am,@pn,@pn2,6,@sid,@frmNuban)";
            //string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,frmAccount) " +
            //    " values(@mb,@ta,@am,@pn,1,@sid,@frmNuban)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@pn2", PIN2 ?? "");
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@frmNuban", prm["frmNuban"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            resp = "ERROR: Could not contact core banking system";
        }
        if (k > 0)
        {
            Upd_acct_byTranstype(req.SessionID, 6);
            resp = "Transaction has been submitted for processing";
            Gadget g = new Gadget();
            g.Insert_Charges(req.SessionID, prm["frmNuban"], 1);
        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }
    //++++++++++++++++++++++++++++++ Inter-bank Transfer +++++++++++++++++++++++++++++++++
    //
    public string SaveInterbankTxnReq(UReq req)
    {
        string resp = "0";
        int activated = -1; string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            activated = int.Parse(dr["Activated"].ToString());
            nuban = dr["Nuban"].ToString();
            addParam("NUBAN", nuban, req);
            if (activated != 1)
            {
                //redirect to set PIN (WISEBANK)
                req.next_op = 2516;
                return "0";
            }
        }
        else
        {
            //redirect to register and set PIN (WISEBANK)
            req.next_op = 2510;
            return "0";
        }
        return resp;
    }
    public string CollectInterTxnAmt(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        int amtLimit = RunCount(prm["MaxAmtPerTrans"]);
        string acct = prm["NUBAN"];

        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Transfer Amount:";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is above 0 naira";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the amount entered is not alphanumeric";
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            var firstName = "";
            try
            {
                var custDetails = GetCustomerDetailsWithAcct(acct);
                firstName = custDetails.FirstName;
                resp = "Dear " + firstName + ", you do not have sufficient balance for this transaction. Please credit your account and try again. Thank you.";
                return resp;
            }
            catch
            {
                resp = "You currently do not have sufficient balance for this transaction.";
            }

            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry you have exceeded your transfer limit for today.";
            return "Transfer amount:%0A" + resp;
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
            return "Transfer amount:%0A" + resp;
        }
        else if (cnt == 105)
        {
            removeParam("cnt", req);
            resp = "Sorry, we were unable to validate your record";
            return resp;
        }

        resp = "Enter Transfer amount";
        return "Transfer amount:%0A" + resp;
    }
    public string SaveInterTxnAmt(UReq req)
    {
        int cnt = 0; string resp = "";
        string frmNuban = ""; Gadget g = new Gadget();
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int activated = -1;
        //check if entry is numeric
        try
        {
            long amt = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2561;
            return "0";
        }

        if (decimal.Parse(req.Msg) <= 0)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2561;
            return "0";
        }

        frmNuban = prm["NUBAN"];
        decimal cusbal = 0; decimal amtval = 0;
        amtval = decimal.Parse(req.Msg.Trim());

        //if amount is over 20k check if customer has set pin 2
        if (amtval > 20000)
        {
            var isPin2Set = checkForPin2(frmNuban, req.Msisdn, req);
            if (!isPin2Set)
            {
                addParam("Wisebank", "1", req);
                req.next_op = 777;
                return "0";
            }
        }

        var balanceInfo = new GetBalanceResp();
        balanceInfo = GetAccountBalance(frmNuban, req.SessionID);
        if (!balanceInfo.result)
        {
            cnt = 105;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2561;
            return "0";
        }
        else
        {
            cusbal = Convert.ToDecimal(balanceInfo.AvailableBalance);
        }

        if (cusbal > amtval)
        {
            //proceed
            var ds1 = getRegisteredProfile(req.Msisdn, frmNuban);
            DataRow dr1 = ds1.Tables[0].Rows[0];
            int trnxLimit = int.Parse(dr1["TrnxLimit"].ToString());
            activated = int.Parse(dr1["Activated"].ToString());

            if (trnxLimit == 1 && activated == 1)
            {
                Tuple<decimal, decimal> limitedTuple = GetLimitMINMAXamt();
                getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                getTotalAirtimeDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);

                if (limitedTuple.Item2 < amtval + Totaldone)
                {
                    var cardCheck = new ValidateWiseBankCardResp();
                    //Check if customer has active card.
                    cardCheck = CheckForValidCard(frmNuban, req.SessionID);
                    if (!cardCheck.result)
                    {
                        cnt = 111;
                        addParam("NoCard", cnt.ToString(), req);
                        req.next_op = 4290;
                        return "0";
                    }
                    else
                    {
                        cnt = 999;//direct to enter last 6 digits
                        addParam("cnt", cnt.ToString(), req);
                        req.next_op = 2512;
                        return "0";
                    }
                }
            }

            if (trnxLimit == 0)
            {
                Tuple<decimal, decimal> limitedTuple = getMINMAXamt();
                getTotalTransDonePerday(amtval, req.Msisdn, req.SessionID, limitedTuple.Item2);
                if (limitedTuple.Item1 < amtval)
                {
                    //Reroute to amount field
                    cnt = 104;
                    int maxAmtPerTrans = Convert.ToInt16(limitedTuple.Item1);
                    addParam("MaxAmtPerTrans", maxAmtPerTrans.ToString(), req);
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 2561;
                    return "0";

                }
                if (limitedTuple.Item2 < amtval + Totaldone)
                {
                    //reroute to amount field
                    cnt = 103;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 2561;
                    return "0";
                }
            }
        }
        else
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2561;
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
    public string CollectInterBeneMode(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "1.Input Account No%0A2.Select from saved beneficiary";
            return "Enter Beneficiary:%0A" + resp;
        }
        return resp;
    }
    public string SaveInterBeneMethod(UReq req)
    {
        string resp = "0";
        try
        {
            if (req.Msg == "1")
            {
                req.next_op = 2565;
            }
            else
            {
                var regexItem = new Regex("^[0-9]*$");
                if (regexItem.IsMatch(req.Msg) && int.Parse(req.Msg) < 3)
                {
                    //continue: no issues
                    addParam("BeneSelected", "1", req);
                }
                else
                {
                    //redirect to beneficiary selection mode page
                    req.next_op = 2563;
                    resp = "0";
                }
            }
        }
        catch
        {
            //redirect to beneficiary selection mode page
            req.next_op = 2563;
            resp = "0";
        }

        return resp;
    }
    //if beneficiary name search is null or customer selects to input acct no
    //
    public string CollectInterBeneAcct(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter Beneficiary Account:";
            return resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Unable to fetch data with name.%0APlease enter beneficiary account:";
            return resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Please enter a valid beneficiary account number:";
            return resp;
        }

        return resp;
    }
    public string SaveInterBene(UReq req)
    {
        string resp = "0"; int cnt = 0;
        try
        {
            var regexItem = new Regex("^[0-9]*$");
            if (regexItem.IsMatch(req.Msg) && req.Msg.Length == 10)
            {
                //continue: no issues
                addParam("TONUBAN", req.Msg, req);
            }
            else
            {
                //redirect to enter beneficiary account
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2565;
                resp = "0";
            }

        }
        catch
        {
            req.next_op = 2565;
            resp = "0";
        }
        return resp;
    }
    public string PaintInterBankList(UReq req)
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
    public int SaveInterBankList(object obj)
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
    //***********************************************************************
    //
    public string CollectInterBeneName(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Search by Beneficiary Name:";
        }
        else if (cnt == 101)
        {
            resp = "Search by Beneficiary Name:%0APlease ensure your enter only letters.";
        }
        return resp;
    }
    public string FetchInterBeneIntodb(UReq req)
    {
        string beneName = "";
        var regexItem = new Regex("^[a-zA-Z ]*$");
        if (regexItem.IsMatch(req.Msg) && !string.IsNullOrEmpty(req.Msg))
        {
            //No numbers. Safe to continue
            beneName = req.Msg.Trim();
        }
        else
        {
            //redirect
            addParam("cnt", "101", req);
            req.next_op = 2569;
            return "0";
        }

        //Get Beneficiary list
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetBeneList");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@sessionid", req.SessionID);
        c.AddParam("@beneName", beneName);
        c.AddParam("@bankID", 0);//0 for inter bank
        int cn = c.ExecuteProc();
        if (cn > 0)
        {
            //Continue to display list of beneficiaries matching the name
        }
        else
        {
            //redirect
            removeParam("BeneSelected", req);
            addParam("cnt", "101", req);
            req.next_op = 2565;
            return "0";
        }

        return "0";
    }
    public string ListInterBeneInfo(UReq req)
    {
        Gadget g = new Gadget(); int cntfirst = 0;
        string resp = "";
        try
        {
            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);

            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            List<BeneList> lb = g.GetBeneListByPage(page.ToString(), req.SessionID);
            if (lb.Count <= 0)
            {
                removeParam("BENENXTPAG", req);
                lb = g.GetBeneListByPage("0", req.SessionID);
            }

            foreach (BeneList item in lb)
            {
                cntfirst += 1;
                if (cntfirst == 1)
                {
                    resp = item.TransRate + " " + item.BeneName + " (" + item.Nuban + "-" + item.BeneBank + ")";
                }
                else
                {
                    int acctListNo = Convert.ToInt16(item.TransRate);
                    resp += "%0A" + acctListNo + " " + item.BeneName + " (" + item.Nuban + "-" + item.BeneBank + ")";
                }
            }

            if (cntfirst < 2)
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
        return "Select Beneficiary%0A" + resp;
    }
    public int SaveInterBeneSelected(object obj)
    {
        Gadget g = new Gadget();
        //new ErrorLog("I entered");
        UReq req = (UReq)obj;
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        if (req.Msg == "0")
        {
            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            page++;
            removeParam("BENENXTPAG", req);
            addParam("BENENXTPAG", page.ToString(), req);
            return 99;
        }
        //check to ensure that the number entered is not 
        int origCnt = g.GetNoofAccts(req.Msisdn);
        try
        {
            if (int.Parse(req.Msg) > origCnt)
            {
                int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
                page++;
                removeParam("BENENXTPAG", req);
                addParam("BENENXTPAG", page.ToString(), req);
                return 99;
            }
        }
        catch
        {
            int page = g.RunBillerNextPage(prm["BENENXTPAG"]);
            page++;
            removeParam("BENENXTPAG", req);
            addParam("BENENXTPAG", page.ToString(), req);
            return 99;
        }

        addParam("BENEITEMSELECT", req.Msg, req);
        addParam("B", "1", req);
        return 1;
    }
    public string DisplaySummaryInter(UReq req)
    {
        string resp = ""; decimal amt = 0; string frmNuban = ""; string theResp = "";
        string prms = getParams(req); int flag = 0; Gadget g = new Gadget();
        NameValueCollection prm = splitParam(prms);
        string SessionID = req.SessionID; char[] sep = { ':' };
        theResp = DoNameEnquiry(prm["TONUBAN"], int.Parse(prm["TOBANK"]));
        amt = decimal.Parse(prm["AMOUNT"]);
        string[] bits = theResp.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        frmNuban = prm["NUBAN"];
        addParam("FROMACCT", frmNuban, req);
        if (frmNuban == "-1" || frmNuban == "")
        {
            return "You are currently not registered with us for USSD service.  Dial *822*25#";
        }
        addParam("NERESPONSE", bits[0], req);
        if (bits[0] == "00")
        {
            //adding params for saving beneficiary
            addParam("ToName", bits[1], req);
            addParam("BankName", bits[3], req);

            resp = "You are transferring the sum of " + getRealMoney(amt) + " from " + frmNuban + " to " + bits[1] + "(" + bits[3] + ")" + "%0A1.Yes%0A2.No";
        }
        else
        {
            resp = getRespDesc(bits[0]) + " kindly try again later.";
            return "Error occured: " + resp;
        }
        return "Confirmation: " + resp;
    }
    //-------Referenced in DisplaySummaryInter-------------
    //
    public string DoNameEnquiry(string toAcct, int tobankcode)
    {
        Gadget g = new Gadget(); string RandomNum = ""; string SessionID = ""; string resp = ""; string Dest_bank_code = "";
        RandomNum = g.GenerateRndNumber(3);
        SessionID = g.newSessionGlobal(RandomNum, 1);
        Dest_bank_code = getTheBankCode(tobankcode.ToString());
        string ToName = "";
        char[] sep = { '*' };
        string[] bits = Dest_bank_code.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        //first check our local table 
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_nip");
        c.SetProcedure("spd_Get_NameEnquiry_From_Table0504");
        c.AddParam("@nuban", toAcct);
        c.AddParam("@bankcode", bits[0]);

        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            return "00:" + dr["AccountName"].ToString() + ":" + " " + ":" + bits[1];
        }
        else
        {

            //call NIBSS based on the toAcct number
            NIPNameEnquiry.NewIBSSoapClient ws = new NIPNameEnquiry.NewIBSSoapClient();
            resp = ws.NameEnquiry(SessionID, bits[0], "3", toAcct);
            resp = resp.Replace("::", ":");
            resp = resp.Replace(":1", ":");
        }
        return resp + ":" + bits[1];
    }
    public string getTheBankCode(string bcode)
    {
        string bcodeval = "";
        try
        {
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_nip");
            cn.SetProcedure("spd_GetParticipatingBanksByID");
            cn.AddParam("@id", bcode);
            DataSet ds = cn.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                bcodeval = dr["bankcode"].ToString() + "*" + dr["bankname"].ToString();
            }
        }
        catch { }
        return bcodeval;
    }
    public string getRespDesc(string rcode)
    {
        string resptext = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn_nip");
        c.SetProcedure("spd_GetResponseDesc");
        c.AddParam("@respCode", rcode);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            resptext = dr["respText"].ToString();
        }
        return resptext;
    }
    public void Upd_acct_byTranstype(string sessionid, int val)
    {
        string sql = "update tbl_USSD_account_id set trans_type=@tt where sessionid = @sid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@tt", val);
        cn.AddParam("@sid", sessionid);
        cn.Update();
    }
    //------------------------------------------------------
    //
    public string SaveSummaryInter(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        try
        {
            string nuban = prm["NUBAN"];
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
                            cnt = 101;
                            addParam("Bypass", cnt.ToString(), req);
                            req.next_op = 2516;
                            return "0";
                        }
                        if (activated == 1)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                //customer is okay to continue
            }

            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")
            {
                req.next_op = 2579;
            }
        }
        catch
        {
            resp = "0";
        }
        return resp;
    }
    public string CollectPINForInterTxn(UReq req)
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
            resp = "The USSD PIN you provided is incorrect. Please check and try again.";
            return "Authentication:%0A" + resp;
        }
        resp = "Enter your USSD PIN";
        return "Authentication:%0A" + resp;
    }
    public string SavetxnPINForInterTxn(UReq req)
    {
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2575;
            return "0";
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
            req.next_op = 2575;
            return "0";
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
                //submit request to transfer is amount less than 20K

                int amt = RunCount(prm["AMOUNT"]);
                if (amt < 20000)
                {
                    //check is bene was selected 
                    int flag = RunCount(prm["BeneSelected"]);
                    if (flag == 1)
                    {
                        //Beneficiary was selected. No reply needed
                        req.next_op = 2579;
                        return "1";
                    }
                    else
                    {
                        //Beneficiary 
                        req.next_op = 2579;
                        return "0";
                    }
                }
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
            req.next_op = 2575;
            return "0";
        }

        return resp;
    }
    public string CollectPIN2ForInterTxn(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter your USSD PIN 2";
            return "Authentication:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 4 digits";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 digits and not alphanumeric";
            return "Authentication for PIN 2:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "The USSD PIN 2 you provided is incorrect. Please check and try again.";
            return "Authentication for PIN 2:%0A" + resp;
        }
        resp = "Enter your USSD PIN 2";
        return "Authentication:%0A" + resp;
    }
    public string SavetxnPIN2ForInterTxn(UReq req)
    {
        int cnt = 0;
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2577;
            return "0";
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
            req.next_op = 2577;
            return "0";
        }
        Encrypto EnDeP = new Encrypto();
        req.Msg = EnDeP.Encrypt(req.Msg);

        //check the registration table to confirm if PIN entered is correct
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetProcedure("spd_GetCusRegPIN2");
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@auth", req.Msg);
        DataSet ds = c.Select("rec");
        string resp = "0";
        if (ds.Tables[0].Rows.Count > 0)
        {

            try
            {
                addParam("PIN2", req.Msg, req);
                string prms = getParams(req);
                NameValueCollection prm = splitParam(prms);
                //check is bene was selected 
                int flag = RunCount(prm["BeneSelected"]);
                if (flag == 1)
                {
                    //Beneficiary was selected. No reply needed
                    req.next_op = 2579;
                    return "1";
                }
                else
                {
                    //Beneficiary 
                    req.next_op = 2579;
                    return "0";
                }
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
            req.next_op = 2577;
            return "0";
        }

        return resp;
    }
    public string SubmitInterTxn(UReq req)
    {
        string resp = ""; string PIN = ""; string PIN2 = "";
        int k = 0;
        try
        {

            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            //prm["AMOUNT"]
            int summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*25# to try again";
                return resp;
            }

            PIN = prm["PIN"];

            try
            {
                PIN2 = prm["PIN2"];
            }
            catch
            {

            }

            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,custauthid2,trans_type,sessionid,bankcode,frmAccount) " +
                " values(@mb,@ta,@am,@pn,@pn2,7,@sid,@bc,@frm)";
            //string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,bankcode,frmAccount) " +
            //    " values(@mb,@ta,@am,@pn,2,@sid,@bc,@frm)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@pn2", PIN2 ?? "");
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@bc", prm["TOBANK"]);
            cn.AddParam("@frm", prm["FROMACCT"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            return resp = "ERROR: Could not contact core banking system " + ex.ToString();
        }

        if (k > 0)
        {
            Upd_acct_byTranstype(req.SessionID, 7);
            //resp = "Transaction has been submitted for processing";
            resp = "Your transaction has been submitted, do you want to save the beneficiary?%0A1.Yes%0A2.No";
        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }
    public string SubmitInterTxnNoReply(UReq req)
    {
        string resp = ""; string PIN = ""; string PIN2 = "";
        int k = 0;
        try
        {

            string prms = getParams(req);
            NameValueCollection prm = splitParam(prms);
            //prm["AMOUNT"]
            int summary = int.Parse(prm["SUMMARY"]);
            if (summary == 2)
            {
                resp = "Transaction cancelled. Dial *822*25# to try again";
                return resp;
            }

            PIN = prm["PIN"];

            try
            {
                PIN2 = prm["PIN2"];
            }
            catch
            {

            }

            string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,custauthid2,trans_type,sessionid,bankcode,frmAccount) " +
                " values(@mb,@ta,@am,@pn,@pn2,7,@sid,@bc,@frm)";
            //string sql = "Insert into tbl_USSD_transfers (mobile,toAccount,amt,custauthid,trans_type,sessionid,bankcode,frmAccount) " +
            //    " values(@mb,@ta,@am,@pn,2,@sid,@bc,@frm)";
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
            cn.SetSQL(sql);
            cn.AddParam("@mb", req.Msisdn);
            cn.AddParam("@ta", prm["TONUBAN"]);
            cn.AddParam("@am", prm["AMOUNT"]);
            cn.AddParam("@pn", PIN);
            cn.AddParam("@pn2", PIN2 ?? "");
            cn.AddParam("@sid", req.SessionID);
            cn.AddParam("@bc", prm["TOBANK"]);
            cn.AddParam("@frm", prm["FROMACCT"]);
            k = Convert.ToInt32(cn.Insert());

        }
        catch (Exception ex)
        {
            return resp = "ERROR: Could not contact core banking system " + ex.ToString();
        }
        if (k > 0)
        {
            Upd_acct_byTranstype(req.SessionID, 7);
            resp = "Transaction has been submitted for processing";

        }
        else
        {
            resp = "Unable to submit please try again";
        }

        return resp;
    }
    //+++++++++++++++++++++++++++ USSD PIN 1 & USSD PIN 2 Reset +++++++++++++++++++++++++
    //
    public string DisplayPINResetMenu(UReq req)
    {
        string resp = "";
        try
        {

            resp = "1 Reset PIN 1%0A2 Reset PIN 2";
            return "USSD PIN Reset Menu%0A" + resp;
        }
        catch
        {
            resp = "1 Reset PIN 1%0A2 Reset PIN 2";
            return "USSD PIN Reset Menu%0A" + resp;

        }
    }
    public string DirectToPinToReset(UReq req)
    {
        try
        {
            long pinSelected = long.Parse(req.Msg);
        }
        catch
        {
            req.next_op = 2558;
            return "0";
        }

        if (RunCount(req.Msg) > 2)
        {
            req.next_op = 2558;
            return "0";
        }

        if (RunCount(req.Msg) == 1)//Redirect to PIN 1
        {
            req.next_op = 2580;
            return "0";
        }

        if (RunCount(req.Msg) == 2)//Redirect to PIN 2
        {
            req.next_op = 2591;
            return "0";
        }

        req.next_op = 2580;
        return "0";
    }
    // ---------- USSD PIN 1 -----------
    //
    public string CollectNUBAN4PIN1(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        //int wrongCnt = RunCount(prm["wrongCnt"]);
        //if (wrongCnt == 3)
        //{
        //    removeParam("wrongCnt", req);
        //    resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
        //    return resp;
        //}
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your account number:";
            return "Collect NUBAN for PIN 1 reset:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length is not less than or greater than 10 digits";
            return "Collect NUBAN for PIN 1 reset:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 10 digits and not alphanumeric";
            return "Collect NUBAN for PIN 1 reset:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter the account number you registered for USSD service";
            return "Collect NUBAN for PIN 1 reset:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Your phone number and account number do not match.";
            return resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            resp = "The account number you entered is not profiled with us.";

            return resp;
        }
        else if (cnt == 106)
        {
            removeParam("cnt", req);
            resp = "Sorry, an error occurred while getting your details";
            return resp;
        }

        resp = "Kindly enter your account number";
        return "Collect NUBAN for PIN 1 reset:%0A" + resp;
    }
    public string SaveNuban4PIN1(UReq req)
    {
        string resp = "0";
        int cnt = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = req.Msg.Trim();
        try
        {
            //check if entry is numeric
            try
            {
                long acctNo = long.Parse(req.Msg);
            }
            catch
            {
                cnt = 101;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2580;
                return "0";
            }

            //check to ensure the lenght of the digits entered is not less or more than 11
            if (req.Msg.Length < 10 || req.Msg.Length > 10)
            {
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2580;
                return "0";
            }

            //check if the account number is profiled
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
                    if (nuban != regisNuban)
                    {
                        cnt = -1;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                if (cnt == -1)
                {
                    cnt = 102;
                    addParam("cnt", cnt.ToString(), req);
                    req.next_op = 2580;
                    return "0";
                }

            }
            else
            {
                //Redirect to register.
                req.next_op = 2510;
                return "0";
            }

            var accountInfo = GetAccountInfoWithAcct(nuban, req.SessionID);
            if (!accountInfo.result)
            {
                cnt = 104;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2580;
                return "0";
            }

            if (accountInfo.PhoneNumber != req.Msisdn)
            {
                cnt = 103;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2580;
                return "0";
            }

            //check if there is an active card
            var cardCheck = CheckForValidCard(nuban, req.SessionID);
            if (!cardCheck.result)
            {
                //this person doesn't have an active card.
                cnt = 111;
                addParam("Bypass", cnt.ToString(), req);
                addParam("cnt", cnt.ToString(), req);
            }

            addParam("NUBAN", nuban, req);

        }
        catch
        {
            cnt = 106;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2580;
            return "0";
        }

        return resp;
    }
    public string CollectBVN4PIN1(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        //int wrongCnt = RunCount(prm["wrongCnt"]);
        //if (wrongCnt == 3)
        //{
        //    removeParam("wrongCnt", req);
        //    resp = "Kindly call your One-Customer Centre on 070078375464 or visit any of our branches to complete your PIN reset. Thank you";
        //    return resp;
        //}
        if (cnt == 0 || cnt == 111)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Kindly enter your BVN";
            return "Bank Verification Number for PIN 1:%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of BVN is not less than or greater than 11 digits";
            return "Bank Verification Number for PIN 1:%0A" + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 11 digits and not alphanumeric";
            return "Bank Verification Number for PIN 1:%0A" + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Oops! The BVN you have entered does not match what we have in our records. Please enter the correct BVN and try again.";
            return "Bank Verification Number for PIN 1:%0A" + resp;
        }
        else if (cnt == 103)
        {
            removeParam("cnt", req);
            resp = "Sorry, an error occurred treating your request";
            return "Bank Verification Number for PIN 1:%0A " + resp;
        }
        //else if (cnt == 104)
        //{
        //    removeParam("cnt", req);
        //    resp = "This phone number doesn't match your BVN profile";
        //    return "Bank Verification Number: " + resp;
        //}
        resp = "Kindly enter your BVN";
        return "Bank Verification Number for PIN 1:%0A" + resp;
    }
    public string SaveBVN4PIN1(UReq req)
    {
        string prms = getParams(req);
        int cnt = 0; string resp = "0";
        NameValueCollection prm = splitParam(prms);

        string nuban = prm["NUBAN"];
        string bvn = req.Msg.Trim();
        try
        {
            //check to ensure the lenght of the digits entered is not less or more than 11
            if (req.Msg.Length != 11)
            {
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2582;
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
                req.next_op = 2582;
                return "0";
            }

            var accountInfo = new GetAccountByNubanResp();
            accountInfo = GetAccountInfoWithAcct(nuban, req.SessionID);

            if (accountInfo.BvnId != bvn)//VALIDATE BVN FROM GATEWAY
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2582;
                return "0";
            }

            cnt = RunCount(prm["Bypass"]);//Send straight to set pin .. no valid card to input
            if (cnt == 111)
            {
                req.next_op = 2586;
                return "0";
            }

            //Else continue to enter last 6 digits
        }
        catch
        {
            cnt = 103;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2582;
            return "0";
        }
        return resp;
    }
    public string CollectLast6Digits4PIN1(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
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
    public string SaveLastSixDigits4PIN1(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];

        try
        {
            long last6digits = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2584;
            return "0";
        }
        if (req.Msg.Length != 6)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2584;
            return "0";
        }

        nuban = removeSpareParams(nuban);

        var cardCheck = ValidateCardPan(nuban, req.Msg, req.SessionID);
        if (!cardCheck.result)
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2584;
            return "0";
        }

        return resp;
    }
    public string SetUserPIN4PIN1(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter any 4 digit of your choice";
            return "PIN 1 Reset: " + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the required length of the PIN is not less than or greater than 4 digits";
            return "PIN 1 Reset: " + resp;
        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure you enter only 4 numeric digits and not alphanumeric";
            return "PIN 1 Reset: " + resp;
        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure your PIN 1 is different from PIN 2";
            return "PIN 1 Reset:%0A" + resp;
        }

        resp = "Kindly enter any 4 digits of your choice";
        return "PIN 1 Reset: " + resp;
    }
    public string SaveCusPIN4PIN1(UReq req)
    {
        int cnt = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        string nuban = prm["NUBAN"];
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2586;
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
            req.next_op = 2586;
            return "0";
        }

        //Check to ensure PIN 1 is different from PIN 2
        string pin1 = req.Msg;
        string PIN2 = getUSSDPin2(req.Msisdn, nuban);
        if (pin1 == PIN2)
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2586;
            return "0";
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
    public string ConfirmSetUserPIN4PIN1(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
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
    public string SaveConfrimPIN4PIN1(UReq req)
    {
        int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2588;
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
            req.next_op = 2588;
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
            req.next_op = 2588;
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
    public string DoPINReset4PIN1(UReq req)
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
                        //SendSms(smsMessage, req.Msisdn);
                        smsMessage = "Hello, your PIN reset was successful! If this isn't you, please contact our one customer centre on 070078375464 immediately ";
                        resp = "Great! PIN reset successful. Continue to enjoy the Magic of *822*25#";
                    }
                    else
                    {
                        //smsMessage = "Dear " + custName + ", your PIN reset was successful." + Environment.NewLine + "Not you? Please contact our one customer centre on +23470078375464  or via customercare@sterlingbankng.com immediately";
                        //resp = "Great! Your PIN reset was successful. Continue to enjoy the benefits of *822#";
                        smsMessage = "Hello, your PIN reset was successful! If this isn't you, please contact our one customer centre on 070078375464 immediately";
                        //SendSms(smsMessage, req.Msisdn);
                        resp = "Great! Your PIN reset was successful. Continue to enjoy the Magic of *822*25#";
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
    // ------------ USSD PIN 2 -----------
    //
    public string CollectNUBAN4PIN2(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
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
            resp = "To complete your PIN 2 reset, kindly go to the nearest branch to request for a debit card.%0AThank you";
            return "Collect NUBAN for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 104)
        {
            removeParam("cnt", req);
            // resp = "The account number you entered is incorrect";
            resp = "The account number you entered is not profiled with us.";
            return "Collect NUBAN for PIN 2 reset:%0A" + resp;
        }
        else if (cnt == 105)
        {
            removeParam("cnt", req);
            resp = "Your phone number and the account number provided do not match.";
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
    public string SaveNuban4PIN2(UReq req)
    {
        int cnt = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
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
            req.next_op = 2591;
            return "0";
        }

        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length != 10)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2591;
            return "0";
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
                    cnt = 0;
                    break;
                }
            }

            if (cnt == -1)
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2591;
                return "0";
            }

        }
        else
        {
            //Redirect to register.
            req.next_op = 2510;
            return "0";
        }

        var accountInfo = GetAccountInfoWithAcct(nuban, req.SessionID);
        if (!accountInfo.result)
        {
            cnt = 104;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2591;
            return "0";
        }

        if (accountInfo.PhoneNumber != req.Msisdn)
        {
            cnt = 105;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2591;
            return "0";
        }

        //check if there is an active card
        var cardCheck = CheckForValidCard(nuban, req.SessionID);
        if (!cardCheck.result)
        {
            //this person doesn't have an active card.
            cnt = 111;
            addParam("Bypass", cnt.ToString(), req);
            addParam("cnt", cnt.ToString(), req);
        }

        addParam("NUBAN", nuban, req);


        return resp;
    }
    public string CollectBVN4PIN2(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
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
    public string SaveBVN4PIN2(UReq req)
    {
        string prms = getParams(req);
        int cnt = 0; string resp = "0";
        NameValueCollection prm = splitParam(prms);

        string nuban = prm["NUBAN"];
        string bvn = req.Msg.Trim();
        try
        {
            //check to ensure the lenght of the digits entered is not less or more than 11
            if (req.Msg.Length != 11)
            {
                cnt = 100;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2593;
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
                req.next_op = 2593;
                return "0";
            }

            var accountInfo = new GetAccountByNubanResp();
            accountInfo = GetAccountInfoWithAcct(nuban, req.SessionID);

            if (accountInfo.BvnId != bvn)//VALIDATE BVN FROM GATEWAY
            {
                cnt = 102;
                addParam("cnt", cnt.ToString(), req);
                req.next_op = 2593;
                return "0";
            }

            cnt = RunCount(prm["Bypass"]);//Send straight to set pin .. no valid card to input
            if (cnt == 111)
            {
                req.next_op = 2597;
                return "0";
            }

            //Else continue to enter last 6 digits
        }
        catch
        {
            cnt = 103;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2593;
            return "0";
        }
        return resp;
    }
    public string CollectLast6Digits4PIN2(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        if (cnt == 0)
        {
            //first entrance
            removeParam("cnt", req);
            resp = "Enter last 6 digits of your card:";
            return "Validation for USSD PIN 2%0A" + resp;
        }
        else if (cnt == 100)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is exactly 6 digits";
            return "Validation for USSD PIN 2%0A" + resp;

        }
        else if (cnt == 101)
        {
            removeParam("cnt", req);
            resp = "Kindly ensure that the number is not alphanumeric";
            return "Validation for USSD PIN 2%0A" + resp;

        }
        else if (cnt == 102)
        {
            removeParam("cnt", req);
            resp = "Oops! You have entered the last 6 digits of your Debit Card wrongly. Please check your Card and try again.";
            return "Validation for USSD PIN 2%0A" + resp;
        }

        resp = "Enter last 6 digits of your card:";
        return resp;
    }
    public string SaveLastSixDigits4PIN2(UReq req)
    {
        string resp = "0"; int cnt = 0;
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];

        try
        {
            long last6digits = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2595;
            return "0";
        }
        if (req.Msg.Length != 6)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2595;
            return "0";
        }

        nuban = removeSpareParams(nuban);

        var cardCheck = ValidateCardPan(nuban, req.Msg, req.SessionID);
        if (!cardCheck.result)
        {
            cnt = 102;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2595;
            return "0";
        }

        return resp;
    }
    public string SetUserPIN4PIN2(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
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
    public string SaveCusPIN4PIN2(UReq req)
    {
        int cnt = 0; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int wrongCnt = RunCount(prm["wrongCnt"]);
        //check to ensure the lenght of the digits entered is not less or more than 4
        if (req.Msg.Length < 4 || req.Msg.Length > 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2597;
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
            req.next_op = 2597;
            return "0";
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
                req.next_op = 2597;
                return "0";
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
    public string ConfirmSetUserPIN4PIN2(UReq req)
    {
        string resp = ""; string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
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
    public string SaveConfrimPIN4PIN2(UReq req)
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
            req.next_op = 2599;
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
            req.next_op = 2599;
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
            req.next_op = 2599;
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
    public string DoPINReset4PIN2(UReq req)
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
                        //SendSms(smsMessage, req.Msisdn);
                        smsMessage = "Hello, your PIN 2 reset was successful! If this isn't you, please contact our one customer centre on 070078375464 immediately ";
                        resp = "Great! PIN 2 reset successful. Continue to enjoy the Magic of *822*25#";
                    }
                    else
                    {
                        //smsMessage = "Dear " + custName + ", your PIN reset was successful." + Environment.NewLine + "Not you? Please contact our one customer centre on +23470078375464  or via customercare@sterlingbankng.com immediately";
                        //resp = "Great! Your PIN reset was successful. Continue to enjoy the benefits of *822#";
                        //SendSms(smsMessage, req.Msisdn);
                        smsMessage = "Hello, your PIN 2 reset was successful! If this isn't you, please contact our one customer centre on 070078375464 immediately";
                        resp = "Great! Your PIN 2 reset was successful. Continue to enjoy the Magic of *822*25#";
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
    // ++++++++++++++++++++++++++++++ Disable Card +++++++++++++++++++++++++++++++++++
    //
    public string SaveDisableCardReq(UReq req)
    {
        string resp = "0"; int cnt = 0;
        int activated = -1; string nuban = "";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_getRegisteredUserProfile");
        c.AddParam("@mobile", req.Msisdn.Trim());
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            activated = int.Parse(dr["Activated"].ToString());
            nuban = dr["Nuban"].ToString();
            addParam("NUBAN", nuban, req);
            if (activated != 1)
            {
                //redirect to set PIN (WISEBANK)
                req.next_op = 2516;
                return "0";
            }
        }
        else
        {
            //redirect to register and set PIN (WISEBANK)
            req.next_op = 2510;
            return "0";
        }

        //check if there is an active card
        var cardCheck = CheckForValidCard(nuban, req.SessionID);
        if (!cardCheck.result)
        {
            //this person doesn't have an active card.
            cnt = 1;
            addParam("NoCard", cnt.ToString(), req);
            req.next_op = 2605;
            return "0";
        }

        return resp;
    }
    public string ShowDisableCardSummary(UReq req)
    {
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        int cnt = RunCount(prm["cnt"]);
        string nuban = prm["NUBAN"];

        resp = "Are you sure you want to disable your card linked to " + nuban + "%0A1 Yes%0A2 No";
        return resp;
    }
    public string SaveDisableCardSumm(UReq req)
    {
        string resp = "0";

        var regexItem = new Regex("^[0-9]*$");
        if (!regexItem.IsMatch(req.Msg) || int.Parse(req.Msg) > 2)
        {
            req.next_op = 2603;
            return "0";
        }
        else
        {
            addParam("SUMMARY", req.Msg, req);
            if (req.Msg == "2")//Route to final step
            {
                req.next_op = 2607;
                return "0";
            }
        }

        return resp;
    }
    public string CollectPINToDisableCard(UReq req)
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
            resp = "The USSD PIN you provided is incorrect. Please check and try again.";
            return "Authentication:%0A" + resp;
        }
        resp = "Enter your USSD PIN";
        return "Authentication:%0A" + resp;
    }
    public string SavePINToDisableCard(UReq req)
    {
        int cnt = 0; string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);

        //check if entry is numeric
        try
        {
            long pin = long.Parse(req.Msg);
        }
        catch
        {
            cnt = 101;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2605;
            return "0";
        }
        //check to ensure the lenght of the digits entered is not less or more than 11
        if (req.Msg.Length != 4)
        {
            cnt = 100;
            addParam("cnt", cnt.ToString(), req);
            req.next_op = 2605;
            return "0";
        }

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
            req.next_op = 2605;
            return "0";
        }

        return resp;
    }
    public string DisableCard(UReq req)
    {
        string resp = "";
        string prms = getParams(req);
        NameValueCollection prm = splitParam(prms);
        string nuban = prm["NUBAN"];
        string summary = prm["SUMMARY"];
        string noCard = prm["NoCard"];

        if (summary == "2")
        {
            resp = "Operation cancelled. Thank you for banking for us";
            return resp;
        }

        if (!string.IsNullOrEmpty(noCard))
        {
            resp = "There is no active card linked to your account.";
            return resp;
        }

        var disableResp = DisableCard(nuban);
        if (!disableResp.result)
        {
            resp = "Sorry, your card wasn't disabled. Please try again later";
            return resp;
        }
        else
        {
            resp = "Your card was successfully disabled";
            return resp;
        }

    }

}