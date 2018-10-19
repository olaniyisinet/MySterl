using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalcore
{
    public class Gadget
    {
        public string gettransdate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
        public string getReference()
        {
            DateTime dt = new DateTime();
            return "11113243" + dt.TimeOfDay;
        }
        

    }
   
}