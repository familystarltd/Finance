namespace System.Infrastructure.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Domain;
    using System.Domain.Specification;
    using System.Infrastructure.CrossCutting.Logging;
    using Resources;
    using Microsoft.EntityFrameworkCore;
    /// <summary>
    /// Repository base class
    /// </summary>
    /// <typeparam name="TEntity">The type of underlying entity in this repository</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        IQueryableUnitOfWork _UnitOfWork;

        /// <summary>
        /// Create a new instance of repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        public Repository(IQueryableUnitOfWork unitOfWork)
        {
            if (unitOfWork == (IUnitOfWork)null)
                throw new ArgumentNullException("unitOfWork cannot be null");

            _UnitOfWork = unitOfWork;
        }


        #region IREPOSITORY MEMBERS
        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get { return _UnitOfWork; }
        }

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <param name="item"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        public virtual void Add(TEntity item)
        {
            if (item != (TEntity)null)
            {
                object obj = GetSet();
                GetSet().Add(item); // add new item in this set
            }
            else
            {
                LoggerFactory.CreateLog()
                          .LogInfo(Messages.info_CannotAddNullEntity, typeof(TEntity).ToString());

            }
        }
        //public void Add<T>(T item) where T : Entity
        //{
        //    if (item != (T)null)
        //    {
        //        _UnitOfWork.Attach<T>(item);
        //    }
        //    else
        //    {
        //        LoggerFactory.CreateLog()
        //                  .LogInfo(Messages.info_CannotModifyNullEntity, typeof(TEntity).ToString());
        //    }
        //}

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <param name="item"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        public virtual void Remove(TEntity item)
        {
            if (item != (TEntity)null)
            {
                //attach item if not exist
                _UnitOfWork.Attach(item);

                //set as "removed"
                GetSet().Remove(item);
            }
            else
            {
                LoggerFactory.CreateLog()
                          .LogInfo(Messages.info_CannotRemoveNullEntity, typeof(TEntity).ToString());
            }
        }

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <param name="item"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        public virtual void Modify(TEntity item)
        {
            if (item != (TEntity)null)
                _UnitOfWork.SetModified(item);
            else
            {
                LoggerFactory.CreateLog()
                          .LogInfo(Messages.info_CannotModifyNullEntity, typeof(TEntity).ToString());
            }
        }
        public void SetModified<T>(T item) where T : Entity
        {
            if (item != (T)null)
                _UnitOfWork.SetModified<T>(item);
            else
            {
                LoggerFactory.CreateLog()
                          .LogInfo(Messages.info_CannotModifyNullEntity, typeof(TEntity).ToString());
            }
        }
        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <param name="item"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        public virtual void TrackItem(TEntity item)
        {
            if (item != (TEntity)null)
                _UnitOfWork.Attach<TEntity>(item);
            else
            {
                LoggerFactory.CreateLog()
                          .LogInfo(Messages.info_CannotTrackNullEntity, typeof(TEntity).ToString());
            }
        }

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <param name="persisted"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        /// <param name="current"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        public virtual void Merge(TEntity persisted, TEntity current)
        {
            _UnitOfWork.ApplyCurrentValues(persisted, current);
        }

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <param name="item"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        /// <returns><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></returns>
        public virtual TEntity Get(Guid id)
        {
            if (id != Guid.Empty)
                return GetSet().SingleOrDefault(d=>d.Id == id);
            else
                return null;
        }

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <returns><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return GetSet();
        }

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <typeparam name="S"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></typeparam>
        /// <param name="pageIndex"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        /// <param name="pageCount"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        /// <param name="orderByExpression"><see cref="CareSystem.Domain.Seedwork.Seedwork.IRepository{TValueObject}"/></param>
        /// <param name="ascending"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        /// <returns><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></returns>
        public virtual IEnumerable<TEntity> GetPaged<KProperty>(int pageIndex, int pageCount, System.Linq.Expressions.Expression<Func<TEntity, KProperty>> orderByExpression, bool ascending)
        {
            var set = GetSet();

            if (ascending)
            {
                return set.OrderBy(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
            else
            {
                return set.OrderByDescending(orderByExpression)
                          .Skip(pageCount * pageIndex)
                          .Take(pageCount);
            }
        }

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <param name="filter"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        /// <returns><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></returns>
        public virtual IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter)
        {
            return GetSet().Where(filter);
        }

        /// <summary>
        /// <see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/>
        /// </summary>
        /// <param name="specification"><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></param>
        /// <returns><see cref="CareSystem.Domain.Seedwork.IRepository{TValueObject}"/></returns>
        public virtual IEnumerable<TEntity> AllMatching(ISpecification<TEntity> specification)
        {
            return GetSet().Where(specification.SatisfiedBy());
        }
        #endregion

        #region IDISPOSABLE MEMBERS
        /// <summary>
        /// <see cref="M:System.IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            if (_UnitOfWork != null)
                _UnitOfWork.Dispose();
        }
        #endregion

        #region Private Methods

        DbSet<TEntity> GetSet()
        {
            return _UnitOfWork.CreateSet<TEntity>();
        }
        #endregion
    }
}