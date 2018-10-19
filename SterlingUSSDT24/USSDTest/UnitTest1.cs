using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomerAccount;

namespace USSDTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            AccountLookupInfo inf = new AccountLookupInfo();
            inf.SessionId = "4";
            inf.ClientMobileNo = "2348039590420";
            inf.RequestMessage = "*822*6*0007891725#";
            string resp = CustomerAccount_DAL.getAccountBalance(inf);
            
        }
    }
}
