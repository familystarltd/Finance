using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Infrastructure.CrossCutting.Framework;
using Finance.Domain.Aggregates.FeeAgg;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using Finance.Web.Model;
using Finance.Domain.Aggregates.CustomerAgg;
using System.Infrastructure.CrossCutting.Framework.Extensions;

namespace Finance.Application.Service
{
    public class InvoiceAppService : IInvoiceAppService
    {
        readonly IFeeRepository _FeeRepository;
        readonly IInvoiceRepository _InvoiceRepository;
        readonly IFinanceTransactionRepository _FinanceTransactionRepository;
        readonly ICustomerRepository _CustomerRepository;
        public InvoiceAppService(IFeeRepository FeeRepository, IFinanceTransactionRepository FinanceTransactionRepository, IInvoiceRepository InvoiceRepository, ICustomerRepository CustomerRepository)
        {
            this._FeeRepository = FeeRepository;
            this._InvoiceRepository = InvoiceRepository;
            this._FinanceTransactionRepository = FinanceTransactionRepository;
            this._CustomerRepository = CustomerRepository;
        }
        #region FEE INVOICE - GENERATE
        /***********************Things To Do***********************
         * Need to do Acceptance Testing 
         * ******************************************************** */
        #region PRIVATE METHODS
        private void AddHourlyRateToInvoice(FeeInvoice invoice, FeeInvoiceDetail invoiceDetail, HourlyRate hourlyRate)
        {
            Calendar calendar = new Calendar(Finance.Application.Service.Settings.AppSetting.BankHolidayOfflineSourceFile);
            int weekdays = 0;
            int saturdays = 0;
            int sundays = 0;
            int bankHolidays = 0;
            calendar.GetDays(invoiceDetail.FromDate.Date, invoiceDetail.ToDate.Date, out weekdays, out saturdays, out sundays, out bankHolidays);
            int weekends = saturdays + sundays;

            switch (hourlyRate.DayPremium)
            {
                case Finance.Domain.Aggregates.FeeAgg.DayPremium.Weekday:
                    invoiceDetail.NoOfDays = weekdays;
                    if (invoiceDetail.NoOfDays > 0 && hourlyRate.NoOfHours > 0 && hourlyRate.RateAmount > 0)
                    {
                        switch (hourlyRate.TimePremium)
                        {
                            case Finance.Domain.Aggregates.FeeAgg.TimePremium.Day:
                                invoiceDetail.Article = string.Format("{0} Weekdays at the Day rate of {1} for {2} hours from {3} to {4} ", invoiceDetail.NoOfDays, hourlyRate.RateAmount, hourlyRate.NoOfHours, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                                break;
                            case Finance.Domain.Aggregates.FeeAgg.TimePremium.Night:
                                invoiceDetail.Article = string.Format("{0} Weekdays at the Night rate of {1} for {2} hours from {3} to {4} ", invoiceDetail.NoOfDays, hourlyRate.RateAmount, hourlyRate.NoOfHours, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                                break;
                        }
                        invoiceDetail.Total = invoiceDetail.NoOfDays * hourlyRate.NoOfHours * hourlyRate.RateAmount;
                        invoice.InvoiceDetails.Add(invoiceDetail);
                    }
                    break;
                case Finance.Domain.Aggregates.FeeAgg.DayPremium.Weekend:
                    invoiceDetail.NoOfDays = weekends;
                    if (invoiceDetail.NoOfDays > 0 && hourlyRate.NoOfHours > 0 && hourlyRate.RateAmount > 0)
                    {
                        switch (hourlyRate.TimePremium)
                        {
                            case Finance.Domain.Aggregates.FeeAgg.TimePremium.Day:
                                invoiceDetail.Article = string.Format("{0} Weekend at the Day rate of {1} for {2} hours from {3} to {4} ", invoiceDetail.NoOfDays, hourlyRate.RateAmount, hourlyRate.NoOfHours, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                                break;
                            case Finance.Domain.Aggregates.FeeAgg.TimePremium.Night:
                                invoiceDetail.Article = string.Format("{0} Weekend at the Night rate of {1} for {2} hours from {3} to {4} ", invoiceDetail.NoOfDays, hourlyRate.RateAmount, hourlyRate.NoOfHours, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                                break;
                        }
                        invoiceDetail.Total = invoiceDetail.NoOfDays * hourlyRate.NoOfHours * hourlyRate.RateAmount;
                        invoice.InvoiceDetails.Add(invoiceDetail);
                    }
                    break;
                case Finance.Domain.Aggregates.FeeAgg.DayPremium.Bankholiday:
                    invoiceDetail.NoOfDays = bankHolidays;
                    if (invoiceDetail.NoOfDays > 0 && hourlyRate.NoOfHours > 0 && hourlyRate.RateAmount > 0)
                    {
                        switch (hourlyRate.TimePremium)
                        {
                            case Finance.Domain.Aggregates.FeeAgg.TimePremium.Day:
                                invoiceDetail.Article = string.Format("{0} Bank Holidays at the Day rate of {1} for {2} hours from {3} to {4} ", invoiceDetail.NoOfDays, hourlyRate.RateAmount, hourlyRate.NoOfHours, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                                break;
                            case Finance.Domain.Aggregates.FeeAgg.TimePremium.Night:
                                invoiceDetail.Article = string.Format("{0} Bank Holidays at the Night rate of {1} for {2} hours from {3} to {4} ", invoiceDetail.NoOfDays, hourlyRate.RateAmount, hourlyRate.NoOfHours, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                                break;
                        }
                        invoiceDetail.Total = invoiceDetail.NoOfDays * hourlyRate.NoOfHours * hourlyRate.RateAmount;
                        invoice.InvoiceDetails.Add(invoiceDetail);
                    }
                    break;
            }
            // GENERATE INVOICE DETAIL
        }
        private void AddDailyRateToInvoice(FeeInvoice invoice, FeeInvoiceDetail invoiceDetail, DailyRate dailyRate)
        {
            Calendar calendar = new Calendar(Finance.Application.Service.Settings.AppSetting.BankHolidayOfflineSourceFile);
            int weekdays = 0;
            int saturdays = 0;
            int sundays = 0;
            int bankHolidays = 0;
            calendar.GetDays(invoiceDetail.FromDate.Date, invoiceDetail.ToDate.Date, out weekdays, out saturdays, out sundays, out bankHolidays);
            int weekends = saturdays + sundays;
            switch (dailyRate.DayPremium)
            {
                case Finance.Domain.Aggregates.FeeAgg.DayPremium.Weekday:
                    invoiceDetail.NoOfDays = weekdays;
                    if (invoiceDetail.NoOfDays > 0)
                    {
                        invoiceDetail.Article = string.Format("{0} Weekdays at the rate of {1} from {2} to {3} ", invoiceDetail.NoOfDays, dailyRate.RateAmount, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                        invoiceDetail.Total = invoiceDetail.NoOfDays * dailyRate.RateAmount;
                        invoice.InvoiceDetails.Add(invoiceDetail);
                    }
                    break;
                case Finance.Domain.Aggregates.FeeAgg.DayPremium.Weekend:
                    invoiceDetail.NoOfDays = weekends;
                    if (invoiceDetail.NoOfDays > 0)
                    {
                        invoiceDetail.Article = string.Format("{0} Weekend at the rate of {1} from {2} to {3} ", invoiceDetail.NoOfDays, dailyRate.RateAmount, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                        invoiceDetail.Total = invoiceDetail.NoOfDays * dailyRate.RateAmount;
                        invoice.InvoiceDetails.Add(invoiceDetail);
                    }
                    break;
                case Finance.Domain.Aggregates.FeeAgg.DayPremium.Bankholiday:
                    invoiceDetail.NoOfDays = bankHolidays;
                    if (invoiceDetail.NoOfDays > 0)
                    {
                        invoiceDetail.Article = string.Format("{0} Bank holidays at the rate of {1} from {2} to {3} ", invoiceDetail.NoOfDays, dailyRate.RateAmount, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
                        invoiceDetail.Total = invoiceDetail.NoOfDays * dailyRate.RateAmount;
                        invoice.InvoiceDetails.Add(invoiceDetail);
                    }
                    break;
            }

        }
        private void AddWeeklyRateToInvoice(FeeInvoice invoice, FeeInvoiceDetail invoiceDetail, WeeklyRate weeklyRate)
        {
            decimal dailyRateAmt = Math.Round(weeklyRate.RateAmount / 7, 2);
            invoiceDetail.NoOfDays = (int)(invoiceDetail.ToDate - invoiceDetail.FromDate).TotalDays + 1;
            invoiceDetail.Article = string.Format("{0} days at the daily rate of {1} from {2} to {3}", invoiceDetail.NoOfDays, dailyRateAmt, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
            invoiceDetail.Total = dailyRateAmt * invoiceDetail.NoOfDays;
            invoice.InvoiceDetails.Add(invoiceDetail);
        }
        private void AddMonthlyRateToInvoice(FeeInvoice invoice, FeeInvoiceDetail invoiceDetail, MonthlyRate rate)
        {
            decimal monthDailyRate = Math.Round(rate.RateAmount / invoiceDetail.NoOfDays, 2);
            invoiceDetail.NoOfDays = (int)(invoiceDetail.ToDate - invoiceDetail.FromDate).TotalDays + 1;
            invoiceDetail.Article = string.Format("{0} days at the daily rate of {1} from {2} to {3}", invoiceDetail.NoOfDays, monthDailyRate, invoiceDetail.FromDate.ToString("dd/MM/yyyy"), invoiceDetail.ToDate.ToString("dd/MM/yyyy"));
            invoiceDetail.Total = Math.Round(monthDailyRate * invoiceDetail.NoOfDays, 2);
            invoice.InvoiceDetails.Add(invoiceDetail);
        }
        #endregion
        public bool ProcessInvoice(DateTime invoiceProcessedDate)
        {
            try
            {
                DateTime FromDate = new DateTime(invoiceProcessedDate.Year, invoiceProcessedDate.Month, 1);
                DateTime ToDate = new DateTime(invoiceProcessedDate.Year, invoiceProcessedDate.Month, DateTime.DaysInMonth(invoiceProcessedDate.Year, invoiceProcessedDate.Month));
                int invoiceNo = _FinanceTransactionRepository.GetNewTransactionRefNo<Invoice>(invoiceProcessedDate.Date);
                if (invoiceNo < 0)
                    return false;
                DateTime invoiceDate = ToDate.Date;
                // Have to check eligible fees -- 14/04/2015
                // Get Fees that fall between FromDate and ToDate
                IEnumerable<Fee> fees = _FeeRepository.GetFeesForInvoiceProcess(FromDate, ToDate);
                DateTime feeFromDate = new DateTime();
                DateTime feeToDate = new DateTime();
                bool IsValidFeeTerm = false;
                try
                {
                    this._InvoiceRepository.UnitOfWork.BeginTransaction();
                    #region GENERATING INVOICES BASED ON FEES
                    // Each fee will have Invoice Fee -> Invoice
                    foreach (Fee fee in fees)
                    {
                        switch (fee.PaymentTerm)
                        {
                            case Finance.Domain.Aggregates.FeeAgg.PaymentTerm.Monthly:
                                feeFromDate = FromDate;
                                feeToDate = ToDate;
                                IsValidFeeTerm = true;
                                break;
                            case Finance.Domain.Aggregates.FeeAgg.PaymentTerm.FourWeely:
                                double totalDays = (ToDate.Date - fee.EffectiveDate.Date).TotalDays + 1;
                                if (totalDays > 0)
                                {
                                    int modDays = (int)totalDays % (7 * 4);
                                    //feeToDate = new DateTime(ToDate.Year, ToDate.Month, daysInMonth - modDays);
                                    feeToDate = ToDate.AddDays(-modDays);
                                    feeFromDate = feeToDate.AddDays(-(7 * 4) + 1);
                                    IsValidFeeTerm = true;
                                }
                                else
                                {
                                    IsValidFeeTerm = false;
                                }
                                break;
                        }
                        if (!IsValidFeeTerm)
                            break;
                        if (feeFromDate.Date <= fee.EffectiveDate.Date && feeToDate.Date >= fee.EffectiveDate.Date)
                            feeFromDate = fee.EffectiveDate.Date;
                        if (fee.ClosingDate.HasValue && feeFromDate.Date <= fee.ClosingDate.Value.Date && feeToDate.Date >= fee.ClosingDate.Value.Date)
                            feeToDate = fee.ClosingDate.Value.Date;
                        FeeInvoice invoice = new FeeInvoice();
                        invoice.InvoiceDetails = new List<InvoiceDetail>();
                        invoice.Id = Guid.NewGuid();
                        invoice.InvoiceNo = invoiceNo;
                        invoiceNo = invoiceNo + 1;
                        invoice.Discriminator = InvoiceType.Fee.ToString();
                        invoice.TransactionStatus = TransactionStatus.New;
                        //invoice.Customer = fee.Customer;
                        //invoice.Funder = fee.Funder;
                        invoice.CustomerId = fee.CustomerId;
                        invoice.FunderId = fee.FunderId;
                        //invoice.Fee = fee;
                        invoice.FeeId = fee.Id;
                        invoice.InvoiceDate = invoiceDate.Date;
                        invoice.DueDate = invoiceDate.Date.AddMonths(1).Date;
                        invoice.InvoiceMonth = new DateTime(invoiceProcessedDate.Year, invoiceProcessedDate.Month, 1);
                        invoice.ProcessedDate = invoiceProcessedDate.Date;
                        // The ContactReference to be used for checking bank payment 
                        invoice.PayContactReference = string.IsNullOrEmpty(fee.PayContactReference) ? invoice.InvoiceNo.ToString() : fee.PayContactReference;
                        invoice.InvoiceStatus = Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceStatus.Draft;
                        invoice.LogUser = Settings.AppSetting.UserName;
                        invoice.LogDate = DateTime.Now;                        
                        _InvoiceRepository.Add(invoice);
                        foreach (FeeRate feeRate in fee.FeeRates)
                        {
                            if (feeRate.Rates.Count > 0)
                            {
                                #region Rate -> InvoiceDetail
                                foreach (Rate rate in feeRate.Rates)
                                {
                                    // Generating Invoice Details based on the Rate which is configured on the Fee
                                    FeeInvoiceDetail invoiceDetail = new FeeInvoiceDetail();
                                    invoiceDetail.Id = Guid.NewGuid();
                                    invoiceDetail.InvoiceId = invoice.Id;
                                    //invoiceDetail.Rate = rate;
                                    invoiceDetail.RateId = rate.Id;

                                    invoiceDetail.FromDate = feeFromDate.Date;
                                    invoiceDetail.ToDate = feeToDate.Date;
                                    if (invoiceDetail.FromDate.Date <= rate.EffectiveDate.Date && feeToDate.Date >= rate.EffectiveDate.Date)
                                        invoiceDetail.FromDate = rate.EffectiveDate.Date;
                                    if (rate.ClosingDate.HasValue && invoiceDetail.FromDate.Date <= rate.ClosingDate.Value.Date && invoiceDetail.ToDate.Date >= rate.ClosingDate.Value.Date)
                                        invoiceDetail.ToDate = rate.ClosingDate.Value.Date;

                                    // Refactoring
                                    switch (feeRate.RateMethod)
                                    {
                                        case Finance.Domain.Aggregates.FeeAgg.RateMethod.Hourly:
                                            this.AddHourlyRateToInvoice(invoice, invoiceDetail, rate as HourlyRate);
                                            break;
                                        case Finance.Domain.Aggregates.FeeAgg.RateMethod.Daily:
                                            this.AddDailyRateToInvoice(invoice, invoiceDetail, rate as DailyRate);
                                            break;
                                        case Finance.Domain.Aggregates.FeeAgg.RateMethod.Weekly:
                                            this.AddWeeklyRateToInvoice(invoice, invoiceDetail, rate as WeeklyRate);
                                            break;
                                        case Finance.Domain.Aggregates.FeeAgg.RateMethod.Monthly:
                                            this.AddMonthlyRateToInvoice(invoice, invoiceDetail, rate as MonthlyRate);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                #endregion
                            }
                        }
                        invoice.Amount = invoice.DueAmount;
                        Customer Customer = _CustomerRepository.GetCustomer(invoice.CustomerId.Value);
                        Customer.Debits += invoice.Amount;                        
                    }
                    #endregion
                    this._InvoiceRepository.UnitOfWork.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _InvoiceRepository.UnitOfWork.RollBackTransaction();
                    throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion
        #region TRANSACTIONS
        public IEnumerable<FinancialTransactionModel> GetAllTransactions(DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                IEnumerable<FinancialTransaction> financialTransaction = this._FinanceTransactionRepository.GetAllTransactions(InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount);
                return financialTransaction.ProjectedToCollection<FinancialTransactionModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion
        #region INVOICES
        public IEnumerable<InvoiceModel> GetInvoices(string InvoiceNos)
        {
            try
            {
                ICollection<Invoice> invoices = new List<Invoice>();
                int invoiceNo = 0;
                foreach (string strInvoiceNo in InvoiceNos.Split(new char[] { ';', ',', ':' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int.TryParse(strInvoiceNo, out invoiceNo);
                    invoices.Add(this._FinanceTransactionRepository.GetTransaction<Invoice>(invoiceNo));
                }
                IEnumerable<InvoiceModel> invoiceModels = invoices.ProjectedToCollection<InvoiceModel>();
                return invoiceModels;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceModel> GetInvoices(DateTime ProcessedDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                return _FinanceTransactionRepository.GetTransactions<Invoice>(ProcessedDate.Date, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceModel> GetInvoices(bool IsPaid, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                return this._InvoiceRepository.GetInvoices(IsPaid, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceModel> GetInvoices(DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                return this._FinanceTransactionRepository.GetTransactions<Invoice>(InvoiceFromDate, InvoiceToDate, PageIndex, PageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceModel> GetInvoices(Web.Model.InvoiceStatus invoiceStatus, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                switch (invoiceStatus)
                {
                    case Finance.Web.Model.InvoiceStatus.New:
                        return this._InvoiceRepository.GetInvoices(Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceStatus.Draft, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
                    case Finance.Web.Model.InvoiceStatus.Void:
                        return this._InvoiceRepository.GetInvoices(Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceStatus.Void, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
                    case Finance.Web.Model.InvoiceStatus.Cancel:
                        return this._InvoiceRepository.GetInvoices(Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceStatus.Cancel, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
                    case Finance.Web.Model.InvoiceStatus.Approved:
                        return this._InvoiceRepository.GetInvoices(Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceStatus.Approved, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
                    case Finance.Web.Model.InvoiceStatus.Paid:
                        return this._InvoiceRepository.GetInvoices(Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceStatus.Paid, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
                }
                return new List<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceModel> GetInvoices(string FunderName, string CustomerName, int CustomerNo, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                IEnumerable<InvoiceModel> invoices = this._FinanceTransactionRepository.GetTransactions<Invoice>(FunderName, CustomerName, CustomerNo, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
                return invoices;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceModel> GetInvoicesByCustomer(Guid CustomerId, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize)
        {
            try
            {
                int TotalRowCount = 0;
                return this._FinanceTransactionRepository.GetTransactionsByCustomer<Invoice>(CustomerId, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceModel> GetInvoicesByFee(Guid FeeId, DateTime? InvoiceFromDate, DateTime? InvoiceToDate, int pageIndex, int pageSize)
        {
            try
            {
                return this._InvoiceRepository.GetInvoicesByFee(FeeId, InvoiceFromDate, InvoiceToDate, pageIndex, pageSize).ProjectedToCollection<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceModel> GetInvoicesForPayment(Guid FunderId, DateTime ReceiptDate)
        {
            try
            {
                IEnumerable<Invoice> invoices = this._InvoiceRepository.GetInvoicesForProcessing(FunderId, ReceiptDate.Date);
                return invoices.Where(inv => inv.DueAmount > 0).ProjectedToCollection<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public InvoiceModel GetInvoice(Guid Invoiceid)
        {
            try
            {
                return this._FinanceTransactionRepository.GetTransaction<Invoice>(Invoiceid).ProjectedTo<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public InvoiceModel GetInvoice(int InvoiceNo)
        {
            try
            {
                return this._FinanceTransactionRepository.GetTransaction<Invoice>(InvoiceNo).ProjectedTo<InvoiceModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public void GetNoOfInvoicesByStatus(out int New, out int Approved, out int Paid, out int Deleted, out int Cancelled, out int UnApproved, out int UnPaid)
        {
            try
            {
                New = 0;
                Approved = 0;
                Paid = 0;
                Deleted = 0;
                Cancelled = 0;
                UnApproved = 0;
                UnPaid = 0;
                _InvoiceRepository.GetNoOfInvoicesByStatus(out New, out Approved, out Paid, out Deleted, out Cancelled, out UnApproved, out UnPaid);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public InvoiceModel UpdateInvoiceStatus(int InvoiceNo, Web.Model.InvoiceStatus invoiceStatus)
        {
            try
            {
                Invoice invoice = this._FinanceTransactionRepository.GetTransaction<Invoice>(InvoiceNo);
                invoice.InvoiceStatus = Mapper.Map<Finance.Domain.Aggregates.FinancialTransactionAgg.InvoiceStatus>(invoiceStatus);
                this._InvoiceRepository.UnitOfWork.Commit();
                return this.GetInvoice(InvoiceNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public InvoiceModel UpdateInvoice(InvoiceModel invoiceModel)
        {
            try
            {
                Invoice invoice = this._FinanceTransactionRepository.GetTransaction<Invoice>(invoiceModel.Id);
                if (invoice == null)
                {
                    invoiceModel.Id = Guid.NewGuid();
                    invoice = DataProjections.ProjectedTo<Invoice>(invoiceModel);                    
                    invoice.InvoiceNo = this._FinanceTransactionRepository.GetNewTransactionRefNo<Invoice>();
                    invoice.TransactionStatus = TransactionStatus.New;
                    invoice.TransactionRef = invoice.InvoiceNo;
                    invoice.PayContactReference = string.IsNullOrEmpty(invoice.PayContactReference.Trim()) ? invoice.InvoiceNo.ToString() : invoice.PayContactReference;
                    invoice.Discriminator = invoice.InvoiceType.ToString();
                    invoice.InvoiceStatus = Domain.Aggregates.FinancialTransactionAgg.InvoiceStatus.Draft;
                    invoice.InvoiceMonth = new DateTime(invoice.ProcessedDate.Year, invoice.ProcessedDate.Month, 1);
                    invoice.InvoiceDate = invoice.ProcessedDate.Date;
                    foreach (InvoiceDetail invoiceDetail in invoice.InvoiceDetails)
                    {
                        invoiceDetail.Id = Guid.NewGuid();
                        invoiceDetail.InvoiceId = invoice.Id;
                    }
                    _InvoiceRepository.Add(invoice);
                }
                else
                {
                    invoice.Customer.Debits -= invoice.Amount;
                    if(invoice.ReceiptInvoices != null && invoice.ReceiptInvoices.Count > 0)
                    {
                        throw new Exception(string.Format("Invoice {0}  cannot be modified", invoice.InvoiceNo));
                    }
                    Invoice invoiceCurrent = DataProjections.ProjectedTo<Invoice>(invoiceModel);
                    if (invoice.CustomerId != invoiceCurrent.CustomerId){
                        Customer customer = this._CustomerRepository.GetCustomer(invoice.CustomerId.Value);
                    }
                    this._InvoiceRepository.Merge(invoice, invoiceCurrent);
                }
                try
                {
                    invoice.Customer = this._CustomerRepository.GetCustomer(invoice.CustomerId.Value);
                    invoice.Customer.Debits += invoice.Amount;
                    this._InvoiceRepository.UnitOfWork.BeginTransaction();
                    this._InvoiceRepository.UnitOfWork.SaveChanges();
                    this._InvoiceRepository.UnitOfWork.Commit();
                }
                catch (Exception ex) { this._InvoiceRepository.UnitOfWork.RollBackTransaction(); throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message); }
                return this.GetInvoice(invoiceModel.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public DateTime? GetFeeInvoiceProcessedMaxDate()
        {
            try
            {
                DateTime? maxDate = this._FinanceTransactionRepository.GetProcessedMaxDate<FeeInvoice>();
                return maxDate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion
        #region INVOICE ARTICLE TEMPLATE
        public IEnumerable<InvoiceArticleTemplateModel> GetInvoiceArticleTemplates(int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                return this._InvoiceRepository.GetInvoiceArticleTemplates(pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceArticleTemplateModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceArticleTemplateModel> GetInvoiceArticleTemplates(string TemplateName, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                return this._InvoiceRepository.GetInvoiceArticleTemplates(TemplateName,pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<InvoiceArticleTemplateModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public InvoiceArticleTemplateModel GetInvoiceArticleTemplate(Guid TemplateId)
        {
            try
            {
                return this._InvoiceRepository.GetInvoiceArticleTemplate(TemplateId).ProjectedTo<InvoiceArticleTemplateModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public InvoiceArticleTemplateModel UpdateInvoiceArticleTemplate(InvoiceArticleTemplateModel Template)
        {
            try
            {
                return this._InvoiceRepository.UpdateInvoiceArticleTemplate(DataProjections.ProjectedTo<InvoiceArticleTemplate>(Template)).ProjectedTo<InvoiceArticleTemplateModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public bool DeleteInvoiceArticleTemplate(Guid TemplateId)
        {
            try
            {
                return this._InvoiceRepository.DeleteInvoiceArticleTemplate(TemplateId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion
    }
}