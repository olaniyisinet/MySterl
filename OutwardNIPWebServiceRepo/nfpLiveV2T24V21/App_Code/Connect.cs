using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;

public class Connect
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
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["nfp"].ConnectionString;
            //conn.Open();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.StoredProcedure;
        }
        catch (Exception ex)
        {
            //ErrorLog e = new ErrorLog(ex);
            Mylogger.Error(ex);
        }
    }


    public Connect(string query, bool sql)
    {
        try
        {
            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["nfp"].ConnectionString;
            //conn.Open();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
        }
        catch (Exception ex)
        {
            //ErrorLog e = new ErrorLog(ex);
            Mylogger.Error(ex);
        }
    }
    public DataSet query(string tblName)
    {
        DataSet ds = new DataSet();
        num_rows = 0;
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
            //ErrorLog e = new ErrorLog(ex);
            Mylogger.Error(ex);
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
            //ErrorLog e = new ErrorLog(ex);
            Mylogger.Error(ex);
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
            //ErrorLog e = new ErrorLog("Error occured " + ex);
            Mylogger.Error("Error occured ", ex);
        }
        return j;
    }
    public object insert()
    {
        cmd.CommandText += "; select @@IDENTITY ";
        object j = 0;
        try
        {
            j = cmd.ExecuteScalar();
            close();
        }
        catch (Exception ex)
        {
            //ErrorLog e = new ErrorLog(ex);
            Mylogger.Error(ex);
        }
        return j;
    }
    public void close()
    {
        try
        {
            conn.Close();
            conn.Dispose();
        }
        catch (Exception ex)
        {
            //ErrorLog e = new ErrorLog(ex);
            Mylogger.Error(ex);
        }
    }
}
