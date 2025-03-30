using FlightAggregator.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging();

builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddSwaggerDocumentation();

builder.Services.AddCorsPolicy();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Aggregator API v1");
    });
}

app.UseAuthentication();

app.UseAuthorization();

app.UseCorsPolicy();

app.UseHttpsRedirection();

app.MapControllers();

try
{
    Log.Information("Запуск приложения...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение завершилось с ошибкой при запуске");
    throw;
}
finally
{
    Log.CloseAndFlush();
}