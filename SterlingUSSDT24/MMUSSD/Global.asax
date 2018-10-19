<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // i need to do an authentication to the mFino server
        // Question? is authentication tied to mobile phone or impersonation by web service calling!
    }
    
    void Application_End(object sender, EventArgs e) 
    {
         
    }
      

     void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            System.IO.File.WriteAllText("g:\\temp\\err.txt", exc.ToString());
            Server.ClearError();
        }

    void Session_Start(object sender, EventArgs e) 
    {
        
    }

    void Session_End(object sender, EventArgs e) 
    {

    }
       
</script>
