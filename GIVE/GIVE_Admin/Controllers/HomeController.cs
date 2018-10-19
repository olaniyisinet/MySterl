using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GIVE_Admin.Helpers;
using GIVE_Admin.Models;
using NIB_EChannels.Helpers;

namespace GIVE_Admin.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Dashboard()
		{
			ViewBag.TotalProjects = Counter.getTotalProjects();
			ViewBag.TotalActiveProjects = Counter.getTotalActiveProjects();
			ViewBag.TotalInActiveProjects = Counter.getTotalInActiveProjects();
			ViewBag.TotalNGOs = Counter.getTotalNGOs();
			return View();
		}
			}
}