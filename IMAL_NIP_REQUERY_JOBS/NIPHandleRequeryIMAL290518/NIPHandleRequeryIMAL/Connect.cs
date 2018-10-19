using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;


namespace NIPHandleRequeryIMAL
{
    class Connect
    {
        public SqlConnection conn;
        public SqlCommand cmd;
        public int returnValue;
        public int num_rows;
        public Connect(string query)
        {
            try
            {
                conn = new SqlConnection();
                conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["mssqlconn"];
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.CommandType = CommandType.StoredProcedure;
            }
            catch (Exception ex)
            {
                Errorlog e = new Errorlog(ex);
            }
        }
        public Connect(string query, bool sql)
        {
            try
            {
                conn = new SqlConnection();
                conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["mssqlconn"];
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
            }
            catch (Exception ex)
            {
                Errorlog e = new Errorlog(ex);
            }
        }
        public DataSet query(string tblName)
        {
            DataSet ds = new DataSet();
            num_rows = -1;
            try
            {
                SqlDataAdapter res = new SqlDataAdapter();
                res.SelectCommand = cmd;
                res.TableMappings.Add("Table", tblName);
                res.Fill(ds);
                num_rows = ds.Tables[0].Rows.Count;
                close();
            }
            catch (Exception ex)
            {
                Errorlog e = new Errorlog(ex);
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
                Errorlog e = new Errorlog(ex);
            }
        }
        public int query()
        {
            int j = 0;
            try
            {
                SqlParameter prm = new SqlParameter();
                prm.SqlDbType = SqlDbType.Int;
                prm.Direction = ParameterDirection.ReturnValue;
                this.cmd.Parameters.Add(prm);


                j = cmd.ExecuteNonQuery();
                returnValue = Convert.ToInt32(prm.Value);
            }
            catch
            {

            }
            close();
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
                Errorlog e = new Errorlog(ex);
            }
        }
    }
}
