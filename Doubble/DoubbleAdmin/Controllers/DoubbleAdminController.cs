using Doubble.BLL;
using DoubbleAdmin.Eacbs;
using DoubbleAdmin.LDAP;
using DoubbleAdmin.Models;
using OfficeOpenXml;
using Sterling.MSSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace DoubbleAdmin.Controllers
{
    public class DoubbleAdminController : Controller
    {
        private readonly ldapSoapClient userr = new ldapSoapClient();
        private readonly Gadget g = new Gadget();
        private readonly DoubbleDBEntities db = new DoubbleDBEntities();
        private readonly banksSoapClient cusDetails = new banksSoapClient();
        private readonly EWS.banksSoapClient cus = new EWS.banksSoapClient();
        private readonly DoubbleService d = new DoubbleService();
        private readonly TerminateFD.banksSoapClient terminate = new TerminateFD.banksSoapClient();


        // GET: DoubbleAdmin
        public ActionResult DoubbleAdmin()
        {
            //cus.UpdateDepositPayoutAccounts

            string sql = "select [FullName],[BVN],[Category],[PayInAccount],[Term],[DateOfEntry],[EffectiveDate],[MaturityDate],[PayInAmount] from ForNonSterlings";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];

            for(int i = 0; i < records.Rows.Count; i++)
            {
                if(records.Rows[i]["FullName"].ToString() == records.Rows[i]["BVN"].ToString())
                {
                    DataTable dat = cusDetails.getAccountFullInfo(records.Rows[i]["PayInAccount"].ToString()).Tables[0];
                    records.Rows[i]["FullName"] = dat.Rows[0]["CUS_SHO_NAME"].ToString();
                }
            }

            string sql1 = "select [FullName],[BVN],[Category],[PayInAccount],[Term],[DateOfEntry],[EffectiveDate],[MaturityDate],[PayInAmount] from ForSterlings";
            Sterling.MSSQL.Connect c1 = new Sterling.MSSQL.Connect("msconn");
            c1.SetSQL(sql1);
            DataSet dt = c1.Select("rec1");
            DataTable records1 = dt.Tables[0];
            for (int i = 0; i < records1.Rows.Count; i++)
            {
                if (records1.Rows[i]["FullName"].ToString() == records1.Rows[i]["BVN"].ToString())
                {
                    DataTable dat = cusDetails.getAccountFullInfo(records1.Rows[i]["PayInAccount"].ToString()).Tables[0];
                    records1.Rows[i]["FullName"] = dat.Rows[0]["CUS_SHO_NAME"].ToString();
                }
            }

            DataTable dtall = new DataTable();
            dtall = records.Copy();
            dtall.Merge(records1);

            int count3 = 0;
            int count5 = 0;
            int count10 = 0;
            int countL = 0;

            for (int i = 0; i < dtall.Rows.Count; i++)
            {
                if (dtall.Rows[i]["Category"].ToString().Trim() == "Doubble 3")
                {
                    count3++;
                }
                else if (dtall.Rows[i]["Category"].ToString().Trim() == "Doubble 5")
                {
                    count5++;
                }
                else if (dtall.Rows[i]["Category"].ToString().Trim() == "Doubble 10")
                {
                    count10++;
                }
                else
                {
                    countL++;
                }

            }
            ViewBag.d3 = count3;
            ViewBag.d5 = count5;
            ViewBag.d10 = count10;
            ViewBag.ls = countL;

            return View(dtall);
        }

        [HttpPost]
        public void ExportDoubbADmin()
        {
            string sql = "select [FullName],[Category],[PayInAccount],[Term],[EffectiveDate],[MaturityDate],[PayInAmount] from ForNonSterlings";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];
            for (int i = 0; i < records.Rows.Count; i++)
            {
                if (records.Rows[i]["FullName"].ToString() == records.Rows[i]["BVN"].ToString())
                {
                    DataTable dat = cusDetails.getAccountFullInfo(records.Rows[i]["PayInAccount"].ToString()).Tables[0];
                    records.Rows[i]["FullName"] = dat.Rows[0]["CUS_SHO_NAME"].ToString();
                }
            }

            string sql1 = "select [FullName],[Category],[PayInAccount],[Term],[EffectiveDate],[MaturityDate],[PayInAmount] from ForSterlings";
            Sterling.MSSQL.Connect c1 = new Sterling.MSSQL.Connect("msconn");
            c1.SetSQL(sql1);
            DataSet dt = c1.Select("rec1");
            DataTable records1 = dt.Tables[0];

            for (int i = 0; i < records1.Rows.Count; i++)
            {
                if (records1.Rows[i]["FullName"].ToString() == records1.Rows[i]["BVN"].ToString())
                {
                    DataTable dat = cusDetails.getAccountFullInfo(records1.Rows[i]["PayInAccount"].ToString()).Tables[0];
                    records1.Rows[i]["FullName"] = dat.Rows[0]["CUS_SHO_NAME"].ToString();
                }
            }

            DataTable dtall = new DataTable();
            dtall = records.Copy();
            dtall.Merge(records1);

            #region Epplus
            HttpContext.Response.Clear();
            HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.AddHeader("content-disposition", "attachment;filename=DoubbleSubscription.xlsx");


            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add("tolu");
                worksheet.Cells["A1"].LoadFromDataTable(dtall, true);

                worksheet.Cells.AutoFitColumns();

                var ms = new MemoryStream();
                excel.SaveAs(ms);
                ms.WriteTo( HttpContext.Response.OutputStream);
            }
            HttpContext.Response.Flush();
            HttpContext.Response.End();
            #endregion

           
        }

        public ActionResult Preterminate()
        {
            string sql = "select [FullName],[ARRANGEMENT_ID],[BVN],[Category],[PayInAccount],[Term],[DateOfEntry],[EffectiveDate],[MaturityDate],[PayInAmount] from ForNonSterlings where Terminate='Terminate'";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];

            PreTerminate p = new PreTerminate();
            List<PreTerminate> list = new List<PreTerminate>();

            

            for (int i = 0; i < records.Rows.Count; i++)
            {
                p.BVN = records.Rows[i]["BVN"].ToString();
                p.Category = records.Rows[i]["Category"].ToString();
                p.DateOfEntry = Convert.ToDateTime(records.Rows[i]["DateOfEntry"]);
                p.EffectiveDate = Convert.ToDateTime(records.Rows[i]["EffectiveDate"]);
                p.FullName = records.Rows[i]["FullName"].ToString();
                p.MaturityDate = Convert.ToDateTime(records.Rows[i]["MaturityDate"]);
                p.PayInAccount = records.Rows[i]["PayInAccount"].ToString();
                p.PayInAmount = records.Rows[i]["PayInAmount"].ToString();
                p.Term = records.Rows[i]["Term"].ToString();
                p.ArrangementId = records.Rows[i]["ARRANGEMENT_ID"].ToString();
                if (records.Rows[i]["FullName"].ToString() == records.Rows[i]["BVN"].ToString())
                {
                    DataTable dat = cusDetails.getAccountFullInfo(records.Rows[i]["PayInAccount"].ToString()).Tables[0];
                    p.FullName = dat.Rows[0]["CUS_SHO_NAME"].ToString();
                }

                list.Add(p);
            }

            string sql1 = "select [FullName],[ARRANGEMENT_ID],[BVN],[Category],[PayInAccount],[Term],[DateOfEntry],[EffectiveDate],[MaturityDate],[PayInAmount] from ForSterlings where Terminate='Terminate'";
            Sterling.MSSQL.Connect c1 = new Sterling.MSSQL.Connect("msconn");
            c1.SetSQL(sql1);
            DataSet dt = c1.Select("rec1");
            DataTable records1 = dt.Tables[0];
            for (int i = 0; i < records1.Rows.Count; i++)
            {
                p.BVN = records.Rows[i]["BVN"].ToString();
                p.Category = records.Rows[i]["Category"].ToString();
                p.DateOfEntry = Convert.ToDateTime(records.Rows[i]["DateOfEntry"]);
                p.EffectiveDate = Convert.ToDateTime(records.Rows[i]["EffectiveDate"]);
                p.FullName = records.Rows[i]["FullName"].ToString();
                p.MaturityDate = Convert.ToDateTime(records.Rows[i]["MaturityDate"]);
                p.PayInAccount = records.Rows[i]["PayInAccount"].ToString();
                p.PayInAmount = records.Rows[i]["PayInAmount"].ToString();
                p.Term = records.Rows[i]["Term"].ToString();
                p.ArrangementId = records.Rows[i]["ARRANGEMENT_ID"].ToString();
                if (records1.Rows[i]["FullName"].ToString() == records1.Rows[i]["BVN"].ToString())
                {
                    DataTable dat = cusDetails.getAccountFullInfo(records1.Rows[i]["PayInAccount"].ToString()).Tables[0];
                    p.FullName = dat.Rows[0]["CUS_SHO_NAME"].ToString();
                }
            }

            return View(p);

        }

        public ActionResult Approve(string arrangementId)
        {
            DataTable records = new DataTable();
            try
            {
                records = d.ArrangementDetails(arrangementId);
                string resp = cus.UpdateDepositPayoutAccounts(arrangementId, Convert.ToDateTime(records.Rows[0]["EffectiveDate"]), records.Rows[0]["PayInAccount"].ToString(), records.Rows[0]["PayInAccount"].ToString(), records.Rows[0]["PayInAccount"].ToString());
                terminate.TerminateFixedDeposit(arrangementId);
                g.Preterminate(arrangementId, "Approve");
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        public ActionResult NewAccount()
        {
            try
            {
                string sql = "select [FullName],[Category],[PayInAccount],[Term],[EffectiveDate],[MaturityDate],[PayInAmount] from ForNonSterlings";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                DataSet ds = c.Select("rec");
                DataTable records = ds.Tables[0];

                if (records.Rows.Count > 0)
                {
                    return View(records);
                }
                else
                {
                    //Mylogger.Error("No records to display for, " + appUser.FullName + " of branch: " + appUser.Bra_code);
                    ViewBag.Response = "No records to display";
                    return View();
                    //return RedirectToAction("RelationshipOfficer", "RelationshipOfficer");
                }
            }
            catch (Exception)
            {
                ViewBag.Response = "Error Occurred";
                return View();

            }


        }

        [HttpPost]
        public void ExportNewAccount()
        {
            string sql = "select [FullName],[Category],[PayInAccount],[Term],[EffectiveDate],[MaturityDate],[PayInAmount] from ForNonSterlings";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];

            var grid = new GridView();
            grid.DataSource = records;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=NewAccount.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            grid.RenderControl(hw);
            Response.Write(sw.ToString());
            Response.End();

        }



        public ActionResult NewAccountNoSub()
        {
            try
            {
                string sql = "select [FullName],[Gender],[MobileNumber],[Email],[BVN],[DateOfBirth],[State],[HomeAddress],[PayInAccount] from ForNonSterlings where ARRANGEMENT_ID IS NULL";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                DataSet ds = c.Select("rec");
                DataTable records = ds.Tables[0];
                if (records.Rows.Count > 0)
                {
                    return View(records);
                }
                else
                {
                    ViewBag.Response = "No records to display";
                    return View();
                }
            }
            catch (Exception)
            {

                ViewBag.Response = "Error Occurred";
                return View();
            }

        }

        [HttpPost]
        public void ExportNewAccountNoSub()
        {
            string sql = "select [FullName],[Gender],[MobileNumber],[Email],[BVN],[DateOfBirth],[State],[HomeAddress],[PayInAccount] from ForNonSterlings where ARRANGEMENT_ID IS NULL";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];

            var grid = new GridView();
            grid.DataSource = records;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=NewAccountNoSub.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            grid.RenderControl(hw);
            Response.Write(sw.ToString());
            Response.End();

            //return "";
        }

        public ActionResult Diaspora()
        {
            try
            {
                string sql = "select [FullName],[Gender],[MobileNumber],[Email],[BVN],[DateOfBirth],[State],[HomeAddress],[PayInAccount] from ForNonSterlings where ContactPerson IS NOT NULL";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                DataSet ds = c.Select("rec");
                DataTable records = ds.Tables[0];
                if (records.Rows.Count > 0)
                {
                    return View(records);
                }
                else
                {
                    ViewBag.Response = "No records to display";
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Response = "Error Occurred";
                return View();
            }

        }
        [HttpPost]
        public void ExportDiaspora()
        {
            string sql = "select [FullName],[Gender],[MobileNumber],[Email],[BVN],[DateOfBirth],[State],[HomeAddress],[PayInAccount] from ForNonSterlings where ContactPerson IS NOT NULL";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];

            var grid = new GridView();
            grid.DataSource = records;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Diaspora.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            grid.RenderControl(hw);
            Response.Write(sw.ToString());
            Response.End();

        }

        public ActionResult TreatedFuture()
        {
            try
            {
                string sql = "select [FullName],[Category],[PayInAccount],[Term],[DateOfEntry],[EffectiveDate],[PayInAmount] from For_Later where ARRANGEMENT_ID IS NOT NULL AND RTRIM(LTRIM(ReferenceID)) NOT IN ('','NULL','false')";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                DataSet ds = c.Select("rec");
                DataTable records = ds.Tables[0];

                if (records.Rows.Count > 0)
                {
                    return View(records);
                }
                else
                {
                    //Mylogger.Error("No records to display for, " + appUser.FullName + " of branch: " + appUser.Bra_code);
                    ViewBag.Response = "No records to display";
                    return View();
                    //return RedirectToAction("RelationshipOfficer", "RelationshipOfficer");
                }
            }
            catch (Exception)
            {

                ViewBag.Response = "Error Occurred";
                return View();
            }
        }

        [HttpPost]
        public void ExportTreatedFuture()
        {
            string sql = "select [FullName],[Category],[PayInAccount],[Term],[DateOfEntry],[EffectiveDate],[PayInAmount] from For_Later where ARRANGEMENT_ID IS NOT NULL AND RTRIM(LTRIM(ReferenceID)) NOT IN ('','NULL','false')";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];

            var grid = new GridView();
            grid.DataSource = records;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=FutureSubscription.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            grid.RenderControl(hw);
            Response.Write(sw.ToString());
            Response.End();

        }

        public ActionResult FutureDate()
        {
            try
            {
                string sql = "select [FullName],[Category],[PayInAccount],[Term],[DateOfEntry],[EffectiveDate],[PayInAmount] from For_Later where ARRANGEMENT_ID is NULL OR ReferenceID = 'false' OR RTRIM(LTRIM(ReferenceID)) = ''";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                DataSet ds = c.Select("rec");
                DataTable records = ds.Tables[0];

                if (records.Rows.Count > 0)
                {
                    return View(records);
                }
                else
                {
                    //Mylogger.Error("No records to display for, " + appUser.FullName + " of branch: " + appUser.Bra_code);
                    ViewBag.Response = "No records to display";
                    return View();
                    //return RedirectToAction("RelationshipOfficer", "RelationshipOfficer");
                }
            }
            catch (Exception)
            {

                ViewBag.Response = "Error Occurred";
                return View();
            }


        }

        [HttpPost]
        public void ExportFutureDate()
        {
            string sql = "select [FullName],[Category],[PayInAccount],[Term],[DateOfEntry],[EffectiveDate],[PayInAmount] from For_Later  where ARRANGEMENT_ID is NULL OR ReferenceID = 'false' OR RTRIM(LTRIM(ReferenceID)) = ''";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];

            var grid = new GridView();
            grid.DataSource = records;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=FutureSubscription.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            grid.RenderControl(hw);
            Response.Write(sw.ToString());
            Response.End();

        }

        public ActionResult doubbleNoSub()
        {
            string sql = "select [FirstName],[LastName],[Email],[MobileNumber],[BVN],[DateOfBirth],[DateOfEntry],[SterlingVerified],[ValCode] from DoubbleRequests where ValCode IS NOT NULL";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];
            if (records.Rows.Count > 0)
            {
                for (int i = 0; i < records.Rows.Count; i++)
                {
                    if (checkExist(records.Rows[0]["ValCode"].ToString()))
                    {
                        records.Rows[i].Delete();
                    }
                }
                //records.Columns.Remove("ValCode");
                records.Columns.RemoveAt(records.Rows.Count - 1);
                records.AcceptChanges();
                return View(records);
            }
            else
            {
                ViewBag.Response = "No records to display";
                return View();
            }
        }

        [HttpPost]
        public void ExportdoubbleNoSub()
        {
            string sql = "select [FirstName],[LastName],[Email],[MobileNumber],[BVN],[DateOfBirth],[DateOfEntry],[SterlingVerified],[ValCode] from DoubbleRequests where ValCode IS NOT NULL";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            DataTable records = ds.Tables[0];

            for (int i = 0; i < records.Rows.Count; i++)
            {
                if (checkExist(records.Rows[0]["ValCode"].ToString()))
                {
                    records.Rows[i].Delete();
                }
            }
            records.Columns.RemoveAt(records.Rows.Count - 1);
            records.AcceptChanges();


            var grid = new GridView();
            grid.DataSource = records;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=NoSubscription.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            grid.RenderControl(hw);
            Response.Write(sw.ToString());
            Response.End();

        }

        public bool checkExist(string valc)
        {
            bool resp = false;
            try
            {
                string sql = "select * from ForSterlings where ValCode = @valc";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@valc", valc);
                DataTable ds = c.Select("rec").Tables[0];
                if (ds.Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                string sql = "select * from ForNonSterlings where ValCode = @valc";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@valc", valc);
                DataTable ds = c.Select("rec").Tables[0];
                if (ds.Rows.Count > 0)
                {
                    return true;
                }

            }
            try
            {
                string sql = "select * from ForNonSterlings where ValCode = @valc";
                Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
                c.SetSQL(sql);
                c.AddParam("@valc", valc);
                DataTable ds = c.Select("rec").Tables[0];
                if (ds.Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {


            }
            return resp;
        }

        public ActionResult ChangeRole()
        {
            string sql = "select * from Roles";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            var mydata = ds.Tables[0].AsEnumerable().Select(d => new Roles { RoleName = d.Field<string>("Role") });
            var getRoleslist = mydata.ToList();
            SelectList list = new SelectList(getRoleslist, "RoleName", "RoleName");
            ViewBag.roles = list;
            Session["roles"] = list;
            return View();
        }

        [HttpPost]
        public ActionResult ChangeRole(FormCollection fm)
        {
            //string uname = userr.checkUserName(fm["Username"]);
            if (!userr.checkUserName(fm["Username"]))
            {
                
                ViewBag.message = "Invalid User Selected";
                ViewBag.roles = Session["roles"];
                return View();
            }
            else
            {
                if (g.checkExisting(fm["Username"]))
                {
                    Mylogger.Info(Session["Username"] + " changed "+ fm["Username"] + "'s role to " + fm["Role"]);
                    updateRole(fm["Username"], fm["Role"]);
                    ViewBag.message = "User's Role Changed Successfully";
                    ViewBag.roles = Session["roles"];
                    return View();
                }
                else
                {

                    ViewBag.message = "User Does Not Exist";
                    ViewBag.roles = Session["roles"];
                    return View();
                }
            }
        }


        public void updateRole(string username, string role)
        {
            string sql = "update Users_tbl set Role=@role where Username=@username";
            Connect c = new Connect("msconn");
            c.SetSQL(sql);
            c.AddParam("@username", username);
            c.AddParam("@role", role);
            c.Select("rec");
            //Mylogger.Info("Record for BVN, " + refId + " was saved to ForNonSterlings table completely.");
        }


        public ActionResult SuperAmin()
        {
            string sql = "select * from Roles";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            var mydata = ds.Tables[0].AsEnumerable().Select(d => new Roles { RoleName = d.Field<string>("Role") });
            var getRoleslist = mydata.ToList();
            SelectList list = new SelectList(getRoleslist, "RoleName", "RoleName");
            ViewBag.roles = list;
            Session["roles"] = list;
            return View();
        }

        [HttpPost]
        public ActionResult SuperAmin(FormCollection fm)
        {
            //string uname = userr.checkUserName(fm["Username"]);
            if (!userr.checkUserName(fm["Username"]))
            {
                ViewBag.message = "Invalid User Selected";
                ViewBag.roles = Session["roles"];
                return View();
            }
            else
            {
                if (g.checkExisting(fm["Username"]))
                {
                    ViewBag.message = "User Already Exists";
                    ViewBag.roles = Session["roles"];
                    return View();
                }
                else
                {
                    Mylogger.Info(Session["Username"] + " created " + fm["Username"] + " as " + fm["Role"]);
                    var usser = new Users_tbl { CreatedDate = DateTime.Now, Role = fm["Role"], Username = fm["Username"] };
                    db.Users_tbl.Add(usser);
                    db.SaveChanges();
                    ViewBag.message = "User Created";
                    ViewBag.roles = Session["roles"];
                    return View();
                }
            }
        }

    }
}