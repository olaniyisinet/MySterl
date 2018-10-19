using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for EtagBalResFail
/// </summary>
public class EtagBalResFail
{

        public string responsecode { get; set; }
        public string status { get; set; }
        public string error { get; set; }
        public string requestid { get; set; }


    public class Error
    {
        public string non_field_errors { get; set; }
        //public string Message { get; set; }
    }
}