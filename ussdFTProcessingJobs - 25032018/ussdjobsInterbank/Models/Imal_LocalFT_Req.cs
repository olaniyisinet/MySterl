using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ussdjobsInterbank.Models
{
    public class Imal_LocalFT_Req
    {
        public string fromAccount { get; set; }
        public string toAccount { get; set; }
        public string amount { get; set; }
        public string requestCode { get; set; }
        public string principalIdentifier { get; set; }
        public string referenceCode { get; set; }
        public string beneficiaryName { get; set; }

    }
}
