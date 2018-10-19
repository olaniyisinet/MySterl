using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class SendMoneyViaUserHandleReq
    {
        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string Bvn { get; set; } //from wallet acct
        [Required]

        public string ReceipientHandle { get; set; }
        [Required]

        public string Amount { get; set; }
        [Required]

        public string PaymentReference { get; set; }
        [Required]
        //- repeat type { never, daily, weekly, monthly, yearly }
        public string RepeatType { get; set; }
        [Required]

        public string ReceipientName { get; set; }
    }
}