using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class NIPInwardFTResp
    {
        public string AvailabeBalanceAfterOperation { get; set; }//43
        public string iMALTransactionCode { get; set; }//44
        public string iMALRequestLogID { get; set; }//45
        public string ErrorCode { get; set; }//46
        public string ResponseCode { get; set; }//46
        public string ResponseMessage { get; set; }//47

        
    }
}