var builder = WebApplication.CreateBuilder(args);

// Регистрируем сервисы приложения
builder.Services.AddApplicationServices();

// Регистрируем контроллеры
builder.Services.AddControllers();

// Регистрируем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Flight Aggregator API",
        Version = "v1",
        Description = "API для поиска и бронирования авиарейсов."
    });
});

var app = builder.Build();

// Глобальная обработка ошибок (опционально)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

if (app.Environment.IsDevelopment())
{
    // Используем Swagger и Swagger UI в режиме разработки
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Aggregator API v1");
    });
}

app.UseHttpsRedirection();

// Маппинг контроллеров
app.MapControllers();

app.Run();