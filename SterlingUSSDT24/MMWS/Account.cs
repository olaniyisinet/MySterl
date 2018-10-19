namespace MMWS
{
    public static class Account
    {
        public static string Activation(Request q)
        {
            q.Service = "Account";
            q.Name = "Activation";
            Poster p = new Poster(q);
            p.AddParam("otp", q.ActivationCode);
            p.AddParam("activationNewPin", q.NewPIN);
            p.AddParam("activationConfirmPin", q.ConfirmPIN);
            return p.Send(); 
        }

        public static string ChangePIN(Request q)
        {
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = "Account";
            q.Name = "ChangePIN";
            Poster p = new Poster(q);
            p.AddParam("newPIN", q.NewPIN);
            p.AddParam("confirmPIN", q.ConfirmPIN);
            return p.SendWithAuth();
        } 

        public static string CheckBalance(Request q)
        { 
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service =  q.SourcePocketCode == "1"? "Wallet" :"Bank";
            q.Name = "CheckBalance";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            return p.SendWithAuth();
        }

        public static string History(Request q)
        { 
            q.AuthKey = Authentication.Login(q, q.PIN);
            q.Service = q.SourcePocketCode == "1" ? "Wallet" : "Bank";
            q.Name = "History";
            Poster p = new Poster(q);
            p.AddParam("sourcePocketCode", q.SourcePocketCode);
            return p.SendWithAuth();
        }
    }
}
