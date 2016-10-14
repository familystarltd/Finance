using System;
using System.Collections.Generic;
using System.Linq;

namespace Finance.Web.Models
{
    public class BusinessViewModel
    {
        public BusinessViewModel()
        {
            Services = new List<Service>();
        }
        public Business Business { get; set; }
        public IEnumerable<Business> Businesses { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
    public class Business : System.Domain.Entity
    {
        public Business()
        {
            Services = new List<Service>();
        }
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
        /// <summary>
        /// Get or set the Departments to this business. May have one or more departments.
        /// </summary>
        public ICollection<Department> Departments { get; set; }
        /// <summary>
        /// Get or set the Services (Care, HR, Finance) to this business. May have one or more Services.
        /// </summary>
        public ICollection<Service> Services { get; set; }
        public bool IsInService(string Service)
        {
            if (this.Services != null && this.Services.Count > 0)
                return this.Services.FirstOrDefault(s => s.Name.Equals(Service, StringComparison.OrdinalIgnoreCase)) != null;
            return false;
        }
    }
    public class Department
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Get or set the name of the Department
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the unique ID of the Business
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Get or set the Business for this Department
        /// </summary>
        public Business Business { get; set; }

    }
    public class Service
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Get or set the Businesses for this Services. May have one or more Businesses.
        /// </summary>
        public ICollection<Business> Businesses { get; set; }
    }
}