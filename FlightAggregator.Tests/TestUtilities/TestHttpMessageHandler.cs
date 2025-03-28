public class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<string> _responseFactory;
    private int _callCount = 0;

    public TestHttpMessageHandler(Func<string> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _callCount++;

        // На первых двух вызовах имитируем InternalServerError (500)
        if (_callCount < 3)
        {
            var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Transient error")
            };
            return Task.FromResult(errorResponse);
        }

        // На третьем вызове возвращаем успешный ответ
        var json = _responseFactory(); // Используем то, что передали в конструктор
        var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(successResponse);
    }

    public int CallCount => _callCount;
}