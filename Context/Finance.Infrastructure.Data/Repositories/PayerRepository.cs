﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Infrastructure.Data;
using Finance.Infrastructure.Data.UnitOfWork;
using Finance.Domain.Aggregates.FeeAgg;
using Finance.Domain.Aggregates.CustomerAgg;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Customer Repository implementation.
    /// <see cref="Finance.Domain.IPayerRepository"/>
    /// </summary>
    public class PayerRepository : Repository<Payer>, IPayerRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public PayerRepository(IFinanceDbContext unitOfWork) : base(unitOfWork) { }
        public override void Merge(Payer persisted, Payer current)
        {
             var currentUOW = this.UnitOfWork as IFinanceDbContext;
            
            if (persisted == null || current == null)
                return;
            currentUOW.ApplyCurrentValues(persisted, current);
            if (persisted.FeeInvoiceBillingContact == null && current.FeeInvoiceBillingContact != null)
                persisted.FeeInvoiceBillingContact = new Contact();
            else if (persisted.FeeInvoiceBillingContact != null && current.FeeInvoiceBillingContact == null)
            {
                currentUOW.Set<Contact>().Remove(persisted.FeeInvoiceBillingContact);
                persisted.FeeInvoiceBillingContact = null;
            }

            if (persisted.FeeInvoiceDeliveryContact == null && current.FeeInvoiceDeliveryContact != null)
                persisted.FeeInvoiceDeliveryContact = new Contact();
            else if (persisted.FeeInvoiceDeliveryContact != null && current.FeeInvoiceDeliveryContact == null)
            {
                currentUOW.Set<Contact>().Remove(persisted.FeeInvoiceDeliveryContact);
                persisted.FeeInvoiceDeliveryContact = null;
            }

            if (persisted.DisbursementInvoiceDeliveryContact == null && current.DisbursementInvoiceDeliveryContact != null)
                persisted.DisbursementInvoiceDeliveryContact = new Contact();
            else if (persisted.DisbursementInvoiceDeliveryContact != null && current.DisbursementInvoiceDeliveryContact == null)
            {
                currentUOW.Set<Contact>().Remove(persisted.DisbursementInvoiceDeliveryContact);
                persisted.DisbursementInvoiceDeliveryContact = null;
            }

            if (persisted.DisbursementInvoiceBillingContact == null && current.DisbursementInvoiceBillingContact != null)
                persisted.DisbursementInvoiceBillingContact = new Contact();
            else if (persisted.DisbursementInvoiceBillingContact != null && current.DisbursementInvoiceBillingContact == null)
            {
                currentUOW.Set<Contact>().Remove(persisted.DisbursementInvoiceBillingContact);
                persisted.DisbursementInvoiceBillingContact = null;
            }
            if (persisted.PersonalContact != null && current.PersonalContact != null)
                currentUOW.ApplyCurrentValues(persisted.PersonalContact, current.PersonalContact);
            if (persisted.FeeInvoiceBillingContact != null && current.FeeInvoiceBillingContact != null)
                currentUOW.ApplyCurrentValues(persisted.FeeInvoiceBillingContact, current.FeeInvoiceBillingContact);
            if (persisted.FeeInvoiceDeliveryContact != null && current.FeeInvoiceDeliveryContact != null)
                currentUOW.ApplyCurrentValues(persisted.FeeInvoiceDeliveryContact, current.FeeInvoiceDeliveryContact);

            if (persisted.DisbursementInvoiceBillingContact != null && current.DisbursementInvoiceBillingContact != null)
                currentUOW.ApplyCurrentValues(persisted.DisbursementInvoiceBillingContact, current.DisbursementInvoiceBillingContact);
            if (persisted.DisbursementInvoiceDeliveryContact != null && current.DisbursementInvoiceDeliveryContact != null)
                currentUOW.ApplyCurrentValues(persisted.DisbursementInvoiceDeliveryContact, current.DisbursementInvoiceDeliveryContact);
        }
        public IEnumerable<Payer> GetPayers(string searchText)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.Payers
                    .Include(f => f.PersonalContact)
                    .Include(f=>f.Fees)
                    .Where(f => f.Name.ToLower().StartsWith(searchText.ToLower()) || string.IsNullOrEmpty(searchText));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Payer> GetPayers(string searchText, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.Payers
                    .Include(f => f.PersonalContact)
                    .Include(f => f.Fees)
                    .Count(f => f.Name.ToLower().StartsWith(searchText.ToLower()) || string.IsNullOrEmpty(searchText));

                return uow.Payers
                     .Include(f => f.PersonalContact)
                    .Include(f => f.FeeInvoiceBillingContact)
                    .Include(f => f.FeeInvoiceDeliveryContact)
                    .Include(f => f.DisbursementInvoiceBillingContact)
                    .Include(f => f.DisbursementInvoiceDeliveryContact)
                     .Include(f => f.Fees)
                     .Where(f => f.Name.ToLower().StartsWith(searchText.ToLower()) || string.IsNullOrEmpty(searchText))
                     .OrderBy(f => f.Name)
                     .Skip(pageIndex * pageSize)
                     .Take(pageSize);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Payer> GetPayersWithFees(DateTime FeeDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            var uow = this.UnitOfWork as IFinanceDbContext;
            TotalRowCount = uow.Payers.Count(f => uow.Fees.Any(fee => fee.PayerId == f.Id && !fee.ClosingDate.HasValue || (fee.ClosingDate.HasValue && DbFunctions.TruncateTime(fee.ClosingDate) >= FeeDate.Date)));
            IEnumerable<Payer> Payers = uow.Payers
                .Where(f =>  uow.Fees.Any(fee=>fee.PayerId == f.Id && !fee.ClosingDate.HasValue || (fee.ClosingDate.HasValue && DbFunctions.TruncateTime(fee.ClosingDate) >= FeeDate.Date)))
                .OrderBy(f => f.Name).Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(f => new Payer
                {
                    Id = f.Id,
                    Name =f.Name,
                    Fees =
                        uow.Fees
                        .Include(cf => cf.Payer.PersonalContact).Include(cf => cf.Payer.FeeInvoiceBillingContact).Include(cf => cf.Payer.FeeInvoiceDeliveryContact).Include(cf => cf.Payer.DisbursementInvoiceBillingContact).Include(cf => cf.Payer.DisbursementInvoiceDeliveryContact)
                        .Where(fee => (!fee.ClosingDate.HasValue || (fee.ClosingDate.HasValue && DbFunctions.TruncateTime(fee.ClosingDate) >= FeeDate.Date)) && fee.PayerId == f.Id).ToList()
                        .Select(fee => new Fee
                        {
                            Id = fee.Id,
                            Name = fee.Name,
                            Description = fee.Description,
                            Payer = fee.Payer,
                            PayerId = fee.PayerId,
                            Customer = fee.Customer,
                            CustomerId = fee.CustomerId,
                            ClosingDate = fee.ClosingDate,
                            EffectiveDate = fee.EffectiveDate,
                            PaymentTerm = fee.PaymentTerm,
                            PayContactReference = fee.PayContactReference,
                            LogDateTime = fee.LogDateTime,
                            LogUser = fee.LogUser,
                            Notes = fee.Notes,
                            FeeRates = uow.FeeRates.Where(fr => fr.FeeId == fee.Id).ToList().Select(
                                                                fr => new FeeRate
                                                                {
                                                                    Id = fr.Id,
                                                                    Fee = fee,
                                                                    FeeId = fee.Id,
                                                                    RateDescription = fr.RateDescription,
                                                                    RateMethod = fr.RateMethod,
                                                                    Rates = uow.Rates.Where(r => r.FeeRateId == fr.Id && !r.ClosingDate.HasValue)
                                                                        .OrderBy(r => (r is DailyRate) ? (r as DailyRate).DayPremium : ((r is HourlyRate) ? (r as HourlyRate).DayPremium : 0))
                                                                        .ThenBy((r => (r is HourlyRate) ? (r as HourlyRate).TimePremium : 0)).ToList()
                                                                }).ToList()
                        }).ToList()
                });
            return Payers;
        }
        public Payer GetPayer(Guid payerId)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.Payers
                    .Include(f => f.PersonalContact)
                    .Include(f => f.FeeInvoiceBillingContact)
                    .Include(f => f.FeeInvoiceDeliveryContact)
                    .Include(f => f.DisbursementInvoiceBillingContact)
                    .Include(f => f.DisbursementInvoiceDeliveryContact)
                    .Where(f => f.Id == payerId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public void RemoveContact(Contact contact)
        {
            var currentUOW = this.UnitOfWork as IFinanceDbContext;
            currentUOW.Set<Contact>().Remove(contact);
        }
    }
}