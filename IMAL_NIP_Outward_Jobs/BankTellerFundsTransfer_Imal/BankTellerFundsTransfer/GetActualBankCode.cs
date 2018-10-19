using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace BankTellerFundsTransfer_lagos3
{
    class GetActualBankCode
    {
        public string getNewBankCode(string bc)
        {
            string NewBC = "";
            string sql = "SELECT refid, bankcode, old_bankcode FROM  tbl_participatingBanks where old_bankcode =@obc";
            Connect c = new Connect(sql, true);
            c.addparam("@obc", bc);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                NewBC = dr["bankcode"].ToString();
            }
            return NewBC;
        }
    }
}
