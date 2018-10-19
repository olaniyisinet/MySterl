using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class PaycodeReq
    {
        public string appid { get; set; }
        public string ttid { get; set; }
        public int amount { get; set; }
        public string codeGenerationChannel { get; set; }
        public string accountNo { get; set; }
        public string transactionRef { get; set; }
        public string subscriber { get; set; }
        public string oneTimePin { get; set; }
        public string paymentChannel { get; set; }
    }
}