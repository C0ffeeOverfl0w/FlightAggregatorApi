namespace FlightAggregator.Domain.ValueObjects;

/// <summary>
/// Представляет денежную сумму с количеством и валютой.
/// </summary>
public sealed record Money(decimal Amount, string Currency)
{
    /// <summary>
    /// Инициализирует новый экземпляр записи <see cref="Money"/> с указанной суммой и валютой по умолчанию USD.
    /// </summary>
    /// <param name="amount">Сумма денег.</param>
    public Money(decimal amount) : this(amount, "RUB") { }

    /// <summary>
    /// Добавляет указанную сумму <see cref="Money"/> к текущему экземпляру.
    /// </summary>
    /// <param name="other">Сумма <see cref="Money"/> для добавления.</param>
    /// <returns>Новый экземпляр <see cref="Money"/> с суммированной суммой.</returns>
    /// <exception cref="InvalidOperationException">Выбрасывается, если валюты двух экземпляров <see cref="Money"/> не совпадают.</exception>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Нельзя складывать суммы в разных валютах.");
        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// Вычитает указанную сумму <see cref="Money"/> из текущего экземпляра.
    /// </summary>
    /// <param name="other">Сумма <see cref="Money"/> для вычитания.</param>
    /// <returns>Новый экземпляр <see cref="Money"/> с вычтенной суммой.</returns>
    /// <exception cref="InvalidOperationException">Выбрасывается, если валюты двух экземпляров <see cref="Money"/> не совпадают.</exception>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Нельзя вычитать суммы в разных валютах.");
        return new Money(Amount - other.Amount, Currency);
    }
}