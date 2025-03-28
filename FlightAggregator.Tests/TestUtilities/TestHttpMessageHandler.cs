namespace FlightAggregator.Tests.TestUtilities;

public class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<string> _responseFactory;
    public int CallCount { get; private set; }

    public TestHttpMessageHandler(Func<string> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CallCount++;
        var json = _responseFactory();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}