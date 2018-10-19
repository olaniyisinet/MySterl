using System;
using System.Collections.Generic;
using System.Web;
using System.Net.Sockets;
using System.Net;
using System.Text;

/// <summary>
/// Summary description for SSM2
/// </summary>
public class SSM2
{
    private string username = "sterlingnibsstest@test.com";
    private string password = "Pass123";
    protected Socket s;
    public SSM2()
    {
        s = ConnectSocket("127.0.0.1", 8082);
    }
    public void createKeys()
    {
        Byte[] bytesSent = Encoding.ASCII.GetBytes("GEN" + username + "#" + password);
        s.Send(bytesSent, bytesSent.Length, 0);
    }

    public string enkrypt(string message)
    {
        Byte[] bytesSent = Encoding.ASCII.GetBytes("ENC" + message);
        Byte[] bytesReceived = new Byte[10000000];

        s.Send(bytesSent, bytesSent.Length, 0);
        int bytes = 0;
        string page = "";
        do
        {
            try
            {

                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            catch { }
        }
        while (bytes > 0);
        return page.Substring(3, page.Length - 3);
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
}