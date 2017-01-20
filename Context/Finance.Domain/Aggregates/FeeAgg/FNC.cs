namespace Finance.Domain.Aggregates.FeeAgg
{
    using System;
    using System.Collections.Generic;
    using System.Domain;
    using Finance.Domain.Aggregates.CustomerAgg;

    /// <summary>
    /// FNC (Funded Nursing Care ) for the Customer
    /// </summary>
    public class FNC : Entity
    {
        /// <summary>
        /// Get or Set the Description for this FNC
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Get or Set the associated Payer for this FNC
        /// </summary>
        public Guid PayerId { get; set; }
        public Payer Payer { get; set; }
        /// <summary>
        /// Get or Set the Payment Reference for this FNC (If it is NULL or Empty generated Invoice No will be the Pay ContactReference)
        /// </summary>
        public string PayReference { get; set; }
        /// <summary>
        /// Get or Set the Payment Cycle for this FNC
        /// </summary>
        public PaymentTerm PaymentTerm { get; set; }
        public ICollection<FNCCustomer> FNCCustomers { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
    public class FNCCustomer : Entity
    {
        public Guid FNCId { get; set; }
        public FNC FNC { get; set; }
        public string CustomerRef { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public ICollection<FNCRate> FNCRates { get; set; }
    }
    public class FNCRate:Entity
    {
        public Guid FNCCustomerId { get; set; }
        public FNCCustomer FNCCustomer { get; set; }
        public RateMethod RateMethod { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ClosingDate { get; set; }

    }
}