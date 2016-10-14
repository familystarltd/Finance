using Finance.Web.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Presentation.WebAPIProxy.Serialization;

namespace Finance.Web.JsonToModelConverters
{
    public class InvoiceDetailModelConverter : CustomJsonConverter<InvoiceDetailModel>
    {
        /// <summary>
        /// The class that will create FinancialTransactionModel when proper json objects are passed in
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        protected override InvoiceDetailModel Create(Type objectType, JObject jsonObject)
        {
            // examine the $type value
            JToken jToken = jsonObject["invoice.invoiceType"];
            if (jToken == null)
                return new InvoiceDetailModel();
            string typeName = (jToken).ToString();
            // based on the $type, instantiate and return a new object
            switch (typeName)
            {
                case "1":
                    return new FeeInvoiceDetailModel();
                case "2":
                    return new FeeInvoiceDetailModel();
                default:
                    return new FeeInvoiceDetailModel();
            }
        }
    }
    public class FinancialTransactionModelConverter : CustomJsonConverter<FinancialTransactionModel>
    {
        /// <summary>
        /// The class that will create FinancialTransactionModel when proper json objects are passed in
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        protected override FinancialTransactionModel Create(Type objectType, JObject jsonObject)
        {
            // examine the $type value
            string typeName = (jsonObject["financialTransactionType"]).ToString();

            // based on the $type, instantiate and return a new object
            switch (typeName)
            {
                case "1":
                    return new FeeInvoiceModel();
                case "2":
                    return new FeeInvoiceModel();
                case "3":
                    return new ReceiptModel();
                case "4":
                    return new CreditNoteModel();
                default:
                    return new InvoiceModel();
            }
        }
    }
}