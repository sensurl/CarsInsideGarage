namespace CarsInsideGarage.Services.Time
{
    public class DateTimeProviderService : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
