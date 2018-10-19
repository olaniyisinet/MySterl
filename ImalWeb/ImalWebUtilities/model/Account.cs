using System;


namespace ImalWebUtilities.model
{
    public class Account
    {
        public int CompanyCode { get; set; }
        public int BranchCode { get; set; }
        public int CurrencyCode { get; set; }
        public int GeneralLedgerCode { get; set; }
        public int CustomerInformationFileSubNumber { get; set; }
        public int SlNo { get; set; }
        public string CustomerShortName { get; set; }
        public string CustomerLongName { get; set; }
        public DateTime DateOpened { get; set; }
        public string DateClosed { get; set; }
        public decimal LedgerBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Telephone { get; set; }
        public string Nuban { get; set; }
        public string Status { get; set; }
        public string DateReinstated { get; set; }

    }
}