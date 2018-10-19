using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtpService.Models
{
    public class AirtimeRechargeReq
    {

        public string ReferenceID { get; set; }
        public string RequestType { get; set; }
        public string Mobile { get; set; }
        public string Beneficiary { get; set; }
        public string Amount { get; set; }
        public string NUBAN { get; set; }
        public string NetworkID { get; set; }
        public string Type { get; set; } //2

    }
}