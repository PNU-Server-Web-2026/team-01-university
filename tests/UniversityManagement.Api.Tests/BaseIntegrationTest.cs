using Npgsql;
using Respawn;
using Respawn.Graph;
using System.Data.Common;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace UniversityManagement.Api.Tests;

[Collection("Api collection")]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected readonly ApiFactory Factory;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(ApiFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(Factory.GetConnectionString());
        await connection.OpenAsync();

        if (!await HasUserTablesAsync(connection))
        {
            return;
        }

        var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = [new Table("__EFMigrationsHistory")]
        });

        await respawner.ResetAsync(connection);
    }

    private static async Task<bool> HasUserTablesAsync(DbConnection connection)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND table_type = 'BASE TABLE'
                  AND table_name <> '__EFMigrationsHistory'
            );
            """;

        var result = await command.ExecuteScalarAsync();
        return result is true;
    }
}