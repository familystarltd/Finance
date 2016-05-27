using Finance.Domain.Aggregates.DisbursementAgg;
using Finance.Domain.Aggregates.FeeAgg;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using System;
using System.Collections.Generic;
using System.Domain;
namespace Finance.Domain.Aggregates.CustomerAgg
{
    /// <summary>
    /// Aggregate Root for Customer Aggregate
    /// </summary>
    public class Customer : Entity
    {
        public Customer() { }
        /// <summary>
        /// If this customer is imported from other system, then the Id will get from that system otherwise generate manually.
        /// </summary>

        /// <summary>
        /// Get or set the sequence number for this customer from the other system
        /// this field is used only for imported customers
        /// </summary>
        public string Ref { get; set; }
        public Guid CompanyId { get; set; }
        /// <summary>
        /// Get or set the company for this customer, If this customer is imported from other system, then the company will get from system's (where the customer imported from) company settings profile.
        /// </summary>
        public virtual Company Company { get; set; }
        /// <summary>
        /// Get or set the personal contact information for this customer
        /// </summary>
        public virtual Contact PersonalInfo { get; set; }
        /// <summary>
        /// Get or set the Fees information to this customer. May have one or more Fees and also maintain the History of Fees
        /// </summary>
        public ICollection<Fee> Fees { get; set; }
        /// <summary>
        /// Get Transactios of this customer.
        /// </summary>
        public ICollection<FinancialTransaction> FinancialTransactions { get; set; }
        /// <summary>
        /// Get or set the Customer Disbursements to this customer. May have one or more Disbursements.
        /// </summary>
        public ICollection<Disbursement> Disbursements { get; set; }
        /// <summary>
        /// Get or set the Customer Disbursements to this customer. May have one or more Disbursements.
        /// </summary>
        public ICollection<CustomerDisbursementFunder> CustomerDisbursementFunders { get; set; }
        /// <summary>
        /// Get or set the current active Funder who funding Disbursements to this customer.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Funder DisbursementFunder { get; set; }
        /// <summary>
        /// Get or set the Active date for this customer
        /// </summary>
        public DateTime ActiveDate { get; set; }
        /// <summary>
        /// Get or set if this customer is disabled
        /// </summary>
        public bool Deactive { get; set; }
        /// <summary>
        /// Get or set date of deactivate for this customer
        /// </summary>
        public DateTime? DeactiveDate { get; set; }
        /// <summary>
        /// Get or set reasons of deactivate for this customer
        /// </summary>
        public string DeactiveReasons { get; set; }
        /// <summary>
        /// Get or set Invoice Total
        /// </summary>
        public decimal Debits { get; set; }
        /// <summary>
        /// Get or set Received Total (mainly from receipts)
        /// </summary>
        public decimal Credits { get; set; }
    }
}