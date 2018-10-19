using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class TwiilioReq
    {
        // public string LoginID { get; set; }
        /// <summary>
        /// Whatsapp number of the receipient, MUST start with "234"
        /// </summary>
        [Required]
        public string WhatsAppNumber { get; set; }
        /// <summary>
        /// the otp to be sent
        /// </summary>
        [Required]
        public string OTP { get; set; }
        /// <summary>
        /// the message..optional
        /// </summary>
       // [Required]

        public string Message { get; set; }
    }
}