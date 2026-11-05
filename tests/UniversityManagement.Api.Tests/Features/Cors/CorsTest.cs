using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace UniversityManagement.Api.Tests.Features.Cors;

public class CorsTests : BaseIntegrationTest
{
    private readonly HttpClient _developmentClient;

    public CorsTests(ApiFactory factory) : base(factory)
    {
        _developmentClient = factory.WithWebHostBuilder(b => b.UseEnvironment("Development")).CreateClient();
    }

    [Theory]
    [InlineData("http://localhost:5173")]
    [InlineData("http://localhost:3000")]
    public async Task PreflightRequest_FromAllowedOrigin_ReturnsAllowOriginHeader(string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/ping");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "POST");

        var response = await _developmentClient.SendAsync(request);

        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.Equal(origin, response.Headers.GetValues("Access-Control-Allow-Origin").First());
    }
}