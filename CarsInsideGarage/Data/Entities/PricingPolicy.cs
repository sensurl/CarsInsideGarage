namespace CarsInsideGarage.Data.Entities
{
    public class PricingPolicy
    {
        public decimal HourlyRate { get; private set; }
        public decimal DailyRate { get; private set; }
        public decimal MonthlyRate { get; private set; }

        private readonly List<PricingRule> _rules = new();
        public IReadOnlyCollection<PricingRule> Rules => _rules.AsReadOnly();

        private PricingPolicy() { }

        public PricingPolicy(decimal hourly, decimal daily, decimal monthly)
        {
            HourlyRate = hourly;
            DailyRate = daily;
            MonthlyRate = monthly;
        }

        public void AddRule(PricingRule rule)
        {
            _rules.Add(rule);
        }

        public decimal GetEffectiveHourlyRate(DateTime dateTime)
        {
            decimal rate = HourlyRate;

            var rule = _rules.FirstOrDefault(r => r.AppliesTo(dateTime));

            if (rule != null)
                rate = rule.Apply(rate);

            return rate;
        }
    }

}
