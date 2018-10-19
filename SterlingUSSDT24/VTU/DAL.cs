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

namespace VTU.DAL
{
    /// <summary>
    /// This is the Data Access Layer for the thre phase of go-pinless application :
    /// USSD phase, Console Engine phase and GUI phase
    /// </summary>
    /// 
    class AppConnection
    {
        public static SqlConnection GetAppConnection()
        {
            string configCon = System.Configuration.ConfigurationManager.AppSettings["mssqlconn"];
            return new SqlConnection(configCon);
        }
    }

    class USSD_db
    {
        //get customer account details bu NUBAN
        public static DataSet getBankDetail(string nuban)
        {
            try
            { 
                VTU.EwService.Service svc = new VTU.EwService.Service();
                DataSet dsVal = svc.GetAccountDetailsByNuban(nuban);
                if (dsVal != null)
                {
                    return dsVal;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        //Store activation request
        public static int Activate(Go_Registered_Account a)
        {
            int resp;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@Mobile",a.Mobile)
                      ,new SqlParameter("@NUBAN",a.NUBAN )
                      ,new SqlParameter("@StatusFlag",a.StatusFlag )
                      ,new SqlParameter("@TransactionRefID",a.TransactionRefID)
                      ,new SqlParameter("@RefID",a.RefID)
                    };

                string sql = " IF NOT Exists (Select 1 from [dbo].[Go_Registered_Account] Where [Mobile] = @Mobile AND [NUBAN] = @NUBAN) "
                           + " BEGIN INSERT INTO Go_Registered_Account ([Mobile],[NUBAN],[DateRegistered],[StatusFlag],[RefID],[TransactionRefID]) "
                           + " VALUES (@Mobile,@NUBAN,GetDate(),@StatusFlag,@RefID,@TransactionRefID ) SELECT  @@ROWCOUNT END ELSE BEGIN SELECT -1 END ";

              resp = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                resp = -99;
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
            }
            return resp;
        }

        public static DataTable getConfigAccount()
        {
            try
            {
                string sql = "select * from tbl_USSD_Accounts ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, null).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getTransaction(string startDate,string endDate)
        {
            try
            {
                string sql = " select * from tbl_USSD_Accounts ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, null).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getDailyTxnSum(string mobile)
        {
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@Mobile",mobile)
                    };
                string sql = "select isnull((select SUM(Amount) from [dbo].[Go_Request] where Convert(date,[RequestDate]) = Convert(date,GETDATE()) AND Mobile  = @Mobile AND RequestStatus <> 2),0)";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getDailyTxnSum(string mobile,int reqType) //1: for self, 2: for friend
        {
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@Mobile",mobile)
                      ,new SqlParameter("@reqType",reqType)
                    };
                string sql = "select isnull((select SUM(Amount) from [dbo].[Go_Request] where Convert(date,[RequestDate]) = Convert(date,GETDATE()) AND Mobile  = @Mobile AND RequestStatus <> 2 AND [RequestType] = @reqType),0)";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getRegisteredAcct(string mobile)
        {
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@Mobile",mobile)
                    };
                string sql = " select top 1 * from [dbo].[Go_Registered_Account] where  Mobile  = @Mobile and Activated in (1,0) and statusflag = 1 Order by Activated desc ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }        

        public static int LogRequest(Go_Request  a)
        {
            int resp;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@Amount",a.Amount)
                      ,new SqlParameter("@Beneficiary",a.Beneficiary )
                      ,new SqlParameter("@Mobile",a.Mobile )
                      ,new SqlParameter("@NUBAN",a.NUBAN)
                      ,new SqlParameter("@RequestStatus",a.RequestStatus)
                      ,new SqlParameter("@RequestType",a.RequestType)
                      ,new SqlParameter("@RequestRef",a.RequestRef)
                      ,new SqlParameter("@NetworkID", Convert.ToInt32(a.NetworkID))
                      ,new SqlParameter("@sessionId",a.sessionId)
                      ,new SqlParameter("@ChannelID",a.ChannelID)
                    };

                string sql = "  INSERT INTO [dbo].[Go_Request] ([Mobile],[Beneficiary],[Amount],[RequestDate],[NUBAN],[RequestRef],[RequestType],[NetworkID],[SessionId],[ChannelID])  "
                           + "  VALUES ( @Mobile, @Beneficiary, @Amount,GetDate(), @NUBAN, @RequestRef, @RequestType,@NetworkID,@sessionId,@ChannelID)  "
                           + "  Select @@Rowcount ";

                resp = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                resp = -99;
            }
            return resp;
        }

        public static object LogRequest_getRefID(Go_Request a)
        {
            object resp;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@Amount",a.Amount)
                      ,new SqlParameter("@Beneficiary",a.Beneficiary)
                      ,new SqlParameter("@Mobile",a.Mobile)
                      ,new SqlParameter("@NUBAN",a.NUBAN)
                      ,new SqlParameter("@RequestStatus",a.RequestStatus)
                      ,new SqlParameter("@RequestType",a.RequestType)
                      ,new SqlParameter("@RequestRef",a.RequestRef)
                      ,new SqlParameter("@NetworkID",a.NetworkID)
                      ,new SqlParameter("@sessionId",a.sessionId)
                      ,new SqlParameter("@ChannelID",a.ChannelID)
                    };

                string sql = "  INSERT INTO [dbo].[Go_Request] ([Mobile],[Beneficiary],[Amount],[RequestDate],[NUBAN],[RequestRef],[RequestType],[NetworkID],[SessionId],[ChannelID],[RequestStatus])  "
                           + "  VALUES ( @Mobile, @Beneficiary, @Amount,GetDate(), @NUBAN, @RequestRef, @RequestType,@NetworkID,@sessionId,@ChannelID,@RequestStatus)  "
                           + "  Select SCOPE_IDENTITY() ";

                resp = SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                resp = -99;
            }
            return resp;
        }

        public static string SendMessage(string PostData,string mobile)
        {
            string prx = ConfigurationManager.AppSettings["PROXY"];
            if (prx == "YES")
            {
                WebRequest.DefaultWebProxy = new WebProxy("10.0.0.120", 80)
                {
                    Credentials = new NetworkCredential("ibsservice", "Sterling123")
                };
            }
            string uri = ConfigurationManager.AppSettings["mfinoAPI"];
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            string txt = uri +"?"+ PostData;
        
            ApplicationLog reqlg = new ApplicationLog(txt, "mfino_user", mobile);
            string responseString;
            using (WebClient client = new WebClient())
            {
                responseString = client.DownloadString(txt);
            }
            ApplicationLog reslg = new ApplicationLog(responseString, "mfino_user", mobile);
            return responseString;
        }

        public static string SendVTURequestwithPin(string PostData, string mobile)
        {
            string prx = ConfigurationManager.AppSettings["PROXY"];
            if (prx == "YES")
            {
                WebRequest.DefaultWebProxy = new WebProxy("10.0.0.120", 80)
                {
                    Credentials = new NetworkCredential("ibsservice", "Sterling123")
                };
            }

            
            string uri = ConfigurationManager.AppSettings["mfinoAPI"];
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            string txt = uri + "?" + PostData;
         
            string responseString ;
            using (WebClient client = new WebClient())
            {
                responseString = client.DownloadString(txt);
            }
            return responseString;
        }

        public static int ValidateBlacklist(string Account,string Misdn)
        {
            int resp;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@Misdn",Misdn)
                      ,new SqlParameter("@Account",Account )
                    };

                string sql = "  select 1 from [dbo].[Go_BlackList] where [Misdn] = @Misdn AND [Account] = @Account  ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        //Convict
                        resp = 1;
                    }
                    else
                    { 
                        //No Convict
                        resp = 0;
                    }
                }
                else
                { 
                    //connection error
                    resp = -1;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                resp = -99;
            }
            return resp;
        }

        public static DataTable SaveSteps(string SessionId, string Misdn, int reqType,string val,string prev,string next)
        {
            string sql = " insert into tbl_USSD_reqstate (SessionId,Msisdn,AppID,params,prev,next) values (@SessionId,@Misdn,@ReqType,@val,@prev,@next) select @@rowcount";
            SqlParameter[] parameters = {
                       new SqlParameter("@SessionId",SessionId)
                      ,new SqlParameter("@Misdn",Misdn )
                      ,new SqlParameter("@ReqType",reqType )
                      ,new SqlParameter("@val",val)
                      ,new SqlParameter("@prev",prev)
                      ,new SqlParameter("@next",next )
                    };

            DataSet ds = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            return ds.Tables[0];
        }

        public static DataTable GetSteps(string SessionId, string Misdn)
        {
            string sql = " select * from tbl_USSD_reqstate where SessionID = @SessionId AND Msisdn = @Msisdn order by refID desc";
            SqlParameter[] parameters = {
                       new SqlParameter("@SessionId",SessionId)
                      ,new SqlParameter("@Misdn",Misdn )
                    };

            DataSet ds = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            return ds.Tables[0];
        }

        public static int UpdateInput(string SessionId, string Misdn, string val, string prev, string next)
        {
            string sql = " Begin If Exists (Select 1 from [dbo].tbl_USSD_reqstate where SessionId = @SessionID and msisdn = @Misdn) begin "
                         + " UPDATE tbl_USSD_reqstate SET params = isnull( params,'') + @Val  ,prev = @prev ,[next] = @next WHERE SessionId = @SessionID and msisdn = @Misdn select @@rowcount "
                         + " end else begin INSERT INTO tbl_USSD_reqstate ( SessionId ,msisdn,params ,Prev ,[Next] ) VALUES ( @SessionID,@Misdn ,@Val ,@prev ,@next) select @@rowcount end End ";

            SqlParameter[] parameters = {
                       new SqlParameter("@SessionId",SessionId)
                      ,new SqlParameter("@Misdn",Misdn )
                      ,new SqlParameter("@val",val)
                      ,new SqlParameter("@prev",prev)
                      ,new SqlParameter("@next",next )
                    };

            DataSet ds = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        }

        public static DataTable getRegisteredProfile(string mobile, string nuban)
        {
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@Mobile",mobile),
                        new SqlParameter("@nuban",nuban)
                    };
                string sql = " select * from [dbo].[Go_Registered_Account] where  Mobile  = @Mobile and Activated in (1,0) and statusflag = 1 and NUBAN = @nuban Order by Activated desc ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable GetLimitMINMAXamt()
        {
            try
            {
                
                string sql = "select minamt,maxamt from tbl_USSD_AMTRANGE_Bypass where EnrollProp=1";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, null).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }

            //string sql = "select minamt,maxamt from tbl_USSD_AMTRANGE_Bypass where EnrollProp=1";
            //Connect c = new Connect(sql, true);
            //DataSet ds = c.query("rec");
            //return ds;
        }

        public static DataTable getTotalTransDonePerday(decimal amt, string mobile, string sessionid, decimal Maxperday)
        {

            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@Mobile",mobile)
                    };
                string sql = "select ISNULL(SUM(amt),0) as totalTOday,ISNULL(count(amt),0) as cnt from tbl_USSD_transfers " +
                " where mobile =@Mobile " +
                " and CONVERT(Varchar(20), dateadded,102) = CONVERT(Varchar(20), GETDATE(),102) and statusflag = 1 ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
            //DataSet ds = new DataSet();
            //bool ok = false;
            //string sql = "select ISNULL(SUM(amt),0) as totalTOday,ISNULL(count(amt),0) as cnt from tbl_USSD_transfers " +
            //    " where mobile =@mob " +
            //    " and CONVERT(Varchar(20), dateadded,102) = CONVERT(Varchar(20), GETDATE(),102) and statusflag = 1 ";
            //Connect c = new Connect(sql, true);
            //c.addparam("@mob", mobile);
            //ds = c.query("rec");
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    DataRow dr = ds.Tables[0].Rows[0];
            //    Totaldone = decimal.Parse(dr["totalTOday"].ToString());
            //    totlcnt = int.Parse(dr["cnt"].ToString());
            //    if (Totaldone + amt > Maxperday)
            //    {
            //        ok = true;
            //    }
            //    else
            //    {
            //        ok = false;
            //    }
            //}
            //else
            //{
            //    ok = false;
            //}

            //return ok;
        }

    }

    class Console_db
    {
        public static DataTable getBanksMobile(string nuban)
        {
            try
            {
                VTU.EwService.Service svc = new VTU.EwService.Service();
                DataSet dsVal = svc.GetAccountDetailsByNuban(nuban);
                if (dsVal != null)
                {
                    return dsVal.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static string DoVTU_Request(Svc_Request svc)
        {
            string resp = "";
            try
            {
                //<ReferenceID> </ReferenceID>
                string raw_req = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><IBSRequest><RequestType>{0}</RequestType><BillId>{1}</BillId><ChannelId>{2}</ChannelId><BillAmount>{3}</BillAmount>"
                               + "<BillAccount>{4}</BillAccount><CallerRefID>{5}</CallerRefID><SubscriberInfo1>{6}</SubscriberInfo1>"
                               + "<SubscriberInfo2>{7}</SubscriberInfo2><HashValue>{8}</HashValue><RefId>{9}</RefId><ReferenceID>{10}</ReferenceID></IBSRequest>";

                string req = string.Format(raw_req,svc.RequestType,svc.BillId,svc.ChannelId,svc.BillAmount,svc.BillAccount,svc.CallerRefID
                                         , svc.SubscriberInfo1, svc.SubscriberInfo2, svc.HashValue, svc.RefId,Constants.AppID);

                ApplicationLog reqlg = new ApplicationLog(req, "vtu_bpas",svc.SubscriberInfo1);
                string EncReq = CryptoSterling.IBS_Encrypt(req);
                IBSServices Ibs = new IBSServices();
                string EncResp = Ibs.IBSBridge(EncReq, Constants.AppID);
                resp = CryptoSterling.IBS_Decrypt(EncResp);

            }
            catch (Exception ex)
            {
                resp = "Error Has occurred";
                ApplicationLog reqlg = new ApplicationLog(ex, "svclog");
            }
            return resp;
        }

        public static int UpdateRequest(Go_Request req,Svc_Response svc_resp)
        {
            int resp;
            try
            {
                SqlParameter[] parameters = {
                      
                       new SqlParameter("@NUBAN",req.NUBAN)
                      ,new SqlParameter("@Mobile",req.Mobile )
                      ,new SqlParameter("@Beneficiary",req.Beneficiary)
                      ,new SqlParameter("@RequestRef",req.RequestRef)
                      ,new SqlParameter("@RequestStatus",req.RequestStatus )
                      ,new SqlParameter("@CallerRefID",req.CallerRefID)

                      ,new SqlParameter("@BillerRef",svc_resp.BillerRef )
                      ,new SqlParameter("@BillerResp",svc_resp.BillerResp)
                      ,new SqlParameter("@RefId",svc_resp.RefId)
                      ,new SqlParameter("@ResponseCode",svc_resp.ResponseCode)
                      ,new SqlParameter("@ResponseText",svc_resp.ResponseText)
                    };

                string sql = "  UPDATE Go_Request SET RefID = @refID, BillerRef = @BillerRef, BillerResp = @BillerResp, ResponseCode = @ResponseCode, ResponseText = @ResponseText, ProcessDate = Getdate(),RequestStatus = @RequestStatus "
                           + "  WHERE Mobile= @Mobile AND Beneficiary = @Beneficiary AND NUBAN = @NUBAN AND RequestRef = @RequestRef AND CallerRefID = @CallerRefID Select @@rowcount ";

                resp = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                resp = -99;
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
            }
            return resp;
        }

        public static DataTable getNetwork(string prefix)
        {
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@NetworkID",prefix)
                    };
                string sql = " select * from [dbo].[Go_Network_Prefix] where  NetworkID like '%'+@NetworkID+'%' ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static void dataArchive(DataTable dt)
        {
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string CallerRefID = dr["CallerRefID"].ToString();
                    string sql = "DECLARE @DELFLAG INT = 0 DECLARE @INSCOUNTER INT = 0 BEGIN TRANSACTION BEGIN TRY  "
                               + " INSERT INTO [Go_Request_Archive] SELECT * FROM [dbo].[Go_Request] WHERE [CallerRefID] = @CallerRefID  SELECT @INSCOUNTER = @@ROWCOUNT "
                               + " DELETE FROM [Go_Request]  WHERE [CallerRefID] = @CallerRefID SELECT @DELFLAG =  @@ROWCOUNT "
                               + " IF(@INSCOUNTER = @DELFLAG) COMMIT ELSE ROLLBACK END TRY BEGIN CATCH  ROLLBACK END CATCH  SELECT @DELFLAG,@INSCOUNTER  ";

                    SqlParameter[] parameters = {
                         new SqlParameter("@CallerRefID",CallerRefID)
                    };
                    DataSet ds = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
                }
            }
            catch (System.Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
            }
        }

        public static void resetTimedOut(int minLapse)
        {
            try
            {
                SqlParameter[] parameters = {
                         new SqlParameter("@minLapse",minLapse)
                        ,new SqlParameter("@Status_Timeout",Constants.Status_Timeout )
                        ,new SqlParameter("@Status_Blue",Constants.Status_Blue)
                    };
                string sql = "Update[dbo].[Go_Request] Set [RequestStatus] = @Status_Timeout, [ProcessDate] = getdate() where DATEDIFF(MINUTE, RequestDate, GETDATE())  > @minLapse and RequestStatus =   @Status_Blue ";
                   
                    DataSet ds = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (System.Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
            }
        }

        public static DataTable getTimedOutRequest(int MinLapses)
        {
            try
            {
                SqlParameter[] parameters = {
                         new SqlParameter("@MinLapses",MinLapses)
                        ,new SqlParameter("@Status_Timeout",Constants.Status_Timeout)
                        ,new SqlParameter("@Status_Blue",Constants.Status_Blue )
                    };

                string sql = " select * from [dbo].[Go_Request] where [ProcessDate] between DATEADD(minute,@MinLapses,GETDATE()) and DATEADD(minute,0,GETDATE()) AND (requestStatus = @Status_Blue OR requestStatus = @Status_Timeout) Order by ProcessDate asc  ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getCompletedRequest()
        {
            try
            {
                string sql = " select * from [dbo].[Go_Request] where Convert(date,RequestDate) < Convert(date,GETDATE())";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static int LogStamp(RequestLog a)
        {
            int resp;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@Proc_RefID",a.Ref)
                      ,new SqlParameter("@Proc_Start",a.Proc_Start  )
                      ,new SqlParameter("@Proc_End",a.Proc_End  )
                      ,new SqlParameter("@NetworkID",a.networkID )
                    };

                string sql = "  INSERT INTO Go_Process_Stamp (Proc_RefID,proc_start,proc_end,NetworkID)  "
                           + "  VALUES ( @Proc_RefID,@Proc_Start,@Proc_End,@NetworkID)  "
                           + "  Select @@Rowcount ";

                resp = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                resp = -99;
            }
            return resp;
        }

        public static int UpdateStamp(RequestLog a)
        {
            int resp;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@Proc_RefID",a.Ref)
                      ,new SqlParameter("@NetworkID",a.networkID )
                      ,new SqlParameter("@proc_end",a.Proc_End )
                    };

                string sql = "  UPDATE Go_Process_Stamp SET proc_end = @proc_end WHERE Proc_RefID = @Proc_RefID AND NetworkID = @NetworkID "
                           + "  Select @@Rowcount ";

                resp = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                resp = -99;
            }
            return resp;
        }

        public static int SetFlag(Go_Request a)
        {
            int resp;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@NUBAN",a.NUBAN)
                      ,new SqlParameter("@Mobile",a.Mobile )
                      ,new SqlParameter("@Beneficiary",a.Beneficiary)
                      ,new SqlParameter("@RequestRef",a.RequestRef)
                      ,new SqlParameter("@RequestStatus",a.RequestStatus)
                      ,new SqlParameter("@CallerRefID",a.CallerRefID)
                    };

                string sql = "  UPDATE Go_Request SET RequestStatus = @RequestStatus "
                           + "  WHERE Mobile= @Mobile AND Beneficiary = @Beneficiary AND NUBAN = @NUBAN AND RequestRef = @RequestRef AND CallerRefID = @CallerRefID Select @@rowcount ";

                resp = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                resp = -99;
            }
            return resp;
        }

        public static string SendMessage(string PostData,string mobile)
        {
            string prx = ConfigurationManager.AppSettings["PROXY"];
            if (prx == "YES")
            {
                WebRequest.DefaultWebProxy = new WebProxy("10.0.0.120", 80)
                {
                    Credentials = new NetworkCredential("ibsservice", "Sterling123")
                };
            }

            string uri = ConfigurationManager.AppSettings["mfinoAPI"];
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            string txt = uri + "?" + PostData;
            ApplicationLog reqlg = new ApplicationLog(DateTime.Now.ToString()+" "+txt, "mfino_user", mobile);
            string responseString;
            using (WebClient client = new WebClient())
            {
                responseString = client.DownloadString(txt);
            }
            ApplicationLog reslg = new ApplicationLog(DateTime.Now.ToString() + " " + responseString, "mfino_user", mobile);
            return responseString;
        }

        public static DataTable getRegisteredAcct()
        {
            try
            {
                string sql = " select * from [dbo].[Go_Registered_Account]  Order by DateRegistered desc ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static void UpdateRegisteredAcct(string oMobile,string nMobile,string nuban)
        {
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@oMobile",oMobile)
                       ,new SqlParameter("@nMobile",nMobile  )
                      ,new SqlParameter("@NUBAN",nuban  )
                    };

                string sql = " update[dbo].[Go_Registered_Account]  set  Mobile = @nMobile Where NUBAN = @NUBAN AND Mobile = @oMobile ";
                SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
            }
        }
    }

    class GUI_db
    {
        #region Reports
        public static DataTable getBanksMobile(string nuban)
        {
            VTU.EwService.Service svc = new VTU.EwService.Service();
            DataSet dsVal = svc.GetAccountDetailsByNuban(nuban);
            if (dsVal != null)
            {
                return dsVal.Tables[0];
            }
            else
            {
                return null;
            }
        }

        public static DataTable getReport(string nm, string st, string en, string fl, string chn)
        {
            if (!string.IsNullOrEmpty(nm))
            {
                Common.formatMobile(ref nm);
            }
            DataTable dt;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@nm",nm )
                      ,new SqlParameter("@st",st )
                      ,new SqlParameter("@en",en )
                      ,new SqlParameter("@fl",fl )
                      ,new SqlParameter("@chn",chn )
                    };

                string sql = " SELECT r.[CallerRefID],r.[Mobile],r.[Beneficiary],r.[Amount] "
                    + "   ,ISNULL((select Mobile_Network from Go_Network_Prefix mn where mn.NetworkID = r.[NetworkID]),'N/A' ) NetworkID ,case r.[ChannelID] when 1  then 'Bank' when 2 then 'Mobile Money' end ChannelID  "
                    + " ,r.[NUBAN], case r.[RequestType] when 1 then 'Self' when 2 then 'Other' end RequestType,r.[RequestDate] "
                    + "  ,case r.[RequestStatus] when 0 then 'Pending' when 1 then 'Completed' when 2 then 'Rejected' end RequestStatus,r.[ProcessDate],r.[SessionID] SessionID,r.[ResponseText] Remark,r.[ResponseCode] RemarkCode "
                    + " FROM [DBO].[GO_REQUEST_ARCHIVE] r WHERE ( [MOBILE] = @nm OR @nm = '' OR  @nm IS NULL OR [NUBAN] = @nm) AND ([REQUESTSTATUS] = @FL OR @FL = -1) AND (ChannelID = @chn OR @chn = 0) AND (convert(date,[RequestDate]) between  convert(date,@st)  and  convert(date,@en) ) ";

                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
            }
            catch  
            {
                dt = null;
            }
            return dt;
        }

        public static DataTable getReport_Daily(string nm, string fl, string chn)
        {
            if (!string.IsNullOrEmpty(nm))
            {
                Common.formatMobile(ref nm);
            }

            DataTable dt;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@nm",nm )
                      ,new SqlParameter("@fl",fl )
                      ,new SqlParameter("@chn",chn )
                    };

                string sql = " SELECT r.[CallerRefID],r.[Mobile],r.[Beneficiary],r.[Amount] "
                    + "   ,ISNULL((select Mobile_Network from Go_Network_Prefix mn where mn.NetworkID = r.[NetworkID]),'N/A' ) NetworkID ,case r.[ChannelID] when 1  then 'Bank' when 2 then 'Mobile Money' end ChannelID  "
                    + " ,r.[NUBAN], case r.[RequestType] when 1 then 'Self' when 2 then 'Other' end RequestType,r.[RequestDate] "
                    + "  ,case r.[RequestStatus] when 0 then 'Pending' when 1 then 'Completed' when 2 then 'Rejected' else 'TimeOut' end RequestStatus,r.[ProcessDate],r.[SessionID] as SessionID,r.[ResponseText] as Remark,r.[ResponseCode] as RemarkCode "
                    + " FROM [DBO].[GO_REQUEST] r WHERE ( [MOBILE] = @nm OR @nm = '' OR  @nm IS NULL OR [NUBAN] = @nm) AND ([REQUESTSTATUS] = @FL OR @FL = -1) AND (ChannelID = @chn OR @chn = 0)  ";
                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
            }
            catch  
            {
                dt = null;
            }
            return dt;
        }

        public static DataSet DashBoadr()
        {
            DataSet dt;
            try
            {
                string sql = " DECLARE @q1  AS NVARCHAR(MAX), @q2  AS NVARCHAR(MAX), @q3  AS NVARCHAR(MAX),@q4  AS NVARCHAR(MAX),@q5  AS NVARCHAR(MAX) set @q1 =  "
                          + " ' With BaseQuery AS (Select n.Mobile_Network NetworkID,RequestStatus,Amount,ChannelID,n.NetworkID ntw from Go_Request left join [dbo].[Go_Network_Prefix] n on n.NetworkID = Go_Request.NetworkID "
                          + "   ) select NetworkID, ([0]+[1]+[2]+[89])  Total,[0] Pending ,[1] Completed ,[2] Rejected ,[89] TimedOut from BaseQuery  PIVOT (count(Amount) for RequestStatus in ([0],[1],[2],[89])) as pvt where channelID = 1  order by ntw asc' "

                          + " set @q2 = ' With BaseQuery AS ( Select n.Mobile_Network NetworkID,RequestStatus,Amount,ChannelID,n.NetworkID ntw from Go_Request left join [dbo].[Go_Network_Prefix] n on n.NetworkID = Go_Request.NetworkID ) "
                          + "  select NetworkID, (Isnull([0],0)+ Isnull([1],0)+ Isnull([2],0)+Isnull([89],0)) Total, Isnull([0],0) Pending ,Isnull([1],0) Completed ,Isnull([2],0) Rejected ,Isnull([89],0) TimedOut from BaseQuery  "
                          + " PIVOT (sum(Amount) for RequestStatus in ([0],[1],[2],[89])) as pvt where channelID = 1  order by ntw asc'  "

                          + "  set @q3 = ' With BaseQuery AS ( Select n.Mobile_Network NetworkID,RequestStatus,Amount,ChannelID,n.NetworkID ntw from Go_Request left join [dbo].[Go_Network_Prefix] n on n.NetworkID = Go_Request.NetworkID "
                          + " ) select NetworkID, ([0]+[1]+[2]+[89])  Total,[0] Pending ,[1] Completed ,[2] Rejected ,[89] TimedOut from BaseQuery PIVOT (count(Amount) for RequestStatus in ([0],[1],[2],[89])) as pvt where channelID = 2  order by ntw asc' "

                          + "  set @q4 = ' With BaseQuery AS (Select n.Mobile_Network NetworkID,RequestStatus,Amount,ChannelID,n.NetworkID ntw from Go_Request left join [dbo].[Go_Network_Prefix] n on n.NetworkID = Go_Request.NetworkID  "
                          + " ) select NetworkID, (Isnull([0],0)+ Isnull([1],0)+ Isnull([2],0)+Isnull([89],0)) Total, Isnull([0],0) Pending ,Isnull([1],0) Completed ,Isnull([2],0) Rejected ,Isnull([89],0) TimedOut from BaseQuery PIVOT (sum(Amount) for RequestStatus in ([0],[1],[2],[89])) as pvt where channelID = 2  order by ntw asc' "

                          + "  set @q5 = 'Select isnull( count(*),0) Today from [dbo].[Go_Registered_Account] where convert(date,[DateRegistered]) = convert(date,getdate()) Select case statusflag when 1 then ''Active'' when 0 then ''Inactive'' end flag,isnull( count(*),0) All_ from [dbo].[Go_Registered_Account] group by  statusflag' "

                          + "  execute(@q1) execute(@q2)  execute(@q3) execute(@q4) execute(@q5)  "

                          + "  ";


                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql);
            }
            catch  
            {
                dt = null;
            }
            return dt;
        }

        public static DataSet DashBoard(string st, string en)
        {
            DataSet dt;
            try
            {
                string sql = " DECLARE @q1  AS NVARCHAR(MAX), @q2  AS NVARCHAR(MAX), @q3  AS NVARCHAR(MAX),@q4  AS NVARCHAR(MAX),@q5  AS NVARCHAR(MAX) set @q1 =  "
                           + " ' With BaseQuery AS (Select n.Mobile_Network NetworkID,RequestStatus,Amount,ChannelID,n.NetworkID ntw from Go_Request_Archive left join [dbo].[Go_Network_Prefix] n on n.NetworkID = Go_Request_Archive.NetworkID   where convert(date,Go_Request_Archive) between  convert(date,@st)  and  convert(date,@en) "
                           + "   ) select NetworkID, ([0]+[1]+[2]+[89])  Total,[0] Pending ,[1] Completed ,[2] Rejected ,[89] TimedOut from BaseQuery  PIVOT (count(Amount) for RequestStatus in ([0],[1],[2],[89])) as pvt where channelID = 1  order by ntw asc' "

                           + " set @q2 = ' With BaseQuery AS ( Select n.Mobile_Network NetworkID,RequestStatus,Amount,ChannelID,n.NetworkID ntw from Go_Request_Archive left join [dbo].[Go_Network_Prefix] n on n.NetworkID = Go_Request_Archive.NetworkID  where convert(date,Go_Request_Archive) between  convert(date,@st)  and  convert(date,@en) ) "
                           + "  select NetworkID, (Isnull([0],0)+ Isnull([1],0)+ Isnull([2],0)+Isnull([89],0)) Total, Isnull([0],0) Pending ,Isnull([1],0) Completed ,Isnull([2],0) Rejected ,Isnull([89],0) TimedOut from BaseQuery  "
                           + " PIVOT (sum(Amount) for RequestStatus in ([0],[1],[2],[89])) as pvt where channelID = 1  order by ntw asc'  "

                           + "  set @q3 = ' With BaseQuery AS ( Select n.Mobile_Network NetworkID,RequestStatus,Amount,ChannelID,n.NetworkID ntw from Go_Request_Archive left join [dbo].[Go_Network_Prefix] n on n.NetworkID = Go_Request_Archive.NetworkID  where convert(date,Go_Request_Archive) between  convert(date,@st)  and  convert(date,@en) "
                           + " ) select NetworkID, ([0]+[1]+[2]+[89])  Total,[0] Pending ,[1] Completed ,[2] Rejected ,[89] TimedOut from BaseQuery PIVOT (count(Amount) for RequestStatus in ([0],[1],[2],[89])) as pvt where channelID = 2  order by ntw asc' "

                           + "  set @q4 = ' With BaseQuery AS (Select n.Mobile_Network NetworkID,RequestStatus,Amount,ChannelID,n.NetworkID ntw from Go_Request_Archive left join [dbo].[Go_Network_Prefix] n on n.NetworkID = Go_Request_Archive.NetworkID   where convert(date,Go_Request_Archive) between  convert(date,@st)  and  convert(date,@en) "
                           + " ) select NetworkID, (Isnull([0],0)+ Isnull([1],0)+ Isnull([2],0)+Isnull([89],0)) Total, Isnull([0],0) Pending ,Isnull([1],0) Completed ,Isnull([2],0) Rejected ,Isnull([89],0) TimedOut from BaseQuery PIVOT (sum(Amount) for RequestStatus in ([0],[1],[2],[89])) as pvt where channelID = 2  order by ntw asc' "

                            + "  set @q5 = ' Select case statusflag when 1 then ''Active'' when 0 then ''Inactive'' end flag,isnull( count(*),0) All_ from [dbo].[Go_Registered_Account]  where convert(date,[DateRegistered]) between  convert(date,@st)  and  convert(date,@en) group by  statusflag' "

                           + "  execute(@q1) execute(@q2)  execute(@q3) execute(@q4) execute(@q5)  "

                           + "  ";

                SqlParameter[] parameters = {
                       new SqlParameter("@st",st )
                      ,new SqlParameter("@en",en )
                    };

                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql);
            }
            catch  
            {
                dt = null;
            }
            return dt;
        }

        public static DataTable getBlackList(string mobile)
        {
            if (!string.IsNullOrEmpty(mobile))
            {
                Common.formatMobile(ref mobile);
            }
            DataTable dt;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@MOBILE",mobile )
                    };
                string sql = " SELECT DISTINCT [MOBILE],[NUBAN],[DATEDEACTIVATED] DATEADDED,1 Channel FROM [DBO].[GO_REGISTERED_ACCOUNT] WHERE STATUSFLAG = 0 AND ([MOBILE] = @MOBILE OR @MOBILE = ' ') "
                           + " UNION ALL    "
                           + " SELECT DISTINCT [MISDN] MISDN ,[ACCOUNT] NUBAN,[DATEADDED] DATEADDED,2 Channel  FROM [DBO].[GO_BLACKLIST] WHERE  ([MISDN] = @MOBILE OR @MOBILE = ' ') ";
                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
            }
            catch  
            {
                dt = null;
            }
            return dt;
        }

        public static DataTable getRegiteredAccount(string mobile, string StatusFlag)
        {
            DataTable dt;
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@MOBILE",mobile )
                       ,new SqlParameter("@StatusFlag",StatusFlag)
                    };
                string sql = " SELECT Mobile,NUBAN,DateRegistered, case StatusFlag when 1 then 'Active' when 0 then 'Inactive' else 'N/A' end StatusFlag  FROM [dbo].[Go_Registered_Account] WHERE  (([MOBILE] = @MOBILE ) OR ([NUBAN] = @MOBILE) OR ( @MOBILE = '') OR ( @MOBILE IS NULL)) AND (StatusFlag = @StatusFlag OR @StatusFlag = -1)  ";

                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
            }
            catch  
            {
                dt = null;
            }
            return dt;
        }

        public static DataTable getBlackListHistory(string mobile)
        {
            if (!string.IsNullOrEmpty(mobile))
            {
                Common.formatMobile(ref mobile);
            }
            DataTable dt;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@MOBILE",mobile )
                    };
                string sql = " SELECT [MISDN],[ACCOUNT],[StatusType],[Comment] ,[DATEADDED] ,AddedBy FROM [dbo].[Go_BlackList_History] WHERE ([MISDN] = @MOBILE  OR ACCOUNT = @MOBILE OR @MOBILE = '') ";

                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
            }
            catch  
            {
                dt = null;
            }
            return dt;
        }

        public static DataSet getBlackListHistoryAll(string mobile)
        {
            if (!string.IsNullOrEmpty(mobile))
            {
                Common.formatMobile(ref mobile);
            }
            DataSet dt;
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@MOBILE",mobile )
                    };

                string sql = " SELECT DISTINCT [MOBILE],[NUBAN],[DATEDEACTIVATED] DATEADDED,1 Channel FROM [DBO].[GO_REGISTERED_ACCOUNT] WHERE STATUSFLAG = 0 AND ([MOBILE] = @MOBILE OR @MOBILE = '') "
                           + " UNION ALL    "
                           + " SELECT DISTINCT [MISDN] MISDN ,[ACCOUNT] NUBAN,[DATEADDED] DATEADDED,2 Channel  FROM [DBO].[GO_BLACKLIST] WHERE  ([MISDN] = @MOBILE OR @MOBILE = '') "
                           + " SELECT [MISDN],[ACCOUNT],[StatusType],[Comment] ,[DATEADDED] ,AddedBy FROM [dbo].[Go_BlackList_History] WHERE ([MISDN] = @MOBILE  OR ACCOUNT = @MOBILE OR @MOBILE = '') Order by DATEADDED desc ";

                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                dt = null;
            }
            return dt;
        }

        #endregion

        #region Process

        public static void SaveLog(AuditLog a)
        {
            try
            {
                SqlParameter[] parameters = {
                       new SqlParameter("@action",a.action )
                      ,new SqlParameter("@activity",a.activity )
                      ,new SqlParameter("@doneby",a.doneby )
                      ,new SqlParameter("@ip",a.ip )
                      ,new SqlParameter("@sess",a.sess)
                    };

                string sql = "INSERT INTO AUDIT (action,activity,doneby,ip,sessionId,dateadded) VALUES (@action,@activity,@doneby,@ip,@sess,getdate()) select @@rowcount";
                SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");

            }
        }

        public static int UpdateAdminProfile(int ID, int RoleID, int Status)
        {
            int retVal = 0;
            try
            {
                SqlParameter[] parameters = {
                      new SqlParameter("@ID",ID )
                     ,new SqlParameter("@RoleID", RoleID)
                     ,new SqlParameter("@Status", Status) 
                    };

                string sql = "UPDATE [dbo].[Go_UsersMgt] SET [RoleID] = @roleID,[Status] = @status WHERE [ID] =  @ID select @@ROWCOUNT ";
                retVal = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                retVal = -99;
            }
            return retVal;
        }

        public static int InsertAdminProfile(string username, string Fullname, string StaffID, string RoleID, string CreatedBy, string Status, string Email, string Deptname, string Unit)
        {
            int retVal = 0;
            try
            {
                SqlParameter[] parameters = {
                      new SqlParameter("@username",username )
                     ,new SqlParameter("@Fullname", Fullname)
                     ,new SqlParameter("@StaffID", StaffID)
                     ,new SqlParameter("@RoleID", RoleID)
                     ,new SqlParameter("@CreatedBy", CreatedBy)
                     ,new SqlParameter("@Status", Status)
                     ,new SqlParameter("@Email", Email)
                     ,new SqlParameter("@Deptname", Deptname)
                     ,new SqlParameter("@Unit", Unit)
                    };

                string sql = " IF NOT EXISTS (SELECT 1 FROM [Go_UsersMgt] WHERE [StaffID] = @StaffID ) BEGIN "
                           + " INSERT INTO [Go_UsersMgt] ([username],[Fullname],[StaffID],[RoleID],[DateCreated],[CreatedBy],[Status],[Email],[Deptname],[Unit]) "
                           + " VALUES (@username,@Fullname,@StaffID,@RoleID,GetDate(),@CreatedBy,@Status,@Email,@Deptname,@Unit) SELECT @@ROWCOUNT END ELSE BEGIN SELECT -1 END ";
                retVal = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                retVal = -99;
            }
            return retVal;
        }

        public static retObject getAdminInfoByUsername(string username)
        {
            retObject retInfo = new retObject();
            try
            {
                SqlParameter[] parameters = {
                      new SqlParameter("@username",username )
                    };
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, "Select * from Go_UsersMgt where username = @username", parameters).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count == 1)
                    {
                        userInfo usf = new userInfo();
                        usf.ID = Convert.ToInt32(dt.Rows[0]["ID"]);
                        usf.RoleID = Convert.ToInt32(dt.Rows[0]["RoleID"]);
                        usf.status = Convert.ToInt32(dt.Rows[0]["status"]);
                        usf.userName = dt.Rows[0]["userName"].ToString();
                        usf.fullname = dt.Rows[0]["fullname"].ToString();
                        usf.StaffID = dt.Rows[0]["StaffID"].ToString();
                        retInfo.code = 1;
                        retInfo.objVal = usf;
                    }
                    else //irregular entries
                    {
                        retInfo.code = -1;
                    }
                }
                else //when no value is return
                {
                    retInfo.code = 0;
                }
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                retInfo.code = -99;
            }
            return retInfo;
        }

        public static DataTable getAdminInfo()
        {
            DataTable dt;
            try
            {
                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, "Select Case status when 0 then 'Inactive' when 1 then 'Active' end Status_,*, case RoleID when 1 then 'Application Manager' when 2 then 'Go-Pinless Admin' end Role_ from Go_UsersMgt", null).Tables[0];
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                dt = null;
            }
            return dt;
        }

        public static DataTable getAdminInfoByID(int ID)
        {
            DataTable dt;
            try
            {
                SqlParameter[] parameters = {
                      new SqlParameter("@ID",ID )
                    };
                dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, "select * from [dbo].[Go_UsersMgt] where ID = @ID", parameters).Tables[0];
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                dt = null;
            }
            return dt;
        }

        public static int UpdateConfig(decimal minPtxn, decimal maxPtxn, decimal maxPday, string code)
        {
            int retVal = 0;
            try
            {
                SqlParameter[] parameters = {
                      new SqlParameter("@var1",minPtxn )
                     ,new SqlParameter("@var2", maxPtxn)
                     ,new SqlParameter("@var3", maxPday) 
                     ,new SqlParameter("@Code",code)
                    };

                string sql = "UPDATE [dbo].[tbl_USSD_Accounts] SET [var1] = @var1,[var2] = @var2,[var3] = @var3 WHERE Code =  @Code select @@ROWCOUNT ";
                retVal = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                retVal = -99;
            }
            return retVal;
        }

        public static DataTable FetchConfig(string code)
        {
            DataTable retVal = null;
            try
            {
                SqlParameter[] parameters = {
                      new SqlParameter("@Code",code)
                    };

                string sql = "Select var1,var2,var3 from  [dbo].[tbl_USSD_Accounts] WHERE Code =  @Code ";
                retVal = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                retVal = null;
            }
            return retVal;
        }

        public static int enListBlackList(BlackList b, BlackListHistory h, int channel)
        {
            int retVal = 0;
            try
            {
                SqlParameter[] parameters = {
                      new SqlParameter("@ACCOUNT",b.Account)
                     ,new SqlParameter("@ADDEDBY", b.AddedBy)
                     ,new SqlParameter("@MISDN", b.Misdn) 
                     ,new SqlParameter("@COMMENT",h.Comment)
                     ,new SqlParameter("@STATUSTYPE",h.StatusType )
                    };

                string sql = "";

                if (channel == 1)
                {
                    sql = " IF NOT EXISTS (SELECT 1 FROM [DBO].[GO_BLACKLIST] WHERE [MISDN] =@MISDN AND [ACCOUNT] = @ACCOUNT ) BEGIN BEGIN TRANSACTION BEGIN TRY  "
                          + " UPDATE [dbo].[Go_Registered_Account] SET [StatusFlag] = 0 ,[DateDeactivated] = getdate() WHERE [Mobile] = @MISDN "
                          + " INSERT INTO [DBO].[GO_BLACKLIST_HISTORY] ( [MISDN],[ACCOUNT],[STATUSTYPE],[ADDEDBY],[COMMENT],[DATEADDED]  ) "
                          + " VALUES ( @MISDN,@ACCOUNT,@STATUSTYPE,@ADDEDBY,@COMMENT,GETDATE() ) "
                          + " COMMIT  SELECT 100 END TRY  BEGIN CATCH ROLLBACK SELECT -1 END CATCH END ELSE BEGIN SELECT -2 END ";
                }
                else
                {
                    sql = " IF NOT EXISTS (SELECT 1 FROM [DBO].[GO_BLACKLIST] WHERE [MISDN] =@MISDN AND [ACCOUNT] = @ACCOUNT ) BEGIN BEGIN TRANSACTION BEGIN TRY  "
                        + " INSERT INTO [DBO].[GO_BLACKLIST] ([MISDN],[ACCOUNT],[ADDEDBY],[DATEADDED]) VALUES (@MISDN,@ACCOUNT,@ADDEDBY,GETDATE() ) "
                        + " INSERT INTO [DBO].[GO_BLACKLIST_HISTORY] ( [MISDN],[ACCOUNT],[STATUSTYPE],[ADDEDBY],[COMMENT],[DATEADDED]  ) "
                        + " VALUES ( @MISDN,@ACCOUNT,@STATUSTYPE,@ADDEDBY,@COMMENT,GETDATE() ) "
                        + " COMMIT  SELECT 100 END TRY  BEGIN CATCH ROLLBACK SELECT -1 END CATCH END ELSE BEGIN SELECT -2 END ";
                }

                retVal = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                retVal = -99;
            }
            return retVal;
        }

        public static int deListBlackList(BlackList b, BlackListHistory h, int channel)
        {
            int retVal = 0;
            try
            {
                SqlParameter[] parameters = {
                      new SqlParameter("@ACCOUNT",b.Account)
                     ,new SqlParameter("@ADDEDBY", b.AddedBy)
                     ,new SqlParameter("@MISDN", b.Misdn) 
                     ,new SqlParameter("@COMMENT",h.Comment)
                     ,new SqlParameter("@STATUSTYPE",h.StatusType )
                      ,new SqlParameter("@StatusFlag","1" )
                    };
                string sql = "";

                if (channel == 1)
                {
                    sql = " Begin  BEGIN TRANSACTION  BEGIN TRY  "
                        + "   UPDATE [dbo].[Go_Registered_Account] SET [StatusFlag] = @StatusFlag WHERE [Mobile] = @MISDN "
                        + "   INSERT INTO [DBO].[GO_BLACKLIST_HISTORY] ( [MISDN],[ACCOUNT],[STATUSTYPE],[ADDEDBY],[COMMENT],[DATEADDED]  )  "
                        + "   VALUES (@MISDN,@ACCOUNT,@STATUSTYPE,@ADDEDBY,@COMMENT,GETDATE() ) SELECT 100"
                        + "   COMMIT END TRY BEGIN CATCH ROLLBACK SELECT -1 END CATCH end ";
                }

                else
                {
                    sql = " begin  BEGIN TRANSACTION  BEGIN TRY  "
                        + "   DELETE FROM [GO_BLACKLIST] WHERE [MISDN] = @MISDN AND [ACCOUNT] = @ACCOUNT "
                        + "   INSERT INTO [DBO].[GO_BLACKLIST_HISTORY] ( [MISDN],[ACCOUNT],[STATUSTYPE],[ADDEDBY],[COMMENT],[DATEADDED]  )  "
                        + "   VALUES (@MISDN,@ACCOUNT,@STATUSTYPE,@ADDEDBY,@COMMENT,GETDATE() ) SELECT 100"
                        + "   COMMIT END TRY BEGIN CATCH ROLLBACK SELECT -1 END CATCH end";
                }


                retVal = (int)SqlHelper.ExecuteScalar(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters);
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                retVal = -99;
            }
            return retVal;
        }

        #endregion
    }

    class Generic_db
    {
        public static DataTable getRequestByStatus(int RequestStatus)
        {
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@RequestStatus",RequestStatus)
                    };
                string sql = " select * from [dbo].[Go_Request] where RequestStatus  = @RequestStatus Order by RequestDate asc ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch  (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getRequestByChannelID(int RequestStatus, int channelID)
        {
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@RequestStatus",RequestStatus)
                       ,new SqlParameter("@ChannelID",channelID)
                    };
                string sql = " select * from [dbo].[Go_Request] where RequestStatus  = @RequestStatus AND ChannelID = @ChannelID Order by RequestDate asc ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getRequestByNetwork(int RequestStatus, int NetworkID)
        {
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@RequestStatus",RequestStatus)
                       ,new SqlParameter("@NetworkID",NetworkID)
                    };
                string sql = " select * from [dbo].[Go_Request] where RequestStatus  = @RequestStatus AND NetworkID = @NetworkID Order by RequestDate asc ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getRequestByNotNetwork(int RequestStatus, int NetworkID)
        {
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@RequestStatus",RequestStatus)
                       ,new SqlParameter("@NetworkID",NetworkID)
                    };
                string sql = " select * from [dbo].[Go_Request] where RequestStatus  = @RequestStatus AND NetworkID <> @NetworkID Order by RequestDate asc ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }

        public static DataTable getRequestByNetworkID(int ChannelID, int NetworkID)
        {
            try
            {
                SqlParameter[] parameters = {
                        new SqlParameter("@ChannelID",ChannelID)
                       ,new SqlParameter("@NetworkID",NetworkID)
                    };
                string sql = " select * from [dbo].[Go_Request] where ChannelID  = @ChannelID AND NetworkID = @NetworkID Order by RequestDate asc ";
                DataTable dt = SqlHelper.ExecuteDataset(AppConnection.GetAppConnection(), CommandType.Text, sql, parameters).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                ApplicationLog reslg = new ApplicationLog(ex, "errorlog_");
                return null;
            }
        }
    }
}