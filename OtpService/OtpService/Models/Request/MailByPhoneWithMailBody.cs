using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class MailByPhoneWithMailBody
    {

        [Required]
        public string phoneNumber { get; set; }
        [Required]
        public string nuban { get; set; }
        [Required]
        public int Appid { get; set; }
        [Required]
        public string destinationEmail { get; set; }
        [Required]
        public string sourceEmail { get; set; }
        [Required]
        public string subject { get; set; }
       // [Required]
        public string mailBody  { get; set; }
    }
}