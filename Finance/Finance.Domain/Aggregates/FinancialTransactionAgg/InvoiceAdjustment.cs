using System;
using System.Collections.Generic;
using System.Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Domain.Aggregates.FinancialTransactionAgg
{
    public class InvoiceAdjustment : Entity
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}