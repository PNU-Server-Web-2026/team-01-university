using System.Net;
using System.Net.Http.Json;

namespace UniversityManagement.Api.Tests.Features.Health;

public class HealthTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk_WhenProcessIsAlive()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(body);
        Assert.Equal("Healthy", body.status);
    }

    [Fact]
    public async Task Health_Ready_ReturnsServiceUnavailable_WhenDatabaseIsNotAvailable()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(body);
        Assert.Equal("Unhealthy", body.status);
    }

    private sealed record HealthResponse(string status, CheckEntry[] checks);
    private sealed record CheckEntry(string name, string status, string duration);
}
