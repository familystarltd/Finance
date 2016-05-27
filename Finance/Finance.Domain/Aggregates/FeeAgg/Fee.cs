namespace Finance.Domain.Aggregates.FeeAgg
{
    using System;
    using System.Collections.Generic;
    using System.Domain;
    using Finance.Domain.Aggregates.CustomerAgg;
    // UK Bank Holidays
    //https://www.gov.uk/bank-holidays.json

    /// <summary>
    /// Fee for the Customer
    /// </summary>
    public class Fee : Entity
    {
        /// <summary>
        /// Get or Set the Name for this fee
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or Set the Description for this fee
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Get or Set the Rate collections for this Fee
        /// </summary>
        public virtual ICollection<FeeRate> FeeRates { get; set; }
        /// <summary>
        /// Get or Set the collection of Notes for this Fee
        /// </summary>
        public virtual ICollection<FeeNote> Notes { get; set; }
        /// <summary>
        /// Get or Set the associated Customer for this fee
        /// </summary>
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        /// <summary>
        /// Get or Set the associated Funder for this fee
        /// </summary>
        public Guid FunderId { get; set; }
        public Funder Funder { get; set; }
        /// <summary>
        /// Get or Set the Payment ContactReference for this fee (If it is NULL or Empty generated Invoice No will be the Pay ContactReference)
        /// </summary>
        public string PayContactReference { get; set; }
        /// <summary>
        /// Get or Set the Payment Cycle for this Fee
        /// </summary>
        public PaymentTerm PaymentTerm { get; set; }
        /// <summary>
        /// Get or Set the EffectiveDate for this Fee
        /// </summary>
        public DateTime EffectiveDate { get; set; }
        /// <summary>
        /// Get or Set the ClosingDate for this Fee
        /// </summary>
        public DateTime? ClosingDate { get; set; }
    }
}