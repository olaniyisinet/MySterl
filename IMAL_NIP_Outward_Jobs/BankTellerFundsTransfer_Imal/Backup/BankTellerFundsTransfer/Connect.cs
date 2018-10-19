using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;


namespace BankTellerFundsTransfer
{
    class Connect
    {
        public SqlConnection conn;
        public SqlCommand cmd;
        public int returnValue;
        public Connect(string query)
        {
            try
            {
                conn = new SqlConnection();
                //conn.ConnectionString = "Data Source=.;Initial Catalog=nfpdb;User ID=nip2;Password=nip1234";
                conn.ConnectionString = "Data Source=10.0.0.211,1490;Initial Catalog=nfpdb;User ID=etrnx;Password=(*59serv#)";
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.CommandType = CommandType.StoredProcedure;
            }
            catch (Exception ex)
            {
                ErrorLog e = new ErrorLog(ex);
            }
        }
        public Connect(string query, bool sql)
        {
            try
            {
                conn = new SqlConnection();
                //conn.ConnectionString = "Data Source=.;Initial Catalog=nfpdb;User ID=nip2;Password=nip1234";
                conn.ConnectionString = "Data Source=10.0.0.211,1490;Initial Catalog=nfpdb;User ID=etrnx;Password=(*59serv#)";
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
        public DataSet query(string tblName)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter res = new SqlDataAdapter();
                res.SelectCommand = cmd;
                res.TableMappings.Add("Table", tblName);
                res.Fill(ds);
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
    }
}
