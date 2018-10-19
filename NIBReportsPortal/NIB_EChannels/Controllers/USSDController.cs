using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NIB_EChannels.Helpers;
using NIB_EChannels.Models;

namespace NIB_EChannels.Controllers
{
    public class USSDController : Controller
    {
        // GET: USSD
        public ActionResult Index()
        {
			if (Session["username"] != null)
			{
				return View();
			}
			else { return View("~/Views/Login/Login.cshtml"); }
		}

		[HttpPost]
		public ActionResult Index(FormCollection collection)
		{
			string sdt = ""; string edt = "";

			try
			{
				bool checkdate = Helper.checkDates(Convert.ToDateTime(collection["startdate"]), Convert.ToDateTime(collection["enddate"]));

				//if (checkdate)
				//{
				//	ViewBag.Message = "Start Date can not be greater than End Date";
				//}
				//else 
                
                if (Convert.ToDateTime(collection["enddate"]) > DateTime.Today){

					ViewBag.Message = "End Date can not be greater than today";
				}
				else {
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
				string fiew = "";
		
				string method = Convert.ToString(collection["method"]);

				string constring = ConfigurationManager.ConnectionStrings["dbConn4"].ConnectionString;
				SqlConnection con = new SqlConnection(constring);
				string query = "SELECT Refid,SessionID,MethodName,Request,ResponseCode,Dateadded,DateProcessed FROM tbl_USSD_Imal_Logs where CONVERT(VARCHAR(25), Dateadded, 120) between '" + sdt + "' and '" + edt + "' and MethodName Like '" + method + "%'";
				DataTable dt = new DataTable();
				con.Open();
				SqlDataAdapter da = new SqlDataAdapter(query, con);
				da.Fill(dt);
				con.Close();
				IList<USSDModel> model = new List<USSDModel>();
				int count = dt.Rows.Count;

				if (count < 1)
				{
					ViewBag.Message = "No USSD Transaction from " + sdt + " to " + edt + " for " + method;
					return View();
				}
				else
				{
					if (method == "Inter-bank Transfer")
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							model.Add(new USSDModel()
							{
								Refid = dt.Rows[i]["Refid"].ToString(),
								SessionID = dt.Rows[i]["SessionID"].ToString(),
								Method = dt.Rows[i]["MethodName"].ToString(),
								//DestinationBankCode = dt.Rows[i]["Request"].ToString(),
								DestinationBankCode = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 0),
								ChannelCode = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 1),
								CustomerShowName = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 2),
								PaymentReference = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 3),
								//PaymentReference = new Splitter().SplitPaymentRef(dt.Rows[i]["Request"].ToString(), "\"paymentReference\":\"", "}"),
								FromAccount = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 4),
								ToAccount = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 5),
								Amount = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 6),
								RequestCode = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 7),
								PrincipalIdentifier = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 8),
								ReferenceCode = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 9),
								BeneficiaryName = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 10),
								NesId = new Splitter().splitInterBRequests(dt.Rows[i]["Request"].ToString(), 11),
								Dateadded = dt.Rows[i]["Dateadded"].ToString(),
								DateProcessed = dt.Rows[i]["DateProcessed"].ToString(),
								ResponseCode = dt.Rows[i]["ResponseCode"].ToString(),
							});
						}
						fiew = "~/Views/USSD/InterBankUSSD.cshtml";
					}
					else
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							model.Add(new USSDModel()
							{
								Refid = dt.Rows[i]["Refid"].ToString(),
								SessionID = dt.Rows[i]["SessionID"].ToString(),
								Method = dt.Rows[i]["MethodName"].ToString(),
								Request = dt.Rows[i]["Request"].ToString(),
								ResponseCode = dt.Rows[i]["ResponseCode"].ToString(),
								Dateadded = dt.Rows[i]["Dateadded"].ToString(),
								DateProcessed = dt.Rows[i]["DateProcessed"].ToString(),
								FromAccount = new Splitter().splitRequests(dt.Rows[i]["Request"].ToString(), 0),
								ToAccount = new Splitter().splitRequests(dt.Rows[i]["Request"].ToString(), 1),
								Amount = new Splitter().splitRequests(dt.Rows[i]["Request"].ToString(), 2),
								RequestCode = new Splitter().splitRequests(dt.Rows[i]["Request"].ToString(), 3),
								ReferenceCode = new Splitter().splitRequests(dt.Rows[i]["Request"].ToString(), 5),
								BeneficiaryName = new Splitter().splitRequests(dt.Rows[i]["Request"].ToString(), 6),
								PaymentReference = new Splitter().SplitPaymentRef(dt.Rows[i]["Request"].ToString(), "\"paymentReference\":\"", "}"),
							});
						}

						fiew = "~/Views/USSD/otherUSSD.cshtml";
					}
					ViewBag.Message = method + " USSD Transaction from " + collection["startdate"] + " to " + collection["enddate"];
					return View(fiew, model);
				}
			}
			catch (Exception e)
			{
				ViewBag.Message = "Error Occured! - " + e.Message;
				return View();
			}
		}
	}
}