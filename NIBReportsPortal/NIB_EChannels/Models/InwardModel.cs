using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NIB_EChannels.Models
{
	public class InwardModel
	{
		public string Id { get; set; }
		public string SessionID { get; set; }
		public string RequestTypeID { get; set; }
		public string RequestIn { get; set; }
		public string TimeIn { get; set; }
		public string ResponseJSON { get; set; }
		public string TimeOut { get; set; }
		public string ResponseCode { get; set; }
		public string ResponseDescription { get; set; }
		public string Beneficiary { get; set; }
		public string Amount { get; set; }
		public string IMALTransactionID { get; set; }
		public string PaymentReference { get; set; }
		public string OriginatorAccountName { get; set; }
		public string Narration { get; set; }
	}
}