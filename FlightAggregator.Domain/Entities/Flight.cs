namespace FlightAggregator.Domain.Entities;

public class Flight(string id,
                    string flightNumber,
                    DateTime departureTime,
                    DateTime arrivalTime,
                    int durationMinutes,
                    Airline airline,
                    Money price,
                    int stops,
                    List<Flight.StopDetailData> stopDetails,
                    string origin,
                    string destination,
                    string source)
{
    public string Id { get; } = Guid.NewGuid().ToString();

    public string FlightNumber { get; } =
        string.IsNullOrWhiteSpace(flightNumber)
            ? throw new ArgumentException("Номер рейса не может быть пустым.", nameof(flightNumber))
            : flightNumber;

    public DateTime DepartureTime { get; } =
        departureTime < DateTime.UtcNow
            ? throw new ArgumentException("Дата вылета не может быть в прошлом.", nameof(departureTime))
            : departureTime;

    public DateTime ArrivalTime { get; } =
        arrivalTime < departureTime
            ? throw new ArgumentException("Дата прилета не может быть раньше даты вылета.", nameof(arrivalTime))
            : arrivalTime;

    public int DurationMinutes { get; } =
        durationMinutes < 0
            ? throw new ArgumentException("Длительность рейса не может быть отрицательной.", nameof(durationMinutes))
            : durationMinutes;

    public Airline Airline { get; } =
        airline ?? throw new ArgumentNullException(nameof(airline));

    public Money Price { get; } =
        price.Amount < 0
            ? throw new ArgumentException("Цена не может быть отрицательной.", nameof(price))
            : price;

    public int Stops { get; } =
        stops < 0
            ? throw new ArgumentException("Количество пересадок не может быть отрицательным.", nameof(stops))
            : stops;

    public List<Flight.StopDetailData> StopDetails { get; } = stopDetails ?? new List<Flight.StopDetailData>();

    public string Origin { get; } =
        string.IsNullOrWhiteSpace(origin)
            ? throw new ArgumentException("Пункт отправления не может быть пустым.", nameof(origin))
            : origin;

    public string Destination { get; } =
        string.IsNullOrWhiteSpace(destination)
            ? throw new ArgumentException("Пункт назначения не может быть пустым.", nameof(destination))
            : destination;

    public string Source { get; } =
        string.IsNullOrWhiteSpace(source)
            ? throw new ArgumentException("Источник не может быть пустым.", nameof(source))
            : source;

    public sealed record StopDetailData(string Airport, int DurationMinutes);
}