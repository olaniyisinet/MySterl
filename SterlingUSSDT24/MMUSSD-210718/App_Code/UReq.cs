public class UReq
{
    public string SessionID;
    public string Msisdn;
    public int Mtype;
    public string Msg;
    public string Network;

    public UReq()
    {
        this.Mtype = 0;
        this.Msisdn = "";
        this.Msg = "";
        this.SessionID = "";
        this.Network = "";
    }

    public int op;
    public int sub_op;
    public int next_op;

    public string Menu;
    public string Method;
    public string MethodSub;

    public bool ok;
}