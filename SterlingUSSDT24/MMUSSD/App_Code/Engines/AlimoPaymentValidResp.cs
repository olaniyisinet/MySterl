using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AlimoPaymentValidResp
/// </summary>

    public class AlimoPaymentValidResp
    {
        public Validationresponse validationResponse { get; set; }
    }

    public class Validationresponse
    {
        public string referenceID { get; set; }
        public string CustomerName { get; set; }
        public string amtPaid { get; set; }
        public string revcode { get; set; }
        public string sessionid { get; set; }
        public string statusCode { get; set; }
        public string statusMessage { get; set; }
        public string hash { get; set; }
    }