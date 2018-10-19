namespace MMWS
{
    public static class Transfer
    {
        public static string History(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Wallet";
            q.Name = "History";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", "1");
            return p.SendWithAuthRaw();
        }

        public static string Confirm(Request q)
        {
            bool conf = q.Confirmed == "1";
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = q.SourcePocketCode == "1"? "Wallet":"Bank";
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
        
        public static string ConfirmInterBank(Request q)
        {
            bool conf = q.Confirmed == "1";
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
            q.Name = "Transfer";
            q.GetParentIDs();
            Poster p = new Poster(q);
            p.AddParam("transferID", q.TransferID);
            p.AddParam("destMDN", q.DestMobile);
            p.AddParam("confirmed", conf.ToString().ToLower());
            p.AddParam("parentTxnID", q.ParentTxnID);
            p.AddParam("destPocketCode", q.DestPocketCode);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("destBankAccount", q.DestBankAccount);
            p.AddParam("destBankCode", q.DestBankCode); 
            return p.SendWithAuth();
        }

        public static string Inquiry(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
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
                message += "%0AEnter 2 to continue";
                return message;
            }

            if (messageCode == "72")
            {
                string parentTxnID = XMLTool.GetNodeData(resp, "parentTxnID");
                string transferID = XMLTool.GetNodeData(resp, "transferID");
                q.SetParentIDs(parentTxnID, transferID);
                message += "%0AEnter 1 to confirm";
                return message;
            }
            return message;
        }

        public static string InquiryUnReg(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
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
                message += "%0AEnter 1 to confirm";
                return message;
            }
            return message;
        }

        public static string InquiryInterBank(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
            q.Name = "TransferInquiry";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("amount", q.Amount);
            p.AddParam("destPocketCode", q.DestPocketCode);
            p.AddParam("destBankAccount", q.DestBankAccount);
            p.AddParam("destBankCode", q.DestBankCode); 
            string resp = p.SendWithAuthRaw();
            string messageCode = XMLTool.GetNodeAttribute(resp, "message", "code");
            string message = XMLTool.GetNodeData(resp, "message");



            if (messageCode == "72")
            {
                string parentTxnID = XMLTool.GetNodeData(resp, "parentTxnID");
                string transferID = XMLTool.GetNodeData(resp, "transferID");
                q.SetParentIDs(parentTxnID, transferID);
                message += "%0AEnter 1 to confirm";
                return message;
            }
            return message;
        }

        public static string CashOutInquiry(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
            q.Name = "CashOutInquiry";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("amount", q.Amount);
            p.AddParam("agentCode", q.AgentCode); 
            string resp = p.SendWithAuthRaw();
            string messageCode = XMLTool.GetNodeAttribute(resp, "message", "code");
            string message = XMLTool.GetNodeData(resp, "message");

            //if (messageCode == "687")
            //{
            //    message += "%0AEnter 2 to continue";
            //    return message;
            //}

            //if (messageCode == "72")
            //{
            //    string parentTxnID = XMLTool.GetNodeData(resp, "parentTxnID");
            //    string transferID = XMLTool.GetNodeData(resp, "transferID");
            //    q.SetParentIDs(parentTxnID, transferID);
            //    message += "%0AEnter 1 to confirm";
            //    return message;
            //}
            return message;
        }

        public static string CashOut(Request q)
        {
            bool conf = q.Confirmed == "1";
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
            q.Name = "CashOut";
            q.GetParentIDs();
            Poster p = new Poster(q);
            p.AddParam("transferID", q.TransferID); 
            p.AddParam("confirmed", conf.ToString().ToLower());
            p.AddParam("parentTxnID", q.ParentTxnID);
            p.AddParam("agentCode", q.AgentCode); 
            return p.SendWithAuth();
        }

        public static string DoInquiry(Request q)
        {
            q.AuthKey = q.AuthKey;
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
            q.Name = "TransferInquiry";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("amount", q.Amount);
            p.AddParam("destPocketCode", q.DestPocketCode);
            p.AddParam("destMDN", q.DestMobile);
            string resp = p.SendWithAuthRaw();
            string messageCode = XMLTool.GetNodeAttribute(resp, "message", "code");
            string message = XMLTool.GetNodeData(resp, "message");
            if (messageCode == "72")
            {
                string parentTxnID = XMLTool.GetNodeData(resp, "parentTxnID");
                string transferID = XMLTool.GetNodeData(resp, "transferID");
                q.SetParentIDs(parentTxnID, transferID);
                message = resp;
                return message;
            }
            return resp;
        }

        public static string DoConfirm(Request q)
        {
            bool conf = q.Confirmed == "1";
            string x = Authentication.DoLogin(q, q.PIN);
            q.AuthKey = q.AuthKey ;
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
            q.Name = "Transfer";
            q.GetParentIDs();
            Poster p = new Poster(q);
            p.AddParam("transferID", q.TransferID);
            p.AddParam("destMDN", q.DestMobile);
            p.AddParam("confirmed", conf.ToString().ToLower());
            p.AddParam("parentTxnID", q.ParentTxnID);
            p.AddParam("destPocketCode", q.DestPocketCode);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            return p.SendWithAuthRaw();
        }

        public static string DoCompleteTransfer(Request q)
        {
            string response = "";
            string xKeys = Authentication.DoJobLogin(q, q.PIN);
            q.AuthKey = xKeys;
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
            q.Name = "TransferInquiry";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            p.AddParam("amount", q.Amount);
            p.AddParam("destPocketCode", q.DestPocketCode);
            p.AddParam("destMDN", q.DestMobile);
            string resp = p.SendWithAuthRaw();
            string messageCode = XMLTool.GetNodeAttribute(resp, "message", "code");
            string message = XMLTool.GetNodeData(resp, "message");
            if (messageCode == "72")
            {
                string parentTxnID = XMLTool.GetNodeData(resp, "parentTxnID");
                string transferID = XMLTool.GetNodeData(resp, "transferID");
                bool conf = q.Confirmed == "1";
                //q.AuthKey = authKey;
                q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
                q.Name = "Transfer";
                Poster px = new Poster(q);
                px.AddParam("transferID", transferID);
                px.AddParam("destMDN", q.DestMobile);
                px.AddParam("confirmed", conf.ToString().ToLower());
                px.AddParam("parentTxnID", parentTxnID);
                px.AddParam("destPocketCode", q.DestPocketCode);
                px.AddParam("sourcePocketCode", q.SourcePocketCode);
                response = px.SendWithAuthRaw();
            }
            return response;
        }
    }
}
