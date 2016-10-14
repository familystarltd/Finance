using Finance.Web.Model;
using System;
using System.Collections.Generic;

namespace Finance.Application.Service
{
    public interface IFeeAppService
    {
        FeeModel GetFee(Guid FeeId);
        FeeModel GetFeeForEdit(Guid FeeId);
        IEnumerable<FeeModel> GetFees(Guid customerId);
        IEnumerable<FeeModel> GetFees(DateTime fromDate, DateTime toDate);
        FeeRateModel GetFeeRate(Guid FeeRateId);
        FeeModel SaveFee(FeeModel feeModel);
        DateTime? GetMinFeeDate();
    }
}
