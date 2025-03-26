namespace FlightAggregator.Domain.Entities;

public class Airline(Guid id, string name)
{
    public Guid Id { get; } = id;

    public string Name { get; } = string.IsNullOrWhiteSpace(name)
        ? throw new ArgumentException("Название авиакомпании не может быть пустым.", nameof(name))
        : name;
}