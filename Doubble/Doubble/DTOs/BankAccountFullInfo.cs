using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doubble.DTOs
{
    [Serializable]
    public class BankAccountFullInfo
    {
        public string AccountNo { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string PhoneNo { get; set; }
        public string ACCT_TYPE { get; set; }
    }
}