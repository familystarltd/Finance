using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Finance.Web.Model
{
    public class PayerModel
    {
        /// <summary>
        /// Unique Identifier
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Get or set the Given short name of this Payer
        /// </summary>
        [Required(ErrorMessage = "Please enter name of payer")]
        public string Name { get; set; }
        /// <summary>
        /// Get or set the contact of this Payer
        /// </summary>
        public ContactModel PersonalContact { get; set; }
        /// <summary>
        /// Get or set the Invoice ContactReference to this Payer (It may be empty if the ContactReference is refer to the invoice)
        /// </summary>
        public string InvoiceContactReference { get; set; }
        /// <summary>
        /// Get or Set the Invoice Contact for this Payer
        /// </summary>
        /// <summary>
        /// Get or Set the Fee Invoice Billing Contact for this Payer
        /// </summary>
        public virtual ContactModel FeeInvoiceBillingContact { get; set; }
        /// <summary>
        /// Get or Set the Fee Invoice Delivery Contact for this Payer
        /// </summary>
        public virtual ContactModel FeeInvoiceDeliveryContact { get; set; }
        /// <summary>
        /// Get or Set the Expenses Invoice Billing Contact for this Payer
        /// </summary>
        public virtual ContactModel DisbursementInvoiceBillingContact { get; set; }
        /// <summary>
        /// Get or Set the Expenses Invoice Delivery Contact for this Payer
        /// </summary>
        public virtual ContactModel DisbursementInvoiceDeliveryContact { get; set; }
        //public virtual ICollection<FeeModel> Fees { get; set; }
        public IEnumerable<PayerModel> Payers { get; set; }
        
    }
}