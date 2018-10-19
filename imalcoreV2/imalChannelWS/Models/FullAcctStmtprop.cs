using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class FullAcctStmtprop<T>
    {
        public List<T> statements { get; set; }
        public string responseCode { get; set; }
        public string skipProcessing { get; set; }
        public string skipLog { get; set; }
    }
}