using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class WalletToWalletReq
    {
        [Required]
        public string amt { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11)]

        public string toacct { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11)]

        public string frmacct { get; set; }
        [Required]

        public string paymentRef { get; set; }
        [Required]

        public string remarks { get; set; }
    }
}