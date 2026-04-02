namespace CarsInsideGarage.Services.Time
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
