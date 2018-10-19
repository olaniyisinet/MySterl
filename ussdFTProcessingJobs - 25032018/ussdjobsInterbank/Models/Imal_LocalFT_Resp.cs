using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ussdjobsInterbank.Models
{
    public class Imal_LocalFT_Resp
    {
        public string availabeBalanceAfterOperation { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string iMALTransactionCode { get; set; }
        public string skipProcessing { get; set; }
        public string originalResponseCode { get; set; }
        public string skipLog { get; set; }
        public string transactionID { get; set; }
    }
}
