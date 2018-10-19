using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using System.Text;

public class AccountService
{

    public string Fee_expl_code = "937";
    public string TSS_bra_code = "";
    public string TSS_cus_num = "";
    public string TSS_cur_code = "";
    public string TSS_led_code = "";
    public string TSS_sub_acct_code = "";

    public string FEE_bra_code = "";
    public string FEE_cus_num = "";
    public string FEE_cur_code = "";
    public string FEE_led_code = "";
    public string FEE_sub_acct_code = "";

    TransactionService ts = new TransactionService();

    public decimal fees_amt = 0;
    public decimal fees_amt2 = 0;
    public decimal fees_amt3 = 0;

    public bool checkConsession(Account a)
    {
        return checkConsession(a.bra_code, a.cus_num, a.cur_code, a.led_code, a.sub_acct_code);
    }

    public bool checkConsession(string bra_code, string cus_num, string cur_code, string led_code, string sub_code)
    {
        bool ok = false;

        Connect c = new Connect("spd_GetConcession");
        c.addparam("@b_code", bra_code);
        c.addparam("@c_num", cus_num);
        c.addparam("@c_code", cur_code);
        c.addparam("@l_code", led_code);
        c.addparam("@s_code", sub_code);
        DataSet ds = c.query("rec");

        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            if (Convert.ToInt16(dr["STATUSFLAG"]) == 1)
            {
                fees_amt = Convert.ToDecimal(dr["CONCESSION_AMT"]);
                fees_amt2 = Convert.ToDecimal(dr["CONCESSION_AMT2"]);
                fees_amt3 = Convert.ToDecimal(dr["CONCESSION_AMT3"]);
                ok = true;
            }
        }
        return ok;
    }

    public bool tssACCok;
    public bool feeACCok;
    public bool checkTSSOnly()
    {
        DataSet dsTss = ts.getCurrentTss();
        //assign the Tss account to the varriables
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            TSS_cus_num = drTss["cusnum"].ToString();
            TSS_cur_code = drTss["curcode"].ToString();
            TSS_led_code = drTss["ledcode"].ToString();
            TSS_sub_acct_code = drTss["subacctcode"].ToString();

            tssACCok = checkAccStatus("223", TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool checkTSSandFeesAccount(string ledger, string bra_code)
    {
        sbp.banks bank = new sbp.banks();
        DataSet ds = bank.checkOpLedger(ledger, "0");//test if income ledger exist and if it is active
        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();
        //assign the Tss account to the varriables
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            TSS_bra_code = drTss["bra_code"].ToString();
            TSS_cus_num = drTss["cusnum"].ToString();
            TSS_cur_code = drTss["curcode"].ToString();
            TSS_led_code = drTss["ledcode"].ToString();
            TSS_sub_acct_code = drTss["subacctcode"].ToString();
        }
        else
        {
            return false;
        }
        //assign the income account to the variables
        if (dsFee.Tables[0].Rows.Count > 0)
        {
            DataRow drFee = dsFee.Tables[0].Rows[0];
            FEE_cus_num = drFee["cusnum"].ToString();
            FEE_cur_code = drFee["curcode"].ToString();
            FEE_led_code = drFee["ledcode"].ToString();
            FEE_sub_acct_code = drFee["subacctcode"].ToString();
        }
        else
        {
            return false;
        }


        if (ds.Tables[0].Rows.Count > 0)
        {
            tssACCok = checkAccStatus(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code);//we need to get the nfp clearing code
            feeACCok = checkAccStatus(bra_code, FEE_cus_num, FEE_cur_code, FEE_led_code, FEE_sub_acct_code);//we neeed to get nibbs income account test            
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool checkAccStatus(string bra_code, string cus_num, string cur_code, string led_code, string sub_code)
    {
        sbp.banks bank = new sbp.banks();
        return bank.checkCusAccount(bra_code, cus_num, cur_code, led_code, sub_code);
    }

    public decimal NIPfee;
    public decimal NIPvat;
    public bool getNIPFee(decimal amountToPay)
    {
        //get the fees from database and assign to variables 
        DataSet ds = ts.getNIPFee(amountToPay);
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

    public decimal calculateFee(decimal amountToPay)
    {
        //get the fees from database and assign to variables 
        DataSet ds = ts.getTransFee();
        if (ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            fees_amt = decimal.Parse(dr["amt1"].ToString());
            fees_amt2 = decimal.Parse(dr["amt2"].ToString());
            fees_amt3 = decimal.Parse(dr["amt3"].ToString());
        }
        else
        {
        }

        decimal feecharge;
        if (amountToPay > 0 && amountToPay < (decimal)1000000)
        {
            feecharge = fees_amt; //for transactions less 1 million
        }
        else if (amountToPay < (decimal)10000000)
        {
            feecharge = fees_amt2; //for transactions less  10 million
        }
        else
        {
            feecharge = fees_amt3; //for transactions 10 Million and above
        }
        return feecharge;
    }

    public decimal thelimitval;
    public bool checkTellerLimit(string bra_code, string Tellerid, decimal totalamttopay)
    {
        sbp.banks bank = new sbp.banks();
        DataSet ds = bank.getTellerLimit(Tellerid, bra_code);
        bool ok = false;
        if (ds.Tables[0].Rows.Count > 0)
        {
            thelimitval = Convert.ToDecimal(ds.Tables[0].Rows[0]["aut_amt"]);
            if (thelimitval >= totalamttopay)
            {
                ok = true;
            }
        }
        return ok;
    }

    public string tra_seq;
    public string RespCreditedamt;
    public string Respreturnedcode1;
    public string Respreturnedcode2;

    public string Prin_Rsp;
    public string Fee_Rsp;
    public string Vat_Rsp;

    public string ResponseMsg;
    public void authorizeTrnxFromSterling(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";

        string expl_code = "";
        Gadget g = new Gadget();

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }

        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();
        //assign the Tss account to the varriables
        //if (dsTss.Tables[0].Rows.Count > 0)
        //{
        //    DataRow drTss = dsTss.Tables[0].Rows[0];
        //    TSS_bra_code = drTss["bra_code"].ToString();
        //    TSS_cus_num = drTss["cusnum"].ToString();
        //    TSS_cur_code = drTss["curcode"].ToString();
        //    TSS_led_code = drTss["ledcode"].ToString();
        //    TSS_sub_acct_code = drTss["subacctcode"].ToString();
        //}
        ////assign the income account to the variables
        //if (dsFee.Tables[0].Rows.Count > 0)
        //{
        //    DataRow drFee = dsFee.Tables[0].Rows[0];
        //    FEE_bra_code = drFee["bra_code"].ToString();
        //    FEE_cus_num = drFee["cusnum"].ToString();
        //    FEE_cur_code = drFee["curcode"].ToString();
        //    FEE_led_code = drFee["ledcode"].ToString();
        //    FEE_sub_acct_code = drFee["subacctcode"].ToString();
        //}
        //if (t.inCust.bra_code == "201" || t.inCust.bra_code == "223" || t.inCust.bra_code == "259" || t.inCust.bra_code == "257" || t.inCust.bra_code == "213" || t.inCust.bra_code == "242" || t.inCust.bra_code == "226" || t.inCust.bra_code == "260" || t.inCust.bra_code == "229" || t.inCust.bra_code == "211")
        if (t.inCust.bra_code == "201" || t.inCust.bra_code == "223" || t.inCust.bra_code == "259" || t.inCust.bra_code == "257" ||
                t.inCust.bra_code == "213" || t.inCust.bra_code == "242" || t.inCust.bra_code == "226" || t.inCust.bra_code == "260" ||
                t.inCust.bra_code == "229" || t.inCust.bra_code == "211" || t.inCust.bra_code == "202" || t.inCust.bra_code == "204" ||
                t.inCust.bra_code == "205" || t.inCust.bra_code == "206" || t.inCust.bra_code == "207" || t.inCust.bra_code == "208" ||
                t.inCust.bra_code == "209" || t.inCust.bra_code == "210" || t.inCust.bra_code == "212" || t.inCust.bra_code == "214" ||
                t.inCust.bra_code == "215" || t.inCust.bra_code == "216" || t.inCust.bra_code == "217" || t.inCust.bra_code == "218" ||
                t.inCust.bra_code == "219" || t.inCust.bra_code == "220" || t.inCust.bra_code == "221" || t.inCust.bra_code == "222" ||
                t.inCust.bra_code == "225" || t.inCust.bra_code == "227" || t.inCust.bra_code == "228" || t.inCust.bra_code == "230" ||
                t.inCust.bra_code == "231" || t.inCust.bra_code == "232" || t.inCust.bra_code == "234" || t.inCust.bra_code == "235" ||
                t.inCust.bra_code == "236" || t.inCust.bra_code == "237" || t.inCust.bra_code == "238" || t.inCust.bra_code == "240" ||
                t.inCust.bra_code == "241" || t.inCust.bra_code == "243" || t.inCust.bra_code == "244" || t.inCust.bra_code == "245" ||
                t.inCust.bra_code == "246" || t.inCust.bra_code == "247" || t.inCust.bra_code == "248" || t.inCust.bra_code == "250" ||
                t.inCust.bra_code == "252" || t.inCust.bra_code == "253" || t.inCust.bra_code == "254" || t.inCust.bra_code == "256" ||
                t.inCust.bra_code == "262" || t.inCust.bra_code == "262" || t.inCust.bra_code == "264" || t.inCust.bra_code == "265" ||
                t.inCust.bra_code == "267" || t.inCust.bra_code == "268" || t.inCust.bra_code == "271" || t.inCust.bra_code == "274" ||
                t.inCust.bra_code == "276" || t.inCust.bra_code == "279" || t.inCust.bra_code == "280" || t.inCust.bra_code == "285" ||
                t.inCust.bra_code == "286" || t.inCust.bra_code == "287" || t.inCust.bra_code == "288" || t.inCust.bra_code == "289" ||
                t.inCust.bra_code == "290" || t.inCust.bra_code == "291" || t.inCust.bra_code == "292" || t.inCust.bra_code == "293" ||
                t.inCust.bra_code == "294" || t.inCust.bra_code == "295" || t.inCust.bra_code == "296" ||

                t.inCust.bra_code == "301" || t.inCust.bra_code == "302" || t.inCust.bra_code == "303" || t.inCust.bra_code == "304" ||
                t.inCust.bra_code == "305" || t.inCust.bra_code == "306" || t.inCust.bra_code == "307" || t.inCust.bra_code == "308" ||
                t.inCust.bra_code == "309" || t.inCust.bra_code == "310" || t.inCust.bra_code == "311" || t.inCust.bra_code == "312" ||
                t.inCust.bra_code == "313" || t.inCust.bra_code == "314" || t.inCust.bra_code == "316" || t.inCust.bra_code == "317" ||
                t.inCust.bra_code == "318" || t.inCust.bra_code == "319" || t.inCust.bra_code == "320" || t.inCust.bra_code == "321" ||
                t.inCust.bra_code == "322" || t.inCust.bra_code == "323" || t.inCust.bra_code == "324" || t.inCust.bra_code == "325" ||
                t.inCust.bra_code == "326" || t.inCust.bra_code == "328" || t.inCust.bra_code == "329" || t.inCust.bra_code == "330" ||
                t.inCust.bra_code == "331" || t.inCust.bra_code == "332" || t.inCust.bra_code == "334" || t.inCust.bra_code == "338" ||
                t.inCust.bra_code == "340" || t.inCust.bra_code == "341" || t.inCust.bra_code == "342" || t.inCust.bra_code == "343" ||
                t.inCust.bra_code == "344" || t.inCust.bra_code == "345" || t.inCust.bra_code == "401" || t.inCust.bra_code == "402" ||
                t.inCust.bra_code == "403" || t.inCust.bra_code == "404" || t.inCust.bra_code == "405" || t.inCust.bra_code == "406" ||
                t.inCust.bra_code == "408" || t.inCust.bra_code == "409" || t.inCust.bra_code == "410" || t.inCust.bra_code == "411" ||
                t.inCust.bra_code == "413" || t.inCust.bra_code == "416" || t.inCust.bra_code == "417" || t.inCust.bra_code == "418" ||
                t.inCust.bra_code == "419" || t.inCust.bra_code == "420" || t.inCust.bra_code == "422" || t.inCust.bra_code == "423" ||
                t.inCust.bra_code == "424" || t.inCust.bra_code == "425" || t.inCust.bra_code == "503" || t.inCust.bra_code == "504" ||
                t.inCust.bra_code == "506" || t.inCust.bra_code == "507" || t.inCust.bra_code == "508" || t.inCust.bra_code == "509" ||
                t.inCust.bra_code == "510" || t.inCust.bra_code == "511" || t.inCust.bra_code == "512" || t.inCust.bra_code == "513" ||
                t.inCust.bra_code == "514" || t.inCust.bra_code == "515" || t.inCust.bra_code == "516" || t.inCust.bra_code == "517" ||
                t.inCust.bra_code == "518" || t.inCust.bra_code == "519" || t.inCust.bra_code == "520" || t.inCust.bra_code == "521" ||
                t.inCust.bra_code == "522" || t.inCust.bra_code == "523" || t.inCust.bra_code == "524" || t.inCust.bra_code == "525" ||
                t.inCust.bra_code == "526" || t.inCust.bra_code == "528" || t.inCust.bra_code == "531" || t.inCust.bra_code == "532" ||
                t.inCust.bra_code == "533" || t.inCust.bra_code == "534" || t.inCust.bra_code == "535" || t.inCust.bra_code == "536" ||
                t.inCust.bra_code == "537" || t.inCust.bra_code == "538" || t.inCust.bra_code == "539" || t.inCust.bra_code == "540" ||
                t.inCust.bra_code == "541" || t.inCust.bra_code == "542" || t.inCust.bra_code == "544" || t.inCust.bra_code == "545" ||
                t.inCust.bra_code == "546" || t.inCust.bra_code == "547" || t.inCust.bra_code == "548" || t.inCust.bra_code == "902" ||
                t.inCust.bra_code == "903")
        {
            //get TSS account based on the branch code of the transacting account
            DataSet dsTss1 = ts.getCurrentTss1();
            if (dsTss1.Tables[0].Rows.Count > 0)
            {
                DataRow drTss1 = dsTss1.Tables[0].Rows[0];
                TSS_bra_code = t.inCust.bra_code;
                TSS_cus_num = drTss1["cusnum"].ToString();
                TSS_cur_code = drTss1["curcode"].ToString();
                TSS_led_code = drTss1["ledcode"].ToString();
                TSS_sub_acct_code = drTss1["subacctcode"].ToString();
            }
        }
        else
        {
            if (dsTss.Tables[0].Rows.Count > 0)
            {
                DataRow drTss = dsTss.Tables[0].Rows[0];
                TSS_bra_code = drTss["bra_code"].ToString();
                TSS_cus_num = drTss["cusnum"].ToString();
                TSS_cur_code = drTss["curcode"].ToString();
                TSS_led_code = drTss["ledcode"].ToString();
                TSS_sub_acct_code = drTss["subacctcode"].ToString();
            }
        }
        //assign the income account to the variables
        //if (t.inCust.bra_code == "201" || t.inCust.bra_code == "223" || t.inCust.bra_code == "259" || t.inCust.bra_code == "257" || t.inCust.bra_code == "213" || t.inCust.bra_code == "242" || t.inCust.bra_code == "226" || t.inCust.bra_code == "260" || t.inCust.bra_code == "229" || t.inCust.bra_code == "211")
        if (t.inCust.bra_code == "201" || t.inCust.bra_code == "223" || t.inCust.bra_code == "259" || t.inCust.bra_code == "257" ||
                t.inCust.bra_code == "213" || t.inCust.bra_code == "242" || t.inCust.bra_code == "226" || t.inCust.bra_code == "260" ||
                t.inCust.bra_code == "229" || t.inCust.bra_code == "211" || t.inCust.bra_code == "202" || t.inCust.bra_code == "204" ||
                t.inCust.bra_code == "205" || t.inCust.bra_code == "206" || t.inCust.bra_code == "207" || t.inCust.bra_code == "208" ||
                t.inCust.bra_code == "209" || t.inCust.bra_code == "210" || t.inCust.bra_code == "212" || t.inCust.bra_code == "214" ||
                t.inCust.bra_code == "215" || t.inCust.bra_code == "216" || t.inCust.bra_code == "217" || t.inCust.bra_code == "218" ||
                t.inCust.bra_code == "219" || t.inCust.bra_code == "220" || t.inCust.bra_code == "221" || t.inCust.bra_code == "222" ||
                t.inCust.bra_code == "225" || t.inCust.bra_code == "227" || t.inCust.bra_code == "228" || t.inCust.bra_code == "230" ||
                t.inCust.bra_code == "231" || t.inCust.bra_code == "232" || t.inCust.bra_code == "234" || t.inCust.bra_code == "235" ||
                t.inCust.bra_code == "236" || t.inCust.bra_code == "237" || t.inCust.bra_code == "238" || t.inCust.bra_code == "240" ||
                t.inCust.bra_code == "241" || t.inCust.bra_code == "243" || t.inCust.bra_code == "244" || t.inCust.bra_code == "245" ||
                t.inCust.bra_code == "246" || t.inCust.bra_code == "247" || t.inCust.bra_code == "248" || t.inCust.bra_code == "250" ||
                t.inCust.bra_code == "252" || t.inCust.bra_code == "253" || t.inCust.bra_code == "254" || t.inCust.bra_code == "256" ||
                t.inCust.bra_code == "262" || t.inCust.bra_code == "262" || t.inCust.bra_code == "264" || t.inCust.bra_code == "265" ||
                t.inCust.bra_code == "267" || t.inCust.bra_code == "268" || t.inCust.bra_code == "271" || t.inCust.bra_code == "274" ||
                t.inCust.bra_code == "276" || t.inCust.bra_code == "279" || t.inCust.bra_code == "280" || t.inCust.bra_code == "285" ||
                t.inCust.bra_code == "286" || t.inCust.bra_code == "287" || t.inCust.bra_code == "288" || t.inCust.bra_code == "289" ||
                t.inCust.bra_code == "290" || t.inCust.bra_code == "291" || t.inCust.bra_code == "292" || t.inCust.bra_code == "293" ||
                t.inCust.bra_code == "294" || t.inCust.bra_code == "295" || t.inCust.bra_code == "296" ||

                t.inCust.bra_code == "301" || t.inCust.bra_code == "302" || t.inCust.bra_code == "303" || t.inCust.bra_code == "304" ||
                t.inCust.bra_code == "305" || t.inCust.bra_code == "306" || t.inCust.bra_code == "307" || t.inCust.bra_code == "308" ||
                t.inCust.bra_code == "309" || t.inCust.bra_code == "310" || t.inCust.bra_code == "311" || t.inCust.bra_code == "312" ||
                t.inCust.bra_code == "313" || t.inCust.bra_code == "314" || t.inCust.bra_code == "316" || t.inCust.bra_code == "317" ||
                t.inCust.bra_code == "318" || t.inCust.bra_code == "319" || t.inCust.bra_code == "320" || t.inCust.bra_code == "321" ||
                t.inCust.bra_code == "322" || t.inCust.bra_code == "323" || t.inCust.bra_code == "324" || t.inCust.bra_code == "325" ||
                t.inCust.bra_code == "326" || t.inCust.bra_code == "328" || t.inCust.bra_code == "329" || t.inCust.bra_code == "330" ||
                t.inCust.bra_code == "331" || t.inCust.bra_code == "332" || t.inCust.bra_code == "334" || t.inCust.bra_code == "338" ||
                t.inCust.bra_code == "340" || t.inCust.bra_code == "341" || t.inCust.bra_code == "342" || t.inCust.bra_code == "343" ||
                t.inCust.bra_code == "344" || t.inCust.bra_code == "345" || t.inCust.bra_code == "401" || t.inCust.bra_code == "402" ||
                t.inCust.bra_code == "403" || t.inCust.bra_code == "404" || t.inCust.bra_code == "405" || t.inCust.bra_code == "406" ||
                t.inCust.bra_code == "408" || t.inCust.bra_code == "409" || t.inCust.bra_code == "410" || t.inCust.bra_code == "411" ||
                t.inCust.bra_code == "413" || t.inCust.bra_code == "416" || t.inCust.bra_code == "417" || t.inCust.bra_code == "418" ||
                t.inCust.bra_code == "419" || t.inCust.bra_code == "420" || t.inCust.bra_code == "422" || t.inCust.bra_code == "423" ||
                t.inCust.bra_code == "424" || t.inCust.bra_code == "425" || t.inCust.bra_code == "503" || t.inCust.bra_code == "504" ||
                t.inCust.bra_code == "506" || t.inCust.bra_code == "507" || t.inCust.bra_code == "508" || t.inCust.bra_code == "509" ||
                t.inCust.bra_code == "510" || t.inCust.bra_code == "511" || t.inCust.bra_code == "512" || t.inCust.bra_code == "513" ||
                t.inCust.bra_code == "514" || t.inCust.bra_code == "515" || t.inCust.bra_code == "516" || t.inCust.bra_code == "517" ||
                t.inCust.bra_code == "518" || t.inCust.bra_code == "519" || t.inCust.bra_code == "520" || t.inCust.bra_code == "521" ||
                t.inCust.bra_code == "522" || t.inCust.bra_code == "523" || t.inCust.bra_code == "524" || t.inCust.bra_code == "525" ||
                t.inCust.bra_code == "526" || t.inCust.bra_code == "528" || t.inCust.bra_code == "531" || t.inCust.bra_code == "532" ||
                t.inCust.bra_code == "533" || t.inCust.bra_code == "534" || t.inCust.bra_code == "535" || t.inCust.bra_code == "536" ||
                t.inCust.bra_code == "537" || t.inCust.bra_code == "538" || t.inCust.bra_code == "539" || t.inCust.bra_code == "540" ||
                t.inCust.bra_code == "541" || t.inCust.bra_code == "542" || t.inCust.bra_code == "544" || t.inCust.bra_code == "545" ||
                t.inCust.bra_code == "546" || t.inCust.bra_code == "547" || t.inCust.bra_code == "548" || t.inCust.bra_code == "902" ||
                t.inCust.bra_code == "903")
        {
            //900/0/1/8710/0 
            FEE_bra_code = t.inCust.bra_code;
            FEE_cus_num = "0";
            FEE_cur_code = "1";
            FEE_led_code = "8710";
            FEE_sub_acct_code = "0";
        }
        else
        {
            if (dsFee.Tables[0].Rows.Count > 0)
            {
                DataRow drFee = dsFee.Tables[0].Rows[0];
                FEE_bra_code = drFee["bra_code"].ToString();
                FEE_cus_num = drFee["cusnum"].ToString();
                FEE_cur_code = drFee["curcode"].ToString();
                FEE_led_code = drFee["ledcode"].ToString();
                FEE_sub_acct_code = drFee["subacctcode"].ToString();
            }
        }
        string xrem = t.sessionid + "/" + t.origin_branch + "/" + t.inCust.cus_sho_name + "/" +
            t.destinationcode + "/" + t.outCust.cusname;


        xrem = xrem.Replace("&amp;", "&");
        xrem = xrem.Replace("&apos;", "'");
        xrem = xrem.Replace("&quot;", "\"");

        xrem = xrem.Replace("& ", "&amp;");
        xrem = xrem.Replace("'", "&apos;");
        xrem = xrem.Replace("\"", "&quot;");



        string xremark1 = "NIP/" + xrem;
        string xremark2 = "NIPFEE/" + xrem;
        string xremark3 = "NIPVAT/" + xrem;

        XMLGenerator xg = new XMLGenerator(t.tellerID);

        xg.startDebit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.vat.ToString(), Fee_expl_code, xremark3);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(FEE_bra_code, FEE_cus_num, FEE_cur_code, FEE_led_code, FEE_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        //xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.VAT_bra_code, t.VAT_cus_num, t.VAT_cur_code, t.VAT_led_code, t.VAT_sub_acct_code, t.vat.ToString(), Fee_expl_code, xremark3);
        xg.endCredit();

        xg.closeXML();

        //uncomment this part letter
        try
        {
            vteller.nfp vs = new vteller.nfp();
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returened values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
            string error_text = xg.debits[0]["err_text"].InnerText;

            StringBuilder resp = new StringBuilder();
            resp.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            resp.Append("<intTrnxResp>");
            resp.Append("<totaldebit>" + xg.credits[0]["amount"].InnerText + "</totaldebit>");
            resp.Append("<remark>" + xremark1 + "</remark>");
            resp.Append("<principal>");
            resp.Append("<acc_num>");
            resp.Append(t.inCust.bra_code + TSS_cus_num + TSS_cur_code + TSS_led_code + TSS_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[0]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[0]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[0]["err_text"].InnerText + "</responseText>");
            resp.Append("</principal>");
            resp.Append("<fee>");
            resp.Append("<acc_num>");
            resp.Append(t.inCust.bra_code + FEE_cus_num + FEE_cur_code + FEE_led_code + FEE_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[1]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[1]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[1]["err_text"].InnerText + "</responseText>");
            resp.Append("</fee>");
            resp.Append("<vat>");
            resp.Append("<acc_num>");
            resp.Append(t.VAT_bra_code + t.VAT_cus_num + t.VAT_cur_code + t.VAT_led_code + t.VAT_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[2]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[2]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[2]["err_text"].InnerText + "</responseText>");
            resp.Append("</vat>");
            resp.Append("</intTrnxResp>");
            ResponseMsg = resp.ToString();

        }
        catch (Exception ex)
        {
            new ErrorLog("Unable to debit customer account " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " for amount " + t.amount.ToString() + " and fee charge " + t.feecharge.ToString() + ex);
            Respreturnedcode1 = "1x";
        }
    }

    //Bank Teller
    public void authorizeBankTellerTrnxFromSterling(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";

        string expl_code = "";
        Gadget g = new Gadget();
        sbp.banks b1 = new sbp.banks();

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }

        string TSSAcct = ""; int Last4 = 0;
        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();
        //assign the Tss account to the varriables
        
        bool foundval = g.isBankCodeFound(t.inCust.bra_code);
        if (foundval)
        {
            //get TSS account based on the branch code of the transacting account
            DataSet dsTss1 = ts.getCurrentTss1();
            if (dsTss1.Tables[0].Rows.Count > 0)
            {
                DataRow drTss1 = dsTss1.Tables[0].Rows[0];
                TSS_bra_code = "NG0020001";// t.inCust.bra_code;
                TSS_cus_num = "";
                TSS_cur_code = "NGN";
                //TSS_led_code = drTss1["ledcode"].ToString(); //UNCOMMENT FOR LIVE
                TSS_led_code = "12501";
                Last4 = int.Parse(TSS_bra_code.Substring(6,3)) + 2000;
                TSSAcct = "NGN" + TSS_led_code + "0001" + Last4.ToString();
                TSS_sub_acct_code = TSSAcct;
                
                //TSS_sub_acct_code = b1.FormInternalAcct(TSS_bra_code, "NGN", TSS_led_code);
                new ErrorLog("TSS account formed for branch " + TSS_bra_code + " is " + TSS_sub_acct_code);
            }
        }
        else
        {
            if (dsTss.Tables[0].Rows.Count > 0)
            {
                DataRow drTss = dsTss.Tables[0].Rows[0];
                TSS_bra_code = "NG0020001";// drTss["bra_code"].ToString();
                TSS_cus_num = "";
                TSS_cur_code = "NGN";
                TSS_led_code = "12501"; 
                Last4 = int.Parse(TSS_bra_code.Substring(6, 3)) + 2000;
                TSSAcct = "NGN" + TSS_led_code + "0001" + Last4.ToString();
                TSS_sub_acct_code = TSSAcct;
                //TSS_sub_acct_code = b1.FormInternalAcct(TSS_bra_code, "NGN", TSS_led_code);
                new ErrorLog("TSS account formed for branch " + TSS_bra_code + " is " + TSS_sub_acct_code);
            }
        }

        //assign the income account to the variables
        bool foundval1 = g.isBankCodeFound(t.inCust.bra_code);
        if (foundval1)
        {
            if (dsFee.Tables[0].Rows.Count > 0)
            {
                DataRow drFee = dsFee.Tables[0].Rows[0];
                FEE_bra_code = "NG0020001";// t.inCust.bra_code;// drFee["bra_code"].ToString();
                FEE_cus_num = drFee["cusnum"].ToString();
                FEE_cur_code = drFee["curcode"].ToString();
                FEE_led_code = drFee["ledcode"].ToString();
                FEE_sub_acct_code = drFee["subacctcode"].ToString();

                //FEE_bra_code = t.inCust.bra_code;
                FEE_bra_code = "NG0020001";
                FEE_cus_num = "";
                FEE_cur_code = "NGN";
                FEE_led_code = "52315";
                FEE_sub_acct_code = "0";
                //Last4 = int.Parse(FEE_bra_code.Substring(6, 3)) + 2000;
                TSSAcct = "PL52315";
                FEE_sub_acct_code = TSSAcct;
            }
        }
        else
        {
            if (dsFee.Tables[0].Rows.Count > 0)
            {
                DataRow drFee = dsFee.Tables[0].Rows[0];
                FEE_bra_code = drFee["bra_code"].ToString();
                FEE_cus_num = drFee["cusnum"].ToString();
                FEE_cur_code = drFee["curcode"].ToString();
                FEE_led_code = drFee["ledcode"].ToString();
                FEE_sub_acct_code = drFee["subacctcode"].ToString();

                FEE_bra_code = "NG0020001";// t.inCust.bra_code;//REM FOR BRANCH
                FEE_cus_num = "";
                FEE_cur_code = "NGN";
                FEE_led_code = "52315";
                FEE_sub_acct_code = "0";
                //Last4 = int.Parse(FEE_bra_code.Substring(6, 3)) + 2000;
                TSSAcct = "PL52315";
                FEE_sub_acct_code = TSSAcct;
            }
        }

        string TheCusShowName = ""; string TheAccountName = "";
        //t.inCust.cus_sho_name = System.Text.RegularExpressions.Regex.Replace(t.inCust.cus_sho_name, @"\s+", " ");
        string xrem = t.sessionid + "/" + t.origin_branch  +"/" + t.inCust.cus_sho_name + "/" +
            t.destinationcode + "/" + t.outCust.cusname + "/" +
            " TellerID " + t.tellerID + "/" + t.paymentRef;

        //split the from name and ensure we have just two names
        //TheCusShowName = System.Text.RegularExpressions.Regex.Replace(t.inCust.cus_sho_name, @"\s+", " ");
        //string[] bits = TheCusShowName.Split(' ');
        //TheCusShowName = bits[0] + " " + bits[1].Substring(0, 1).ToUpper();

        //TheAccountName = System.Text.RegularExpressions.Regex.Replace(t.AccountName, @"\s+", " ");
        //string[] bits1 = TheAccountName.Split(' ');
        //TheAccountName = bits1[0] + " " + bits1[1].Substring(0, 1).ToUpper();

        //string FormRmks = "";
        //FormRmks = "SBPFROM:" + "232:" + "STERLING BANK PLC:" + t.inCust.sub_acct_code + ":NGN:" + t.inCust.cus_sho_name + ":" + t.inCust.cus_sho_name + ":" + t.AccountName + ":" + t.origin_branch + ":" + t.sessionid + "SBP" +
          //   "SBPTO:" + t.destinationcode + ":" + g.getNIPBankName(t.destinationcode) + ":" + t.AccountNumber + ":NGN:" + g.getNIPBankName(t.destinationcode) + ":" + t.inCust.cus_sho_name + ":" + t.AccountName + "SBP";
        //232232:SBN:0005969437:NGN:CHIGOZIE A:NG0020007:999232160224131902022901765402|000001:GTB:0005969437:NGN:CHIGOIE A:12344444

        //FormRmks = "000001:SBN:" + t.inCust.sub_acct_code + ":NGN:" + t.inCust.cus_sho_name + ":" + t.origin_branch + ":" + t.sessionid + "/" + t.destinationcode + ":" + g.getNIPBankName(t.destinationcode) + ":" + t.AccountNumber + ":NGN:" + t.AccountName + ":" + t.destinationcode;
        //for SAS
        //FormRmks = t.sessionid + ":" + "000001:SBN:" + t.inCust.sub_acct_code + ":NGN:" + TheCusShowName + ":000001" + t.origin_branch + "/" + t.destinationcode + ":" + g.getNIPBankName(t.destinationcode) + ":" + t.AccountNumber + ":NGN:" + TheAccountName  + ":" + t.destinationcode;

        //xrem = FormRmks;

        if (xrem.Length > 136)
        {
            xrem = xrem.Substring(0, 136); //banks colum takes 200 max
        }

        xrem = xrem.Replace("&amp;", "&");
        xrem = xrem.Replace("&apos;", "'");
        xrem = xrem.Replace("&quot;", "\"");

        xrem = xrem.Replace("&", "&amp;");
        xrem = xrem.Replace("'", "&apos;");
        xrem = xrem.Replace("\"", "&quot;");



        string xremark1 = "NIP/" + g.RemoveSpecialChars(xrem);
        string xremark2 = "NIPFEE/" + g.RemoveSpecialChars(xrem);
        string xremark3 = "NIPVAT/" + g.RemoveSpecialChars(xrem);
        //ensure the remarks are not more than 136
        if (xremark1.Length > 136)
        {
            xremark1 = xremark1.Substring(0, 136);
        }

        if (xremark2.Length > 136)
        {
            xremark2 = xremark2.Substring(0, 136);
        }

        if (xremark3.Length > 136)
        {
            xremark3 = xremark3.Substring(0, 136);
        }

        XMLGenerator xg = new XMLGenerator(t.tellerID);
        xg.startDebit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.vat.ToString(), Fee_expl_code, xremark3);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(FEE_bra_code, FEE_cus_num, FEE_cur_code, FEE_led_code, FEE_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.VAT_bra_code, t.VAT_cus_num, t.VAT_cur_code, t.VAT_led_code, t.VAT_sub_acct_code, t.vat.ToString(), Fee_expl_code, xremark3);
        xg.endCredit();

        xg.closeXML();

        try
        {
            vteller.nfp vs = new vteller.nfp();
            //vs.Timeout = 40000;
              vs.Timeout = 400000;
            //xg.resp = vs.NIBBS(xg.req, "NIBSS Transfer from Sterling " + t.Refid);
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returened values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
            string error_text = xg.debits[0]["err_text"].InnerText;

            Prin_Rsp = xg.debits[0].ChildNodes[7].InnerText;
            Fee_Rsp = xg.debits[1].ChildNodes[7].InnerText;
            Vat_Rsp = xg.debits[2].ChildNodes[7].InnerText;

            StringBuilder resp = new StringBuilder();
            resp.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            resp.Append("<intTrnxResp>");
            resp.Append("<totaldebit>" + xg.credits[0]["amount"].InnerText + "</totaldebit>");
            resp.Append("<remark>" + xremark1 + "</remark>");
            resp.Append("<principal>");
            resp.Append("<acc_num>");
            resp.Append(t.inCust.bra_code + TSS_cus_num + TSS_cur_code + TSS_led_code + TSS_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[0]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[0]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[0]["err_text"].InnerText + "</responseText>");
            resp.Append("</principal>");
            resp.Append("<fee>");
            resp.Append("<acc_num>");
            resp.Append(t.inCust.bra_code + FEE_cus_num + FEE_cur_code + FEE_led_code + FEE_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[1]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[1]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[1]["err_text"].InnerText + "</responseText>");
            resp.Append("</fee>");
            resp.Append("</intTrnxResp>");
            ResponseMsg = resp.ToString();

        }
        catch (Exception ex)
        {
            new ErrorLog("Unable to debit customer account " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " for amount " + t.amount.ToString() + " and fee charge " + t.feecharge.ToString() + ex);
            Respreturnedcode1 = "1x";
        }
    }

    //added by ololade money 01052018
    internal void DoImalDebit(Transaction t, string principalAcct, string FeeAcct, string VatAcct)
    {

        ImalVteller.Services vtel = new ImalVteller.Services();
 
        string respMain = "";
        string respFee = "";
        string respVat = "";
        string appender = " :response for imal nip for sessionid " + t.sessionid + " and refid " + t.Refid;

        t.Remark = t.sessionid + "/" + t.inCust.cus_sho_name + "/" + t.outCust.cusname + "/" + t.paymentRef;
        try
        {
            Mylogger1.Info("about to call imal debit credit web service for nuban " + t.nuban + " and sessionid " + t.sessionid+" and pass debit of principal amt of "+t.amount.ToString()+" to credit acct "+principalAcct);
            respMain = vtel.FundstransFer(t.nuban, principalAcct, t.amount, "ImalNIPPrincipal " + t.Remark);
        }
        catch (Exception ex)
        {
            Mylogger1.Info(ex.ToString());
        }


        if (respMain.Contains("00*Successful"))
        {
            new ErrorLog("result from imal vteller for principal is "+respMain + appender + ". remark formed is " + t.Remark);
            Mylogger1.Info("result from imal vteller for principal is " + respMain + appender + ". remark formed is " + t.Remark);

            Prin_Rsp = respMain;
            Mylogger1.Info("about to call imal debit credit web service for nuban " + t.nuban + " and sessionid " + t.sessionid + " and pass debit of fee amt of " + t.feecharge.ToString() + " to credit acct " + FeeAcct);
            respFee = vtel.FundstransFer(t.nuban, FeeAcct, t.feecharge, "ImalNIPFee " + t.Remark);
            Fee_Rsp = respFee;
            new ErrorLog("result from imal vteller for fee debit is " + respFee + appender + ". remark formed is " + t.Remark);

            Mylogger1.Info("about to call imal debit credit web service for nuban " + t.nuban + " and sessionid " + t.sessionid + " and pass debit of VAT amt of " + t.vat.ToString() + " to credit acct " + VatAcct);
            respVat = vtel.FundstransFer(t.nuban, VatAcct, t.vat, "ImalNIPVat " + t.Remark);
            Vat_Rsp = respVat;
            new ErrorLog("result from imal vteller for fee vat is " + respVat + appender + ". remark formed is " + t.Remark);
            Respreturnedcode1 = "0";

            //update iMal response on successful
            TransactionService tsv = new TransactionService();
            tsv.UpdateT24FTResponse(Prin_Rsp, Fee_Rsp, Vat_Rsp, t.Refid);
        }
        else
        {
            new ErrorLog("debiting the principal unsuccessful "+respMain);
            Mylogger1.Info(respMain);
            Respreturnedcode1 = respMain;
        }

    }

    //ibs
    public string error_text;
    public void authorizeIBSTrnxFromSterling(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";
        sbp.banks b1 = new sbp.banks();
        string expl_code = "";
        Gadget g = new Gadget();

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }

        string TSSAcct = ""; int Last4 = 0;
        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();
        //assign the Tss account to the varriables

        bool foundval = g.isBankCodeFound(t.inCust.bra_code);
        if (foundval)
        {
            //get TSS account based on the branch code of the transacting account
            DataSet dsTss1 = ts.getCurrentTss1();
            if (dsTss1.Tables[0].Rows.Count > 0)
            {
                DataRow drTss1 = dsTss1.Tables[0].Rows[0];
                TSS_bra_code = "NG0020001";// t.inCust.bra_code;
                TSS_cus_num = "";
                TSS_cur_code = "NGN";
                //TSS_led_code = drTss1["ledcode"].ToString(); //UNCOMMENT FOR LIVE
                TSS_led_code = "12501";
                Last4 = int.Parse(TSS_bra_code.Substring(6, 3)) + 2000;
                TSSAcct = "NGN" + TSS_led_code + "0001" + Last4.ToString();
                TSS_sub_acct_code = TSSAcct; 
                //TSS_sub_acct_code = b1.FormInternalAcct(TSS_bra_code, "NGN", TSS_led_code);
            }
        }
        else
        {
            if (dsTss.Tables[0].Rows.Count > 0)
            {
                DataRow drTss = dsTss.Tables[0].Rows[0];
                TSS_bra_code = "NG0020001"; //drTss["bra_code"].ToString();
                TSS_cus_num = "";
                TSS_cur_code = "NGN";
                //TSS_led_code = drTss["ledcode"].ToString(); //UMCOMMENT FOR LIVE
                TSS_led_code = "12501";
                Last4 = int.Parse(TSS_bra_code.Substring(6, 3)) + 2000;
                TSSAcct = "NGN" + TSS_led_code + "0001" + Last4.ToString();
                TSS_sub_acct_code = TSSAcct;
                //TSS_sub_acct_code = b1.FormInternalAcct(TSS_bra_code, "NGN", TSS_led_code);
            }
        }

        //assign the income account to the variables
        //Modification by Chigozie Anyasor as requsted to ensure 
        //to enusre USSD fees for NIP goes into a separate account PL52259
        // while sterling momey, sterling one and IBS goes into another. PL52340 ,
        // PL for NIP_PL_ACCT_CIB
        string NIP_PL_ACCT_USSD = ConfigurationManager.AppSettings["NIP_PL_ACCT_USSD"].ToString();
        string NIP_PL_ACCT_OTHERS = ConfigurationManager.AppSettings["NIP_PL_ACCT_OTHERS"].ToString();
        string NIP_PL_ACCT_CIB = ConfigurationManager.AppSettings["NIP_PL_ACCT_CIB"].ToString();
        bool foundval1 = g.isBankCodeFound(t.inCust.bra_code);
        if (foundval1)
        {
            if (dsFee.Tables[0].Rows.Count > 0)
            {
                DataRow drFee = dsFee.Tables[0].Rows[0];
                FEE_bra_code = drFee["bra_code"].ToString();
                FEE_cus_num = drFee["cusnum"].ToString();
                FEE_cur_code = drFee["curcode"].ToString();
                FEE_led_code = drFee["ledcode"].ToString();
                FEE_sub_acct_code = drFee["subacctcode"].ToString();

                if (t.Appid == 26)
                {
                    FEE_bra_code = "NG0020001";// t.inCust.bra_code;
                    FEE_cus_num = "";
                    FEE_cur_code = "NGN";
                    FEE_led_code = "52259";
                    //TSSAcct = "PL52315";PL52259
                    TSSAcct = NIP_PL_ACCT_USSD;
                    FEE_sub_acct_code = TSSAcct;
                }
                else if (t.Appid == 31)
                {
                    FEE_bra_code = "NG0020001";// t.inCust.bra_code;
                    FEE_cus_num = "";
                    FEE_cur_code = "NGN";
                    FEE_led_code = "52522";
                    TSSAcct = NIP_PL_ACCT_CIB;
                    FEE_sub_acct_code = TSSAcct;
                }
                else
                {
                    FEE_bra_code = "NG0020001";// t.inCust.bra_code;
                    FEE_cus_num = "";
                    FEE_cur_code = "NGN";
                    FEE_led_code = "52340";
                    //TSSAcct = PL52340
                    TSSAcct = NIP_PL_ACCT_OTHERS;
                    FEE_sub_acct_code = TSSAcct;
                }
            }
        }
        else
        {
            if (dsFee.Tables[0].Rows.Count > 0)
            {
                DataRow drFee = dsFee.Tables[0].Rows[0];
                FEE_bra_code = "NG0020001";// drFee["bra_code"].ToString();
                FEE_cus_num = drFee["cusnum"].ToString();
                FEE_cur_code = drFee["curcode"].ToString();
                FEE_led_code = drFee["ledcode"].ToString();
                FEE_sub_acct_code = drFee["subacctcode"].ToString();

                if (t.Appid == 26)
                {
                    FEE_bra_code = "NG0020001";// t.inCust.bra_code;
                    FEE_cus_num = "";
                    FEE_cur_code = "NGN";
                    FEE_led_code = "52259";
                    TSSAcct = NIP_PL_ACCT_USSD;
                    FEE_sub_acct_code = TSSAcct;
                }
                else if (t.Appid == 31)
                {
                    FEE_bra_code = "NG0020001";// t.inCust.bra_code;
                    FEE_cus_num = "";
                    FEE_cur_code = "NGN";
                    FEE_led_code = "52522";
                    TSSAcct = NIP_PL_ACCT_CIB;
                    FEE_sub_acct_code = TSSAcct;
                }
                else
                {
                    FEE_bra_code = "NG0020001";// t.inCust.bra_code;
                    FEE_cus_num = "";
                    FEE_cur_code = "NGN";
                    FEE_led_code = "52340";
                    TSSAcct = NIP_PL_ACCT_OTHERS;
                    FEE_sub_acct_code = TSSAcct;
                }
            }
        }

        string xrem = "";
        new ErrorLog("appid " + t.Appid.ToString());

        string FormRmks = "";

        if (t.Appid == 5)
        {

            xrem = t.sessionid + "/" + t.origin_branch + "/" + t.inCust.cus_sho_name + "/" + "MFINO Ref:" + t.ReferenceID +
                t.destinationcode + "/" + t.outCust.cusname;
        }
        else
        {
       xrem = t.sessionid + "/" + t.origin_branch + "/" + t.inCust.cus_sho_name + "/" + t.destinationcode + "/" + t.outCust.cusname; 

        }
        xrem = xrem.Replace("&amp;", "&");
        xrem = xrem.Replace("&apos;", "'");
        xrem = xrem.Replace("&quot;", "\"");

        //xrem = xrem.Replace("& ", "&amp;");
        xrem = xrem.Replace("&", "&amp;");
        xrem = xrem.Replace("'", "&apos;");
        xrem = xrem.Replace("\"", "&quot;");


        string xremark1 = "NIP/" + xrem;
        string xremark2 = "NIPFEE/" + xrem;
        string xremark3 = "NIPVAT/" + xrem;

        //ensure the remarks are not more than 136
        if (xremark1.Length > 136)
        {
            xremark1 = xremark1.Substring(0, 136);
        }

        if (xremark2.Length > 136)
        {
            xremark2 = xremark2.Substring(0, 136);
        }

        if (xremark3.Length > 136)
        {
            xremark3 = xremark3.Substring(0, 136);
        }

        XMLGenerator xg = new XMLGenerator(t.tellerID);
        xg.startDebit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.vat.ToString(), Fee_expl_code, xremark3);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(FEE_bra_code, FEE_cus_num, FEE_cur_code, FEE_led_code, FEE_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        //xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.VAT_bra_code, t.VAT_cus_num, t.VAT_cur_code, t.VAT_led_code, t.VAT_sub_acct_code, t.vat.ToString(), Fee_expl_code, xremark3);
        xg.endCredit();

        xg.closeXML();
        //new ErrorLog("Request to vTeller " + xg.req.ToString() + " for " + t.sessionid);
        try
        {
            vteller.nfp vs = new vteller.nfp();
            //vs.Timeout = 136000;
            //vs.Timeout = 40000;
	    vs.Timeout = 300000;
            //xg.resp = vs.NIBBS(xg.req, "NIBSS Transfer from Sterling " + t.Refid);240000
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returened values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
            error_text = xg.debits[0]["err_text"].InnerText;

            Prin_Rsp = xg.debits[0].ChildNodes[7].InnerText;
            Fee_Rsp = xg.debits[1].ChildNodes[7].InnerText;
            Vat_Rsp = xg.debits[2].ChildNodes[7].InnerText;

            StringBuilder resp = new StringBuilder();
            resp.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            resp.Append("<intTrnxResp>");
            resp.Append("<totaldebit>" + xg.credits[0]["amount"].InnerText + "</totaldebit>");
            resp.Append("<remark>" + xremark1 + "</remark>");
            resp.Append("<principal>");
            resp.Append("<acc_num>");
            resp.Append(t.inCust.bra_code + TSS_cus_num + TSS_cur_code + TSS_led_code + TSS_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[0]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[0]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[0]["err_text"].InnerText + "</responseText>");
            resp.Append("</principal>");
            resp.Append("<fee>");
            resp.Append("<acc_num>");
            resp.Append(t.inCust.bra_code + FEE_cus_num + FEE_cur_code + FEE_led_code + FEE_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[1]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[1]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[1]["err_text"].InnerText + "</responseText>");
            resp.Append("</fee>");
            resp.Append("</intTrnxResp>");
            ResponseMsg = resp.ToString();
            new ErrorLog("Response from vTeller " + ResponseMsg + " for " + t.sessionid);
        }
        catch (Exception ex)
        {
            new ErrorLog("Unable to debit customer account " + " The error " + ex + " " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " for amount " + t.amount.ToString() + " and fee charge " + t.feecharge.ToString() + ex);
            Respreturnedcode1 = "1x";
        }
    }

    public void authorizeTrnxToSterling(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";
        Gadget g = new Gadget();

        string expl_code = "";

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }

        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();
        int Last4 = 0; string TSSAcct = "";
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            TSS_bra_code = drTss["bra_code"].ToString();
            TSS_cus_num = "";
            TSS_cur_code = "NGN";
            //TSS_led_code = drTss["ledcode"].ToString(); //UMCOMMENT FOR LIVE
            TSS_led_code = "12501";
            Last4 = int.Parse(TSS_bra_code.Substring(6, 3)) + 2000;
            TSSAcct = "NGN" + TSS_led_code + "0001" + Last4.ToString();
            TSS_sub_acct_code = TSSAcct;
            TSS_sub_acct_code = "0007891725";
        }
        if (dsFee.Tables[0].Rows.Count > 0)
        {
            DataRow drFee = dsFee.Tables[0].Rows[0];
            FEE_bra_code = drFee["bra_code"].ToString();
            FEE_cus_num = drFee["cusnum"].ToString();
            FEE_cur_code = drFee["curcode"].ToString();
            FEE_led_code = drFee["ledcode"].ToString();
            FEE_sub_acct_code = drFee["subacctcode"].ToString();

            FEE_bra_code = drFee["bra_code"].ToString();
            FEE_cus_num = "";
            FEE_cur_code = "NGN";
            FEE_led_code = "52315";
            FEE_sub_acct_code = "0";
            Last4 = int.Parse(FEE_bra_code.Substring(6, 3)) + 2000;
            TSSAcct = "NGN" + TSS_led_code + "0001" + Last4.ToString();
            FEE_sub_acct_code = TSSAcct;
            FEE_sub_acct_code = "0008533701";
        }

        //if (dsTss.Tables[0].Rows.Count > 0)
        //{
        //    DataRow drTss = dsTss.Tables[0].Rows[0];
        //    TSS_bra_code = drTss["bra_code"].ToString();
        //    TSS_cus_num = drTss["cusnum"].ToString();
        //    TSS_cur_code = drTss["curcode"].ToString();
        //    TSS_led_code = drTss["ledcode"].ToString();
        //    TSS_sub_acct_code = drTss["subacctcode"].ToString();
        //}
        ////assign the income account to the variables
        //if (dsFee.Tables[0].Rows.Count > 0)
        //{
        //    DataRow drFee = dsFee.Tables[0].Rows[0];
        //    FEE_bra_code = drFee["bra_code"].ToString();
        //    FEE_cus_num = drFee["cusnum"].ToString();
        //    FEE_cur_code = drFee["curcode"].ToString();
        //    FEE_led_code = drFee["ledcode"].ToString();
        //    FEE_sub_acct_code = drFee["subacctcode"].ToString();
        //}

        //string xremark1 = "NIBSS Transfer by Order of " + g.RemoveSpecialChars(t.outCust.cusname) + " in favour of " + g.RemoveSpecialChars(t.inCust.cus_sho_name);
        //string xremark2 = "Commission Received NIBSS Transfer";

        t.paymentRef = t.paymentRef.Replace("&amp;", "&");
        t.paymentRef = t.paymentRef.Replace("&apos;", "'");
        t.paymentRef = t.paymentRef.Replace("&quot;", "\"");

        t.paymentRef = t.paymentRef.Replace("& ", "&amp;");
        t.paymentRef = t.paymentRef.Replace("'", "&apos;");
        t.paymentRef = t.paymentRef.Replace("\"", "&quot;");

        t.narration = t.narration.Replace("&amp;", "&");
        t.narration = t.narration.Replace("&apos;", "'");
        t.narration = t.narration.Replace("&quot;", "\"");

        t.narration = t.narration.Replace("& ", "&amp;");
        t.narration = t.narration.Replace("'", "&apos;");
        t.narration = t.narration.Replace("\"", "&quot;");

        t.senderAcctname = t.senderAcctname.Replace("&amp;", "&");
        t.senderAcctname = t.senderAcctname.Replace("&apos;", "'");
        t.senderAcctname = t.senderAcctname.Replace("&quot;", "\"");

        t.senderAcctname = t.senderAcctname.Replace("& ", "&amp;");
        t.senderAcctname = t.senderAcctname.Replace("'", "&apos;");
        t.senderAcctname = t.senderAcctname.Replace("\"", "&quot;");

        string xrem = t.Remark + t.sessionid + " PAYREF:" + t.paymentRef +
            " SENDER: " + t.senderAcctname + " REMARK: " + t.narration;
        if (xrem.Length > 136)
        {
            xrem = xrem.Substring(0, 136); //banks colum takes 200 max
        }
        string xremark1 = "NIP From " + xrem;
        string xremark2 = "NIPFEE From " + xrem;

        XMLGenerator xg = new XMLGenerator(t.tellerID);

        xg.startDebit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, "0", Fee_expl_code, xremark2);
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, "0", Fee_expl_code, xremark2);
        //xg.addAccount(t.inCust.bra_code, FEE_cus_num, FEE_cur_code, FEE_led_code, FEE_sub_acct_code, "0", "905", xremark2);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, "0", Fee_expl_code, xremark2);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, "0", Fee_expl_code, xremark2);
        xg.endCredit();

        xg.closeXML();

        try
        {
            //uncomment this part letter
            vteller.nfp vs = new vteller.nfp();
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returned values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
        }
        catch (Exception ex)
        {
            new ErrorLog("unable to credit Customer account " + t.inCust.sub_acct_code + " with amount " + t.amount.ToString() + ex);
        }
    }

    public void authorizeTrnxReversal(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";
        Gadget g = new Gadget();


        string expl_code = "";

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }

        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            TSS_bra_code = drTss["bra_code"].ToString();
            TSS_cus_num = drTss["cusnum"].ToString();
            TSS_cur_code = drTss["curcode"].ToString();
            TSS_led_code = drTss["ledcode"].ToString();
            TSS_sub_acct_code = drTss["subacctcode"].ToString();
        }
        //assign the income account to the variables
        if (dsFee.Tables[0].Rows.Count > 0)
        {
            DataRow drFee = dsFee.Tables[0].Rows[0];
            FEE_cus_num = drFee["cusnum"].ToString();
            FEE_cur_code = drFee["curcode"].ToString();
            FEE_led_code = drFee["ledcode"].ToString();
            FEE_sub_acct_code = drFee["subacctcode"].ToString();
        }

        string xremark1 = "NIP Reversal " + "/" + t.sessionid;
        string xremark2 = "NIPFEE Reversal " + "/" + t.sessionid;

        XMLGenerator xg = new XMLGenerator(t.tellerID);

        xg.startDebit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(FEE_bra_code, FEE_cus_num, FEE_cur_code, FEE_led_code, FEE_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        //xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, calculateFee(t.amount).ToString(), Fee_expl_code, xremark2);

        xg.endDebit();

        xg.startCredit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, calculateFee(t.amount).ToString(), Fee_expl_code, xremark2);
        xg.endCredit();

        xg.closeXML();

        //uncomment this part letter
        try
        {
            vteller.nfp vs = new vteller.nfp();
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returned values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
        }
        catch (Exception ex)
        {
            new ErrorLog("Unable to do Reversal for customer account " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " for amount " + t.amount.ToString() + " and fee charge " + calculateFee(t.amount).ToString() + ex);
        }
    }
    public void authorizeTrnxReversalMobile(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";
        Gadget g = new Gadget();

        string expl_code = "";

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }
        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            TSS_bra_code = drTss["bra_code"].ToString();
            TSS_cus_num = drTss["cusnum"].ToString();
            TSS_cur_code = drTss["curcode"].ToString();
            TSS_led_code = drTss["ledcode"].ToString();
            TSS_sub_acct_code = drTss["subacctcode"].ToString();
        }
        //assign the income account to the variables
        if (dsFee.Tables[0].Rows.Count > 0)
        {
            DataRow drFee = dsFee.Tables[0].Rows[0];
            FEE_cus_num = drFee["cusnum"].ToString();
            FEE_cur_code = drFee["curcode"].ToString();
            FEE_led_code = drFee["ledcode"].ToString();
            FEE_sub_acct_code = drFee["subacctcode"].ToString();
        }

        string xremark1 = "NIP Reversal " + "/" + t.sessionid;
        string xremark2 = "NIPFEE Reversal " + "/" + t.sessionid;

        XMLGenerator xg = new XMLGenerator(t.tellerID);

        xg.startDebit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.endCredit();

        xg.closeXML();

        //uncomment this part letter
        try
        {
            vteller.nfp vs = new vteller.nfp();
            xg.resp = vs.NIBBS(xg.req,  t.Refid.ToString());
            xg.parseResponse();

            ////collect returned values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
        }
        catch (Exception ex)
        {
            new ErrorLog("Unable to do Mobile Reversal for customer account" + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " for amount " + t.amount.ToString() + " and fee charge " + t.feecharge.ToString() + ex);
        }
    }
    public void authorizeTrnxReversalIBS(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";
        Gadget g = new Gadget();

        string expl_code = "";

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }
        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            TSS_bra_code = drTss["bra_code"].ToString();
            TSS_cus_num = drTss["cusnum"].ToString();
            TSS_cur_code = drTss["curcode"].ToString();
            TSS_led_code = drTss["ledcode"].ToString();
            TSS_sub_acct_code = drTss["subacctcode"].ToString();
        }
        //assign the income account to the variables
        if (dsFee.Tables[0].Rows.Count > 0)
        {
            DataRow drFee = dsFee.Tables[0].Rows[0];
            FEE_cus_num = drFee["cusnum"].ToString();
            FEE_cur_code = drFee["curcode"].ToString();
            FEE_led_code = drFee["ledcode"].ToString();
            FEE_sub_acct_code = drFee["subacctcode"].ToString();
        }

        string xremark1 = "NIP Reversal " + "/" + t.sessionid;
        string xremark2 = "NIPFEE Reversal " + "/" + t.sessionid;

        XMLGenerator xg = new XMLGenerator(t.tellerID);

        xg.startDebit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.endCredit();

        xg.closeXML();

        //uncomment this part letter
        try
        {
            vteller.nfp vs = new vteller.nfp();
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returned values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
        }
        catch (Exception ex)
        {
            new ErrorLog("Unable to do Mobile Reversal for customer account" + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " for amount " + t.amount.ToString() + " and fee charge " + t.feecharge.ToString() + ex);
        }
    }
    public Account getBalance(Account a)
    {
        try
        {
            EACBS.banks bank = new EACBS.banks();

            //sbp.banks bank = new sbp.banks();
            //DataSet ds = bank.getCusBalance(a.bra_code, a.cus_num, a.cur_code, a.led_code, a.sub_acct_code); //Live Banks
            //DataSet ds = bank.getCusBalanceTest(a.bra_code, a.cus_num, a.cur_code, a.led_code,a.sub_acct_code); //TestBanks
            DataSet ds = bank.getAccountFullInfo(a.sub_acct_code);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                a.rtnMssg = 1;
                a.avail_bal = Convert.ToDecimal(dr["UsableBal"]);
                a.cle_bal = Convert.ToDecimal(dr["UsableBal"]);
                a.cus_sho_name = Convert.ToString(dr["CUS_SHO_NAME"]);
                a.bal_limit = Convert.ToDecimal(dr["bal_lim"]);
                //a.status = Convert.ToInt32(dr["sta_code"]);
            }
            else
            {
                a.rtnMssg = 2;
                a.ref_key = "";
                //a.cle_bal = null;
            }
        }
        catch (Exception ex)
        {
            ErrorLog err = new ErrorLog(ex);
        }
        return a;
    }
    //authorize Ebills
    public void authorizeEbills(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";

        string expl_code = "";
        Gadget g = new Gadget();

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }


        DataSet dsTss = ts.getCurrentTss();
        //DataSet dsFee = ts.getCurrentIncomeAcct(); getCurrentEbillsIncomeAcct
        DataSet dsFee = ts.getCurrentEbillsIncomeAcct();
        //assign the Tss account to the varriables
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            TSS_bra_code = drTss["bra_code"].ToString();
            TSS_cus_num = drTss["cusnum"].ToString();
            TSS_cur_code = drTss["curcode"].ToString();
            TSS_led_code = drTss["ledcode"].ToString();
            TSS_sub_acct_code = drTss["subacctcode"].ToString();
        }

        //assign the income account to the variables
        if (dsFee.Tables[0].Rows.Count > 0)
        {
            DataRow drFee = dsFee.Tables[0].Rows[0];
            FEE_cus_num = drFee["cusnum"].ToString();
            FEE_cur_code = drFee["curcode"].ToString();
            FEE_led_code = drFee["ledcode"].ToString();
            FEE_sub_acct_code = drFee["subacctcode"].ToString();
        }

        string xrem = t.origin_branch + "/" + t.inCust.cus_sho_name + "/" +
            t.destinationcode + "/" + t.outCust.cusname + "/" + t.sessionid;


        xrem = xrem.Replace("&amp;", "&");
        xrem = xrem.Replace("&apos;", "'");
        xrem = xrem.Replace("&quot;", "\"");

        xrem = xrem.Replace("& ", "&amp;");
        xrem = xrem.Replace("'", "&apos;");
        xrem = xrem.Replace("\"", "&quot;");



        string xremark1 = "NIP/Ebills/" + xrem;
        string xremark2 = "NIPFEE/Ebills/" + xrem;
        string xremark3 = "NIPVAT/Ebills/" + xrem;

        XMLGenerator xg = new XMLGenerator(t.tellerID);

        xg.startDebit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.vat.ToString(), Fee_expl_code, xremark3);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.VAT_bra_code, t.VAT_cus_num, t.VAT_cur_code, t.VAT_led_code, t.VAT_sub_acct_code, t.vat.ToString(), "905", xremark3);
        xg.endCredit();

        xg.closeXML();

        try
        {
            vteller.nfp vs = new vteller.nfp();
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returened values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
            string error_text = xg.debits[0]["err_text"].InnerText;

            StringBuilder resp = new StringBuilder();
            resp.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            resp.Append("<intTrnxResp>");
            resp.Append("<totaldebit>" + xg.credits[0]["amount"].InnerText + "</totaldebit>");
            resp.Append("<remark>" + xremark1 + "</remark>");
            resp.Append("<principal>");
            resp.Append("<acc_num>");
            resp.Append(t.inCust.bra_code + TSS_cus_num + TSS_cur_code + TSS_led_code + TSS_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[0]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[0]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[0]["err_text"].InnerText + "</responseText>");
            resp.Append("</principal>");
            resp.Append("<fee>");
            resp.Append("<acc_num>");
            resp.Append(t.inCust.bra_code + FEE_cus_num + FEE_cur_code + FEE_led_code + FEE_sub_acct_code);
            resp.Append("</acc_num>");
            resp.Append("<tra_seq>" + xg.debits[1]["tra_seq"].InnerText + "</tra_seq>");
            resp.Append("<responseCode>" + xg.debits[1]["return_status"].InnerText + "</responseCode>");
            resp.Append("<responseText>" + xg.debits[1]["err_text"].InnerText + "</responseText>");
            resp.Append("</fee>");
            resp.Append("</intTrnxResp>");
            ResponseMsg = resp.ToString();

        }
        catch (Exception ex)
        {
            new ErrorLog("Unable to debit customer account " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " for amount " + t.amount.ToString() + " and fee charge " + t.feecharge.ToString() + ex);
            Respreturnedcode1 = "1x";
        }
    }

    //account block
    public void authorizeTrnxToSterling_BlkAMt(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";
        Gadget g = new Gadget();

        string expl_code = "";

        //DataSet dsExp = ts.getExpcode();
        //if (dsExp.Tables[0].Rows.Count > 0)
        //{
        //    DataRow drexp = dsExp.Tables[0].Rows[0];
        //    expl_code = drexp["expcodeVal"].ToString();
        //}
        //else
        //{
        //}
        int Last4 = 0; string TSSAcct = "";
        DataSet dsTss = ts.getCurrentTss11();
        string blk_bra_code = ""; string blk_cus_num = ""; string blk_cur_code = ""; string blk_led_code = ""; string blk_sub_acct_code = "";
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            blk_bra_code = drTss["bra_code"].ToString();
            blk_cus_num = "";// drTss["cusnum"].ToString();
            blk_cur_code = "NGN";// drTss["curcode"].ToString();
            blk_led_code = drTss["ledcode"].ToString();
            Last4 = int.Parse(blk_bra_code.Substring(6, 3)) + 2000;
            TSSAcct = blk_cur_code + blk_led_code + "0001" + Last4.ToString();

            blk_sub_acct_code = TSSAcct;// drTss["subacctcode"].ToString();

        }

        //string xremark1 = "NIBSS Transfer by Order of " + g.RemoveSpecialChars(t.outCust.cusname) + " in favour of " + g.RemoveSpecialChars(t.inCust.cus_sho_name);
        //string xremark2 = "Commission Received NIBSS Transfer";

        t.paymentRef = t.paymentRef.Replace("&amp;", "&");
        t.paymentRef = t.paymentRef.Replace("&apos;", "'");
        t.paymentRef = t.paymentRef.Replace("&quot;", "\"");

        t.paymentRef = t.paymentRef.Replace("& ", "&amp;");
        t.paymentRef = t.paymentRef.Replace("'", "&apos;");
        t.paymentRef = t.paymentRef.Replace("\"", "&quot;");

        t.narration = t.narration.Replace("&amp;", "&");
        t.narration = t.narration.Replace("&apos;", "'");
        t.narration = t.narration.Replace("&quot;", "\"");

        t.narration = t.narration.Replace("& ", "&amp;");
        t.narration = t.narration.Replace("'", "&apos;");
        t.narration = t.narration.Replace("\"", "&quot;");

        //t.senderAcctname = t.senderAcctname.Replace("&amp;", "&");
        //t.senderAcctname = t.senderAcctname.Replace("&apos;", "'");
        //t.senderAcctname = t.senderAcctname.Replace("&quot;", "\"");

        //t.senderAcctname = t.senderAcctname.Replace("& ", "&amp;");
        //t.senderAcctname = t.senderAcctname.Replace("'", "&apos;");
        //t.senderAcctname = t.senderAcctname.Replace("\"", "&quot;");

        //string xrem = t.Remark + ":" + t.sessionid + ":PAYREF:" + t.paymentRef + ":" +
        //    " SENDER: " + t.senderAcctname + " REMARK: " + t.narration;
        string xrem = t.Remark;
        if (xrem.Length > 136)
        {
            xrem = xrem.Substring(0, 136); //banks colum takes 200 max
        }
        string xremark1 = "NIP From " + xrem;
        string xremark2 = "NIPFEE From " + xrem;
        string xremark3 = "NIPVAT/" + xrem;

        XMLGenerator xg = new XMLGenerator(t.tellerID);
        string expcode = "111";
        xg.startDebit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expcode, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, "0", expcode, xremark2);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, "0", expcode, xremark3);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(blk_bra_code, blk_cus_num, blk_cur_code, blk_led_code, blk_sub_acct_code, t.amount.ToString(), expcode, xremark1);
        xg.addAccount(blk_bra_code, blk_cus_num, blk_cur_code, blk_led_code, blk_sub_acct_code, t.feecharge.ToString(), expcode, xremark2);
        xg.addAccount(t.VAT_bra_code, t.VAT_cus_num, t.VAT_cur_code, t.VAT_led_code, t.VAT_sub_acct_code, t.vat.ToString(), expcode, xremark3);
        xg.endCredit();

        xg.closeXML();

        try
        {
            //uncomment this part letter
            vteller.nfp vs = new vteller.nfp();
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returned values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
            string error_text = xg.debits[0]["err_text"].InnerText;
        }
        catch (Exception ex)
        {
            new ErrorLog("unable to credit Customer account " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " with amount " + t.amount.ToString() + ex);
        }
    }
    public void authorizeTrnxToSterling_BlkAMtRev(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";
        Gadget g = new Gadget();

        string expl_code = "";

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }

        DataSet dsTss = ts.getCurrentTss11();
        string blk_bra_code = ""; string blk_cus_num = ""; string blk_cur_code = ""; string blk_led_code = ""; string blk_sub_acct_code = "";
        int Last4 = 0; string TSSAcct = "";
        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            //blk_bra_code = drTss["bra_code"].ToString();
            //blk_cus_num = drTss["cusnum"].ToString();
            //blk_cur_code = drTss["curcode"].ToString();
            //blk_led_code = drTss["ledcode"].ToString();
            //blk_sub_acct_code = drTss["subacctcode"].ToString();

            blk_bra_code = drTss["bra_code"].ToString();
            blk_cus_num = "";// drTss["cusnum"].ToString();
            blk_cur_code = "NGN";// drTss["curcode"].ToString();
            blk_led_code = drTss["ledcode"].ToString();
            Last4 = int.Parse(blk_bra_code.Substring(6, 3)) + 2000;
            TSSAcct = blk_cur_code + blk_led_code + "0001" + Last4.ToString();

            blk_sub_acct_code = TSSAcct;// drTss["subacctcode"].ToString();
        }

        //string xremark1 = "NIBSS Transfer by Order of " + g.RemoveSpecialChars(t.outCust.cusname) + " in favour of " + g.RemoveSpecialChars(t.inCust.cus_sho_name);
        //string xremark2 = "Commission Received NIBSS Transfer";

        t.paymentRef = t.paymentRef.Replace("&amp;", "&");
        t.paymentRef = t.paymentRef.Replace("&apos;", "'");
        t.paymentRef = t.paymentRef.Replace("&quot;", "\"");

        t.paymentRef = t.paymentRef.Replace("& ", "&amp;");
        t.paymentRef = t.paymentRef.Replace("'", "&apos;");
        t.paymentRef = t.paymentRef.Replace("\"", "&quot;");

        t.narration = t.narration.Replace("&amp;", "&");
        t.narration = t.narration.Replace("&apos;", "'");
        t.narration = t.narration.Replace("&quot;", "\"");

        t.narration = t.narration.Replace("& ", "&amp;");
        t.narration = t.narration.Replace("'", "&apos;");
        t.narration = t.narration.Replace("\"", "&quot;");

        //t.senderAcctname = t.senderAcctname.Replace("&amp;", "&");
        //t.senderAcctname = t.senderAcctname.Replace("&apos;", "'");
        //t.senderAcctname = t.senderAcctname.Replace("&quot;", "\"");

        //t.senderAcctname = t.senderAcctname.Replace("& ", "&amp;");
        //t.senderAcctname = t.senderAcctname.Replace("'", "&apos;");
        //t.senderAcctname = t.senderAcctname.Replace("\"", "&quot;");

        //string xrem = t.Remark + ":" + t.sessionid + ":PAYREF:" + t.paymentRef + ":" +
        //    " SENDER: " + t.senderAcctname + " REMARK: " + t.narration;
        string xrem = t.Remark;
        if (xrem.Length > 136)
        {
            xrem = xrem.Substring(0, 136); //banks colum takes 200 max
        }
        string xremark1 = "NIP From " + xrem;
        string xremark2 = "NIPFEE From " + xrem;
        string xremark3 = "NIPVAT/" + xrem;

        XMLGenerator xg = new XMLGenerator(t.tellerID);

        xg.startDebit();
        xg.addAccount(blk_bra_code, blk_cus_num, blk_cur_code, blk_led_code, blk_sub_acct_code, t.amount.ToString(), "2995", xremark1);
        xg.addAccount(blk_bra_code, blk_cus_num, blk_cur_code, blk_led_code, blk_sub_acct_code, t.feecharge.ToString(), "2995", xremark2);
        xg.addAccount(t.VAT_bra_code, t.VAT_cus_num, t.VAT_cur_code, t.VAT_led_code, t.VAT_sub_acct_code, t.vat.ToString(), "2995", xremark3);
        xg.endDebit();

        xg.startCredit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), "2995", xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, "0", "2995", xremark2);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, "0", "2995", xremark3);
        xg.endCredit();

        xg.closeXML();

        try
        {
            //uncomment this part letter
            vteller.nfp vs = new vteller.nfp();
            xg.resp = vs.NIBBS(xg.req, t.Refid.ToString());
            xg.parseResponse();

            ////collect returned values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;
            string error_text = xg.debits[0]["err_text"].InnerText;
        }
        catch (Exception ex)
        {
            new ErrorLog("unable to credit Customer account " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " with amount " + t.amount.ToString() + ex);
        }
    }

    public void authorizeTrnxToSterlingDD(Transaction t)
    {
        RespCreditedamt = "x";
        Respreturnedcode1 = "x";
        Respreturnedcode2 = "x";
        Gadget g = new Gadget();

        string expl_code = "";

        DataSet dsExp = ts.getExpcode();
        if (dsExp.Tables[0].Rows.Count > 0)
        {
            DataRow drexp = dsExp.Tables[0].Rows[0];
            expl_code = drexp["expcodeVal"].ToString();
        }
        else
        {
        }

        DataSet dsTss = ts.getCurrentTss();
        DataSet dsFee = ts.getCurrentIncomeAcct();

        if (dsTss.Tables[0].Rows.Count > 0)
        {
            DataRow drTss = dsTss.Tables[0].Rows[0];
            string TSSAcct = ""; int Last4 = 0;
            //TSS_bra_code = t.inCust.bra_code;
            TSS_bra_code = "NG0020001";
            TSS_cus_num = "";
            TSS_cur_code = "NGN";
            TSS_led_code = "12501";
            Last4 = int.Parse(TSS_bra_code.Substring(6, 3)) + 2000;
            TSSAcct = TSS_cur_code + TSS_led_code + "0001" + Last4.ToString();
            TSS_sub_acct_code = TSSAcct;

        }
        int FEELast4 = 0; string FEETSSAcct = "";
        //assign the income account to the variables
        if (dsFee.Tables[0].Rows.Count > 0)
        {
            DataRow drFee = dsFee.Tables[0].Rows[0];
            FEE_bra_code = drFee["bra_code"].ToString();
            FEE_cus_num = drFee["cusnum"].ToString();
            FEE_cur_code = drFee["curcode"].ToString();
            FEE_led_code = drFee["ledcode"].ToString();
            FEE_sub_acct_code = drFee["subacctcode"].ToString();

            FEE_bra_code = "NG0020001";// t.inCust.bra_code;
            FEE_cus_num = "";
            FEE_cur_code = "NGN";
            FEE_led_code = "52315";
            FEE_sub_acct_code = "0";
            FEELast4 = int.Parse(FEE_bra_code.Substring(6, 3)) + 2000;
            FEETSSAcct = "NGN" + TSS_led_code + "0001" + FEELast4.ToString();
            FEE_sub_acct_code = FEETSSAcct;
            FEE_sub_acct_code = "PL52315";
        }

        //string xremark1 = "NIBSS Transfer by Order of " + g.RemoveSpecialChars(t.outCust.cusname) + " in favour of " + g.RemoveSpecialChars(t.inCust.cus_sho_name);
        //string xremark2 = "Commission Received NIBSS Transfer";

        t.paymentRef = t.paymentRef.Replace("&amp;", "&");
        t.paymentRef = t.paymentRef.Replace("&apos;", "'");
        t.paymentRef = t.paymentRef.Replace("&quot;", "\"");

        t.paymentRef = t.paymentRef.Replace("& ", "&amp;");
        t.paymentRef = t.paymentRef.Replace("'", "&apos;");
        t.paymentRef = t.paymentRef.Replace("\"", "&quot;");

        t.narration = t.narration.Replace("&amp;", "&");
        t.narration = t.narration.Replace("&apos;", "'");
        t.narration = t.narration.Replace("&quot;", "\"");

        t.narration = t.narration.Replace("& ", "&amp;");
        t.narration = t.narration.Replace("&", "&amp;");
        t.narration = t.narration.Replace("'", "&apos;");
        t.narration = t.narration.Replace("\"", "&quot;");

        t.senderAcctname = t.senderAcctname.Replace("&amp;", "&");
        t.senderAcctname = t.senderAcctname.Replace("&apos;", "'");
        t.senderAcctname = t.senderAcctname.Replace("&quot;", "\"");

        t.senderAcctname = t.senderAcctname.Replace("& ", "&amp;");
        t.senderAcctname = t.senderAcctname.Replace("&", "&amp;");
        t.senderAcctname = t.senderAcctname.Replace("'", "&apos;");
        t.senderAcctname = t.senderAcctname.Replace("\"", "&quot;");


        string xrem = t.sessionid + "/" + t.Remark + " PAYREF:" + t.paymentRef +
            " SENDER: " + t.senderAcctname + " REMARK: " + t.narration;

        if (xrem.Length > 136)
        {
            xrem = xrem.Substring(0, 136); //banks colum takes 200 max
        }
        string xremark1 = "NIP From " + xrem.Trim().Replace("  ", " "); ;
        if (xremark1.Length > 136)
        {
            //new ErrorLog("The Remarks is greather than 136 for transaction " + t.sessionid + " the remakrs was " + xremark1);
            Mylogger.Info("The Remarks is greather than 136 for transaction " + t.sessionid + " the remakrs was " + xremark1);
            xremark1 = xremark1.Substring(0, 136);
            xremark1 = xremark1.Replace("&apo", "");
            xremark1 = xremark1.Replace("&quo", "");
        }

        string xremark2 = "NIPFEE From " + xrem.Trim().Replace("  ", " ");
        if (xremark2.Length > 136)
        {
            //new ErrorLog("The Remarks is greather than 136 for transaction " + t.sessionid + " the remakrs was " + xremark2);
            Mylogger.Info("The Remarks is greather than 136 for transaction " + t.sessionid + " the remakrs was " + xremark2);
            xremark2 = xremark2.Substring(0, 136);
            xremark2 = xremark2.Replace("&apo", "");
            xremark2 = xremark2.Replace("&quo", "");
        }

        XMLGenerator xg = new XMLGenerator(t.tellerID);

        //debit customer and credit TSS for DD
        xg.startDebit();
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(t.inCust.bra_code, t.inCust.cus_num, t.inCust.cur_code, t.inCust.led_code, t.inCust.sub_acct_code, "0", Fee_expl_code, xremark2);

        xg.endDebit();

        xg.startCredit();
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.amount.ToString(), expl_code, xremark1);
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, t.feecharge.ToString(), Fee_expl_code, xremark2);
        xg.addAccount(TSS_bra_code, TSS_cus_num, TSS_cur_code, TSS_led_code, TSS_sub_acct_code, "0", Fee_expl_code, xremark2);


        xg.endCredit();

        xg.closeXML();

        try
        {
            //uncomment this part letter
            vteller.nfp vs = new vteller.nfp();
            vs.Timeout = 60000;
            xg.resp = vs.NIBBSInward(xg.req, t.Refid.ToString() + "001");
            xg.parseResponse();


            ////collect returned values
            RespCreditedamt = xg.credits[0]["amount"].InnerText;
            Respreturnedcode1 = xg.debits[0]["return_status"].InnerText;
            Respreturnedcode2 = xg.debits[1]["return_status"].InnerText;

            Prin_Rsp = xg.debits[0].ChildNodes[7].InnerText;
            Fee_Rsp = xg.debits[1].ChildNodes[7].InnerText;
            Vat_Rsp = "";

        }
        catch (Exception ex)
        {
            //new ErrorLog("unable to credit Customer account " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " with amount " + t.amount.ToString() + " " + ex + " Error code " + Respreturnedcode1);
            Mylogger.Error("unable to credit Customer account " + t.inCust.bra_code + t.inCust.cus_num + t.inCust.cur_code + t.inCust.led_code + t.inCust.sub_acct_code + " with amount " + t.amount.ToString() + " " + ex + " Error code " + Respreturnedcode1);
            string sql2 = "";
            sql2 = "update tbl_WStrans set staggingStatus =@s where sessionid=@sid";
            Connect c2 = new Connect(sql2, true);
            c2.addparam("@s", 102);
            c2.addparam("@sid", t.sessionid);
            c2.query();
        }
    }
}
