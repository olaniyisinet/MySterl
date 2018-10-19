namespace MMWS
{
    public static class Wallet
    {
        public static string CheckBalance(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Wallet";
            q.Name = "CheckBalance";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", "1");
            return p.SendWithAuth();
        }

        public static string History(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Wallet";
            q.Name = "History";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", "1");
            return p.SendWithAuthRaw();
        }

        public static string TransferConfirm(Request q)
        {
            bool conf = q.Confirmed == "1";
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Wallet";
            q.Name = "Transfer";
            q.GetParentIDs();
            Poster p = new Poster(q);
            p.AddParam("transferID", q.TransferID);
            p.AddParam("destMDN", q.DestMobile);
            p.AddParam("confirmed", conf.ToString().ToLower());
            p.AddParam("parentTxnID", q.ParentTxnID);
            p.AddParam("destPocketCode", q.DestPocketCode);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            return p.SendWithAuth();
        }

        public static string TransferInquiry(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Wallet";
            q.Name = "TransferInquiry";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("amount", q.Amount);
            p.AddParam("destPocketCode", q.DestPocketCode);
            p.AddParam("destMDN", q.DestMobile);
            string resp = p.SendWithAuthRaw();
            string messageCode = XMLTool.GetNodeAttribute(resp, "message", "code");
            string message = XMLTool.GetNodeData(resp, "message");

            if (messageCode == "687")
            {
                message += "%0AEnter 2 again to continue";
                return message;
            }

            if (messageCode == "72")
            {
                string parentTxnID = XMLTool.GetNodeData(resp, "parentTxnID");
                string transferID = XMLTool.GetNodeData(resp, "transferID");
                q.SetParentIDs(parentTxnID, transferID);
                message += "%0AEnter 1 again to confirm";
                return message;
            }
            return message;
        }

        public static string TransferInquiryUnReg(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Wallet";
            q.Name = "TransferInquiry";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("amount", q.Amount);
            p.AddParam("destPocketCode", q.DestPocketCode);
            p.AddParam("destMDN", q.DestMobile);
            p.AddParam("subLastName", q.SubLastName);
            p.AddParam("subFirstName", q.SubFirstName);
            string resp = p.SendWithAuthRaw();
            string messageCode = XMLTool.GetNodeAttribute(resp, "message", "code");
            string message = XMLTool.GetNodeData(resp, "message");



            if (messageCode == "676")
            {
                string parentTxnID = XMLTool.GetNodeData(resp, "parentTxnID");
                string transferID = XMLTool.GetNodeData(resp, "transferID");
                q.SetParentIDs(parentTxnID, transferID);
                message += "%0AEnter 1 again to confirm";
                return message;
            }
            return message;
        }
    }
}
