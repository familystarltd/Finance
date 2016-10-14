namespace Finance.Domain.Aggregates.CustomerAgg
{
    using System;
    using System.Collections.Generic;

    using System.Domain;

    public enum RelationType
    {
        NextOfKin,
        GP
    }
    public enum NameTitle
    {
        Mr = 1,
        Mrs = 2,
        Miss = 3,
        Ms = 4,
        Mas = 5,
        Mx = 6,
        Dr = 7,
        Sir = 8,
        Rev = 9
    }
    public enum GenderType
    {
        Male = 1,
        Female = 2,
        Heterosexual = 3,
        Homosexual = 4,
        Bisexual = 5,
        Pansexual = 6,
        Polysexual = 7,
        Asexual = 8
    }

    public enum MaritalType
    {
        Single = 1,
        Married = 2,
        Divorced = 3,
        Widowed = 4,
        Separated = 5
    }
    public class Contact : Entity
    {
        public NameTitle Title { get; set; }
        /// <summary>
        /// Get or set the Given name of this contact
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Get or set the surname of this contact
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Get or set the full name of this contact
        /// </summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(this.FirstName) && string.IsNullOrEmpty(this.LastName))
                    return Company;
                return string.Format("{0} {1}", this.FirstName , this.LastName);
            }
            set { }
        }

        /// <summary>
        /// Get or set the Gender of this contact
        /// </summary>
        public GenderType Gender { get; set; }

        /// <summary>
        /// Get or set the company name
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Get or set the department
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Get or set the contact ContactReference
        /// </summary>
        public string ContactReference { get; set; }

        /// <summary>
        /// Get or set the telephone 
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Get or set the email address 
        /// </summary>
       public string Email { get; set; }

        /// <summary>
        /// Get or set the address of this contact
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Get or set the postal code
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// Get or set associated photo for this contact
        /// </summary>
        public byte[] Photo { get; set; }
        /// <summary>
        /// Get or set the Date of Birth
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
    }
}