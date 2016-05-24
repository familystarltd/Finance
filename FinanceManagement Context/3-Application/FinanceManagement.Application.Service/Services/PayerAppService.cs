using System;
using System.Collections.Generic;
using FinanceManagement.Domain.Aggregates.FeeAgg;
using FinanceManagement.Web.Model;
using System.Infrastructure.CrossCutting.Framework.Extensions;
namespace FinanceManagement.Application.Service
{
    public class FunderAppService : IFunderAppService
    {
        readonly IFunderRepository _FunderRepository;
        public FunderAppService(IFunderRepository FunderRepository)
        {
            this._FunderRepository = FunderRepository;
        }
        public IEnumerable<FunderModel> GetFunders(string searchText)
        {
            try
            {
                return this._FunderRepository.GetFunders(searchText).ProjectedToCollection<FunderModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<FunderModel> GetFunders(string searchText, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                searchText = string.IsNullOrEmpty(searchText) ? string.Empty : searchText.Trim();
                return this._FunderRepository.GetFunders(searchText, pageIndex, pageSize, out TotalRowCount).ProjectedToCollection<FunderModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public FunderModel GetFunder(Guid FunderId)
        {
            try
            {
                return this._FunderRepository.GetFunder(FunderId).ProjectedTo<FunderModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public FunderModel SaveFunder(FunderModel funderModel)
        {
            try
            {
                Funder funder = DataProjections.ProjectedTo<Funder>(funderModel);
                if (funder.PersonalContact != null && funder.PersonalContact.Id == Guid.Empty)
                    funder.PersonalContact.Id = Guid.NewGuid();
                if (funder.FeeInvoiceDeliveryContact != null && funder.FeeInvoiceDeliveryContact.Id == Guid.Empty)
                    funder.FeeInvoiceDeliveryContact.Id = Guid.NewGuid();
                if (funder.FeeInvoiceBillingContact != null && funder.FeeInvoiceBillingContact.Id == Guid.Empty)
                    funder.FeeInvoiceBillingContact.Id = Guid.NewGuid();
                if (funder.DisbursementInvoiceBillingContact != null && funder.DisbursementInvoiceBillingContact.Id == Guid.Empty)
                    funder.DisbursementInvoiceBillingContact.Id = Guid.NewGuid();
                if (funder.DisbursementInvoiceDeliveryContact != null && funder.DisbursementInvoiceDeliveryContact.Id == Guid.Empty)
                    funder.DisbursementInvoiceDeliveryContact.Id = Guid.NewGuid();
                Funder funderOriginal = _FunderRepository.GetFunder(funderModel.Id);
                if (funderOriginal == null)
                {
                    if (funder.Id == Guid.Empty)
                        funder.Id = Guid.NewGuid();
                    _FunderRepository.Add(funder);
                    this._FunderRepository.UnitOfWork.SaveChanges();
                }
                else
                {
                    this._FunderRepository.Merge(funderOriginal, funder);
                    this._FunderRepository.UnitOfWork.Commit();
                }
                return _FunderRepository.Get(funder.Id).ProjectedTo<FunderModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public void DeleteFunder(Guid Id)
        {
            try
            {
                Funder funder = _FunderRepository.GetFunder(Id);
                if (funder != null)
                {
                    if (funder.PersonalContact != null)
                        _FunderRepository.RemoveContact(funder.PersonalContact);
                    if (funder.FeeInvoiceBillingContact != null)
                        _FunderRepository.RemoveContact(funder.FeeInvoiceBillingContact);
                    if (funder.FeeInvoiceDeliveryContact != null)
                        _FunderRepository.RemoveContact(funder.FeeInvoiceDeliveryContact);
                    if (funder.DisbursementInvoiceBillingContact != null)
                        _FunderRepository.RemoveContact(funder.DisbursementInvoiceBillingContact);
                    if (funder.DisbursementInvoiceDeliveryContact != null)
                        _FunderRepository.RemoveContact(funder.DisbursementInvoiceDeliveryContact);
                    this._FunderRepository.Remove(funder);
                    this._FunderRepository.UnitOfWork.Commit();
                }
                else
                    throw new ArgumentException("Funder Not Found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
