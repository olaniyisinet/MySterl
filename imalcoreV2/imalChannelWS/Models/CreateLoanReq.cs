using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imalChannelWS.Models
{
    /// <summary>
    /// Create Loan Request Object
    /// </summary>
    public class CreateLoanReq
    {
        /// <summary>
        /// Gets or sets the request code.
        /// </summary>
        /// <value>
        /// The request code.
        /// </value>
        public string requestCode { get; set; }
        /// <summary>
        /// Gets or sets the principal identifier.
        /// </summary>
        /// <value>
        /// The principal identifier.
        /// </value>
        public string principalIdentifier { get; set; }
        /// <summary>
        /// Gets or sets the reference code.
        /// </summary>
        /// <value>
        /// The reference code.
        /// </value>
        public string referenceCode { get; set; }

        public float branchCode { get; set; }
        public float productClass { get; set; }
        public float counterParty { get; set; }
        public float dealCy { get; set; }
        public float dealAmount { get; set; }

        public DateTime dealDate { get; set; }
        public DateTime valueDate { get; set; }
        public DateTime maturityDate { get; set; }

        public float floatingRate { get; set; }
        public string contribType { get; set; }
        public float contribYield { get; set; }

        public float nostroAccBr { get; set; }
        public float nostroAccCy { get; set; }
        public float nostroAccGl { get; set; }
        public float nostroAccCif { get; set; }
        public float nostroAccSl { get; set; }
    }
}