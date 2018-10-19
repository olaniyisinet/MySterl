using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TreasuryBillItem
/// </summary>
public class TreasuryBillItem
{
    private string _shortname;
    private string _refid;
    private string _datetoMaturity;
    private string _transrage;

    public string shortname
    {
        get { return _shortname; }
        set { _shortname = value; }
    }

    public string Refid
    {
        get { return _refid; }
        set { _refid = value; }
    }
    public string DatetoMaturity
    {
        get { return _datetoMaturity; }
        set { _datetoMaturity = value; }
    }
    public string TransRate
    {
        get { return _transrage; }
        set { _transrage = value; }
    }

    public void Set(DataRow dr)
    {
        shortname = Convert.ToString(dr["shortname"]);
        Refid = Convert.ToString(dr["Refid"]);
        TransRate = Convert.ToString(dr["TransRate"]);
        DatetoMaturity = Convert.ToDateTime(dr["DatetoMaturity"]).ToString("ddMMyyyy");
    }
}