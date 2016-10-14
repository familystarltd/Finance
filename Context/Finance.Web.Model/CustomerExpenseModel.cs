using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Finance.Web.Model
{
    public class CustomerDisbursementPayerModel
    {
        public CustomerDisbursementPayerModel() { }
        public Guid Id { get; set; }
        /// <summary>
        /// Get or Set the associated Customer for this fee
        /// </summary>
        public Guid CustomerId { get; set; }
        public virtual CustomerModel Customer { get; set; }

        /// <summary>
        /// Get or set the Payers who funding Disbursements to this Customer Disbursement Payer.
        /// </summary>
        public Guid? PayerId { get; set; }
        public PayerModel Payer { get; set; }

        public ICollection<DisbursementModel> Disbursements { get; set; }
        /// <summary>
        /// Get or set the Active date for this customer
        /// </summary>
        public DateTime ActiveDate { get; set; }
        /// <summary>
        /// Get or set date of deactivate for this customer
        /// </summary>
        public DateTime? DeactiveDate { get; set; }
    }
    public class DisbursementViewModel
    {
        public Guid CustomerId { get; set; }
        public CustomerModel Customer { get; set; }
        public DisbursementModel DisbursementModel { get; set; }
    }
    /// <summary>
    /// Expense for the Customer
    /// </summary>
    public class ExpenseModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Get or set the Given name of this expense
        /// </summary>
        public string Name { get; set; }
    }
    public class DisbursementModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Get or set the Date for this expense which is happen to the customer
        /// </summary>
        public DateTime DisbursementDate { get; set; }
        /// <summary>
        /// Get or set the customer for this expense which is happen to the customer
        /// </summary>
        public Guid CustomerId { get; set; }
        public CustomerModel Customer { get; set; }
        /// <summary>
        /// Get or set the funding source for this expense which is happen to the customer
        /// </summary>
        public Guid PayerId { get; set; }
        public PayerModel Payer { get; set; }
        /// <summary>
        /// Get or set the Expense which is happen to the customer
        /// </summary>
        public Guid ExpenseId { get; set; }
        public ExpenseModel Expense { get; set; }
        /// <summary>
        /// Get or set associated document for this expense which is happen to the customer
        /// </summary>
        public byte[] Document { get; set; }
        /// <summary>
        /// Get or set the Charge of this Expense which is happen to the customer
        /// </summary>
        [DataType(DataType.Currency, ErrorMessage = "Please enter valid amount")]
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        [Range(0.01, 9999999999, ErrorMessage = "Please enter valid amount")]
        public decimal Amount { get; set; }
        /// <summary>
        /// Get or set the date of last processed of the Customer's Expense 
        /// </summary>
        public DateTime? LastProcessedDate { get; set; }
    }
}