using System;
using System.Linq;
using System.Collections.Generic;
using System.Infrastructure.Data;
using Finance.Infrastructure.Data.UnitOfWork;
using Finance.Domain.Aggregates.FeeAgg;
using Finance.Domain.Aggregates.CustomerAgg;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Customer Repository implementation.
    /// <see cref="Finance.Domain.ICustomerRepository"/>
    /// </summary>
    public class FeeRepository : Repository<Fee>, IFeeRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public FeeRepository(IFinanceDbContext unitOfWork) : base(unitOfWork) { }

        public override void Merge(Fee persisted, Fee current)
        {
            base.Merge(persisted, current);
        }

        public IEnumerable<Fee> GetFees(Guid customerId)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Fee> GetFees(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                IEnumerable<Fee> fees = uow.Fees
                    .Include(f => f.Customer.PersonalInfo)
                    .Include(f => f.Payer)
                    .Include(f => f.Notes)
                    .Where(f =>
                        (!f.Customer.DeactiveDate.HasValue || (f.Customer.DeactiveDate.HasValue && (f.Customer.DeactiveDate.Value >= fromDate.Date && f.Customer.DeactiveDate.Value <= toDate.Date)))
                        &&
                        (f.EffectiveDate <= fromDate.Date)
                        &&
                        (!f.ClosingDate.HasValue || (f.ClosingDate.HasValue && (f.ClosingDate.Value >= fromDate.Date)))).ToList()
                    .Select(
                    fee => new Fee
                    {
                        Id = fee.Id,
                        Name = fee.Name,
                        Description = fee.Description,
                        Payer = fee.Payer,
                        PayerId = fee.PayerId,
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
                                                                }).ToList()});
                return fees;
            } // a ? b : (c ? d : e)
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Fee> GetFeesForInvoiceProcess(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                IEnumerable<Fee> fees = uow.Fees
                    .Include(f => f.Customer)
                    .Include(f => f.Notes)
                    .Where(f =>
                        (!f.Customer.DeactiveDate.HasValue || (f.Customer.DeactiveDate.HasValue && (f.Customer.DeactiveDate.Value >= fromDate.Date && f.Customer.DeactiveDate.Value <= toDate.Date)))
                        &&
                        (f.EffectiveDate <= fromDate.Date)
                        &&
                        (!f.ClosingDate.HasValue || (f.ClosingDate.HasValue && (f.ClosingDate.Value >= fromDate.Date)))).ToList()
                    .Select(
                    fee => new Fee
                    {
                        Id = fee.Id,
                        Name = fee.Name,
                        Description = fee.Description,
                        PayerId = fee.PayerId,
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
                                                                //        .Select(
                                                                //rate => new WeeklyRate
                                                                //{
                                                                //    Id = rate.Id,
                                                                //    FeeRate = null,
                                                                //    FeeRateId = rate.FeeRateId,
                                                                //    ClosingDate = rate.ClosingDate,
                                                                //    EffectiveDate = rate.EffectiveDate,
                                                                //    RateAmount = rate.RateAmount
                                                                //}).ToList()
                                                                }).ToList()
                    });
                return fees;
            } // a ? b : (c ? d : e)
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Fee GetFeeForEdit(Guid FeeId)
        {
             var uow = this.UnitOfWork as IFinanceDbContext;
             return uow.Fees
                     .Include(f => f.Customer).ThenInclude(f=>f.PersonalInfo)
                     .Include(f => f.Payer).ThenInclude(f=>f.PersonalContact)
                     .Include(f => f.Payer).ThenInclude(f => f.FeeInvoiceBillingContact)
                     .Include(f => f.Payer).ThenInclude(f => f.FeeInvoiceDeliveryContact)
                     .Include(f=>f.FeeRates).ThenInclude(fr=>fr.Rates)
                     .Include(f => f.Notes)
                     .Where(f => f.Id == FeeId).SingleOrDefault();
        }
        public Fee GetFee(Guid FeeId)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;

                Fee fee = uow.Fees
                    .Include(f => f.Customer).ThenInclude(f => f.PersonalInfo)
                     .Include(f => f.Payer).ThenInclude(f => f.PersonalContact)
                     .Include(f => f.Payer).ThenInclude(f => f.FeeInvoiceBillingContact)
                     .Include(f => f.Payer).ThenInclude(f => f.FeeInvoiceDeliveryContact)
                    .Include(f => f.Notes)
                    .Where(f => f.Id == FeeId)
                    .ToList()
                    .Select(
                    f => new Fee
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Description = f.Description,
                        Payer = f.Payer,
                        PayerId = f.PayerId,
                        Customer = f.Customer,
                        CustomerId = f.CustomerId,
                        ClosingDate = f.ClosingDate,
                        EffectiveDate = f.EffectiveDate,
                        PaymentTerm = f.PaymentTerm,
                        PayContactReference = f.PayContactReference,
                        LogDateTime = f.LogDateTime,
                        LogUser = f.LogUser,
                        Notes = f.Notes,
                        FeeRates = uow.FeeRates.Where(fr => fr.FeeId == f.Id).ToList().Select(
                                                                fr => new FeeRate
                                                                {
                                                                    Id = fr.Id,
                                                                    Fee = null,
                                                                    FeeId = f.Id,
                                                                    RateDescription = fr.RateDescription,
                                                                    RateMethod = fr.RateMethod,
                                                                    Rates = uow.Rates.Where(r => r.FeeRateId == fr.Id && !r.ClosingDate.HasValue)
                                                                        .OrderBy(r => (r is DailyRate) ? (r as DailyRate).DayPremium : ((r is HourlyRate) ? (r as HourlyRate).DayPremium : 0))
                                                                        .ThenBy((r => (r is HourlyRate) ? (r as HourlyRate).TimePremium : 0)).ToList()
                                                                }).ToList()
                    }
                    ).ToList().SingleOrDefault();
                if (fee != null)
                    fee.Customer.Fees = null;
                return fee;
            } // a ? b : (c ? d : e)
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public FeeRate GetFeeRate(Guid FeeRateId)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.FeeRates
                    .Include(fr => fr.Rates)
                    .Include(fr => fr.Fee)
                    .Where(fr => fr.Id == FeeRateId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Fee SaveFee(Fee fee)
        {
            return new Fee();
        }
        public void MergeFeeRate(FeeRate feeRatePersistence, FeeRate feeRateCurrent)
        {
            var financeContext = this.UnitOfWork as IFinanceDbContext;
            financeContext.ApplyCurrentValues(feeRatePersistence, feeRateCurrent);
        }
        public void AddFeeRate(FeeRate feeRate)
        {
            var financeContext = this.UnitOfWork as IFinanceDbContext;
            financeContext.FeeRates.Add(feeRate);
        }
        public void AddRate(Rate rate)
        {
            var financeContext = this.UnitOfWork as IFinanceDbContext;
            financeContext.Rates.Add(rate);
        }
        public void RemoveRate(Rate rate)
        {
            var financeContext = this.UnitOfWork as IFinanceDbContext;
            financeContext.Rates.Remove(rate);
        }
        public DateTime? GetMinFeeDate()
        {
            var uow = this.UnitOfWork as IFinanceDbContext;
            List<DateTime?> MinFeeDates = new List<DateTime?>();
            MinFeeDates.Add(uow.Rates.Where(r=>!r.ClosingDate.HasValue).Min(r => (DateTime?)r.EffectiveDate));
            return MinFeeDates.Max();
        }
    }
}