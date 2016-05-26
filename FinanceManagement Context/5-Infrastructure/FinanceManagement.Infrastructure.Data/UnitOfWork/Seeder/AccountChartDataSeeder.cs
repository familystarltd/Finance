using FinanceManagement.Domain.Aggregates.DisbursementAgg;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
using System;
using System.Linq;

namespace FinanceManagement.Infrastructure.Data.UnitOfWork
{
    class AccountChartDataSeeder
    {
        IFinanceDbContext context;
        public AccountChartDataSeeder(IFinanceDbContext context)
        {
            this.context = context;
        }
        public void Seed()
        {
            if (this.context.AccountGroups.Count() > 0 && this.context.Accounts.Count() > 0)
                return;
            try
            {
                
                this.context.AccountGroups.AddRange(
                    // ASSETS -> Balance Sheet
                    new AccountGroup() { Id = new Guid("10000000-0000-0000-0000-000000000000"), Name = "Assets", AccountType = FinancialStatement.BalanceSheet, DoubleEntry = DoubleEntry.Debit, IsPrimary = true, IsSystem = true },
                    new AccountGroup() { Id = new Guid("10000001-0000-0000-0000-000000000000"), Name = "Fixed Assets", AccountType = FinancialStatement.BalanceSheet, DoubleEntry = DoubleEntry.Debit, AccountGroupParentId = new Guid("10000000-0000-0000-0000-000000000000"), IsSystem = true },
                    new AccountGroup() { Id = new Guid("10000002-0000-0000-0000-000000000000"), Name = "Current Assets", AccountType = FinancialStatement.BalanceSheet, DoubleEntry = DoubleEntry.Debit, AccountGroupParentId = new Guid("10000000-0000-0000-0000-000000000000") , IsSystem = true },
                    new AccountGroup() { Id = new Guid("10000002-1000-0000-0000-000000000000"), Name = "Cash", AccountType = FinancialStatement.BalanceSheet, DoubleEntry = DoubleEntry.Debit, AccountGroupParentId = new Guid("10000002-0000-0000-0000-000000000000"), IsSystem = true },
                    // LIABILITIES -> Balance Sheet
                    new AccountGroup() { Id = new Guid("20000000-0000-0000-0000-000000000000"), Name = "Liabilities", AccountType = FinancialStatement.BalanceSheet, DoubleEntry = DoubleEntry.Credit, IsPrimary = true, IsSystem = true },
                    new AccountGroup() { Id = new Guid("20000001-0000-0000-0000-000000000000"), Name = "Long Term Liabilities", AccountType = FinancialStatement.BalanceSheet, DoubleEntry = DoubleEntry.Credit, AccountGroupParentId = new Guid("20000000-0000-0000-0000-000000000000"), IsSystem = true },
                    new AccountGroup() { Id = new Guid("20000002-0000-0000-0000-000000000000"), Name = "Current Liabilities", AccountType = FinancialStatement.BalanceSheet, DoubleEntry = DoubleEntry.Credit, AccountGroupParentId = new Guid("20000000-0000-0000-0000-000000000000"), IsSystem = true },
                    //EQUITY -> Balance Sheet
                    new AccountGroup() { Id = new Guid("30000000-0000-0000-0000-000000000000"), Name = "Equity", AccountType = FinancialStatement.BalanceSheet, DoubleEntry = DoubleEntry.Credit, IsPrimary = true, IsSystem = true },
                    //INCOME -> Income Statement
                    new AccountGroup() { Id = new Guid("40000000-0000-0000-0000-000000000000"), Name = "Income", AccountType = FinancialStatement.IncomeStatement, DoubleEntry = DoubleEntry.Credit, IsPrimary = true, IsSystem = true },
                    //EXPENSES -> Income Statement
                    new AccountGroup() { Id = new Guid("50000000-0000-0000-0000-000000000000"), Name = "Expenses", AccountType = FinancialStatement.IncomeStatement, DoubleEntry = DoubleEntry.Debit, IsPrimary = true, IsSystem = true }
                );
                foreach (string exp in Expenses)
                {
                    var Expense = new Expense()
                    {
                        Id = Guid.NewGuid(),
                        Name = exp
                    };
                    this.context.Expenses.Add(Expense);
                    this.context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static string[] Expenses =
        {
            "Chiropody treatment",
            "Hair dressing",
            "Toileteries",
            "Dress",
            "Transport",
            "Physiotherapy",
            "Cigarettt",
            "Stamps & stationary"
        };
    }
}
