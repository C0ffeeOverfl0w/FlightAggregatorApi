namespace FlightAggregator.Infrastructure.Providers;

/// <inheritdoc/>
internal class FlightProviderFactory(
    IEnumerable<IFlightProvider> providers,
    ILogger<FlightProviderFactory> logger) : IFlightProviderFactory
{
    private readonly IEnumerable<IFlightProvider> _providers = providers;
    private readonly ILogger<FlightProviderFactory> _logger = logger;

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
            throw new ArgumentException($"Стратегия для провайдера '{providerName}' не найдена.");

        return provider;
    }

    /// <inheritdoc/>
    public IFlightProvider GetAllProviders() => new CombinedFlightProvider(_providers, _logger);
}