using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace NIPHandleRequeryIMAL
{
    class Trnx
    {
        public Int32 Refid; public string sessionid;
        public int Transnature; public int channelCode;
        public string BatchNumber; public string paymentRef;
        public string mandateRefNum; public string BillerID;
        public string BillerName; public string senderbankcode;
        public string senderbank; public string senderaccount;
        public string sendername; public decimal amt; public decimal feecharge;
        public string bra_code; public string cus_num;
        public string cur_code; public string led_code;
        public string sub_acct_code; public string accname;
        public string Remark; DateTime inputdate; DateTime approveddate;
        public int Approvevalue; public string transtype; public string Responsecode;
        public string ResponseMsg; public int FTadvice; DateTime FTadviceDate;
        public string reversalStatus; public int staggingStatus; public string remarks;
        public int TransProcessed; public string Errcode;
        public DateTime TransProcessDate;


        public string SessionID;
        public string DestinationInstitutionCode;
        public string NameEnquiryRef;
        public string ChannelCode;
        public string BeneficiaryAccountName;
        public string BeneficiaryAccountNumber;
        public string BeneficiaryBankVerificationNumber;

        public string OriginatorAccountName;
        public string OriginatorAccountNumber;
        public string OriginatorBankVerificationNumber;

        public string OriginatorKYCLevel;
        public string TransactionLocation;
        public string BeneficiaryKYCLevel;
        public string Narration;
        public string PaymentReference;
        public string Amount;
        public string ResponseCode;
        public string toNibss;
    

        public void setTrnx(DataRow dr)
        {
            try
            {
                Refid = Convert.ToInt32(dr["Refid"].ToString());
                //Console.WriteLine("Processing transaction ID: " + Refid.ToString());
                Transnature = int.Parse(dr["Transnature"].ToString());
                channelCode = int.Parse(dr["channelCode"].ToString());
                sessionid = dr["sessionid"].ToString();
                SessionID = dr["sessionid"].ToString();
                BatchNumber = dr["BatchNumber"].ToString().Trim();
                BeneficiaryAccountNumber = dr["Nuban"].ToString().Trim();
                paymentRef = dr["paymentRef"].ToString();
                PaymentReference = dr["paymentRef"].ToString();
                mandateRefNum = dr["mandateRefNum"].ToString();
                BillerID = dr["BillerID"].ToString();
                BillerName = dr["BillerName"].ToString();

                senderbankcode = System.Configuration.ConfigurationManager.AppSettings["sendindBankcode"]; 
                sendername = dr["sendername"].ToString();
                OriginatorAccountName = dr["sendername"].ToString();
                amt = decimal.Parse(dr["amt"].ToString());
                Amount = dr["amt"].ToString();

                // feecharge = decimal.Parse(dr["feecharge"].ToString());
                try
                {
                    feecharge = decimal.Parse(dr["feecharge"].ToString());
                }
                catch
                {
                    feecharge = 0;
                }
                bra_code = dr["bra_code"].ToString();
                cus_num = dr["cus_num"].ToString();
                cur_code = dr["cur_code"].ToString();
                led_code = dr["led_code"].ToString();
                sub_acct_code = dr["sub_acct_code"].ToString();
                Console.WriteLine("Processing transaction ID: " + Refid.ToString() + " Branch " + bra_code);
                accname = dr["accname"].ToString();

                remarks = dr["Remark"].ToString();
                remarks = remarks.Replace("& #40;", "(");
                remarks = remarks.Replace("& #41;", ")");
                remarks = remarks.Replace("& #40", "(");
                remarks = remarks.Replace("& #41", ")");
                remarks = remarks.Replace("&", " ");

                Narration = remarks;
                inputdate = Convert.ToDateTime(dr["inputdate"].ToString());
                approveddate = Convert.ToDateTime(dr["approveddate"].ToString());
                Approvevalue = Convert.ToInt32(dr["Approvevalue"]);
                transtype = dr["transtype"].ToString();
                Responsecode = dr["Responsecode"].ToString();
                staggingStatus = Convert.ToInt32(dr["staggingStatus"]);
                new Errorlog("value in the Errorcode " + Convert.ToString(dr["Errcode"]));
                Errcode = Convert.ToString(dr["Errcode"]);
                //TransProcessed = dr["TransProcessed"].ToString();
            }
            catch { }
        }
    }
}
