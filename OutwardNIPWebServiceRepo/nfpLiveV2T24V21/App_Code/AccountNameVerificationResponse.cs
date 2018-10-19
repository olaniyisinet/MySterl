using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for AccountNameVerificationResponse
/// </summary>
public class AccountNameVerificationResponse
{
    public AccountNameVerificationResponse() { }
    public string Status;
    public string StatusMessage;
    public string AccountName;
    public string KYCLevel;
    public string BVN;

    public string BranchCode;

    public string CustomerNumber;

    public string CurrencyCode;

    public string LedgerCode;

    public string AccountStatus;
    

    public AccountNameVerificationResponse(string sts, string stsmsg, string acctnm, string kyc, string bv, 
        string branchcode, string customernumber, string currencycode, string ledgercode,string accountstatus)
    {
        Status = sts;
        StatusMessage = stsmsg;
        AccountName = acctnm;
        KYCLevel = kyc;
        BVN = bv;
        BranchCode = branchcode;
        CustomerNumber = customernumber;
        CurrencyCode = currencycode;
        LedgerCode = ledgercode;
        AccountStatus = accountstatus;
    }
}