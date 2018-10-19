using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class FullAcctStmtReq
    {
        public string account { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
    }
}