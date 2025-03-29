namespace FlightAggregator.Application.DTOs;
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