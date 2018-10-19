using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BillerItem
/// </summary>
public class BillerItem
{
    private string _shortName;
    private string _amount;
    private string _transRate;
    private string _itemFee;

    public string ShortName
    {
        get { return _shortName; }
        set { _shortName = value; }
    }

    public string Amount
    {
        get { return _amount; }
        set { _amount = value; }
    }
    public string ItemFee
    {
        get { return _itemFee; }
        set { _itemFee = value; }
    }
    public string TransRate
    {
        get { return _transRate; }
        set { _transRate = value; }
    }

    public void Set(DataRow dr)
    {
        ShortName = Convert.ToString(dr["ShortName"]);
        Amount = Convert.ToString(dr["Amount"]);
        TransRate = Convert.ToString(dr["TransRate"]);
        ItemFee = Convert.ToString(dr["ItemFee"]);
    }

}