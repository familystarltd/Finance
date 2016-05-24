using System;
using System.Collections.Generic;
using System.Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public class InvoiceArticleTemplate : Entity
    {
        public string Name { get; set; }
        public string ArticleTemplate { get; set; }
    }
}