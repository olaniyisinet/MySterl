using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTU.ObjectInfo
{
   public class Go_Network_Prefix
    {
        public  string Mobile_Prefix { get; set; }
        public  string Mobile_Network { get; set; }
    }

   public class Go_Request
    {
        public  string Mobile { get; set; }
        public  string Beneficiary { get; set; }
        public  string Amount { get; set; }
        public  string RequestDate { get; set; }
        public  string RequestStatus { get; set; }
        public  string NUBAN { get; set; }
        public  string RequestString { get; set; }
        public  string RequestRef { get; set; }
        public  string RequestType { get; set; }
        public  string BillCode { get; set; }
        public  string NetworkID { get; set; }
        public string CallerRefID { get; set; }
        public string sessionId { get; set; }
        public string ChannelID { get; set; }
    }

   public class Go_Registered_Account
    {
        public  string Mobile { get; set; }
        public  string NUBAN { get; set; }
        public  string DateRegistered { get; set; }
        public  string DateDeactivated { get; set; }
        public  string Comment { get; set; }
        public  string StatusFlag { get; set; }
        public  string RefID { get; set; }
        public  string TransactionRefID { get; set; }
    }

   public class Go_Bill
    {
        public  string BillCode { get; set; }
        public  string BID { get; set; }
        public  string CID { get; set; }
        public  string BName { get; set; }
        public  string BAmt { get; set; }
        public  string BCharge { get; set; }
        public  string SInfo1 { get; set; }
        public  string SInfo2 { get; set; }
        public  string BCur { get; set; }
    }

   public class Svc_Request
    {
        public string RequestType { get; set; }
        public string BillId { get; set; }
        public string ChannelId { get; set; }
        public string BillAmount { get; set; }
        public string BillAccount { get; set; }
        public string CallerRefID { get; set; }
        public string SubscriberInfo1 { get; set; }
        public string SubscriberInfo2 { get; set; }
        public string HashValue { get; set; }
        public string RefId { get; set; }
    }

   public class Svc_Response
    {
        public string RefId { get; set; }
        public string CallerRefID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string BillerRef { get; set; }
        public string BillerResp { get; set; }
        public string HashValue { get; set; }
    }

   public class RequestLog
   {
       public string Ref { get; set; }
       public string Proc_Start { get; set; }
       public string Proc_End { get; set; }
       public string networkID { get; set; }
   }

   public class userInfo
   {
       public int ID { get; set; }
       public string userName { get; set; }
       public int RoleID { get; set; }
       public int status { get; set; }
       public string fullname { get; set; }
       public string StaffID { get; set; }
   }

   public class retObject
   {
       public int code { get; set; }
       public string message { get; set; }
       public object objVal { get; set; }
   }  

   public class AuditLog
   {
       public string action;
       public string activity;
       public long doneby;
       public string ip;
       public string sess;
   }

   public class BlackList
   {
       public string Misdn { get; set; }
       public string Account { get; set; }
       public string DateAdded { get; set; }
       public string AddedBy { get; set; }


   }

   public class BlackListHistory
   {
       public string Misdn { get; set; }
       public string Account { get; set; }
       public string DateAdded { get; set; }
       public string AddedBy { get; set; }
       public string StatusType { get; set; }
       public string Comment { get; set; }
   }

   public class LogFolder
   {
       public string ID { get; set; }
       public string Name { get; set; }
   }

   public class LogSubFolder
   {
       public string ID { get; set; }
       public string FolderName { get; set; }
       public string Name { get; set; }
   }
}