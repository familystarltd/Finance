using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Web.Model
{
    public class InvoiceDetailModel
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public InvoiceModel Invoice { get; set; }
        public string Article { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }

        #region CALCULATED FIELDS
        public decimal VATAmount
        {
            get
            {
                return TotalPlusVAT - Total;
            }
        }

        public decimal TotalPlusVAT
        {
            get
            {
                return Total * (1 + VAT / 100);
            }
        }
        #endregion
    }
}
