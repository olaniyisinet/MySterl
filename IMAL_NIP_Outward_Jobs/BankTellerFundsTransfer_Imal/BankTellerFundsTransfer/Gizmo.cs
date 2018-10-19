using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace BankTellerFundsTransfer_lagos3
{
    class Gizmo
    {
        public bool findKyc(Int32 ledcode)
        {
            bool found = false;
            string sql_kyc = "select kycledger from tbl_kyc_ledger where kycledger=@kyc";
            Connect c = new Connect(sql_kyc, true);
            c.addparam("@kyc", ledcode);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                found = true;
            }
            else
            {
                found = false;
            }
            return found;
        }
    }
}
