using System.Net;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace UniversityManagement.Api.Tests.Features.OpenApi;

public class OpenApiEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public OpenApiEndpointsTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task OpenApi_InDevelopment_ReturnsOkAndMetadata()
    {
        var client = _factory.WithWebHostBuilder(b => b.UseEnvironment("Development")).CreateClient();

        var scalarResponse = await client.GetAsync("/scalar/v1"); 
        var openApiResponse = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, scalarResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, openApiResponse.StatusCode);

        var json = await openApiResponse.Content.ReadAsStringAsync();

        Assert.Contains("University Management API", json);

        Assert.Contains("Ping", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task OpenApi_OutsideDevelopment_ReturnsNotFound()
    {
        var client = _factory.WithWebHostBuilder(b => b.UseEnvironment("Production")).CreateClient();

        var scalarResponse = await client.GetAsync("/scalar/v1");
        var openApiResponse = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.NotFound, scalarResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, openApiResponse.StatusCode);
    }
}
