using System;
using System.Collections.Generic;
using Finance.Domain.Aggregates.FeeAgg;
using Finance.Web.Model;
using System.Infrastructure.CrossCutting.Framework.Extensions;
namespace Finance.Application.Service
{
    public class PayerAppService : IPayerAppService
    {
        readonly IPayerRepository _PayerRepository;
        public PayerAppService(IPayerRepository PayerRepository)
        {
            this._PayerRepository = PayerRepository;
        }
        public IEnumerable<PayerModel> GetPayers(string searchText)
        {
            try
            {
                return this._PayerRepository.GetPayers(searchText).ProjectedToCollection<PayerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<PayerModel> GetPayers(string searchText, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                searchText = string.IsNullOrEmpty(searchText) ? string.Empty : searchText.Trim();
                return this._PayerRepository.GetPayers(searchText, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<PayerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public PayerModel GetPayer(Guid PayerId)
        {
            try
            {
                return this._PayerRepository.GetPayer(PayerId).ProjectedTo<PayerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public PayerModel SavePayer(PayerModel payerModel)
        {
            try
            {
                Payer payer = DataProjections.ProjectedTo<Payer>(payerModel);
                if (payer.PersonalContact != null && payer.PersonalContact.Id == Guid.Empty)
                    payer.PersonalContact.Id = Guid.NewGuid();
                if (payer.FeeInvoiceDeliveryContact != null && payer.FeeInvoiceDeliveryContact.Id == Guid.Empty)
                    payer.FeeInvoiceDeliveryContact.Id = Guid.NewGuid();
                if (payer.FeeInvoiceBillingContact != null && payer.FeeInvoiceBillingContact.Id == Guid.Empty)
                    payer.FeeInvoiceBillingContact.Id = Guid.NewGuid();
                if (payer.DisbursementInvoiceBillingContact != null && payer.DisbursementInvoiceBillingContact.Id == Guid.Empty)
                    payer.DisbursementInvoiceBillingContact.Id = Guid.NewGuid();
                if (payer.DisbursementInvoiceDeliveryContact != null && payer.DisbursementInvoiceDeliveryContact.Id == Guid.Empty)
                    payer.DisbursementInvoiceDeliveryContact.Id = Guid.NewGuid();
                Payer payerOriginal = _PayerRepository.GetPayer(payerModel.Id);
                if (payerOriginal == null)
                {
                    if (payer.Id == Guid.Empty)
                        payer.Id = Guid.NewGuid();
                    _PayerRepository.Add(payer);
                    this._PayerRepository.UnitOfWork.SaveChanges();
                }
                else
                {
                    this._PayerRepository.Merge(payerOriginal, payer);
                    this._PayerRepository.UnitOfWork.Commit();
                }
                return _PayerRepository.Get(payer.Id).ProjectedTo<PayerModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public void DeletePayer(Guid Id)
        {
            try
            {
                Payer payer = _PayerRepository.GetPayer(Id);
                if (payer != null)
                {
                    if (payer.PersonalContact != null)
                        _PayerRepository.RemoveContact(payer.PersonalContact);
                    if (payer.FeeInvoiceBillingContact != null)
                        _PayerRepository.RemoveContact(payer.FeeInvoiceBillingContact);
                    if (payer.FeeInvoiceDeliveryContact != null)
                        _PayerRepository.RemoveContact(payer.FeeInvoiceDeliveryContact);
                    if (payer.DisbursementInvoiceBillingContact != null)
                        _PayerRepository.RemoveContact(payer.DisbursementInvoiceBillingContact);
                    if (payer.DisbursementInvoiceDeliveryContact != null)
                        _PayerRepository.RemoveContact(payer.DisbursementInvoiceDeliveryContact);
                    this._PayerRepository.Remove(payer);
                    this._PayerRepository.UnitOfWork.Commit();
                }
                else
                    throw new ArgumentException("Payer Not Found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
