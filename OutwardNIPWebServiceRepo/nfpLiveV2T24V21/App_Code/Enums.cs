using System;


public enum ChannelCode : int
{
    Bank_Teller = 1,
    Internet_Banking = 2,
    Mobile_Phones = 3,
    POS_Terminal = 4,
    ATM = 5,
    Web_Portal = 6,
    Third_Party = 7
}


public class Record
{
    public string RecID;
    public string AccountNumber;
    public string AccountName;
    public string OriginatorName;
    public string Narration;
    public string PaymentReference;
    public string Amount;
    public string ResponseCode;

    public string BillerName;
    public string BillerID;
    public string MandateReferenceNumber;

    public string InstitutionCode;
    public string InstitutionName;
    public string Category;
}

