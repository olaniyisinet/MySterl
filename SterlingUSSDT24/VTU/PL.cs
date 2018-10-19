using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VTU.BLL;
using VTU.UTILITIES;
using VTU.ObjectInfo;
using System.Data;


namespace VTU.PL
{
   public class USSD
    {
       public static string DoActivation(string Mobile,string Nuban)
       {          
           return USSD_BLL.Activate(Nuban, Mobile);
       }

       public static string DoBuySelf(string mobile,string Amount,string network,string sessionId, string frmNuban)
       {
           return USSD_BLL.Buy_Self(mobile, Amount, network, sessionId, frmNuban);
       }

       public static string Buy_Friend(string mobile_friend, string mobile, string Amount, string network, string sessionId,string pin, string frmNuban)
       {
           return USSD_BLL.Buy_Friend(mobile_friend, mobile, Amount, network, sessionId,pin, frmNuban);
       }

       public static DataTable SaveSteps(string SessionId, string Misdn, int reqType, string val, string prev, string next)
       {
           return USSD_BLL.SaveSteps( SessionId,Misdn,reqType,val,prev,next);
       }

       public static DataTable GetSteps(string SessionId, string Misdn)
       {
           return USSD_BLL.GetSteps(SessionId, Misdn);
       }

       public static int UpdateInput(string SessionId, string Misdn, string val, string prev, string next)
       {
           return USSD_BLL.UpdateInput(SessionId, Misdn, val, prev, next);
       }
    }

   public class ConsoleJobs
    {
       public static string DoVTU(Svc_Request svc)
       {
           return Console_BLL.DoVTU(svc);
       }

       public static DataTable getNetwork(string prefix)
       {
           return Console_BLL.getNetwork(prefix);
       }

       public static int UpdateRequest(Go_Request req, Svc_Response svc_resp)
       {
           return Console_BLL.UpdateRequest(req, svc_resp);
       }

       public static void dataArchive(DataTable dt)
       {
           Console_BLL.dataArchive(dt);
       }

       public static DataTable getTimedOutRequest(int MinLapses)
       {
           return Console_BLL.getTimedOutRequest(MinLapses);
       }

       public static DataTable getCompletedRequest()
       {
           return Console_BLL.getCompletedRequest();
       }

       public static int UpdateStamp(RequestLog log)
       {
           return Console_BLL.UpdateStamp(log);
       }

       public static int LogStamp(RequestLog log)
       {
           return Console_BLL.LogStamp(log);
       }

       public static string SendMessage(string data,string mobile)
       {
           return Console_BLL.SendMessage(data,mobile);
       }

       public static int SetFlag(Go_Request a)
       {
           return Console_BLL.SetFlag(a);
       }

       public static void resetTimedOut(int minLapse)
       {
           Console_BLL.resetTimedOut(minLapse);
       }

       public static DataSet getBankDetail(string nuban)
       {
           return Console_BLL.getBankDetail(nuban);
       }

       public static DataTable getRegisteredAcct()
       {
           return Console_BLL.getRegisteredAcct();
       }

       public static void UpdateRegisteredAcct(string oMobile, string nMobile, string nuban)
       {
           Console_BLL.UpdateRegisteredAcct(oMobile,nMobile,nuban);
       }
    }

   public class GUI
   {
       public static void SaveLog(AuditLog a)
       {

           GUI_BLL.SaveLog(a);
       }

       public static int UpdateAdminProfile(int ID, int RoleID, int Status)
       {
           return GUI_BLL.UpdateAdminProfile(ID, RoleID, Status);
       }

       public static int InsertAdminProfile(string username, string Fullname, string StaffID, string RoleID, string CreatedBy, string Status, string Email, string Deptname, string Unit)
       {
           return GUI_BLL.InsertAdminProfile(username, Fullname, StaffID, RoleID, CreatedBy, Status, Email, Deptname, Unit);
       }

       public static retObject getAdminInfoByUsername(string username)
       {
           return GUI_BLL.getAdminInfoByUsername(username);
       }

       public static DataTable getAdminInfo()
       {
           return GUI_BLL.getAdminInfo();
       }

       public static DataTable getAdminInfoByID(int ID)
       {
           return GUI_BLL.getAdminInfoByID(ID);
       }

       public static int UpdateConfig(decimal minPtxn, decimal maxPtxn, decimal maxPday, string code)
       {
           return GUI_BLL.UpdateConfig(minPtxn, maxPtxn, maxPday, code);
       }

       public static DataTable getConfig(string code)
       {
           return GUI_BLL.FetchConfig(code);
       }

       public static DataTable getReport(string nm, string st, string en, string fl, string chn)
       {
           return GUI_BLL.getReport(nm, st, en, fl, chn);
       }

       public static DataTable getReport_Daily(string nm, string fl, string chn)
       {
           return GUI_BLL.getReport_Daily(nm, fl, chn);
       }

       public static DataSet DashBoadr()
       {
           return GUI_BLL.DashBoadr();
       }

       public static DataSet DashBoard(string st, string en)
       {
           return GUI_BLL.DashBoard(st, en);
       }

       public static DataTable getBlackList(string mobile)
       {
           return GUI_BLL.getBlackList(mobile);
       }

       public static DataTable getBlackListHistory(string mobile)
       {
           return GUI_BLL.getBlackListHistory(mobile);
       }

       public static DataSet getBlackListHistoryAll(string mobile)
       {
           return GUI_BLL.getBlackListHistoryAll(mobile);
       }

       public static DataTable getRegiteredAccount(string mobile, string StatusFlag)
       {
           return GUI_BLL.getRegiteredAccount(mobile, StatusFlag);
       }

       public static int enListBlackList(BlackList b, BlackListHistory h, int channel)
       {
           return GUI_BLL.enListBlackList(b, h, channel);
       }

       public static int deListBlackList(BlackList b, BlackListHistory h, int channel)
       {
           return GUI_BLL.deListBlackList(b, h, channel);
       }
   }

   public class General
   {
       public static DataTable getRequestByStatus(int RequestStatus)
       {
           return Generic_BLL.getRequestByStatus(RequestStatus);
       }

       public static DataTable getRequestByNetwork(int RequestStatus, int Network)
       {
           return Generic_BLL.getRequestByNetwork(RequestStatus, Network);
       }

       public static DataTable getRequestByNotNetwork(int RequestStatus, int Network)
       {
           return Generic_BLL.getRequestByNotNetwork(RequestStatus, Network);
       }

       public static DataTable getRequestByChannelID(int RequestStatus, int channelID)
       {
           return Generic_BLL.getRequestByChannelID(RequestStatus, channelID);
       }

       public static DataTable getRequestByNetworkID(int ChannelID =0, int NetworkID = 0)
       {
           return Generic_BLL.getRequestByNetworkID(ChannelID, NetworkID);
       }
   }
}