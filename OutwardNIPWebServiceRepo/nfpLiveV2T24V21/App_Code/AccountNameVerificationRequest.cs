using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for AccountNameVerificationRequest
/// </summary>
public class AccountNameVerificationRequest
{
    AccountNameVerificationRequest() { }
    public string InstitutionCode;
    public string AccountNumber;

    public AccountNameVerificationRequest(string Instcode, string AcctNum)
    {
        InstitutionCode = Instcode;
        AccountNumber = AcctNum;
    }
}