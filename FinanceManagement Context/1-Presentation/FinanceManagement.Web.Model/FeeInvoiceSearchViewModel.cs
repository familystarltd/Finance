using System;
using System.Collections.Generic;

namespace FinanceManagement.Web.Model
{
    public class PaginationHeader
    {
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public string PageLink { get; set; }
        public string PrevPageLink { get; set; }
        public string NextPageLink { get; set; }
    }
    public class InvoiceViewModel
    {
        public string CompanyName { get; set; }
        public  InvoiceTypeModel InvoiceType { get; set; }
        public PaginationHeader Pagination { get; set; }
        public InvoiceModel Invoice { get; set; }
        public IEnumerable<InvoiceModel> Invoices { get; set; }
        public string FunderName { get; set; }
        public string CustomerName { get; set; }
        public int CustomerNo { get; set; }
        public int InvoiceNoSearch { get; set; }
        public DateTime? InvoiceProcessedFromDate { get; set; }
        public DateTime? InvoiceProcessedToDate { get; set; }
        public DateTime? InvoiceProcessedMaxDate { get; set; }
        public int NoOfNewInvoices { get; set; }
        public int NoOfApprovedInvoices { get; set; }
        public int NoOfPaidInvoices { get; set; }
        public int NoOfCancelledInvoices { get; set; }
        public int NoOfDeletedInvoices { get; set; }
        public int NoOfUnApprovedInvoices { get; set; }
        public int NoOfUnPaidInvoices{ get; set; }
        public bool IsPdfPrint { get; set; }

    }
}