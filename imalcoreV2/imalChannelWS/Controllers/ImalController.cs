using BankCore;
using BankCore.t24;
using imalChannelWS.Models;
using imalcore;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml;

namespace imalChannelWS.Controllers
{
    public class ImalController : ApiController
    {
        /// <summary>
        /// API to do Balance enquiry
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BalEnquiry")]
        public async Task<HttpResponseMessage> BalEnquiry([FromBody] BalEnquiryReq r)
        {
            string json = "";
            Mylogger.Info("Balance enquiry request received for customer " + r.accountNumber);
            //get the request, process and respond
            OraConntxn cn = new OraConntxn("imal.P_API_GET_AMF_BALANCE", true);
            //input
            cn.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, 1);//1
            cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, DBNull.Value);//2
            cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, DBNull.Value);//3
            cn.addparam("AL_API_CODE", "int", ParameterDirection.Input, 1);//4
            cn.addparam("AL_COMP_CODE", "int", ParameterDirection.Input, 1);//5
            cn.addparam("AL_BRANCH", "int", ParameterDirection.Input, DBNull.Value); //6
            cn.addparam("AS_CURRENCY", "string", ParameterDirection.Input, DBNull.Value);//7
            cn.addparam("AL_GLCODE", "int", ParameterDirection.Input, DBNull.Value);//8
            cn.addparam("AL_CIF_SUB_NO", "int", ParameterDirection.Input, DBNull.Value);//9
            cn.addparam("AL_SL_NO", "int", ParameterDirection.Input, DBNull.Value);//10
            cn.addparam("AS_ADD_REFERENCE", "string", ParameterDirection.Input, r.account);//11
            //output  
            cn.addparam("OS_CY_CODE", "int", ParameterDirection.Output);//12
            cn.addparam("OS_GL_NAME_ENG", "string", ParameterDirection.Output, "", 4000);//13
            cn.addparam("OS_GL_NAME_ARAB", "string", ParameterDirection.Output, "", 4000);//14
            cn.addparam("OS_BRIEF_NAME_ENG", "string", ParameterDirection.Output, "", 4000);//15
            cn.addparam("OS_BRIEF_NAME_ARAB", "string", ParameterDirection.Output, "", 4000);//16
            cn.addparam("OS_LONG_NAME_ENG", "string", ParameterDirection.Output, "", 4000);//17
            cn.addparam("OS_LONG_NAME_ARAB", "string", ParameterDirection.Output, "", 4000);//18
            cn.addparam("ODEC_FC_BAL", "string", ParameterDirection.Output, "", 4000);//19
            cn.addparam("ODEC_FC_AVAIL_BAL", "string", ParameterDirection.Output, "", 4000);//20
            cn.addparam("ODEC_CV_BAL", "string", ParameterDirection.Output, "", 4000);//21
            cn.addparam("ODEC_CV_AVAIL_BAL", "string", ParameterDirection.Output, "", 4000);//22
            cn.addparam("OS_STATUS", "string", ParameterDirection.Output, "", 4000);//23
            cn.addparam("OD_OPEN_DATE", "string", ParameterDirection.Output, "", 4000);//24
            cn.addparam("OD_MATURITY", "string", ParameterDirection.Output, "", 4000);//25
            cn.addparam("OD_LAST_DEPOSIT_DATE", "string", ParameterDirection.Output, "", 4000);//26
            cn.addparam("OD_LAST_WITHDRAWAL_DATE", "string", ParameterDirection.Output, "", 4000);//27
            cn.addparam("OD_LAST_ACTIVITY_DATE", "string", ParameterDirection.Output, "", 4000);//28
            cn.addparam("ODEC_LAST_DEPOSIT_AMOUNT", "string", ParameterDirection.Output, "", 4000);//29
            cn.addparam("ODEC_LAST_WITHDRAWAL_AMOUNT", "string", ParameterDirection.Output, "", 4000);//30
            cn.addparam("ODEC_HOLD_AMOUNT", "decimal", ParameterDirection.Output);//31
            cn.addparam("ODEC_TODAY_CREDIT_AMOUNT", "string", ParameterDirection.Output, "", 4000);//32
            cn.addparam("ODEC_TODAY_DEBIT_AMOUNT", "string", ParameterDirection.Output, "", 4000);//33
            cn.addparam("OL_BRANCH_CODE", "string", ParameterDirection.Output, "", 4000);//34
            cn.addparam("OS_BRANCH_NAME", "string", ParameterDirection.Output, "", 4000);//35
            cn.addparam("OD_BRANCH_DATE", "string", ParameterDirection.Output, "", 4000);//36
            cn.addparam("OL_SUM_UNCLRD_CHQS", "string", ParameterDirection.Output, "", 4000);//37
            cn.addparam("OL_LIMIT_ACC", "string", ParameterDirection.Output, "", 4000);//38
            cn.addparam("OS_OFFICIER_NAME", "string", ParameterDirection.Output, "", 4000);//39
            cn.addparam("OL_ERROR_CODE", "string", ParameterDirection.Output, "", 4000);//40
            cn.addparam("OS_ERROR_DESC", "string", ParameterDirection.Output, "", 4000);//41

            try
            {
                cn.query();
            }
            catch (Exception ex)
            {
                new ErrorLog("Error occured executing oracle stored proc " + r.accountNumber + " ex " + ex);
                Mylogger.Info("Error occured executing oracle stored proc " + r.accountNumber + " ex " + ex);
                BalEnquiryResp resp = new BalEnquiryResp
                {
                    accountNumber = "",
                    availableBalance = "",
                    ledgerBalance = "",
                    accountStatus = "",
                    currencyCode = "",
                    accountTypeDescriptionEng = "",
                    shortAccountDescriptionEng = "",
                    fullAccountDescriptionEng = "",
                    accountBalanceFC = "",
                    availableAccountBalanceFC = "",
                    accountBalanceCV = "",
                    availableAccountBalanceCV = "",
                    lastDepositAmount = "",
                    lastWithdrawalAmount = "",
                    accountBranchCode = "",
                    accountBranchName = "",
                    responseCode = "-100005",//invalid account
                    errorCode = "",
                    skipProcessing = "false",
                    originalResponseCode = "-100005",//invalid account
                    skipLog = "false"
                };
                json = JsonConvert.SerializeObject(resp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
            //Read the output parameter after execution
            string accountNumber = r.account;
            string currencyCode = cn.cmd.Parameters["@OS_CY_CODE"].Value.ToString();
            string accountStatus = cn.cmd.Parameters["@OS_STATUS"].Value.ToString();
            string accountTypeDescriptionEng = cn.cmd.Parameters["@OS_GL_NAME_ENG"].Value.ToString();
            string shortAccountDescriptionEng = cn.cmd.Parameters["@OS_BRIEF_NAME_ENG"].Value.ToString();
            string fullAccountDescriptionEng = cn.cmd.Parameters["@OS_LONG_NAME_ENG"].Value.ToString();
            string accountBalanceFC = cn.cmd.Parameters["@ODEC_FC_BAL"].Value.ToString();
            string availableAccountBalanceFC = cn.cmd.Parameters["@ODEC_FC_AVAIL_BAL"].Value.ToString();
            string availableBalance = cn.cmd.Parameters["@ODEC_CV_AVAIL_BAL"].Value.ToString();//ODEC_FC_AVAIL_BAL
            string ledgerBalance = cn.cmd.Parameters["@ODEC_CV_BAL"].Value.ToString();
            string lastDepositAmount = cn.cmd.Parameters["@ODEC_LAST_DEPOSIT_AMOUNT"].Value.ToString();
            string lastWithdrawalAmount = cn.cmd.Parameters["@ODEC_LAST_WITHDRAWAL_AMOUNT"].Value.ToString();
            string accountBranchCode = cn.cmd.Parameters["@OL_BRANCH_CODE"].Value.ToString();
            string accountBranchName = cn.cmd.Parameters["@OS_BRANCH_NAME"].Value.ToString();
            string responseCode = cn.cmd.Parameters["@OL_ERROR_CODE"].Value.ToString();
            string errorCode = cn.cmd.Parameters["@OL_ERROR_CODE"].Value.ToString();
            string errormsg = cn.cmd.Parameters["@OS_ERROR_DESC"].Value.ToString();
            string skipProcessing = "false";
            string originalResponseCode = responseCode;
            string skipLog = "false";
            if (responseCode == "0")
            {
                BalEnquiryResp resp = new BalEnquiryResp
                {
                    accountNumber = accountNumber,
                    availableBalance = availableBalance,
                    ledgerBalance = ledgerBalance,
                    accountStatus = accountStatus,
                    currencyCode = currencyCode,
                    accountTypeDescriptionEng = accountTypeDescriptionEng,
                    shortAccountDescriptionEng = shortAccountDescriptionEng,
                    fullAccountDescriptionEng = fullAccountDescriptionEng,
                    accountBalanceFC = accountBalanceFC,
                    availableAccountBalanceFC = availableAccountBalanceFC,
                    accountBalanceCV = availableBalance,
                    availableAccountBalanceCV = availableBalance,
                    lastDepositAmount = lastDepositAmount,
                    lastWithdrawalAmount = lastWithdrawalAmount,
                    accountBranchCode = accountBranchCode,
                    accountBranchName = accountBranchName,
                    responseCode = responseCode,
                    errorCode = errorCode,
                    skipProcessing = skipProcessing,
                    originalResponseCode = originalResponseCode,
                    skipLog = skipLog
                };
                json = JsonConvert.SerializeObject(resp);
            }
            else
            {
                BalEnquiryResp resp = new BalEnquiryResp
                {
                    accountNumber = accountNumber,
                    availableBalance = availableBalance,
                    ledgerBalance = ledgerBalance,
                    accountStatus = accountStatus,
                    currencyCode = currencyCode,
                    accountTypeDescriptionEng = accountTypeDescriptionEng,
                    shortAccountDescriptionEng = shortAccountDescriptionEng,
                    fullAccountDescriptionEng = fullAccountDescriptionEng,
                    accountBalanceFC = accountBalanceFC,
                    availableAccountBalanceFC = availableAccountBalanceFC,
                    accountBalanceCV = ledgerBalance,
                    availableAccountBalanceCV = ledgerBalance,
                    lastDepositAmount = lastDepositAmount,
                    lastWithdrawalAmount = lastWithdrawalAmount,
                    accountBranchCode = accountBranchCode,
                    accountBranchName = accountBranchName,
                    responseCode = "-100009",//invalid account
                    errorCode = errorCode,
                    skipProcessing = skipProcessing,
                    originalResponseCode = "-100009",//invalid account
                    skipLog = skipLog
                };
                json = JsonConvert.SerializeObject(resp);
            }
            return Request.CreateResponse(HttpStatusCode.OK, json);
        }

        /// <summary>
        /// API to get customer's account details
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AccountDetails")]
        public async Task<HttpResponseMessage> AccountDetails([FromBody] AccountDetailsReq r)
        {
            DataSet ds = new DataSet(); string json = ""; string acctType = ""; string currCode = "";
            string sql = @"select long_name_eng,branch_code,cif_sub_no,currency_code,gl_code, " +
                " sl_no,status,cv_avail_bal,ytd_cv_bal,last_trans_date,BLOCKED_CV from imal.amf " +
                         " where additional_reference='" + r.account + "'";
            Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
            cn.SetSQL(sql);
            try
            {
                ds = cn.Select();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    decimal bal = Convert.ToDecimal(dr["cv_avail_bal"].ToString());
                    string blockedFunds = dr["BLOCKED_CV"].ToString();
                    if (string.IsNullOrEmpty(blockedFunds))
                    {
                        blockedFunds = "0";
                    }

                    decimal blockFund = Convert.ToDecimal(blockedFunds);
                    decimal realBalance = bal + blockFund;

                    string sql1 = @"select gl.brief_desc_eng from imal.gen_ledger gl where gl.gl_code = (select gl_code from imal.amf where  additional_reference='" + r.account + "')";
                    Sterling.Oracle.Connect c = new Sterling.Oracle.Connect("conn_imal");
                    c.SetSQL(sql1);
                    var ds1 = c.Select();
                    if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
                    {
                        acctType = ds1.Tables[0].Rows[0]["BRIEF_DESC_ENG"].ToString();
                    }

                    string sql2 = @"select cy.brief_desc_eng from imal.currencies cy where cy.currency_code = (select currency_code from imal.amf where  additional_reference='" + r.account + "')";
                    Sterling.Oracle.Connect c1 = new Sterling.Oracle.Connect("conn_imal");
                    c1.SetSQL(sql2);
                    var ds2 = c1.Select();
                    if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                    {
                        currCode = ds2.Tables[0].Rows[0]["BRIEF_DESC_ENG"].ToString();
                    }

                    string bvn = ""; string customerNumber = dr["cif_sub_no"].ToString();
                    string sql3 = @"select ADD_STRING1 from imal.cif where cif_no = '" + customerNumber + "'";
                    Sterling.Oracle.Connect c2 = new Sterling.Oracle.Connect("conn_imal");
                    c2.SetSQL(sql3);
                    var ds3 = c2.Select();
                    if (ds3 != null && ds3.Tables[0].Rows.Count > 0)
                    {
                        bvn = ds3.Tables[0].Rows[0]["ADD_STRING1"].ToString();
                    }

                    AccountDetailsResp resp = new AccountDetailsResp
                    {
                        branchCode = dr["branch_code"].ToString(),
                        subAccountCode = dr["sl_no"].ToString(),
                        currencyCode = dr["currency_code"].ToString(),
                        glCode = dr["gl_code"].ToString(),
                        customerNumber = dr["cif_sub_no"].ToString(),
                        name = dr["long_name_eng"].ToString(),
                        BVN = bvn,
                        responseCode = "0",
                        skipProcessing = "false",
                        skipLog = "false",
                        status = dr["status"].ToString(),
                        availableBalance = realBalance.ToString(),
                        ledgerBalance = dr["ytd_cv_bal"].ToString(),
                        lastTransactionDate = dr["last_trans_date"].ToString(),
                        LedgerName = acctType,
                        CurrencyName = currCode
                    };
                    json = JsonConvert.SerializeObject(resp);
                }
                else
                {
                    AccountDetailsResp resp = new AccountDetailsResp
                    {
                        branchCode = null,
                        subAccountCode = null,
                        currencyCode = null,
                        glCode = null,
                        customerNumber = null,
                        name = null,
                        responseCode = "-100009",
                        skipProcessing = null,
                        skipLog = null,
                        status = null,
                        availableBalance = null,
                        ledgerBalance = null,
                        lastTransactionDate = null
                    };
                    json = JsonConvert.SerializeObject(resp);
                    Mylogger.Info("No record found account balance request : " + r.account);
                }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                AccountDetailsResp resp = new AccountDetailsResp
                {
                    branchCode = null,
                    subAccountCode = null,
                    currencyCode = null,
                    glCode = null,
                    customerNumber = null,
                    name = null,
                    responseCode = "-100005",
                    skipProcessing = null,
                    skipLog = null,
                    status = null,
                    availableBalance = null,
                    ledgerBalance = null,
                    lastTransactionDate = null
                };
                json = JsonConvert.SerializeObject(resp);
                Mylogger.Info("Error occured during Account Details search: " + r.account + " " + ex.ToString());
            }
            return Request.CreateResponse(HttpStatusCode.OK, json);
        }

        /// <summary>
        /// API to do transaction Requery
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Requery")]
        public async Task<HttpResponseMessage> Requery([FromBody] RequeryReq r)
        {
            DataSet ds = new DataSet(); string json = "";
            //string sql = @"select ctstrs.status, ctstrs.trs_no , ctstrs.atm_reference from imal.ctstrs " +
            //    " where atm_reference=:instr1 and reference1=:instr2 " +
            //    " and acc_additional_reference=:acc_ref order by trs_date desc";

            string sql = @"select ctstrs.status, ctstrs.trs_no , ctstrs.atm_reference from imal.ctstrs " +
                 " where  acc_additional_reference='" + r.fromAccount + "' and" +
                 " atm_reference='" + r.referenceCodeToFind + "'" +
                 " and reference1='" + r.referenceCode + "'" +
                 " order by trs_date desc ";
            Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
            cn.SetSQL(sql);
            try
            {
                ds = cn.Select();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    RequeryResp rsp = new RequeryResp
                    {
                        responseCode = "0",
                        requestCode = r.requestCode,
                        principalIdentifier = r.principalIdentifier,
                        referenceCode = r.referenceCode,
                        referenceCodeToFind = r.referenceCodeToFind,
                        status = dr["status"].ToString()
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    RequeryResp rsp = new RequeryResp
                    {
                        responseCode = "-100010",
                        requestCode = r.requestCode,
                        principalIdentifier = r.principalIdentifier,
                        referenceCode = r.referenceCode,
                        referenceCodeToFind = r.referenceCodeToFind,
                        status = ""
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                }
            }
            catch (Exception ex)
            {
                Mylogger.Info("An error occured doing requery for acct " + r.fromAccount + " " + ex);
                RequeryResp rsp = new RequeryResp
                {
                    responseCode = "-100005",
                    requestCode = r.requestCode,
                    principalIdentifier = r.principalIdentifier,
                    referenceCode = r.referenceCode,
                    referenceCodeToFind = r.referenceCodeToFind,
                    status = ""
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "");
        }

        /// <summary>
        /// API to get MINI Account statements
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("MiniAcctStmt")]
        public async Task<HttpResponseMessage> MiniAcctStmt([FromBody] MiniAcctStmtReq r)
        {
            string responseCode = ""; string json = "";
            Gadget g = new Gadget();
            //get the request, process and respond
            OraConntxn cn = new OraConntxn("IMAL.P_API_LAST_N_TRX_AMT", true);
            cn.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, DBNull.Value);//1
            cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, DBNull.Value, 200);//2
            cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, DBNull.Value, 200);//3
            cn.addparam("AL_API_CODE", "int", ParameterDirection.Input, 53);//4
            cn.addparam("AL_LAST_N", "int", ParameterDirection.Input, 10);//5
            cn.addparam("AL_COMP_CODE", "int", ParameterDirection.Input, 1);//6
            cn.addparam("AL_BRANCH_CODE", "int", ParameterDirection.Input, DBNull.Value);//7
            cn.addparam("AL_CURRENCY", "int", ParameterDirection.Input, DBNull.Value);//8
            cn.addparam("AL_GL_CODE", "int", ParameterDirection.Input, DBNull.Value);//9
            cn.addparam("AL_CIF_SUB_NO", "int", ParameterDirection.Input, DBNull.Value);//10
            cn.addparam("AL_SL", "int", ParameterDirection.Input, DBNull.Value);//11
            cn.addparam("AS_ADD_REFERENCE", "string", ParameterDirection.Input, r.account, 200);//12
            cn.addparam("AL_FROM_AMOUNT", "int", ParameterDirection.Input, DBNull.Value);//13
            cn.addparam("AL_TO_AMOUNT", "int", ParameterDirection.Input, DBNull.Value);//14
            cn.addparam("AS_CARD", "string", ParameterDirection.Input, "1233", 200);//15
            cn.addparam("AL_CARD_PRESENT", "int", ParameterDirection.Input, 0);//16
            //output
            cn.addparam("OL_BRANCH_CODE", "int", ParameterDirection.Output);//17
            cn.addparam("OS_BRANCH_NAME", "string", ParameterDirection.Output, "", 200);//18
            cn.addparam("OS_ADD_REFERENCE", "string", ParameterDirection.Output, "", 200);//19
            cn.addparam("OL_GL_CODE", "int", ParameterDirection.Output);//20
            cn.addparam("OL_CURRENCY_CODE", "int", ParameterDirection.Output);//21
            cn.addparam("OL_LAST_NO", "int", ParameterDirection.Output);//22
            cn.addparam("P_CURSOR", "curs", ParameterDirection.Output);//23
            cn.addparam("OL_ERROR_CODE", "int", ParameterDirection.Output);//24
            cn.addparam("OS_ERROR_DESC", "string", ParameterDirection.Output, "", 200);//25
            OracleDataReader bal = null;
            try
            {
                bal = cn.query1();
            }
            catch (Exception ex)
            {
                Mylogger.Info("Error occured doing statement for acct " + r.account + " " + ex);
                MiniAcctStmtResp resp1 = new MiniAcctStmtResp
                {
                    responseCode = "-100005",
                    responseMessage = "",
                    skipProcessing = "false",
                    skipLog = "false"
                };
                json = JsonConvert.SerializeObject(resp1);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            string temp0 = cn.cmd.Parameters["@OL_BRANCH_CODE"].Value.ToString();
            string temp1 = cn.cmd.Parameters["@OS_BRANCH_NAME"].Value.ToString();
            string temp2 = cn.cmd.Parameters["@OS_ADD_REFERENCE"].Value.ToString();
            string temp3 = cn.cmd.Parameters["@OL_GL_CODE"].Value.ToString();
            string temp4 = cn.cmd.Parameters["@OL_CURRENCY_CODE"].Value.ToString();
            string temp5 = cn.cmd.Parameters["@OL_LAST_NO"].Value.ToString();
            //string temp6 = cn.cmd.Parameters["@P_CURSOR"].Value.ToString();
            string temp7 = cn.cmd.Parameters["@OL_ERROR_CODE"].Value.ToString();
            string temp8 = cn.cmd.Parameters["@OS_ERROR_DESC"].Value.ToString();

            StringBuilder finalresponse = new StringBuilder();
            if (bal.HasRows)
            {
                responseCode = "0";
                while (bal.Read())
                {
                    finalresponse.Append(g.formatData(bal.GetValue(2).ToString(), bal.GetValue(14).ToString(), bal.GetValue(13).ToString()) + "~");
                }
                bal.NextResult();
            }
            else
            {
                responseCode = "-100014";
            }
            cn.close();
            MiniAcctStmtResp resp = new MiniAcctStmtResp
            {
                responseCode = responseCode,
                responseMessage = finalresponse.ToString(),
                skipProcessing = "false",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(resp);
            return Request.CreateResponse(HttpStatusCode.OK, json);
        }

        /// <summary>
        /// API to get full account statements
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("FullAcctStmt")]
        public async Task<HttpResponseMessage> FullAcctStmt([FromBody] FullAcctStmtReq r)
        {
            //request type 227
            string responseCode = ""; DateTime dt1; DateTime dt2; string sdate = ""; string edate = "";
            dt1 = Convert.ToDateTime(r.startDate); string json = "";
            sdate = dt1.ToString("dd-MMM-yy").ToUpper();
            dt2 = Convert.ToDateTime(r.endDate);
            edate = dt2.ToString("dd-MMM-yy").ToUpper();
            List<FullAcctStmtResp> FullAcctRespList = new List<FullAcctStmtResp>();
            FullAcctStmtprop<FullAcctStmtResp> rsp = new FullAcctStmtprop<FullAcctStmtResp>();

            OraConntxn cn = new OraConntxn("IMAL.P_API_FULL_STATEMENT", true);
            cn.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, 4);//1
            cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, DBNull.Value, 200);//2
            cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, DBNull.Value, 200);//3
            cn.addparam("AL_API_CODE", "int", ParameterDirection.Input, 132);//4
            cn.addparam("AL_COMPANY", "int", ParameterDirection.Input, 1);//5
            cn.addparam("AS_ACCOUNT", "string", ParameterDirection.Input, r.account, 200);//6
            cn.addparam("AD_FROM_DATE", "string", ParameterDirection.Input, sdate);//7
            cn.addparam("AD_TO_DATE", "string", ParameterDirection.Input, edate);//8
            cn.addparam("AS_VT_FLAG", "string", ParameterDirection.Input, "V", 200);//9
            cn.addparam("AS_CARD", "string", ParameterDirection.Input, DBNull.Value, 200);//10
            cn.addparam("AL_CARD_PRESENT", "int", ParameterDirection.Input, 0);//11
            cn.addparam("AL_LAST_N", "int", ParameterDirection.Input, 100000000);//12
            cn.addparam("AL_REV_FLAG", "int", ParameterDirection.Input, 1);//13
            //output
            cn.addparam("P_CURSOR", "curs", ParameterDirection.Output);//14
            cn.addparam("ODEC_NET_BALANCE", "decimal", ParameterDirection.Output);//15
            cn.addparam("OL_RECORD_NO", "int", ParameterDirection.Output);//16
            cn.addparam("OL_ERROR", "int", ParameterDirection.Output);//17
            cn.addparam("OS_MESSAGE", "string", ParameterDirection.Output, "", 200);//18
            OracleDataReader dr = null;
            try
            {
                dr = cn.query1();
            }
            catch (Exception ex)
            {
            }

            string temp0 = cn.cmd.Parameters["@ODEC_NET_BALANCE"].Value.ToString();
            string temp1 = cn.cmd.Parameters["@OL_RECORD_NO"].Value.ToString();
            string temp2 = cn.cmd.Parameters["@OL_ERROR"].Value.ToString();
            string temp3 = cn.cmd.Parameters["@OS_MESSAGE"].Value.ToString();

            if (temp3 == null || temp3 == "")
            {
            }
            int rowcount = 0; string message = "";
            StringBuilder finalresponse = new StringBuilder();
            if (dr.HasRows)
            {
                responseCode = "0";
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (i != dr.FieldCount - 1)
                        {
                            DateTime dtv1 = new DateTime();
                            dtv1 = Convert.ToDateTime(dr.GetValue(9).ToString());
                            string vd = dtv1.ToString("MMM dd, yyyy");

                            DateTime dtv2 = new DateTime();
                            dtv2 = Convert.ToDateTime(dr.GetValue(8).ToString());
                            string td = dtv2.ToString("MMM dd, yyyy");

                            FullAcctRespList.Add(new FullAcctStmtResp()
                            {
                                companyCode = dr.GetValue(0).ToString(),
                                branchCode = dr.GetValue(1).ToString(),
                                currencyCode = dr.GetValue(2).ToString(),
                                glCode = dr.GetValue(3).ToString(),
                                customerNumber = dr.GetValue(4).ToString(),
                                subAccount = dr.GetValue(5).ToString(),
                                operationNumber = dr.GetValue(6).ToString(),
                                lineNumber = dr.GetValue(7).ToString(),
                                valueDate = vd,
                                baseCurrencyTransactionAmount = dr.GetValue(10).ToString(),
                                baseAccountTransactionAmount = dr.GetValue(11).ToString(),
                                balance = dr.GetValue(12).ToString(),
                                description = dr.GetValue(13).ToString(),
                                descriptionArab = dr.GetValue(14).ToString(),
                                descriptionArab1 = dr.GetValue(15).ToString(),
                                jvType = dr.GetValue(17).ToString(),
                                transactionType = dr.GetValue(18).ToString(),
                                openingBalance = dr.GetValue(19).ToString(),
                                ctsTransactionNumber = dr.GetValue(21).ToString(),
                                addDate = Convert.ToDateTime(dr.GetValue(22).ToString()).ToString("MMM dd, yyyy"),
                                skipProcessing = "false",
                                skipLog = "false",
                                transactionDate = td,
                            });
                        }
                    }
                }
                dr.NextResult();
                rsp.statements = FullAcctRespList;
                rsp.responseCode = "0";
                rsp.skipProcessing = "false";
                rsp.skipLog = "false";
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            else
            {
                rsp.statements = FullAcctRespList;
                responseCode = "-100014";
                rsp.skipProcessing = "false";
                rsp.skipLog = "false";
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
            cn.close();

            return Request.CreateResponse(HttpStatusCode.OK, "");
        }

        /// <summary>
        /// API to get customers address details
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CustomerAddress")]
        public async Task<HttpResponseMessage> CustomerAddress([FromBody] CustomerAddressReq r)
        {
            OraConntxn cn = new OraConntxn("IMAL.STERLING_P_CUSTOMER_ADDRESS", true);
            cn.addparam("BRANCHCODE", "int", ParameterDirection.Input, r.branchCode);//1
            cn.addparam("CUSTOMERNUMBER", "int", ParameterDirection.Input, r.customerNumber, 4);//2
            //output
            cn.addparam("ADDRESS", "string", ParameterDirection.Output, "", 200);//3
            cn.addparam("EMAIL", "string", ParameterDirection.Output, "", 200);//4
            cn.addparam("MOBILE", "string", ParameterDirection.Output, "", 200);//5

            try
            {
                cn.query();
            }
            catch (Exception ex)
            {
            }
            string responseCode = ""; string json = "";
            string temp0 = cn.cmd.Parameters["@ADDRESS"].Value.ToString();
            string temp1 = cn.cmd.Parameters["@EMAIL"].Value.ToString();
            string temp2 = cn.cmd.Parameters["@MOBILE"].Value.ToString();
            if (temp0 != null)
            {
                responseCode = "0";
            }
            else
            {
                responseCode = "";
            }
            CustomerAddressResp resp = new CustomerAddressResp
            {
                name = temp0,
                responseCode = responseCode,
                responseMessage = temp0,
                emailAddress = temp1,
                skipProcessing = "false",
                skipLog = "false"
            };
            json = JsonConvert.SerializeObject(resp);
            return Request.CreateResponse(HttpStatusCode.OK, json);
        }

        /// <summary>
        /// API to get Customers branch details
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CustBranchDetails")]
        public async Task<HttpResponseMessage> CustBranchDetails([FromBody] CustBranchDetailsReq r)
        {
            //234
            DataSet ds = new DataSet(); string json = "";
            string sql = @"select distinct b.branch_code,b.long_desc_eng from imal.amf a,branches b " +
                " where a.cif_sub_no=:cif_sub_no and a.comp_code=1 and b.comp_code=1 " +
                " and (a.branch_code= b.branch_code)";
            Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
            cn.SetSQL(sql);
            cn.AddParam(":cif_sub_no", r.customerNumber);
            //build a list
            List<CustBranchDetailsResp> custBranchDetailsRespList = new List<CustBranchDetailsResp>();
            CustBranchDetailsprop<CustBranchDetailsResp> rsp = new CustBranchDetailsprop<CustBranchDetailsResp>();
            try
            {
                ds = cn.Select();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        custBranchDetailsRespList.Add(new CustBranchDetailsResp() { branchCode = dr["branch_code"].ToString(), name = dr["long_desc_eng"].ToString() });
                    }
                    rsp.branches = custBranchDetailsRespList;

                    rsp.customerNumber = r.customerNumber;
                    rsp.responseCode = "0";
                    rsp.errorCode = "0";
                    rsp.skipProcessing = "false";
                    rsp.skipLog = "false";
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    rsp.branches = custBranchDetailsRespList;
                    rsp.customerNumber = r.customerNumber;
                    rsp.responseCode = "-100009";
                    rsp.errorCode = "-100009";
                    rsp.skipProcessing = "false";
                    rsp.skipLog = "false";
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                }
            }
            catch (Exception ex)
            {
                rsp.branches = custBranchDetailsRespList;
                rsp.customerNumber = r.customerNumber;
                rsp.responseCode = "-100011";
                rsp.errorCode = "-100011";
                rsp.skipProcessing = "false";
                rsp.skipLog = "false";
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "");
        }

        /// <summary>
        /// API to retrieve account details for customers
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RetrieveAcct")]
        public async Task<HttpResponseMessage> RetrieveAcct([FromBody] RetrieveAcctReq r)
        {
            //226
            DataSet ds = new DataSet(); string json = "";
            string sql = @"select a.long_name_eng,a.branch_code,a.cif_sub_no,a.currency_code,a.gl_code,a.sl_no, " +
                " a.status,a.additional_reference,a.cv_avail_bal,a.ytd_cv_bal,a.last_trans_date, " +
                " b.long_desc_eng,b.parent_gl from imal.amf a, gen_ledger b " +
                " where a.gl_code=b.gl_code and  a.cif_sub_no =:cif_sub_no ";
            Sterling.Oracle.Connect cn = new Sterling.Oracle.Connect("conn_imal");
            cn.SetSQL(sql);
            cn.AddParam(":cif_sub_no", r.customerNumber);
            List<RetrieveAcctResp> RetrieveAcctRespList = new List<RetrieveAcctResp>();
            RetrieveAcctprop<RetrieveAcctResp> rsp = new RetrieveAcctprop<RetrieveAcctResp>();
            try
            {
                ds = cn.Select();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        RetrieveAcctRespList.Add(new RetrieveAcctResp()
                        {
                            branchCode = dr["branch_code"].ToString(),
                            subAccountCode = dr["sl_no"].ToString(),
                            currencyCode = dr["currency_code"].ToString(),
                            glCode = dr["gl_code"].ToString(),
                            customerNumber = dr["cif_sub_no"].ToString(),
                            account = dr["additional_reference"].ToString(),
                            name = dr["long_name_eng"].ToString(),
                            status = dr["status"].ToString(),
                            availableBalance = dr["cv_avail_bal"].ToString(),
                            ledgerBalance = dr["ytd_cv_bal"].ToString(),
                            lastTransactionDate = dr["last_trans_date"].ToString(),
                            accountDescription = dr["long_desc_eng"].ToString()
                        });
                    }
                    rsp.accounts = RetrieveAcctRespList;
                    rsp.responseCode = "0";
                    rsp.skipProcessing = "false";
                    rsp.skipLog = "false";
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    rsp.accounts = RetrieveAcctRespList;
                    rsp.responseCode = "0";
                    rsp.skipProcessing = "false";
                    rsp.skipLog = "false";
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                }
            }
            catch (Exception ex)
            {
                rsp.accounts = RetrieveAcctRespList;
                rsp.responseCode = "0";
                rsp.skipProcessing = "false";
                rsp.skipLog = "false";
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
        }

        /// <summary>
        /// API for Token debit
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("TokenDebit")]
        public async Task<HttpResponseMessage> TokenDebit([FromBody] TokenDebitReq r)
        {
            string TokenDebit = ConfigurationManager.AppSettings["TokenDebit"].ToString();
            string theResp = ""; string rmks = ""; string[] bits = null;
            rmks = "Debit taken for Token charges"; string json = "";
            imalcore.ServicesSoapClient ws = new imalcore.ServicesSoapClient();
            //send the details to
            theResp = ws.FundstransFer(r.fromAccount, TokenDebit, decimal.Parse(r.amount), rmks);
            bits = theResp.Split('*');
            if (bits[0] == "00")
            {
                TokenDebitResp rsp = new TokenDebitResp
                {
                    availabeBalanceAfterOperation = bits[2],
                    responseCode = "0",
                    responseMessage = "Transaction was successful",
                    errorCode = "0",
                    errorMessage = "Transaction was successful",
                    iMALTransactionCode = "0",
                    skipProcessing = "false",
                    originalResponseCode = "0",
                    skipLog = "false"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            else
            {
                TokenDebitResp rsp = new TokenDebitResp
                {
                    availabeBalanceAfterOperation = "",
                    responseCode = bits[0],
                    responseMessage = bits[1],
                    errorCode = bits[0],
                    errorMessage = bits[1],
                    iMALTransactionCode = "0",
                    skipProcessing = "false",
                    originalResponseCode = bits[0],
                    skipLog = "false"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "");
        }

        /// <summary>
        /// API for Local funds transfer which includes IMAL to IMAL and IMAL to Sterling Bank
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("LocalFT")]
        public async Task<HttpResponseMessage> LocalFT([FromBody] LocalFTReq r)
        {
            Gadget g = new Gadget();
            string theResp = ""; string rmks = ""; string[] bits = null; string json = ""; string toAcct = "";
            string T24Account = "";
            string T24FromAcct = ConfigurationManager.AppSettings["T24FromAcct"].ToString();
            string ImalT24 = ConfigurationManager.AppSettings["T24IMALmirror"].ToString();
            try
            {
                if (string.IsNullOrEmpty(r.paymentReference))
                {
                    rmks = "Transfer of amount " + r.amount + " to " + r.beneficiaryName + " from " + r.fromAccount + "ref " + r.referenceCode;
                }
                else
                {
                    rmks = r.paymentReference;
                }
                imalcore.ServicesSoapClient ws = new imalcore.ServicesSoapClient();
                //first check if the account is sterling or imal
                sbpswitch.sbpswitchSoapClient sw = new sbpswitch.sbpswitchSoapClient();
                int bankid = sw.getInternalBankID(r.toAccount);
                if (bankid == 1)
                {
                    string FrmName = ""; string ToName = ""; string responseCode = ""; string responseText = "";
                    EACBS.banksSoapClient ea = new EACBS.banksSoapClient();
                    DataSet ds = ea.getAccountFullInfo(r.toAccount);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ToName = dr["cus_sho_name"].ToString();
                    }
                    else
                    {
                        FrmName = toAcct;
                        LocalFTResp rsp2 = new LocalFTResp
                        {
                            availabeBalanceAfterOperation = "",
                            responseCode = "-100030",
                            responseMessage = "Invalid beneficiary account number",
                            errorCode = "-100030",
                            errorMessage = "Invalid beneficiary account number",
                            iMALTransactionCode = "0",
                            skipProcessing = "false",
                            originalResponseCode = "-100030",
                            skipLog = "false"
                        };
                        json = JsonConvert.SerializeObject(rsp2);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                    }
                    //this is IMAL to T24 Sterling txn.
                    //get the T24 mirror account on imal to credit
                    toAcct = ImalT24;
                    T24Account = T24FromAcct;
                    theResp = ws.FundstransFer(r.fromAccount, toAcct, decimal.Parse(r.amount), rmks);
                    bits = theResp.Split('*');
                    if (bits[0] == "00")
                    {
                        //get name from imal
                        g.getImalCustNameByNuban(r.fromAccount);
                        FrmName = g.ImalCusName;

                        T24Bank bankCore = new T24Bank();
                        ITransactionResult result = null;
                        string transtype = "FT";
                        int transcodeselector = 999;
                        string appName = "Imal Api";
                        string bankcoreRemarks = "Transfer of " + r.amount + " from " + FrmName + " to " + ToName + "Ref: " + r.referenceCode + "";
                        result = bankCore.Transfer(T24FromAcct, r.toAccount, transtype, Convert.ToDecimal(r.amount), bankcoreRemarks, transcodeselector, appName);
                        if (result != null)
                        {
                            if (string.IsNullOrEmpty(result.TransactionReference))
                            {
                                // unsuccessful transfer

                            }
                        }
                        else
                        {
                            //unsuccessful transfer

                        }

                        //ibs1.BSServicesSoapClient ib = new ibs1.BSServicesSoapClient();
                        //StringBuilder rqt = new StringBuilder();
                        //StringBuilder rsp = new StringBuilder();
                        ////if successful then call ibsservice
                        //rqt.Clear();
                        //rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                        //rqt.Append("<IBSRequest>");
                        //rqt.Append("<ReferenceID>" + r.referenceCode + "</ReferenceID>");
                        //rqt.Append("<RequestType>" + "102" + "</RequestType>");
                        //rqt.Append("<FromAccount>" + T24FromAcct + "</FromAccount>");
                        //rqt.Append("<ToAccount>" + r.toAccount + "</ToAccount>");
                        //rqt.Append("<Amount>" + r.amount + "</Amount>");
                        //rqt.Append("<PaymentReference>" + "Transfer of " + r.amount + " from " + FrmName + " to " + ToName + "Ref " + r.referenceCode + "</PaymentReference>");
                        //rqt.Append("</IBSRequest>");
                        //string str = "";
                        //str = rqt.ToString();
                        //str = g.Encrypt(str, 26);

                        //string resp = ib.IBSBridge(str, 26);
                        //resp = g.Decrypt(resp, 26);
                        //XmlDocument xmlDoc = new XmlDocument();
                        //xmlDoc.LoadXml(resp);
                        //responseCode = xmlDoc.GetElementsByTagName("ResponseCode").Item(0).InnerText;
                        //responseText = xmlDoc.GetElementsByTagName("ResponseText").Item(0).InnerText;

                        LocalFTResp rsp1 = new LocalFTResp
                        {
                            availabeBalanceAfterOperation = bits[2],
                            responseCode = "0",
                            responseMessage = "Transaction was successful",
                            errorCode = "0",
                            errorMessage = "Transaction was successful",
                            iMALTransactionCode = bits[4],
                            skipProcessing = "false",
                            originalResponseCode = "0",
                            skipLog = "false",
                            transactionID = r.referenceCode
                        };
                        json = JsonConvert.SerializeObject(rsp1);
                        return Request.CreateResponse(HttpStatusCode.OK, json);
                    }
                    else
                    {
                        LocalFTResp rsp1 = new LocalFTResp
                        {
                            availabeBalanceAfterOperation = "",
                            responseCode = bits[0],
                            responseMessage = bits[1],
                            errorCode = bits[0],
                            errorMessage = bits[1],
                            iMALTransactionCode = "0",
                            skipProcessing = "false",
                            originalResponseCode = bits[0],
                            skipLog = "false",
                            transactionID = r.referenceCode
                        };
                        json = JsonConvert.SerializeObject(rsp1);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                    }
                }
                else//IMAL to IMAL txn
                {
                    toAcct = r.toAccount;
                    theResp = ws.FundstransFer(r.fromAccount, toAcct, decimal.Parse(r.amount), rmks);
                    bits = theResp.Split('*');
                    if (bits[0] == "00")
                    {
                        LocalFTResp rsp1 = new LocalFTResp
                        {
                            availabeBalanceAfterOperation = bits[2],
                            transactionID = r.referenceCode,
                            responseCode = "0",
                            responseMessage = "Transaction was successful",
                            errorCode = "0",
                            errorMessage = "Transaction was successful",
                            iMALTransactionCode = bits[4],
                            skipProcessing = "false",
                            originalResponseCode = "0",
                            skipLog = "false"
                        };
                        json = JsonConvert.SerializeObject(rsp1);
                        return Request.CreateResponse(HttpStatusCode.OK, json);
                    }
                    else
                    {
                        LocalFTResp rsp1 = new LocalFTResp
                        {
                            availabeBalanceAfterOperation = "",
                            responseCode = bits[0],
                            responseMessage = bits[1],
                            errorCode = bits[0],
                            errorMessage = bits[1],
                            iMALTransactionCode = "0",
                            skipProcessing = "false",
                            originalResponseCode = bits[0],
                            skipLog = "false"
                        };
                        json = JsonConvert.SerializeObject(rsp1);
                        return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "");
        }

        [HttpPost]
        [ActionName("NIPInwardFT")]
        public async Task<HttpResponseMessage> NIPInwardFT([FromBody] NIPInwardFT r)
        {
            MyEncryDecr myse = new MyEncryDecr();
            NIPInwardFT dd = new NIPInwardFT();
            int reqID = 36;
            string newNarration = "";
            string InwardKey = ConfigurationSettings.AppSettings["InwardKey"].ToString();
            string getTellerID = ConfigurationSettings.AppSettings["getTellerID"].ToString();
            string UserID = ConfigurationSettings.AppSettings["UserID"].ToString();
            string json = "";
            string jso = JsonConvert.SerializeObject(r);
            string SessionID = myse.Decrypt(r.SessionID, InwardKey);
            string OriginatorAccountName = myse.Decrypt(r.OriginatorAccountName, InwardKey);
            string BeneficiaryAccountNumber = myse.Decrypt(r.BeneficiaryAccountNumber, InwardKey);
            decimal ddd = Convert.ToDecimal(myse.Decrypt(r.Amount, InwardKey));
            int number = (int)ddd;
            string Amount = number.ToString();
            string PaymentReference = myse.Decrypt(r.PaymentReference, InwardKey);
            string Narrations = myse.Decrypt(r.Narration, InwardKey);
            string getFromAcct = dd.getFromAccount(reqID);
            //checking before process
            bool retCheck = dd.checkRequestAlreadyLogged(SessionID);
            if (!retCheck)
            {
                NIPInwardFTResp resp = new NIPInwardFTResp
                {
                    AvailabeBalanceAfterOperation = "",
                    iMALTransactionCode = "",
                    iMALRequestLogID = "",
                    ErrorCode = "",
                    ResponseCode = "94",
                    ResponseMessage = "Duplicate request.",
                };
                string jsonNews = JsonConvert.SerializeObject(resp);
                dd.UpdateRequestStatus(jsonNews, resp.ResponseCode, SessionID, resp.iMALTransactionCode);

                json = SessionID + ":" + resp.ResponseCode;
                json = myse.Encrypt(json, InwardKey);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            //log request since it hasn't been logged before
            dd.LogRequest(jso, SessionID, BeneficiaryAccountNumber, Amount, getFromAcct);

            Mylogger.Info("Inward transfer to " + BeneficiaryAccountNumber + " from " + OriginatorAccountName);
            if (!string.IsNullOrEmpty(Narrations))
            {
                string[] Narration = Narrations.Trim().Split('/');
                if (Narration.Length != 0)
                {
                    for (int i = 0; i < Narration.Length; i++)
                    {
                        if (i != 0)
                        {
                            newNarration += Narration[i] + "/";
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(newNarration))
            {
                newNarration = Narrations;
            }
            // check if currency codes match
            bool retCurCheck = false;

            retCurCheck = dd.checkCurrencyMatch(r);
            if (!retCurCheck)
            {
                NIPInwardFTResp resp = new NIPInwardFTResp
                {
                    AvailabeBalanceAfterOperation = "",
                    iMALTransactionCode = "",
                    iMALRequestLogID = "",
                    ErrorCode = "",
                    ResponseCode = "01",
                    ResponseMessage = "TO AND FRO ACCOUNT CURRENCY CODES DO NOT MATCH",
                };
                json = myse.Encrypt(SessionID + ":" + "01", InwardKey);
                string jsonN = JsonConvert.SerializeObject(resp);
                dd.UpdateRequestStatus(jsonN, resp.ResponseCode, SessionID, resp.iMALTransactionCode);

                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }


            OraConntxn cn = new OraConntxn("imal.p_api_transfer_ex3", true);
            cn.addparam("al_channel_id", "int", ParameterDirection.Input, 2);
            cn.addparam("as_user_id", "string", ParameterDirection.Input, UserID);//2 gettellername
            cn.addparam("as_machine_name", "string", ParameterDirection.Input, "1");//3
            cn.addparam("al_api_code", "int", ParameterDirection.Input, 59);//4  api code (54:Bill Payment, 205:transfer own ,account,59: transfer 3rd, account,60:topup,61:e-share
            cn.addparam("al_comp_code", "int", ParameterDirection.Input, 1);//5
            cn.addparam("al_branch", "int", ParameterDirection.Input, 1); //6
            cn.addparam("al_teller", "int", ParameterDirection.Input, getTellerID);//7
            cn.addparam("al_trxtype", "int", ParameterDirection.Input, 26);//8
            cn.addparam("al_use_card_accno", "int", ParameterDirection.Input, 0);//9 use card account number (0:use from_account and to_account, 1: use card_no_primary account
            cn.addparam("as_transaction_type", "string", ParameterDirection.Input, "C");//10 c for credit, d for debit.used when al_use_card_accno = 1
            cn.addparam("as_card", "string", ParameterDirection.Input, DBNull.Value); //);// card-no mandatory in case 11
            cn.addparam("al_acc_br", "int", ParameterDirection.Input, DBNull.Value);//12
            cn.addparam("al_acc_cy", "int", ParameterDirection.Input, DBNull.Value);//13
            cn.addparam("al_acc_gl", "int", ParameterDirection.Input, DBNull.Value);//14
            cn.addparam("al_acc_cif", "int", ParameterDirection.Input, DBNull.Value);//15
            cn.addparam("al_acc_sl", "int", ParameterDirection.Input, DBNull.Value);//16
            cn.addparam("as_account", "string", ParameterDirection.Input, getFromAcct);//17
            cn.addparam("al_to_acc_br", "int", ParameterDirection.Input, DBNull.Value);//18
            cn.addparam("al_to_acc_cy", "int", ParameterDirection.Input, DBNull.Value);//19
            cn.addparam("al_to_acc_gl", "int", ParameterDirection.Input, DBNull.Value);//20 gl_code

            string referenceNo = dd.truncateToFitField(newNarration, 50);
            cn.addparam("al_to_acc_cif", "int", ParameterDirection.Input, DBNull.Value);//21 cif_sub_no
            cn.addparam("al_to_acc_sl", "int", ParameterDirection.Input, DBNull.Value);//22 sl_no
            cn.addparam("as_toaccount", "string", ParameterDirection.Input, BeneficiaryAccountNumber);//23
            cn.addparam("adec_amount", "int", ParameterDirection.Input, Amount);//24
            cn.addparam("as_currency", "string", ParameterDirection.Input, "566");//25
            cn.addparam("as_date_time", "Date", ParameterDirection.Input, DateTime.Now.ToString("dd-MMM-yyyy"));//26dd.getFormattedDate(DateTime.Now.Date)
            cn.addparam("as_reference", "string", ParameterDirection.Input, referenceNo);//27r.getreferencecode
            cn.addparam("al_pos", "int", ParameterDirection.Input, 0);//28
            cn.addparam("as_desc", "string", ParameterDirection.Input, dd.truncateToFitField(Narrations));//29
            cn.addparam("as_desc_arab", "string", ParameterDirection.Input, referenceNo);//30
            cn.addparam("adt_value_date", "Date", ParameterDirection.Input, DateTime.Now.ToString("dd-MMM-yyyy"));//31
            cn.addparam("as_biller_code", "string", ParameterDirection.Input, DBNull.Value);//32
            cn.addparam("as_so_reference", "string", ParameterDirection.Input, DBNull.Value);//33
            cn.addparam("al_add_number", "int", ParameterDirection.Input, 0);//34
            cn.addparam("al_num_of_shares", "int", ParameterDirection.Input, DBNull.Value);//35
            cn.addparam("as_instructions1", "string", ParameterDirection.Input, PaymentReference);//36
            cn.addparam("as_instructions2", "string", ParameterDirection.Input, DBNull.Value);//37
            cn.addparam("as_instructions3", "string", ParameterDirection.Input, DBNull.Value);//38
            cn.addparam("as_instructions4", "string", ParameterDirection.Input, DBNull.Value);//39
            cn.addparam("al_trx_purpose", "int", ParameterDirection.Input, DBNull.Value);//40 as_trx_purpose (mandatory when api code in 54,70)
            cn.addparam("as_approved_trx", "string", ParameterDirection.Input, 1);//41 as_approved_trx 1:approved, 0: not approved
            cn.addparam("ol_share_ref", "string", ParameterDirection.Output, "", 4000);//42
            cn.addparam("odec_avail", "string", ParameterDirection.Output, "", 4000);//43
            cn.addparam("ol_trx_code", "string", ParameterDirection.Output, "", 4000);//44
            cn.addparam("ol_transaction_id", "string", ParameterDirection.Output, "", 4000);//45
            cn.addparam("ol_error_code", "string", ParameterDirection.Output, "", 4000);//46
            cn.addparam("os_message", "string", ParameterDirection.Output, "", 4000);//47
            try
            {
                cn.query();
            }
            catch (Exception ex)
            {
                NIPInwardFTResp resp = new NIPInwardFTResp
                {
                    AvailabeBalanceAfterOperation = "",
                    iMALTransactionCode = "",
                    iMALRequestLogID = "",
                    ErrorCode = "",
                    ResponseCode = "97",
                    ResponseMessage = cn.cmd.Parameters["@os_message"].Value.ToString(),
                };
                string jsonNews = JsonConvert.SerializeObject(resp);
                //update log
                dd.UpdateRequestStatus(jsonNews, resp.ResponseCode, SessionID, "0");
                json = myse.Encrypt(SessionID + ":" + resp.ResponseCode, InwardKey);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
            NIPInwardFTResp resps = new NIPInwardFTResp
            {
                AvailabeBalanceAfterOperation = cn.cmd.Parameters["@ol_share_ref"].Value.ToString(),
                iMALTransactionCode = cn.cmd.Parameters["@odec_avail"].Value.ToString(),
                iMALRequestLogID = cn.cmd.Parameters["@ol_trx_code"].Value.ToString(),
                ErrorCode = cn.cmd.Parameters["@ol_error_code"].Value.ToString(),
                ResponseCode = cn.cmd.Parameters["@ol_error_code"].Value.ToString(),
                ResponseMessage = cn.cmd.Parameters["@os_message"].Value.ToString(),
            };
            string jsonNew = JsonConvert.SerializeObject(resps);
            //update log
            json = myse.Encrypt(SessionID + ":" + cn.cmd.Parameters["@ol_error_code"].Value.ToString() + ":" + cn.cmd.Parameters["@ol_trx_code"].Value.ToString(), InwardKey);
            dd.UpdateRequestStatus(jsonNew, cn.cmd.Parameters["@ol_error_code"].Value.ToString(), SessionID, cn.cmd.Parameters["@ol_trx_code"].Value.ToString());
            return Request.CreateResponse(HttpStatusCode.OK, json);
        }

        /// <summary>
        /// API for NIBSS Name enquiry (NIBSS)
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("NIPNameEnquiry")]
        public async Task<HttpResponseMessage> NameEnquiry([FromBody] NameEnquiryReq r)
        {
            string sid = ""; Gadget g = new Gadget(); string gen3 = ""; string[] bits = null;
            gen3 = g.GenerateRndNumber(3); string json = "";
            try
            {
                NIBSSService.sbp_outward_transSoapClient nip = new NIBSSService.sbp_outward_transSoapClient();
                sid = g.newSessionGlobal(gen3, 2);
                //do the name enquiry
                string theResp = nip.NameEnquiryIMAL(sid, r.destinationBankCode, r.channelCode, r.account);
                bits = theResp.Split(':');
                if (bits[0] == "00")
                {
                    NameEnquiryResp rsp = new NameEnquiryResp
                    {
                        nameDetails = bits[1],
                        responseCode = "0",
                        errorCode = "0",
                        skipProcessing = "false",
                        originalResponseCode = "0",
                        skipLog = "false",
                        neSid = sid
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    NameEnquiryResp rsp = new NameEnquiryResp
                    {
                        nameDetails = bits[1],
                        responseCode = bits[0],
                        errorCode = bits[0],
                        skipProcessing = "false",
                        originalResponseCode = bits[0],
                        skipLog = "false",
                        neSid = sid
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                }
            }
            catch (Exception ex)
            {
                NameEnquiryResp rsp = new NameEnquiryResp
                {
                    nameDetails = "-1",
                    responseCode = "-1",
                    errorCode = ex.ToString(),
                    skipProcessing = "false",
                    originalResponseCode = "-1",
                    skipLog = "false",
                    neSid = sid
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
        }

        /// <summary>
        /// API for Inter-bank Funds Transfer (NIBSS)
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("NIPFundsTransfer")]
        public async Task<HttpResponseMessage> FundsTransfer([FromBody] NipFundsTransferReq r)
        {
            Gadget g = new Gadget(); string json = ""; string category = ""; string currency_code = "";
            string[] bits = null; string cusname = ""; string status = ""; decimal bal = 0; string bvn = "";
            decimal dailyminlimit = 0; bool nipVatFee = false; TotaldonePerday Tdp1 = new TotaldonePerday();
            bool isFound = false; CheckSavingsLedger cl = new CheckSavingsLedger();
            //get customer details
            string details = g.getImalCustNameByNuban(r.fromAccount);
            long txid = 0;
            bits = details.Split('*'); int cus_class = 0;
            string IMALTSSAcct = ConfigurationManager.AppSettings["IMALTSSAcct"].ToString();
            string IMALFeeAcct = ConfigurationManager.AppSettings["IMALFeeAcct"].ToString();
            string IMALVatAcct = ConfigurationManager.AppSettings["IMALVatAcct"].ToString();
            string Indiv = ConfigurationManager.AppSettings["Indiv"].ToString();
            string Corp = ConfigurationManager.AppSettings["Corp"].ToString();
            string AllowedLedgers = ConfigurationManager.AppSettings["AllowedLedgers"].ToString();
            decimal MaxpertxnIndvi = decimal.Parse(ConfigurationManager.AppSettings["MaxpertxnIndvi"].ToString());
            cusname = bits[0]; status = bits[1]; bal = decimal.Parse(bits[2]); category = bits[3];
            currency_code = bits[4]; bvn = bits[5];
            //r.nesid = r.referenceCode;
            long logval = g.SaveRequest(r);
            string custid = ""; string branchCode = "";
            ImalDetails imalinfo = g.GetImalDetailsByNuban(r.fromAccount);
            custid = imalinfo.CustId;
            branchCode = imalinfo.Branchcode;
            //check if the customer is account is active
            if (status != "A")
            {
                return Request.AccountNotActive(logval);
            }
            //check for sufficient balance
            if (decimal.Parse(r.amount) > bal)
            {
                return Request.CheckSuffiBalance(logval);
            }
            //get the class of the customer
            if (Indiv.Contains(category))
            {
                cus_class = 1; //individual
            }
            else if (Corp.Contains(category))
            {
                cus_class = 2;//category
            }
            //check the amount being transfered by individual
            if (cus_class == 1 && decimal.Parse(r.amount) > MaxpertxnIndvi)
            {
                return Request.CheckIndividualPerTrans(logval);
            }
            //check if the currency code is naira
            if (currency_code != "566")
            {
                return Request.CheckCurrency(logval);
            }

            //check for ledgers and bounce if not allowed
            if (AllowedLedgers.Contains(category))
            {
                //proceed
            }
            else
            {
                return Request.TransNotPermited(logval);
            }
            //check to ensure that amount is not equal to or less than 0
            decimal maxPerTrans = 0; decimal maxPerday = 0;
            if (decimal.Parse(r.amount) == 0 || decimal.Parse(r.amount) < 0)
            {
                return Request.ZeroAmtorLess(logval);
            }

            //check if customer has concession
            bool HasConcessionPerTransPerday = g.getMaxperTransPerday(r.fromAccount);
            if (HasConcessionPerTransPerday)
            {
                //get the list of savings account from imal team
                isFound = cl.isLedgerFound(category);
                if (isFound)
                {
                    CheckSavingsLedger EFTamt = new CheckSavingsLedger();
                    EFTamt.getMaxEFTAmt();
                    maxPerTrans = EFTamt.maxpertran;
                    maxPerday = EFTamt.maxperday;
                }
                else
                {
                    maxPerTrans = g.maxPerTrans;
                    maxPerday = g.maxPerday;
                }
                bool totalfound = Tdp1.getTotalTransDonePerday(maxPerday, decimal.Parse(r.amount), r.fromAccount);
                //check if the customer has exceeded the max per trans
                if (decimal.Parse(r.amount) > maxPerTrans)
                {
                    return Request.CheckmaxPerTrans(logval);
                }
                //check if the customer has exceeded max per day
                if (decimal.Parse(r.amount) + Tdp1.Totaldone > maxPerday)
                {
                    decimal my1sum = 0;
                    my1sum = decimal.Parse(r.amount) + Tdp1.Totaldone;
                    return Request.CheckmaxPerDay(logval);
                }
                //check if the balance in customers account can accomodate the principal + fee + vat
                nipVatFee = g.getNIPFeeandVat(decimal.Parse(r.amount));
                if (nipVatFee)
                {
                    decimal total_amt_debit = 0;
                    total_amt_debit = decimal.Parse(r.amount) + g.NIPfee + g.NIPvat;
                    if (total_amt_debit > bal)
                    {
                        //insufficient
                        return Request.CheckSuffiBalance(logval);
                    }
                    else
                    {
                        //proceed
                    }
                }
                else
                {
                    return Request.UnableToComputeVatFee(logval);
                }
                //at this stage the customer details are fine to proceed for debit
                string[] bitRsp = null;
                string gen3 = g.GenerateRndNumber(3);
                string sid = g.newSessionGlobal(gen3, 2);
                string debit_cust = g.ImalDebit(r, IMALTSSAcct, IMALFeeAcct, IMALVatAcct, sid, logval);
                bitRsp = debit_cust.Split(':');
                if (bitRsp[0] == "00")
                {
                    //update the txn table

                    //proceed and send to NIBSS
                    NIBSSService.sbp_outward_transSoapClient nip = new NIBSSService.sbp_outward_transSoapClient();
                    string ResponseCode = nip.FT_SendtoNIBSSIMAL(r.nesid, branchCode, custid, currency_code, category, "1", r.amount, g.NIPfee.ToString(), r.customerShowName,
                        r.destinationBankCode, r.channelCode, r.beneficiaryName, r.toAccount, r.paymentReference, sid, r.beneBVN, bvn, r.fromAccount, g.NIPvat.ToString());
                    if (ResponseCode == "00")
                    {
                        return Request.SuccessfulNIP(logval, ResponseCode);
                    }
                    else
                    {
                        string rspstmt = "";
                        switch (ResponseCode)
                        {
                            case "03": //txt = "Invalid sender"; break;
                            case "05": //txt = "Do not honor"; break;
                            case "06": //txt = "Dormant account"; break;
                            case "07": //txt = "Invalid account"; break;
                            case "08": //txt = "Account name mismatch"; break;
                            case "09": //txt = "Request processing in progress"; break;
                            case "12": //txt = "Invalid transaction"; break;
                            case "13": //txt = "Invalid amount"; break;
                            case "14": //txt = "Invalid Batch Number"; break;
                            case "15": //txt = "Invalid Session or Record ID"; break;
                            case "16": //txt = "Unknown Bank Code"; break;
                            case "17": //txt = "Invalid Channel"; break;
                            case "18": //txt = "Wrong Method Call"; break;
                            case "21": //txt = "No action taken"; break;
                            case "25": //txt = "Unable to locate record"; break;
                            case "26": //txt = "Duplicate record"; break;
                            case "30": //txt = "Wrong destination account format"; break;
                            case "34": //txt = "Suspected fraud"; break;
                            case "35": //txt = "Contact sending bank"; break;
                            case "51": //txt = "No sufficient funds"; break;
                            case "57": //txt = "Transaction not permitted to sender"; break;
                            case "58": //txt = "Transaction not permitted on channel"; break;
                            case "61": //txt = "Transfer Limit Exceeded"; break;
                            case "63": //txt = "Security violation"; break;
                            case "65": //txt = "Exceeds withdrawal frequency"; break;
                            case "68": //txt = "Response received too late"; break;
                            case "91": //txt = "Beneficiary Bank not available"; break;
                            case "92": //txt = "Routing Error"; break;
                            case "94": //txt = "Duplicate Transaction"; break;
                            case "96": //txt = "Corresponding Bank is currently offline."; break;
                            case "97": //txt = "Timeout waiting for response from destination."; break;
                                rspstmt = ResponseCode;
                                g.updateNIPCode(ResponseCode, logval);
                                g.NIPReversal(logval, g.amtxRef, g.feetxRef, g.vattxRef);
                                //new ErrorLog("IBS Reversal was done==>" + rsp);
                                break;

                            case "1x":
                            default:
                                rspstmt = ResponseCode;// "5";
                                break;
                        }
                        return Request.UNSuccessfulNIP(logval, ResponseCode);
                    }
                }
                else
                {
                    if (bits[0] == "0D")
                    {
                        return Request.UnableToComputeVatFee(logval);
                    }
                    else if (bits[0] == "0C")
                    {
                        return Request.UnabletoDebitVat(logval);
                    }
                    else if (bits[0] == "0B")
                    {
                        return Request.UnabletoDebitFee(logval);
                    }
                    else if (bits[0] == "0A")
                    {
                        return Request.UnabletoDebitPrincipal(logval);
                    }
                }//end of else
            }//end for concession customers
            else
            {
                //for people without concession
                Tdp1.getTotalTransDonePerday(maxPerday, decimal.Parse(r.amount), r.fromAccount);
                DataSet dsl = g.getCBNamt(cus_class);
                DataRow drl;
                if (cus_class == 1)
                {
                    if (dsl.Tables[0].Rows.Count > 0)
                    {
                        drl = dsl.Tables[0].Rows[0];
                        maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                        maxPerday = decimal.Parse(drl["maxamt"].ToString());
                    }
                    dailyminlimit = maxPerTrans;
                    //check if the ledger is for savings
                    isFound = cl.isLedgerFound(category);
                    if (isFound)
                    {
                        CheckSavingsLedger EFTamt = new CheckSavingsLedger();
                        EFTamt.getMaxEFTAmt();
                        maxPerTrans = EFTamt.maxpertran;
                        maxPerday = EFTamt.maxperday;
                    }
                }
                else if (cus_class == 2)
                {
                    if (dsl.Tables[0].Rows.Count > 0)
                    {
                        drl = dsl.Tables[0].Rows[0];
                        maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                        maxPerday = decimal.Parse(drl["maxamt"].ToString());
                    }
                    dailyminlimit = maxPerTrans;
                }
                else if (cus_class == 3)//Enterprise and Government
                {
                    if (dsl.Tables[0].Rows.Count > 0)
                    {
                        drl = dsl.Tables[0].Rows[0];
                        maxPerTrans = decimal.Parse(drl["minamt"].ToString());
                        maxPerday = decimal.Parse(drl["maxamt"].ToString());
                    }
                    dailyminlimit = maxPerTrans;
                }
                //ensure that the maxper trans and maxperday is not 0
                if (maxPerTrans == 0 || maxPerday == 0)
                {
                    return Request.TransNotPermited(logval);
                }

                //this is to ensure that customers will not exceed the daily cbn limit
                if (decimal.Parse(r.amount) <= maxPerTrans)
                {
                    if (decimal.Parse(r.amount) + Tdp1.Totaldone <= maxPerday)
                    {
                        //proceed
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                    }
                }
                else
                {
                    return Request.ExceedAllowedCBNlimit(logval);
                }
                //check if the balance in customers account can accomodate the principal + fee + vat
                nipVatFee = g.getNIPFeeandVat(decimal.Parse(r.amount));
                if (nipVatFee)
                {
                    decimal total_amt_debit = 0;
                    total_amt_debit = decimal.Parse(r.amount) + g.NIPfee + g.NIPvat;
                    if (total_amt_debit > bal)
                    {
                        return Request.CheckSuffiBalance(logval);
                    }
                    else
                    {
                        //proceed
                    }
                }
                else
                {
                    return Request.UnableToComputeVatFee(logval);
                }

                //checks for tia1 accounts
                //at this stage the customer details are fine to proceed for debit
                string[] bitRsp = null;
                string gen3 = g.GenerateRndNumber(3);
                string sid = g.newSessionGlobal(gen3, 2);
                string debit_cust = g.ImalDebit(r, IMALTSSAcct, IMALFeeAcct, IMALVatAcct, sid, logval);
                bitRsp = debit_cust.Split(':');
                if (bitRsp[0] == "00")//successful debit within IMAL core
                {
                    //proceed and send to NIBSS
                    NIBSSService.sbp_outward_transSoapClient nip = new NIBSSService.sbp_outward_transSoapClient();
                    string ResponseCode = nip.FT_SendtoNIBSSIMAL(r.nesid, branchCode, custid, currency_code, category, "1", r.amount, g.NIPfee.ToString(), r.customerShowName,
                        r.destinationBankCode, r.channelCode, r.beneficiaryName, r.toAccount, r.paymentReference, sid, r.beneBVN, bvn, r.fromAccount, g.NIPvat.ToString());
                    //string ResponseCode = nip.FT_SendtoNIBSSIMAL(r.nesid, "", "", "", category, "", r.amount, g.NIPfee.ToString(), r.customerShowName,
                    //    r.destinationBankCode, r.channelCode, r.TransType, r.toAccount, r.paymentReference, sid, r.beneBVN, bvn);
                    if (ResponseCode == "00")
                    {
                        return Request.SuccessfulNIP(logval, ResponseCode);
                    }
                    else
                    {
                        string rspstmt = "";
                        switch (ResponseCode)
                        {
                            case "03": //txt = "Invalid sender"; break;
                            case "05": //txt = "Do not honor"; break;
                            case "06": //txt = "Dormant account"; break;
                            case "07": //txt = "Invalid account"; break;
                            case "08": //txt = "Account name mismatch"; break;
                            case "09": //txt = "Request processing in progress"; break;
                            case "12": //txt = "Invalid transaction"; break;
                            case "13": //txt = "Invalid amount"; break;
                            case "14": //txt = "Invalid Batch Number"; break;
                            case "15": //txt = "Invalid Session or Record ID"; break;
                            case "16": //txt = "Unknown Bank Code"; break;
                            case "17": //txt = "Invalid Channel"; break;
                            case "18": //txt = "Wrong Method Call"; break;
                            case "21": //txt = "No action taken"; break;
                            case "25": //txt = "Unable to locate record"; break;
                            case "26": //txt = "Duplicate record"; break;
                            case "30": //txt = "Wrong destination account format"; break;
                            case "34": //txt = "Suspected fraud"; break;
                            case "35": //txt = "Contact sending bank"; break;
                            case "51": //txt = "No sufficient funds"; break;
                            case "57": //txt = "Transaction not permitted to sender"; break;
                            case "58": //txt = "Transaction not permitted on channel"; break;
                            case "61": //txt = "Transfer Limit Exceeded"; break;
                            case "63": //txt = "Security violation"; break;
                            case "65": //txt = "Exceeds withdrawal frequency"; break;
                            case "68": //txt = "Response received too late"; break;
                            case "91": //txt = "Beneficiary Bank not available"; break;
                            case "92": //txt = "Routing Error"; break;
                            case "94": //txt = "Duplicate Transaction"; break;
                            case "96": //txt = "Corresponding Bank is currently offline."; break;
                            case "97": //txt = "Timeout waiting for response from destination."; break;
                                rspstmt = ResponseCode;
                                g.updateNIPCode(ResponseCode, logval);
                                g.NIPReversal(logval, g.amtxRef, g.feetxRef, g.vattxRef);
                                break;

                            case "1x":
                            default:
                                rspstmt = ResponseCode;// "5";
                                break;
                        }
                        return Request.UNSuccessfulNIP(logval, ResponseCode);
                    }
                }
                else
                {
                    if (bits[0] == "0D")
                    {
                        return Request.UnableToComputeVatFee(logval);
                    }
                    else if (bits[0] == "0C")
                    {
                        return Request.UnabletoDebitVat(logval);
                    }
                    else if (bits[0] == "0B")
                    {
                        return Request.UnabletoDebitFee(logval);
                    }
                    else if (bits[0] == "0A")
                    {
                        return Request.UnabletoDebitPrincipal(logval);
                    }
                }//end of else
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "");
        }

        /// <summary>
        /// API for transaction Reversals
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("FTReversal")]
        public async Task<HttpResponseMessage> FTReversal([FromBody] FTReversalReq r)
        {
            string txn = r.referenceCode; string json = "";
            //get the request, process and respond
            OraConntxn cn = new OraConntxn("imal.P_API_REVERSE_TRX", true);
            //input
            cn.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, 1);//1
            cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, "ATMT", 200);//user id 2
            cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, "1", 200);//3
            cn.addparam("AL_API_CODE", "int", ParameterDirection.Input, 218, 200);//4
            cn.addparam("AL_COMP_CODE", "int", ParameterDirection.Input, 1, 200);//5
            cn.addparam("AL_BRANCH_CODE", "int", ParameterDirection.Input, 1, 200);//6
            cn.addparam("AL_TRSNO", "int", ParameterDirection.Input, r.referenceCode, 200);//7
            cn.addparam("AS_TRS_REFERENCE", "string", ParameterDirection.Input, DBNull.Value, 200);//8

            //output
            cn.addparam("OL_OP_NO", "int", ParameterDirection.Output);//9
            cn.addparam("OL_ERROR_CODE", "int", ParameterDirection.Output);//10
            cn.addparam("OS_ERROR_DESC", "string", ParameterDirection.Output, "", 4000);//11

            try
            {
                cn.query();
            }
            catch (Exception ex)
            {
            }
            //Read the output parameter after execution
            string temp0 = cn.cmd.Parameters["@OL_OP_NO"].Value.ToString();
            string temp1 = cn.cmd.Parameters["@OL_ERROR_CODE"].Value.ToString();
            string temp2 = cn.cmd.Parameters["@OS_ERROR_DESC"].Value.ToString();
            if (temp1 == "0")
            {
                FTReversalResp rsp = new FTReversalResp
                {
                    responseCode = "0",
                    errorCode = "0",
                    skipProcessing = "false",
                    skipLog = "false"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            else
            {
                FTReversalResp rsp = new FTReversalResp
                {
                    responseCode = "0",
                    errorCode = "0",
                    skipProcessing = "false",
                    skipLog = "false"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
        }

        /// <summary>
        /// API to create IMAL customer/CIF
        /// </summary>
        /// <remarks> AppName accepts max 8 characters</remarks>
        [HttpPost]
        [ActionName("CreateCIF")]
        [ResponseType(typeof(CreateCIFResp))]
        public async Task<HttpResponseMessage> CreateCIF([FromBody] CreateCIFReq r)
        {
            string txn = r.referenceCode; string json = ""; string hostName = "";
            Gadget g = new Gadget();
            //get the request, process and respond
            OraConntxn cn = new OraConntxn("imal.P_API_CREATE_CIF", true);
            try
            {
                DateTime dt1 = DateTime.Now.Date; string cdate = ""; string fname = r.FirstName + " " + r.LastName;
                //string completeName = r.FirstName + " " + r.MiddleName + " "+ r.LastName;
                string dob = "";
                DateTime DoB = r.DOB.Date;
                dob = DoB.ToString("dd-MMM-yy").ToUpper();
                cdate = dt1.ToString("dd-MMM-yy").ToUpper();
                hostName = System.Net.Dns.GetHostName();

                if (!ModelState.IsValid)
                {
                    CreateCIFResp rsp = new CreateCIFResp
                    {
                        responseCode = "99",
                        errorCode = "Invalid request. Check parameters",
                        skipProcessing = "false",
                        skipLog = "Invalid request. Check parameters"
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
                }

                //Check if customer has CIF already
                string cifNo = g.CheckForCIF(r.BVN.Trim());
                if (!string.IsNullOrEmpty(cifNo))
                {
                    //Return same CifNo to use to create account.
                    CreateCIFResp rsp = new CreateCIFResp
                    {
                        responseCode = "0",
                        errorCode = "CIF already exists",
                        skipProcessing = "false",
                        skipLog = "false",
                        cifNo = cifNo
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);

                }

                cn.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, 1);//1
                //cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, "333", 8);//2
                cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, r.AppName, 8);//2
                cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, hostName, 40);//3
                cn.addparam("AL_API_CODE", "int", ParameterDirection.Input, 107);//4
                cn.addparam("AL_COMP_CODE", "int", ParameterDirection.Input, 1);//5
                cn.addparam("AL_BRANCH_CODE", "int", ParameterDirection.Input, r.BranchCode);//6 Branch code
                cn.addparam("AL_CIF_TYPE", "int", ParameterDirection.Input, 1);//7 CIF Type Should be an iMAL value
                cn.addparam("AL_ID_TYPE", "int", ParameterDirection.Input, 3);//8 CIF ID type Should be an iMAL value
                cn.addparam("ADT_ESTAB_DATE", "string", ParameterDirection.Input, dob);//9 (Establishment DATE IF CIF OF a company || Birth date if CIF OF individual)
                cn.addparam("AS_SHORT_NAME_ENG", "string", ParameterDirection.Input, fname);//10
                cn.addparam("AS_SHORT_NAME_ARAB", "string", ParameterDirection.Input, fname);//11
                cn.addparam("AS_LONG_NAME_ENG", "string", ParameterDirection.Input, fname);//12
                cn.addparam("AS_LONG_NAME_ARAB", "string", ParameterDirection.Input, fname);//13
                cn.addparam("AS_ID_NO", "string", ParameterDirection.Input, r.MeansOfIDNo);//14 CIF ID number//A05569876A
                cn.addparam("AS_LANGUAGE", "string", ParameterDirection.Input, DBNull.Value, 200);//15 Language ‘E’: English ‘A’:defaulted to English if argument is null
                cn.addparam("AL_NATION_CODE", "int", ParameterDirection.Input, 566);//16 Nation code Should be an iMAL value
                cn.addparam("AL_COUNTRY_CODE", "int", ParameterDirection.Input, 566);//17 Country code
                cn.addparam("AL_PRIORITY_CODE", "int", ParameterDirection.Input, 1);//18 Priority code
                cn.addparam("AS_RESIDENT", "string", ParameterDirection.Input, DBNull.Value, 200);//19
                cn.addparam("AL_CIVIL_CODE", "int", ParameterDirection.Input, 22);//20
                cn.addparam("AS_CREATED_BY", "string", ParameterDirection.Input, r.AppName, 200);//21
                cn.addparam("AL_DEPT", "int", ParameterDirection.Input, 102);//22
                cn.addparam("AL_DIVISION", "int", ParameterDirection.Input, 1);//23
                cn.addparam("AL_ECO_SECTOR", "int", ParameterDirection.Input, 99);//24
                cn.addparam("AS_FIRST_NAME_ENG", "string", ParameterDirection.Input, r.FirstName);//25 First name english
                cn.addparam("AS_LAST_NAME_ENG", "string", ParameterDirection.Input, r.LastName);//26 last name english
                cn.addparam("AS_TEL", "string", ParameterDirection.Input, r.Telephone);//27 Telephone
                cn.addparam("AS_FIRST_NAME_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//28
                cn.addparam("AS_SEC_NAME_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//29
                cn.addparam("AS_LAST_NAME_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//30
                cn.addparam("AS_ADDRESS1_ENG", "string", ParameterDirection.Input, r.Address1);//31  Address1
                cn.addparam("AS_ADDRESS2_ENG", "string", ParameterDirection.Input, DBNull.Value, 200);//32 Address2
                cn.addparam("AS_ADDRESS3_ENG", "string", ParameterDirection.Input, DBNull.Value, 200);//33 Address3
                cn.addparam("AS_AUTH_ID", "string", ParameterDirection.Input, DBNull.Value, 200);//34
                cn.addparam("AS_AUTH_NAME", "string", ParameterDirection.Input, DBNull.Value, 200);//35
                cn.addparam("AS_SEC_NAME_ENG", "string", ParameterDirection.Input, DBNull.Value, 200);//36 Sector name 
                cn.addparam("AS_REL_OFFICER", "string", ParameterDirection.Input, DBNull.Value, 200);//37 Relation officer
                cn.addparam("ADT_DATE_CREATED", "string", ParameterDirection.Input, cdate, 200);//38
                cn.addparam("ADT_DATE_MODIFIED", "string", ParameterDirection.Input, DBNull.Value, 200);//39
                cn.addparam("AS_STATUS", "string", ParameterDirection.Input, "A");//40 // Status ‘C’: Created ‘A’: Approved
                cn.addparam("AS_TYPE", "string", ParameterDirection.Input, r.customerType);//41 // Type ‘V’: Individual ‘T’: Institution
                cn.addparam("AS_KYC_COMPLETED", "string", ParameterDirection.Input, DBNull.Value, 200);//42 // KYC completed ‘Y’: Yes ‘N’:No
                cn.addparam("AS_MARITAL_STATUS", "string", ParameterDirection.Input, r.Maritalstatus, 200);//43 Marital status ‘S’: Single ‘M’: Married ‘D’: Divorced ‘W’: Widowed
                cn.addparam("AS_PC_IND", "string", ParameterDirection.Input, DBNull.Value, 200);//44//CIF category P - Potential,C - Client, N - Neither,V - VIP
                cn.addparam("AS_POPULATED", "string", ParameterDirection.Input, "1");//45
                cn.addparam("AS_SHOW_SECRET_NO", "string", ParameterDirection.Input, DBNull.Value, 200);//46
                cn.addparam("AL_REL_OFF_ID", "int", ParameterDirection.Input, DBNull.Value, 200);//47
                cn.addparam("AL_MONTHLY_SALARY", "int", ParameterDirection.Input, DBNull.Value, 200);//48
                cn.addparam("AL_SUB_ECO_SECTOR", "int", ParameterDirection.Input, DBNull.Value, 200);//49
                cn.addparam("AS_SEXE", "string", ParameterDirection.Input, r.Gender);//50 ---- Gender (Male, Female)
                cn.addparam("AS_ADD_REF", "string", ParameterDirection.Input, DBNull.Value, 200);//51
                cn.addparam("AS_BILL_FLAG", "string", ParameterDirection.Input, DBNull.Value, 200);//52
                cn.addparam("AS_IND", "string", ParameterDirection.Input, "M");//53
                cn.addparam("AL_TRX_TYPE", "int", ParameterDirection.Input, DBNull.Value, 200);//54
                cn.addparam("AL_CY", "int", ParameterDirection.Input, DBNull.Value, 200);//55
                cn.addparam("AL_ACC_BR", "int", ParameterDirection.Input, DBNull.Value, 200);//56
                cn.addparam("AL_ACC_CY", "int", ParameterDirection.Input, DBNull.Value, 200);//57
                cn.addparam("AL_ACC_GL", "int", ParameterDirection.Input, DBNull.Value, 200);//58
                cn.addparam("AL_ACC_CIF", "int", ParameterDirection.Input, DBNull.Value, 200);//59
                cn.addparam("AL_ACC_SL", "int", ParameterDirection.Input, DBNull.Value, 200);//60
                //cn.addparam("ADT_ADD_DATE1", "string", ParameterDirection.Input, DateTime.Now, 200);//61//Datetime.now
                cn.addparam("ADT_ADD_DATE1", "string", ParameterDirection.Input, cdate);//61//Datetime.now
                cn.addparam("ADT_ADD_DATE2", "string", ParameterDirection.Input, DBNull.Value, 200);//62
                cn.addparam("ADT_ADD_DATE3", "string", ParameterDirection.Input, DBNull.Value, 200);//63
                cn.addparam("ADT_ADD_DATE4", "string", ParameterDirection.Input, DBNull.Value, 200);//64
                cn.addparam("ADT_ADD_DATE5", "string", ParameterDirection.Input, DBNull.Value, 200);//65
                cn.addparam("AL_ADD_NUMBER1", "int", ParameterDirection.Input, DBNull.Value, 200);//66
                cn.addparam("AL_ADD_NUMBER2", "int", ParameterDirection.Input, DBNull.Value, 200);//67
                cn.addparam("AL_ADD_NUMBER3", "int", ParameterDirection.Input, DBNull.Value, 200);//68
                cn.addparam("AL_ADD_NUMBER4", "int", ParameterDirection.Input, DBNull.Value, 200);//69
                cn.addparam("AL_ADD_NUMBER5", "int", ParameterDirection.Input, DBNull.Value, 200);//70
                //cn.addparam("AS_ADD_STRING1", "string", ParameterDirection.Input, DBNull.Value, 200);//71//BVN
                cn.addparam("AS_ADD_STRING1", "string", ParameterDirection.Input, r.BVN);//71//BVN
                cn.addparam("AS_ADD_STRING2", "string", ParameterDirection.Input, DBNull.Value, 200);//72
                cn.addparam("AS_ADD_STRING3", "string", ParameterDirection.Input, DBNull.Value, 200);//73
                cn.addparam("AS_ADD_STRING4", "string", ParameterDirection.Input, DBNull.Value, 200);//74
                cn.addparam("AS_ADD_STRING5", "string", ParameterDirection.Input, DBNull.Value, 200);//75
                cn.addparam("AS_ADD_STRING6", "string", ParameterDirection.Input, DBNull.Value, 200);//76
                cn.addparam("AS_ADD_STRING7", "string", ParameterDirection.Input, DBNull.Value, 200);//77
                cn.addparam("AS_ADD_STRING8", "string", ParameterDirection.Input, DBNull.Value, 200);//78
                cn.addparam("AS_ADD_STRING9", "string", ParameterDirection.Input, DBNull.Value, 200);//79
                cn.addparam("AS_ADD_STRING10", "string", ParameterDirection.Input, DBNull.Value, 200);//80
                cn.addparam("AS_ADD_STRING11", "string", ParameterDirection.Input, DBNull.Value, 200);//81
                cn.addparam("AS_ADD_STRING12", "string", ParameterDirection.Input, DBNull.Value, 200);//82
                cn.addparam("AS_ADD_STRING13", "string", ParameterDirection.Input, DBNull.Value, 200);//83
                cn.addparam("AS_ADD_STRING14", "string", ParameterDirection.Input, DBNull.Value, 200);//84
                cn.addparam("AS_ADD_STRING15", "string", ParameterDirection.Input, DBNull.Value, 200);//85
                //output
                cn.addparam("ol_cif_no", "int", ParameterDirection.Output);//86// CIF NUMBER
                cn.addparam("ol_error_code", "int", ParameterDirection.Output);//87 // 	0  Success,-99 Access Denied,< 0 error code for specific error
                cn.addparam("os_error_desc", "string", ParameterDirection.Output, "", 4000);//88 // Error description

                cn.query();


            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
            }
            //Read the output parameter after execution
            string temp0 = cn.cmd.Parameters["@ol_cif_no"].Value.ToString();
            string temp1 = cn.cmd.Parameters["@ol_error_code"].Value.ToString();
            string temp2 = cn.cmd.Parameters["@os_error_desc"].Value.ToString();
            if (temp1 == "0")
            {
                string err = "";
                try
                {
                    //Save cif address details
                    OraConntxn c = new OraConntxn("imal.P_API_CREATE_CIF_ADDRESS", true);//p_api_create_cif_address
                    c.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, 1);//1
                    c.addparam("AS_USER_ID", "string", ParameterDirection.Input, "333", 8);//2
                    c.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, hostName, 40);//3
                    c.addparam("AL_API_CODE", "int", ParameterDirection.Input, 316);//4
                    c.addparam("AL_COMP_CODE", "int", ParameterDirection.Input, 1);//5
                    c.addparam("AL_BRANCH_CODE", "int", ParameterDirection.Input, r.BranchCode);//6
                    c.addparam("AL_CIF_NO", "int", ParameterDirection.Input, Convert.ToInt64(temp0));//7 --CUSTOMER NO
                    c.addparam("AL_LINE_NO", "int", ParameterDirection.Input, DBNull.Value, 200);//8
                    c.addparam("AS_ADDRESS_DESC", "string", ParameterDirection.Input, DBNull.Value, 200);//9
                    c.addparam("AS_ADDRESS1_ENG", "string", ParameterDirection.Input, r.Address1);//10
                    c.addparam("AS_ADDRESS2_ENG", "string", ParameterDirection.Input, r.Address2);//11
                    c.addparam("AS_ADDRESS3_ENG", "string", ParameterDirection.Input, r.Address3);//12
                    c.addparam("AS_ADDRESS4_ENG", "string", ParameterDirection.Input, DBNull.Value, 200);//13
                    c.addparam("AS_ADDRESS1_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//14
                    c.addparam("AS_ADDRESS2_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//15
                    c.addparam("AS_ADDRESS3_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//16
                    c.addparam("AS_ADDRESS4_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//17
                    c.addparam("AL_PRINT_STAT", "int", ParameterDirection.Input, DBNull.Value, 200);//18
                    c.addparam("AS_CONTACT_NAME", "string", ParameterDirection.Input, DBNull.Value, 200);//19
                    c.addparam("AS_CONTACT_NAME_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//20
                    c.addparam("AS_STREET_DETAILS_ENG", "string", ParameterDirection.Input, DBNull.Value, 200);//21
                    c.addparam("AS_CITY_ENG", "string", ParameterDirection.Input, r.City);//22 
                    c.addparam("AS_GOVERNERATE_ENG", "string", ParameterDirection.Input, DBNull.Value, 200);//23
                    c.addparam("AS_STREET_DETAILS_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//24
                    c.addparam("AS_CITY_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//25
                    c.addparam("AS_GOVERNERATE_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//26
                    c.addparam("AS_SALUTATION_ENG", "string", ParameterDirection.Input, DBNull.Value, 200);//27
                    c.addparam("AS_SALUTATION_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//28
                    c.addparam("AS_FAX", "string", ParameterDirection.Input, DBNull.Value, 200);//29
                    c.addparam("AS_TEL", "string", ParameterDirection.Input, r.Telephone);//30
                    c.addparam("AS_DEFAULT_ADD", "string", ParameterDirection.Input, "1");//31
                    c.addparam("AS_PO_BOX", "string", ParameterDirection.Input, DBNull.Value, 200);//32
                    c.addparam("AL_POSTAL_CODE", "int", ParameterDirection.Input, DBNull.Value, 200);//33
                    c.addparam("AL_POBOX_AREA", "int", ParameterDirection.Input, DBNull.Value, 200);//34
                    c.addparam("AL_COUNTRY", "int", ParameterDirection.Input, 566);//35 Country code
                    c.addparam("AL_REGION", "int", ParameterDirection.Input, DBNull.Value, 200);//36
                    c.addparam("AS_MOBILE", "string", ParameterDirection.Input, r.Telephone);//37
                    c.addparam("AS_HOME_TEL", "string", ParameterDirection.Input, r.Telephone);//38
                    c.addparam("AS_WORK_TEL", "string", ParameterDirection.Input, DBNull.Value, 200);//39
                    c.addparam("AS_EMAIL", "string", ParameterDirection.Input, r.Email);//40
                    c.addparam("AS_OTHER_TEL", "string", ParameterDirection.Input, DBNull.Value, 200);//41
                    c.addparam("AS_ADDITIONAL_REFERENCE", "string", ParameterDirection.Input, DBNull.Value, 200);//42
                    c.addparam("AS_POSTAL_CODE1", "string", ParameterDirection.Input, DBNull.Value, 200);//43
                    c.addparam("ADT_EXPIRY_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//44
                    c.addparam("AL_CITY_CODE", "int", ParameterDirection.Input, DBNull.Value, 200);//45
                    c.addparam("AL_SECTOR_CODE", "int", ParameterDirection.Input, DBNull.Value, 200);//46
                    c.addparam("AL_ADDRESS_DESCRIPTION", "int", ParameterDirection.Input, DBNull.Value, 200);//47
                    c.addparam("AS_PERM_ADDRESS", "string", ParameterDirection.Input, DBNull.Value, 200);//48
                    c.addparam("ADT_FROM_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//49
                    c.addparam("ADT_TO_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//50

                    //OUTPUT
                    c.addparam("ol_cif_no", "int", ParameterDirection.Output);//51// CIF NUMBER
                    c.addparam("ol_line_no", "int", ParameterDirection.Output);//52
                    c.addparam("ol_error_code", "int", ParameterDirection.Output);//53 // 	0  Success,-99 Access Denied,< 0 error code for specific error
                    c.addparam("os_error_desc", "string", ParameterDirection.Output, "", 4000);//54 // Error description



                    c.query();

                    //Read the output parameter after execution
                    string cifTemp0 = c.cmd.Parameters["@ol_cif_no"].Value.ToString();
                    string cifTemp1 = c.cmd.Parameters["@ol_error_code"].Value.ToString();
                    err = c.cmd.Parameters["@os_error_desc"].Value.ToString();
                }
                catch (Exception ex)
                {
                    new ErrorLog("ERROR SAVING ADDRESS" + ex + "ERROR FROM PROCEDURE" + err);
                }

                CreateCIFResp rsp = new CreateCIFResp
                {
                    responseCode = "0",
                    errorCode = "0",
                    skipProcessing = "false",
                    skipLog = "false",
                    cifNo = temp0
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            else
            {
                CreateCIFResp rsp = new CreateCIFResp
                {
                    responseCode = "99",
                    errorCode = temp1,
                    skipProcessing = "false",
                    skipLog = temp2
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
        }

        /// <summary>
        /// API to create IMAL Account
        /// </summary>
        /// <remarks> AppName accepts max 8 characters</remarks>
        [HttpPost]
        [ActionName("CreateAccount")]
        [ResponseType(typeof(CreateCIFResp))]
        public async Task<HttpResponseMessage> CreateAccount([FromBody] CreateAccReq r)
        {
            string txn = r.referenceCode; string json = "";
            //get the request, process and respond
            OraConntxn cn = new OraConntxn("IMAL.P_API_ACCOUNT_CREATION", true);
            DateTime dt1 = DateTime.Now.Date; string cdate = "";
            string hostName = Dns.GetHostName();
            cdate = dt1.ToString("dd-MMM-yy").ToUpper();

            if (!ModelState.IsValid)
            {
                CreateCIFResp rsp = new CreateCIFResp
                {
                    responseCode = "99",
                    errorCode = "Invalid request. Check parameters",
                    skipProcessing = "false",
                    skipLog = "Invalid request. Check parameters"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, rsp);
            }

            cn.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, 2);//1
            //cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, "MODEL.B", 200);//2
            cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, r.AppName, 200);//2
            cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, hostName, 200);//3
            cn.addparam("AL_API_CODE", "int", ParameterDirection.Input, 105);//4
            cn.addparam("ADT_DATE", "string", ParameterDirection.Input, cdate, 200);//5
            cn.addparam("AL_COMP_CODE", "int", ParameterDirection.Input, 1);//6
            cn.addparam("AL_BRANCH_CODE", "int", ParameterDirection.Input, r.BranchCode);//7
            cn.addparam("AS_CURRENCY_CODE", "string", ParameterDirection.Input, "566", 200);//8
            cn.addparam("AL_GL_CODE", "int", ParameterDirection.Input, r.GL_CODE);//9
            cn.addparam("AL_CIF_SUB_NO", "int", ParameterDirection.Input, r.CIFNo);//10
            //cn.addparam("AS_TELLER_ID", "string", ParameterDirection.Input, "MODEL.B", 200);//11
            cn.addparam("AS_TELLER_ID", "string", ParameterDirection.Input, r.AppName, 200);//11
            cn.addparam("AS_RENEW", "string", ParameterDirection.Input, "N", 200);//12
            cn.addparam("AL_TRF_BR", "int", ParameterDirection.Input, DBNull.Value, 200);//13
            cn.addparam("AL_TRF_CY", "int", ParameterDirection.Input, DBNull.Value, 200);//14
            cn.addparam("AL_TRF_GL", "int", ParameterDirection.Input, DBNull.Value, 200);//15
            cn.addparam("AL_TRF_CIF", "int", ParameterDirection.Input, DBNull.Value, 200);//16
            cn.addparam("AL_TRF_SL", "int", ParameterDirection.Input, DBNull.Value, 200);//17
            cn.addparam("AS_TRF_ADD_REF", "string", ParameterDirection.Input, DBNull.Value, 200);//18
            cn.addparam("AS_PFT_POST_TO", "string", ParameterDirection.Input, "1", 200);//19
            cn.addparam("AL_PROFIT_BR", "int", ParameterDirection.Input, DBNull.Value, 200);//20
            cn.addparam("AL_PROFIT_CY", "int", ParameterDirection.Input, DBNull.Value, 200);//21
            cn.addparam("AL_PROFIT_GL", "int", ParameterDirection.Input, DBNull.Value, 200);//22
            cn.addparam("AL_PROFIT_CIF", "int", ParameterDirection.Input, DBNull.Value, 200);//23
            cn.addparam("AL_PROFIT_SL", "int", ParameterDirection.Input, DBNull.Value, 200);//24
            cn.addparam("AS_PROFIT_ADD_REF", "string", ParameterDirection.Input, DBNull.Value, 200);//25
            cn.addparam("AL_MATURITY_GL", "int", ParameterDirection.Input, DBNull.Value, 200);//26
            cn.addparam("AS_EXT_TRF", "string", ParameterDirection.Input, DBNull.Value, 200);//27
            cn.addparam("AL_OFF_BR", "int", ParameterDirection.Input, DBNull.Value, 200);//28
            cn.addparam("AL_OFF_CY", "int", ParameterDirection.Input, DBNull.Value, 200);//29
            cn.addparam("AL_OFF_GL", "int", ParameterDirection.Input, DBNull.Value, 200);//30
            cn.addparam("AL_OFF_CIF", "int", ParameterDirection.Input, DBNull.Value, 200);//31
            cn.addparam("AL_OFF_SL", "int", ParameterDirection.Input, DBNull.Value, 200);//32
            cn.addparam("AS_OFF_ADD_REF", "string", ParameterDirection.Input, DBNull.Value, 200);//33
            cn.addparam("AL_TRANSFER_AM", "int", ParameterDirection.Input, DBNull.Value, 200);//34 //transfer amount
            cn.addparam("AL_DEBIT_BRANCH", "int", ParameterDirection.Input, DBNull.Value, 200);//35
            cn.addparam("AL_DEBIT_CURRENCY", "int", ParameterDirection.Input, DBNull.Value, 200);//36
            cn.addparam("AL_DEBIT_GL_CODE", "int", ParameterDirection.Input, DBNull.Value);//37
            cn.addparam("AL_DEBIT_CIF_SUB_NO", "int", ParameterDirection.Input, DBNull.Value, 200);//38
            cn.addparam("AL_DEBIT_SL", "int", ParameterDirection.Input, DBNull.Value, 200);//39, 200
            cn.addparam("AS_DEBIT_ADD_REF", "string", ParameterDirection.Input, DBNull.Value, 200);//40
            cn.addparam("AS_REMARKS", "string", ParameterDirection.Input, DBNull.Value, 200);//41
            cn.addparam("AL_DIV_CODE", "int", ParameterDirection.Input, 5);//42
            cn.addparam("AL_DEPT_CODE", "int", ParameterDirection.Input, 501);//43
            cn.addparam("AS_REFERENCE", "string", ParameterDirection.Input, r.REFERENCE, 200);//44 Fix reference of transaction
            cn.addparam("AL_POS", "int", ParameterDirection.Input, 1);//45
            cn.addparam("AS_INST1", "string", ParameterDirection.Input, DBNull.Value, 200);//46
            cn.addparam("AS_INST2", "string", ParameterDirection.Input, DBNull.Value, 200);//47
            cn.addparam("AL_TRXTYPE", "int", ParameterDirection.Input, 3);//48
            cn.addparam("AS_STATUS", "string", ParameterDirection.Input, "I", 200);//49
            cn.addparam("AS_CREATED_TRX", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            //output
            cn.addparam("OS_ADD_REF", "string", ParameterDirection.InputOutput, "", 4000);//51
            cn.addparam("OL_TRS_NO", "int", ParameterDirection.InputOutput);//52
            cn.addparam("OL_POINT_RATE", "int", ParameterDirection.InputOutput);//53
            cn.addparam("ODT_MATE_DATE", "date", ParameterDirection.InputOutput);//54
            cn.addparam("OL_NEW_BAL", "int", ParameterDirection.InputOutput);//55
            cn.addparam("OS_ACC_NAME", "string", ParameterDirection.InputOutput, "", 200);//56
            cn.addparam("OL_ERROR_CODE", "int", ParameterDirection.InputOutput);//57
            cn.addparam("OS_ERROR_DESC", "string", ParameterDirection.InputOutput, "", 4000);//58

            try
            {
                //cn.query1();
                cn.query();
            }
            catch (Exception ex)
            {
            }
            //Read the output parameter after execution
            string temp0 = cn.cmd.Parameters["@OL_NEW_BAL"].Value.ToString();
            string temp1 = cn.cmd.Parameters["@OL_ERROR_CODE"].Value.ToString();
            string temp2 = cn.cmd.Parameters["@OS_ERROR_DESC"].Value.ToString();
            string temp3 = cn.cmd.Parameters["@OS_ADD_REF"].Value.ToString();
            string temp4 = cn.cmd.Parameters["@OL_TRS_NO"].Value.ToString();

            if (temp1 == "0")
            {
                CreateCIFResp rsp = new CreateCIFResp
                {
                    responseCode = "0",
                    errorCode = "0",
                    skipProcessing = "false",
                    skipLog = "false",
                    cifNo = r.CIFNo,
                    AccountNo = temp3
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            else
            {
                CreateCIFResp rsp = new CreateCIFResp
                {
                    responseCode = "99",
                    errorCode = temp1,
                    skipProcessing = "false",
                    skipLog = temp2
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
        }

        /// <summary>
        /// Creates Loan on IMAL core banking Application.
        /// </summary>
        /// <param name="r">The request.</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName(nameof(CreateLoan))]
        public async Task<HttpResponseMessage> CreateLoan([FromBody] CreateLoanReq r)
        {
            string json;
            //get the request, process and respond
            var cn = new OraConntxn("IMAL.P_API_CREATE_INVDEAL", true);

            var counterParty = 147078;

            cn.addparam("AL_COMP_CODE", "float", ParameterDirection.Input, 1);//1
            cn.addparam("AL_BRANCH_CODE", "float", ParameterDirection.Input, 1);//2
            cn.addparam("AL_PRODUCT_CLASS", "float", ParameterDirection.Input, 1001);//3
            cn.addparam("AL_COUNTER_PARTY", "float", ParameterDirection.Input, counterParty);//4
            cn.addparam("AS_SOURCE_OF_FUND", "string", ParameterDirection.Input, DBNull.Value, 200);//5
            cn.addparam("AS_ADDRESS1", "string", ParameterDirection.Input, DBNull.Value, 200);//6
            cn.addparam("AS_ADDRESS2", "string", ParameterDirection.Input, DBNull.Value, 200);//7
            cn.addparam("AS_ADDRESS3", "string", ParameterDirection.Input, DBNull.Value, 200);//8
            cn.addparam("AS_CONTACT", "string", ParameterDirection.Input, DBNull.Value, 200);//9
            cn.addparam("AL_COUNTRY", "float", ParameterDirection.Input, DBNull.Value, 200);//10
            cn.addparam("AL_ECO_SECTOR", "float", ParameterDirection.Input, DBNull.Value, 200);//11
            cn.addparam("AS_TELEPHONE", "string", ParameterDirection.Input, DBNull.Value, 200);//12
            cn.addparam("AS_FAX", "string", ParameterDirection.Input, DBNull.Value, 200);//13
            cn.addparam("AL_GUARANTOR", "float", ParameterDirection.Input, DBNull.Value, 200);//14
            cn.addparam("AS_REFERENCE", "string", ParameterDirection.Input, DBNull.Value, 200);//15
            cn.addparam("AL_DEAL_CY", "float", ParameterDirection.Input, r.dealCy);//16
            cn.addparam("ADEC_DEAL_AMOUNT", "decimal", ParameterDirection.Input, r.dealAmount);//17
            cn.addparam("ADEC_DEAL_CHARGE", "float", ParameterDirection.Input, DBNull.Value, 200);//18
            cn.addparam("ADEC_DEAL_CY_RATE", "float", ParameterDirection.Input, DBNull.Value, 200);//19
            cn.addparam("ADEC_DEAL_UNIT", "float", ParameterDirection.Input, DBNull.Value, 200);//20
            cn.addparam("AD_DEAL_DATE", "date", ParameterDirection.Input, r.dealDate);//21
            cn.addparam("AD_VALUE_DATE", "date", ParameterDirection.Input, r.valueDate);//22
            cn.addparam("AD_MATURITY_DATE", "date", ParameterDirection.Input, r.maturityDate);//23
            cn.addparam("ADT_FIRST_INSTALL_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//24
            cn.addparam("AL_PERIODICITY_NBR", "float", ParameterDirection.Input, DBNull.Value, 200);//25
            cn.addparam("AS_PERIODICITY_TYPE", "string", ParameterDirection.Input, DBNull.Value, 200);//26
            cn.addparam("ADEC_INSTALL_AMOUNT", "float", ParameterDirection.Input, DBNull.Value, 200);//27
            cn.addparam("ADEC_FLOATING_RATE", "decimal", ParameterDirection.Input, r.floatingRate);//28
            cn.addparam("ADEC_MOD_PERCENT", "float", ParameterDirection.Input, DBNull.Value, 200);//29
            cn.addparam("AS_VDATE_INS", "string", ParameterDirection.Input, DBNull.Value, 200);//30
            cn.addparam("AS_VDATE_INS_MAT", "string", ParameterDirection.Input, DBNull.Value, 200);//31
            cn.addparam("AS_REMARKS1", "string", ParameterDirection.Input, DBNull.Value, 200);//32
            cn.addparam("AS_REMARKS2", "string", ParameterDirection.Input, DBNull.Value, 200);//33
            cn.addparam("AS_DESCRIPTION", "string", ParameterDirection.Input, DBNull.Value, 200);//34 //transfer amount
            cn.addparam("AS_CONTRIB_TYPE", "string", ParameterDirection.Input, r.contribType, 200);//35
            cn.addparam("ADEC_CONTRIB_YIELD", "float", ParameterDirection.Input, r.contribYield);//36
            cn.addparam("AL_PARTY_NO", "float", ParameterDirection.Input, counterParty);//37
            cn.addparam("AL_CONTRIB_ACC_BR", "float", ParameterDirection.Input, DBNull.Value, 200);//38
            cn.addparam("AL_CONTRIB_ACC_CY", "float", ParameterDirection.Input, DBNull.Value, 200);//39, 200
            cn.addparam("AL_CONTRIB_ACC_GL", "float", ParameterDirection.Input, DBNull.Value, 200);//40
            cn.addparam("AL_CONTRIB_ACC_CIF", "float", ParameterDirection.Input, DBNull.Value, 200);//41
            cn.addparam("AL_CONTRIB_ACC_SL", "float", ParameterDirection.Input, DBNull.Value, 200);//42
            cn.addparam("AS_CONTRIB_ACC_REF", "string", ParameterDirection.Input, DBNull.Value, 200);//43
            cn.addparam("ADEC_CONTRIB_CROSS_RATE", "float", ParameterDirection.Input, DBNull.Value, 200);//44 Fix reference of transaction
            cn.addparam("ADEC_CONTRIB_CROSS_NULTDIV", "string", ParameterDirection.Input, DBNull.Value, 200);//45
            cn.addparam("AL_NOSTRO_ACC_BR", "float", ParameterDirection.Input, r.nostroAccBr);//46
            cn.addparam("AL_NOSTRO_ACC_CY", "float", ParameterDirection.Input, r.nostroAccCy);//47
            cn.addparam("AL_NOSTRO_ACC_GL", "float", ParameterDirection.Input, r.nostroAccGl);//48
            cn.addparam("AL_NOSTRO_ACC_CIF", "float", ParameterDirection.Input, r.nostroAccCif);//49
            cn.addparam("AL_NOSTRO_ACC_SL", "float", ParameterDirection.Input, r.nostroAccSl);//50
            cn.addparam("AS_NOSTRO_ACC_REF", "string", ParameterDirection.Input, DBNull.Value);//50
            cn.addparam("ADEC_NOSTRO_CROSS_RATE", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_NOSTRO_CROSS_NULTDIV", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_LIMIT_SL_NO", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_TRADE_DEALNO", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_PERIODICITY_POS", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_REPAYMENT_TEMPLATE_CODE", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_MARGIN_RATE", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_NO_OF_PAYMENTS", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ROUNDING_FACTOR", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_PAY_RES_AMT", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ACTUAL_GRACE_PERIOD", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_ACTUAL_GRACE_PERIODICITY", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_DEAL_PERIOD_COMPOUNDING", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_GRACE_PERIOD_COMPOUNDING", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_GRACE_COMPD_PERIOD_NBR", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_GRACE_COMPD_PERIOD_TYPE", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_MINIMUM_YIELD", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_MAXIMUM_YIELD", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_MINIMUM_FLOATING_RATE", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_MAXIMUM_FLOATING_RATE", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_DOWN_PAYMENT", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_FLOATING_RATE_CODE", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_FLOATING_RATE_PERIOD_NBR", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_FLOATING_RATE_PERIOD_TYPE", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_LINKED_ABI_NUMBER", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_LINKED_ABI_SERIAL_NUMBER", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_LAST_HOLDER_CIF", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_BROKER_CODE", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_PORTFOLIO_CIF", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_PORTFOLIO_SEQ", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_LINKED_DEAL_BRANCH", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_LINKED_DEAL_NO", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ROLLED_DEAL_BRANCH", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ROLLED_DEAL_NO", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_MAT_ACC_BR", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_MAT_ACC_CY", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_MAT_ACC_GL", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_MAT_ACC_CIF", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_MAT_ACC_SL", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_ROLLOVER_ADDTNL_AMOUNT", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ROLLOVER_ADDTNL_ACC_BR", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ROLLOVER_ADDTNL_ACC_CY", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ROLLOVER_ADDTNL_ACC_GL", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ROLLOVER_ADDTNL_ACC_CIF", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_ROLLOVER_ADDTNL_ACC_SL", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_ORIGIN", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_PLAN_REF_NBR", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_PLAN_REF_SEQ", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AL_MUDARABAH_RATE_CODE", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("AS_MODARIB_PARTY_BANK", "string", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_MODARIB_CONTRIB_PERC", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_MODARIB_PARTY_PERC", "float", ParameterDirection.Input, DBNull.Value, 200);//50
            cn.addparam("ADEC_EXPECTED_ROR", "float", ParameterDirection.Input, DBNull.Value, 200);//50

            //output
            cn.addparam("OL_DEAL_NO", "float", ParameterDirection.InputOutput, "", 4000);//50
            cn.addparam("OL_ERROR_CODE", "float", ParameterDirection.InputOutput, "", 4000);//50
            cn.addparam("OS_ERROR_DESC", "string", ParameterDirection.InputOutput, "", 4000);//50

            try
            {
                //cn.query1();
                cn.query();
            }
            catch (Exception ex)
            {
                Mylogger.Info("An error occured while executing query for creating loan:\n" + ex);
            }
            //Read the output parameter after execution
            var dealNo = cn.cmd.Parameters["@OL_DEAL_NO"].Value.ToString();
            var errorCode = cn.cmd.Parameters["@OL_ERROR_CODE"].Value.ToString();
            var errorDesc = cn.cmd.Parameters["@OS_ERROR_DESC"].Value.ToString();

            if (errorCode == "0") //success
            {
                var rsp = new CreateLoanResp
                {
                    responseCode = "0",
                    errorCode = int.Parse(errorCode),
                    skipProcessing = "false",
                    skipLog = "false",
                    dealNo = int.Parse(dealNo)
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.OK, json);
            }
            else
            {
                var rsp = new CreateLoanResp
                {
                    responseCode = "99",
                    errorCode = int.Parse(errorCode),
                    skipProcessing = "false",
                    skipLog = errorDesc
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
        }

        [HttpPost]
        [ActionName("getAccountDetails")]
        public async Task<HttpResponseMessage> getAccountDetails([FromBody] NameEnquiryReq r)
        {
            string sid = ""; Gadget g = new Gadget(); string gen3 = ""; string[] bits = null;
            gen3 = g.GenerateRndNumber(3); string json = "";
            try
            {
                NIBSSService.sbp_outward_transSoapClient nip = new NIBSSService.sbp_outward_transSoapClient();
                sid = g.newSessionGlobal(gen3, 2);
                //do the name enquiry
                string theResp = nip.NameEnquiryIMAL(sid, r.destinationBankCode, r.channelCode, r.account);
                bits = theResp.Split(':');
                if (bits[0] == "00")
                {
                    NameEnquiryResp rsp = new NameEnquiryResp
                    {
                        nameDetails = bits[1],
                        responseCode = "0",
                        errorCode = "0",
                        skipProcessing = "false",
                        originalResponseCode = "0",
                        skipLog = "false",
                        neSid = sid
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    NameEnquiryResp rsp = new NameEnquiryResp
                    {
                        nameDetails = bits[1],
                        responseCode = bits[0],
                        errorCode = bits[0],
                        skipProcessing = "false",
                        originalResponseCode = bits[0],
                        skipLog = "false",
                        neSid = sid
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                }
            }
            catch (Exception ex)
            {
                NameEnquiryResp rsp = new NameEnquiryResp
                {
                    nameDetails = "-1",
                    responseCode = "-1",
                    errorCode = ex.ToString(),
                    skipProcessing = "false",
                    originalResponseCode = "-1",
                    skipLog = "false",
                    neSid = sid
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
        }

        [HttpPost]
        [ActionName("ReserveFunds")]
        public async Task<HttpResponseMessage> ReserveFunds([FromBody] ReserveFundsReq r)
        {
            //get the request, process and respond
            OraConntxn cn = new OraConntxn("imal.P_API_ATM_WITHDRAWAL", true);
            DateTime dt1 = DateTime.Now.Date; string cdate = "";
            cdate = dt1.ToString("dd-MMM-yy").ToUpper();
            string hostName = System.Net.Dns.GetHostName(); string json = "";

            if (!ModelState.IsValid)
            {
                ReserveFundsResp rsp = new ReserveFundsResp
                {
                    responseCode = "99",
                    errorCode = "99",
                    errorMsg = "Invalid Request"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }

            string as_ref = "0100#01-" + r.LockId + "/00010@ster";
            try
            {
                cn.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, 1);//1
                cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, "ATMT", 200);//2
                cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, hostName, 200);//3
                cn.addparam("AL_API_CODE", "int", ParameterDirection.Input, 75);//4
                cn.addparam("AL_COMPANY", "int", ParameterDirection.Input, 1);//5
                cn.addparam("AL_BRANCH", "int", ParameterDirection.Input, 1);//6
                cn.addparam("AL_TELLER", "int", ParameterDirection.Input, 2330);//7
                cn.addparam("AL_TRXTYPE", "int", ParameterDirection.Input, 10);//8
                cn.addparam("AS_CARD", "string", ParameterDirection.Input, DBNull.Value, 200);//9
                cn.addparam("AS_ACCOUNT", "string", ParameterDirection.Input, r.account.Trim());//10-- Account no
                cn.addparam("AS_TO_ACC", "string", ParameterDirection.Input, DBNull.Value, 200);//11
                cn.addparam("ADEC_AMOUNT", "int", ParameterDirection.Input, r.AmountToReserve);//12 --Amount to Reserve
                cn.addparam("AS_CURRENCY", "string", ParameterDirection.Input, "566", 200);//13 -- Currency
                cn.addparam("AS_DATE_TIME", "string", ParameterDirection.Input, cdate, 200);//14
                cn.addparam("AS_REFERENCE", "string", ParameterDirection.Input, as_ref.Trim());//15 -- Reference used to lock funds
                cn.addparam("AL_POS", "int", ParameterDirection.Input, 0);//16
                cn.addparam("AL_RELEASE_DAYS", "int", ParameterDirection.Input, 1);//17
                cn.addparam("AS_DESC", "string", ParameterDirection.Input, r.Narration.Trim());//18 -- Narration of funds locked
                cn.addparam("AS_DESC_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//19
                cn.addparam("AL_CARD_PRESENT", "int", ParameterDirection.Input, 0);//20
                cn.addparam("AL_CHECK_BALANCE", "int", ParameterDirection.Input, 1);//21
                cn.addparam("AL_USE_ACCOUNT", "int", ParameterDirection.Input, 1);//22
                cn.addparam("AL_USE_ACCOUNT_BR", "int", ParameterDirection.Input, 0);//23
                cn.addparam("AL_USE_ACCOUNT_CY", "int", ParameterDirection.Input, 0);//24
                cn.addparam("AL_TRX_STATUS", "int", ParameterDirection.Input, 1);//25
                cn.addparam("AL_TRX_ALERT", "int", ParameterDirection.Input, 0);//26
                cn.addparam("AL_NUMBER_6", "int", ParameterDirection.Input, DBNull.Value, 200);//27
                cn.addparam("AL_COMPARE_DATE", "int", ParameterDirection.Input, 0);//28
                cn.addparam("AL_USE_DATE", "int", ParameterDirection.Input, 0);//29
                cn.addparam("AL_NUMBER_7", "int", ParameterDirection.Input, DBNull.Value, 200);//30
                cn.addparam("AL_TIME_OUT", "int", ParameterDirection.Input, 0);//31
                cn.addparam("AL_FEES_TRXTYPE", "int", ParameterDirection.Input, 0);//32
                cn.addparam("ADEC_FEES_AMOUNT", "int", ParameterDirection.Input, 0);//33
                cn.addparam("AS_FEES_ACCOUNT", "string", ParameterDirection.Input, "0");//34
                cn.addparam("AL_USE_FEES_ACCOUNT", "int", ParameterDirection.Input, 0);//35
                cn.addparam("AL_USE_FEES_ACCOUNT_BR", "int", ParameterDirection.Input, 0);//36
                cn.addparam("AL_USE_FEES_ACCOUNT_CY", "int", ParameterDirection.Input, 0);//37
                cn.addparam("AL_COUNT_SPLIT", "int", ParameterDirection.Input, 1);//38
                cn.addparam("ADEC_TO_AMOUNT", "int", ParameterDirection.Input, 0);//39
                cn.addparam("ADEC_EXCH_RATE", "int", ParameterDirection.Input, 0);//40
                cn.addparam("AL_HOF_CHARGE", "int", ParameterDirection.Input, 0);//41
                cn.addparam("AL_HOF_TRXTYPE", "int", ParameterDirection.Input, 0);//42
                cn.addparam("AL_DIFF_TD_VD", "int", ParameterDirection.Input, 0);//43
                cn.addparam("ADT_VALUE_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//44
                cn.addparam("AL_NUMBER_1", "int", ParameterDirection.Input, DBNull.Value, 200);//45
                cn.addparam("AL_NUMBER_2", "int", ParameterDirection.Input, DBNull.Value, 200);//46
                cn.addparam("AL_NUMBER_3", "int", ParameterDirection.Input, DBNull.Value, 200);//47
                cn.addparam("AL_NUMBER_4", "int", ParameterDirection.Input, DBNull.Value, 200);//48
                cn.addparam("AL_NUMBER_5", "int", ParameterDirection.Input, DBNull.Value, 200);//49
                cn.addparam("AS_STRING_1", "string", ParameterDirection.Input, DBNull.Value, 200);//50
                cn.addparam("AS_STRING_2", "string", ParameterDirection.Input, DBNull.Value, 200);//51
                cn.addparam("AS_STRING_3", "string", ParameterDirection.Input, DBNull.Value, 200);//52
                cn.addparam("AS_STRING_4", "string", ParameterDirection.Input, DBNull.Value, 200);//53
                cn.addparam("AS_STRING_5", "string", ParameterDirection.Input, DBNull.Value, 200);//54
                cn.addparam("ADT_DATE_1", "string", ParameterDirection.Input, DBNull.Value, 200);//55
                cn.addparam("ADT_DATE_2", "string", ParameterDirection.Input, DBNull.Value, 200);//56
                cn.addparam("ADT_DATE_3", "string", ParameterDirection.Input, DBNull.Value, 200);//57
                cn.addparam("ADT_DATE_4", "string", ParameterDirection.Input, DBNull.Value, 200);//58
                cn.addparam("ADT_DATE_5", "string", ParameterDirection.Input, DBNull.Value, 200);//59
                cn.addparam("AL_ATM_TRX", "int", ParameterDirection.Input, DBNull.Value, 200);//60
                cn.addparam("AS_TRX_AMT", "string", ParameterDirection.Input, DBNull.Value, 200);//61
                cn.addparam("AS_SETTL_AMT", "string", ParameterDirection.Input, DBNull.Value, 200);//62
                cn.addparam("AS_TRACE_NUM", "string", ParameterDirection.Input, DBNull.Value, 200);//63
                cn.addparam("AS_SETTL_RATE", "string", ParameterDirection.Input, DBNull.Value, 200);//64
                cn.addparam("AS_LOCAL_TIME", "string", ParameterDirection.Input, DBNull.Value, 200);//65
                cn.addparam("AS_LOCAL_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//66
                cn.addparam("AS_SETTL_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//67
                cn.addparam("AS_EXPIRY_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//68
                cn.addparam("AS_CAPTURE_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//69
                cn.addparam("AS_IID", "string", ParameterDirection.Input, DBNull.Value, 200);//70
                cn.addparam("AS_TRACK2", "string", ParameterDirection.Input, DBNull.Value, 200);//71
                cn.addparam("AS_RET_REF_NO", "string", ParameterDirection.Input, DBNull.Value, 200);//72
                cn.addparam("AS_ACCEPTOR_TERM_ID", "string", ParameterDirection.Input, DBNull.Value, 200);//73
                cn.addparam("AS_ACCEPTOR_ID_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//74
                cn.addparam("AS_ACCEPTOR_NAME", "string", ParameterDirection.Input, DBNull.Value, 200);//75
                cn.addparam("AS_TRX_CURRENCY", "string", ParameterDirection.Input, DBNull.Value, 200);//76
                cn.addparam("AS_SETTL_CURRENCY", "string", ParameterDirection.Input, DBNull.Value, 200);//77
                cn.addparam("AS_TERMINAL_DATA", "string", ParameterDirection.Input, DBNull.Value, 200);//78
                cn.addparam("AS_POS_RESPONSE_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//79
                cn.addparam("AS_POS_SETTL_DATA", "string", ParameterDirection.Input, DBNull.Value, 200);//80
                cn.addparam("AS_REPLACEMENT_AMT", "string", ParameterDirection.Input, DBNull.Value, 200);//81
                cn.addparam("AS_REPLACEMENT_AMT_FC", "string", ParameterDirection.Input, DBNull.Value, 200);//82
                cn.addparam("AS_TRX_TYPE", "string", ParameterDirection.Input, DBNull.Value, 200);//83
                cn.addparam("AS_AUTH_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//84
                cn.addparam("AS_FWD_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//85
                cn.addparam("AS_M_TYPE", "string", ParameterDirection.Input, DBNull.Value, 200);//86
                cn.addparam("AS_PROCESS_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//87
                cn.addparam("AS_CHOLDER_AMT_BILLING", "string", ParameterDirection.Input, DBNull.Value, 200);//88
                cn.addparam("AS_ACQ_INST_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//89
                cn.addparam("AS_FWD_INST_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//90
                cn.addparam("AS_FEE_CURRENCY", "string", ParameterDirection.Input, DBNull.Value, 200);//91
                cn.addparam("AS_FEE_TRX_PROCESS", "string", ParameterDirection.Input, DBNull.Value, 200);//92
                cn.addparam("AS_FEE_TRX", "string", ParameterDirection.Input, DBNull.Value, 200);//93
                cn.addparam("AS_CY_CODE_BILLING", "string", ParameterDirection.Input, DBNull.Value, 200);//94
                cn.addparam("AS_CREDIT_NBR", "string", ParameterDirection.Input, DBNull.Value, 200);//95
                cn.addparam("AS_ORG_DATA", "string", ParameterDirection.Input, DBNull.Value, 200);//96
                cn.addparam("AS_FILE_NAME", "string", ParameterDirection.Input, DBNull.Value, 200);//97
                cn.addparam("AL_INTERFACE_CODE", "int", ParameterDirection.Input, DBNull.Value, 200);//98

                //output
                cn.addparam("ODEC_AVAIL", "int", ParameterDirection.InputOutput);//99
                cn.addparam("OL_ERROR", "int", ParameterDirection.InputOutput);//100
                cn.addparam("OS_ERR_MSG", "string", ParameterDirection.InputOutput, "", 200);//101
                cn.addparam("OL_TRX_CODE", "int", ParameterDirection.InputOutput);//102
                cn.addparam("OL_FEES_TRX_CODE", "int", ParameterDirection.InputOutput);//103

                cn.query();

                //Read the output parameter after execution
                string temp0 = cn.cmd.Parameters["@OL_TRX_CODE"].Value.ToString();
                string temp1 = cn.cmd.Parameters["@OL_ERROR"].Value.ToString();
                string temp2 = cn.cmd.Parameters["@OS_ERR_MSG"].Value.ToString();


                if (temp1 == "0")
                {
                    ReserveFundsResp rsp = new ReserveFundsResp
                    {
                        responseCode = "0",
                        errorCode = "0",
                        transactionCode = temp0,
                        lockReference = as_ref
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    ReserveFundsResp rsp = new ReserveFundsResp
                    {
                        responseCode = "99",
                        errorCode = temp1,
                        errorMsg = temp2
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }

            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                ReserveFundsResp rsp = new ReserveFundsResp
                {
                    responseCode = "-1",
                    errorCode = "-1",
                    errorMsg = "Error occurred during processing"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, json);
            }


        }

        [HttpPost]
        [ActionName("ReleaseFunds")]
        public async Task<HttpResponseMessage> ReleaseFunds([FromBody] ReleaseFundsReq r)
        {

            //get the request, process and respond
            OraConntxn cn = new OraConntxn("imal.P_API_ATM_PARTIALREVERSAL", true);
            DateTime dt1 = DateTime.Now.Date; string cdate = "";
            cdate = dt1.ToString("dd-MMM-yy").ToUpper();
            string hostName = System.Net.Dns.GetHostName(); string json = "";

            if (!ModelState.IsValid)
            {
                ReleaseFundsResp rsp = new ReleaseFundsResp
                {
                    responseCode = "99",
                    errorCode = "99",
                    errorMsg = "Invalid Request"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }

            try
            {
                cn.addparam("AL_CHANNEL_ID", "int", ParameterDirection.Input, 1);//1
                cn.addparam("AS_USER_ID", "string", ParameterDirection.Input, "ATMT", 200);//2
                cn.addparam("AS_MACHINE_NAME", "string", ParameterDirection.Input, hostName, 200);//3
                cn.addparam("AL_API_CODE", "int", ParameterDirection.Input, 90);//4
                cn.addparam("AL_COMPANY", "int", ParameterDirection.Input, 1);//5
                cn.addparam("AL_BRANCH", "int", ParameterDirection.Input, 1);//6
                cn.addparam("AS_OLD_REFERENCE", "string", ParameterDirection.Input, r.reference.Trim(), 200);//7 -- Reference used to lock funds
                cn.addparam("AS_REV_TYPE", "string", ParameterDirection.Input, "R", 200);//8
                cn.addparam("AL_TELLER", "int", ParameterDirection.Input, 2330);//9
                cn.addparam("AL_TRXTYPE", "int", ParameterDirection.Input, 10);//10
                cn.addparam("AS_CARD", "string", ParameterDirection.Input, DBNull.Value, 200);//11
                cn.addparam("AS_ACCOUNT", "string", ParameterDirection.Input, r.account.Trim());//12 -- Account 
                cn.addparam("ADEC_AMOUNT", "int", ParameterDirection.Input, 0);//13
                cn.addparam("AS_CURRENCY", "string", ParameterDirection.Input, "566", 200);//14 -- currency
                cn.addparam("ADT_DATE_TIME", "string", ParameterDirection.Input, cdate, 200);//15
                cn.addparam("AS_REFERENCE", "string", ParameterDirection.Input, DBNull.Value, 200);//16
                cn.addparam("AL_POS", "int", ParameterDirection.Input, 0);//17
                cn.addparam("AL_RELEASE_DAYS", "int", ParameterDirection.Input, 0);//18
                cn.addparam("AS_DESC", "string", ParameterDirection.Input, r.Narration.Trim());//19 -- Narration for release of funds
                cn.addparam("AS_DESC_ARAB", "string", ParameterDirection.Input, DBNull.Value, 200);//20
                cn.addparam("AL_CARD_PRESENT", "int", ParameterDirection.Input, 0);//21
                cn.addparam("AL_CHECK_BALANCE", "int", ParameterDirection.Input, 1);//22
                cn.addparam("AL_USE_ACCOUNT", "int", ParameterDirection.Input, 1);//23
                cn.addparam("AL_USE_ACCOUNT_BR", "int", ParameterDirection.Input, 0);//24
                cn.addparam("AL_USE_ACCOUNT_CY", "int", ParameterDirection.Input, 0);//25
                cn.addparam("AL_TRX_STATUS", "int", ParameterDirection.Input, 1);//26
                cn.addparam("AL_TRX_ALERT", "int", ParameterDirection.Input, 0);//27
                cn.addparam("AL_COMPARE_DATE", "int", ParameterDirection.Input, 0);//28
                cn.addparam("AL_USE_DATE", "int", ParameterDirection.Input, 0);//29
                cn.addparam("AL_NUMBER_6", "int", ParameterDirection.Input, DBNull.Value, 200);//30
                cn.addparam("AL_NUMBER_7", "int", ParameterDirection.Input, DBNull.Value, 200);//31
                cn.addparam("AL_TIME_OUT", "int", ParameterDirection.Input, 0);//32
                cn.addparam("AL_COUNT_SPLIT", "int", ParameterDirection.Input, 1);//33
                cn.addparam("AL_FEES_TRXTYPE", "int", ParameterDirection.Input, 0);//34
                cn.addparam("ADEC_FEES_AMOUNT", "int", ParameterDirection.Input, 0);//35
                cn.addparam("AS_FEES_ACCOUNT", "string", ParameterDirection.Input, "0");//36
                cn.addparam("AL_USE_FEES_ACCOUNT", "int", ParameterDirection.Input, 0);//37
                cn.addparam("AL_USE_FEES_ACCOUNT_BR", "int", ParameterDirection.Input, 0);//38
                cn.addparam("AL_USE_FEES_ACCOUNT_CY", "int", ParameterDirection.Input, 0);//39
                cn.addparam("ADEC_TO_AMOUNT", "int", ParameterDirection.Input, 0);//40
                cn.addparam("ADEC_EXCH_RATE", "int", ParameterDirection.Input, 0);//41
                cn.addparam("AL_DIFF_TD_VD", "int", ParameterDirection.Input, 0);//42
                cn.addparam("ADT_VALUE_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//43
                cn.addparam("AL_NUMBER_1", "int", ParameterDirection.Input, DBNull.Value, 200);//44
                cn.addparam("AL_NUMBER_2", "int", ParameterDirection.Input, DBNull.Value, 200);//45
                cn.addparam("AL_NUMBER_3", "int", ParameterDirection.Input, DBNull.Value, 200);//46
                cn.addparam("AL_NUMBER_4", "int", ParameterDirection.Input, DBNull.Value, 200);//47
                cn.addparam("AL_NUMBER_5", "int", ParameterDirection.Input, DBNull.Value, 200);//48
                cn.addparam("AS_STRING_1", "string", ParameterDirection.Input, DBNull.Value, 200);//49
                cn.addparam("AS_STRING_2", "string", ParameterDirection.Input, DBNull.Value, 200);//50
                cn.addparam("AS_STRING_3", "string", ParameterDirection.Input, DBNull.Value, 200);//51
                cn.addparam("AS_STRING_4", "string", ParameterDirection.Input, DBNull.Value, 200);//52
                cn.addparam("AS_STRING_5", "string", ParameterDirection.Input, DBNull.Value, 200);//53
                cn.addparam("ADT_DATE_1", "string", ParameterDirection.Input, DBNull.Value, 200);//54
                cn.addparam("ADT_DATE_2", "string", ParameterDirection.Input, DBNull.Value, 200);//55
                cn.addparam("ADT_DATE_3", "string", ParameterDirection.Input, DBNull.Value, 200);//56
                cn.addparam("ADT_DATE_4", "string", ParameterDirection.Input, DBNull.Value, 200);//57
                cn.addparam("ADT_DATE_5", "string", ParameterDirection.Input, DBNull.Value, 200);//58
                cn.addparam("AL_ATM_TRX", "int", ParameterDirection.Input, DBNull.Value, 200);//59
                cn.addparam("AS_TRX_AMT", "string", ParameterDirection.Input, DBNull.Value, 200);//60
                cn.addparam("AS_SETTL_AMT", "string", ParameterDirection.Input, DBNull.Value, 200);//61
                cn.addparam("AS_TRACE_NUM", "string", ParameterDirection.Input, DBNull.Value, 200);//62
                cn.addparam("AS_SETTL_RATE", "string", ParameterDirection.Input, DBNull.Value, 200);//63
                cn.addparam("AS_LOCAL_TIME", "string", ParameterDirection.Input, DBNull.Value, 200);//64
                cn.addparam("AS_LOCAL_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//65
                cn.addparam("AS_SETTL_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//66
                cn.addparam("AS_EXPIRY_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//67
                cn.addparam("AS_CAPTURE_DATE", "string", ParameterDirection.Input, DBNull.Value, 200);//68
                cn.addparam("AS_IID", "string", ParameterDirection.Input, DBNull.Value, 200);//69
                cn.addparam("AS_TRACK2", "string", ParameterDirection.Input, DBNull.Value, 200);//70
                cn.addparam("AS_RET_REF_NO", "string", ParameterDirection.Input, DBNull.Value, 200);//71
                cn.addparam("AS_ACCEPTOR_TERM_ID", "string", ParameterDirection.Input, DBNull.Value, 200);//72
                cn.addparam("AS_ACCEPTOR_ID_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//73
                cn.addparam("AS_ACCEPTOR_NAME", "string", ParameterDirection.Input, DBNull.Value, 200);//74
                cn.addparam("AS_TRX_CURRENCY", "string", ParameterDirection.Input, DBNull.Value, 200);//75
                cn.addparam("AS_SETTL_CURRENCY", "string", ParameterDirection.Input, DBNull.Value, 200);//76
                cn.addparam("AS_TERMINAL_DATA", "string", ParameterDirection.Input, DBNull.Value, 200);//77
                cn.addparam("AS_POS_RESPONSE_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//78
                cn.addparam("AS_POS_SETTL_DATA", "string", ParameterDirection.Input, DBNull.Value, 200);//79
                cn.addparam("AS_REPLACEMENT_AMT", "string", ParameterDirection.Input, DBNull.Value, 200);//80
                cn.addparam("AS_REPLACEMENT_AMT_FC", "string", ParameterDirection.Input, DBNull.Value, 200);//81
                cn.addparam("AS_TRX_TYPE", "string", ParameterDirection.Input, DBNull.Value, 200);//82
                cn.addparam("AS_AUTH_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//83
                cn.addparam("AS_FROM_ACC", "string", ParameterDirection.Input, DBNull.Value, 200);//84
                cn.addparam("AS_TO_ACC", "string", ParameterDirection.Input, DBNull.Value, 200);//85
                cn.addparam("AS_FWD_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//86
                cn.addparam("AS_M_TYPE", "string", ParameterDirection.Input, DBNull.Value, 200);//87
                cn.addparam("AS_PROCESS_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//88
                cn.addparam("AS_CHOLDER_AMT_BILLING", "string", ParameterDirection.Input, DBNull.Value, 200);//89
                cn.addparam("AS_ACQ_INST_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//90
                cn.addparam("AS_FWD_INST_CODE", "string", ParameterDirection.Input, DBNull.Value, 200);//91
                cn.addparam("AS_FEE_CURRENCY", "string", ParameterDirection.Input, DBNull.Value, 200);//92
                cn.addparam("AS_FEE_TRX_PROCESS", "string", ParameterDirection.Input, DBNull.Value, 200);//93
                cn.addparam("AS_FEE_TRX", "string", ParameterDirection.Input, DBNull.Value, 200);//94
                cn.addparam("AS_CY_CODE_BILLING", "string", ParameterDirection.Input, DBNull.Value, 200);//95
                cn.addparam("AS_CREDIT_NBR", "string", ParameterDirection.Input, DBNull.Value, 200);//96
                cn.addparam("AS_ORG_DATA", "string", ParameterDirection.Input, DBNull.Value, 200);//97
                cn.addparam("AS_FILE_NAME", "string", ParameterDirection.Input, DBNull.Value, 200);//98
                cn.addparam("AL_INTERFACE_CODE", "int", ParameterDirection.Input, DBNull.Value, 200);//99

                //output
                cn.addparam("ODEC_AVAIL", "int", ParameterDirection.InputOutput);//100
                cn.addparam("OL_ERROR", "int", ParameterDirection.InputOutput);//101
                cn.addparam("OS_ERR_MSG", "string", ParameterDirection.InputOutput, "", 200);//102
                cn.addparam("OL_TRX_CODE", "int", ParameterDirection.InputOutput);//103
                cn.addparam("OL_FEES_TRX_CODE", "int", ParameterDirection.InputOutput);//104

                cn.query();

                //Read the output parameter after execution
                string temp0 = cn.cmd.Parameters["@OL_TRX_CODE"].Value.ToString();
                string temp1 = cn.cmd.Parameters["@OL_ERROR"].Value.ToString();
                string temp2 = cn.cmd.Parameters["@OS_ERR_MSG"].Value.ToString();


                if (temp1 == "0")
                {
                    ReleaseFundsResp rsp = new ReleaseFundsResp
                    {
                        responseCode = "0",
                        errorCode = "0",
                        transactionCode = temp0,
                        responseMessage = "Funds released"
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    ReleaseFundsResp rsp = new ReleaseFundsResp
                    {
                        responseCode = "99",
                        errorCode = temp1,
                        errorMsg = temp2
                    };
                    json = JsonConvert.SerializeObject(rsp);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }


            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                ReleaseFundsResp rsp = new ReleaseFundsResp
                {
                    responseCode = "-1",
                    errorCode = "-1",
                    errorMsg = "Error occurred during processing"
                };
                json = JsonConvert.SerializeObject(rsp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, json);

            }


        }

        /// <summary>
        /// API for Bills Payment
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("BillsPay")]
        public async Task<HttpResponseMessage> BillsPay([FromBody] BillsPayReq r)
        {

            Gadget g = new Gadget();
            string theResp = ""; string rmks = ""; string[] bits = null; string json = ""; string toAccount = "";

            try
            {
                toAccount = ConfigurationManager.AppSettings[r.ChannelName].ToString();
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                json = ex.Message;
                return Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }

            try
            {
                rmks = r.amount + " " + r.TransType + " on " + r.ChannelName + " from " + r.fromAccount + " ref " + r.referenceCode;

                imalcore.ServicesSoapClient ws = new imalcore.ServicesSoapClient();
                //first check if the account is sterling or imal
                sbpswitch.sbpswitchSoapClient sw = new sbpswitch.sbpswitchSoapClient();
                int bankid = sw.getInternalBankID(toAccount);

                //IMAL to IMAL txn

                theResp = ws.FundstransFer(r.fromAccount, toAccount, decimal.Parse(r.amount), rmks);
                bits = theResp.Split('*');
                if (bits[0] == "00")
                {
                    BillsPayResp rsp1 = new BillsPayResp
                    {
                        availabeBalanceAfterOperation = bits[2],
                        transactionID = r.referenceCode,
                        responseCode = "0",
                        responseMessage = "Transaction was successful",
                        errorCode = "0",
                        errorMessage = "Transaction was successful",
                        iMALTransactionCode = bits[4],
                        skipProcessing = "false",
                        originalResponseCode = "0",
                        skipLog = "false"
                    };
                    json = JsonConvert.SerializeObject(rsp1);
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    LocalFTResp rsp1 = new LocalFTResp
                    {
                        availabeBalanceAfterOperation = "",
                        responseCode = bits[0],
                        responseMessage = bits[1],
                        errorCode = bits[0],
                        errorMessage = bits[1],
                        iMALTransactionCode = "0",
                        skipProcessing = "false",
                        originalResponseCode = bits[0],
                        skipLog = "false"
                    };
                    json = JsonConvert.SerializeObject(rsp1);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, json);
                }
            }
            catch (Exception ex)
            {
                new ErrorLog(ex);
                json = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, json);
        }
    }
}