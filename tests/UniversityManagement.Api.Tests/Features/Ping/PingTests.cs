using System.Threading.Tasks;
using Xunit;

namespace UniversityManagement.Api.Tests.Features.Ping;

public class PingTests : BaseIntegrationTest
{
    public PingTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Ping_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/ping");
        response.EnsureSuccessStatusCode();
    }
}