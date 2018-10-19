using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ussdjobsInterbank.Models

{
    public  class ImalDetails
    {
        public int Status { get; set; }
        //public string Mobile { get; set; }
        public string BVN { get; set; }
        public string CurrencyCode { get; set; }
        public string CustomerName { get; set; }
        public string CustId { get; set; }
        public double AvailBal { get; set; }
        public string LedgerCode { get; set; }
        public string AcctStatus { get; set; }
    }
}
