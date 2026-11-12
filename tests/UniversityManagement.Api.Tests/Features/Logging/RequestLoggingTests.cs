using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Serilog.Events;

namespace UniversityManagement.Api.Tests.Features.Logging;

public class RequestLoggingTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RequestCompletion_LogsMethodPathStatusAndDuration()
    {
        factory.LogSink.Clear();

        var response = await _client.GetAsync("/api/ping");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var logEvent = await WaitForRequestLogAsync("/api/ping");

        Assert.Equal(LogEventLevel.Information, logEvent.Level);

        var message = logEvent.RenderMessage();
        Assert.Contains("GET", message);
        Assert.Contains("/api/ping", message);
        Assert.Contains("200", message);
        Assert.Matches(@"\d+\.\d{4} ms", message);

        Assert.Equal("GET", GetScalar(logEvent, "RequestMethod"));
        Assert.Equal("/api/ping", GetScalar(logEvent, "RequestPath"));
        Assert.Equal(200, GetScalar(logEvent, "StatusCode"));
        Assert.IsType<ScalarValue>(logEvent.Properties["Elapsed"]);
    }

    [Fact]
    public async Task RequestLogging_DoesNotLogQueryStringSecretsOrAuthHeaders()
    {
        factory.LogSink.Clear();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/ping?access_token=super-secret-token&user_password=hunter2");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "bearer-secret-value");

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);

        var logEvent = await WaitForRequestLogAsync("/api/ping");

        Assert.Equal("405", GetScalar(logEvent, "StatusCode")?.ToString());
        Assert.Equal("/api/ping", GetScalar(logEvent, "RequestPath"));

        var allOutput = string.Join(
            "\n",
            factory.LogSink.Snapshot().SelectMany(e =>
                new[] { e.RenderMessage() }.Concat(e.Properties.Values.Select(p => p.ToString() ?? string.Empty))));

        Assert.DoesNotContain("super-secret-token", allOutput);
        Assert.DoesNotContain("hunter2", allOutput);
        Assert.DoesNotContain("bearer-secret-value", allOutput);
        Assert.DoesNotContain("access_token", allOutput);
    }

    private async Task<LogEvent> WaitForRequestLogAsync(string path)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (DateTime.UtcNow < deadline)
        {
            var match = factory.LogSink.Snapshot()
                .LastOrDefault(e => e.RenderMessage().Contains("responded") && e.RenderMessage().Contains(path));
            if (match is not null)
            {
                return match;
            }

            await Task.Delay(50);
        }

        throw new TimeoutException($"Не знайдено лог-запис запиту для '{path}'.");
    }

    private static object? GetScalar(LogEvent logEvent, string propertyName)
    {
        Assert.True(logEvent.Properties.TryGetValue(propertyName, out var property), $"Відсутня властивість '{propertyName}'.");
        var scalar = Assert.IsType<ScalarValue>(property);
        return scalar.Value;
    }
}
