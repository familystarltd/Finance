namespace FinanceManagement.Domain.Aggregates.DisbursementAgg
{
    using System.Domain;

    /// <summary>
    /// Expenses
    /// </summary>
    public class Expense : Entity
    {
        /// <summary>
        /// Get or set the Given name of this Expense
        /// </summary>
        public string Name { get; set; }
    }
}