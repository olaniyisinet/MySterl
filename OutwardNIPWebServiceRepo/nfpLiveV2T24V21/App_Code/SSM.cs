using System;
using System.Data;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class SSM
{
    //private string username = "qwerty";
    private string username = "sterlingnibsstest@test.com";
    private string password = "Pass123";
    protected Socket s;
    public SSM()
    {
        //s = ConnectSocket("127.0.0.1", 8088);
        s = ConnectSocket("127.0.0.1", 8087);
    }

    public void createKeys()
    {

        try
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes("GEN" + username + "#" + password);
            s.Send(bytesSent, bytesSent.Length, 0);
        }
        catch (Exception ex)
        {
            new ErrorLog(ex);
        }
    }

    public string enkrypt(string message)
    {
        string page = "";

        //new ErrorLog(message);
        try
        {

            Mylogger1.Info(message);
            //new ErrorLog("Raw Request to be encrpyted " + message);
            Byte[] bytesSent = Encoding.ASCII.GetBytes("ENC" + message);
            Byte[] bytesReceived = new Byte[10000000];

            s.Send(bytesSent, bytesSent.Length, 0);
            int bytes = 0;

            do
            {
                bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);
        }
        catch (Exception ex)
        {
            Mylogger1.Info("SSM exception occurred as >> " + ex + " for request " + message);
            //new ErrorLog("SSM exception occurred as >> " + ex + " for request " + message);
        }

        //new ErrorLog(page);
        Mylogger1.Info(page);
        //new ErrorLog("The encrypted message returned is " + page + " for the request " + message);
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
        new ErrorLog(page);
        return page.Substring(3, page.Length - 3);
    }

    protected Socket ConnectSocket(string server, int port)
    {
        Socket s = null;
        IPHostEntry hostEntry = null;
        hostEntry = Dns.GetHostEntry(server);
        foreach (IPAddress address in hostEntry.AddressList)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
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
                catch (Exception ex)
                {
                    if (tempSocket != null)
                        tempSocket.Close();

                }
            }
        }
        return s;
    }
}
