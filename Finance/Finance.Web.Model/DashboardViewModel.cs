using System;
using System.Collections.Generic;

namespace Finance.Web.Model
{
    public enum Month : int
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
    public struct InvoiceStatusCount
    {
        public int NoOfNewInvoices { get; set; }
    }
    public class DashboardViewModel
    {
        public string CompanyName { get; set; }
        public CompanyModel Company { get; set; }
        public string InvoiceNoSearch { get; set; }
        public bool IsBankHolidayFileExists { get; set; }
        public DateTime? InvoiceProcessedDate { get; set; }
        public DateTime? InvoiceProcessedMaxDate { get; set; }
        public IEnumerable<CompanyModel> Companies { get; set; }
        public IEnumerable<CustomerModel> Customers { get; set; }
        public PaginationHeader CustomerPagination { get; set; }
        public IEnumerable<CustomerModel> CustomersWithoutFees { get; set; }
        public int NoOfNewInvoices { get; set; }
        public int NoOfApprovedInvoices { get; set; }
        public int NoOfPaidInvoices { get; set; }
        public int NoOfCancelledInvoices { get; set; }
        public int NoOfDeletedInvoices { get; set; }
        public int NoOfUnApprovedInvoices { get; set; }
        public int NoOfUnPaidInvoices{ get; set; }


    }
}
