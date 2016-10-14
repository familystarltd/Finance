using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Web.Model
{
    public class FeeRateModel
    {
        public FeeRateModel()
        {
            Rates = new List<RateModel>();
        }
        /// <summary>
        /// Id for the Rate model which is Unique Identifier
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Get or Set the associated Fee for this Rate
        /// </summary>
        public Guid FeeId { get; set; }
        public FeeModel Fee { get; set; }
        /// <summary>
        /// Get or Set the method of rate for this Fee
        /// </summary>
        public RateMethod RateMethod { get; set; }
        public string RateDescription { get; set; }
        public ICollection<RateModel> Rates { get; set; }
    }
}
