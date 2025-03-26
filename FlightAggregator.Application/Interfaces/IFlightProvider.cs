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
        DateTime? departureDate,
        string? airline,
        decimal? maxPrice,
        int? maxStops,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}