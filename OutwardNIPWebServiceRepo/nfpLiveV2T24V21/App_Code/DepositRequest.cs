using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for DepositRequest
/// </summary>
public class DepositRequest
{
    DepositRequest() { }
    public string InstitutionCode;
    public string SourceAccountNumber;
    public string SourceAccountName;
    public string DestinationAccountNumber;
    public string DestinationAccountName;
    public string SourceBankCode;
    public string ReferenceNumber;
    public string Amount;
    public string Narration;
    public string SessionID;


    public DepositRequest(string Instcode, string suractNum, string suracctName, string destactAtnum, string desctacctName, string refe, string amt, string narr)
    {
        InstitutionCode = Instcode;
        SourceAccountNumber = suractNum;
        SourceAccountName = suracctName;
        DestinationAccountNumber = destactAtnum;
        DestinationAccountName = desctacctName;
        ReferenceNumber = refe;
        Amount = amt;
        Narration = narr;
    }

    public DepositRequest(string Instcode, string suractNum, string suracctName, string destactAtnum, string desctacctName, string sourcebankcode, string refe, string amt, string narr, string sessionId)
    {
        InstitutionCode = Instcode;
        SourceAccountNumber = suractNum;
        SourceAccountName = suracctName;
        DestinationAccountNumber = destactAtnum;
        DestinationAccountName = desctacctName;
        SourceBankCode = sourcebankcode;
        ReferenceNumber = refe;
        Amount = amt;
        Narration = narr;
        SessionID = sessionId;

    }
}