using System.Collections.Generic;
using System.Domain;

namespace Finance.Domain.Aggregates.CustomerAgg
{
    public class Business : Entity
    {
        /// <summary>
        /// Get or set the name for this business
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the Tax Office Name for this business
        /// </summary>
        public string TaxOfficeName { get; set; }
        /// <summary>
        /// Get or set the Tax ContactReference for this business
        /// </summary>
        public string PAYERef { get; set; }
        /// <summary>
        /// Get or set the address of the business
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Get or set the post code for this contact
        /// </summary>
        public string PostCode { get; set; }
        /// <summary>
        /// Get or set the telephone 
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// Get or set the Fax 
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// Get or set the email address 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Get or set logo for this business
        /// </summary>
        public byte[] Logo { get; set; }
    }
}
