using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for bankTeller
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class bankTeller : System.Web.Services.WebService {

    [WebMethod]
    public string NameEnquiry(string SessionID, string DestinationBankCode, string ChannelCode, string AccountNumber)
    {
        string msg = "";

        string responsecodeVal = "";
        string AcctNameval = ""; string BankVerificationNumber = ""; string KYCLevel = "";
        if (AccountNumber.Length > 10)
        {
            return "07" + ":" + AcctNameval + ":" + BankVerificationNumber + ":" + KYCLevel;
        }

        try
        {
            decimal acct = decimal.Parse(AccountNumber);
        }
        catch
        {
            return "07" + ":" + AcctNameval + ":" + BankVerificationNumber + ":" + KYCLevel;
        }
        TR_SingleNameEnquiry sne = new TR_SingleNameEnquiry();
        sne.SessionID = SessionID;
        sne.DestinationInstitutionCode = DestinationBankCode;
        sne.ChannelCode = ChannelCode;
        sne.AccountNumber = AccountNumber;
        sne.createRequest();

        if (!sne.sendRequest()) //unsuccessful request
        {
            responsecodeVal = sne.ResponseCode;
            BankVerificationNumber = sne.BankVerificationNumber;
            KYCLevel = sne.KYCLevel;
            AcctNameval = "";
            //msg = responsecodeVal + ":" + AcctNameval;
            msg = responsecodeVal + ":" + AcctNameval + ":" + BankVerificationNumber + ":" + KYCLevel;
            //new ErrorLog(msg);
            Mylogger1.Info(msg);
        }
        else
        {
            responsecodeVal = sne.ResponseCode;
            AcctNameval = sne.AccountName;
            BankVerificationNumber = sne.BankVerificationNumber;
            KYCLevel = sne.KYCLevel;

            msg = responsecodeVal + ":" + AcctNameval + ":" + BankVerificationNumber + ":" + KYCLevel;
            //AcctNameval = sne.AccountName;
            //msg = responsecodeVal + ":" + AcctNameval;
            //new ErrorLog(msg);
            Mylogger1.Info(msg);
        }
        return msg;
    }

    [WebMethod]
    public string DebitandSendtoNIBSS(string sessionid, string bracodeval, string cusnumval, string curcodeval,
        string ledcodeval, string subacctval, string amt, string fee, string orignatorName, string DestinationBankCode,
        string ChannelCode, string AccountName, string AccountNumber, string paymentRef, string cusshowname, string tellerid,
        string BeneficiaryBankVerificationNumber,string BeneficiaryKYCLevel, string OriginatorAccountNumber, 
        string OriginatorBankVerificationNumber, string OriginatorKYCLevel, string TransactionLocation, string NameEnquiryRef)
    {


        string imalTSS = string.Empty;
        string imalFeeAcct = string.Empty;
        string imalVatAcct = string.Empty;


        //check if the name enquiry session exists already
        Connect cnSNE = new Connect("spd_checkExistSessionNE");
        cnSNE.addparam("@sessionne", sessionid);
        cnSNE.query();
        if (cnSNE.returnValue > 0)
        {
            return "6:0"; //
        }
        if (AccountNumber.Length > 10)
        {
            return "07:" + "Invalid Account";
        }

        try
        {
            decimal acctval = decimal.Parse(AccountNumber);
        }
        catch
        {
            return "07:" + "Invalid Account";
        }
        string vatNUBAN = ""; string feeNUBAN = ""; string NIPAcctNUBAN = "";
        string msg;
        string ledger = "";
        string logval = "0";
        Gadget g = new Gadget();
        AccountService acs = new AccountService();
        Transaction t = new Transaction();
        TransactionService tsv = new TransactionService();
        string sessionidNE = "";
        sessionidNE = sessionid;//get session id from the already done NE
        t.bra_code = bracodeval;
        t.cusnum = cusnumval;
        t.curcode = curcodeval;
        t.ledcode = ledcodeval;
        t.subacctcode = subacctval;
        t.paymentRef = paymentRef;
        t.amount = decimal.Parse(amt);
        t.destinationcode = DestinationBankCode;
        t.transactionCode = g.newTrnxRef(bracodeval);
        //orignatorName = orignatorName.Replace("&amp;amp;", "&amp;");
        t.originatorname = orignatorName;
        t.feecharge = decimal.Parse(fee);
        t.AccountName = AccountName;
        t.AccountNumber = AccountNumber;
        t.channelCode = Convert.ToInt16(ChannelCode);
        t.sessionid = sessionidNE;
        t.nuban = OriginatorAccountNumber;
        string acct = bracodeval + cusnumval + curcodeval + ledcodeval + subacctval;


        string nuban = "";
        decimal avail_bal = 0;
        //sbp.banks b = new sbp.banks();
        EACBS.banks b = new EACBS.banks();
        ImalWS.Service imal = new ImalWS.Service();
        SwitchCBA sww = new SwitchCBA();
        int cbatype;
        //get balance by customerNuban
        cbatype = sww.GetCBATypeByStartingDigits(OriginatorAccountNumber);
        Mylogger1.Info("just finished calling get cbatype for " + nuban + ", sessionid " + sessionid);
        switch (cbatype)
        {
            case 1:

                //get balance by customerNuban

                DataSet ds = new DataSet();
                try
                {
                    ds = b.getAccountFullInfo(OriginatorAccountNumber); //b.getBalanceDetails1(t.bra_code, t.cusnum, t.curcode, t.ledcode, t.subacctcode);
                }
                catch (Exception ex)
                {
                    new ErrorLog("Error retrieving balance for NIP at branch for account number " + OriginatorAccountNumber + " the error" + ex);
                }
                //get customer details from T24
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    avail_bal = decimal.Parse(dr["UsableBal"].ToString());
                    t.ledcode = dr["T24_LED_CODE"].ToString();
                    nuban = dr["NUBAN"].ToString();

                    new ErrorLog("Balance gotten for account number " + OriginatorAccountNumber + " for T24 as at the time of transaction is " + avail_bal.ToString());
                }


                break;
            case 2:
                DataSet dss = imal.GetAccountByAccountNumber(OriginatorAccountNumber);
                if (dss.Tables[0].Rows.Count > 0)
                {
                    DataRow drr = dss.Tables[0].Rows[0];
                    bracodeval = drr["BRANCHCODE"].ToString();
                    //cusnum = drr["cus_num"].ToString();
                    nuban = OriginatorAccountNumber;
                    //currency = dr["cur_code"].ToString();
                    t.bra_code = bracodeval;
                    //currency = drr["CURRENCYCODE"].ToString();
                    t.ledcode = drr["LED_CODE"].ToString();
                    //subacct = dr["sub_acct_code"].ToString();
                    //subacct = "";
                    avail_bal = decimal.Parse(drr["AVAIL_BAL"].ToString());
                    //CUS_SHO_NAME = drr["CUSTOMERNAME"].ToString();
                    Mylogger.Info("just finished calling getaccountinfo for imal  to do imal tx for nuban " + nuban + ", sessionid is " + sessionid);
                    new ErrorLog("just finished calling getaccountinfo for imal  to do imal tx for nuban " + nuban + ", sessionid is " + sessionid);
                }
                break;
            default:
                break;
        }

        int Last4 = 0; string TSSAcct = "";
        string sqlcons = "";
        sqlcons = "select bra_code,cus_num,cur_code,led_code,sub_acct_code,concessionfee,addedby,statusflag " +
    " from tbl_nipconcession where nuban = @nu and statusflag = 1";
        //sqlcons = "select bra_code,cus_num,cur_code,led_code,sub_acct_code,concessionfee,addedby,statusflag " +
        //    " from tbl_nipconcession where bra_code=@bc and cus_num =@cu and cur_code=@cr and led_code=@lc " +
        //    " and sub_acct_code =@sc and statusflag = 1";
        Connect conS = new Connect(sqlcons, true);
        conS.addparam("@nu", OriginatorAccountNumber);
        //conS.addparam("@bc", bracodeval);
        //conS.addparam("@cu", cusnumval);
        //conS.addparam("@cr", curcodeval);
        //conS.addparam("@lc", ledcodeval);
        //conS.addparam("@sc", subacctval);
        DataSet dscons = conS.query("rec");
        if (dscons.Tables[0].Rows.Count > 0)
        {
            DataRow drcon = dscons.Tables[0].Rows[0];
            t.feecharge = decimal.Parse(drcon["concessionfee"].ToString());
            if (acs.getNIPFee(t.amount))
            {
                bool foundval = g.isBankCodeFound(bracodeval);
                if (foundval)
                {
                    if (acs.getNIPFee(t.amount))
                    {
                        t.feecharge = acs.NIPfee;
                        t.vat = acs.NIPvat;
                        t.VAT_bra_code = "NG0020001";// t.bra_code;
                        t.VAT_cus_num = "";
                        t.VAT_cur_code = "NGN";
                        t.VAT_led_code = "17201";
                        Last4 = int.Parse(t.VAT_bra_code.Substring(6, 3)) + 2000;
                        TSSAcct = "NGN" + t.VAT_led_code + "0001" + Last4.ToString();
                        t.VAT_sub_acct_code = TSSAcct;

                        //t.VAT_sub_acct_code = b1.FormInternalAcct(t.VAT_bra_code, "NGN", t.VAT_led_code);
                        if (t.VAT_sub_acct_code == "")
                        {
                            msg = "Unable to form the VAT account for " + t.bra_code + " NGN ledcode 17201";
                            t.ResponseCode = "241";
                            tsv.InsertPartialOutgoing(t);
                            Mylogger1.Info(msg);
                            return "241:" + msg;
                        }
                        //new ErrorLog("Vat account formed for Branch " + t.VAT_bra_code+ " is " + t.VAT_sub_acct_code);
                        Mylogger1.Info("Vat account formed for Branch " + t.VAT_bra_code + " is " + t.VAT_sub_acct_code);
                    }
                    else
                    {
                        msg = "Error: Unable to compute VAT and Fee for account " + acct;
                        t.ResponseCode = "24";
                        tsv.InsertPartialOutgoing(t);
                        return "24:" + msg;
                    }
                }
                else
                {
                    if (acs.getNIPFee(t.amount))
                    {
                        t.feecharge = acs.NIPfee;
                        t.vat = acs.NIPvat;
                        t.VAT_bra_code = "NG0020001";
                        t.VAT_cus_num = "0";
                        t.VAT_cur_code = "NGN";
                        t.VAT_led_code = "17201";
                        Last4 = int.Parse(t.VAT_bra_code.Substring(6, 3)) + 2000;
                        TSSAcct = "NGN" + t.VAT_led_code + "0001" + Last4.ToString();
                        t.VAT_sub_acct_code = TSSAcct;

                        //t.VAT_sub_acct_code = b1.FormInternalAcct(t.VAT_bra_code, "NGN", t.VAT_led_code);
                        if (t.VAT_sub_acct_code == "")
                        {
                            msg = "Unable to form the VAT account for " + t.bra_code + " NGN ledcode 17201";
                            t.ResponseCode = "241";
                            tsv.InsertPartialOutgoing(t);
                            Mylogger1.Info(msg);
                            return "241:" + msg;
                        }
                        //new ErrorLog("Vat account formed for Branch " + t.VAT_bra_code + " is " + t.VAT_sub_acct_code);
                        Mylogger1.Info("Vat account formed for Branch " + t.VAT_bra_code + " is " + t.VAT_sub_acct_code);
                    }
                    else
                    {
                        msg = "Error: Unable to compute VAT and Fee for account " + acct;
                        t.ResponseCode = "24";
                        tsv.InsertPartialOutgoing(t);
                        return "24:" + msg;
                    }
                }
            }
        }
        else
        {
            //get The new fee and VAT
            if (acs.getNIPFee(t.amount))
            {
                bool foundval = g.isBankCodeFound(bracodeval);
                if (foundval)
                {
                    if (acs.getNIPFee(t.amount))
                    {
                        t.feecharge = acs.NIPfee;
                        t.vat = acs.NIPvat;
                        t.VAT_bra_code = "NG0020001";// t.bra_code;
                        t.VAT_cus_num = "";
                        t.VAT_cur_code = "NGN";
                        t.VAT_led_code = "17201";
                        Last4 = int.Parse(t.VAT_bra_code.Substring(6, 3)) + 2000;
                        TSSAcct = "NGN" + t.VAT_led_code + "0001" + Last4.ToString();
                        t.VAT_sub_acct_code = TSSAcct;
                        //t.VAT_sub_acct_code = b1.FormInternalAcct(t.VAT_bra_code, "NGN", t.VAT_led_code);
                        if (t.VAT_sub_acct_code == "")
                        {
                            msg = "Vat account formed for Branch " + t.VAT_bra_code + " is " + t.VAT_sub_acct_code;
                            t.ResponseCode = "241";
                            tsv.InsertPartialOutgoing(t);
                            Mylogger1.Info(msg);
                            return "241:" + msg;
                        }
                        //new ErrorLog("Vat account formed for Branch " + t.VAT_bra_code + " is " + t.VAT_sub_acct_code);
                        Mylogger1.Info("Vat account formed for Branch " + t.VAT_bra_code + " is " + t.VAT_sub_acct_code);
                    }
                    else
                    {
                        msg = "Error: Unable to compute VAT and Fee for account " + acct;
                        t.ResponseCode = "24";
                        tsv.InsertPartialOutgoing(t);
                        return "24:" + msg;
                    }
                }
                else
                {
                    if (acs.getNIPFee(t.amount))
                    {
                        t.feecharge = acs.NIPfee;
                        t.vat = acs.NIPvat;
                        t.VAT_bra_code = "NG0020001";
                        t.VAT_cus_num = "0";
                        t.VAT_cur_code = "NGN";
                        t.VAT_led_code = "17201";
                        Last4 = int.Parse(t.VAT_bra_code.Substring(6, 3)) + 2000;
                        TSSAcct = "NGN" + t.VAT_led_code + "0001" + Last4.ToString();
                        t.VAT_sub_acct_code = TSSAcct;
                        //t.VAT_sub_acct_code = b1.FormInternalAcct(t.VAT_bra_code, "NGN", t.VAT_led_code);
                        if (t.VAT_sub_acct_code == "")
                        {
                            //new ErrorLog("Unable to form the VAT account for " + t.bra_code + " NGN ledcode 17201");
                            Mylogger1.Info("Unable to form the VAT account for " + t.bra_code + " NGN ledcode 17201");
                        }
                        msg = "Vat account formed for Branch " + t.VAT_bra_code + " is " + t.VAT_sub_acct_code;
                        t.ResponseCode = "241";
                        tsv.InsertPartialOutgoing(t);
                        Mylogger1.Info(msg);
                    }
                    else
                    {
                        msg = "Error: Unable to compute VAT and Fee for account " + acct;
                        t.ResponseCode = "24";
                        tsv.InsertPartialOutgoing(t);
                        return "24:" + msg;
                    }
                }

            }
            else
            {
                t.ResponseCode = "24";
                tsv.InsertPartialOutgoing(t);
                msg = "Error: Unable to get the fee and vat";
                return msg;
            }
        }

        //let the core handle this
        //if (t.amount + t.vat + t.feecharge > avail_bal)
        //{
        //    //new ErrorLog("51:Insufficient funds (principal + vat + fee) for outward sessionid " + t.sessionid);
        //    Mylogger1.Info("51:Insufficient funds (principal + vat + fee) for outward sessionid " + t.sessionid);
        //    t.ResponseCode = "51";
        //    tsv.InsertPartialOutgoing(t);
        //    return "51:Insufficient funds (principal + vat + fee)";
        //}

        //t.vat = 37.5M;
        //t.VAT_bra_code = "900";
        //t.VAT_cus_num = "0";
        //t.VAT_cur_code = "1";
        //t.VAT_led_code = "4522";
        //t.VAT_sub_acct_code = "0";

        //Log transaction Request First
        //logval = tsv.InsertNameBankTellerTrans(t);
        
        logval = tsv.InsertOutgoingNibss(t);
        if (t.curcode != "NGN")
        {
            //new ErrorLog("57:Transaction not permitted " + acct + " Refid " + logval);
            Mylogger1.Info("57:Transaction not permitted " + acct + " Refid " + logval);
            msg = "57:Transaction not permitted";
            t.Refid = Int32.Parse(logval);
            t.ResponseCode = "57";
            tsv.updateByRefID(t);
            return msg;
        }

        //do not allow ledger 57-- added 08-Nov-2013
        //if (t.ledcode == "57" || t.ledcode == "74" || t.ledcode == "66" || t.ledcode == "67" || t.ledcode == "68" || t.ledcode == "70" || t.ledcode == "71" || t.ledcode == "72" || t.ledcode == "78" || t.ledcode == "97" || t.ledcode == "98" || t.ledcode == "99")
        //{
        //    new ErrorLog("57:Transaction not permitted " + acct + " Refid " + logval);
        //    msg = "57:Transaction not permitted";
        //    return msg;
        //}

        bool isLedNotAllwed = g.isLedgerNotAllowed(t.ledcode);
        if (isLedNotAllwed)
        {
            //new ErrorLog("57:Transaction not permitted " + acct + " Refid " + logval);
            Mylogger1.Info("57:Transaction not permitted " + acct + " Refid " + logval);
            msg = "57:Transaction not permitted";
            t.Refid = Int32.Parse(logval);
            t.ResponseCode = "57";
            tsv.updateByRefID(t);
            return msg;
        }

        //Update 13-SEP-12 ensure that the amount value is not 0 and not less than 0 and that limits apply 
        decimal maxPerTrans = 0;
        decimal maxPerday = 0;
        decimal sum = 0; decimal balAmttobe = 0;
        if (t.amount == 0 || t.amount < 0)
        {
            //new ErrorLog("Sorry, the amount value cannot be less than or equal to 0: " + acct);
            Mylogger1.Info("Sorry, the amount value cannot be less than or equal to 0: " + acct);
            msg = "Sorry, the amount value cannot be less than or equal to 0: " + acct;
            t.Refid = Int32.Parse(logval);
            t.ResponseCode = "13";
            tsv.updateByRefID(t);
            return msg;
        }


        //get global settings
        tsv.getTransactionLimits();
        maxPerTrans = tsv.maxPerTrans;
        maxPerday = tsv.maxPerday;

        //tsv.getTransactionLimits();
        maxPerTrans = tsv.maxPerTrans;
        maxPerday = tsv.maxPerday;
        if (maxPerTrans == 0 || maxPerday == 0)
        {
            //new ErrorLog("Unable to get the maximum amount per day/transaction");
            Mylogger1.Info("Unable to get the maximum amount per day/transaction");
            msg = "Unable to get the maximum amount per day/transaction";
            t.Refid = Int32.Parse(logval);
            t.ResponseCode = "57";
            tsv.updateByRefID(t);
            return "57:Transaction not permitted";
        }
        try
        {
            //check for consession 04-OCT-2014 modified by Chigozie
            string sqlcon = "select maxperday,maxpertran from tbl_nipconcessionTrnxlimits " +
               " where nuban =@nu and statusflag=1";

            //string sqlcon = "select maxperday,maxpertran from tbl_nipconcessionTrnxlimits " +
            //    " where bra_code =@bc and cus_num =@cn and cur_code =@cr and led_code =@lc and sub_acct_code=@sc and statusflag=1";
            Connect cnCon = new Connect(sqlcon, true);
            cnCon.addparam("@nu", t.nuban);
            //cnCon.addparam("@bc", t.bra_code);
            //cnCon.addparam("@cn", t.cusnum);
            //cnCon.addparam("@cr", t.curcode);
            //cnCon.addparam("@lc", t.ledcode);
            //cnCon.addparam("@sc", t.subacctcode);
            DataSet dscn = cnCon.query("rec");
            if (dscn.Tables[0].Rows.Count > 0)
            {
                DataRow drcn = dscn.Tables[0].Rows[0];
                maxPerTrans = decimal.Parse(drcn["maxpertran"].ToString());
                maxPerday = decimal.Parse(drcn["maxperday"].ToString());
            }
            else
            {
            }
        }
        catch (Exception ex)
        {
            //new ErrorLog("Error concess " + ex);
            Mylogger1.Error("Error concess ", ex);
        }
        //check if the amount to transfer is within the minimum amount to transfer
        if (t.amount <= maxPerTrans)
        {
            //check if the total transfer done by this customer has exceeded total of 10m
            //tsv.getTotalSumdone(t.bra_code, t.cusnum, t.curcode, t.ledcode, t.subacctcode);
            try
            {
                //ISNULL(sum(amt), 0)
                //string sqltt = "select SUM(amt) as amt from tbl_nibssmobile " +
                //string sqltt = "select ISNULL(sum(amt), 0) as amt from tbl_nibssmobile " +
                //    " where channelCode=1 and vTellerMsg=1 " +
                //    " and CONVERT(varchar(50),dateadded,102)  = CONVERT(varchar(50), GETDATE(),102)" +
                //    " and bra_code = @bc and cus_num =@cn and cur_code =@cr and led_code =@lc and sub_acct_code =@sc ";
                string sqltt = "select ISNULL(sum(amt), 0) as amt from tbl_nibssmobile " +
                " where channelCode=1 and vTellerMsg=1 " +
                " and CONVERT(varchar(50),dateadded,102)  = CONVERT(varchar(50), GETDATE(),102)" +
                " and nuban = @nu ";
                Connect ct = new Connect(sqltt, true);
                ct.addparam("@nu", t.nuban);
                //ct.addparam("@bc", t.bra_code);
                //ct.addparam("@cn", t.cusnum);
                //ct.addparam("@cr", t.curcode);
                //ct.addparam("@lc", t.ledcode);
                //ct.addparam("@sc", t.subacctcode);
                DataSet dst = ct.query("rec");
                if (dst.Tables[0].Rows.Count > 0)
                {
                    DataRow dtr = dst.Tables[0].Rows[0];
                    sum = decimal.Parse(dtr["amt"].ToString());
                }
                else
                {
                    sum = 0;
                }
            }
            catch (Exception ex)
            {
                //new ErrorLog("Error " + ex);
                Mylogger1.Error("Error ", ex);
            }
            //sum = tsv.sum;
            if (maxPerday >= sum)
            {
                //check if the current amount to transfer + the amount transfered so far is greater than 10m
                balAmttobe = t.amount + sum;
                if (maxPerday >= balAmttobe)
                {
                    //proceed
                }
                else
                {
                    //new ErrorLog("Sorry, this transaction will not be processed because the amount to transfer is greater than the balance if processed. balance if processed will be: " + balAmttobe.ToString() + " while maximum allowed tranfer per day is  " + maxPerday.ToString() + " for " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
                    Mylogger1.Info("Sorry, this transaction will not be processed because the amount to transfer is greater than the balance if processed. balance if processed will be: " + balAmttobe.ToString() + " while maximum allowed tranfer per day is  " + maxPerday.ToString() + " for " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
                    msg = "Sorry, this transaction will not be processed because the amount to transfer is greater than the balance if processed. balance if processed will be: " + balAmttobe.ToString() + " while maximum allowed tranfer per day is  " + maxPerday.ToString();
                    t.Refid = Int32.Parse(logval);
                    t.ResponseCode = "61";
                    tsv.updateByRefID(t);
                    return "61:IBS";
                }
            }
            else
            {
                //new ErrorLog("Sorry, you have exceeded your transfer limit for the day " + maxPerday.ToString() + maxPerday.ToString() + " for " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
                Mylogger1.Info("Sorry, you have exceeded your transfer limit for the day " + maxPerday.ToString() + maxPerday.ToString() + " for " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
                msg = "Sorry, you cannot transfer amount greater than " + maxPerday.ToString();
                t.Refid = Int32.Parse(logval);
                t.ResponseCode = "61";
                tsv.updateByRefID(t);
                return "61:IBS";
            }

        }
        else
        {
            //new ErrorLog("Sorry, you cannot transfer amount greater than " + maxPerTrans.ToString() + " per transaction " + maxPerday.ToString() + " for " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
            Mylogger1.Info("Sorry, you cannot transfer amount greater than " + maxPerTrans.ToString() + " per transaction " + maxPerday.ToString() + " for " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
            msg = "Sorry, you cannot transfer amount greater than " + maxPerTrans.ToString() + " per transaction ";
            t.Refid = Int32.Parse(logval);
            t.ResponseCode = "61";
            tsv.updateByRefID(t);
            return "61:IBS";
        }


        //******************************************************************************

        t.Refid = Int32.Parse(logval);
        //update the Response code
        t.ResponseCode = "00";
        tsv.UpdateBankTellerTransactions(t);

     

        t.tellerID = tellerid;
        

        t.transactionCode = g.newTrnxRef(bracodeval);

        t.origin_branch = t.bra_code;
        t.inCust.cus_sho_name = orignatorName;
        t.outCust.cusname = g.RemoveSpecialChars(t.AccountName) + "/" + t.transactionCode;

        t.inCust.bra_code = t.bra_code;
        t.inCust.cus_num = t.cusnum;
        t.inCust.cur_code = t.curcode;
        t.inCust.led_code = t.ledcode;
        //t.inCust.sub_acct_code = t.subacctcode;
        t.inCust.sub_acct_code = nuban;

        string bankCode = g.getBranchNIPCode(t.bra_code);
        //
        //string NewSessionid = g.newSessionId(DestinationBankCode);
        string NewSessionid = g.newSessionGlobal(bankCode, t.channelCode); 
        t.sessionid = NewSessionid; //sessionid for Funds Transfer


        //check if the ledger is 84 if it is then check the amount
        //if (t.ledcode == "84")
        if (t.ledcode == "6009")
        {
            //check if the amount is greather than 20000
            if (t.amount > 20000)
            {
                t.Refid = Int32.Parse(logval);
                t.ResponseCode = "12";
                tsv.updateByRefID(t);
                //12 invalid Transaction
                return "12:" + logval.ToString();// msg;
            }
        }

        tsv.UpdateFTSessionID(t.Refid, t.sessionid);

        if (cbatype == 1)
        {
            acs.authorizeBankTellerTrnxFromSterling(t);

        }
        else if (cbatype == 2)
        {
            Mylogger1.Info("about to start getting imal tss,fee and vat for nuban" + nuban + " and sessionid " + sessionid);
            imalTSS = GetImalPrincipalAcct();
            imalFeeAcct = GetImalFeeAcct();
            imalVatAcct = GetImalVatAcct();
            Mylogger1.Info("done getting imal tss,fee and vat for nuban" + nuban + " and sessionid " + sessionid+". the principal acct is "+imalTSS+", the fee acct is "+imalFeeAcct+" and the VAT acct is "+imalVatAcct+". Now going to the method DoImalDebit");
            acs.DoImalDebit(t, imalTSS, imalFeeAcct, imalVatAcct);
        }

       
        if (acs.Respreturnedcode1 == "1x")
        {
            tsv.UpdateBankTellervTeller(t, 3);//vTeller timed out
            msg = "Error: Transaction was not completed. Please check your balance " +
                "before performing another transaction";

            t.ResponseCode = "1x";
            t.Refid = Int32.Parse(logval);
            int cn1 = tsv.UpdateBankTellerTrans(t);
            return "x02:" + logval.ToString();// msg;
        }

        if (acs.Respreturnedcode1 != "0")
        {
            //new ErrorLog("Returned response from BANKS " + acs.Respreturnedcode1);
            Mylogger1.Info("Returned response from BANKS " + acs.Respreturnedcode1);
            tsv.UpdateBankTellervTeller(t, 2);//unable to debit
            msg = "Error: Unable to debit customer's account for Principal";
        
            string Respval = "";
            if (acs.error_text.Contains("Post No Debits"))
            {
                Respval = "x1000"; //Post No Debits (Override ID - POSTING.RESTRICT)
            }
            else if (acs.error_text.Contains("Incomplete Documentation"))
            {
                Respval = "x1001"; //Incomplete Documentation (Override ID - POSTING.RESTRICT)
            }
            else if (acs.error_text.Contains("BVN"))
            {
                Respval = "x1002"; //BVN (Override ID - POSTING.RESTRICT)
            }
            else if (acs.error_text.Contains("Dormant Account Restriction"))
            {
                Respval = "x1003"; //Dormant Account Restriction (Override ID - POSTING.RESTRICT)
            }
            else if (acs.error_text.Contains("Failed Address Verification"))
            {
                Respval = "x1004"; //Dormant Account Restriction (Override ID - POSTING.RESTRICT)
            }
            else if (acs.error_text.Contains("Unauthorised overdraft"))
            {
                Respval = "x1005"; //Unauthorised overdraft of NGN 10 on account
            }
            else if (acs.error_text.Contains("Inactive Account Restriction"))
            {
                Respval = "x1006"; //Inactive Account Restriction (Override ID - POSTING.RESTRICT)
            }
            else if (acs.error_text.Contains("INVALID SWIFT CHAR"))
            {
                Respval = "x1007"; //INVALID SWIFT CHAR
            }
            else if (acs.error_text.Contains(" REJECTED"))
            {
                Respval = "x1008"; //VALIDATION ERROR - REJECTED
            }
            else if (acs.error_text.Contains("is inactive"))
            {
                Respval = "x1009"; //is inactive
            }
            else if (acs.error_text.Contains("Connection refused: connect"))
            {
                Respval = "x1010"; //Connection refused: connect
            }
            else if (acs.error_text.Contains("Account has a short fall of balance"))
            {
                Respval = "x1011"; //Account has a short fall of balance
            }
            else if (acs.error_text.Contains("Below minimum value"))
            {
                Respval = "x1012"; //Below minimum value
            }
            else if (acs.error_text.Contains("TOO MANY DECIMALS"))
            {
                Respval = "x1013"; //TOO MANY DECIMALS
            }
            else if (acs.error_text.Contains("Account Upgrade Required (Override ID - POSTING.RESTRICT)"))
            {
                Respval = "x1014"; //Account Upgrade Required (Override ID - POSTING.RESTRICT)
            }
            else if (acs.error_text.Contains("Customer Address Verification"))
            {
                Respval = "x1015"; //Customer Address Verification
            }
            else
            {
                Respval = "x03";
                Mylogger1.Info("x03 response text " + acs.error_text + "THIS IS TO CONFIRM");
            }
            t.ResponseCode = Respval;
            t.Refid = Int32.Parse(logval);
            int cn1 = tsv.UpdateBankTellerTrans(t);
            return Respval + ":" + logval.ToString();

        }
        //mark trans type
        MarkTransType mt = new MarkTransType();
        mt.markTransType(t.Refid);
        //update vTeller Message column
        tsv.UpdateBankTellervTeller(t, 1);//debited successfully
        msg = "Customer has been debitted!";
        //update the tbl_mobile with response from T24
        tsv.UpdateT24FTResponse(acs.Prin_Rsp, acs.Fee_Rsp, acs.Vat_Rsp, t.Refid);

        //return "00: The T24test was successfull..."; //remove this aferwards 
        //if successful

        TR_SingleFundTransferDC sft = new TR_SingleFundTransferDC();
        sft.SessionID = NewSessionid;
        sft.DestinationInstitutionCode = DestinationBankCode;
        sft.ChannelCode = ChannelCode;
        sft.BeneficiaryAccountName = t.AccountName;
        sft.BeneficiaryAccountNumber = AccountNumber;
        sft.OriginatorAccountName = orignatorName;
        sft.BeneficiaryBankVerificationNumber = BeneficiaryBankVerificationNumber;
        sft.BeneficiaryKYCLevel = BeneficiaryKYCLevel;
        sft.OriginatorAccountNumber = OriginatorAccountNumber;
        sft.OriginatorBankVerificationNumber = OriginatorBankVerificationNumber;
        sft.OriginatorKYCLevel = OriginatorKYCLevel;
        //sft.Narration = "Transfer from " + cusshowname + " to " + t.AccountName;
        //sft.Narration = paymentRef;// "Transfer from " + cusshowname + " to " + t.AccountName;
        //if (paymentRef.Length > 100)
        //{
        //   paymentRef = paymentRef.Substring(0, 100);
        //}
        if (paymentRef == "")
        {
            paymentRef = "Transfer from " + cusshowname + " to " + t.AccountName;

        }
        if (paymentRef.Length > 100)
        {
            paymentRef = paymentRef.Substring(0, 100);
        }
        sft.Narration = paymentRef;

        sft.NameEnquiryRef = NameEnquiryRef;
        sft.PaymentReference = "" ;
        t.paymentRef = paymentRef;
        sft.Amount = amt;
        sft.createRequest();

        TransactionService trs = new TransactionService();
        //send Funds Transfer Request to NIBSS
        sft.sendRequest();

        string rspstmt = "";
        switch (sft.ResponseCode)
        {
            case "00":
                //success
                rspstmt = "3";  //acs.ResponseMsg;
                break;
            case "03": //txt = "Invalid sender"; break;
            case "05": //txt = "Do not honor"; break;
            case "06": //txt = "Dormant account"; break;
            case "07": //txt = "Invalid account"; break;
            case "08": //txt = "Account name mismatch"; break;
            case "09": //txt = "Request processing in progress"; break;
            case "12": //txt = "Invalid transaction"; break;
            case "13": //txt = "Invalid amount"; break;
            case "14": //txt = "Invalid Batch Number"; break;
            case "15": //txt = "Invalid Session or Record ID"; break;
            case "16": //txt = "Unknown Bank Code"; break;
            case "17": //txt = "Invalid Channel"; break;
            case "18": //txt = "Wrong Method Call"; break;
            case "21": //txt = "No action taken"; break;
            case "25": //txt = "Unable to locate record"; break;
            case "26": //txt = "Duplicate record"; break;
            case "30": //txt = "Wrong destination account format"; break;
            case "34": //txt = "Suspected fraud"; break;
            case "35": //txt = "Contact sending bank"; break;
            case "51": //txt = "No sufficient funds"; break;
            case "57": //txt = "Transaction not permitted to sender"; break;
            case "58": //txt = "Transaction not permitted on channel"; break;
            case "61": //txt = "Transfer Limit Exceeded"; break;
            case "63": //txt = "Security violation"; break;
            case "65": //txt = "Exceeds withdrawal frequency"; break;
            case "68": //txt = "Response received too late"; break;
            case "91": //txt = "Beneficiary Bank not available"; break;
            case "92": //txt = "Routing Error"; break;
            case "94": //txt = "Duplicate Transaction"; break;
            case "96": //txt = "Corresponding Bank is currently offline."; break;
            case "97": //txt = "Timeout waiting for response from destination."; break;
                string rsp = NIPReversal(logval);
                //rspstmt = "4";// "Error: " + sft.ResponseCode + ", [" + rsp + "]";
                //new ErrorLog("IBS Reversal was done==>" + rsp);
                Mylogger1.Info("IBS Reversal was done==>" + rsp);
                break;

            case "1x":
            default:
                rspstmt = "5";
                
                break;

        }
        //update the table with the response and sessioid for DC
        t.ResponseCode = sft.ResponseCode;
        t.Refid = Int32.Parse(logval);
        t.sessionid = sft.SessionID;
        int cn = trs.UpdateMobileTrans(t);

        return rspstmt + ":" + logval.ToString();
    }

    private string GetImalVatAcct()
    {
        string acct = "";
        DataSet ds = new DataSet();
        Connect c = new Connect("spd_GetImalVatAcct");
        ds = c.query("recs");
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            acct = ds.Tables[0].Rows[0]["sub_acct_code"].ToString();
        }

        return acct;
    }

    private string GetImalFeeAcct()
    {
        string acct = "";
        DataSet ds = new DataSet();
        Connect c = new Connect("spd_GetImalFeeAcct");
        ds = c.query("recs");
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            acct = ds.Tables[0].Rows[0]["tssacct"].ToString();
        }

        return acct;
    }

    private string GetImalPrincipalAcct()
    {
        string acct = "";
        DataSet ds = new DataSet();
        Connect c = new Connect("spd_GetImalPrincipalAcct");
        ds = c.query("recs");
        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            acct = ds.Tables[0].Rows[0]["sub_acct_code"].ToString();
        }

        return acct;
    }
    public string NIPReversal(string logval)
    {
        //insert into the mobile reversal table
        Connect cn = new Connect("spd_mobileTrnxReverse");
        cn.addparam("@refid", logval);
        cn.addparam("@doneby", "AUTOSYSTEM");
        cn.addparam("@action", "reverse");
        cn.query();
        return "";
    }

    [WebMethod]
    public string NIBSSReversal(string sessionid, string bracodeval, string cusnumval, string curcodeval, string ledcodeval, string subacctval, string amt, string fee, string orignatorName, string DestinationBankCode, string ChannelCode, string AccountName, string AccountNumber, string Narration, string cusshowname)
    {
        //insert record


        string msg;
        string ledger = "";
        Gadget g = new Gadget();
        AccountService acs = new AccountService();
        Transaction t = new Transaction();
        TransactionService tsv = new TransactionService();

        t.bra_code = bracodeval;
        t.cusnum = cusnumval;
        t.curcode = curcodeval;
        t.ledcode = ledcodeval;
        t.subacctcode = subacctval;
        t.amount = decimal.Parse(amt);

        //go and get the operational fee/income ledger from database
        DataSet dsledger = tsv.getCurrentIncomeAcct();
        if (dsledger.Tables[0].Rows.Count > 0)
        {
            DataRow drledger = dsledger.Tables[0].Rows[0];
            ledger = drledger["ledcode"].ToString();
        }
        //ensure that the TSS and Income ledger for the approving branch exist
        if (!acs.checkTSSandFeesAccount(ledger, t.bra_code))
        {
            //accounts are not okay
            msg = "ERROR! \nKindly Contact the Branch Operation that Ledger:8720 has not been activated.  This transaction will not be processed.";
            return msg;
        }

        if (!acs.tssACCok) //is TSS ok?
        {
            //print err
            msg = "ERROR! \nKindly verify that your Branch's Clearing Account is active";
            return msg; //stop
        }

        if (!acs.feeACCok) //is Fee ok?
        {
            //print err
            msg = "Kindly verify that your Branch's Nibss Income Account is active";
            return msg; //stop
        }

        t.tellerID = "9990";
        //compute the amount to pay
        t.feecharge = acs.calculateFee(t.amount);

        t.origin_branch = t.bra_code;
        t.inCust.cus_sho_name = cusshowname;
        t.outCust.cusname = AccountName + "_Mobile Transaction:Reversal";

        t.inCust.bra_code = t.bra_code;
        t.inCust.cus_num = t.cusnum;
        t.inCust.cur_code = t.curcode;
        t.inCust.led_code = t.ledcode;
        t.inCust.sub_acct_code = t.subacctcode;

        string NewSessionid = sessionid;
        t.sessionid = NewSessionid;
        t.feecharge = decimal.Parse(fee);
        acs.authorizeTrnxReversalIBS(t);
        if (acs.Respreturnedcode1 != "0")
        {
            msg = "Reversal was not Successful";
            return msg;
        }
        else
        {
            return "Reversal was Successful";
        }
    }

    [WebMethod]
    public string requeryTrnx(string sessionNE)
    {
        string sql = "select nameResponse, response, requeryStatus, vTellerMsg,reversalStatus " +
            "from tbl_nibssmobile where sessionidNE = @ne";
        Connect cn = new Connect(sql, true);
        cn.addparam("@ne", sessionNE);
        DataSet ds = cn.query("recs");

        string resp = "";
        if (ds.Tables[0].Rows.Count > 0)
        {
            resp = ds.Tables[0].Rows[0]["nameresponse"].ToString();
            resp += ",";
            resp += ds.Tables[0].Rows[0]["response"].ToString();
            resp += ",";
            resp += ds.Tables[0].Rows[0]["requeryStatus"].ToString();
            resp += ",v";
            resp += ds.Tables[0].Rows[0]["vtellermsg"].ToString();
            resp += ",";
            if (ds.Tables[0].Rows[0]["reversalStatus"].ToString() == "TREATED")
            {
                resp += "00";
            }
        }
        else
        {
            resp = ",,,,";
        }
        return resp;
    }
}

