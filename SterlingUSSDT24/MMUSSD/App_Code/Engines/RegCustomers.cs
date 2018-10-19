using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using VTU.ObjectInfo;
using VTU.UTILITIES;
using VTU.CRYPTO;
using VTU.IBSService;
using VTU.EwService;
using System.Configuration;
using System.Net;
using System.IO;
using PlentyMoney;

/// <summary>
/// Summary description for RegCustomers
/// </summary>
public class RegCustomers : BaseEngine
{

    public int Activate(Go_Registered_Account a, string pin, int bypass)
    {
        int resp;
        try
        {
            //SqlParameter[] parameters = {
            //           new SqlParameter("@Mobile",a.Mobile)
            //          ,new SqlParameter("@NUBAN",a.NUBAN )
            //          ,new SqlParameter("@StatusFlag",a.StatusFlag )
            //          ,new SqlParameter("@TransactionRefID",a.TransactionRefID)
            //          ,new SqlParameter("@RefID",a.RefID)
            //        };

            //string sql = " IF NOT Exists (Select 1 from [dbo].[Go_Registered_Account] Where [Mobile] = @Mobile AND [NUBAN] = @NUBAN) "
            //           + " BEGIN INSERT INTO Go_Registered_Account ([Mobile],[NUBAN],[DateRegistered],[StatusFlag],[RefID],[TransactionRefID]) "
            //           + " VALUES (@Mobile,@NUBAN,GetDate(),@StatusFlag,@RefID,@TransactionRefID ) SELECT  @@ROWCOUNT END ELSE BEGIN SELECT -1 END ";

            //resp = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);

            if (bypass == 102)
            {
                Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
                cn.SetProcedure("spd_RegisterCustomerWithLimit");
                cn.AddParam("@Mobile", a.Mobile);
                cn.AddParam("@NUBAN", a.NUBAN);
                cn.AddParam("@StatusFlag", a.StatusFlag);
                cn.AddParam("@TransactionRefID", a.TransactionRefID);
                cn.AddParam("@RefID", a.RefID);
                cn.AddParam("@Pin", pin);
                resp = Convert.ToInt32(cn.ExecuteProc());
            }
            else
            {
                Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn");
                cn.SetProcedure("sbp_RegisterCustomer");
                cn.AddParam("@Mobile", a.Mobile);
                cn.AddParam("@NUBAN", a.NUBAN);
                cn.AddParam("@StatusFlag", a.StatusFlag);
                cn.AddParam("@TransactionRefID", a.TransactionRefID);
                cn.AddParam("@RefID", a.RefID);
                cn.AddParam("@Pin", pin);
                resp = Convert.ToInt32(cn.ExecuteProc());
            }

        }
        catch (Exception ex)
        {
            resp = -99;
            ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
        }
        return resp;
    }


    public string DoActivate(string mobile, string nuban, string pin, int bypass)
    {
        string resp = "";

        
        //check if nuban starts with 05.
        if (nuban.StartsWith("05"))
        {
            IMALEngine ime = new IMALEngine();
            resp = ime.DoImalReg(mobile, nuban, pin,bypass);
            return resp;
        }

        //Check if BankOne Customer
        if (nuban.StartsWith("11"))
        {
            //Treat as BankOne
            BankOneService.BankOneTransactionRequestServiceClient bankOne_WS = new BankOneService.BankOneTransactionRequestServiceClient();
            //BankOneNameEnquiry bank1 = new BankOneNameEnquiry();
            BankOneClass bank1 = new BankOneClass();
            bank1.accountNumber = nuban;
            string bankOneReq = bank1.createRequestForNameEnq();
            string encrptdReq = EncryptTripleDES(bankOneReq);
            string getBankOneResp = bankOne_WS.BankOneGetAccountName(encrptdReq);
            bool isBankOne = bank1.readResponseForNameEnq(getBankOneResp);
            if (bank1.status == "True")
            {
                Go_Registered_Account g = new Go_Registered_Account();
                g.Mobile = mobile;
                g.NUBAN = nuban;
                g.StatusFlag = Constants.Status_Green.ToString();
                g.TransactionRefID = DateTime.Now.Ticks.ToString();
                g.RefID = "1987";
                int retval = Activate(g, pin,bypass);
                switch (retval)
                {
                    case -99:
                        resp = "All connections are currently in use%0A Kindly try again!";
                        break;
                    case -1:
                        resp = "You have already been activated for this service%0ADial *822*Amount# to recharge your phone.%0Ae.g *822*500# ";
                        break;
                    case 1:
                        resp = "Great! You are successfully registered for Sterling USSD Service. Please dial *822# to experience the magic…";
                        break;
                }
            }
            return resp;
        }

        //else go to T24
        EACBS.banksSoapClient ws = new EACBS.banksSoapClient();
        DataSet ds = ws.getAccountFullInfo(nuban);
        if (ds != null)
        {
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];
                        string mob = dr["MOB_NUM"].ToString();
                        VTU.UTILITIES.Common.formatMobile(ref mob);
                        VTU.UTILITIES.Common.formatMobile(ref mobile);
                        string curCode = dr["T24_CUR_CODE"].ToString();
                        if (mobile == mob)
                        {
                            if (curCode == "566")
                            {
                                Go_Registered_Account g = new Go_Registered_Account();
                                g.Mobile = mobile;
                                g.NUBAN = nuban;
                                g.StatusFlag = Constants.Status_Green.ToString();
                                g.TransactionRefID = DateTime.Now.Ticks.ToString();
                                g.RefID = "1987";
                                int retval = Activate(g, pin, bypass);
                                switch (retval)
                                {
                                    case -99:
                                        resp = "All connections are currently in use%0A Kindly try again!";
                                        break;
                                    case -1:
                                        resp = "You have already been activated for this service%0ADial *822*Amount# to recharge your phone.%0Ae.g *822*500# ";
                                        break;
                                    case 1:
                                        resp = "Great! You are successfully registered for Sterling USSD Service. Please dial *822# to experience the magic…";
                                        //resp = "You have successfully set your PIN.%0A Dial *822# to continue your transaction.    %0A You can now send up to N50,000/transaction.";
                                        break;
                                }
                            }
                            else // Non Naira Account 
                            {
                                // Non Naira account is not allowed for this service
                                resp = "The specified account is not allowed for this service.%0AEnter a valid naira account!";
                            }
                        }
                        else
                        {
                            // Phone number not match with the NUBAN supplied
                            //resp = "We could not match this phone number to your bank profile.%0AKindly approach the nearest sterling bank to update your profile.";
                            resp = "Your phone number and account number do not match. Kindly visit the nearest branch to update your details";
                        }
                    }
                    else
                    {
                        //Unusal record set returned
                        resp = "All connections are currently in use, Please try again.";
                    }
                }
                else
                {
                    //When no record exist on banks
                    resp = "This service applies to sterling customers only.%0AKindly approach the nearest sterling bank to open account with us.Thank you.";
                }
            }
            else
            {
                //When no record exist on banks
                resp = "This service applies to sterling customers only.%0AKindly approach the nearest sterling bank to open account with us.Thank you.";
            }
        }
        else
        {
            //Exception in disguise
            resp = "All connections are currently in use, Please try again.";
        }
        return resp;
    }
}