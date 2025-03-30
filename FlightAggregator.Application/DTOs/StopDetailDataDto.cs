namespace FlightAggregator.Application.DTOs;
/// <summary>
/// DTO для данных о пересадке.
/// </summary>
/// <param name="Id">Идентификатор</param>
/// <param name="Airport">Аэропорт</param>
/// <param name="DurationMinutes">Время пересадки</param>
public record StopDetailDataDto(
        string Id,
        string Airport,
        int DurationMinutes
    );