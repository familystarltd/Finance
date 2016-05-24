//namespace FinanceManagement.Domain.Aggregates.CustomerAgg
//{
//    using System;
//    using System.Domain;
//    using FeeAgg;
//    public enum PayType : int
//    {
//        Disbursements=1,
//        Fees=2
//    }

//    /// <summary>
//    /// Funding setup for the Customer
//    /// </summary>
//    public class CustomerPay : Entity
//    {
//        /// <summary>
//        /// Get or set the type of funding
//        /// </summary>
//        public PayType PayType { get; set; }
//        /// <summary>
//        /// Get or set the customer for this Customer Funding
//        /// </summary>
//        public Guid CustomerId { get; set; }
//        public Customer Customer { get; set; }
//        /// <summary>
//        /// Get or set the funding source for this Customer Funding
//        /// </summary>
//        public Guid? FunderId { get; set; }
//        public Funder Funder { get; set; }
//        /// <summary>
//        /// Get or Set the Payment ContactReference for this customer funding (If it is NULL or Empty generated Invoice No will be the Pay ContactReference)
//        /// </summary>
//        public string PayContactReference { get; set; }
//        /// <summary>
//        /// Get or Set the Payment Cycle for this Fee
//        /// </summary>
//        public PaymentTerm PaymentTerm { get; set; }

//        /// <summary>
//        /// Get or set the active date for this customer funding source
//        /// </summary>
//        public DateTime ActiveDate { get; set; }
//        /// <summary>
//        /// Get or set the inactive date for this customer funding source
//        /// </summary>
//        public DateTime? InactiveDate { get; set; }
//        /// <summary>
//        /// Get or set the Fees information to this customer funding. May have one or more Fees and also maintain the History of Fees
//        /// </summary>
//        //public Guid? FeeId { get; set; }
//        //public Fee Fee { get; set; }
//    }
//}