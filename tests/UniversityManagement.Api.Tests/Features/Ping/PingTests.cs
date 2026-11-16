using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;
using UniversityManagement.Api.Data;
using Xunit;
using System.Collections.Generic;

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
[Fact]
public async Task ResetDatabase_DoesNotDeleteEfMigrationsHistory()
{
    using var scope = Factory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    const string probeMigrationId = "20260101000000_RespawnIgnoreCheck";

    await dbContext.Database.ExecuteSqlRawAsync(
        """INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES ({0}, {1});""",
        probeMigrationId,
        "10.0.0");

    try
    {
        var appliedBefore = await GetMigrationHistoryAsync(dbContext);

        await ResetDatabaseAsync();

        var appliedAfter = await GetMigrationHistoryAsync(dbContext);

        Assert.Equal(appliedBefore, appliedAfter);
        Assert.Contains(probeMigrationId, appliedAfter);
    }
    finally
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            """DELETE FROM "__EFMigrationsHistory" WHERE "MigrationId" = {0};""",
            probeMigrationId);
    }
}

private static async Task<List<string>> GetMigrationHistoryAsync(
    AppDbContext dbContext)
{
    return await dbContext.Database
        .SqlQueryRaw<string>(
            """SELECT "MigrationId" AS "Value" FROM "__EFMigrationsHistory" ORDER BY "MigrationId";""")
        .ToListAsync();
}
}