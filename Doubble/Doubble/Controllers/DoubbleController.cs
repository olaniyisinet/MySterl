using Doubble.BLL;
using Doubble.BLL.DTOs;
using Doubble.BLL.LoanService;
using Doubble.DAL;
using Doubble.DTOs;
using Doubble.Eacbs;
using Doubble.GenerateOtp;
using Doubble.MAIL;
using Doubble.Models;
using Newtonsoft.Json;
using Sterling.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Doubble.Controllers
{

    public class DoubbleController : Controller
    {
        private readonly banksSoapClient cusDetails = new banksSoapClient();
        private readonly GenerateOtpSoapClient genOtp = new GenerateOtpSoapClient();
        private readonly EWS.ServiceSoapClient bv = new EWS.ServiceSoapClient();
        private readonly DoubbleMAILService mailServ = new DoubbleMAILService();
        private readonly DoubbleService d = new DoubbleService();
        private readonly DoubbleDalService dal = new DoubbleDalService();
        private readonly Gadget g = new Gadget();
        private readonly EWS1.banksSoapClient cusd = new EWS1.banksSoapClient();

        [AllowAnonymous]
        public ActionResult Doubble()
        {

            ViewBag.Response = Session["respMessage"];
            return View();
        }

        [HttpPost]
        public ActionResult Doubble(DoubbleRequest model)
        {
            DataTable bnv = new DataTable();
            try
            {
                DataTable record = d.checkUserExists(model.BVN);
                if (record.Rows.Count > 0)
                {
                    model.DateOfEntry = DateTime.Now;
                    List<BankAccount> acctNos = d.eligibleDoubbleAccounts(model.BVN);
                    
                    Session["acctNos"] = acctNos;
                    model.DateOfBirth = Convert.ToDateTime(record.Rows[0]["DateOfBirth"]);
                    var mod = new ForSterling { FirstName = record.Rows[0]["FirstName"].ToString(), LastName = record.Rows[0]["LastName"].ToString(), BVN = record.Rows[0]["BVN"].ToString(), Email = record.Rows[0]["Email"].ToString(), MobileNumber = record.Rows[0]["MobileNumber"].ToString(), DateOfBirth = model.DateOfBirth };
                    if (acctNos.Count < 1)
                    {
                        mod.PayInAccount = record.Rows[0]["PayInAccount"].ToString();
                        mod.BeneficiaryAccount = record.Rows[0]["BeneficiaryAccount"].ToString();
                        mod.CusNum = record.Rows[0]["CusNum"].ToString();
                    }
                    Mylogger.Info("BVN, " + model.BVN + " was routed to For-Sterling Page.");
                    return RedirectToAction("ForSterling", "Doubble", mod);
                }
            }
            catch (Exception e)
            {
                ViewBag.message = "Contact Administrator";
                Mylogger.Error("BVN, " + model.BVN + " encountered error " + e);
                return View(model);
            }

            try
            {
                bnv = bv.GetBVN(model.BVN).Tables[0];
            }
            catch (Exception e)
            {
                Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while getting details from BVN service.");
                ViewBag.message = "Contact Administrator";
                return View(model);
            }

            if (model.FirstName.ToUpper().Trim() != bnv.Rows[0]["FirstName"].ToString().ToUpper().Trim() || model.LastName.ToUpper().Trim() != bnv.Rows[0]["LastName"].ToString().ToUpper().Trim() || !model.DateOfBirth.Equals(Convert.ToDateTime(bnv.Rows[0]["DateOfBirth"])))
            {

                Mylogger.Info("User with BVN, " + model.BVN + " entered First Name: " + model.FirstName + ", Last Name: " + model.LastName + ", Date of Birth: " + model.DateOfBirth);
                ViewBag.message = "Please ensure your First name, Last name and Date of Birth is the same as registered on your BVN.";
                return View(model);
            }

            if (bnv.Rows.Count > 0)
            {
                Mylogger.Info("BVN, " + model.BVN + " was validated.");
                DataTable dt = new DataTable();
                EWS1.banksSoapClient scc = new EWS1.banksSoapClient();
                try
                {
                    dt = scc.getCustomerAccountsByBVN(model.BVN).Tables[0];
                }
                catch (Exception e)
                {
                    ViewBag.message = "Contact Administrator";
                    Mylogger.Error("BVN, " + model.BVN + " encountered error while connecting to EACBS " + e);
                    return View(model);
                }

                Gadget g = new Gadget();
                model.DateOfEntry = DateTime.Now;

                if (dt.Rows.Count < 1)
                {
                    try
                    {
                        try
                        {
                            model.ValCode = g.RandomString(10);
                            dal.saveDoubble(model.FirstName, model.LastName, model.BVN, model.MobileNumber, model.Email, model.DateOfBirth, "Onboard", model.ValCode);
                        }
                        catch (Exception e)
                        {

                            Mylogger.Error("BVN, " + model.BVN + " encountered error, " + e + " while trying to insert to DoubbleRequest Table.");
                            ViewBag.message = "Contact Administrator";
                            return View(model);
                        }

                        var md = new ForNonSterling { FirstName = model.FirstName, LastName = model.LastName, BVN = model.BVN, DateOfBirth = model.DateOfBirth, DateOfEntry = model.DateOfEntry, Email = model.Email, MobileNumber = model.MobileNumber };
                        Mylogger.Info("BVN, " + model.BVN + " was routed to For Non Sterling Page.");
                        return RedirectToAction("ForNonSterling", "Doubble", md);

                    }
                    catch (Exception e)
                    {
                        ViewBag.message = "Contact Administrator";
                        Mylogger.Error("BVN, " + model.BVN + " encountered error " + e);
                        return View(model);
                    }
                }
                else
                {
                    List<BankAccount> acctNos = d.eligibleDoubbleAccounts(model.BVN);
                    Session["acctNos"] = acctNos;
                    model.DateOfBirth = Convert.ToDateTime(bnv.Rows[0]["DateOfBirth"]);
                    var mod = new ForSterling { FirstName = model.FirstName, LastName = model.LastName, BVN = model.BVN, Email = model.Email, MobileNumber = model.MobileNumber, DateOfBirth = model.DateOfBirth };

                    try
                    {
                        model.ValCode = g.RandomString(10);
                        dal.saveDoubble(model.FirstName, model.LastName, model.BVN, model.MobileNumber, model.Email, model.DateOfBirth, "Sterling", model.ValCode);
                    }
                    catch (Exception e)
                    {

                        Mylogger.Error("BVN, " + model.BVN + " encountered error, " + e + " while trying to insert to DoubbleRequest Table.");
                        ViewBag.message = "Contact Administrator";
                        return View(model);
                    }
                    Mylogger.Info("BVN, " + model.BVN + " was routed to For-Sterling Page.");
                    return RedirectToAction("ForSterling", "Doubble", mod);
                }
            }
            else
            {
                ViewBag.Message = "Enter Valid Details.";
                Mylogger.Info("BVN, " + model.BVN + " entered wrong credentials.");
                try
                {
                    model.ValCode = g.RandomString(10);
                    dal.saveDoubble(model.FirstName, model.LastName, model.BVN, model.MobileNumber, model.Email, model.DateOfBirth, "Invalid BVN", model.ValCode);
                }
                catch (Exception e)
                {
                    ViewBag.message = "Contact Administrator";
                    Mylogger.Error("BVN, " + model.BVN + " encountered error, " + e + " while trying to insert to DoubbleRequest Table.");
                    return View(model);
                }

                return View(model);
            }

        }

        public ActionResult ForSterling(ForSterling model)
        {

            ViewBag.message = Session["error_msg"];
            List<BankAccount> acctList = (List<BankAccount>)Session["acctNos"];
            SelectList accList = new SelectList(acctList, "Account", "Account");
            ViewBag.Accounts = accList;
            Session["accounts"] = accList;
            return View(model);
        }

        [HttpPost]
        public ActionResult ForSterling(FormCollection fm, ForSterling model)
        {
            string j = "";
            string txtbx = fm["Account"];
            
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataTable dj = new DataTable();

            if (d.validAmount(model.PayInAmount, fm["CategoryName"]).Length != 0 || !String.IsNullOrEmpty(d.validAmount(model.PayInAmount, fm["CategoryName"])))
            {
                Mylogger.Error("BVN, " + model.BVN + " inputted amount " + model.PayInAmount);
                ViewBag.Message = d.validAmount(model.PayInAmount, fm["CategoryName"]);
                ViewBag.Accounts = Session["accounts"];
                return View(model);
            }
            model.PayInAccount = fm["Account2"].Substring(0, 10);
            if (model.BeneficiaryAccount == "Others")
            {
                model.BeneficiaryAccount = txtbx;
                model.BeneficiaryType = "Others";
            }
            else
            {
                model.BeneficiaryAccount = fm["Account1"].Substring(0, 10);
                model.BeneficiaryType = "Self";
            }
            try
            {
                dt = cusd.getAccountFullInfo(model.BeneficiaryAccount).Tables[0];
                dj = cusd.getAccountFullInfo(model.PayInAccount).Tables[0];
            }
            catch (Exception e)
            {
                Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while getting details from EACBS getAccountFulInfo service.");
                ViewBag.Message = "Contact Administrator";
                ViewBag.Accounts = Session["accounts"];
                return View(model);
            }

            if (dt.Rows.Count < 1)
            {
                Mylogger.Error("BVN, " + model.BVN + " chose a non-existing beneficiary account");
                ViewBag.Accounts = Session["accounts"];
                ViewBag.Message = "Enter a valid Beneficiary Account Number";
                return View(model);
            }
            model.BeneficiaryName = dt.Rows[0]["CUS_SHO_NAME"].ToString();
            model.FullName = dj.Rows[0]["CUS_SHO_NAME"].ToString();
            model.Category = fm["CategoryName"];
            try
            {
                model.Term = dal.getCategoryTerm(model.Category);
            }
            catch (Exception e)
            {
                ViewBag.Accounts = Session["accounts"];
                Mylogger.Error("BVN, " + model.BVN + " encountered error while getting category from Database.");
                ViewBag.Message = "Contact Administrator or Try Again.";
                return View(model);
            }
            model.DateOfEntry = DateTime.Now;
            Mylogger.Info("BVN, " + model.BVN + " was routed to CreateAccount Page.");
            return RedirectToAction("CreateAccount", "Doubble", model);

        }

        public ActionResult CreateAccount(ForSterling model)
        {
            ViewBag.categ = model.Category;
            ViewBag.term = model.Term;
            ViewBag.doe = string.Format("{0:MMMM dd, yyyy}", model.EffectiveDate);
            ViewBag.eod = string.Format("{0:MMMM dd, yyyy}", model.EffectiveDate.AddYears(model.Term));
            ViewBag.payInam = model.PayInAmount;
            ViewBag.payInacc = model.PayInAccount;
            ViewBag.payoutacc = model.BeneficiaryAccount;
            ViewBag.lastN = model.FullName;
            ViewBag.BName = model.BeneficiaryName;
            ViewBag.BAcc = model.BeneficiaryAccount;
            return View(model);

        }

        [HttpPost]
        public ActionResult CreateAccount(FormCollection fm, ForSterling model)
        {
            //Old
            DateTime thisDate1 = new DateTime(2019, 09, 28);
            Gadget g = new Gadget();
            decimal amt = Convert.ToDecimal(model.PayInAmount);
            DataTable dt = cusDetails.getAccountFullInfo(model.PayInAccount).Tables[0];

            model.CusNum = dt.Rows[0]["CUS_NUM"].ToString();
            if (model.DAOCode == null)
            {
                model.DAOCode = "";
            }

            ClassResponse resop = new ClassResponse();
            string otp = fm["Otp"];
            int resp = genOtp.verifyOtp(model.PayInAccount, otp, 1);
            if (resp == 1)
            {
                if (model.EffectiveDate > DateTime.Today)
                {
                    if (String.IsNullOrEmpty(model.ValCode) || model.ValCode.Trim().Length == 0)
                    {
                        model.ValCode = g.RandomString(10);
                    }

                    try
                    {
                        mailServ.sendLater(model.FirstName, model.EffectiveDate.ToString("dd MMMM yyyy"), model.PayInAmount, model.Email);
                        dal.SaveForLater(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.EffectiveDate, "Sterling", model.Term, model.ValCode, model.FullName, model.DateOfBirth);
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while inserting to For_Later table. Error from is: " + e.Message);
                        ViewBag.message = "Contact Administrator";
                        return View(model);
                    }
                    Session["respMessage"] = "Your request has been submitted.";
                    return RedirectToAction("Doubble", "Doubble");
                }
                try
                {
                    resop = d.createDoubble(model.CusNum, model.PayInAmount, model.Term, model.PayInAccount, model.BeneficiaryAccount, model.DAOCode, model.Category);
                }
                catch (Exception e)
                {
                    Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                    ViewBag.message = "Contact Administrator";
                    return View(model);
                }

                if (resop.ResponseText.Contains("Unauthorised overdraft") || resop.ResponseText.Contains("Account has a short fall of balance"))
                {
                    ViewBag.message = "Kindly Fund Your Account.";
                    Mylogger.Error("BVN, " + model.BVN + " failed while creating Doubble Product. Error from service is: " + resop.ResponseText);
                    return View(model);
                }
                if (resop.ResponseCode == "1")
                {
                    try
                    {
                    model.ARRANGEMENT_ID = d.ArrangementID(resop.ReferenceID);
                        DataTable record = d.checkPassword(model.BVN);
                        if (record.Rows.Count > 0)
                        {
                            dal.insertForSterling(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, resop.ReferenceID, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.FullName, record.Rows[0]["Password"].ToString(), model.ARRANGEMENT_ID, model.Term, model.EffectiveDate, model.EffectiveDate.AddYears(model.Term), model.DateOfBirth);
                            if (fm["Category"] != "Doubble Lumpsum")
                            {
                                mailServ.sendExDoubble(model.FirstName, g.thAppend(model.DateOfEntry.Day.ToString()), model.Email);
                            }
                            else
                            {
                                mailServ.sendExDoubbleLump(model.FirstName, model.Email);
                            }
                            Mylogger.Error("BVN, " + model.BVN + "'s Doubble product was created. Reference ID is: " + resop.ReferenceID);
                            Session["respMessage"] = "Your REFERENCE ID is: " + resop.ReferenceID;
                            return RedirectToAction("Doubble", "Doubble");
                        }
                        model.Password = g.RandomString(10);
                        dal.insertForSterling(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, resop.ReferenceID, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.FullName, model.Password, model.ARRANGEMENT_ID, model.Term, model.EffectiveDate, model.EffectiveDate.AddYears(model.Term), model.DateOfBirth);

                        if (fm["Category"] != "Doubble Lumpsum")
                        {
                            mailServ.sendDoubbleOpeningMail(model.FirstName, g.thAppend(model.DateOfEntry.Day.ToString()), model.Email, model.Password);
                        }
                        else
                        {
                            mailServ.LsendDoubbleOpeningMail(model.FirstName, model.Email, model.Password);
                        }
                        Mylogger.Error("BVN, " + model.BVN + "'s Doubble product was created. Reference ID is: " + resop.ReferenceID);
                        Session["respMessage"] = "Your REFERENCE ID is: " + resop.ReferenceID;
                        return RedirectToAction("Doubble", "Doubble");

                    }
                    catch (Exception e)
                    {
                        Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                        ViewBag.message = "Contact Administrator";
                        return RedirectToAction("CreateAccount", model);
                    }
                }
                ViewBag.message = "Kindly Try Again.";
                return RedirectToAction("CreateAccount", model);
            }
            ViewBag.Accounts = Session["accounts"];
            ViewBag.Message = "Your OTP Expired.";
            return RedirectToAction("CreateAccount", model);
        }
        [HttpPost]
        public string sendOTPNST(string mob_num, string email, string acct)
        {
            try
            {
                string j = genOtp.doGenerateOtpAndMailByPhoneNumberWithMailBodyD(mob_num, acct, 1, email, "e-business@sterlingbankng.com", "Doubble", "Your Doubble code is", "Your Doubble code is");
                if (j.StartsWith("1"))
                {
                    return "OTP Sent!";
                }
                return "";
            }
            catch (Exception e)
            {
                return "";
            }
        }

        [HttpPost]
        public string getBVN(string bvn)
        {
            return "test";
        }
        public ActionResult ForNonSterling(ForNonSterling model)
        {
            ViewBag.fname = model.FirstName;
            string sql = "select * from State_tbl";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            var mydata = ds.Tables[0].AsEnumerable().Select(d => new State { StateName = d.Field<string>("StateName") });
            var getState = mydata.ToList();
            SelectList list = new SelectList(getState, "StateName", "StateName");
            ViewBag.StateName = list;
            Session["statesName"] = list;
            return View(model);
        }

        [HttpPost]
        public ActionResult ForNonSterling(FormCollection fm, ForNonSterling model)
        {

            EWS1.banksSoapClient scc = new EWS1.banksSoapClient();
            DataTable ds = new DataTable();
            DataTable dg = new DataTable();
            string acct = "";
            string accountNo = "";
            if (model.Gender == "Male")
            {
                model.Title = "Mr";
            }
            else
            {
                model.Title = "Mrs";
            }
            model.State = fm["StateName"];
            Gadget g = new Gadget();
            DateTime thisDate1 = new DateTime(2018, 7, 27);
            
            var res = new ForNonSterling { BVN = model.BVN, DateOfBirth = model.DateOfBirth, DateOfEntry = thisDate1, Email = model.Email, FirstName = model.FirstName, Gender = model.Gender, HomeAddress = model.HomeAddress, LastName = model.LastName, MobileNumber = model.MobileNumber, State = model.State, Title = model.Title };
            int k = 0;
            try
            {
                k = genOtp.verifyOtp("66403", fm["Otp"], 1);
            }
            catch (Exception e)
            {
                ViewBag.StateName = Session["statesName"];
                ViewBag.Message = "Contact Admin or Try Again";
                Mylogger.Error("Individual Customer with BVN, " + model.BVN + "'s OTP could not be verified due to " + e.Message);
                return View(model);
            }
            if (k == 1)
            {
                try
                {
                    ds = scc.CreateIndividualCustomer(model.FirstName, model.LastName, model.HomeAddress, "4200", model.DAOCode, "4202", "2", "NG", "1", "NG", model.DAOCode, model.BVN, "NG0020555", model.Gender, model.DateOfBirth.ToString("yyyyMMdd"), model.MobileNumber, model.Title, model.Email, "", g.getStateVal(model.State)).Tables[0];
                }
                catch (Exception e)
                {
                    ViewBag.StateName = Session["statesName"];
                    ViewBag.Message = "Contact Admin or Try Again";
                    Mylogger.Error("Individual Customer with BVN, " + model.BVN + " could not be created due to " + e.Message);
                    return View(model);
                }
                if (ds.Rows[0]["ResponseCode"].ToString() == "1")
                {
                    acct = ds.Rows[0]["CustomerID"].ToString();
                    Mylogger.Info("Customer Number " + acct + " was created for " + model.BVN);
                    try
                    {
                        string repsonse = scc.Customer_UpdateResidenceStatus(acct, "DIASPORA");
                        if (repsonse != "11|SUCCESS")
                        {
                            Mylogger.Error("Resident status for " + acct + " was not updated successfully.");
                        }
                        Mylogger.Info("Resident status for " + acct + " was updated successfully.");
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error("Error " + e + " occurred while trying to update residence status for " + acct);
                    }
                    dg = scc.CreateAccount(acct, "KIA.KIA1", "NGN", thisDate1.ToString(@"dd-MMM-yyyy"), "NG0020555", "0", model.DAOCode, model.LastName + " " + model.FirstName).Tables[0];
                    if (dg.Rows[0]["ResponseCode"].ToString() == "1")
                    {
                        if (String.IsNullOrEmpty(model.ValCode) || model.ValCode.Trim().Length == 0)
                        {
                            model.ValCode = g.RandomString(10);
                        }
                        accountNo = dg.Rows[0]["AccountNumber"].ToString();
                        Mylogger.Info("Account Number " + accountNo + " was created for " + model.BVN);
                        model.BeneficiaryAccount = accountNo;
                        model.PayInAccount = accountNo;
                        model.CusNum = acct;
                        try
                        {
                            if (String.IsNullOrEmpty(model.ContactPerson) || model.ContactPerson.Trim().Length == 0)
                            {
                                dal.StoreToForNonSterlingTbl(model.FirstName, model.LastName, model.Email, model.MobileNumber, model.BVN, model.DateOfBirth, thisDate1, model.Title, model.Gender, model.State, model.HomeAddress, model.CusNum, model.PayInAccount, model.ValCode);
                            }
                            else
                            {
                                dal.StoreToForNonSterlingDisapora(model.FirstName, model.LastName, model.Email, model.MobileNumber, model.BVN, model.DateOfBirth, thisDate1, model.Title, model.Gender, model.State, model.HomeAddress, model.CusNum, model.PayInAccount, model.ValCode, model.ContactPerson, model.ContactAddress, model.ContactMobileNumber);
                            }
                            dal.StoreToForNonSterlingTbl(model.FirstName, model.LastName, model.Email, model.MobileNumber, model.BVN, model.DateOfBirth, thisDate1, model.Title, model.Gender, model.State, model.HomeAddress, model.CusNum, model.PayInAccount, model.ValCode);
                            mailServ.sendAccountOpeningMail(model.FirstName, model.PayInAccount, model.ValCode, model.Email);
                        }
                        catch (Exception e)
                        {
                            ViewBag.StateName = Session["statesName"];
                            ViewBag.message = "Contact Admin or Try Again";
                            Mylogger.Error("Individual Customer with BVN, " + model.BVN + " could not be stored/sent mail due to " + e.Message);
                            return View(model);
                        }
                        string WorkflowUrl = WebConfigurationManager.AppSettings["WorkflowUrl"];
                        string jsonOrder = null;
                        try
                        {
                            var refresp = new WorkFlowDto { AccountNumber = accountNo, Channel = "Doubble", CustomerID = acct, Date = DateTime.Today };
                            string responseMessage = string.Empty;
                            string URL = WorkflowUrl;
                            //var data = "";
                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                            request.Method = "POST";
                            request.ContentType = "application/json";
                            jsonOrder = JsonConvert.SerializeObject(refresp);
                            var data = Encoding.UTF8.GetBytes(jsonOrder);
                            request.ContentLength = data.Length;
                            string message = request.Method + URL + request.ContentType;
                            using (var stream = request.GetRequestStream())
                            {
                                stream.Write(data, 0, data.Length);
                            }
                            HttpWebResponse response = null;
                            try
                            {
                                response = (HttpWebResponse)request.GetResponse();
                                using (Stream responseStream = response.GetResponseStream())
                                {
                                    if (responseStream != null)
                                    {
                                        using (StreamReader reader = new StreamReader(responseStream))
                                        {
                                            responseMessage = reader.ReadToEnd();
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                responseMessage = "{\"errorMessages\":[\"" + e.Message.ToString() + "\"],\"errors\":{}}";
                                Mylogger.Error("Workflow could not be started for account " + accountNo + " with base no " + acct + " Error " + e + " was encountered.");
                            }
                            finally
                            {
                                if (response != null)
                                {
                                    ((IDisposable)response).Dispose();
                                }
                            }

                        }
                        catch (Exception e)
                        {


                        }
                        return RedirectToAction("CongPage", "Doubble", model);
                    }
                }
            }
            Mylogger.Error("OTP Expired for " + model.BVN);
            string j = genOtp.doGenerateOtpToMail(1, model.Email, "e-business@sterlingbankng.com", "OTP", "");
            ViewBag.Message = "Your OTP Expired";
            ViewBag.StateName = Session["statesName"];
            return View(model);
        }


        [HttpPost]
        public string resendOTPNST(string mob_num, string email)
        {
            try
            {
                string j = genOtp.doGenerateOtpAndMailByPhoneNumberWithMailBodyD(mob_num, "66403", 1, email, "e-business@sterlingbankng.com", "Doubble", "Your Doubble code is", "Your Doubble code is");
                if (j.StartsWith("1"))
                {
                    return "OTP Sent!";
                }
                return "";
            }
            catch (Exception e)
            {
                return "";
            }

        }
        public ActionResult CongPage(ForNonSterling model)
        {
            ViewBag.fname = model.FirstName;
            ViewBag.acct = model.PayInAccount;
            return View(model);
        }

        [HttpPost]
        public ActionResult CongPage(FormCollection fm, ForNonSterling model)
        {
            return RedirectToAction("AccountCreation", "Doubble", model);
        }

        public ActionResult AccountCreation(ForNonSterling model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult AccountCreation(FormCollection fm, ForNonSterling model)
        {
            model.BeneficiaryAccount = model.PayInAccount;
            model.BeneficiaryType = "Self";
            EWS1.banksSoapClient cusd = new EWS1.banksSoapClient();
            DataSet ds = new DataSet();
            if (d.validAmount(model.PayInAmount, fm["CategoryName"]).Length != 0 || !String.IsNullOrEmpty(d.validAmount(model.PayInAmount, fm["CategoryName"])))
            {
                Mylogger.Error("BVN, " + model.BVN + " inputted amount " + model.PayInAmount);
                ViewBag.Message = d.validAmount(model.PayInAmount, fm["CategoryName"]);
                ViewBag.Accounts = Session["accounts"];
                return View(model);
            }
            model.BeneficiaryName = model.LastName + " " + model.FirstName;
            model.Category = fm["CategoryName"];
            try
            {
                model.Term = dal.getCategoryTerm(model.Category);
            }
            catch (Exception e)
            {
                Mylogger.Error("BVN, " + model.BVN + " encountered error while getting category from Database.");
                ViewBag.Message = "Contact Administrator or Try Again.";
                return View(model);
            }
            Mylogger.Info("BVN, " + model.BVN + " was routed to ConfirmCreate Page.");
            return RedirectToAction("ConfirmCreate", "Doubble", model);

        }

        public ActionResult ConfirmCreate(ForNonSterling model)
        {
            ViewBag.categ = model.Category;
            ViewBag.term = model.Term;
            ViewBag.doe = string.Format("{0:MMMM dd, yyyy}", model.EffectiveDate);
            ViewBag.eod = string.Format("{0:MMMM dd, yyyy}", model.EffectiveDate.AddYears(model.Term));
            ViewBag.payInam = model.PayInAmount;
            ViewBag.payInacc = model.PayInAccount;
            ViewBag.payoutacc = model.BeneficiaryAccount;
            ViewBag.lastN = model.FirstName + " " + model.LastName;
            ViewBag.BName = model.BeneficiaryName;
            ViewBag.BAcc = model.BeneficiaryAccount;
            ViewBag.message = Session["error_msg"];
            return View(model);
        }

        [HttpPost]
        public ActionResult ConfirmCreate(FormCollection fm, ForNonSterling model)
        {
            decimal amt = Convert.ToDecimal(model.PayInAmount);
            if (model.DAOCode == null)
            {
                model.DAOCode = "";
            }
            Gadget g = new Gadget();
            if (model.EffectiveDate > DateTime.Today)
            {
                try
                {
                    mailServ.sendLater(model.FirstName, model.EffectiveDate.ToString("dd MMMM yyyy"), model.PayInAmount, model.Email);
                    dal.SaveForLater(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.EffectiveDate, "Onboard", model.Term, model.ValCode, model.LastName + " " + model.FirstName, model.DateOfBirth);
                }
                catch (Exception e)
                {
                    Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while inserting to For_Later table. Error from is: " + e.Message);
                    ViewBag.message = "Contact Administrator";
                    return RedirectToAction("ConfirmCreate", model);
                }
                Session["respMessage"] = "Your request has been submitted.";
                return RedirectToAction("Doubble", "Doubble");
            }
            //Old
            DateTime thisDate1 = new DateTime(2018, 7, 27);



            ClassResponse resop = new ClassResponse();
            try
            {

                bool ret = cusd.PerformPostingRestrictionOperation(model.PayInAccount, "NG0020006", "NGN", 1, "DEL");
                resop = d.createDoubble(model.CusNum, model.PayInAmount, model.Term, model.PayInAccount, model.BeneficiaryAccount, model.DAOCode, model.Category);
                bool reet = cusd.PerformPostingRestrictionOperation(model.PayInAccount, "NG0020006", "NGN", 1, "ADD");

            }
            catch (Exception e)
            {
                Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                ViewBag.message = "Contact Administrator";
                return RedirectToAction("ConfirmCreate", model);
            }
            if (resop.ResponseText.Contains("Unauthorised overdraft") || resop.ResponseText.Contains("Account has a short fall of balance"))
            {
                Mylogger.Error("BVN, " + model.BVN + " failed while creating Doubble Product. Error from service is: " + resop.ResponseText);
                Session["error_msg"] = "Kindly Fund Your Account";
                return RedirectToAction("ConfirmCreate", model);
            }

            if (resop.ResponseCode == "1")
            {
                try
                {
                    //model.ARRANGEMENT_ID = annuity.GetArrangementIdFromActivityId(resop.ReferenceID);
                    model.ARRANGEMENT_ID = d.ArrangementID(resop.ReferenceID);
                    model.Password = g.RandomString(10);
                    dal.updateForNonSterling(model.Category, model.PayInAmount, model.BeneficiaryName, model.BeneficiaryAccount, model.BeneficiaryType, resop.ReferenceID, model.DAOCode, model.ValCode, model.ARRANGEMENT_ID, model.Term, model.EffectiveDate, model.EffectiveDate.AddYears(model.Term), model.Password);

                    if (fm["Category"] != "Doubble Lumpsum")
                    {
                        mailServ.sendDoubbleOpeningMail(model.FirstName, g.thAppend(model.DateOfEntry.Day.ToString()), model.Email, model.Password);
                    }
                    else
                    {
                        mailServ.LsendDoubbleOpeningMail(model.FirstName, model.Email, model.Password);
                    }
                    Mylogger.Error("BVN, " + model.BVN + "'s Doubble product was created. Reference ID is: " + resop.ReferenceID);
                    Session["respMessage"] = "Your REFERENCE ID is: " + resop.ReferenceID;
                    return RedirectToAction("Doubble", "Doubble");
                }
                catch (Exception e)
                {
                    Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                    ViewBag.message = "Contact Administrator";
                    return RedirectToAction("ConfirmCreate", model);
                }

            }
            Mylogger.Error("BVN, " + model.BVN + " returned error while creating Doubble Product. Error from service is: " + resop.ResponseText);
            ViewBag.message = "Try Again or Contact Administrator.";
            //return View(model);
            return RedirectToAction("ConfirmCreate", model);

        }

        [HttpGet]
        public ActionResult NewInvestment()
        {
            ViewBag.message = Session["error_msg"]; //Session["BVN"].ToString());
            EWS1.banksSoapClient scc = new EWS1.banksSoapClient();
            DataTable record = d.checkPassword(Session["BVN"].ToString());
            Gadget g = new Gadget();
            List<BankAccount> acctNos = d.eligibleDoubbleAccounts(Session["BVN"].ToString());
            Session["acctNos"] = acctNos;
            var mod = new ForSterling { FirstName = record.Rows[0]["FirstName"].ToString(), LastName = record.Rows[0]["LastName"].ToString(), BVN = record.Rows[0]["BVN"].ToString(), Email = record.Rows[0]["Email"].ToString(), MobileNumber = record.Rows[0]["MobileNumber"].ToString(), DateOfBirth = Convert.ToDateTime(record.Rows[0]["DateOfBirth"]) };
            List<BankAccount> acctList = acctNos;
            SelectList accList = new SelectList(acctList, "Account", "Account");
            ViewBag.Accounts = accList;
            Session["accounts"] = accList;
            return View(mod);
        }

        [HttpPost]
        public ActionResult NewInvestment(FormCollection fm, ForSterling model)
        {

            string txtbx = fm["Account"];
            EWS1.banksSoapClient cusd = new EWS1.banksSoapClient();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataTable dj = new DataTable();

            if (d.validAmount(model.PayInAmount, fm["CategoryName"]).Length != 0 || !String.IsNullOrEmpty(d.validAmount(model.PayInAmount, fm["CategoryName"])))
            {
                Mylogger.Error("BVN, " + model.BVN + " inputted amount " + model.PayInAmount);
                ViewBag.Message = d.validAmount(model.PayInAmount, fm["CategoryName"]);
                ViewBag.Accounts = Session["accounts"];
                return View(model);
            }
            model.PayInAccount = fm["Account2"].Substring(0, 10);
            if (String.IsNullOrEmpty(txtbx) || txtbx.Trim().Length == 0)
            {
                model.BeneficiaryAccount = model.PayInAccount;
                model.BeneficiaryType = "Self";
            }
            else
            {
                model.BeneficiaryAccount = txtbx;
                model.BeneficiaryType = "Others";
            }
            try
            {
                dt = cusd.getAccountFullInfo(model.BeneficiaryAccount).Tables[0];
                dj = cusd.getAccountFullInfo(model.PayInAccount).Tables[0];
            }
            catch (Exception e)
            {
                Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while getting details from EACBS getAccountFulInfo service.");
                ViewBag.Message = "Contact Administrator";
                ViewBag.Accounts = Session["accounts"];
                return View(model);
            }

            if (dt.Rows.Count < 1)
            {
                Mylogger.Error("BVN, " + model.BVN + " chose a non-existing beneficiary account");
                ViewBag.Accounts = Session["accounts"];
                ViewBag.Message = "Enter a valid Beneficiary Account Number";
                return View(model);
            }
            model.BeneficiaryName = dt.Rows[0]["CUS_SHO_NAME"].ToString();
            model.FullName = dj.Rows[0]["CUS_SHO_NAME"].ToString();
            model.Category = fm["CategoryName"];
            try
            {
                model.Term = dal.getCategoryTerm(model.Category);
            }
            catch (Exception e)
            {
                ViewBag.Accounts = Session["accounts"];
                Mylogger.Error("BVN, " + model.BVN + " encountered error while getting category from Database.");
                ViewBag.Message = "Contact Administrator or Try Again.";
                return View(model);
            }
            model.DateOfEntry = DateTime.Now;
            Mylogger.Info("BVN, " + model.BVN + " was routed to CreateAccount Page.");
            return RedirectToAction("ConfNewInvestment", "Doubble", model);
        }


        public ActionResult ConfNewInvestment(ForSterling model)
        {

            ViewBag.categ = model.Category;
            ViewBag.term = model.Term;
            ViewBag.doe = string.Format("{0:MMMM dd, yyyy}", model.EffectiveDate);
            ViewBag.eod = string.Format("{0:MMMM dd, yyyy}", model.EffectiveDate.AddYears(model.Term));
            ViewBag.payInam = model.PayInAmount;
            ViewBag.payInacc = model.PayInAccount;
            ViewBag.payoutacc = model.BeneficiaryAccount;
            ViewBag.lastN = model.FullName;
            ViewBag.BName = model.BeneficiaryName;
            ViewBag.BAcc = model.BeneficiaryAccount;
            return View(model);
        }

        [HttpPost]
        public ActionResult ConfNewInvestment(FormCollection fm, ForSterling model)
        {
            //Old
            Session["BVN"] = model.BVN;
            DateTime thisDate1 = DateTime.Now;
            Gadget g = new Gadget();
            decimal amt = Convert.ToDecimal(model.PayInAmount);
            DataTable dt = cusDetails.getAccountFullInfo(model.PayInAccount).Tables[0];
            model.CusNum = dt.Rows[0]["CUS_NUM"].ToString();
            if (model.DAOCode == null)
            {
                model.DAOCode = "";
            }
            ClassResponse resop = new ClassResponse();
            string otp = fm["Otp"];
            int resp = genOtp.verifyOtp(model.PayInAccount, otp, 1);
            if (resp == 1)
            {
                if (model.EffectiveDate > DateTime.Today)
                {
                    if (String.IsNullOrEmpty(model.ValCode) || model.ValCode.Trim().Length == 0)
                    {
                        model.ValCode = g.RandomString(10);
                    }
                    try
                    {
                        mailServ.sendLater(model.FirstName, model.EffectiveDate.ToString("dd mmm yyyy"), model.PayInAmount, model.Email);

                        dal.SaveForLater(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.EffectiveDate, "Sterling", model.Term, model.ValCode, model.FullName, model.DateOfBirth);
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while inserting to For_Later table. Error from is: " + e.Message);
                        ViewBag.message = "Contact Administrator";
                        return View(model);
                    }
                    //Session["respMessage"] = "Your request has been submitted.";
                    return RedirectToAction("BlankPage", "Doubble");
                }
                try
                {
                    resop = d.createDoubble(model.CusNum, model.PayInAmount, model.Term, model.PayInAccount, model.BeneficiaryAccount, model.DAOCode, model.Category);

                }
                catch (Exception e)
                {
                    Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                    ViewBag.message = "Contact Administrator";
                    return View(model);
                }

                if (resop.ResponseText.Contains("Unauthorised overdraft") || resop.ResponseText.Contains("Account has a short fall of balance"))
                {
                    ViewBag.message = "Kindly Fund Your Account.";
                    Mylogger.Error("BVN, " + model.BVN + " failed while creating Doubble Product. Error from service is: " + resop.ResponseText);
                    return View(model);
                }
                if (resop.ResponseCode == "1")
                {
                    try
                    {
                        model.ARRANGEMENT_ID = d.ArrangementID(resop.ReferenceID);
                        DataTable record = d.checkPassword(model.BVN);
                        if (record.Rows.Count > 0)
                        {
                            dal.insertForSterling(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, resop.ReferenceID, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.FullName, record.Rows[0]["Password"].ToString(), model.ARRANGEMENT_ID, model.Term, model.EffectiveDate, model.EffectiveDate.AddYears(model.Term), model.DateOfBirth);
                            if (fm["Category"] != "Doubble Lumpsum")
                            {
                                mailServ.sendExDoubble(model.FirstName, g.thAppend(model.DateOfEntry.Day.ToString()), model.Email);
                            }
                            else
                            {
                                mailServ.sendExDoubbleLump(model.FirstName, model.Email);
                            }
                            Mylogger.Error("BVN, " + model.BVN + "'s Doubble product was created. Reference ID is: " + resop.ReferenceID);
                            //Session["respMessage"] = "Your REFERENCE ID is: " + resop.ReferenceID;
                            return RedirectToAction("BlankPage", "Doubble");
                        }
                    }
                    catch (Exception e)
                    {
                        Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                        ViewBag.message = "Contact Administrator";
                        return View(model);
                    }
                }
                return View(model);
            }
            ViewBag.Accounts = Session["accounts"];
            ViewBag.Message = "Your OTP Expired.";
            return View(model);
        }

        public ActionResult BlankPage()
        {
            return View();
        }



        [AllowAnonymous]
        public ActionResult Continue()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Continue(FormCollection fm)
        {
            string token = fm["sentCode"];
            string acct = fm["acctNo"];
            DataTable dt = new DataTable();
            DataTable ds = new DataTable();
            DataTable dj = new DataTable();
            try
            {
                string sql = "select * from ForNonSterlings where ValCode=@token and PayInAccount=@acct AND ARRANGEMENT_ID is NULL ";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@token", token);
                c.AddParam("@acct", acct);
                //DataSet ds = c.Select("rec");
                dt = c.Select("rec").Tables[0];


            }

            catch (Exception e)
            {
                //ViewBag.message = "Contact Administrator or Try Again.";
                //return View();
            }
            try
            {
                string sql1 = "select * from For_Later where ValCode=@token and PayInAccount=@acct";
                Sterling.MSSQL.Connect c1 = new Sterling.MSSQL.Connect("msconn");
                c1.SetSQL(sql1);
                c1.AddParam("@token", token);
                c1.AddParam("@acct", acct);
                ds = c1.Select("rec1").Tables[0];
            }
            catch (Exception e)
            {


            }
            try
            {
                string sql = "select * from ForNonSterlings where ValCode=@token and BeneficiaryAccount=@acct AND ARRANGEMENT_ID is NULL";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@token", token);
                c.AddParam("@acct", acct);
                //DataSet ds = c.Select("rec");
                dj = c.Select("rec").Tables[0];


            }

            catch (Exception e)
            {
                //ViewBag.message = "Contact Administrator or Try Again.";
                //return View();
            }
            if (dt.Rows.Count > 0 || dj.Rows.Count > 0 && ds.Rows.Count < 1)
            {
                ForNonSterling model = new ForNonSterling();
                try
                {
                    model.FirstName = dt.Rows[0]["FirstName"].ToString();
                    model.LastName = dt.Rows[0]["LastName"].ToString();
                    model.Email = dt.Rows[0]["Email"].ToString();
                    model.MobileNumber = dt.Rows[0]["MobileNumber"].ToString();
                    model.BVN = dt.Rows[0]["BVN"].ToString();
                    model.DateOfBirth = Convert.ToDateTime(dt.Rows[0]["DateOfBirth"]);
                    model.DateOfEntry = Convert.ToDateTime(dt.Rows[0]["DateOfEntry"]);
                    model.Title = dt.Rows[0]["Title"].ToString();
                    model.Gender = dt.Rows[0]["Gender"].ToString();
                    model.State = dt.Rows[0]["State"].ToString();
                    model.HomeAddress = dt.Rows[0]["HomeAddress"].ToString();
                    model.CusNum = dt.Rows[0]["CusNum"].ToString();
                    model.PayInAccount = acct;
                    model.ValCode = token;
                }
                catch (Exception e)
                {
                    model.FirstName = dj.Rows[0]["FirstName"].ToString();
                    model.LastName = dj.Rows[0]["LastName"].ToString();
                    model.Email = dj.Rows[0]["Email"].ToString();
                    model.MobileNumber = dj.Rows[0]["MobileNumber"].ToString();
                    model.BVN = dj.Rows[0]["BVN"].ToString();
                    model.DateOfBirth = Convert.ToDateTime(dj.Rows[0]["DateOfBirth"]);
                    model.DateOfEntry = Convert.ToDateTime(dj.Rows[0]["DateOfEntry"]);
                    model.Title = dj.Rows[0]["Title"].ToString();
                    model.Gender = dj.Rows[0]["Gender"].ToString();
                    model.State = dj.Rows[0]["State"].ToString();
                    model.HomeAddress = dj.Rows[0]["HomeAddress"].ToString();
                    model.CusNum = dj.Rows[0]["CusNum"].ToString();
                    model.PayInAccount = acct;
                    model.ValCode = token;
                }
                Mylogger.Info("BVN " + model.BVN + " and account, " + acct + " was verified to continue.");
                return RedirectToAction("AccountCreation", "Doubble", model);
            }
            Mylogger.Error("No record found for " + acct + ".");
            ViewBag.message = "Invalid details.";
            return View();



        

    }

        







    }
}
