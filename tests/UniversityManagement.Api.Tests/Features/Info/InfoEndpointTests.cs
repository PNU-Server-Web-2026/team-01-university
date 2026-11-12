using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UniversityManagement.Api.Features.Info;
using Xunit;

namespace UniversityManagement.Api.Tests.Features.Info;

public sealed class InfoEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public InfoEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetInfo_ReturnsOk_WithValidSystemDetails()
    {
        var response = await _client.GetAsync("/api/info");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<InfoResponse>();
        Assert.NotNull(content);
        Assert.Equal("University Management API", content.Name);
        Assert.False(string.IsNullOrWhiteSpace(content.Version));
        Assert.False(string.IsNullOrWhiteSpace(content.Environment));
        Assert.True(content.ServerTimeUtc <= DateTimeOffset.UtcNow);
    }
}