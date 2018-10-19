using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ussdjobsInterbank.Models

{

    public class Imal_NIP_NameEnqResp
    {
        public string nameDetails { get; set; }
        public string responseCode { get; set; }
        public int errorCode { get; set; }
        public bool skipProcessing { get; set; }
        public string originalResponseCode { get; set; }
        public bool skipLog { get; set; }


    }
}
