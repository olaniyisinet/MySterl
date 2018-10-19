using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoubbleAdmin.Models
{
    public class ForNonSterling
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string BVN { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string State { get; set; }
        public string HomeAddress { get; set; }
        public string Category { get; set; }
        public string CusNum { get; set; }
        public int Term { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime DateOfEntry { get; set; }
        public string PayInAccount { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAccount { get; set; }
        public string BeneficiaryType { get; set; }
        public string PayInAmount { get; set; }
        public string ReferenceID { get; set; }
        public string DAOCode { get; set; }
        public string ARRANGEMENT_ID { get; set; }
        public string ValCode { get; set; }
        public string FullName { get; set; }
        

    }
    public enum Gender
    {
        Male,
        Female
    }
    public enum Title
    {
        Mr,
        Mrs,
        Miss
    }
}