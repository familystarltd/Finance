using System;
using System.Collections.Generic;
using System.Domain;

namespace FinanceManagement.Domain.Aggregates.FeeAgg
{
    public interface IFunderRepository : IRepository<Funder>
    {
        IEnumerable<Funder> GetFunders(string searchText);
        IEnumerable<Funder> GetFunders(string searchText, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<Funder> GetFundersWithFees(DateTime FeeDate, int pageIndex, int pageSize, out int TotalRowCount);
        Funder GetFunder(Guid FunderId);
        void RemoveContact(CustomerAgg.Contact contact);

        
    }
}