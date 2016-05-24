using System;
using System.Domain;

namespace FinanceManagement.Domain.Aggregates.FeeAgg
{
    public abstract class Rate : Entity
    {
        /// <summary>
        /// Get or Set the associated Rate for this RateDetail
        /// </summary>
        public Guid FeeRateId { get; set; }
        public virtual FeeRate FeeRate { get; set; }
        public decimal RateAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
    public class WeeklyRate : Rate { }
    public class MonthlyRate : Rate { }
    public class DailyRate : Rate { public DayPremium DayPremium { get; set; } }
    public class HourlyRate : DailyRate
    {
        public TimePremium TimePremium { get; set; }
        public int NoOfHours { get; set; }
        public decimal TotalRate { get { return NoOfHours * RateAmount; } }
    }
    //public class HourlyRate : DailyRate
    //{
    //    public TimePremium TimePremium { get; set; }
    //    public int NoOfHours { get; set; }
    //    public decimal TotalRate { get { return NoOfHours * RateAmount; } }
    //}
}
