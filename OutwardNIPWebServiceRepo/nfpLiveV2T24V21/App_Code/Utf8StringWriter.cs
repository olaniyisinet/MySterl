using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for Utf8StringWriter
/// </summary>
public class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding
    {
        get { return new UTF8Encoding(false); }
    }
}