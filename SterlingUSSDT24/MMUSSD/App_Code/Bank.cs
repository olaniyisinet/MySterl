 
using System;
using System.Collections.Generic;
using System.Data;
using MMWS;

public class Bank
{
    private string _bankCode;
    private string _bankName;
    private string _bankShort;
    private string _transRate;
    public string BankCode
    {
        get { return _bankCode; }
        set { _bankCode = value; }
    }

    public string BankName
    {
        get { return _bankName; }
        set { _bankName = value; }
    }

    public string BankShort
    {
        get { return _bankShort; }
        set { _bankShort = value; }
    }

    public string TransRate
    {
        get { return _transRate; }
        set { _transRate = value; }
    }

    public void Set(DataRow dr)
    {
        BankCode = Convert.ToString(dr["bankcode"]);
        BankName = Convert.ToString(dr["BankName"]);
        BankShort = Convert.ToString(dr["BankShort"]);
        TransRate = Convert.ToString(dr["TransRate"]);
    }

     
}

public class  BankService
{
    public List<Bank> GetBanksByPage(string page)
    {
        List<Bank> lb = new List<Bank>();
        int pg = 0;
        try
        {
            pg = Convert.ToInt32(page);
        }
        catch
        {
        }

        int pageSize = 8;
        float offset = pg*pageSize;

        //string sql = " select top " + pageSize + " * from tbl_banks " +
        //    " where bankshort not in " +
        //    " (select top " + offset.ToString("0") + " bankshort from tbl_banks order by bankshort) " +
        //    " order by bankshort";

        string sql = " SELECT top " + pageSize + " * FROM tbl_participatingBanks " +
                     " where category = 2 and statusflag = 1 and bankshort is not null and bankshort not in ('STERLING')" +
                     " and bankshort not in " +
                     " (select top " + offset.ToString("0") +
                     " bankshort  from tbl_participatingBanks " +
                     " where category = 2 and statusflag = 1 and bankshort not in ('STERLING') order by TransRate asc) " +
                     " order by TransRate asc";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect("mssqlconn_nip");
        new ErrorLog("sql val " + sql);
        cn.SetSQL(sql);
        DataSet ds = cn.Select();
        if(cn.num_rows > 0)
        {
            for(int i = 0; i < cn.num_rows; i++)
            {
                Bank b = new Bank();
                b.Set(ds.Tables[0].Rows[i]);
                lb.Add(b);
            }
        }
        return lb;
    }

    public List<Bank> GetBanks()
    {
        List<Bank> lb = new List<Bank>();
        string sql = " select   * from tbl_banks order by bankshort";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        DataSet ds = cn.Select();
        if (cn.num_rows > 0)
        {
            for (int i = 0; i < cn.num_rows; i++)
            {
                Bank b = new Bank();
                b.Set(ds.Tables[0].Rows[i]);
                lb.Add(b);
            }
        }
        return lb;
    }

    public string GetBankName(string bankcode)
    {
        string sql = "select bankshort from tbl_banks where bankcode = @bc";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@bc", bankcode);
        return cn.SelectScalar();
    }
}