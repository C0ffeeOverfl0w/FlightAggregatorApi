namespace FlightAggregator.Infrastructure.Providers;

/// <summary>
/// Фабрика для работы с провайдерами авиаперелетов.
/// </summary>
public interface IFlightProviderFactory
{
    /// <summary>
    /// Получает провайдера по имени.
    /// </summary>
    /// <param name="providerName">Имя провайдера.</param>
    /// <returns>Провайдер авиаперелетов.</returns>
    IFlightProvider GetPrоvider(string providerName);

    /// <summary>
    /// Получает всех провайдеров.
    /// </summary>
    /// <returns>Список всех провайдеров авиаперелетов.</returns>
    IFlightProvider GetAllProviders();
}