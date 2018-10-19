using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace NIB_EChannels.Helpers
{
	public class Splitter
	{

		public string splitRequests(string result, int position)
		{
			if (result == "") { return ""; }
			else
			{
				string req = "";
				string[] strarr = null;
				char[] splitchar = { ',' };

				strarr = result.Split(splitchar);

				req = strarr[position].Split(':')[1];
				return RemoveSpecialCharacters(req);
			}
		}

		public string splitInterBRequests(string result, int position)
		{
			if (result == "") { return ""; }
			else
			{
				string req = "";
				string[] strarr = null;
				char[] splitchar = { ',' };

				strarr = result.Split(splitchar);

				req = strarr[position];
				return devide(req, ':');
			}
		}

	public string devide(string word, char character)
		{
			string req = "";
			int len = word.Split(character).Length;

			if (len == 2)
			{
				req = word.Split(character)[1];
			}
			else if (len == 3)
			{
				req = word.Split(character)[1] + " " + word.Split(character)[2];
			}
			else {
			req=	word.Split(character)[0];
			}

			return RemoveSpecialCharacters(req);
		}

		public string splits2(string result, int position)
		{
			if (result == "") { return ""; }
			else
			{
				string req = "";
				string[] strarr = null;
				char[] splitchar = { ',' };

				strarr = result.Split(splitchar);
							req = strarr[position].Split(':')[1];
				return req;
			}
		}

		public string splitAndDecrypt(string result, int position)
		{
			string InwardKey = ConfigurationSettings.AppSettings["InwardKey"].ToString();
			result = RemoveCharacters(result);
			string req = "";
			string[] strarr = null;
			char[] splitchar = { ',' };

			strarr = result.Split(splitchar);

			string arr = strarr[position].Split(':')[1];
			req = new MyEncryDecr().Decrypt(arr, InwardKey);

			return req;
		}

		//for encrypted strings
		public string RemoveCharacters(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in str)
			{
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '+' || c == '=' || c == ':' || c == '/' || c == ',')
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		public string RemoveSpecialCharacters(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in str)
			{
				if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		public string splitString(string request, string start, string end)
		{
			string req = "";
			int pFrom = request.IndexOf(start) + end.Length;
			int pTo = request.LastIndexOf(end);

			req = request.Substring(pFrom, pTo - pFrom);

			return RemoveSpecialCharacters(req);
		}

		//public string SplitRefString(string request, string start, string end)
		//{
		//	string req = "";
		//	int pFrom = request.IndexOf(start) + end.Length;
		//	int pTo = request.LastIndexOf(end);

		//	req = request.Substring(pFrom, pTo - pFrom);

		//	return RemoveStrings(RemoveSpecialCharacters(req));
		//}

		public string SplitPaymentRef(string request, string start, string end)
		{
			string req = "";
			int pFrom = request.IndexOf(start) + end.Length;
			int pTo = request.LastIndexOf(end);

			req = request.Substring(pFrom, pTo - pFrom);

		//	return req.Split(':')[1] + req.Split(':')[2];
			return req.Substring(19);
		}

		public string RemoveStrings(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in str)
			{
				if ((c >= '0' && c <= '9'))
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

	}
}