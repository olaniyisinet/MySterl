using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]

        public string Password { get; set; }
       // [Required]

      //  public string userid { get; set; }
    }
}