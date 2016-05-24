namespace System.Domain
{
    using System;

    /// <summary>
    /// Contract for Unit of Work Pattern
    /// In this solution, the Unit Of Work is implemented using the out-of-box Entity Framework Context (EF 4.1 DbContext) Persistent Engine
    /// But in order to comply the PI (Persistence Ignorant) principle in Domain Model, have to implement this interface (Contract).
    /// This interface (Contract) should be complied by and UoW implementation to be used with this Domain.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        /// <summary>
        /// Commit all changes made in a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,  
        /// then an exception is thrown
        ///</remarks>
        void Commit();

        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,
        /// then 'client changes' are refreshed - Client wins
        ///</remarks>
        void CommitAndRefreshChanges();
        
        /// <summary>
        /// Rollback tracked changes. See References of UnitOfWork pattern
        /// </summary>
        void RollbackChanges();

        void BeginTransaction();

        void CommitExecuteCommandTransaction();

        void RollBackTransaction();

        bool IsTransactionInUse { get; set; }

    }
}
