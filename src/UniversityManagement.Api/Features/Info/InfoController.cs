using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Api.Features.Info;

[ApiController]
[Route("api/[controller]")]
public sealed class InfoController(IHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<InfoResponse>(StatusCodes.Status200OK)]
    public IActionResult GetInfo()
    {
        var assembly = typeof(Program).Assembly;
                
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                      ?? assembly.GetName().Version?.ToString()
                      ?? "1.0.0";
        
        var cleanVersion = version.Split('+')[0];

        var response = new InfoResponse(
            Name: "University Management API",
            Version: cleanVersion,
            Environment: environment.EnvironmentName,
            ServerTimeUtc: DateTimeOffset.UtcNow
        );

        return Ok(response);
    }
}