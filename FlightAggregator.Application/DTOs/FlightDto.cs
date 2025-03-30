namespace FlightAggregator.Application.DTOs;
/// <summary>
/// DTO объект для представления информации о рейсе.
/// </summary>
/// <param name="FlightNumber">Номер рейса</param>
/// <param name="DepartureTime">Время вылета</param>
/// <param name="ArrivalTime">Время прилета</param>
/// <param name="Origin">Место вылета</param>
/// <param name="Destination">Место прилета</param>
/// <param name="DurationMinutes">Время перелета</param>
/// <param name="StopDetails">Пересадки</param>
/// <param name="Airline">Авиакомпания</param>
/// <param name="Price">Цена</param>
/// <param name="Stops">Кол-во остановок</param>
public record FlightDto(string FlightNumber,
                        DateTime DepartureTime,
                        DateTime ArrivalTime,
                        string Origin,
                        string Destination,
                        int DurationMinutes,
                        List<StopDetailDataDto> StopDetails,
                        string Airline,
                        decimal Price,
                        int Stops);