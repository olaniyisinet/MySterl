using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class CreateCIFResp
    {
        public string responseCode { get; set; }
        public string errorCode { get; set; }
        public string skipProcessing { get; set; }
        public string skipLog { get; set; }
        public string cifNo { get; set; }
        public string AccountNo { get; set; }
    }
}