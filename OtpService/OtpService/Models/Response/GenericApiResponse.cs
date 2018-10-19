using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OtpService.Models.Response
{
    public class GenericApiResponse<T>
    {

        public string message { get; set; }
        public string response { get; set; }
        public string responsedata { get; set; }
        //var data type
        public T data { get; set; }
    }
}