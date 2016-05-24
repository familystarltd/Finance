using System;
using System.Linq;
using System.Collections.Generic;
using FinanceManagement.Web.Model;
using FinanceManagement.Domain.Aggregates.FeeAgg;
using System.Infrastructure.CrossCutting.Framework.Extensions;

namespace FinanceManagement.Application.Service
{
    public class FeeAppService : IFeeAppService
    {
        IFeeRepository _FeeRepository;
        public FeeAppService(IFeeRepository feeRepository)
        {
            _FeeRepository = feeRepository;
        }
        public FeeModel GetFee(Guid FeeId)
        {
            try
            {
                return this._FeeRepository.GetFee(FeeId).ProjectedTo<FeeModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public FeeModel GetFeeForEdit(Guid FeeId)
        {
            try
            {
                return this._FeeRepository.GetFeeForEdit(FeeId).ProjectedTo<FeeModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<FeeModel> GetFees(Guid customerId)
        {
            try
            {
                return this._FeeRepository.GetFees(customerId).ProjectedToCollection<FeeModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<FeeModel> GetFees(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return this._FeeRepository.GetFees(fromDate.Date,toDate.Date).ProjectedToCollection<FeeModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public FeeRateModel GetFeeRate(Guid FeeRateId)
        {
            try
            {
                return this._FeeRepository.GetFeeRate(FeeRateId).ProjectedTo<FeeRateModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #region PRIVATE METHODS
        private void AddNewFeeRate(FeeRate feeRate)
        {
            if (feeRate.Id == Guid.Empty)
                feeRate.Id = Guid.NewGuid();
            _FeeRepository.AddFeeRate(feeRate);
            foreach (Rate rate in feeRate.Rates)
            {
                rate.FeeRateId = feeRate.Id;
                if (rate.Id == Guid.Empty)
                    rate.Id = Guid.NewGuid();
            }

        }
        private IEnumerable<Rate> CheckAnyChangesOnFeeRates(FeeRate FeeRate, IEnumerable<Rate> RatesPersistence)
        {
            List<Rate> ActiveRates = RatesPersistence.ToList().FindAll(r => r.ClosingDate == null || r.ClosingDate.Value == DateTime.MinValue);
            if (ActiveRates != null && ActiveRates.Count() > 0)
            {
                // As long as Closing Date has been set by Server (not by client) for Rate, this could be possible 
                foreach (Rate rate in FeeRate.Rates.ToList().FindAll(r => r.ClosingDate == null || r.ClosingDate.Value == DateTime.MinValue))
                {
                    #region CODE
                    Rate ratePersistence = ActiveRates.SingleOrDefault(r => r.Id == rate.Id);
                    Rate rateCurrent = FeeRate.Rates.ToList().SingleOrDefault(r => r.Id == rate.Id);
                    if (ratePersistence == null)
                    {
                        rateCurrent.Id = Guid.NewGuid();
                        rateCurrent.FeeRateId = FeeRate.Id;
                        //FeeRate.Rates.Add(rateCurrent);
                        _FeeRepository.AddRate(rateCurrent);
                    }
                    else
                    {
                        switch (FeeRate.RateMethod)
                        {
                            case FinanceManagement.Domain.Aggregates.FeeAgg.RateMethod.Hourly:
                                HourlyRate hourlyRate = rateCurrent as HourlyRate;
                                HourlyRate hourlyRatePersistence = ratePersistence as HourlyRate;
                                if (hourlyRatePersistence.RateAmount != hourlyRate.RateAmount
                                    ||
                                    hourlyRatePersistence.NoOfHours != hourlyRate.NoOfHours
                                    )
                                {

                                    _FeeRepository.AddRate(
                                        new HourlyRate
                                        {
                                            Id = Guid.NewGuid(),
                                            FeeRateId = hourlyRate.FeeRateId,
                                            RateAmount = hourlyRate.RateAmount,
                                            NoOfHours = hourlyRate.NoOfHours,
                                            EffectiveDate = hourlyRate.EffectiveDate,
                                            DayPremium = hourlyRate.DayPremium,
                                            TimePremium = hourlyRate.TimePremium,
                                            ClosingDate = null
                                        });
                                    _FeeRepository.RemoveRate(hourlyRatePersistence);
                                    if (ratePersistence.EffectiveDate.Date != rateCurrent.EffectiveDate.Date)
                                    {
                                        _FeeRepository.AddRate(
                                        new HourlyRate
                                        {
                                            Id = ratePersistence.Id,
                                            FeeRateId = hourlyRatePersistence.FeeRateId,
                                            RateAmount = hourlyRatePersistence.RateAmount,
                                            NoOfHours = hourlyRatePersistence.NoOfHours,
                                            EffectiveDate = hourlyRatePersistence.EffectiveDate,
                                            DayPremium = hourlyRatePersistence.DayPremium,
                                            TimePremium = hourlyRatePersistence.TimePremium,
                                            ClosingDate = rate.EffectiveDate.AddDays(-1)
                                        });
                                    }
                                }
                                break;
                            case FinanceManagement.Domain.Aggregates.FeeAgg.RateMethod.Daily:
                                DailyRate dailyRate = rateCurrent as DailyRate;
                                DailyRate dailyRatePersistence = ratePersistence as DailyRate;
                                if (dailyRatePersistence.RateAmount != dailyRate.RateAmount)
                                {
                                    _FeeRepository.AddRate(
                                        new DailyRate
                                        {
                                            Id = Guid.NewGuid(),
                                            RateAmount = dailyRate.RateAmount,
                                            EffectiveDate = dailyRate.EffectiveDate,
                                            FeeRateId = dailyRate.FeeRateId,
                                            DayPremium = dailyRate.DayPremium,
                                            ClosingDate = null
                                        });
                                    _FeeRepository.RemoveRate(dailyRatePersistence);
                                    if (ratePersistence.EffectiveDate.Date != rateCurrent.EffectiveDate.Date)
                                    {
                                        _FeeRepository.AddRate(
                                        new DailyRate
                                        {
                                            Id = ratePersistence.Id,
                                            RateAmount = dailyRatePersistence.RateAmount,
                                            EffectiveDate = dailyRatePersistence.EffectiveDate,
                                            FeeRateId = dailyRatePersistence.FeeRateId,
                                            DayPremium = dailyRatePersistence.DayPremium,
                                            ClosingDate = rate.EffectiveDate.AddDays(-1)
                                        });

                                        //dailyRatePersistence.ClosingDate = dailyRate.EffectiveDate.AddDays(-1);
                                        //financeContext.ApplyCurrentValues(dailyRatePersistence, dailyRate);
                                    }
                                }
                                break;
                            case FinanceManagement.Domain.Aggregates.FeeAgg.RateMethod.Weekly:
                                if (ratePersistence.RateAmount != rateCurrent.RateAmount)
                                {
                                    _FeeRepository.AddRate(
                                        new WeeklyRate
                                        {
                                            Id = Guid.NewGuid(),
                                            RateAmount = rateCurrent.RateAmount,
                                            EffectiveDate = rateCurrent.EffectiveDate,
                                            FeeRateId = rateCurrent.FeeRateId,
                                            ClosingDate = null
                                        });
                                    _FeeRepository.RemoveRate(ratePersistence);
                                    if (ratePersistence.EffectiveDate.Date != rateCurrent.EffectiveDate.Date)
                                    {
                                        _FeeRepository.AddRate(
                                            new WeeklyRate
                                            {
                                                Id = ratePersistence.Id,
                                                RateAmount = ratePersistence.RateAmount,
                                                EffectiveDate = ratePersistence.EffectiveDate,
                                                FeeRateId = ratePersistence.FeeRateId,
                                                ClosingDate = rate.EffectiveDate.AddDays(-1)
                                            });
                                    }
                                }
                                break;
                            case FinanceManagement.Domain.Aggregates.FeeAgg.RateMethod.Monthly:
                                MonthlyRate monthlyRate = rateCurrent as MonthlyRate;
                                if (ratePersistence.RateAmount != monthlyRate.RateAmount)
                                {
                                    _FeeRepository.AddRate(
                                        new MonthlyRate
                                        {
                                            Id = Guid.NewGuid(),
                                            RateAmount = monthlyRate.RateAmount,
                                            EffectiveDate = monthlyRate.EffectiveDate,
                                            FeeRateId = monthlyRate.FeeRateId,
                                            ClosingDate = null
                                        });
                                    _FeeRepository.RemoveRate(ratePersistence);
                                    _FeeRepository.AddRate(
                                        new MonthlyRate
                                        {
                                            Id = ratePersistence.Id,
                                            RateAmount = ratePersistence.RateAmount,
                                            EffectiveDate = ratePersistence.EffectiveDate,
                                            FeeRateId = ratePersistence.FeeRateId,
                                            ClosingDate = rate.EffectiveDate.AddDays(-1)
                                        });
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                }
            }
            return new List<Rate>();
        }
        #endregion
        public FeeModel SaveFee(FeeModel feeModel)
        {
            try
            {
                Fee fee = DataProjections.ProjectedTo<Fee>(feeModel);
                Fee feePersistent = this._FeeRepository.GetFeeForEdit(fee.Id);
                if (feePersistent == null)
                {
                    fee.Id = Guid.NewGuid();
                    foreach (FeeRate feeRate in fee.FeeRates)
                    {
                        feeRate.FeeId = fee.Id;
                        this.AddNewFeeRate(feeRate);
                    }
                    _FeeRepository.Add(fee);
                    this._FeeRepository.UnitOfWork.SaveChanges();
                    feeModel.Id = fee.Id;
                }
                else
                {
                    FeeRate feeRatePersistence;
                    foreach (FeeRate feeRateCurrent in fee.FeeRates)
                    {
                        feeRatePersistence = feePersistent.FeeRates.SingleOrDefault(fr => fr.Id == feeRateCurrent.Id);
                        if (feeRatePersistence == null)
                        {
                            this.AddNewFeeRate(feeRateCurrent);
                        }
                        else
                        {
                            CheckAnyChangesOnFeeRates(feeRateCurrent, feeRatePersistence.Rates);
                            _FeeRepository.MergeFeeRate(feeRatePersistence, feeRateCurrent);
                        }
                    }
                    this._FeeRepository.Merge(feePersistent, fee);
                    this._FeeRepository.UnitOfWork.Commit();
                }
                return _FeeRepository.GetFee(feeModel.Id).ProjectedTo<FeeModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}