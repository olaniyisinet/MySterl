using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using GIVE_Admin.Helpers;

namespace NIB_EChannels.Helpers
{
	public class Counter
	{
		public static int getTotalProjects()
		{
			int total = 0;
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select count(*) from Projects" ;
					conn.Open();

					SqlCommand command = new SqlCommand(query, conn);

					total = (Int32)command.ExecuteScalar();

					conn.Close();

					return total;
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				return total;
			}
		}

		public static int getTotalActiveProjects()
		{
			int total = 0;
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select count(*) from Projects where Status= 'Active' ";
					conn.Open();

					SqlCommand command = new SqlCommand(query, conn);

					total = (Int32)command.ExecuteScalar();

					conn.Close();

					return total;
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				return total;
			}
		}

		public static int getTotalInActiveProjects()
		{
			int total = 0;
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select count(*) from Projects where Status= 'Inactive' ";
					conn.Open();

					SqlCommand command = new SqlCommand(query, conn);

					total = (Int32)command.ExecuteScalar();

					conn.Close();

					return total;
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				return total;
			}
		}

		public static int getTotalNGOs()
		{
			int total = 0;
			try
			{
				using (SqlConnection conn = new SqlConnection(Getter.Getdbcon()))
				{
					string query = "select count(*) from NGOs";
					conn.Open();

					SqlCommand command = new SqlCommand(query, conn);

					total = (Int32)command.ExecuteScalar();

					conn.Close();

					return total;
				}
			}
			catch (Exception ex)
			{
				ErrorLog.LogException(ex);
				return total;
			}
		}
	}
}