using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace UniversityManagement.Api.Tests.Features.Cors;

public class CorsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public CorsTests(ApiFactory factory)
    {
        _client = factory.WithWebHostBuilder(b => b.UseEnvironment("Development")).CreateClient();
    }

    
    [Theory]
    [InlineData("http://localhost:5173")]
    [InlineData("http://localhost:3000")]
    public async Task PreflightRequest_FromAllowedOrigin_ReturnsAllowOriginHeader(string origin)
    {
      
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/ping");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "POST");

        
        var response = await _client.SendAsync(request);

        
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"), "Заголовок Access-Control-Allow-Origin відсутній у відповіді");
        
        var allowOriginHeader = response.Headers.GetValues("Access-Control-Allow-Origin").First();
        Assert.Equal(origin, allowOriginHeader);
    }

    [Fact]
    public async Task PreflightRequest_FromDisallowedOrigin_DoesNotReturnCorsHeaders()
    {
       
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/ping");
        request.Headers.Add("Origin", "http://evil-hacker.com");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        
        var response = await _client.SendAsync(request);

        
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"), "Для недозволеного origin сервер не повинен повертати Access-Control-Allow-Origin");
        Assert.False(response.Headers.Contains("Access-Control-Allow-Methods"), "Для недозволеного origin сервер не повинен повертати Access-Control-Allow-Methods");
    }
}