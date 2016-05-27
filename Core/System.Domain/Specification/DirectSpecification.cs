namespace System.Domain.Specification
{

    using System;
    using System.Linq.Expressions;
    /// <summary>
    /// A Direct Specification is a simple implementation
    /// of specification that acquire this from a lambda expression
    /// in  constructor
    /// </summary>
    /// <typeparam name="TEntity">Type of entity that check this specification</typeparam>
    public sealed class DirectSpecification<TEntity> : Specification<TEntity> where TEntity : class
    {
        Expression<Func<TEntity, bool>> _MatchingCriteria;

        /// <summary>
        /// Default constructor for Direct Specification
        /// </summary>
        /// <param name="matchingCriteria">A Matching Criteria</param>

        public DirectSpecification(Expression<Func<TEntity,bool>> matchingCriteria)
        {
            if (matchingCriteria == (Expression<Func<TEntity, bool>>)null)
                throw new ArgumentNullException("Matching Criteria cannot be null");

            _MatchingCriteria = matchingCriteria;
        }


        public override Expression<Func<TEntity, bool>> SatisfiedBy()
        {
            return _MatchingCriteria;
        }
    }
}
