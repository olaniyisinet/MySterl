using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class CheckSavingsLedger
    {
        public decimal maxpertran = 0;
        public decimal maxperday = 0;
        public bool isLedgerFound(string led_code)
        {
            bool found = false;
            try
            {
                DataSet ds = new DataSet();
                string sql = @"select * from imal.gen_ledger where gl_code =:led_code";
                Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
                cn.SetSQL(sql);
                cn.AddParam(":led_code", led_code);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    found = true;
                }
                else
                {
                    found = false;
                }
            }
            catch (Exception ex)
            {
                
            }
            return found;
        }
        public decimal getMaxEFTAmt()
        {
            decimal val = 0;
            try
            {
                string sql = "select maxpertrans, maxperday from tbl_savingsLecodeEFTAmt where statusflag=1";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("conn_nip");
                c.SetSQL(sql);
                DataSet ds = c.Select("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    maxpertran = decimal.Parse(dr["maxpertrans"].ToString());
                    maxperday = decimal.Parse(dr["maxperday"].ToString());
                }
            }
            catch (Exception ex)
            {
                
            }
            return val;
        }
    }
}