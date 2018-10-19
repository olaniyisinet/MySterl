namespace MMWS
{
    public static class Authentication
    {
        public static string Login(Request q, string authenticationString)
        {
            string ak = q.GetLogin().Trim();
            if (ak == "")
            {
                q.Service = "Account";
                q.Name = "Login";
                q.AuthKey = authenticationString;
                Poster p = new Poster(q);
                ak = p.Login();
                q.SetLogin(ak);
            }
            return ak;
        }

        public static string DoLogin(Request q, string authenticationString)
        {
            string ak = q.GetLogin().Trim();
            if (ak == "")
            {
                q.Service = "Account";
                q.Name = "Login";
                q.AuthKey = authenticationString;
                q.ChannelID = 6;
                q.InstID = string.Empty;
                Poster p = new Poster(q);
                ak = p.Login();
                q.SetLogin(ak);
            }
            return ak;
        }

        public static string DoJobLogin(Request q, string authenticationString)
        {
            string ak = "";
                q.Service = "Account";
                q.Name = "Login";
                q.AuthKey = authenticationString;
                q.ChannelID = 6;
                q.InstID = string.Empty;
                Poster p = new Poster(q);
                ak = p.Login();
                return ak;
        }
    }
}
