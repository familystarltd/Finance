using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Domain.Aggregates.CustomerAgg
{
    /// <summary>
    /// This is the factory for Contact creation, which means that the main purpose
    /// is to encapsulate the creation knowledge.
    /// What is created is a transient entity instance, with nothing being said about persistence as yet
    /// </summary>
    public static class ContactFactory
    {
        /// <summary>
        /// Create a new transient contact
        /// </summary>
        /// <param name="contact">The contact</param>
        /// <returns>A valid contact</returns>
        public static Contact MaterializeContact1(Contact contact)
        {
            if (contact == null)
                return null;
            if(string.IsNullOrEmpty(contact.FirstName) &&  string.IsNullOrEmpty(contact.LastName) && string.IsNullOrEmpty(contact.Company))
            {
                return null;
            }
            if ((string.IsNullOrEmpty(contact.FirstName) || string.IsNullOrEmpty(contact.LastName)) && !string.IsNullOrEmpty(contact.Company))
            {
                contact.FirstName = string.IsNullOrEmpty(contact.FirstName) ? string.Empty : contact.FirstName;
                contact.LastName = string.IsNullOrEmpty(contact.LastName) ? string.Empty : contact.LastName;
                return contact;
            }
            if (!string.IsNullOrEmpty(contact.FirstName) && !string.IsNullOrEmpty(contact.LastName))
            {
                return contact;
            }
            return null;
        }
    }
}
