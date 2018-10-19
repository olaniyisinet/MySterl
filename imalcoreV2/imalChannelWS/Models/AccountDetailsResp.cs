using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class AccountDetailsResp
    {
        public string branchCode { get; set; }
        public string subAccountCode { get; set; }
        public string currencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string LedgerName { get; set; }
        public string glCode { get; set; }
        public string customerNumber { get; set; }
        public string name { get; set; }
        public string BVN { get; set; }
        public string responseCode { get; set; }
        public string skipProcessing { get; set; }
        public string skipLog { get; set; }
        public string status { get; set; }
        public string availableBalance { get; set; }
        public string ledgerBalance { get; set; }
        public string lastTransactionDate { get; set; }
    }
}