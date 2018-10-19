using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImalWebUtilities.model
{
    public class Statement
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public string TimeCreated { get; set; }
        public DateTime Valuedate { get; set; }
    }
}