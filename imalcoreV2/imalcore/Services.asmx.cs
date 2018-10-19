using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace imalcore
{

    /// <summary>
    /// Summary description for Services
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]

    public class Services : System.Web.Services.WebService
    {


        [WebMethod]
        public DataSet AccountValidation(string acctnumber)
        {
            DataSet ds = new DataSet();

            string sql = @"select distinct c.LONG_NAME_ENG as CustomerName,d.ADDITIONAL_REFERENCE as NUBAN,'NG0020006' as BranchCode,
                         c.CIF_NO as CUS_NUM,d.CURRENCY_CODE as CurrencyCode,d.STATUS as STA_CODE,d.GL_CODE as LED_CODE,d.GL_CODE as ProductCode,abs(d.CV_AVAIL_BAL) as AVAIL_BAL,d.CIF_SUB_NO
                         from imal.cif c INNER JOIN imal.cif_address ad on c.CIF_NO = ad.CIF_NO INNER JOIN imal.amf d on d.CIF_SUB_NO=ad.CIF_NO
                         and d.additional_reference='" + acctnumber + "'";
            Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
            cn.SetSQL(sql);
            try
            {
                ds = cn.Select();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    Mylogger.Info("Response for querying aacount number : " + acctnumber.ToString() + " " + dr["CustomerName"].ToString());

                }
                else
                {
                    Mylogger.Info("No record found for this : " + acctnumber.ToString());
                }

            }
            catch (Exception ex)
            {
                Mylogger.Info(" Info selecting from datasource: " + ex.ToString());
            }

            return ds;
        }
        [WebMethod]
        public string FundstransFer(string fromacct, string toacct, decimal amt, string remarks)
        {
            Mylogger.Info("Request to be treated: From account " + fromacct + " to acct " + toacct + " amt: " + amt.ToString() + " narration " + remarks);
            string as_descrmks = ""; string as_descArabRmks = "";

            as_descrmks = remarks;
            as_descArabRmks = remarks;
            if (as_descrmks.Length >= 80)
            {
                as_descrmks = remarks.Substring(0, 80);//29
            }
            if (as_descArabRmks.Length >= 100)
            {
                as_descArabRmks = remarks.Substring(0, 100);//30
            }

            if (remarks.Length >= 50)//87
            {
                remarks = remarks.Substring(0, 50);
            }
            Gadget g = new Gadget();
            OraConntxn cn = new OraConntxn("imal.P_API_TRANSFER_EX3", true);
            cn.addparam("AL_CHANNEL_ID", "decimal", ParameterDirection.Input, 2);//channel id  1
            cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, "ATMT", 200);//user id 2
            cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, "1", 200);// machine name 3
            cn.addparam("AL_API_CODE", "decimal", ParameterDirection.Input, 59);//api code 4
            cn.addparam("AL_COMP_CODE", "decimal", ParameterDirection.Input, 1);//comp code 5
            cn.addparam("AL_BRANCH", "decimal", ParameterDirection.Input, 1); // 6 branch
            cn.addparam("AL_TELLER", "decimal", ParameterDirection.Input, 18);//find out if the teller is fixed  7
            cn.addparam("AL_TRXTYPE", "decimal", ParameterDirection.Input, 26);//transaction 8
            cn.addparam("AL_USE_CARD_ACCNO", "decimal", ParameterDirection.Input, 0);//AL_USE_CARD_ACCNO 9
            cn.addparam("AS_TRANSACTION_TYPE", "string", ParameterDirection.Input, "C", 200);//crdit 10
            cn.addparam("AS_CARD", "string", ParameterDirection.Input, DBNull.Value, 200);//11
            cn.addparam("AL_ACC_BR", "decimal", ParameterDirection.Input, DBNull.Value);//12
            cn.addparam("AL_ACC_CY", "decimal", ParameterDirection.Input, DBNull.Value);//13
            cn.addparam("AL_ACC_GL", "decimal", ParameterDirection.Input, DBNull.Value);//14
            cn.addparam("AL_ACC_CIF", "decimal", ParameterDirection.Input, DBNull.Value);//15
            cn.addparam("AL_ACC_SL", "decimal", ParameterDirection.Input, DBNull.Value);//16
            cn.addparam("AS_ACCOUNT", "string", ParameterDirection.Input, fromacct, 200);//17
            cn.addparam("AL_TO_ACC_BR", "decimal", ParameterDirection.Input, DBNull.Value);//18
            cn.addparam("AL_TO_ACC_CY", "decimal", ParameterDirection.Input, DBNull.Value);//19
            cn.addparam("AL_TO_ACC_GL", "decimal", ParameterDirection.Input, DBNull.Value);//20
            cn.addparam("AL_TO_ACC_CIF", "decimal", ParameterDirection.Input, DBNull.Value);//21
            cn.addparam("AL_TO_ACC_SL", "decimal", ParameterDirection.Input, DBNull.Value);//22
            cn.addparam("AS_TOACCOUNT", "string", ParameterDirection.Input, toacct, 200);//23
            cn.addparam("ADEC_AMOUNT", "decimal", ParameterDirection.Input, amt);//24 amt
            cn.addparam("AS_CURRENCY", "string", ParameterDirection.Input, "566", 200);//25
            cn.addparam("AS_DATE_TIME", "datetime", ParameterDirection.Input, DateTime.Now); //g.gettransdate());//26
            cn.addparam("AS_REFERENCE", "string", ParameterDirection.Input, remarks, 200);//27//reference//50 length
            cn.addparam("AL_POS", "decimal", ParameterDirection.Input, DBNull.Value);//28
            cn.addparam("AS_DESC", "string", ParameterDirection.Input, as_descrmks, 200);//29//remarks//80 length
            cn.addparam("AS_DESC_ARAB", "string", ParameterDirection.Input, as_descArabRmks, 200);//30//100 length
            cn.addparam("ADT_VALUE_DATE", "datetime", ParameterDirection.Input, DateTime.Now); //g.gettransdate());//31
            cn.addparam("AS_BILLER_CODE", "string", ParameterDirection.Input, DBNull.Value);//32
            cn.addparam("AS_SO_REFERENCE", "string", ParameterDirection.Input, DBNull.Value);//33 
            cn.addparam("AL_ADD_NUMBER", "decimal", ParameterDirection.Input, DBNull.Value);//34
            cn.addparam("AL_NUM_OF_SHARES", "decimal", ParameterDirection.Input, DBNull.Value);//35
            cn.addparam("AS_INSTRUCTIONS1", "string", ParameterDirection.Input, DBNull.Value);//36
            cn.addparam("AS_INSTRUCTIONS2", "string", ParameterDirection.Input, DBNull.Value);//37
            cn.addparam("AS_INSTRUCTIONS3", "string", ParameterDirection.Input, DBNull.Value);//38
            cn.addparam("AS_INSTRUCTIONS4", "string", ParameterDirection.Input, DBNull.Value);//39
            cn.addparam("AL_TRX_PURPOSE", "decimal", ParameterDirection.Input, DBNull.Value);//40
            cn.addparam("AS_APPROVED_TRX", "string", ParameterDirection.Input, "1", 200);//41
            cn.addparam("OL_TRX_CODE", "decimal", ParameterDirection.InputOutput, DBNull.Value);

            //output parameters
            cn.addparam("OL_SHARE_REF", "decimal", ParameterDirection.Output);
            cn.addparam("ODEC_AVAIL", "decimal", ParameterDirection.Output);
            cn.addparam("OL_TRANSACTION_ID", "decimal", ParameterDirection.Output);
            cn.addparam("OL_ERROR_CODE", "decimal", ParameterDirection.Output);
            cn.addparam("OS_MESSAGE", "string", ParameterDirection.Output, "", 4000);
            try
            {
                cn.query();
            }
            catch (Exception ex)
            {
                Mylogger.Info(" Info executing query: " + ex.ToString());
            }

            //Read the output parameter after execution
            string temp = cn.cmd.Parameters["@OL_SHARE_REF"].Value.ToString(); //avail_bal
            string temp0 = cn.cmd.Parameters["@ODEC_AVAIL"].Value.ToString(); //OL_TRX_CODE
            string temp1 = cn.cmd.Parameters["@OL_TRX_CODE"].Value.ToString();
            string temp2 = cn.cmd.Parameters["@OL_TRANSACTION_ID"].Value.ToString();//txn code
            string temp3 = cn.cmd.Parameters["@OL_ERROR_CODE"].Value.ToString();//resp code
            string temp4 = cn.cmd.Parameters["@OS_MESSAGE"].Value.ToString();//resp message
            if (temp3 == "0")
            {
                Mylogger.Info("Transaction successful for to account " + toacct + " with amt " + amt.ToString() + " response code 00");
                return "00*" + "Successful" + "*" + temp + "*" + temp2 + "*" + temp0;
            }
            else
            {
                Mylogger.Info("Transaction not successful for to account " + toacct + " with amt " + amt.ToString() + temp3 + "*" + temp4);
                return temp3 + "*" + temp4;
            }
            return "";
        }
    }
}
