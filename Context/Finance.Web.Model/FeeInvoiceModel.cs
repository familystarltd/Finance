using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Web.Model
{
    public class FeeInvoiceModel : InvoiceModel
    {
        /// <summary>
        /// Get or Set the associated Fee for this Fee Invoice
        /// </summary>
        public Guid FeeId { get; set; }
        public FeeModel Fee { get; set; }
        new public virtual ICollection<FeeInvoiceDetailModel> InvoiceDetails { get; set; }
    }
}
