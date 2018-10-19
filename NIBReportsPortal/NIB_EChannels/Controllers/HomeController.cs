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
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Transactions()
		{
			if (Session["username"] != null)
			{
				return View();
			}
			else { return View("~/Views/Login/Login.cshtml"); }
		}

		public ActionResult Dashboard()
		{
			if (Session["username"] != null)
			{
				return View();
			}
			else {
				return View("~/Views/Login/Login.cshtml");
			}
		}
			}
}