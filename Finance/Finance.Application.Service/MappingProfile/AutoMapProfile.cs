using AutoMapper;
using Finance.Domain;
using Finance.Domain.Aggregates.CustomerAgg;
using Finance.Domain.Aggregates.DisbursementAgg;
using Finance.Domain.Aggregates.FeeAgg;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using Finance.Web.Model;
using System;
using System.Collections.Generic;
using System.Infrastructure.CrossCutting.Framework.Adapter;
using System.Infrastructure.CrossCutting.Framework.Extensions;
using System.Linq;

namespace Finance.Application.Service.MappingProfile
{
    public class DomainToViewModelMapper : Profile
    {
        public DomainToViewModelMapper()
        {

        }
        public T Map<T>(object source)
        {
            return this.Map<T>(source);
        }
        protected override void Configure()
        {
            CreateMap<Contact, ContactModel>().ForMember(model => model.PictureRawPhoto, mc => mc.MapFrom(c => c.Photo));
            CreateMap<Rate, RateModel>().Include<WeeklyRate, RateModel>().Include<HourlyRate, RateModel>().Include<MonthlyRate, RateModel>().Include<DailyRate, RateModel>();
            CreateMap<WeeklyRate, RateModel>()
                .ForMember(wr => wr.RateMethod, mc => mc.UseValue(Web.Model.RateMethod.Weekly))
                .ForMember(wr => wr.DayPremium, rm => rm.Ignore())
                .ForMember(wr => wr.TimePremium, rm => rm.Ignore())
                .ForMember(wr => wr.NoOfHours, rm => rm.Ignore())
                .ForMember(wr => wr.TotalRate, rm => rm.Ignore());
            CreateMap<MonthlyRate, RateModel>()
                .ForMember(wr => wr.RateMethod, mc => mc.UseValue(Web.Model.RateMethod.Monthly))
                .ForMember(wr => wr.DayPremium, rm => rm.Ignore())
                .ForMember(wr => wr.TimePremium, rm => rm.Ignore())
                .ForMember(wr => wr.NoOfHours, rm => rm.Ignore())
                .ForMember(wr => wr.TotalRate, rm => rm.Ignore());
            CreateMap<DailyRate, RateModel>()
                .ForMember(wr => wr.RateMethod, mc => mc.UseValue(Web.Model.RateMethod.Daily))
                .ForMember(wr => wr.TimePremium, rm => rm.Ignore())
                .ForMember(wr => wr.NoOfHours, rm => rm.Ignore())
                .ForMember(wr => wr.TotalRate, rm => rm.Ignore());
            CreateMap<HourlyRate, RateModel>().ForMember(wr => wr.RateMethod, mc => mc.UseValue(Web.Model.RateMethod.Hourly));
            CreateMap<FinancialTransaction, FinancialTransactionModel>().ForMember(tran => tran.FinancialTransactionType, opt => opt.Ignore()).Include<Invoice, InvoiceModel>().Include<Receipt, ReceiptModel>().Include<CreditNote, CreditNoteModel>();
            CreateMap<Invoice, InvoiceModel>().ReverseMap();
            CreateMap<FeeInvoice, FeeInvoiceModel>().ReverseMap();
            CreateMap<InvoiceDetail, InvoiceDetailModel>().ReverseMap();
            CreateMap<FeeInvoiceDetail, FeeInvoiceDetailModel>().ReverseMap();
            CreateMap<Invoice, InvoiceModel>().Include<FeeInvoice, FeeInvoiceModel>();
            CreateMap<InvoiceDetail, InvoiceDetailModel>().Include<FeeInvoiceDetail, FeeInvoiceDetailModel>();
            CreateMap<Company, CompanyModel>().ReverseMap();
            CreateMap<Customer, CustomerModel>().ReverseMap();
            CreateMap<Funder, FunderModel>().ReverseMap();
            CreateMap<Fee, FeeModel>().ReverseMap();
            CreateMap<FeeRate, FeeRateModel>().ReverseMap();
            CreateMap<InvoiceArticleTemplate, InvoiceArticleTemplateModel>().ReverseMap();
            CreateMap<InvoiceAdjustment, InvoiceAdjustmentModel>().ReverseMap();
            CreateMap<Receipt, ReceiptModel>().ReverseMap();
            CreateMap<ReceiptInvoice, ReceiptInvoiceModel>().ReverseMap();
            CreateMap<CreditNote, CreditNoteModel>().ReverseMap();
            CreateMap<AppLog, AppLogModel>().ReverseMap();
            CreateMap<Expense, ExpenseModel>().ReverseMap();
            CreateMap<Disbursement, DisbursementModel>().ReverseMap();
            CreateMap<CustomerDisbursementFunder, CustomerDisbursementFunderModel>().ReverseMap();            
        }
    }
    public class ViewModelToDomainMapper : Profile
    {
        public ViewModelToDomainMapper()
        {

        }
        public T Map<T>(object source)
        {
            return this.Map<T>(source);
        }
        private static Rate RateCreator(RateModel rateModel)
        {
            Web.Model.RateMethod rateMethod;
            if (rateModel.FeeRate != null)
                rateMethod = rateModel.FeeRate.RateMethod;
            else
                rateMethod = rateModel.RateMethod;
            switch (rateMethod)
            {
                case Finance.Web.Model.RateMethod.Hourly:
                    return new HourlyRate
                    {
                        DayPremium = Mapper.Map<Domain.Aggregates.FeeAgg.DayPremium>(rateModel.DayPremium),
                        TimePremium = Mapper.Map<Domain.Aggregates.FeeAgg.TimePremium>(rateModel.TimePremium),
                        NoOfHours = rateModel.NoOfHours
                    };
                case Finance.Web.Model.RateMethod.Daily:
                    return new DailyRate
                    {
                        DayPremium = Mapper.Map<Domain.Aggregates.FeeAgg.DayPremium>(rateModel.DayPremium)
                    };
                case Finance.Web.Model.RateMethod.Weekly:
                    return new WeeklyRate();
                case Finance.Web.Model.RateMethod.Monthly:
                    return new MonthlyRate();
            }
            return null;
        }
        protected override void Configure()
        {
            CreateMap<ContactModel, Contact>().ForMember(dest => dest.Photo, mc => mc.MapFrom(c => c.PictureRawPhoto));
            CreateMap<RateModel, Rate>().ConstructUsing(RateCreator);
            CreateMap<FinancialTransactionModel, FinancialTransaction>().ForMember(tran => tran.FinancialTransactionType, opt => opt.Ignore())
                .Include<InvoiceModel, Invoice>()
                .Include<ReceiptModel, Receipt>()
                .Include<CreditNoteModel, CreditNote>();
            CreateMap<DisbursementModel, Disbursement>().ForMember(dest => dest.Expense,
                m => m.ResolveUsing(src =>
                {
                    return new Expense() { Name = src.Expense.Name, Id = src.Expense.Id };
                }));
        }
    }
}