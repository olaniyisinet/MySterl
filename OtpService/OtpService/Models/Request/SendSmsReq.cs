using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class SendSmsReq
    {
      
        [Required]
        public string msg { get; set; }
        [Required]
        public string mobnum { get; set; }
        [Required]

        public int Appid { get; set; }
    }
}