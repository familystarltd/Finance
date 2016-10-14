using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Finance.Web.Model
{
    public class BusinessModel
    {
        /// <summary>
        /// Id for the business which is Unique Identifier
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Name of the Business
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set address line 1
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Get or set address line 2
        /// </summary>
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Get or set address line 3
        /// </summary>
        public string AddressLine3 { get; set; }

        /// <summary>
        /// Get or set address line 4
        /// </summary>
        public string AddressLine4 { get; set; }

        /// <summary>
        /// Get or set the Council of this address 
        /// </summary>
        public string Council { get; set; }

        /// <summary>
        /// Get or set the city of this address 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Get or set the zip code
        /// </summary>
        public string PostCode { get; set; }
    }
}
