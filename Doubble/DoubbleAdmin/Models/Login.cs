using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoubbleAdmin.Models
{
    public class Login
    {
        [Required, DisplayName("AD-Username :")]
        public string Username { get; set; }
        [Required, DisplayName("Password :")]
        public string Password { get; set; }
    }
}