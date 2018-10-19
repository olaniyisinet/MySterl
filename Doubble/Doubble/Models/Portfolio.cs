using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doubble.Models
{
    public class Portfolio
    {
        public string ArrangementId { get; set; }
        public string InvestmentType { get; set; }
        public string TotalInvestmentValue { get; set; }
        public string TotalContributions { get; set; }
        public string InterestEarned { get; set; }
        public string PayInAccount { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAccount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public string PayInAmount { get; set; }
        public string BeneficiaryType { get; set; }

    }
}