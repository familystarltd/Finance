﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Web.Model
{
    public class CustomerViewModel
    {
        public string CompanyName { get; set; }
        public PaginationHeader Pagination { get; set; }
        public CustomerModel Customer { get; set; }
        public IEnumerable<CustomerModel> Customers { get; set; }
        public string FunderName { get; set; }
        public string CustomerName { get; set; }
    }
    public class CustomerModel
    {
        /// <summary>
        /// If this customer is imported from other system, then the Id will get from that system otherwise generate manually.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Get or set the sequence number for this customer from the other system
        /// this field is used only for imported customers
        /// </summary>
        public string Ref { get; set; }
        /// <summary>
        /// Get or set the company for this customer, If this customer is imported from other system, then the company will get from system's (where the customer imported from) company settings profile.
        /// </summary>
        public Guid CompanyId { get; set; }
        public CompanyModel Company { get; set; }
        /// <summary>
        /// Get or set the personal contact information for this customer
        /// </summary>
        public ContactModel PersonalInfo { get; set; }
        /// <summary>
        /// Get or set the Date Of Admission for this customer
        /// </summary>
        public DateTime ActiveDate { get; set; }
        /// <summary>
        /// Get or set if this customer is enabled
        /// </summary>
        public bool Deactive { get; set; }
        public DateTime? DeactiveDate { get; set; }
        public string DeactiveReasons { get; set; }
        /// <summary>
        /// Get or set Invoice Total
        /// </summary>
        public decimal Debits { get; set; }
        /// <summary>
        /// Get or set Received Total (mainly from receipts)
        /// </summary>
        public decimal Credits { get; set; }
        public DateTime? MaxFeeSetupDate { get; set; }
        public IEnumerable<FeeModel> Fees { get; set; }
        /// <summary>
        /// Get FeeInvoices of this customer.
        /// </summary>
        public ICollection<InvoiceModel> Invoices { get; set; }
        /// <summary>
        /// Get Transactios of this customer.
        /// </summary>
        public ICollection<FinancialTransactionModel> FinancialTransactions { get; set; }

        /// <summary>
        /// Get or set the Customer Disbursements to this customer. May have one or more Disbursements.
        /// </summary>
        public ICollection<DisbursementModel> Disbursements { get; set; }

        /// <summary>
        /// Get or set the current active Funder who funding Disbursements to this customer.
        /// </summary>
        public FunderModel DisbursementFunder { get; set; }

        /// <summary>
        /// Get or set the Customer Disbursement Funders to this customer. May have one or more.
        /// </summary>
        public ICollection<CustomerDisbursementFunderModel> CustomerDisbursementFunders { get; set; }
    }
}
