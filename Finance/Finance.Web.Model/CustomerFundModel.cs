//using System;
//using System.Collections.Generic;

//namespace Finance.Web.Model
//{
//    /// <summary>
// /// Funding setup DTO for the Customer Pay
// /// </summary>
//    public class CustomerPayModel
//    {
//        /// <summary>
//        /// Get or set the type of funding
//        /// </summary>
//        public int PayType { get; set; }
//        /// <summary>
//        /// Get or set the customer for this Customer Funding
//        /// </summary>
//        public Guid CustomerId { get; set; }
//        public CustomerModel Customer { get; set; }
//        /// <summary>
//        /// Get or set the funding source for this Customer Funding
//        /// </summary>
//        public Guid? FunderId { get; set; }
//        public FunderModel Funder { get; set; }
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
//        public Guid? FeeId { get; set; }
//        public FeeModel Fee { get; set; }
//    }
//}