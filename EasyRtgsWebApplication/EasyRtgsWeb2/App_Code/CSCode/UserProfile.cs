using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace NovelTradeRoleMgt
{
   public class UserProfile
    {
       public string TellerID;
       public string UserID;
        public string Role;
        public string  Email;
        public string Name;
        public string BraCode;

        public string WorkflowDbConnectionString { get; set; }
        public UserProfile(string workflowDbConnString)
        {
            WorkflowDbConnectionString = workflowDbConnString;
        }
        public UserProfile()
        {
        }
        public void getDetailsForUser(string username)
        {
            //string sql = "SELECT   [DocPropID] ,[Type] ,[Value] ,[ErrorMsg] ,[DocID] ,[DocType]  ,[Owner] FROM [WorkFlowDB].[dbo].[tblDocumentsProperties]  where doctype='SwiftPro' and owner=@username";
            string sql = "SELECT   [DocPropID] ,[Type] ,[Value] ,[ErrorMsg] ,[DocID] ,[DocType]  ,[Owner] FROM [WorkFlowDB].[dbo].[wfUserRole]  where owner=@username";
            SqlConnection cn = new SqlConnection(WorkflowDbConnectionString);
            using (cn)
            {
                try {
                    cn.Open();
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow r in ds.Tables[0].Rows)
                            {
                                switch (r["Type"].ToString().Trim().ToLower())
                                {
                                    case "role":
                                        Role = r["Value"].ToString();
                                        break;
                                    case "userid":
                                        UserID = r["Value"].ToString();
                                        break;
                                    case "email":
                                        Email = r["Value"].ToString();
                                        break;
                                    case "name":
                                        Name = r["Value"].ToString();
                                        break;
                                    case "bracode":
                                        BraCode = r["Value"].ToString();
                                        break;
                                    case "tellerid":
                                        TellerID = r["Value"].ToString();
                                            break;
                                }
                                
                                    
                            }
                        }
                    }
                }
                catch (Exception ex) { 
                    //TODO Log exception here
                    throw ex;
                }
                
            }
            
        }

        public string getUsernameByRole(string rolename, string bracode, string separator)
        {
            string sql = string.Empty;

            
            string username = string.Empty;
            SqlConnection cn = new SqlConnection(WorkflowDbConnectionString);
            using (cn)
            {
                try
                {
                    cn.Open();
                    DataSet ds = new DataSet();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    if (!string.IsNullOrEmpty(bracode))
                    {
                        
                        sql = "SELECT  [DocPropID] ,[Type] ,[Value] ,[ErrorMsg] ,[DocID] ,[DocType] ,[Owner] FROM [WorkFlowDB].[dbo].[wfUserRole]  where Owner in ( SELECT  [Owner] FROM [WorkFlowDB].[dbo].[wfUserRole]  where [Type]='Role' and Value=@role and Owner<>''  ) and  [Type]='BraCode' and Value=@bracode";
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("@bracode",bracode);
                    }
                    else
                    {
                        sql = "SELECT  [DocPropID] ,[Type] ,[Value] ,[ErrorMsg]  ,[DocID] ,[DocType] ,[Owner] FROM [WorkFlowDB].[dbo].[wfUserRole]  where [Type]='Role' and Value=@role";
                        cmd.CommandText = sql;
                    }
                    
                    cmd.Parameters.AddWithValue("@role", rolename);
                    SqlDataReader reader = cmd.ExecuteReader();
                    using (reader)
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader["Owner"] != DBNull.Value)
                                {
                                    username += reader["Owner"].ToString() + separator;
                                }
                            }
                            
                            reader.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    //TODO Log exception here
                    throw ex;
                }
                finally
                {
                    if (cn != null) cn.Close();
                }

            }
            return username;

        }
    }
}
