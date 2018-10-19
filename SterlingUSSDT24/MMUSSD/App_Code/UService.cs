using System;
using System.Xml;

public class UService
{
    public void saveTrnx(UReq req, UResp resp)
    {
        string sql = "insert into tbl_USSD_trnx (sessionid, req_msisdn,req_type,req_msg," +
            "resp_type,resp_msg,resp_cost,resp_ref,dateAdded) values " +
            " (@ssi,@rqn,@rqt,@rqm,@rst,@rsm,@rsc,@rsr,getdate())";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@rqn", req.Msisdn);
        cn.AddParam("@rqt", req.Mtype);
        cn.AddParam("@rqm", req.Msg);
        cn.AddParam("@rst", resp.Mtype);
        cn.AddParam("@rsm", resp.Msg);
        cn.AddParam("@rsc", resp.Cost);
        cn.AddParam("@rsr", resp.Ref);
        cn.Insert();
    }

    public void saveState(UReq req)
    {
        string sql = "select * from tbl_USSD_reqstate where sessionid = @ssi and msisdn=@msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.Persist = true;
        cn.SetSQL(sql);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        cn.Select();
        if (cn.num_rows > 0)
        {
            sql = "update tbl_USSD_reqstate set op_id=@opi,sub_op_id= @soi, lastUpdated = getdate()" +
                " where sessionid=@ssi and msisdn = @msi"; 
            cn.SetSQL(sql);
            cn.AddParam("@ssi", req.SessionID);
            cn.AddParam("@msi", req.Msisdn);
            cn.AddParam("@opi", req.op);
            cn.AddParam("@soi", req.sub_op);
            cn.Update();
        }
        else
        {
            sql = "insert into tbl_USSD_reqstate (sessionid, msisdn,op_id, sub_op_id,lastUpdated,AppID)" +
                " values (@ssi,@msi,@opi,@soi,getdate(),@AppID)";
            cn.SetSQL(sql);
            cn.AddParam("@ssi", req.SessionID);
            cn.AddParam("@msi", req.Msisdn);
            cn.AddParam("@opi", req.op);
            cn.AddParam("@soi", req.sub_op);
            cn.AddParam("@AppID", 1);
            cn.Insert();
        }
        cn.CloseAll();
    }

    public UReq readRequest(string xmltext)
    {
        UReq req = new UReq();

        try
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmltext);
            req.Msisdn = xml.GetElementsByTagName("msisdn").Item(0).InnerText;
            req.SessionID = xml.GetElementsByTagName("sessionid").Item(0).InnerText;
            req.Mtype = Convert.ToInt32(xml.GetElementsByTagName("type").Item(0).InnerText);
            req.Msg = xml.GetElementsByTagName("msg").Item(0).InnerText;
            req.Network = xml.GetElementsByTagName("network").Item(0).InnerText;
            req.ok = true;
        }
        catch
        {
            req.ok = false;
        }
        return req;
    }

    public string writeResponse(UResp resp)
    {
        resp.Msg = resp.Msg.Replace("%0A", "&#10;");
        string bdat = "<?xml version=\"1.0\" ?><ussd>" +
            "<type>" + resp.Mtype.ToString() + "</type>" +
            "<msg>" + resp.Msg + "</msg>" +
            "</ussd>";
        //new ErrorLog(bdat);
        //new ErrorLog("I got to creating response");
        //new ErrorLog("Response returned for " + bdat);
        return bdat;
    }
}