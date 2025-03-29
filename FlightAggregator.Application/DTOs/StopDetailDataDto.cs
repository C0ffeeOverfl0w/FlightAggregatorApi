namespace FlightAggregator.Application.DTOs;

public record StopDetailDataDto(
        string Id,
        string Airport,
        int DurationMinutes
    );