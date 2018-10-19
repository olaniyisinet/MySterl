using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class SendOtpReq
    {
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string mobile { get; set; }
        [StringLength(11, MinimumLength = 11)]
        public string UserID { get; set; }
    }
}