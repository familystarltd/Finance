using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Infrastructure.CrossCutting.Framework.Extensions;
using Finance.Domain.Aggregates.DisbursementAgg;

namespace Finance.Application.Service
{
    public class CustomerAppService : ICustomerAppService
    {
        readonly ICustomerRepository _CustomerRepository;
        public CustomerAppService(ICustomerRepository customerRepository)
        {
            this._CustomerRepository = customerRepository;
        }
        public CustomerModel SaveCustomer(CustomerModel customerModel)
        {
            try
            {
                Customer customerOriginal = _CustomerRepository.GetCustomer(customerModel.Id);
                customerModel.Business = null;
                if (customerOriginal == null)
                {
                    if (customerModel.Id == Guid.Empty)
                        customerModel.Id = Guid.NewGuid();
                    Customer cust = DataProjections.ProjectedTo<Customer>(customerModel);
                    _CustomerRepository.Add(cust);
                    this._CustomerRepository.UnitOfWork.SaveChanges();
                    this._CustomerRepository.UnitOfWork.Commit();
                }
                else
                {
                    this._CustomerRepository.Merge(customerOriginal, DataProjections.ProjectedTo<Customer>(customerModel));
                    this._CustomerRepository.UnitOfWork.Commit();
                }
                return _CustomerRepository.GetCustomer(customerModel.Id).ProjectedTo<CustomerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public CustomerModel GetCustomer(Guid customerId)
        {
            try
            {
                CustomerModel customer = _CustomerRepository.GetCustomer(customerId).ProjectedTo<CustomerModel>();
                if (customer != null)
                    customer.MaxFeeSetupDate = _CustomerRepository.GetMaxFeeSetupDate(customer.Id);
                    return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CustomerModel> GetActiveCustomers(string searchText)
        {
            try
            {
                return _CustomerRepository.GetActiveCustomers(searchText).ProjectedToCollection<CustomerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CustomerModel> GetAllCustomers(string searchText)
        {
            try
            {
                return _CustomerRepository.GetAllCustomers(searchText).ProjectedToCollection<CustomerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CustomerModel> GetAllCustomers(string searchText, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                searchText = string.IsNullOrEmpty(searchText) ? string.Empty : searchText.Trim();
                IEnumerable<CustomerModel> customers = _CustomerRepository.GetAllCustomers(searchText, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<CustomerModel>();
                return customers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CustomerModel> GetCustomersByBusiness(string business)
        {
            try
            {
                return _CustomerRepository.GetCustomersByBusiness(business).ProjectedToCollection<CustomerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CustomerModel> GetCustomersWithFees(DateTime FeeDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                List<Customer> cus = _CustomerRepository.GetCustomersWithFees(FeeDate, pageIndex, pageSize, out TotalRowCount).ToList();
                IEnumerable<CustomerModel> customers = cus.ProjectedToCollection<CustomerModel>();
                return customers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CustomerModel> GetCustomersWithoutFees()
        {
            try
            {
                return _CustomerRepository.GetCustomersWithoutFees().ProjectedToCollection<CustomerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public DateTime? GetMaxFeeSetupDate(Guid CustomerId)
        {
            return _CustomerRepository.GetMaxFeeSetupDate(CustomerId);
        }

        #region EXPENSES
        public CustomerModel GetCustomerWithDisbursements(System.Guid CustomerId, DateTime FromDate,DateTime ToDate)
        {
            //recover existing Customer and map
            Customer Customer = _CustomerRepository.GetCustomer(CustomerId);
            if (Customer != null)
                Customer.Disbursements = _CustomerRepository.GetDisbursements(FromDate, ToDate).ToList();
            return Customer.ProjectedTo<CustomerModel>(); ;
        }
        public DisbursementViewModel AddNewDisbursement(DisbursementViewModel DisbursementViewModel)
        {
            try
            {
                DateTime DisbursementsDate = DisbursementViewModel.DisbursementModel.DisbursementDate.Date;
                DateTime FromDate = new DateTime(DisbursementsDate.Year, DisbursementsDate.Month, 1);
                DateTime ToDate = new DateTime(DisbursementsDate.Year, DisbursementsDate.Month, DateTime.DaysInMonth(DisbursementsDate.Year, DisbursementsDate.Month));
                Customer Customer = _CustomerRepository.GetCustomer(DisbursementViewModel.CustomerId);
                if (Customer != null)
                {
                    Disbursement disbursement = new Disbursement();
                    disbursement.Id = DisbursementViewModel.DisbursementModel.Id != Guid.Empty ? DisbursementViewModel.DisbursementModel.Id : Guid.NewGuid();
                    disbursement.DisbursementDate = DisbursementViewModel.DisbursementModel.DisbursementDate;
                    disbursement.Amount = DisbursementViewModel.DisbursementModel.Amount;
                    disbursement.Document = DisbursementViewModel.DisbursementModel.Document;
                    disbursement.Expense = this._CustomerRepository.GetExpenses().SingleOrDefault(e => e.Name == DisbursementViewModel.DisbursementModel.Expense.Name);

                    disbursement.CustomerId = Customer.Id;
                    disbursement.PayerId = Customer.DisbursementPayer != null ? (Guid?) Customer.DisbursementPayer.Id : null;
                    this._CustomerRepository.AddDisbursement(disbursement);
                    _CustomerRepository.UnitOfWork.SaveChanges();
                }
                DisbursementViewModel.DisbursementModel = new DisbursementModel();
                DisbursementViewModel.Customer = this.GetCustomer(Customer.Id);
                return DisbursementViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public CustomerModel GetCustomerForExpensesInvoice(System.Guid customerId)
        {
            //recover existing customer and map
            var customer = new Customer();//_CustomerRepository.GetCustomerWithExpenses(customerId, DateTime.Now);
            CustomerModel Customer = null;
            if (customer != null)
                Customer = customer.ProjectedTo<CustomerModel>();//Adapting POCO to DTO
            return Customer;
        }
        public IEnumerable<CustomerModel> GetCustomersForExpensesInvoice(int pageIndex, int pageCount)
        {
            var customer = new List<Customer>(); //_CustomerRepository.GetCustomersForExpensesInvoice(pageIndex, pageCount);
            return customer.ProjectedToCollection<CustomerModel>();//Adapting POCO to DTO

        }
        public IEnumerable<ExpenseModel> GetExpenses(string search)
        {
            search = string.IsNullOrEmpty(search) ? string.Empty : search;
            IEnumerable<Expense> expenses = this._CustomerRepository.GetExpenses(search);
            if (expenses == null)
                expenses = new List<Expense>();
            return expenses.ProjectedToCollection<ExpenseModel>();

        }
        public IEnumerable<Expense> GetExpenses()
        {
            return this._CustomerRepository.GetExpenses();
        }
        public bool DeleteDisbursement(Guid id)
        {
            bool result = false;
            try
            {
                _CustomerRepository.DeleteDisbursement(_CustomerRepository.GetDisbursement(id));
                _CustomerRepository.UnitOfWork.Commit();
                result = true;
            }
            catch {
                result = false;
            }
            return result;
        }
        #endregion
    }
}