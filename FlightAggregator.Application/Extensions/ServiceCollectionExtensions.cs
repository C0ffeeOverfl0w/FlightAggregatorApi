namespace FlightAggregator.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR и pipeline behaviors
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Все валидаторы из сборки
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        // AutoMapper-профили
        services.AddAutoMapper(typeof(FlightProfile));
        services.AddAutoMapper(typeof(ResponseToDomainProfile));

        // Доменные сервисы
        services.AddScoped<BookingDomainService>();

        // Сервисы фильтрации и сортировки
        services.AddScoped<IFlightFilterService, FlightFilterService>();
        services.AddScoped<IFlightSortService, FlightSortService>();

        // Отдельные фильтры
        services.AddScoped<IFlightFilter, OriginFilter>();
        services.AddScoped<IFlightFilter, DestinationFilter>();
        services.AddScoped<IFlightFilter, DateFilter>();

        return services;
    }
}