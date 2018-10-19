using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NIB_EChannels
{
    public class Gadget
    {

        public DataTable search(string name, string table_name)
        {
            string sql = null;
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            string[] strArr = null;
            char[] splitchar = { ' ' };
            strArr = name.Split(splitchar);
            DataTable dt = new DataTable();

            if (strArr.Count() == 2)
            {
                sql = "select * from " + table_name + " where First_Name='" + strArr[0] + "' OR First_Name = '" + strArr[1] + "'  OR Last_Name='" + strArr[0] + "' OR Last_Name = '" + strArr[1] + "' OR Middle_Name='" + strArr[0] + "' OR Middle_Name = '" + strArr[1] + "' ";
                c.SetSQL(sql);
                dt = c.Select("rec").Tables[0];
                return SearchFor2(strArr, dt);
            }
            else
            {
                sql = "select * from " + table_name + " where First_Name='" + strArr[0] + "' OR First_Name = '" + strArr[1] + "' OR First_Name = '" + strArr[2] + "' OR Last_Name='" + strArr[0] + "' OR Last_Name = '" + strArr[1] + "' OR Last_Name = '" + strArr[2] + "' OR Middle_Name='" + strArr[0] + "' OR Middle_Name = '" + strArr[1] + "' OR Middle_Name = '" + strArr[2] + "' ";
                c.SetSQL(sql);
                dt = c.Select("rec").Tables[0];
                return SearchFor3(strArr, dt);
            }

            




        }

        static DataTable SearchFor2(String [] strArr, DataTable dt)
        {
            string strA = strArr[0].ToUpper().Trim();
            string strB = strArr[1].ToUpper().Trim();
            string strC ="";
            string strD = "";
            string strE = "";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            for(int i = 0; i < strArr.Count(); i++)
            {

                strC = dt.Rows[i]["First_Name"].ToString().ToUpper().Trim();
                strD = dt.Rows[i]["Last_Name"].ToString().ToUpper().Trim();
                strE = dt.Rows[i]["Middle_Name"].ToString().ToUpper().Trim();

                int count = 0;
                DataRow dr = dt.Rows[i];
                if (strA == strC & strB == strD)
                {
                    count++;
                }
                else if(strA == strC & strB == strE)
                {
                    count++;
                }
                else if (strA == strD && strB == strC)
                {
                    count++;
                }
                else if (strA == strD && strB == strE)
                {
                    count++;
                }
                else if (strA == strE && strB == strC)
                {
                    count++;
                }
                else if (strA == strE && strB == strD)
                {
                    count++;
                }

                if(count < 1)
                {
                    dr.Delete();
                }

            }
            return dt;
            

        }

        static DataTable SearchFor3(String[] strArr, DataTable dt)
        {
            //strArr = new String[3];
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            for (int i = 0; i < strArr.Count(); i++)
            {
                int count = 0;
                DataRow dr = dt.Rows[i];
                if (strArr[0].ToUpper().Trim() == dt.Rows[i]["First_Name"].ToString().ToUpper().Trim() && strArr[1].ToUpper().Trim() == dt.Rows[i]["Last_Name"].ToString().ToUpper().Trim() && strArr[2].ToUpper().Trim() == dt.Rows[i]["Middle_Name"].ToString().ToUpper().Trim())
                {
                    count++;
                }
                else if (strArr[0].ToUpper().Trim() == dt.Rows[i]["First_Name"].ToString().ToUpper().Trim() && strArr[1].ToUpper().Trim() == dt.Rows[i]["Middle_Name"].ToString().ToUpper().Trim() && strArr[2].ToUpper().Trim() == dt.Rows[i]["Last_Name"].ToString().ToUpper().Trim())
                {
                    count++;
                }
                else if (strArr[0].ToUpper().Trim() == dt.Rows[i]["Last_Name"].ToString().ToUpper().Trim() && strArr[1].ToUpper().Trim() == dt.Rows[i]["First_Name"].ToString().ToUpper().Trim() && strArr[2].ToUpper().Trim() == dt.Rows[i]["Middle_Name"].ToString().ToUpper().Trim())
                {
                    count++;
                }
                else if (strArr[0].ToUpper().Trim() == dt.Rows[i]["Last_Name"].ToString().ToUpper().Trim() && strArr[1].ToUpper().Trim() == dt.Rows[i]["Middle_Name"].ToString().ToUpper().Trim() && strArr[2].ToUpper().Trim() == dt.Rows[i]["First_Name"].ToString().ToUpper().Trim())
                {
                    count++;
                }
                else if (strArr[0].ToUpper().Trim() == dt.Rows[i]["Middle_Name"].ToString().ToUpper().Trim() && strArr[1].ToUpper().Trim() == dt.Rows[i]["First_Name"].ToString().ToUpper().Trim() && strArr[2].ToUpper().Trim() == dt.Rows[i]["Last_Name"].ToString().ToUpper().Trim())
                {
                    count++;
                }
                else if (strArr[0].ToUpper().Trim() == dt.Rows[i]["Middle_Name"].ToString().ToUpper().Trim() && strArr[1].ToUpper().Trim() == dt.Rows[i]["Last_Name"].ToString().ToUpper().Trim() && strArr[2].ToUpper().Trim() == dt.Rows[i]["First_Name"].ToString().ToUpper().Trim())
                {
                    count++;
                }

                if (count < 1)
                {
                    dr.Delete();
                }

            }
            return dt;


        }

        public string getLastUpdated(string url)
        {
            string resp = "";
            Sterling.MSSQL.Connect c = new Sterling.MSSQL.Connect("msconn");
            string sql = "select dateGenerated from updateRec where ReportType= '" + url + "'";
            c.SetSQL(sql);
            DataTable dt = c.Select("rec").Tables[0];
            resp = dt.Rows[0]["dateGenerated"].ToString();
            return resp;
        }


    }
}