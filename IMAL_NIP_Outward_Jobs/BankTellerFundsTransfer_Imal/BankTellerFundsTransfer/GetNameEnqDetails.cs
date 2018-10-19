using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace BankTellerFundsTransfer_lagos3
{
    class GetNameEnqDetails
    {
        public string BankVerificationNumber = "";
        public string KYCLevel = ""; public string ResponseCode = "";
        public string getdetailsbySID(string sessionid)
        {
            string sql = "select SessionIDNE, DestinationInstitutionCode, ChannelCode, AccountNumber, " +
                " AccountName, BankVerificationNumber, KYCLevel, ResponseCode from tbl_altransactionsNE where SessionIDNE = @sid";
            Connect c = new Connect(sql, true);
            c.addparam("@sid", sessionid);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                BankVerificationNumber = dr["BankVerificationNumber"].ToString();
                KYCLevel = dr["KYCLevel"].ToString();
            }
            return "";
        }
    }
}
