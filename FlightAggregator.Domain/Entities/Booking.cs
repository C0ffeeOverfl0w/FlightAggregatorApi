namespace FlightAggregator.Domain.Entities;

/// <summary>
/// Класс, представляющий бронирование.
/// </summary>
public class Booking
{
    public Guid? Id { get; private set; }
    public string? FlightId { get; private set; }
    public string? FlightNumber { get; private set; }
    public string? PassengerName { get; private set; }
    public string? PassengerEmail { get; private set; }
    public DateTime? BookingDate { get; private set; }

    public virtual Flight? Flight { get; private set; }

    protected Booking()
    { }

    public Booking(Flight flight, string passengerName, string passengerEmail)
    {
        Flight = flight ?? throw new ArgumentNullException(nameof(flight));
        if (string.IsNullOrWhiteSpace(passengerName))
            throw new ArgumentException("Имя пассажира не может быть пустым.", nameof(passengerName));
        if (string.IsNullOrWhiteSpace(passengerEmail))
            throw new ArgumentException("Email пассажира не может быть пустым.", nameof(passengerEmail));

        Id = Guid.NewGuid();
        PassengerName = passengerName;
        PassengerEmail = passengerEmail;
        BookingDate = DateTime.UtcNow;
    }
}