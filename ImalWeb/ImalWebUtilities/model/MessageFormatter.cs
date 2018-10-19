using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImalWebUtilities.model
{
    public class MessageFormatter
    {
        public static string Error(string message)
        {
            return "<p class='alert bg-danger fg-red'>" + message + "</p>";
        }

        public static string Success(string message)
        {
            return "<p class='alert bg-success fg-red'>" + message + "</p>";
        }
    }
}