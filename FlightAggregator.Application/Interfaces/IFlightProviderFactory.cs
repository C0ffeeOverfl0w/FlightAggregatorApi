namespace FlightAggregator.Application.Interfaces;

public enum FlightProviderType
{
    ProviderA,
    ProviderB
}

/// <summary>
/// Фабрика для получения экземпляра IFlightProvider в зависимости от типа провайдера.
/// </summary>
public interface IFlightProviderFactory
{
    IFlightProvider GetFlightProvider(FlightProviderType providerType);
}