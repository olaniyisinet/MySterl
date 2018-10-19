using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using GIVE_Admin.Helpers;
using GIVE_Admin.Models;

namespace GIVE_Admin.Controllers
{
    public class NGOController : Controller
    {
        // GET: NGO
        public ActionResult Index()
        {

			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select * from NGOs";
					DataTable dt = new DataTable();
					conn.Open();
					SqlDataAdapter da = new SqlDataAdapter(query, conn);
					da.Fill(dt);
					conn.Close();
					int count = dt.Rows.Count;

					IList<NGOModel> model = new List<NGOModel>();

					if (count < 1)
					{
						ViewBag.Message = "No NOG(s) Found";
						return View();
					}
					else
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							model.Add(new NGOModel()
							{
								Id = int.Parse(dt.Rows[i]["Id"].ToString()),
								Name = dt.Rows[i]["Name"].ToString(),
								YearFound = dt.Rows[i]["Year_Found"].ToString(),
								Motto = dt.Rows[i]["Motto"].ToString(),
								Email = dt.Rows[i]["Email"].ToString(),
								Phone = dt.Rows[i]["Phone"].ToString(),
								Address = dt.Rows[i]["Address"].ToString(),
								About = dt.Rows[i]["About"].ToString(),
								Projects = dt.Rows[i]["Projects"].ToString(),
								Bank = dt.Rows[i]["BankName"].ToString(),
								AccountNumber = dt.Rows[i]["AccountNumber"].ToString(),
								Category = dt.Rows[i]["Category"].ToString(),
								ImagePath = dt.Rows[i]["ImagePath"].ToString()
							});
						}
					}
					return View(model);
				}
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				ErrorLog.LogException(ex);
				return View();
			}
		}

        // GET: NGO/Details/5
        public ActionResult Details(int id)
        {
			try
			{
				return View(getOne(id));
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				ErrorLog.LogException(ex);
				return View();
			}
		}

        // GET: NGO/Create
        public ActionResult Create()
        {
			try
			{
				NGOModel model = new NGOModel();
				{
					model.Categories = Getter.GetCategories();
					model.AllProjects = Getter.GetProjects();
					model.AllBanks = Getter.GetBanks();
				}
				return View(model);
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				ErrorLog.LogException(ex);
				return View();
			}
		}

        // POST: NGO/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, HttpPostedFileBase file)
        {
			var absolutePath = "";
			string path = "";
			try
			{
				try
				{
					 path = Path.Combine(Server.MapPath("~/Images/"), Path.GetFileName(file.FileName));
					file.SaveAs(path);
					string filePath = "~/Images/" + file.FileName;

				 absolutePath = HttpContext.Server.MapPath(filePath);
				}
				catch (Exception ex) {
					absolutePath = path;
					ErrorLog.LogException(ex);
				}
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand();

					cmd.Connection = conn;
					cmd.CommandText = "Insert into NGOs(Name,Year_Found,Motto,Email,Phone,Address,About,Projects,BankName,BankCode,AccountNumber,Category,ImagePath) values(@Name,@Year_Found,@Motto,@Email,@Phone,@Address,@About,@Projects,@BankName,@BankCode,@AccountNumber,@Category,@ImagePath)";
					cmd.Parameters.AddWithValue("@Name", collection["Name"].ToString());
					cmd.Parameters.AddWithValue("@Year_Found", collection["YearFound"].ToString());
					cmd.Parameters.AddWithValue("@Motto", collection["Motto"].ToString());
					cmd.Parameters.AddWithValue("@Email", collection["Email"].ToString());
					cmd.Parameters.AddWithValue("@Phone", collection["Phone"].ToString());
					cmd.Parameters.AddWithValue("@Address", collection["Address"].ToString());
					cmd.Parameters.AddWithValue("@About", collection["About"].ToString());
					cmd.Parameters.AddWithValue("@Projects", collection["Projects"].ToString());
					cmd.Parameters.AddWithValue("@BankName", collection["Bank"].ToString());
					cmd.Parameters.AddWithValue("@BankCode", Getter.getBankCode(collection["Bank"].ToString()));
					cmd.Parameters.AddWithValue("@AccountNumber", collection["AccountNumber"].ToString());
					cmd.Parameters.AddWithValue("@Category", collection["Category"].ToString());
					cmd.Parameters.AddWithValue("@ImagePath", absolutePath);

					try
					{
						cmd.ExecuteNonQuery();
						ViewBag.Message = "Record Added Successfully";
						return RedirectToAction("Index");
					}
					catch (Exception ex)
					{
						ErrorLog.LogException(ex);
						ViewBag.Message = ex;
						return View() ;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				ViewBag.Message = ex;
				return View();
			}
		}

        // GET: NGO/Edit/5
        public ActionResult Edit(int id)
        {
			try
			{				
					ViewBag.Categories = Getter.GetCategories();
					ViewBag.AllProjects = Getter.GetProjects();
					ViewBag.AllBanks = Getter.GetBanks();
				
				return View(getOne(id));
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				ErrorLog.LogException(ex);
				return View();
			}
		}

        // POST: NGO/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection, HttpPostedFileBase file)
        {
			var absolutePath = "";
			string path = "";
			try
			{
				try
				{
					path = Path.Combine(Server.MapPath("~/Images/NGO/"), Path.GetFileName(collection["ImagePath"].ToString()));
					file.SaveAs(path);
					string filePath = "~/Images/NGO/" + Path.GetFileName(collection["ImagePath"].ToString());
					absolutePath = HttpContext.Server.MapPath(filePath);
				}
				catch (Exception ex)
				{
					ErrorLog.LogException(ex);
					absolutePath = path;
				}
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand();

					cmd.Connection = conn;
					cmd.CommandText = "Update NGOs set Name=@Name,Year_Found=@Year_Found,Motto=@Motto,Email=@Email,Phone=@Phone,Address=@Address,About=@About,Projects=@Projects,BankName=@BankName,BankCode=@BankCode,AccountNumber=@AccountNumber,ImagePath=@ImagePath where Id = " + id;
					cmd.Parameters.AddWithValue("@Name", collection["Name"].ToString());
					cmd.Parameters.AddWithValue("@Year_Found", collection["YearFound"].ToString());
					cmd.Parameters.AddWithValue("@Motto", collection["Motto"].ToString());
					cmd.Parameters.AddWithValue("@Email", collection["Email"].ToString());
					cmd.Parameters.AddWithValue("@Phone", collection["Phone"].ToString());
					cmd.Parameters.AddWithValue("@Address", collection["Address"].ToString());
					cmd.Parameters.AddWithValue("@About", collection["About"].ToString());
					cmd.Parameters.AddWithValue("@Projects", collection["SelectedProjects"].ToString());
					cmd.Parameters.AddWithValue("@BankName", collection["Bank"].ToString());
					cmd.Parameters.AddWithValue("@BankCode", Getter.getBankCode(collection["Bank"].ToString()));
					cmd.Parameters.AddWithValue("@AccountNumber", collection["AccountNumber"].ToString());
					cmd.Parameters.AddWithValue("@Category", collection["Category"].ToString());
					cmd.Parameters.AddWithValue("@ImagePath", absolutePath);

					try
					{
						cmd.ExecuteNonQuery();
						ViewBag.Message = "Record Updated Successfully";
					}
					catch (Exception ex)
					{
						ErrorLog.LogException(ex);
						ViewBag.Message = ex.Message;
					}
				}

				return RedirectToAction("Index");

			}
			catch (Exception ex)
			{
				ViewBag.category = Getter.GetCategories();

				ViewBag.Message = ex.Message;
				return View();
			}
		}

        // GET: NGO/Delete/5
        public ActionResult Delete(int id)
        {
			try
			{
				return View(getOne(id));
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				ErrorLog.LogException(ex);
				return View();
			}
		}

        // POST: NGO/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand();

					cmd.Connection = conn;
					cmd.CommandText = "delete from NGOs where Id =" + id;
					try
					{
						cmd.ExecuteNonQuery();
						ViewBag.Message = "Record Deleted Successfully";
					}
					catch (Exception ex)
					{
						ErrorLog.LogException(ex);
						ViewBag.Message = ex.Message;
					}
				}
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				return View();
			}
		}

		public NGOModel getOne(int id)
		{
			NGOModel model = new NGOModel();
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select * from NGOs where Id =" + id;
					conn.Open();

					SqlCommand command = new SqlCommand(query, conn);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							//Console.WriteLine(String.Format("{0}", reader["id"]));

							model.Id = int.Parse(reader["Id"].ToString());
							model.Name = reader["Name"].ToString();
							model.YearFound = reader["Year_Found"].ToString();
							model.Motto = reader["Motto"].ToString();
							model.Email = reader["Email"].ToString();
							model.Phone = reader["Phone"].ToString();
							model.Address = reader["Address"].ToString();
							model.About = reader["About"].ToString();
							model.Projects = reader["Projects"].ToString();
							model.Bank = reader["BankName"].ToString();
							model.AccountNumber = reader["AccountNumber"].ToString();
							model.Category = reader["Category"].ToString();
							model.ImagePath = reader["ImagePath"].ToString();
							model.Categories = Getter.GetCategories();
							model.AllProjects = Getter.GetProjects();
							model.AllBanks = Getter.GetBanks();
							model.SelectedProjects = reader["Projects"].ToString().Split(',');
						}
					}

					conn.Close();

					return model;
				}
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				ErrorLog.LogException(ex);

				model.Id = 0;
				model.Name ="";
				model.YearFound = "";
				model.Motto = "";
				model.Email ="";
				model.Phone = "";
				model.Address = "";
				model.About = "";
				model.Projects = "";
				model.Bank = "";
				model.AccountNumber = "";
				model.Category = "";
				model.ImagePath ="";
			return model;
			}
		}
	}
}
