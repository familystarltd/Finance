using System;
using System.Domain;
using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Domain.Aggregates.FeeAgg;
namespace Finance.Domain.Aggregates.DisbursementAgg
{
    public class Disbursement : Entity
    {
        /// <summary>
        /// Get or set the Date for this Disbursement which is happen to the customer
        /// </summary>
        public DateTime DisbursementDate { get; set; }
        /// <summary>
        /// Get or set the customer for this Disbursement which is happen to the customer
        /// </summary>
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        /// <summary>
        /// Get or set the funding source for this Disbursement which is happen to the customer
        /// </summary>
        public Guid? FunderId { get; set; }
        public Funder Funder { get; set; }
        /// <summary>
        /// Get or set the Disbursement which is happen to the customer
        /// </summary>
        public Guid ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }
        /// <summary>
        /// Get or set associated document for this Disbursement which is happen to the customer
        /// </summary>
        public byte[] Document { get; set; }
        /// <summary>
        /// Get or set the Charge of this Disbursement which is happen to the customer
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Get or set the date of last processed of the Customer's Disbursement 
        /// </summary>
        public DateTime? LastProcessedDate { get; set; }
    }
}