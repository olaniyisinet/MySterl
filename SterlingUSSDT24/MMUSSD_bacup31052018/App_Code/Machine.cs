using System;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;

public class Machine: BaseEngine
{
    public int Mtype;

    public string getMenu(int i, int j)
    {
        //connect to database and get 
        string sql = "select op_menu from tbl_USSD_states where op_id = @i and op_resp = @j";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@i", i);
        cn.AddParam("@j", j);
        return cn.SelectScalar(); 
    }

    public string getFXN(int i, int j)
    {
        //connect to database and get 
        string sql = "select op_method from tbl_USSD_states where op_id = @i and op_resp = @j";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@i", i);
        cn.AddParam("@j", j);
        return cn.SelectScalar();
    }

    public string doNextOperation(ref UReq req)
    {
        Mtype = 2;
        if(req.Mtype > 1)
            getPreviousState(ref req);
         
        do
        {
            getState(ref req);
            if (req.Menu != "")
            {
                req.op = req.next_op;
                req.sub_op = 0;
                continue;
            }
            ProcessState(ref req);
        } while (req.Method != "");
        return parseMenu(req);
    }

    private void ProcessState(ref UReq req)
    {       
        if (req.next_op == -1 && req.Method == "0")
        {
            req.sub_op = Gadget.ToInt32(req.Msg);
            return;
        }

        if (req.next_op == -1 && req.Method != "0")
        {
            object k = InvokeMethod(req.Method, req.MethodSub, req);
            req.sub_op = Gadget.ToInt32(k);
            return;
        }

        if (req.next_op > 0 && req.Method == "0")
        {
            req.op = req.next_op;
            req.sub_op = 0;
            return;
        }

        if (req.next_op > 0 && req.Method != "0")
        {
            object k = InvokeMethod(req.Method, req.MethodSub, req);
            req.op = req.next_op;
            req.sub_op = Gadget.ToInt32(k);
            return;
        }
    }

    public string parseMenu(UReq req)
    {
        if (req.Menu.StartsWith("||"))
        {
            char[] c = { '|' };
            string meth = req.Menu.Trim(c);
            object k = InvokeMethod(meth, req.MethodSub, req);
            req.Menu = Convert.ToString(k);
        }

        if (req.Menu.StartsWith("::"))
        {
            char[] c = { ':' };
            string meth = req.Menu.Trim(c);
            object k = InvokeMethod(meth, req.MethodSub, req);
            req.Menu = Convert.ToString(k);
            Mtype = 3;
        }

        if (req.Menu == "-1") req.Menu = "ERROR: Contact admin; method invoke";
        if (req.Menu == "-2") req.Menu = "ERROR: Contact admin; engine invoke";
        if (req.Menu == "-3") req.Menu = "ERROR: Contact admin; api invoke";

        if (req.Menu.StartsWith("ERROR")) Mtype = 3;

        ////******************* Added this for message control *************
        //string prms = getParams(req);int cnt = 0;
        //NameValueCollection prm = splitParam(prms);
        //try
        //{
        //    cnt = RunCount(prm["cnt"]);
        //}
        //catch
        //{
        //    cnt = 0;
        //}
        

        //if(cnt > 0)
        //{
        //    req.Menu = "Confirm Set PIN: The confirmation PIN entered does not match the original PIN.  Kindly ensure you enter same PIN to proceed";
        //}
        ////********************************************************************
        return req.Menu;
    }

    public object InvokeMethod(string methodName, object objSomeType)
    {
        Type type = Type.GetType("Engine");
        if (type == null)
        {
            return -2;
        }
        object instance = Activator.CreateInstance(type);
        MethodInfo methodInfo = type.GetMethod(methodName);
        if (methodInfo == null)
        {
            return -1;
        }
        object[] obj = new Object[1] { objSomeType };
        object j = methodInfo.Invoke(instance, obj);
        return j;
    }

    public object InvokeMethod(string methodName, string className, object objSomeType)
    {
        Type type = Type.GetType(className);
        if (type == null)
        {
            return -2;
        }
        object instance = Activator.CreateInstance(type);
        MethodInfo methodInfo = type.GetMethod(methodName);
        if (methodInfo == null)
        {
            return -1;
        }
        object[] obj = new Object[1] { objSomeType };
        object j = methodInfo.Invoke(instance, obj);
        return j;
    }

    public void endSession(UReq req)
    {
        // ends a session when the customer releases
        //string sql = "delete from tbl_USSD_reqState where sessionid = @ssi and msisdn = @msi";
        //Connect2 cn = new Connect2(sql);
        //cn.AddParam("@ssi", req.SessionID);
        //cn.AddParam("@msi", req.Msisdn);
        //cn.delete();
        new ErrorLog("Ending session:" + req.SessionID);
    }

    private void getState(ref UReq req)
    {
        string sql = "select * from tbl_USSD_states where op_id = @i and op_resp = @j";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@i", req.op);
        cn.AddParam("@j", req.sub_op);
        DataSet ds = cn.Select();
        if (cn.num_rows > 0)
        {
            req.op = Convert.ToInt32(ds.Tables[0].Rows[0]["op_id"]);
            req.sub_op = Convert.ToInt32(ds.Tables[0].Rows[0]["op_resp"]);
            req.next_op = Convert.ToInt32(ds.Tables[0].Rows[0]["next_op_id"]);
            req.Menu = Convert.ToString(ds.Tables[0].Rows[0]["op_menu"]);
            req.Method = Convert.ToString(ds.Tables[0].Rows[0]["op_method"]);
            req.MethodSub = Convert.ToString(ds.Tables[0].Rows[0]["op_method_sub"]);
        }
        else
        {
            req.op = 0;
            req.sub_op = 0;
            req.next_op = 1;
            req.Menu = "";
            req.Method = "";
            req.MethodSub = "";
        }
    }

    private void getPreviousState(ref UReq req)
    {
        string sql = "select * from tbl_USSD_reqState where sessionid = @ssi and msisdn = @msi";
        Sterling.MSSQL.Connect cn = new Sterling.MSSQL.Connect();
        cn.SetSQL(sql);
        cn.AddParam("@ssi", req.SessionID);
        cn.AddParam("@msi", req.Msisdn);
        DataSet ds = cn.Select();
        if (cn.num_rows > 0)
        {
            req.op = Convert.ToInt32(ds.Tables[0].Rows[0]["op_id"]);
            req.sub_op = Convert.ToInt32(ds.Tables[0].Rows[0]["sub_op_id"]);
        }
        else
        {
            req.op = 0;
            req.sub_op = 0;
        }
    }
}