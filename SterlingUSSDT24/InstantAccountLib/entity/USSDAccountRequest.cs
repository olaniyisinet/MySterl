using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.sbp.instantacct.entity
{
    public class USSDAccountRequest : EntityBase
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string Mobile { get; set; }
        public string BVN { get; set; }
        public bool IsCustomer { get; set; }
        public string ExistingAccountNumber { get; set; }
        public int Statusflag { get; set; } 
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime DateOpened { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime DateApproved { get; set; }
    }
}
