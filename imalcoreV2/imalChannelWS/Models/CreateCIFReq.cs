using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    public class CreateCIFReq
    {
        //public string requestCode { get; set; }
        //public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
        [Required]

        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        //public string MiddleName { get; set; }
        public string Telephone { get; set; }
        [Required]
        public string MeansOfIDNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        //public string AccountType { get; set; }
        public string Maritalstatus { get; set; }
        public string customerType { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        [Required]
        public string BVN { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        [Required]
        public string AppName { get; set; }
        public int BranchCode { get; set; }
    }
}