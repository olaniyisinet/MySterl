using System;
using System.Data;

namespace com.sbp.instantacct.entity
{
    public class InstantAccount
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string Mobile { get; set; }
        public string BVN { get; set; }
        public bool IsCustomer { get; set; }
        public string ExistingAccountNumber { get; set; }
        public int Statusflag { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime DateOpened { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime DateApproved { get; set; }

        public void Set(DataRow dr)
        {
            Id = Convert.ToInt32(dr["Id"]);
            SessionId = Convert.ToString(dr["SessionId"]);
            Mobile = Convert.ToString(dr["Mobile"]);
            BVN = Convert.ToString(dr["BVN"]);
            IsCustomer = Convert.ToBoolean(dr["IsCustomer"]);
            ExistingAccountNumber = Convert.ToString(dr["ExistingAccountNumber"]);
            Statusflag = Convert.ToInt32(dr["Statusflag"]);
            DateAdded = Convert.ToDateTime(dr["DateAdded"]);
            DateUpdated = Convert.ToDateTime(dr["DateUpdated"]);
            CustomerId = Convert.ToString(dr["CustomerId"]);
            AccountNumber = Convert.ToString(dr["AccountNumber"]);
            DateOpened = Convert.ToDateTime(dr["DateOpened"]);
            ApprovedBy = Convert.ToString(dr["ApprovedBy"]);
            DateApproved = Convert.ToDateTime(dr["DateApproved"]);
        }
    }
}
