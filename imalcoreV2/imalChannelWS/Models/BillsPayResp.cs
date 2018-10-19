using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
	public class BillsPayResp
	{
		public string availabeBalanceAfterOperation { get; set; }
		public string responseCode { get; set; }
		public string responseMessage { get; set; }
		public string errorCode { get; set; }
		public string errorMessage { get; set; }
		public string iMALTransactionCode { get; set; }
		public string skipProcessing { get; set; }
		public string originalResponseCode { get; set; }
		public string skipLog { get; set; }
		public string transactionID { get; set; }
	}
}