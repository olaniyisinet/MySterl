using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class FullAcctStmtResp
    {
        public string companyCode { get; set; }
        public string branchCode { get; set; }
        public string currencyCode { get; set; }
        public string glCode { get; set; }
        public string customerNumber { get; set; }
        public string subAccount { get; set; }
        public string operationNumber { get; set; }
        public string lineNumber { get; set; }
        public string valueDate { get; set; }
        public string baseCurrencyTransactionAmount { get; set; }
        public string baseAccountTransactionAmount { get; set; }
        public string balance { get; set; }
        public string description { get; set; }
        public string descriptionArab { get; set; }
        public string descriptionArab1 { get; set; }
        public string jvType { get; set; }
        public string transactionType { get; set; }
        public string openingBalance { get; set; }
        public string ctsTransactionNumber { get; set; }
        public string addDate { get; set; }
        public string skipProcessing { get; set; }
        public string skipLog { get; set; }
        public string transactionDate { get; set; }
    }
}