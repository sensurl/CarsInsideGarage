using CarsInsideGarage.Data.Enums;

namespace CarsInsideGarage.Data.Entities
{
    public class PricingRule
    {
        public int Id { get; private set; }

        // Optional time window (e.g. 18:00–22:00)
        public TimeSpan? StartHour { get; private set; }
        public TimeSpan? EndHour { get; private set; }

        // Either multiplier OR adjustment
        public decimal? Multiplier { get; private set; }
        public decimal? Adjustment { get; private set; }

        private PricingRule() { }

        public PricingRule(
            TimeSpan? startHour,
            TimeSpan? endHour,
            decimal? multiplier,
            decimal? adjustment)
        {
            if (multiplier.HasValue && adjustment.HasValue)
                throw new ArgumentException("Rule cannot have both multiplier and adjustment.");

            if (!multiplier.HasValue && !adjustment.HasValue)
                throw new ArgumentException("Rule must define multiplier or adjustment.");

            StartHour = startHour;
            EndHour = endHour;
            Multiplier = multiplier;
            Adjustment = adjustment;
        }

        public bool AppliesTo(DateTime dateTime)
        {
            if (StartHour.HasValue && EndHour.HasValue)
            {
                var time = dateTime.TimeOfDay;

                if (time < StartHour.Value || time >= EndHour.Value)
                    return false;
            }

            return true;
        }

        public decimal Apply(decimal baseRate)
        {
            if (Multiplier.HasValue)
                return baseRate * Multiplier.Value;

            return baseRate + Adjustment!.Value;
        }
    }

}
