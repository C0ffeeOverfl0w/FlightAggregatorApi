namespace FlightAggregator.Tests.Behaviors;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<string, string>>> _loggerMock;
    private readonly LoggingBehavior<string, string> _behavior;
    private readonly Mock<RequestHandlerDelegate<string>> _nextMock;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<string, string>>>();
        _behavior = new LoggingBehavior<string, string>(_loggerMock.Object);
        _nextMock = new Mock<RequestHandlerDelegate<string>>();
    }

    [Fact]
    public async Task Handle_WhenRequestSucceeds_LogsSuccess()
    {
        // Arrange
        const string request = "TestRequest";
        const string response = "TestResponse";
        _nextMock.Setup(next => next()).ReturnsAsync(response);

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(response, result);
        _loggerMock.VerifyLog(LogLevel.Information,
            Times.Once(),
            "Обработка запроса String: {@Request}",
            It.Is<object[]>(args => args[0].ToString() == request));
        _loggerMock.VerifyLog(LogLevel.Information,
            Times.Once(),
            "Запрос String успешно обработан за {ElapsedMs} мс. Ответ: {@Response}",
            It.IsAny<object[]>());
    }

    [Fact]
    public async Task Handle_WhenRequestFails_LogsError()
    {
        // Arrange
        const string request = "TestRequest";
        var exception = new Exception("Test error");
        _nextMock.Setup(next => next()).ThrowsAsync(exception);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _behavior.Handle(request, _nextMock.Object, CancellationToken.None));
        _loggerMock.VerifyLog(LogLevel.Information,
            Times.Once(),
            "Обработка запроса String: {@Request}",
            It.Is<object[]>(args => args[0].ToString() == request));
        _loggerMock.VerifyLog(LogLevel.Error,
            Times.Once(),
            "Ошибка при обработке запроса String за {ElapsedMs} мс: {@Request}",
            It.IsAny<object[]>());
    }
}

// Вспомогательный метод для упрощения проверки логов
public static class LoggerMockExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times, string messageTemplate, object[] args)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(messageTemplate)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            times);
    }
}