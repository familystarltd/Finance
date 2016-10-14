using Finance.Domain.Aggregates.DisbursementAgg;
using Finance.Domain.Aggregates.FeeAgg;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using System;
using System.Collections.Generic;
using System.Domain;
namespace Finance.Domain.Aggregates.CustomerAgg
{
    /// <summary>
    /// Entity for CustomerDisbursementPayer
    /// </summary>
    public class CustomerDisbursementPayer : Entity
    {
        public CustomerDisbursementPayer() { }

        /// <summary>
        /// Get or Set the associated Customer for this fee
        /// </summary>
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Get or set the Payers who funding Disbursements to this Customer Disbursement Payer.
        /// </summary>
        public Guid? PayerId { get; set; }
        public Payer Payer { get; set; }

        public ICollection<Disbursement> Disbursements { get; set; }
        /// <summary>
        /// Get or set the Active date for this customer
        /// </summary>
        public DateTime ActiveDate { get; set; }
        /// <summary>
        /// Get or set date of deactivate for this customer
        /// </summary>
        public DateTime? DeactiveDate { get; set; }
    }
}