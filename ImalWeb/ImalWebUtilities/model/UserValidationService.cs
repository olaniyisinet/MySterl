using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Reflection;
using log4net;


namespace ImalWebUtilities.model
{
    public class UserValidationService
    {
        public  string ErrorMessage { get; set; }
        
        public  bool IsAuthenticated(string usr, string pwd)
        {
            bool authenticated = false;

            try
            {
                var entry = new DirectoryEntry("LDAP://sterlingbank", @"sterlingbank\"+usr, pwd);
                object nativeObject = entry.NativeObject;
                authenticated = true;
                
            }
            catch (DirectoryServicesCOMException cex)
            {
                ErrorMessage = cex.Message;

            }
            catch (Exception ex)
            {
                //not authenticated due to some other exception [this is optional]
                ErrorMessage = ex.Message;
            }
            return authenticated;
        }
    }
}