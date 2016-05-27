namespace System.Domain.Specification
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// NotEspecification convert a original
    /// specification with NOT logic operator
    /// </summary>
    /// <typeparam name="TEntity">Type of element for this specificaiton</typeparam>
    public sealed class NOTSpecification<T> : Specification<T> where T : class
    {
        #region MEMBERS

        Expression<Func<T, bool>> _OriginalCriteria;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor for NotSpecificaiton
        /// </summary>
        /// <param name="originalSpecification">Original specification</param>
        public NOTSpecification(ISpecification<T> originalSpecification)
        {

            if (originalSpecification == (ISpecification<T>)null)
                throw new ArgumentNullException("originalSpecification");

            _OriginalCriteria = originalSpecification.SatisfiedBy();
        }

        /// <summary>
        /// Constructor for NotSpecification
        /// </summary>
        /// <param name="originalSpecification">Original specificaiton</param>
        public NOTSpecification(Expression<Func<T, bool>> originalSpecification)
        {
            if (originalSpecification == (Expression<Func<T, bool>>)null)
                throw new ArgumentNullException("originalSpecification");

            _OriginalCriteria = originalSpecification;
        }

        #endregion

        #region OVERRIDE SPECIFICATION METHODS

        /// <summary>
        /// <see cref="Microsoft.Samples.NLayerApp.Domain.Seedwork.Specification.ISpecification{TEntity}"/>
        /// </summary>
        /// <returns><see cref="Microsoft.Samples.NLayerApp.Domain.Seedwork.Specification.ISpecification{TEntity}"/></returns>
        public override Expression<Func<T, bool>> SatisfiedBy()
        {

            return Expression.Lambda<Func<T, bool>>(Expression.Not(_OriginalCriteria.Body),
                                                         _OriginalCriteria.Parameters.Single());
        }

        #endregion
    }
}
