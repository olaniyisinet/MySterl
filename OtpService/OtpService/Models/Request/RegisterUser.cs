using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class RegisterUser
    {
        [Required]
        public string firstname { get; set; }
        [Required]
        public string lastname { get; set; }
        [Required]
        [EmailAddress]
      
        public string email { get; set; }
        [Required]
        public string mobile { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string bvn { get; set; }
        [Required]
       // [RegularExpression(@"(\S\D)+", ErrorMessage = "Space and numbers not allowed")]
        public string HandleUsername { get; set; }
        [Required]
        public string Password { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
 
    }
}
//[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
//   ErrorMessage = "Please enter valid email id.")] 