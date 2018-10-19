using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doubble.Models
{
    public class DoubbleRequest
    {
        [Key]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string BVN { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfEntry { get; set; }
        public string SterlingVerified { get; set; }
        public string ValCode { get; set; }
    }
}