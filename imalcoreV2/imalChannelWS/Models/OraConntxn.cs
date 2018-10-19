using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace imalcore
{
    public class OraConntxn
    {
        public OracleConnection conn;
        public OracleCommand cmd;
        public OracleCommand cmd2;
        public OracleTransaction trnx;

        public OraConntxn(string sproc, bool tx)
        {
            try
            {
                conn = new OracleConnection();
                string conn_imal = System.Configuration.ConfigurationManager.AppSettings["conn_imal"];
                conn.ConnectionString = conn_imal;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                //start transaction
                trnx = conn.BeginTransaction();
                cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandText = sproc;
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd2 = new OracleCommand("alter session set nls_date_format='ddmmyyyy'", conn);
            }
            catch (Exception ex)
            {
                Mylogger.Info(" Info connecting to Imal Datasource: " + ex.ToString());
            }
        }

        public DataSet query(string tblName)
        {
            DataSet ds = new DataSet();
            try
            {
                OracleDataAdapter res = new OracleDataAdapter();
                res.SelectCommand = cmd;
                res.TableMappings.Add("Table", tblName);

                res.Fill(ds);
                close();
            }
            catch (Exception ex)
            {
                Mylogger.Info(" Info trying to fill Dataset: " + ex.ToString());
            }
            return ds;
        }

        /// <summary>
        /// Addparams the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The value.</param>
        /// <param name="size">The size.</param>
        public void addparam(string name, string type, ParameterDirection direction, object value, int size)
        {
            try
            {
                OracleParameter prm = new OracleParameter("@" + name, value);
                switch (type)
                {
                    case "string": prm.OracleDbType = OracleDbType.Varchar2; prm.Size = size; break;
                    case "int": prm.OracleDbType = OracleDbType.Int32; prm.Size = size; break;
                    case "float": prm.OracleDbType = OracleDbType.BinaryFloat; prm.Size = size; break;
                    case "datetime": prm.OracleDbType = OracleDbType.TimeStamp; break;
                    case "blob": prm.OracleDbType = OracleDbType.Blob; break;
                    case "decimal": prm.OracleDbType = OracleDbType.Decimal; break;
                    case "curs": prm.OracleDbType = OracleDbType.RefCursor; break;
                    case "date": prm.OracleDbType = OracleDbType.Date; break;
                }
                prm.Direction = direction;
                cmd.Parameters.Add(prm);
                Mylogger.Info(name + ":=" + prm.Value + ";");
            }
            catch (Exception ex)
            {
                Mylogger.Info("Info adding parameters , direction, value, size: " + ex.ToString());
            }
        }

        /// <summary>
        /// Addparams the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The value.</param>
        public void addparam(string name, string type, ParameterDirection direction, object value)
        {
            try
            {
                OracleParameter prm = new OracleParameter("@" + name, value);
                switch (type)
                {
                    case "string": prm.OracleDbType = OracleDbType.Varchar2; break;
                    case "int": prm.OracleDbType = OracleDbType.Int32; break;
                    case "datetime": prm.OracleDbType = OracleDbType.TimeStamp; break;
                    case "blob": prm.OracleDbType = OracleDbType.Blob; break;
                    case "decimal": prm.OracleDbType = OracleDbType.Decimal; break;
                    case "curs": prm.OracleDbType = OracleDbType.RefCursor; break;
                    case "date": prm.OracleDbType = OracleDbType.Date; break;
                    case "float": prm.OracleDbType = OracleDbType.BinaryFloat; break;
                }
                prm.Direction = direction;
                cmd.Parameters.Add(prm);

                //foreach (OracleParameter p in ab)
                //{
                Mylogger.Info(name + ":=" + prm.Value + ";");
                //}
            }
            catch (Exception ex)
            {
                Mylogger.Info("Info adding parameters , direction, value, size: " + ex.ToString());
            }
        }

        /// <summary>
        /// Addparams the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="direction">The direction.</param>
        public void addparam(string name, string type, ParameterDirection direction)
        {
            try
            {
                OracleParameter prm = new OracleParameter();
                prm.ParameterName = "@" + name;
                switch (type)
                {
                    case "string": prm.OracleDbType = OracleDbType.Varchar2; break;
                    case "int": prm.OracleDbType = OracleDbType.Int32; break;
                    case "datetime": prm.OracleDbType = OracleDbType.TimeStamp; break;
                    case "blob": prm.OracleDbType = OracleDbType.Blob; break;
                    case "decimal": prm.OracleDbType = OracleDbType.Decimal; break;
                    case "curs": prm.OracleDbType = OracleDbType.RefCursor; break;
                    case "date": prm.OracleDbType = OracleDbType.Date; break;
                }
                prm.Direction = direction;
                cmd.Parameters.Add(prm);
                Mylogger.Info(name + ":=" + prm.Value + ";");
            }
            catch (Exception ex)
            {
                Mylogger.Info("Info adding parameters , direction, value, size: " + ex.ToString());
            }
        }

        public OracleDataReader query1()
        {
            OracleDataReader b = cmd.ExecuteReader();
            try
            {
                trnx.Commit();
                //close();
            }
            catch (Exception ex)
            {
                Mylogger.Info("Info Executing query: " + ex.ToString());
            }
            return b;
        }

        public int query()
        {
            //OracleParameterCollection ab = cmd.Parameters;
            //System.Text.StringBuilder values = new System.Text.StringBuilder();
            //foreach (OracleParameter p in ab)
            //{
            //    values.AppendLine(p.ParameterName + "=" + p.ToString() + ", ");
            //}
            //String v = Convert.ToString(values);
            //Mylogger.Info(" Executing query: " + cmd.CommandText);
            int j = cmd.ExecuteNonQuery();
            try
            {
                trnx.Commit();
                close();
            }
            catch (Exception ex)
            {
                Mylogger.Info("Info Executing query: " + ex.ToString());
            }
            return j;
        }

        public void close()
        {
            try
            {
                trnx.Dispose();
            }
            catch
            {
                Mylogger.Info("Info  disposing transaction " + trnx.ToString());
            }
            conn.Close();
        }
    }
}