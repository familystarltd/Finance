namespace FinanceManagement.Domain.Aggregates.CustomerAgg
{
    using System;
    using System.Domain;
    public class DisbursementsPayment : Entity
    {
        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }
    }
}