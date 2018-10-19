using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EtagBalResSucc
/// </summary>
public class EtagBalResSucc
{
    public string status { get; set; }
    public string hash { get; set; }
    public string reference { get; set; }
    public string requestId { get; set; }
    public string responsecode { get; set; }
    public  data Data { get; set; }

    public class data
    {
        public string name { get; set; }
        public string account_number { get; set; }
        public string balance_date { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string balance { get; set; }
    }

}