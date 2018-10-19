using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class GetUser
    {
        [StringLength(11, MinimumLength = 11)]
        public string Bvn { get; set; }
    }
}