using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Sterling.MSSQL;
using imalcore;
using System.Configuration;

namespace imalChannelWS.Models
{
    public class NIPInwardFT
    {
        public string SessionID { get; set; }
        public string OriginatorAccountName { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string Amount { get; set; }
        public string PaymentReference { get; set; }
        public string Narration { get; set; }
        public string getFromAccount( int request)
        {
            var sql = "select [Account] from [dbo].[tbl_InternalAccount] where [Request] =@RequestID";
            Connect cn = new Connect("mssqlSpay");
            cn.SetSQL(sql);
            cn.AddParam("@RequestID", request);
            string ret = cn.Select().Tables[0].Rows[0][0].ToString();
            cn.Close();
            return ret;
        }
        public string truncateToFitField(string remarks)
        {
            if (remarks != null && (!string.IsNullOrEmpty(remarks)))
            {
                if (remarks.Length <= 40)
                {
                    return remarks;
                }
                else
                {
                    // truncate remarks
                    return remarks.Substring(0, 40);
                }
            }
            return remarks;
        }
        public string truncateToFitField(String remarks, int length)
        {
            if (remarks != null && (!string.IsNullOrEmpty(remarks)))
            {
                if (remarks.Length <= length)
                {
                    return remarks;
                }
                else
                {
                    // truncate remarks
                    return remarks.Substring(0, length);
                }
            }
            return remarks;
        }

        public bool checkCurrencyMatch(NIPInwardFT req)
        {
            MyEncryDecr myEncryDecr = new MyEncryDecr();
            string InwardKey = ConfigurationSettings.AppSettings["InwardKey"].ToString();
            int reqID =Convert.ToInt32(ConfigurationSettings.AppSettings["requestID"].ToString());
            NIPInwardFT ft = new NIPInwardFT();
            //int reqID = 36;
            string toAcc = myEncryDecr.Decrypt(req.BeneficiaryAccountNumber, InwardKey); ;
            string froAcc = ft.getFromAccount(reqID);
            DataSet dsFrm = getAccountDetails(froAcc);
            DataSet dsTo = getAccountDetails(toAcc);
            if (dsFrm != null && dsTo != null)
            {
                if (dsFrm.Tables[0].Rows.Count != 0 && dsTo.Tables[0].Rows.Count != 0)
                {
                    if (dsFrm.Tables[0].Rows[0]["currency_code"].ToString() == dsTo.Tables[0].Rows[0]["currency_code"].ToString())
                    {
                        return true;
                    }
                    else
                    { return false; }
                }
                else
                { return false; }
            }
            else { return false; }
        }
        public DataSet getAccountDetails(string acctNo)
        {
            DataSet ds = null;
            var sql = "select long_name_eng,branch_code,cif_sub_no,currency_code,gl_code,sl_no,status,cv_avail_bal,ytd_cv_bal,last_trans_date from imal.amf where additional_reference =:additional_reference";
            Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
            cn.SetSQL(sql);
            cn.AddParam(":additional_reference", acctNo);
            ds= cn.Select();
            cn.Close();
            return ds;
        }
        public void LogRequest( string jso, string SessionID, string BeneficiaryAcctNo, string Amount,string OriginAccount)
        {
            string sql = @"INSERT INTO tbl_trnx (SessionID, RequestIn,BeneficiaryAcctNo,Amount,OriginAccount) VALUES ( @SessionID, @RequestIn,@BeneficiaryAcctNo,@Amount,@OriginAccount)";
            Connect cn = new Connect("mssqlSpay");
            cn.SetSQL(sql);
            cn.AddParam("@SessionID", SessionID);
            cn.AddParam("@RequestIn", jso);
            cn.AddParam("@BeneficiaryAcctNo", BeneficiaryAcctNo);
            cn.AddParam("@Amount", Amount);
            cn.AddParam("@OriginAccount", OriginAccount);
            var ret = cn.Insert();
            if(!string.IsNullOrEmpty(cn.errmsg))
            {
                Mylogger.Info("Unable to log request with sessionID " + SessionID + ", reason " + cn.errmsg);
            }
            cn.Close();
        }
        public bool checkRequestAlreadyLogged(string SessionID)
        {
            string sql = "SELECT Id, SessionID, RequestTypeId, RequestIn, TimeIn, ResponseJSON, TimeOut, ResponseCode FROM tbl_trnx WHERE SessionID = @SessionID";
            Connect cn = new Connect("mssqlSpay");
            cn.SetSQL(sql);
            cn.AddParam("@SessionID", SessionID);
            DataSet ds = cn.Select();
            if (!string.IsNullOrEmpty(cn.errmsg))
            {
                Mylogger.Info("Unable to check request status for " + SessionID + ", reason " + cn.errmsg);
            }
            if(ds.Tables[0].Rows.Count!=0)
            {
                cn.Close();
                return false;
            }
            else
            {
                cn.Close();
                return true;
            }
        }

        public void UpdateRequestStatus(string ResponseJSON, string ResponseCode, string sessionID, string IMALTransactionID)
        {
            string sql = "UPDATE tbl_trnx SET SessionID = @SessionID, ResponseJSON = @ResponseJSON, TimeOut = @TimeOut, ResponseCode = @ResponseCode,IMALTransactionID=@IMALTransactionID WHERE SessionID = @SessionID";
            Connect cn = new Connect("mssqlSpay");
            cn.SetSQL(sql);
            cn.AddParam("@SessionID", sessionID);
            cn.AddParam("@ResponseJSON", ResponseJSON);
            cn.AddParam("@TimeOut", DateTime.Now);
            cn.AddParam("@ResponseCode", ResponseCode);
            cn.AddParam("@IMALTransactionID", IMALTransactionID ?? "");
            cn.Update();
            cn.Close();
        }
    }
}