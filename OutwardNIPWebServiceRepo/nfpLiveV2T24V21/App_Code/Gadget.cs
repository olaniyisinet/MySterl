using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Text;
using Tj.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Data;

public class Gadget
{

    public int UpdateNIPInward(string ResponseCode, string SessionID, string bra_code, string cus_num, string cur_code, 
        string led_code, string sub_acct_code, decimal amount, string PaymentReference, string Narration,
        string OriginatorAccountName, string BeneficiaryAccountName, string NameEnquiryRef,
        string BeneficiaryBankVerificationNumber, string OriginatorAccountNumber, string OriginatorBankVerificationNumber,
        string OriginatorKYCLevel, string TransactionLocation)
    {
        int cn = -1;
        Connect c1 = new Connect("spd_WSupdatetrans");
        c1.addparam("@sessionid", SessionID);
        c1.addparam("@bra_code", bra_code);
        c1.addparam("@cus_num", cus_num);
        c1.addparam("@cur_code", cur_code);
        c1.addparam("@led_code", led_code);
        c1.addparam("@sub_acct_code", sub_acct_code);
        c1.addparam("@amt", amount);
        c1.addparam("@payRef", PaymentReference);
        c1.addparam("@manRef", "");
        c1.addparam("@remark", Narration);
        c1.addparam("@originSender", OriginatorAccountName);
        c1.addparam("@Responsecode", ResponseCode);
        c1.addparam("@accname", BeneficiaryAccountName);
        c1.addparam("@feecharge", 0);
        c1.addparam("@ResponseMsg", responseCodes(ResponseCode));

        c1.addparam("@NameEnquiryRef", NameEnquiryRef);
        c1.addparam("@BeneficiaryBankVerificationNumber", BeneficiaryBankVerificationNumber);
        c1.addparam("@OriginatorAccountNumber", OriginatorAccountNumber);
        c1.addparam("@OriginatorBankVerificationNumber", OriginatorBankVerificationNumber);

        c1.addparam("@OriginatorKYCLevel", OriginatorKYCLevel);
        c1.addparam("@TransactionLocation", TransactionLocation);
        cn = c1.query();
        if (cn > 0)
        {
            //log the the block here
            //new ErrorLog("57 returned for customer when Rest_txt == false " + trx.SessionID + " for amt " + t.amount + " with the restcode " + RestCode.ToString() + " SID " + trx.SessionID);
            //Mylogger.Info("57 returned for customer when Rest_txt == false " + trx.SessionID + " for amt " + t.amount + " with the restcode " + RestCode.ToString() + " SID " + trx.SessionID);
            //return trx.createResponse();
        }
        else
        {

        }
        return cn;
    }



