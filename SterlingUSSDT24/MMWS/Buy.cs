using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMWS
{
    public static class Buy
    {
        public static string AirtimePurchaseInquiry(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Buy";
            q.Name = "AirtimePurchaseInquiry";
            Poster p = new Poster(q);
            p.AddParam("amount", q.Amount);
            p.AddParam("companyID", q.CompanyID);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("destMDN", q.DestMobile); 
            string resp = p.SendWithAuthRaw();
            string messageCode = XMLTool.GetNodeAttribute(resp, "message", "code");
            string message = XMLTool.GetNodeData(resp, "message");



            if (messageCode == "713")
            {
                string parentTxnID = XMLTool.GetNodeData(resp, "parentTxnID");
                string transferID = XMLTool.GetNodeData(resp, "transferID");
                q.SetParentIDs(parentTxnID, transferID);
                message += "%0AEnter 1 to confirm";
                return message;
            }
            return message;
        }

        public static string AirtimePurchaseConfirm(Request q)
        {
            bool conf = q.Confirmed == "1";
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Buy";
            q.Name = "AirtimePurchase";
            q.GetParentIDs();
            Poster p = new Poster(q);
            p.AddParam("amount", q.Amount);
            p.AddParam("companyID", q.CompanyID);
            p.AddParam("destMDN", q.DestMobile);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("confirmed", conf.ToString().ToLower());
            p.AddParam("transferID", q.TransferID);
            p.AddParam("parentTxnID", q.ParentTxnID);  
            return p.SendWithAuth();
        }
    }
}
