using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoubbleAdmin.Models
{
    public class Roles
    {
        public string RoleName { get; internal set; }

        public class Category
        {
            [Key]
            public int id { get; set; }
            public string RoleName { get; set; }

        }
    }
}