namespace FlightAggregator.Application.DTOs;
public record FlightDto(string FlightNumber,
                        DateTime DepartureDate,
                        string Airline,
                        decimal Price,
                        int Stops);