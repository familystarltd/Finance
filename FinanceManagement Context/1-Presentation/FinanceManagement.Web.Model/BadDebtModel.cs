using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Web.Model
{
    public class BadDebtModel : FinancialTransactionModel
    {
        //public override FinancialTransactionTypeModel FinancialTransactionType
        //{
        //    get { return FinancialTransactionTypeModel.BadDebt; }
        //    set { value = FinancialTransactionTypeModel.BadDebt; }
        //}
        public virtual InvoiceModel Invoice { get; set; }
    }
}
