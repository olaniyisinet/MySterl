using DoubbleAdmin.LDAP;
using DoubbleAdmin.Models;
using Microsoft.AspNet.Identity;
using OneDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace DoubbleAdmin.Controllers
{
    public class LoginController : Controller
    {
        private readonly string pwd = WebConfigurationManager.AppSettings["AdminPwd"];
        private readonly ldapSoapClient userr = new ldapSoapClient();
        private readonly Gadget g = new Gadget();
        // GET: Login
        public TimeSpan Timeout { get; set; }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // GET: Logn
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Login model, string returnUrl)
        {
            Session["Username"] = model.Username;
            if (model.Username == "SuperAdmin")
            {
                if (model.Password == pwd)
                {
                    Mylogger.Info(model.Username + " logged in.");
                    g.updateLastPrompt(model.Username);
                    return RedirectToAction("SuperAmin", "DoubbleAdmin");
                }
                else
                {
                    Mylogger.Info(model.Username + " entered wrong credentials.");
                    ViewBag.message = "Enter Valid Credentials";
                    return View();
                }
            }
            else
            {
                if (userr.login(model.Username, model.Password))
                {
                    if (g.getRole(model.Username).Length != 0 || !String.IsNullOrEmpty(g.getRole(model.Username)))
                    {
                        if (g.getRole(model.Username) == "Super User")
                        {
                            Mylogger.Info(model.Username + " logged in as SuperAmin.");
                            g.updateLastPrompt(model.Username);
                            return RedirectToAction("SuperAmin", "DoubbleAdmin");
                        }
                        else
                        {
                            Mylogger.Info(model.Username + " logged in as DoubbleAdmin.");
                            g.updateLastPrompt(model.Username);
                            return RedirectToAction("DoubbleAdmin", "DoubbleAdmin");
                        }
                    }
                    else
                    {
                        Mylogger.Info(model.Username + " is not allowed on this platform.");
                        ViewBag.message = "You are not allowed on this platform.";
                        return View();
                    }
                }
                else
                {
                    Mylogger.Info(model.Username + " entered wrong credentials.");
                    ViewBag.message = "Enter Valid Credentials";
                    return View();
                }
            }
        }



        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Login", "Login");
        }

        // [Authorize(Roles="HOP teller")]



        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(new string[] { DefaultAuthenticationTypes.ApplicationCookie });
            return RedirectToAction("Login", "Login");
        }
    }
}