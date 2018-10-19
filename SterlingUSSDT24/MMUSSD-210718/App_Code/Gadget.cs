using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Tj.Cryptography;


public class Gadget
{
    //**************************************** Aug 4th 2017 ********************************************
    public DataSet GetImalAccountsByMobileNo(String mb)
    {
        string sql = "select NUBAN from Go_Registered_Account where mobile =@mb and Activated = 1 and StatusFlag=1 and SUBSTRING(NUBAN,1,2) ='05'";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mb", mb);
        DataSet ds = c.Select("rec");
        return ds;
    }
    public string GetImalBalance(string nuban)
    {
        string val = "";

        DataSet ds = new DataSet(); string json = "";
        string sql = @"select long_name_eng,branch_code,cif_sub_no,currency_code,gl_code, " +
            " sl_no,status,cv_avail_bal,ytd_cv_bal,last_trans_date from imal.amf " +
                     " where additional_reference='" + nuban + "'";
        Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
        cn.SetSQL(sql);
        ds = cn.Select();
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            val = dr["cv_avail_bal"].ToString().Replace("-", "") + "*" + dr["gl_code"].ToString();
        }
        else
        {
            val = "*";
        }
        return val;
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

    public int GetNoofAccts(string mob)
    {
        int val = 0;
        //string sql = "SELECT count(*) as cnt FROM Go_Registered_Account " +
        //             " where Mobile = @mob " +
        //             " and StatusFlag = 1 and Activated = 1";
        string sql = "SELECT count(distinct NUBAN) as cnt FROM Go_Registered_Account " +
                     " where Mobile = @mob " +
                     " and StatusFlag = 1 and Activated in (1,0)";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@mob", mob);
        DataSet ds = c.Select("rec");
        if(ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            val = int.Parse(dr["cnt"].ToString());
        }
        return val;
    }
    public string GetNubanByListID(int id, UReq req)
    {
        string nuban = "";
        string sql = "select * from tbl_USSD_acctList where mobile =@mob and TransRate = @id and sessionid =@sid";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@mob", req.Msisdn);
        c.AddParam("@id", id);
        c.AddParam("@sid", req.SessionID);
        DataSet ds = c.Select("rec");
        if(ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            nuban = dr["nuban"].ToString();
        }
        return nuban;
    }
    public List<AccountList> GetListofAcctsByPage(string page, string sid)
    {
        List<AccountList> lb = new List<AccountList>();
        int pg = 0;
        try
        {
            pg = Convert.ToInt32(page);
        }
        catch
        {
        }

        int pageSize = 5;
        float offset = pg * pageSize;

        string sql = " SELECT top " + pageSize + " * FROM tbl_USSD_acctList " +
                     " where statusflag = 1 and sessionid =@sid " +
                     " and shortname not in " +
                     " (select top " + offset.ToString("0") +
                     " shortname  from tbl_USSD_acctList " +
                     " where statusflag = 1 and sessionid =@sid order by TransRate asc) " +
                     " order by TransRate asc";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@sid", sid);
        DataSet ds = cn.Select();
        if (cn.num_rows > 0)
        {
            for (int i = 0; i < cn.num_rows; i++)
            {
                AccountList b = new AccountList();
                b.Set(ds.Tables[0].Rows[i]);
                lb.Add(b);
            }
        }
        return lb;
    }
    //**************************************************************************************************
    //IMAL keys

