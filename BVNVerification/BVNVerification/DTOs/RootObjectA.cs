using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BVNVerification.DTOs
{
    public class RootObjectA
    {
        public string ResponseCode { get; set; }
        public List<BVNS> ValidationResponses { get; set; }
    }
}