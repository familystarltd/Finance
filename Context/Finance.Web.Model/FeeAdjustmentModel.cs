using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finance.Web.Model
{
    public class InvoiceAdjustmentModel
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
