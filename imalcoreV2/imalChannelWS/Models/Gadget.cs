using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.Threading;
using System.Configuration;

namespace imalChannelWS.Models
{
    public class Gadget
    {
        public string getRealMoney(decimal amt)
        {
            string amtval = "";
            amtval = amt.ToString("#,##.00");
            return amtval;
        }
        public string formatData(string dt, string amount, string trantype)
        {

            dt = dt.Replace("-", "");
            dt = dt.Replace(" ", "");
            dt = dt.Replace(":", "");
            dt = dt.Substring(0, dt.Length - 2);
            if (amount.StartsWith("-"))
            {
                amount = amount.Substring(1);
            }
            decimal theAmt = decimal.Parse(amount);
            String amt = getRealMoney(theAmt);
            amt = amt.Replace(",", "");
            // format the amount
            String dataToReturn = dt + "|99|" + trantype + "|" + amt;
            //LOG.debug("fomatted data to return: " + dataToReturn);
            return dataToReturn;
        }
        public string BinaryToString(string binary)
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
        public String Encrypt(String val, Int32 Appid)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            try
            {
                string sql = ""; string sharedkeyval = ""; string sharedvectorval = "";
                sql = "select SharedKey, Sharedvector from tbl_applicationKey where Appid=@Appid";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqibs");
                c.SetSQL(sql);
                c.AddParam("@Appid", Appid);
                DataSet ds = c.Select("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    sharedkeyval = dr["SharedKey"].ToString();
                    sharedkeyval = BinaryToString(sharedkeyval);

                    sharedvectorval = dr["Sharedvector"].ToString();
                    sharedvectorval = BinaryToString(sharedvectorval);
                }

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
        public String Decrypt(String val, Int32 Appid)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            try
            {

                string sql = ""; string sharedkeyval = ""; string sharedvectorval = "";
                sql = "select SharedKey, Sharedvector from tbl_applicationKey where Appid=@Appid";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqibs");
                c.SetSQL(sql);
                c.AddParam("@Appid", Appid);
                DataSet ds = c.Select("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    sharedkeyval = dr["SharedKey"].ToString();
                    sharedkeyval = BinaryToString(sharedkeyval);

                    sharedvectorval = dr["Sharedvector"].ToString();
                    sharedvectorval = BinaryToString(sharedvectorval);
                }

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
        public string ImalCusName = "";
        public string getImalCustNameByNuban(string account)
        {
            string val = "";
            DataSet ds = new DataSet(); string json = "";
            //string sql = @"select long_name_eng,branch_code,cif_sub_no,currency_code,gl_code, " +
            //    " sl_no,status,cv_avail_bal,ytd_cv_bal,last_trans_date from imal.amf " +
            //             " where additional_reference='" + account + "'";
            string sql = @"select distinct c.LONG_NAME_ENG as CustomerName,d.ADDITIONAL_REFERENCE as NUBAN,'NG0020006' as BranchCode,c.ADD_STRING1,
                         c.CIF_NO as CUS_NUM,d.CURRENCY_CODE as CurrencyCode,d.STATUS as STA_CODE,d.GL_CODE as LED_CODE,d.GL_CODE as ProductCode,abs(d.CV_AVAIL_BAL) as AVAIL_BAL,d.CIF_SUB_NO
                         from imal.cif c INNER JOIN imal.cif_address ad on c.CIF_NO = ad.CIF_NO INNER JOIN imal.amf d on d.CIF_SUB_NO=ad.CIF_NO
                         and d.additional_reference='" + account + "'";
            Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
            cn.SetSQL(sql);
            try
            {
                ds = cn.Select();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    val = dr["CustomerName"].ToString() + "*" + dr["STA_CODE"].ToString() + "*" + dr["AVAIL_BAL"].ToString() +
                        "*" + dr["LED_CODE"].ToString() + "*" + dr["CurrencyCode"].ToString() + "*" + dr["ADD_STRING1"].ToString();
                    ImalCusName = dr["CustomerName"].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return val;
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

            Thread.Sleep(50);
            string nipBin = ConfigurationManager.AppSettings["NIPbin"];
            return nipBin + DateTime.Now.ToString("yyMMddHHmmss") + _bracode.ToString("0000") + channelcode.ToString("00") + GenerateRndNumber(6);
        }
        public decimal maxPerTrans = 0; public decimal maxPerday = 0;
        public bool getMaxperTransPerday(string nuban)
        {
            bool found = false;
            DataSet ds = new DataSet();
            string sql = "";
            sql = "select bra_code,cus_num,cur_code,led_code,sub_acct_code,maxpertran,maxperday,addedby,statusflag from tbl_nipconcessionTrnxlimits where statusflag=1 " +
                " and nuban= @nu";
            try
            {
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
                c.SetSQL(sql);
                c.AddParam("@nu", nuban);
                ds = c.Select("rec");
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
                found = false;
            }
            return found;
        }
        public decimal NIPfee;
        public decimal NIPvat;
        public string amtxRef = "";
        public string feetxRef = "";
        public string vattxRef = "";
        public bool getNIPFeeandVat(decimal amountToPay)
        {
            //get the fees from database and assign to variables 
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
            c.SetProcedure("spd_getNIPFeeCharge");
            c.AddParam("@amt", amountToPay);
            DataSet ds = c.Select("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                NIPfee = decimal.Parse(dr["feeAmount"].ToString());
                NIPvat = decimal.Parse(dr["vat"].ToString());
                return true;
            }
            else
            {
                return false;
            }
        }
        public string ImalDebit(NipFundsTransferReq r, string TssAcct, string Feeacct, string VatAcct, string sid, long logval)
        {
            string rspTss = ""; string rspFee = ""; string rspVat = ""; string finalResp = "";
            string rsp = "";
            string[] bits = null; decimal fee = 0; decimal vat = 0; Gadget g = new Gadget();
            string remarks = "Nip transfer from " + r.customerShowName + " to " + r.beneficiaryName + " SID " + sid;
            remarks = remarks + " " + r.paymentReference;
            remarks = remarks.Substring(0, 50);
            imalcore.ServicesSoapClient ws = new imalcore.ServicesSoapClient();
            //get fee and vat
            bool found = getNIPFeeandVat(decimal.Parse(r.amount));
            if (found)
            {
                fee = NIPfee;
                vat = NIPvat;
            }
            else
            {
                return "0D" + ":" + amtxRef + ":" + feetxRef + ":" + vattxRef; ;//unable to get vat and fee
            }
            //debit for the amount
            rspTss = ws.FundstransFer(r.fromAccount, TssAcct, decimal.Parse(r.amount), remarks);
            if (rspTss.Contains("Successful"))
            {
                //successful
                bits = rspTss.Split('*');
                amtxRef = bits[3];
                g.updateAmtTaken(amtxRef, logval);
                //proceed to take fee

                rspFee = ws.FundstransFer(r.fromAccount, Feeacct, fee, remarks);
                if (rspFee.Contains("Successful"))
                {
                    //successful
                    bits = rspFee.Split('*');
                    feetxRef = bits[3];
                    g.updateAmtTaken(feetxRef, logval);
                    //proceed to take vat
                    rspVat = ws.FundstransFer(r.fromAccount, VatAcct, vat, remarks);
                    if (rspVat.Contains("Successful"))
                    {
                        bits = rspVat.Split('*');
                        vattxRef = bits[3];
                        g.updateAmtTaken(vattxRef, logval);
                    }
                    else
                    {
                        return "0C" + ":" + amtxRef + ":" + feetxRef + ":" + vattxRef;//unable to debit for vat
                    }
                }
                else
                {
                    return "0B" + ":" + amtxRef + ":" + feetxRef + ":" + vattxRef;//unable to debit
                }
                //Everything is good
                rsp = "00" + ":" + amtxRef + ":" + feetxRef + ":" + vattxRef;
            }
            else
            {
                return "0A" + ":" + amtxRef + ":" + feetxRef + ":" + vattxRef;//unable to debit customer
            }

            return rsp;
        }
        public long ToLong(object val, long otherwise = 0)
        {
            long k = otherwise;
            try
            {
                k = Convert.ToInt64(DBClean(val.ToString()));
            }
            catch { }
            return k;
        }
        public static string DBClean(string key)
        {
            return System.Web.Security.AntiXss.AntiXssEncoder.XmlEncode(key);
        }
        public void updateAmtTaken(string amtxRef, long id)
        {
            string sql = "update tbl_nib_nip_outward set amtxRef =@ref, amtTaken = 1 where refid = @id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
            c.SetSQL(sql);
            c.AddParam("@ref", amtxRef);
            c.AddParam("@id", id);
            c.Execute();
        }
        public void updateFeeTaken(string feetxRef, long id)
        {
            string sql = "update tbl_nib_nip_outward set feetxRef =@ref, feeTaken = 1 where refid = @id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
            c.SetSQL(sql);
            c.AddParam("@ref", feetxRef);
            c.AddParam("@id", id);
            c.Execute();
        }
        public void updateVatTaken(string vattxRef, long id)
        {
            string sql = "update tbl_nib_nip_outward set vattxRef =@ref, vatTaken = 1 where refid = @id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
            c.SetSQL(sql);
            c.AddParam("@ref", vattxRef);
            c.AddParam("@id", id);
            c.Execute();
        }
        public void updateResponseCode(string rsp, long refid, string json)
        {
            string sql = "update tbl_nib_nip_outward set apprespcode =@rsp, apprespjson = @js where refid = @id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
            c.SetSQL(sql);
            c.AddParam("@rsp", rsp);
            c.AddParam("@js", json);
            c.AddParam("@id", refid);
            c.Execute();
        }
        public void updateNIPCode(string rsp, long refid)
        {
            string sql = "update tbl_nib_nip_outward set ftresponse =@rsp where refid = @id";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
            c.SetSQL(sql);
            c.AddParam("@rsp", rsp);
            c.AddParam("@id", refid);
            c.Execute();
        }
        public long SaveRequest(NipFundsTransferReq r)
        {
            long TransactId = 0;
            string sql = "insert into tbl_nib_nip_outward (sessionidNE,nuban,toAcct,destinationBankCode,channelCode, " +
                " customerShowName,paymentReference,TransType,amt,neresponse) " +
                " values(@sessionidNE,@nuban,@toAcct,@destinationBankCode,@channelCode, " +
                " @customerShowName,@paymentReference,@TransType,@amt,@neresponse)";
            //Just do an insert to db
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("conn_nip");
            cn.SetSQL(sql);
            //cn.AddParam("@sessionidNE", r.nesid);
            cn.AddParam("@sessionidNE", r.referenceCode);
            cn.AddParam("@nuban", r.fromAccount);
            cn.AddParam("@toAcct", r.toAccount);
            cn.AddParam("@destinationBankCode", r.destinationBankCode);
            cn.AddParam("@channelCode", r.channelCode);
            cn.AddParam("@customerShowName", r.customerShowName);
            cn.AddParam("@paymentReference", r.paymentReference);
            cn.AddParam("@TransType", r.beneficiaryName);
            //cn.AddParam("@beneBVN", r.beneBVN);@beneBVN,
            cn.AddParam("@amt", decimal.Parse(r.amount));
            cn.AddParam("@neresponse", r.nersp);
            try
            {
                TransactId = ToLong(cn.Insert());
            }
            catch (Exception ex)
            {
            }
            return TransactId;
        }
        public string NIPReversal(long logval, string amtxRef, string feetxRef, string vattxRef)
        {
            string sql = "insert into tbl_nib_nip_reversal (refid,amtxRef,feetxRef,vattxRef,doneby,action) " +
                                            "values(@refid,@amtxRef,@feetxRef,@vattxRef,@doneby,@action)";
            //insert into the mobile reversal table
            Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("conn_nip");
            cn.SetSQL(sql);
            cn.AddParam("@refid", logval);
            cn.AddParam("@amtxRef", amtxRef);
            cn.AddParam("@feetxRef", feetxRef);
            cn.AddParam("@vattxRef", vattxRef);
            cn.AddParam("@doneby", "AUTOSYSTEM");
            cn.AddParam("@action", "reverse");
            cn.ExecuteProc();
            return "";
        }
        public DataSet getCBNamt(int val)
        {
            string sql = "select minamt,maxamt,cus_class from tbl_nipcbnamt where cus_class = @val and statusflag=1";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
            c.SetSQL(sql);
            c.AddParam("@val", val);
            DataSet ds = c.Select("rec");
            return ds;
        }

        public ImalDetails GetImalDetailsByNuban(string nuban)
        {
            string sql = @"select * from imal.amf where ADDITIONAL_REFERENCE ='" + nuban + "'";
            Sterling.Oracle.Connect c = new Sterling.Oracle.Connect("conn_imal");
            c.SetSQL(sql);
            try
            {
                var ds = c.Select();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var customerNumber = dr["cif_sub_no"].ToString();
                    var branchcode = dr["branch_code"].ToString();
                    string sqlBvn = @"select ADD_STRING1 from imal.cif where cif_no = '" + customerNumber + "'";
                    Sterling.Oracle.Connect c2 = new Sterling.Oracle.Connect("conn_imal");
                    c2.SetSQL(sqlBvn);
                    var ds2 = c2.Select();
                    var Bvn = ds2.Tables[0].Rows[0]["ADD_STRING1"].ToString();
                    string custBal = GetRealImalBalance(dr["cv_avail_bal"].ToString().Trim());
                    return new ImalDetails
                    {
                        Status = 1,
                        BVN = Bvn,
                        CurrencyCode = dr["currency_code"].ToString(),
                        CustomerName = dr["long_name_eng"].ToString(),
                        CustId = customerNumber,
                        AvailBal = Convert.ToDouble(custBal),
                        LedgerCode = dr["gl_code"].ToString(),
                        AcctStatus = dr["status"].ToString(),
                        Branchcode = branchcode
                    };
                }

                return new ImalDetails
                {
                    Status = 0
                };
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);

                return new ImalDetails
                {
                    Status = -1,
                    CurrencyCode = "",
                    CustomerName = "",
                    CustId = "",
                    AvailBal = 0,
                    LedgerCode = "",
                    AcctStatus = ""
                };
            }
        }

        public bool validateMobileAndNuban(string nuban, string mobileNo)
        {
            bool resp = false;
            string sqlMob = @"SELECT CAD.TEL as mob1, CAD.MOBILE as mob2, CIF.TEL as mob3 FROM IMAL.AMF" +
                       " LEFT OUTER JOIN IMAL.CIF CIF ON AMF.CIF_SUB_NO = CIF.CIF_NO" +
                       " LEFT OUTER JOIN IMAL.CIF_ADDRESS CAD ON AMF.CIF_SUB_NO = CAD.CIF_NO AND CAD.DEFAULT_ADD = 1" +
                       " WHERE AMF.ADDITIONAL_REFERENCE = '" + nuban + "'";
            Sterling.Oracle.Connect c = new Sterling.Oracle.Connect("conn_imal");
            c.SetSQL(sqlMob);
            var ds = c.Select();
            var mob1 = ConvertMobile234(ds.Tables[0].Rows[0]["mob1"].ToString());
            var mob2 = ConvertMobile234(ds.Tables[0].Rows[0]["mob2"].ToString());
            var mob3 = ConvertMobile234(ds.Tables[0].Rows[0]["mob3"].ToString());

            if (mobileNo == mob1 || mobileNo == mob2 || mobileNo == mob3)
            {
                resp = true;
            }
            return resp;
        }

        public string GetRealImalBalance(string AcctBalance)
        {
            string resp = "";
            if (AcctBalance.StartsWith("-"))
            {
                resp = AcctBalance.Remove(0, 1);
            }
            else
            {
                AcctBalance = "-" + AcctBalance;
                resp = AcctBalance;
            }
            return resp;
        }
        public static string ConvertMobile234(string mobile)
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

        public string CheckForCIF(string bvn)
        {
            string cif = "";
            string sql = @"SELECT CIF.CIF_NO as CusNum, AMF.ADDITIONAL_REFERENCE FROM IMAL.CIF LEFT OUTER JOIN  IMAL.AMF ON CIF.CIF_NO = AMF.CIF_SUB_NO WHERE CIF.ADD_STRING1 = '" + bvn + "'";
            Sterling.Oracle.Connect c = new Sterling.Oracle.Connect("conn_imal");
            c.SetSQL(sql);
            var ds = c.Select();

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        cif = ds.Tables[0].Rows[0]["CusNum"].ToString();
                    }
                }
            }
            return cif;
        }
    }
}