namespace Finance.Domain.Aggregates.FeeAgg
{
    public enum PaymentTerm : int
    {
        Monthly=1,
        FourWeely = 2
        //Weekly=3,
        //Fortnightly=4,
        //Quarterly = 5,
        //SemiAnnually = 6,
        //Annually = 7
    }
    public enum RateMethod : int
    {
        Hourly=1,
        Daily=2,
        Weekly=3,
        Monthly=4
    }
    public enum DayPremium : int
    {
        Weekday = 1,
        Weekend = 2,
        Bankholiday = 3
    }
    public enum TimePremium : int
    {
        Day = 1,
        Night = 2
    }
}