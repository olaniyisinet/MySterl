using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Imal_NIP_NameEnqReq
/// </summary>
public class Imal_NIP_NameEnqReq
{
    public string account { get; set; }
    public string requestCode { get; set; }
    public string principalIdentifier { get; set; }
    public string referenceCode { get; set; }
    public string destinationBankCode { get; set; }
    public string channelCode { get; set; }
}