using HotChocolate.Execution;

namespace Demo.Accounts.Data;

public class AccountContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public static async Task SeedDataAsync(
        IRequestExecutor executor, 
        CancellationToken cancellationToken = default)
    {
        var services = executor.Schema.Services.GetRootServiceProvider();

        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AccountContext>();
        
        if (await context.Database.EnsureCreatedAsync(cancellationToken))
        {
            await context.Users.AddRangeAsync(
                new User
                {
                    Name = "Ada Lovelace",
                    Birthdate = new DateTime(1815, 12, 10),
                    Username = "@ada"
                },
                new User
                {
                    Name = "Alan Turing",
                    Birthdate = new DateTime(1912, 06, 23),
                    Username = "@alan"
                });
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}