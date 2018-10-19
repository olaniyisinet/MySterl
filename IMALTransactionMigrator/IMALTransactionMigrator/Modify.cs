using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMALTransactionMigrator
{
	class Modify
	{
		public string ModifyTextColumn(string St, string newString, string trans_id)
		{
			//Console.WriteLine("transaction code -- " + trans_id + "\n");
			//Console.WriteLine("Orignal Message \n" +St);
			string finalsms = "";

			if (trans_id == "16" || trans_id == "17") {

				int pFrom = St.IndexOf("Desc:") + "Desc:".Length;
				int pTo = St.LastIndexOf("Amt");

				string result = St.Substring(pFrom, pTo - pFrom);

				finalsms = St.Replace(result, "  " + newString + " \n");

				//Console.WriteLine("Original Message Replaced ... \n" +finalsms);
			}
			else
			{
				finalsms = St;

				//	Console.WriteLine("Original Message Not Replaced ... \n" + finalsms);	
			}
			return finalsms;
		}

	}
}
