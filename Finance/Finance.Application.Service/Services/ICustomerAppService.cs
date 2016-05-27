using Finance.Domain.Aggregates.DisbursementAgg;
using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Application.Service
{
    public interface ICustomerAppService
    {
        CustomerModel GetCustomer(Guid customerId);
        IEnumerable<CustomerModel> GetActiveCustomers(string searchText);
        IEnumerable<CustomerModel> GetAllCustomers(string searchText);
        IEnumerable<CustomerModel> GetAllCustomers(string searchText, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<CustomerModel> GetCustomersByCompany(string company);
        IEnumerable<CustomerModel> GetCustomersWithFees(DateTime FeeDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<CustomerModel> GetCustomersWithoutFees();
        CustomerModel SaveCustomer(CustomerModel customer);
        DateTime? GetMaxFeeSetupDate(Guid CustomerId);
        #region EXPENSES
        CustomerModel GetCustomerWithDisbursements(System.Guid CustomerId, DateTime FromDate, DateTime ToDate);
        DisbursementViewModel AddNewDisbursement(DisbursementViewModel DisbursementViewModel);
        CustomerModel GetCustomerForExpensesInvoice(System.Guid customerId);
        IEnumerable<CustomerModel> GetCustomersForExpensesInvoice(int pageIndex, int pageCount);
        IEnumerable<ExpenseModel> GetExpenses(string search);
        IEnumerable<Expense> GetExpenses();
        bool DeleteDisbursement(Guid id);
        #endregion
    }
}
