using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class CustBranchDetailsprop<T>
    {
        public List<T> branches { get; set; }
        public string customerNumber { get; set; }
        public string responseCode { get; set; }
        public string errorCode { get; set; }
        public string skipProcessing { get; set; }
        public string skipLog { get; set; }
    }
}