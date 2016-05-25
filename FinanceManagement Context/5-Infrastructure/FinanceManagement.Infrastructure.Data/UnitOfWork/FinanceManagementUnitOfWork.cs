namespace FinanceManagement.Infrastructure.Data.UnitOfWork
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Infrastructure.Data;
    using FinanceManagement.Infrastructure.Data.UnitOfWork.Mapping;
    using FinanceManagement.Domain.Aggregates.CustomerAgg;
    using FinanceManagement.Domain.Aggregates.FeeAgg;
    using FinanceManagement.Domain.Aggregates.DisbursementAgg;
    using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using System.Reflection;
    public class FinanceManagementDbContext : DbContext, IQueryableUnitOfWork
    {
        public string ConnectionString { get; set; }
        public FinanceManagementDbContext(DbContextOptions<FinanceManagementDbContext> options) : base(options)
        {
            
        }

        //public FinanceManagementContext(string connection) : base(connection)
        //{
        //    //The Lazy Loading property enables loading the sub-objects of model up front
        //    this.Configuration.ProxyCreationEnabled = false;

        //    /*The Proxy Creation property is used in conjugation with Lazy Loading property, 
        //     * so if is set to false the “ResidentManagerUnitOfWork” won’t load sub-objects unless Include method is called. * */
        //    this.Configuration.LazyLoadingEnabled = false;
        //    /*
        //     * Configured the initialization and migration strategy of the database to migrate to latest version 
        //     * if a model has changed (i.e. new property has been added). 
        //     * To implement this need to add new class called “FinanceManagementContextDBConfiguration” 
        //     * which derives from class “System.Data.Entity.Migrations.DbMigrationsConfiguration<TContext>”.

        //     * useSuppliedContext:If set to true the initializer is run using the connection information from the context that triggered initialization.
        //     * Otherwise, the connection information will be taken from a context constructed using the default constructor or registered factory if applicable.
        //     * */
        //    Database.SetInitializer(new MigrateDatabaseToLatestVersion<FinanceManagementContext, FinanceManagementContextDBConfiguration>(useSuppliedContext:true));
        //}
        #region DbContext Overrides
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddEntityConfigurationsFromAssembly(GetType().GetTypeInfo().Assembly);
        }
        #endregion

        #region IDBSET MEMBERS
        //DbSet properties, this means that every POCO class is transferred to a database table.
        public DbSet<Company> Companies { get; set; }
        public DbSet<AppLog> AppLogs { get; set; }
        public DbSet<AccountGroup> AccountGroups { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Funder> Funders { get; set; }
        public DbSet<CustomerDisbursementFunder> CustomerDisbursementFunders { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<FeeRate> FeeRates { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Disbursement> Disbursements { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<InvoiceArticleTemplate> InvoiceArticleTemplates { get; set; }
        #endregion

        #region IQUERYABLEUNITOFWORK MEMBERS
        public DbSet<TEntity> CreateSet<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
        public void Attach<TEntity>(TEntity item) where TEntity : class
        {
            //attach and set as unchanged
            base.Entry<TEntity>(item).State = EntityState.Unchanged;
        }
        public void SetModified<TEntity>(TEntity item) where TEntity : class
        {
            //this operation also attach item in object state manager
            base.Entry<TEntity>(item).State = EntityState.Modified;
        }
        public void ApplyCurrentValues<TEntity>(TEntity original, TEntity current) where TEntity : class
        {
            //if it is not attached, attach original and set current values
            this.Update<TEntity>(current);
            //base.Entry<TEntity>(original).CurrentValues.SetValues(current);
        }
        public void Commit()
        {
            try
            {
                base.SaveChanges();
                if (transaction != null && !IsTransactionInUse)
                {
                    transaction.Commit();
                    transaction.Dispose();
                }
            }
            catch { }
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
        }
        public void CommitExecuteCommandTransaction()
        {
            if (transaction != null && !IsTransactionInUse)
            {
                transaction.Commit();
                transaction.Dispose();
            }
        }
        public void CommitAndRefreshChanges()
        {
            bool saveFailed = false;

            do
            {
                try
                {
                    base.SaveChanges();
                    saveFailed = false;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    ex.Entries.ToList()
                              .ForEach(entry =>
                              {
                                  entry.State = EntityState.Unchanged;
                              });

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
            } while (saveFailed);
        }
        public void RollbackChanges()
        {
            // set all entities in change tracker 
            // as 'unchanged state'
            base.ChangeTracker.Entries()
                              .ToList()
                              .ForEach(entry => entry.State = EntityState.Unchanged);
        }
        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string sqlQuery, params object[] parameters)
        {
            throw new NotImplementedException();
            //return base.Database.ExecuteSqlCommand.SqlQuery<TEntity>(sqlQuery, parameters);
        }
        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return base.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }
        IDbContextTransaction transaction = null;
        public void BeginTransaction()
        {
            if (!IsTransactionInUse)
                transaction = base.Database.BeginTransaction();
        }
        public void RollBackTransaction()
        {
            if (transaction != null && !IsTransactionInUse)
            {
                transaction.Rollback();
                transaction.Dispose();
            }
        }
        public new void SaveChanges()
        {
            try
            {
                base.SaveChanges();
            }
            catch { }
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
        }
        public bool IsTransactionInUse { get; set; }
        #endregion
    }
}