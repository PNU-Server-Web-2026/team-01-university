using Npgsql;
using Respawn;
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
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    protected BaseIntegrationTest(ApiFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    // Викликається ПЕРЕД кожним [Fact] або [Theory] тестом
    public async Task InitializeAsync()
    {
        _dbConnection = new NpgsqlConnection(Factory.GetConnectionString());
        await _dbConnection.OpenAsync();

        // Ініціалізуємо Respawner для PostgreSQL
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" }
        });

        // Очищаємо всі таблиці від даних перед запуском тесту
        await _respawner.ResetAsync(_dbConnection);
    }

    // Викликається ПІСЛЯ кожного тесту
    public async Task DisposeAsync()
    {
        if (_dbConnection != null)
        {
            await _dbConnection.CloseAsync();
            await _dbConnection.DisposeAsync();
        }
    }
}