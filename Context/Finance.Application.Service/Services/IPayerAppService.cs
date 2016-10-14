using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Service
{
    public interface IPayerAppService
    {
        IEnumerable<PayerModel> GetPayers(string searchText);
        IEnumerable<PayerModel> GetPayers(string searchText, int pageIndex, int pageSize, out int TotalRowCount);
        PayerModel GetPayer(Guid PayerId);
        PayerModel SavePayer(PayerModel payerModel);
        void DeletePayer(Guid Id);
    }
}
