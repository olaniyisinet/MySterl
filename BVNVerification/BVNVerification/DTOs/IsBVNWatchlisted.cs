using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BVNVerification.DTOs
{
    public class IsBVNWatchlisted
    {
        public string ResponseCode { get; set; }
        public string BVN { get; set; }
        public string BankCode { get; set; }
        public string Category { get; set; }
        public string WatchListed { get; set; }
    }
}