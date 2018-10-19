 
using System;
using System.Collections.Generic;
using System.Data;

public class MNO
{
    private string _mnoid;
    private string _mnoName;

    public string MNOID
    {
        get { return _mnoid; }
        set { _mnoid = value; }
    }

    public string MNOName
    {
        get { return _mnoName; }
        set { _mnoName = value; }
    }

    public void Set(DataRow dr)
    {
        MNOID = Convert.ToString(dr["moID"]);
        MNOName = Convert.ToString(dr["moName"]); 
    }
}

public class  MNOService
{
    public List<MNO> GetMOByPage(string page)
    {
        List<MNO> lb = new List<MNO>();
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

        string sql = " select top " + pageSize + " * from tbl_mobile_ops " +
            " where moid not in " +
            " (select top " + offset.ToString("0") + " moid from tbl_mobile_ops order by moid) " +
            " order by moid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        DataSet ds = cn.Select();
        if(cn.num_rows > 0)
        {
            for(int i = 0; i < cn.num_rows; i++)
            {
                MNO b = new MNO();
                b.Set(ds.Tables[0].Rows[i]);
                lb.Add(b);
            }
        }
        return lb;
    }

    public List<MNO> GetMOs()
    {
        List<MNO> lb = new List<MNO>();
        string sql = " select   * from tbl_mobile_ops order by moid";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        DataSet ds = cn.Select();
        if (cn.num_rows > 0)
        {
            for (int i = 0; i < cn.num_rows; i++)
            {
                MNO b = new MNO();
                b.Set(ds.Tables[0].Rows[i]);
                lb.Add(b);
            }
        }
        return lb;
    }

    public string GetMOName(string moName)
    {
        string sql = "select moname from tbl_mobile_ops where moid = @bc";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@bc", moName);
        return cn.SelectScalar();
    }
}