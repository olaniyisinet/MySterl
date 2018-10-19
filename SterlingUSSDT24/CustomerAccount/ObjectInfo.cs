using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerAccount
{
    public class AccountLookupInfo
    {
        public int ID { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string AccountType { get; set; }
        public string Dateopen { get; set; }
        public string AccountNumber { get; set; }
        public string SessionId { get; set; }
        public string ClientMobileNo { get; set; }
        public string DateRequest { get; set; }
        public string RequestStatue { get; set; }
        public string RequestMessage { get; set; }
        public string DateRespond { get; set; }
        public string BranchCode { get; set; }
        public string CurrencyCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductSequence { get; set; } 
    }


    public class AccountInfo
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string CustomerID { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime BirthDate { get; set; }



        internal void Set(System.Data.DataRow dr)
        {
            AccountNumber = Convert.ToString(dr["MAP_ACC_NO"]);
            Email = Convert.ToString(dr["EMAIL"]);
            AccountName = Convert.ToString(dr["CUS_SHO_NAME"]);
            DateOpen = Convert.ToDateTime(dr["DATE_OPEN"]);
            CustomerID = Convert.ToString(dr["CUS_NUM"]);
            Mobile = Convert.ToString(dr["MOBILE_PHONE"]);
            //BirthDate = Convert.ToDateTime(dr["BIRTH_DATE"]);

        }
    }


    public class Account
    {
        public string AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string AltAccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BVN { get; set; }

        public string CustomerId { get; set; }

        public string AltNumber { get; set; }

        public decimal AvailBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal BalanceLimit { get; set; }

        public string BraCode { get; set; }
        public string BranchName { get; set; }
        public string CurCode { get; set; }
        public string CurrencyName { get; set; }
        public string LedCode { get; set; }
        public string LedgerName { get; set; }
        public string SubAcctCode { get; set; }
        public string CustomerClass { get; set; }
        public string RestInd { get; set; }
        public string StaCode { get; set; }


        public string MobNum { get; set; }
        public string Email { get; set; }


        public string CustomerType { get; set; }


        public DateTime LastTransactDate { get; set; }

        public string Officer { get; set; }

        public DateTime DateOpened { get; set; }



        public string BraCodeInt()
        {
            return BraCode;
        }


    }
}


 