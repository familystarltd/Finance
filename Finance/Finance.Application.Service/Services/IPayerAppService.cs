using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Service
{
    public interface IFunderAppService
    {
        IEnumerable<FunderModel> GetFunders(string searchText);
        IEnumerable<FunderModel> GetFunders(string searchText, int pageIndex, int pageSize, out int TotalRowCount);
        FunderModel GetFunder(Guid FunderId);
        FunderModel SaveFunder(FunderModel funderModel);
        void DeleteFunder(Guid Id);
    }
}
