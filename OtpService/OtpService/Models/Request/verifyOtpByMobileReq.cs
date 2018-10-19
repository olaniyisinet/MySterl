using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class verifyOtpByMobileReq
    {

        [Required]
        public string mobile { get; set; }
        [Required]
        public string otp { get; set; }
        [Required]
        public int Appid { get; set; }
    }
}