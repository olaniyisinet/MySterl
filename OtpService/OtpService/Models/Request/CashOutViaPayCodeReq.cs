using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class CashOutViaPayCodeReq
    {
       [Required]
        public int Amount { get; set; }
        [Required]
        public string mobile { get; set; }
        [StringLength(4,ErrorMessage ="must enter a 4-digit pin", MinimumLength = 4)]
        public string OneTimePin { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        public string CustomerEmail { get; set; }
    }
}