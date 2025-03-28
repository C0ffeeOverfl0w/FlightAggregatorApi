namespace FlightAggregator.Domain.Services;

public sealed class BookingDomainService
{
    /// <summary>
    /// Создает бронирование для заданного рейса, проверяя, что рейс еще не прошел.
    /// </summary>
    /// <param name="flight">Рейс для бронирования.</param>
    /// <param name="passengerName">Имя пассажира.</param>
    /// <param name="passengerEmail">Email пассажира.</param>
    /// <returns>Созданное бронирование.</returns>
    /// <exception cref="InvalidOperationException">Если рейс уже прошел.</exception>
    public Booking CreateBooking(Flight flight, string passengerName, string passengerEmail)
    {
        if (flight.DepartureTime < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Нельзя забронировать рейс, который уже прошел.");
        }

        var booking = new Booking(flight, passengerName, passengerEmail);

        return booking;
    }
}