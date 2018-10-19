using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class CreateAccReq
    {
        //public string requestCode { get; set; }
        //public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
        [Required]
        public string GL_CODE { get; set; }
        [Required]
        public string CIFNo { get; set; }
        public string REFERENCE { get; set; }
        [Required, MaxLength(8)]
        public string AppName { get; set; }
        public int BranchCode { get; set; }

    }
}