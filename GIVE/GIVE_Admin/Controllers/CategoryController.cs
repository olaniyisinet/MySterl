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
    public class CategoryController : Controller
    {
		string connString = ConfigurationManager.ConnectionStrings["dbConn"].ToString();
		// GET: Category
		public ActionResult Index()
        {
			try
			{
				using (SqlConnection conn = new SqlConnection(connString))
				{
					string query = "select * from Categories";
					DataTable dt = new DataTable();
					conn.Open();
					SqlDataAdapter da = new SqlDataAdapter(query, conn);
					da.Fill(dt);
					conn.Close();
					int count = dt.Rows.Count;

					IList<CategoryModel> model = new List<CategoryModel>();

					if (count < 1)
					{
						ViewBag.Message = "No Category(s) Found";
						return View();
					}
					else
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							model.Add(new CategoryModel()
							{
								ID = int.Parse(dt.Rows[i]["Id"].ToString()),
								Name = dt.Rows[i]["Name"].ToString(),
								Description = dt.Rows[i]["Description"].ToString(),
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

		// GET: Category/Details/5
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

		// GET: Category/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Category/Create
		[HttpPost]
		public ActionResult Create(FormCollection collection)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(connString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand();

					cmd.Connection = conn;
					cmd.CommandText = "Insert into Categories(Name,Description) values(@Name,@Description)";
					cmd.Parameters.AddWithValue("@Name", collection["Name"].ToString());
					cmd.Parameters.AddWithValue("@Description", collection["Description"].ToString());
					
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
			catch (Exception ex)
			{
				ViewBag.Message = ex.Message;
				return View();
			}
		}

		// GET: Category/Edit/5
		[HttpGet]
		public ActionResult Edit(int id)
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

		// POST: Category/Edit/5
		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
				try
				{
					using (SqlConnection conn = new SqlConnection(connString))
					{
						conn.Open();
						SqlCommand cmd = new SqlCommand();
					
						cmd.Connection = conn;
						cmd.CommandText = "Update Categories set Name=@Name, Description=@Description where Id =" + id;
						cmd.Parameters.AddWithValue("@Name", collection["Name"].ToString());
						cmd.Parameters.AddWithValue("@Description", collection["Description"].ToString());
					
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
					ViewBag.Message = ex.Message;
				ErrorLog.LogException(ex);
					return View();
				}
			}

		// GET: Category/Delete/5
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

		// POST: Category/Delete/5
		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(connString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand();

					cmd.Connection = conn;
					cmd.CommandText = "delete from Categories where Id =" + id;
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

		public CategoryModel getOne(int id)
		{
			CategoryModel model = new CategoryModel();
			try
			{
				using (SqlConnection conn = new SqlConnection(connString))
				{
					string query = "select * from Categories where Id =" + id;
					conn.Open();
				//	SqlDataAdapter da = new SqlDataAdapter(query, conn);

					SqlCommand command = new SqlCommand(query, conn);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							//Console.WriteLine(String.Format("{0}", reader["id"]));
							model.ID = int.Parse(reader["Id"].ToString());
							model.Name = reader["Name"].ToString();
							model.Description = reader["Description"].ToString();
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

				model.Name = "";
				model.Description = "";

				return model;
			}
		}

	}
}
