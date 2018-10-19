using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class LocalFTReq
    {
        public string fromAccount { get; set; }
        public string toAccount { get; set; }
        public string amount { get; set; }
        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
        public string beneficiaryName { get; set; }
        public string paymentReference { get; set; }
    }
}