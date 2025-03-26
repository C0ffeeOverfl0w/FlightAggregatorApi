namespace FlightAggregator.Application.Extensions;

/// <summary>
/// Класс расширений для работы с предикатами.
/// </summary>
public static class PredicateExtensions
{
    /// <summary>
    /// Возвращает новый предикат, который является логическим И двух предикатов.
    /// </summary>
    /// <typeparam name="T">Тип входного параметра предикатов.</typeparam>
    /// <param name="first">Первый предикат.</param>
    /// <param name="second">Второй предикат.</param>
    /// <returns>Новый предикат, который возвращает true, если оба предиката возвращают true.</returns>
    public static Func<T, bool> And<T>(this Func<T, bool> first, Func<T, bool> second)
        => x => first(x) && second(x);
}