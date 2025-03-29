namespace FlightAggregator.Domain.Entities;

public record Airline(string Name)
{
    public string Name { get; } = string.IsNullOrWhiteSpace(Name)
        ? throw new ArgumentException("Название авиакомпании не может быть пустым.", nameof(Name))
        : Name;
}