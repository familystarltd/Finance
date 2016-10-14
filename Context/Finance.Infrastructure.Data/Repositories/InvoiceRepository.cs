using System;
using System.Linq;

using System.Collections.Generic;
using System.Infrastructure.Data;
using Finance.Infrastructure.Data.UnitOfWork;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Customer Repository implementation.
    /// <see cref="Finance.Domain.IInvoiceRepository"/>
    /// </summary>
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public InvoiceRepository(IFinanceDbContext unitOfWork) : base(unitOfWork) { }
        public override void Merge(Invoice persisted, Invoice current)
        {
            if (current == null || persisted == null)
                return;
            // Remove Invoice Details
            var deletedInvoiceDetails = persisted.InvoiceDetails.Where(xrefDb => !current.InvoiceDetails.Any(inv => xrefDb.Id == inv.Id)).ToArray();
            foreach (var invD in deletedInvoiceDetails)
            {
                persisted.InvoiceDetails.Remove(invD);
                var uow = this.UnitOfWork as IFinanceDbContext;
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
        public Invoice GetInvoice(int InvoiceNo)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.Invoices
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .Include(inv => inv.ReceiptInvoices).ThenInclude(r => r.Receipt)
                            .Include(inv => inv.InvoiceDetails)
                    .SingleOrDefault(inv => inv.InvoiceNo == InvoiceNo);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Invoice> GetInvoices(bool IsPaid, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                IEnumerable<Invoice> invoices = new List<Invoice>();
                var uow = this.UnitOfWork as IFinanceDbContext;
                if (IsPaid)
                {
                    if (FromDate.HasValue && ToDate.HasValue)
                    {
                        TotalRowCount = uow.Invoices.Count(inv =>
                            (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) <= 0) && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)));
                        invoices = uow.Invoices
                            .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .Include(inv=>inv.InvoiceDetails)
                            .Include(inv=>inv.ReceiptInvoices).ThenInclude(ri=>ri.Receipt)
                            .Include(inv => inv.CreditNotes)
                            .Where(inv =>
                                (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) <= 0) && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                                         .Skip(PageIndex * PageSize)
                                         .Take(PageSize);
                    }
                    else
                    {
                        TotalRowCount = uow.Invoices.Count(inv => (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0)) <= 0);
                        invoices = uow.Invoices
                            .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .Include(inv => inv.InvoiceDetails)
                            .Include(inv => inv.ReceiptInvoices).ThenInclude(ri => ri.Receipt)
                            .Include(inv => inv.CreditNotes)
                            .Where(inv => (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0)) <= 0)
                                         .Skip(PageIndex * PageSize)
                                         .Take(PageSize);
                    }
                }
                else
                {
                    if (FromDate.HasValue && ToDate.HasValue)
                    {
                        TotalRowCount = uow.Invoices.Count(inv =>
                            (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) > 0) && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)));
                        invoices = uow.Invoices
                            .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .Include(inv => inv.InvoiceDetails)
                            .Include(inv => inv.ReceiptInvoices).ThenInclude(ri => ri.Receipt)
                            .Include(inv => inv.CreditNotes)
                            .Where(inv =>
                                    (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) > 0) && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                                    .OrderBy(inv => inv.InvoiceNo)
                                    .Skip(PageIndex * PageSize)
                                    .Take(PageSize);
                    }
                    else
                    {
                        TotalRowCount = uow.Invoices.Count(inv => (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0)) > 0);
                        invoices = uow.Invoices
                            .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .Include(inv => inv.InvoiceDetails)
                            .Include(inv => inv.ReceiptInvoices).ThenInclude(ri => ri.Receipt)
                            .Include(inv => inv.CreditNotes)
                            .Where(inv => (inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0)) > 0)
                                    .OrderBy(inv => inv.InvoiceNo)
                                    .Skip(PageIndex * PageSize)
                                    .Take(PageSize);
                    }
                }
                return invoices;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Invoice> GetInvoices(string PayerName, string CustomerName, int CustomerNo, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                IEnumerable<Invoice> invoices = new List<Invoice>();
                PayerName = string.IsNullOrEmpty(PayerName) ? "" : PayerName;
                CustomerName = string.IsNullOrEmpty(CustomerName) ? "" : CustomerName;
                TotalRowCount = uow.Invoices.Count(tr => ((!string.IsNullOrEmpty(PayerName) && tr.Payer.Name.Equals(PayerName.Trim(), StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(PayerName))
                            &&
                            ((!string.IsNullOrEmpty(CustomerName) && tr.Customer.PersonalInfo.FullName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(CustomerName))
                            &&
                            ((CustomerNo > 0 && tr.Customer.Ref == CustomerNo.ToString()) || CustomerNo == 0)
                            &&
                            (
                                (FromDate.HasValue && ToDate.HasValue
                                &&
                                DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value)
                                &&
                                DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                                ||
                                (!FromDate.HasValue || !ToDate.HasValue)
                            )
                    );
                invoices = uow.Invoices
                            .Include(inv => inv.ReceiptInvoices).ThenInclude(r=>r.Receipt)
                            .Include(inv=>inv.CreditNotes)
                            .Include(inv=>inv.InvoiceDetails)
                            .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .Include(inv => inv.InvoiceDetails)
                            .Where(tr => ((!string.IsNullOrEmpty(PayerName) && tr.Payer.Name.Equals(PayerName.Trim(), StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(PayerName))
                                &&
                                ((!string.IsNullOrEmpty(CustomerName) && tr.Customer.PersonalInfo.FullName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(CustomerName))
                                &&
                                ((CustomerNo > 0 && tr.Customer.Ref == CustomerNo.ToString()) || CustomerNo == 0)
                                &&
                                (
                                    (FromDate.HasValue && ToDate.HasValue
                                    &&
                                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value)
                                    &&
                                    DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                                    ||
                                    (!FromDate.HasValue && !ToDate.HasValue)
                                )
                            )
                    .OrderByDescending(tr => tr.TransactionRef)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);

                return invoices;
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
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.FinancialTransactions.OfType<Invoice>().Count((inv =>
                                 ((FromDate.HasValue && ToDate.HasValue) && inv.InvoiceStatus == invoiceStatus && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                                 ||
                                 (inv.InvoiceStatus == invoiceStatus)));

                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .OfType<Invoice>()
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
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .OfType<FeeInvoice>()
                    .Where(inv => inv.Fee.Id == FeeId
                            && (DbFunctions.TruncateTime(inv.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)))
                            .OrderBy(inv => inv.InvoiceNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Invoice> GetInvoices(Guid PayerId, DateTime ToDate)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<Invoice>()
                    .Where(inv => inv.PayerId == PayerId
                            && (DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ToDate)))
                            .OrderBy(inv => inv.InvoiceNo);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Invoice> GetInvoicesForProcessing(Guid PayerId, DateTime ProcessedDate)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.Invoices
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                            .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                            .Include(inv=>inv.ReceiptInvoices).ThenInclude(r=>r.Receipt)
                            .Include(inv=>inv.InvoiceDetails)
                    .Where(inv => inv.PayerId == PayerId && (inv.TransactionStatus != TransactionStatus.Void || inv.TransactionStatus == TransactionStatus.Processed)
                            && (DbFunctions.TruncateTime(inv.ProcessedDate) <= DbFunctions.TruncateTime(ProcessedDate)))
                            .OrderBy(inv => inv.InvoiceNo);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        public IEnumerable<InvoiceDetail> GetFeeInvoiceDetais(Guid InvoiceId)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.FeeInvoiceDetails.Include(inv=>inv.Invoice).ThenInclude(inv=>inv.ReceiptInvoices).ThenInclude(r=>r.Receipt)
                    .Include(inv=>inv.Invoice).ThenInclude(inv=>inv.CreditNotes)
                    .Include(i=> i.Rate).ThenInclude(r=>r.FeeRate)
                    .Where(inv => inv.InvoiceId == InvoiceId);
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
                var uow = this.UnitOfWork as IFinanceDbContext;
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
                var uow = this.UnitOfWork as IFinanceDbContext;
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
                TemplateName = string.IsNullOrEmpty(TemplateName) ? "" : TemplateName;
                var uow = this.UnitOfWork as IFinanceDbContext;
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
                var uow = this.UnitOfWork as IFinanceDbContext;
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
                    var uow = this.UnitOfWork as IFinanceDbContext;
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
                var uow = this.UnitOfWork as IFinanceDbContext;
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