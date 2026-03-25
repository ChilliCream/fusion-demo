namespace Demo.Reviews.Data;

public class ReviewContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<User> Users => Set<User>();
    
    public static async Task SeedDataAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ReviewContext>();
        
        await context.Database.MigrateAsync(cancellationToken);
    }
}
