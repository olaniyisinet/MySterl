using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class NameEnquiryResp
    {
        public string nameDetails { get; set; }
        public string responseCode { get; set; }
        public string errorCode { get; set; }
        public string skipProcessing { get; set; }
        public string originalResponseCode { get; set; }
        public string skipLog { get; set; }
        public string neSid { get; set; }
    }
}