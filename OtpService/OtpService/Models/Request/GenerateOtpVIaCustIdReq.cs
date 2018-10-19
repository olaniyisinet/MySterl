using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class GenerateOtpVIaCustIdReq
    {
        [Required]
        public string UserIDD { get; set; }
        [Required]
        public string mail { get; set; }
        [Required]
        public string phonenumber { get; set; }
    }
}