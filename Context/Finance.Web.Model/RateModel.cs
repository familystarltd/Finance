using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finance.Web.Model
{
    public class RateModel
    {
        //FeeRateModel _FeeRate;
        public RateModel()
        {
            //FeeRate = new FeeRateModel();
        }
        /// <summary>
        /// Id for the Rate Detail model which is Unique Identifier
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Get or Set the associated Method of Rate for this RateDetail
        /// </summary>
        public RateMethod RateMethod{ get; set; }
        /// <summary>
        /// Get or Set the associated Rate for this RateDetail
        /// </summary>
        public Guid FeeRateId { get; set; }
        public FeeRateModel FeeRate { get; set; }
        public DayPremium DayPremium { get; set; }
        public TimePremium TimePremium { get; set; }
        public int NoOfHours { get; set; }
        public decimal RateAmount { get; set; }
        public decimal TotalRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
}