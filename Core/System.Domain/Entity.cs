namespace System.Domain
{
    using System;

    /// <summary>
    /// Base class for entities (maily for domain model)
    /// </summary>
    public abstract class Entity
    {
        #region Members
        int? requestedHashCode;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Persistent object identifier
        /// </summary>
        public virtual Guid Id
        {
            get;
            set;
        }
        public string LogUser { get; set; }

        private DateTime? logDateTime;
        public DateTime? LogDateTime { get; set; }

        #endregion

        #region public Methods
        /// <summary>
        /// Check if this entity is transient, i.e. without identity at this moment
        /// </summary>
        /// <returns>True is entity is transient, else false</returns>
        public bool IsTransient()
        {
            return this.Id == null || this.Id == Guid.Empty;
        }
        /// <summary>
        /// Generate the new identity for this entity
        /// </summary>
        public void GenerateNewIdentity()
        {
            if (this.IsTransient())
                this.Id = IdentityGenerator.NewSequentialGuid();
        }

        /// <summary>
        /// Change current for new a new non transient identity
        /// </summary>
        /// <param name="identity">the new identity</param>
        public void ChangeCurrentIdentity(Guid identity)
        {
            if (identity != Guid.Empty && identity != null)
                this.Id = identity;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// <see cref="M:System.Object.Equals"/>
        /// </summary>
        /// <param name="obj"><see cref="M:System.Object.Equals"/> </param>
        /// <returns><see cref="M:System.Object.Equals"/> </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            Entity item = (Entity)obj;
            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return true;
        }
        /// <summary>
        /// <see cref="M:System.Object.GetHashCode"/>
        /// </summary>
        /// <returns><see cref="M:System.Object.GetHashCode"/></returns>
        public override int GetHashCode()
        {
            if (!this.IsTransient())
            {
                if (!this.requestedHashCode.HasValue)
                    this.requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                return requestedHashCode.Value;
            }
            return base.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
        #endregion
    }
}
