using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GIVE_Admin.Helpers;
using GIVE_Admin.Models;

namespace GIVE_Admin.Controllers
{
	public class ProjectsController : Controller
	{
		// GET: Projects
		public ActionResult Index()
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select * from Projects";
					DataTable dt = new DataTable();
					conn.Open();
					SqlDataAdapter da = new SqlDataAdapter(query, conn);
					da.Fill(dt);
					conn.Close();
					int count = dt.Rows.Count;

					IList<ProjectsModel> model = new List<ProjectsModel>();

					if (count < 1)
					{
						ViewBag.Message = "No Project(s) Found";
						return View();
					}
					else
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							model.Add(new ProjectsModel()
							{
								Id = int.Parse(dt.Rows[i]["Id"].ToString()),
								Name = dt.Rows[i]["Name"].ToString(),
								Description = dt.Rows[i]["Description"].ToString(),
								Category = dt.Rows[i]["Category"].ToString(),
								TargetAmount = dt.Rows[i]["Target_Amount"].ToString(),
								Duration = dt.Rows[i]["Duration"].ToString(),
								EndDate = dt.Rows[i]["Expiry_Date"].ToString(),
								Image = dt.Rows[i]["ImagePath"].ToString(),
								Status = dt.Rows[i]["Status"].ToString()
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

		// GET: Projects/Details/5
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

		// GET: Projects/Create
		public ActionResult Create()
		{
			try
			{
				ProjectsModel model = new ProjectsModel();
				{
					model.Categories = Getter.GetCategories();
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

		// POST: Projects/Create
		[HttpPost]
		public ActionResult Create(FormCollection collection, HttpPostedFileBase file)
		{
			try
			{
				string path = Path.Combine(Server.MapPath("~/Images/Projects"), Path.GetFileName(file.FileName));
				file.SaveAs(path);
				string filePath = "~/Images/Projects" + file.FileName;

				var absolutePath = HttpContext.Server.MapPath(filePath);

				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand();

					var dateAndTime = DateTime.Now;
					var date = dateAndTime.Date;
					var exp = date.AddDays(int.Parse(collection["Duration"].ToString()) * 7);

					cmd.Connection = conn;
					cmd.CommandText = "Insert into Projects(Name,Description,Category,Target_Amount,Duration,Expiry_Date,ImagePath,Status) values(@Name,@Description,@Category,@Target_Amount,@Duration,@Expiry_Date,@ImagePath,@Status)";
					cmd.Parameters.AddWithValue("@Name", collection["Name"].ToString());
					cmd.Parameters.AddWithValue("@Description", collection["Description"].ToString());
					cmd.Parameters.AddWithValue("@Category", collection["Category"].ToString());
					cmd.Parameters.AddWithValue("@Target_Amount", Double.Parse(collection["TargetAmount"].ToString()));
					cmd.Parameters.AddWithValue("@Duration", collection["Duration"].ToString());
					cmd.Parameters.AddWithValue("@Expiry_Date", exp);
					cmd.Parameters.AddWithValue("@ImagePath", absolutePath);
					cmd.Parameters.AddWithValue("@Status", collection["Status"].ToString());

					try
					{
						cmd.ExecuteNonQuery();
						ViewBag.Message = "Record Added Successfully";
					}
					catch (Exception ex)
					{
						ErrorLog.LogException(ex);
						ViewBag.Message = ex;
					}
				}

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: Projects/Edit/5
		[HttpGet]
		public ActionResult Edit(int id)
		{
			try
			{
				ViewBag.category = Getter.GetCategories();
				//Session["category"] = GetCategories();
				return View(getOne(id));
			}
			catch (Exception ex)
			{
				ViewBag.category = Getter.GetCategories();

				ViewBag.Message = ex.Message;
				ErrorLog.LogException(ex);
				return View();
			}
		}

		// POST: Projects/Edit/5
		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection, HttpPostedFileBase file)
		{
			var absolutePath = "";
			string path = "";
			try
			{
				try
				{
					path = Path.Combine(Server.MapPath("~/Images/Projects/"), Path.GetFileName(collection["Image"].ToString()));
					file.SaveAs(path);
					string filePath = "~/Images/Projects/" + Path.GetFileName(collection["Image"].ToString());
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

						var dateAndTime = DateTime.Now;
						var date = dateAndTime.Date;
						var exp = date.AddDays(int.Parse(collection["Duration"].ToString()) * 7);

						cmd.Connection = conn;
						cmd.CommandText = "Update Projects set Name=@Name, Description=@Description,Category=@Category,Target_Amount=@Target_Amount,Duration=@Duration,Expiry_Date=@Expiry_Date,ImagePath=@ImagePath,Status=@Status where Id =" + id;
						cmd.Parameters.AddWithValue("@Name", collection["Name"].ToString());
						cmd.Parameters.AddWithValue("@Description", collection["Description"].ToString());
						cmd.Parameters.AddWithValue("@Category", collection["Category"].ToString());
						cmd.Parameters.AddWithValue("@Target_Amount", Double.Parse(collection["TargetAmount"].ToString()));
						cmd.Parameters.AddWithValue("@Duration", collection["Duration"].ToString());
						cmd.Parameters.AddWithValue("@Expiry_Date", exp);
						cmd.Parameters.AddWithValue("@ImagePath", absolutePath);
						///cmd.Parameters.AddWithValue("@ImagePath", "");
						cmd.Parameters.AddWithValue("@Status", collection["Status"].ToString());

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

		// GET: Projects/Delete/5
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

		// POST: Projects/Delete/5
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
					cmd.CommandText = "delete from Projects where Id =" + id;
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

		public ProjectsModel getOne(int id)
		{
			ProjectsModel model = new ProjectsModel();
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select * from Projects where Id =" + id;
					conn.Open();
					SqlDataAdapter da = new SqlDataAdapter(query, conn);

					SqlCommand command = new SqlCommand(query, conn);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							//Console.WriteLine(String.Format("{0}", reader["id"]));

							model.Id = int.Parse(reader["Id"].ToString());
							model.Name = reader["Name"].ToString();
							model.Description = reader["Description"].ToString();
							model.Category = reader["Category"].ToString();
							model.TargetAmount = reader["Target_Amount"].ToString();
							model.Duration = reader["Duration"].ToString();
							model.EndDate = reader["Expiry_Date"].ToString();
							model.Image = reader["ImagePath"].ToString();
							model.Status = reader["Status"].ToString();
							model.Categories = Getter.GetCategories();
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
				model.Name = "";
				model.Description = "";
				model.Category = "";
				model.TargetAmount = "";
				model.Duration = "";
				model.EndDate = "";
				model.Image = "";
				model.Status = "";

				return model;
			}
		}
	}
}