using Microsoft.AspNet.Identity;
using OneDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using NIB_EChannels.Models;
using NIB_EChannels.Helpers;

namespace NIB_EChannels.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public TimeSpan Timeout { get; set; }
        [AllowAnonymous]
        public ActionResult Login()
        {
			Session["username"] = "";
			return View();
        }

        // GET: Logn
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(Login model, string returnUrl)
        {
            HttpContext.GetOwinContext().Authentication.SignOut(new string[] { DefaultAuthenticationTypes.ApplicationCookie });

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //bool logged = true; 
            bool logged = OneDirectory.Profile.Authenticate(HttpUtility.HtmlEncode(model.Username), HttpUtility.HtmlEncode(model.Password));

            if (logged)
            {
                string role = string.Empty;
                try
                {
                    var applicationId = Int32.Parse(ConfigurationManager.AppSettings["applicationId"].Trim());
                    AppUser appUser = OneDirectory.Profile.GetAppUserInfoTest(model.Username, applicationId);

                    if (appUser.UserID <= 0)
                    {
                        ModelState.AddModelError("", "You are not allowed on this platform");
						return View(model);
					//	return RedirectToAction("Dashboard", "Home");
					}
                    role = appUser.RoleName ?? "none";
                    Session["username"] = appUser.FullName ?? "none";
                    Session["identity"] = appUser.Bra_code ?? "none";
                    Session["AdName"] = model.Username.ToString();

					//TempData["user"] = Session
						
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, appUser.UserName),
                    new Claim(ClaimTypes.Role,role),
                    new Claim("RoleID",appUser.RoleID.ToString())
                };

                    var claimIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(claimIdentity);

					//if (appUser.RoleName == "Compliance Officer")
					//{
					//    //Mylogger.Info(appUser.FullName + "::Logged in");
					//    return RedirectToAction("Transactions", "Home");
					//}
					//else if (appUser.RoleName == "CAD Officer")
					//{
					//    //Mylogger.Info(appUser.FullName + "::Logged in");
					//    return RedirectToAction("Transactions", "Home");
					//}
					return RedirectToAction("Dashboard", "Home");
				}
                catch (Exception ex)
                {
                    ErrorLog.LogException(ex);
                }

                return RedirectToAction("Login", "Login");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View("~/Views/Login/Login.cshtml", model);
				            }
        }
		
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Dashboard", "Home");
        }

        // [Authorize(Roles="HOP teller")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(new string[] { DefaultAuthenticationTypes.ApplicationCookie });
            return RedirectToAction("Login", "Login");
        }
    }
}