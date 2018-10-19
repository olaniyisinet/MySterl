using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doubble.Models
{
    public class ForSterling
    {

        [Key]
        public int ID { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string BVN { get; set; }
        public string Category { get; set; }
        public string PayInAccount { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAccount { get; set; }
        public string BeneficiaryType { get; set; }
        public DateTime DateOfEntry { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PayInAmount { get; set; }
        public string ReferenceID { get; set; }
        public string DAOCode { get; set; }
        public string BraCode { get; set; }
        public string Email { get; set; }
        public string CusNum { get; set; }
        public int Term { get; set; }
        public string ARRANGEMENT_ID { get; set; }
        public string Password { get; set; }

        public string ValCode { get; set; }
    }
}