using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class AirTimeReq
    {
        [StringLength(11, MinimumLength = 11)]
        public string bvn { get; set; } //
        public string amount { get; set; }
        public string NetworkProvider { get; set; }

        public string PhoneNumber { get; set; }


        //public string ReferenceID { get; set; }
        //public string RequestType { get; set; }
        //public string Mobile { get; set; }
        //public string Beneficiary { get; set; }
        //public string Amount { get; set; }
        //public string NUBAN { get; set; }
        //public string NetworkID { get; set; }
        //public string Type { get; set; } //2
    }
}