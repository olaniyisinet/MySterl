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
    public class InwardController : Controller
    {
        // GET: Inward
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
				String constring = ConfigurationManager.ConnectionStrings["dbConn5"].ConnectionString;
				SqlConnection con = new SqlConnection(constring);
				string query = " SELECT Id,SessionID,RequestTypeId, RequestIn,TimeIn,ResponseJSON,TimeOut,ResponseCode,BeneficiaryAcctNo,Amount,OriginAccount FROM tbl_trnx where CONVERT(VARCHAR(25), TimeIn, 120) between  '" + sdt + "' and '" + edt + "'";
				DataTable dt = new DataTable();
				con.Open();
				SqlDataAdapter da = new SqlDataAdapter(query, con);
				da.Fill(dt);
				con.Close();
				IList<InwardModel> model = new List<InwardModel>();
				MyEncryDecr enc = new MyEncryDecr();
				int count = dt.Rows.Count;

				if (count < 1)
				{
					ViewBag.Massage = "No Inward Transactions from " + collection["startdate"] + " to " + collection["enddate"];
					return View();
				}
				else
				{
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						model.Add(new InwardModel()
						{
							Id = dt.Rows[i]["Id"].ToString(),
							SessionID = dt.Rows[i]["SessionID"].ToString(),
							RequestTypeID = dt.Rows[i]["RequestTypeId"].ToString(),
							RequestIn = dt.Rows[i]["RequestIn"].ToString(),
							TimeIn = dt.Rows[i]["TimeIn"].ToString(),
							ResponseJSON = dt.Rows[i]["ResponseJSON"].ToString(),
							TimeOut = dt.Rows[i]["TimeOut"].ToString(),
							ResponseCode = dt.Rows[i]["ResponseCode"].ToString(),
							ResponseDescription = new Splitter().splitRequests(dt.Rows[i]["ResponseJSON"].ToString(), 5),
							Beneficiary = dt.Rows[i]["BeneficiaryAcctNo"].ToString(),
							Amount = dt.Rows[i]["Amount"].ToString(),
							PaymentReference = new Splitter().splitAndDecrypt(dt.Rows[i]["RequestIn"].ToString(), 4),
							OriginatorAccountName = new Splitter().splitAndDecrypt(dt.Rows[i]["RequestIn"].ToString(), 1),
							Narration = new Splitter().splitAndDecrypt(dt.Rows[i]["RequestIn"].ToString(), 5),
							//IMALTransactionID = dt.Rows[i]["IMALTransactionID"].ToString(),
						});
					}
					ViewBag.Massage = "Inward Transactions from " + collection["startdate"] + " to " + collection["enddate"];
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