using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for SBPSwitch
/// </summary>
public class SBPSwitch
{
    //check if it is sterling transaction
    public int getInternalBankID(string AccountNumber)
    {
        
        int bankid = 0;
        try
        {
            if (AccountNumber.StartsWith("99"))
            {
                Mylogger.Info("BankOne Inward " + AccountNumber);
                bankid = 3;
            }
            else
            {
                string sql = "select * from tbl_Switchdetails where @ac between RangeFrom and Rangeto";
                Connect c = new Connect(sql, true);
                c.addparam("@ac", AccountNumber);
                DataSet ds = c.query("rec");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    bankid = int.Parse(dr["Bankid"].ToString());
                    if (bankid == 1)
                    {
                        //new ErrorLog("Sterling Inward " + AccountNumber);
                    }
                    else
                    {
                        //new ErrorLog("IMAL Inward " + AccountNumber);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //new ErrorLog("Error occured getting bankid based on the NUBAN: " + AccountNumber + " supplied " + ex);
        }
        return bankid;
    }
}
