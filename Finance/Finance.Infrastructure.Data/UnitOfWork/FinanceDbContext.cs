namespace Finance.Infrastructure.Data.UnitOfWork
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Finance.Domain.Aggregates.CustomerAgg;
    using Finance.Domain.Aggregates.FeeAgg;
    using Finance.Domain.Aggregates.DisbursementAgg;
    using Finance.Domain.Aggregates.FinancialTransactionAgg;
    using Domain;

    public class ConnectionStringDatabase
    {
        public string ConnectionString { get; set; }
    }
    public interface IFinanceDbContext: IQueryableUnitOfWork
    {
        #region IDBSET MEMBERS
        //DbSet properties, this means that every POCO class is transferred to a database table.
        DbSet<Company> Companies { get; set; }
        DbSet<AppLog> AppLogs { get; set; }
        DbSet<AccountGroup> AccountGroups { get; set; }
        DbSet<Account> Accounts { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<Funder> Funders { get; set; }
        DbSet<CustomerDisbursementFunder> CustomerDisbursementFunders { get; set; }
        DbSet<Fee> Fees { get; set; }
        DbSet<FeeRate> FeeRates { get; set; }
        DbSet<Rate> Rates { get; set; }
        DbSet<Expense> Expenses { get; set; }
        DbSet<Disbursement> Disbursements { get; set; }
        DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        DbSet<InvoiceArticleTemplate> InvoiceArticleTemplates { get; set; }
        #endregion
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
    public class FinanceDbContext : DbContext, IFinanceDbContext
    {
        public string ConnectionString { get; set; }
        public FinanceDbContext(ConnectionStringDatabase ConnectionStringDatabase)
        {
            ConnectionString = ConnectionStringDatabase.ConnectionString;
            if (Database.EnsureCreated())
            {
                Database.Migrate();
            }
        }

        #region DBCONTEXT OVERRIDES
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            #region NOT SUPPORTED YET BY ENTITY FRAMEWORK CORE (26/05/2016) --- HAVE TO BE CHANGED
            //    //The Lazy Loading property enables loading the sub-objects of model up front
            //    this.Configuration.ProxyCreationEnabled = false;

            //    /*The Proxy Creation property is used in conjugation with Lazy Loading property, 
            //     * so if is set to false the “ResidentManagerUnitOfWork” won’t load sub-objects unless Include method is called. * */
            //    this.Configuration.LazyLoadingEnabled = false;
            //    /*
            //     * Configured the initialization and migration strategy of the database to migrate to latest version 
            //     * if a model has changed (i.e. new property has been added). 
            //     * To implement this need to add new class called “FinanceContextDBConfiguration” 
            //     * which derives from class “System.Data.Entity.Migrations.DbMigrationsConfiguration<TContext>”.

            //     * useSuppliedContext:If set to true the initializer is run using the connection information from the context that triggered initialization.
            //     * Otherwise, the connection information will be taken from a context constructed using the default constructor or registered factory if applicable.
            //     * */
            //    Database.SetInitializer(new MigrateDatabaseToLatestVersion<FinanceContext, FinanceContextDBConfiguration>(useSuppliedContext:true));
            #endregion

            if (ConnectionString.Contains("FileName"))// SQLite
            {
                optionsBuilder.UseSqlite(ConnectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
             * Type per Hierachy(TPH) --- Currently supports TPH, but Have to change to TPT - 26/05/2016
             * Type per Type(TPT) | N/A --- Need to be impelemented when is compeleted by Microsoft Entityframework Team
             * Type per Concrete Class (TPC) --
             */
            modelBuilder.AddEntityConfigurationsFromAssembly(GetType().GetTypeInfo().Assembly);
        }
        #endregion

        #region IQUERYABLEUNITOFWORK MEMBERS
        public DbSet<TEntity> CreateSet<TEntity>() where TEntity : class => base.Set<TEntity>();
        public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity) => base.Attach<TEntity>(entity); 
        public void SetModified<TEntity>(TEntity item) where TEntity : class => base.Entry<TEntity>(item).State = EntityState.Modified;
        public void ApplyCurrentValues<TEntity>(TEntity original, TEntity current) where TEntity : class
        {
            //if it is not attached, attach original and set current values
            foreach(IProperty property in base.Entry(original).Metadata.GetProperties())
            {
                PropertyEntry originalProperty = base.Entry<TEntity>(original).Property(property.Name);
                if(!property.IsKey())
                {
                    originalProperty.IsModified = true;
                    originalProperty.CurrentValue = base.Entry<TEntity>(current).Property(property.Name).CurrentValue;
                }
            }
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
            //catch (DbEntityValidationException ex)
            //{
            //    // Retrieve the error messages as a list of strings.
            //    var errorMessages = ex.EntityValidationErrors
            //            .SelectMany(x => x.ValidationErrors)
            //            .Select(x => x.ErrorMessage);
            //    // Join the list to a single string.
            //    var fullErrorMessage = string.Join("; ", errorMessages);
            //    // Combine the original exception message with the new one.
            //    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
            //    // Throw a new DbEntityValidationException with the improved exception message.
            //    throw new Exception(exceptionMessage);
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
            //return base.Database.SqlQuery<TEntity>(sqlQuery, parameters);
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
        public DbSet<Company> Companies
        {
            get; set;
        }
        public DbSet<AppLog> AppLogs
        {
            get; set;
        }
        public DbSet<AccountGroup> AccountGroups
        {
            get; set;
        }
        public DbSet<Account> Accounts
        {
            get; set;
        }
        public DbSet<Customer> Customers
        {
            get; set;
        }
        public DbSet<Funder> Funders
        {
            get; set;
        }
        public DbSet<CustomerDisbursementFunder> CustomerDisbursementFunders
        {
            get; set;
        }
        public DbSet<Fee> Fees
        {
            get; set;
        }
        public DbSet<FeeRate> FeeRates { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Disbursement> Disbursements { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<InvoiceArticleTemplate> InvoiceArticleTemplates { get; set; }
        #endregion
    }

}