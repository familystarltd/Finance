using System;
using System.Collections.Generic;
using System.Domain;

namespace Finance.Domain.Aggregates.FeeAgg
{
    public interface IPayerRepository : IRepository<Payer>
    {
        IEnumerable<Payer> GetPayers(string searchText);
        IEnumerable<Payer> GetPayers(string searchText, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<Payer> GetPayersWithFees(DateTime FeeDate, int pageIndex, int pageSize, out int TotalRowCount);
        Payer GetPayer(Guid PayerId);
        void RemoveContact(CustomerAgg.Contact contact);

        
    }
}