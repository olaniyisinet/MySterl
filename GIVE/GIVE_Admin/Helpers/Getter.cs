using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GIVE_Admin.Helpers;

namespace GIVE_Admin.Helpers
{
	public class Getter
	{
		public static string Getdbcon()
		{
			string connString = ConfigurationManager.ConnectionStrings["dbConn"].ToString();
			return connString;
		}

		public static IList<SelectListItem> GetCategories()
		{
			var item = new List<SelectListItem>();
			try
			{
				using (SqlConnection conn = new SqlConnection(Getdbcon()))
				{
					string query = "select * from Categories";
					DataTable dt = new DataTable();
					conn.Open();
					SqlDataAdapter da = new SqlDataAdapter(query, conn);
					da.Fill(dt);
					conn.Close();
					int count = dt.Rows.Count;

					if (count < 1)
					{
						item.Add(new SelectListItem() { Value = null, Text = "", Selected = true });
						return item;
					}
					else
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							item.Add(new SelectListItem
							{
								Value = dt.Rows[i]["Name"].ToString(),
								Text = dt.Rows[i]["Name"].ToString(),
							});
						}
						return item;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				item.Add(new SelectListItem
				{
					Value = null,
					Text = "",
				});

				return item;
			}
		}
		
		public static IList<SelectListItem> GetProjects()
		{
			var item = new List<SelectListItem>();
			try
			{
				using (SqlConnection conn = new SqlConnection(Getdbcon()))
				{
					string query = "select * from Projects";
					DataTable dt = new DataTable();
					conn.Open();
					SqlDataAdapter da = new SqlDataAdapter(query, conn);
					da.Fill(dt);
					conn.Close();
					int count = dt.Rows.Count;

					if (count < 1)
					{
						item.Add(new SelectListItem() { Value = null, Text = "", Selected = true });
						return item;
					}
					else
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							item.Add(new SelectListItem
							{
								Value = dt.Rows[i]["Name"].ToString(),
								Text = dt.Rows[i]["Name"].ToString(),
							});
						}
						return item;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				item.Add(new SelectListItem
				{
					Value = null,
					Text = "",
				});

				return item;
			}
		}

		public static IList<SelectListItem> GetBanks()
		{
			var item = new List<SelectListItem>();
			try
			{
				using (SqlConnection conn = new SqlConnection(Getdbcon()))
				{
					string query = "select * from tbl_banks";
					DataTable dt = new DataTable();
					conn.Open();
					SqlDataAdapter da = new SqlDataAdapter(query, conn);
					da.Fill(dt);
					conn.Close();
					int count = dt.Rows.Count;

					if (count < 1)
					{
						item.Add(new SelectListItem() { Value = null, Text = "", Selected = true });
						return item;
					}
					else
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{
							item.Add(new SelectListItem
							{
								Value = dt.Rows[i]["bank_name"].ToString(),
								Text = dt.Rows[i]["bank_name"].ToString(),
							});
						}
						return item;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				item.Add(new SelectListItem
				{
					Value = null,
					Text = "",
				});

				return item;
			}
		}

		public static string getBankCode(string bankName)
		{
			string code = "";
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select * from tbl_banks where bank_name =" + bankName;
					conn.Open();
					SqlDataAdapter da = new SqlDataAdapter(query, conn);

					SqlCommand command = new SqlCommand(query, conn);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							code = reader["bank_code"].ToString();
						}
					}

					conn.Close();

					return code;
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
								return code;
			}		}


		public static IEnumerable<SelectListItem> GetSelectedProjects(string selectedProject)
		{
			var item = new List<SelectListItem>();
			try
			{
				using (SqlConnection conn = new SqlConnection(Getdbcon()))
				{
					string[] projects = selectedProject.Split(',');

						int count = projects.Length;

					if (count < 1)
					{
						item.Add(new SelectListItem() { Value = null, Text = "", Selected = true });
						return item;
					}
					else
					{
						for (int i = 0; i < count; i++)
						{
							item.Add(new SelectListItem
							{
								Value = projects[i].ToString(),
								Text = projects[i].ToString(),
							});
						}
						return item;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				item.Add(new SelectListItem
				{
					Value = null,
					Text = "",
				});

				return item;
			}
		}
	}
}