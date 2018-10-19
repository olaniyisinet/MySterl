using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NIB_EChannels.Helpers
{
	public class Helper
	{
		public static bool checkDates(DateTime date1, DateTime date2)
		{
			// validate your date here and return True if validated
			if (date1 > date2)
			{
				return false;
			}
			else
			{
				return true;
			}	
		}

		public static bool checkToday(DateTime date2)
		{
			// validate your date here and return True if validated
			if (date2 > DateTime.Today)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}