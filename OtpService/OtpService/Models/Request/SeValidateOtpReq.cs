using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class SeValidateOtpReq
    {
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string mobile { get; set; }
        public string Otp { get; set; }
    }
}