    public  String Encrypt(String val)
    {
        MemoryStream ms = new MemoryStream();
        string rsp = "";
        try
        {
            string sharedkeyval = ""; string sharedvectorval = "";

            sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100011111";
            sharedkeyval = BinaryToString(sharedkeyval);

            sharedvectorval = "0000000100000010000000110000010100000111000010110000110100011111";
            sharedvectorval = BinaryToString(sharedvectorval);
            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
        }
        catch
        {
        }
        return Convert.ToBase64String(ms.ToArray());
    }
    public  String Decrypt(String val)
    {
        MemoryStream ms = new MemoryStream();
        string rsp = "";
        try
        {
            string sharedkeyval = ""; string sharedvectorval = "";

            sharedkeyval = "000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100011111";
            sharedkeyval = BinaryToString(sharedkeyval);

            sharedvectorval = "0000000100000010000000110000010100000111000010110000110100011111";
            sharedvectorval = BinaryToString(sharedvectorval);
            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);


            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();
        }
        catch
        {

        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }
    public  string BinaryToString(string binary)
    {
        if (string.IsNullOrEmpty(binary))
            throw new ArgumentNullException("binary");

        if ((binary.Length % 8) != 0)
            throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < binary.Length; i += 8)
        {
            string section = binary.Substring(i, 8);
            int ascii = 0;
            try
            {
                ascii = Convert.ToInt32(section, 2);
            }
            catch
            {
                throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
            }
            builder.Append((char)ascii);
        }
        return builder.ToString();
    }

    //***************** for ATM PIN change ************************************
    public string getPANbyID(string panid, string sid)
    {
        string resp = "";string sql = "";
        sql = "select pan from tbl_USSD_cardPAN where panid = @pid and sessionid = @sid";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@pid", int.Parse(panid));
        c.AddParam("@sid", sid);
        DataSet ds = c.Select();
        if(ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            resp = dr["pan"].ToString();
        }
        return resp;
    }
    public string MaskPan(string pan)
    {
        if (pan.Length > 10)
            return pan.Substring(0, 6) + string.Empty.PadLeft(pan.Length - 10, '*') + pan.Substring(pan.Length - 4, 4);
        return pan;
    }
   
    public string pan; public int panid; public string sessionid;
    private readonly object _locker = new object();
    public void saveRec()
    {
        lock (_locker)
        {
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
            cn.SetProcedure("spd_InsertUSSD_CardPAN");
            cn.AddParam("@panid", this.panid);
            cn.AddParam("@pan", this.pan);
            cn.AddParam("@sessionid", this.sessionid);
            cn.ExecuteProc();
        }
    }
    public void saveRec1()
    {
        lock (_locker)
        {
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
            cn.SetProcedure("spd_InsertIntoATMPINChange");
            cn.AddParam("@mobile", this.Mobile);
            cn.AddParam("@sessionid", this.sidval);
            cn.AddParam("@pan", this.panval);
            cn.AddParam("@seq_nr", this.Seqval);
            cn.AddParam("@exp_date", this.ExdVal);
            cn.AddParam("@fepresponsecode", this.RespVal);
            cn.ExecuteProc();
        }
    }
    public void InsertCardPANs(int thePanid, string thePan, string theSid)
    {
        this.panid = thePanid;
        this.pan = thePan;
        this.sessionid = theSid;
        Thread worker = new Thread(new ThreadStart(saveRec));
        worker.Start();
    }
    public string Mobile;public string sidval; public string panval; public string Seqval; public string ExdVal; public string RespVal;
    public void InsertFInalRecord(string themb, string thesd, string thePan, string theSeq, string theExpd, string theRes)
    {
        this.Mobile = themb;
        this.sidval = thesd;
        this.panval = thePan;
        this.Seqval = theSeq;
        this.ExdVal = theExpd;
        this.RespVal = theRes;
        Thread worker = new Thread(new ThreadStart(saveRec1));
        worker.Start();
    }
    public string getPaddedCusnum(string cusnum)
    {
        string padCusnume = "";
        if (cusnum.Length < 7)
        {
            padCusnume = cusnum.PadLeft(7, '0');
        }
        else
        {
            padCusnume = cusnum;
        }
        return padCusnume;
    }
    public string getPAN_Seqnr(string cusnum, string cusnumpadded)
    {
        //use this to filter out unique response
        HashSet<string> s = new HashSet<string>();
        string cards = ""; string pan = "";
        string sql = "SELECT distinct pan,seq_nr,expiry_date FROM pc_cards_1_A WHERE customer_id IN ('" + cusnum + "', '" + cusnumpadded + "') AND " +
                         "hold_rsp_code IS NULL AND card_status = '1' AND expiry_date > = '" + DateTime.Now.ToString("yyMM") + "'";

        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("FEP");
        c.SetSQL(sql);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                pan = dr["pan"].ToString() + "+" + dr["seq_nr"].ToString();
                s.Add(pan);
            }
            string panvalue = string.Empty;
            foreach (string unique in s)
            {
                panvalue += unique + "*";
            }
            panvalue = panvalue.TrimEnd(new char[] { '*' });
            cards = panvalue.TrimEnd("::".ToCharArray());
        }
        else
        {
            cards = "";
        }
        return cards;
    }
    public string getPAN(string cardPAN, string SessionID)
    {
        string resp = ""; char[] sep = { '*' };
        string pan = ""; int val = 0;
        //split the cardpan
        cardPAN = cardPAN.Trim(sep);
        string[] ThePAN = cardPAN.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < ThePAN.Length; i++)
        {
            val += 1;
            pan = ThePAN[i].ToString();
            string[] bits = pan.Split('+');
            InsertCardPANs(val, pan, SessionID);
            resp += "%0A" + val.ToString() + "." + MaskPan(bits[0]);
        }
        return resp;
    }
    public string getPANBySessionID(string SessionID)
    {
        string val; string resp = ""; string pan = "";
        string sql = "select panid,pan from tbl_ussd_cardPAN where sessionid =@sid and statusflag = 0 order by panid asc";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
        c.SetSQL(sql);
        c.AddParam("@sid", sessionid);
        DataSet ds = c.Select();
        if(ds.Tables[0].Rows.Count > 0)
        {
            for(int i = 0; i <= ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                val = dr["panid"].ToString();
                pan = dr["pan"].ToString();
                resp += "%0A" + val.ToString() + ". " + pan;
            }
        }
        return resp;
    }
    //**************************************************************

    //***************************************************************
    public int GetBillerIDbyTransRate(int tid)
    {
        int billerid = 0;
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_GetbillersByTransRateISW");
        c.AddParam("@TransRate", tid);
        DataSet ds = c.Select("rec");
        if(ds.Tables[0].Rows.Count > 0 )
        {
            DataRow dr = ds.Tables[0].Rows[0];
            billerid = int.Parse(dr["BillerID"].ToString());
        }
        return billerid;
    }
    public int RunBillerNextPage(string s)
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
    public List<BillerItem> GetBillersByPage(string page, int billerid)
    {
        List<BillerItem> lb = new List<BillerItem>();
        int pg = 0;
        try
        {
            pg = Convert.ToInt32(page);
        }
        catch
        {
        }

        int pageSize = 5;
        float offset = pg * pageSize;

        string sql = " SELECT top " + pageSize + " * FROM tbl_GetPaymentItemISW " +
                     " where statusflag = 1 and BillerId =@bid " +
                     " and shortname not in " +
                     " (select top " + offset.ToString("0") +
                     " shortname  from tbl_GetPaymentItemISW " +
                     " where statusflag = 1 and BillerId =@bid order by TransRate asc) " +
                     " order by TransRate asc";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        //new ErrorLog("sql val " + sql);
        cn.SetSQL(sql);
        cn.AddParam("@bid", billerid);
        DataSet ds = cn.Select();
        if (cn.num_rows > 0)
        {
            for (int i = 0; i < cn.num_rows; i++)
            {
                BillerItem b = new BillerItem();
                b.Set(ds.Tables[0].Rows[i]);
                lb.Add(b);
            }
        }
        return lb;
    }

    //************************************************************




    public int ConfirmUserStatus(string mb)
    {
        int val = -1;
        //1 not found. 2 found but not active 3.active
        try
        {
            //check if you are registered and have pin set
            string sql = ""; int activated = -1; string nuban = "";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
            c.SetProcedure("spd_getRegisteredUserProfile");
            c.AddParam("@mobile", mb);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                activated = int.Parse(dr["Activated"].ToString());
            }
            else
            {
                //not found or registered
                //req.next_op = 122;
                //return 0;
                return 1;
            }

            if (activated == 1)
            {
                val = 3; //found/registered/active
            }
            else if (activated == 0)
            {
                // found but not active
                return 2;
            }
        }
        catch
        {

        }
        return val;
    }

    public string GetAccountsByMobileNo2(String mb)
    {
        string nuban = "";
        string sql = "select NUBAN from Go_Registered_Account where mobile =@mb and Activated in (1,0) and StatusFlag=1 order by Activated desc";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mb", mb);
        DataSet ds = c.Select("rec");
        if(ds.Tables[0].Rows.Count > 0)
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
    public void Insert_Charges(string sid, string nuban, int transtypeid)
    {
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetProcedure("spd_Insert_USSD_txn_for_charges");
        c.AddParam("@sessionid", sid);
        c.AddParam("@debitAccount", nuban);
        c.AddParam("@transtypeid", transtypeid);
        c.ExecuteProc();
    }
    public DataSet GetAccountsByMobileNo(string mb)
    {
        string sql = "select ACCOUNTID as MAP_ACC_NO,CUR_CODE,led_code from tbl_USSD_CUSACCT where MOBILE =@mb";
        
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@mb", mb);
        DataSet ds = cn.Select("rec");


        return ds;
    }


    //for nibss merchant pay/////////////////////////////////////////////////////////
    public bool CardAuthenticate(string Nuban, Int32 last4digits)
    {
        bool found = false;
        string sql = "select * from tbl_USSD_card_auth where cardacct =@cat and last4carddigits =@l4d ";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        //Connect c = new Connect(sql, true);
        cn.AddParam("@cat", Nuban);
        cn.AddParam("@l4d", last4digits);
        DataSet ds = cn.Select("rec");
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
    public string getRealMoney(decimal amt)
    {
        string amtval = "";
        amtval = amt.ToString("#,##.00");
        return amtval;
    }
    public string GenerateRndNumber(int cnt)
    {
        string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        Random rand1 = new Random();
        string txt = "";
        for (int j = 0; j < cnt; j++)
            txt += key2[rand1.Next(0, 9)];
        return txt;
    }
    public string newSessionGlobal(string branchCode, int channelcode)
    {
        int _bracode = 0;
        try
        {
            _bracode = Convert.ToInt32(branchCode);
            if (_bracode > 9999 || _bracode < 0)
            {
                _bracode = 0;
            }
        }
        catch { }

        Thread.Sleep(10);
        return "999232" + DateTime.Now.ToString("yyMMddHHmmss") + _bracode.ToString("0000") + channelcode.ToString("00") + GenerateRndNumber(6);
    }
    //////////////////////////////////////////////////////////////////////////////////////////
    //to encrpt
    public static string enkrypt(string strEnk)
    {
        SymCryptography cryptic = new SymCryptography();
        cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";
        return cryptic.Encrypt(strEnk);

    }
    //to decrypt
    public static string dekrypt(string strDek)
    {
        SymCryptography cryptic = new SymCryptography();
        cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";
        return cryptic.Decrypt(strDek);
    }    

    public static int ToInt32(object p)
    {
        int resp = 0;
        try
        {
            resp = Convert.ToInt32(p);
        }
        catch { }
        return resp;
    }

    public string GetCusNumByMobileNo(String mb)
    {
        string customerID = "";
        string sql = "select a.CUSTOMER from OverallMapperAcctReplica a  " + 
                      "  inner join Go_Registered_Account b " +
                      "  on a.NUBAN = b.NUBAN " + 
                      "  where b.Mobile = @mb";
        Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect();
        c.SetSQL(sql);
        c.AddParam("@mb", mb);
        DataSet ds = c.Select("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            customerID = dr["CUSTOMER"].ToString();
        }
        else
        {
            customerID = "";
        }
        return customerID;
    }

    public string GetT24CustId(string mobile)
    {
        string custid = "";
        EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        var ds = ws.getCustomerAccountsByMobileNo2(mobile);
        if(ds.Tables[0].Rows.Count > 0)
        {
            custid = ds.Tables[0].Rows[0]["CustomerId"].ToString();
        }
        else
        {
            //Likely mobile is not profiled on T24
        }
        return custid;

    }


    //************************************* Tbills *************************************
    public int RunTbillNextPage(string s)
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
    public List<TreasuryBillItem> GetTbillsByPage(string page)
    {
        List<TreasuryBillItem> lb = new List<TreasuryBillItem>();
        int pg = 0;
        try
        {
            pg = Convert.ToInt32(page);
        }
        catch
        {
        }

        int pageSize = 5;
        float offset = pg * pageSize;

        string sql = " SELECT top " + pageSize + " * FROM tbl_USSD_Tbills " +
                     " where statusflag = 1 " +
                     " and shortname + ' ' + convert(Varchar(50), datetoMaturity,112) not in " +
                     " (select top " + offset.ToString("0") +
                     " shortname + ' ' + convert(Varchar(50), datetoMaturity,112)  from tbl_USSD_Tbills " +
                     " where statusflag = 1 order by TransRate asc) " +
                     " order by TransRate asc";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        DataSet ds = cn.Select();
        if (cn.num_rows > 0)
        {
            for (int i = 0; i < cn.num_rows; i++)
            {
                TreasuryBillItem b = new TreasuryBillItem();
                b.Set(ds.Tables[0].Rows[i]);
                lb.Add(b);
            }
        }
        return lb;
    }
    //**********************************************************************************
}