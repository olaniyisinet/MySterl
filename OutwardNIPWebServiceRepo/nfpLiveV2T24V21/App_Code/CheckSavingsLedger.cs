using System;
using System.Data;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for CheckSavingsLedger
/// </summary>
public class CheckSavingsLedger
{
    public decimal maxpertran = 0;
    public decimal maxperday = 0;
    public bool isLedgerFound(string led_code)
    {
        bool found = false;
        try
        {
            string sql = "select led_code from tbl_savingsLedcode where led_code =@lc";
            Connect c = new Connect(sql, true);
            c.addparam("@lc", led_code);
            DataSet ds = c.query("rec");
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
            new ErrorLog("Error occured checking if a ledger is a savings account " + ex);
        }
        return found;
    }

    public decimal getMaxEFTAmt()
    {
        decimal val = 0;
        try
        {
            string sql = "select maxpertrans, maxperday from tbl_savingsLecodeEFTAmt where statusflag=1";
            Connect c = new Connect(sql, true);
            DataSet ds = c.query("rec");
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                maxpertran = decimal.Parse(dr["maxpertrans"].ToString());
                maxperday = decimal.Parse(dr["maxperday"].ToString());
            }
        }
        catch (Exception ex)
        {
            new ErrorLog("Error occured getting EFT amt for savings account " + ex);
        }
        return val;
    }
}