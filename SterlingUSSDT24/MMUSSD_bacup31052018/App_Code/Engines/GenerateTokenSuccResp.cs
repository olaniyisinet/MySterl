using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for GenerateTokenSuccResp
/// </summary>
public class GenerateTokenSuccResp
{
        public string subscriberId { get; set; }
        public string payWithMobileToken { get; set; }
        public string tokenLifeTimeInMinutes { get; set; }

}