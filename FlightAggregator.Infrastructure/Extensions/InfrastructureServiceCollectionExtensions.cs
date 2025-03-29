namespace FlightAggregator.Infrastructure.Extensions;

/// <summary>
/// Класс расширений для регистрации инфраструктурных сервисов.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Добавляет инфраструктурные сервисы в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если базовый URL для провайдеров не настроен.</exception>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMemoryCache();
        var baseUrlA = configuration["FlightProviders:ProviderA:BaseUrl"];
        var baseUrlB = configuration["FlightProviders:ProviderB:BaseUrl"];

        if (string.IsNullOrEmpty(baseUrlA))
        {
            throw new ArgumentNullException(nameof(baseUrlA), "Base URL for Provider A is not configured.");
        }
        if (string.IsNullOrEmpty(baseUrlB))
        {
            throw new ArgumentNullException(nameof(baseUrlB), "Base URL for Provider B is not configured.");
        }

        // Регистрация HTTP-клиентов с именованными конфигурациями
        services.AddHttpClient("ProviderA", client =>
        {
            client.BaseAddress = new Uri(baseUrlA);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        services.AddHttpClient("ProviderB", client =>
        {
            client.BaseAddress = new Uri(baseUrlB);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Регистрация провайдеров
        services.AddScoped(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var mapper = sp.GetRequiredService<IMapper>();
            var client = httpClientFactory.CreateClient("ProviderA");

            return new FakeFlightProviderA(client, mapper);
        });
        services.AddScoped(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var mapper = sp.GetRequiredService<IMapper>();
            var client = httpClientFactory.CreateClient("ProviderB");
            return new FakeFlightProviderB(client, mapper);
        });

        services.AddScoped(sp =>
        {
            var strategies = new List<IFlightProvider>
        {
            sp.GetRequiredService<FakeFlightProviderA>(),
            sp.GetRequiredService<FakeFlightProviderB>()
        };
            var logger = sp.GetRequiredService<ILogger<CombinedFlightProvider>>();

            return new CombinedFlightProvider(strategies, logger);
        });

        // Регистрация фабрики провайдеров
        services.AddScoped<IFlightProviderFactory, FlightProviderFactory>();

        // Регистрация кэширующего декоратора
        services.AddScoped<IFlightProvider>(sp =>
        {
            var composite = sp.GetRequiredService<CombinedFlightProvider>();
            var cache = sp.GetRequiredService<IMemoryCache>();
            return new CachingFlightProvider(composite, cache);
        });

        services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();

        return services;
    }

    /// <summary>
    /// Создает политику повторных попыток (retry) - 3 попытки с экспоненциальной задержкой.
    /// </summary>
    /// <returns>Политика повторных попыток.</returns>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    /// <summary>
    /// Создает политику circuit breaker - прерываем цепочку при 5 последовательных ошибках на 30 секунд.
    /// </summary>
    /// <returns>Политика circuit breaker.</returns>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}