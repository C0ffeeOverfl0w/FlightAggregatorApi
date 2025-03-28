namespace FlightAggregator.Application.Interfaces;

/// <summary>
/// Интерфейс для получения данных о рейсе.
/// Абстрагирует источник данных (например, API авиаперевозчика или базу данных).
/// </summary>
public interface IFlightProvider
{
    /// <summary>
    /// Получает список рейсов с возможностью фильтрации и пагинации.
    /// </summary>
    /// <param name="flightNumber">Номер рейса (опционально).</param>
    /// <param name="departureDate">Дата вылета (опционально).</param>
    /// <param name="airline">Авиакомпания (опционально).</param>
    /// <param name="maxPrice">Максимальная цена (опционально).</param>
    /// <param name="maxStops">Максимальное количество пересадок (опционально).</param>
    /// <param name="pageNumber">Номер страницы (начиная с 1).</param>
    /// <param name="pageSize">Количество элементов на странице.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Список рейсов.</returns>
    Task<IEnumerable<Flight>> GetFlightsAsync(
        string? flightNumber,
        string origin,
        string destination,
        DateTime? departureDate,
        DateTime? returnDate,
        string? airline,
        decimal? maxPrice,
        int? maxStops,
        int passengers,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Бронирует рейс.
    /// </summary>
    /// <param name="request">Запрос на бронирование, содержащий номер рейса и имя пассажира.</param>
    /// <param name="cancellationToken">Токен отмены для отмены операции.</param>
    /// <returns>Ответ на запрос бронирования, содержащий идентификатор бронирования, результат и сообщение.</returns>
    Task<BookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Отменяет бронирование.
    /// </summary>
    /// <param name="bookingId">Идентификатор брони</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Ответ на отмену</returns>
    Task<BookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken);

    /// <summary>
    /// Запрос на бронирование рейса.
    /// </summary>
    /// <param name="FlightNumber">Номер рейса.</param>
    /// <param name="PassengerName">Имя пассажира.</param>
    public record BookingRequest(string FlightNumber, string PassengerName);
    /// <summary>
    /// Ответ на запрос бронирования рейса.
    /// </summary>
    /// <param name="BookingId">Идентификатор бронирования.</param>
    /// <param name="IsSuccess">Успешность бронирования.</param>
    /// <param name="Message">Сообщение о результате бронирования.</param>
    public record BookingResponse(string? BookingId, string FlightNumber, string PassengerName, DateTime? BookingDate, bool IsSuccess, string? Message);
}