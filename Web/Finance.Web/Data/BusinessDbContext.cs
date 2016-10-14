using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Finance.Web.Models;
using System.Reflection;
using System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Finance.Web.Data
{
    public interface IBusinessRepository
    {
        #region BUSINESS
        IEnumerable<Business> GetBusinesses();
        IEnumerable<Business> GetBusinesses(string Service);
        Business GetBusinessById(Guid businessId);
        Business GetBusinessByName(string businessName);
        Business SaveBusiness(Business business);
        Business AddBusiness(Business Business);
        #endregion

        #region DEPARTMENT
        IEnumerable<Department> GetDepartments(Guid BusinessId);
        Department GetDepartment(Guid DepartmentId);
        Department AddDepartment(Department Department);
        Department SaveDepartment(Department Department);
        bool DeleteDepartment(Guid DepartmentId);
        #endregion

        #region SERVICES
        IEnumerable<Service> GetServices();
        Service GetService(string BusinessName, string Service);
        #endregion
    }
    public class BusinessRepository: IBusinessRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>

        BusinessDbContext BusinessContext;
        public BusinessRepository(BusinessDbContext businessContext) {this.BusinessContext = businessContext; }

        #region BUSINESS
        public IEnumerable<Business> GetBusinesses()
        {
            try
            {
                return this.BusinessContext.Businesses.Include(c => c.Departments).Include(c => c.Services);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Business> GetBusinesses(string Service)
        {
            try
            {
                return this.BusinessContext.Businesses.Include(c => c.Departments).Include(c => c.Services).Where(c => c.Services.Where(s=>s.Name.Equals(Service,StringComparison.OrdinalIgnoreCase)).Count() > 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Business GetBusinessById(Guid businessId)
        {
            try
            {
                return this.BusinessContext.Businesses.Include(c => c.Departments).Include(c => c.Services).Where(c => c.Id == businessId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Business GetBusinessByName(string businessName)
        {
            try
            {
                return this.BusinessContext.Businesses.Include(c => c.Departments).Include(c => c.Services).Where(c => c.Name == businessName).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Business SaveBusiness(Business business)
        {
            try
            {
                Business businessLocal = this.GetBusinessById(business.Id);
                if (businessLocal == null)
                {
                    business.Id = Guid.NewGuid();
                    businessLocal = this.BusinessContext.Businesses.Add(business).Entity;
                }
                else
                {
                    this.MergeBusiness(businessLocal, business);
                }
                this.BusinessContext.SaveChanges();
                return business;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        private void MergeBusiness(Business persisted, Business current)
        {
            if (persisted == null || current == null)
                return;
            this.BusinessContext.ApplyCurrentValues<Business>(persisted,current);
            persisted.Services.Clear();
            foreach(Service srv in current.Services)
            {
                persisted.Services.Add(srv);
            }
            if (persisted.Address != null && current.Address != null)
                this.BusinessContext.ApplyCurrentValues<Business>(persisted, current);
        }
        public Business AddBusiness(Business Business)
        {
            try
            {
                return this.BusinessContext.Businesses.Add(Business).Entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion

        #region DEPARTMENT
        public IEnumerable<Department> GetDepartments(Guid BusinessId)
        {
            try
            {
                return this.BusinessContext.Departments.Where(d => d.BusinessId == BusinessId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Department GetDepartment(Guid DepartmentId)
        {
            try
            {
                return this.BusinessContext.Departments.Where(d => d.Id == DepartmentId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Department AddDepartment(Department Department)
        {
            try
            {
                return this.BusinessContext.Departments.Add(Department).Entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Department SaveDepartment(Department Department)
        {
            try
            {
                Department DepartmentPersisted = this.GetDepartment(Department.Id);
                if(DepartmentPersisted == null)
                {
                    Department = this.BusinessContext.Departments.Add(Department).Entity;
                }
                this.BusinessContext.ApplyCurrentValues<Department>(DepartmentPersisted,Department);
                this.BusinessContext.SaveChanges();
                return Department;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public bool DeleteDepartment(Guid DepartmentId)
        {
            try
            {
                Department DepartmentPersisted = this.GetDepartment(DepartmentId);
                if (DepartmentPersisted != null)
                {
                    this.BusinessContext.Departments.Remove(DepartmentPersisted);
                    this.BusinessContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion

        #region SERVICES
        public IEnumerable<Service> GetServices()
        {
            try
            {
                return this.BusinessContext.Services;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public Service GetService(string BusinessName, string Service)
        {
            try
            {
                Business business = this.BusinessContext.Businesses.Include(c=>c.Services).FirstOrDefault(c=>c.Name.Equals(BusinessName, StringComparison.OrdinalIgnoreCase));
                if(business != null && business.Services != null && business.Services.Count >0)
                {
                    return business.Services.FirstOrDefault(s => s.Name.Equals(Service, StringComparison.OrdinalIgnoreCase));
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion
    }
    public class BusinessDbContext : DbContext, IQueryableUnitOfWork
    {
        public string ConnectionString { get; set; }
        public BusinessDbContext(DbContextOptions<BusinessDbContext> options) :base(options)
        {
            if (Database.EnsureCreated())
            {
                Database.Migrate();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (ConnectionString.Contains("FileName"))// SQLite
            //{
            //    optionsBuilder.UseSqlite(ConnectionString);
            //}
            //else
            //{
            //    optionsBuilder.UseSqlServer(ConnectionString);
            //}
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
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Service> Services { get; set; }
        #region IQUERYABLEUNITOFWORK MEMBERS
        public DbSet<TEntity> CreateSet<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
        //public void Attach<TEntity>(TEntity item) where TEntity : class
        //{
        //    //attach and set as unchanged
        //    base.Entry<TEntity>(item).State = EntityState.Unchanged;
        //}
        public void SetModified<TEntity>(TEntity item) where TEntity : class
        {
            //this operation also attach item in object state manager
            base.Entry<TEntity>(item).State = EntityState.Modified;
        }
        public void ApplyCurrentValues<TEntity>(TEntity original, TEntity current) where TEntity : class
        {
            foreach (IProperty property in base.Entry(original).Metadata.GetProperties())
            {
                PropertyEntry originalProperty = base.Entry<TEntity>(original).Property(property.Name);
                if (!property.IsKey())
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
            catch(Exception ex) //(DbEntityValidationException e)
            {

                //foreach (var eve in e.EntityValidationErrors)
                //{
                //    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //            ve.PropertyName, ve.ErrorMessage);
                //    }
                //}
                throw;
            }
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
                                  //entry.OriginalValues.SetValues(entry.GetDatabaseValues());
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
            catch //(DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                //var errorMessages = ex.EntityValidationErrors
                //        .SelectMany(x => x.ValidationErrors)
                //        .Select(x => x.ErrorMessage);
                //// Join the list to a single string.
                //var fullErrorMessage = string.Join("; ", errorMessages);
                //// Combine the original exception message with the new one.
                //var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                //// Throw a new DbEntityValidationException with the improved exception message.
                //throw new Exception(exceptionMessage);
                throw new Exception("Error occured on saving data........");
            }
        }
        public bool IsTransactionInUse { get; set; }
        #endregion
    }
    class BusinessMap : EntityMappingConfiguration<Business>
    {
        public override void Map(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Business> biz)
        {
            biz.HasKey(c => c.Id);
            biz.HasMany(c => c.Departments).WithOne(b => b.Business).HasForeignKey(d => d.BusinessId);
            biz.ToTable("Business");
        }
    }
    class DepartmentMap : EntityMappingConfiguration<Department>
    {
        public override void Map(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Department> dep)
        {
            dep.HasKey(d => d.Id);
            dep.ToTable("Department");
        }
    }
}