using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class NameEnquiryReq
    {
        public string account { get; set; }
        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
        public string destinationBankCode { get; set; }
        public string channelCode { get; set; }
    }
}