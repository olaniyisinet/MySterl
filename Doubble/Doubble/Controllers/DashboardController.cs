using Doubble.BLL;
using Doubble.BLL.DTOs;
using Doubble.BLL.LoanService;
using Doubble.DAL;
using Doubble.DTOs;
using Doubble.EWS1;
using Doubble.GenerateOtp;
using Doubble.Models;
using Microsoft.AspNet.Identity;
using Sterling.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doubble.Controllers
{
    public class DashboardController : Controller
    {

        Gadget g = new Gadget();
        private readonly LoanServicesSoapClient annuity = new LoanServicesSoapClient();
        private readonly banksSoapClient cusDetails = new banksSoapClient();
        private readonly DoubbleService d = new DoubbleService();
        private readonly DoubbleDalService dal = new DoubbleDalService();
        public TimeSpan Timeout { get; set; }
        public ActionResult LogIn()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogIn(FormCollection fm, Login model)
        {
            if (checkFirstTime(model.BVN, model.Password))
            {
                return RedirectToAction("CahngePwd", "Dashboard", model);
            }
            else
            {
                if (checkAvail(model.BVN, model.Password))
                {
                    model.Password = "";
                    return RedirectToAction("Dashboard", "Dashboard", model);
                }
                else
                {
                    ViewBag.message = "Enter Valid Credentials";
                    return View();
                }

            }

        }


        public ActionResult ForgotPassword(FormCollection fm)
        {
            GenerateOtpSoapClient genOtp = new GenerateOtpSoapClient();
            if (String.IsNullOrEmpty(fm["bvn"]) && String.IsNullOrEmpty(fm["otp"]))
            {
                ViewBag.resp = null;
                return View();
            }
            else if (!String.IsNullOrEmpty(fm["bvn"]))
            {

                DataTable record = d.checkUserExists(fm["bvn"]);

                if (record.Rows.Count < 1)
                {
                    ViewBag.message = "Enter a Valid BVN";
                    return View();
                }
                else
                {
                    try
                    {
                        genOtp.doGenerateOtpAndMailByPhoneNumberWithMailBodyD(record.Rows[0]["MobileNumber"].ToString(), record.Rows[0]["PayInAccount"].ToString(), 1, record.Rows[0]["Email"].ToString(), "e-business@sterlingbankng.com", "Doubble", "Your Doubble code is", "Your Doubble code is");
                        Session["mail"] = record.Rows[0]["Email"].ToString();
                        Session["bvn"] = fm["bvn"];
                        ViewBag.resp = "NotNull";
                        return View();
                    }
                    catch (Exception e)
                    {

                        ViewBag.message = "Contact Administrator";
                        return View();
                    }
                }
            }
            else if (!String.IsNullOrEmpty(fm["otp"]) && !String.IsNullOrEmpty(fm["newPwd"]) && !String.IsNullOrEmpty(fm["confPwd"]))
            {
                DataTable record = d.checkUserExists(Session["bvn"].ToString());
                int k = genOtp.verifyOtp(record.Rows[0]["PayInAccount"].ToString(), fm["otp"], 1);
                if (k == 1)
                {
                    if (fm["newPwd"] == fm["confPwd"])
                    {
                        Login model = new Login();
                        model.BVN = Session["bvn"].ToString();
                        updatePwd(g.Encrypt(fm["newPwd"]), Session["bvn"].ToString());
                        return RedirectToAction("Dashboard", "Dashboard", model);
                    }
                    ViewBag.message = "Your Password Must Match.";
                    return View();
                }
                ViewBag.message = "Your OTP has expired";
                return View();
            }

            return View();
        }

        public ActionResult CahngePwd(Login model)
        {

            return View();
        }

        [HttpPost]
        public ActionResult CahngePwd(FormCollection fm, Login model)
        {

            string bvn = model.BVN;
            string new_pwd = fm["new_pwd"];
            string conf_pwd = fm["conf_pwd"];

            if (new_pwd == conf_pwd)
            {
                updatePwd(g.Encrypt(new_pwd), bvn);
                return RedirectToAction("Dashboard", "Dashboard", model);
            }
            ViewBag.message = "Try Again!";
            return View();
        }

        public ActionResult Profiler(string bvn)
        {
            if (bvn == null || bvn == "")
            {
                bvn = Session["BVN"].ToString();
            }
            DataTable record = d.checkUserExists(bvn);
            ViewBag.name = record.Rows[0]["FirstName"] + " " + record.Rows[0]["LastName"];
            ViewBag.mail = record.Rows[0]["Email"];
            ViewBag.phoneNo = record.Rows[0]["MobileNumber"];
            ViewBag.bvn = record.Rows[0]["BVN"];
            ViewBag.fName = record.Rows[0]["FirstName"];
            return View();
        }

        [HttpGet]
        public ActionResult NewInvestment()
        {
            DataTable record = d.checkPassword(Session["BVN"].ToString());
            ViewBag.fName = record.Rows[0]["FirstName"];

            return View();
        }

        public ActionResult Dashboard(string bvn)
        {

            if (bvn == null || bvn == "")
            {
                bvn = Session["BVN"].ToString();
            }

            else if (bvn != null || bvn != "")
            {
                Session["BVN"] = bvn;
            }
            DataTable record = d.checkPassword(bvn);
            ViewBag.name = record.Rows[0]["FirstName"] + " " + record.Rows[0]["LastName"];
            ViewBag.mail = record.Rows[0]["Email"];
            ViewBag.phoneNo = record.Rows[0]["MobileNumber"];
            ViewBag.bvn = record.Rows[0]["BVN"];
            ViewBag.fName = record.Rows[0]["FirstName"];
            Session["fName"] = record.Rows[0]["FirstName"];

            return View();
        }

        [HttpPost]
        public ActionResult Profiler(FormCollection fm)
        {

            return View();
        }

        public ActionResult InvestList(Login model)
        {
            ViewBag.fName = Session["fName"];
            ViewBag.message = Session["ErrorMessage"];

            DataTable record = d.checkUserExists(Session["BVN"].ToString());
            ViewBag.fName = record.Rows[0]["FirstName"];
            DataTable dt = new DataTable();
            try
            {
                dt = getSterling(Session["BVN"].ToString());
            }
            catch (Exception e)
            {

            }
            DataTable dat = new DataTable();
            try
            {
                dat = getNotSterling(Session["BVN"].ToString());
            }
            catch (Exception e)
            {

            }


            DataTable ds = new DataTable();
            List<Portfolio> list = new List<Portfolio>();
            Portfolio p = new Portfolio();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    p.ArrangementId = dt.Rows[i]["ARRANGEMENT_ID"].ToString();
                    p.InvestmentType = dt.Rows[i]["Category"].ToString();
                    p.PayInAccount = dt.Rows[i]["PayInAccount"].ToString();
                    p.BeneficiaryName = dt.Rows[i]["BeneficiaryName"].ToString();
                    p.BeneficiaryAccount = dt.Rows[i]["BeneficiaryAccount"].ToString();

                    ds = annuity.GetAaDetailSchedule(p.ArrangementId).Tables[0];

                    int cnt = countSettled(p.ArrangementId);


                    try
                    {
                        p.TotalContributions = (Convert.ToDouble(ds.Rows[i]["TotalDue"]) * cnt).ToString();
                        //p.InterestEarned = ds.Rows[cnt - 1]["Interest"].ToString();

                        p.TotalInvestmentValue = ds.Rows[cnt - 1]["Outstanding"].ToString();
                        p.InterestEarned = (Convert.ToDouble(p.TotalInvestmentValue) - Convert.ToDouble(p.TotalContributions)).ToString();
                    }
                    catch (Exception e)
                    {
                        p.InterestEarned = "0";
                        p.TotalInvestmentValue = p.TotalContributions;
                    }

                    list.Add(new Portfolio() { ArrangementId = p.ArrangementId, InterestEarned = p.InterestEarned, InvestmentType = p.InvestmentType, TotalContributions = p.TotalContributions, TotalInvestmentValue = p.TotalInvestmentValue, BeneficiaryAccount = p.BeneficiaryAccount, PayInAccount = p.PayInAccount });


                }
            }


            if (dat.Rows.Count > 0)
            {
                for (int i = 0; i < dat.Rows.Count; i++)
                {
                    p.ArrangementId = dat.Rows[i]["ARRANGEMENT_ID"].ToString();
                    p.InvestmentType = dat.Rows[i]["Category"].ToString();
                    p.PayInAccount = dt.Rows[i]["PayInAccount"].ToString();
                    p.BeneficiaryName = dt.Rows[i]["BeneficiaryName"].ToString();
                    p.BeneficiaryAccount = dt.Rows[i]["BeneficiaryAccount"].ToString();

                    ds = annuity.GetAaDetailSchedule(p.ArrangementId).Tables[0];

                    int cnt = countSettled(p.ArrangementId);


                    try
                    {
                        p.TotalContributions = (Convert.ToDouble(ds.Rows[i]["TotalDue"]) * cnt).ToString();
                        //p.InterestEarned = ds.Rows[cnt - 1]["Interest"].ToString();                        
                        p.TotalInvestmentValue = ds.Rows[cnt - 1]["Outstanding"].ToString();
                        p.InterestEarned = (Convert.ToDouble(p.TotalInvestmentValue) - Convert.ToDouble(p.TotalContributions)).ToString();
                    }
                    catch (Exception e)
                    {
                        p.InterestEarned = "0";
                        p.TotalInvestmentValue = p.TotalContributions;
                    }

                    list.Add(new Portfolio() { ArrangementId = p.ArrangementId, InterestEarned = p.InterestEarned, InvestmentType = p.InvestmentType, TotalContributions = p.TotalContributions, TotalInvestmentValue = p.TotalInvestmentValue, BeneficiaryAccount = p.BeneficiaryAccount, PayInAccount = p.PayInAccount });


                }
            }


            return View(list);
        }

        public ActionResult Beneficiary(string ArrangementId)
        {
            ViewBag.message = Session["ErrorMessage"];
            ViewBag.fName = Session["fName"];
            Portfolio model = new Portfolio();
            DataTable record = new DataTable();

            try
            {
                List<BankAccount> acctNos = d.eligibleDoubbleAccounts(Session["BVN"].ToString());
                SelectList accList = new SelectList(acctNos, "Account", "Account");
                ViewBag.Accounts = accList;
                Session["accounts"] = accList;

                record = d.ArrangementDetails(ArrangementId);

                 

                model.ArrangementId = ArrangementId;
                ViewBag.ArrangementId = model.ArrangementId;

                model.BeneficiaryAccount = record.Rows[0]["BeneficiaryAccount"].ToString();
                ViewBag.BeneficiaryAccount = model.BeneficiaryAccount;
                Session["beneficiaryAcct"] = model.BeneficiaryAccount;

                model.BeneficiaryName = record.Rows[0]["BeneficiaryName"].ToString();
                ViewBag.BeneficiaryName = model.BeneficiaryName;

                model.EffectiveDate = Convert.ToDateTime(record.Rows[0]["EffectiveDate"]);
                ViewBag.EffectiveDate = model.EffectiveDate;

                model.InvestmentType = record.Rows[0]["Category"].ToString();
                ViewBag.InvestmentType = model.InvestmentType;

                model.MaturityDate = Convert.ToDateTime(record.Rows[0]["MaturityDate"]);
                ViewBag.MaturityDate = model.MaturityDate;

                model.PayInAccount = record.Rows[0]["PayInAccount"].ToString();
                ViewBag.PayInAccount = model.PayInAccount;

                model.PayInAmount = record.Rows[0]["PayInAmount"].ToString();
                ViewBag.PayInAmount = model.PayInAmount;

                DataTable ds = annuity.GetAaDetailSchedule(ArrangementId).Tables[0];
                int cnt = countSettled(model.ArrangementId);

                try
                {
                    model.TotalContributions = (Convert.ToDouble(ds.Rows[0]["TotalDue"]) * cnt).ToString();
                    ViewBag.TotalContributions = model.TotalContributions;

                    model.TotalInvestmentValue = ds.Rows[cnt - 1]["Outstanding"].ToString();
                    ViewBag.TotalInvestmentValue = model.TotalInvestmentValue;

                    model.InterestEarned = (Convert.ToDouble(model.TotalInvestmentValue) - Convert.ToDouble(model.TotalContributions)).ToString();
                    ViewBag.InterestEarned = model.InterestEarned;
                }
                catch (Exception e)
                {
                    model.InterestEarned = "0";
                    ViewBag.InterestEarned = model.InterestEarned;

                    model.TotalInvestmentValue = model.TotalContributions;
                    ViewBag.TotalInvestmentValue = model.TotalInvestmentValue;
                }
            }
            catch (Exception e)
            {


            }

            return View(model);
        }


        [HttpPost]
        public ActionResult Beneficiary(Portfolio model, FormCollection fm)
        {
            Mylogger.Info("Change of Beneficiary was submitted for " + model.ArrangementId);
            DataTable dt = new DataTable();
            if (model.BeneficiaryType == "Others")
            {
                model.BeneficiaryAccount = fm["Account"];
                try
                {
                    dt = cusDetails.getAccountFullInfo(model.BeneficiaryAccount).Tables[0];
                    if (dt.Rows.Count < 1)
                    {
                        Session["ErrorMessage"] = "Kindly input a valid Beneficiary Account";
                        Mylogger.Error("Invalid account number " + model.BeneficiaryAccount + " for Arrangement ID " + model.ArrangementId);
                        return RedirectToAction("Beneficiary", model);
                    }
                    model.BeneficiaryName = dt.Rows[0]["CUS_SHO_NAME"].ToString();
                }
                catch (Exception e)
                {
                    Session["ErrorMessage"] = "Kindly Try Again.";
                    
                    Mylogger.Error("Error occurred while getting account details for account number " + model.BeneficiaryAccount + " for Arrangement ID " + model.ArrangementId);
                    return RedirectToAction("Beneficiary", model);
                }
                
                model.BeneficiaryType = "Others";
            }
            else
            {
                
                model.BeneficiaryAccount = fm["Account1"].Substring(0, 10);
                dt = cusDetails.getAccountFullInfo(model.BeneficiaryAccount).Tables[0];
                model.BeneficiaryName = dt.Rows[0]["BeneficiaryName"].ToString();
                model.BeneficiaryType = "Self";
            }
            try
            {

                string resp = cusDetails.UpdateDepositPayoutAccounts(model.ArrangementId, model.EffectiveDate, model.BeneficiaryAccount, model.BeneficiaryAccount, model.BeneficiaryAccount);
                if (!resp.StartsWith("1"))
                {
                    Mylogger.Error("Response " + resp + " was gotten for " + model.ArrangementId + " while changing beneficiary to " + model.BeneficiaryAccount);
                    Session["ErrorMessage"] = "Kindly Try Again.";
                    return RedirectToAction("Beneficiary", model);
                }
                else
                {
                    dal.updateBeneficiary(model.ArrangementId, model.BeneficiaryAccount, model.BeneficiaryType, model.BeneficiaryName);
                    Mylogger.Info("Beneficiary successfully changed for " + model.ArrangementId + " from " + Session["beneficiaryAcct"].ToString() + " to " + model.BeneficiaryAccount);
                    return RedirectToAction("InvestList");
                }


            }
            catch (Exception)
            {
                Session["ErrorMessage"] = "Kindly Try Again.";
                Mylogger.Error("Error occurred while getting account details for account number " + model.BeneficiaryAccount + " for Arrangement ID " + model.ArrangementId);
                return RedirectToAction("Beneficiary", model);
            }

        }

        public ActionResult PreTerminate(string ArrangementId)
        {

            try
            {
                dal.Terminate(ArrangementId);
                Mylogger.Info("Arrangement ID " + ArrangementId + " was submitted for pre-termination");
            }
            catch (Exception e)
            {
                Session["ErrorMessage"] = "Kindly Try Again.";
                Mylogger.Error("Error " + e + " was gotten for " + ArrangementId + " while updating table for pr-termination");
                return RedirectToAction("InvestList");
            }

            return RedirectToAction("InvestList");
        }

        static int countSettled(string arrangementId)
        {
            LoanServicesSoapClient annuity = new LoanServicesSoapClient();
            int count = 0;
            DataTable dt = annuity.GetAaBillsDetails(arrangementId).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["Status"].ToString() == "Settled")
                {
                    count++;
                }
            }
            return count;
        }

        static DataTable getSterling(string bvn)
        {
            string sql = "select * from ForSterlings where BVN = @bvn and ARRANGEMENT_ID is not NULL and Terminate is NULL";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];
        }

        static DataTable getNotSterling(string bvn)
        {
            string sql = "select * from ForNonSterlings where BVN = @bvn and ARRANGEMENT_ID is not NULL and Terminate is NULL";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@bvn", bvn);
            return c.Select("rec").Tables[0];
        }

        static void updatePwd(string pwd, string bvn)
        {
            try
            {
                string sql = "update ForSterlings set Password=@pwd,changedPassword='1' where BVN=@bvn";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");

                c.SetSQL(sql);
                c.AddParam("@pwd", pwd);
                c.AddParam("@bvn", bvn);
                c.Select("rec");
            }
            catch (Exception e)
            {

            }
            try
            {
                string sql1 = "update ForNonSterlings set Password=@pwd,changedPassword='1' where BVN=@bvn";
                Sterling.MSSQL.Connect c1 = new Sterling.MSSQL.Connect("msconn");
                c1.SetSQL(sql1);
                c1.AddParam("@pwd", pwd);
                c1.AddParam("@bvn", bvn);
                c1.Select("rec");
            }
            catch (Exception e)
            {

            }

        }

        public bool checkFirstTime(string bvn, string pwd)
        {
            bool valu = false;
            DataTable ds = new DataTable();
            DataTable dt = new DataTable();
            try
            {
                string sql = "select * from ForSterlings where changedPassword = '0' AND BVN = @bvn AND Password = @pwd";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@bvn", bvn);
                c.AddParam("@pwd", pwd);
                ds = c.Select("rec").Tables[0];
            }
            catch (Exception e)
            {

            }
            try
            {
                string sql1 = "select * from ForNonSterlings where changedPassword = '0' AND BVN = @bvn AND Password = @pwd";
                Sterling.MSSQL.Connect c1 = new Sterling.MSSQL.Connect("msconn");
                c1.SetSQL(sql1);
                c1.AddParam("@bvn", bvn);
                c1.AddParam("@pwd", pwd);
                dt = c1.Select("rec").Tables[0];
            }
            catch (Exception e)
            {

            }


            if (ds.Rows.Count > 0 || dt.Rows.Count > 0)
            {
                valu = true;
            }

            return valu;
        }

        public bool checkAvail(string bvn, string pwd)
        {

            bool valu = false;
            DataTable ds = new DataTable();
            DataTable dt = new DataTable();
            try
            {
                string sql = "select * from ForSterlings where changedPassword = '1' AND BVN = @bvn AND Password = @pwd";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@bvn", bvn);
                c.AddParam("@pwd", g.Encrypt(pwd));
                ds = c.Select("rec").Tables[0];
            }
            catch (Exception e)
            {

            }

            try
            {

                string sql1 = "select * from ForNonSterlings where changedPassword = '1' AND BVN = @bvn AND Password = @pwd";
                Sterling.MSSQL.Connect c1 = new Sterling.MSSQL.Connect("msconn");
                c1.SetSQL(sql1);
                c1.AddParam("@bvn", bvn);
                c1.AddParam("@pwd", g.Encrypt(pwd));
                dt = c1.Select("rec").Tables[0];
            }
            catch (Exception e)
            {

            }


            if (ds.Rows.Count > 0 || dt.Rows.Count > 0)
            {
                valu = true;
            }

            return valu;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("LogIn", "Dashboard");
        }

        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(new string[] { DefaultAuthenticationTypes.ApplicationCookie });
            return RedirectToAction("LogIn", "Dashboard");
        }

    }
}