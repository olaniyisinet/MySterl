using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class RequeryReq
    {
        public string fromAccount { get; set; }
        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
        public string referenceCodeToFind { get; set; }
    }
}