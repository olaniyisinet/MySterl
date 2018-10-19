using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;

namespace NovelTradeRoleMgt
{
    public class TransactionMovement
    {
        public static int HOP_PENDING_FLAG = 1;
        public static int HOP_REJECTED_BY_HOP = 2;
        public static int PENDING_INPUTTER =3;
        public static int REJECTED_BY_INPUTTER = 4;
        public static int PENDING_VERIFIER= 5;
        public static int REJECTED_BY_VERIFIER = 6;
        public static int  APPROVED_BY_VERIFIER = 7;
        public static int 	PUSH_TO_SWIFT = 8;
        public static int DELETED_BY_CSO = 9;
        public SqlConnection DbConnection{get; set;} //Auto-properties
        public string ApplicationConnectionString { get; set; }

        //public TransactionMovement()
        //{

        //}

        public TransactionMovement(string appConnString)
        {
            ApplicationConnectionString = appConnString;
        }
        /// <summary>
        /// ///////////////////
        /// </summary>
        /// <param name="transID"></param>
        /// <param name="status"></param>
        /// <param name="comment"></param>
        /// <param name="usr"></param>
        /// <returns></returns>
        public bool moveTransaction(long transID, string status, string comment, UserProfile usr)
        {
            bool statusFlag = false;
            string reference = string.Empty;
            //Commented on the 23rd June 2015 so that the same request_status is not set twice.
            string sql = "UPDATE tblSwiftRequest set request_status=@status, comments=@comment  where reqID=@transid";

            //SqlConnection cn = new SqlConnection(Utils.GetConnectionStringTrade());
            SqlConnection cn = new SqlConnection(ApplicationConnectionString);
            using (cn)
            {
                try
                {
                    cn.Open();
                    //First get the senders Reference for this reqID
                    string sqlGetReference = "select sendersReference from tblSwiftRequest where reqID=@transid";
                    SqlCommand cmdGetReference = new SqlCommand(sqlGetReference, cn);

                    cmdGetReference.Parameters.AddWithValue("@transid", transID);
                    object retVal = cmdGetReference.ExecuteScalar();
                    try
                    {
                        if (retVal != null)
                        {
                            reference = (string)retVal;
                        }
                    }
                    catch (Exception ex)
                    {
                        //not critical yet.
                        throw ex;
                    }

                    SqlTransaction trans = null;
                    SqlCommand cmd = new SqlCommand(sql, cn);
                    
                    
                    cmd.Parameters.AddWithValue("@transid", transID);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    trans = cn.BeginTransaction();
                    cmd.Transaction = trans;
                    using (trans)
                    {
                        int ret = cmd.ExecuteNonQuery();

                        string sqlInsertHistory = "INSERT INTO [tblTransaction] ([transactionID] ,[role] ,[User_Id] ,[entrydate] ,[isActive],[Action_performed],[req_status],[sendersReference]) VALUES (@transactionID ,@role,@User_Id,GETDATE(),@isActive,@Action_performed,@status, @sendersReference)";

                        SqlCommand cmdInsert = new SqlCommand(sqlInsertHistory, cn);
                        cmdInsert.Parameters.AddWithValue("@transactionID", transID);
                        cmdInsert.Parameters.AddWithValue("@role", usr.Role);
                        cmdInsert.Parameters.AddWithValue("@User_Id", usr.UserID);
                        cmdInsert.Parameters.AddWithValue("@isActive", 1);
                        cmdInsert.Parameters.AddWithValue("@Action_performed", comment);
                        cmdInsert.Parameters.AddWithValue("@status", status);
                        cmdInsert.Parameters.AddWithValue("@sendersReference", reference);
                        cmdInsert.Transaction = trans;
                        int ret2 = cmdInsert.ExecuteNonQuery();

                        if (ret >= 0 && ret2 >= 0)
                        {
                            statusFlag = true;
                            try
                            {
                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                //Log error commiting
                                statusFlag = false;
                                throw ex;
                            }
                        }
                        else
                        {
                            statusFlag = false;
                            try
                            {
                                trans.Rollback();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {                    
                    throw ex;
                }
            }
            return statusFlag;
        }
        public bool moveTransaction2(long transID, string status, string comment, UserProfile usr, SqlConnection cn)
        {
            bool statusFlag = false;
            string reference = string.Empty;
            //Commented on the 23rd June 2015 so that the same request_status is not set twice.
            string sql = "UPDATE tblSwiftRequest set request_status=@status, comments=@comment  where reqID=@transid";

           // SqlConnection cn = new SqlConnection(Utils.GetConnectionStringTrade());
            
                try
                {
                    if (cn.State != System.Data.ConnectionState.Open)
                    {
                        cn.Open();
                    }
                    
                    //First get the senders Reference for this reqID
                    string sqlGetReference = "select sendersReference from tblSwiftRequest where reqID=@transid";
                    SqlCommand cmdGetReference = new SqlCommand(sqlGetReference, cn);

                    cmdGetReference.Parameters.AddWithValue("@transid", transID);
                    object retVal = cmdGetReference.ExecuteScalar();
                    if (retVal != null)
                    {
                        reference = (string)retVal;
                    }

                    SqlTransaction trans = null;
                    SqlCommand cmd = new SqlCommand(sql, cn);

                    cmd.Parameters.AddWithValue("@transid", transID);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    trans = cn.BeginTransaction();
                    cmd.Transaction = trans;
                    using (trans)
                    {
                        int ret = cmd.ExecuteNonQuery();

                        string sqlInsertHistory = "INSERT INTO [tblTransaction] ([transactionID] ,[role] ,[User_Id] ,[entrydate] ,[isActive],[Action_performed],[req_status],[sendersReference]) VALUES (@transactionID ,@role,@User_Id,GETDATE(),@isActive,@Action_performed,@status, @sendersReference)";

                        SqlCommand cmdInsert = new SqlCommand(sqlInsertHistory, cn);
                        cmdInsert.Parameters.AddWithValue("@transactionID", transID);
                        cmdInsert.Parameters.AddWithValue("@role", usr.Role);
                        cmdInsert.Parameters.AddWithValue("@User_Id", usr.UserID);
                        cmdInsert.Parameters.AddWithValue("@isActive", 1);
                        cmdInsert.Parameters.AddWithValue("@Action_performed", comment);
                        cmdInsert.Parameters.AddWithValue("@status", status);
                        cmdInsert.Parameters.AddWithValue("@sendersReference", reference);
                        cmdInsert.Transaction = trans;
                        int ret2 = cmdInsert.ExecuteNonQuery();

                        if (ret >= 0 && ret2 >= 0)
                        {
                            statusFlag = true;
                            try
                            {
                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                //Log error commiting
                                statusFlag = false;
                                throw ex;
                                
                            }
                        }
                        else
                        {
                            statusFlag = false;
                            try
                            {
                                trans.Rollback();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    //log
                    throw ex;
                }
          
            return statusFlag;
        }
        //////////////////////////////////////////////Old code above ////////////////////////////////////

        ///// <summary>
        ///// ///////////////////
        ///// </summary>
        ///// <param name="transID"></param>
        ///// <param name="status"></param>
        ///// <param name="comment"></param>
        ///// <param name="usr"></param>
        ///// <returns></returns>
        //public bool moveTransaction(long transID, string status, string comment, UserProfile usr, SqlConnection conn)
        //{
        //    bool statusFlag = false;
        //    string reference = string.Empty;
        //    //Commented on the 23rd June 2015 so that the same request_status is not set twice.
        //    string sql = "UPDATE tblSwiftRequest set request_status=@status, comments=@comment  where reqID=@transid";

        //    SqlConnection cn = conn;//new SqlConnection(Utils.GetConnectionStringTrade());
        //    //using (cn)
        //    //{
        //        try
        //        {
        //            if (cn.State != System.Data.ConnectionState.Open)
        //            {
        //                cn.Open();
        //            }

        //            //First get the senders Reference for this reqID
        //            string sqlGetReference = "select sendersReference from tblSwiftRequest where reqID=@transid";
        //            SqlCommand cmdGetReference = new SqlCommand(sqlGetReference, cn);

        //            cmdGetReference.Parameters.AddWithValue("@transid", transID);
        //            object retVal = cmdGetReference.ExecuteScalar();
        //            if (retVal != null)
        //            {
        //                reference = (string)retVal;
        //            }

        //            SqlTransaction trans = null;
        //            SqlCommand cmd = new SqlCommand(sql, cn);

        //            cmd.Parameters.AddWithValue("@transid", transID);
        //            cmd.Parameters.AddWithValue("@status", status);
        //            cmd.Parameters.AddWithValue("@comment", comment);
        //            trans = cn.BeginTransaction();
        //            cmd.Transaction = trans;
        //            using (trans)
        //            {
        //                int ret = cmd.ExecuteNonQuery();

        //                string sqlInsertHistory = "INSERT INTO [tblTransaction] ([transactionID] ,[role] ,[User_Id] ,[entrydate] ,[isActive],[Action_performed],[req_status],[sendersReference]) VALUES (@transactionID ,@role,@User_Id,GETDATE(),@isActive,@Action_performed,@status, @sendersReference)";

        //                SqlCommand cmdInsert = new SqlCommand(sqlInsertHistory, cn);
        //                cmdInsert.Parameters.AddWithValue("@transactionID", transID);
        //                cmdInsert.Parameters.AddWithValue("@role", usr.Role);
        //                cmdInsert.Parameters.AddWithValue("@User_Id", usr.UserID);
        //                cmdInsert.Parameters.AddWithValue("@isActive", 1);
        //                cmdInsert.Parameters.AddWithValue("@Action_performed", comment);
        //                cmdInsert.Parameters.AddWithValue("@status", status);
        //                cmdInsert.Parameters.AddWithValue("@sendersReference", reference);
        //                cmdInsert.Transaction = trans;
        //                int ret2 = cmdInsert.ExecuteNonQuery();

        //                if (ret >= 0 && ret2 >= 0)
        //                {
        //                    statusFlag = true;
        //                    try
        //                    {
        //                        trans.Commit();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        //Log error commiting
        //                        statusFlag = false;
        //                    }
        //                }
        //                else
        //                {
        //                    statusFlag = false;
        //                    try
        //                    {
        //                        trans.Rollback();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }

        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            //log
        //        }
        //        finally
        //        {
        //            if (cn != null) cn.Close();
        //        }
        //    //}
        //    return statusFlag;
        //}

        public bool moveConcession(int bracode, int cusnum, string status, string comment, UserProfile usr)
        {
            bool statusFlag = false;
            string reference = string.Empty;
            string sql = "UPDATE tblConMain SET request_status=@status  WHERE bra_code=@bracode AND cus_num=@cusnum";
            //SqlConnection cn = new SqlConnection(Utils.GetConnectionStringTrade());
            SqlConnection cn = new SqlConnection(ApplicationConnectionString);
            using (cn)
            {
                try
                {
                    cn.Open();

                    SqlCommand cmd = new SqlCommand(sql, cn);
                    cmd.Parameters.AddWithValue("@bracode", bracode);
                    cmd.Parameters.AddWithValue("@cusnum", cusnum);
                    cmd.Parameters.AddWithValue("@status", status);
                    int i = cmd.ExecuteNonQuery();
                    if (i >= 0)
                    {
                        statusFlag = true;
                    }
                }
                catch (Exception ex)
                {
                    //log
                    throw ex;
                }
                finally
                {
                   if (cn !=null) cn.Close();
                }
            }
            return statusFlag;
        }
    }
}
