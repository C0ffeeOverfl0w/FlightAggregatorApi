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

        // На первых двух вызовах возвращаем 500 (имитация транзиентной ошибки)
        if (_callCount < 3)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Transient error")
            });
        }

        // На третьем вызове — успешный ответ
        var json = _responseFactory();
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }

    public int CallCount => _callCount;
}