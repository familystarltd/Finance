using System;
using System.Linq;

using System.Collections.Generic;
using System.Infrastructure.Data;
using FinanceManagement.Infrastructure.Data.UnitOfWork;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
namespace FinanceManagement.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Customer Repository implementation.
    /// <see cref="FinanceManagement.Domain.IInvoiceRepository"/>
    /// </summary>
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public InvoiceRepository(FinanceManagementContext unitOfWork) : base(unitOfWork) { }
        public override void Merge(Invoice persisted, Invoice current)
        {
            if (current == null || persisted == null)
                return;
            // Remove Invoice Details
            var deletedInvoiceDetails = persisted.InvoiceDetails.Where(xrefDb => !current.InvoiceDetails.Any(inv => xrefDb.Id == inv.Id)).ToArray();
            foreach (var invD in deletedInvoiceDetails)
            {
                persisted.InvoiceDetails.Remove(invD);
                var uow = this.UnitOfWork as FinanceManagementContext;
                uow.Set<InvoiceDetail>().Remove(invD);
            }
            // Edit Invoice Details
            foreach (var editInvoiceDetails in persisted.InvoiceDetails)
            {
                var invoiceDetail = current.InvoiceDetails.FirstOrDefault(invD => invD.Id == editInvoiceDetails.Id);
                if (invoiceDetail != null)
                {
                    editInvoiceDetails.Article = invoiceDetail.Article;
                    editInvoiceDetails.Total = invoiceDetail.Total;
                    editInvoiceDetails.VAT = invoiceDetail.VAT;
                }
            }
            // Add New Invoice Details
            var newInvoiceDetails = current.InvoiceDetails.Where(invD => invD.Id == Guid.Empty).ToArray();
            foreach (var newInvoiceDetail in newInvoiceDetails)
            {
                persisted.InvoiceDetails.Add(new InvoiceDetail
                {
                    Id = Guid.NewGuid(),
                    Article = newInvoiceDetail.Article,
                    InvoiceId = persisted.Id,
                    Total = newInvoiceDetail.Total,
                    VAT = newInvoiceDetail.VAT
                });
            }
            base.Merge(persisted, current);
        }
        public IEnumerable<Invoice> GetInvoices(bool IsPaid, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                if (IsPaid)
                {
                    TotalRowCount = uow.FinancialTransactions.OfType<Invoice>().Count(inv =>
                            ((FromDate.HasValue && ToDate.HasValue) && inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) <= 0 && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                            ||
                            (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) <= 0));

                    return uow.FinancialTransactions.OfType<Invoice>()
                            .Where(inv =>
                            ((FromDate.HasValue && ToDate.HasValue) && inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) <= 0 && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                            ||
                            (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) <= 0))
                                     .Skip(PageIndex * PageSize)
                                     .Take(PageSize);
                }
                else
                {
                    TotalRowCount = uow.FinancialTransactions.OfType<Invoice>().Count(inv =>
                            ((FromDate.HasValue && ToDate.HasValue) && inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) > 0 && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                            ||
                            (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) > 0));
                    IEnumerable<Invoice> invoices = uow.FinancialTransactions.OfType<Invoice>().Where(inv =>
                                ((FromDate.HasValue && ToDate.HasValue) && inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) > 0 && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                                ||
                                (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) > 0))
                                .OrderBy(inv => inv.InvoiceNo)
                                .Skip(PageIndex * PageSize)
                                .Take(PageSize);

                    return invoices;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Invoice> GetInvoices(InvoiceStatus invoiceStatus, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.FinancialTransactions.OfType<Invoice>().Count((inv =>
                                 ((FromDate.HasValue && ToDate.HasValue) && inv.InvoiceStatus == invoiceStatus && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                                 ||
                                 (inv.InvoiceStatus == invoiceStatus)));

                return uow.FinancialTransactions.OfType<Invoice>()
                        .Where((inv =>
                                 ((FromDate.HasValue && ToDate.HasValue) && inv.InvoiceStatus == invoiceStatus && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                                 ||
                                 (inv.InvoiceStatus == invoiceStatus))).OrderBy(inv => inv.InvoiceNo)
                                 .Skip(pageIndex * pageSize)
                                 .Take(pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Invoice> GetInvoicesByFee(Guid FeeId, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageCount)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                return uow.FinancialTransactions.OfType<FeeInvoice>()
                    .Where(inv => inv.Fee.Id == FeeId
                            && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                            .OrderBy(inv => inv.InvoiceNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Invoice> GetInvoices(Guid FunderId, DateTime ToDate)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                return uow.FinancialTransactions.OfType<Invoice>()
                    .Where(inv => inv.FunderId == FunderId
                            && (DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate)))
                            .OrderBy(inv => inv.InvoiceNo);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Invoice> GetInvoicesForProcessing(Guid FunderId, DateTime ProcessedDate)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                return uow.FinancialTransactions.OfType<Invoice>()
                    .Where(inv => inv.FunderId == FunderId && (inv.TransactionStatus != TransactionStatus.Void || inv.TransactionStatus == TransactionStatus.Processed)
                            && (DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ProcessedDate)))
                            .OrderBy(inv => inv.InvoiceNo);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

        }
        public void GetNoOfInvoicesByStatus(out int New, out int Approved, out int Paid, out int Deleted, out int Cancelled, out int UnApproved, out int UnPaid)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                New = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.InvoiceStatus == InvoiceStatus.Draft);
                Deleted = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.InvoiceStatus == InvoiceStatus.Void);
                Cancelled = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.InvoiceStatus == InvoiceStatus.Cancel);
                Approved = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.InvoiceStatus == InvoiceStatus.Approved);
                //Paid = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.InvoiceStatus == InvoiceStatus.Paid);
                Paid = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.Amount - ((decimal?)inv.ReceiptInvoices.Sum(r => r.AmountReceived) ?? 0) <= 0);
                UnApproved = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.InvoiceStatus == InvoiceStatus.Draft);
                //UnPaid = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.InvoiceStatus == InvoiceStatus.New || inv.InvoiceStatus == InvoiceStatus.Approved);
                UnPaid = uow.FinancialTransactions.OfType<Invoice>().Count(inv => inv.Amount - ((decimal?)inv.ReceiptInvoices.Sum(r => r.AmountReceived) ?? 0) > 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #region ARTICLE TEMPLATE
        public IEnumerable<InvoiceArticleTemplate> GetInvoiceArticleTemplates(int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.InvoiceArticleTemplates.Count();
                return uow.InvoiceArticleTemplates.OrderBy(t => t.Name)
                                 .Skip(pageIndex * pageSize)
                                 .Take(pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<InvoiceArticleTemplate> GetInvoiceArticleTemplates(string TemplateName, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.InvoiceArticleTemplates.Count(t => t.Name.ToLower().Contains(TemplateName.ToLower()) || string.IsNullOrEmpty(TemplateName.Trim()));
                return uow.InvoiceArticleTemplates
                    .Where(t => t.Name.ToLower().Contains(TemplateName.ToLower()) || string.IsNullOrEmpty(TemplateName.Trim()))
                    .OrderBy(t => t.Name)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public InvoiceArticleTemplate GetInvoiceArticleTemplate(Guid TemplateId)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                return uow.InvoiceArticleTemplates.FirstOrDefault(t => t.Id == TemplateId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public InvoiceArticleTemplate UpdateInvoiceArticleTemplate(InvoiceArticleTemplate Template)
        {
            try
            {
                InvoiceArticleTemplate invoiceArticleTemplate = this.GetInvoiceArticleTemplate(Template.Id);
                if (invoiceArticleTemplate == null)
                {
                    invoiceArticleTemplate = new InvoiceArticleTemplate();
                    invoiceArticleTemplate.Id = Guid.NewGuid();
                    var uow = this.UnitOfWork as FinanceManagementContext;
                    uow.InvoiceArticleTemplates.Add(invoiceArticleTemplate);
                }
                invoiceArticleTemplate.Name = Template.Name;
                invoiceArticleTemplate.ArticleTemplate = Template.ArticleTemplate;
                this.UnitOfWork.SaveChanges();
                return this.GetInvoiceArticleTemplate(invoiceArticleTemplate.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public bool DeleteInvoiceArticleTemplate(Guid TemplateId)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                uow.InvoiceArticleTemplates.Remove(this.GetInvoiceArticleTemplate(TemplateId));
                this.UnitOfWork.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion
    }
}