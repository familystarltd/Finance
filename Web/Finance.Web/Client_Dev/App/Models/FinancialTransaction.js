//jQuery.extend({
//    Model: {
//        Receipt: {
//            Id: "",
//            ReceiptNo: "",
//            PaymentMethod: "",
//            RefPrefix: "",
//            FinancialTransactionType: "",
//            Discriminator: "",
//            TransactionRef: "",
//            ProcessedDate: "",
//            Amount: "",
//            Description: "",
//            ReceiptFeeInvoices: [],
//            CreditNote: "",
//            CreditNoteId: ""
//        },
//        ReceiptFeeInvoice: { Id: "", ReceiptId: "", Receipt: "", FeeInvoiceId: "", FeeInvoice: "", AmountReceived: "" }
//    }
//});

var Receipt = function () {
    this.id = "";
    this.receiptNo = "";
    this.ContactReference = "";
    this.payerId = "";
    this.payer = "";
    this.paymentMethod = "";
    this.refPrefix = "";
    this.financialTransactionType = "";
    this.discriminator = "";
    this.transactionRef = "";
    this.processedDate = "";
    this.amount = "";
    this.description = "";
    this.receiptInvoices = [];
    this.creditNote = "";
    this.creditNoteId = "";
};
var ReceiptInvoice = function () { id = ""; receiptId = ""; receipt = new Receipt(); invoiceId = ""; invoice = ""; amountReceived = ""; };

var CreditNote = function () {
    this.id = "";
    this.creditNoteNo = "";
    this.refPrefix = "";
    this.financialTransactionType = "";
    this.discriminator = "";
    this.transactionRef = "";
    this.processedDate = "";
    this.amount = "";
    this.creditAvailable = "";
    this.description = "";
    this.payerId = "";
    this.receipts = [];
    this.invoiceId = "";
    this.invoiceCreditAmount = "";
};
var Payer = function () {
    this.name = "";
}



