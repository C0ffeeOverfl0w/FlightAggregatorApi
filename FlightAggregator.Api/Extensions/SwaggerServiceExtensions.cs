namespace FlightAggregator.Api.Extensions;

/// <summary>
/// Класс расширений для настройки документации Swagger.
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Добавляет документацию Swagger в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Flight Aggregator API",
                Version = "v1",
                Description = "API для поиска и бронирования авиарейсов.",
                Contact = new OpenApiContact
                {
                    Name = "Сергей Королёв",
                    Email = "gospodinkorolev@gmail.com"
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            // Схема безопасности JWT Bearer
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Введите JWT токен в формате: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });

        return services;
    }

    /// <summary>
    /// Использует документацию Swagger в приложении.
    /// </summary>
    /// <param name="app">Построитель приложения.</param>
    public static void UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Aggregator API v1");
        });
    }
}