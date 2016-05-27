//using System.Data.Entity.ModelConfiguration;
//using Finance.Domain.Aggregates.CustomerAgg;
//using Finance.Domain.Aggregates.FeeAgg;

//namespace Finance.Infrastructure.Data.UnitOfWork.Mapping
//{
//    /// <summary>
//    /// The CustomerPay entity type configuration
//    /// </summary>
//    class CustomerPayMap : EntityTypeConfiguration<CustomerPay>
//    {
//        public CustomerPayMap()
//        {
//            this.HasKey(f => f.Id).Property(f => f.Id);
//            this.Property(f => f.FunderId).IsRequired();
//            //this.HasRequired(f => f.Customer).WithMany(c => c.Fees).HasForeignKey(f => f.CustomerPay.CustomerId).WillCascadeOnDelete(false);
//            this.HasRequired(f => f.Funder).WithMany(funder => funder.CustomerPays).HasForeignKey(f => f.FunderId).WillCascadeOnDelete(false);
//            //this.HasOptional(cf => cf.Fee).WithMany();//.HasForeignKey(f=>f.FeeId).WillCascadeOnDelete(false);//.WithMany().HasForeignKey(c=>c.FeeId).WillCascadeOnDelete(false);//.WithOptionalDependent(c => c.CustomerPay).Map(p => p.MapKey("Id"));
//            this.ToTable("CustomerPay");
//        }
//    }
//}
