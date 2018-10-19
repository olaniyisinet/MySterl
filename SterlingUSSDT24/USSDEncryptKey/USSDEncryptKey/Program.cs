using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace USSDEncryptKey
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'USSDEncryptKey.exe'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection processList = searcher.Get();
                cnt = processList.Count;
                if (cnt > 1)
                {
                    throw new Exception("An instance of this Application is already Running!!!.This instance will be stopped.");
                }
                else
                {
                    while (true)
                    {
                        DataSet ds = new DataSet();
                        Console.WriteLine("");
                        Console.WriteLine("Processing new transactions [EncryptFlag=0]....");
                        ds = getTransactions();
                        processDS(ds);

                        Console.WriteLine("");
                        Console.WriteLine("Process waiting Time....");
                        Thread.Sleep(1000);
                        Console.WriteLine("");
                        Console.WriteLine("Process continued ....");
                    }
                }
            }
            catch (Exception ex) { }

        }
        static DataSet getTransactions()
        {
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            string sql = "select * from [Go_Registered_Account] where Activated=1 and len(custauthid)=4";
            c.SetSQL(sql);
            DataSet ds = c.Select("rec");
            return ds;
        }
        static void processDS(DataSet ds)
        {
            int cnt = ds.Tables[0].Rows.Count; string custauthid = ""; Int32 Referenceid = 0;
            string valu1 = ""; string valu2 = "";
            if (cnt == 0)
            {
                //return;
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                {
                    valu1 = "";
                    DataRow dr = ds.Tables[0].Rows[i];
                    Console.WriteLine("");
                    Console.WriteLine("Processing item " + i.ToString() + " of " + cnt.ToString());
                    //get the values assigned into the respective variables
                    Referenceid = Int32.Parse(dr["Referenceid"].ToString());
                    custauthid = dr["custauthid"].ToString();
                    //encrypt the value

                    Encrypto ec = new Encrypto();

                    valu1 = ec.Encrypt(custauthid);
                    valu2 = ec.Decrypt(valu1);
                    //update the record with the encrypted value
                    int val = updatePassPin(Referenceid, valu1);
                    if(val > 0)
                    {
                        UpdateRecord(Referenceid);
                    }
                    //valu2 = EnDeP.Decrypt(valu1, key.passKey);
                }
            }
        }//end for processDS

        public static int updatePassPin(Int32 refid, string encrytval)
        {
            int val = 0;
            string sql = "update Go_Registered_Account set custauthid =@custauthid where Referenceid =@rid";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetSQL(sql);
            c.AddParam("@custauthid", encrytval);
            c.AddParam("@rid", refid);
            val = c.Execute();
            return val;
        }
        public static void UpdateRecord(Int32 refid)
        {
            string sql = "update Go_Registered_Account set EncryptFlag =1 where Referenceid=@id ";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("mssqlconn");
            c.SetSQL(sql);
            c.AddParam("@id", refid);
            c.Execute();
        }
    }
}
