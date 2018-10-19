using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ImalRequest
/// </summary>
public class ImalRequest
{
    public string account { get; set; }
    public string requestCode { get; set; }
    public string principalIdentifier { get; set; }
    public string referenceCode { get; set; }
}