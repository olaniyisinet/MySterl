using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Text;
public class TransactionService
{
    public decimal maxPerTrans = 0;
    public decimal maxPerday = 0;
    public decimal sum = 0;

    //get the current TSS11
    public DataSet getCurrentTss11()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getCurrentTss11");
            ds = c.query("recs");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }


    public void mailTrnx(Transaction t, string tuser, string msg, string eml1, string eml2, string eml3)
    {
        //success
        //insert into audit trail
        try
        {
            string bby = "<p><h1>Transaction with the following details:</p></h1>" +
                //"<p> Beneficiary Bank:</p>" + t.BENEBANK+
                //"<p> Beneficiary Name:</p>" + t.benename +
               "<p> Transaction Date:</p>" + DateTime.Now.ToString() +
               "<p> Amount:</p>" + t.amount.ToString() + "bracode " + t.inCust.bra_code + " cusnum " + t.inCust.cus_num +
               "<p> Amount In Words:</p>" + t.amountWords +
               "<p> has been approved by:</p>" + tuser +
               "<p> The transaction was not successful. </p>" +
               "<p> Exception :<i>" + msg + "</i></p>" +
               "<p> Click here to log on http://10.0.0.151:102 </p>";

            swcMailer mm = new swcMailer(eml1);
            mm.mailBody = bby;
            mm.mailSubject = "NIBBS Mail : Your Transaction has been approved::The transaction was not successful but customer's account was debited for charges";
            mm.addTo(eml2);
            mm.addCC(eml3);
            mm.addBCC("Chigozie.Anyasor@Sterlingbankng.com");
            mm.addBCC("Adedayo.Okesola@Sterlingbankng.com");
            mm.sendTheMail();
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
    }
    public Transaction getTrnxById(int refid)
    {
        Transaction t = new Transaction();
        Connect c = new Connect("spd_getnibbstrans");
        c.addparam("refid", refid);
        DataSet ds = c.query("rec");
        DataRow dr = ds.Tables[0].Rows[0];
        if (ds.Tables[0].Rows.Count > 0)
        {
            t.Refid = Convert.ToInt32(dr["Refid"]);
            t.sessionid = Convert.ToString(dr["sessionid"]);
            t.transactionCode = Convert.ToString(dr["transactioncode"]);
            t.transactionNature = Convert.ToInt16(dr["Transnature"]);
            t.channelCode = Convert.ToInt16(dr["channelCode"]);
            t.BatchNumber = Convert.ToString(dr["BatchNumber"]);
            t.paymentRef = Convert.ToString(dr["paymentRef"]);
            t.mandateRefNum = Convert.ToString(dr["mandateRefNum"]);
            t.outCust.bankcode = Convert.ToString(dr["Benebankcode"]);
            t.outCust.bankname = Convert.ToString(dr["Benebank"]);
            t.outCust.accountnumber = Convert.ToString(dr["Beneaccount"]);
            t.outCust.cusname = Convert.ToString(dr["Benename"]);
            t.amount = Convert.ToDecimal(dr["amt"]);
            t.amountWords = Convert.ToString(dr["amtinword"]);
            if (dr["feecharge"] == DBNull.Value)
            {
                t.feecharge = 0;
            }
            else
            {
                t.feecharge = Convert.ToDecimal(dr["feecharge"]);
            }
            t.inCust.bra_code = Convert.ToString(dr["bra_code"]);
            t.inCust.cus_num = Convert.ToString(dr["cus_num"]);
            t.inCust.cur_code = Convert.ToString(dr["cur_code"]);
            t.inCust.led_code = Convert.ToString(dr["led_code"]);
            t.inCust.sub_acct_code = Convert.ToString(dr["sub_acct_code"]);
            t.inCust.cus_sho_name = Convert.ToString(dr["accname"]);
            t.inCust.mobile = Convert.ToString(dr["mobile"]);

            t.formnum = Convert.ToString(dr["formnum"]);
            t.Remark = Convert.ToString(dr["Remark"]);
            t.docfilename = Convert.ToString(dr["docfilename"]);
            t.mime = Convert.ToString(dr["mime"]);
            t.origin_branch = Convert.ToString(dr["Orig_bra_code"]);
            t.inputedby = Convert.ToString(dr["inputedby"]);
            try
            {
                t.inputdate = Convert.ToDateTime(dr["inputdate"]);
            }
            catch (Exception ex)
            {
                t.inputdate = new DateTime(1970, 1, 1);
            }
            t.approvedby = Convert.ToString(dr["approvedby"]);
            try
            {
                t.approveddate = Convert.ToDateTime(dr["approveddate"]);
            }
            catch (Exception ex)
            {
                t.approveddate = new DateTime(1970, 1, 1);
            }
            t.approvevalue = Convert.ToInt16(dr["Approvevalue"]);
            t.csoemail = Convert.ToString(dr["csoemail"]);
            t.bmemail = Convert.ToString(dr["bmemail"]);
            t.hopemail = Convert.ToString(dr["hopemail"]);
        }
        else
        {
            t.Refid = 0;
        }        
        return t;
    }

    public Transaction setTrnx(DataRow dr)
    {
        Transaction t = new Transaction();
        t.Refid = Convert.ToInt32(dr["Refid"]);
        t.sessionid = Convert.ToString(dr["sessionid"]);
        t.transactionCode = Convert.ToString(dr["transactioncode"]);
        t.transactionNature = Convert.ToInt16(dr["Transnature"]);
        t.channelCode = Convert.ToInt16(dr["channelCode"]);
        t.BatchNumber = Convert.ToString(dr["BatchNumber"]);
        t.paymentRef = Convert.ToString(dr["paymentRef"]);
        t.mandateRefNum = Convert.ToString(dr["mandateRefNum"]);
        t.outCust.bankcode = Convert.ToString(dr["Benebankcode"]);
        t.outCust.bankname = Convert.ToString(dr["Benebank"]);
        t.outCust.accountnumber = Convert.ToString(dr["Beneaccount"]);
        t.outCust.cusname = Convert.ToString(dr["Benename"]);
        t.amount = Convert.ToDecimal(dr["amt"]);
        t.amountWords = Convert.ToString(dr["amtinword"]);
        if (dr["feecharge"] == DBNull.Value)
        {
            t.feecharge = 0;
        }
        else
        {
            t.feecharge = Convert.ToDecimal(dr["feecharge"]);
        }
        t.inCust.bra_code = Convert.ToString(dr["bra_code"]);
        t.inCust.cus_num = Convert.ToString(dr["cus_num"]);
        t.inCust.cur_code = Convert.ToString(dr["cur_code"]);
        t.inCust.led_code = Convert.ToString(dr["led_code"]);
        t.inCust.sub_acct_code = Convert.ToString(dr["sub_acct_code"]);
        t.inCust.cus_sho_name = Convert.ToString(dr["accname"]);
        t.inCust.mobile = Convert.ToString(dr["mobile"]);

        t.formnum = Convert.ToString(dr["formnum"]);
        t.Remark = Convert.ToString(dr["Remark"]);
        t.docfilename = Convert.ToString(dr["docfilename"]);
        t.mime = Convert.ToString(dr["mime"]);
        t.origin_branch = Convert.ToString(dr["Orig_bra_code"]);
        t.inputedby = Convert.ToString(dr["inputedby"]);
        try
        {
            t.inputdate = Convert.ToDateTime(dr["inputdate"]);
        }
        catch (Exception ex)
        {
            t.inputdate = new DateTime(1970, 1, 1);
        }
        t.approvedby = Convert.ToString(dr["approvedby"]);
        try
        {
            t.approveddate = Convert.ToDateTime(dr["approveddate"]);
        }
        catch (Exception ex)
        {
            t.approveddate = new DateTime(1970, 1, 1);
        }
        t.approvevalue = Convert.ToInt16(dr["Approvevalue"]);
        t.csoemail = Convert.ToString(dr["csoemail"]);
        t.bmemail = Convert.ToString(dr["bmemail"]);
        t.hopemail = Convert.ToString(dr["hopemail"]);
        t.nameresponse = Convert.ToString(dr["nameresponse"]);
        t.billerName = Convert.ToString(dr["BillerName"]);
        t.billerId = Convert.ToString(dr["BillerId"]);
        t.mandateRefNum = Convert.ToString(dr["mandateRefNum"]);
        return t;
    }
    public int updateTrnxById(Transaction t)
    {
        try
        {
            Connect c = new Connect("spd_updatenibbstransaction");
            c.addparam("@Refid", t.Refid);
            c.addparam("@sessionid", t.sessionid);
            c.addparam("@transactioncode", t.transactionCode);
            c.addparam("@Transnature", t.transactionNature);
            c.addparam("@channelCode", t.channelCode);
            c.addparam("@BatchNumber", t.BatchNumber);
            c.addparam("@paymentRef", t.paymentRef);
            c.addparam("@mandateRefNum", t.mandateRefNum);
            c.addparam("@BillerName", t.billerName);
            c.addparam("@BillerId", t.billerId);
            c.addparam("@Benebankcode", t.outCust.bankcode);
            c.addparam("@Benebank", t.outCust.bankname);
            c.addparam("@Beneaccount", t.outCust.accountnumber);
            c.addparam("@Benename", t.outCust.cusname);
            c.addparam("@amt", t.amount);
            c.addparam("@amtinword", t.amountWords);
            c.addparam("@feecharge", t.feecharge);
            c.addparam("@bra_code", t.inCust.bra_code);
            c.addparam("@cus_num", t.inCust.cus_num);
            c.addparam("@cur_code", t.inCust.cur_code);
            c.addparam("@led_code", t.inCust.led_code);
            c.addparam("@sub_acct_code", t.inCust.sub_acct_code);
            c.addparam("@accname", t.inCust.cus_sho_name);
            c.addparam("@mobile", t.inCust.mobile);
            c.addparam("@formnum", t.formnum);
            c.addparam("@Remark", t.Remark);
            c.addparam("@docfilename", t.docfilename);
            c.addparam("@mime", t.mime);
            c.addparam("@Orig_bra_code", t.origin_branch);
            c.addparam("@inputedby", t.inputedby);
            c.addparam("@inputdate", t.inputdate);
            c.addparam("@approvedby", t.approvedby);
            c.addparam("@approveddate", t.approveddate);
            c.addparam("@Approvevalue", t.approvevalue);
            c.addparam("@csoemail", t.csoemail);
            c.addparam("@bmemail", t.bmemail);
            c.addparam("@hopemail", t.hopemail);
            c.query();
            return c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return 0;
    }
    public Transaction InsertTransaction()
    {
        Transaction t = new Transaction();
        try
        {
            Connect c = new Connect("spd_Insertnibbstrans");
            c.query();
            t.Refid = c.returnValue;
        }
        catch (Exception ex)
        {
            t.Refid = 0;
        }
        return t;
    }
    public void updateTrnxStatus(int status, string nameresponse, Int32 Refid)
    {
        Connect c = new Connect("spd_UpdateTrnxStatus");
        c.addparam("Refid", Refid);
        c.addparam("status", status);
        //c.addparam("sessionid", sessionid);
        c.addparam("nameresponse", nameresponse);
        c.query();
    }

    public void updateTrnxStatus(string sessionid, int status, string nameresponse, Int32 Refid)
    {
        Connect c = new Connect("spd_UpdateTrnxStatus");
        c.addparam("Refid", Refid);
        c.addparam("status", status);
        c.addparam("sessionid", sessionid);
        c.addparam("nameresponse", nameresponse);
        c.query();
    }
    public void UpdateTrnxReversal(int status, Int32 Refid)
    {
        Connect c = new Connect("spd_UpdateTrnxReversal");
        c.addparam("Refid", Refid);
        c.addparam("status", status);
        c.query();
    }
    public void updateTrnxStatusCol(string sessionid, int status, Int32 Refid)
    {
        Connect c = new Connect("spd_UpdateTrnxStatus2");
        c.addparam("Refid", Refid);
        c.addparam("status", status);
        c.addparam("sessionid", sessionid);
        c.query();
    }

    public void updateTrnxApproveStatus(string sessionid, int approvevalue)
    {
        Connect c = new Connect("spd_UpdateTrnxStatus3");
        c.addparam("sessionid", sessionid);
        c.addparam("approvevalue", approvevalue);
        c.query();
    }


    public int checkBatch(string batchNumber)
    {
        Connect c = new Connect("spd_getCountbyBatchNumber");
        c.addparam("BatchNumber", batchNumber);
        DataSet ds = c.query("rec");
        int cnt = Convert.ToInt16(ds.Tables[0].Rows[0]["cn"]);
        return cnt;
    }

    public DataSet searchTrnxByDate(DateTime sdate, DateTime edate, string originbracode)
    {
        //sdate = sdate.AddDays(-1);
        edate = edate.AddDays(1);
        Connect c = new Connect("spd_GetBatchTrnxs");
        c.addparam("Orig_bra_code", originbracode);
        c.addparam("sdate", sdate);
        c.addparam("edate", edate);
        return c.query("recs");
    }

    public DataSet getTrnxSubRecords(string trnxCode)
    {
        Connect c = new Connect("spd_GetBatchTrnxRecords");
        c.addparam("transactioncode", trnxCode);
        return c.query("recs");
    }
    public DataSet getTrnxBySesionId(string sessionID)
    {
        Connect c = new Connect("spd_GetRecbySessionId");
        c.addparam("sessionid", sessionID);
        return c.query("recs");
    }
    
    public int updateSingleTnxById(Transaction t)
    {
        try
        {
            Connect c = new Connect("spd_UpdateSingleAprrovedTxn");
            c.addparam("Refid", t.Refid);
            c.addparam("transactioncode", t.transactionCode);
            c.addparam("feecharge", t.feecharge);
            c.addparam("paymentRef", t.paymentRef);
            c.addparam("approvedby", t.approvedby);
            c.addparam("approveddate", t.approveddate);
            c.query();
            return c.returnValue;
        }
        catch (Exception ex)
        {

            ErrorLog err = new ErrorLog(ex);
        }
        return 0;
    }
    public int spd_UpdateBulkAprroved09Txn(Transaction t)
    {
        try
        {
            Connect c = new Connect("spd_UpdateBulkNameEnquiry");
            c.addparam("BatchNumber", t.BatchNumber);
            c.query();
            return c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return 0;
    }
    public int UpdateBulkFundTransfer(Transaction t)
    {
        try
        {
            Connect c = new Connect("spd_UpdateBulkFundsTransfer");
            c.addparam("BatchNumber", t.BatchNumber);
            c.query();
            return c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return 0;
    }
    public int CheckTraStatus(Transaction t)
    {
        try
        {
            Connect c = new Connect("spd_CheckTransStatus");
            c.addparam("BatchNumber", t.BatchNumber);
            c.query();
            return c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return 0;
    }
    public void mailNewTrnx(Transaction t)
    {
        //success
        //insert into audit trail
        try
        {
            string bby = "<p><h1>Transaction with the following details:</p></h1>" +
                //"<p> Beneficiary Bank:</p>" + t.BENEBANK+
                //"<p> Beneficiary Name:</p>" + t.benename +
               "<p> Transaction Date:</p>" + DateTime.Now.ToString() +
               "<p> Amount:</p>" + t.amount.ToString() + "bracode " + t.inCust.bra_code + " cusnum " + t.inCust.cus_num +
               "<p> Amount In Words:</p>" + t.amountWords +
              // "<p> has been approved by:</p>" + p.fullname +
               "<p> The transaction was not successful. </p>" +
               "<p> Click here to log on http://10.0.0.151:102 </p>";

            Mailer mm = new Mailer();
            mm.mailBody = bby;
            mm.mailSubject = "NIBSS Mail: Transaction waiting for HOP approval ";

            mm.addTo(t.csoemail);
            mm.addCC(t.bmemail);
            mm.addBCC("Chigozie.Anyasor@Sterlingbankng.com");
            mm.addBCC("Adedayo.Okesola@Sterlingbankng.com");
            mm.sendTheMail();
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
    }

    public DataSet getNIPFee(decimal amt)
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getNIPFeeCharge");
            c.addparam("@amt", amt);
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }

    public int RejectTrans(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_RejectTrans");
            c.addparam("@transactioncode", t.transactionCode.Trim());
            c.addparam("@approvedby", t.approvedby);

            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }

    //get explanation code value
    public DataSet getExpcode()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getExplcode");
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }

    //get the current TSS
    public DataSet getCurrentTss()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getCurrentTss");
            ds = c.query("recs");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }
    //get the new TSS account
    public DataSet getCurrentTss1()
    {
        DataSet ds = new DataSet();
        try
        {
            string sql = "select cus_num as cusnum,cur_code as curcode,led_code as ledcode,sub_acct_code as subacctcode" +
                " from tbl_sett_branch_acct where statusflag=1 ";
            Connect c = new Connect(sql, true);
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            new ErrorLog("Error occured getting the TSS account");
        }
        return ds;
    }
    //get the current TSS
    public DataSet getCurrentFees()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getCurrentFees");
            ds = c.query("recs");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }

    //new
    public bool getMaxperTransPerday(string bracode, string cusnum, string curcode, string ledcode, string subacctcode, string nuban)
    {
        bool found = false;
        DataSet ds = new DataSet();
        string sql = "";
        //sql = "select bra_code,cus_num,cur_code,led_code,sub_acct_code,maxpertran,maxperday,addedby,statusflag from tbl_nipconcessionTrnxlimits where statusflag=1 " +
        //    " and bra_code=@bc and cus_num=@cn and cur_code=@cc and led_code=@lc and sub_acct_code =@sc";
        sql = "select bra_code,cus_num,cur_code,led_code,sub_acct_code,maxpertran,maxperday,addedby,statusflag from tbl_nipconcessionTrnxlimits where statusflag=1 " +
            " and nuban= @nu";
        try
        {
            Connect c = new Connect(sql, true);
            c.addparam("@nu", nuban);
            //c.addparam("@bc", bracode);
            //c.addparam("@cn", cusnum);
            //c.addparam("@cc", curcode);
            //c.addparam("@lc", ledcode);
            //c.addparam("@sc", subacctcode);
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                maxPerTrans = decimal.Parse(dr["maxpertran"].ToString());
                maxPerday = decimal.Parse(dr["maxperday"].ToString());
                found = true;
            }
            else
            {
                found = false;
            }
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
            found = false;
        }
        return found;
    }

    public DataSet getCurrentExpcode()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getcurrentExpcode");
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }
    //get the current Income account
    public DataSet getCurrentIncomeAcct()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getCurrentIncomeAcct");
            ds = c.query("recs");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }
    //insert new tss account
    public int AddNewTssAccount(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_AddTssAcct");
            c.addparam("@bra_code", t.bra_code);
            c.addparam("@cusnum", t.cusnum);
            c.addparam("@curcode", t.curcode);
            c.addparam("@ledcode", t.ledcode);
            c.addparam("@subacctcode", t.subacctcode);
            c.addparam("@Addedby", t.Addedby);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
    //explanation code
    public int AddNewExpcode(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_AddExpcode");
            c.addparam("@expcodeVal", t.expcode);
            c.addparam("@addedby", t.Addedby);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }

    public int AddNewFees(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_AddFees");
            c.addparam("@Amt1", t.amt1);
            c.addparam("@Amt2", t.amt2);
            c.addparam("@Amt3", t.amt3);
            c.addparam("@Addedby", t.Addedby);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }

    public int AddNewFeeAccount(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_AddFeeAcct");
            c.addparam("@bra_code", t.bra_code);
            c.addparam("@cusnum", t.cusnum);
            c.addparam("@curcode", t.curcode);
            c.addparam("@ledcode", t.ledcode);
            c.addparam("@subacctcode", t.subacctcode);
            c.addparam("@Addedby", t.Addedby);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }

    public DataSet getTransFee()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getFeeCharge");
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }

    public void updateByRefID(Transaction t)
    {
        string sql = "Update tbl_nibssmobile set response =@rsp where refid = @rid";
        try
        {
            Connect c = new Connect(sql, true);
            c.addparam("@rsp", t.ResponseCode);
            c.addparam("@rid", t.Refid);
            c.query();
        }
        catch (Exception ex)
        {
            new ErrorLog("Error occured updating record with sessionid " + t.sessionidNE + " " + ex);
        }
    }
    public void InsertPartialOutgoing(Transaction t)
    {
        DataSet ds = new DataSet();
        string sql = @"insert into tbl_nibssmobile
(sessionidNE,transactioncode,channelCode,paymentRef,amt,feecharge,vat,
originatorname,bra_code,cus_num,cur_code,led_code,sub_acct_code,AccountName,AccountNumber, bankcode,nuban,nameResponse,response)
values
(@sessionidNE,@transactioncode,@channelCode,@paymentRef,@amt,@feecharge,@vat,@originatorname,@bra_code,
@cus_num,@cur_code,@led_code,@sub_acct_code,@AccountName,@AccountNumber,@bankcode,@nuban,'00',@response);
select @@identity as tcode";
        string resp = "";
        if (t.paymentRef.Length > 100)
        {
            t.paymentRef = t.paymentRef.Substring(0, 100);
        }
        try
        {
            Connect c = new Connect(sql, true);
            c.addparam("@sessionidNE", t.sessionid);
            c.addparam("@transactioncode", t.transactionCode);
            c.addparam("@channelCode", t.channelCode);
            c.addparam("@paymentRef", t.paymentRef);
            c.addparam("@amt", t.amount);
            c.addparam("@feecharge", t.feecharge);
            c.addparam("@vat", t.vat);
            c.addparam("@originatorname", t.originatorname);
            c.addparam("@bra_code", t.bra_code);
            c.addparam("@cus_num", t.cusnum);
            c.addparam("@cur_code", t.curcode);
            c.addparam("@led_code", t.ledcode);
            c.addparam("@sub_acct_code", t.subacctcode);
            c.addparam("@AccountName", t.AccountName);
            c.addparam("@AccountNumber", t.AccountNumber);
            c.addparam("@bankcode", t.destinationcode);
            c.addparam("@nuban", t.nuban);
            c.addparam("@response", t.ResponseCode);
            ds = c.query("rec");
            resp = ds.Tables[0].Rows[0]["tcode"].ToString();
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog("Error Occured while inserting for sid " + t.sessionid + ex);
        }
    }

    public string InsertOutgoingNibss(Transaction t)
    {
        DataSet ds = new DataSet();
        string sql = @"insert into tbl_nibssmobile
(sessionidNE,transactioncode,channelCode,paymentRef,amt,feecharge,vat,
originatorname,bra_code,cus_num,cur_code,led_code,sub_acct_code,AccountName,AccountNumber, bankcode,nuban)
values
(@sessionidNE,@transactioncode,@channelCode,@paymentRef,@amt,@feecharge,@vat,@originatorname,@bra_code,
@cus_num,@cur_code,@led_code,@sub_acct_code,@AccountName,@AccountNumber,@bankcode,@nuban);
select @@identity as tcode";
        new ErrorLog("New insert into table " + t.sessionid + " " + t.channelCode + " " + t.paymentRef + " " + t.destinationcode + " " + t.amount.ToString() + " " + t.feecharge.ToString());
        string resp = "";
        if (t.paymentRef.Length > 100)
        {
            t.paymentRef = t.paymentRef.Substring(0, 100);
        }
        try
        {
        Connect c = new Connect(sql, true);
        c.addparam("@sessionidNE", t.sessionid);
        c.addparam("@transactioncode", t.transactionCode);
        c.addparam("@channelCode", t.channelCode);
        c.addparam("@paymentRef", t.paymentRef);
        c.addparam("@amt", t.amount);
        c.addparam("@feecharge", t.feecharge);
        c.addparam("@vat", t.vat);
        c.addparam("@originatorname", t.originatorname);
        c.addparam("@bra_code", t.bra_code);
        c.addparam("@cus_num", t.cusnum);
        c.addparam("@cur_code", t.curcode);
        c.addparam("@led_code", t.ledcode);
        c.addparam("@sub_acct_code", t.subacctcode);
        c.addparam("@AccountName", t.AccountName);
        c.addparam("@AccountNumber", t.AccountNumber);
        c.addparam("@bankcode", t.destinationcode);
        c.addparam("@nuban", t.nuban);
        ds = c.query("rec");
            resp = ds.Tables[0].Rows[0]["tcode"].ToString();
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog("Error Occured while inserting for sid " + t.sessionid + ex);
        }
        return ds.Tables[0].Rows[0]["tcode"].ToString();
        //return resp;
        //return "";
    }

    //insert Mobile Transaction
    public string InsertNameMobileTrans(Transaction t)
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_InsertMobileTrans");
            c.addparam("@sessionidNE", t.sessionid);
            c.addparam("@transactioncode", t.transactionCode);
            c.addparam("@channelCode", t.channelCode);
            c.addparam("@paymentRef", t.paymentRef);
            c.addparam("@amt", t.amount);
            c.addparam("@feecharge", t.feecharge);
            c.addparam("@vat", t.vat);
            c.addparam("@originatorname", t.originatorname);
            c.addparam("@bra_code", t.bra_code);
            c.addparam("@cus_num", t.cusnum);
            c.addparam("@cur_code", t.curcode);
            c.addparam("@led_code", t.ledcode);
            c.addparam("@sub_acct_code", t.subacctcode);
            c.addparam("@AccountName", t.AccountName);
            c.addparam("@AccountNumber", t.AccountNumber);
            //c.addparam("@destbankcode", t.destinationcode);
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds.Tables[0].Rows[0]["tcode"].ToString();
    }

    public string InsertMobileTrans(Transaction t)
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_InsertMobileTrans");
            c.addparam("@sessionid", t.sessionid);
            c.addparam("@transactioncode", t.transactionCode);
            c.addparam("@channelCode", t.channelCode);
            c.addparam("@paymentRef", t.paymentRef);
            c.addparam("@amt", t.amount);
            c.addparam("@feecharge", t.feecharge);
            c.addparam("@originatorname", t.originatorname);
            c.addparam("@bra_code", t.bra_code);
            c.addparam("@cus_num", t.cusnum);
            c.addparam("@cur_code", t.curcode);
            c.addparam("@led_code", t.ledcode);
            c.addparam("@sub_acct_code", t.subacctcode);
            c.addparam("@AccountName", t.AccountName);
            c.addparam("@AccountNumber", t.AccountNumber);
            ds = c.query("rec");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds.Tables[0].Rows[0]["tcode"].ToString();
    }

    public int UpdateMobileTrans(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateMobileTrans");
            c.addparam("@Refid", t.Refid);
            c.addparam("@response", t.ResponseCode);
            c.addparam("@sessionid", t.sessionid);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
    public int UpdateMobileTransactions(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateNibssMobileNameEnquiry");
            c.addparam("@Refid", t.Refid);
            c.addparam("@nameResponse", t.ResponseCode);
            c.addparam("@AccountName", t.AccountName);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
    public int UpdateMobileTransFundsTransfer(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateNibssMobileFundsTransfer");
            c.addparam("@Refid", t.Refid);
            c.addparam("@response", t.ResponseCode);
            c.addparam("@responseDesc", t.ResponseDesc);
            c.addparam("@recipientName", t.RecipientName);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
    public int UpdateMobilevTeller(Transaction t,int status)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateVteller");
            c.addparam("@vTellerMsg",status);
            c.addparam("@refid",t.Refid);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }


    //ibs
    //Update IBS transactions
    public int UpdateIBSTransactions(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateNibssMobileNameEnquiry");
            c.addparam("@Refid", t.Refid);
            c.addparam("@nameResponse", t.ResponseCode);
            c.addparam("@AccountName", t.AccountName);
            c.query();
            cn = c.returnValue;
            //new ErrorLog("IBS Update NE Response==>" + t.Refid + t.ResponseCode + t.AccountName);
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        //return cn;
        return cn;
    }
    public string InsertNameIBSTrans(Transaction t)
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_InsertMobileTrans");
            c.addparam("@sessionidNE", t.sessionid);
            c.addparam("@transactioncode", t.transactionCode);
            c.addparam("@channelCode", t.channelCode);
            c.addparam("@paymentRef", t.paymentRef);
            c.addparam("@amt", t.amount);
            c.addparam("@feecharge", t.feecharge);
            c.addparam("@vat", t.vat);
            c.addparam("@originatorname", t.originatorname);
            c.addparam("@bra_code", t.bra_code);
            c.addparam("@cus_num", t.cusnum);
            c.addparam("@cur_code", t.curcode);
            c.addparam("@led_code", t.ledcode);
            c.addparam("@sub_acct_code", t.subacctcode);
            c.addparam("@AccountName", t.AccountName);
            c.addparam("@AccountNumber", t.AccountNumber);
            ds = c.query("rec");
            //new ErrorLog("IBS NE==>" + t.sessionid + " " + t.transactionCode + " " + t.channelCode + " " + t.paymentRef + " " + t.feecharge + " " + t.originatorname + " " + t.bra_code + " " + t.cusnum + " " + t.curcode + " " + t.ledcode + " " + t.subacctcode + " " + t.AccountName + " " + t.AccountNumber);
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds.Tables[0].Rows[0]["tcode"].ToString();
    }
    public int UpdateIBSTrans(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateMobileTrans");
            c.addparam("@Refid", t.Refid);
            c.addparam("@response", t.ResponseCode);
            c.addparam("@sessionid", t.sessionid);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
    public int UpdateIBSvTeller(Transaction t, int status)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateVteller");
            c.addparam("@vTellerMsg", status);
            c.addparam("@refid", t.Refid);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        cn = 1;
        //new ErrorLog("IBS Update vTeller==>" + t.Refid + " " + status);
        return cn;
    }



    //BANK TELLER
    public string InsertNameBankTellerTrans(Transaction t)
    {
        new ErrorLog("Working on trnx: " + t.sessionid);

        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_InsertMobileTrans");
            c.addparam("@sessionidNE", t.sessionid);
            c.addparam("@transactioncode", t.transactionCode);
            c.addparam("@channelCode", t.channelCode);
            c.addparam("@paymentRef", t.paymentRef);
            c.addparam("@amt", t.amount);
            c.addparam("@feecharge", t.feecharge);
            c.addparam("@vat", t.vat);
            c.addparam("@originatorname", t.originatorname);
            c.addparam("@bra_code", t.bra_code);
            c.addparam("@cus_num", t.cusnum);
            c.addparam("@cur_code", t.curcode);
            c.addparam("@led_code", t.ledcode);
            c.addparam("@sub_acct_code", t.subacctcode);
            c.addparam("@AccountName", t.AccountName);
            c.addparam("@AccountNumber", t.AccountNumber); 
            ds = c.query("rec");            
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds.Tables[0].Rows[0]["tcode"].ToString();
    }
    public int UpdateBankTellerTransactions(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateNibssMobileNameEnquiry");
            c.addparam("@Refid", t.Refid);
            c.addparam("@nameResponse", t.ResponseCode);
            c.addparam("@AccountName", t.AccountName);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog("Error Occured while updating NEResp with Refid " + t.Refid.ToString() + ex);
        }
        return cn;
    }

    public int UpdateT24FTResponse2(string Prin_rsp, string Fee_rsp, string Vat_rsp, string refid)
    {
        int cn = 0;
        try
        {
            new ErrorLog("Prepare to update record with sessionid " + refid);
            string sql = "update tbl_nibssmobileT24 set Prin_Rsp =@pr,Fee_Rsp =@fr,Vat_Rsp =@vr, testStatus=1 where sessionidNE=@rid";
            Connect c = new Connect(sql, true);
            c.addparam("@pr", Prin_rsp);
            c.addparam("@fr", Fee_rsp);
            c.addparam("@vr", Vat_rsp);
            c.addparam("@rid", refid);
            cn = c.query();
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }

    public int UpdateT24FTResponse(string Prin_rsp, string Fee_rsp, string Vat_rsp, Int32 refid)
    {
        int cn = 0;
        try
        {

            string sql = "update tbl_nibssmobile set Prin_Rsp =@pr,Fee_Rsp =@fr,Vat_Rsp =@vr where Refid=@rid";
            Connect c = new Connect(sql, true);
            c.addparam("@pr", Prin_rsp);
            c.addparam("@fr", Fee_rsp);
            c.addparam("@vr", Vat_rsp);
            c.addparam("@rid", refid);
            cn = c.query();
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }
    public int UpdateBankTellervTeller(Transaction t, int status)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateVteller");
            c.addparam("@vTellerMsg", status);
            c.addparam("@refid", t.Refid);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        cn = 1;
        //new ErrorLog("IBS Update vTeller==>" + t.Refid + " " + status);
        return cn;
    }
    public int UpdateBankTellerTrans(Transaction t)
    {
        int cn = 0;
        try
        {
            Connect c = new Connect("spd_UpdateMobileTrans");
            c.addparam("@Refid", t.Refid);
            c.addparam("@response", t.ResponseCode);
            c.addparam("@sessionid", t.sessionid);
            c.query();
            cn = c.returnValue;
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return cn;
    }

    //Transaction limits for New IBS
    public DataSet getTransactionLimitsNewIBSCurrent()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getTransactionLimitNewIBSCurrent");
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                maxPerTrans = decimal.Parse(dr["maxpertran"].ToString());
                maxPerday = decimal.Parse(dr["maxperday"].ToString());
            }
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
        }
        return ds;
    }
    //savings
    public DataSet getTransactionLimitsNewIBSSavings()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getTransactionLimitNewIBSSavings");
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                maxPerTrans = decimal.Parse(dr["maxpertran"].ToString());
                maxPerday = decimal.Parse(dr["maxperday"].ToString());
            }
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
        }
        return ds;
    }

    public DataSet getTransactionLimits()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getTransactionLimit");
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                maxPerTrans = decimal.Parse(dr["maxpertran"].ToString());
                maxPerday = decimal.Parse(dr["maxperday"].ToString());
            }
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
        }
        return ds;
    }
    public DataSet getTotalSumdone(string bracode, string cusnum, string curcode, string ledcode, string subacctcode)
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getTotalTransfer");
            c.addparam("@bra_code", bracode);
            c.addparam("@cus_num", cusnum);
            c.addparam("@cur_code", curcode);
            c.addparam("@led_code", ledcode);
            c.addparam("@sub_acct_code", subacctcode);
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                try
                {
                    sum = decimal.Parse(dr["amt"].ToString());
                }
                catch
                {
                    sum = 0;
                }
            }
            else
            {
                sum = 0;
            }
        }
        catch (Exception ex)
        {
            new ErrorLog("Error getting total sum " + ex);
        }
        return ds;
    }

    public DataSet getTotalSumdoneInward(string bracode, string cusnum, string curcode, string ledcode, string subacctcode, string senderCode)
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getTotalTransferInward");
            c.addparam("@bra_code", bracode);
            c.addparam("@cus_num", cusnum);
            c.addparam("@cur_code", curcode);
            c.addparam("@led_code", ledcode);
            c.addparam("@sub_acct_code", subacctcode);
            c.addparam("@senderbankcode", senderCode);
            ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                sum = decimal.Parse(dr["amt"].ToString());
            }
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
        }
        return ds;
    }



    public int UpdateFTSessionID(int refid, string sessionid)
    {
        try
        {
            new ErrorLog("Prepare to Update Sessionid for " + refid.ToString() + " and " + sessionid);
        }
        catch
        {
            //procced
        }

        int cn = 0;
        string sql = "update tbl_nibssmobile set sessionid = @sessionid where refid = @Refid";
        try
        {
            Connect c = new Connect(sql,true);
            c.addparam("@Refid", refid);
            c.addparam("@sessionid", sessionid);
            cn = c.query();
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog("Unable to Update Sessionid for FT " + ex);
        }
        return cn;
    }

    public DataSet getCurrentEbillsIncomeAcct()
    {
        DataSet ds = new DataSet();
        try
        {
            Connect c = new Connect("spd_getCurrentEbillsIncomeAcct");
            ds = c.query("recs");
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return ds;
    }
}
