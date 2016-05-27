namespace System.Domain.Specification
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A Logic OR Specification
    /// </summary>
    /// <typeparam name="T">Type of entity that check this specification</typeparam>
    public sealed class ORSpecification<T> : Specification<T> where T : class
    {
        #region MEMBERS

        private ISpecification<T> _RightSideSpecification = null;
        private ISpecification<T> _LeftSideSpecification = null;

        #endregion

        #region PUBLIC CONSTRUCTOR

        /// <summary>
        /// Default constructor for AndSpecification
        /// </summary>
        /// <param name="leftSide">Left side specification</param>
        /// <param name="rightSide">Right side specification</param>
        public ORSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
        {
            if (leftSide == (ISpecification<T>)null)
                throw new ArgumentNullException("leftSide");

            if (rightSide == (ISpecification<T>)null)
                throw new ArgumentNullException("rightSide");

            this._LeftSideSpecification = leftSide;
            this._RightSideSpecification = rightSide;
        }

        #endregion

        /// <summary>
        /// Left side specification
        /// </summary>
        public ISpecification<T> LeftSideSpecification
        {
            get { return _LeftSideSpecification; }
        }

        /// <summary>
        /// Righ side specification
        /// </summary>
        public ISpecification<T> RightSideSpecification
        {
            get { return _RightSideSpecification; }
        }
        /// <summary>
        /// <see cref="Microsoft.Samples.NLayerApp.Domain.Seedwork.Specification.ISpecification{T}"/>
        /// </summary>
        /// <returns><see cref="Microsoft.Samples.NLayerApp.Domain.Seedwork.Specification.ISpecification{T}"/></returns>
        public override Expression<Func<T, bool>> SatisfiedBy()
        {
            Expression<Func<T, bool>> left = _LeftSideSpecification.SatisfiedBy();
            Expression<Func<T, bool>> right = _RightSideSpecification.SatisfiedBy();

            return (left.Or(right));

        }
    }
}
