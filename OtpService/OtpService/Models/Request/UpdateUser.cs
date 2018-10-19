using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OtpService.Models.Request
{
    public class UpdateUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]

        public string Mobile { get; set; }
        [Required]

        public string Bvn { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string HandleUsername { get; set; }
    }
}