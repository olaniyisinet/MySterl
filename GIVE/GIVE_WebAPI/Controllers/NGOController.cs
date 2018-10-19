using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GIVE_WebAPI.Helpers;
using GIVE_WebAPI.Models;

namespace GIVE_WebAPI.Controllers
{
	public class NGOController : ApiController
	{
		public List<NGOModel> Get(int? id = null)
		{
			List<NGOModel> NGO = new List<NGOModel>();
			SqlDataReader reader = null;
			SqlConnection myConnection = new SqlConnection
			{
				ConnectionString = Getter.Getdbcon()
			};

			SqlCommand sqlCmd = new SqlCommand
			{
				CommandType = CommandType.Text
			};

			if (id != null)
				sqlCmd.CommandText = "Select * from NGOs where Id=" + id + "";
			else
				sqlCmd.CommandText = "Select * from NGOs ";

			sqlCmd.Connection = myConnection;
			myConnection.Open();
			reader = sqlCmd.ExecuteReader();

			while (reader.Read())
			{
				NGOModel model = new NGOModel
				{
					Id = int.Parse(reader["Id"].ToString()),
					Name = reader["Name"].ToString(),
					YearFound = reader["Year_Found"].ToString(),
					Motto = reader["Motto"].ToString(),
					Email = reader["Email"].ToString(),
					Phone = reader["Phone"].ToString(),
					Address = reader["Address"].ToString(),
					About = reader["About"].ToString(),
					Projects = reader["Projects"].ToString(),
					Bank = reader["BankName"].ToString(),
					BankCode = reader["BankCode"].ToString(),
					AccountNumber = reader["AccountNumber"].ToString(),
					Category = reader["Category"].ToString(),
					ImagePath = reader["ImagePath"].ToString(),
					SelectedProjects = reader["Projects"].ToString().Split(',')
				};

				NGO.Add(model);
			}
			myConnection.Close();
			return NGO;
		}
	}
}