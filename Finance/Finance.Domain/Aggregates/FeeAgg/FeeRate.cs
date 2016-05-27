using System;
using System.Collections.Generic;
using System.Domain;

namespace Finance.Domain.Aggregates.FeeAgg
{
    public class FeeRate : Entity
    {
        /// <summary>
        /// Get or Set the associated Fee for this Rate
        /// </summary>
        public Guid FeeId { get; set; }
        public Fee Fee { get; set; }
        /// <summary>
        /// Get or Set the method of rate for this Fee
        /// </summary>
        public RateMethod RateMethod { get; set; }
        public string RateDescription { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }
    }
}