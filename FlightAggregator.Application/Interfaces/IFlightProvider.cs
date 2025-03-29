namespace FlightAggregator.Application.Interfaces;

/// <summary>
/// Интерфейс для работы с провайдерами авиаперелетов.
/// </summary>
public interface IFlightProvider
{
    /// <summary>
    /// Поиск рейсов согласно переданному запросу.
    /// </summary>
    /// <param name="query">Запрос на поиск рейсов.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список найденных рейсов.</returns>
    Task<IEnumerable<Flight>> GetFlightsAsync(SearchFlightsQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Бронирование рейса.
    /// </summary>
    /// <param name="request">Запрос на бронирование.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ от провайдера о бронировании.</returns>
    Task<ProviderBookingResponse> BookFlightAsync(BookingRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Отмена бронирования.
    /// </summary>
    /// <param name="bookingId">Идентификатор бронирования.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ от провайдера об отмене бронирования.</returns>
    Task<ProviderBookingResponse> CancelBookingAsync(string bookingId, CancellationToken cancellationToken);
}