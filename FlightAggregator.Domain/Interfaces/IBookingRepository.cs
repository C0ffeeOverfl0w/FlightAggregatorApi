namespace FlightAggregator.Domain.Interfaces;

/// <summary>
/// Интерфейс для работы с репозиторием бронирований.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Получить бронирование по идентификатору.
    /// </summary>
    /// <param name="bookingId">Идентификатор бронирования.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Бронь.</returns>
    Task<Booking> GetByIdAsync(string bookingId, CancellationToken cancellationToken);

    /// <summary>
    /// Добавить новое бронирование.
    /// </summary>
    /// <param name="booking">Бронирование для добавления.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task AddAsync(Booking booking, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить существующее бронирование.
    /// </summary>
    /// <param name="booking">Бронирование для обновления.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task UpdateAsync(Booking booking, CancellationToken cancellationToken);
}