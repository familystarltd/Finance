namespace Finance.Web.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

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

    /// <summary>
    /// This is the data transfer object for Contact entity. The name of properties for this type is based on conventions of many mappers
    /// to simplificate the mapping process
    /// </summary>
    public class ContactModel
    {
        public ContactModel()
        {
            Address = string.Empty;
            ContactReference = string.Empty;
        }
        /// <summary>
        /// The Contact identifier
        /// </summary>
        public Guid Id { get; set; }

        public bool EnableValidation { get; set; }

        /// <summary>
        /// Get or set the Given name of this resident
        /// </summary>
        [Required(ErrorMessage = "Please select title")]
        public NameTitle Title { get; set; }

        /// <summary>
        /// Get or set the Given name of this resident
        /// </summary>
        [Required(ErrorMessage = "Please enter First name")]
        [StringLength(250, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Get or set the surname of this resident
        /// </summary>
        [Required(ErrorMessage = "Please enter Surname")]
        [StringLength(50, ErrorMessage = "Sur name cannot be longer than 50 characters.")]
        [Display(Name = "Surname")]
        public string LastName { get; set; }

        /// <summary>
        /// Get or set the full name of this resident
        /// </summary>
        public string FullName
        {
            get
            {
                return string.IsNullOrEmpty(this.FirstName) ? string.Empty : string.Format("{0} {1}", this.FirstName, this.LastName);
            }
            set { }
        }

        /// <summary>
        /// Get or set the Gender of this contact
        /// </summary>
        public GenderType Gender { get; set; }

        /// <summary>
        /// Get the raw of photo
        /// </summary>
        public byte[] PictureRawPhoto { get; set; }

        /// <summary>
        /// Get or set the company name
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Get or set the name of department
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
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please enter valid e mail address. (e.g. yourname@domain.com)")]
        public string Email { get; set; }

        /// <summary>
        /// Get or set the address of this contact
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Get or set the postal code
        /// </summary>
        public string PostCode { get; set; }

        public bool Valid()
        {

            //-->Check first name property
            if (String.IsNullOrWhiteSpace(this.FirstName))
            {
                return false;
            }
            //-->Check last name property
            if (String.IsNullOrWhiteSpace(this.LastName))
            {
                return false;
            }
            return true;
        }
    }
}