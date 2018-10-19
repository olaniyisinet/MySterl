using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Threading;
using System.Data;
using System.Net;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using Sterling.BaseLIB.Utility;

namespace NIPHandleRequeryIMAL
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
               
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'NIPHandleRequeryIMAL.exe'";
                //ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                //ManagementObjectCollection processList = searcher.Get();
                //cnt = processList.Count;
                cnt = 0;
                if (cnt > 1)
                {
                    return;
                }
                else
                {
                    while (true)
                    {
                        DataSet ds = new DataSet();
                        Console.WriteLine("");
                        Console.WriteLine("Processing new transactions [staggingstatus=101]....");

                        ds = getTransactions();
                        //transferDS(ds);
                        QueryTrnxThreaded(ds);
                        Console.WriteLine("");
                        Console.WriteLine("Process waiting Time....");
                        Thread.Sleep(10000);
                        Console.WriteLine("");
                        Console.WriteLine("Process continued ....");
                    }
                }
            }
            catch (Exception ex)
            {
                new Errorlog(ex);
                return;
            }
        }

        static DataSet getTransactions()
        {
            string sqlx = @"update tbl_WStrans set staggingstatus = 101 where staggingstatus = 0 
            and DATEDIFF(mi, inputdate, GETDATE()) >=2  and inwardtype=2 and Responsecode='00' and staggingStatus='0'";            
            Connect cnx = new Connect(sqlx, true);
            cnx.query();

            //get transactions that has stayed up to 10mins for today
            string sql = "select * from  tbl_WStrans where Approvevalue=1 " +
            " and staggingStatus = 101 and sessionid is not null and " +
            " TransProcessed=0 and CONVERT(varchar(50),inputdate,112) = CONVERT(varchar(50),GETDATE(),112) " +
            " and  inwardtype =2 and Responsecode='00' and requery is null order by inputdate asc ";

            Connect cn = new Connect(sql, true);
            return cn.query("recs");
        }

        static void QueryTrnxThreaded(DataSet ds)
        {
            //check if the dataset has records, if not return
            int cnt = 0;
            try
            {
                cnt = ds.Tables[0].Rows.Count;
            }
            catch
            {

            }
            if (cnt <= 0) return;

            //for every datarow
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Thread r = new Thread(new ParameterizedThreadStart(RequeryTrnx));
                r.Start(dr);
                Thread.Sleep(500);
                //RequeryTrnx(dr);
            }
        }

        public static void RequeryTrnx(object _dr)
        {
           
            var sql = "";
            DataRow dr = (DataRow)_dr;
            Gadget g = new Gadget();
            Trnx tt = new Trnx();
            MyEncryDecr m = new MyEncryDecr();
            string rsp = String.Empty;
            Transaction trx = new Transaction();
            imal.NIBankingClientService im = new imal.NIBankingClientService();

            string bra_code = "";
            string cus_num = "";
            string cur_code = "";
            string led_code = "";
            string sub_acct_code = "";
            string rsp1 = String.Empty;

            tt.setTrnx(dr);

            if (tt.staggingStatus != 101) return;

            UpdateStaggingStatus(tt.sessionid, 999);


            bool adviceSent = CheckAdvice(tt.sessionid);
            if (adviceSent) return;


            string resp = CallNibss(tt.sessionid, tt.channelCode.ToString(), tt.senderbankcode);
            if (resp == "??") //timed out
            {
                UpdateStaggingStatus(tt.sessionid, 101);
                return;
            }

            if (resp != "00")
            {
                UpdateStaggingStatusRequery(tt.sessionid, 0, resp);
                return;
            }

            if (resp == "00")
            {
                UpdateStaggingStatusRequery(tt.sessionid, 9, resp);

                tt.SessionID = m.Encrypt(tt.SessionID, "1239879000");
                tt.OriginatorAccountName = m.Encrypt(tt.OriginatorAccountName, "1239879000");
                tt.Amount = m.Encrypt(tt.Amount, "1239879000");
                tt.PaymentReference = m.Encrypt(tt.PaymentReference, "1239879000");
                tt.Narration = m.Encrypt(tt.Narration, "1239879000");
                tt.BeneficiaryAccountNumber = m.Encrypt(tt.BeneficiaryAccountNumber, "1239879000");

                //rsp = im.ftSingleCreditRequest(tt.SessionID, tt.OriginatorAccountName, tt.BeneficiaryAccountNumber, tt.Amount, tt.PaymentReference, tt.Narration, "XXX");
                //rsp = m.Decrypt(rsp, "1239879000");
                //new Errorlog("Response gotten for IMAL FundTransfer for account " + tt.SessionID + " " + tt.BeneficiaryAccountNumber + " is " + rsp);
                //string[] bits = rsp.Split(':');

                string url = ConfigurationSettings.AppSettings["ImalCoreBaseURI"].ToString();
                var client = new RestClient(url + "api/Imal/NIPInwardFT");
                var requests = new RestRequest(Method.POST);
                requests.AddHeader("ContentType", "application/json");
                requests.RequestFormat = DataFormat.Json;
                requests.AddBody(new
                {
                    SessionID = tt.SessionID,
                    OriginatorAccountName = tt.OriginatorAccountName,
                    BeneficiaryAccountNumber = tt.BeneficiaryAccountNumber,
                    Amount = tt.Amount,
                    PaymentReference = tt.PaymentReference,
                    Narration = tt.Narration
                });
                var ss = client.Execute(requests);
                rsp = JsonConvert.DeserializeObject<dynamic>(ss.Content);

                rsp = m.Decrypt(rsp, "1239879000");
                new ErrorLog("Response gotten for IMAL FundTransfer for account " + tt.SessionID + " " + tt.BeneficiaryAccountNumber + " is " + rsp);
                string[] bits = rsp.Split(':');

                
                tt.SessionID = m.Decrypt(tt.SessionID, "1239879000");
                tt.OriginatorAccountName = m.Decrypt(tt.OriginatorAccountName, "1239879000");
                tt.BeneficiaryAccountNumber = m.Decrypt(tt.BeneficiaryAccountNumber, "1239879000");
                tt.Amount = m.Decrypt(tt.Amount, "1239879000");
                tt.PaymentReference = m.Decrypt(tt.PaymentReference, "1239879000");
                tt.Narration = m.Decrypt(tt.Narration, "1239879000");

                if (bits[1] == "0" || bits[1] == "00")
                {
                    UpdateStaggingStatusRequery(tt.sessionid, 1, resp);
                }
                else
                {
                    //19 meaning the processing was not successful at the imal.  This needs to be investigated
                    UpdateStaggingStatusRequery(tt.sessionid, 19, resp);
                }
                //update the imal record
                updateImalTransbySID(bits[0], bits[1], bits[2]);
            }
        }

        private static void UpdateStaggingStatus(string sessionid, int staggingstatus)
        {
            string sql2 = "update tbl_WStrans set staggingStatus =@s, switchdate = getdate() where sessionid=@sid and inwardtype=2";
            Connect c2 = new Connect(sql2, true);
            c2.addparam("@s", staggingstatus);
            c2.addparam("@sid", sessionid);
            c2.query();
        }

        public static void updateImalTransbySID(string sid, string rsp, string trx_code)
        {
            Gadget g = new Gadget();
            string sql = "";
            sql = "update tbl_wstrans set Responsecode=@rsp, ResponseMsg=@rmg, Prin_Rsp=@prin, Approvevalue=1 where sessionid=@sid and inwardtype=2";
            Connect c = new Connect(sql, true);
            c.addparam("@sid", sid);
            c.addparam("@rsp", rsp);
            c.addparam("@prin", trx_code);
            c.addparam("@rmg", g.responseCodes(rsp));
            c.query();
        }

        private static void UpdateStaggingStatusRequery(string sessionid, int staggingstatus, string requerystatus)
        {
            string sql3 = "update tbl_WStrans set staggingStatus =@s,Requery = @Requery  where sessionid=@sid and inwardtype=2";
            Connect c3 = new Connect(sql3, true);
            c3.addparam("@s", staggingstatus);
            c3.addparam("@Requery", requerystatus);
            c3.addparam("@sid", sessionid);
            c3.query();
        }

        private static void UpdateStaggingStatusRequery2(int staggingstatus, string requerystatus)
        {
            string sql3 = @"update tbl_WStrans set staggingStatus =@s,Requery = @Requery  where 
                          sessionid in ('000016180530075516000501043552','000013180530074742000119919327','000014180530081725272156182167','000016180530080025000501048259','000016180530081655000501063736','000013180530082550000119931519','000013180530082339000119930776','000016180530083302000501082326','000013180530083423000119934969','000004180530090537410381104105','000013180530085844000119945735','000016180530090047000501117515','000013180530091808000119956998','000013180530092224000119959485','000016180530092649000501155237','000013180530093356000119965314','000013180530091253000119952573','000004180530094102850194354482','000004180530095226654324421478','000003180530094749000096441505','000005180530095505003080426088','000014180530095639250892131433','000003180530095219000096442982','000012180530100315000040888601') and inwardtype=2";
            Connect c3 = new Connect(sql3, true);
            c3.addparam("@s", staggingstatus);
            c3.addparam("@Requery", requerystatus);
            c3.query();
        }
        private static string CallNibss(string sessionid, string channelcode, string senderbankcode)
        {
            NIPRequery.Requery ws = new NIPRequery.Requery();
            //NetworkCredential nc = new NetworkCredential("spservice", "Kinder$$098", "sterlingbank");
            //ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            //WebProxy pr = new WebProxy("10.0.0.120", 80);
            //ws.Proxy = pr;
            //ws.Proxy.Credentials = nc;
            string resp = "";
            try
            {
                resp = ws.requery(sessionid, channelcode, senderbankcode);
                if (resp.Length != 2)
                {
                    resp = "??";
                }
            }
            catch (Exception ex)
            {
                resp = "??";
            }
            return resp;
        }

        private static bool CheckAdvice(string sessionid)
        {
            string sql = "select * from tbl_WStrans where FTadvice = 1 and sessionid=@sid";
            Connect c = new Connect(sql, true);
            c.addparam("@sid", sessionid);
            DataSet ds3 = new DataSet();
            ds3 = c.query("rec");
            if (c.num_rows == 0) return false;

            if (c.num_rows < 0)
            {
                UpdateStaggingStatus(sessionid, 0);
                return true;
            }
            //advice came in so transaction will not be treated
            //new Errorlog("Unable to process this transaction because the transaction has Reversal advice from NIBSS " + tt.sessionid);

            string sql1 = "";
            sql1 = "update tbl_WStrans set staggingStatus =@s,TransProcessed=2,TransProcessDate=getdate() where sessionid=@sid and inwardtype=2";
            Connect c1 = new Connect(sql1, true);
            c1.addparam("@s", 1);
            c1.addparam("@sid", sessionid);
            c1.query();
            return true;
        }
    }
}
