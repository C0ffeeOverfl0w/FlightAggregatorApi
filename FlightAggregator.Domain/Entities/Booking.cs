namespace FlightAggregator.Domain.Entities;

/// <summary>
/// Класс, представляющий бронирование.
/// </summary>
public class Booking(Flight flight, string passengerName, string passengerEmail)
{
    /// <summary>
    /// Уникальный идентификатор бронирования.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Рейс, связанный с бронированием.
    /// </summary>
    public Flight Flight { get; } =
        flight ?? throw new ArgumentNullException(nameof(flight));

    /// <summary>
    /// Имя пассажира.
    /// </summary>
    public string PassengerName { get; } =
        string.IsNullOrWhiteSpace(passengerName)
            ? throw new ArgumentException("Имя пассажира обязательно.", nameof(passengerName))
            : passengerName;

    /// <summary>
    /// Email пассажира.
    /// </summary>
    public string PassengerEmail { get; } =
        string.IsNullOrWhiteSpace(passengerEmail)
            ? throw new ArgumentException("Email пассажира обязателен.", nameof(passengerEmail))
            : passengerEmail;

    /// <summary>
    /// Дата бронирования.
    /// </summary>
    public DateTime BookingDate { get; } = DateTime.UtcNow;

    public BookingStatus Status { get; private set; } = BookingStatus.Active;

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Бронирование уже отменено.");
        Status = BookingStatus.Cancelled;
    }

    public enum BookingStatus
    {
        Active,
        Cancelled
    }
}