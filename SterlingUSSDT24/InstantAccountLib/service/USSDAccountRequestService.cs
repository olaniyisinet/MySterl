using System;
using com.sbp.instantacct.entity;
using com.sbpws.utility;

namespace com.sbp.instantacct.service
{
    public class USSDAccountRequestService
    {
        public static string[] Insert(USSDAccountRequest item)
        {
            string resp = "99|0";
            try
            {
                var ws = new OCAWS.ocaserviceSoapClient();
                resp = ws.USSDAccountRequest(JSONize.SerializeToString(item));
            }
            catch(Exception ex)
            {
                new ErrorLog(ex);
            }
            char[] sep = { '|' };
            return resp.Split(sep);
        }
         
    }
}
