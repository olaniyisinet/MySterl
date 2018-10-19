using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class BalEnquiryResp
    {
        public string accountNumber { get; set; }
        public string availableBalance { get; set; }
        public string ledgerBalance { get; set; }
        public string accountStatus { get; set; }
        public string currencyCode { get; set; }
        public string accountTypeDescriptionEng { get; set; }
        public string shortAccountDescriptionEng { get; set; }
        public string fullAccountDescriptionEng { get; set; }
        public string accountBalanceFC { get; set; }
        public string availableAccountBalanceFC { get; set; }
        public string accountBalanceCV { get; set; }
        public string availableAccountBalanceCV { get; set; }
        public string lastDepositAmount { get; set; }
        public string lastWithdrawalAmount { get; set; }
        public string accountBranchCode { get; set; }
        public string accountBranchName { get; set; }
        public string responseCode { get; set; }
        public string errorCode { get; set; }
        public string skipProcessing { get; set; }
        public string originalResponseCode { get; set; }
        public string skipLog { get; set; }
    }
}