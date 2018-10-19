using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateLoanResp
    {
        public string responseCode { get; set; }
        public string skipProcessing { get; set; }
        public int errorCode { get; set; }
        public string skipLog { get; set; }
        public int dealNo { get; set; }
    }
}