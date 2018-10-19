using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    /// <summary>
    /// verify otp while passing in the minutes of expiration
    /// </summary>
    public class verifyOtpHigherDelayReq
    {
       

        [Required]
        public string nuban { get; set; }
        [Required]
        public string otp { get; set; }
        [Required]
        public int Appid { get; set; }
        /// <summary>
        /// minutes of otp validation
        /// </summary>
        [Required]
        public int minutes { get; set; }
    }
}