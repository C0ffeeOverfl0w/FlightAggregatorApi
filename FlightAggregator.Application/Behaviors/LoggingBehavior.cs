namespace FlightAggregator.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString(); // Уникальный ID для отслеживания запроса

        _logger.LogInformation("Начало обработки запроса {RequestName} с ID {RequestId}: {@Request}",
            requestName, requestId, request);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation("Запрос {RequestName} с ID {RequestId} успешно обработан за {ElapsedMs} мс. Ответ: {@Response}",
                requestName, requestId, stopwatch.ElapsedMilliseconds, response);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Ошибка при обработке запроса {RequestName} с ID {RequestId} за {ElapsedMs} мс: {@Request}",
                requestName, requestId, stopwatch.ElapsedMilliseconds, request);
            throw;
        }
    }
}