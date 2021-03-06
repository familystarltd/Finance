﻿using Finance.Domain.Aggregates.DisbursementAgg;
using System;
using System.Linq;

namespace Finance.Infrastructure.Data.UnitOfWork
{
    class ExpensesDataSeeder
    {
        IFinanceDbContext context;
        public ExpensesDataSeeder(IFinanceDbContext context)
        {
            this.context = context;
        }
        public void Seed()
        {
            if (this.context.Expenses.Count() > 0)
                return;
            try
            {
                foreach(string exp in Expenses)
                {
                    var Expense = new Expense()
                    {
                        Id= Guid.NewGuid(),
                        Name = exp
                    };
                    this.context.Expenses.Add(Expense);
                    this.context.SaveChanges();
                }
            }
            catch(Exception ex)
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
