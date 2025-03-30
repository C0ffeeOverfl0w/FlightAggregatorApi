namespace FlightAggregator.Application.DTOs;
/// <summary>
/// Запрос на вход в систему.
/// </summary>
/// <param name="Email">Адрес почты</param>
public record LoginRequest(string Email);