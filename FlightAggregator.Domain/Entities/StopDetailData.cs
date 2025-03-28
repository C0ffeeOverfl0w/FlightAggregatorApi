namespace FlightAggregator.Domain.Entities;

public class StopDetailData
{
    public string Airport { get; }
    public int DurationMinutes { get; }

    public StopDetailData(string airport, int durationMinutes)
    {
        Airport = airport;
        DurationMinutes = durationMinutes;
    }
}