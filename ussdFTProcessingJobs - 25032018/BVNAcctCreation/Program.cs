using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace BVNAcctCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int cnt;
                string query = "Select * from Win32_Process Where Name = 'BVNAcctCreation.exe'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection processList = searcher.Get();
                cnt = processList.Count;
                if (cnt > 1)
                {
                    throw new Exception("An instance of this Application is already Running!!!.This instance will be stopped.");
                    //return;
                }
                else
                {
                    while (true)
                    {
                        DataSet ds = new DataSet();
                        Console.WriteLine("");
                        Console.WriteLine("Processing new transactions [statusflag=0]....");
                        ds = getPendingReq();
                        processDS(ds);

                        Console.WriteLine("");
                        Console.WriteLine("Process waiting Time....");
                        Thread.Sleep(20000);
                        Console.WriteLine("");
                        Console.WriteLine("Process continued ....");
                    }
                }
            }
            catch
            {

            }
        }
        static DataSet getPendingReq()
        {
            //get all pending transactions to be treated
            string sql = "SELECT refid,Sessionid,mobile,BVN,RegistrationDate,statusflag "+
                       " FROM tbl_USSD_acct_open where statusflag = 0 and product is not null and bvn is not null";
            Connect cn = new Connect(sql, true);
            return cn.query("recs");
        }
        static void processDS(DataSet ds)
        {
            StringBuilder rqt = new StringBuilder(); Utility g = new Utility();
            StringBuilder rsp = new StringBuilder(); Int32 refid=0;
            string resp = ""; string bvn = ""; string FirstName = ""; string MiddleName = "";
            string LastName = ""; DateTime DateOfBirth; string PhoneNumber = "";
            string MainBvn = ""; string sms = ""; string mobile = "";
            DateTime RegistrationDate; string EnrollmentBankCode = ""; string EnrollmentBranch = "";
            int cnt = ds.Tables[0].Rows.Count;
            if (cnt == 0)
            {
                //return;
            }
            else
            {
                for (int i = 0; i < cnt; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    MainBvn = dr["BVN"].ToString();
                    mobile = dr["mobile"].ToString();
                    refid = Int32.Parse(dr["refid"].ToString());
                    BVN.ProcessWebBVNSoapClient ws = new BVN.ProcessWebBVNSoapClient();
                    //form the request
                    rqt.Clear();
                    rqt.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    rqt.Append("<BVNRequest>");
                    rqt.Append("<BVN>" + MainBvn + "</BVN>");
                    rqt.Append("</BVNRequest>");
                    string str = "";
                    str = rqt.ToString();
                    str = g.Encrypt(str);
                    resp = ws.ProcessWeb(str, 12);
                    //decrypt the response
                    XmlDocument xmlDoc = new XmlDocument();
                    try
                    {
                        resp = g.Decrypt(resp);
                        xmlDoc.LoadXml(resp);
                        bvn = xmlDoc.GetElementsByTagName("bvn").Item(0).InnerText;
                        if (bvn.ToUpper() == "NO BVN")
                        {
                            int cn = g.UpdateRec1(refid, 99);
                            if (cn > 0)
                            {
                                new Errorlog("Customer record with BVN " + bvn + " has been set for account opening processing job on " + DateTime.Now.ToString());
                            }
                            sms = "Dear Customer, " + "\n\r We were unable to verify your bvn details from NIBSS. ";
                            g.insertIntoInfobip(sms, mobile);
                            return;
                        }
                        FirstName = xmlDoc.GetElementsByTagName("FirstName").Item(0).InnerText;
                        MiddleName = xmlDoc.GetElementsByTagName("MiddleName").Item(0).InnerText;
                        LastName = xmlDoc.GetElementsByTagName("LastName").Item(0).InnerText;
                        DateOfBirth = Convert.ToDateTime(xmlDoc.GetElementsByTagName("DateOfBirth").Item(0).InnerText);
                        PhoneNumber = xmlDoc.GetElementsByTagName("PhoneNumber").Item(0).InnerText;
                        RegistrationDate = Convert.ToDateTime(xmlDoc.GetElementsByTagName("RegistrationDate").Item(0).InnerText);
                        EnrollmentBankCode = xmlDoc.GetElementsByTagName("EnrollmentBankCode").Item(0).InnerText;
                        EnrollmentBranch = xmlDoc.GetElementsByTagName("EnrollmentBranch").Item(0).InnerText;

                        //check if the bvn is the same as the one returned
                        if(MainBvn == bvn)
                        {
                            //proceed to update the table accordingly for the account opening processing
                            int cn = g.UpdateRec(refid, FirstName, MiddleName, LastName, DateOfBirth, EnrollmentBranch, RegistrationDate, EnrollmentBankCode, 1);
                            if(cn > 0)
                            {
                                new Errorlog("Customer record with BVN " + bvn +  " has been set for account opening processing job on " + DateTime.Now.ToString());
                            }
                        }
                        else
                        {
                            sms = "Dear " + FirstName +  " " + LastName  + "\n\r We were unable to verify your bvn details from NIBSS. ";
                            g.insertIntoInfobip(sms, mobile);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        new Errorlog("Error occured during processing " + ex);
                    }
                }
            }
        }
    }
}
