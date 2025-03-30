namespace FlightAggregator.Api.Extensions;

/// <summary>
/// Расширения для конфигурации CORS.
/// </summary>
public static class CorsServiceExtensions
{
    private const string AllowAllPolicy = "AllowAll";

    /// <summary>
    /// Добавляет политику CORS с полным разрешением.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AllowAllPolicy, policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }

    /// <summary>
    /// Подключает CORS с политикой AllowAll.
    /// </summary>
    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        return app.UseCors(AllowAllPolicy);
    }
}