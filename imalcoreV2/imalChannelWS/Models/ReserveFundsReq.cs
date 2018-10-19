using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class ReserveFundsReq
    {
        [Required]        
        public string account { get; set; }
        [Required]
        public int AmountToReserve { get; set; }
        [Required]
        public string LockId { get; set; }
        [Required]
        public string Narration { get; set; }

        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
    }
}