using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using BankDLL;
using System.Threading;
using System.Data;
using System.Xml;
using System.Text;
using System.Configuration;
using System.Net;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class sbp_nibss : System.Web.Services.WebService
{

    string bvn = ""; int sta_code = 0;
    MyEncryDecr m = new MyEncryDecr();
    TR_FinancialInstitutionListRequest fil;
    TR_BulkFundTransferDC bft;
    TR_BulkNameEnquiry bne;
    TransactionService tsv = new TransactionService();
    TR_SingleStatusQuery trx = new TR_SingleStatusQuery();
    AccountService acs = new AccountService();
    static string actualaccount;
    sbp.banks b = new sbp.banks();
    [WebMethod]
    public string FinancialInstitutionListRequest(string request)
    {
        //new ErrorLog("New Financial Institution Request " + request);
        Mylogger.Info("New Financial Institution Request " + request);
        fil = new TR_FinancialInstitutionListRequest();
        fil.xml = request;
        if (!fil.readRequest())
        {
            fil.ResponseCode = "30";
            return fil.createResponse();
        }

        if (fil.BatchNumber != "" || fil.BatchNumber != null)
        {
            Thread worker = new Thread(new ThreadStart(do_UpdateParticipatingbankList));
            worker.Start();
            TR_FinancialInstitutionListRequest trx = fil;
            fil.NumberOfRecords = trx.NumberOfRecords;
            fil.BatchNumber = trx.BatchNumber;
            fil.ChannelCode = trx.ChannelCode;
            fil.ResponseCode = "00";
            return fil.createResponse();
        }
        return "";
    }
    protected void do_UpdateParticipatingbankList()
    {
        TR_FinancialInstitutionListRequest trx = fil;

        for (int i = 0; i < trx.Record.Length; i++)
        {
            Record r = trx.Record[i];
            //insert record into the tbl_participating banks
            InsertInto(r.InstitutionCode, r.InstitutionName, int.Parse(r.Category));
        }

    }
    protected void InsertInto(string bankcode, string bankname, int cat)
    {
        try
        {
            string sql = "";
            sql = "insert into tbl_participatingBanks(bankcode,bankname,category,statusflag) " +
                "values(@bk,@bn,@c,1)";
            Connect c = new Connect(sql, true);
            c.addparam("@bk", bankcode);
            c.addparam("@bn", bankname);
            c.addparam("@c", cat);
            c.insert();
        }
        catch (Exception ex)
        {
            //new ErrorLog("Error Occured " + ex);
            Mylogger.Error("Error Occured ", ex);
        }
    }
    [WebMethod]
    public string balanceenquiry(string request)
    {
        Gadget g = new Gadget();
        TR_Balanceenquiry trx = new TR_Balanceenquiry();
        trx.xml = request;
        //new ErrorLog("Nibss:" + request);
        //new ErrorLog("New Bal request recieved " + request);
        Mylogger.Info("New Bal request recieved " + request);
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }
        else
        {
            decimal custBal = 0;
            ////////////////////////////////////////////
            GetcustomerName gc = new GetcustomerName();
            string cusName = gc.getTheCustomerName(trx.TargetAccountNumber);
            bvn = gc.bvn;
            custBal = gc.FrmAcctbal;
            string balval = g.printMoney(custBal);
            custBal = decimal.Parse(balval);
            trx.AvailableBalance = custBal.ToString();
            ////////////////////////////////////////////
            //commented out 14th dec 2016...Tunde Ifafore
            //if (bvn == trx.TargetBankVerificationNumber)
            //{
            if (cusName == "")
            {
                trx.TargetAccountName = "";
                trx.ResponseCode = "07";
            }
            else if (cusName == trx.TargetAccountName)
            {
                trx.ResponseCode = "00";
                trx.TargetAccountName = cusName;
                return trx.createResponse();
            }
            else
            {
                //name mismatch
                trx.TargetAccountName = "";
                trx.ResponseCode = "08";
            }
            //}
            //else
            //{

            //}
            return trx.createResponse();
        }
    }
    [WebMethod]
    public string nameenquirysingleitem(string request)
    {
        Gadget g = new Gadget();
        int check = 0;
        DataSet ds = new DataSet();
        TR_SingleNameEnquiry trx = new TR_SingleNameEnquiry();
        trx.xml = request;
        string bra_code = "";
        string cus_num = "";
        string cur_code = "";
        string led_code = "";
        string sub_acct_code = "";
        int rest_ind = 0;
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }
        int sbpVal = 0;

        if (trx.AccountNumber.StartsWith("05"))//imal
        {
            sbpVal = 2;
        }
            else if(trx.AccountNumber.StartsWith("99"))//bankone
        {
            sbpVal = 3;
        }
        else
        {
            sbpVal = 1;
        }
        if (sbpVal == 1)
        {
            GetcustomerName gc = new GetcustomerName();
            try
            {
                if (trx.AccountNumber.Length == 10)
                {
                    check = 1;
                }
                else
                {
                    trx.AccountName = "";
                    trx.ResponseCode = "07";
                    return trx.createResponse();
                }

                trx.AccountName = gc.getTheCustomerName(trx.AccountNumber);
                bra_code = gc.Frm_bra_code;
                cus_num = gc.Frm_cus_num;
                cur_code = gc.Frm_cur_code;
                led_code = gc.Frm_led_code;
                sub_acct_code = "";// gc.Frm_sub_acct_code;
                sta_code = gc.sta_code;
                rest_ind = gc.rest_ind;
                bvn = gc.bvn;


                bool kyc = g.findKyc(Int32.Parse(led_code));
                if (kyc)
                {
                    trx.KYCLevel = "1";
                }
                else
                {
                    trx.KYCLevel = "3";
                }

                int RestCode = -2;
                RestCode = gc.Rest_code;
                if (RestCode == -1)
                {
                    //proceed
                    trx.BankVerificationNumber = bvn;
                    trx.ResponseCode = "00";
                }
                else
                {
                    if (gc.Rest_txt == "FALSE")
                    {
                        string sqlRst = "select RestCode from tbl_AllowedRest where RestCode in (@RestCode)";
                        Connect cR = new Connect(sqlRst, true);
                        cR.addparam("@RestCode", RestCode);
                        DataSet dsR = cR.query("rec");
                        if (dsR.Tables[0].Rows.Count == 0)
                        {
                            trx.ResponseCode = "57";
                            return trx.createResponse();
                        }
                    }
                    else
                    {
                        //check get the account res indicator to know if it is allowed to receive credit
                        string sqlRst = "select RestCode from tbl_AllowedRest where RestCode in " + gc.res;
                        Connect cR = new Connect(sqlRst, true);
                        DataSet dsR = cR.query("rec");
                        if (dsR.Tables[0].Rows.Count == 0)
                        {
                            trx.ResponseCode = "57";
                            return trx.createResponse();
                        }
                    }
                }

                //check to ensure only allowed ledgers can tranact
                if (led_code == "3146" || led_code == "3147" || led_code == "3144" || led_code == "3148" || led_code == "3148" || led_code == "3145" || led_code == "3151" || led_code == "6602" || led_code == "1701")
                {
                    trx.ResponseCode = "57";
                    return trx.createResponse();
                }
                //ensure the account is active alone
                if (sta_code == 2)
                {
                    trx.ResponseCode = "57";
                    return trx.createResponse();
                }
                //ensure account is not closed
                if (sta_code == 3)
                {
                    trx.ResponseCode = "06";
                    return trx.createResponse();
                }

                //check if the sending bank is active in our list of participating bank
                string Sid = trx.SessionID;
                //Sid = Sid.Substring(0, 3);
                Sid = Sid.Substring(0, 6);
                string sqlSid = "";
                sqlSid = "select * from tbl_participatingBanks where bankcode = @bcode and statusflag=1";
                Connect cSid = new Connect(sqlSid, true);
                cSid.addparam("@bcode", Sid);
                DataSet dsSid = cSid.query("rec");
                if (dsSid.Tables[0].Rows.Count == 0)
                {
                    trx.ResponseCode = "57";
                    return trx.createResponse();
                }
                //ensure restrictions are not within this 
                if (rest_ind == 2 || rest_ind == 3 || rest_ind == 6 || rest_ind == 7)
                {
                    trx.AccountName = "";
                    trx.ResponseCode = "58";
                    return trx.createResponse();
                }

                if (cur_code != "NGN")
                {
                    trx.AccountName = "";
                    trx.ResponseCode = "57";
                    return trx.createResponse();
                }
                long cnu = Convert.ToInt64(cus_num);
                if (cnu < 20000)
                {
                    trx.AccountName = "";
                    trx.ResponseCode = "57";
                    return trx.createResponse();
                }
                trx.ResponseCode = "00";
                ///////////////////////////////////////////////////
                return trx.createResponse();
            }
            catch (Exception ex)
            {
                new ErrorLog("Error occured during name enquiry for" + trx.SessionID + " " + ex);
                trx.AccountName = "";
                trx.ResponseCode = "07";
                return trx.createResponse();
            }
        }
        else if (sbpVal == 2)
        {
            //call webservice
            string rsp = "";
            imal.NIBankingClientService im = new imal.NIBankingClientService();
            trx.AccountNumber = m.Encrypt(trx.AccountNumber, "1239879000");
            rsp = im.nameEnquiry(trx.AccountNumber, "xx");
            rsp = m.Decrypt(rsp, "1239879000");
            new ErrorLog("Response gotten for IMAL Name enquiry for account " + m.Decrypt(trx.AccountNumber, "1239879000") + " is " + rsp);
            trx.AccountNumber = m.Decrypt(trx.AccountNumber, "1239879000");
            string[] bits = rsp.Split(':');
            trx.AccountName = bits[1];
            trx.ResponseCode = bits[2];
            return trx.createResponse();
        }
        else if (sbpVal == 3)
        {
            string acctresp = "";
            string acctName = "";
            string ResponseCode = "";
            string BVN = "";
            string KYCLevel = "";
            //call bank one webservice
            string InstCode = ConfigurationSettings.AppSettings["AppZoneIntCode"].ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append("<AccountNameVerificationRequest>");
            sb.Append("<InstitutionCode>" + InstCode + "</InstitutionCode>");
            sb.Append("<AccountNumber>" + trx.AccountNumber + "</AccountNumber>");
            sb.Append("</AccountNameVerificationRequest>");
            AccountNameVerificationRequest rqt = new AccountNameVerificationRequest("5", trx.AccountNumber);
            string rsp = "";
            try
            {
                //init pn = init();
                rsp = init().BankOneGetAccountName(sb.ToString());
                //Deserialize
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(rsp);
                acctresp = xmlDoc.GetElementsByTagName("Status").Item(0).InnerText;
                acctName = xmlDoc.GetElementsByTagName("AccountName").Item(0).InnerText;
                ResponseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                BVN = xmlDoc.GetElementsByTagName("BVN").Item(0).InnerText;
                KYCLevel = xmlDoc.GetElementsByTagName("KYCLevel").Item(0).InnerText;
                if (acctresp == "True")
                {
                    trx.AccountName = acctName;
                    trx.ResponseCode = ResponseCode;
                    return trx.createResponse();
                }
                else
                {
                    trx.AccountName = "";
                    trx.ResponseCode = "07";
                    return trx.createResponse();
                }
            }
            catch (Exception ex)
            {
                trx.AccountName = "";
                trx.ResponseCode = "07";
                return trx.createResponse();
            }
        }
        else
        {
            trx.AccountName = "";
            trx.ResponseCode = "26";
            return trx.createResponse();
        }
        //}
    }
    public BankOneService.SterlingFundsTransferService init()
    {
        BankOneService.SterlingFundsTransferService c = new BankOneService.SterlingFundsTransferService();
        string urlLive = ConfigurationSettings.AppSettings["BankOneService.SterlingFundsTransferService"].ToString();
        c.Url = urlLive;
        int ServPort = Convert.ToInt32(ConfigurationSettings.AppSettings["ServerPort"].ToString());
        string serProxy = ConfigurationSettings.AppSettings["ServerProxy"].ToString();
        IWebProxy proxy = new WebProxy(serProxy, ServPort);
        c.Proxy = proxy;
        c.RequestEncoding = Encoding.GetEncoding("utf-8");
        NetworkCredential netCredential = new NetworkCredential();
        Uri uri = new Uri(c.Url);
        ICredentials credentials = netCredential.GetCredential(uri, "Basic");
        c.Credentials = credentials;
        c.PreAuthenticate = true;
        return c;
    }
    [WebMethod]
    public string fundtransferbulkitem_dc(string request)
    {
        bft = new TR_BulkFundTransferDC();
        bft.xml = request;
        if (!bft.readRequest())
        {
            bft.ResponseCode = "30";
            return bft.createResponse();
        }
        if (bft.BatchNumber == "1010101010101010101")
        {
            Thread worker = new Thread(new ThreadStart(do_fundtransferbulkitem_dc));
            worker.Start();
            bft.ResponseCode = "09";
            return bft.createResponse();
        }
        else
        {
            Thread worker = new Thread(new ThreadStart(do_fundtransferbulkitem_dc));
            worker.Start();
            bft.ResponseCode = "09";
            return bft.createResponse();
        }
    }


    private bool MandateCodeExist(string MandateRef)
    {
        bool found = false; DataSet ds = new DataSet();
        string sql = "select MandateReferenceNumber from tbl_nip_mandate where MandateReferenceNumber=@mc";
        Connect c = new Connect(sql, true);
        c.addparam("@mc", MandateRef);
        try
        {
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            Mylogger.Error("Error occured checking if mandateref exist ", ex);
        }
        if (ds.Tables[0].Rows.Count > 0)
        {
            found = true;
        }
        else
        {
            found = false;
        }
        return found;
    }

    private bool MandateCodeAndAmountExist(string MandateRef, decimal amt)
    {
        bool found = false; DataSet ds = new DataSet();
        string sql = "select MandateReferenceNumber,Amount from tbl_nip_mandate where MandateReferenceNumber=@mc and Amount=@amt";
        Connect c = new Connect(sql, true);
        c.addparam("@mc", MandateRef);
        c.addparam("@amt", amt);
        try
        {
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            Mylogger.Error("Error occured checking if mandate exist ", ex);
        }
        if (ds.Tables[0].Rows.Count > 0)
        {
            found = true;
        }
        else
        {
            found = false;
        }
        return found;
    }

    [WebMethod]
    public string MandateAdviceRequest(string request)
    {
        Gadget g = new Gadget();
        TR_MandateAdvice trx = new TR_MandateAdvice();
        trx.xml = request;
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }

        if (trx.MandateReferenceNumber == "")
        {
            trx.ResponseCode = "71";
            return trx.createResponse();
        }

        //check if mandate already exist
        bool isMandateExist = MandateCodeExist(trx.MandateReferenceNumber);
        if (isMandateExist)
        {
            trx.ResponseCode = "26";
            return trx.createResponse();
        }
        try
        {
            //proceed to insert if all is fine
            string sql = "";
            sql = "insert into tbl_nip_mandate(SessionID,DestinationInstitutionCode,ChannelCode,MandateReferenceNumber,Amount,DebitAccountName,DebitAccountNumber, " +
                " DebitBankVerificationNumber,DebitKYCLevel,BeneficiaryAccountName,BeneficiaryAccountNumber,BeneficiaryBankVerificationNumber,BeneficiaryKYCLevel) " +
                " values(@sid,@dcode,@cc,@mn,@amt,@debi_acc_name,@debit_acc_num,@dbvn,@bkyc,@ban,@banNum,@bbvn,@bkycl)";

            if (trx.DebitKYCLevel == "" || trx.DebitKYCLevel == null)
            {
                trx.DebitKYCLevel = "1";
            }
            if (trx.BeneficiaryKYCLevel == "" || trx.BeneficiaryKYCLevel == null)
            {
                trx.BeneficiaryKYCLevel = "1";
            }

            Connect c = new Connect(sql, true);
            c.addparam("@sid", trx.SessionID);
            c.addparam("@dcode", trx.DestinationInstitutionCode);
            c.addparam("@cc", trx.ChannelCode);
            c.addparam("@mn", trx.MandateReferenceNumber);
            c.addparam("@amt", decimal.Parse(trx.Amount));
            c.addparam("@debi_acc_name", trx.DebitAccountName);
            c.addparam("@debit_acc_num", trx.DebitAccountNumber);
            c.addparam("@dbvn", trx.DebitBankVerificationNumber);
            c.addparam("@bkyc", int.Parse(trx.DebitKYCLevel));
            c.addparam("@ban", trx.BeneficiaryAccountName);
            c.addparam("@banNum", trx.BeneficiaryAccountNumber);
            c.addparam("@bbvn", trx.BeneficiaryBankVerificationNumber);
            c.addparam("@bkycl", trx.BeneficiaryKYCLevel);
            int cn = c.query();
            if (cn > 0)
            {
                trx.ResponseCode = "00";
            }
            else
            {
                trx.ResponseCode = "05";
            }
        }
        catch (Exception ex)
        {
            trx.ResponseCode = "05";
            //new ErrorLog("Error occured inserting into tbl_nip_mandate table " + ex);
            Mylogger.Error("Error occured inserting into tbl_nip_mandate table ", ex);
        }
        return trx.createResponse();
    }

    [WebMethod]
    public string fundtransfersingleitem_dd(string request)
    {
        Gadget g = new Gadget();
        int check = 0;
        TR_SingleFundTransferDD trx = new TR_SingleFundTransferDD();

        trx.xml = request;
        string bra_code = "";
        string cus_num = "";
        string cur_code = "";
        string led_code = "";
        string sub_acct_code = "";

        string TSS_cus_num = "";
        string TSS_cur_code = "";
        string TSS_led_code = "";
        string TSS_sub_acct_code = "";

        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }

        Gadget ggg = new Gadget();
        if (trx.SessionID.StartsWith("999"))
        {
            //proceed
        }
        else
        {
            bool SessionExist = ggg.isSessionIDExist(trx.SessionID);
            if (SessionExist)
            {
                trx.ResponseCode = "26";
                return trx.createResponse();
            }
            else
            {
                //proceed
            }
        }

        if (trx.MandateReferenceNumber == "")
        {
            trx.ResponseCode = "71";
            return trx.createResponse();
        }

        //check to ensure the mandate already exit else respond with 25
        bool checkMandate = MandateCodeExist(trx.MandateReferenceNumber);
        if (!checkMandate)
        {
            trx.ResponseCode = "05";
            return trx.createResponse();
        }
        //commented out by Tosin Ogunniran 01/19/2018
        //bool checkMandateandAmt = MandateCodeAndAmountExist(trx.MandateReferenceNumber, decimal.Parse(trx.Amount));
        //if (!checkMandateandAmt)
        //{
        //    trx.ResponseCode = "05";
        //    return trx.createResponse();
        //}
        //check if the account number is equal to 10 (NUBAN)
        //if equal to 10 then get the corresponding customer account
        int rest_ind = 0; decimal custBal = 0;
        ////////////////////////////////////////////
        GetcustomerName gc = new GetcustomerName();
        string cusName = gc.getTheCustomerName(trx.DebitAccountNumber);
        bra_code = gc.Frm_bra_code;
        cus_num = gc.Frm_cus_num;
        cur_code = gc.Frm_cur_code;
        led_code = gc.Frm_led_code;
        //sub_acct_code = gc.Frm_sub_acct_code;
        sub_acct_code = trx.DebitAccountNumber;
        sta_code = gc.sta_code;
        rest_ind = gc.rest_ind;
        bvn = gc.bvn;
        custBal = gc.FrmAcctbal;
        ////////////////////////////////////////////

        if (rest_ind != 0)
        {
            trx.ResponseCode = "57";
            return trx.createResponse();
        }

        bool kyc = g.findKyc(Int32.Parse(led_code));
        if (kyc)
        {
            trx.BeneficiaryKYCLevel = "1";
        }
        else
        {
            trx.BeneficiaryKYCLevel = "3";
        }


        ////////////////////////////////////////////////////
        //check if customer name exist in the folder CusName and use the name there
        GetCusName The_Cus_Name = new GetCusName();

        cusName = The_Cus_Name.getCusName(trx.DebitAccountNumber, trx.DebitAccountName);
        ///////////////////////////////////////////////////

        if (cusName.ToUpper() == trx.DebitAccountName.ToUpper())
        {
            //trx.AccountName = dr["cus_sho_name"].ToString();
            //bra_code = dr["bra_code"].ToString();
            //cus_num = dr["cus_num"].ToString();
            //cur_code = dr["cur_code"].ToString();
            //led_code = dr["led_code"].ToString();
            //sub_acct_code = dr["sub_acct_code"].ToString();
            //log the incoming request to tbl_WStrans
            Connect c1 = new Connect("spd_WSInserttrans_DD");
            c1.addparam("@sessionid", trx.SessionID);
            c1.addparam("@bra_code", bra_code);
            c1.addparam("@cus_num", cus_num);
            c1.addparam("@cur_code", cur_code);
            c1.addparam("@led_code", led_code);
            c1.addparam("@sub_acct_code", sub_acct_code);
            c1.addparam("@Transnature", 0);
            c1.addparam("@batchNum", "");
            c1.addparam("@originBankCode", trx.DestinationInstitutionCode);
            c1.addparam("@channelCode", trx.ChannelCode);
            c1.addparam("@status", 0);
            c1.addparam("@mandateRefNum", trx.MandateReferenceNumber);
            c1.addparam("@BillerID", "0");
            c1.addparam("@BillerName", "");
            c1.addparam("@nuban", trx.DebitAccountNumber);
            c1.query();
        }
        else
        {
            trx.AccountName = "Name Mismatch";
            trx.ResponseCode = "08";
            return trx.createResponse();
        }
        //Gadget g = new Gadget();
        //prepare to send to debiting
        AccountService acs = new AccountService();

        Transaction t = new Transaction();
        t.inCust.bra_code = bra_code;
        t.inCust.cus_num = cus_num;
        t.inCust.cur_code = cur_code;
        t.inCust.led_code = led_code;
        t.inCust.sub_acct_code = sub_acct_code;
        t.amount = decimal.Parse(trx.Amount);
        t.tellerID = "9990"; //remember to get a teller id
        t.Remark = g.GetBankNames(trx.SessionID.Substring(0, 3)) + " Trns type:Debit";
        t.feecharge = decimal.Parse(trx.TransactionFee);
        t.narration = trx.Narration;
        t.senderAcctname = trx.BeneficiaryAccountName;
        //acs.authorizeTrnxToSterling_DD(t);

        //check if TSS has sufficient Balance to cater for the Transaction amount
        TransactionService ts = new TransactionService();
        DataSet dsTss = ts.getCurrentTss();
        int TSSLast4; string TSSAcct = "";
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];


            string TSS_bra_code = "NG0020001";
            TSS_cus_num = drTss["cusnum"].ToString();
            TSS_cur_code = "NGN";
            TSS_led_code = "12501";
            TSSLast4 = int.Parse(TSS_bra_code.Substring(6, 3)) + 2000;
            TSSAcct = "NGN" + TSS_led_code + "0001" + TSSLast4.ToString();
            TSS_sub_acct_code = TSSAcct;
        }


        //bal = atss.getBalance(bal);
        if (custBal < decimal.Parse(trx.Amount))
        {
            trx.ResponseCode = "51";
            trx.AccountName = "No sufficient funds in TSS account";
            return trx.createResponse();
        }
        t.sessionid = trx.SessionID;
        //acs.authorizeTrnxToSterling_dd(t);
        acs.authorizeTrnxToSterlingDD(t);
        //if (acs.Respreturnedcode1 == "0")
        //{
        if (acs.Respreturnedcode1 == "0")
        {
            trx.ResponseCode = "00";
            //trx.Amount = trx.Amount;// g.TRUmoneyToISOmoney(Convert.ToDecimal(acs.RespCreditedamt));

            //log trnx
            Connect c = new Connect("spd_WSupdatetrans");
            c.addparam("@sessionid", trx.SessionID);
            c.addparam("@bra_code", bra_code);
            c.addparam("@cus_num", cus_num);
            c.addparam("@cur_code", cur_code);
            c.addparam("@led_code", led_code);
            c.addparam("@sub_acct_code", sub_acct_code);
            c.addparam("@amt", t.amount);
            c.addparam("@payRef", trx.PaymentReference);
            c.addparam("@manRef", trx.MandateReferenceNumber);
            c.addparam("@remark", trx.Narration);
            c.addparam("@originSender", "");
            c.addparam("@Responsecode", trx.ResponseCode);
            c.addparam("@accname", trx.DebitAccountName);
            c.addparam("@feecharge", t.feecharge);
            c.addparam("@ResponseMsg", g.responseCodes(trx.ResponseCode));

            c.addparam("@NameEnquiryRef", trx.NameEnquiryRef);
            c.addparam("@BeneficiaryBankVerificationNumber", trx.BeneficiaryBankVerificationNumber);
            c.addparam("@OriginatorAccountNumber", trx.DebitAccountNumber);
            c.addparam("@OriginatorBankVerificationNumber", trx.DebitBankVerificationNumber);
            c.addparam("@OriginatorKYCLevel", int.Parse(trx.DebitKYCLevel));
            c.addparam("@TransactionLocation", trx.TransactionLocation);
            c.query();

            string sql21 = "";
            sql21 = "update tbl_WStrans set staggingStatus =@s,TransProcessed=1,TransProcessDate=getdate(),Prin_Rsp=@Prin_Rsp,Fee_Rsp=@Fee_Rsp,Vat_Rsp=@Vat_Rsp where sessionid=@sid";
            Connect c1 = new Connect(sql21, true);
            c1.addparam("@s", 1);
            c1.addparam("@sid", t.sessionid);
            c1.addparam("@Prin_Rsp", acs.Prin_Rsp);
            c1.addparam("@Fee_Rsp", acs.Fee_Rsp);
            c1.addparam("@Vat_Rsp", acs.Vat_Rsp);
            try
            {
                c1.query();
            }
            catch
            {

            }
        }
        else
        {
            trx.ResponseCode = "21";
            string sql21 = "";
            sql21 = "update tbl_WStrans set Responsecode =@Rcode,TransProcessDate=getdate() where sessionid=@sid";
            Connect c1 = new Connect(sql21, true);
            c1.addparam("@Rcode", trx.ResponseCode);
            c1.addparam("@sid", t.sessionid);
            try
            {
                c1.query();
            }
            catch
            {

            }
        }
        //}
        //else
        //{
        //    trx.ResponseCode = "21";
        //}

        if (check == 1)
        {
            trx.AccountNumber = actualaccount;
        }
        return trx.createResponse();
    }

    protected void do_fundtransferbulkitem_dc()
    {
        TR_BulkFundTransferDC trx = bft;
        TR_BulkFundTransferNotificationDC rsp = new TR_BulkFundTransferNotificationDC();
        rsp.DestinationBankCode = trx.DestinationBankCode;
        rsp.ChannelCode = trx.ChannelCode;
        rsp.BatchNumber = trx.BatchNumber;
        rsp.NumberOfRecords = trx.NumberOfRecords;

        //check if batch exists

        rsp.createRequest();
        for (int i = 0; i < trx.Record.Length; i++)
        {
            Record r = trx.Record[i];
            bank2 b = new bank2();

            //do account enquiry
            b.Getcusname(r.AccountNumber);
            if (b.Responsecode == "00")
            {
                //account exists
                Connect c1 = new Connect("spd_WScheckpendingtrans");
                c1.addparam("@sessionid", r.RecID);
                c1.addparam("@bra_code", b.bra_code);
                c1.addparam("@cus_num", b.cus_num);
                c1.addparam("@cur_code", b.cur_code);
                c1.addparam("@led_code", b.led_code);
                c1.addparam("@sub_acct_code", b.sub_acct_code);
                DataSet ds = c1.query("trnx");
                if (ds.Tables[0].Rows.Count < 1)
                {
                    r.ResponseCode = "57";
                }
                else
                {
                    //trx is pending
                    //authorize -- send to banks
                    Gadget g = new Gadget();
                    //go to vTeller
                    AccountService acs = new AccountService();

                    Transaction t = new Transaction();
                    t.inCust.bra_code = b.bra_code;
                    t.inCust.cus_num = b.cus_num;
                    t.inCust.cur_code = b.cur_code;
                    t.inCust.led_code = b.led_code;
                    t.inCust.sub_acct_code = b.sub_acct_code;
                    t.amount = g.ISOmoneyToTRUmoney(r.Amount);
                    t.tellerID = "9990"; //remember to get a teller id
                    acs.authorizeTrnxToSterling(t);

                    if (acs.Respreturnedcode1 == "0")
                    {
                        r.ResponseCode = "00";
                        r.Amount = g.TRUmoneyToISOmoney(Convert.ToDecimal(acs.RespCreditedamt));

                        //log trnx
                        Connect c = new Connect("spd_WSupdatetrans");
                        c.addparam("@sessionid", r.RecID);
                        c.addparam("@bra_code", b.bra_code);
                        c.addparam("@cus_num", b.cus_num);
                        c.addparam("@cur_code", b.cur_code);
                        c.addparam("@led_code", b.led_code);
                        c.addparam("@sub_acct_code", b.sub_acct_code);
                        c.addparam("@amt", t.amount);
                        c.addparam("@payRef", r.PaymentReference);
                        c.addparam("@manRef", "");
                        c.addparam("@remark", r.Narration);
                        c.addparam("@originSender", r.OriginatorName);
                        c.query();
                    }
                    else
                    {
                        r.ResponseCode = "21";
                    }
                }
            }
            else
            {
                //wrong account
                r.ResponseCode = "07";
            }
            rsp.addRecord(r);
        }

        //make ft notification request
        rsp.sendRequest();
    }

    [WebMethod]
    public string nameenquirybulkitem(string request)
    {
        bne = new TR_BulkNameEnquiry();
        bne.xml = request;
        if (!bne.readRequest())
        {
            bne.ResponseCode = "30";
            return bne.createResponse();
        }
        if (bne.BatchNumber == "1010101010101010101")
        {
            Thread worker = new Thread(new ThreadStart(do_nameenquirybulkitem));
            worker.Start();

            bne.ResponseCode = "09";
            return bne.createResponse();
        }
        else
        {
            Thread worker = new Thread(new ThreadStart(do_nameenquirybulkitem));
            worker.Start();

            bne.ResponseCode = "09";
            return bne.createResponse();
        }
    }

    protected void do_nameenquirybulkitem()
    {
        TR_BulkNameEnquiry trx = bne;
        TR_BulkNameEnquiryNotification rsp = new TR_BulkNameEnquiryNotification();
        rsp.DestinationBankCode = trx.DestinationBankCode;
        rsp.ChannelCode = trx.ChannelCode;
        rsp.BatchNumber = trx.BatchNumber;
        rsp.NumberOfRecords = trx.NumberOfRecords;

        rsp.createRequest();

        //read thru the record
        for (int i = 0; i < trx.Record.Length; i++)
        {
            Record r = trx.Record[i];
            //check if this account exists for each
            bank2 b = new bank2();
            int status = 0;
            b.Getcusname(r.AccountNumber);
            if (b.Responsecode == "00")
            {
                r.AccountName = b.cus_sho_name;
                r.ResponseCode = "00";
                status = 1;
            }
            else
            {
                r.ResponseCode = "07";
                status = 0;
            }

            rsp.addRecord(r);
            //log to database
            Connect c = new Connect("spd_WSInserttrans");
            c.addparam("@sessionid", r.RecID);
            c.addparam("@bra_code", b.bra_code);
            c.addparam("@cus_num", b.cus_num);
            c.addparam("@cur_code", b.cur_code);
            c.addparam("@led_code", b.led_code);
            c.addparam("@sub_acct_code", b.sub_acct_code);
            c.addparam("@Transnature", 1);
            c.addparam("@batchNum", rsp.BatchNumber);
            c.addparam("@originBankCode", rsp.DestinationBankCode);
            c.addparam("@channelCode", rsp.ChannelCode);
            c.addparam("@status", status);
            c.query();
        }

        if (!rsp.sendRequest())
        {
            //request did not go!            
        }

    }
    public void updateInwardType(string sessionid, int inwardtype)
    {
        string sql = "update tbl_WStrans set inwardtype=@inw where sessionid=@sid";
        Connect c = new Connect(sql, true);
        c.addparam("@inw", inwardtype);
        c.addparam("@sid", sessionid);
        c.query();
    }
    [WebMethod]
    public string fundtransfersingleitem_dc(string request)
    {
        Int32 val = 0;
        DateTime startTime = DateTime.Now;
        //////////////////////////////////////
        DataSet ds = new DataSet();
        TR_SingleFundTransferDC trx = new TR_SingleFundTransferDC();

        trx.xml = request;
        string bra_code = "";
        string cus_num = "";
        string cur_code = "";
        string led_code = "";
        string sub_acct_code = "";

        Transaction t = new Transaction();
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }
        Gadget ggg = new Gadget();
        if (trx.SessionID.StartsWith("999"))
        {
            //proceed
        }
        else
        {
            bool SessionExist = ggg.isSessionIDExist(trx.SessionID);
            if (SessionExist)
            {
                trx.ResponseCode = "26";
                return trx.createResponse();
            }
            else
            {
                //proceed
            }
        }

        t.NameEnquiryRef = trx.NameEnquiryRef;
        t.BeneficiaryBankVerificationNumber = trx.BeneficiaryBankVerificationNumber;
        t.OriginatorAccountNumber = trx.OriginatorAccountNumber;
        t.OriginatorBankVerificationNumber = trx.OriginatorBankVerificationNumber;
        t.OriginatorKYCLevel = trx.OriginatorKYCLevel;
        t.TransactionLocation = trx.TransactionLocation;
        t.amount = decimal.Parse(trx.Amount);


        int sbpVal = 0;
        if (trx.BeneficiaryAccountNumber.StartsWith("05"))//imal
        {
            sbpVal = 2;
        }
        else if (trx.BeneficiaryAccountNumber.StartsWith("99"))//bankone
        {
            sbpVal = 3;
        }
        else
        {
            sbpVal = 1;
        }
        if (sbpVal == 1)
        {
            Gadget g = new Gadget();
            //Removal of checking the name in the core and log the request direct////////////////////
            try
            {
                DateTime today = DateTime.Today;
                DateTime dt = DateTime.Now;
                DateTime dt2 = new DateTime(today.Year, today.Month, today.Day, 23, 40, 0);

                if (dt > dt2)
                {
                    dt = today.AddDays(1).AddMinutes(5);
                }
                Connect c = new Connect("spd_WSInserttrans");
                c.addparam("@sessionid", trx.SessionID);
                c.addparam("@bra_code", bra_code);
                c.addparam("@cus_num", cus_num);
                c.addparam("@cur_code", cur_code);
                c.addparam("@led_code", led_code);
                c.addparam("@sub_acct_code", sub_acct_code);
                c.addparam("@Transnature", 0);
                c.addparam("@batchNum", "");
                c.addparam("@originBankCode", trx.DestinationInstitutionCode);
                c.addparam("@channelCode", trx.ChannelCode);
                c.addparam("@status", 0);
                c.addparam("@transtype", "Fund Transfer Request:Direct Credit-Single Item");
                c.addparam("@amt", decimal.Parse(trx.Amount));
                c.addparam("@nuban", trx.BeneficiaryAccountNumber);
                c.addparam("@tradate", dt);
                int cv = c.query();

                if (cv > 0)
                {
                    trx.ResponseCode = "00";

                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                //new ErrorLog("Error occured while inserting DC for " + trx.SessionID);
                Mylogger.Error("Error occured while inserting DC for " + trx.SessionID, ex);
            }

            ///////////////////////////////////////////////////////////


            //update the transaction type against the sessionid
            updateInwardType(trx.SessionID, 1);

            t.amount = decimal.Parse(trx.Amount);

            t.sessionid = trx.SessionID;

            //trx.Amount = acs.RespCreditedamt;

            //log trnx
            Connect c11 = new Connect("spd_WSupdatetrans");
            c11.addparam("@sessionid", trx.SessionID);
            c11.addparam("@bra_code", bra_code);
            c11.addparam("@cus_num", cus_num);
            c11.addparam("@cur_code", cur_code);
            c11.addparam("@led_code", led_code);
            c11.addparam("@sub_acct_code", sub_acct_code);
            c11.addparam("@amt", t.amount);
            c11.addparam("@payRef", trx.PaymentReference);
            c11.addparam("@manRef", "");
            c11.addparam("@remark", trx.Narration);
            c11.addparam("@originSender", trx.OriginatorAccountName);
            c11.addparam("@Responsecode", trx.ResponseCode);
            c11.addparam("@accname", trx.BeneficiaryAccountName);
            c11.addparam("@feecharge", 0);
            c11.addparam("@ResponseMsg", g.responseCodes(trx.ResponseCode));

            c11.addparam("@NameEnquiryRef", trx.NameEnquiryRef);
            c11.addparam("@BeneficiaryBankVerificationNumber", trx.BeneficiaryBankVerificationNumber);
            c11.addparam("@OriginatorAccountNumber", trx.OriginatorAccountNumber);
            c11.addparam("@OriginatorBankVerificationNumber", trx.OriginatorBankVerificationNumber);


            c11.addparam("@OriginatorKYCLevel", trx.OriginatorKYCLevel);
            c11.addparam("@TransactionLocation", trx.TransactionLocation);
            int fin = 0;
            try
            {
                fin = c11.query();
            }
            catch (Exception ex)
            {
                new ErrorLog("An error occured updating record with sessionid " + trx.SessionID + " as "+ ex);
            }

            if (fin > 0)
            {
                trx.ResponseCode = "00";
            }
            else
            {

            }
            trx.Amount = t.amount.ToString();
            return trx.createResponse();
        }
        else if (sbpVal == 2)
        {
            string rsp = ""; string rsp1 = ""; string stacode = "";
            //imal transactions
            imal.NIBankingClientService im = new imal.NIBankingClientService();
            //check for duplicate
            Connect cx = new Connect("spd_CheckDuplicateSessionid2");
            cx.addparam("@sessionid", trx.SessionID);
            int donx = cx.query();
            if (donx == 0)
            {
                trx.ResponseCode = "26";
                new ErrorLog("DUPLICATE TRNX");
                new ErrorLog(trx.xml);
                return trx.createResponse();
            }

            //go to IMAL and get the customers account details
            trx.BeneficiaryAccountNumber = m.Encrypt(trx.BeneficiaryAccountNumber, "1239879000");
            rsp1 = im.getAccountDetails(trx.BeneficiaryAccountNumber.Trim(), "xx");
            rsp1 = m.Decrypt(rsp1, "1239879000");
            Gadget gg = new Gadget();
            string[] bits1 = rsp1.Split(':');
            //DataRow dr = ds.Tables[0].Rows[0];
            trx.BeneficiaryAccountNumber = bits1[0];
            string cusNam = trx.BeneficiaryAccountName;
            if (cusNam.ToUpper() == trx.BeneficiaryAccountName.ToUpper())
            {

                trx.BeneficiaryAccountName = bits1[1];
                bra_code = bits1[2];
                cus_num = bits1[3];
                cur_code = bits1[4];
                led_code = bits1[5];
                sub_acct_code = bits1[6];
                string stacode1 = bits1[7];

                DateTime today = DateTime.Today;
                DateTime dt = DateTime.Now;
                DateTime dt2 = new DateTime(today.Year, today.Month, today.Day, 23, 40, 0);

                if (dt > dt2)
                {
                    dt = today.AddDays(1).AddMinutes(5);
                }

                Connect c = new Connect("spd_WSInserttrans");
                c.addparam("@sessionid", trx.SessionID);
                c.addparam("@bra_code", bra_code);
                c.addparam("@cus_num", cus_num);
                c.addparam("@cur_code", cur_code);
                c.addparam("@led_code", led_code);
                c.addparam("@sub_acct_code", sub_acct_code);
                c.addparam("@Transnature", 0);
                c.addparam("@batchNum", "");
                c.addparam("@originBankCode", trx.DestinationInstitutionCode);
                c.addparam("@channelCode", trx.ChannelCode);
                c.addparam("@status", 0);
                c.addparam("@transtype", "Fund Transfer Request:Direct Credit-Single Item");
                c.addparam("@amt", decimal.Parse(trx.Amount));
                c.addparam("@nuban", trx.BeneficiaryAccountNumber);
                c.addparam("@tradate", dt);
                int cv = c.query();

                //update the transaction type against the sessionid
                updateInwardType(trx.SessionID, 2);

                if (stacode1 == "A" || stacode1 == "I")
                {
                    //proceed
                }
                else
                {
                    trx.ResponseCode = "57";
                    return trx.createResponse();
                }

                if (cur_code != "566")
                {
                    trx.ResponseCode = "57";
                    return trx.createResponse();
                }

            }
            else
            {
                trx.BeneficiaryAccountName = "";
                trx.ResponseCode = "08";
                Connect c1 = new Connect("spd_WSupdatetrans");
                c1.addparam("@sessionid", trx.SessionID);
                c1.addparam("@bra_code", bra_code);
                c1.addparam("@cus_num", cus_num);
                c1.addparam("@cur_code", cur_code);
                c1.addparam("@led_code", led_code);
                c1.addparam("@sub_acct_code", sub_acct_code);
                c1.addparam("@amt", decimal.Parse(trx.Amount));
                c1.addparam("@payRef", trx.PaymentReference);
                c1.addparam("@manRef", "");
                c1.addparam("@remark", trx.Narration);
                c1.addparam("@originSender", trx.OriginatorAccountName);
                c1.addparam("@Responsecode", trx.ResponseCode);
                c1.addparam("@accname", trx.BeneficiaryAccountName);
                c1.addparam("@feecharge", 0);
                c1.addparam("@ResponseMsg", gg.responseCodes(trx.ResponseCode));

                c1.addparam("@NameEnquiryRef", trx.NameEnquiryRef);
                c1.addparam("@BeneficiaryBankVerificationNumber", trx.BeneficiaryBankVerificationNumber);
                c1.addparam("@OriginatorAccountNumber", trx.OriginatorAccountNumber);
                c1.addparam("@OriginatorBankVerificationNumber", trx.OriginatorBankVerificationNumber);

                c1.addparam("@OriginatorKYCLevel", trx.OriginatorKYCLevel);
                c1.addparam("@TransactionLocation", trx.TransactionLocation);
                c1.query();
                return trx.createResponse();
            }

            //calling TSQ from NIBSS for comfirmation before applying credit 
            //TSQ calreq = new TSQ();
            //var response = calreq.NIBSSrequery(trx.SessionID, trx.ChannelCode, trx.DestinationInstitutionCode);
            //if (response != "00")
            //{

            //}
            //end calling TSQ from NIBSS for comfirmation before applying credit 

            //now processing this part on job
            //trx.SessionID = m.Encrypt(trx.SessionID, "1239879000");
            //trx.OriginatorAccountName = m.Encrypt(trx.OriginatorAccountName, "1239879000");
            ////trx.AccountNumber = m.Encrypt(trx.AccountNumber, "1239879000");
            //trx.Amount = m.Encrypt(trx.Amount, "1239879000");
            //trx.PaymentReference = m.Encrypt(trx.PaymentReference, "1239879000");
            //trx.Narration = m.Encrypt(trx.Narration, "1239879000");

            //rsp = im.ftSingleCreditRequest(trx.SessionID, trx.OriginatorAccountName, trx.BeneficiaryAccountNumber, trx.Amount, trx.PaymentReference, trx.Narration, "XXX");
            //rsp = m.Decrypt(rsp, "1239879000");
            //new ErrorLog("Response gotten for IMAL FundTransfer for account " + trx.SessionID + " " + trx.BeneficiaryAccountNumber + " is " + rsp);
            //string[] bits = rsp.Split(':');

            //trx.SessionID = m.Decrypt(trx.SessionID, "1239879000");
            //trx.OriginatorAccountName = m.Decrypt(trx.OriginatorAccountName, "1239879000");
            //trx.BeneficiaryAccountNumber = m.Decrypt(trx.BeneficiaryAccountNumber, "1239879000");
            //trx.Amount = m.Decrypt(trx.Amount, "1239879000");
            //trx.PaymentReference = m.Decrypt(trx.PaymentReference, "1239879000");
            //trx.Narration = m.Decrypt(trx.Narration, "1239879000");
            //now processing this part on job
            //updateImalTransbySID(bits[0], bits[1]);
            //trx.AccountName = bits[0];
            try
            {
                //trx.ResponseCode = bits[1];
                trx.ResponseCode = "00";
                new ErrorLog("IMAL SessionID to be updated " + trx.SessionID);
                Connect c11 = new Connect("spd_WSupdatetrans");
                c11.addparam("@sessionid", trx.SessionID);
                c11.addparam("@bra_code", bra_code);
                c11.addparam("@cus_num", cus_num);
                c11.addparam("@cur_code", cur_code);
                c11.addparam("@led_code", led_code);
                c11.addparam("@sub_acct_code", sub_acct_code);
                c11.addparam("@amt", decimal.Parse(trx.Amount));
                c11.addparam("@payRef", trx.PaymentReference);
                c11.addparam("@manRef", "");
                c11.addparam("@remark", trx.Narration);
                c11.addparam("@originSender", trx.OriginatorAccountName);
                c11.addparam("@Responsecode", trx.ResponseCode);
                c11.addparam("@accname", trx.BeneficiaryAccountName);
                c11.addparam("@feecharge", 0);
                c11.addparam("@ResponseMsg", gg.responseCodes(trx.ResponseCode));

                c11.addparam("@NameEnquiryRef", trx.NameEnquiryRef);
                c11.addparam("@BeneficiaryBankVerificationNumber", trx.BeneficiaryBankVerificationNumber);
                c11.addparam("@OriginatorAccountNumber", trx.OriginatorAccountNumber);
                c11.addparam("@OriginatorBankVerificationNumber", trx.OriginatorBankVerificationNumber);

                c11.addparam("@OriginatorKYCLevel", trx.OriginatorKYCLevel);
                c11.addparam("@TransactionLocation", trx.TransactionLocation);
                c11.query();
            }
            catch (Exception ex)
            {
                new ErrorLog("Error Occured while updating IMAL final respons " + ex);
            }
            return trx.createResponse();
        }
        else if (sbpVal == 3)//bankone
        {//ooo
            //bankone
            string rsp = "";
            BankOneService.SterlingFundsTransferService bnew = new BankOneService.SterlingFundsTransferService();
            ////check for duplicate
            Connect cx = new Connect("spd_CheckDuplicateSessionid2");
            cx.addparam("@sessionid", trx.SessionID);
            int donx = cx.query();
            if (donx == 0)
            {
                trx.ResponseCode = "26";
                new ErrorLog("DUPLICATE TRNX");
                new ErrorLog(trx.xml);
                return trx.createResponse();
            }
            //get the name from the bankone service 
            string xml = "";
            string InstCode = ConfigurationSettings.AppSettings["AppZoneIntCode"].ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append("<AccountNameVerificationRequest>");
            sb.Append("<InstitutionCode>" + InstCode + "</InstitutionCode>");
            sb.Append("<AccountNumber>" + trx.BeneficiaryAccountNumber + "</AccountNumber>");
            sb.Append("</AccountNameVerificationRequest>");

            AccountNameVerificationRequest rqt1 = new AccountNameVerificationRequest(InstCode, trx.BeneficiaryAccountNumber);
            xml = Util.Serialize(rqt1);
            rsp = init().BankOneGetAccountName(sb.ToString());
            AccountNameVerificationResponse resp = Util.Deserialize(rsp);
            string cusNam = resp.AccountName;
            new ErrorLog("Response gotten for bankone  name enquiry for account " + trx.SessionID + " " + cusNam + " is " + rsp);
            if (cusNam.ToUpper() == trx.BeneficiaryAccountName.ToUpper())
            {
                DepositRequest rqt = new DepositRequest(InstCode, trx.OriginatorAccountNumber, trx.OriginatorAccountName, trx.BeneficiaryAccountNumber, trx.BeneficiaryAccountName, trx.DestinationInstitutionCode, trx.PaymentReference, trx.Amount, trx.Narration, trx.SessionID);
                xml = Util.Serialize(rqt);
                BankoneEncryt enc = new BankoneEncryt();
                xml = enc.EncryptTripleDES(xml);
                DepositResponse resp1 = new DepositResponse();
                try
                {
                    rsp = init().BankOneInterBankDeposit(xml);
                    resp1 = Util.DeserializeDeposit(rsp);
                    string dddd = resp1.ResponseCode;
                    trx.ResponseCode = resp1.ResponseCode;
                }
                catch (Exception ex)
                {
                    new ErrorLog(ex);
                    trx.ResponseCode = "96";
                }
                //logging third party details
                Connect2 TL = new Connect2("spd_InsertThirdPartyTrnxs");
                TL.addparam("@originatorAccountNumber", trx.OriginatorAccountNumber);
                TL.addparam("@originatorAccountName", trx.OriginatorAccountName);
                TL.addparam("@beneficiaryAccountNumber", trx.BeneficiaryAccountNumber);
                TL.addparam("@beneficiaryAccountName", trx.BeneficiaryAccountName);
                TL.addparam("@paymentReference", trx.PaymentReference);
                TL.addparam("@amount", trx.Amount);
                TL.addparam("@narration", trx.Narration);
                TL.addparam("@response", trx.ResponseCode);
                TL.addparam("@StatusMessage", resp1.StatusMessage);
                TL.addparam("@RequestType", "Inward");
                TL.addparam("@SessionID", trx.SessionID);
                TL.query();
                return trx.createResponse();
            }
            else
            {
                trx.BeneficiaryAccountName = "";
                trx.ResponseCode = "08";
                return trx.createResponse();
            }
        }
        return "";
        //}
    }
    public void updateImalTransbySID(string sid, string rsp)
    {
        Gadget g = new Gadget();
        string sql = "";
        sql = "update tbl_wstrans set Responsecode=@rsp, ResponseMsg=@rmg, Approvevalue=1 where sessionid=@sid";
        Connect c = new Connect(sql, true);
        c.addparam("@sid", sid);
        c.addparam("@rsp", rsp);
        c.addparam("@rmg", g.responseCodes(rsp));
        c.query();
    }
    [WebMethod]
    public string fundstransferAdviceRequest_dc(string request)
    {
        sbp.banks b = new sbp.banks();
        int cn = 0;
        DataSet ds = new DataSet();
        TR_FundsTransferAdviceRqtDC trx = new TR_FundsTransferAdviceRqtDC();
        trx.xml = request;
        //new ErrorLog("New Advice " + request);
        Mylogger.Info("New Advice " + request);
        SBPSwitch sbp = new SBPSwitch();
        int sbpVal = 0;

        //if (sbpVal == 1)
        //{
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }
        sbpVal = sbp.getInternalBankID(trx.BeneficiaryAccountNumber);
        if (sbpVal == 1)
        {
            //mark this transaction for Funds transfer Advice Response.
            Connect c = new Connect("spd_FundsTrnsAdviceDC");
            c.addparam("@sessionid", trx.SessionID);
            cn = c.query();
            if (cn == 1)
            {
                trx.ResponseCode = "00";
                return trx.createResponse();

            }
            else
            {
                trx.ResponseCode = "21";
                return trx.createResponse();
            }

        }
        else if (sbpVal == 2)
        {
            string rsp = "";
            trx.SessionID = m.Encrypt(trx.SessionID, "1239879000");
            trx.BeneficiaryAccountNumber = m.Encrypt(trx.BeneficiaryAccountNumber, "1239879000");
            trx.Amount = m.Encrypt(trx.Amount, "1239879000");


            imal.NIBankingClientService im = new imal.NIBankingClientService();
            rsp = im.reverse(trx.SessionID, trx.BeneficiaryAccountNumber, trx.Amount, "xx");
            rsp = m.Decrypt(rsp, "1239879000");
            string[] bits = rsp.Split(':');
            trx.ResponseCode = bits[1];

            trx.SessionID = m.Decrypt(trx.SessionID, "1239879000");
            trx.BeneficiaryAccountNumber = m.Decrypt(trx.BeneficiaryAccountNumber, "1239879000");
            trx.Amount = m.Decrypt(trx.Amount, "1239879000");

            return trx.createResponse();
        }
        return trx.createResponse();
    }
    [WebMethod]
    public string fundstransferAdviceRequest_dd(string request)
    {
        sbp.banks b = new sbp.banks();
        int cn = 0;
        DataSet ds = new DataSet();
        TR_FundsTransferAdviceRqtDD trx = new TR_FundsTransferAdviceRqtDD();
        trx.xml = request;

        SBPSwitch sbp = new SBPSwitch();
        int sbpVal = 0;

        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }
        sbpVal = sbp.getInternalBankID(trx.BeneficiaryAccountNumber);
        if (sbpVal == 1)
        {
            //check if sessionid exist
            bool chkSessioid = SessionIdExist(trx.SessionID);
            if (!chkSessioid)
            {
                trx.ResponseCode = "15";
                return trx.createResponse();
            }

            string refidval = getRefid(trx.SessionID);
            //mark this transaction for Funds transfer Advice Response.
            Connect c = new Connect("spd_FundsTrnsAdviceDC");
            c.addparam("@sessionid", trx.SessionID);
            cn = c.query();
            if (cn == 1)
            {
                DataSet ds1 = new DataSet();
                //new ErrorLog("Sessionid for Advice " + trx.SessionID + " has been marked for Advice");
                Mylogger.Info("Sessionid for Advice " + trx.SessionID + " has been marked for Advice");
                //confirm on BANKS if customer got the credit
                trx.ResponseCode = "00";
                return trx.createResponse();
            }
            else
            {
                trx.ResponseCode = "21";
                return trx.createResponse();
            }


        }
        else if (sbpVal == 2)
        {
            string rsp = "";
            trx.SessionID = m.Encrypt(trx.SessionID, "1239879000");
            trx.BeneficiaryAccountNumber = m.Encrypt(trx.BeneficiaryAccountNumber, "1239879000");
            trx.Amount = m.Encrypt(trx.Amount, "1239879000");


            imal.NIBankingClientService im = new imal.NIBankingClientService();
            rsp = im.reverse(trx.SessionID, trx.BeneficiaryAccountNumber, trx.Amount, "xx");
            rsp = m.Decrypt(rsp, "1239879000");
            string[] bits = rsp.Split(':');
            trx.ResponseCode = bits[1];

            trx.SessionID = m.Decrypt(trx.SessionID, "1239879000");
            trx.BeneficiaryAccountNumber = m.Decrypt(trx.BeneficiaryAccountNumber, "1239879000");
            trx.Amount = m.Decrypt(trx.Amount, "1239879000");

            return trx.createResponse();
        }
        return trx.createResponse();
    }
    public string getRefid(string sessionid)
    {
        string Refid = "";
        Connect c = new Connect("spd_getRefidBySessionid");
        c.addparam("@sessionid", sessionid);
        DataSet ds = c.query("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            Refid = dr["Refid"].ToString();
        }
        return Refid;
    }
    public bool SessionIdExist(string sessionid)
    {
        bool found = false;
        try
        {
            Connect c = new Connect("spd_SessionidExist");
            c.addparam("@sessionid", sessionid);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                found = true;
            }
            else
            {
                found = false;
            }
        }
        catch (Exception ex)
        {
            //new ErrorLog(ex);
            Mylogger.Error(ex);
            found = false;
        }
        return found;
    }

    [WebMethod]
    public string txnstatusquerysingleitem(string request)
    {

        TransactionService tsv = new TransactionService();
        string rsp = "";
        trx.xml = request;
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }
        //search by transid
        DataSet ds = tsv.getTrnxBySesionId(trx.SessionID);
        if (ds.Tables[0].Rows.Count < 1)
        {
            trx.ResponseCode = "12";//invalid transaction
            return trx.createResponse();
        }
        if (ds.Tables[0].Rows.Count > 0) //single transaction
        {
            DataRow dr = ds.Tables[0].Rows[0];
            rsp = dr["Responsecode"].ToString();
            TR_SingleStatusQuery ssq = new TR_SingleStatusQuery();
            ssq.SessionID = trx.SessionID;
            ssq.SourceInstitutionCode = trx.SourceInstitutionCode;
            ssq.ResponseCode = rsp;
            return ssq.createResponse();
        }
        return "";
    }
    protected string runSingleJob()
    {
        DataSet ds = new DataSet();
        TR_SingleStatusQuery ssq = new TR_SingleStatusQuery();
        TransactionService tsv = new TransactionService();

        ssq.SessionID = trx.SessionID;
        ssq.SourceInstitutionCode = trx.SourceInstitutionCode;
        ssq.ChannelCode = trx.ChannelCode;
        ssq.ResponseCode = "00";
        return ssq.createResponse();
    }
    public void Upd_blk_tbl(string sid, string advice)
    {
        string sql = "update tbl_block_unblock set T24Advice=@adv where SessionID=@sid";
        Connect c = new Connect(sql, true);
        c.addparam("@adv", advice);
        c.addparam("@sid", sid);
        c.query();
    }
    [WebMethod]
    public string AmountBlockRequest(string request)
    {
        EACBS.banks ws = new EACBS.banks();
        //new ErrorLog("New Block Rqt " + request);
        Mylogger.Info("New Block Rqt " + request);
        Gadget g = new Gadget();
        TR_AmountBlockRequest trx = new TR_AmountBlockRequest();
        trx.xml = request;
        //new ErrorLog("Nibss:" + request);
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }
        bool found = DoesRefercodeexist(trx.ReferenceCode);
        if (found)
        {
            trx.ResponseCode = "26";
            return trx.createResponse();
        }
        string bra_code = ""; string cus_num = ""; string cur_code = "";
        string led_code = ""; string sub_acct_code = "";
        decimal custBal = 0;
        GetcustomerName gc = new GetcustomerName();
        string cusName = gc.getTheCustomerName(trx.TargetAccountNumber);
        bra_code = gc.Frm_bra_code;
        cus_num = gc.Frm_cus_num;
        cur_code = gc.Frm_cur_code;
        led_code = gc.Frm_led_code;
        sub_acct_code = gc.Frm_sub_acct_code;
        sta_code = gc.sta_code;
        bvn = gc.bvn;
        ////////////////////////////////////////////

        if (bvn == trx.TargetBankVerificationNumber)
        {
            //if the bvn matched then proceed
            //call the service to block the fund
            //insert into table
            string sql = "insert into tbl_block_unblock(SessionID,DestinationInstitutionCode,ChannelCode,ReferenceCode, " +
                " TargetAccountName,TargetBankVerificationNumber,TargetAccountNumber,ReasonCode,Amount,Narration,bra_code, " +
                " cus_num,cur_code,led_code,sub_acct_code,Actionflag,Bal_out_standing) " +
                " values(@sd,@dc,@cc,@ref,@ta,@tv,@tan,@rc,@amt,@na,@bc,@cn,@cr,@lc,@sc,@af,@balamt)";
            Connect c = new Connect(sql, true);
            c.addparam("@sd", trx.SessionID);
            c.addparam("@dc", trx.DestinationInstitutionCode);
            c.addparam("@cc", trx.ChannelCode);
            c.addparam("@ref", trx.ReferenceCode);
            c.addparam("@ta", trx.TargetAccountName);
            c.addparam("@tv", trx.TargetBankVerificationNumber);
            c.addparam("@tan", trx.TargetAccountNumber);
            c.addparam("@rc", trx.ReasonCode);
            c.addparam("@amt", decimal.Parse(trx.Amount));
            c.addparam("@na", trx.Narration);
            c.addparam("@bc", bra_code);
            c.addparam("@cn", cus_num);
            c.addparam("@cr", cur_code);
            c.addparam("@lc", led_code);
            c.addparam("@sc", sub_acct_code);
            c.addparam("@af", 2);
            c.addparam("@balamt", decimal.Parse(trx.Amount));
            int cn = c.query();
            if (cn > 0)
            {
                //if (custBal < decimal.Parse(trx.Amount))
                //{
                //    trx.ResponseCode = "51";
                //    return trx.createResponse();
                //}
                string T24advice = "";
                TransactionService ts = new TransactionService();

                Transaction t = new Transaction();
                t.inCust.bra_code = bra_code;
                t.inCust.cus_num = cus_num;
                t.inCust.cur_code = cur_code;
                t.inCust.cur_code = "NGN";
                t.inCust.led_code = led_code;
                t.inCust.sub_acct_code = trx.TargetAccountNumber;// sub_acct_code;
                t.amount = decimal.Parse(trx.Amount);
                t.tellerID = "9990"; //remember to get a teller id
                t.Remark = trx.Narration + " Ref: " + trx.ReferenceCode;
                t.amount = decimal.Parse(trx.Amount);
                //t.feecharge = decimal.Parse(trx.TransactionFee);
                t.narration = trx.Narration;
                //t.senderAcctname = trx.BeneficiaryAccountName;
                T24advice = ws.AmountLockwithoutDate(trx.TargetAccountNumber, t.amount);
                //new ErrorLog("Response Returned for Blk acct with sessionid " + trx.SessionID + " is " + T24advice);
                Mylogger.Info("Response Returned for Blk acct with sessionid " + trx.SessionID + " is " + T24advice);
                //send to T24 to block
                string[] bits = T24advice.Split('|');
                if (bits[0] == "1")
                {
                    trx.ResponseCode = "00";
                    Upd_blk_tbl(trx.SessionID, bits[1]);
                    //acs.authorizeTrnxToSterling_BlkAMt(t);
                }
                else
                {
                    trx.ResponseCode = "21";
                }
            }

        }
        else
        {
            //bvn dont match
            trx.ResponseCode = "70";
        }
        return trx.createResponse();
    }

    [WebMethod]
    public string AmountUnblockRequest(string request)
    {
        EACBS.banks ws = new EACBS.banks();
        string T24Advice = ""; string rsp = "";
        //new ErrorLog("New unBlock Rqt " + request);
        Mylogger.Info("New unBlock Rqt " + request);
        TR_AmountUnblockRequest trx = new TR_AmountUnblockRequest();
        trx.xml = request;
        //new ErrorLog("Nibss:" + request);
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }

        string bra_code = ""; string cus_num = ""; string cur_code = "";
        string led_code = ""; string sub_acct_code = "";
        decimal custBal = 0; decimal Bal_out_standing = 0; decimal Amount = 0; decimal Bal_amt = 0;
        GetcustomerName gc = new GetcustomerName();
        string cusName = gc.getTheCustomerName(trx.TargetAccountNumber);
        bra_code = gc.Frm_bra_code;
        cus_num = gc.Frm_cus_num;
        cur_code = gc.Frm_cur_code;
        led_code = gc.Frm_led_code;
        sub_acct_code = gc.Frm_sub_acct_code;
        sta_code = gc.sta_code;
        bvn = gc.bvn;
        ////////////////////////////////////////////
        if (bvn == trx.TargetBankVerificationNumber)
        {
            if (decimal.Parse(trx.Amount) < 0)
            {
                trx.ResponseCode = "13";
                return trx.createResponse();
            }
            //first check if the reference number exist
            string sql = "select ReferenceCode,Amount,Bal_amt,Bal_out_standing,T24Advice from tbl_block_unblock where ReferenceCode=@rcode and Actionflag=2 and statusflag=0";
            Connect c = new Connect(sql, true);
            c.addparam("@rcode", trx.ReferenceCode);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                Amount = decimal.Parse(dr["Amount"].ToString());
                //Bal_amt = decimal.Parse(dr["Bal_amt"].ToString());
                Bal_out_standing = decimal.Parse(dr["Bal_out_standing"].ToString());
                T24Advice = dr["T24Advice"].ToString();
                if (decimal.Parse(trx.Amount) > Amount)
                {
                    trx.ResponseCode = "13";
                    return trx.createResponse();
                }
                if (decimal.Parse(trx.Amount) > Bal_out_standing)
                {
                    trx.ResponseCode = "13";
                    return trx.createResponse();
                }

                if (decimal.Parse(trx.Amount) <= Bal_out_standing)
                {

                    TransactionService ts = new TransactionService();
                    Transaction t = new Transaction();
                    t.inCust.bra_code = bra_code;
                    t.inCust.cus_num = cus_num;
                    t.inCust.cur_code = "NGN";// cur_code;
                    t.inCust.led_code = led_code;
                    t.inCust.sub_acct_code = trx.TargetAccountNumber;// sub_acct_code;
                    t.amount = decimal.Parse(trx.Amount);
                    t.tellerID = "9990"; //remember to get a teller id
                    t.Remark = trx.Narration + " Ref: " + trx.ReferenceCode;
                    t.amount = decimal.Parse(trx.Amount);
                    //t.feecharge = decimal.Parse(trx.TransactionFee);
                    t.narration = trx.Narration;
                    //t.senderAcctname = trx.BeneficiaryAccountName;
                    rsp = ws.PatialUnLockAmount(T24Advice, t.amount.ToString());
                    string[] bits = rsp.Split('|');
                    //acs.authorizeTrnxToSterling_BlkAMtRev(t);
                    if (bits[0] == "1")
                    {
                        //get the new outstanding
                        Bal_out_standing = Bal_out_standing - decimal.Parse(trx.Amount);
                        if (Bal_out_standing == 0)
                        {
                            updateBlkUnblktable(trx.ReferenceCode, 1);
                            updateDatecompleted(trx.ReferenceCode);
                        }
                        else
                        {
                            updateBlkUnblktable(trx.ReferenceCode, 0);
                            UpdateOutStandingBal(trx.ReferenceCode, Bal_out_standing);
                        }
                        trx.ResponseCode = "00";
                        return trx.createResponse();
                    }
                    else
                    {
                        trx.ResponseCode = "21";
                    }
                }
                else
                {
                    trx.ResponseCode = "13";
                    return trx.createResponse();
                }
            }
            else
            {
                trx.ResponseCode = "25";
            }
        }
        else
        {
            //bvn dont match
        }
        return trx.createResponse();
    }

    [WebMethod]
    public string AccountBlockRequest(string request)
    {
        TR_AccountBlockRequest trx = new TR_AccountBlockRequest();
        trx.xml = request;
        //new ErrorLog("Nibss:" + request);
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }
        bool found = DoesRefercodeexist(trx.ReferenceCode);
        if (found)
        {
            trx.ResponseCode = "26";
            return trx.createResponse();
        }
        string bra_code = ""; string cus_num = ""; string cur_code = "";
        string led_code = ""; string sub_acct_code = "";
        decimal custBal = 0; int prevRestInd = -1;
        GetcustomerName gc = new GetcustomerName();
        string cusName = gc.getTheCustomerName(trx.TargetAccountNumber);
        bra_code = gc.Frm_bra_code;
        cus_num = gc.Frm_cus_num;
        cur_code = gc.Frm_cur_code;
        led_code = gc.Frm_led_code;
        sub_acct_code = gc.Frm_sub_acct_code;
        sta_code = gc.sta_code;
        bvn = gc.bvn;
        prevRestInd = gc.rest_ind;
        ////////////////////////////////////////////
        if (bvn == trx.TargetBankVerificationNumber)
        {
            //if the bvn matched then proceed
            //insert into table
            string sql = "insert into tbl_block_unblock(SessionID,DestinationInstitutionCode,ChannelCode,ReferenceCode, " +
                " TargetAccountName,TargetBankVerificationNumber,TargetAccountNumber,ReasonCode,Amount,Narration,bra_code, " +
                " cus_num,cur_code,led_code,sub_acct_code,Actionflag,prevRestInd) " +
                " values(@sd,@dc,@cc,@ref,@ta,@tv,@tan,@rc,@amt,@na,@bc,@cn,@cr,@lc,@sc,@af,@pi)";
            Connect c = new Connect(sql, true);
            c.addparam("@sd", trx.SessionID);
            c.addparam("@dc", trx.DestinationInstitutionCode);
            c.addparam("@cc", trx.ChannelCode);
            c.addparam("@ref", trx.ReferenceCode);
            c.addparam("@ta", trx.TargetAccountName);
            c.addparam("@tv", trx.TargetBankVerificationNumber);
            c.addparam("@tan", trx.TargetAccountNumber);
            c.addparam("@rc", trx.ReasonCode);
            c.addparam("@amt", 0);
            c.addparam("@na", trx.Narration);
            c.addparam("@bc", bra_code);
            c.addparam("@cn", cus_num);
            c.addparam("@cr", cur_code);
            c.addparam("@lc", led_code);
            c.addparam("@sc", sub_acct_code);
            c.addparam("@af", 3);
            c.addparam("@pi", prevRestInd);
            int cn = c.query();
            if (cn > 0)
            {
                //call the service to block the fund
                bool done = b.sbp_block_acct(bra_code, cus_num, cur_code, led_code, sub_acct_code, 30);
                if (done)
                {
                    //update the statusflag as completed from 0 to 1
                    string sql1 = "update tbl_block_unblock set statusflag = 0 where SessionID =@sid";
                    Connect c1 = new Connect(sql1, true);
                    c1.addparam("@sid", trx.SessionID);
                    c1.query();
                    //insert into table
                    trx.ResponseCode = "00";
                    return trx.createResponse();
                }
                else
                {
                    trx.ResponseCode = "70";
                }
            }
            else
            {
                trx.ResponseCode = "70";
            }
        }
        else
        {
            //bvn dont match
        }
        return trx.createResponse();
    }

    [WebMethod]
    public string AccountUnblockRequest(string request)
    {
        int prevRestInd = -1;
        TR_AccountUnblockRequest trx = new TR_AccountUnblockRequest();
        trx.xml = request;
        //new ErrorLog("Nibss:" + request);
        if (!trx.readRequest())
        {
            trx.ResponseCode = "30";
            return trx.createResponse();
        }

        string bra_code = ""; string cus_num = ""; string cur_code = "";
        string led_code = ""; string sub_acct_code = "";
        decimal custBal = 0;
        GetcustomerName gc = new GetcustomerName();
        string cusName = gc.getTheCustomerName(trx.TargetAccountNumber);
        bra_code = gc.Frm_bra_code;
        cus_num = gc.Frm_cus_num;
        cur_code = gc.Frm_cur_code;
        led_code = gc.Frm_led_code;
        sub_acct_code = gc.Frm_sub_acct_code;
        sta_code = gc.sta_code;
        bvn = gc.bvn;
        ////////////////////////////////////////////
        if (bvn == trx.TargetBankVerificationNumber)
        {
            //if the bvn matched then proceed
            //go to tbl_block_unblock and get the save previous 
            string sql = "select prevRestInd from tbl_block_unblock where bra_code= @bc and cus_num =@cn and cur_code = @cr and led_code = @lc and sub_acct_code =@sc and statusflag=0 and Actionflag=3";
            Connect c = new Connect(sql, true);
            c.addparam("@bc", bra_code);
            c.addparam("@cn", cus_num);
            c.addparam("@cr", cur_code);
            c.addparam("@lc", led_code);
            c.addparam("@sc", sub_acct_code);
            DataSet ds = c.query("resp");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                prevRestInd = int.Parse(dr["prevRestInd"].ToString());
                bool done = b.sbp_unblock_acct(bra_code, cus_num, cur_code, led_code, sub_acct_code, 30, prevRestInd);
                if (done)
                {
                    updateBlkUnblktable(trx.ReferenceCode, 1);
                    trx.ResponseCode = "00";
                    return trx.createResponse();
                }
            }
        }
        else
        {
            //bvn dont match
        }
        return trx.createResponse();
    }

    public void updateBlkUnblktable(string refcod, int flag)
    {
        string sql = "";
        sql = "update tbl_block_unblock set statusflag=@flag where ReferenceCode=@rcode";
        Connect c = new Connect(sql, true);
        c.addparam("@flag", flag);
        c.addparam("@rcode", refcod);
        c.query();
    }
    public void updateDatecompleted(string refcod)
    {
        string sql = "";
        sql = "update tbl_block_unblock set datecompleted=getdate(),Bal_out_standing=0 where ReferenceCode=@rcode";
        Connect c = new Connect(sql, true);
        c.addparam("@rcode", refcod);
        c.query();
    }
    public void UpdateOutStandingBal(string refcod, decimal amt)
    {
        string sql = "";
        sql = "update tbl_block_unblock set Bal_out_standing=@amt where ReferenceCode=@rcode";
        Connect c = new Connect(sql, true);
        c.addparam("@amt", amt);
        c.addparam("@rcode", refcod);
        c.query();
    }
    private bool DoesRefercodeexist(string refcod)
    {
        bool found = false;
        string sql = "";
        sql = "select ReferenceCode from tbl_block_unblock where ReferenceCode=@rcode";
        Connect c = new Connect(sql, true);
        c.addparam("@rcode", refcod);
        DataSet ds = c.query("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            found = true;
        }
        else
        {
            found = false;
        }
        return found;
    }
}