    public bool isSessionIDExist(string sid)
    {
        bool found = false;
        string sql = "select sessionid from tbl_WStrans where sessionid =@sid";
        Connect c = new Connect(sql, true);
        c.addparam("@sid", sid);
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
    public bool isDateHoliday(DateTime dt)
    {
        bool found = false;
        try
        {
            string sql = "select * from tbl_public_holiday where holiday =@dt";
            Connect c = new Connect(sql, true);
            c.addparam("@dt", dt);
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
            new ErrorLog("Error occured while checking the holiday table for account number");
            found = true;
        }
        return found;
    }
    public string getNIPBankName(string bc)
    {
        string val = "";
        string sql = "SELECT bankname as bankname1 FROM  tbl_participatingBanks where statusflag=1 and bankcode =@obc";
        Connect c = new Connect(sql, true);
        c.addparam("@obc", bc);
        DataSet ds = c.query("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            val = dr["bankname1"].ToString();
        }
        return val;
    }

    public string getBranchNIPCode(string T24bcode)
    {
        string sql = ""; string bracode = "";
        sql = "select BANK_BRACODE from tbl_branchlist_t24 where  T24_BRACODE = @bc";
        Connect c = new Connect(sql, true);
        c.addparam("@bc", T24bcode);
        DataSet ds = c.query("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            bracode = dr["BANK_BRACODE"].ToString();
        }
        return bracode;
    }

    //move this to T24 ///////////////////
    public bool isLedgerNotAllowed(string lc)
    {
        bool found = false;
        string sql = "select * from tbl_sbpLedcodenotallowed where led_code=@lc and statusflag =1";
        Connect c = new Connect(sql, true);
        c.addparam("@lc", lc);
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
    public bool isBankCodeFound(string bc)
    {
        bool found = false;
        string sql = "SELECT T24_BRACODE from tbl_sbpbankcodes where T24_BRACODE =@bc and statusflag =1";
        Connect c = new Connect(sql, true);
        c.addparam("@bc", bc);
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

    ////////////////////////////////////
    public string CleanXmlText(string txt)
    {
        txt = txt.Replace("&amp;", "&");
        txt = txt.Replace("&apos;", "'");
        txt = txt.Replace("&quot;", "\"");

        txt = txt.Replace("&", "&amp;");
        txt = txt.Replace("'", "&apos;");
        txt = txt.Replace("\"", "&quot;");
        return txt;
    }
    //public string newSessionGlobal(string branchCode, int channelcode)
    //{
    //    int _bracode = 0;
    //    try
    //    {
    //        _bracode = Convert.ToInt32(branchCode);
    //        if (_bracode > 9999 || _bracode < 0)
    //        {
    //            _bracode = 0;
    //        }
    //    }
    //    catch { }

    //    Thread.Sleep(50);
    //    return "999232" + DateTime.Now.ToString("yyMMddHHmmss") + _bracode.ToString("0000") + channelcode.ToString("00") + GenerateRndNumber(6);
    //}
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

        Thread.Sleep(50);
        return "000001" + DateTime.Now.ToString("yyMMddHHmmss") + _bracode.ToString("0000") + channelcode.ToString("00") + GenerateRndNumber(6);
    }
    public string newSessionGlobalSterlingMoney(string branchCode, int channelcode)
    {
        //999017 is sterling scheme code for Sterling money
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

        Thread.Sleep(50);
        return "999017" + DateTime.Now.ToString("yyMMddHHmmss") + _bracode.ToString("0000") + channelcode.ToString("00") + GenerateRndNumber(6);
    }
    public bool findKyc(Int32 ledcode)
    {
        bool found = false;
        string sql_kyc = "select kycledger from tbl_kyc_ledger where kycledger=@kyc";
        Connect c = new Connect(sql_kyc, true);
        c.addparam("@kyc", ledcode);
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
    public decimal MaxNIPtrnx;
    public decimal getMaxNIPTrnx(string bra_code, string cus_num, string cur_code, string led_code, string sub_acct_code)
    {
        string sql = "select ISNULL(max(amt),0) as maxNIP from tbl_nibssmobile " +
            " where bra_code=@bc and cus_num=@cn and cur_code=@cc and led_code=@lc and sub_acct_code=@sc and vTellerMsg=1 " +
            " union " +
            " select ISNULL(max(amt),0) as maxNIP from tbl_nibssmobile_batch " +
            " where bra_code=@bc and cus_num=@cn and cur_code=@cc and led_code=@lc and sub_acct_code=@sc and vTellerMsg=1 " +
            "  ORDER BY maxNIP desc ";

        Connect c = new Connect(sql, true);
        c.addparam("@bc", bra_code);
        c.addparam("@cn", cus_num);
        c.addparam("@cc", cur_code);
        c.addparam("@lc", led_code);
        c.addparam("@sc", sub_acct_code);
        DataSet ds = c.query("rec");
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            MaxNIPtrnx = decimal.Parse(dr["maxNIP"].ToString());
        }
        else
        {

        }
        return MaxNIPtrnx;
    }
    public string getStatus(string input)
    {
        string str = "<span";
        switch (input)
        {
            case "0": str += " style='color:#777;'>Pending for Name Request"; break;
            case "1": str += " style='color:#f90;'>Awaiting NIBSS Name Response"; break;
            case "2": str += " style='color:#30f;'>Ready for HOP Authorization"; break;
            case "3": str += " style='color:#c69;'>Authorized but awaiting NIBSS Response"; break;
            case "31": str += " style='color:#c69;'>Authorized but Not Sent to NIBSS"; break;
            case "32": str += " style='color:#c69;'>Authorized but Rejected by NIBSS"; break;
            case "4": str += " style='color:#0c0;'>Successful Transaction"; break;
            case "99": str += " style='color:#c69;'>Transaction Rejected"; break;
        }
        return str + "</span>";
    }

    public string getApproval(string input)
    {
        string str = "<span";
        switch (input)
        {
            case "0": str += " style='color:#777;'>PENDING"; break;
            case "1": str += " style='color:green;'>APPROVED"; break;
            case "2": str += " style='color:red;'>REJECTED"; break;
        }
        return str + "</span>";
    }
    public string getStatus2(string input)
    {
        string str = "<span";
        switch (input)
        {
            case "0": str += " style='color:orange;'>Pending"; break;
            case "1": str += " style='color:orange;'>Transaction Received and currently been processed by NIBSS.  You "; break;
            case "2": str += " style='color:green;'>You can proceed to approve transaction"; break;
            case "3": str += " style='color:orange;'>Approved transactions has been received by NIBSS and is currently been processed"; break;
            case "4": str += " style='color:orange;'>Transaction procssing was successful"; break;
        }
        return str + "</span>";
    }
    //to encrpt
    public string enkrypt(string strEnk)
    {
        SymCryptography cryptic = new SymCryptography();
        cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";

        return cryptic.Encrypt(strEnk);

    }
    //to decrypt
    public string dekrypt(string strDek)
    {
        SymCryptography cryptic = new SymCryptography();
        cryptic.Key = "wqdj~yriu!@*k0_^fa7431%p$#=@hd+&";
        return cryptic.Decrypt(strDek);
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

    public string newPaymentRef(int refid)
    {
        long k = refid + 1000000000;
        return k.ToString();
    }

    public string newTrnxRef(string bra_code)
    {
        return "232" + DateTime.Now.ToString("yyyyMMddHHmmss");
    }

    public string newSessionId(string bankcode)
    {
        Thread.Sleep(50);
        return "232" + bankcode + DateTime.Now.ToString("yyMMddHHmmss") + GenerateRndNumber(12);
    }


    public string newSessionId(string bankcode, string branchCode, int channelcode)
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
        Thread.Sleep(50);
        return "232" + bankcode + DateTime.Now.ToString("yyMMddHHmmss") + _bracode.ToString("0000") + channelcode.ToString("00") + GenerateRndNumber(6);
    }

    public string newSessionIdIBS(string bankcode, Int32 refid)
    {
        Thread.Sleep(50);
        return "232" + bankcode + DateTime.Now.ToString("yyMMddHHmmss") + refid.ToString("000000000000");
    }

    public string newRecordId(string bankcode)
    {
        Thread.Sleep(50);
        return "232" + bankcode + DateTime.Now.ToString("yyMMddHHmmss") + GenerateRndNumber(12);
    }

    public string newBatchNum(string bankcode)
    {
        Thread.Sleep(50);
        return "232" + bankcode + DateTime.Now.ToString("yyMMdd") + GenerateRndNumber(12);
    }

    public string TRUmoneyToISOmoney(decimal amt)
    {
        amt = amt * 100;
        return amt.ToString("000000000000");
    }

    public decimal ISOmoneyToTRUmoney(string amt)
    {
        decimal t = Convert.ToDecimal(amt);
        return t / 100;
    }


    public DateTime checkDate(string year, string month, string day)
    {
        //date checker
        DateTime datechecker = new DateTime();
        int y = 0;
        int m = 0;
        int d = 0;
        y = Convert.ToInt32(year);
        m = Convert.ToInt32(month);
        d = Convert.ToInt32(day);
        try
        {
            datechecker = new DateTime(y, m, d);
        }
        catch (Exception ex)
        {
            int j = DateTime.Now.Year - 90;
            datechecker = new DateTime(j, 1, 1);
        }
        return datechecker;
    }
    public DateTime checkDate(string dob)
    {
        //date checker
        DateTime datechecker = new DateTime();
        try
        {
            char[] sep = { '-' };
            string[] dt = dob.Split(sep);
            int y = Convert.ToInt32(dt[0]);
            int m = Convert.ToInt32(dt[1]);
            int d = Convert.ToInt32(dt[2]);
            datechecker = new DateTime(y, m, d);
        }
        catch (Exception ex)
        {
            int j = DateTime.Now.Year - 90;
            datechecker = new DateTime(j, 1, 1);
        }
        return datechecker;
    }

    public bool checkEmail(string emailAddress)
    {
        string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
              + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
              + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
              + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
              + @"[a-zA-Z]{2,}))$";
        Regex reStrict = new Regex(patternStrict);
        bool isStrictMatch = reStrict.IsMatch(emailAddress);
        return isStrictMatch;

    }

    public bool validatenum(string number)
    {
        bool ichar = true;
        for (int i = 0; i < number.Length; i++)
        {
            if (!char.IsNumber(number[i]))
            {
                ichar = false;
            }
        }
        return ichar;
    }
    public string printMoney(decimal amt)
    {
        return amt.ToString("#,###,###,###,##0.00");
    }
    public string printDate(DateTime dt)
    {
        string dtm = dt.ToString("MMM d, yyyy h:mm tt");
        if (dtm == "Jan 1, 0001 12:00 AM" || dtm == "Jan 1, 1900 12:00 AM")
        {
            dtm = "not set";
        }
        dtm = dtm.Replace(" 12:00 AM", "");
        return dtm;
    }
    public decimal makeMoney(string amt)
    {
        decimal mny;
        try
        {
            mny = Convert.ToDecimal(amt);
        }
        catch (Exception ex)
        {
            mny = (decimal)0;
        }
        return mny;
    }

    public bool checkFileType(string filename)
    {
        string ext = Path.GetExtension(filename).ToLower();
        string allowed = ".pdf.jpeg.jpg";
        if (allowed.Contains(ext))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //public string getStatus(string input)
    //{
    //    string str = "<span";
    //    switch (input)
    //    {
    //        case "0": str += " style='color:orange;'>PENDING"; break;
    //        case "1": str += " style='color:GREEN;'>APPROVED"; break;
    //        case "2": str += " style='color:RED;'>REJECTED"; break;
    //    }
    //    return str + "</span>";
    //}
    public string GetBankNames(string code)
    {
        string txt = "Unknown Code";
        switch (code)
        {
            case "000001": txt = "Sterling Bank"; break;
            case "000002": txt = "Keystone Bank"; break;
            case "000003": txt = "First City Monument Bank"; break;
            case "000004": txt = "United Bank for Africa"; break;
            case "000005": txt = "Diamond Bank"; break;
            case "000006": txt = "JAIZ Bank"; break;
            case "000007": txt = "Fidelity Bank"; break;
            case "000008": txt = "Skye Bank"; break;
            case "000009": txt = "Citi Bank"; break;
            case "000010": txt = "Ecobank Bank"; break;
            case "000011": txt = "Unity Bank"; break;
            case "000012": txt = "StanbicIBTC Bank"; break;
            case "000013": txt = "GTBank Plc"; break;
            case "000014": txt = "Access Bank"; break;
            case "000015": txt = "Zenith Bank"; break;
            case "000016": txt = "First Bank of Nigeria"; break;
            case "000017": txt = "Wema Bank"; break;
            case "000018": txt = "Union Bank"; break;
            case "000019": txt = "Enterprise Bank"; break;
            case "000020": txt = "Heritage"; break;
            case "000021": txt = "StandardChartered"; break;
            case "060001": txt = "Coronation"; break;
            case "070001": txt = "NPF MicroFinance Bank"; break;
            case "070002": txt = "Fortis Microfinance Bank"; break;
            case "070006": txt = "Covenant"; break;
            case "090001": txt = "ASOSavings"; break;
            case "090003": txt = "JubileeLife"; break;
            case "090004": txt = "Parralex"; break;
            case "090005": txt = "Trustbond"; break;
            case "100001": txt = "FET"; break;
            case "100002": txt = "Pagatech"; break;
            case "100003": txt = "Parkway-ReadyCash"; break;
            case "100004": txt = "Paycom"; break;
            case "100005": txt = "Cellulant"; break;
            case "100006": txt = "eTranzact"; break;
            case "100007": txt = "StanbicMobileMoney"; break;
            case "100008": txt = "EcoMobile"; break;
            case "100009": txt = "GTMobile"; break;
            case "100010": txt = "TeasyMobile"; break;
            case "100011": txt = "Mkudi"; break;
            case "100012": txt = "VTNetworks"; break;
            case "100013": txt = "AccessMobile"; break;
            case "100014": txt = "FBNMobile"; break;
            case "100015": txt = "ChamsMobile"; break;
            case "100016": txt = "FortisMobile"; break;
            case "100017": txt = "Hedonmark"; break;
            case "100018": txt = "ZenithMobile"; break;
            case "100019": txt = "Fidelity Mobile"; break;
            case "100020": txt = "MoneyBox"; break;
            case "100021": txt = "Eartholeum"; break;
            case "400001": txt = "FSDH"; break;
            case "999999": txt = "NIP Virtual Bank"; break;
        }
        return txt;
    }

    public string responseCodes(string code)
    {
        string txt = "Unknown Code";
        code = code.Trim();
        switch (code)
        {
            case "00": txt = "Approved or completed successfully"; break;
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
            case "97": txt = "Timeout waiting for response from destination"; break;
        }
        return txt;
    }
    public string RemoveSpecialChars(string str)
    {
        string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "-", "_", "(", ")", ":", "|", "[", "]" };
        for (int i = 0; i < chars.Length; i++)
        {
            if (str.Contains(chars[i]))
            {
                str = str.Replace(chars[i], " ");
            }
        }
        return str;
    }
    public string getMime(string input)
    {
        switch (input)
        {
            case ".jpg": input = "image/jpeg"; break;
            case ".jpeg": input = "image/jpeg"; break;
            case ".pdf": input = "application/pdf"; break;
            case ".gif": input = "image/gif"; break;
            case ".png": input = "image/png"; break;
            case ".tif": input = "image/tiff"; break;
            case ".tiff": input = "image/tiff"; break;
            case ".zip": input = "application/zip"; break;
            case ".zipx": input = "application/zip"; break;
        }
        return input;
    }
    public string GenerateRnd()
    {
        string[] key1 = { "b", "c", "d", "f", "g", "h", "j", "k", "m", "n", "p", "q", "r", "t", "v", "w", "x", "y", "z" };
        string[] key2 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        Random rand1 = new Random();
        string txt = "";
        //for (int i = 0; i < 7; i++)
        //    txt += key1[rand1.Next(0, 18)];
        for (int j = 0; j < 4; j++)  //the < 4 will return just for digits
            txt += key2[rand1.Next(0, 9)];
        return txt;
    }
}
