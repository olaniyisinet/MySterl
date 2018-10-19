using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class ReserveFundsResp
    {
        public string responseCode { get; set; }
        public string errorCode { get; set; }
        public string errorMsg { get; set; }
        public string transactionCode { get; set; }
        public string lockReference { get; set; }
    }
}