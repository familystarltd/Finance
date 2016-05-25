using System;
using System.Linq;
using System.Collections.Generic;
using System.Infrastructure.Data;
using FinanceManagement.Domain.Aggregates.CustomerAgg;
using FinanceManagement.Infrastructure.Data.UnitOfWork;
using FinanceManagement.Domain.Aggregates.FeeAgg;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using FinanceManagement.Domain.Aggregates.DisbursementAgg;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Customer Repository implementation.
    /// <see cref="FinanceManagement.Domain.ICustomerRepository"/>
    /// </summary>
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public CustomerRepository(FinanceManagementDbContext unitOfWork) : base(unitOfWork) { }
        public Customer GetCustomer(Guid customerId)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementDbContext;
                Customer customer = uow.Customers
                    .Include(c => c.PersonalInfo)
                    .Include(c => c.Company)
                    .Include(c => c.Fees).ThenInclude(f => f.Funder).ThenInclude(f=>f.PersonalContact)
                    .Include(c => c.Fees).ThenInclude(f => f.Funder).ThenInclude(f => f.FeeInvoiceBillingContact)
                    .Include(c => c.Fees).ThenInclude(f => f.Funder).ThenInclude(f => f.FeeInvoiceDeliveryContact)
                    .Where(c => c.Id == customerId).SingleOrDefault();
                if (customer != null)
                {
                    CustomerDisbursementFunder customerDisbursementFunder = uow.CustomerDisbursementFunders.Include(c=>c.Funder).Include(c=>c.Disbursements).Where(c=>c.CustomerId == customer.Id && !c.DeactiveDate.HasValue).OrderByDescending(c=>c.ActiveDate).FirstOrDefault();
                    customer.DisbursementFunder = customerDisbursementFunder != null ? customerDisbursementFunder.Funder : null;
                }
                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public override void Merge(Customer persisted, Customer current)
        {
            var currentUOW = this.UnitOfWork as FinanceManagementDbContext;
            
            if (persisted == null || current == null)
                return;
            currentUOW.ApplyCurrentValues(persisted, current);
            if (persisted.PersonalInfo != null && current.PersonalInfo != null)
                currentUOW.ApplyCurrentValues(persisted.PersonalInfo, current.PersonalInfo);
        }
        public IEnumerable<Customer> GetAllCustomers(string searchText)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementDbContext;
                return uow.Customers.Distinct()
                    .Include(c => c.PersonalInfo)
                    .Include(c => c.Company)
                    .Where(c => c.PersonalInfo.FirstName.Contains(searchText) || c.PersonalInfo.LastName.Contains(searchText) || string.IsNullOrEmpty(searchText));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Customer> GetAllCustomers(string searchText, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementDbContext;
                TotalRowCount = uow.Customers.Distinct()
                    .Include(c => c.PersonalInfo)
                    .Include(c => c.Company)
                    .Count(c => c.PersonalInfo.FirstName.Contains(searchText) || c.PersonalInfo.LastName.Contains(searchText) || string.IsNullOrEmpty(searchText));

                return uow.Customers.Distinct()
                    .Include(c => c.PersonalInfo)
                    .Include(c => c.Company)
                    .Where(c => c.PersonalInfo.FirstName.Contains(searchText) || c.PersonalInfo.LastName.Contains(searchText) || string.IsNullOrEmpty(searchText))
                    .OrderBy(c=>c.Ref)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Customer> GetActiveCustomers(string searchText)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementDbContext;
                return uow.Customers
                    .Include(c => c.PersonalInfo)
                    .Include(c => c.Company)
                    .Where(c => !c.Deactive && (c.PersonalInfo.FirstName.Contains(searchText) || c.PersonalInfo.LastName.Contains(searchText) || string.IsNullOrEmpty(searchText)));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Customer> GetCustomersByCompany(string company)
        {
            try
            {
                // Get last 12 months invoices only
                DateTime invoiceFromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-11).Date;
                DateTime invoiceToDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
                DateTime feeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).Date.AddDays(-1);
                var uow = this.UnitOfWork as FinanceManagementDbContext;
                IEnumerable<Customer> customers = uow.Customers
                    .Include(c => c.PersonalInfo)
                    .Include(c => c.Company)
                    .Where(c => c.Company.Name.Equals(company, StringComparison.OrdinalIgnoreCase) && !c.Deactive).ToList()
                    .Select(c => new Customer
                    {
                        Id = c.Id,
                        ActiveDate = c.ActiveDate,
                        Company = c.Company,
                        PersonalInfo = c.PersonalInfo,
                        Debits = c.Debits,
                        Credits = c.Credits,
                        Fees = 
                        uow.Fees
                        .Include(cf => cf.Funder.PersonalContact).Include(cf => cf.Funder.FeeInvoiceBillingContact).Include(cf => cf.Funder.FeeInvoiceDeliveryContact).Include(cf=>cf.Funder.DisbursementInvoiceBillingContact).Include(cf => cf.Funder.DisbursementInvoiceDeliveryContact)
                        .Where(f => (!f.ClosingDate.HasValue || (f.ClosingDate.HasValue && DbFunctions.TruncateTime(f.ClosingDate) >= feeDate.Date)) && f.CustomerId == c.Id).ToList()
                        .Select(fee=>new Fee
                        {
                                Id = fee.Id,
                                Name = fee.Name,
                                Description = fee.Description,
                                Funder = fee.Funder,
                                FunderId = fee.FunderId,
                                Customer = fee.Customer,
                                CustomerId = fee.CustomerId,
                                ClosingDate=fee.ClosingDate,
                                EffectiveDate = fee.EffectiveDate,
                                PaymentTerm=fee.PaymentTerm,
                                PayContactReference=fee.PayContactReference,
                                LogDateTime = fee.LogDateTime,
                                LogUser = fee.LogUser,
                                Notes = fee.Notes,
                                FeeRates = uow.FeeRates.Where(fr => fr.FeeId == fee.Id).ToList().Select(
                                                                fr => new FeeRate
                                                                {
                                                                    Id = fr.Id,
                                                                    Fee = fee,
                                                                    FeeId = fee.Id,
                                                                    RateDescription = fr.RateDescription,
                                                                    RateMethod = fr.RateMethod,
                                                                    Rates = uow.Rates.Where(r => r.FeeRateId == fr.Id && !r.ClosingDate.HasValue)
                                                                        .OrderBy(r => (r is DailyRate) ? (r as DailyRate).DayPremium : ((r is HourlyRate) ? (r as HourlyRate).DayPremium : 0))
                                                                        .ThenBy((r => (r is HourlyRate) ? (r as HourlyRate).TimePremium : 0)).ToList()
                                                                }).ToList()
                        }).ToList()
                    });
                return customers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Customer> GetCustomersWithFees(DateTime FeeDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            var uow = this.UnitOfWork as FinanceManagementDbContext;
            //DateTime FeeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).Date.AddDays(-1);
            TotalRowCount = uow.Customers.Count(c => !c.Deactive || (c.Deactive && DbFunctions.TruncateTime(c.DeactiveDate) >= FeeDate.Date)); 
                  
            IEnumerable<Customer> customers = uow.Customers
                .Include(c => c.PersonalInfo)
                .Include(c => c.Company)
                .Where(c => !c.Deactive || (c.Deactive && DbFunctions.TruncateTime(c.DeactiveDate) >= FeeDate.Date))
                .OrderBy(c => c.Ref).Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(c => new Customer
                {
                    Id = c.Id,
                    ActiveDate = c.ActiveDate,
                    Company = c.Company,
                    PersonalInfo = c.PersonalInfo,
                    Debits = c.Debits,
                    Credits = c.Credits,
                    Fees =
                        uow.Fees
                        .Include(cf => cf.Funder.PersonalContact).Include(cf => cf.Funder.FeeInvoiceBillingContact).Include(cf => cf.Funder.FeeInvoiceDeliveryContact).Include(cf => cf.Funder.DisbursementInvoiceBillingContact).Include(cf => cf.Funder.DisbursementInvoiceDeliveryContact)
                        .Where(f => (!f.ClosingDate.HasValue || (f.ClosingDate.HasValue && DbFunctions.TruncateTime(f.ClosingDate) >= FeeDate.Date)) && f.CustomerId == c.Id).ToList()
                        .Select(fee => new Fee
                        {
                            Id = fee.Id,
                            Name = fee.Name,
                            Description = fee.Description,
                            Funder = fee.Funder,
                            FunderId = fee.FunderId,
                            Customer = fee.Customer,
                            CustomerId = fee.CustomerId,
                            ClosingDate = fee.ClosingDate,
                            EffectiveDate = fee.EffectiveDate,
                            PaymentTerm = fee.PaymentTerm,
                            PayContactReference = fee.PayContactReference,
                            LogDateTime = fee.LogDateTime,
                            LogUser = fee.LogUser,
                            Notes = fee.Notes,
                            FeeRates = uow.FeeRates.Where(fr => fr.FeeId == fee.Id).ToList().Select(
                                                                fr => new FeeRate
                                                                {
                                                                    Id = fr.Id,
                                                                    Fee = fee,
                                                                    FeeId = fee.Id,
                                                                    RateDescription = fr.RateDescription,
                                                                    RateMethod = fr.RateMethod,
                                                                    Rates = uow.Rates.Where(r => r.FeeRateId == fr.Id && !r.ClosingDate.HasValue)
                                                                        .OrderBy(r => (r is DailyRate) ? (r as DailyRate).DayPremium : ((r is HourlyRate) ? (r as HourlyRate).DayPremium : 0))
                                                                        .ThenBy((r => (r is HourlyRate) ? (r as HourlyRate).TimePremium : 0)).ToList()
                                                                }).ToList()
                        }).ToList()
                });
            return customers;
        }
        public IEnumerable<Customer> GetCustomersWithoutFees()
        {
            try
            {
                DateTime feeDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month+1, 1).Date.AddDays(-1);
                var uow = this.UnitOfWork as FinanceManagementDbContext;
                return uow.Customers
                    .Include(c => c.PersonalInfo)
                    .Include(c => c.Company)
                    .Include(c => c.Fees)
                    .Include(c => c.CustomerDisbursementFunders)
                    .Where(c => (!c.Fees.Any(f => f.CustomerId == c.Id))
                        || (c.Fees.Any(f => f.CustomerId == c.Id && f.ClosingDate.HasValue && DbFunctions.TruncateTime(f.ClosingDate) < feeDate.Date)
                        || (c.CustomerDisbursementFunders.Any(cdf => cdf.CustomerId == cdf.Id && (cdf.ActiveDate >= feeDate.Date || cdf.DeactiveDate.HasValue && DbFunctions.TruncateTime(cdf.DeactiveDate) < feeDate.Date)))
                        )
                        && !c.Deactive).ToList().Select(c =>
                        new Customer
                        {
                            Id = c.Id,
                            PersonalInfo = c.PersonalInfo,
                            Fees = c.Fees,
                            Company = c.Company,
                            ActiveDate = c.ActiveDate,
                            Ref = c.Ref,
                            DisbursementFunder = uow.CustomerDisbursementFunders.Include(cdf => cdf.Funder).Include(cdf => cdf.Disbursements).Where(cdf => cdf.CustomerId == c.Id && !cdf.DeactiveDate.HasValue).OrderByDescending(cdf => cdf.ActiveDate).FirstOrDefault() != null ?
                            uow.CustomerDisbursementFunders.Include(cdf => cdf.Funder).Include(cdf => cdf.Disbursements).Where(cdf => cdf.CustomerId == c.Id && !cdf.DeactiveDate.HasValue).OrderByDescending(cdf => cdf.ActiveDate).FirstOrDefault().Funder : null

                        }
                    );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        #region DISBURSEMENTS
        public IEnumerable<Expense> GetExpenses()
        {
            var uow = this.UnitOfWork as FinanceManagementDbContext;
            return uow.Expenses;
        }
        public IEnumerable<Expense> GetExpenses(string search)
        {
            var UOW = this.UnitOfWork as FinanceManagementDbContext;
            return UOW.Expenses.Where(e => e.Name.ToLower().Contains(search.ToLower()) || string.IsNullOrEmpty(search.Trim()));
        }
        public IEnumerable<Disbursement> GetDisbursements(DateTime FromDate, DateTime ToDate)
        {
            var currentUOW = this.UnitOfWork as FinanceManagementDbContext;            
            return currentUOW.Disbursements.Include(e => e.Expense).Include(e => e.Funder).Where(c => c.DisbursementDate >= FromDate && c.DisbursementDate <= ToDate);
        }
        public IEnumerable<Disbursement> GetDisbursements(Guid CustomerID, DateTime FromDate, DateTime ToDate)
        {
            var currentUOW = this.UnitOfWork as FinanceManagementDbContext;            
            //DateTime dtFromExp = new DateTime(DisbursementsDate.Year, DisbursementsDate.Month, 1);
            //DateTime dtToExp = new DateTime(DisbursementsDate.Year, DisbursementsDate.Month, DateTime.DaysInMonth(DisbursementsDate.Year, DisbursementsDate.Month));
            return currentUOW.Disbursements
                             .Where(c => c.CustomerId == CustomerID && c.DisbursementDate >= FromDate && c.DisbursementDate <= ToDate)
                             .Include(e => e.Expense);
        }
        public Disbursement GetDisbursement(Guid Id)
        {
            var currentUOW = this.UnitOfWork as FinanceManagementDbContext;
            
            return currentUOW.Disbursements
                             .Where(c => c.Id == Id)
                             .Include(e => e.Expense).SingleOrDefault();
        }
        public void AddDisbursement(Disbursement Disbursement)
        {
            var currentUOW = this.UnitOfWork as FinanceManagementDbContext;
            currentUOW.Disbursements.Add(Disbursement);
        }
        public void DeleteDisbursement(Disbursement Disbursement)
        {
            var uow = this.UnitOfWork as FinanceManagementDbContext;
            uow.Disbursements.Remove(Disbursement);
        }
        public DateTime? GetMaxFeeSetupDate(Guid CustomerId)
        {
            var uow = this.UnitOfWork as FinanceManagementDbContext;
            List<DateTime?> MaxFeeSetupDates = new List<DateTime?>();
            MaxFeeSetupDates.Add(uow.FinancialTransactions.OfType<FeeInvoice>().Where(inv => inv.CustomerId == CustomerId && inv.InvoiceStatus != InvoiceStatus.Cancel && inv.InvoiceStatus != InvoiceStatus.Void).Max(inv => (DateTime?)inv.ProcessedDate));
            //MaxFeeSetupDates.Add(uow.CustomerPays.Where(c=>c.CustomerId == CustomerId).Max(cf => (DateTime?)cf.ActiveDate));
            MaxFeeSetupDates.Add(uow.Customers.Where(c=>c.Id == CustomerId).Max(c => (DateTime?)c.ActiveDate));
            return MaxFeeSetupDates.Max();
        }
        #endregion
    }
}