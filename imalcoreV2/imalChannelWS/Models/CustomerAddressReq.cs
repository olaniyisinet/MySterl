using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class CustomerAddressReq
    {
        public string customerNumber { get; set; }
        public string branchCode { get; set; }
        public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
        public string requestCode { get; set; }
    }
}