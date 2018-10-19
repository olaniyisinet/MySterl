using NIB_EChannels.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NIB_EChannels.Controllers
{
    public class NIPBranchController : Controller
    {
        // GET: NIPBranch
        public ActionResult Index()
        {
            return View();
        }

         // POST: NIPBranch/Create
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            string sdt = ""; string edt = "";

            try
            {
             //   bool checkdate = Helper.checkDates(Convert.ToDateTime(collection["startdate"]), Convert.ToDateTime(collection["enddate"]));

                //if (checkdate)
                //{
                //	ViewBag.Message = "Start Date can not be greater than End Date";
                //}
                //else 
                if (Convert.ToDateTime(collection["enddate"]) > DateTime.Today)
                {

                    ViewBag.Message = "End Date can not be greater than today";
                }
                else
                {
                    sdt = String.Format("{0:yyyy-MM-dd 00:00:00}", Convert.ToDateTime(collection["startdate"]));
                    edt = String.Format("{0:yyyy-MM-dd 23:59:59}", Convert.ToDateTime(collection["enddate"]));
                }
            }
            catch
            {
                ViewBag.Message = "Kindly select a date range";
                return View();
            }

            try
            {
                String constring = ConfigurationManager.ConnectionStrings["dbConn6"].ConnectionString;
                SqlConnection con = new SqlConnection(constring);
                string query = " SELECT * FROM dbo.tbl_nibssmobile where CONVERT(VARCHAR(25), dateadded, 120) between  '" + sdt + "' and '" + edt + "' and channelcode=1 and nuban like '050%' order by refid desc";
                DataTable dt = new DataTable();
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
                con.Close();
                IList<NIP_Branch> model = new List<NIP_Branch>();
               
                int count = dt.Rows.Count;

                if (count < 1)
                {
                    ViewBag.Massage = "No NIP at Branch from " + collection["startdate"] + " to " + collection["enddate"];
                    return View();
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        model.Add(new NIP_Branch()
                        {
                            Refid = dt.Rows[i]["Refid"].ToString(),
                            sessionid = dt.Rows[i]["sessionid"].ToString(),
                            sessionidNE = dt.Rows[i]["sessionidNE"].ToString(),
                            transactioncode = dt.Rows[i]["transactioncode"].ToString(),
                            channelCode = dt.Rows[i]["channelCode"].ToString(),
                            BatchNumber = dt.Rows[i]["BatchNumber"].ToString(),
                            paymentRef = dt.Rows[i]["paymentRef"].ToString(),
                            amount = dt.Rows[i]["amt"].ToString(),
                            feecharge = dt.Rows[i]["feecharge"].ToString(),
                            vat = dt.Rows[i]["vat"].ToString(),
                            AccountName = dt.Rows[i]["AccountName"].ToString(),
                            AccountNumber =dt.Rows[i]["AccountNumber"].ToString(),
                            originatorname = dt.Rows[i]["originatorname"].ToString(),
                            bra_code = dt.Rows[i]["bra_code"].ToString(),
                            cus_num = dt.Rows[i]["cus_num"].ToString(),
                            cur_code = dt.Rows[i]["cur_code"].ToString(),
                            led_code = dt.Rows[i]["led_code"].ToString(),
                            sub_acct_code = dt.Rows[i]["sub_acct_code"].ToString(),
                            accname = dt.Rows[i]["accname"].ToString(),
                            response = dt.Rows[i]["response"].ToString(),
                            dateadded = dt.Rows[i]["dateadded"].ToString(),
                            requeryStatus = dt.Rows[i]["requeryStatus"].ToString(),
                            nameResponse = dt.Rows[i]["nameResponse"].ToString(),
                            lastupdate = dt.Rows[i]["lastupdate"].ToString(),
                            reversalstatus = dt.Rows[i]["reversalstatus"].ToString(),
                            vTellerMsg = dt.Rows[i]["vTellerMsg"].ToString(),
                            oddResponse = dt.Rows[i]["oddResponse"].ToString(),
                            FTadvice = dt.Rows[i]["FTadvice"].ToString(),
                            FTadviceDate = dt.Rows[i]["FTadviceDate"].ToString(),
                            mailSent = dt.Rows[i]["mailSent"].ToString(),
                            outwardTrnsType = dt.Rows[i]["outwardTrnsType"].ToString(),
                            appsTransType = dt.Rows[i]["appsTransType"].ToString(),
                            nuban = dt.Rows[i]["nuban"].ToString(),
                            bankcode = dt.Rows[i]["bankcode"].ToString(),
                            Prin_Rsp = dt.Rows[i]["Prin_Rsp"].ToString(),
                            Fee_Rsp = dt.Rows[i]["Fee_Rsp"].ToString(),
                            Vat_Rsp = dt.Rows[i]["Vat_Rsp"].ToString(),
                            Prin_Rsp1 = dt.Rows[i]["Prin_Rsp1"].ToString(),
                            Fee_Rsp1 = dt.Rows[i]["Fee_Rsp1"].ToString(),
                            Vat_Rsp1 = dt.Rows[i]["Vat_Rsp1"].ToString(),
                            Account_Status = dt.Rows[i]["Account_Status"].ToString(),
                            Restriction = dt.Rows[i]["Restriction"].ToString(),
                        });
                    }
                    ViewBag.Massage = " NIP at Branch from " + collection["startdate"] + " to " + collection["enddate"];
                    return View(model);
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = "Error Occured! - " + e.Message;
                //new ErrorLog(e.Message, e.StackTrace);
                return View();
            }
        }
    }
}
