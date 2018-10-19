using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace MMWS
{


    public class Connect2
    {
        public SqlConnection conn;
        public SqlCommand cmd;
        public int num_rows;
        public int returnValue;
        public Connect2(string query)
        {
            try
            {
                conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
        }

        public DataSet select()
        {
            SqlDataAdapter res = new SqlDataAdapter();
            res.SelectCommand = this.cmd;
            res.TableMappings.Add("Table", "recs");
            DataSet ds = new DataSet();
            res.Fill(ds);
            num_rows = ds.Tables["recs"].Rows.Count;
            this.close();
            return ds;
        }
        public int delete()
        {
            int j = 0;
            try
            {
                j = cmd.ExecuteNonQuery();
                close();
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
            return j;
        }
        public int update()
        {
            int j = 0;
            try
            {
                j = cmd.ExecuteNonQuery();
                close();
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
            return j;
        }

        public int insert()
        {
            this.cmd.CommandText += "; select @@IDENTITY ";
            int j = 0;
            try
            {
                j = Convert.ToInt32(cmd.ExecuteScalar());
                close();
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
                new ErrorLog(cmd.CommandText);
            }
            return j;
        }

        public DataSet query(string tblName)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter res = new SqlDataAdapter();
                res.SelectCommand = cmd;
                res.TableMappings.Add("Table", tblName);
                res.Fill(ds);
                num_rows = ds.Tables[tblName].Rows.Count;
                close();
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
            return ds;
        }
        public void addparam(string key, object val)
        {
            try
            {
                SqlParameter prm = this.cmd.Parameters.AddWithValue(key, val);
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
        }

        public int query()
        {
            SqlParameter prm = new SqlParameter();
            prm.SqlDbType = SqlDbType.Int;
            prm.Direction = ParameterDirection.ReturnValue;
            this.cmd.Parameters.Add(prm);
            returnValue = 0;
            int j = 0;
            try
            {
                j = cmd.ExecuteNonQuery();
                returnValue = Convert.ToInt32(prm.Value);
                close();
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
            return j;
        }
        public void close()
        {
            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
        }
        public string selectScalar()
        {
            string j = "";
            try
            {
                j = Convert.ToString(cmd.ExecuteScalar());
                close();
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
            return j;
        }
    }
}
