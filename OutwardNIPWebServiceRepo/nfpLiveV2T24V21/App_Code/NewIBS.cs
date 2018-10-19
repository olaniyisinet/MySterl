using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;


/// <summary>
/// Summary description for NewIBS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class NewIBS : System.Web.Services.WebService
{
    [WebMethod]
    public string NameEnquiry(string SessionID, string DestinationBankCode, string ChannelCode, string AccountNumber)
    {
        string msg = "";
        string responsecodeVal = "";
        string AcctNameval = ""; string BankVerificationNumber = ""; string KYCLevel = "";
        TR_SingleNameEnquiry sne = new TR_SingleNameEnquiry();

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

        try
        {
            //new ErrorLog("Received NE for NewIBS " + SessionID + " " + DestinationBankCode + " " + ChannelCode + " " + AccountNumber);
            Mylogger1.Info("Received NE for NewIBS " + SessionID + " " + DestinationBankCode + " " + ChannelCode + " " + AccountNumber);
        }
        catch
        {
            //proceed
        }

        sne.SessionID = SessionID;
        sne.DestinationInstitutionCode = DestinationBankCode;
        sne.ChannelCode = ChannelCode;
        sne.AccountNumber = AccountNumber;
        sne.createRequest();

        if (!sne.sendRequest()) //unsuccessful request
        {
            //assign values returned from nibss to parameters
            responsecodeVal = sne.ResponseCode;
            BankVerificationNumber = sne.BankVerificationNumber;
            KYCLevel = sne.KYCLevel;

            AcctNameval = "";
            msg = responsecodeVal + ":" + AcctNameval + ":" + BankVerificationNumber + ":" + KYCLevel;

            try
            {
                //new ErrorLog(msg);
                Mylogger1.Info(msg);
            }
            catch
            {

            }
        }
        else
        {
            responsecodeVal = sne.ResponseCode;
            AcctNameval = sne.AccountName;
            BankVerificationNumber = sne.BankVerificationNumber;
            KYCLevel = sne.KYCLevel;

            msg = responsecodeVal + ":" + AcctNameval + ":" + BankVerificationNumber + ":" + KYCLevel;
            try
            {
                //new ErrorLog(msg);
                Mylogger1.Info(msg);
            }
            catch
            {
            }
        }
        return msg;
    }

    [WebMethod]
    public string DebitandSendtoNIBSS(string sessionid, string bracodeval, string cusnumval, string curcodeval,
        string ledcodeval, string subacctval, string amt, string fee, string orignatorName, string DestinationBankCode,
        string ChannelCode, string AccountName, string AccountNumber, string paymentRef, string cusshowname, Int32 Appid, string Referenceidval,
        string BeneficiaryBankVerificationNumber, string BeneficiaryKYCLevel, string OriginatorAccountNumber, string OriginatorBankVerificationNumber,
        string OriginatorKYCLevel, string TransactionLocation, string NameEnquiryRef)
    {
        string req111 = sessionid + ", " + bracodeval + ", " + cusnumval + ", " + curcodeval + ", " + ledcodeval + ", "
            + subacctval + ", " + amt + ", " + fee + ", " + orignatorName + ", " + DestinationBankCode + ", "
            + ChannelCode + ", " + AccountName + ", " + AccountNumber + ", " + paymentRef + ", " + cusshowname + ", "
            + Appid + ", " + Referenceidval + ", " + BeneficiaryBankVerificationNumber + ", " + BeneficiaryKYCLevel + ", "
            + OriginatorAccountNumber + ", " + OriginatorBankVerificationNumber + ", "
            + OriginatorKYCLevel + ", " + TransactionLocation + ", " + NameEnquiryRef;

        //added to ensure that Mobile, USSD does not exceed their respective per transfer limit

        if (Appid == 5 || Appid == 26 || Appid == 17)
        {
            //Kia kia- 6009 (Transfer Limit - N3000.00)
            if (decimal.Parse(amt) > 3000 && ledcodeval == "6009")
            {
                return "13:" + "Invalid Amount";
            }
            //Kia kia plus-6010 (Transfer Limit - N10,000.00)
            if (decimal.Parse(amt) > 10000 && ledcodeval == "6010")
            {
                return "13:" + "Invalid Amount";
            }
            //Sterling Fanatic Kick off – 6011 (Transfer Limit - N3000.00)
            if (decimal.Parse(amt) > 3000 && ledcodeval == "6011")
            {
                return "13:" + "Invalid Amount";
            }

            //Sterling Fanatic Kick off – 6012 (Transfer Limit - N10000.00)
            if (decimal.Parse(amt) > 10000 && ledcodeval == "6012")
            {
                return "13:" + "Invalid Amount";
            }
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

        string Narrationval = "";
        //sbp.banks b1 = new sbp.banks();
        //new ErrorLog("TRansaction received to be processed for to account " + AccountNumber + "sessionNE " + sessionid);
        Mylogger1.Info("TRansaction received to be processed for to account " + AccountNumber + "sessionNE " + sessionid);
        //check if the name enquiry session exists already
        Connect cnSNE = new Connect("spd_checkExistSessionNE");
        cnSNE.addparam("@sessionne", sessionid);
        cnSNE.query();
        if (cnSNE.returnValue > 0)
        {
            return "6:0"; //
        }
        decimal MaxNIPdone = 0;
        decimal dailyminlimit = 0;
        string msg;
        string ledger = "";
        string logval = "0"; int Last4 = 0; string vatNUBAN = ""; string TSSAcct = "";
        Gadget g = new Gadget(); TotaldonePerday Tdp = new TotaldonePerday();
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
        //t.feecharge = decimal.Parse(fee); 
        t.AccountName = AccountName;
        t.AccountNumber = AccountNumber;
        t.channelCode = int.Parse(ChannelCode);// 2;//Channel code for Internet Banking
        t.sessionid = sessionidNE;


        //new parameters
        t.BeneficiaryBankVerificationNumber = BeneficiaryBankVerificationNumber;
        t.BeneficiaryKYCLevel = BeneficiaryKYCLevel;
        t.OriginatorAccountNumber = OriginatorAccountNumber;
        t.OriginatorBankVerificationNumber = OriginatorBankVerificationNumber;
        t.OriginatorKYCLevel = OriginatorKYCLevel;
        t.TransactionLocation = TransactionLocation;
        t.NameEnquiryRef = NameEnquiryRef;
        t.nuban = OriginatorAccountNumber;
        string acct = bracodeval + cusnumval + curcodeval + ledcodeval + subacctval;
        t.ReferenceID = Referenceidval;
        t.Appid = Appid;
        decimal avail_bal = 0;
        EACBS.banks ws = new EACBS.banks();
        DataSet dst24 = new DataSet();
        try
        {
            dst24 = ws.getAccountFullInfo(OriginatorAccountNumber);
        }
        catch (Exception ex)
        {
            //new ErrorLog("Error Occured" + ex);
            Mylogger1.Error("Error Occured ", ex);
        }
        int cus_class = 0; string cusclassval = ""; int CustomerStatusCode = 0;
        //get customer details from T24
        if (dst24.Tables[0].Rows.Count > 0)
        {
            DataRow drt24 = dst24.Tables[0].Rows[0];
            avail_bal = decimal.Parse(drt24["UsableBal"].ToString());
            t.ledcode = drt24["T24_LED_CODE"].ToString();
            CustomerStatusCode = int.Parse(drt24["CustomerStatusCode"].ToString());
            t.nuban = OriginatorAccountNumber;
            cusclassval = drt24["CustomerStatus"].ToString();
            bracodeval = drt24["T24_BRA_CODE"].ToString();
            t.bra_code = bracodeval;
            //if (cusclassval == "Individual Customer")
            if (CustomerStatusCode == 1 || CustomerStatusCode == 6)
            {
                cus_class = 1;
            }
            else
            {
                cus_class = 2;
            }

        }

        DateTime dt = DateTime.Today;
        bool found_hol = g.isDateHoliday(dt);


        //check if the account is corporate and bounce the account tbl_public_holiday
        if (CustomerStatusCode == 2)
        {
            //check the holiday table 
            if (found_hol)
            {
                //new ErrorLog("57:Transaction not permitted for customer with nuban for corporate on weeknd " + OriginatorAccountNumber + " because account is corporate");
                Mylogger1.Info("57:Transaction not permitted for customer with nuban for corporate on weeknd " + OriginatorAccountNumber + " because account is corporate");
                msg = "57:Transaction not permitted";
                t.Refid = Int32.Parse(logval);
                t.ResponseCode = "57";
                //insert partail outgoing since the tranaction will not pass this section
                tsv.InsertPartialOutgoing(t);
                return msg;
            }
        }


        if (cus_class == 1 && t.amount > 1000000 && !CheckIfCustomerHasConcession(OriginatorAccountNumber))
        {
            //new ErrorLog("Customer with account " + OriginatorAccountNumber + " has concession and the txnamt " + amt + " is higher than the max per tran");
            Mylogger1.Info("Customer with account " + OriginatorAccountNumber + " has concession and the txnamt " + amt + " is higher than the max per tran");
            t.ResponseCode = "09x";
            msg = g.responseCodes(t.ResponseCode).Replace("customer1", orignatorName);
            //insert partail outgoing since the tranaction will not pass this section
            tsv.InsertPartialOutgoing(t);
            return t.ResponseCode + ":" + msg;
        }

        try
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
                    TSSAcct = t.VAT_cur_code + t.VAT_led_code + "0001" + Last4.ToString();
                    t.VAT_sub_acct_code = TSSAcct;
                    //t.VAT_sub_acct_code = b1.FormInternalAcct(t.VAT_bra_code, t.VAT_cur_code, t.VAT_led_code);
                    if (t.VAT_sub_acct_code == "")
                    {
                        msg = "Unable to form the VAT account for " + t.bra_code + " NGN ledcode 17201";
                        //insert partail outgoing since the tranaction will not pass this section
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
                    //insert partail outgoing since the tranaction will not pass this section
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
                    t.VAT_sub_acct_code = TSSAcct;// b1.FormInternalAcct(t.VAT_bra_code, "NGN", t.VAT_led_code);
                    if (t.VAT_sub_acct_code == "")
                    {
                        msg = "Unable to form the VAT account for " + t.bra_code + " NGN ledcode 17201";
                        //insert partail outgoing since the tranaction will not pass this section
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
                    //insert partail outgoing since the tranaction will not pass this section
                    t.ResponseCode = "24";
                    tsv.InsertPartialOutgoing(t);
                    return "24:" + msg;
                }
            }
        }
        catch (Exception ex)
        {
            //new ErrorLog("Error occured " + ex);
            Mylogger1.Error("Error occured ", ex);
            msg = "Error: Unable to compute VAT and Fee for account " + acct;
            //insert partail outgoing since the tranaction will not pass this section
            t.ResponseCode = "24";
            tsv.InsertPartialOutgoing(t);
            return "24:" + msg;
        }

        //Log transaction Request First
        //logval = tsv.InsertNameIBSTrans(t);
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
        else
        {
            //it means it is equal to 1
            t.curcode = "NGN";
        }

        bool isLedNotAllwed = g.isLedgerNotAllowed(t.ledcode);
        if (isLedNotAllwed)
        {
            //new ErrorLog("57:Transaction not permitted " + acct + " Refid " + logval);
            Mylogger1.Info("57:Transaction not permitted " + acct + " Refid " + logval);
            msg = "57:Transaction not permitted";
            t.ResponseCode = "57";
            tsv.updateByRefID(t);
            return msg;
        }

        //Update 13-SEP-12 ensure that the amount value is not 0 and not less than 0 and that limits apply 
        decimal maxPerTrans = 0;
        decimal maxPerday = 0;
        if (t.amount == 0 || t.amount < 0)
        {
            //new ErrorLog("Sorry, the amount value cannot be less than or equal to 0: " + acct);
            Mylogger1.Info("Sorry, the amount value cannot be less than or equal to 0: " + acct);
            msg = "Sorry, the amount value cannot be less than or equal to 0: " + acct;
            t.Refid = Int32.Parse(logval);
            t.ResponseCode = "57";
            tsv.updateByRefID(t);
            return "57:Transaction not permitted";
        }

        //25-OCT-2013
        //check if customer has maximum trans limit per day per transaction concession 

        //new ErrorLog("Cus_class for account number " + OriginatorAccountNumber + " is " + cus_class.ToString());
        Mylogger1.Info("Cus_class for account number " + OriginatorAccountNumber + " is " + cus_class.ToString());

        //check if customer has maximum trans limit per day per transaction concession 
        bool HasConcessionPerTransPerday = tsv.getMaxperTransPerday(t.bra_code, t.cusnum, t.curcode, t.ledcode, t.subacctcode, OriginatorAccountNumber);
        //new ErrorLog("COncession check for account " + OriginatorAccountNumber + " returned " + HasConcessionPerTransPerday.ToString());
        Mylogger1.Info("COncession check for account " + OriginatorAccountNumber + " returned " + HasConcessionPerTransPerday.ToString());
        if (HasConcessionPerTransPerday)
        {
            maxPerTrans = tsv.maxPerTrans;
            maxPerday = tsv.maxPerday;
            //new ErrorLog("MaxperTrans returned " + maxPerTrans.ToString() + " maxPerday " + maxPerday.ToString() + " for customer " + OriginatorAccountNumber);
            Mylogger1.Info("MaxperTrans returned " + maxPerTrans.ToString() + " maxPerday " + maxPerday.ToString() + " for customer " + OriginatorAccountNumber);
            TotaldonePerday Tdp1 = new TotaldonePerday();
            bool totalfound = Tdp1.getTotalTransDonePerday(bracodeval, cusnumval, curcodeval, ledcodeval, subacctval, maxPerday, decimal.Parse(amt), OriginatorAccountNumber);

            if (decimal.Parse(amt) > maxPerTrans)
            {
                //exit the customer at this point
                //new ErrorLog("Customer with account " + OriginatorAccountNumber + " has concession and the txnamt " + amt + " is higher than the max per tran" + maxPerTrans.ToString());
                Mylogger1.Info("Customer with account " + OriginatorAccountNumber + " has concession and the txnamt " + amt + " is higher than the max per tran" + maxPerTrans.ToString());
                t.ResponseCode = "09x";
                tsv.updateByRefID(t);
                msg = g.responseCodes(t.ResponseCode).Replace("customer1", orignatorName);
                return t.ResponseCode + ":" + msg;
            }

            if (decimal.Parse(amt) + Tdp1.Totaldone > maxPerday)
            {
                decimal my1sum = 0;
                my1sum = decimal.Parse(amt) + Tdp1.Totaldone;
                //exit the customer at this point
                //new ErrorLog("Customer with account " +  OriginatorAccountNumber + " has concession and the txnamt plus the totalNipdone " + my1sum.ToString() + "is higher than the max per tran" + maxPerTrans.ToString());
                Mylogger1.Info("Customer with account " + OriginatorAccountNumber + " has concession and the txnamt plus the totalNipdone " + my1sum.ToString() + "is higher than the max per tran" + maxPerTrans.ToString());
                t.ResponseCode = "09x";
                tsv.updateByRefID(t);
                msg = g.responseCodes(t.ResponseCode).Replace("customer1", orignatorName);
                return t.ResponseCode + ":" + msg;
            }


            //execute the request

            t.Refid = Int32.Parse(logval);
            //update the Response code
            t.ResponseCode = "00";
            tsv.UpdateIBSTransactions(t);



            t.tellerID = "9990";

            t.transactionCode = g.newTrnxRef(bracodeval);

            t.origin_branch = t.bra_code;
            t.inCust.cus_sho_name = orignatorName;
            //t.outCust.cusname = g.RemoveSpecialChars(t.AccountName) + "/" + t.transactionCode;
            t.outCust.cusname = g.RemoveSpecialChars(t.AccountName) + "/" + g.RemoveSpecialChars(paymentRef);

            t.inCust.bra_code = t.bra_code;
            t.inCust.cus_num = t.cusnum;
            t.inCust.cur_code = t.curcode;
            t.inCust.led_code = t.ledcode;
            //t.inCust.sub_acct_code = t.subacctcode;
            t.inCust.sub_acct_code = t.nuban;

            //string NewSessionid = g.newSessionIdIBS(DestinationBankCode, t.Refid); newSessionGlobal
            string bankCode = g.getBranchNIPCode(t.bra_code);
            string NewSessionid = g.newSessionGlobal(bankCode, t.channelCode);// g.newSessionId(DestinationBankCode, t.bra_code, t.channelCode);
            t.sessionid = NewSessionid; //sessionid for Funds Transfer


            tsv.UpdateFTSessionID(t.Refid, t.sessionid);

            //check if the ledger is 84 if it is then check the amount
            if (t.ledcode == "6009")
            {
                //check if the amount is greather than 20000
                if (t.amount > 20000)
                {
                    //12 invalid Transaction
                    t.Refid = Int32.Parse(logval);
                    t.ResponseCode = "13";
                    tsv.updateByRefID(t);
                    return "13:" + logval.ToString();// msg;
                }
            }

            acs.authorizeIBSTrnxFromSterling(t);
            if (acs.Respreturnedcode1 == "1x")
            {
                //new ErrorLog("The Response from Banks is " + acs.Respreturnedcode1 + "  and hence, vTeller logs it as 3 For SessionID " + t.sessionid);
                Mylogger1.Info("The Response from Banks is " + acs.Respreturnedcode1 + "  and hence, vTeller logs it as 3 For SessionID " + t.sessionid + " T24 msg ==> " + acs.error_text);
                tsv.UpdateIBSvTeller(t, 3);//vTeller timed out
                msg = "Error: Transaction was not completed. Please check your balance " +
                    "before performing another transaction";

                t.ResponseCode = "1x";
                t.Refid = Int32.Parse(logval);
                int cn1 = tsv.UpdateIBSTrans(t);
                //NIPReversal(logval);
                //return "2:" + logval.ToString();// msg;
                return "x02:" + logval.ToString() + ":" + acs.error_text;
            }

            if (acs.Respreturnedcode1 != "0")
            {
                //new ErrorLog("The Response from Banks is " + acs.Respreturnedcode1 + "  and hence, vTeller logs it as 2. For SessionID " + t.sessionid);
                Mylogger1.Info("The Response from Banks is " + acs.Respreturnedcode1 + "  and hence, vTeller logs it as 2. For SessionID " + t.sessionid + " T24 msg ==> " + acs.error_text);
                tsv.UpdateIBSvTeller(t, 2);//unable to debit
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
                else if (acs.error_text == "Account " + t.nuban + " - Account Upgrade Required (Override ID - POSTING.RESTRICT)")
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
                    Mylogger1.Info("x03 response text " + acs.error_text + "THIS WAS ADDED TO CONFIRM");
                }
                //
                t.ResponseCode = Respval;
                t.Refid = Int32.Parse(logval);
                int cn1 = tsv.UpdateIBSTrans(t);
                return Respval + ":" + logval.ToString() + ":" + acs.error_text;
            }

            //mark trans type
            MarkTransType mt = new MarkTransType();
            mt.markTransType(t.Refid);
            //update vTeller Message column
            tsv.UpdateIBSvTeller(t, 1);//debited successfully
            msg = "Customer has been debitted!";
            //update the tbl_mobile with response from T24
            tsv.UpdateT24FTResponse(acs.Prin_Rsp, acs.Fee_Rsp, acs.Vat_Rsp, t.Refid);

            //return "00: The T24test was successfull..."; //remove this aferwards 
            //if successful

            TR_SingleFundTransferDC sft = new TR_SingleFundTransferDC();

            sft.SessionID = NewSessionid;
            sft.DestinationInstitutionCode = DestinationBankCode;
            sft.ChannelCode = ChannelCode;
            sft.BeneficiaryAccountName = t.AccountName.Trim();
            sft.BeneficiaryAccountNumber = AccountNumber;
            sft.OriginatorAccountName = orignatorName.Trim();
            sft.BeneficiaryBankVerificationNumber = BeneficiaryBankVerificationNumber;
            sft.BeneficiaryKYCLevel = BeneficiaryKYCLevel;
            sft.OriginatorAccountNumber = OriginatorAccountNumber.Trim();
            sft.OriginatorBankVerificationNumber = OriginatorBankVerificationNumber.Trim();
            sft.OriginatorKYCLevel = OriginatorKYCLevel;
            Narrationval = "Transfer from " + cusshowname.Trim() + " to " + t.AccountName.Trim();
            if (Narrationval.Length > 100)
            {
                Narrationval = Narrationval.Substring(0, 100).Trim();
            }
            //Narrationval = Narrationval.Substring(0, 100);
            sft.Narration = Narrationval;// "Transfer from " + cusshowname + " to " + t.AccountName;

            //if (paymentRef.Length > 100)
            //{
            //    paymentRef = paymentRef.Substring(0, 100);
            //}
            sft.NameEnquiryRef = NameEnquiryRef.Trim();
            sft.PaymentReference = paymentRef.Trim();
            t.paymentRef = paymentRef.Trim();
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
                    rspstmt = sft.ResponseCode;// "3";  //acs.ResponseMsg;
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
                    rspstmt = sft.ResponseCode;
                    string rsp = NIPReversal(logval);
                    //new ErrorLog("IBS Reversal was done==>" + rsp);
                    Mylogger1.Info("IBS Reversal was done==>" + rsp);
                    break;
                case "1x":
                default:
                    rspstmt = sft.ResponseCode;// "5";
                    break;

            }
            //update the table with the response and sessioid for DC
            t.ResponseCode = sft.ResponseCode;
            t.Refid = Int32.Parse(logval);
            t.sessionid = sft.SessionID;
            int cn = trs.UpdateMobileTrans(t);

            return rspstmt + ":" + logval.ToString();
        }
        else
        {
            //get the total sum done for today
            Tdp.getTotalTransDonePerday(bracodeval, cusnumval, curcodeval, ledcodeval, subacctval, maxPerday, decimal.Parse(amt), OriginatorAccountNumber);
            //check if the amount is greather than the maxmum the customer has ever done
            //get the current CBN limit

            if (found_hol)
            {
                if (CustomerStatusCode == 1 || CustomerStatusCode == 6)
                {
                    if (Tdp.totlcnt >= 3)
                    {
                        t.ResponseCode = "65";
                        t.Refid = Int32.Parse(logval);
                        tsv.updateByRefID(t);
                        msg = g.responseCodes(t.ResponseCode).Replace("customer1", orignatorName);
                        //new ErrorLog("Error 65 (Transfer Limit Exceeded on holiday and weekend) returned for customer (velocity > 3) transaction with sessionid " + sessionid);
                        Mylogger1.Info("Error 65 (Transfer Limit Exceeded on holiday and weekend) returned for customer (velocity > 3) transaction with sessionid " + sessionid);
                        return t.ResponseCode + ":" + msg;
                    }

                    if (Tdp.Totaldone + decimal.Parse(amt) > 200000)
                    {
                        t.ResponseCode = "65";
                        t.Refid = Int32.Parse(logval);
                        tsv.updateByRefID(t);
                        msg = g.responseCodes(t.ResponseCode).Replace("customer1", orignatorName);
                        //new ErrorLog("Error 65 (Transfer Limit Exceeded on holiday and weekend) returned for customer (200,000 on holiday and weekend) transaction with sessionid " + sessionid);
                        Mylogger1.Info("Error 65 (Transfer Limit Exceeded on holiday and weekend) returned for customer (200,000 on holiday and weekend) transaction with sessionid " + sessionid);
                        return t.ResponseCode + ":" + msg;
                    }
                }
            }

            DataSet dsl = getCBNamt(cus_class);
            DataRow drl;
            //new ErrorLog("Pocessing  account " + OriginatorAccountNumber + " with cus_class "+ cus_class.ToString());
            Mylogger1.Info("Pocessing  account " + OriginatorAccountNumber + " with cus_class " + cus_class.ToString());
            if (cus_class == 1)
            {
                if (dsl.Tables[0].Rows.Count > 0)
                {
                    drl = dsl.Tables[0].Rows[0];
                    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                }

                //Indivi_dailylimit = 1000000;
                //maxlimitPerday = 5000000;
                dailyminlimit = maxPerTrans;

                //check if the ledger is for savings
                CheckSavingsLedger cl = new CheckSavingsLedger();
                bool isFound = cl.isLedgerFound(ledcodeval);
                if (isFound)
                {
                    CheckSavingsLedger EFTamt = new CheckSavingsLedger();
                    EFTamt.getMaxEFTAmt();
                    maxPerTrans = EFTamt.maxpertran;
                    maxPerday = EFTamt.maxperday;
                }
            }
            else if (cus_class == 2)
            {
                if (dsl.Tables[0].Rows.Count > 0)
                {
                    drl = dsl.Tables[0].Rows[0];
                    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                }
                //corp_DailyLimit = 10000000;
                //maxlimitPerday = 100000000;
                dailyminlimit = maxPerTrans;
            }
            else if (cus_class == 3)
            {
                if (dsl.Tables[0].Rows.Count > 0)
                {
                    drl = dsl.Tables[0].Rows[0];
                    maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                    maxPerday = decimal.Parse(drl["maxamt"].ToString());
                }
                //corp_DailyLimit = 10000000;
                //maxlimitPerday = 100000000;
                dailyminlimit = maxPerTrans;
            }
        }
        //tsv.getTransactionLimits();


        if (maxPerTrans == 0 || maxPerday == 0)
        {
            //new ErrorLog("Unable to get the maximum amount per day/transaction for account " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
            Mylogger1.Info("Unable to get the maximum amount per day/transaction for account " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
            msg = "Unable to get the maximum amount per day/transaction";
            t.Refid = Int32.Parse(logval);
            t.ResponseCode = "57";
            tsv.updateByRefID(t);
            return "57:Transaction not permitted";
        }

        //this is to ensure that customers will not exceed the daily cbn limit
        if (t.amount <= maxPerTrans)
        {
            if (t.amount + Tdp.Totaldone <= maxPerday)
            {

            }
            else
            {
                //exit the customer at this point
                //new ErrorLog("CBN Limit: Customer was caught " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
                Mylogger1.Info("CBN Limit: Customer was caught " + t.bra_code + t.cusnum + t.curcode + t.ledcode + t.subacctcode);
                t.ResponseCode = "09x";
                t.Refid = Int32.Parse(logval);
                tsv.updateByRefID(t);
                msg = g.responseCodes(t.ResponseCode).Replace("customer1", "Dear, " + orignatorName);
                return t.ResponseCode + ":" + msg;
            }
        }
        else
        {
            t.ResponseCode = "61";
            t.Refid = Int32.Parse(logval);
            tsv.updateByRefID(t);
            msg = g.responseCodes(t.ResponseCode).Replace("customer1", orignatorName);
            return t.ResponseCode + ":" + msg;
        }
        //end 

        t.Refid = Int32.Parse(logval);
        //update the Response code
        t.ResponseCode = "00";
        tsv.UpdateIBSTransactions(t);



        t.tellerID = "9990";

        t.transactionCode = g.newTrnxRef("232");

        t.origin_branch = t.bra_code;
        t.inCust.cus_sho_name = orignatorName;
        //t.outCust.cusname = g.RemoveSpecialChars(t.AccountName) + "/" + t.transactionCode;
        t.outCust.cusname = g.RemoveSpecialChars(t.AccountName) + "/" + g.RemoveSpecialChars(paymentRef);

        t.inCust.bra_code = t.bra_code;
        t.inCust.cus_num = t.cusnum;
        t.inCust.cur_code = t.curcode;
        t.inCust.led_code = t.ledcode;
        t.inCust.sub_acct_code = t.nuban;// t.subacctcode;

        //string NewSessionid1 = g.newSessionIdIBS(DestinationBankCode, t.Refid);
        string bankCodeval = g.getBranchNIPCode(t.bra_code);

        string NewSessionid1 = g.newSessionGlobal(bankCodeval, t.channelCode); // g.newSessionId(DestinationBankCode, t.bra_code, t.channelCode);
        t.sessionid = NewSessionid1; //sessionid for Funds Transfer


        tsv.UpdateFTSessionID(t.Refid, t.sessionid);

        //sbp.banks kia = new sbp.banks();
        //int cust_type = kia.getKiaKiaClass(t.bra_code, t.cusnum, t.curcode, t.ledcode, t.subacctcode);
        int cust_type = 0;
        //check if the ledger is 84 if it is then check the amount
        //if (t.ledcode == "84" && cust_type == 1)
        if (t.ledcode == "6009" && cust_type == 1)
        {
            //check if the amount is greather than 20000
            if (t.amount > 20000)
            {
                //12 invalid Transaction
                t.Refid = Int32.Parse(logval);
                t.ResponseCode = "13";
                tsv.updateByRefID(t);
                //tsv.UpdateIBSTrans(t);
                return "13:" + logval.ToString();// msg;
            }
        }
        //else if (t.ledcode == "5068" && cust_type == 1)
        else if (t.ledcode == "6011" && cust_type == 1)
        {
            //check if the amount is greather than 20000
            if (t.amount > 20000)
            {
                //12 invalid Transaction
                t.Refid = Int32.Parse(logval);
                t.ResponseCode = "13";
                tsv.updateByRefID(t);
                return "13:" + logval.ToString();// msg;
            }
        }

        acs.authorizeIBSTrnxFromSterling(t);
        if (acs.Respreturnedcode1 == "1x")
        {
            //new ErrorLog("The Response from Banks is " + acs.Respreturnedcode1 + "  and hence, vTeller logs it as 3 For SessionID " + t.sessionid);
            Mylogger1.Info("The Response from Banks is " + acs.Respreturnedcode1 + "  and hence, vTeller logs it as 3 For SessionID " + t.sessionid + " T24 msg ==>" + acs.error_text);
            tsv.UpdateIBSvTeller(t, 3);//vTeller timed out
            msg = "Error: Transaction was not completed. Please check your balance " +
                "before performing another transaction";

            t.ResponseCode = "1x";
            t.Refid = Int32.Parse(logval);
          
            //NIPReversal(logval);
            //return "2:" + logval.ToString();// msg;
            return "x02:" + logval.ToString() + ":" + acs.error_text;
        }

        if (acs.Respreturnedcode1 != "0")
        {
            //new ErrorLog("The Response from Banks is " + acs.Respreturnedcode1 + "  and hence, vTeller logs it as 2. For SessionID " + t.sessionid);
            Mylogger1.Info("The Response from Banks is " + acs.Respreturnedcode1 + "  and hence, vTeller logs it as 2. For SessionID " + t.sessionid + " T24 msg ==>" + acs.error_text);
            tsv.UpdateIBSvTeller(t, 2);//unable to debit
            msg = "Error: Unable to debit customer's account for Principal";
            t.ResponseCode = "x03";
            t.Refid = Int32.Parse(logval);
        //    int cn1 = tsv.UpdateIBSTrans(t);
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
            else if (acs.error_text == "Account " + t.nuban + " - Account Upgrade Required (Override ID - POSTING.RESTRICT)")
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
                Mylogger1.Info("x03 response text " + acs.error_text);
            }
            t.ResponseCode = Respval;
            t.Refid = Int32.Parse(logval);
            int cn1 = tsv.UpdateIBSTrans(t);
            return Respval + ":" + logval.ToString() + ":" + acs.error_text;
        }

        //mark trans type
        MarkTransType mt1 = new MarkTransType();
        mt1.markTransType(t.Refid);
        //update vTeller Message column
        tsv.UpdateIBSvTeller(t, 1);//debited successfully
        msg = "Customer has been debitted!";

        //update the tbl_mobile with response from T24
        tsv.UpdateT24FTResponse(acs.Prin_Rsp, acs.Fee_Rsp, acs.Vat_Rsp, t.Refid);

        //return "00: The T24test was successfull..."; //remove this aferwards 
        //if successful


        TR_SingleFundTransferDC sft1 = new TR_SingleFundTransferDC();
        sft1.SessionID = NewSessionid1;
        sft1.DestinationInstitutionCode = DestinationBankCode;
        sft1.ChannelCode = ChannelCode;
        sft1.BeneficiaryAccountName = t.AccountName.Trim();
        sft1.BeneficiaryAccountNumber = AccountNumber;
        sft1.OriginatorAccountName = orignatorName.Trim();
        sft1.BeneficiaryBankVerificationNumber = BeneficiaryBankVerificationNumber;
        sft1.BeneficiaryKYCLevel = BeneficiaryKYCLevel;
        sft1.OriginatorAccountNumber = OriginatorAccountNumber.Trim();
        sft1.OriginatorBankVerificationNumber = OriginatorBankVerificationNumber.Trim();
        sft1.OriginatorKYCLevel = OriginatorKYCLevel;
        Narrationval = "Transfer from " + cusshowname.Trim() + " to " + t.AccountName.Trim();
        if (Narrationval.Length > 100)
        {
            Narrationval = Narrationval.Substring(0, 100);
        }
        sft1.Narration = Narrationval.Trim();// "Transfer from " + cusshowname + " to " + t.AccountName;

        //if (paymentRef.Length > 100)
        //{
        //    paymentRef = paymentRef.Substring(0, 100);
        //}
        sft1.NameEnquiryRef = NameEnquiryRef.Trim();

        sft1.PaymentReference = paymentRef.Trim();
        t.paymentRef = paymentRef.Trim();
        sft1.Amount = amt;
        //sft1.SessionID = NewSessionid1;
        //sft1.DestinationBankCode = DestinationBankCode;
        //sft1.ChannelCode = ChannelCode;
        //sft1.AccountName = t.AccountName;
        //sft1.AccountNumber = AccountNumber;
        //sft1.OriginatorName = orignatorName;
        //sft1.Narration = "Transfer from " + cusshowname + " to " + t.AccountName;

        ////if (paymentRef.Length > 100)
        ////{
        ////    paymentRef = paymentRef.Substring(0, 100);
        ////}

        //sft1.PaymentReference = paymentRef;
        //t.paymentRef = paymentRef;
        //sft1.Amount = amt;
        sft1.createRequest();

        TransactionService trs1 = new TransactionService();
        //send Funds Transfer Request to NIBSS
        sft1.sendRequest();

        string rspstmt1 = "";
        switch (sft1.ResponseCode)
        {
            case "00":
                //success
                rspstmt1 = sft1.ResponseCode;// "3";  //acs.ResponseMsg;
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
                rspstmt1 = sft1.ResponseCode;
                string rsp1 = NIPReversal(logval);
                //new ErrorLog("IBS Reversal was done==>" + rsp1);
                Mylogger1.Info("IBS Reversal was done==>" + rsp1);
                break;
            case "1x":
            default:
                rspstmt1 = sft1.ResponseCode;// "5";
                break;

        }
        //update the table with the response and sessioid for DC
        t.ResponseCode = sft1.ResponseCode;
        t.Refid = Int32.Parse(logval);
        t.sessionid = sft1.SessionID;
        int cn11 = trs1.UpdateMobileTrans(t);

        return rspstmt1 + ":" + logval.ToString() + ":" + acs.tra_seq; ;
    }


    public DataSet getCBNamt(int val)
    {
        string sql = "select minamt,maxamt,cus_class from tbl_nipcbnamt where cus_class = @val and statusflag=1";
        Connect c = new Connect(sql, true);
        c.addparam("@val", val);
        DataSet ds = c.query("rec");
        return ds;
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
        //new ErrorLog(sessionNE + "=> " + resp, 1);
        return resp;
    }

    private bool CheckIfCustomerHasConcession(string nuban)
    {
        TransactionService ts = new TransactionService();
        return ts.getMaxperTransPerday("", "", "", "", "", nuban);
    }

}

