﻿jQuery.extend({
    Fee: {
        id: "",
        name:"",
        description: "",
        customerId:"",
        customer:"",
        payerId: "",
        payer:"",
        paymentTerm:"",
        effectiveDate:"",
        closingDate:"",
        feeRates: [],
        FeeAdjustments: [],
        Notes: []
    },
});

jQuery.extend({
    FeeRate: {
        Id:"",
        RateMethod:"",
        RateDescription:"",
        Rates:[]
}
});
jQuery.extend({
    Rate: {
        Id: "",
        RateMethod: "",
        RateDescription: "",
        Rates: []
    }
});