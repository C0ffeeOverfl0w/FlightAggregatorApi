namespace FlightAggregator.Infrastructure.Providers;

/// <inheritdoc/>
internal class FlightProviderFactory(
    IEnumerable<IFlightProvider> providers,
    ILoggerFactory loggerFactory) : IFlightProviderFactory
{
    private readonly IEnumerable<IFlightProvider> _providers = providers;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    /// <inheritdoc/>
    public IFlightProvider GetPrоvider(string providerName)
    {
        var provider = _providers.FirstOrDefault(s =>
        {
            return s switch
            {
                FakeFlightProviderA _ => providerName.Equals("ProviderA", StringComparison.OrdinalIgnoreCase),
                FakeFlightProviderB _ => providerName.Equals("ProviderB", StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        });

        if (provider == null)
        {
            var logger = _loggerFactory.CreateLogger<FlightProviderFactory>();
            logger.LogWarning("Провайдер {ProviderName} не найден", providerName);
            throw new ArgumentException($"Стратегия для провайдера '{providerName}' не найдена.");
        }

        return provider;
    }

    /// <inheritdoc/>
    public IFlightProvider GetAllProviders()
    {
        var logger = _loggerFactory.CreateLogger<CombinedFlightProvider>();
        return new CombinedFlightProvider(_providers, logger);
    }
}