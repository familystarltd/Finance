using Finance.Domain.Aggregates.DisbursementAgg;
using System;
using System.Collections.Generic;
using System.Domain;

namespace Finance.Domain.Aggregates.CustomerAgg
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer GetCustomer(Guid customerId);
        IEnumerable<Customer> GetAllCustomers(string searchText);
        IEnumerable<Customer> GetActiveCustomers(string searchText);
        IEnumerable<Customer> GetAllCustomers(string searchText, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<Customer> GetCustomersByBusiness(string business);
        IEnumerable<Customer> GetCustomersWithFees(DateTime FeeDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<Customer> GetCustomersWithoutFees();
        DateTime? GetMaxFeeSetupDate(Guid CustomerId);

        #region DisbursementS
        IEnumerable<Expense> GetExpenses();
        IEnumerable<Expense> GetExpenses(string search);
        IEnumerable<Disbursement> GetDisbursements(DateTime FromDate, DateTime ToDate);
        IEnumerable<Disbursement> GetDisbursements(Guid CustomerID, DateTime FromDate, DateTime ToDate);
        Disbursement GetDisbursement(Guid Id);
        void AddDisbursement(Disbursement residentDisbursement);
        void DeleteDisbursement(Disbursement Disbursement);
        #endregion

        #region BUSINESS

        #endregion
    }
}