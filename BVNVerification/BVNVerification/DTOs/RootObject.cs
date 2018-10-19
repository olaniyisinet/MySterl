using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BVNVerification.DTOs
{
    public class RootObject
    {
        public string ResponseCode { get; set; }
        public List<ValidationRespons> ValidationResponses { get; set; }
    }
}