using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace FinanceManagement.Web.Model
{
    public class FunderModel
    {
        /// <summary>
        /// Unique Identifier
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Get or set the Given short name of this Funder
        /// </summary>
        [Required(ErrorMessage = "Please enter name of funder")]
        public string Name { get; set; }
        /// <summary>
        /// Get or set the contact of this Funder
        /// </summary>
        public ContactModel PersonalContact { get; set; }
        /// <summary>
        /// Get or set the Invoice ContactReference to this Funder (It may be empty if the ContactReference is refer to the invoice)
        /// </summary>
        public string InvoiceContactReference { get; set; }
        /// <summary>
        /// Get or Set the Invoice Contact for this Funder
        /// </summary>
        /// <summary>
        /// Get or Set the Fee Invoice Billing Contact for this Funder
        /// </summary>
        public virtual ContactModel FeeInvoiceBillingContact { get; set; }
        /// <summary>
        /// Get or Set the Fee Invoice Delivery Contact for this Funder
        /// </summary>
        public virtual ContactModel FeeInvoiceDeliveryContact { get; set; }
        /// <summary>
        /// Get or Set the Expenses Invoice Billing Contact for this Funder
        /// </summary>
        public virtual ContactModel DisbursementInvoiceBillingContact { get; set; }
        /// <summary>
        /// Get or Set the Expenses Invoice Delivery Contact for this Funder
        /// </summary>
        public virtual ContactModel DisbursementInvoiceDeliveryContact { get; set; }
        public virtual ICollection<FeeModel> Fees { get; set; }
        public IEnumerable<FunderModel> Funders { get; set; }
        
    }
}