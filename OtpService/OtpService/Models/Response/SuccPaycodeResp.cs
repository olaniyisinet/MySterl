using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtpService.Models.Response
{
    public class SuccPaycodeResp
    {
        public string subscriberId { get; set; }
        public string payWithMobileToken { get; set; }
        public string tokenLifeTimeInMinutes { get; set; }
    }
}