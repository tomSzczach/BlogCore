using BlogCore.DAL.Data;
using BlogCore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace BlogCore.DAL.Tests
{
    [TestClass]
    public abstract class IntegrationTestBase
    {
        protected static readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("blog_test_db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        protected BlogContext _context = null!;
        protected BlogRepository _repository = null!;
        private Respawner _respawner = null!;

        [AssemblyInitialize]
        public static async Task AssemblyInit(TestContext context)
        {
            // Uruchomienie kontenera raz dla wszystkich testów w projekcie
            await _dbContainer.StartAsync();
        }

        [TestInitialize]
        public async Task Setup()
        {
            var connectionString = _dbContainer.GetConnectionString();

            // 1. Konfiguracja EF Core
            var options = new DbContextOptionsBuilder<BlogContext>()
                .UseNpgsql(connectionString)
                .Options;

            _context = new BlogContext(options);

            await _context.Database.EnsureCreatedAsync(); // Tworzy schemat
            _repository = new BlogRepository(_context);

            // 2. Inicjalizacja Respawn przy użyciu aktywnego połączenia
            await using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                    TablesToIgnore = [new Table("__EFMigrationsHistory")]
                });
            }

            // 3. Pierwszy reset bazy
            await ResetDatabaseAsync();
        }

        protected async Task ResetDatabaseAsync()
        {
            if (_respawner != null)
            {
                var connectionString = _dbContainer.GetConnectionString();

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await _respawner.ResetAsync(connection);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Zwalnianie zasobów po każdym teście
            _context.Dispose();
        }

        [AssemblyCleanup]
        public static async Task AssemblyCleanup()
        {
            // Zatrzymanie kontenera po zakończeniu wszystkich testów
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }
    }
}
