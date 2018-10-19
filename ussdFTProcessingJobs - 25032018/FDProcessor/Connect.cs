﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDProcessor
{
    class Connect
    {
        public SqlConnection conn;
        public SqlCommand cmd;
        public int returnValue;
        public Connect(string query, bool a, bool b)
        {
            try
            {
                conn = new SqlConnection();
                conn.ConnectionString = "Data Source=10.0.41.101;Initial Catalog=IBSWebserviceDBv2;Persist Security Info=True;User ID=sa;Password=tylent";
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
        public Connect(string query, bool sql, bool c, bool b)
        {
            try
            {
                conn = new SqlConnection();
                conn.ConnectionString = "Data Source=10.0.41.188;Initial Catalog=proxy;Persist Security Info=True;User ID=test;Password=test";
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
            }
            catch (Exception ex)
            {
                new Errorlog("Error occured while connecting to INFOBIP db" + ex.ToString());
            }
        }
        public Connect(string query)
        {
            try
            {
                conn = new SqlConnection();
                conn.ConnectionString = "Data Source=10.0.41.101;Initial Catalog=MobileMoney;User ID=sa;Password=tylent";
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
                conn.ConnectionString = "Data Source=10.0.41.101;Initial Catalog=MobileMoney;User ID=sa;Password=tylent";
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
                Errorlog e = new Errorlog(ex);
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
                Errorlog e = new Errorlog(ex);
            }
        }
    }
}
