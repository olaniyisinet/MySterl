using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlentyMoney
{
        public class mFinoInfo
        {
            public string SessionID { get; set; }
            public string Misdn { get; set; }
            public string ServiceName { get; set; }
            public string TxnName { get; set; }
            public string SourceMdn { get; set; }
            public string DestMdn { get; set; }
            public string Amount { get; set; }
            public string mFlag { get; set; }
            public string sctlID { get; set; }
            public string refID { get; set; }
            public string Remark { get; set; }
            public string txnDate { get; set; }
            public string PIN { get; set; }
            public string sourcePocketCode { get; set; }
            public string destPocketCode { get; set; }
            public string transferID { get; set; }
            public string parentTxnID { get; set; }
            public string MsgCode { get; set; }
            public string AuthKey { get; set; }

            public string InstitutionID { get; set; }
            public string ChannelID { get; set; }

            public string plentyCertPath { get; set; }
        }
}
