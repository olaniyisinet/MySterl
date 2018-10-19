using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Sterling.MSSQL;
using imalcore;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

namespace imalChannelWS.Models
{
	public class BillsPayReq
	{
		public string requestCode { get; set; }
		public string fromAccount { get; set; }
		public string amount { get; set; }
		public string TransType { get; set; }
		public string ChannelName { get; set; }
		public string referenceCode { get; set; }
		public string principalIdentifier { get; set; }
	}
}