using System;
using System.Domain;
namespace Finance.Domain.Aggregates.FeeAgg
{
    public class FeeNote : Entity
    {
        /// <summary>
        /// Get or Set the Date to this Note for the Fee
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Get or Set the Note to this Note for the Fee
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Get or Set whether this note should shown on invoice
        /// </summary>
        public bool IsForInvoice { get; set; }
    }
}
