using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doubble.Models
{
    public class CreateAnnuity
    {
        public string customerid { get; set; }
        public string currency { get; set; }
        public DateTime effectiveDate { get; set; }
        public Decimal commitmentAmount { get; set; }
        public int termInYear { get; set; }
        public string payinAccountNumber { get; set; }
        public string payoutAccountNumber1 { get; set; }
        public string payoutAccountNumber2 { get; set; }
        public string payoutAccountNumber3 { get; set; }
        public string daoCode { get; set; }
        public string BraCode { get; set; }
    }
}