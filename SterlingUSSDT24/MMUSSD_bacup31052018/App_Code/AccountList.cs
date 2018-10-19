using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AccountList
/// </summary>
public class AccountList
{
    private string _nuban;
    private string _transRate;

    public string Nuban
    {
        get { return _nuban; }
        set { _nuban = value; }
    }
    public string TransRate
    {
        get { return _transRate; }
        set { _transRate = value; }
    }

    public void Set(DataRow dr)
    {
        Nuban = Convert.ToString(dr["Nuban"]);
        TransRate = Convert.ToString(dr["TransRate"]);
    }
}