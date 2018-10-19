using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NIB_EChannels.Models
{
    public class NIP_Branch
    {
        public string Refid { get; set; }
        public string sessionid { get; set; }
        public string sessionidNE { get; set; }
        public string transactioncode { get; set; }
        public string channelCode { get; set; }
        public string BatchNumber { get; set; }
        public string paymentRef { get; set; }
        public string amount { get; set; }
        public string feecharge { get; set; }
        public string vat { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string originatorname { get; set; }
        public string bra_code { get; set; }
        public string cus_num { get; set; }
        public string cur_code { get; set; }
        public string led_code { get; set; }
        public string sub_acct_code { get; set; }
        public string accname { get; set; }
        public string response { get; set; }
        public string dateadded { get; set; }
        public string requeryStatus { get; set; }
        public string nameResponse { get; set; }
        public string lastupdate { get; set; }
        public string reversalstatus { get; set; }
        public string vTellerMsg { get; set; }
        public string oddResponse { get; set; }
        public string FTadvice { get; set; }
        public string FTadviceDate { get; set; }
        public string mailSent { get; set; }
        public string outwardTrnsType { get; set; }
        public string appsTransType { get; set; }
        public string nuban { get; set; }
        public string bankcode { get; set; }
        public string Prin_Rsp { get; set; }
        public string Fee_Rsp { get; set; }
        public string Vat_Rsp { get; set; }
        public string Prin_Rsp1 { get; set; }
        public string Fee_Rsp1 { get; set; }
        public string Vat_Rsp1 { get; set; }
        public string Account_Status { get; set; }
        public string Restriction { get; set; }
    }
}