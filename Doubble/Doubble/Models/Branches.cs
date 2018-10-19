using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doubble.Models
{
    public class Branches
    {
        [Key]
        public string T24ID { get; set; }
        public string Company_Name { get; set; }
        public string Mnemonic { get; set; }
        public string Sub_Div_Code { get; set; }
        public string Bra_Code { get; set; }
        public string Address { get; set; }
    }
}