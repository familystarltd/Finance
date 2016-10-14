using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Domain.Aggregates.FeeAgg;
using System;
using System.Collections.Generic;
using System.Domain;

namespace Finance.Domain.Aggregates.FeeAgg
{
    public interface IFeeRepository : IRepository<Fee>
    {
        Fee GetFee(Guid FeeId);
        Fee GetFeeForEdit(Guid FeeId);
        IEnumerable<Fee> GetFees(Guid customerId);
        IEnumerable<Fee> GetFees(DateTime fromDate,DateTime toDate);
        IEnumerable<Fee> GetFeesForInvoiceProcess(DateTime fromDate, DateTime toDate);
        FeeRate GetFeeRate(Guid FeeRateId);
        Fee SaveFee(Fee fee);
        void MergeFeeRate(FeeRate feeRatePersistence, FeeRate feeRateCurrent);
        void AddFeeRate(FeeRate feeRate);
        void AddRate(Rate rate);
        void RemoveRate(Rate rate);
        DateTime? GetMinFeeDate();
    }
}