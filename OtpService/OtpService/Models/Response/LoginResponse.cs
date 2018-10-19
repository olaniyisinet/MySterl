using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtpService.Models.Response
{
    public class LoginResponse
    {
        public int userid { get; set; }
        public string firstname { get; set; }

        public string lastname { get; set; }
       
        public string email { get; set; }

        public string pwd { get; set; }

        public string mobile { get; set; }

        //public string currency { get; set; }

        public string bvn { get; set; }

        public DateTime dob { get; set; }

        public string longitude { get; set; }

        public string latitude { get; set; }

        public int industryid { get; set; }

        public int statusflag { get; set; }

        // public string datemodified { get; set; }

        public int safe_zone_flag { get; set; }
    }
}