using BVN;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
/// <summary>
/// Summary description for DoBankBVN
/// </summary>
public class DoBankBVN
{
    public static SqlConnection GetAppConnection()
    {
        string configCon = ConfigurationManager.AppSettings["mssqlconn_bvn"];
        return new SqlConnection(configCon);
    }
    public static string getDOB(string customerid)
    {
        string val = "";
        DateTime dt = new DateTime();
        EACBS.banksSoapClient sc = new EACBS.banksSoapClient("banksSoap");
        DataSet ds = sc.getCustomrInfo(customerid);
        if(ds.Tables[0].Rows.Count > 0)
        {
            DataRow dr = ds.Tables[0].Rows[0];
            dt = Convert.ToDateTime(dr["Dateofbirth"].ToString());
        }
        val = dt.ToString("yyyy-mm-dd");
        return val;
    }
    public string DoBank(string bvn, string mob)
    {
        string mob_234 = mob;
        string mob_080 = mob;
        BVN_Util.formatMobile_to_080(ref mob_080);
        BVN_Util.formatMobile_to_234(ref mob_234);

        string procLog = "";
        string resp = "";
        EACBS.banksSoapClient sc = new EACBS.banksSoapClient("banksSoap");
        try
        {
            if (bvn.Length == 11)
            {
                
                DataSet ds = sc.getCustomerAccountsByMobileNo(mob);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        int cnt_all = 0;
                        int cnt_fail = 0;
                        int cnt_success = 0;
                        int cnt_duplicate = 0;

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var dt_distinct = (from dr in ds.Tables[0].AsEnumerable()
                                               select new
                                               {
                                                   MOBILE_PHONE = mob
                                                   ,
                                                   MAP_ACC_NO = dr.Field<object>("NUBAN")
                                                   ,
                                                   CustomerID = dr.Field<object>("CustomerId")
                                               }).Distinct().ToList();

                            DataTable dtx = BVN_Util.ToDataTable(dt_distinct);
                            cnt_all = dtx.Rows.Count;
                            foreach (DataRow dr in dtx.Rows)
                            {
                                string dob = "";
                                dob = getDOB(dr["CustomerId"].ToString());
                                //year month day
                                try
                                {
                                    SqlParameter[] parameters = {
                                       new SqlParameter("@BVN",bvn)
                                      ,new SqlParameter("@AccountNo",dr["MAP_ACC_NO"])
                                      ,new SqlParameter("@AccountType","BANK")
                                      ,new SqlParameter("@PhoneNo",mob)
                                      ,new SqlParameter("@DateofBirth",dob)
                                      ,new SqlParameter("@BatchID",mob)
                                      ,new SqlParameter("@RequestFlag","New")
                                      ,new SqlParameter("@Source","USSD")
                                    };

                                    string sql = " INSERT INTO tbl_stagingArea (BVN,AccountNo,AccountType,PhoneNo,DateofBirth,DateUploaded,BatchID,RequestFlag,Source) VALUES (@BVN,@AccountNo,@AccountType,@PhoneNo,@DateofBirth,GETDATE(),@BatchID,@RequestFlag,@Source) select @@rowcount";
                                    int val = (int)SqlHelper.ExecuteScalar(GetAppConnection(), CommandType.Text, sql, parameters);
                                    cnt_success = cnt_success + 1;
                                    procLog = string.Format("Request for BVN:{0} with Account:{1} respond with:{2}", bvn, dr["MAP_ACC_NO"], val);
                                    ApplicationLog_ log = new ApplicationLog_(procLog, "bvnlog", mob);

                                }
                                catch (SqlException ex)
                                {
                                    procLog = string.Format("Request for BVN:{0} with Account:{1} respond with:{2}", bvn, dr["MAP_ACC_NO"], ex.Message);
                                    ApplicationLog_ log = new ApplicationLog_(procLog, "bvnlog", mob);
                                    int v = ex.Number;
                                    switch (v)
                                    {
                                        case 2627: //composite ke/primary key violation error
                                            cnt_duplicate = cnt_duplicate + 1;
                                            continue;
                                        default: //other error type
                                            cnt_fail = cnt_fail + 1;
                                            continue;
                                    }
                                }
                            }

                            if (cnt_success == 0 && cnt_fail == 0 && cnt_duplicate == 0)
                            {
                                //non operation occurred ..this is very slimmy or almost impossible to occurre
                                resp = "Operation could not be completed at this time!Please try again.";
                            }

                            if (cnt_all == cnt_duplicate)
                            {
                                //already processed
                                resp = "BVN " + bvn + " has already been submitted by you!%0AKindly approach the nearest sterling bank for enquiry.";
                            }

                            if (cnt_fail > 0 && cnt_success > 0)
                            {
                                //some record failed, some record pass
                                resp = "Some account could not be mapped to your BVN.Kindly approach the nearest sterling bank for more enquiry";
                            }

                            if (cnt_fail == cnt_all)
                            {
                                //all failed
                                resp = "BVN number " + bvn + " could not be submitted at this time. Please try again!";
                            }

                            if (cnt_all == cnt_success)
                            {
                                //all success
                                resp = "BVN " + bvn + " has been successfully submitted for mapping to your profile.%0AKindly note that this is subject to further verification.";
                            }
                        }
                        else
                        {
                            //No record found
                            resp = "We could not match this phone number to your bank profile.%0AKindly approach the nearest sterling bank to update your profile.";
                        }
                    }
                    else
                    {
                        //service error
                        resp = "All connections are currently in use!%0APlease try again.";
                    }
                }
                else
                {
                    //Service connection error
                    resp = "All connections are currently in use!%0APlease try again.";
                }
            }
            else
            {
                //Invalid bvn lenght
                resp = "Invalid BVN number!%0ASupply a valid 11 digits BVN number";
            }
        }
        catch (Exception ex)
        {
            ApplicationLog_ log = new ApplicationLog_(ex.Message, "bvnlog", mob);
            resp = "All connections are currently in use!%0APlease try again.";
        }
        return resp;
    }
}