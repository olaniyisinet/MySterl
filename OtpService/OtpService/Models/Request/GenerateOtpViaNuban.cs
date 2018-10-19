using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class GenerateOtpViaNuban
    {
        /// <summary>
        /// account number of user
        /// </summary>
         [Required]
        public string nuban { get; set; }
        /// <summary>
        /// Appid of sending
        /// </summary>
        [Required]
        public int Appid { get; set; }
        public string otp { get; set; }

    }
}