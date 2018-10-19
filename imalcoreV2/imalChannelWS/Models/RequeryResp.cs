using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class RequeryResp
    {
        public string responseCode { get; set; }
        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
        public string referenceCodeToFind { get; set; }
        public string status { get; set; }
    }
}