using Health.Domain.Contracts;
using Health.Persistence.Data.DbContexts;
using Health.Persistence.IdentityData.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Health.Web.Extensions
{
    public static class WebApplicationRegistration
    {

        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContextService = scope.ServiceProvider.GetRequiredService<HealthCareDbContext>();
            var pendingMigration = await dbContextService.Database.GetPendingMigrationsAsync();
            if (pendingMigration.Any())
                await dbContextService.Database.MigrateAsync();

            return app;
        }
        public static async Task<WebApplication> MigrateIdentityDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dbContextService = scope.ServiceProvider.GetRequiredService<HealthCareIdentityDbContext>();
            var pendingMigration = await dbContextService.Database.GetPendingMigrationsAsync();
            if (pendingMigration.Any())
                await dbContextService.Database.MigrateAsync();

            return app;
        }

        public static async Task<WebApplication> SeedIdentityDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var dataInitializerService = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>("Identity");
            await dataInitializerService.InitializeAsync();
            return app;
        }

    }
}
