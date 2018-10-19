using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelTradeRoleMgt
{
  public  class Utils
    {
        internal static string GetConnectionStringWorkflow()
        {
            string constr = string.Empty;
            constr = "Data Source=10.0.0.211,1490;Initial Catalog=WorkflowDB;User ID=appusr;Password=(#usr4*); Application Name=NovelTradeFT";
            
            return constr;
        }

        internal static string GetConnectionStringTrade()
        {
            string constr = string.Empty;
            //constr = "Data Source=10.0.0.211,1490;Initial Catalog=TradeServicesTest;User ID=Trade;Password=(64*radeT); Application Name=NovelTradeFT";
           constr = "Data Source=10.0.41.99;Initial Catalog=TradeServicesTest;User ID=sa;Password=system;Application Name=NovelTradeFT";
           // constr = "Data Source=10.0.41.250,1490;Initial Catalog=TradeServicesTest;User ID=appusr;Password=appusr;Application Name=NovelTradeFT";
            return constr;
        }
    }
}
