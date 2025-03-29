namespace FlightAggregator.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining<SearchFlightsQueryValidator>();

        services.AddAutoMapper(typeof(FlightProfile));
        services.AddAutoMapper(typeof(ResponseToDomainProfile));

        services.AddScoped<BookingDomainService>();

        // Регистрация фильтров
        services.AddScoped<IFlightFilterService, FlightFilterService>();
        services.AddScoped<IFlightFilter, OriginFilter>();
        services.AddScoped<IFlightFilter, DestinationFilter>();
        services.AddScoped<IFlightFilter, DateFilter>();

        services.AddScoped<IFlightSortService, FlightSortService>();

        return services;
    }
}