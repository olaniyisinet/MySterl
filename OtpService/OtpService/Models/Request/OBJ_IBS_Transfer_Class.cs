using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class OBJ_IBS_Transfer_Class
    {
        public string ReferenceID { get; set; }
        public string RequestType { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public decimal Amount { get; set; }
        public string PaymentReference { get; set; }
        public string SessionID { get; set; }
        public string DestionationBankCode { get; set; }
        public string BeneficiaryName { get; set; }
    }
}