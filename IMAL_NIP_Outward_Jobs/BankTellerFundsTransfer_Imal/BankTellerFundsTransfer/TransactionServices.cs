using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace BankTellerFundsTransfer_lagos3
{
    class TransactionServices
    {
        public decimal currentFee;
        public string approvedby = "";
        public decimal getCUrrentFee()
        {
            DataSet ds = new DataSet();
            try
            {
                Connect c = new Connect("spd_getCurrenFee");
                ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    currentFee = Convert.ToDecimal(dr["feeAmt"]);
                }
            }
            catch (Exception ex)
            {
                ErrorLog err = new ErrorLog(ex);
            }
            return currentFee;
        }
        public string getApprovebySessionid(string sessionID)
        {
            DataSet ds = new DataSet();
            try
            {
                Connect c = new Connect("spd_getBankTellerRecBySessionid");
                c.addparam("@sessionid", sessionID);
                ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    approvedby = Convert.ToString(dr["approvedby"]);
                }
            }
            catch (Exception ex)
            {
                ErrorLog err = new ErrorLog(ex);
            }
            return approvedby;
        }
    }
}
