using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ussdjobs.Models
{
   public class Imal_NIP_FtResp
    {
        public string responseCode { get; set; }
        public string errorCode { get; set; }
        public string skipProcessing { get; set; }
        public string originalResponseCode { get; set; }
        public string skipLog { get; set; }
    }
}
