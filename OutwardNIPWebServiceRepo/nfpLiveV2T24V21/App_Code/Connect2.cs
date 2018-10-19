using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Configuration;

/// <summary>
/// Summary description for Connect
/// </summary>
public class Connect2
{
    public SqlConnection conn;
    public SqlCommand cmd;
    public int returnValue;
    public int num_rows;
    public Connect2(string query)
    {
        try
        {
            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationSettings.AppSettings["mssqlconn"].ToString();
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.StoredProcedure;
        }
        catch (Exception ex)
        {
        }
    }
    public Connect2(string query, bool sql)
    {
        try
        {
            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationSettings.AppSettings["mssqlconn"].ToString();
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
        }
        catch (Exception ex)
        {
        }
    }

    public Connect2(string query, bool sql, bool c)
    {
        try
        {
            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationSettings.AppSettings["mssqlconn"].ToString();
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
        }
        catch (Exception ex)
        {

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
        }
    }
    public int insert()
    {
        this.cmd.CommandText += "; select @@IDENTITY ";
        int j = 0;
        try
        {
            j = Convert.ToInt32(cmd.ExecuteScalar());
            num_rows = j;
            close();
        }
        catch (Exception ex)
        {
        }
        return j;
    }
}