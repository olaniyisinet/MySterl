using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BeneList
/// </summary>
public class BeneList
{
    private string _BeneBank;
    private string _transRate;
    private string _BeneName;
    private string _nuban;

    public string BeneBank
    {
        get { return _BeneBank; }
        set { _BeneBank = value; }
    }
    public string TransRate
    {
        get { return _transRate; }
        set { _transRate = value; }
    }

    public string BeneName
    {
        get { return _BeneName; }
        set { _BeneName = value; }
    }

    public string Nuban
    {
        get { return _nuban; }
        set { _nuban = value; }
    }

    public void Set(DataRow dr)
    {
        BeneBank = Convert.ToString(dr["BeneficiaryBank"]);
        BeneName = Convert.ToString(dr["BeneficiaryName"]);
        TransRate = Convert.ToString(dr["TransRate"]);
        Nuban = Convert.ToString(dr["Nuban"]);
    }
}