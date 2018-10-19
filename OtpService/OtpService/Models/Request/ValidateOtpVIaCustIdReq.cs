using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class ValidateOtpVIaCustIdReq
    {
        [Required]
        public string LoginID { get; set; }
        [Required]
        public int AppID { get; set; }
        [Required]
        public string Token  { get; set; }
    }
}