using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class ReleaseFundsReq
    {
        [Required]
        public string account { get; set; }
        [Required]
        public string reference { get; set; }
        [Required]
        public string Narration { get; set; }
        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
    }
}