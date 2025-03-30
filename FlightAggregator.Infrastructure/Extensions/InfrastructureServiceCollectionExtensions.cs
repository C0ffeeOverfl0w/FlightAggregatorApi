namespace FlightAggregator.Infrastructure.Extensions;

/// <summary>
/// Класс расширений для добавления инфраструктурных сервисов в коллекцию сервисов.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    private const string DefaultConnection = "DefaultConnection";
    private const string ProviderAConfigPath = "FlightProviders:ProviderA";
    private const string ProviderBConfigPath = "FlightProviders:ProviderB";

    /// <summary>
    /// Добавляет инфраструктурные сервисы в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddCaching()
            .AddHttpClients(configuration)
            .AddFlightProviders()
            .AddRepositories()
            .AddAuthentication();

        return services;
    }

    /// <summary>
    /// Добавляет конфигурацию базы данных в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DefaultConnection);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(DefaultConnection, "Строка подключения к базе данных не настроена.");
        }

        return services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    /// <summary>
    /// Добавляет кэширование в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        return services.AddMemoryCache();
    }

    /// <summary>
    /// Добавляет HTTP-клиенты в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    private static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var (baseUrlA, baseUrlB) = GetProviderUrls(configuration);

        services
            .AddHttpClient<FakeFlightProviderA>("ProviderA", client => client.BaseAddress = new Uri(baseUrlA))
            .AddPolicies();

        services
            .AddHttpClient<FakeFlightProviderB>("ProviderB", client => client.BaseAddress = new Uri(baseUrlB))
            .AddPolicies();

        return services;
    }

    /// <summary>
    /// Получает URL-адреса провайдеров из конфигурации.
    /// </summary>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Кортеж с URL-адресами провайдеров.</returns>
    private static (string baseUrlA, string baseUrlB) GetProviderUrls(IConfiguration configuration)
    {
        var baseUrlA = configuration[$"{ProviderAConfigPath}:BaseUrl"];
        var baseUrlB = configuration[$"{ProviderBConfigPath}:BaseUrl"];

        if (string.IsNullOrEmpty(baseUrlA))
            throw new ArgumentNullException(nameof(baseUrlA), "Базовый URL для провайдера рейсов A не настроен.");

        if (string.IsNullOrEmpty(baseUrlB))
            throw new ArgumentNullException(nameof(baseUrlB), "Базовый URL для провайдера рейсов B не настроен.");

        return (baseUrlA, baseUrlB);
    }

    /// <summary>
    /// Добавляет политики для HTTP-клиентов.
    /// </summary>
    /// <param name="builder">Построитель HTTP-клиентов.</param>
    /// <returns>Обновленный построитель HTTP-клиентов.</returns>
    private static IHttpClientBuilder AddPolicies(this IHttpClientBuilder builder)
    {
        return builder
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());
    }

    /// <summary>
    /// Добавляет провайдеров перелетов в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    private static IServiceCollection AddFlightProviders(this IServiceCollection services)
    {
        services
            .AddScoped<FakeFlightProviderA>(sp => CreateProvider<FakeFlightProviderA>(sp, "ProviderA"))
            .AddScoped<FakeFlightProviderB>(sp => CreateProvider<FakeFlightProviderB>(sp, "ProviderB"))
            .AddScoped<CombinedFlightProvider>(sp => new CombinedFlightProvider(
                new List<IFlightProvider>
                {
                        sp.GetRequiredService<FakeFlightProviderA>(),
                        sp.GetRequiredService<FakeFlightProviderB>()
                },
                sp.GetRequiredService<ILogger<CombinedFlightProvider>>()))
            .AddScoped<IFlightProviderFactory, FlightProviderFactory>()
            .AddScoped<IFlightProvider>(sp => new CachingFlightProvider(
                sp.GetRequiredService<CombinedFlightProvider>(),
                sp.GetRequiredService<IMemoryCache>()));

        return services;
    }

    /// <summary>
    /// Создает экземпляр провайдера полетов.
    /// </summary>
    /// <typeparam name="T">Тип провайдера полетов.</typeparam>
    /// <param name="sp">Провайдер сервисов.</param>
    /// <param name="httpClientName">Имя HTTP-клиента.</param>
    /// <returns>Экземпляр провайдера полетов.</returns>
    private static T CreateProvider<T>(IServiceProvider sp, string httpClientName) where T : IFlightProvider
    {
        var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient(httpClientName);
        var mapper = sp.GetRequiredService<IMapper>();

        var loggerType = typeof(ILogger<>).MakeGenericType(typeof(T));
        var logger = sp.GetRequiredService(loggerType);

        return (T)Activator.CreateInstance(typeof(T), httpClient, mapper, logger)!;
    }

    /// <summary>
    /// Добавляет репозитории в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
    }

    /// <summary>
    /// Добавляет аутентификацию в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    private static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        return services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    }

    /// <summary>
    /// Получает политику повторных попыток.
    /// </summary>
    /// <returns>Политика повторных попыток.</returns>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    /// <summary>
    /// Получает политику автоматического прерывания.
    /// </summary>
    /// <returns>Политика автоматического прерывания.</returns>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}