using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoubbleAdmin.Models
{
    public class PreTerminate
    {
        public string FullName { get; set; }
        public string BVN { get; set; }
        public string Category { get; set; }
        public string PayInAccount { get; set; }
        public string Term { get; set; }
        public DateTime DateOfEntry { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public string PayInAmount { get; set; }

        public string ArrangementId { get; set; }
    }
}