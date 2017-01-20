namespace Finance.Domain.Aggregates.FinancialTransactionAgg
{
    using Finance.Domain.Aggregates.FeeAgg;
    using System;
    using System.ComponentModel;
    using System.Domain;

    /// <summary>
    /// Fee Invoice details
    /// </summary>
    public class FeeInvoiceDetail : InvoiceDetail
    {
        public bool IsApproved { get; set; }
        public Guid RateId { get; set; }
        public virtual Rate Rate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int NoOfHours
        {
            get
            {
                if (Rate != null && Rate is HourlyRate)
                    return (Rate as HourlyRate).NoOfHours;
                return 0;
            }
        }
        public int NoOfDays { get; set; }
    }
}