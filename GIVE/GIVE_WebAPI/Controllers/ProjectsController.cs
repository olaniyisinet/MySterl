using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using GIVE_WebAPI.Helpers;
using GIVE_WebAPI.Models;

namespace GIVE_WebAPI.Controllers
{
	public class ProjectsController : ApiController
	{
		// GET api/Projects
		public List<ProjectsModel> Get(int? id = null)
		{
			List<ProjectsModel> model = new List<ProjectsModel>();
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
				sqlCmd.CommandText = "Select * from Projects where Id=" + id + "";
			else
				sqlCmd.CommandText = "Select * from Projects ";

			sqlCmd.Connection = myConnection;
			myConnection.Open();
			reader = sqlCmd.ExecuteReader();

			while (reader.Read())
			{
				ProjectsModel proj = new ProjectsModel
				{
					Id = int.Parse(reader["Id"].ToString()),
					Name = reader["Name"].ToString(),
					Description = reader["Description"].ToString(),
					Category = reader["Category"].ToString(),
					TargetAmount = reader["Target_Amount"].ToString(),
					Duration = reader["Duration"].ToString(),
					EndDate = reader["Expiry_Date"].ToString(),
					Image = reader["ImagePath"].ToString(),
					Status = reader["Status"].ToString()
				};

				model.Add(proj);
			}
			myConnection.Close();
			return model;
		}
	}
}