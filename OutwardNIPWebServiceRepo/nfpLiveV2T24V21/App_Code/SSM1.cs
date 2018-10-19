using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Sockets;
using System.Text;
using System.Net;

/// <summary>
/// Summary description for SSM1
/// </summary>
public class SSM1
{
        //private string username = "qwerty";
    private string username = "sterlingnibsstest@test.com";
    private string password = "Pass123";
    protected Socket s;
	public SSM1()
	{
        s = ConnectSocket("127.0.0.1", 8088);
        //s = ConnectSocket("127.0.0.1", 9095);
        //s = ConnectSocket("127.0.0.1", 8088);
	}

    public void createKeys()
    {
        Byte[] bytesSent = Encoding.ASCII.GetBytes("GEN" + username + "#" + password);
        s.Send(bytesSent, bytesSent.Length, 0);
    }

    public void closeSSM()
    {
        s.Disconnect(false);
        s.Close();
    }

    public string enkrypt(string message)
    {

        new ErrorLog(message);
        Byte[] bytesSent = Encoding.ASCII.GetBytes("ENC" + message);
        Byte[] bytesReceived = new Byte[10000000];

        s.Send(bytesSent, bytesSent.Length, 0);
        int bytes = 0;
        string page = "";
        do
        {
            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
        }
        while (bytes > 0);

        new ErrorLog(page);
        closeSSM();
        return page.Substring(3,page.Length - 3);
    }

    public string dekrypt(string message)
    {
        Byte[] bytesSent = Encoding.ASCII.GetBytes("DEC" + password + "#" + message);
        Byte[] bytesReceived = new Byte[10000000];

        s.Send(bytesSent, bytesSent.Length, 0);
        int bytes = 0;
        string page = "";
        do
        {
            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
        }
        while (bytes > 0);
        new ErrorLog(page);
        closeSSM();
        return page.Substring(3, page.Length - 3);
    }

    protected Socket ConnectSocket(string server, int port)
    {
        Socket s = null;
        IPHostEntry hostEntry = null;
        hostEntry = Dns.GetHostEntry(server);
        foreach (IPAddress address in hostEntry.AddressList)
        {
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            tempSocket.Connect(ipe);
            if (tempSocket.Connected)
            {
                s = tempSocket;
                break;
            }
            else
            {
                continue;
            }
        }
        return s;
    }

    public string sha256(string message)
    {
        //string HashKey = "DBEECACCB4210977ACE73A1D873CA59F";
        string HashKey = "806BC2C8CD945164703D34FB0EB05EB0";
        Byte[] bytesSent = Encoding.ASCII.GetBytes("SHA" + HashKey + "#" + message);
        Byte[] bytesReceived = new Byte[10000000];

        s.Send(bytesSent, bytesSent.Length, 0);
        int bytes = 0;
        string page = "";
        do
        {
            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
        }
        while (bytes > 0);
        //new ErrorLog(page);
        return page.Substring(3, page.Length - 3);
    }
}
