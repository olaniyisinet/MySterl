using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMALTransactionMigrator
{
    class PerformJob
    {
       
    }
    //private static 
    public static  DataSet doRead()
    {
        DataTable dtProxy = new DataTable("proxytable");
        dtProxy.Columns.Add("Phone");
        dtProxy.Columns.Add("sender");
        dtProxy.Columns.Add("text");
        dtProxy.Columns.Add("inserted_at");
        dtProxy.Columns.Add("processed_at");
        dtProxy.Columns.Add("sent_at");
        dtProxy.Columns.Add("dlr_timestamp");
        dtProxy.Columns.Add("dlr_status");
        dtProxy.Columns.Add("dlr_description");
        dtProxy.Columns.Add("nuban");
        dtProxy.Columns.Add("ledgercode");
        dtProxy.Columns.Add("currency");
        dtProxy.Columns.Add("aans_status");


        string sqlConnStr = ConfigurationSettings.AppSettings["mssqlconn"].ToString();
        string dTable = "proxytable";
        DataSet ds = null;
        try
        {
            var sql = "select * from SMS_MESSAGES where PROCESSED=0  AND ROWNUM <10000";
            Sterling.Oracle.Connect CN = new Sterling.Oracle.Connect();
            CN.SetSQL(sql);
            ds = CN.Select();
            if(ds!=null)
            {
                if(ds.Tables[0].Rows.Count!=0)
                {
                    ds.Tables[0].TableName="proxytable";
                    using (var connection = new SqlConnection(sqlConnStr))
                    {
                        connection.Open();
                        //Open bulkcopy connection.
                        using (var bulkcopy = new SqlBulkCopy(connection))
                        {
                            bulkcopy.DestinationTableName = dTable;
                            bulkcopy.WriteToServer(ds.Tables[0]);
                            connection.Close();
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {

        }
        return ds;
    }
}
