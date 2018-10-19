using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class doGenerateOtpAndMailReq
    {
        //public string doGenerateOtpAndMailByPhoneNumber(string phoneNumber, string nuban, Int32 Appid, string destinationEmail, string sourceEmail, string subject)

        [Required]

        public string nuban { get; set; }
        [Required]
        public string destinationEmail { get; set; }
        [Required]
        public int Appid { get; set; }
        [Required]
        public string sourceEmail { get; set; }
        [Required]
        public string mobnum { get; set; }
        [Required]
        public string subject { get; set; }
      
    }
}