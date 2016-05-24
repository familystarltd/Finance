using System;
using System.Collections.Generic;
using System.Domain;
namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public enum FinancialStatement : int
    {
        BalanceSheet= 1,
        IncomeStatement = 2,
        CashFlow=3
    }
    public enum DoubleEntry : int
    {
        None = 0,
        Debit = 1,
        Credit = 2
    }
    public enum AccountingEffect : int
    {
        Increase = 1,
        Decrease = 2
    }
    public class AccountGroup : Entity
    {
        public string Name { get; set; }
        public Guid? AccountGroupParentId { get; set; }
        public virtual AccountGroup AccountGroupParent { get; set; }
        public virtual ICollection<AccountGroup> AccountGroups { get; set; }
        FinancialStatement accountType;
        public FinancialStatement AccountType { get { return accountType; } set { if (AccountGroupParent != null) { accountType = AccountGroupParent.AccountType; } else { accountType = value; } } }
        DoubleEntry doubleEntry;
        public DoubleEntry DoubleEntry { get { return doubleEntry; } set { if (AccountGroupParent != null) { doubleEntry = AccountGroupParent.DoubleEntry; } else { doubleEntry = value;  } } }
        //AccountingEffect accountingEffect;
        //public AccountingEffect AccountingEffect { get { return accountingEffect; } set { if (AccountGroupParent != null) { accountingEffect = AccountGroupParent.AccountingEffect; } else { accountingEffect = value; } } }
        public bool IsSystem { get; set; }
        public bool IsPrimary { get; set; }
    }
    public class Account : Entity
    {
        public int NominalCode { get; set; }
        public string Name { get; set; }
        public AccountGroup AccountGroup { get; set; }
        public Guid? ContraOffsetAccountId { get; set; }
        public Account ContraOffsetAccount { get; set; }
        public virtual ICollection<Account> ContraAccounts { get; set; }
        public bool IsSystem { get; set; }
    }
}