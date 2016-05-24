using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinanceManagement.Web.Model
{
    public class FeeInvoiceDetailModel : InvoiceDetailModel
    {
        public bool IsApproved { get; set; }
        public Guid RateId { get; set; }
        public virtual RateModel Rate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int NoOfHours { get; set; }
        public int NoOfDays { get; set; }
    }
}
