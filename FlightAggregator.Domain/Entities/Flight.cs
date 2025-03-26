namespace FlightAggregator.Domain.Entities;

public class Flight(string flightNumber, DateTime departureDate, Airline airline, Money price, int stops)
{
    public Guid Id { get; } = Guid.NewGuid();

    public string FlightNumber { get; } =
        string.IsNullOrWhiteSpace(flightNumber)
            ? throw new ArgumentException("Номер рейса не может быть пустым.", nameof(flightNumber))
            : flightNumber;

    public DateTime DepartureDate { get; } = departureDate;

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
}