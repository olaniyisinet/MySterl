using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class GetUSerTrxnReq
    {
        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string bvn { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ToDate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime FromDate { get; set; }
    }
}