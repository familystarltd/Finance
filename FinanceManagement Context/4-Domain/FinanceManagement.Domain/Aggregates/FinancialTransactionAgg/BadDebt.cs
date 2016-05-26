using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public class BadDebt : FinancialTransaction
    {
        public override FinancialTransactionType FinancialTransactionType
        {
            get { return FinancialTransactionAgg.FinancialTransactionType.BadDebt; }
            set { value = FinancialTransactionAgg.FinancialTransactionType.BadDebt; }
        }
        public Guid? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}