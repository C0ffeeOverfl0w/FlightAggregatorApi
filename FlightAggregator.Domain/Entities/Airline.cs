namespace FlightAggregator.Domain.Entities;

public record Airline
{
    public int? Id { get; init; }
    public string? Name { get; init; }

    // Конструктор для EF Core
    public Airline() { }

    // Бизнес-конструктор
    public Airline(string name) : this(null, name) { }

    public Airline(int? id, string name)
    {
        Id = id;
        Name = !string.IsNullOrWhiteSpace(name)
            ? name
            : throw new ArgumentException("Название авиакомпании не может быть пустым");
    }
}