namespace UniversityManagement.Api.Features.Info;

public sealed record InfoResponse(
    string Name,
    string Version,
    string Environment,
    DateTimeOffset ServerTimeUtc
);