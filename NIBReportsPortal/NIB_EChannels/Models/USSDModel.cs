using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NIB_EChannels.Models
{
	public class USSDModel
	{
		public string Refid { get; set; }
		public string SessionID { get; set; }
		public string Method { get; set; }
		public string Request { get; set; }
		public string Response { get; set; }
		public string ResponseCode { get; set; }
		public string Dateadded { get; set; }
		public string DateProcessed { get; set; }
		public string FromAccount { get; set; }
		public string ToAccount { get; set; }
		public string Amount { get; set; }
		public string RequestCode { get; set; }
		public string ReferenceCode { get; set; }
		public string BeneficiaryName { get; set; }
		public string PaymentReference { get; set; }
		public string DestinationBankCode { get; set; }
		public string CustomerShowName { get; set; }
		public string ChannelCode { get; set; }
		public string PrincipalIdentifier { get; set; }
		public string NesId { get; set; }
	}
}