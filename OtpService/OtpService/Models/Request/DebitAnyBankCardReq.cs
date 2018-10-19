using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class DebitAnyBankCardReq
    {
        public string customerId { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string pin { get; set; }
        public string cvv { get; set; }
        public string expiry_date { get; set; }
        public string pan { get; set; }
        [StringLength(11, MinimumLength = 11)]
        public string bvn { get; set; }
    }
}