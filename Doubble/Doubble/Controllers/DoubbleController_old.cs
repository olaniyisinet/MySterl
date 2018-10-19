using Doubble.BVNService;
using Doubble.DTOs;
using Doubble.Eacbs;
using Doubble.GenerateOtp;
using Doubble.LoanService;
using Doubble.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sterling.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Doubble.Controllers
{
    public enum httpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class DoubbleController : Controller
    {
        private readonly banksSoapClient cusDetails = new banksSoapClient();
        private readonly GenerateOtpSoapClient genOtp = new GenerateOtpSoapClient();
        private readonly DoubbleDBContext db = new DoubbleDBContext();
        //private readonly ServiceSoapClient bv = new ServiceSoapClient();
        private readonly LoanServicesSoapClient annuity = new LoanServicesSoapClient();
        private readonly string baseUrl = WebConfigurationManager.AppSettings["DoubbleAPIUrl"];
        private readonly string appMode = WebConfigurationManager.AppSettings["appMode"];
        // GET: Doubble
        [AllowAnonymous]
        public ActionResult Doubble()
        {

            Session["accounts"] = null;
            Session["acctNos"] = null;

            ViewBag.Response = Session["respMessage"];
            return View();
        }

        [HttpPost]

        public ActionResult Doubble(DoubbleRequest model)
        {
            //string isVerified = "NotSterling";
            if (String.IsNullOrEmpty(model.Email) || model.Email.Trim().Length == 0 || !model.Email.Contains("@"))
            {
                Mylogger.Error("BVN, " + model.BVN + " entered an invalid e-mail address");
                ViewBag.message = "Enter Valid Details.";
                return View(model);

            }
            else if (String.IsNullOrEmpty(model.FirstName) || model.FirstName.Trim().Length == 0)
            {
                Mylogger.Error("BVN, " + model.BVN + "'s FirstName is empty.");
                ViewBag.message = "Enter Valid Details.";
                return View(model);
            }
            else if (String.IsNullOrEmpty(model.LastName) || model.LastName.Trim().Length == 0)
            {
                Mylogger.Error("BVN, " + model.BVN + "'s LastName is empty.");
                ViewBag.message = "Enter Valid Details.";
                return View(model);
            }
            else if (String.IsNullOrEmpty(model.MobileNumber) || model.MobileNumber.Trim().Length == 0)
            {
                Mylogger.Error("BVN, " + model.BVN + "'s MobileNumber is empty.");
                ViewBag.message = "Enter Valid Details.";
                return View(model);
            }
            DataTable bnv = new DataTable();
            EWS.ServiceSoapClient bv = new EWS.ServiceSoapClient();
            try
            {
                bnv =   bv.GetBVN(model.BVN).Tables[0];
                string fName = bnv.Rows[0]["FirstName"].ToString();
                string lName = bnv.Rows[0]["LastName"].ToString();
                DateTime dob = Convert.ToDateTime(bnv.Rows[0]["DateOfBirth"]);
            }
            catch (Exception e)
            {
                Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while getting details from BVN service.");
                ViewBag.message = "Contact Administrator";
                return View(model);
            }

            if(model.FirstName.ToUpper().Trim() != bnv.Rows[0]["FirstName"].ToString().ToUpper().Trim() || model.LastName.ToUpper().Trim() != bnv.Rows[0]["LastName"].ToString().ToUpper().Trim() || !model.DateOfBirth.Equals(Convert.ToDateTime(bnv.Rows[0]["DateOfBirth"])))
            {

                Mylogger.Info("User with BVN, " + model.BVN + " entered First Name: " + model.FirstName + ", Last Name: " + model.LastName + ", Date of Birth: " + model.DateOfBirth );
                ViewBag.message = "Please ensure your First name, Last name and Date of Birth is the same as registered on your BVN.";
                return View(model);
            }

            if (bnv.Rows.Count > 0)
            {
                Mylogger.Info("BVN, " + model.BVN + " was validated.");
                var url = baseUrl + "api/v1/ValidateBVN?bvn=" + model.BVN;
                string strResponseValue = string.Empty;

                httpVerb httpMethod = httpVerb.GET;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = httpMethod.ToString();

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
                                strResponseValue = reader.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    strResponseValue = "{\"errorMessages\":[\"" + e.Message.ToString() + "\"],\"errors\":{}}";
                    Mylogger.Error("BVN, " + model.BVN + " encountered error, " + e + " while trying to get account with BVN.");
                }
                finally
                {
                    if (response != null)
                    {
                        ((IDisposable)response).Dispose();
                    }
                }
                Gadget g = new Gadget();
                model.DateOfEntry = DateTime.Now;

                if (strResponseValue.Contains("No account Exists for"))
                {
                    try
                    {
                        genOtp.doGenerateOtpToMail(1, model.Email, "toluwalope.ogundipe@sterlingbankng.com", "Test", "");
                        Mylogger.Info("OTP was generated for BVN, " + model.BVN);
                        //Live OTP module To Be Updated with Valid Account Number
                        //genOtp.doGenerateOtpAndMailByPhoneNumber(model.MobileNumber, "nuban", 1, model.Email, "toluwalope.ogundipe@sterlingbankng.com", "Test");
                    }
                    catch (Exception e)
                    {

                        Mylogger.Error("BVN, " + model.BVN + " encountered error, " + e + " while trying to get OTP.");
                        ViewBag.message = "Contact Administrator";
                        return View(model);
                    }

                    try
                    {
                        g.StoreToDoubbleTbl(model.FirstName, model.LastName, model.Email, model.MobileNumber, model.BVN, model.DateOfBirth, model.DateOfEntry, "NotSterling");

                    }
                    catch (Exception e)
                    {

                        Mylogger.Error("BVN, " + model.BVN + " encountered error, " + e + " while trying to insert to DoubbleRequest Table.");
                        ViewBag.message = "Contact Administrator";
                        return View(model);
                    }


                    //model.FirstName = bnv.Rows[0]["FirstName"].ToString();
                    //model.LastName = bnv.Rows[0]["LastName"].ToString();
                    //model.DateOfBirth = Convert.ToDateTime(bnv.Rows[0]["DateOfBirth"]);
                    var md = new ForNonSterling { FirstName = model.FirstName, LastName = model.LastName, BVN = model.BVN, DateOfBirth = model.DateOfBirth, DateOfEntry = model.DateOfEntry, Email = model.Email, MobileNumber = model.MobileNumber };
                    Mylogger.Info("BVN, " + model.BVN + " was routed to For Non Sterling Page.");
                    return RedirectToAction("ForNonSterling", "Doubble", md);
                }
                else if (strResponseValue == "[]")
                {
                    Mylogger.Error("BVN, " + model.BVN + " encountered error as EACBS getAccountWithBVN method is currently unavailable");
                    ViewBag.message = "Contact Administrator or Try Again.";
                    try
                    {
                        g.StoreToDoubbleTbl(model.FirstName, model.LastName, model.Email, model.MobileNumber, model.BVN, model.DateOfBirth, model.DateOfEntry, "Undecided");
                    }
                    catch (Exception e)
                    {

                        Mylogger.Error("BVN, " + model.BVN + " encountered error, " + e + " while trying to insert to DoubbleRequest Table.");

                        return View(model);
                    }
                    return View(model);

                }
                else if (strResponseValue.Contains("Contact"))
                {

                    Mylogger.Error("BVN, " + model.BVN + " encountered error,  while trying to get accounts with BVN.");

                    ViewBag.message = "Contact Administrator or Try Again.";
                    return View(model);

                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<BankAccountFullInfo> items = JsonConvert.DeserializeObject<List<BankAccountFullInfo>>(strResponseValue);


                List<BankAccount> acctNos = new List<BankAccount>();
                BankAccount k = new BankAccount();

                foreach (BankAccountFullInfo item in items)
                {
                    k.Account = item.AccountNo;
                    k.ACCT_TYPE = item.ACCT_TYPE;
                    acctNos.Add(new BankAccount() { Account = k.Account + " - " + k.ACCT_TYPE });
                }

                Session["acctNos"] = acctNos;

                if (strResponseValue.ToList().Count != 0 && !strResponseValue.Contains("No account Exists for") && strResponseValue != "[]")
                {
                    model.DateOfBirth = Convert.ToDateTime(bnv.Rows[0]["DateOfBirth"]);

                    var mod = new ForSterling { FirstName = model.FirstName, LastName = model.LastName, BVN = model.BVN, Email = model.Email, MobileNumber = model.MobileNumber };

                    try
                    {
                        g.StoreToDoubbleTbl(model.FirstName, model.LastName, model.Email, model.MobileNumber, model.BVN, model.DateOfBirth, model.DateOfEntry, "Sterling");
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
                Gadget g = new Gadget();
                ViewBag.Message = "Enter Valid Details.";
                try
                {
                    g.StoreToDoubbleTbl(model.FirstName, model.LastName, model.Email, model.MobileNumber, model.BVN, model.DateOfBirth, DateTime.Now, "Invalid BVN");
                }
                catch (Exception e)
                {

                    Mylogger.Error("BVN, " + model.BVN + " encountered error, " + e + " while trying to insert to DoubbleRequest Table.");
                    return View(model);
                }

                return View();
            }






            return View();
        }

        public ActionResult AccountCreation(ForNonSterling model)
        {

            string sql = "select * from Categories";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");

            var mydata = ds.Tables[0].AsEnumerable().Select(d => new Category { CategoryName = d.Field<string>("CategoryName") });

            var getCategorylist = mydata.ToList();
            SelectList list = new SelectList(getCategorylist, "CategoryName", "CategoryName");
            ViewBag.Category = list;
            Session["category"] = list;



            return View();
        }

        [HttpPost]
        public ActionResult AccountCreation(FormCollection fm, ForNonSterling model)
        {

            string txtbx = fm["Account"];


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

            EWS1.banksSoapClient cusd = new EWS1.banksSoapClient();
            DataTable dt = new DataTable();
            try
            {
                dt = cusd.getAccountFullInfo(model.BeneficiaryAccount).Tables[0];
            }
            catch (Exception e)
            {
                ViewBag.Category = Session["category"];
                Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while getting details from EACBS getAccountFulInfo service.");
                ViewBag.Message = "Contact Administrator";
                return View();
            }

            if (dt.Rows.Count < 1)
            {
                ViewBag.Category = Session["category"];
                Mylogger.Error("BVN, " + model.BVN + " entered an invalid Beneficiary Number.");
                ViewBag.Message = "Enter a valid Beneficiary Account Number";
                return View();
            }

            model.BeneficiaryName = dt.Rows[0]["CUS_SHO_NAME"].ToString();


            model.Category = fm["CategoryName"];

            DataSet ds = new DataSet();

            try
            {
                string sql = "select Term from Categories where CategoryName = @cat";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@cat", model.Category);
                ds = c.Select("rec");

            }
            catch (Exception e)
            {
                ViewBag.Category = Session["category"];
                Mylogger.Error("BVN, " + model.BVN + " encountered error while getting category from Database.");
                ViewBag.Message = "Contact Administrator or Try Again.";
                return View();
            }
            DataTable dat = ds.Tables[0];
            model.Term = Convert.ToInt16(dat.Rows[0]["Term"]);


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
            return View();
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
                    g.insertForLater(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.EffectiveDate, "Onboard", model.Term, model.ValCode, model.LastName + " " + model.FirstName);

                }
                catch (Exception e)
                {
                    Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while inserting to For_Later table. Error from is: " + e.Message);
                    ViewBag.message = "Contact Administrator";
                    return View();
                }

                Session["respMessage"] = "Your request has been submitted.";

                return RedirectToAction("Doubble", "Doubble");
            }
            //Old
            DateTime thisDate1 = new DateTime(2018, 7, 3);

            if (appMode == "online")
            {
                thisDate1 = DateTime.Now;
            }


            ClassResponse resop = new ClassResponse();
            bannks.banksSoapClient banks = new bannks.banksSoapClient();

            try
            {
                bool ret = banks.PerformPostingRestrictionOperation(model.PayInAccount, "NG0020006", "NGN", 1, "DEL");
                if (fm["Category"] != "Doubble L")
                {
                    resop = annuity.CreateAnnuityDeposit(model.CusNum, "NGN", thisDate1, amt, model.Term, model.PayInAccount, model.BeneficiaryAccount, model.BeneficiaryAccount, model.BeneficiaryAccount, model.DAOCode, "");

                }
                else
                {
                    resop = annuity.CreateAnnuityDepositLumpSum(model.CusNum, "NGN", thisDate1, amt, model.Term, model.PayInAccount, model.BeneficiaryAccount, model.BeneficiaryAccount, model.BeneficiaryAccount, model.DAOCode, "");
                }
            }
            catch (Exception e)
            {
                Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                ViewBag.message = "Contact Administrator";
                return View();
            }

           
            if (resop.ResponseCode == "1")
            {
                try
                {
                    bool ret = banks.PerformPostingRestrictionOperation(model.PayInAccount, "NG0020006", "NGN", 1, "ADD");
                    model.ARRANGEMENT_ID = annuity.GetArrangementIdFromActivityId(resop.ReferenceID);
                    model.ValCode = g.RandomString(10);
                    g.updateForNonSterling(model.Category, model.PayInAmount, model.BeneficiaryName, model.BeneficiaryAccount, model.BeneficiaryType, resop.ReferenceID, model.DAOCode, model.ValCode, model.ARRANGEMENT_ID, model.Term, model.EffectiveDate, model.EffectiveDate.AddYears(model.Term));

                    if (fm["Category"] != "Doubble L")
                    {
                        g.sendDoubbleOpeningMail(model.FirstName, model.PayInAmount, model.DateOfEntry.Day.ToString(), model.Email, model.PayInAccount, model.ValCode);
                    }
                    else
                    {
                        g.LsendDoubbleOpeningMail(model.FirstName, model.PayInAmount, model.DateOfEntry.Day.ToString(), model.Email, model.PayInAccount, model.ValCode);
                    }


                }
                catch (Exception e)
                {
                    Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                    //ViewBag.message = "Contact Administrator";
                    //return View();
                }
                Mylogger.Error("BVN, " + model.BVN + "'s Doubble product was created. Reference ID is: " + resop.ReferenceID);


                Session["respMessage"] = "Your REFERENCE ID is: " + resop.ReferenceID;

                return RedirectToAction("Doubble", "Doubble");
            }

            Mylogger.Error("BVN, " + model.BVN + " returned error while creating Doubble Product. Error from service is: " + resop.ResponseText);

            ViewBag.message = "Try Again or Contact Administrator.";
            return View();


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
            //var getState = db.States.Where(d => d.ID > 0).ToList();
            SelectList list = new SelectList(getState, "StateName", "StateName");
            ViewBag.StateName = list;
            Session["statesName"] = list;






            return View();
        }

        [HttpPost]
        public ActionResult ForNonSterling(FormCollection fm, ForNonSterling model)
        {

            if (String.IsNullOrEmpty(model.HomeAddress) || model.HomeAddress.Trim().Length == 0)
            {
                ViewBag.StateName = Session["statesName"];
                ViewBag.message = "Kindly enter a valid Address";
                return View(model);

            }

            if (String.IsNullOrEmpty(fm["Otp"]) || fm["Otp"].Trim().Length == 0)
            {
                ViewBag.StateName = Session["statesName"];
                ViewBag.message = "Kindly Input OTP.";
                return View(model);

            }



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


            DateTime thisDate1 = new DateTime(2018, 7, 3);
            if (appMode == "online")
            {
                thisDate1 = DateTime.Now;
            }


            var res = new ForNonSterling { BVN = model.BVN, DateOfBirth = model.DateOfBirth, DateOfEntry = thisDate1, Email = model.Email, FirstName = model.FirstName, Gender = model.Gender, HomeAddress = model.HomeAddress, LastName = model.LastName, MobileNumber = model.MobileNumber, State = model.State, Title = model.Title };



            EWS1.banksSoapClient scc = new EWS1.banksSoapClient();
            DataTable ds = new DataTable();
            DataTable dg = new DataTable();

            string acct = "";
            string accountNo = "";

            int k = 0;

            try
            {
                k = genOtp.verifyOtpToMail(model.Email, fm["Otp"], 1, 2);
            }
            catch (Exception e)
            {
                ViewBag.StateName = Session["statesName"];
                ViewBag.Message = "Contact Admin or Try Again";
                Mylogger.Error("Individual Customer with BVN, " + model.BVN + "'s OTP could not be verified due to " + e.Message);
                return View();
            }


            if (k == 1)
            {
                try
                {
                    ds = scc.CreateIndividualCustomer(model.FirstName, model.LastName, model.HomeAddress, "4200", "777", "4202", "2", "NG", "1", "NG", "777", model.BVN, "NG200006", model.Gender, model.DateOfBirth.ToString("yyyyMMdd"), model.MobileNumber, model.Title, model.Email, "", g.getStateVal(model.State)).Tables[0];

                }
                catch (Exception e)
                {
                    ViewBag.StateName = Session["statesName"];
                    ViewBag.Message = "Contact Admin or Try Again";
                    Mylogger.Error("Individual Customer with BVN, " + model.BVN + " could not be created due to " + e.Message);
                    return View();

                }

                if (ds.Rows[0]["ResponseCode"].ToString() == "1")
                {

                    acct = ds.Rows[0]["CustomerID"].ToString();
                    Mylogger.Info("Customer Number " + acct + " was created for " + model.BVN);
                    dg = scc.CreateAccount(acct, "KIA.KIA1", "NGN", thisDate1.ToString(@"dd-MMM-yyyy"), "NG0020006", "0", "777", model.LastName + " " + model.FirstName).Tables[0];

                    if (dg.Rows[0]["ResponseCode"].ToString() == "1")
                    {
                        model.ValCode = g.RandomString(10);
                        accountNo = dg.Rows[0]["AccountNumber"].ToString();
                        Mylogger.Info("Account Number " + accountNo + " was created for " + model.BVN);
                        model.BeneficiaryAccount = accountNo;
                        model.PayInAccount = accountNo;
                        model.CusNum = acct;
                        try
                        {
                            g.StoreToForNonSterlingTbl(model.FirstName, model.LastName, model.Email, model.MobileNumber, model.BVN, model.DateOfBirth, thisDate1, model.Title, model.Gender, model.State, model.HomeAddress, model.CusNum, model.PayInAccount, model.ValCode);
                            g.sendAccountOpeningMail(model.FirstName, model.PayInAccount, model.ValCode, model.Email);
                        }
                        catch (Exception e)
                        {
                            ViewBag.StateName = Session["statesName"];
                            ViewBag.message = "Contact Admin or Try Again";
                            Mylogger.Error("Individual Customer with BVN, " + model.BVN + " could not be stored/sent mail due to " + e.Message);
                            return View();
                        }


                        return RedirectToAction("CongPage", "Doubble", model);
                    }

                }
            }
            Mylogger.Error("OTP Expired for " + model.BVN);
            string j = genOtp.doGenerateOtpToMail(1, model.Email, "toluwalope.ogundipe@gmail.com", "Test", "");
            ViewBag.Message = "Try Again";
            ViewBag.StateName = Session["statesName"];
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

            string sql = "select * from ForNonSterlings where ValCode=@token and PayInAccount=@acct";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@token", fm["sentCode"]);
            c.AddParam("@acct", fm["acctNo"]);
            DataSet ds = c.Select("rec");
            DataTable dt = ds.Tables[0];

            

            if (dt.Rows.Count < 1)
            {
                Mylogger.Error("No record found for " + acct + ".");
                ViewBag.message = "Invalid details.";
                return View();
            }
            ForNonSterling model = new ForNonSterling();
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
            model.CusNum = dt.Rows[0]["HomeAddress"].ToString();
            model.PayInAccount = acct;
            model.ValCode = token;
            Mylogger.Info("BVN " + model.BVN + " and account, " + acct + " was verified to continue.");
            return RedirectToAction("AccountCreation", "Doubble", model);

        }

        public ActionResult CongPage(ForNonSterling model)
        {
            ViewBag.fname = model.FirstName;
            ViewBag.acct = model.PayInAccount;
            return View();
        }

        [HttpPost]
        public ActionResult CongPage(FormCollection fm, ForNonSterling model)
        {

            return RedirectToAction("AccountCreation", "Doubble", model);
        }

        public ActionResult ForSterling(ForSterling model)
        {
            string sql = "select * from Categories";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");

            var mydata = ds.Tables[0].AsEnumerable().Select(d => new Category { CategoryName = d.Field<string>("CategoryName") });

            var getCategorylist = mydata.ToList();
            SelectList list = new SelectList(getCategorylist, "CategoryName", "CategoryName");
            ViewBag.Category = list;
            Session["category"] = list;



            List<BankAccount> acctList = (List<BankAccount>)Session["acctNos"];
            SelectList accList = new SelectList(acctList, "Account", "Account");
            ViewBag.Accounts = accList;
            Session["accounts"] = accList;

            return View();
        }

        [HttpPost]
        public ActionResult ForSterling(FormCollection fm, ForSterling model)
        {
            

            string txtbx = fm["Account"];

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

            EWS1.banksSoapClient cusd = new EWS1.banksSoapClient();
            DataTable dt = new DataTable();
            DataTable dj = new DataTable();
            try
            {
                dt = cusd.getAccountFullInfo(model.BeneficiaryAccount).Tables[0];
                dj = cusd.getAccountFullInfo(model.PayInAccount).Tables[0];
            }
            catch (Exception e)
            {
                ViewBag.Category = Session["category"];
                Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while getting details from EACBS getAccountFulInfo service.");
                ViewBag.Message = "Contact Administrator";
                return View();
            }

            if (dt.Rows.Count < 1)
            {
                
                ViewBag.Category = Session["category"];
                ViewBag.Accounts = Session["accounts"];
                //ViewBag.branches = Session["branch"];
                ViewBag.Message = "Enter a valid Beneficiary Account Number";
                return View();
            }
            model.BeneficiaryName = dt.Rows[0]["CUS_SHO_NAME"].ToString();
            model.FullName = dj.Rows[0]["CUS_SHO_NAME"].ToString();

            //model.PayInAccount = fm["Account2"];


            model.Category = fm["CategoryName"];

            DataSet ds = new DataSet();

            try
            {
                string sql = "select Term from Categories where CategoryName = @cat";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@cat", model.Category);
                ds = c.Select("rec");

            }
            catch (Exception e)
            {
                ViewBag.Category = Session["category"];
                Mylogger.Error("BVN, " + model.BVN + " encountered error while getting category from Database.");
                ViewBag.Message = "Contact Administrator or Try Again.";
                return View();
            }
            DataTable dat = ds.Tables[0];
            model.Term = Convert.ToInt16(dat.Rows[0]["Term"]);


            model.DateOfEntry = DateTime.Now;

            //int j = genOtp.doGenerateOtp(model.PayInAccount, 1);
            string j = "";
            j = genOtp.doGenerateOtpToMail(1, model.Email, "toluwalope.ogundipe@sterlingbankng.com", "Test", "");


            if (j == "1")
            {
                Mylogger.Info("BVN, " + model.BVN + " was routed to CreateAccount Page.");

                return RedirectToAction("CreateAccount", "Doubble", model);

            }

            ViewBag.message = "Kindly Try Again.";
            ViewBag.Category = Session["category"];
            ViewBag.Accounts = Session["accounts"];
            return View("ForSterling", model);

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
            return View();

        }

        [HttpPost]
        public ActionResult CreateAccount(FormCollection fm, ForSterling model)
        {
            Gadget g = new Gadget();
            decimal amt = Convert.ToDecimal(model.PayInAmount);
            DataTable dt = cusDetails.getAccountFullInfo(model.PayInAccount).Tables[0];

            model.CusNum = dt.Rows[0]["CUS_NUM"].ToString();

            if (model.DAOCode == null)
            {
                model.DAOCode = "";

            }
            model.ValCode = g.RandomString(10);

            if (model.EffectiveDate > DateTime.Today)
            {
                try
                {
                    g.insertForLater(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.EffectiveDate, "Sterling", model.Term, model.ValCode, model.FullName);

                }
                catch(Exception e)
                {
                    Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while inserting to For_Later table. Error from is: " + e.Message);
                    ViewBag.message = "Contact Administrator";
                    return View();
                }

                Session["respMessage"] = "Your request has been submitted.";

                return RedirectToAction("Doubble", "Doubble");
            }

            
            //Old
            DateTime thisDate1 = new DateTime(2018, 7, 3);

            if (appMode == "online")
            {
                thisDate1 = DateTime.Now;
            }

            ClassResponse resop = new ClassResponse();            

            string otp = fm["Otp"];
                        
            
            //int resp = genOtp.verifyOtp(accountNo, otp, 1);
            int resp = genOtp.verifyOtpToMail(model.Email, otp, 1, 2);
            

            if (resp == 1)
            {


                try
                {
                    if (fm["Category"] != "Doubble L")
                    {
                        resop = annuity.CreateAnnuityDeposit(model.CusNum, "NGN", thisDate1, amt, model.Term, model.PayInAccount, model.BeneficiaryAccount, model.BeneficiaryAccount, model.BeneficiaryAccount, model.DAOCode, "");

                    }
                    else
                    {
                        resop = annuity.CreateAnnuityDepositLumpSum(model.CusNum, "NGN", thisDate1, amt, model.Term, model.PayInAccount, model.BeneficiaryAccount, model.BeneficiaryAccount, model.BeneficiaryAccount, model.DAOCode, "");
                    }
                }
                catch (Exception e)
                {
                    Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                    ViewBag.message = "Contact Administrator";
                    return View();
                }


                if (resop.ResponseCode == "1")
                {
                    try
                    {
                        model.ARRANGEMENT_ID = annuity.GetArrangementIdFromActivityId(resop.ReferenceID);
                        g.insertForSterling(model.FirstName, model.LastName, model.MobileNumber, model.BVN, model.Category, model.PayInAccount, model.BeneficiaryName, model.BeneficiaryAccount, model.DateOfEntry, model.PayInAmount, resop.ReferenceID, model.DAOCode, model.BeneficiaryType, model.Email, model.CusNum, model.FullName, model.ValCode, model.ARRANGEMENT_ID,model.Term, model.EffectiveDate, model.EffectiveDate.AddYears(model.Term));

                        if (fm["Category"] != "Doubble L")
                        {
                            g.sendDoubbleOpeningMail(model.FirstName, model.PayInAmount, model.DateOfEntry.Day.ToString(), model.Email, model.PayInAccount, model.ValCode);
                        }
                        else
                        {
                            g.LsendDoubbleOpeningMail(model.FirstName, model.PayInAmount, model.DateOfEntry.Day.ToString(), model.Email, model.PayInAccount, model.ValCode);
                        }


                    }
                    catch (Exception e)
                    {
                        Mylogger.Error("BVN, " + model.BVN + " returned error: " + e + " while creating Doubble Product. Error from service is: " + resop.ResponseText);
                        //ViewBag.message = "Contact Administrator";
                        //return View();
                    }
                    Mylogger.Error("BVN, " + model.BVN + "'s Doubble product was created. Reference ID is: " + resop.ReferenceID);


                    Session["respMessage"] = "Your REFERENCE ID is: " + resop.ReferenceID;

                    return RedirectToAction("Doubble", "Doubble");

                }


                return RedirectToAction("ForSterling", "Doubble", model);




            }
            ViewBag.Category = Session["category"];
            ViewBag.Accounts = Session["accounts"];

            ViewBag.Message = "Kindly Try Again.";
            return RedirectToAction("ForSterling", "Doubble");
        }
    }
}
