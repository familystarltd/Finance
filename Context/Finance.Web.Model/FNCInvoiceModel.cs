using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Web.Model
{
    public class FNCInvoiceModel : InvoiceModel
    {
        /// <summary>
        /// Get or Set the associated FNC for this Invoice
        /// </summary>
        public Guid? FNCId { get; set; }
        public virtual FNCModel FNC { get; set; }
        new public virtual ICollection<FNCInvoiceDetailModel> InvoiceDetails { get; set; }
    }
    public class FNCInvoiceDetailModel : InvoiceDetailModel
    {
        public Guid? FNCRateId { get; set; }
        public virtual FNCRateModel FNCRate { get; set; }
        public decimal? FNCRateOverride { get; set; }
        public int NoOfDays { get; set; }
    }
}
