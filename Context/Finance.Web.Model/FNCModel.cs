using System;
using System.Collections.Generic;
namespace Finance.Web.Model
{
    public class FNCModel
    {
        /// <summary>
        /// Id for the Fee model which is Unique Identifier
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Get or Set the Description for this FNC
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Get or Set the associated Payer for this FNC
        /// </summary>
        public Guid PayerId { get; set; }
        public PayerModel Payer { get; set; }
        /// <summary>
        /// Get or Set the Payment ContactReference for this FNC (If it is NULL or Empty generated Invoice No will be the Pay ContactReference)
        /// </summary>
        public string PayReference { get; set; }
        /// <summary>
        /// Get or Set the Payment Cycle for this FNC
        /// </summary>
        public PaymentTerm PaymentTerm { get; set; }
        public ICollection<FNCCustomerModel> FNCCustomers { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
    public class FNCCustomerModel
    {
        /// <summary>
        /// Id for the Fee model which is Unique Identifier
        /// </summary>
        public Guid Id { get; set; }
        public Guid FNCId { get; set; }
        public FNCModel FNC { get; set; }
        public string CustomerRef { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerModel Customer { get; set; }
        public ICollection<FNCRateModel> FNCRates { get; set; }
    }
    public class FNCRateModel
    {
        public Guid FNCCustomerId { get; set; }
        public FNCCustomerModel FNCCustomer { get; set; }
        public RateMethod RateMethod { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
}