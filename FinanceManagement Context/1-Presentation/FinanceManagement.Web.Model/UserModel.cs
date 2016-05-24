using System;
using System.Collections;
using System.Collections.Generic;

namespace FinanceManagement.Web.Model
{
    public abstract class AppModel
    {
        public Guid Id { get; set; }
        public string LogUser { get; set; }

        public DateTime? LogDateTime { get; set; }
    }
    public class AppLogModel: AppModel
    {
        /// <summary>
        /// Get or set the company name
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// Get or set the Content of Request
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
    public class UserModel
    {
        public UserModel()
        {
            Roles = new List<string>();
        }
        /// <summary>
        /// Get or set the name of this User
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the collection of role of this User
        /// </summary>
        public ICollection<string> Roles { get; set; }
        /// <summary>
        /// Get or set the Token for this User
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Get or set the Logged In date time 
        /// </summary>
        public DateTime LoggedIn { get; set; }
    }
}