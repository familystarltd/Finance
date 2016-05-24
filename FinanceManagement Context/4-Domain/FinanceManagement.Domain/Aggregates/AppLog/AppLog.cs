using System;
using System.Collections.Generic;
using System.Domain;
namespace FinanceManagement.Domain
{
    public class AppLog : Entity
    {
        /// <summary>
        /// Get or set the company name
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// Get or set the Request name
        /// </summary>
        public string Request { get; set; }
        /// <summary>
        /// Get or set the Request From
        /// </summary>
        public string RequestFrom { get; set; }
        /// <summary>
        /// Get or set the Request Method name
        /// </summary>
        public string RequestMethod { get; set; }
    }
}