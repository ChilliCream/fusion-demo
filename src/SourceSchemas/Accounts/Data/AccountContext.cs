namespace Demo.Accounts.Data;

public class AccountContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public static async Task SeedDataAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AccountContext>();
        
        await context.Database.MigrateAsync(cancellationToken);
    }
}
